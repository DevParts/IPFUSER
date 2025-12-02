using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class FileGroupCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public FileGroup this[int index] => GetObjectByIndex(index) as FileGroup;

	public FileGroup this[string name] => GetObjectByName(name) as FileGroup;

	internal FileGroupCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(FileGroup[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public FileGroup ItemById(int id)
	{
		return (FileGroup)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(FileGroup);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new FileGroup(this, key, state);
	}

	public void Remove(FileGroup fileGroup)
	{
		if (fileGroup == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("fileGroup"));
		}
		RemoveObj(fileGroup, new SimpleObjectKey(fileGroup.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(FileGroup fileGroup)
	{
		AddImpl(fileGroup);
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
