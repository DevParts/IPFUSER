using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class PropertyNotSetException : SmoException
{
	private string propertyName;

	public override SmoExceptionType SmoExceptionType => SmoExceptionType.PropertyNotSetException;

	public override string Message => string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.PropertyNotSetExceptionText(propertyName));

	public string PropertyName => propertyName;

	public PropertyNotSetException()
	{
		propertyName = string.Empty;
		Init();
	}

	public PropertyNotSetException(string message, Exception innerException)
		: base(message, innerException)
	{
		propertyName = string.Empty;
		Init();
	}

	public PropertyNotSetException(string propertyName)
	{
		this.propertyName = propertyName;
		Init();
	}

	private PropertyNotSetException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		propertyName = info.GetString("propertyName");
		Init();
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("propertyName", propertyName);
		base.GetObjectData(info, context);
	}

	private void Init()
	{
		SetHelpContext("PropertyNotSetExceptionText");
	}
}
