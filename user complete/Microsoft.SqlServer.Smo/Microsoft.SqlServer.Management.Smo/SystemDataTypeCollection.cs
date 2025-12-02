using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class SystemDataTypeCollection : SimpleObjectCollectionBase
{
	public Server Parent => base.ParentInstance as Server;

	public SystemDataType this[int index] => GetObjectByIndex(index) as SystemDataType;

	public SystemDataType this[string name] => GetObjectByName(name) as SystemDataType;

	internal SystemDataTypeCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(SystemDataType[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public SystemDataType ItemById(int id)
	{
		return (SystemDataType)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(SystemDataType);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new SystemDataType(this, key, state);
	}

	public void Add(SystemDataType dataType)
	{
		AddImpl(dataType);
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
