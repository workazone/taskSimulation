using Unity.Entities;
using Unity.Mathematics;

namespace Simulation.Modules
{
    [GenerateAuthoringComponent]
    public struct CellComponent : IComponentData
    {
        // x,y,z,w => left, right, bottom, top
        public float4 Borders;

        public CellComponent(float4 borders)
        {
            Borders = borders;
        }

        public CellComponent(float3 position, float3 scale)
        {
            Borders = new float4(
                position.x - scale.x,
                position.x + scale.x,
                position.z - scale.z,
                position.z + scale.z);
        }
    }
}