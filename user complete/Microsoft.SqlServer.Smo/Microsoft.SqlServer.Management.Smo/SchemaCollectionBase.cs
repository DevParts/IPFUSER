using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.Broker;

namespace Microsoft.SqlServer.Management.Smo;

public class SchemaCollectionBase : SortedListCollectionBase
{
	internal SchemaCollectionBase(SqlSmoObject parent)
		: base(parent)
	{
	}

	protected override void InitInnerCollection()
	{
		base.InternalStorage = new SmoSortedList(new SchemaObjectComparer(StringComparer));
	}

	internal void RemoveObject(string name, string schema)
	{
		base.InternalStorage.Remove(new SchemaObjectKey(name, schema));
	}

	public bool Contains(string name)
	{
		return Contains(new SchemaObjectKey(name, GetDefaultSchema()));
	}

	public bool Contains(string name, string schema)
	{
		return Contains(new SchemaObjectKey(name, schema));
	}

	internal virtual string GetDefaultSchema()
	{
		SqlSmoObject sqlSmoObject = null;
		sqlSmoObject = ((base.ParentInstance is Database) ? base.ParentInstance : ((base.ParentInstance is ServiceBroker) ? ((ServiceBroker)base.ParentInstance).Parent : ((!(base.ParentInstance.ParentColl.ParentInstance is Database)) ? base.ParentInstance.ParentColl.ParentInstance.ParentColl.ParentInstance : base.ParentInstance.ParentColl.ParentInstance)));
		if (sqlSmoObject.State == SqlSmoState.Creating || sqlSmoObject.IsDesignMode)
		{
			return "dbo";
		}
		return sqlSmoObject.Properties["DefaultSchema"].Value as string;
	}

	internal override ObjectKeyBase CreateKeyFromUrn(Urn urn)
	{
		string attribute = urn.GetAttribute("Name");
		if (attribute == null || (attribute.Length == 0 && !CanHaveEmptyName(urn)))
		{
			throw new SmoException(ExceptionTemplatesImpl.PropertyMustBeSpecifiedInUrn("Name", urn.Type));
		}
		string text = urn.GetAttribute("Schema");
		if (text == null || text.Length == 0)
		{
			text = GetDefaultSchema();
		}
		return new SchemaObjectKey(attribute, text);
	}
}
