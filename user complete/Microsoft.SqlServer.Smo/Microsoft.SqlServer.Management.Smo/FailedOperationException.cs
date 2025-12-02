using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class FailedOperationException : SmoException
{
	private string operation = string.Empty;

	[NonSerialized]
	private object failedObject;

	private string reason = string.Empty;

	public override SmoExceptionType SmoExceptionType => SmoExceptionType.FailedOperationException;

	public string Operation
	{
		get
		{
			return operation;
		}
		set
		{
			operation = value;
		}
	}

	public object FailedObject
	{
		get
		{
			return failedObject;
		}
		set
		{
			failedObject = value;
		}
	}

	public override string Message
	{
		get
		{
			if (base.Message.Length <= 0 && operation != null && operation.Length > 0 && failedObject != null)
			{
				string empty = string.Empty;
				if (failedObject is SqlSmoObject)
				{
					empty = ExceptionTemplatesImpl.FailedOperationExceptionText(operation, SqlSmoObject.GetTypeName(failedObject.GetType().Name), ((SqlSmoObject)failedObject).key.GetExceptionName());
				}
				else if (failedObject is AbstractCollectionBase)
				{
					SqlSmoObject parentInstance = ((AbstractCollectionBase)failedObject).ParentInstance;
					empty = ExceptionTemplatesImpl.FailedOperationExceptionTextColl(operation, SqlSmoObject.GetTypeName(failedObject.GetType().Name), SqlSmoObject.GetTypeName(parentInstance.GetType().Name), parentInstance.key.GetExceptionName());
				}
				else
				{
					empty = ExceptionTemplatesImpl.FailedOperationExceptionText2(operation);
				}
				return empty + ((reason != null) ? (" " + reason) : string.Empty);
			}
			return base.Message;
		}
	}

	public FailedOperationException()
	{
		SetHelpContext("FailedOperationExceptionText");
	}

	public FailedOperationException(string message)
		: base(message)
	{
		SetHelpContext("FailedOperationExceptionText");
	}

	public FailedOperationException(string message, Exception innerException)
		: base(message, innerException)
	{
		SetHelpContext("FailedOperationExceptionText");
	}

	private FailedOperationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		reason = info.GetString("reason");
		SetHelpContext("FailedOperationExceptionText");
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("reason", reason);
		base.GetObjectData(info, context);
	}

	public FailedOperationException(string operation, object failedObject, Exception innerException)
		: base("", innerException)
	{
		this.operation = operation;
		this.failedObject = failedObject;
		SetHelpContext("FailedOperationExceptionText");
		Data.Add("HelpLink.EvtData1", operation);
		if (failedObject != null)
		{
			Data.Add("HelpLink.EvtData2", failedObject.GetType().Name);
		}
	}

	public FailedOperationException(string operation, object failedObject, Exception innerException, string reason)
		: base("", innerException)
	{
		this.operation = operation;
		this.failedObject = failedObject;
		this.reason = reason;
		SetHelpContext("FailedOperationExceptionText");
		Data.Add("HelpLink.EvtData1", operation);
		if (failedObject != null)
		{
			Data.Add("HelpLink.EvtData2", failedObject.GetType().Name);
		}
	}
}
