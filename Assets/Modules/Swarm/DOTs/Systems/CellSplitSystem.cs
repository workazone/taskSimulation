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
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(CellFitBordersSystem))]
    [BurstCompile]
    public class CellSplitSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem _endSimEcbSystem;

        protected override void OnCreate()
        {
            _endSimEcbSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        public static void SpawnCell(int entityInQueryIndex, Entity entity, EntityCommandBuffer.Concurrent commandBuffer, float3 positon, float3 scale)
        {
            Entity cellEntity = commandBuffer.Instantiate(entityInQueryIndex, entity);
            commandBuffer.SetComponent(entityInQueryIndex, cellEntity, new Translation() {Value = positon});
            commandBuffer.SetComponent(entityInQueryIndex, cellEntity, new CellComponent(positon, scale));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var commandBuffer = _endSimEcbSystem.CreateCommandBuffer().ToConcurrent();

            // split job
            var splitHandle = Entities.WithAll<SplitTag>().ForEach((Entity entity, int entityInQueryIndex, ref CellComponent cell, ref NonUniformScale scale, ref Translation translation) =>
                {
                    commandBuffer.RemoveComponent<SplitTag>(entityInQueryIndex, entity);
                    commandBuffer.AddComponent<DirtyTag>(entityInQueryIndex, entity);

                    // сместить ячейку на позицию новой, справа сверху
                    scale.Value /= 2f;
                    translation.Value.x += scale.Value.x;
                    translation.Value.z += scale.Value.z;
                    commandBuffer.SetComponent(entityInQueryIndex, entity, new CellComponent(translation.Value, scale.Value));

                    // нагенерить 3 новых ячейки
                    // справа снизу
                    SpawnCell(entityInQueryIndex, entity, commandBuffer, new float3(translation.Value.x, translation.Value.y, translation.Value.z - 2f * scale.Value.z), scale.Value);
                    SpawnCell(entityInQueryIndex, entity, commandBuffer, new float3(translation.Value.x - 2f * scale.Value.x, translation.Value.y, translation.Value.z - 2f * scale.Value.z), scale.Value);
                    SpawnCell(entityInQueryIndex, entity, commandBuffer, new float3(translation.Value.x - 2f * scale.Value.x, translation.Value.y, translation.Value.z), scale.Value);
                })
                .WithBurst()
                .WithName("SplitCells")
                .Schedule(inputDeps);

            _endSimEcbSystem.AddJobHandleForProducer(splitHandle);

            return splitHandle;
        }
    }
}