using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Simulation.Base;
using UnityEngine;

namespace Simulation.Modules
{
    public interface IEnvironmentType : IControlType
    {
    }

    public interface IEnvironmentViewType : IViewType
    {
        NotifiableProp<Vector2> SimAreaSize { get; }
    }

    public class EnvironmentData : IEnvironmentType
    {
        public Action Activate { get; set; }
    }

    [CreateAssetMenu(fileName = "EnvironmentModule", menuName = "ScriptableObjects/EnvironmentModule", order = 12)]
    public class EnvironmentModule : ModuleOfType<EnvironmentControl, IEnvironmentType>
    {
    }
}