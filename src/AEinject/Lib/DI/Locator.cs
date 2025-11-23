using AEinject.Lib.Attribute;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AEinject.Lib
{
	public static class FactoryLocator
	{
		public static readonly ConcurrentDictionary<Type, IEnumerable<MethodInfo>> _factories;
		private static bool _isInit = false;

		static FactoryLocator()
		{
			_factories = new ConcurrentDictionary<Type, IEnumerable<MethodInfo>>();
		}
		public static void Init()
		{
			if (!_isInit)
			{
				TriggerAttributes();
				_isInit = true;
			}
			return;
		}

		public static void Register(Type factoryType, Type targetType)
		{
			if (_factories.ContainsKey(factoryType))
			{
				return;
			}

			var targetMethods =
				from method in targetType.GetMethods()
				where method.Name == "Create"
				select method;

			_factories.TryAdd(targetType, targetMethods);

		}
		internal static T1 CreateInstance<T1>(object[] @params = null)
		{

			var targetMethod = _factories[typeof(T1)].FirstOrDefault((method) => ParametersMatch(method.GetParameters(), @params.Select((c) => c.GetType()).ToArray()));

			if (targetMethod.Invoke(null, @params) is T1 targetObject)
			{
				return targetObject;
			}

			throw new ArgumentException($"the factory method for creating a {typeof(T1).ToString()} type object with @params parameters was not found", nameof(@params));
		}

		private static bool ParametersMatch(ParameterInfo[] parameters, Type[] argumentTypes)
		{
			if (parameters.Length != argumentTypes.Length)
				return false;

			for (int i = 0; i < parameters.Length; i++)
			{
				if (!parameters[i].ParameterType.IsAssignableFrom(argumentTypes[i]))
					return false;
			}

			return true;
		}

		private static void TriggerAttributes()
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Type targetAttribute = typeof(LocatorFactoryAttribute);

			var targetAssemblies =
				from assembly in assemblies
				let assemblyName = assembly.GetName().Name
				where !assemblyName.StartsWith("System.") &&
					  !assemblyName.StartsWith("Microsoft.") &&
					  !assemblyName.StartsWith("Windows.") &&
					  !assemblyName.StartsWith("mscorlib") &&
					  !assemblyName.StartsWith("netstandard") &&
					  !assemblyName.Equals("System") &&
					  !assembly.IsDynamic
				select assembly;



			Parallel.ForEach(targetAssemblies, (assembly) =>
			{
				try
				{
					foreach (var type in assembly.GetTypes())
					{
						try
						{
							if (type.IsDefined(targetAttribute, false))
							{
								var attributes = type.GetCustomAttributes(targetAttribute, false);
							}
						}
						catch (Exception ex) when (ex is TypeLoadException || ex is FileNotFoundException || ex is BadImageFormatException)
						{
						}

					}
				}
				catch (Exception ex) when (ex is FileNotFoundException || ex is BadImageFormatException || ex is ReflectionTypeLoadException)
				{

				}

			});

		}
	}
}