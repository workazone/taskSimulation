using System;
using System.Collections;
using System.Collections.Generic;
using Simulation.Base;
using UnityEngine;

namespace Simulation.Modules
{
    public class SimStatsControl : IBindableControl<ISimStatsType>, ISimStataViewData
    {
        public event DisposeHandler OnDispose;

        private IModuleProvider _provider;
        private IDisposable _registerDisposer = default;
        private ISimStatsType _controlType = new SimStatsData();
        private bool _binded = default;

        public bool TryRegisterTo(IModuleProvider provider)
        {
            _provider = provider;
            _registerDisposer = provider.RegisterControl<ISimStatsType>(this, _controlType, this);

            return _registerDisposer != null;
        }

        public void Bind(ISimConfigType config, ISwarmType swarmType)
        {
            if (_binded)
                return;

            CheckBindings(config, swarmType);

            config.Activate();

            _binded = true;
        }

        private void CheckBindings(ISimConfigType config, ISwarmType swarmType)
        {
            if (config == null)
                throw new NullReferenceException(nameof(ISimConfigType));
            if (swarmType == null)
                throw new NullReferenceException(nameof(ISwarmType));
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