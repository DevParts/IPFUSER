namespace Microsoft.SqlServer.Management.Smo;

public class AuditActionTypeConverter : EnumToDisplayNameConverter
{
	public AuditActionTypeConverter()
		: base(typeof(AuditActionType))
	{
	}
}
