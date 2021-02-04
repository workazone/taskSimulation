using System;

namespace Simulation.Base
{
    public interface IBindableView : IDisposable
    {
        void RegisterTo(IModuleProvider provider);
    }
}