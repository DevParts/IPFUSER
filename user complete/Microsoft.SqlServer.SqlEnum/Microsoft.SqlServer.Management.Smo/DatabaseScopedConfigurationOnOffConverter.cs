namespace Microsoft.SqlServer.Management.Smo;

public class DatabaseScopedConfigurationOnOffConverter : EnumToDisplayNameConverter
{
	public DatabaseScopedConfigurationOnOffConverter()
		: base(typeof(DatabaseScopedConfigurationOnOff))
	{
	}
}
