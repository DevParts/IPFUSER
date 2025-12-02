using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ExternalLibraryCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public ExternalLibrary this[int index] => GetObjectByIndex(index) as ExternalLibrary;

	public ExternalLibrary this[string name] => GetObjectByName(name) as ExternalLibrary;

	internal ExternalLibraryCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ExternalLibrary[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public ExternalLibrary ItemById(int id)
	{
		return (ExternalLibrary)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ExternalLibrary);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ExternalLibrary(this, key, state);
	}

	public void Add(ExternalLibrary externalLibrary)
	{
		AddImpl(externalLibrary);
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
