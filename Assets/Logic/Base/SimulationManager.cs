using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
    public class SimulationManager : MonoBehaviour
    {
        [SerializeField] private Module[] _modules;

        private IModuleProvider _provider;

        private void Awake()
        {
            _provider = new ModuleProvider();

            foreach (var module in _modules)
                module.RegisterTo(_provider);

            _provider.Complete();
        }
    }
}