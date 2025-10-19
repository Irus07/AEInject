namespace AEinject.Lib.DI.Container
{
	internal class SingletonLifeManager : ILifeTimeManager
	{
		private readonly Type typeImplementation;
		private readonly Type serviceType;
		private readonly object[]? @params;

		private readonly object? ClassObject;

		public SingletonLifeManager(Type TypeImplementation, Type ServiceType, object[]? @params = null)
		{
			typeImplementation = TypeImplementation;
			serviceType = ServiceType;
			this.@params = @params;

			ClassObject = (@params is null) ?
						Activator.CreateInstance(typeImplementation) :
						Activator.CreateInstance(typeImplementation, @params);
		}

		public object GetInstance()
		{
			object rez = ClassObject!;

			return rez;
		}
	}

}
