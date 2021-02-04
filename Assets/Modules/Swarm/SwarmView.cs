using System.Collections;
using System.Collections.Generic;
using Simulation.Base;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using Unity.Transforms;

namespace Simulation.Modules
{
    public class SwarmView : ViewBase<ISwarmType, ISwarmViewType>
    {
        [SerializeField] private GameObject _unitPrefab;
        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private CellGridJobSystem _gridJobSystem;

        private SimConfig _config;
        private EntityManager _entityManager;
        private Entity _unitEntityPrefab;
        private Entity _cellEntityPrefab;

        protected override void StartView()
        {
            ViewData.Config.OnChanged += StartSpawning;
        }

        private void StartSpawning(SimConfig config)
        {
            // _config = config;

            var radius = UnityEngine.Random.Range(config.unitSpawnMinRadius, config.unitSpawnMaxRadius);
            var speed = UnityEngine.Random.Range(config.unitSpawnMinSpeed, config.unitSpawnMaxSpeed);

            var width = config.gameAreaWidth - 2f * radius;
            var height = config.gameAreaHeight - 2f * radius;

            var maxSide = Mathf.Max(width, height);

            //
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            _cellEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_cellPrefab, settings);

            //
            var entities = new NativeArray<Entity>(1, Allocator.Temp);
            entities[0] = _entityManager.Instantiate(_cellEntityPrefab);
            _entityManager.SetComponentData(entities[0], new Translation {Value = new float3(0f, 0.7f, 0f)});
            _entityManager.SetComponentData(entities[0], new Rotation {Value = quaternion.identity});
            _entityManager.SetComponentData(entities[0], new NonUniformScale() {Value = maxSide / 2f});
            _entityManager.SetComponentData(entities[0], new CellComponent (float3.zero, maxSide / 2f));
            entities.Dispose();
        }
    }
}