using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcInvalidStateException : SfcException
{
	public SfcInvalidStateException()
	{
	}

	public SfcInvalidStateException(string message)
		: base(message)
	{
	}

	public SfcInvalidStateException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcInvalidStateException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
