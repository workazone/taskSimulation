using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Simulation.Base
{
    public struct ControlRegistration
    {
        public Type Type;
        public object Object;
        public IControlType ControlType;
        public IViewType ViewType;

        public ControlRegistration(Type type, object o, IControlType controlType, IViewType viewType)
        {
            Type = type;
            Object = o;
            ControlType = controlType;
            ViewType = viewType;
        }
    }

    public class ModuleProvider : IModuleProvider
    {
        private Dictionary<Type, ControlRegistration> _registrations = new Dictionary<Type, ControlRegistration>();

        public IDisposable RegisterControl<T>(object obj, IControlType controlType, IViewType viewType = null, bool bindImmediately = false) where T : IControlType
        {
            if (_registrations.ContainsKey(typeof(T)))
            {
                Debug.LogError($"control of type {(typeof(T)).Name} already registered");
                return null;
            }

            _registrations[typeof(T)] = new ControlRegistration
            {
                Object = obj,
                Type = typeof(T),
                ControlType = controlType,
                ViewType = viewType
            };

            if(bindImmediately)
                BindControl(typeof(T));

            return new UnregisterControl<T>(_registrations, typeof(T));
        }

        public void BindAllControls()
        {
            foreach (var control in _registrations)
            {
                BindControl(control.Key);
            }
        }

        public void BindControl(Type type)
        {
            var reg = ResolveRegistration(type);

            var bindMethod = reg.Object.GetType().GetMethod("Bind");

            if (bindMethod == null)
                return;

            var dependencies = bindMethod.GetParameters().Select(t =>  ResolveControlType(t.ParameterType));

            bindMethod.Invoke(reg.Object, dependencies.ToArray());
        }

        public IViewType ResolveViewType<T>() where T : IControlType
        {
            return ResolveViewType(typeof(T));
        }

        private IControlType ResolveControlType(Type type)
        {
            if (_registrations.ContainsKey(type))
                return _registrations[type].ControlType;

            throw new InvalidOperationException($"Unknown control of type: {type}");
        }

        private IViewType ResolveViewType(Type type)
        {
            if (_registrations.ContainsKey(type))
                return _registrations[type].ViewType;

            throw new InvalidOperationException($"Unknown control of type: {type}");
        }

        private object ResolveObject(Type type)
        {
            if (_registrations.ContainsKey(type))
                return _registrations[type].Object;

            throw new InvalidOperationException($"Unknown control of type: {type}");
        }

        private ControlRegistration ResolveRegistration(Type type)
        {
            if (_registrations.ContainsKey(type))
                return _registrations[type];

            throw new InvalidOperationException($"Unknown control of type: {type}");
        }
    }

    public class UnregisterControl<T> : IDisposable
    {
        private readonly Dictionary<Type, ControlRegistration> _controls;
        private readonly Type _type;

        internal UnregisterControl(Dictionary<Type, ControlRegistration> controls, Type type)
        {
            _controls = controls;
            _type = type;
        }

        public void Dispose()
        {
            if (_controls.ContainsKey(_type))
            {
                _controls.Remove(_type);
            }
        }
    }
}