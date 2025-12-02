using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ApplicationRoleCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public ApplicationRole this[int index] => GetObjectByIndex(index) as ApplicationRole;

	public ApplicationRole this[string name] => GetObjectByName(name) as ApplicationRole;

	internal ApplicationRoleCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ApplicationRole[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public ApplicationRole ItemById(int id)
	{
		return (ApplicationRole)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ApplicationRole);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ApplicationRole(this, key, state);
	}

	public void Add(ApplicationRole applicationRole)
	{
		AddImpl(applicationRole);
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
