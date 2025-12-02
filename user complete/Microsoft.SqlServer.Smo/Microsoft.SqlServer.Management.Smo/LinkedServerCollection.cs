using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class LinkedServerCollection : SimpleObjectCollectionBase
{
	public Server Parent => base.ParentInstance as Server;

	public LinkedServer this[int index] => GetObjectByIndex(index) as LinkedServer;

	public LinkedServer this[string name] => GetObjectByName(name) as LinkedServer;

	internal LinkedServerCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(LinkedServer[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public LinkedServer ItemById(int id)
	{
		return (LinkedServer)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(LinkedServer);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new LinkedServer(this, key, state);
	}

	public void Add(LinkedServer linkedServer)
	{
		AddImpl(linkedServer);
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
