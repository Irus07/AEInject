using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEinject.Lib.DI.Container
{
	internal class ServiceDescriptor
	{
		internal Type ServiceType;
		internal Type TypeImplementation;
		internal ServiceLifeTime ServiceLifeTime;
		internal object[]? ClassParams;

		public ServiceDescriptor(Type serviceType, ServiceLifeTime serviceLifeTime, Type typeImplementation)
		{
			ServiceType = serviceType;
			ServiceLifeTime = serviceLifeTime;
			TypeImplementation = typeImplementation;

		}

		public ServiceDescriptor(Type serviceType, ServiceLifeTime serviceLifeTime, Type typeImplementation, object[] @params)
			: this(serviceType,
				  serviceLifeTime,
				  typeImplementation)
		{
			ClassParams = @params;
		}

		internal object GetInstance()
		{
			object instance;

			if (ClassParams is null)
			{
				instance = Activator.CreateInstance(TypeImplementation)!;
			}
			else
			{
				instance = Activator.CreateInstance(TypeImplementation, ClassParams)!;
			}

			return instance;
		}

	}

}
