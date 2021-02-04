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
        private bool _binded = default;
        private bool _activated = default;

        public bool TryRegisterTo(IModuleProvider provider)
        {
            _provider = provider;
            _registerDisposer = provider.RegisterControl<ISimConfigType>(this, _controlType);

            return _registerDisposer != null;
        }

        public void Bind()
        {
            if (_binded)
                return;

            _controlType.Activate += ActivateOnDemand;

            _binded = true;
        }

        private void ActivateOnDemand()
        {
            if (_activated)
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