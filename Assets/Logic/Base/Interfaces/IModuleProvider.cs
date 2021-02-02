using System;

namespace Simulation
{
    public interface IModuleProvider
    {
        IDisposable Register<T>(Func<object> func, bool force = false);
        object Resolve(Type type);
        bool TryResolve(Type type, out object obj);
        void Complete();
    }
}