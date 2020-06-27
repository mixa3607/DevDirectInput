﻿using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DevDirectInput.Replay
{
    public class InputReplay
    {
        [JsonProperty] 
        public const short Version = 1;

        public bool StopOnTrigger { get; set; } = false;
        public bool StartOnTrigger { get; set; } = false;

        public string[] DevicePaths { get; set; } = new string[0];
        public int[] InputDeviceIds { get; set; } = new int[0];
        public int[] TriggerDeviceIds { get; set; } = new int[0];

        public float TickRate { get; set; } = 10;

        public List<uint[][]?> Updates { get; set; } = new List<uint[][]?>();
        public List<uint[][]?> PostAbortUpdates { get; set; } = new List<uint[][]?>();

        public string ToJson(Formatting formatting = Formatting.None, NamingStrategy? namingStrategy = null)
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = namingStrategy ?? new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.None
            });
        }
    }
}