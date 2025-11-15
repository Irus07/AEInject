using AEinject.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AEInject.Lib.DI.Container
{
	internal class ServiceDescriptor
	{
		internal readonly Type ServiceType;
		internal readonly Type TypeImplementation;
		internal readonly ServiceLifeTime ServiceLifeTime;
		internal readonly object[]? ClassParams;
		private readonly LifeTimeManagerFactory _factory;

		private readonly Lazy<MethodInfo?> _factoryMethod;

		public ServiceDescriptor(Type serviceType, ServiceLifeTime serviceLifeTime,
			Type typeImplementation, object[]? classParams = null)
		{
			ServiceType = serviceType;
			ServiceLifeTime = serviceLifeTime;
			TypeImplementation = typeImplementation;
			ClassParams = classParams;

			_factory = new(serviceLifeTime, typeImplementation, serviceType, classParams);
			_factoryMethod = new Lazy<MethodInfo?>(() =>
			{
				try
				{
					var factories = FactoryLocator.GetFactories();
					if (factories.TryGetValue(typeImplementation, out var method))
					{
						return method;
					}
					return null;
				}
				catch
				{
					return null;
				}
			});
		}

		internal object GetInstance(params object[] constructorParameters)
		{
			if (_factoryMethod.Value != null)
			{
				try
				{
					return _factoryMethod.Value.Invoke(null, new object[] { constructorParameters });
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException(
						$"Failed to create instance of {TypeImplementation} using generated factory", ex);
				}
			}
			if (constructorParameters != null && constructorParameters.Length > 0)
			{
				throw new InvalidOperationException(
					$"Cannot create instance of {TypeImplementation} with parameters. " +
					"No generated factory found and lifetime managers don't support parameters.");
			}

			ILifeTimeManager instance = _factory.GetInstance();
			return instance.GetInstance();
		}

	}

}
