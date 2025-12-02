using System;
using System.Management;

namespace Microsoft.SqlServer.Management.Smo;

internal sealed class ViewEventsWorker : ObjectInSchemaEventsWorker
{
	public ViewEventsWorker(View target)
		: base(target, typeof(ViewEventSet), typeof(ViewEventValues))
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
			View view = (View)Target;
			return EventsWorkerBase.CreateWqlQueryForTargetObject(eventClass, view.ParentColl.ParentInstance.InternalName, view.Schema, objectType, view.Name, "VIEW");
		}
		return base.CreateWqlQuery(eventClass);
	}
}
