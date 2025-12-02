using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
public sealed class ConnectionCannotBeChangedException : ConnectionException
{
	public ConnectionCannotBeChangedException()
	{
	}

	public ConnectionCannotBeChangedException(string message)
		: base(message)
	{
	}

	public ConnectionCannotBeChangedException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private ConnectionCannotBeChangedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
