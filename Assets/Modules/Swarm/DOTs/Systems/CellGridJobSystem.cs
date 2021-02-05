using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

namespace Simulation.Modules
{
    public class CellGridJobSystem : JobComponentSystem
    {
        // x,y,z,w => left, right, bottom, top
        public float4 Borders;

        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate()
        {
            commandBufferSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        [BurstCompile]
        struct SplitJob : IJobForEachWithEntity<CellComponent, NonUniformScale, Translation>
        {
            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeArray<float4> Borders;

            public EntityCommandBuffer.Concurrent CommandBufferSplit;

            public void Execute(Entity entity, int index, ref CellComponent cellComponent, ref NonUniformScale scale, ref Translation translation)
            {
                // удалить если ячейка полностью за границей
                if (cellComponent.Edges.y < Borders[0].x || cellComponent.Edges.x > Borders[0].y || cellComponent.Edges.w < Borders[0].z || cellComponent.Edges.z > Borders[0].w)
                {
                    CommandBufferSplit.DestroyEntity(index, entity);
                    return;
                }

                if (cellComponent.Size < 0.1f)
                    return;

                // раздробить ячейку на 4 дочерние ячейки если она частично выходит за границы
                if (cellComponent.Edges.x < Borders[0].x || cellComponent.Edges.y > Borders[0].y || cellComponent.Edges.z < Borders[0].z || cellComponent.Edges.w > Borders[0].w)
                {
                    // сместить ячейку на позицию новой, справа сверху
                    cellComponent.Size /= 2f;
                    cellComponent.Position.x += cellComponent.Size / 2f;
                    cellComponent.Position.z += cellComponent.Size / 2f;
                    cellComponent.Edges.x += cellComponent.Size;
                    cellComponent.Edges.z += cellComponent.Size;
                    translation.Value = cellComponent.Position;
                    scale.Value /= 2f;

                    // нагенерить 3 новых ячейки
                    // справа снизу
                    Entity spawned = CommandBufferSplit.Instantiate(index, entity);
                    CellComponent cp = new CellComponent(new float3(cellComponent.Position.x, cellComponent.Position.y, cellComponent.Position.z - cellComponent.Size), cellComponent.Size / 2f);
                    CommandBufferSplit.SetComponent(index, spawned, cp);
                    CommandBufferSplit.SetComponent(index, spawned, new Translation() {Value = cp.Position});
                    CommandBufferSplit.SetComponent(index, spawned, new NonUniformScale() {Value = scale.Value});

                    // слева снизу
                    spawned = CommandBufferSplit.Instantiate(index, entity);
                    cp = new CellComponent(new float3(cellComponent.Position.x - cellComponent.Size, cellComponent.Position.y, cellComponent.Position.z - cellComponent.Size), cellComponent.Size / 2f);
                    CommandBufferSplit.SetComponent(index, spawned, cp);
                    CommandBufferSplit.SetComponent(index, spawned, new Translation() {Value = cp.Position});
                    CommandBufferSplit.SetComponent(index, spawned, new NonUniformScale() {Value = scale.Value});

                    // слева сверху
                    spawned = CommandBufferSplit.Instantiate(index, entity);
                    cp = new CellComponent(new float3(cellComponent.Position.x - cellComponent.Size, cellComponent.Position.y, cellComponent.Position.z), cellComponent.Size / 2f);
                    CommandBufferSplit.SetComponent(index, spawned, cp);
                    CommandBufferSplit.SetComponent(index, spawned, new Translation() {Value = cp.Position});
                    CommandBufferSplit.SetComponent(index, spawned, new NonUniformScale() {Value = scale.Value});
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<float4> borders = new NativeArray<float4>(1, Allocator.Persistent);
            borders[0] = Borders;

            JobHandle splitJobHandle = new SplitJob
            {
                CommandBufferSplit = commandBufferSystem.CreateCommandBuffer().ToConcurrent(),
                Borders = borders
            }.Schedule(this, inputDeps);

            commandBufferSystem.AddJobHandleForProducer(splitJobHandle);

            return splitJobHandle;
        }
    }
}