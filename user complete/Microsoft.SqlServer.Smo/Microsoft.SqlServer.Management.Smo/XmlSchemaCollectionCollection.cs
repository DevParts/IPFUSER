using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class XmlSchemaCollectionCollection : SchemaCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public XmlSchemaCollection this[int index] => GetObjectByIndex(index) as XmlSchemaCollection;

	public XmlSchemaCollection this[string name]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema())) as XmlSchemaCollection;
		}
	}

	public XmlSchemaCollection this[string name, string schema]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			if (schema == null)
			{
				throw new ArgumentNullException("schema cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, schema)) as XmlSchemaCollection;
		}
	}

	internal XmlSchemaCollectionCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(XmlSchemaCollection[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public XmlSchemaCollection ItemById(int id)
	{
		return (XmlSchemaCollection)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(XmlSchemaCollection);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new XmlSchemaCollection(this, key, state);
	}

	public void Add(XmlSchemaCollection xmlSchemaCollection)
	{
		if (xmlSchemaCollection == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("xmlSchemaCollection"));
		}
		AddImpl(xmlSchemaCollection);
	}

	internal SqlSmoObject GetObjectByName(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("schema cannot be null");
		}
		return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema()));
	}
}
