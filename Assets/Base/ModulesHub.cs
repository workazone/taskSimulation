using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Base
{
    public class ModulesHub : MonoBehaviour
    {
        [SerializeField] private ModuleBase[] _modules;

        public ModuleBase[] Modules
        {
            get { return _modules; }
        }
    }
}