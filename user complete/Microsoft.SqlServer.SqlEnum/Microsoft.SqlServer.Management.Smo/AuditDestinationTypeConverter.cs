namespace Microsoft.SqlServer.Management.Smo;

public class AuditDestinationTypeConverter : EnumToDisplayNameConverter
{
	public AuditDestinationTypeConverter()
		: base(typeof(AuditDestinationType))
	{
	}
}
