using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Broker;

public sealed class MessageTypeCollection : SimpleObjectCollectionBase
{
	private StringComparer m_comparer;

	internal override StringComparer StringComparer => m_comparer;

	public ServiceBroker Parent => base.ParentInstance as ServiceBroker;

	public MessageType this[int index] => GetObjectByIndex(index) as MessageType;

	public MessageType this[string name] => GetObjectByName(name) as MessageType;

	internal MessageTypeCollection(SqlSmoObject parentInstance, StringComparer comparer)
		: base(parentInstance)
	{
		m_comparer = comparer;
	}

	public void CopyTo(MessageType[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public MessageType ItemById(int id)
	{
		return (MessageType)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(MessageType);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new MessageType(this, key, state);
	}

	public void Add(MessageType messageType)
	{
		AddImpl(messageType);
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
