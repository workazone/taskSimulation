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
using Unity.Rendering;
using UnityEngine;

namespace Simulation.Modules
{
    [UpdateAfter(typeof(SpawnPointsSystem))]
    [BurstCompile]
    public class CreateUnitSystem : ComponentSystem
    {
        public Entity UnitEntityPrefabRed;
        public Entity UnitEntityPrefabBlue;

        protected override void OnUpdate()
        {
            var prefabRed = UnitEntityPrefabRed;
            var prefabBlue = UnitEntityPrefabBlue;

            uint seed = Convert.ToUInt32(UnityEngine.Random.Range(1, 10000));
            var random = new Unity.Mathematics.Random(seed);

            Entities.ForEach((Entity entity, DynamicBuffer<PointPairElement> pairs, ref SpawnPoint point) =>
            {
                if (pairs.Length == 0)
                    PostUpdateCommands.DestroyEntity(entity);
                else
                {
                    var pair = pairs[random.NextInt(0, pairs.Length)];
                    pairs[0] = pair;
                }
            });

            var pointsGroup = GetEntityQuery(typeof(SpawnPoint), ComponentType.ReadOnly<PointPairElement>());
            var randomId = UnityEngine.Random.Range(0, pointsGroup.CalculateEntityCount());

            Entities.ForEach((DynamicBuffer<PointPairElement> pairs, ref SpawnPoint point) =>
            {
                randomId--;
                if (randomId == -1)
                {
                    var unitEntity = PostUpdateCommands.Instantiate(prefabRed);
                    PostUpdateCommands.SetComponent(unitEntity, new Translation() {Value = new float3(point.Position.x, 1.5f, point.Position.y)});
                    PostUpdateCommands.SetComponent(unitEntity, new UnitComponent() {Radius = point.Radius, Position = point.Position});
                    PostUpdateCommands.SetComponent(unitEntity, new NonUniformScale() {Value = point.Radius});
                    PostUpdateCommands.AddComponent<DirtyTag>(unitEntity);
                    PostUpdateCommands.AddBuffer<OverlapElement>(unitEntity);

                    var pairPoint = pairs[0].PairPoint;
                    var pairUnitEntity = PostUpdateCommands.Instantiate(prefabBlue);
                    PostUpdateCommands.SetComponent(pairUnitEntity, new Translation() {Value = new float3(pairPoint.Position.x, 1.5f, pairPoint.Position.y)});
                    PostUpdateCommands.SetComponent(pairUnitEntity, new UnitComponent() {Radius = pairPoint.Radius, Position = pairPoint.Position});
                    PostUpdateCommands.SetComponent(pairUnitEntity, new NonUniformScale() {Value = pairPoint.Radius});
                    PostUpdateCommands.AddComponent<DirtyTag>(pairUnitEntity);
                    PostUpdateCommands.AddBuffer<OverlapElement>(pairUnitEntity);
                }
            });

            Entities.WithAll<SpawnPoint>().ForEach((Entity e) =>
            {
                PostUpdateCommands.DestroyEntity(e);
            });

            Entities.WithNone<DirtyTag>().WithAll<CellComponent>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.AddComponent<DirtyTag>(entity);
            });
        }

        public static Entity SetupUnit(EntityCommandBuffer ecb, Entity entity, float2 position, float radius)
        {
            var unit = ecb.Instantiate(entity);
            ecb.SetComponent(unit, new Translation() {Value = new float3(position.x, 1.5f, position.y)});
            ecb.SetComponent(unit, new UnitComponent() {Radius = radius});
            ecb.AddComponent<DirtyTag>(unit);

            return entity;
        }
    }
}