using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Simulation.Base
{
    public class EntryManager : ScriptableObject
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
        }
    }
}