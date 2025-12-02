using System;
using System.Management;

namespace Microsoft.SqlServer.Management.Smo;

internal sealed class TableEventsWorker : ObjectInSchemaEventsWorker
{
	public TableEventsWorker(Table target)
		: base(target, typeof(TableEventSet), typeof(TableEventValues))
	{
	}

	protected override EventQuery CreateWqlQuery(string eventClass)
	{
		bool flag = false;
		string objectType = string.Empty;
		if (eventClass.IndexOf("_INDEX", StringComparison.Ordinal) > 0)
		{
			flag = true;
			objectType = "Index";
		}
		else if (eventClass.IndexOf("_STATISTICS", StringComparison.Ordinal) > 0)
		{
			flag = true;
			objectType = "Statistics";
		}
		if (flag)
		{
			Table table = (Table)Target;
			return EventsWorkerBase.CreateWqlQueryForTargetObject(eventClass, table.ParentColl.ParentInstance.InternalName, table.Schema, objectType, table.Name, "TABLE");
		}
		return base.CreateWqlQuery(eventClass);
	}
}
