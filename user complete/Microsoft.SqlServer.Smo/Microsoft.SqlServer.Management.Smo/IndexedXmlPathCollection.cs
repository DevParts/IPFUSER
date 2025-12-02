using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class IndexedXmlPathCollection : ParameterCollectionBase
{
	public Index Parent => base.ParentInstance as Index;

	public IndexedXmlPath this[int index] => GetObjectByIndex(index) as IndexedXmlPath;

	public IndexedXmlPath this[string name] => GetObjectByKey(new SimpleObjectKey(name)) as IndexedXmlPath;

	internal IndexedXmlPathCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(IndexedXmlPath[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(IndexedXmlPath);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new IndexedXmlPath(this, key, state);
	}

	public void Add(IndexedXmlPath indexedXmlPath)
	{
		AddImpl(indexedXmlPath);
	}

	public void Add(IndexedXmlPath indexedXmlPath, string insertAtColumnName)
	{
		AddImpl(indexedXmlPath, new SimpleObjectKey(insertAtColumnName));
	}

	public void Add(IndexedXmlPath indexedXmlPath, int insertAtPosition)
	{
		AddImpl(indexedXmlPath, insertAtPosition);
	}

	public void Remove(IndexedXmlPath indexedXmlPath)
	{
		if (indexedXmlPath == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("indexedXmlPath"));
		}
		RemoveObj(indexedXmlPath, indexedXmlPath.key);
	}

	public IndexedXmlPath ItemById(int id)
	{
		return (IndexedXmlPath)GetItemById(id);
	}
}
