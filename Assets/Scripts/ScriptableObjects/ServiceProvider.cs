using System.Collections.Generic;
using UnityEngine;
using System;

namespace Simulation
{
    [CreateAssetMenu(fileName = "ServiceProvider", menuName = "ScriptableObjects/ServiceProvider", order = 1)]
    public class ServiceProvider : ScriptableObject, IServiceProvider
    {
        private Dictionary<Type, Func<object>> services = new Dictionary<Type, Func<object>>();

        public IDisposable Register<T>(Func<object> func, bool force = false)
        {
            if (services.ContainsKey(typeof(T)))
            {
                if (force)
                    services.Remove(typeof(T));
                else
                {
                    Debug.LogError($"manager of type {(typeof(T)).Name} already registered");
                    return null;
                }
            }

            services[typeof(T)] = func;

            return new ServiceUnregister<T>(services, typeof(T));
        }

        public bool TryResolve<T>(out Func<object> func)
        {
            if (services.TryGetValue(typeof(T), out Func<object> service))
            {
                func = service;
                return true;
            }

            func = null;
            return false;
        }
    }
    
    public class ServiceUnregister<T> : IDisposable
    {
        private readonly Dictionary<Type, Func<object>> _services;
        private readonly Type _service;

        internal ServiceUnregister(Dictionary<Type, Func<object>> services, Type service)
        {
            _services = services;
            _service = service;
        }

        public void Dispose()
        {
            if (_services.ContainsKey(_service))
                _services.Remove(_service);
        }
    }
}