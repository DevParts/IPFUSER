using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcMissingParentException : SfcException
{
	public SfcMissingParentException()
	{
	}

	public SfcMissingParentException(string message)
		: base(message)
	{
	}

	public SfcMissingParentException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcMissingParentException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
