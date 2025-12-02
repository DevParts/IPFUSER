using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class UserCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public User this[int index] => GetObjectByIndex(index) as User;

	public User this[string name] => GetObjectByName(name) as User;

	internal UserCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(User[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public User ItemById(int id)
	{
		return (User)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(User);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new User(this, key, state);
	}

	public void Add(User user)
	{
		AddImpl(user);
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
