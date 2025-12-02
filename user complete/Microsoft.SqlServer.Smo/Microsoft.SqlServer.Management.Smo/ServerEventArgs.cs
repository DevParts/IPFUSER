using System;
using System.Management;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ServerEventArgs : EventArgs
{
	private EventPropertyCollection properties;

	private EventType eventType;

	public EventType EventType => eventType;

	public DateTime PostTime => (DateTime)properties["PostTime"].Value;

	public int Spid => (int)properties["SPID"].Value;

	public string SqlInstance => (string)properties["SQLInstance"].Value;

	public EventPropertyCollection Properties => properties;

	internal ServerEventArgs(EventType eventType, PropertyDataCollection properties)
	{
		this.eventType = eventType;
		this.properties = new EventPropertyCollection(properties);
		this.properties.Add("EventClass", this.eventType);
	}
}
