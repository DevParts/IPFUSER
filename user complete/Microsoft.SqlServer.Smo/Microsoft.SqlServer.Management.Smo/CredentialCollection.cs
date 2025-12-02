using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class CredentialCollection : SimpleObjectCollectionBase
{
	public Server Parent => base.ParentInstance as Server;

	public Credential this[int index] => GetObjectByIndex(index) as Credential;

	public Credential this[string name] => GetObjectByName(name) as Credential;

	internal CredentialCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Credential[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public Credential ItemById(int id)
	{
		return (Credential)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Credential);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Credential(this, key, state);
	}

	public void Add(Credential credential)
	{
		AddImpl(credential);
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
