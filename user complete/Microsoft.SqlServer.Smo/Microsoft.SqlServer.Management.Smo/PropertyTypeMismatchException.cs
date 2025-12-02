using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class PropertyTypeMismatchException : SmoException
{
	private string propertyName;

	private string receivedType;

	private string expectedType;

	public override SmoExceptionType SmoExceptionType => SmoExceptionType.PropertyTypeMismatchException;

	public override string Message => ExceptionTemplatesImpl.PropertyTypeMismatchExceptionText(propertyName, receivedType, expectedType);

	public string PropertyName => propertyName;

	public string ReceivedType => receivedType;

	public string ExpectedType => expectedType;

	public PropertyTypeMismatchException()
	{
		Init();
	}

	public PropertyTypeMismatchException(string message)
		: base(message)
	{
		Init();
	}

	public PropertyTypeMismatchException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	public PropertyTypeMismatchException(string propertyName, string receivedType, string expectedType)
	{
		this.propertyName = propertyName;
		this.receivedType = receivedType;
		this.expectedType = expectedType;
		Data["HelpLink.EvtData1"] = propertyName;
	}

	private PropertyTypeMismatchException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		propertyName = info.GetString("propertyName");
		receivedType = info.GetString("receivedType");
		expectedType = info.GetString("expectedType");
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("propertyName", propertyName);
		info.AddValue("receivedType", receivedType);
		info.AddValue("expectedType", expectedType);
		base.GetObjectData(info, context);
	}

	private void Init()
	{
		propertyName = string.Empty;
		receivedType = string.Empty;
		expectedType = string.Empty;
		SetHelpContext("PropertyTypeMismatchExceptionText");
	}
}
