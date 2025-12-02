using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class RuleCollection : SchemaCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public Rule this[int index] => GetObjectByIndex(index) as Rule;

	public Rule this[string name]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema())) as Rule;
		}
	}

	public Rule this[string name, string schema]
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
			return GetObjectByKey(new SchemaObjectKey(name, schema)) as Rule;
		}
	}

	internal RuleCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Rule[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public Rule ItemById(int id)
	{
		return (Rule)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Rule);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Rule(this, key, state);
	}

	public void Add(Rule rule)
	{
		if (rule == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("rule"));
		}
		AddImpl(rule);
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
