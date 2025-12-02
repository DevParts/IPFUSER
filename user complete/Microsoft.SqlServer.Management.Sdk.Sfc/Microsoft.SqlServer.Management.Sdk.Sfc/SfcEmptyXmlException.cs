using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcEmptyXmlException : SfcException
{
	public override string Message => SfcStrings.EmptySfcXml;

	public SfcEmptyXmlException()
	{
	}

	public SfcEmptyXmlException(string message)
		: base(message)
	{
	}

	public SfcEmptyXmlException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcEmptyXmlException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
