using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[Serializable]
public class SfcObjectNotFoundException : SfcException
{
	public SfcObjectNotFoundException()
	{
	}

	public SfcObjectNotFoundException(string message)
		: base(message)
	{
	}

	public SfcObjectNotFoundException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected SfcObjectNotFoundException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
