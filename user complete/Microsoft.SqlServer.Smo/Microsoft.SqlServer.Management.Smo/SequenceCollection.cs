using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class SequenceCollection : SchemaCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public Sequence this[int index] => GetObjectByIndex(index) as Sequence;

	public Sequence this[string name]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema())) as Sequence;
		}
	}

	public Sequence this[string name, string schema]
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
			return GetObjectByKey(new SchemaObjectKey(name, schema)) as Sequence;
		}
	}

	internal SequenceCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Sequence[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public Sequence ItemById(int id)
	{
		return (Sequence)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Sequence);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Sequence(this, key, state);
	}

	public void Add(Sequence sequence)
	{
		if (sequence == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("sequence"));
		}
		AddImpl(sequence);
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
