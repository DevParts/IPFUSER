using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcInvalidArgumentException : SfcException
{
	public SfcInvalidArgumentException()
	{
	}

	public SfcInvalidArgumentException(string message)
		: base(message)
	{
	}

	public SfcInvalidArgumentException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcInvalidArgumentException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
