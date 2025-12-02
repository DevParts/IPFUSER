using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class SymmetricKeyCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public SymmetricKey this[int index] => GetObjectByIndex(index) as SymmetricKey;

	public SymmetricKey this[string name] => GetObjectByName(name) as SymmetricKey;

	internal SymmetricKeyCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(SymmetricKey[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public SymmetricKey ItemById(int id)
	{
		return (SymmetricKey)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(SymmetricKey);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new SymmetricKey(this, key, state);
	}

	public void Add(SymmetricKey symmetricKey)
	{
		AddImpl(symmetricKey);
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
