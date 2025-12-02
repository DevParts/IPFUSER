using System.Management;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

internal sealed class ObjectEventsWorker : EventsWorkerBase
{
	private SqlSmoObject target;

	protected override SqlSmoObject Target => target;

	public ObjectEventsWorker(SqlSmoObject target)
		: base(target, typeof(ObjectEventSet), typeof(ObjectEventValues))
	{
		this.target = target;
	}

	protected override string GetEventClass(int eventID)
	{
		StringBuilder stringBuilder = new StringBuilder();
		switch (eventID)
		{
		case 0:
			stringBuilder.Append("Alter");
			break;
		case 1:
			stringBuilder.Append("Drop");
			break;
		}
		string text = target.GetType().Name;
		switch (text)
		{
		case "UserDefinedType":
			if (eventID == 0)
			{
				throw new SmoException(ExceptionTemplatesImpl.NotSupportedNotification(text, "Alter"));
			}
			text = "Type";
			break;
		case "ServerRole":
			text = "Role";
			break;
		case "Statistic":
			if (eventID == 0)
			{
				return "UPDATE_STATISTICS";
			}
			text = "Statistics";
			break;
		case "Synonym":
			if (eventID == 0)
			{
				throw new SmoException(ExceptionTemplatesImpl.NotSupportedNotification(text, "Alter"));
			}
			break;
		case "BrokerService":
			text = "Service";
			break;
		case "ServiceContract":
			text = "Contract";
			break;
		case "ServiceRoute":
			text = "Route";
			break;
		}
		stringBuilder.Append(text);
		return EventsWorkerBase.ConvertToEventClass(stringBuilder);
	}

	protected override EventQuery CreateWqlQuery(string eventClass)
	{
		EventQuery result = null;
		switch (target.GetType().Name)
		{
		case "Certificate":
		case "Login":
		case "ServerRole":
		case "Endpoint":
			result = EventsWorkerBase.CreateWqlQueryForServer(eventClass);
			break;
		case "ApplicationRole":
		case "UserDefinedType":
		case "PartitionFunction":
		case "PartitionScheme":
		case "Schema":
		case "Synonym":
		case "Sequence":
		case "Trigger":
		case "User":
		case "BrokerService":
		case "BrokerPriority":
		case "MessageType":
		case "RemoteServiceBinding":
		case "ServiceContract":
		case "ServiceRoute":
			result = EventsWorkerBase.CreateWqlQueryForDatabase(eventClass, target.GetDBName());
			break;
		case "Index":
		{
			TableViewBase tableViewBase2 = (TableViewBase)target.ParentColl.ParentInstance;
			result = EventsWorkerBase.CreateWqlQueryForTargetObject(eventClass, tableViewBase2.ParentColl.ParentInstance.InternalName, tableViewBase2.Schema, "Index", tableViewBase2.Name, (tableViewBase2 is Table) ? "TABLE" : "VIEW");
			break;
		}
		case "Statistic":
		{
			TableViewBase tableViewBase = (TableViewBase)target.ParentColl.ParentInstance;
			result = EventsWorkerBase.CreateWqlQueryForTargetObject(eventClass, tableViewBase.ParentColl.ParentInstance.InternalName, tableViewBase.Schema, "Statistics", tableViewBase.Name, (tableViewBase is Table) ? "TABLE" : "VIEW");
			break;
		}
		}
		return result;
	}
}
