using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Simulation.Modules
{
    [GenerateAuthoringComponent]
    public struct SpawnPoint : IComponentData
    {
        public float Radius;
        public float2 Position;
    }
}