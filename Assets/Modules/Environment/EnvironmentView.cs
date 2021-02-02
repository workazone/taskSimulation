using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Modules
{
    public class EnvironmentView : MonoBehaviour
    {
        [SerializeField] private Module _module;
        [SerializeField] private SkinnedMeshRenderer _SMeshRenderer;
        [SerializeField] private Transform[] _areaBones;

        private IEnvironment _control;
        private SizeData _data;

        private void Awake()
        {
            _control = (IEnvironment) _module;
            _control?.Data.Subscribe(d => Redraw(d));
        }

        private void Redraw(SizeData data)
        {
            _data = data;

            _areaBones[0].localPosition = new Vector3(-_data.Width / 2f, 0f, -_data.Height / 2f);
            _areaBones[1].localPosition = new Vector3(-_data.Width / 2f, 0f, _data.Height / 2f);
            _areaBones[2].localPosition = -_areaBones[0].localPosition;
            _areaBones[3].localPosition = -_areaBones[1].localPosition;

            _SMeshRenderer.localBounds = new Bounds(_SMeshRenderer.localBounds.center, new Vector3(_data.Width, 1f, _data.Height));
        }
    }
}