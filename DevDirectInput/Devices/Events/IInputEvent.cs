namespace DevDirectInput.Devices.Events
{
    public interface IInputEvent
    {
        public IInputDevice? Device { get; }
    }
}