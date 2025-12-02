namespace Microsoft.SqlServer.Management.Smo;

internal class SecurityPredicateObjectComparer : ObjectComparerBase
{
	internal SecurityPredicateObjectComparer()
		: base(null)
	{
	}

	public override int Compare(object obj1, object obj2)
	{
		return ((SecurityPredicateObjectKey)obj1).SecurityPredicateID - ((SecurityPredicateObjectKey)obj2).SecurityPredicateID;
	}
}
