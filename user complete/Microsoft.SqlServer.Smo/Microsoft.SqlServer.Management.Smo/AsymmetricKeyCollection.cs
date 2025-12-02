using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class AsymmetricKeyCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public AsymmetricKey this[int index] => GetObjectByIndex(index) as AsymmetricKey;

	public AsymmetricKey this[string name] => GetObjectByName(name) as AsymmetricKey;

	internal AsymmetricKeyCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(AsymmetricKey[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public AsymmetricKey ItemById(int id)
	{
		return (AsymmetricKey)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(AsymmetricKey);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new AsymmetricKey(this, key, state);
	}

	public void Add(AsymmetricKey asymmetricKey)
	{
		AddImpl(asymmetricKey);
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
