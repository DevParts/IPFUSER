using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(IndexTypeConverter))]
public enum IndexType
{
	[TsqlSyntaxString("CLUSTERED INDEX")]
	[LocDisplayName("Clustered")]
	ClusteredIndex,
	[LocDisplayName("NonClustered")]
	[TsqlSyntaxString("INDEX")]
	NonClusteredIndex,
	[TsqlSyntaxString("PRIMARY XML INDEX")]
	[LocDisplayName("PrimaryXml")]
	PrimaryXmlIndex,
	[TsqlSyntaxString("XML INDEX")]
	[LocDisplayName("SecondaryXml")]
	SecondaryXmlIndex,
	[LocDisplayName("Spatial")]
	[TsqlSyntaxString("SPATIAL INDEX")]
	SpatialIndex,
	[LocDisplayName("NonClusteredColumnStore")]
	[TsqlSyntaxString("NONCLUSTERED COLUMNSTORE INDEX")]
	NonClusteredColumnStoreIndex,
	[LocDisplayName("NonClusteredHash")]
	[TsqlSyntaxString("NONCLUSTERED HASH INDEX")]
	NonClusteredHashIndex,
	[LocDisplayName("SelectiveXml")]
	[TsqlSyntaxString("SELECTIVE XML INDEX")]
	SelectiveXmlIndex,
	[TsqlSyntaxString("")]
	[LocDisplayName("SecondarySelectiveXml")]
	SecondarySelectiveXmlIndex,
	[LocDisplayName("ClusteredColumnStore")]
	[TsqlSyntaxString("CLUSTERED COLUMNSTORE INDEX")]
	ClusteredColumnStoreIndex,
	[TsqlSyntaxString("HEAP")]
	[LocDisplayName("Heap")]
	HeapIndex
}
