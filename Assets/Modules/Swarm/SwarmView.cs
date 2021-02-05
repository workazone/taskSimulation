using System;
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
        private CellGridJobSystem _cellGridJobSystem;
        private Entity _unitEntityPrefab;
        private Entity _cellEntityPrefab;
        private SimStateType _state = SimStateType.None;
        private bool _setupComplete = default;
        private int _createdCount = 0;

        protected override void StartView()
        {
            ViewData.Config.OnChanged += cfg => _config = cfg;
            ViewData.State.OnChanged += state => _state = state;
        }

        private void Update()
        {
            switch (_state)
            {
                case SimStateType.Setup:
                    SetupSystems();
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
            if(_setupComplete)
                return;

            var radius = UnityEngine.Random.Range(_config.unitSpawnMinRadius, _config.unitSpawnMaxRadius);
            var speed = UnityEngine.Random.Range(_config.unitSpawnMinSpeed, _config.unitSpawnMaxSpeed);

            var width = _config.gameAreaWidth - 2f * radius;
            var height = _config.gameAreaHeight - 2f * radius;

            var maxSide = Mathf.Max(width, height);

            // system
            _cellGridJobSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<CellGridJobSystem>();
            _cellGridJobSystem.Borders = new float4(-width / 2f, width / 2f, -height / 2f, height / 2f);

            //
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            _cellEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_cellPrefab, settings);
            _unitEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_unitPrefab, settings);

            //
            var entity = _entityManager.Instantiate(_cellEntityPrefab);
            _entityManager.SetComponentData(entity, new Translation {Value = new float3(0f, 0.7f, 0f)});
            _entityManager.SetComponentData(entity, new Rotation {Value = quaternion.identity});
            _entityManager.SetComponentData(entity, new NonUniformScale() {Value = maxSide / 2f});
            _entityManager.SetComponentData(entity, new CellComponent(float3.zero, maxSide / 2f));

            _setupComplete = true;

            ViewData.State.Value = SimStateType.Spawning;
        }

        private void ProcessSpawning()
        {
        }

        private void ProcessMoving()
        {
        }
    }
}