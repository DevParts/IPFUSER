namespace Microsoft.SqlServer.Management.Smo;

public class SecurityPredicateTypeConverter : EnumToDisplayNameConverter
{
	public SecurityPredicateTypeConverter()
		: base(typeof(SecurityPredicateType))
	{
	}
}
