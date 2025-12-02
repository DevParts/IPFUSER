using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcPropertyReadOnlyException : SfcException
{
	private string propertyName;

	public override string Message => propertyName;

	public SfcPropertyReadOnlyException()
	{
		propertyName = string.Empty;
		Init();
	}

	public SfcPropertyReadOnlyException(string propertyName)
	{
		this.propertyName = propertyName;
		Init();
	}

	private SfcPropertyReadOnlyException(SerializationInfo info, StreamingContext context)
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
		SetHelpContext("SfcPropertyReadOnlyException");
	}
}
