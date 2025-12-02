using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcUnsupportedVersionException : SfcException
{
	public SfcUnsupportedVersionException()
	{
	}

	public SfcUnsupportedVersionException(string message)
		: base(message)
	{
	}

	public SfcUnsupportedVersionException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcUnsupportedVersionException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
