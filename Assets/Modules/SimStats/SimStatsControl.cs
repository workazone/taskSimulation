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

        public void Bind()
        {
            if (_binded)
                return;

            _controlType.State.Value = SimStateType.Configurate;

            _binded = true;
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