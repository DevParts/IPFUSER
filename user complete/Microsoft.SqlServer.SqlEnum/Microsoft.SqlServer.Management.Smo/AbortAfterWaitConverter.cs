namespace Microsoft.SqlServer.Management.Smo;

public class AbortAfterWaitConverter : EnumToDisplayNameConverter
{
	public AbortAfterWaitConverter()
		: base(typeof(AbortAfterWait))
	{
	}
}
