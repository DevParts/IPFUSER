using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class FullTextIndexColumnCollection : SimpleObjectCollectionBase
{
	public FullTextIndex Parent => base.ParentInstance as FullTextIndex;

	public FullTextIndexColumn this[int index] => GetObjectByIndex(index) as FullTextIndexColumn;

	public FullTextIndexColumn this[string name] => GetObjectByName(name) as FullTextIndexColumn;

	internal FullTextIndexColumnCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(FullTextIndexColumn[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(FullTextIndexColumn);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new FullTextIndexColumn(this, key, state);
	}

	public void Remove(FullTextIndexColumn fullTextIndexColumn)
	{
		if (fullTextIndexColumn == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("fullTextIndexColumn"));
		}
		RemoveObj(fullTextIndexColumn, new SimpleObjectKey(fullTextIndexColumn.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(FullTextIndexColumn fullTextIndexColumn)
	{
		AddImpl(fullTextIndexColumn);
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
