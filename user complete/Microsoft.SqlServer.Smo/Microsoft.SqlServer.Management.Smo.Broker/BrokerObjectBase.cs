using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo.Broker;

public class BrokerObjectBase : ScriptNameObjectBase, IScriptable
{
	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public override string Name
	{
		get
		{
			return base.Name;
		}
		set
		{
			base.Name = value;
		}
	}

	internal BrokerObjectBase(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		SetServerObject(parentColl.ParentInstance.GetServerObject());
		if (base.ParentColl.ParentInstance is ServiceBroker)
		{
			m_comparer = ((ServiceBroker)base.ParentColl.ParentInstance).Parent.StringComparer;
		}
		else
		{
			m_comparer = ((BrokerObjectBase)base.ParentColl.ParentInstance).StringComparer;
		}
	}

	protected internal BrokerObjectBase()
	{
	}

	protected internal override string GetDBName()
	{
		if (base.ParentColl.ParentInstance is ServiceBroker)
		{
			return ((ServiceBroker)base.ParentColl.ParentInstance).Parent.Name;
		}
		return ((BrokerObjectBase)base.ParentColl.ParentInstance).GetDBName();
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}
}
