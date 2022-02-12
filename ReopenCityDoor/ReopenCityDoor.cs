using System;
using System.Collections.Generic;
using System.Linq;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReopenCityDoor
{
    public class ReopenCityDoor : Mod
    {
        internal static ReopenCityDoor instance;

        /// <summary>
        /// Override whether the gate should be open.
        /// Input: false on Fungal side, True on City side
        /// Output: False to leave the gate closed
        /// </summary>
        public static event Func<bool, bool> ShouldOpenGate;
        internal static bool GetShouldOpenGate(bool citySide)
        {
            return ShouldOpenGate?.Invoke(citySide) ?? true;
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
        }

        private void OpenGate(Scene oldScene, Scene newScene)
        {
            switch (newScene.name)
            {
                case "Fungus2_08" when GetShouldOpenGate(false) || GameManager.instance.entryGateName == "rightRCD":
                    OpenFungalGate(newScene);
                    break;
                case "Crossroads_49b" when GetShouldOpenGate(true) || GameManager.instance.entryGateName == "leftRCD":
                    OpenCityGate(newScene);
                    break;
            }
        }

        private void OpenFungalGate(Scene scene)
        {
            scene.DestroyRootGameObject("Ruins_front_gate");
            scene.DestroyRootGameObject("Ruins_gate_0004_a");
            Extensions.CreateTransition(new(30, 58), new(31, 66), "rightRCD", "Crossroads_49b", "leftRCD");
        }

        private void OpenCityGate(Scene scene)
        {
            scene.DestroyRootGameObject("Ruins_front_gate");
            Extensions.CreateTransition(new(-1, 6), new(0, 14), "leftRCD", "Fungus2_08", "rightRCD");
        }
    }
}