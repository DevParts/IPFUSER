using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcObjectInitializationException : SfcException
{
	private string objName;

	public override string Message => SfcStrings.SfcObjectInitFailed(objName);

	public SfcObjectInitializationException()
	{
		objName = string.Empty;
	}

	public SfcObjectInitializationException(string keyName)
	{
		objName = keyName;
	}

	public SfcObjectInitializationException(string keyName, Exception innerException)
		: base(SfcStrings.SfcObjectInitFailed(keyName), innerException)
	{
		objName = keyName;
	}

	private SfcObjectInitializationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		objName = (string)info.GetValue("objName", typeof(string));
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("objName", objName);
		base.GetObjectData(info, context);
	}
}
