using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
public sealed class PropertyNotSetException : ConnectionException
{
	public PropertyNotSetException()
	{
	}

	public PropertyNotSetException(string message)
		: base(message)
	{
	}

	public PropertyNotSetException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private PropertyNotSetException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
