using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class UnsupportedVersionException : SmoException
{
	public override SmoExceptionType SmoExceptionType => SmoExceptionType.UnsupportedVersionException;

	public UnsupportedVersionException()
	{
		Init();
	}

	public UnsupportedVersionException(string message)
		: base(message)
	{
		Init();
	}

	public UnsupportedVersionException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	private UnsupportedVersionException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		Init();
	}

	private void Init()
	{
		SetHelpContext("UnsupportedVersion");
	}
}
