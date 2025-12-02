using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcInvalidQueryExpressionException : SfcException
{
	public SfcInvalidQueryExpressionException()
	{
	}

	public SfcInvalidQueryExpressionException(string message)
		: base(message)
	{
	}

	public SfcInvalidQueryExpressionException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcInvalidQueryExpressionException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
