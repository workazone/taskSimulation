using System;
using System.Collections;
using System.Collections.Generic;
using Simulation.Modules;
using UnityEngine;

namespace Simulation.Base
{
    public class CameraView : ViewBase<ICameraType, ICameraViewType>
    {
        [SerializeField] private Transform _cameraTrans;
        [SerializeField] private float _rotationSpeed;

        private bool _ready;

        protected override void StartView()
        {
            _ready = true;
        }

        private void Update()
        {
            if (_ready)
                transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}