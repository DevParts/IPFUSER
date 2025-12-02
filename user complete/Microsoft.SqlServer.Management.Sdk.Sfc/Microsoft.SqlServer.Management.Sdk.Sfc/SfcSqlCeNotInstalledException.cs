using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcSqlCeNotInstalledException : SfcException
{
	public SfcSqlCeNotInstalledException()
	{
	}

	public SfcSqlCeNotInstalledException(string message)
		: base(message)
	{
	}

	public SfcSqlCeNotInstalledException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private SfcSqlCeNotInstalledException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
