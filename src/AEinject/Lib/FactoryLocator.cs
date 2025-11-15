using AEinject.Lib.Attribute;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace AEinject.Lib
{
	public static class FactoryLocator
	{
		private readonly static Lazy<Dictionary<Type, Func<object>>> _cache;
		private static readonly Lazy<Dictionary<string, Type>> _typeIndex;
		private static readonly ConcurrentDictionary<Type, Type> _factoryToServiceCache ;
		public static IReadOnlyDictionary<Type, Func<object>> GetFactories() => _cache.Value;

		static FactoryLocator()
		{
			_cache = new Lazy<Dictionary<Type, Func<object>>>(() =>
			{
				var factories = new Dictionary<Type, Func<object>>();
				var assemblies = GetRelevantAssemblies();


				foreach (var assembly in assemblies)
				{
					var factoryTypes = 
					from type in assembly.GetTypes()
					where type.Name.EndsWith("_IncrementalFactory")
					where type.Namespace == "DIFactoryGenerator.Factories"
					where type.IsClass
					select type;


					foreach (var factoryType in factoryTypes)
					{
						var serviceType = ExtractServiceType(factoryType);
						if (serviceType != null)
						{
							var factoryDelegate = CreateCompiledDelegate(factoryType);
							factories[serviceType] = factoryDelegate;
						}
					}
				}
				return factories;
			}, isThreadSafe: true);
			_factoryToServiceCache = new ConcurrentDictionary<Type, Type> { };
			_typeIndex = new Lazy<Dictionary<string, Type>>(() =>
			{
				var assemblies = GetUserAssemblies();
				return assemblies
					.SelectMany(asm =>
					{
						try { return asm.GetTypes(); }
						catch { return Array.Empty<Type>(); }
					})
					.GroupBy(t => t.Name)
					.ToDictionary(g => g.Key, g => g.First()); 
			});
		}

		private static Func<object> CreateCompiledDelegate(Type factoryType)
		{
			var factoryMethod = factoryType.GetMethod("Create",BindingFlags.Public | BindingFlags.Static);

			if (factoryMethod is null)
				return null;

			
			var call = Expression.Call(factoryMethod);
			var lambda = Expression.Lambda<Func<object>>(call);
			return lambda.Compile();
		}

		private static Type ExtractServiceType(Type factoryType)
		{
			var serviceName = factoryType.Name.Replace("_IncrementalFactory", "");
			_typeIndex.Value.TryGetValue(serviceName, out var serviceType);
			return serviceType;
		}

		private static IEnumerable<Assembly> GetRelevantAssemblies()
		{
			return AppDomain.CurrentDomain.GetAssemblies()
				.Where(asm =>
					!asm.IsDynamic &&
					!asm.FullName.StartsWith("System.") &&
					!asm.FullName.StartsWith("Microsoft.") &&
					!asm.FullName.StartsWith("netstandard"));
		}

		private static IEnumerable<Assembly> GetUserAssemblies()
		{
			
			return AppDomain.CurrentDomain.GetAssemblies()
				.Where(asm => !IsSystemAssembly(asm));
		}
		private static bool IsSystemAssembly(Assembly assembly)
		{
			var name = assembly.FullName;
			return name.StartsWith("System.") ||
				   name.StartsWith("Microsoft.") ||
				   name.StartsWith("netstandard") ||
				   assembly.IsDynamic;
		}
	}
}
