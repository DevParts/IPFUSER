using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
public sealed class DisconnectedConnectionException : ConnectionException
{
	public DisconnectedConnectionException()
	{
	}

	public DisconnectedConnectionException(string message)
		: base(message)
	{
	}

	public DisconnectedConnectionException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private DisconnectedConnectionException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
