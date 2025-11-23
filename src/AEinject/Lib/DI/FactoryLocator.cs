using AEinject.Lib.Attribute;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AEinject.Lib
{
	public static class FactoryLocator
	{
		public static readonly ConcurrentDictionary<Type, IEnumerable<MethodInfo>> _factories;
		private static bool _isInit = false;
		private static object _lock = new object();

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
			lock (_lock)
			{
				if (_factories.ContainsKey(factoryType))
					return;

				var targetMethods =
					from method in factoryType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
					where method.Name.Trim() == "Create"
					select method;

				IEnumerable<MethodInfo>? targetMethodsList = targetMethods.ToList(); //necessary for immediate execution of the LINQ query

				//List<MethodInfo> targetMethods = new List<MethodInfo>();

				//MethodInfo[] allMethods = factoryType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static );
				//foreach (MethodInfo method in allMethods)
				//{
				//	if (method.Name.Trim() == "Create")
				//	{
				//		targetMethods.Add(method);
				//	}
				//}

				_factories.TryAdd(targetType, targetMethods);
			}
		}
		internal static T1 CreateInstance<T1>(object[] @params = null)
		{

			var targetMethod = _factories[typeof(T1)].FirstOrDefault((method) => ParametersMatch(method.GetParameters(), @params.Select((c) => c.GetType()).ToArray()));

			if (targetMethod.Invoke(null, @params) is T1 targetObject)
				return targetObject;

			throw new ArgumentException($"the factory method for creating a {typeof(T1).ToString()} type object with @params parameters was not found", nameof(@params));
		}

		private static bool ParametersMatch(ParameterInfo[] parameters, Type[] argumentTypes)
		{
			if (parameters.Length != argumentTypes.Length)
				return false;

			for (int i = 0; i < parameters.Length; i++)
				if (!parameters[i].ParameterType.IsAssignableFrom(argumentTypes[i]))
					return false;

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


			foreach (var assembly in targetAssemblies)
			{
				try
				{
					foreach (var type in assembly.GetTypes())
						try
						{
							if (type.IsDefined(targetAttribute, false))
							{
								var attributes = type.GetCustomAttributes(targetAttribute, false);
							}
						}
						catch (Exception ex) when (ex is TypeLoadException || ex is FileNotFoundException || ex is BadImageFormatException) { }
				}
				catch (Exception ex) when (ex is FileNotFoundException || ex is BadImageFormatException || ex is ReflectionTypeLoadException)
				{
				}
			}
			//_ = Parallel.ForEach(targetAssemblies, (assembly) =>
			//{
			//	try
			//	{
			//		foreach (var type in assembly.GetTypes())
			//			try
			//			{
			//				if (type.IsDefined(targetAttribute, false))
			//				{
			//					var attributes = type.GetCustomAttributes(targetAttribute, false);
			//				}
			//			}
			//			catch (Exception ex) when (ex is TypeLoadException || ex is FileNotFoundException || ex is BadImageFormatException) { }
			//	}
			//	catch (Exception ex) when (ex is FileNotFoundException || ex is BadImageFormatException || ex is ReflectionTypeLoadException)
			//	{
			//	}

			//});

		}
		internal static Func<object[]?, T1?> CreateDescriptor<T1>(object[]? @params = null)
		{
			var x = _factories[typeof(T1)];


			Func<object[]?, T1?> func = (@params) =>
			{


				var x = _factories[typeof(T1)];

				if (@params is null)
					@params = Array.Empty<object>();

				//if (x.Any())
				//	throw new ArgumentException("X IS NULL");

				var types = @params.Select(x => x.GetType().FullName);



				var targetMethod = (
					from method in x
					let param = method.GetParameters()
					let methodParamTypes = param.Select(p => p.ParameterType.FullName)
					where param.Length == @params.Length
					where types.SequenceEqual(methodParamTypes)
					select method
				).FirstOrDefault();

				//MethodInfo targetMethod = null;
				//lock (_lock){
					
				//	foreach (var method in x)
				//	{
				//		if (ParametersMatch(method.GetParameters(), @params.Select(p => p.GetType()).ToArray()))
				//		{
				//			targetMethod = method;
				//			break;
				//		}
				//	}
				//}
				



				if (targetMethod is null)
					throw new ArgumentException($"The factory for creating an object that meets the {@params} parameters was not found");


				if (targetMethod.Invoke(null, @params) is T1 targetRes)
				{
					return targetRes;
				}
				else
				{
					throw new ArgumentException($"the factory method returned an unexpected type, but expected {typeof(T1)}");
				}

			};

			return func;

		}
	}
}