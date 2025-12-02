namespace Microsoft.SqlServer.Management.Smo;

public class IndexTypeConverter : EnumToDisplayNameConverter
{
	public IndexTypeConverter()
		: base(typeof(IndexType))
	{
	}
}
