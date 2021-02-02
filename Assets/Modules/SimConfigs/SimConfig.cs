using System;
using System.IO;
using System.Threading.Tasks;
using Simulation;
using UnityEngine;

namespace Simulation.Modules
{
    [CreateAssetMenu(fileName = "SimConfigs", menuName = "ScriptableObjects/SimConfigs", order = 11)]
    public class SimConfig : Module, ISimConfig
    {
        public GameConfig GameConfig
        {
            get { return _config; }
        }

        private GameConfig _config;
        private IDisposable _unregister;

        public override void RegisterTo(IModuleProvider provider)
        {
            _unregister = provider.Register<ISimConfig>(() => this);
        }

        protected override void SetupOnScene()
        {
            var dir = Application.persistentDataPath;

            LoadConfig(dir, $"{nameof(GameConfig)}.json");
        }

        public void LoadConfig(string source, string file)
        {
            if (!Directory.Exists(source))
                throw new DirectoryNotFoundException(source);

            var path = Path.Combine(source, file);
            if (!File.Exists(path))
                throw new FileNotFoundException($"{file} in {source}");

            var text = File.ReadAllText(path);
            _config = GameConfig.FromJSON(text);
        }

        private void OnDestroy()
        {
            _unregister.Dispose();
        }
    }
}