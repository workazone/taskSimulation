using Unity.Entities;
using Unity.Mathematics;

namespace Simulation.Modules
{
    [GenerateAuthoringComponent]
    public struct CellComponent : IComponentData
    {
        public float4 Borders;

        public CellComponent(float4 borders)
        {
            Borders = borders;
        }

        public CellComponent(float3 posiiton, float3 scale)
        {
            Borders = new float4(
                posiiton.x - scale.x,
                posiiton.x + scale.x,
                posiiton.y - scale.y,
                posiiton.y + scale.y);
        }
    }
}