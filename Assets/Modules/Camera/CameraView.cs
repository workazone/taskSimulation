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
            ViewData.SimAreaSize.OnChanged += SetCameraDistance;
            _ready = true;
        }

        private void SetCameraDistance(Vector2 size)
        {
            // TODO: переделать чтобы камера вращалась на тап и подстравивалась под размер на экране
            var camLocPos = _cameraTrans.localPosition;
            camLocPos.z = -Mathf.Max(size.x, size.y);
            _cameraTrans.localPosition = camLocPos;
        }

        private void Update()
        {
            if (_ready)
                transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}