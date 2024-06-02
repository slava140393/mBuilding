using System;
using System.Collections.Generic;

namespace mBuilding.DI
{
    public class DIContainer
    {
        private readonly DIContainer _parentContainer;
        private readonly Dictionary<(string, Type), DIRegistrations> _registrations = new();
        private readonly HashSet<(string, Type)> _resolutions = new();
        public DIContainer(DIContainer parentContainer=null)
        {
            _parentContainer = parentContainer;
        }

        public void RegisterSingleton<T>(Func<DIContainer, T> factory)
        {
            RegisterSingleton(null, factory);
        }
        public void RegisterSingleton<T>(string tag, Func<DIContainer, T> factory)
        {
            var key = (tag, typeof(T));
            Register(key, factory, true);
        }
        public void RegisterTransient<T>(Func<DIContainer, T> factory)
        {
            RegisterTransient(null, factory);
        }
        public void RegisterTransient<T>(string tag, Func<DIContainer, T> factory)
        {
            var key = (tag, typeof(T));
            Register(key, factory, false);
        }

        public void RegisterInstance<T>(T instance)
        {
            RegisterInstance(null, instance);
        }
        public void RegisterInstance<T>(string tag, T instance)
        {
            var key = (tag, typeof(T));
            if (_registrations.ContainsKey(key))
            {
                throw new Exception($"DI: Factory with tag {key.Item1} and type {key.Item2.FullName} has already registered");
            }

            _registrations[key] = new DIRegistrations()
            {
                Instance = instance,
                IsSingleton = true,
            };
        }

        public T Resolve<T>(string tag = null)
        {
            var key = (tag, typeof(T));
            if (_resolutions.Contains(key))
            {
                throw new Exception($"Cyclic dependency for tag {tag} and type {key.Item2.FullName}");
            }
            _resolutions.Add(key);

            try// Try для упрощения т.е. finally будет выполняться при каждом return, вместо того чтобы добавить перед каждым Remove
            {
                if (_registrations.TryGetValue(key, out DIRegistrations registration))
                {
                    if (registration.IsSingleton)
                    {
                        if (registration.Instance == null && registration.Factory != null)
                        {
                            registration.Instance = registration.Factory(this); 
                            //Func<DIContainer, object> - делегат возвращающий объект указанного типа. Прим: Func<string, string> selector = str => str.ToUpper(); "orange" -> ORANGE
                            // Сам Instance  создается только в момент запроса
                        }
                        return (T) registration.Instance;
                    }
                    return (T) registration.Factory(this);
                }
                if (_parentContainer != null)
                {
                    return _parentContainer.Resolve<T>(tag);
                }
            }
            finally
            {
                _resolutions.Remove(key);
            }
            throw new Exception($"DI: Couldn't find dependency for tag {tag} and type {key.Item2.FullName}");
        }
        private void Register<T>((string, Type) key, Func<DIContainer, T> factory, bool isSingleton)
        {
            if (_registrations.ContainsKey(key))
            {
                throw new Exception($"DI: Factory with tag {key.Item1} and type {key.Item2.FullName} has already registered");
            }

            _registrations[key] = new DIRegistrations()
            {
                Factory = c => factory(c),
                IsSingleton = isSingleton,
            };
        }
    }

}