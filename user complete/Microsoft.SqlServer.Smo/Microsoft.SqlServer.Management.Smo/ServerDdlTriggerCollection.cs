using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ServerDdlTriggerCollection : SimpleObjectCollectionBase
{
	public Server Parent => base.ParentInstance as Server;

	public ServerDdlTrigger this[int index] => GetObjectByIndex(index) as ServerDdlTrigger;

	public ServerDdlTrigger this[string name] => GetObjectByName(name) as ServerDdlTrigger;

	internal ServerDdlTriggerCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ServerDdlTrigger[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public ServerDdlTrigger ItemById(int id)
	{
		return (ServerDdlTrigger)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ServerDdlTrigger);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ServerDdlTrigger(this, key, state);
	}

	public void Add(ServerDdlTrigger serverDdlTrigger)
	{
		AddImpl(serverDdlTrigger);
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
