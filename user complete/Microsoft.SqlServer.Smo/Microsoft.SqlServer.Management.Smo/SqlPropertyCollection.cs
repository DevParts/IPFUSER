namespace Microsoft.SqlServer.Management.Smo;

public class SqlPropertyCollection : PropertyCollection
{
	internal SqlPropertyCollection(SqlSmoObject parent, SqlPropertyMetadataProvider pmp)
		: base(parent, pmp)
	{
	}

	public SqlPropertyInfo GetPropertyInfo(string name)
	{
		return ((SqlPropertyMetadataProvider)base.PropertiesMetadata).GetPropertyInfo(LookupID(name));
	}

	public SqlPropertyInfo[] EnumPropertyInfo(SqlServerVersions versions)
	{
		return ((SqlPropertyMetadataProvider)base.PropertiesMetadata).EnumPropertyInfo(versions);
	}

	public SqlPropertyInfo[] EnumPropertyInfo()
	{
		return EnumPropertyInfo(SqlServerVersions.Version90);
	}
}
