using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[CLSCompliant(false)]
public class ServerAuditSpecificationExtender : SmoObjectExtender<ServerAuditSpecification>, ISfcValidate
{
	private StringCollection audits;

	private DataTable auditSpecDetails;

	private ValidationState gridValidationState;

	[ExtendedProperty]
	public StringCollection Audits
	{
		get
		{
			if (audits == null)
			{
				audits = new StringCollection();
				Server server = base.Parent.Parent;
				if (server != null)
				{
					Urn urn = "Server/Audit";
					string[] fields = new string[1] { "Name" };
					Request request = new Request(urn, fields);
					DataTable dataTable = new Enumerator().Process(server.ConnectionContext, request);
					foreach (DataRow row in dataTable.Rows)
					{
						audits.Add(row["Name"].ToString());
					}
				}
			}
			return audits;
		}
	}

	[ExtendedProperty]
	public DataTable AuditSpecificationDetails
	{
		get
		{
			if (auditSpecDetails == null)
			{
				Urn urn = new Urn(base.Parent.Urn.Value + "/ServerAuditSpecificationDetail");
				string[] fields = new string[5] { "AuditActionType", "ObjectClass", "ObjectSchema", "ObjectName", "Principal" };
				Request req = new Request(urn, fields);
				auditSpecDetails = base.Parent.ExecutionManager.GetEnumeratorData(req);
			}
			return auditSpecDetails;
		}
		set
		{
			if (base.Parent.State == SqlSmoState.Creating)
			{
				List<AuditSpecificationDetail> list = new List<AuditSpecificationDetail>();
				foreach (DataRow row in value.Rows)
				{
					AuditSpecificationDetail item = new AuditSpecificationDetail((AuditActionType)AuditSpecificationDetail.StringToEnum[row["AuditActionType"].ToString()], row["ObjectClass"].ToString(), row["ObjectSchema"].ToString(), row["ObjectName"].ToString(), row["Principal"].ToString());
					list.Add(item);
				}
				base.Parent.AddAuditSpecificationDetail(list);
			}
			auditSpecDetails = value;
		}
	}

	[ExtendedProperty]
	public ValidationState GridValidationState
	{
		get
		{
			if (gridValidationState == null)
			{
				gridValidationState = new ValidationState();
			}
			return gridValidationState;
		}
		set
		{
			gridValidationState = value;
		}
	}

	[ExtendedProperty]
	public string Type => "ServerAuditSpecification";

	[ExtendedProperty]
	public SqlSmoState State => base.Parent.State;

	[ExtendedProperty]
	public string DatabaseName => "master";

	[ExtendedProperty]
	public ServerConnection ConnectionContext => base.Parent.Parent.ConnectionContext;

	public ServerAuditSpecificationExtender()
	{
	}

	public ServerAuditSpecificationExtender(ServerAuditSpecification serverAuditSpecification)
		: base(serverAuditSpecification)
	{
	}

	public ValidationState Validate(string methodName, params object[] arguments)
	{
		if (string.IsNullOrEmpty(base.Parent.Name))
		{
			return new ValidationState(ExceptionTemplatesImpl.EnterName, "Name");
		}
		if (string.IsNullOrEmpty(base.Parent.AuditName))
		{
			return new ValidationState(ExceptionTemplatesImpl.EnterServerAudit, "AuditName");
		}
		return GridValidationState;
	}
}
