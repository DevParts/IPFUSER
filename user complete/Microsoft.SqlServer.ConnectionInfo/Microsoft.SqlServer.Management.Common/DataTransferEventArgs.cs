using System;

namespace Microsoft.SqlServer.Management.Common;

public class DataTransferEventArgs : EventArgs
{
	private DataTransferEventType eventType;

	private string message;

	public DataTransferEventType DataTransferEventType => eventType;

	public string Message => message;

	public DataTransferEventArgs(DataTransferEventType eventType, string message)
	{
		this.eventType = eventType;
		this.message = message;
	}
}
