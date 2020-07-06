using System;
using System.Collections.Generic;
using System.Linq;
using DevDirectInput.Devices.Events;
using DevDirectInput.Devices.Options;
using DevDirectInput.Enums;
using DevDirectInput.Replay;

namespace DevDirectInput.Devices.Touchpads
{
    [Obsolete("Use configurable touchpad")]
    public class MemuTouchpad : ITouchpad
    {
        public EDeviceType Type { get; } = EDeviceType.Touchpad;
        IDeviceOptions IInputDevice.Options => Options;

        public EDevicePurpose Purpose { get; set; }
        public bool SupportAborting { get; } = false;
        public ITouchpadOptions Options { get; }

        private int _trackingId = 100;
        private readonly InputTimeline<TouchpadEvent> _timeline = new InputTimeline<TouchpadEvent>();

        public MemuTouchpad(ITouchpadOptions options)
        {
            Options = options;
        }

        public MemuTouchpad(ITouchpadOptions options, EDevicePurpose purpose)
        {
            Options = options;
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

        public void Swipe(IAbsolutePosition from, IAbsolutePosition to, int startTick, int ticks)
        {
            throw new NotImplementedException();
        }

        public void Swipe(int fromX, int fromY, int toX, int toY, int startTick, int ticks)
        {
            throw new NotImplementedException();
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
                    update.Add(new RawInputEvent(EEventType.EvAbs, EEventCode.AbsMtSlot, (uint)touchpadEvent.Slot));
                    switch (touchpadEvent.EventType)
                    {
                        case ETouchpadEventType.Press:
                            update.Add(new RawInputEvent(EEventType.EvAbs, EEventCode.AbsMtPositionX, (uint)touchpadEvent.XPos));
                            update.Add(new RawInputEvent(EEventType.EvAbs, EEventCode.AbsMtPositionY, (uint)touchpadEvent.YPos));
                            update.Add(new RawInputEvent(EEventType.EvAbs, EEventCode.AbsMtTouchMajor, 10));
                            update.Add(new RawInputEvent(EEventType.EvAbs, EEventCode.AbsMtTrackingId, (uint)touchpadEvent.TrackingId));
                            break;
                        case ETouchpadEventType.Release:
                            update.Add(new RawInputEvent(EEventType.EvAbs, EEventCode.AbsMtTouchMajor, (uint)0));
                            update.Add(new RawInputEvent(EEventType.EvAbs, EEventCode.AbsMtTrackingId, uint.MaxValue));
                            break;
                        default: throw new NotSupportedException($"Touchpad event type \"{touchpadEvent.EventType}\" not supported");
                    }
                }

                update.Add(new RawInputEvent(EEventType.EvSyn, EEventCode.SynReport, (uint)0));
                updates.Add(update);
            }

            return updates;
        }

        public List<List<RawInputEvent>?> BuildPostAbortRawUpdates()
        {
            throw new NotImplementedException();
        }
    }
}