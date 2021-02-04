using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Base
{
    public abstract class ViewBase<TC, TV> : MonoBehaviour, IBindableView
        where TC : IControlType
        where TV : IViewType
    {
        protected TV ViewData;

        public void RegisterTo(IModuleProvider provider)
        {
            if (provider.ResolveViewType<TC>() is TV vd)
                ViewData = vd;

            ViewData.OnDispose += Dispose;
            StartView();
        }

        protected abstract void StartView();

        public virtual void Dispose()
        {
            Destroy(gameObject);
        }
    }
}