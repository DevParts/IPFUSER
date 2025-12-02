using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class InternalEnumeratorException : EnumeratorException
{
	public InternalEnumeratorException()
	{
		base.HResult = 7;
	}

	public InternalEnumeratorException(string message)
		: base(message)
	{
		base.HResult = 7;
	}

	public InternalEnumeratorException(string message, Exception innerException)
		: base(message, innerException)
	{
		base.HResult = 7;
	}

	private InternalEnumeratorException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		base.HResult = 7;
	}
}
