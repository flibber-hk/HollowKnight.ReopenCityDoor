﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ReopenCityDoor
{
    public class GlobalSettings
    {
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

        public bool IsRandoEnabled() => RandoSetting != RandoGateSetting.Disabled;

        public void LoadRandoFrom(GlobalSettings settings)
        {
            this.RandoSetting = settings.RandoSetting;
        }
    }
}
