using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class QueryNotSupportedEnumeratorException : EnumeratorException
{
	public QueryNotSupportedEnumeratorException()
	{
		base.HResult = 9;
	}

	public QueryNotSupportedEnumeratorException(string message)
		: base(message)
	{
		base.HResult = 9;
	}

	public QueryNotSupportedEnumeratorException(string message, Exception innerException)
		: base(message, innerException)
	{
		base.HResult = 9;
	}

	private QueryNotSupportedEnumeratorException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		base.HResult = 9;
	}
}
