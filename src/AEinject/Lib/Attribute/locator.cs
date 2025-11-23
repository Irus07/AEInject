using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEinject.Lib.Attribute
{
	[AttributeUsage
		(AttributeTargets.Class,
		AllowMultiple = false,
		Inherited = false
		)]
	public class LocatorFactoryAttribute : System.Attribute
	{
		public Type ServiceType { get; }
		public Type FactoryType { get; }

		public LocatorFactoryAttribute(Type serviceType, Type factoryType)
		{
			ServiceType = serviceType;
			FactoryType = factoryType;

			FactoryLocator.Register(factoryType, serviceType);
		}
	}
}