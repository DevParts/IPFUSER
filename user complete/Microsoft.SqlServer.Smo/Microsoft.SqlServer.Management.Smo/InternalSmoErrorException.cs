using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class InternalSmoErrorException : SmoException
{
	public override SmoExceptionType SmoExceptionType => SmoExceptionType.InternalSmoErrorException;

	public InternalSmoErrorException()
	{
		Init();
	}

	public InternalSmoErrorException(string message)
		: base(message)
	{
		Init();
	}

	public InternalSmoErrorException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	private InternalSmoErrorException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		Init();
	}

	private void Init()
	{
		SetHelpContext("InternalSmoErrorException");
	}
}
