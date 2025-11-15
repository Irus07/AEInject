using AEInject.Lib.DI.Container;

namespace AEInject.Lib.DI.Services;

public static class ServiceProvider
{
	private static DIContainer _container;

	public static void Initialize(DIContainer container)
	{
		_container = container ?? throw new ArgumentNullException(nameof(container));

	}

	public static T GetServiceInstance<T>()
	{
		if (_container == null)
			throw new InvalidOperationException("ServiceProvider has not been initialized. Call Initialize first.");

		return _container.GetService<T>();
	}

	public static T GetServiceInstance<T>(params object[] constructorParameters)
	{
		if (_container == null)
			throw new InvalidOperationException("ServiceProvider has not been initialized. Call Initialize first.");

		return _container.GetService<T>(constructorParameters);
	}

	public static object GetServiceInstance(Type serviceType)
	{
		if (_container == null)
			throw new InvalidOperationException("ServiceProvider has not been initialized. Call Initialize first.");

		return _container.GetService(serviceType);
	}


}