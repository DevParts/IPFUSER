using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class InvalidConfigurationFileEnumeratorException : EnumeratorException
{
	public InvalidConfigurationFileEnumeratorException()
	{
		base.HResult = 2;
	}

	public InvalidConfigurationFileEnumeratorException(string message)
		: base(message)
	{
		base.HResult = 2;
	}

	public InvalidConfigurationFileEnumeratorException(string message, Exception innerException)
		: base(message, innerException)
	{
		base.HResult = 2;
	}

	private InvalidConfigurationFileEnumeratorException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		base.HResult = 2;
	}
}
