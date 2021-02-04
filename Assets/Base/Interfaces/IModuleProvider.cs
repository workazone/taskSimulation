using System;

namespace Simulation.Base
{
    public interface IModuleProvider
    {
        IDisposable RegisterControl<T>(object obj, IControlType controlType, IViewType viewType = null, bool bindImmediately = false) where T : IControlType;
        IViewType ResolveViewType<T>() where T : IControlType;
        void BindAllControls();
        void BindControl(Type type);
    }
}