using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class SchemaCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public Schema this[int index] => GetObjectByIndex(index) as Schema;

	public Schema this[string name] => GetObjectByName(name) as Schema;

	internal SchemaCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Schema[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public Schema ItemById(int id)
	{
		return (Schema)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Schema);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Schema(this, key, state);
	}

	public void Add(Schema schema)
	{
		AddImpl(schema);
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
