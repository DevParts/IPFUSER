using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ServerRoleCollection : SimpleObjectCollectionBase
{
	public Server Parent => base.ParentInstance as Server;

	public ServerRole this[int index] => GetObjectByIndex(index) as ServerRole;

	public ServerRole this[string name] => GetObjectByName(name) as ServerRole;

	internal ServerRoleCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ServerRole[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public ServerRole ItemById(int id)
	{
		return (ServerRole)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ServerRole);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ServerRole(this, key, state);
	}

	public void Add(ServerRole serverRole)
	{
		AddImpl(serverRole);
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
