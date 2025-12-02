using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcUnregisteredXmlDomainException : SfcException
{
	public SfcUnregisteredXmlDomainException()
	{
	}

	public SfcUnregisteredXmlDomainException(string message)
		: base(message)
	{
	}

	public SfcUnregisteredXmlDomainException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcUnregisteredXmlDomainException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
