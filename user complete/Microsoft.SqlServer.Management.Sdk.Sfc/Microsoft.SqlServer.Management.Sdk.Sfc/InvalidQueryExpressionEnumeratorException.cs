using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class InvalidQueryExpressionEnumeratorException : EnumeratorException
{
	public InvalidQueryExpressionEnumeratorException()
	{
		base.HResult = 1;
	}

	public InvalidQueryExpressionEnumeratorException(string message)
		: base(message)
	{
		base.HResult = 1;
	}

	public InvalidQueryExpressionEnumeratorException(string message, Exception innerException)
		: base(message, innerException)
	{
		base.HResult = 1;
	}

	private InvalidQueryExpressionEnumeratorException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		base.HResult = 1;
	}
}
