using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

internal sealed class UserPermissionCollection : SimpleObjectCollectionBase
{
	public SqlSmoObject Parent => base.ParentInstance;

	public UserPermission this[int index] => GetObjectByIndex(index) as UserPermission;

	public UserPermission this[string name] => GetObjectByName(name) as UserPermission;

	internal UserPermissionCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(UserPermission[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public UserPermission ItemById(int id)
	{
		return (UserPermission)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(UserPermission);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new UserPermission(this, key, state);
	}

	internal SqlSmoObject GetObjectByName(string name)
	{
		return GetObjectByKey(new SimpleObjectKey(name));
	}
}
