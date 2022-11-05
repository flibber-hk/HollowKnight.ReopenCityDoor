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
        internal MenuPage ReopenCityDoorPage;
        internal MenuEnum<GlobalSettings.RandoGateSetting> cityDoorButton;
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
            JumpToRPButton.AddHideAndShowEvent(landingPage, ReopenCityDoorPage);
            SetTopLevelButtonColor();

            button = JumpToRPButton;
            return true;
        }

        private void SetTopLevelButtonColor()
        {
            if (JumpToRPButton != null)
            {
                JumpToRPButton.Text.color = ReopenCityDoor.GS.IsRandoEnabled() ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
            }
        }


        private void ConstructMenu(MenuPage landingPage)
        {
            ReopenCityDoorPage = new MenuPage(Localize("ReopenCityDoor"), landingPage);
            cityDoorButton = new(ReopenCityDoorPage, "Fungal-City Door");
            cityDoorButton.Bind(ReopenCityDoor.GS, typeof(GlobalSettings).GetField(nameof(GlobalSettings.RandoSetting)));
            cityDoorButton.SelfChanged += obj => SetTopLevelButtonColor();

            rcdVIP = new(ReopenCityDoorPage, new(0, 300), 50f, true, new[] { cityDoorButton });
            Localize(cityDoorButton);
        }

        internal void UpdateMenu()
        {
            Instance.cityDoorButton.SetValue(ReopenCityDoor.GS.RandoSetting);
        }
    }
}