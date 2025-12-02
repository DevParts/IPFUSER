using System;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[CLSCompliant(false)]
public class AuditExtender : SmoObjectExtender<Audit>, ISfcValidate
{
	[ExtendedProperty]
	public SqlSmoState State => base.Parent.State;

	[ExtendedProperty]
	public ServerConnection ConnectionContext => base.Parent.Parent.ConnectionContext;

	public AuditExtender()
	{
	}

	public AuditExtender(Audit audit)
		: base(audit)
	{
	}

	public ValidationState Validate(string methodName, params object[] arguments)
	{
		if (string.IsNullOrEmpty(base.Parent.Name))
		{
			return new ValidationState(ExceptionTemplatesImpl.EnterAuditName, "Name");
		}
		if (base.Parent.DestinationType == AuditDestinationType.File)
		{
			if (string.IsNullOrEmpty(base.Parent.FilePath))
			{
				return new ValidationState(ExceptionTemplatesImpl.EnterFilePath, "FilePath");
			}
			if (base.Parent.ReserveDiskSpace && base.Parent.MaximumFileSize == 0)
			{
				return new ValidationState(ExceptionTemplatesImpl.ReserveDiskSpaceNotAllowedWhenMaxFileSizeIsUnlimited, "ReserveDiskSpace");
			}
		}
		else if (base.Parent.DestinationType == AuditDestinationType.Url && string.IsNullOrEmpty(base.Parent.FilePath))
		{
			return new ValidationState(ExceptionTemplatesImpl.EnterStoragePath, "Path");
		}
		return base.Parent.Validate(methodName, arguments);
	}
}
