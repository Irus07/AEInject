using System;
using System.Collections.Generic;
using System.Linq;
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

		public ServiceDescriptor(Type serviceType, ServiceLifeTime serviceLifeTime, Type typeImplementation, object[]? classParams = null)
		{
			ServiceType = serviceType;
			ServiceLifeTime = serviceLifeTime;
			TypeImplementation = typeImplementation;
			ClassParams = classParams;

			_factory = new(serviceLifeTime, typeImplementation, serviceType, classParams);
		}


		internal object GetInstance()
		{
			ILifeTimeManager instance = _factory.GetInstance();

			return instance.GetInstance();
		}

	}

}
