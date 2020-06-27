using DevDirectInput.Devices.Options;

namespace DevDirectInput.Devices.Touchpads
{
    public interface ITouchpad : IInputDevice
    {
        new ITouchpadOptions Options { get; }
        void Tap(IAbsolutePosition position, int startTick);
        void Tap(IAbsolutePosition position, int startTick, int ticks);
        void Tap(int xPos, int yPos, int startTick);
        void Tap(int xPos, int yPos, int startTick, int ticks);
    }
}