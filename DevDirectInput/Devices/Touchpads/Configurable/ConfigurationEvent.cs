using System;
using DevDirectInput.Devices.Events;
using DevDirectInput.Enums;

namespace DevDirectInput.Devices.Touchpads.Configurable
{
    public struct ConfigurationEvent
    {
        public EValueType ValueType;
        public EEventType EventType;
        public EEventCode EventCode;
        public uint EventValue;

        public ConfigurationEvent(EValueType valueType, EEventType eventType, EEventCode eventCode, uint eventValue = 0)
        {
            ValueType = valueType;
            EventType = eventType;
            EventCode = eventCode;
            EventValue = eventValue;
        }

        public RawInputEvent GetRaw(TouchpadEvent touchpadEvent)
        {
            var value = ValueType switch
            {
                EValueType.EventSlot => (uint)touchpadEvent.Slot,
                EValueType.EventTrackingId => (uint)touchpadEvent.TrackingId,
                EValueType.EventXPos => (uint)touchpadEvent.XPos,
                EValueType.EventYPos => (uint)touchpadEvent.YPos,
                EValueType.UintMax => uint.MaxValue,
                EValueType.Value => EventValue,
                _ => throw new NotSupportedException($"Value type \"{ValueType}\" not supported")
            };
            return new RawInputEvent(EventType, EventCode, value);
        }

        public RawInputEvent GetRaw()
        {
            var value = ValueType switch
            {
                EValueType.UintMax => uint.MaxValue,
                EValueType.Value => EventValue,
                _ => throw new NotSupportedException($"Value type \"{ValueType}\" not supported for no event overload")
            };
            return new RawInputEvent(EventType, EventCode, value);
        }
    }
}