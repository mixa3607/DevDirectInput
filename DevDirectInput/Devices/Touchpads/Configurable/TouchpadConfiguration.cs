using System.Collections.Generic;
using DevDirectInput.Enums;

namespace DevDirectInput.Devices.Touchpads.Configurable
{
    public class TouchpadConfiguration
    {
        public Dictionary<ETouchpadEventType, ConfigurationEvent[]> Update { get; set; } = new Dictionary<ETouchpadEventType, ConfigurationEvent[]>();
        public ConfigurationEvent[] PreSequence { get; set; } = new ConfigurationEvent[0];
        public ConfigurationEvent[] PostUpdate { get; set; } = new ConfigurationEvent[0];
        public ConfigurationEvent[] PostAbort { get; set; } = new ConfigurationEvent[0];
    }
}