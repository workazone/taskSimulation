using System;
using System.Collections;
using System.Collections.Generic;
using Simulation.Base;
using UnityEngine;

namespace Simulation.Modules
{
    public class EnvironmentControl : IBindableControl<IEnvironmentType>, IEnvironmentViewType
    {
        public event DisposeHandler OnDispose;
        public NotifiableProp<Vector2> SimAreaSize { get; private set; } = new NotifiableProp<Vector2>();

        private IModuleProvider _provider;
        private IDisposable _registerDisposer = default;
        private IEnvironmentType _controlType = new EnvironmentData();
        private bool _binded = default;

        public bool TryRegisterTo(IModuleProvider provider)
        {
            _provider = provider;
            _registerDisposer = provider.RegisterControl<IEnvironmentType>(this, _controlType, this);

            return _registerDisposer != null;
        }

        public void Bind(ISimConfigType config)
        {
            if (_binded)
                return;

            CheckBindings(config);

            config.Activate();
            config.Config.OnChanged += cfg => SimAreaSize.Value = new Vector2(cfg.gameAreaWidth, cfg.gameAreaHeight);

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