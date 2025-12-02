using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Broker;

public sealed class ServiceContractCollection : SimpleObjectCollectionBase
{
	private StringComparer m_comparer;

	internal override StringComparer StringComparer => m_comparer;

	public ServiceBroker Parent => base.ParentInstance as ServiceBroker;

	public ServiceContract this[int index] => GetObjectByIndex(index) as ServiceContract;

	public ServiceContract this[string name] => GetObjectByName(name) as ServiceContract;

	internal ServiceContractCollection(SqlSmoObject parentInstance, StringComparer comparer)
		: base(parentInstance)
	{
		m_comparer = comparer;
	}

	public void CopyTo(ServiceContract[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public ServiceContract ItemById(int id)
	{
		return (ServiceContract)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ServiceContract);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ServiceContract(this, key, state);
	}

	public void Add(ServiceContract serviceContract)
	{
		AddImpl(serviceContract);
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
