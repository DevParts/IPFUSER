using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(ExternalFileFormatTypeConverter))]
public enum ExternalFileFormatType
{
	None = -1,
	[TsqlSyntaxString("DELIMITEDTEXT")]
	DelimitedText,
	[TsqlSyntaxString("RCFILE")]
	RcFile,
	[TsqlSyntaxString("ORC")]
	Orc,
	[TsqlSyntaxString("PARQUET")]
	Parquet
}
