using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcInvalidMoveException : SfcException
{
	public SfcInvalidMoveException()
	{
	}

	public SfcInvalidMoveException(string message)
		: base(message)
	{
	}

	public SfcInvalidMoveException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcInvalidMoveException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
