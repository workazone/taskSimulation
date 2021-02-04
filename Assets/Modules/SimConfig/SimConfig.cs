using System.IO;
using UnityEngine;

namespace Simulation.Modules
{
    [System.Serializable]
    public struct SimConfig
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

        public static SimConfig FromJson(string path)
        {
            return JsonUtility.FromJson<SimConfig>(path);
        }

        public static void ToJson(SimConfig config, string path)
        {
            var jsonStr = JsonUtility.ToJson(config);
            File.WriteAllText(jsonStr, path);
        }
    }
}