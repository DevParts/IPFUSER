using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcInvalidStreamException : SfcException
{
	public SfcInvalidStreamException()
	{
	}

	public SfcInvalidStreamException(string message)
		: base(message)
	{
	}

	public SfcInvalidStreamException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcInvalidStreamException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
