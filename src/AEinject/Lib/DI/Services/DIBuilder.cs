using System.ComponentModel.DataAnnotations;
using AEinject.Lib;
using AEInject.Lib.DI.Container;
using static AEInject.Lib.DI.Container.ServiceLifeTime;

namespace AEInject.Lib.DI.Services;

public class DIBuilder
{
	private DIBuilder()
	{
		if (_container is null)
			_container = new DIContainer();
	}


	public static DIBuilder CreateDIBuilder()
	{
		return new DIBuilder();
	}


	private static DIContainer _container;

	
	public void AddSingleton<Interface, Class>(object[]? parameters = null)
	{
		Сheck(typeof(Interface),
			typeof(Class));

		if (FactoryLocator._factories.ContainsKey(typeof(Class)))
		{
			ServiceDescriptor descriptor = new(
			typeof(Interface),
			Singleton,
			typeof(Class),
			parameters);
		}
		else
		{
			ServiceDescriptor descriptor = new(
			typeof(Interface),
			Singleton,
			typeof(Class),
			parameters);


			_container.AddService(descriptor);
		}

		
	}

	public void AddTransient<Interface, Class>(object[]? parameters = null)
	{
		Сheck(typeof(Interface),
			typeof(Class));

		ServiceDescriptor descriptor = new(
			typeof(Interface),
			Transient,
			typeof(Class),
			parameters);

		_container.AddService(descriptor);
	}

	public void Build() { }

	//Share responsibility 
	private void Сheck(Type interfaceType, Type classType)
	{
		if (!interfaceType.IsAssignableFrom(classType))
			throw new ArgumentException($"The class {classType.FullName} does not implement the interface {interfaceType.FullName}");

		if (_container.ContainsKey(classType))
			throw new ArgumentException($"An implementation has already been defined for the interface {interfaceType.FullName}");

	}
}
