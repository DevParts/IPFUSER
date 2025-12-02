using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcInvalidKeyChainException : SfcException
{
	public override string Message => SfcStrings.InvalidKeyChain;

	public SfcInvalidKeyChainException()
	{
	}

	public SfcInvalidKeyChainException(string message)
		: base(message)
	{
	}

	public SfcInvalidKeyChainException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public SfcInvalidKeyChainException(Exception innerException)
		: base(SfcStrings.InvalidKeyChain, innerException)
	{
	}

	private SfcInvalidKeyChainException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
