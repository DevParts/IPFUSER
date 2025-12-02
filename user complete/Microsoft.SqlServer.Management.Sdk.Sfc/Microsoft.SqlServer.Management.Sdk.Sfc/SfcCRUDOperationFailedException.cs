using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcCRUDOperationFailedException : SfcException
{
	public SfcCRUDOperationFailedException()
	{
	}

	public SfcCRUDOperationFailedException(string message)
		: base(message)
	{
	}

	public SfcCRUDOperationFailedException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcCRUDOperationFailedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
