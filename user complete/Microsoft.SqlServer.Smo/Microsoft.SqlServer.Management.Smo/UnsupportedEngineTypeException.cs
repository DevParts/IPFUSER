using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class UnsupportedEngineTypeException : SmoException
{
	public override SmoExceptionType SmoExceptionType => SmoExceptionType.UnsupportedEngineTypeException;

	public UnsupportedEngineTypeException()
	{
		Init();
	}

	public UnsupportedEngineTypeException(string message)
		: base(message)
	{
		Init();
	}

	public UnsupportedEngineTypeException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	private UnsupportedEngineTypeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		Init();
	}

	private void Init()
	{
		SetHelpContext("UnsupportedEngineType");
	}
}
