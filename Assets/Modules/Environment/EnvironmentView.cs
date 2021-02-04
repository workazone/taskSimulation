using System;
using System.Collections;
using System.Collections.Generic;
using Simulation.Base;
using UnityEngine;

namespace Simulation.Modules
{
    public class EnvironmentView : ViewBase<IEnvironmentType, IEnvironmentViewType>
    {
        [SerializeField] private SkinnedMeshRenderer _meshRenderer;
        [SerializeField] private Transform[] _areaBones;

        protected override void StartView()
        {
            ViewData.SimAreaSize.OnChanged += Redraw;
        }

        private void Redraw(Vector2 size)
        {
            _areaBones[0].localPosition = new Vector3(-size.x / 2f, 0f, -size.y / 2f);
            _areaBones[1].localPosition = new Vector3(-size.x / 2f, 0f, size.y / 2f);
            _areaBones[2].localPosition = -_areaBones[0].localPosition;
            _areaBones[3].localPosition = -_areaBones[1].localPosition;

            _meshRenderer.localBounds = new Bounds(_meshRenderer.localBounds.center, new Vector3(size.x, 1f, size.y));
        }
    }
}