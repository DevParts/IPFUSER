using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
public sealed class ChangePasswordFailureException : ConnectionException
{
	public ChangePasswordFailureException()
	{
	}

	public ChangePasswordFailureException(string message)
		: base(message)
	{
	}

	public ChangePasswordFailureException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private ChangePasswordFailureException(SerializationInfo si, StreamingContext sc)
		: base(si, sc)
	{
	}
}
