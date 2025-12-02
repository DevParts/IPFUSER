using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
public sealed class ExecutionFailureException : ConnectionException
{
	public ExecutionFailureException()
	{
	}

	public ExecutionFailureException(string message)
		: base(message)
	{
	}

	public ExecutionFailureException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private ExecutionFailureException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
