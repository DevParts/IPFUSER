using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcDesignModeException : SfcException
{
	public SfcDesignModeException()
	{
	}

	public SfcDesignModeException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public SfcDesignModeException(string message)
		: base(message)
	{
	}

	private SfcDesignModeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
