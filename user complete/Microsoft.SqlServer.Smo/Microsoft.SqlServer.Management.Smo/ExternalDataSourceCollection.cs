using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ExternalDataSourceCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public ExternalDataSource this[int index] => GetObjectByIndex(index) as ExternalDataSource;

	public ExternalDataSource this[string name] => GetObjectByName(name) as ExternalDataSource;

	internal ExternalDataSourceCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ExternalDataSource[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public ExternalDataSource ItemById(int id)
	{
		return (ExternalDataSource)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ExternalDataSource);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ExternalDataSource(this, key, state);
	}

	public void Add(ExternalDataSource externalDataSource)
	{
		AddImpl(externalDataSource);
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
