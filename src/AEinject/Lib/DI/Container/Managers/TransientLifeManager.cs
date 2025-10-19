namespace AEinject.Lib.DI.Container
{
	internal class TransientLifeManager : ILifeTimeManager
	{
		private readonly Type _typeImplementation;
		private readonly Type _serviceType;
		private readonly object[]? _params;

		public TransientLifeManager(Type typeImplementation, Type serviceType, object[]? @params)
		{
			_typeImplementation = typeImplementation;
			_serviceType = serviceType;
			_params = @params;
		}

		public object GetInstance()
		{
			object? instance = _params is null ?
				Activator.CreateInstance(_typeImplementation) : 
				Activator.CreateInstance(_typeImplementation, _params);


			return instance!;
		}
	}

}
