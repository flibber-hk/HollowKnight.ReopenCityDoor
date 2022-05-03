using System;
using Modding;

namespace ReopenCityDoor.Rando
{
    /// <summary>
    /// Module to manage the behaviour of the city gate when running through ItemChanger.
    /// </summary>
    public class CityGateModule : ItemChanger.Modules.Module
    {
        public bool GateOpen { get; set; } = false;

        public override void Initialize()
        {
            ModHooks.GetPlayerBoolHook += SkillBoolGetOverride;
            ModHooks.SetPlayerBoolHook += SkillBoolSetOverride;

            ReopenCityDoor.ShouldOpenGate += OverrideShouldOpenGate;
        }

        public override void Unload()
        {
            ModHooks.GetPlayerBoolHook -= SkillBoolGetOverride;
            ModHooks.SetPlayerBoolHook -= SkillBoolSetOverride;

            ReopenCityDoor.ShouldOpenGate -= OverrideShouldOpenGate;
        }

        private bool OverrideShouldOpenGate(bool citySide)
        {
            return GateOpen;
        }

        private bool SkillBoolGetOverride(string name, bool orig)
        {
            if (name == Consts.FungalCityGateBoolName)
            {
                return GateOpen;
            }
            return orig;
        }

        private bool SkillBoolSetOverride(string name, bool orig)
        {
            if (name == Consts.FungalCityGateBoolName)
            {
                GateOpen = orig;
            }
            return orig;
        }
    }
}
