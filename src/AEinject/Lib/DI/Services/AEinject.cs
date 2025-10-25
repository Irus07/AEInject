using AEInject.Lib.DI;
using AEInject.Lib.DI.Services;
namespace AEInject;

public static class AEinject
{
	public static DIBuilder CreateBuilder() => DIBuilder.CreateDIBuilder();
	public static ServiceProvider GetServiceProvider() => ServiceProvider.GetServiceProvider();
}
