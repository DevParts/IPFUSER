using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ServerAuditSpecificationCollection : SimpleObjectCollectionBase
{
	public Server Parent => base.ParentInstance as Server;

	public ServerAuditSpecification this[int index] => GetObjectByIndex(index) as ServerAuditSpecification;

	public ServerAuditSpecification this[string name] => GetObjectByName(name) as ServerAuditSpecification;

	internal ServerAuditSpecificationCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ServerAuditSpecification[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public ServerAuditSpecification ItemById(int id)
	{
		return (ServerAuditSpecification)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ServerAuditSpecification);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ServerAuditSpecification(this, key, state);
	}

	public void Add(ServerAuditSpecification serverAuditSpecification)
	{
		AddImpl(serverAuditSpecification);
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
