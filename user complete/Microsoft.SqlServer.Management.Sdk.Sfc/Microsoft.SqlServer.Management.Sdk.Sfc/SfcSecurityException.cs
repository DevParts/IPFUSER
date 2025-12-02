using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcSecurityException : SfcException
{
	public SfcSecurityException()
	{
	}

	public SfcSecurityException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public SfcSecurityException(string message)
		: base(message)
	{
	}

	private SfcSecurityException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
