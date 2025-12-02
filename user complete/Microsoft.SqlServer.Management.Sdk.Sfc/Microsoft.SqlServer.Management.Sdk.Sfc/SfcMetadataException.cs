using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public class SfcMetadataException : SfcException
{
	public SfcMetadataException()
	{
	}

	public SfcMetadataException(string message)
		: base(message)
	{
	}

	public SfcMetadataException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected SfcMetadataException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
