namespace Microsoft.SqlServer.Management.Smo;

internal class NumberedObjectComparer : ObjectComparerBase
{
	internal NumberedObjectComparer()
		: base(null)
	{
	}

	public override int Compare(object obj1, object obj2)
	{
		return ((NumberedObjectKey)obj1).Number - ((NumberedObjectKey)obj2).Number;
	}
}
