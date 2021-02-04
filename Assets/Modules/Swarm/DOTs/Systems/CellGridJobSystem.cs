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
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate()
        {
            commandBufferSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            EntityCommandBuffer.Concurrent commandBufferSplit = commandBufferSystem.CreateCommandBuffer().ToConcurrent();

            NativeArray<float4> borders = new NativeArray<float4>(1, Allocator.Persistent); // x,y,z,w => left, right, bottom, top
            borders[0] = new float4(-39f, 39f, -50f, 50f);

            JobHandle splitJobHandle = Entities.ForEach((int entityInQueryIndex, Entity entity, ref CellComponent cellComponent, ref NonUniformScale scale, ref Translation translation) =>
                {
                    // удалить если ячейка полностью за границей
                    if (cellComponent.Edges.y < borders[0].x || cellComponent.Edges.x > borders[0].y || cellComponent.Edges.w < borders[0].z || cellComponent.Edges.z > borders[0].w)
                    {
                        commandBufferSplit.DestroyEntity(entityInQueryIndex, entity);
                        return;
                    }

                    if (cellComponent.Size < 0.1f)
                        return;

                    // раздробить ячейку на 4 дочерние ячейки если она частично выходит за границы
                    if (cellComponent.Edges.x < borders[0].x || cellComponent.Edges.y > borders[0].y || cellComponent.Edges.z < borders[0].z || cellComponent.Edges.w > borders[0].w)
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
                        Entity spawned = commandBufferSplit.Instantiate(entityInQueryIndex, entity);
                        CellComponent cp = new CellComponent(new float3(cellComponent.Position.x, cellComponent.Position.y, cellComponent.Position.z - cellComponent.Size), cellComponent.Size / 2f);
                        commandBufferSplit.SetComponent(entityInQueryIndex, spawned, cp);
                        commandBufferSplit.SetComponent(entityInQueryIndex, spawned, new Translation() {Value = cp.Position});
                        commandBufferSplit.SetComponent(entityInQueryIndex, spawned, new NonUniformScale() {Value = scale.Value});

                        // слева снизу
                        spawned = commandBufferSplit.Instantiate(entityInQueryIndex, entity);
                        cp = new CellComponent(new float3(cellComponent.Position.x - cellComponent.Size, cellComponent.Position.y, cellComponent.Position.z - cellComponent.Size), cellComponent.Size / 2f);
                        commandBufferSplit.SetComponent(entityInQueryIndex, spawned, cp);
                        commandBufferSplit.SetComponent(entityInQueryIndex, spawned, new Translation() {Value = cp.Position});
                        commandBufferSplit.SetComponent(entityInQueryIndex, spawned, new NonUniformScale() {Value = scale.Value});

                        // слева сверху
                        spawned = commandBufferSplit.Instantiate(entityInQueryIndex, entity);
                        cp = new CellComponent(new float3(cellComponent.Position.x - cellComponent.Size, cellComponent.Position.y, cellComponent.Position.z), cellComponent.Size / 2f);
                        commandBufferSplit.SetComponent(entityInQueryIndex, spawned, cp);
                        commandBufferSplit.SetComponent(entityInQueryIndex, spawned, new Translation() {Value = cp.Position});
                        commandBufferSplit.SetComponent(entityInQueryIndex, spawned, new NonUniformScale() {Value = scale.Value});
                    }
                })
                .WithBurst()
                .WithDeallocateOnJobCompletion(borders)
                .WithName("SplittingCells")
                .Schedule(inputDeps);

            commandBufferSystem.AddJobHandleForProducer(splitJobHandle);

            return splitJobHandle;
        }
    }
}