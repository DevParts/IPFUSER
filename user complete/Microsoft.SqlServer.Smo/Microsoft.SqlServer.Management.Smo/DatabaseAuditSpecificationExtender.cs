using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[CLSCompliant(false)]
public class DatabaseAuditSpecificationExtender : SmoObjectExtender<DatabaseAuditSpecification>, ISfcValidate
{
	private StringCollection audits;

	private DataTable auditSpecDetails;

	private ValidationState gridValidationState;

	private static readonly StringCollection granularActions = new StringCollection();

	[ExtendedProperty]
	public StringCollection Audits
	{
		get
		{
			if (audits == null)
			{
				audits = new StringCollection();
				Server serverObject = base.Parent.Parent.GetServerObject();
				if (serverObject != null)
				{
					Urn urn = "Server/Audit";
					string[] fields = new string[1] { "Name" };
					Request request = new Request(urn, fields);
					DataTable dataTable = new Enumerator().Process(serverObject.ConnectionContext, request);
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
				Urn urn = new Urn(base.Parent.Urn.Value + "/DatabaseAuditSpecificationDetail");
				string[] fields = new string[5] { "AuditActionType", "ObjectClass", "ObjectSchema", "ObjectName", "Principal" };
				Request req = new Request(urn, fields);
				auditSpecDetails = base.Parent.ExecutionManager.GetEnumeratorData(req);
				foreach (DataRow row in auditSpecDetails.Rows)
				{
					if (!IsGranular(row["AuditActionType"].ToString()))
					{
						row["Principal"] = string.Empty;
					}
				}
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
	public string Type => "DatabaseAuditSpecification";

	[ExtendedProperty]
	public SqlSmoState State => base.Parent.State;

	[ExtendedProperty]
	public string DatabaseName => base.Parent.Parent.Name;

	[ExtendedProperty]
	public ServerConnection ConnectionContext => base.Parent.Parent.GetServerObject().ConnectionContext;

	public DatabaseAuditSpecificationExtender()
	{
	}

	public DatabaseAuditSpecificationExtender(DatabaseAuditSpecification databaseAuditSpecification)
		: base(databaseAuditSpecification)
	{
	}

	private bool IsGranular(string action)
	{
		if (granularActions.Count == 0)
		{
			granularActions.Add("SELECT");
			granularActions.Add("INSERT");
			granularActions.Add("UPDATE");
			granularActions.Add("DELETE");
			granularActions.Add("EXECUTE");
			granularActions.Add("REFERENCES");
			granularActions.Add("RECEIVE");
		}
		if (granularActions.Contains(action.Trim().ToUpper()))
		{
			return true;
		}
		return false;
	}

	public ValidationState Validate(string methodName, params object[] arguments)
	{
		if (string.IsNullOrEmpty(base.Parent.Name))
		{
			return new ValidationState(ExceptionTemplatesImpl.EnterName, "Name");
		}
		if (string.IsNullOrEmpty(base.Parent.AuditName) && (!base.Parent.Parent.GetServerObject().ConnectionContext.IsContainedAuthentication || !(methodName == "Alter")))
		{
			return new ValidationState(ExceptionTemplatesImpl.EnterServerAudit, "AuditName");
		}
		return GridValidationState;
	}
}
