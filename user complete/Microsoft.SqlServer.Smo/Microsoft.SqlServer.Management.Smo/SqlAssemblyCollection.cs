using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class SqlAssemblyCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public SqlAssembly this[int index] => GetObjectByIndex(index) as SqlAssembly;

	public SqlAssembly this[string name] => GetObjectByName(name) as SqlAssembly;

	internal SqlAssemblyCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(SqlAssembly[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public SqlAssembly ItemById(int id)
	{
		return (SqlAssembly)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(SqlAssembly);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new SqlAssembly(this, key, state);
	}

	public void Add(SqlAssembly sqlAssembly)
	{
		AddImpl(sqlAssembly);
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
