using System;
using System.IO;
using Simulation.Base;
using UnityEngine;

namespace Simulation.Modules
{
    public enum SimStateType
    {
        None,
        Spawning,
        Moving
    }

    public interface ISimStatsType : IControlType
    {
        NotifiableProp<SimStateType> State { get; }
    }

    public interface ISimStataViewData : IViewType
    {

    }

    public class SimStatsData : ISimStatsType
    {
        private NotifiableProp<SimStateType> _state = new NotifiableProp<SimStateType>();

        public NotifiableProp<SimStateType> State
        {
            get { return _state; }
        }

        public Action Activate { get; set; }
    }

    [CreateAssetMenu(fileName = "SimStatsModule", menuName = "ScriptableObjects/SimStatsModule", order = 14)]
    public class SimStatsModule : ModuleOfType<SimStatsControl, ISimStatsType>
    {
    }
}