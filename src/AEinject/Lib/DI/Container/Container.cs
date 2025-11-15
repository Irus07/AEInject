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
    internal class DIContainer
    {
        protected static Dictionary<Type, ServiceDescriptor> _container;

        public DIContainer()
        {
            if (_container is null)
                _container = new Dictionary<Type, ServiceDescriptor>();
        }

        internal ServiceDescriptor GetService<T1>() => _container[typeof(T1)];


        internal object GetService(Type ServiceType)
        {
            throw new NotImplementedException();

        }

        internal void AddService(ServiceDescriptor descriptor) => _container.Add(descriptor.ServiceType, descriptor);

        internal bool ContainsKey(Type key) => _container.ContainsKey(key);


    }
}
