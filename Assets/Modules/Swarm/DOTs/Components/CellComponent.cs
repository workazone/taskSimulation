using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct CellComponent : IComponentData
{
    public float Size;
    public float3 Position;
    public float4 Edges;  // x,y,z,w => left, right, bottom, top

    public CellComponent(float3 pos, float halfSize)
    {
        Position = pos;
        Size = 2f * halfSize;
        Edges = new float4(pos.x - halfSize, pos.x + halfSize, pos.y - halfSize, pos.y + halfSize);
    }
}
