using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class WrongPropertyValueException : SmoException
{
	private Property property;

	public override SmoExceptionType SmoExceptionType => SmoExceptionType.WrongPropertyValueException;

	public override string Message
	{
		get
		{
			new StringBuilder();
			if (property != null)
			{
				return string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.WrongPropertyValueExceptionText(property.Name, (property.Value != null) ? property.Value.ToString() : string.Empty));
			}
			return base.Message;
		}
	}

	public Property Property => property;

	public WrongPropertyValueException()
	{
		Init();
	}

	public WrongPropertyValueException(string message)
		: base(message)
	{
		Init();
	}

	public WrongPropertyValueException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	public WrongPropertyValueException(Property propertyObject)
	{
		property = propertyObject;
		Data["HelpLink.EvtData1"] = propertyObject.Name;
		Init();
	}

	private WrongPropertyValueException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		property = (Property)info.GetValue("property", typeof(Property));
		Init();
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("property", property);
		base.GetObjectData(info, context);
	}

	private void Init()
	{
		SetHelpContext("WrongPropertyValueExceptionText");
	}
}
