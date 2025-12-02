using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcInvalidXmlParentTypeException : SfcException
{
	public SfcInvalidXmlParentTypeException()
	{
	}

	public SfcInvalidXmlParentTypeException(string message)
		: base(message)
	{
	}

	public SfcInvalidXmlParentTypeException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcInvalidXmlParentTypeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
