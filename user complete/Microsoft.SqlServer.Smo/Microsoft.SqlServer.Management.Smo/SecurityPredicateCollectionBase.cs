using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class SecurityPredicateCollectionBase : SortedListCollectionBase
{
	internal SecurityPredicateCollectionBase(SqlSmoObject parent)
		: base(parent)
	{
	}

	protected override void InitInnerCollection()
	{
		base.InternalStorage = new SmoSortedList(new SecurityPredicateObjectComparer());
	}

	public bool Contains(int securityPredicateID)
	{
		return Contains(new SecurityPredicateObjectKey(securityPredicateID));
	}

	public SecurityPredicate GetItemByTargetObjectID(int targetObjectID)
	{
		return GetItemByTargetObjectID(targetObjectID, SecurityPredicateType.Filter, SecurityPredicateOperation.All);
	}

	public SecurityPredicate GetItemByTargetObjectID(int targetObjectID, SecurityPredicateType predicateType, SecurityPredicateOperation predicateOperation)
	{
		foreach (SecurityPredicate item in base.InternalStorage)
		{
			if (item.TargetObjectID == targetObjectID && item.PredicateType == predicateType && item.PredicateOperation == predicateOperation)
			{
				return item;
			}
		}
		return null;
	}

	internal override ObjectKeyBase CreateKeyFromUrn(Urn urn)
	{
		int securityPredicateID = int.Parse(urn.GetAttribute("SecurityPredicateID"), SmoApplication.DefaultCulture);
		return new SecurityPredicateObjectKey(securityPredicateID);
	}
}
