using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class PlanGuideCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public PlanGuide this[int index] => GetObjectByIndex(index) as PlanGuide;

	public PlanGuide this[string name] => GetObjectByName(name) as PlanGuide;

	internal PlanGuideCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(PlanGuide[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public PlanGuide ItemById(int id)
	{
		return (PlanGuide)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(PlanGuide);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new PlanGuide(this, key, state);
	}

	public void Remove(PlanGuide planGuide)
	{
		if (planGuide == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("planGuide"));
		}
		RemoveObj(planGuide, new SimpleObjectKey(planGuide.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(PlanGuide planGuide)
	{
		AddImpl(planGuide);
	}

	internal SqlSmoObject GetObjectByName(string name)
	{
		return GetObjectByKey(new SimpleObjectKey(name));
	}

	internal override ObjectKeyBase CreateKeyFromUrn(Urn urn)
	{
		string attribute = urn.GetAttribute("Name");
		if (attribute == null || attribute.Length == 0)
		{
			throw new SmoException(ExceptionTemplatesImpl.PropertyMustBeSpecifiedInUrn("Name", urn.Type));
		}
		return new SimpleObjectKey(attribute);
	}
}
