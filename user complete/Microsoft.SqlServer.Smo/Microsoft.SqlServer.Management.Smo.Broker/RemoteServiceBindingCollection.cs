using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Broker;

public sealed class RemoteServiceBindingCollection : SimpleObjectCollectionBase
{
	public ServiceBroker Parent => base.ParentInstance as ServiceBroker;

	public RemoteServiceBinding this[int index] => GetObjectByIndex(index) as RemoteServiceBinding;

	public RemoteServiceBinding this[string name] => GetObjectByName(name) as RemoteServiceBinding;

	internal RemoteServiceBindingCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(RemoteServiceBinding[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public RemoteServiceBinding ItemById(int id)
	{
		return (RemoteServiceBinding)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(RemoteServiceBinding);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new RemoteServiceBinding(this, key, state);
	}

	public void Add(RemoteServiceBinding remoteServiceBinding)
	{
		AddImpl(remoteServiceBinding);
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
