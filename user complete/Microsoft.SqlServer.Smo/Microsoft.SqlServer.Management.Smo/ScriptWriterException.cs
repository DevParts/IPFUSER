using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class ScriptWriterException : SmoException
{
	public override SmoExceptionType SmoExceptionType => SmoExceptionType.ScriptWriterException;

	public ScriptWriterException()
	{
		Init();
	}

	public ScriptWriterException(string message)
		: base(message)
	{
		Init();
	}

	public ScriptWriterException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	private ScriptWriterException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		Init();
	}

	private void Init()
	{
		SetHelpContext("ScriptWriterException");
	}
}
