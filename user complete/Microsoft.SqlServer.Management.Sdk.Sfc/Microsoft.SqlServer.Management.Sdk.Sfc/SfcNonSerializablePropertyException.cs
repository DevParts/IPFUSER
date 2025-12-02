using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcNonSerializablePropertyException : SfcException
{
	public SfcNonSerializablePropertyException()
	{
	}

	public SfcNonSerializablePropertyException(string message)
		: base(message)
	{
	}

	public SfcNonSerializablePropertyException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcNonSerializablePropertyException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
