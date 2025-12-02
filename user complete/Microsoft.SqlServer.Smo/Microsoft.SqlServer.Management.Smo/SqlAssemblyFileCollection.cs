using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class SqlAssemblyFileCollection : SimpleObjectCollectionBase
{
	public SqlAssembly Parent => base.ParentInstance as SqlAssembly;

	public SqlAssemblyFile this[int index] => GetObjectByIndex(index) as SqlAssemblyFile;

	public SqlAssemblyFile this[string name] => GetObjectByName(name) as SqlAssemblyFile;

	internal SqlAssemblyFileCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(SqlAssemblyFile[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public SqlAssemblyFile ItemById(int id)
	{
		return (SqlAssemblyFile)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(SqlAssemblyFile);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new SqlAssemblyFile(this, key, state);
	}

	public void Add(SqlAssemblyFile file)
	{
		AddImpl(file);
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
