using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class PropertyReadOnlyException : SmoException
{
	private string propertyName;

	public override SmoExceptionType SmoExceptionType => SmoExceptionType.PropertyReadOnlyException;

	public override string Message => ExceptionTemplatesImpl.PropertyReadOnlyExceptionText(propertyName);

	public string PropertyName => propertyName;

	public PropertyReadOnlyException()
	{
		propertyName = string.Empty;
		Init();
	}

	public PropertyReadOnlyException(string message, Exception innerException)
		: base(message, innerException)
	{
		propertyName = string.Empty;
		Init();
	}

	public PropertyReadOnlyException(string propertyName)
	{
		this.propertyName = propertyName;
		Init();
		Data["HelpLink.EvtData1"] = propertyName;
	}

	private PropertyReadOnlyException(SerializationInfo info, StreamingContext context)
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
		SetHelpContext("PropertyReadOnlyExceptionText");
	}
}
