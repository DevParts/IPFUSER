using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class UnknownPropertyEnumeratorException : EnumeratorException
{
	public UnknownPropertyEnumeratorException()
	{
		base.HResult = 3;
	}

	public UnknownPropertyEnumeratorException(string propertyName)
		: base(SfcStrings.UnknownProperty(propertyName))
	{
		base.HResult = 3;
	}

	public UnknownPropertyEnumeratorException(string propertyName, Exception innerException)
		: base(SfcStrings.UnknownProperty(propertyName), innerException)
	{
		base.HResult = 3;
	}

	private UnknownPropertyEnumeratorException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		base.HResult = 3;
	}
}
