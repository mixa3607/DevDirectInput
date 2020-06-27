namespace DevDirectInput.Devices.Options
{
    public interface IDeviceOptions
    {
        string Path { get; }
    }

    public class DeviceOptions : IDeviceOptions
    {
        public string Path { get; set; }

        public DeviceOptions(string path)
        {
            Path = path;
        }
    }
}