using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class InvalidVersionSmoOperationException : SmoException
{
	private ServerVersion version;

	public override SmoExceptionType SmoExceptionType => SmoExceptionType.InvalidVersionSmoOperationException;

	public override string Message
	{
		get
		{
			if (version != null)
			{
				if (version.Major >= 9)
				{
					return ExceptionTemplatesImpl.InvalidVersionSmoOperation(LocalizableResources.ServerYukon);
				}
				if (version.Major == 8)
				{
					return ExceptionTemplatesImpl.InvalidVersionSmoOperation(LocalizableResources.ServerShiloh);
				}
				if (version.Major == 7)
				{
					return ExceptionTemplatesImpl.InvalidVersionSmoOperation(LocalizableResources.ServerSphinx);
				}
			}
			return string.Empty;
		}
	}

	public InvalidVersionSmoOperationException()
	{
		Init();
	}

	public InvalidVersionSmoOperationException(string message)
		: base(message)
	{
		Init();
	}

	public InvalidVersionSmoOperationException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	public InvalidVersionSmoOperationException(ServerVersion version)
	{
		this.version = version;
	}

	private InvalidVersionSmoOperationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		version = (ServerVersion)info.GetValue("version", typeof(ServerVersion));
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("version", version);
		base.GetObjectData(info, context);
	}

	private void Init()
	{
		version = null;
		SetHelpContext("InvalidVersionSmoOperation");
	}
}
