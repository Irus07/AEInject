namespace AEInject.Lib.Attribute
{
	[AttributeUsage
		(AttributeTargets.Field |
		AttributeTargets.Parameter |
		AttributeTargets.Property ,
		AllowMultiple = false ,
		Inherited = false 
		)]
	public sealed class InjectAttribute : System.Attribute
	{
		// AEInject.Lib.Attribute.InjectAttribute
	}
}
