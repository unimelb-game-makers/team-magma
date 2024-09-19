using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.ServiceLocator
{ 
    /**
 * The ServiceLocator class is a singleton that allows services to be registered and retrieved by type.
 *
 * How to user a service locator:
     * 0. Create a singleton class that inherits from ServiceLocator
     * 1: Created Interface of the service inherited from IGameService
     * 2. Include services required for the service in the inherited interface
     * 3. Register the IService in the implementation of the service interface to the service locator of specific type
        * a. Register(this)
     * Now the service is registered and can be retrieved by the service locator
 */
    public class ServiceLocator : Singleton<ServiceLocator>
    {
        private readonly Dictionary<string, List<IGameService>> services = new ();
        public List<T> Get<T>() where T : IGameService
        {
            string key = typeof(T).Name;
            if (!services.ContainsKey(key))
            {
                throw new InvalidOperationException($"No services registered for type {key}.");
            }

            // Attempt to cast each service to the desired type and return the list
            return services[key].OfType<T>().ToList();
        }
        public void Register<T>(T service) where T : IGameService
        {
            string key = typeof(T).Name;
            if (!this.services.ContainsKey(key))
            {
                this.services[key] = new List<IGameService>();
            }

            this.services[key].Add(service);
        }
        public void Unregister<T>(T service) where T : IGameService
        {
            string key = typeof(T).Name;
            if (!this.services.ContainsKey(key))
            {
                return;
            }

            this.services[key].Remove(service);
            if (this.services[key].Count == 0)
            {
                this.services.Remove(key);
            }
        }
    }
}