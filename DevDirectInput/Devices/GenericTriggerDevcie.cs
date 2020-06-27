using System.Collections.Generic;
using DevDirectInput.Devices.Options;
using DevDirectInput.Enums;

namespace DevDirectInput.Devices
{
    public class GenericTriggerDevice : IInputDevice
    {
        public EDevicePurpose Purpose { get; } = EDevicePurpose.Trigger;
        public EDeviceType Type { get; } = EDeviceType.Keyboard;
        public bool SupportAborting { get; } = false;
        public IDeviceOptions Options { get; }

        public GenericTriggerDevice(IDeviceOptions options)
        {
            Options = options;
        }

        public List<List<RawInputEvent>?> BuildRawUpdates()
        {
            throw new System.NotSupportedException();
        }

        public List<List<RawInputEvent>?> BuildPostAbortRawUpdates()
        {
            throw new System.NotSupportedException();
        }
    }
}