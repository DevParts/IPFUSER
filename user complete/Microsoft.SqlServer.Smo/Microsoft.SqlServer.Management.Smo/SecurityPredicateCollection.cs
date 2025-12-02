using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class SecurityPredicateCollection : SecurityPredicateCollectionBase
{
	public SecurityPolicy Parent => base.ParentInstance as SecurityPolicy;

	public SecurityPredicate this[int index] => GetObjectByIndex(index) as SecurityPredicate;

	internal SecurityPredicateCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void Add(SecurityPredicate securityPredicate)
	{
		if (base.InternalStorage.Contains(securityPredicate.key))
		{
			securityPredicate.SecurityPredicateID = this[base.Count - 1].SecurityPredicateID + 1;
			securityPredicate.key = new SecurityPredicateObjectKey(securityPredicate.SecurityPredicateID);
		}
		base.InternalStorage.Add(securityPredicate.key, securityPredicate);
	}

	public void CopyTo(SecurityPredicate[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(SecurityPredicate);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new SecurityPredicate(this, key, state);
	}
}
