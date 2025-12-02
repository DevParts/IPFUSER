namespace Microsoft.SqlServer.Management.Smo;

public class MessageObjectBase : SqlSmoObject
{
	internal MessageObjectBase(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	internal MessageObjectBase(ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
	}

	protected internal MessageObjectBase()
	{
	}

	internal override ObjectKeyBase GetEmptyKey()
	{
		return new MessageObjectKey(0, null);
	}
}
