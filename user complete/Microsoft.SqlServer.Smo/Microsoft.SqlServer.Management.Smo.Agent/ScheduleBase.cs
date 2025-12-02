using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public class ScheduleBase : AgentObjectBase
{
	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID
	{
		get
		{
			int num = ((ScheduleObjectKey)key).ID;
			if (num == JobScheduleCollectionBase.GetDefaultID())
			{
				Property property = base.Properties["ID"];
				if (property.Retrieved || property.Dirty)
				{
					num = (int)property.Value;
				}
			}
			return num;
		}
	}

	internal ScheduleBase(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState eState)
		: base(parentColl, key, eState)
	{
	}

	protected internal ScheduleBase()
	{
	}

	protected void SetId(int id)
	{
		((ScheduleObjectKey)key).ID = id;
	}

	internal override ObjectKeyBase GetEmptyKey()
	{
		return new ScheduleObjectKey(null, JobScheduleCollectionBase.GetDefaultID());
	}
}
