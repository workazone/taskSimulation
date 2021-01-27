using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Simulation
{
    public abstract class EntryPoint : ScriptableObject
    {
        [RuntimeInitializeOnLoadMethod()]
        public static void Default()
        {
            CreateInitialPrefab();
        }

        private static void CreateInitialPrefab()
        {
            var main = Instantiate(Resources.Load("Main")) as GameObject;
            main.name = "Main";
            var manager = main.GetComponent<ISimulationManager>();
            manager.Activate();
        }
    }
}