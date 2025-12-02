using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcObjectNotScriptableException : SfcException
{
	public SfcObjectNotScriptableException()
	{
	}

	public SfcObjectNotScriptableException(string message)
		: base(message)
	{
	}

	public SfcObjectNotScriptableException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcObjectNotScriptableException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
