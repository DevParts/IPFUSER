using System;

namespace Microsoft.SqlServer.Management.Smo;

internal sealed class ExecuteNonQueryCompletedEventArgs : EventArgs
{
	private ExecutionStatus executionStatus;

	private Exception lastException;

	public ExecutionStatus ExecutionStatus => executionStatus;

	public Exception LastException => lastException;

	internal ExecuteNonQueryCompletedEventArgs(ExecutionStatus status, Exception lastException)
	{
		executionStatus = status;
		this.lastException = lastException;
	}
}
