using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Simulation.Base
{
    public class SimulationManager : MonoBehaviour
    {
        [SerializeField] private ModulesHub[] _hubs;

        private ModuleBase[] _modules;
        private IModuleProvider _provider;

        private void Awake()
        {
            _provider = new ModuleProvider();

            _modules = _hubs.SelectMany(t => t.Modules).ToArray();

            foreach (var module in _modules)
                module?.Init(_provider);

            _provider.BindAllControls();
        }
    }
}