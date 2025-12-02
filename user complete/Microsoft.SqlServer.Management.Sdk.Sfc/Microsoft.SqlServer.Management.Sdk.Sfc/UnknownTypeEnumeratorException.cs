using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class UnknownTypeEnumeratorException : EnumeratorException
{
	public UnknownTypeEnumeratorException()
	{
		base.HResult = 4;
	}

	public UnknownTypeEnumeratorException(string typeName)
		: base(SfcStrings.UnknownType(typeName))
	{
		base.HResult = 4;
	}

	public UnknownTypeEnumeratorException(string typeName, Exception innerException)
		: base(SfcStrings.UnknownType(typeName), innerException)
	{
		base.HResult = 4;
	}

	private UnknownTypeEnumeratorException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		base.HResult = 4;
	}
}
