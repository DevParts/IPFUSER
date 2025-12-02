using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
public sealed class ConnectionFailureException : ConnectionException
{
	public ConnectionFailureException()
	{
	}

	public ConnectionFailureException(string message)
		: base(message)
	{
	}

	public ConnectionFailureException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private ConnectionFailureException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
