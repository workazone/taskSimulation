using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Simulation
{
    public class ModuleProvider : IModuleProvider
    {
        private Dictionary<Type, object> _modules = new Dictionary<Type, object>();

        public IDisposable Register<T>(Func<object> func, bool force = false)
        {
            if (_modules.ContainsKey(typeof(T)))
            {
                if (force)
                    _modules.Remove(typeof(T));
                else
                {
                    Debug.LogError($"module of type {(typeof(T)).Name} already registered");
                    return null;
                }
            }

            _modules[typeof(T)] = func();

            return new Unregister<T>(_modules, typeof(T));
        }

        public object Resolve(Type type)
        {
            if (_modules.ContainsKey(type))
                return _modules[type];

            throw new InvalidOperationException($"Not registered module of type: {type}");
        }

        public bool TryResolve(Type type, out object obj)
        {
            if (_modules.TryGetValue(type, out object value))
            {
                obj = value;
                return true;
            }

            Debug.LogError($"Unresolved module of type: {type.Name}");
            obj = null;
            return false;
        }

        public void Complete()
        {
            foreach (var module in _modules)
            {
                var init = module.Value.GetType().GetMethod("Init");

                if(init == null)
                    continue;

                var dependencies = init.GetParameters().Select(t =>
                {
                    TryResolve(t.ParameterType, out var obj);
                    return obj;
                });

                init.Invoke(module.Value, dependencies.ToArray());
            }
        }
    }

    public class Unregister<T> : IDisposable
    {
        private readonly Dictionary<Type, object> _modules;
        private readonly Type _module;

        internal Unregister(Dictionary<Type, object> modules, Type module)
        {
            _modules = modules;
            _module = module;
        }

        public void Dispose()
        {
            if (_modules.ContainsKey(_module))
                _modules.Remove(_module);
        }
    }
}