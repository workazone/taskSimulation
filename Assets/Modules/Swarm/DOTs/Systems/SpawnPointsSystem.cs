using System;
using System.Diagnostics;
using System.Drawing;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

namespace Simulation.Modules
{
    [UpdateAfter(typeof(SpawnPointsSystem))]
    [BurstCompile]
    public class SpawnPointsSystem : JobComponentSystem
    {
        public float4 Borders;
        public float UnitRadius = default;

        private EntityQuery _cellsGroup;
        private EntityQuery _pointsGroup;
        private NativeArray<Unity.Mathematics.Random> _randomArray;

        protected override void OnCreate()
        {
            _cellsGroup = GetEntityQuery(ComponentType.ReadOnly<CellComponent>());
            _pointsGroup = GetEntityQuery(typeof(SpawnPoint));
            RequireForUpdate(_cellsGroup);
            RequireForUpdate(_pointsGroup);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var borders = Borders;
            var radius = UnitRadius;
            var seed = System.Convert.ToUInt32(UnityEngine.Random.Range(1, 10000));
            var randomSeed = new Unity.Mathematics.Random(seed);
            var randomArray = World.GetExistingSystem<RandomSystem>().RandomArray;

            var prePositionHandle = Entities.ForEach((ref SpawnPoint point) =>
                {
                    point.Radius = radius;
                    point.Position = new float2(borders.x + (borders.y - borders.x) * randomSeed.NextFloat(), borders.z + (borders.w - borders.z) * randomSeed.NextFloat());
                })
                .WithBurst()
                .WithName("PrePositionSpawnPoints")
                .Schedule(inputDeps);

            var cellComponents = _cellsGroup.ToComponentDataArray<CellComponent>(Allocator.Persistent);

            // borders x,y,z,w => left, right, bottom, top
            var positionHandle = Entities
                .WithNativeDisableParallelForRestriction(randomArray)
                .ForEach((int nativeThreadIndex, ref SpawnPoint point) =>
                {
                    var closestCellBorders = float4.zero;
                    var closestDistanceSq = 500000000f;

                    for (int i = 0; i < cellComponents.Length; i++)
                    {
                        float2 deltaPos = float2.zero;

                        if (point.Position.x < cellComponents[i].Borders.x)
                            deltaPos.x = point.Position.x - cellComponents[i].Borders.x;
                        else if (point.Position.x > cellComponents[i].Borders.y)
                            deltaPos.x = point.Position.x - cellComponents[i].Borders.y;

                        if (point.Position.y < cellComponents[i].Borders.z)
                            deltaPos.y = point.Position.y - cellComponents[i].Borders.z;
                        else if (point.Position.y > cellComponents[i].Borders.w)
                            deltaPos.y = point.Position.y - cellComponents[i].Borders.w;

                        // если point попал внутрь данного cell - выбрать его и остановить поиск
                        if (deltaPos.x == 0f && deltaPos.y == 0f)
                        {
                            closestCellBorders = cellComponents[i].Borders;
                            break;
                        }

                        // если поинт снаружи - то рассчитать квадратичное расстояние для сравнения
                        var distSq = deltaPos.x * deltaPos.x + deltaPos.y * deltaPos.y;
                        if (distSq < closestDistanceSq)
                        {
                            closestDistanceSq = distSq;
                            closestCellBorders = cellComponents[i].Borders;
                        }
                    }

                    var random = randomArray[nativeThreadIndex];

                    point.Position.x = random.NextFloat(closestCellBorders.x, closestCellBorders.y);
                    point.Position.y = random.NextFloat(closestCellBorders.z, closestCellBorders.w);

                    randomArray[nativeThreadIndex] = random;
                })
                .WithBurst()
                .WithName("PositionSpawnPointsToCells")
                .WithDeallocateOnJobCompletion(cellComponents)
                .Schedule(prePositionHandle);

            var points = _pointsGroup.ToComponentDataArray<SpawnPoint>(Allocator.TempJob);

            var pairingHandle = Entities.ForEach((int entityInQueryIndex, Entity entity, ref DynamicBuffer<PointPairElement> pairs, ref SpawnPoint point) =>
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        var deltaPos = point.Position - points[i].Position;
                        var distSq = deltaPos.x * deltaPos.x + deltaPos.y * deltaPos.y;

                        if (distSq >= radius * radius)
                            pairs.Add(new PointPairElement {PairPoint = points[i]});
                    }
                })
                .WithBurst()
                .WithName("PairSpawnPoints")
                .WithDeallocateOnJobCompletion(points)
                .Schedule(positionHandle);

            return pairingHandle;
        }
    }
}