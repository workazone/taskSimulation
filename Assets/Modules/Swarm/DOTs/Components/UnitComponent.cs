using Unity.Entities;
using Unity.Mathematics;

namespace Simulation.Modules
{
    [GenerateAuthoringComponent]
    public struct UnitComponent : IComponentData
    {
        public float2 Position;
        public float Radius;
    }
}