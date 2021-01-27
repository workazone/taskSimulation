using System;

namespace Simulation
{
    public interface IServiceProvider
    {
        IDisposable Register<T>(Func<object> func, bool force = false);
        bool TryResolve<T>(out Func<object> func);
    }
}