using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class MissingObjectException : SmoException
{
	public override SmoExceptionType SmoExceptionType => SmoExceptionType.MissingObjectException;

	public override string Message => base.Message;

	public MissingObjectException()
	{
		Init();
	}

	public MissingObjectException(string message)
		: base(message)
	{
		Init();
	}

	public MissingObjectException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	private MissingObjectException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		Init();
	}

	private void Init()
	{
		SetHelpContext("ObjectDoesNotExist");
	}
}
