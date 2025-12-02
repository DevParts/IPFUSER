namespace Microsoft.SqlServer.Management.Smo;

public class RangeTypeConverter : EnumToDisplayNameConverter
{
	public RangeTypeConverter()
		: base(typeof(RangeType))
	{
	}
}
