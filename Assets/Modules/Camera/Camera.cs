using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Modules
{
    public interface ICamera
    {
    }

    [CreateAssetMenu(fileName = "Camera", menuName = "ScriptableObjects/Camera", order = 13)]
    public class Camera : Module, ICamera
    {
        [SerializeField] private GameObject _viewPrefab;

        private IEnvironment _environment;
        private SizeData _sizeData;
        private bool _initialized;

        public override void RegisterTo(IModuleProvider provider)
        {
            provider.Register<ICamera>(() => this);
        }

        public async void Init(IEnvironment environment)
        {
            if(_initialized)
                return;

            if (environment == null)
                throw new NullReferenceException(nameof(environment));

            var module = environment as Module;
            if (module != null && !module.Ready)
                module.InitialSetupOnScene();

            _environment = environment;

            _environment.Data.Subscribe(d => UpdateVisibleAreaSize(d));

            InitialSetupOnScene();

            _initialized = true;
        }

        protected override void SetupOnScene()
        {
            var go = Instantiate(_viewPrefab);
        }

        private void UpdateVisibleAreaSize(SizeData size)
        {

        }
    }
}