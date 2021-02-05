using System;
using System.Collections;
using System.Collections.Generic;
using Simulation.Base;
using UnityEngine;

namespace Simulation.Modules
{
    public class CameraControl : IBindableControl<ICameraType>, ICameraViewType
    {
        private ICameraType _controlType = new CameraData();
        private IModuleProvider _provider;
        private IDisposable _registerDisposer = default;
        private Vector2 _simAreaSize;
        private bool _binded = default;

        public NotifiableProp<Vector2> SimAreaSize { get; } = new NotifiableProp<Vector2>();
        public event DisposeHandler OnDispose;

        public bool TryRegisterTo(IModuleProvider provider)
        {
            _provider = provider;
            _registerDisposer = provider.RegisterControl<ICameraType>(this, _controlType, this);

            return _registerDisposer != null;
        }

        public void Bind(ISimStatsType stats, ISimConfigType config)
        {
            if (_binded)
                return;

            CheckBindings(stats, config);

            config.Config.OnChanged += cfg => _simAreaSize = new Vector2(cfg.gameAreaWidth, cfg.gameAreaHeight);
            stats.State.OnChanged += SetupOnState;

            _binded = true;
        }

        private void SetupOnState(SimStateType state)
        {
            if (state == SimStateType.Setup)
                SimAreaSize.Value = _simAreaSize;
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