using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class AlertCategoryCollection : SimpleObjectCollectionBase
{
	public JobServer Parent => base.ParentInstance as JobServer;

	public AlertCategory this[int index] => GetObjectByIndex(index) as AlertCategory;

	public AlertCategory this[string name] => GetObjectByName(name) as AlertCategory;

	internal AlertCategoryCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(AlertCategory[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public AlertCategory ItemById(int id)
	{
		return (AlertCategory)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(AlertCategory);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new AlertCategory(this, key, state);
	}

	public void Remove(AlertCategory alertCategory)
	{
		if (alertCategory == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("alertCategory"));
		}
		RemoveObj(alertCategory, new SimpleObjectKey(alertCategory.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(AlertCategory alertCategory)
	{
		AddImpl(alertCategory);
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
