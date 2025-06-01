using System;
using System.Collections.Generic;

namespace WinFormsApp1.Managers
{
    /// <summary>
    /// Simple dependency injection container for game services
    /// </summary>
    public class GameServiceContainer
    {
        private readonly Dictionary<Type, object> _singletonServices = new();
        private readonly Dictionary<Type, Func<object>> _transientFactories = new();
        private readonly object _lockObject = new object();

        /// <summary>
        /// Register a singleton service instance
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="instance">Service instance</param>
        public void RegisterSingleton<T>(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            lock (_lockObject)
            {
                _singletonServices[typeof(T)] = instance;
            }
        }

        /// <summary>
        /// Register a singleton service with a factory function
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="factory">Factory function to create the service</param>
        public void RegisterSingleton<T>(Func<T> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            lock (_lockObject)
            {
                if (!_singletonServices.ContainsKey(typeof(T)))
                {
                    _singletonServices[typeof(T)] = factory();
                }
            }
        }

        /// <summary>
        /// Register a transient service with a factory function
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="factory">Factory function to create the service</param>
        public void RegisterTransient<T>(Func<T> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            lock (_lockObject)
            {
                _transientFactories[typeof(T)] = () => factory();
            }
        }

        /// <summary>
        /// Get a service instance
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>Service instance</returns>
        public T GetService<T>()
        {
            var serviceType = typeof(T);

            lock (_lockObject)
            {
                // Check for singleton first
                if (_singletonServices.ContainsKey(serviceType))
                {
                    return (T)_singletonServices[serviceType];
                }

                // Check for transient factory
                if (_transientFactories.ContainsKey(serviceType))
                {
                    return (T)_transientFactories[serviceType]();
                }
            }

            throw new InvalidOperationException($"Service of type {serviceType.Name} is not registered");
        }

        /// <summary>
        /// Check if a service is registered
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>True if service is registered</returns>
        public bool IsRegistered<T>()
        {
            var serviceType = typeof(T);
            lock (_lockObject)
            {
                return _singletonServices.ContainsKey(serviceType) || _transientFactories.ContainsKey(serviceType);
            }
        }

        /// <summary>
        /// Remove a service registration
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        public void Unregister<T>()
        {
            var serviceType = typeof(T);
            lock (_lockObject)
            {
                _singletonServices.Remove(serviceType);
                _transientFactories.Remove(serviceType);
            }
        }

        /// <summary>
        /// Clear all service registrations
        /// </summary>
        public void Clear()
        {
            lock (_lockObject)
            {
                _singletonServices.Clear();
                _transientFactories.Clear();
            }
        }

        /// <summary>
        /// Get all registered service types
        /// </summary>
        /// <returns>List of registered service types</returns>
        public List<Type> GetRegisteredServiceTypes()
        {
            lock (_lockObject)
            {
                var types = new List<Type>();
                types.AddRange(_singletonServices.Keys);
                types.AddRange(_transientFactories.Keys);
                return types;
            }
        }
    }
} 