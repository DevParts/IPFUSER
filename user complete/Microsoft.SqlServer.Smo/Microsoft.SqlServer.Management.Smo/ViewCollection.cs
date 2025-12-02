using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ViewCollection : SchemaCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public View this[int index] => GetObjectByIndex(index) as View;

	public View this[string name]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema())) as View;
		}
	}

	public View this[string name, string schema]
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
			return GetObjectByKey(new SchemaObjectKey(name, schema)) as View;
		}
	}

	internal ViewCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(View[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public View ItemById(int id)
	{
		return (View)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(View);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new View(this, key, state);
	}

	public void Add(View view)
	{
		if (view == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("view"));
		}
		AddImpl(view);
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
