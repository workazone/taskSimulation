using System;
using System.Collections;
using System.Collections.Generic;
using Simulation.Base;
using UnityEngine;

namespace Simulation.Base
{
    public class ModuleOfType<TB, TC> : ModuleBase
        where TB : class, IBindableControl<TC>, new()
        where TC : class, IControlType
    {
        [SerializeField] protected GameObject _viewPrefab;

        public override void Init(IModuleProvider provider)
        {
            CreateControl(provider);

            if (_viewPrefab != null)
                CreateView(provider);
        }

        protected void CreateControl(IModuleProvider provider)
        {
            var control = new TB();

            if(!control.TryRegisterTo(provider))
                control.Dispose();
        }

        protected void CreateView(IModuleProvider provider)
        {
            var view = Instantiate(_viewPrefab).GetComponentInChildren<IBindableView>();
            if (view == null)
                throw new ArgumentNullException(nameof(IBindableView));

            view.RegisterTo(provider);
        }
    }
}