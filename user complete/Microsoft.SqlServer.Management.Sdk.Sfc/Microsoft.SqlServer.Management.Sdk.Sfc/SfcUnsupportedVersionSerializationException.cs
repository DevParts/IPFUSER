using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcUnsupportedVersionSerializationException : SfcException
{
	public SfcUnsupportedVersionSerializationException()
	{
	}

	public SfcUnsupportedVersionSerializationException(string message)
		: base(message)
	{
	}

	public SfcUnsupportedVersionSerializationException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcUnsupportedVersionSerializationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
