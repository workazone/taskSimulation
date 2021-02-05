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
        private NotifiableProp<SimStateType> _publicState;
        private SimConfig _config;
        private bool _binded = default;

        public NotifiableProp<SimConfig> Config { get; } = new NotifiableProp<SimConfig>();
        public NotifiableProp<SimStateType> State { get; } = new NotifiableProp<SimStateType>();

        public bool TryRegisterTo(IModuleProvider provider)
        {
            _provider = provider;
            _registerDisposer = provider.RegisterControl<ISwarmType>(this, _controlType, this);

            return _registerDisposer != null;
        }

        public void Bind(ISimStatsType stats, ISimConfigType config)
        {
            if (_binded)
                return;

            CheckBindings(stats, config);

            _publicState = stats.State;

            config.Config.OnChanged += cfg => _config = cfg;
            stats.State.OnChanged += OnPublicStateChanged;

            _binded = true;
        }

        private void OnPublicStateChanged(SimStateType state)
        {
            switch (state)
            {
                case SimStateType.Setup:
                    Config.Value = _config;
                    State.Value = state;
                    break;
                case SimStateType.Spawning:
                    break;
                case SimStateType.Moving:
                    break;
                default:
                    break;
            }
        }

        private void OnViewStateChanged(SimStateType stateReady)
        {
            switch (stateReady)
            {
                case SimStateType.Spawning:
                    break;
                case SimStateType.Moving:
                    break;
                default:
                    break;
            }
        }

        private void CheckBindings(ISimStatsType stats, ISimConfigType config)
        {
            if (stats == null)
                throw new NullReferenceException(nameof(ISimStatsType));
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