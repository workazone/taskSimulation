using System;
using System.IO;
using Simulation.Base;
using UnityEngine;

namespace Simulation.Modules
{
    public enum SimState
    {
        None,
        Warming,
        Idle,
        Generation,
        Simualtion
    }

    public struct SimStatsData
    {
        public SimState Current;
        public SimState Previous;

        public SimStatsData(SimState current, SimState previous)
        {
            Current = current;
            Previous = previous;
        }
    }

    public interface ISimStatsType : IControlType
    {
        NotifiableProp<SimStatsData> Data { get; }
    }

    [CreateAssetMenu(fileName = "SimStatsModule", menuName = "ScriptableObjects/SimStatsModule", order = 14)]
    public class SimStatsModule : ModuleOfType<SimStatsControl, ISimStatsType>
    {
    }
}