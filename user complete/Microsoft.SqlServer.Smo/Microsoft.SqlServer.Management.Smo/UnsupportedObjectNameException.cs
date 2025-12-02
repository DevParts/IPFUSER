using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class UnsupportedObjectNameException : SmoException
{
	public override SmoExceptionType SmoExceptionType => SmoExceptionType.UnsupportedObjectNameException;

	public UnsupportedObjectNameException()
	{
	}

	public UnsupportedObjectNameException(string message)
		: base(message)
	{
	}

	public UnsupportedObjectNameException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private UnsupportedObjectNameException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
