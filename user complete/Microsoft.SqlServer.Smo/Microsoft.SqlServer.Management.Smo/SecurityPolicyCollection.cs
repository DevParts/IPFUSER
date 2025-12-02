using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class SecurityPolicyCollection : SchemaCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public SecurityPolicy this[int index] => GetObjectByIndex(index) as SecurityPolicy;

	public SecurityPolicy this[string name]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema())) as SecurityPolicy;
		}
	}

	public SecurityPolicy this[string name, string schema]
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
			return GetObjectByKey(new SchemaObjectKey(name, schema)) as SecurityPolicy;
		}
	}

	internal SecurityPolicyCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(SecurityPolicy[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public SecurityPolicy ItemById(int id)
	{
		return (SecurityPolicy)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(SecurityPolicy);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new SecurityPolicy(this, key, state);
	}

	public void Add(SecurityPolicy securityPolicy)
	{
		if (securityPolicy == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("securityPolicy"));
		}
		AddImpl(securityPolicy);
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
