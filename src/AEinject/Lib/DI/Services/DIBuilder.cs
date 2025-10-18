using System.ComponentModel.DataAnnotations;
using AEinject.Lib.DI.Container;
using static AEinject.Lib.DI.Container.ServiceLifeTime;

namespace AEinject.Lib.DI.Services;

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

    public void AddSingleton<Interface, Class>()
    {
        Сheck(typeof(Interface),
            typeof(Class));

        ServiceDescriptor descriptor = new(
            typeof(Interface),
            Singleton,
            typeof(Class));


        _container.AddService(descriptor);
    }
    public void AddSingleton<Interface, Class>(object[] parameters) 
    {
		Сheck(typeof(Interface),
			typeof(Class));

		ServiceDescriptor descriptor = new(
			typeof(Interface),
			Singleton,
			typeof(Class),
            parameters);


		_container.AddService(descriptor);
	}
    public void AddTransient<Interface, Class>() { }
    public void AddTransient<Interface, Class>(object[] parameters) { }

    public void Build() { }

    private void Сheck(Type interfaceType, Type classType)
    {
        if (!interfaceType.IsAssignableFrom(classType))
            throw new ArgumentException($"The class {classType.FullName} does not implement the interface {interfaceType.FullName}");

        if (_container.ContainsKey(classType))
            throw new ArgumentException($"An implementation has already been defined for the interface {interfaceType.FullName}");

    }
}
