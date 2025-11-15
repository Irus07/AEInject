using AEInject.Lib.DI.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AEInject.Lib.DI.Container.ServiceLifeTime;

namespace AEinject.Lib.DI.Container
{
    public static class  ContainerExtensions
    {
		public static void AddSingleton<Interface, Class>(this DIContainer container, object[]? parameters = null)
		{
			Сheck(container, typeof(Interface),
				typeof(Class));

			ServiceDescriptor descriptor = new(
				typeof(Interface),
				Singleton,
				typeof(Class),
				parameters);


			container.AddService(descriptor);
		}
		public static void AddTransient<Interface, Class>(this DIContainer container, object[]? parameters = null)
		{
			Сheck(container,typeof(Interface),
				typeof(Class));

			ServiceDescriptor descriptor = new(
				typeof(Interface),
				Transient,
				typeof(Class),
				parameters);

			container.AddService(descriptor);
		}


		private static void Сheck(DIContainer _container, Type interfaceType, Type classType)
		{
			if (!interfaceType.IsAssignableFrom(classType))
				throw new ArgumentException($"The class {classType.FullName} does not implement the interface {interfaceType.FullName}");

			if (_container.ContainsKey(classType))
				throw new ArgumentException($"An implementation has already been defined for the interface {interfaceType.FullName}");
		}
	}

}
