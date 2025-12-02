using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class InvalidScriptingOptionException : SmoException
{
	public override SmoExceptionType SmoExceptionType => SmoExceptionType.InvalidScriptingOptionException;

	public InvalidScriptingOptionException()
	{
		Init();
	}

	public InvalidScriptingOptionException(string message)
		: base(message)
	{
		Init();
	}

	public InvalidScriptingOptionException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	private InvalidScriptingOptionException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		Init();
	}

	private void Init()
	{
		SetHelpContext("InvalidScriptingOptionException");
	}
}
