using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcNonSerializableTypeException : SfcException
{
	public SfcNonSerializableTypeException()
	{
	}

	public SfcNonSerializableTypeException(string message)
		: base(message)
	{
	}

	public SfcNonSerializableTypeException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcNonSerializableTypeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
