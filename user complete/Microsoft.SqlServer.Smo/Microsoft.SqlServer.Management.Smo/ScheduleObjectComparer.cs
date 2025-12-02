using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

internal class ScheduleObjectComparer : ObjectComparerBase
{
	internal ScheduleObjectComparer(IComparer stringComparer)
		: base(stringComparer)
	{
	}

	public override int Compare(object obj1, object obj2)
	{
		ScheduleObjectKey scheduleObjectKey = obj1 as ScheduleObjectKey;
		ScheduleObjectKey scheduleObjectKey2 = obj2 as ScheduleObjectKey;
		if (scheduleObjectKey2.ID > -1 && scheduleObjectKey.ID > -1)
		{
			return scheduleObjectKey.ID - scheduleObjectKey2.ID;
		}
		return stringComparer.Compare(scheduleObjectKey.Name, scheduleObjectKey2.Name);
	}
}
