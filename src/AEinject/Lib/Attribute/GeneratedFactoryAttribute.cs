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
	public class GeneratedFactoryAttribute : System.Attribute
	{
		public Type ServiceType { get; }

		public GeneratedFactoryAttribute(Type serviceType)
		{
			ServiceType = serviceType;
		}
	}
}
