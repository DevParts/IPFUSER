using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class PropertyCannotBeRetrievedException : SmoException
{
	private string reason = string.Empty;

	private string propertyName;

	private object failedObject;

	public override SmoExceptionType SmoExceptionType => SmoExceptionType.PropertyCannotBeRetrievedException;

	public override string Message
	{
		get
		{
			string text = null;
			text = ((propertyName == null || propertyName.Length <= 0 || failedObject == null) ? base.Message : ExceptionTemplatesImpl.PropertyCannotBeRetrievedExceptionText(propertyName, SqlSmoObject.GetTypeName(failedObject.GetType().Name), failedObject.ToString()));
			if (reason.Length > 0)
			{
				text = text + " " + reason;
			}
			return text;
		}
	}

	public string PropertyName => propertyName;

	public object FailedObject => failedObject;

	public PropertyCannotBeRetrievedException()
	{
		propertyName = string.Empty;
		Init();
	}

	public PropertyCannotBeRetrievedException(string message)
		: base(message)
	{
		propertyName = string.Empty;
		Init();
	}

	public PropertyCannotBeRetrievedException(string message, Exception innerException)
		: base(message, innerException)
	{
		propertyName = string.Empty;
		Init();
	}

	public PropertyCannotBeRetrievedException(string propertyName, object failedObject)
	{
		this.propertyName = propertyName;
		this.failedObject = failedObject;
		Init();
		Data["HelpLink.EvtData1"] = propertyName;
	}

	internal PropertyCannotBeRetrievedException(string propertyName, object failedObject, string reason)
	{
		this.propertyName = propertyName;
		this.failedObject = failedObject;
		this.reason = reason;
		Init();
		Data["HelpLink.EvtData1"] = propertyName;
	}

	private PropertyCannotBeRetrievedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		propertyName = info.GetString("propertyName");
		reason = info.GetString("reason");
		Init();
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("propertyName", propertyName);
		info.AddValue("reason", reason);
		base.GetObjectData(info, context);
	}

	private void Init()
	{
		SetHelpContext("PropertyCannotBeRetrievedExceptionText");
	}
}
