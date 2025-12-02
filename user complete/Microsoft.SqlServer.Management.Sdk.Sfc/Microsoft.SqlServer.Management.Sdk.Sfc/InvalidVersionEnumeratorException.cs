using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class InvalidVersionEnumeratorException : EnumeratorException
{
	public InvalidVersionEnumeratorException()
	{
		base.HResult = 10;
	}

	public InvalidVersionEnumeratorException(string message)
		: base(message)
	{
		base.HResult = 10;
	}

	public InvalidVersionEnumeratorException(string message, Exception innerException)
		: base(message, innerException)
	{
		base.HResult = 10;
	}

	private InvalidVersionEnumeratorException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		base.HResult = 10;
	}
}
