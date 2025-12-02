using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(ExternalDataSourceTypeConverter))]
public enum ExternalDataSourceType
{
	[TsqlSyntaxString("HADOOP")]
	Hadoop,
	[TsqlSyntaxString("RDBMS")]
	Rdbms,
	[TsqlSyntaxString("SHARD_MAP_MANAGER")]
	ShardMapManager
}
