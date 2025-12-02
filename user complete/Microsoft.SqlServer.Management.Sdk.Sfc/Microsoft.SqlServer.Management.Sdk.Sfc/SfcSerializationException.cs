using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcSerializationException : SfcException
{
	public SfcSerializationException()
	{
	}

	public SfcSerializationException(string message)
		: base(message)
	{
	}

	public SfcSerializationException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public SfcSerializationException(Exception innerException)
		: base(SfcStrings.SfcInvalidSerialization, innerException)
	{
	}

	private SfcSerializationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
