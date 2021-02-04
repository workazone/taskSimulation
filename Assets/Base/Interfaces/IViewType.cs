// базовый интерфейс для обмена данными между Control и его View

using System;

namespace Simulation.Base
{
    public delegate void DisposeHandler();

    public interface IViewType
    {
        event DisposeHandler OnDispose;
    }
}