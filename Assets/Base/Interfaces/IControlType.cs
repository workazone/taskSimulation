// интерфейс для данных передаваемых между Control'ами

using System;

namespace Simulation.Base
{
    public interface IControlType
    {
        Action Activate { get; set; }
    }
}