using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcInvalidKeyException : SfcException
{
	private string keyName;

	public override string Message => SfcStrings.InvalidKey(keyName);

	public SfcInvalidKeyException()
	{
		keyName = string.Empty;
	}

	public SfcInvalidKeyException(string keyName)
	{
		this.keyName = keyName;
	}

	public SfcInvalidKeyException(string keyName, Exception innerException)
		: base(SfcStrings.InvalidKey(keyName), innerException)
	{
		this.keyName = keyName;
	}

	private SfcInvalidKeyException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		keyName = (string)info.GetValue("keyName", typeof(string));
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("keyName", keyName);
		base.GetObjectData(info, context);
	}
}
