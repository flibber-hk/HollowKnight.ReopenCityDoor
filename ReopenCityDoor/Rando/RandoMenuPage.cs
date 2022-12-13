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
        internal static RandoMenuPage Instance { get; private set; }

        public static void OnExitMenu()
        {
            Instance = null;
        }

        public static void Hook()
        {
            RandomizerMenuAPI.AddMenuPage(ConstructMenu, HandleButton);
            MenuChangerMod.OnExitMainMenu += OnExitMenu;
        }

        private static bool HandleButton(MenuPage landingPage, out SmallButton button)
        {
            button = Instance.JumpToRPButton;
            return true;
        }

        private void SetButtonColours()
        {
            if (JumpToRPButton != null)
            {
                JumpToRPButton.Text.color = ReopenCityDoor.GS.IsRandoEnabled() ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
            }
            if (cityDoorButton != null)
            {
                cityDoorButton.Text.color = ReopenCityDoor.GS.IsRandoEnabled() ? Colors.TRUE_COLOR : Colors.FALSE_COLOR;
            }
        }


        private static void ConstructMenu(MenuPage landingPage) => Instance = new(landingPage);

        private RandoMenuPage(MenuPage landingPage)
        {
            ReopenCityDoorPage = new MenuPage(Localize("ReopenCityDoor"), landingPage);
            cityDoorButton = new(ReopenCityDoorPage, "Fungal-City Door");
            cityDoorButton.Bind(ReopenCityDoor.GS, typeof(GlobalSettings).GetField(nameof(GlobalSettings.RandoSetting)));
            cityDoorButton.SelfChanged += obj => SetButtonColours();

            rcdVIP = new(ReopenCityDoorPage, new(0, 300), 50f, true, new[] { cityDoorButton });
            Localize(cityDoorButton);

            JumpToRPButton = new(landingPage, Localize("ReopenCityDoor"));
            JumpToRPButton.AddHideAndShowEvent(landingPage, ReopenCityDoorPage);
            SetButtonColours();
        }

        internal void UpdateMenu()
        {
            cityDoorButton.SetValue(ReopenCityDoor.GS.RandoSetting);
        }
    }
}