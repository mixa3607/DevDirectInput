using System;
using System.Collections.Generic;
using System.IO;
using DevDirectInput.Devices;
using DevDirectInput.Devices.Options;
using DevDirectInput.Devices.Touchpads;
using DevDirectInput.Devices.Touchpads.Configurable;
using DevDirectInput.Enums;
using DevDirectInput.Replay;
using Newtonsoft.Json;

namespace SampleProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            PreBuildTouchpadSample();
            ConfigurableTouchpadSample();
        }

        static void ConfigurableTouchpadSample()
        {
            //create target and trigger devices
            var touchpadOptions = new TouchpadOptions()
            {
                Multitouch = 10,
                MaxXPos = 1079,
                MaxYPos = 2339,
                DefaultClickPressure = 4,
                DefaultClickTicks = 2,
                Path = "/dev/input/event6"
            };
            var touchpadConfiguration = new TouchpadConfiguration()
            {
                PreSequence = new[]
                {
                    new ConfigurationEvent(EValueType.EventSlot, EEventType.EvAbs, EEventCode.AbsMtSlot),
                },
                Update = new Dictionary<ETouchpadEventType, ConfigurationEvent[]>()
                {
                    {
                        ETouchpadEventType.Press, new[]
                        {
                            new ConfigurationEvent(EValueType.EventXPos, EEventType.EvAbs, EEventCode.AbsMtPositionX),
                            new ConfigurationEvent(EValueType.EventYPos, EEventType.EvAbs, EEventCode.AbsMtPositionY),
                            new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 10),
                            new ConfigurationEvent(EValueType.EventTrackingId, EEventType.EvAbs,
                                EEventCode.AbsMtTrackingId),
                        }
                    },
                    {
                        ETouchpadEventType.Release, new[]
                        {
                            new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 0),
                            new ConfigurationEvent(EValueType.UintMax, EEventType.EvAbs, EEventCode.AbsMtTrackingId),
                        }
                    }
                },
                PostUpdate = new []
                {
                    new ConfigurationEvent(EValueType.Value, EEventType.EvSyn, EEventCode.SynReport, 0), 
                }
            };
            var touchpad = new ConfigurableTouchpad(touchpadConfiguration, touchpadOptions, EDevicePurpose.Target);
            var trigger = new GenericTriggerDevice(new DeviceOptions("/dev/input/event2"));

            //add events
            touchpad.Tap(450, 350, 0);
            touchpad.Tap(650, 350, 1, 10);

            //create replay builder
            var replayBuilder = new InputReplayBuilder
            {
                StopOnTrigger = true,
                StartOnTrigger = false,
                TickRate = 10
            };
            //add devices
            replayBuilder.AddDevice(touchpad);
            replayBuilder.AddDevice(trigger);
            //build
            var replay = replayBuilder.Build();

            //serialize to json
            var jsonStr = replay.ToJson(Formatting.Indented);

            Console.WriteLine(jsonStr);
            File.WriteAllText("./configurable.json", jsonStr);
        }

        static void PreBuildTouchpadSample()
        {
            //create target and trigger devices
            var touchpadOptions = new TouchpadOptions()
            {
                Multitouch = 10,
                MaxXPos = 1079,
                MaxYPos = 2339,
                DefaultClickPressure = 4,
                DefaultClickTicks = 2,
                Path = "/dev/input/event6"
            };
            var touchpad = new MemuTouchpad(touchpadOptions, EDevicePurpose.Target);
            var trigger = new GenericTriggerDevice(new DeviceOptions("/dev/input/event2"));

            //add events
            touchpad.Tap(450, 350, 0);
            touchpad.Tap(650, 350, 1, 10);

            //create replay builder
            var replayBuilder = new InputReplayBuilder
            {
                StopOnTrigger = true, 
                StartOnTrigger = false, 
                TickRate = 10
            };
            //add devices
            replayBuilder.AddDevice(touchpad);
            replayBuilder.AddDevice(trigger);
            //build
            var replay = replayBuilder.Build();

            //serialize to json
            var jsonStr = replay.ToJson(Formatting.Indented);

            Console.WriteLine(jsonStr);
            File.WriteAllText("./prebuild.json", jsonStr);
        }
    }
}
