using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class CertificateCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public Certificate this[int index] => GetObjectByIndex(index) as Certificate;

	public Certificate this[string name] => GetObjectByName(name) as Certificate;

	internal CertificateCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Certificate[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public Certificate ItemById(int id)
	{
		return (Certificate)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Certificate);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Certificate(this, key, state);
	}

	public void Add(Certificate certificate)
	{
		AddImpl(certificate);
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
