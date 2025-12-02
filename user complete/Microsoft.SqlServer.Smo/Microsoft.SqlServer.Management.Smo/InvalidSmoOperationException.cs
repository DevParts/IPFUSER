using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class InvalidSmoOperationException : SmoException
{
	private string opName;

	private SqlSmoState state;

	public override SmoExceptionType SmoExceptionType => SmoExceptionType.InvalidSmoOperationException;

	public override string Message
	{
		get
		{
			if (opName.Length == 0)
			{
				return base.Message;
			}
			return ExceptionTemplatesImpl.InvalidSmoOperationExceptionText(opName, state.ToString());
		}
	}

	public InvalidSmoOperationException()
	{
		Init();
	}

	public InvalidSmoOperationException(string message)
		: base(message)
	{
		Init();
	}

	public InvalidSmoOperationException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	public InvalidSmoOperationException(string opName, SqlSmoState state)
	{
		this.opName = opName;
		this.state = state;
		Data["HelpLink.EvtData1"] = opName;
	}

	private InvalidSmoOperationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		opName = info.GetString("opName");
		state = (SqlSmoState)info.GetValue("state", typeof(SqlSmoState));
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("opName", opName);
		info.AddValue("state", state);
		base.GetObjectData(info, context);
	}

	private void Init()
	{
		opName = string.Empty;
		state = SqlSmoState.Creating;
		SetHelpContext("InvalidSmoOperationExceptionText");
	}
}
