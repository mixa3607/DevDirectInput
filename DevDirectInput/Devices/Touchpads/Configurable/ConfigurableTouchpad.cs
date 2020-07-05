using System;
using System.Collections.Generic;
using System.Linq;
using DevDirectInput.Devices.Events;
using DevDirectInput.Devices.Options;
using DevDirectInput.Enums;
using DevDirectInput.Replay;

namespace DevDirectInput.Devices.Touchpads.Configurable
{
    public class ConfigurableTouchpad : ITouchpad
    {
        public EDeviceType Type { get; } = EDeviceType.Touchpad;
        IDeviceOptions IInputDevice.Options => Options;

        public EDevicePurpose Purpose { get; set; }
        public bool SupportAborting { get; } = true;
        public ITouchpadOptions Options { get; }

        private int _trackingId = 100;
        private readonly InputTimeline<TouchpadEvent> _timeline = new InputTimeline<TouchpadEvent>();
        private readonly TouchpadConfiguration _config;

        public ConfigurableTouchpad(TouchpadConfiguration config, ITouchpadOptions options)
        {
            Options = options;
            _config = config;
        }

        public ConfigurableTouchpad(TouchpadConfiguration config, ITouchpadOptions options, EDevicePurpose purpose) :
            this(config, options)
        {
            Purpose = purpose;
        }

        private byte FindFreeSlot(int startTick, int ticks)
        {
            for (byte i = 0; i < Options.Multitouch; i++)
            {
                if (!_timeline.GetEventsLists().Skip(startTick).Take(ticks).Any(x => x.Any(y => y.Slot == i)))
                {
                    return i;
                }
            }

            throw new ArgumentException($"No free slots. All {Options.Multitouch} is used");
        }

        public void Tap(IAbsolutePosition position, int startTick)
            => Tap(position, startTick, Options.DefaultClickTicks);

        public void Tap(IAbsolutePosition position, int startTick, int ticks)
            => Tap(position.X, position.Y, startTick, ticks);

        public void Tap(int xPos, int yPos, int startTick)
            => Tap(xPos, yPos, startTick, Options.DefaultClickTicks);

        public void Tap(int xPos, int yPos, int startTick, int ticks)
        {
            var slot = FindFreeSlot(startTick, ticks);
            if (xPos > Options.MaxXPos)
                throw new ArgumentException($"X position ({xPos}) more then allow ({Options.MaxXPos})");
            if (yPos > Options.MaxYPos)
                throw new ArgumentException($"Y position ({yPos}) more then allow ({Options.MaxYPos})");

            _timeline.Add(startTick, new TouchpadEvent()
            {
                Pressure = Options.DefaultClickPressure,
                Slot = slot,
                EventType = ETouchpadEventType.Press,
                XPos = xPos,
                YPos = yPos,
                TrackingId = _trackingId
            });

            for (int i = startTick + 1; i < startTick + ticks; i++)
            {
                _timeline.Add(i, new TouchpadEvent()
                {
                    Pressure = Options.DefaultClickPressure,
                    Slot = slot,
                    EventType = ETouchpadEventType.Press,
                    XPos = xPos,
                    YPos = yPos,
                    TrackingId = _trackingId
                });
            }

            _timeline.Add(startTick + ticks, new TouchpadEvent()
            {
                Pressure = Options.DefaultClickPressure,
                Slot = slot,
                EventType = ETouchpadEventType.Release,
                TrackingId = _trackingId
            });
            _trackingId++;
        }


        public List<List<RawInputEvent>?> BuildRawUpdates()
        {
            var updates = new List<List<RawInputEvent>?>();
            foreach (var touchpadEvents in _timeline.GetEventsLists())
            {
                if (touchpadEvents == null)
                {
                    updates.Add(null);
                    continue;
                }

                var update = new List<RawInputEvent>();
                foreach (var touchpadEvent in touchpadEvents)
                {
                    update.AddRange(_config.PreSequence.Select(x => x.GetRaw(touchpadEvent)));
                    if (!_config.Update.ContainsKey(touchpadEvent.EventType))
                    {
                        throw new NotSupportedException(
                            $"Touchpad event type \"{touchpadEvent.EventType}\" not supported");
                    }

                    update.AddRange(_config.Update[touchpadEvent.EventType].Select(x => x.GetRaw(touchpadEvent)));
                }

                update.AddRange(_config.PostUpdate.Select(x => x.GetRaw()));
                updates.Add(update);
            }

            return updates;
        }

        public List<List<RawInputEvent>?> BuildPostAbortRawUpdates()
        {
            return new List<List<RawInputEvent>?>()
            {
                new List<RawInputEvent>(_config.PostAbort.Select(x => x.GetRaw()))
            };
        }
    }
}