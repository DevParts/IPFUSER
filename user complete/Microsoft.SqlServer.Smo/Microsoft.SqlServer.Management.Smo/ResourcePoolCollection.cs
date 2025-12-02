using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ResourcePoolCollection : SimpleObjectCollectionBase
{
	public ResourceGovernor Parent => base.ParentInstance as ResourceGovernor;

	public ResourcePool this[int index] => GetObjectByIndex(index) as ResourcePool;

	public ResourcePool this[string name] => GetObjectByName(name) as ResourcePool;

	internal ResourcePoolCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ResourcePool[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public ResourcePool ItemById(int id)
	{
		return (ResourcePool)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ResourcePool);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ResourcePool(this, key, state);
	}

	public void Add(ResourcePool resourcePool)
	{
		AddImpl(resourcePool);
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
