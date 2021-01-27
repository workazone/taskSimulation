namespace Simulation
{
    public interface IConfigReader
    {
        GameConfig LoadConfig(string dir, string file);
    }
}