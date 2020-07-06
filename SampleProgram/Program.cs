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
            ConfigurableTouchpadSample();
        }

        static void ConfigurableTouchpadSample()
        {
            //create target and trigger devices
            var touchpadOptions = new TouchpadOptions()
            {
                Multitouch = 14,
                MaxXPos = 1279,
                MaxYPos = 719,
                DefaultClickPressure = 4,
                DefaultClickTicks = 2,
                Path = "/dev/input/event6"
            };
            var touchpadConfiguration = GetMemuTpadConfig();
            var touchpad = new ConfigurableTouchpad(touchpadConfiguration, touchpadOptions, EDevicePurpose.Target);
            var trigger = new GenericTriggerDevice(new DeviceOptions("/dev/input/event2"));

            //add events
            touchpad.Tap(1000, 350, 1, 10);
            touchpad.Swipe(new AbsolutePosition(40, 10), new AbsolutePosition(40, 300), 1, 10);
            touchpad.Swipe(new AbsolutePosition(40, 800), new AbsolutePosition(40, 310), 1, 10);

            //create replay builder
            var replayBuilder = new InputReplayBuilder(20);
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

        public static TouchpadConfiguration GetMemuTpadConfig()
        {
            return new TouchpadConfiguration()
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
                        ETouchpadEventType.Hold, new[]
                        {
                            new ConfigurationEvent(EValueType.EventXPos, EEventType.EvAbs, EEventCode.AbsMtPositionX),
                            new ConfigurationEvent(EValueType.EventYPos, EEventType.EvAbs, EEventCode.AbsMtPositionY),
                            new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 10),
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
                PostUpdate = new[] //syn
                {
                    new ConfigurationEvent(EValueType.Value, EEventType.EvSyn, EEventCode.SynReport, 0),
                },
                PostAbort = new [] //reset all slots
                {
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtSlot, 0),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 0),
                    new ConfigurationEvent(EValueType.UintMax, EEventType.EvAbs, EEventCode.AbsMtTrackingId),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtSlot, 1),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 0),
                    new ConfigurationEvent(EValueType.UintMax, EEventType.EvAbs, EEventCode.AbsMtTrackingId),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtSlot, 2),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 0),
                    new ConfigurationEvent(EValueType.UintMax, EEventType.EvAbs, EEventCode.AbsMtTrackingId),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtSlot, 3),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 0),
                    new ConfigurationEvent(EValueType.UintMax, EEventType.EvAbs, EEventCode.AbsMtTrackingId),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtSlot, 4),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 0),
                    new ConfigurationEvent(EValueType.UintMax, EEventType.EvAbs, EEventCode.AbsMtTrackingId),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtSlot, 5),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 0),
                    new ConfigurationEvent(EValueType.UintMax, EEventType.EvAbs, EEventCode.AbsMtTrackingId),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtSlot, 6),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 0),
                    new ConfigurationEvent(EValueType.UintMax, EEventType.EvAbs, EEventCode.AbsMtTrackingId),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtSlot, 7),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 0),
                    new ConfigurationEvent(EValueType.UintMax, EEventType.EvAbs, EEventCode.AbsMtTrackingId),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtSlot, 8),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 0),
                    new ConfigurationEvent(EValueType.UintMax, EEventType.EvAbs, EEventCode.AbsMtTrackingId),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtSlot, 9),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 0),
                    new ConfigurationEvent(EValueType.UintMax, EEventType.EvAbs, EEventCode.AbsMtTrackingId),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtSlot, 10),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 0),
                    new ConfigurationEvent(EValueType.UintMax, EEventType.EvAbs, EEventCode.AbsMtTrackingId),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtSlot, 11),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 0),
                    new ConfigurationEvent(EValueType.UintMax, EEventType.EvAbs, EEventCode.AbsMtTrackingId),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtSlot, 12),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 0),
                    new ConfigurationEvent(EValueType.UintMax, EEventType.EvAbs, EEventCode.AbsMtTrackingId),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtSlot, 13),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 0),
                    new ConfigurationEvent(EValueType.UintMax, EEventType.EvAbs, EEventCode.AbsMtTrackingId),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtSlot, 14),
                    new ConfigurationEvent(EValueType.Value, EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 0),

                    new ConfigurationEvent(EValueType.Value, EEventType.EvSyn, EEventCode.SynReport, 0),
                }
            };
        }
    }
}
