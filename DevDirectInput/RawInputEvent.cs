using DevDirectInput.Enums;

namespace DevDirectInput
{
    public struct RawInputEvent
    {
        public RawInputEvent(EEventType type, EEventCode code, ETouchState value)
        {
            Type = type;
            Code = code;
            Value = (uint) value;
        }

        public RawInputEvent(EEventType type, EEventCode code, uint value)
        {
            Type = type;
            Code = code;
            Value = value;
        }

        public readonly EEventType Type;
        public readonly EEventCode Code;
        public readonly uint Value;

        public uint[] ToUintArray(uint deviceId)
        {
            return new[] {deviceId, (uint) Type, (uint) Code, Value};
        }

        public uint[] ToUintArray()
        {
            return new[] {(uint) Type, (uint) Code, Value};
        }

        public override string ToString()
        {
            return $"{Type} {Code} {Value}";
        }
    }
}