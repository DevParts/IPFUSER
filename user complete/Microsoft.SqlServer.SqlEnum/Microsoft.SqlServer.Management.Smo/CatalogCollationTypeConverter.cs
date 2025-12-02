namespace Microsoft.SqlServer.Management.Smo;

public class CatalogCollationTypeConverter : EnumToDisplayNameConverter
{
	public CatalogCollationTypeConverter()
		: base(typeof(CatalogCollationType))
	{
	}
}
