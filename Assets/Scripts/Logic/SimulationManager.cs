using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
    public class SimulationManager : MonoBehaviour, ISimulationManager
    {
        public static SimulationManager Instance
        {
            get { return _instance; }
        }

        private static SimulationManager _instance;

        [SerializeField] private Service[] _services;

        private Dictionary<Type, Func<object>> _managers = new Dictionary<Type, Func<object>>();

        private bool _activated;

        public async void Activate()
        {
            if (_activated)
                return;

            if (_instance != null)
                Destroy(this);

            for (int i = 0; i < _services.Length; i++)
            {
                await _services[i].Activate();
            }

            _instance = this;
        }
    }
}