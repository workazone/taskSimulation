using System;
using Simulation.Base;

namespace Simulation.Base
{
    public interface IBindableControl<T> : IDisposable where T : class, IControlType
    {
        bool TryRegisterTo(IModuleProvider provider);
    }
}