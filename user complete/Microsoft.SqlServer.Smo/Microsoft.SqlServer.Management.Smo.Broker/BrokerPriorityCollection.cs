using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Broker;

public sealed class BrokerPriorityCollection : SimpleObjectCollectionBase
{
	private StringComparer m_comparer;

	internal override StringComparer StringComparer => m_comparer;

	public ServiceBroker Parent => base.ParentInstance as ServiceBroker;

	public BrokerPriority this[int index] => GetObjectByIndex(index) as BrokerPriority;

	public BrokerPriority this[string name] => GetObjectByName(name) as BrokerPriority;

	internal BrokerPriorityCollection(SqlSmoObject parentInstance, StringComparer comparer)
		: base(parentInstance)
	{
		m_comparer = comparer;
	}

	public void CopyTo(BrokerPriority[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public BrokerPriority ItemById(int id)
	{
		return (BrokerPriority)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(BrokerPriority);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new BrokerPriority(this, key, state);
	}

	public void Add(BrokerPriority brokerPriority)
	{
		AddImpl(brokerPriority);
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
