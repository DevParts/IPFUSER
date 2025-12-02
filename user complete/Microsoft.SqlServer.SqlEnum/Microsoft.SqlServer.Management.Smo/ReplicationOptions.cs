using System;

namespace Microsoft.SqlServer.Management.Smo;

[Flags]
public enum ReplicationOptions
{
	None = 0,
	Published = 1,
	Subscribed = 2,
	MergePublished = 4,
	MergeSubscribed = 8
}
