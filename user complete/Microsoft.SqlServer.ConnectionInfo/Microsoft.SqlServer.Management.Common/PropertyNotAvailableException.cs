using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
public sealed class PropertyNotAvailableException : ConnectionException
{
	public PropertyNotAvailableException()
	{
	}

	public PropertyNotAvailableException(string message)
		: base(message)
	{
	}

	public PropertyNotAvailableException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private PropertyNotAvailableException(SerializationInfo si, StreamingContext sc)
	{
	}
}
