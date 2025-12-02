using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class SynonymCollection : SchemaCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public Synonym this[int index] => GetObjectByIndex(index) as Synonym;

	public Synonym this[string name]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema())) as Synonym;
		}
	}

	public Synonym this[string name, string schema]
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
			return GetObjectByKey(new SchemaObjectKey(name, schema)) as Synonym;
		}
	}

	internal SynonymCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Synonym[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public Synonym ItemById(int id)
	{
		return (Synonym)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Synonym);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Synonym(this, key, state);
	}

	public void Add(Synonym synonym)
	{
		if (synonym == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("synonym"));
		}
		AddImpl(synonym);
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
