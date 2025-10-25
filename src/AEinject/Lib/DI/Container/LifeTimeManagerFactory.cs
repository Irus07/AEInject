namespace AEInject.Lib.DI.Container
{
	internal class LifeTimeManagerFactory
	{
		private ServiceLifeTime ServiceLifeTime;
		private Type TypeImplementation;
		private Type ServiceType;
		private object[]? ClassParams;

		private readonly ILifeTimeManager _manager ;

		public LifeTimeManagerFactory(ServiceLifeTime serviceLifeTime, Type typeImplementation, Type serviceType, object[]? classParams = null)
		{
			ServiceLifeTime = serviceLifeTime;
			TypeImplementation = typeImplementation;
			ServiceType = serviceType;
			ClassParams = classParams;

			_manager = ServiceLifeTime switch
			{
				ServiceLifeTime.Singleton => new SingletonLifeManager(
				   TypeImplementation,
				   ServiceType,
				   ClassParams),

				ServiceLifeTime.Transient => new TransientLifeManager(
				   TypeImplementation,
				   ServiceType,
				   ClassParams)

			};
		}

		public ILifeTimeManager GetInstance() => _manager;
		
	}

}
