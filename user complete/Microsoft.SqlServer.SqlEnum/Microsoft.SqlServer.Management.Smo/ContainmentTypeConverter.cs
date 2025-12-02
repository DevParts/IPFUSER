namespace Microsoft.SqlServer.Management.Smo;

internal class ContainmentTypeConverter : EnumToDisplayNameConverter
{
	public ContainmentTypeConverter()
		: base(typeof(ContainmentType))
	{
	}
}
