using System.Collections.Generic;
using DevDirectInput.Devices.Options;
using DevDirectInput.Enums;

namespace DevDirectInput.Devices
{
    public interface IInputDevice
    {
        IDeviceOptions Options { get; }
        EDevicePurpose Purpose { get; }
        EDeviceType Type { get; }
        bool SupportAborting { get; }
        List<List<RawInputEvent>?> BuildRawUpdates();
        List<List<RawInputEvent>?> BuildPostAbortRawUpdates();
    }
}