using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

namespace Simulation.Modules
{
    [BurstCompile]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class CellFitBordersSystem : JobComponentSystem
    {
        // x,y,z,w => left, right, bottom, top
        public float4 Borders;

        private EndSimulationEntityCommandBufferSystem _endSimEcbSystem;

        protected override void OnCreate()
        {
            _endSimEcbSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var borders = Borders;
            var commandBuffer = _endSimEcbSystem.CreateCommandBuffer().ToConcurrent();

            var checkHandle = Entities.WithAll<DirtyTag>().ForEach((int entityInQueryIndex, Entity entity, in CellComponent cell, in NonUniformScale scale, in Translation translation) =>
                {
                    commandBuffer.RemoveComponent<DirtyTag>(entityInQueryIndex, entity);

                    // удалить если ячейка за границей
                    if (cell.Borders.y < borders.x || cell.Borders.x > borders.y || cell.Borders.w < borders.z || cell.Borders.z > borders.w)
                    {
                        commandBuffer.AddComponent<DeleteTag>(entityInQueryIndex, entity);
                    }
                    // ячейка выступает за границу частично - разделить если позволяет размер, иначе удалить
                    else if (cell.Borders.x < borders.x || cell.Borders.y > borders.y || cell.Borders.z < borders.z || cell.Borders.w > borders.w)
                    {
                        if (scale.Value.x < 0.05f)
                        {
                            commandBuffer.AddComponent<DeleteTag>(entityInQueryIndex, entity);
                        }
                        else
                            commandBuffer.AddComponent<SplitTag>(entityInQueryIndex, entity);
                    }
                })
                .WithBurst()
                .WithName("MarkCells")
                .Schedule(inputDeps);

            _endSimEcbSystem.AddJobHandleForProducer(checkHandle);

            return checkHandle;
        }
    }
}