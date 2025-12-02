using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcPropertyNotSetException : SfcException
{
	private string propertyName;

	public override string Message => SfcStrings.PropertyNotSet(propertyName);

	public SfcPropertyNotSetException()
	{
		propertyName = string.Empty;
		Init();
	}

	public SfcPropertyNotSetException(string propertyName)
	{
		this.propertyName = propertyName;
		Init();
	}

	private SfcPropertyNotSetException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		propertyName = (string)info.GetValue("propertyName", typeof(string));
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("propertyName", propertyName);
		base.GetObjectData(info, context);
	}

	private void Init()
	{
		SetHelpContext("SfcPropertyNotSetException");
	}
}
