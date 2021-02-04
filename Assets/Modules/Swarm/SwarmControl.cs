using System;
using System.Collections;
using System.Collections.Generic;
using Simulation.Base;
using UnityEngine;

namespace Simulation.Modules
{
    public class SwarmControl : IBindableControl<ISwarmType>, ISwarmViewType
    {
        public event DisposeHandler OnDispose;

        private IModuleProvider _provider;
        private IDisposable _registerDisposer = default;
        private ISwarmType _controlType = new SwarmData();
        private bool _binded = default;

        public NotifiableProp<SimConfig> Config { get; } = new NotifiableProp<SimConfig>();

        public bool TryRegisterTo(IModuleProvider provider)
        {
            _provider = provider;
            _registerDisposer = provider.RegisterControl<ISwarmType>(this, _controlType, this);

            return _registerDisposer != null;
        }

        public void Bind(ISimConfigType config)
        {
            if (_binded)
                return;

            CheckBindings(config);

            config.Activate();
            config.Config.OnChanged += cfg => Config.Value = cfg;

            _binded = true;
        }

        private void CheckBindings(ISimConfigType config)
        {
            if (config == null)
                throw new NullReferenceException(nameof(ISimConfigType));
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