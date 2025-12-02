using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class LoginCollection : SimpleObjectCollectionBase
{
	public Server Parent => base.ParentInstance as Server;

	public Login this[int index] => GetObjectByIndex(index) as Login;

	public Login this[string name] => GetObjectByName(name) as Login;

	internal LoginCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Login[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public Login ItemById(int id)
	{
		return (Login)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Login);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Login(this, key, state);
	}

	public void Add(Login login)
	{
		AddImpl(login);
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
