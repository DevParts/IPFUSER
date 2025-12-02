using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(ClusterMemberTypeConverter))]
public enum ClusterMemberType
{
	[LocDisplayName("cmtNode")]
	Node,
	[LocDisplayName("cmtDiskWitness")]
	DiskWitness,
	[LocDisplayName("cmtFileshareWitness")]
	FileshareWitness
}
