using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(FileGroupTypeConverter))]
public enum FileGroupType
{
	[LocDisplayName("fgtRowsFileGroup")]
	RowsFileGroup = 0,
	[LocDisplayName("fgtFileStreamDataFileGroup")]
	FileStreamDataFileGroup = 2,
	[LocDisplayName("fgtMemoryOptimizedDataFileGroup")]
	MemoryOptimizedDataFileGroup = 3
}
