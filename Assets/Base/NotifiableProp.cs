using System;

namespace Simulation.Base
{
    public class NotifiableProp<T>
    {
        public delegate void OnChangedEventHandler(T value);

        protected T _value;
        protected event OnChangedEventHandler _onChanged;
        protected object objectLock = new Object();

        public event OnChangedEventHandler OnChanged
        {
            add
            {
                lock (objectLock)
                {
                    _onChanged += value;

                    value.Invoke(_value);
                }
            }
            remove
            {
                lock (objectLock)
                {
                    _onChanged -= value;
                }
            }
        }

        public T Value
        {
            get { return _value; }

            set
            {
                if (!_value.Equals(value))
                {
                    _value = value;
                    _onChanged?.Invoke(_value);
                }
            }
        }

        public NotifiableProp(T value = default(T))
        {
            _value = value;
        }

        public void RaiseChanged()
        {
            _onChanged?.Invoke(_value);
        }
    }
}