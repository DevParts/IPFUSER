using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class WorkloadGroupCollection : SimpleObjectCollectionBase
{
	public ResourcePool Parent => base.ParentInstance as ResourcePool;

	public WorkloadGroup this[int index] => GetObjectByIndex(index) as WorkloadGroup;

	public WorkloadGroup this[string name] => GetObjectByName(name) as WorkloadGroup;

	internal WorkloadGroupCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(WorkloadGroup[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public WorkloadGroup ItemById(int id)
	{
		return (WorkloadGroup)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(WorkloadGroup);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new WorkloadGroup(this, key, state);
	}

	public void Add(WorkloadGroup workloadGroup)
	{
		AddImpl(workloadGroup);
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
