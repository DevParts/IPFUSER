using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcUnregisteredXmlTypeException : SfcException
{
	public SfcUnregisteredXmlTypeException()
	{
	}

	public SfcUnregisteredXmlTypeException(string message)
		: base(message)
	{
	}

	public SfcUnregisteredXmlTypeException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcUnregisteredXmlTypeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
