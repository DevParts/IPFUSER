using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class OperatorCategoryCollection : SimpleObjectCollectionBase
{
	public JobServer Parent => base.ParentInstance as JobServer;

	public OperatorCategory this[int index] => GetObjectByIndex(index) as OperatorCategory;

	public OperatorCategory this[string name] => GetObjectByName(name) as OperatorCategory;

	internal OperatorCategoryCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(OperatorCategory[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public OperatorCategory ItemById(int id)
	{
		return (OperatorCategory)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(OperatorCategory);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new OperatorCategory(this, key, state);
	}

	public void Remove(OperatorCategory operatorCategory)
	{
		if (operatorCategory == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("operatorCategory"));
		}
		RemoveObj(operatorCategory, new SimpleObjectKey(operatorCategory.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(OperatorCategory operatorCategory)
	{
		AddImpl(operatorCategory);
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
