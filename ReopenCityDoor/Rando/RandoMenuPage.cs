using MenuChanger;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using MenuChanger.Extensions;
using RandomizerMod.Menu;
using static RandomizerMod.Localization;

namespace ReopenCityDoor.Rando
{
    public class RandoMenuPage
    {
        internal MenuPage ReopenCityDoor;
        internal MenuElementFactory<GlobalSettings> rcdMEF;
        internal VerticalItemPanel rcdVIP;

        internal SmallButton JumpToRPButton;

        private static RandoMenuPage _instance = null;
        internal static RandoMenuPage Instance => _instance ?? (_instance = new RandoMenuPage());

        public static void OnExitMenu()
        {
            _instance = null;
        }

        public static void Hook()
        {
            RandomizerMenuAPI.AddMenuPage(Instance.ConstructMenu, Instance.HandleButton);
            MenuChangerMod.OnExitMainMenu += OnExitMenu;
        }

        private bool HandleButton(MenuPage landingPage, out SmallButton button)
        {
            JumpToRPButton = new(landingPage, Localize("ReopenCityDoor"));
            JumpToRPButton.AddHideAndShowEvent(landingPage, ReopenCityDoor);
            button = JumpToRPButton;
            return true;
        }

        private void ConstructMenu(MenuPage landingPage)
        {
            ReopenCityDoor = new MenuPage(Localize("ReopenCityDoor"), landingPage);
            rcdMEF = new(ReopenCityDoor, global::ReopenCityDoor.ReopenCityDoor.GS);
            rcdVIP = new(ReopenCityDoor, new(0, 300), 50f, true, rcdMEF.Elements);
            Localize(rcdMEF);
        }
    }
}