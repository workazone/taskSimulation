using System;
using System.Collections;
using System.Collections.Generic;
using Simulation.Base;
using UnityEngine;

namespace Simulation.Modules
{
    public interface ICameraType : IControlType
    {
    }

    public interface ICameraViewType : IViewType
    {
        NotifiableProp<Vector2> SimAreaSize { get; }
    }

    [CreateAssetMenu(fileName = "CameraModule", menuName = "ScriptableObjects/CameraModule", order = 13)]
    public class CameraModule : ModuleOfType<CameraControl, ICameraType>
    {
    }
}