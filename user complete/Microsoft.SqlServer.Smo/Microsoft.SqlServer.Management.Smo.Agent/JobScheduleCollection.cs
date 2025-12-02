using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class JobScheduleCollection : JobScheduleCollectionBase
{
	public SqlSmoObject Parent => base.ParentInstance;

	public JobSchedule this[int index] => GetObjectByIndex(index) as JobSchedule;

	public JobSchedule this[string name] => GetObjectByKey(new ScheduleObjectKey(name, JobScheduleCollectionBase.GetDefaultID())) as JobSchedule;

	internal JobScheduleCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(JobSchedule[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public JobSchedule ItemById(int id)
	{
		IEnumerator enumerator = ((IEnumerable)this).GetEnumerator();
		while (enumerator.MoveNext())
		{
			JobSchedule jobSchedule = (JobSchedule)enumerator.Current;
			if (jobSchedule.ID == id)
			{
				return jobSchedule;
			}
		}
		return null;
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(JobSchedule);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new JobSchedule(this, key, state);
	}

	internal SqlSmoObject GetObjectByName(string name)
	{
		return GetObjectByKey(new ScheduleObjectKey(name, JobScheduleCollectionBase.GetDefaultID()));
	}
}
