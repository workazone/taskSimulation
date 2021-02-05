using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Simulation.Base;
using UnityEngine;

namespace Simulation.Modules
{
    public class SimConfigControl : IBindableControl<ISimConfigType>
    {
        public event DisposeHandler OnDispose;

        private const string fileName = "GameConfig";
        private IModuleProvider _provider;
        private IDisposable _registerDisposer = default;
        private ISimConfigType _controlType = new SimConfigData();
        private NotifiableProp<SimStateType> _publicState;
        private bool _binded = default;
        private bool _activated = default;

        public bool TryRegisterTo(IModuleProvider provider)
        {
            _provider = provider;
            _registerDisposer = provider.RegisterControl<ISimConfigType>(this, _controlType);

            return _registerDisposer != null;
        }

        public void Bind(ISimStatsType simStats)
        {
            if (_binded)
                return;

            CheckBindings(simStats);

            _publicState = simStats.State;

            _publicState.OnChanged += ActivateOnState;

            _binded = true;
        }

        private void CheckBindings(ISimStatsType simStats)
        {
            if (simStats == null)
                throw new NullReferenceException(nameof(ISimStatsType));
        }

        private void ActivateOnState(SimStateType state)
        {
            if (_activated || state != SimStateType.Configurate)
                return;

            LoadConfig();

            _activated = true;
        }

        private void LoadConfig()
        {
            var source = Application.persistentDataPath;
            var file = $"{fileName}.json";

            if (!Directory.Exists(source))
                throw new DirectoryNotFoundException(source);

            var path = Path.Combine(source, file);
            if (!File.Exists(path))
                throw new FileNotFoundException($"{file} in {source}");

            var text = File.ReadAllText(path);
            _controlType.Config.Value = SimConfig.FromJson(text);

            _publicState.OnChanged -= ActivateOnState;
            _publicState.Value = SimStateType.Setup;
        }

        public void Dispose()
        {
            _registerDisposer?.Dispose();
            _registerDisposer = null;
            OnDispose?.Invoke();
            OnDispose = null;
        }
    }
}