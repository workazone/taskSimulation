using System;
using System.IO;
using Simulation.Base;
using UnityEngine;

namespace Simulation.Modules
{
    public interface ISimConfigType : IControlType
    {
        NotifiableProp<SimConfig> Config { get; }
    }

    public class SimConfigData : ISimConfigType
    {
        private NotifiableProp<SimConfig> _config = new NotifiableProp<SimConfig>();

        public NotifiableProp<SimConfig> Config
        {
            get { return _config; }
        }

        public Action Activate { get; set; }
    }

    [CreateAssetMenu(fileName = "SimConfigModule", menuName = "ScriptableObjects/SimConfigModule", order = 11)]
    public class SimConfigModule : ModuleOfType<SimConfigControl, ISimConfigType>
    {
    }
}