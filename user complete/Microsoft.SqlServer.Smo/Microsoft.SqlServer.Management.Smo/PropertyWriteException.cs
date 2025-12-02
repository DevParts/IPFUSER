using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class PropertyWriteException : SmoException
{
	private string propertyName;

	private string objectKind;

	private string objectName;

	private string reason;

	public override SmoExceptionType SmoExceptionType => SmoExceptionType.PropertyWriteException;

	public override string Message => ExceptionTemplatesImpl.FailedToWriteProperty(propertyName, objectKind, objectName, reason);

	public string PropertyName => propertyName;

	public PropertyWriteException()
	{
		Init();
	}

	public PropertyWriteException(string message)
		: base(message)
	{
		Init();
	}

	public PropertyWriteException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	public PropertyWriteException(string propertyName, string objectKind, string objectName, string reason)
	{
		Init();
		this.propertyName = propertyName;
		this.objectKind = objectKind;
		this.objectName = objectName;
		this.reason = reason;
		Data["HelpLink.EvtData1"] = propertyName;
	}

	private PropertyWriteException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		propertyName = info.GetString("propertyName");
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("propertyName", propertyName);
		base.GetObjectData(info, context);
	}

	private void Init()
	{
		propertyName = string.Empty;
		objectKind = string.Empty;
		objectName = string.Empty;
		reason = string.Empty;
		SetHelpContext("PropertyWriteException");
	}
}
