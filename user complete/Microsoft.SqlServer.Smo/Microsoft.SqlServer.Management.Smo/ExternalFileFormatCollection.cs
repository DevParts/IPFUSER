using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ExternalFileFormatCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public ExternalFileFormat this[int index] => GetObjectByIndex(index) as ExternalFileFormat;

	public ExternalFileFormat this[string name] => GetObjectByName(name) as ExternalFileFormat;

	internal ExternalFileFormatCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ExternalFileFormat[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public ExternalFileFormat ItemById(int id)
	{
		return (ExternalFileFormat)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ExternalFileFormat);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ExternalFileFormat(this, key, state);
	}

	public void Add(ExternalFileFormat externalFileFormat)
	{
		AddImpl(externalFileFormat);
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
