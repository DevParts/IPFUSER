using System;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class AsyncStatus
{
	private ExecutionStatus executionStatus;

	private Exception lastException;

	public ExecutionStatus ExecutionStatus => executionStatus;

	public Exception LastException => lastException;

	internal AsyncStatus()
	{
	}

	internal AsyncStatus(ExecutionStatus executionStatus, Exception lastException)
	{
		this.executionStatus = executionStatus;
		this.lastException = lastException;
	}
}
