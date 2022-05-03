﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ReopenCityDoor
{
    public class GlobalSettings
    {
        [MenuChanger.Attributes.MenuIgnore]
        public bool GateOpen = true;
        
        [JsonConverter(typeof(StringEnumConverter))]
        public enum RandoGateSetting
        {
            DefineRefs,
            Disabled,
            Open,
            Unlockable,
        }
        public RandoGateSetting RandoSetting = RandoGateSetting.Disabled;
    }
}
