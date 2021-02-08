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
        // x,y,z,w => left, right, bottom, top
        public float4 Borders;

        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate()
        {
            commandBufferSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // delete job
            var borders = Borders;
            var commandBuffer = commandBufferSystem.CreateCommandBuffer().ToConcurrent();

            var deleteHandle = Entities.WithAll<DeleteTag>().ForEach((int entityInQueryIndex, Entity entity, in NonUniformScale scale, in Translation translation) =>
                {
                    commandBuffer.DestroyEntity(entityInQueryIndex, entity);
                })
                .WithBurst()
                .WithName("DestoyCells")
                .Schedule(inputDeps);

            commandBufferSystem.AddJobHandleForProducer(deleteHandle);

            return deleteHandle;
        }
    }
}
