using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class LinkedServerLoginCollection : SimpleObjectCollectionBase
{
	public LinkedServer Parent => base.ParentInstance as LinkedServer;

	public LinkedServerLogin this[int index] => GetObjectByIndex(index) as LinkedServerLogin;

	public LinkedServerLogin this[string name] => GetObjectByName(name) as LinkedServerLogin;

	internal LinkedServerLoginCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(LinkedServerLogin[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(LinkedServerLogin);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new LinkedServerLogin(this, key, state);
	}

	public void Add(LinkedServerLogin linkedServerLogin)
	{
		AddImpl(linkedServerLogin);
	}

	internal SqlSmoObject GetObjectByName(string name)
	{
		return GetObjectByKey(new SimpleObjectKey(name));
	}

	internal override ObjectKeyBase CreateKeyFromUrn(Urn urn)
	{
		string attribute = urn.GetAttribute("Name");
		if (attribute == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.PropertyMustBeSpecifiedInUrn("Name", urn.Type));
		}
		return new SimpleObjectKey(attribute);
	}
}
