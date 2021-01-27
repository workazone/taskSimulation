using System;
using System.IO;
using System.Threading.Tasks;
using Simulation;
using UnityEngine;

namespace Simulation
{
    [System.Serializable]
    public struct GameConfig
    {
        public float gameAreaWidth;
        public float gameAreaHeight;
        public float numUnitsToSpawn;
        public float unitSpawnDelay;
        public float unitSpawnMinRadius;
        public float unitSpawnMaxRadius;
        public float unitSpawnMinSpeed;
        public float unitSpawnMaxSpeed;
        public float unitDestroyRadius;

        public static GameConfig FromJSON(string path)
        {
            return JsonUtility.FromJson<GameConfig>(path);
        }

        public static void ToJSON(GameConfig config, string path)
        {
            var jsonStr = JsonUtility.ToJson(config);
            File.WriteAllText(jsonStr, path);
        }
    }

    [CreateAssetMenu(fileName = "ConfigReader", menuName = "ScriptableObjects/ConfigReader", order = 1)]
    public class ConfigReader : Service, IConfigReader
    {
        public override async Task Activate()
        {
            var dir = Application.persistentDataPath;

            try
            {
                await Task.Run(() => LoadConfig(dir, $"{nameof(GameConfig)}.json"));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public GameConfig LoadConfig(string dir, string file)
        {
            if (!Directory.Exists(dir))
                throw new DirectoryNotFoundException(dir);

            var path = Path.Combine(dir, file);
            if (!File.Exists(path))
                throw new FileNotFoundException($"{file} in {dir}");

            var text = File.ReadAllText(path);
            var config = GameConfig.FromJSON(text);
            return config;
        }
    }
}