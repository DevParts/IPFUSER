namespace Microsoft.SqlServer.Management.Smo.Agent;

public class AgentObjectBase : NamedSmoObject
{
	internal AgentObjectBase(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		SetServerObject(parentColl.ParentInstance.GetServerObject());
		if (base.ParentColl.ParentInstance is JobServer)
		{
			m_comparer = ((JobServer)base.ParentColl.ParentInstance).Parent.StringComparer;
		}
		else
		{
			m_comparer = ((AgentObjectBase)base.ParentColl.ParentInstance).StringComparer;
		}
	}

	internal AgentObjectBase(ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
	}

	internal AgentObjectBase(string name)
	{
		Name = name;
	}

	protected internal AgentObjectBase()
	{
	}

	protected internal override string GetDBName()
	{
		return "msdb";
	}
}
