using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class UnsupportedCompatLevelException : SmoException
{
	public override SmoExceptionType SmoExceptionType => SmoExceptionType.UnsupportedCompatLevelException;

	public UnsupportedCompatLevelException()
	{
		Init();
	}

	public UnsupportedCompatLevelException(string message)
		: base(message)
	{
		Init();
	}

	public UnsupportedCompatLevelException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	private UnsupportedCompatLevelException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		Init();
	}

	private void Init()
	{
		SetHelpContext("UnsupportedCompatLevelException");
	}
}
