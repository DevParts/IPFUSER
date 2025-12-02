using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class AuditCollection : SimpleObjectCollectionBase
{
	public Server Parent => base.ParentInstance as Server;

	public Audit this[int index] => GetObjectByIndex(index) as Audit;

	public Audit this[string name] => GetObjectByName(name) as Audit;

	internal AuditCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Audit[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public Audit ItemById(int id)
	{
		return (Audit)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Audit);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Audit(this, key, state);
	}

	public void Add(Audit audit)
	{
		AddImpl(audit);
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
