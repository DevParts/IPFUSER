using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Broker;

public sealed class BrokerServiceCollection : SimpleObjectCollectionBase
{
	private StringComparer m_comparer;

	internal override StringComparer StringComparer => m_comparer;

	public ServiceBroker Parent => base.ParentInstance as ServiceBroker;

	public BrokerService this[int index] => GetObjectByIndex(index) as BrokerService;

	public BrokerService this[string name] => GetObjectByName(name) as BrokerService;

	internal BrokerServiceCollection(SqlSmoObject parentInstance, StringComparer comparer)
		: base(parentInstance)
	{
		m_comparer = comparer;
	}

	public void CopyTo(BrokerService[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public BrokerService ItemById(int id)
	{
		return (BrokerService)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(BrokerService);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new BrokerService(this, key, state);
	}

	public void Add(BrokerService brokerService)
	{
		AddImpl(brokerService);
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
