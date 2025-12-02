using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcPathConversionException : SfcException
{
	public SfcPathConversionException()
	{
	}

	public SfcPathConversionException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public SfcPathConversionException(string message)
		: base(message)
	{
	}

	private SfcPathConversionException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
