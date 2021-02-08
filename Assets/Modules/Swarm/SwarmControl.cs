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

        public NotifiableProp<SimConfig> ViewConfig { get; } = new NotifiableProp<SimConfig>();
        public NotifiableProp<SimStateType> ViewState { get; } = new NotifiableProp<SimStateType>();

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

            config.Config.OnChanged += cfg => _config = cfg;
            stats.State.OnChanged += OnPublicStateChanged;
            ViewState.OnChanged += OnViewStateChanged;

            _publicState = stats.State;

            _binded = true;
        }

        private void OnPublicStateChanged(SimStateType state)
        {
            switch (state)
            {
                case SimStateType.Setup:
                    ViewConfig.Value = _config;
                    ViewState.Value = state;
                    break;
                case SimStateType.Spawning:
                    ViewState.Value = state;
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
                    _publicState.Value = SimStateType.Spawning;
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