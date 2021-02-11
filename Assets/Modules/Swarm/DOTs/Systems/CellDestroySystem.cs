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
    [UpdateAfter(typeof(CellSplitSystem))]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [BurstCompile]
    public class CellDestroySystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate()
        {
            commandBufferSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // delete job
            var commandBuffer = commandBufferSystem.CreateCommandBuffer().ToConcurrent();

            var deleteHandle = Entities.WithAll<DeleteTag>().ForEach((int entityInQueryIndex, Entity entity) =>
                {
                    commandBuffer.DestroyEntity(entityInQueryIndex, entity);
                })
                .WithBurst()
                .WithName("DestroyEntities")
                .Schedule(inputDeps);

            commandBufferSystem.AddJobHandleForProducer(deleteHandle);

            return deleteHandle;
        }
    }
}
