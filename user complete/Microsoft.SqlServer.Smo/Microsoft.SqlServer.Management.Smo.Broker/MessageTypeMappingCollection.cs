using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Broker;

public sealed class MessageTypeMappingCollection : SimpleObjectCollectionBase
{
	public ServiceContract Parent => base.ParentInstance as ServiceContract;

	public MessageTypeMapping this[int index] => GetObjectByIndex(index) as MessageTypeMapping;

	public MessageTypeMapping this[string name] => GetObjectByName(name) as MessageTypeMapping;

	internal MessageTypeMappingCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(MessageTypeMapping[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(MessageTypeMapping);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new MessageTypeMapping(this, key, state);
	}

	public void Remove(MessageTypeMapping messageTypeMapping)
	{
		if (messageTypeMapping == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("messageTypeMapping"));
		}
		RemoveObj(messageTypeMapping, new SimpleObjectKey(messageTypeMapping.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(MessageTypeMapping messageTypeMapping)
	{
		AddImpl(messageTypeMapping);
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
