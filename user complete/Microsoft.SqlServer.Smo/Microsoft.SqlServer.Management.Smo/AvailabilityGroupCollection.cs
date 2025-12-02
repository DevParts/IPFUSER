using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class AvailabilityGroupCollection : SimpleObjectCollectionBase
{
	public Server Parent => base.ParentInstance as Server;

	public AvailabilityGroup this[int index] => GetObjectByIndex(index) as AvailabilityGroup;

	public AvailabilityGroup this[string name] => GetObjectByName(name) as AvailabilityGroup;

	internal AvailabilityGroupCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(AvailabilityGroup[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public AvailabilityGroup ItemById(int id)
	{
		return (AvailabilityGroup)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(AvailabilityGroup);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new AvailabilityGroup(this, key, state);
	}

	public void Remove(AvailabilityGroup availabilityGroup)
	{
		if (availabilityGroup == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("availabilityGroup"));
		}
		RemoveObj(availabilityGroup, new SimpleObjectKey(availabilityGroup.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(AvailabilityGroup availabilityGroup)
	{
		AddImpl(availabilityGroup);
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
