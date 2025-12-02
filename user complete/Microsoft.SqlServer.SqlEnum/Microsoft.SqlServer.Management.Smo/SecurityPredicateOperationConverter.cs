namespace Microsoft.SqlServer.Management.Smo;

public class SecurityPredicateOperationConverter : EnumToDisplayNameConverter
{
	public SecurityPredicateOperationConverter()
		: base(typeof(SecurityPredicateOperation))
	{
	}
}
