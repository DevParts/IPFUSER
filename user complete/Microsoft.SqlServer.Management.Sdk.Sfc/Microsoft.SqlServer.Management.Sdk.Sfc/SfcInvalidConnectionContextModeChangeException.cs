using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class SfcInvalidConnectionContextModeChangeException : SfcException
{
	private string fromMode;

	private string toMode;

	public override string Message => SfcStrings.SfcInvalidConnectionContextModeChange(fromMode, toMode);

	public SfcInvalidConnectionContextModeChangeException()
	{
		fromMode = string.Empty;
		toMode = string.Empty;
	}

	public SfcInvalidConnectionContextModeChangeException(string fromMode, string toMode)
	{
		this.fromMode = fromMode;
		this.toMode = toMode;
	}

	public SfcInvalidConnectionContextModeChangeException(string fromMode, string toMode, Exception innerException)
		: base(SfcStrings.SfcInvalidConnectionContextModeChange(fromMode, toMode), innerException)
	{
		this.fromMode = fromMode;
		this.toMode = toMode;
	}

	private SfcInvalidConnectionContextModeChangeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		fromMode = (string)info.GetValue("fromMode", typeof(string));
		toMode = (string)info.GetValue("toMode", typeof(string));
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("fromMode", fromMode);
		info.AddValue("toMode", toMode);
		base.GetObjectData(info, context);
	}
}
