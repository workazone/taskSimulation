﻿using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

namespace Simulation.Modules
{
    [GenerateAuthoringComponent]
    // [InternalBufferCapacity(16)]         // optional
    public struct OverlapElement : IBufferElementData
    {
        public bool HasOverlap;
    }
}