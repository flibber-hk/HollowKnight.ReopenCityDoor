using System;
using System.Collections.Generic;
using System.Linq;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReopenCityDoor
{
    public class ReopenCityDoor : Mod, IGlobalSettings<GlobalSettings>, IMenuMod
    {
        internal static ReopenCityDoor instance;

        public static GlobalSettings GS = new();
        public void OnLoadGlobal(GlobalSettings gs) => GS = gs;
        public GlobalSettings OnSaveGlobal() => GS;

        public bool ToggleButtonInsideMenu => true;
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry>()
            {
                new IMenuMod.MenuEntry
                {
                    Name = "Gate state",
                    Description = "Choose the default state of the city/fungal gate",
                    Values = new string[] { "Open", "Closed" },
                    Saver = opt => GS.GateOpen = (opt == 0),
                    Loader = () => GS.GateOpen ? 0 : 1
                }
            };
        }

        /// <summary>
        /// Override whether the gate should be open.
        /// Input: false on Fungal side, true on City side
        /// Output: False to leave the gate closed
        /// </summary>
        public static event Func<bool, bool> ShouldOpenGate;
        internal static bool GetShouldOpenGate(bool citySide)
        {
            return ShouldOpenGate?.Invoke(citySide) ?? GS.GateOpen;
        }

        public ReopenCityDoor() : base(null)
        {
            instance = this;
        }
        
        public override string GetVersion()
        {
            return GetType().Assembly.GetName().Version.ToString();
        }
        
        public override void Initialize()
        {
            Log("Initializing Mod...");

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OpenGate;

            bool rando = ModHooks.GetMod("Randomizer 4") is Mod;
            bool ic = ModHooks.GetMod("ItemChangerMod") is Mod;
            Rando.RandoInterop.Hook(rando, ic);
        }

        public void Unload()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OpenGate;
        }

        private void OpenGate(Scene oldScene, Scene newScene)
        {
            switch (newScene.name)
            {
                case ItemChanger.SceneNames.Fungus2_08 when GetShouldOpenGate(false) || GameManager.instance.entryGateName == Consts.FungalGateName:
                    OpenFungalGate(newScene);
                    break;
                case ItemChanger.SceneNames.Crossroads_49b when GetShouldOpenGate(true) || GameManager.instance.entryGateName == Consts.CityGateName:
                    OpenCityGate(newScene);
                    break;
            }
        }

        private void OpenFungalGate(Scene scene)
        {
            scene.DestroyRootGameObject("Ruins_front_gate");
            scene.DestroyRootGameObject("Ruins_gate_0004_a");
            Extensions.CreateTransition(new(30, 58), new(31, 66), Consts.FungalGateName, ItemChanger.SceneNames.Crossroads_49b, Consts.CityGateName);
        }

        private void OpenCityGate(Scene scene)
        {
            scene.DestroyRootGameObject("Ruins_front_gate");
            Extensions.CreateTransition(new(-1, 6), new(0, 14), Consts.CityGateName, ItemChanger.SceneNames.Fungus2_08, Consts.FungalGateName);
        }
    }
}