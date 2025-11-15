using AEInject.Lib.Attribute;
using AEInject.Lib.DI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEInject.Lib.DI.Container
{
    [DIFactory]
    public class DIContainer
    {
		private Dictionary<Type, ServiceDescriptor> _container;
		private readonly object _lockObject = new object();

		public DIContainer()
		{
			_container = new Dictionary<Type, ServiceDescriptor>();
		}

		internal T GetService<T>() => (T)GetService(typeof(T));

		internal T GetService<T>(params object[] constructorParameters)
			=> (T)GetService(typeof(T), constructorParameters);

		internal object GetService(Type serviceType, params object[] constructorParameters)
		{
			lock (_lockObject)
			{
				if (_container.TryGetValue(serviceType, out var descriptor))
				{
					return descriptor.GetInstance(constructorParameters);
				}
				throw new InvalidOperationException($"Service {serviceType.Name} is not registered");
			}
		}

		internal object GetService(Type serviceType)
		{
			return GetService(serviceType, null);
		}

		internal void AddService(ServiceDescriptor descriptor)
		{
			lock (_lockObject)
			{
				_container.Add(descriptor.ServiceType, descriptor);
			}
		}

		internal bool ContainsKey(Type key)
		{
			lock (_lockObject)
			{
				return _container.ContainsKey(key);
			}
		}

		internal IEnumerable<Type> GetRegisteredServiceTypes()
		{
			lock (_lockObject)
			{
				return _container.Keys.ToList();
			}
		}
		internal bool TryGetService<T>(out T service)
		{
			lock (_lockObject)
			{
				if (_container.TryGetValue(typeof(T), out var descriptor))
				{
					service = (T)descriptor.GetInstance();
					return true;
				}
				service = default(T);
				return false;
			}
		}
	}
}
