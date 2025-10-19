namespace AEinject.Lib.DI.Container
{
	internal class LifeTimeManagerFactory
	{
		private ServiceLifeTime ServiceLifeTime;
		private Type TypeImplementation;
		private Type ServiceType;
		private object[]? ClassParams;

		public LifeTimeManagerFactory(ServiceLifeTime serviceLifeTime, Type typeImplementation, Type serviceType, object[]? classParams = null)
		{
			ServiceLifeTime = serviceLifeTime;
			TypeImplementation = typeImplementation;
			ServiceType = serviceType;
			ClassParams = classParams;
		}

		public ILifeTimeManager GetInstance()
		{
			return ServiceLifeTime switch
			{
				ServiceLifeTime.Singleton => new SingletonLifeManager(
				   TypeImplementation,
				   ServiceType,
				   ClassParams),

				ServiceLifeTime.Transient => new TransientLifeManager()

			};
		}
	}

}
