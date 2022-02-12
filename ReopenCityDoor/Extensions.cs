using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReopenCityDoor
{
    public static class Extensions
    {
        public static bool DestroyRootGameObject(this Scene scene, string objectName)
        {
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                if (go.name == objectName)
                {
                    UnityEngine.Object.Destroy(go);
                    return true;
                }
            }
            return false;
        }

        public static GameObject CreateTransition(Vector2 bl, Vector2 tr, string gateName, string targetScene, string targetGate)
        {
            GameObject transitionGO = new GameObject();
            transitionGO.name = gateName;
            transitionGO.transform.position = (bl + tr) / 2;
            
            BoxCollider2D box = transitionGO.AddComponent<BoxCollider2D>();
            box.size = tr - bl;
            box.offset = Vector2.zero;
            box.isTrigger = true;

            TransitionPoint tp = transitionGO.AddComponent<TransitionPoint>();
            tp.targetScene = targetScene;
            tp.entryPoint = targetGate;
            tp.nonHazardGate = true;

            GateSnap snap = transitionGO.AddComponent<GateSnap>();

            transitionGO.SetActive(true);
            return transitionGO;
        }
    }
}
