using System.IO;
using UnityEngine;

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
