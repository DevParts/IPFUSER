using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
public sealed class InvalidArgumentException : ConnectionException
{
	public InvalidArgumentException()
	{
	}

	public InvalidArgumentException(string message)
		: base(message)
	{
	}

	public InvalidArgumentException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private InvalidArgumentException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
