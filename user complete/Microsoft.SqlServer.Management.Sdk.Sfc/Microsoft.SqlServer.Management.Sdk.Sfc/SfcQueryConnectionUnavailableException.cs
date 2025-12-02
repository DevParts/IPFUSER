using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcQueryConnectionUnavailableException : SfcException
{
	public override string Message => SfcStrings.SfcQueryConnectionUnavailable;

	public SfcQueryConnectionUnavailableException()
	{
	}

	public SfcQueryConnectionUnavailableException(string message)
		: base(message)
	{
	}

	public SfcQueryConnectionUnavailableException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcQueryConnectionUnavailableException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
