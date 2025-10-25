using AEInject.Lib.DI.Container;

namespace AEInject.Lib.DI.Services;

public class ServiceProvider
{
    private ServiceProvider()
    {

        if (_container is null)
            _container = new DIContainer();
    }

    private static DIContainer _container;

    internal static ServiceProvider GetServiceProvider() => new ServiceProvider();


    
    public T1 GetService<T1>()
    {
        ServiceDescriptor descriptor = _container.GetService<T1>();

        //var @class = Activator.CreateInstance(descriptor.TypeImplementation);
        var @class = descriptor.GetInstance();
        if (@class is T1 rez)
            return rez;


        throw new NotImplementedException();
    }


}