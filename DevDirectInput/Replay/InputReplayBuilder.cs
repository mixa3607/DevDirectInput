using System;
using System.Collections.Generic;
using System.Linq;
using DevDirectInput.Devices;
using DevDirectInput.Enums;

namespace DevDirectInput.Replay
{
    public class InputReplayBuilder
    {
        public float TickRate { get; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string About { get; set; }

        private readonly List<IInputDevice> _devices = new List<IInputDevice>();

        public InputReplayBuilder(float tickRate)
        {
            TickRate = tickRate;
        }

        public void AddDevice(IInputDevice device)
        {
            _devices.Add(device);
        }

        private enum TimelineType
        {
            Main,
            Abort
        }

        private static List<uint[][]?> BuildTimeline(TimelineType type, IReadOnlyList<IInputDevice> devices)
        {
            var timeline = new List<List<uint[]>?>();
            for (var deviceIdx = 0; deviceIdx < devices.Count; deviceIdx++)
            {
                var device = devices[deviceIdx];
                if (device.Purpose != EDevicePurpose.Target || (!device.SupportAborting && type == TimelineType.Abort))
                {
                    continue;
                }

                var deviceUpdates = type switch
                {
                    TimelineType.Abort => device.BuildPostAbortRawUpdates(),
                    TimelineType.Main => device.BuildRawUpdates(),
                    _ => throw new NotSupportedException($"Time line type \"{type}\" is not supported")
                };

                if (timeline.Count < deviceUpdates.Count)
                    timeline.AddRange(new List<uint[]>?[deviceUpdates.Count - timeline.Count]);

                for (var updateIdx = 0; updateIdx < deviceUpdates.Count; updateIdx++)
                {
                    var deviceUpdate = deviceUpdates[updateIdx];
                    if (deviceUpdate == null)
                        continue;

                    if (timeline[updateIdx] == null)
                        timeline[updateIdx] = new List<uint[]>();
                    var update = timeline[updateIdx];

                    #pragma warning disable CS8602 // list increased to required size
                    update.AddRange(deviceUpdate.Select(deviceEvent => deviceEvent.ToUintArray((uint) deviceIdx)));
                    #pragma warning restore CS8602
                }
            }

            return timeline.Select(x => x?.ToArray()).ToList();
        }

        public InputReplay Build()
        {
            var devicePaths = new List<string>();
            var inputDevices = new List<int>();
            var triggerDevices = new List<int>();

            for (var i = 0; i < _devices.Count; i++)
            {
                var device = _devices[i];
                devicePaths.Add(device.Options.Path);
                switch (device.Purpose)
                {
                    case EDevicePurpose.Target:
                        inputDevices.Add(i);
                        break;
                    case EDevicePurpose.Trigger:
                        triggerDevices.Add(i);
                        break;
                    case EDevicePurpose.None:
                        break;
                    default:
                        throw new NotSupportedException($"Device purpose type \"{device.Purpose}\" not supported");
                }
            }

            var mainTimeline = BuildTimeline(TimelineType.Main, _devices);
            var abortTimeline = BuildTimeline(TimelineType.Abort, _devices);

            return new InputReplay()
            {
                TickRate = TickRate,
                DevicePaths = devicePaths.ToArray(),
                InputDeviceIds = inputDevices.ToArray(),
                TriggerDeviceIds = triggerDevices.ToArray(),
                Updates = mainTimeline,
                PostAbortUpdates = abortTimeline,

                Author = Author,
                Name = Name,
                About = About
            };
        }
    }
}