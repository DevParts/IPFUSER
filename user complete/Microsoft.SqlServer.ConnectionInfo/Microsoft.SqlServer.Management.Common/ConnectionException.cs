using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
public class ConnectionException : SystemException
{
	public ConnectionException()
	{
	}

	public ConnectionException(string message)
		: base(message)
	{
	}

	public ConnectionException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected ConnectionException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
