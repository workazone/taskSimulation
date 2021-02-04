using System.Threading.Tasks;
using UnityEngine;
using System;

namespace Simulation.Base
{
    public abstract class ModuleBase : ScriptableObject
    {
        public abstract void Init(IModuleProvider provider);
    }
}