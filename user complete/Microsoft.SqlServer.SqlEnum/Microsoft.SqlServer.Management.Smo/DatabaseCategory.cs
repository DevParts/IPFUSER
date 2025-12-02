namespace Microsoft.SqlServer.Management.Smo;

internal enum DatabaseCategory
{
	Published = 1,
	Subscribed = 2,
	MergePublished = 4,
	MergeSubscribed = 8
}
