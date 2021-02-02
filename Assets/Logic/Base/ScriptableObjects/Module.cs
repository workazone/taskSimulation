using System.Threading.Tasks;
using UnityEngine;
using System;

namespace Simulation
{
    public abstract class Module : ScriptableObject
    {
        protected bool _ready;
        public bool Ready
        {
            get { return _ready; }
        }

        public abstract void RegisterTo(IModuleProvider provider);

        public void InitialSetupOnScene()
        {
            if(_ready)
                return;

            try
            {
                SetupOnScene();
                _ready = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        protected abstract void SetupOnScene();
    }
}