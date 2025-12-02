using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class CollectionNotAvailableException : SmoException
{
	private string colname;

	private ServerVersion serverVersion;

	public override SmoExceptionType SmoExceptionType => SmoExceptionType.CollectionNotAvailableException;

	public override string Message => string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.CollectionNotAvailable(colname, (serverVersion != null) ? serverVersion.ToString() : string.Empty));

	public string CollectionName => colname;

	public ServerVersion ServerVersion => serverVersion;

	public CollectionNotAvailableException()
	{
		Init();
	}

	public CollectionNotAvailableException(string message)
		: base(message)
	{
		Init();
	}

	public CollectionNotAvailableException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	public CollectionNotAvailableException(string colname, ServerVersion serverVersion)
	{
		this.colname = colname;
		this.serverVersion = serverVersion;
	}

	private CollectionNotAvailableException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		colname = info.GetString("colname");
		serverVersion = (ServerVersion)info.GetValue("serverVersion", typeof(ServerVersion));
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("colname", colname);
		info.AddValue("serverVersion", serverVersion);
		base.GetObjectData(info, context);
	}

	private void Init()
	{
		colname = string.Empty;
		serverVersion = null;
		SetHelpContext("CollectionNotAvailable");
	}
}
