using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcInvalidRenameException : SfcException
{
	public SfcInvalidRenameException()
	{
	}

	public SfcInvalidRenameException(string message)
		: base(message)
	{
	}

	public SfcInvalidRenameException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcInvalidRenameException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
