using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Simulation.Modules
{
    public struct SizeData
    {
        public float Width;
        public float Height;

        public SizeData(float width, float height)
        {
            Width = width;
            Height = height;
        }
    }

    public class Notifiable<T>
    {
        private Action<T> _changed;
        private T _current;

        public void Subscribe(Action<T> action)
        {
            _changed += action;

            action?.Invoke(_current);
        }

        public T Get()
        {
            return _current;
        }

        public void Set(T value)
        {
            if (!_current.Equals(value))
            {
                _current = value;
                _changed?.Invoke(_current);
            }
        }
    }

    public interface IEnvironment
    {
        Notifiable<SizeData> Data { get; }
    }

    [CreateAssetMenu(fileName = "Environment", menuName = "ScriptableObjects/Environment", order = 12)]
    public class Environment : Module, IEnvironment
    {
        public Notifiable<SizeData> Data { get; private set; } = new Notifiable<SizeData>();

        [SerializeField] private GameObject _viewPrefab;

        private EnvironmentView _view;
        private ISimConfig _config;
        private bool _initialized;

        public override void RegisterTo(IModuleProvider provider)
        {
            provider.Register<IEnvironment>(() => this);
        }

        public async void Init(ISimConfig config)
        {
            if(_initialized)
                return;

            if (config == null)
                throw new NullReferenceException(nameof(config));

            var module = config as Module;
            if (module != null && !module.Ready)
                module.InitialSetupOnScene();

            _config = config;

            Data.Set(new SizeData(_config.GameConfig.gameAreaWidth, _config.GameConfig.gameAreaHeight));

            InitialSetupOnScene();

            _initialized = true;
        }

        protected override void SetupOnScene()
        {
            var go = Instantiate(_viewPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}