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
    [UpdateAfter(typeof(CreateUnitSystem))]
    [BurstCompile]
    public class CellRemapSystem : ComponentSystem
    {
        private EntityQuery _cellsGroup;
        private EntityQuery _unitsGroup;

        protected override void OnCreate()
        {
            _cellsGroup = GetEntityQuery(ComponentType.ReadOnly<CellComponent>(), ComponentType.ReadOnly<DirtyTag>());
            _unitsGroup = GetEntityQuery(typeof(UnitComponent), ComponentType.ReadOnly<DirtyTag>());
            RequireForUpdate(_cellsGroup);
            RequireForUpdate(_unitsGroup);
        }

        protected override void OnUpdate()
        {
            Entities.WithAll<DirtyTag, UnitComponent>().ForEach((DynamicBuffer<OverlapElement> overlaps) =>
            {
                overlaps.Clear();
            });

            var getOverlapsData = GetBufferFromEntity<OverlapElement>(false);
            var unitEntities = _unitsGroup.ToEntityArray(Allocator.TempJob);
            var units = _unitsGroup.ToComponentDataArray<UnitComponent>(Allocator.TempJob);

            Entities.WithAll<DirtyTag>().ForEach((Entity entity, ref CellComponent cell, ref Translation translation, ref NonUniformScale scale) =>
            {
                PostUpdateCommands.RemoveComponent<DirtyTag>(entity);

                var cellCenter = new float2((cell.Borders.x + cell.Borders.y) * 0.5f, (cell.Borders.z + cell.Borders.w) * 0.5f);
                var cellHalfSize = new float2(cell.Borders.y - cellCenter.x, cell.Borders.w - cellCenter.y);

                for (int i = 0; i < units.Length; i++)
                {
                    // если есть наложение
                    if (Intersects(units[i], cellCenter, cellHalfSize))
                    {
                        // найти координаты угла ячейки дальнего от центра юнита
                        var fartherCornerCoords = new float2(cellCenter.x >= units[i].Position.x ? cell.Borders.y : cell.Borders.x, cellCenter.y > units[i].Position.y ? cell.Borders.w : cell.Borders.z);
                        fartherCornerCoords.x -= units[i].Position.x;
                        fartherCornerCoords.y -= units[i].Position.y;
                        var farthestDistanceSq = fartherCornerCoords.x * fartherCornerCoords.x + fartherCornerCoords.y * fartherCornerCoords.y;

                        // если ячейка полность внутри юнита - удалить ячейку
                        if (farthestDistanceSq < 4f * units[i].Radius * units[i].Radius)
                        {
                            PostUpdateCommands.AddComponent<DeleteTag>(entity);
                        }
                        // иначе - ячейка частично накладывается на пространство юнита - расщепить ячейку если она еще не слишком мала, если мала - удалить ячейку
                        else
                        {
                            // если ячейка слишком мала чтобы дальше дробиться - удалить ячейку
                            if (scale.Value.x < 0.1f)
                            {
                                PostUpdateCommands.AddComponent<DeleteTag>(entity);
                            }
                            else
                            {
                                PostUpdateCommands.AddComponent<SplitTag>(entity);
                                getOverlapsData[unitEntities[i]].Add(new OverlapElement {HasOverlap = true});
                            }
                        }
                    }
                }
            });

            unitEntities.Dispose();
            units.Dispose();

            Entities.WithAll<DirtyTag, UnitComponent>().ForEach((Entity entity, DynamicBuffer<OverlapElement> overlaps) =>
            {
                if (overlaps.Length == 0)
                    PostUpdateCommands.RemoveComponent<DirtyTag>(entity);
            });
        }

        public static bool Intersects(UnitComponent unit, float2 rectCenter, float2 rectHalfSize)
        {
            var deltaPos = math.abs(unit.Position - rectCenter);

            if (deltaPos.x > (rectHalfSize.x + 2f * unit.Radius)) { return false; }
            if (deltaPos.y > (rectHalfSize.y + 2f * unit.Radius)) { return false; }

            if (deltaPos.x <=rectHalfSize.x) { return true; }
            if (deltaPos.y <= rectHalfSize.y) { return true; }

            var cornerDistance = new float2((deltaPos.x - rectHalfSize.x), (deltaPos.y - rectHalfSize.y));
            var cornerDistanceSq = cornerDistance.x * cornerDistance.x + cornerDistance.y * cornerDistance.y;

            return (cornerDistanceSq <= (4f * unit.Radius * unit.Radius));
        }
    }
}