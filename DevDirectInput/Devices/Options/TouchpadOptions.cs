namespace DevDirectInput.Devices.Options
{
    public interface ITouchpadOptions : IDeviceOptions
    {
        int Multitouch { get; }
        int MaxXPos { get; }
        int MaxYPos { get; }
        int DefaultClickPressure { get; }
        int DefaultClickTicks { get; }
    }

    public struct TouchpadOptions : ITouchpadOptions
    {
        public int Multitouch { get; set; }
        public int MaxXPos { get; set; }
        public int MaxYPos { get; set; }

        public int DefaultClickPressure { get; set; }
        public int DefaultClickTicks { get; set; }

        public string Path { get; set; }
    }
}