using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;

namespace ReopenCityDoor.Rando
{
    internal static class RandoSettingsManagerInterop
    {
        public static void Hook()
        {
            RandoSettingsManagerMod.Instance.RegisterConnection(new RandoPlusSettingsProxy());
        }
    }

    internal class RandoPlusSettingsProxy : RandoSettingsProxy<GlobalSettings, string>
    {
        public override string ModKey => ReopenCityDoor.instance.GetName();

        public override VersioningPolicy<string> VersioningPolicy { get; }
            = new EqualityVersioningPolicy<string>(ReopenCityDoor.instance.GetVersion());

        public override void ReceiveSettings(GlobalSettings settings)
        {
            ReopenCityDoor.GS.LoadRandoFrom(settings);
            RandoMenuPage.Instance.UpdateMenu();
        }

        public override bool TryProvideSettings(out GlobalSettings settings)
        {
            settings = ReopenCityDoor.GS;
            return settings.IsRandoEnabled();
        }
    }
}