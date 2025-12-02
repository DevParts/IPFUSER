using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class DataFileCollection : SimpleObjectCollectionBase
{
	public FileGroup Parent => base.ParentInstance as FileGroup;

	public DataFile this[int index] => GetObjectByIndex(index) as DataFile;

	public DataFile this[string name] => GetObjectByName(name) as DataFile;

	internal DataFileCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(DataFile[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public DataFile ItemById(int id)
	{
		return (DataFile)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(DataFile);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new DataFile(this, key, state);
	}

	public void Remove(DataFile dataFile)
	{
		if (dataFile == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("dataFile"));
		}
		RemoveObj(dataFile, new SimpleObjectKey(dataFile.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(DataFile dataFile)
	{
		AddImpl(dataFile);
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
