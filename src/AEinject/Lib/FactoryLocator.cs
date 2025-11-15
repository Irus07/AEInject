using AEinject.Lib.Attribute;
using System.Collections.Concurrent;
using System.Reflection;

namespace AEinject.Lib
{
	public static class FactoryLocator
	{
		private readonly static Lazy<ConcurrentDictionary<Type, MethodInfo>> _cache;

		public static IReadOnlyDictionary<Type, MethodInfo> GetFactories() => _cache.Value;

		static FactoryLocator()
		{
			_cache = new Lazy<ConcurrentDictionary<Type, MethodInfo>>(() =>
			{
				var factories = new ConcurrentDictionary<Type, MethodInfo>();
				var assemblies = GetRelevantAssemblies();

				foreach (var assembly in assemblies)
				{
					try
					{
						var factoryTypes = assembly.GetTypes()
							.Where(type => type.Name.EndsWith("_IncrementalFactory", StringComparison.Ordinal) &&
										  type.Namespace == "DIFactoryGenerator.Factories" &&
										  type.IsClass && type.IsAbstract && type.IsSealed);

						foreach (var factoryType in factoryTypes)
						{
							var serviceType = ExtractServiceType(factoryType);
							if (serviceType != null)
							{
								var createMethod = factoryType.GetMethod("Create",
									BindingFlags.Public | BindingFlags.Static,
									null,
									new Type[] { typeof(object[]) },
									null);

								if (createMethod != null)
								{
									factories.TryAdd(serviceType, createMethod);
								}
							}
						}
					}
					catch (ReflectionTypeLoadException)
					{
						
					}
				}

				return factories;
			}, LazyThreadSafetyMode.ExecutionAndPublication);
		}

		private static Type ExtractServiceType(Type factoryType)
		{
			try
			{
				var attribute = factoryType.GetCustomAttribute<GeneratedFactoryAttribute>();
				return attribute?.ServiceType;
			}
			catch
			{
				return null;
			}
		}

		private static IEnumerable<Assembly> GetRelevantAssemblies()
		{
			return AppDomain.CurrentDomain.GetAssemblies()
				.Where(asm => !IsSystemAssembly(asm) && !asm.IsDynamic);
		}

		private static bool IsSystemAssembly(Assembly assembly)
		{
			if (assembly.IsDynamic)
				return true;

			var fullName = assembly.FullName ?? "";
			return fullName.StartsWith("System.") ||
				   fullName.StartsWith("Microsoft.") ||
				   fullName.StartsWith("netstandard") ||
				   fullName.StartsWith("AEInject.") ||
				   fullName.StartsWith("DIFactoryGenerator");
		}

		public static object CreateInstance(Type serviceType, params object[] constructorParameters)
		{
			if (_cache.Value.TryGetValue(serviceType, out var factoryMethod))
			{
				return factoryMethod.Invoke(null, new object[] { constructorParameters });
			}
			return null;
		}

		public static T CreateInstance<T>(params object[] constructorParameters)
		{
			return (T)CreateInstance(typeof(T), constructorParameters);
		}
	}
}