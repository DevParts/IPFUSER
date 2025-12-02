using System;

namespace Microsoft.SqlServer.Management.Common;

public class DataTransferProgressEventArgs : EventArgs
{
	private DataTransferProgressEventType eventType;

	private long progressCount;

	private string transferId;

	private Exception ex;

	public DataTransferProgressEventType DataTransferProgressEventType => eventType;

	public string TransferId => transferId;

	public long ProgressCount => progressCount;

	public Exception Exception => ex;

	public DataTransferProgressEventArgs(DataTransferProgressEventType eventType, string transferId, long progressCount, Exception ex)
	{
		this.eventType = eventType;
		this.transferId = transferId;
		this.progressCount = progressCount;
		this.ex = ex;
	}
}
