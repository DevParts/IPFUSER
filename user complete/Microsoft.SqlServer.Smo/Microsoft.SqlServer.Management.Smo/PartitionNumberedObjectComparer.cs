namespace Microsoft.SqlServer.Management.Smo;

internal class PartitionNumberedObjectComparer : ObjectComparerBase
{
	internal PartitionNumberedObjectComparer()
		: base(null)
	{
	}

	public override int Compare(object obj1, object obj2)
	{
		return ((PartitionNumberedObjectKey)obj1).Number - ((PartitionNumberedObjectKey)obj2).Number;
	}
}
