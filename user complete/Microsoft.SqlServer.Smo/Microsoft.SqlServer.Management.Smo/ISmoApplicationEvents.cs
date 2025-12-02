namespace Microsoft.SqlServer.Management.Smo;

public interface ISmoApplicationEvents
{
	event SmoApplication.ObjectCreatedEventHandler ObjectCreated;

	event SmoApplication.ObjectDroppedEventHandler ObjectDropped;

	event SmoApplication.ObjectRenamedEventHandler ObjectRenamed;

	event SmoApplication.ObjectAlteredEventHandler ObjectAltered;

	event SmoApplication.AnyObjectEventHandler AnyObjectEvent;

	event SmoApplication.DatabaseEventHandler DatabaseEvent;
}
