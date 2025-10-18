using AEinject.Lib.DI;
using AEinject.Lib.DI.Services;
namespace AEinject;

public static class AEinject
{
	public static DIBuilder CreateBuilder() => DIBuilder.CreateDIBuilder();
	public static ServiceProvider GetServiceProvider() => ServiceProvider.GetServiceProvider();
}
