using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class UnsupportedFeatureException : SmoException
{
	public override SmoExceptionType SmoExceptionType => SmoExceptionType.UnsupportedFeatureException;

	public UnsupportedFeatureException()
	{
		Init();
	}

	public UnsupportedFeatureException(string message)
		: base(message)
	{
		Init();
	}

	public UnsupportedFeatureException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	private UnsupportedFeatureException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		Init();
	}

	private void Init()
	{
		SetHelpContext("UnsupportedFeatureException");
	}
}
