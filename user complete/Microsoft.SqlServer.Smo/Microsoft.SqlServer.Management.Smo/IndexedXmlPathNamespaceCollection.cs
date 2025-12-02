using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class IndexedXmlPathNamespaceCollection : SimpleObjectCollectionBase
{
	public Index Parent => base.ParentInstance as Index;

	public IndexedXmlPathNamespace this[int index] => GetObjectByIndex(index) as IndexedXmlPathNamespace;

	public IndexedXmlPathNamespace this[string name] => GetObjectByName(name) as IndexedXmlPathNamespace;

	internal IndexedXmlPathNamespaceCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(IndexedXmlPathNamespace[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(IndexedXmlPathNamespace);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new IndexedXmlPathNamespace(this, key, state);
	}

	public void Remove(IndexedXmlPathNamespace indexedXmlPathNamespace)
	{
		if (indexedXmlPathNamespace == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("indexedXmlPathNamespace"));
		}
		RemoveObj(indexedXmlPathNamespace, new SimpleObjectKey(indexedXmlPathNamespace.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(IndexedXmlPathNamespace indexedXmlPathNamespace)
	{
		AddImpl(indexedXmlPathNamespace);
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
