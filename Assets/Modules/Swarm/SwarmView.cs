using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private GameObject _unitPrefabRed;
        [SerializeField] private GameObject _unitPrefabBlue;
        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private CellSplitSystem _splitSystem;

        private SimConfig _config;
        private EntityManager _entityManager;
        private CellSplitSystem _cellSplitSystem;
        private CellDestroySystem _cellDestroyJobSystem;
        private CellFitBordersSystem _cellFitBordersSystem;
        private SpawnPointsSystem _spawnPointsSystem;
        private CreateUnitSystem _createUnitSystem;
        private Entity _unitEntityPrefabRed;
        private Entity _unitEntityPrefabBlue;
        private Entity _cellEntityPrefab;
        private SimStateType _state = SimStateType.None;
        private EntityArchetype _spawnPointArchetype;
        private int _fitGridEmptyFrames = default;
        private float _unitRadius = default;
        private float _spawnTimer = default;
        private int _spawnCount = default;
        private bool _setupComplete = default;
        private bool _spawningComplete = default;
        private int _createdCount = 0;

        protected override void StartView()
        {
            ViewData.ViewConfig.OnChanged += cfg => _config = cfg;
            ViewData.ViewState.OnChanged += state => _state = state;
        }

        private void Update()
        {
            switch (_state)
            {
                case SimStateType.Setup:
                    SetupSystems();
                    _fitGridEmptyFrames += _cellFitBordersSystem.ShouldRunSystem() ? -_fitGridEmptyFrames : 1;
                    if (_fitGridEmptyFrames > 3)
                    {
                        Debug.Log($"arrangement compelete!");
                        ViewData.ViewState.Value = SimStateType.Spawning;
                        _cellFitBordersSystem.Enabled = false;
                    }

                    break;
                case SimStateType.Spawning:
                    ProcessSpawning();
                    break;
                case SimStateType.Moving:
                    ProcessMoving();
                    break;
                default:
                    break;
            }
        }

        private void SetupSystems()
        {
            if (_setupComplete)
                return;

            // values
            _unitRadius = UnityEngine.Random.Range(_config.unitSpawnMinRadius, _config.unitSpawnMaxRadius);
            var speed = UnityEngine.Random.Range(_config.unitSpawnMinSpeed, _config.unitSpawnMaxSpeed);

            var width = _config.gameAreaWidth - 2f * _unitRadius;
            var height = _config.gameAreaHeight - 2f * _unitRadius;

            var maxSide = Mathf.Max(width, height);

            var borders = new float4(-width / 2f, width / 2f, -height / 2f, height / 2f);

            // manager
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            // prefabs
            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            _cellEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_cellPrefab, settings);
            _unitEntityPrefabRed = GameObjectConversionUtility.ConvertGameObjectHierarchy(_unitPrefabRed, settings);
            _unitEntityPrefabBlue = GameObjectConversionUtility.ConvertGameObjectHierarchy(_unitPrefabBlue, settings);

            // systems
            _cellFitBordersSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<CellFitBordersSystem>();
            _cellSplitSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<CellSplitSystem>();
            _cellDestroyJobSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<CellDestroySystem>();
            _spawnPointsSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SpawnPointsSystem>();
            _createUnitSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<CreateUnitSystem>();

            _cellFitBordersSystem.Borders = borders;
            _spawnPointsSystem.Borders = borders;
            _spawnPointsSystem.UnitRadius = _unitRadius;
            _createUnitSystem.UnitEntityPrefabRed = _unitEntityPrefabRed;
            _createUnitSystem.UnitEntityPrefabBlue = _unitEntityPrefabBlue;

            // archetypes
            _spawnPointArchetype = _entityManager.CreateArchetype(
                typeof(Translation),
                typeof(SpawnPoint),
                typeof(PointPairElement));

            //
            var entity = _entityManager.Instantiate(_cellEntityPrefab);
            _entityManager.SetComponentData(entity, new Translation {Value = new float3(0f, 0.7f, 0f)});
            _entityManager.SetComponentData(entity, new Rotation {Value = quaternion.identity});
            _entityManager.SetComponentData(entity, new NonUniformScale() {Value = maxSide / 2f});
            _entityManager.SetComponentData(entity, new CellComponent(new float3(0f, 0.7f, 0f), maxSide / 2f));

            _setupComplete = true;
        }

        private void ProcessSpawning()
        {
            if (_spawningComplete)
                return;

            _spawnTimer += Time.deltaTime;
            if (_spawnTimer < _config.unitSpawnDelay)
                return;

            _spawnTimer -= _config.unitSpawnDelay * 0.5f;
            _spawnCount += 2;

            if (_spawnCount > _config.numUnitsToSpawn)
            {
                _spawningComplete = true;
                Debug.Log($"spawning complete, created {_spawnCount} units");
            }
            else
                _entityManager.CreateEntity(_spawnPointArchetype, 16, Allocator.Temp);
        }

        private void ProcessMoving()
        {
        }
    }
}