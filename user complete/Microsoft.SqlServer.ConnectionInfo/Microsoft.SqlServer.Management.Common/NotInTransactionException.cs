using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
public sealed class NotInTransactionException : ConnectionException
{
	public NotInTransactionException()
	{
	}

	public NotInTransactionException(string message)
		: base(message)
	{
	}

	public NotInTransactionException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private NotInTransactionException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
