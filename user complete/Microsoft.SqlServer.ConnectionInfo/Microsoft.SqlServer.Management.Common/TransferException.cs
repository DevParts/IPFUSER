using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
public class TransferException : SqlServerManagementException
{
	public TransferException()
	{
	}

	public TransferException(string message)
		: base(message)
	{
	}

	public TransferException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected TransferException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
