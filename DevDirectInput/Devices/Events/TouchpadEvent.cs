using DevDirectInput.Enums;

namespace DevDirectInput.Devices.Events
{
    public interface ITouchpadEvent : IInputEvent
    {
        int TrackingId { get; set; }
        int Slot { get; set; }
        int Pressure { get; set; }
        int XPos { get; set; }
        int YPos { get; set; }
        ETouchpadEventType EventType { get; set; }
    }

    public class TouchpadEvent : ITouchpadEvent
    {
        public int TrackingId { get; set; }
        public int Slot { get; set; }
        public int Pressure { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public ETouchpadEventType EventType { get; set; }
        public IInputDevice? Device { get; set; }
    }
}