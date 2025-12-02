using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class AvailabilityReplicaCollection : SimpleObjectCollectionBase
{
	private StringComparer m_comparer;

	internal override StringComparer StringComparer => m_comparer;

	public AvailabilityGroup Parent => base.ParentInstance as AvailabilityGroup;

	public AvailabilityReplica this[int index] => GetObjectByIndex(index) as AvailabilityReplica;

	public AvailabilityReplica this[string name] => GetObjectByName(name) as AvailabilityReplica;

	internal AvailabilityReplicaCollection(SqlSmoObject parentInstance, StringComparer comparer)
		: base(parentInstance)
	{
		m_comparer = comparer;
	}

	public void CopyTo(AvailabilityReplica[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public AvailabilityReplica ItemById(int id)
	{
		return (AvailabilityReplica)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(AvailabilityReplica);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new AvailabilityReplica(this, key, state);
	}

	public void Remove(AvailabilityReplica availabilityReplica)
	{
		if (availabilityReplica == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("availabilityReplica"));
		}
		RemoveObj(availabilityReplica, new SimpleObjectKey(availabilityReplica.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(AvailabilityReplica availabilityReplica)
	{
		AddImpl(availabilityReplica);
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
