using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class SfcDependencyException : SmoException
{
	public override SmoExceptionType SmoExceptionType => SmoExceptionType.SfcDependencyException;

	public SfcDependencyException()
	{
		Init();
	}

	public SfcDependencyException(string message)
		: base(message)
	{
		Init();
	}

	public SfcDependencyException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	private SfcDependencyException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		Init();
	}

	private void Init()
	{
		SetHelpContext("SfcDependencyException");
	}
}
