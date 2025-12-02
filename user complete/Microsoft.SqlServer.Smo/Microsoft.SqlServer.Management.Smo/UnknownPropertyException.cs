using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class UnknownPropertyException : SmoException
{
	private string propertyName;

	private ServerVersion[] supportedVersions;

	private ServerVersion currentVersion;

	public override SmoExceptionType SmoExceptionType => SmoExceptionType.UnknownPropertyException;

	public override string Message
	{
		get
		{
			if (base.Message.Length <= 0)
			{
				return ExceptionTemplatesImpl.UnknownPropertyExceptionText(propertyName);
			}
			return base.Message;
		}
	}

	public string PropertyName => propertyName;

	public ServerVersion[] SupportedVersions => supportedVersions;

	public ServerVersion CurrentVersion => currentVersion;

	public UnknownPropertyException()
	{
		Init();
	}

	public UnknownPropertyException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	public UnknownPropertyException(string propertyName)
		: base(string.Empty)
	{
		this.propertyName = propertyName;
		Data["HelpLink.EvtData1"] = propertyName;
	}

	internal UnknownPropertyException(string propertyName, string message)
		: base(message)
	{
		this.propertyName = propertyName;
		Data["HelpLink.EvtData1"] = propertyName;
	}

	private UnknownPropertyException(SerializationInfo info, StreamingContext context)
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
		supportedVersions = null;
		currentVersion = null;
		SetHelpContext("UnknownPropertyExceptionText");
	}
}
