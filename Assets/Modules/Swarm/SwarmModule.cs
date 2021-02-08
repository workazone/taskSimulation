using System;
using System.Collections;
using System.Collections.Generic;
using Simulation.Base;
using UnityEngine;

namespace Simulation.Modules
{
    public interface ISwarmType : IControlType
    {
    }

    public interface ISwarmViewType : IViewType
    {
        NotifiableProp<SimConfig> ViewConfig { get; }
        NotifiableProp<SimStateType> ViewState { get; }
    }

    public class SwarmData : ISwarmType
    {
    }

    [CreateAssetMenu(fileName = "SwarmModule", menuName = "ScriptableObjects/SwarmModule", order = 15)]
    public class SwarmModule : ModuleOfType<SwarmControl, ISwarmType>
    {
    }
}