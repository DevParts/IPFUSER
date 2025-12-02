using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class UnsupportedEngineEditionException : SmoException
{
	public override SmoExceptionType SmoExceptionType => SmoExceptionType.UnsupportedEngineEditionException;

	public UnsupportedEngineEditionException()
	{
		Init();
	}

	public UnsupportedEngineEditionException(string message)
		: base(message)
	{
		Init();
	}

	public UnsupportedEngineEditionException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	private UnsupportedEngineEditionException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		Init();
	}

	private void Init()
	{
		SetHelpContext("UnsupportedEngineEdition");
	}
}
