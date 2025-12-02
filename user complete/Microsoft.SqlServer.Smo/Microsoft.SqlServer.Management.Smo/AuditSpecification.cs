using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Reflection;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

public class AuditSpecification : ScriptNameObjectBase, ICreatable, IAlterable, IDroppable, IDropIfExists, IScriptable
{
	private bool isInitialized;

	private List<AuditSpecificationDetail> enumAuditSpecDetails;

	private List<AuditSpecificationDetail> auditSpecificationDetailsList;

	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public override string Name
	{
		get
		{
			return base.Name;
		}
		set
		{
			base.Name = value;
		}
	}

	private List<AuditSpecificationDetail> AuditSpecificationDetailsList
	{
		get
		{
			if (auditSpecificationDetailsList == null)
			{
				auditSpecificationDetailsList = new List<AuditSpecificationDetail>();
			}
			return auditSpecificationDetailsList;
		}
		set
		{
			auditSpecificationDetailsList = value;
		}
	}

	internal AuditSpecification(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		isInitialized = false;
	}

	protected internal AuditSpecification()
	{
		isInitialized = false;
	}

	public ICollection<AuditSpecificationDetail> EnumAuditSpecificationDetails()
	{
		this.ThrowIfNotSupported(typeof(DatabaseAuditSpecification));
		if (base.State == SqlSmoState.Creating)
		{
			return null;
		}
		CheckObjectState();
		if (base.IsDesignMode)
		{
			enumAuditSpecDetails = new List<AuditSpecificationDetail>();
			foreach (AuditSpecificationDetail auditSpecificationDetails in AuditSpecificationDetailsList)
			{
				enumAuditSpecDetails.Add(auditSpecificationDetails);
			}
		}
		else if (!isInitialized)
		{
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(base.Urn.Value != null, "The Urn value is null");
			Urn urn = new Urn(base.Urn.Value + "/" + base.Urn.Type + "Detail");
			Request req = new Request(urn);
			DataTable enumeratorData = ExecutionManager.GetEnumeratorData(req);
			enumAuditSpecDetails = new List<AuditSpecificationDetail>();
			foreach (DataRow row in enumeratorData.Rows)
			{
				enumAuditSpecDetails.Add(new AuditSpecificationDetail((AuditActionType)AuditSpecificationDetail.StringToEnum[row["AuditActionType"].ToString().Trim()], row["ObjectClass"].ToString(), row["ObjectSchema"].ToString(), row["ObjectName"].ToString(), row["Principal"].ToString()));
			}
			isInitialized = true;
		}
		return enumAuditSpecDetails;
	}

	public void Create()
	{
		this.ThrowIfNotSupported(typeof(DatabaseAuditSpecification));
		CreateImpl();
	}

	public void Alter()
	{
		this.ThrowIfNotSupported(typeof(DatabaseAuditSpecification));
		AlterImpl();
	}

	public void Drop()
	{
		this.ThrowIfNotSupported(typeof(DatabaseAuditSpecification));
		DropImpl();
	}

	public void DropIfExists()
	{
		this.ThrowIfNotSupported(typeof(DatabaseAuditSpecification));
		DropImpl(isDropIfExists: true);
	}

	public override void Refresh()
	{
		this.ThrowIfNotSupported(typeof(DatabaseAuditSpecification));
		isInitialized = false;
		base.Refresh();
	}

	public StringCollection Script()
	{
		this.ThrowIfNotSupported(typeof(DatabaseAuditSpecification));
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		this.ThrowIfNotSupported(typeof(DatabaseAuditSpecification));
		return ScriptImpl(scriptingOptions);
	}

	public void AddAuditSpecificationDetail(AuditSpecificationDetail auditSpecificationDetail)
	{
		this.ThrowIfNotSupported(typeof(DatabaseAuditSpecification));
		if (base.State == SqlSmoState.Existing && !base.IsDesignMode)
		{
			try
			{
				List<AuditSpecificationDetail> list = new List<AuditSpecificationDetail>();
				list.Add(auditSpecificationDetail);
				StringCollection queries = AddRemoveAuditSpecificationDetail(list, add: true, useDb: true);
				ExecutionManager.ExecuteNonQuery(queries);
				isInitialized = false;
				return;
			}
			catch (Exception ex)
			{
				SqlSmoObject.FilterException(ex);
				throw new FailedOperationException(ExceptionTemplatesImpl.AddAuditSpecificationDetail, this, ex);
			}
		}
		AuditSpecificationDetailsList.Add(auditSpecificationDetail);
	}

	public void AddAuditSpecificationDetail(ICollection<AuditSpecificationDetail> auditSpecificationDetails)
	{
		this.ThrowIfNotSupported(typeof(DatabaseAuditSpecification));
		if (base.State == SqlSmoState.Existing && !base.IsDesignMode)
		{
			try
			{
				StringCollection queries = AddRemoveAuditSpecificationDetail(auditSpecificationDetails, add: true, useDb: true);
				ExecutionManager.ExecuteNonQuery(queries);
				isInitialized = false;
				return;
			}
			catch (Exception ex)
			{
				SqlSmoObject.FilterException(ex);
				throw new FailedOperationException(ExceptionTemplatesImpl.AddAuditSpecificationDetail, this, ex);
			}
		}
		foreach (AuditSpecificationDetail auditSpecificationDetail in auditSpecificationDetails)
		{
			AuditSpecificationDetailsList.Add(auditSpecificationDetail);
		}
	}

	public void RemoveAuditSpecificationDetail(AuditSpecificationDetail auditSpecificationDetail)
	{
		this.ThrowIfNotSupported(typeof(DatabaseAuditSpecification));
		if (base.State == SqlSmoState.Existing && !base.IsDesignMode)
		{
			try
			{
				List<AuditSpecificationDetail> list = new List<AuditSpecificationDetail>();
				list.Add(auditSpecificationDetail);
				StringCollection queries = AddRemoveAuditSpecificationDetail(list, add: false, useDb: true);
				ExecutionManager.ExecuteNonQuery(queries);
				isInitialized = false;
				return;
			}
			catch (Exception ex)
			{
				SqlSmoObject.FilterException(ex);
				throw new FailedOperationException(ExceptionTemplatesImpl.RemoveAuditSpecificationDetail, this, ex);
			}
		}
		if (AuditSpecificationDetailsList != null)
		{
			AuditSpecificationDetailsList.Remove(auditSpecificationDetail);
		}
	}

	public void RemoveAuditSpecificationDetail(ICollection<AuditSpecificationDetail> auditSpecificationDetails)
	{
		this.ThrowIfNotSupported(typeof(DatabaseAuditSpecification));
		if (base.State == SqlSmoState.Existing && !base.IsDesignMode)
		{
			try
			{
				StringCollection queries = AddRemoveAuditSpecificationDetail(auditSpecificationDetails, add: false, useDb: true);
				ExecutionManager.ExecuteNonQuery(queries);
				isInitialized = false;
				return;
			}
			catch (Exception ex)
			{
				SqlSmoObject.FilterException(ex);
				throw new FailedOperationException(ExceptionTemplatesImpl.AddAuditSpecificationDetail, this, ex);
			}
		}
		if (AuditSpecificationDetailsList == null)
		{
			return;
		}
		foreach (AuditSpecificationDetail auditSpecificationDetail in auditSpecificationDetails)
		{
			AuditSpecificationDetailsList.Remove(auditSpecificationDetail);
		}
	}

	private StringCollection AddRemoveAuditSpecificationDetail(ICollection<AuditSpecificationDetail> auditSpecificationDetails, bool add, bool useDb)
	{
		CheckObjectState();
		if (auditSpecificationDetails.Count < 1)
		{
			throw new ArgumentNullException("auditSpecificationDetails");
		}
		string text = GetType().InvokeMember("ParentType", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty, null, null, new object[0], SmoApplication.DefaultCulture) as string;
		StringBuilder stringBuilder = new StringBuilder();
		StringCollection stringCollection = new StringCollection();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER {0} AUDIT SPECIFICATION {1}", new object[2] { text, FullQualifiedName });
		stringBuilder.Append(ScriptAddDropAuditActionTypePart(auditSpecificationDetails, add));
		if (useDb)
		{
			AddDatabaseContext(stringCollection, new ScriptingPreferences(this));
		}
		stringCollection.Add(stringBuilder.ToString());
		return stringCollection;
	}

	internal override void ScriptCreate(StringCollection query, ScriptingPreferences sp)
	{
		SqlSmoObject.ThrowIfBelowVersion100(sp.TargetServerVersionInternal);
		string text = GetType().InvokeMember("ParentType", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty, null, null, new object[0], SmoApplication.DefaultCulture) as string;
		StringBuilder stringBuilder = new StringBuilder();
		Property property = base.Properties.Get("AuditName");
		if (property.IsNull)
		{
			throw new PropertyNotSetException("AuditName");
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AUDIT_SPECIFICATION, new object[3]
			{
				"NOT",
				text.ToLower(SmoApplication.DefaultCulture),
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.AppendLine();
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.AppendLine();
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE {0} AUDIT SPECIFICATION {1}", new object[2] { text, FullQualifiedName });
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "FOR SERVER AUDIT {0}", new object[1] { SqlSmoObject.MakeSqlBraket(property.Value.ToString()) });
		if (base.State != SqlSmoState.Creating && !base.IsDesignMode)
		{
			AuditSpecificationDetailsList = new List<AuditSpecificationDetail>(EnumAuditSpecificationDetails());
		}
		if (AuditSpecificationDetailsList != null)
		{
			stringBuilder.Append(ScriptAddDropAuditActionTypePart(AuditSpecificationDetailsList, add: true));
		}
		Property property2 = base.Properties.Get("Enabled");
		if (!property2.IsNull)
		{
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "WITH (STATE = {0})", new object[1] { ((bool)property2.Value) ? "ON" : "OFF" });
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendLine();
			stringBuilder.Append(Scripts.END);
		}
		query.Add(stringBuilder.ToString());
		if (AuditSpecificationDetailsList != null && !base.IsDesignMode)
		{
			AuditSpecificationDetailsList.Clear();
		}
	}

	private string ScriptAddDropAuditActionTypePart(ICollection<AuditSpecificationDetail> auditSpecificationDetails, bool add)
	{
		StringBuilder stringBuilder = new StringBuilder();
		AuditActionTypeConverter auditActionTypeConverter = new AuditActionTypeConverter();
		int num = 0;
		foreach (AuditSpecificationDetail auditSpecificationDetail in auditSpecificationDetails)
		{
			num++;
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} ({1}", new object[2]
			{
				add ? "ADD" : "DROP",
				auditActionTypeConverter.ConvertToInvariantString(auditSpecificationDetail.Action)
			});
			string text = string.Empty;
			if (!string.IsNullOrEmpty(auditSpecificationDetail.ObjectClass))
			{
				text = text + auditSpecificationDetail.ObjectClass + "::";
			}
			if (!string.IsNullOrEmpty(auditSpecificationDetail.ObjectSchema))
			{
				text = text + SqlSmoObject.MakeSqlBraket(auditSpecificationDetail.ObjectSchema) + ".";
			}
			if (!string.IsNullOrEmpty(auditSpecificationDetail.ObjectName))
			{
				text += SqlSmoObject.MakeSqlBraket(auditSpecificationDetail.ObjectName);
			}
			if (!string.IsNullOrEmpty(text))
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " ON {0}", new object[1] { text });
				if (!string.IsNullOrEmpty(auditSpecificationDetail.Principal))
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " BY {0}", new object[1] { SqlSmoObject.MakeSqlBraket(auditSpecificationDetail.Principal) });
				}
			}
			stringBuilder.Append(Globals.RParen);
			if (num < auditSpecificationDetails.Count)
			{
				stringBuilder.Append(Globals.comma);
			}
		}
		return stringBuilder.ToString();
	}

	internal override void ScriptDrop(StringCollection query, ScriptingPreferences sp)
	{
		SqlSmoObject.ThrowIfBelowVersion100(sp.TargetServerVersionInternal);
		string text = GetType().InvokeMember("ParentType", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty, null, null, new object[0], SmoApplication.DefaultCulture) as string;
		StringBuilder stringBuilder = new StringBuilder();
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AUDIT_SPECIFICATION, new object[3]
			{
				"",
				text.ToLower(SmoApplication.DefaultCulture),
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.AppendLine();
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP {0} AUDIT SPECIFICATION {1}", new object[2] { text, FullQualifiedName });
		query.Add(stringBuilder.ToString());
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		string text = GetType().InvokeMember("ParentType", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty, null, null, new object[0], SmoApplication.DefaultCulture) as string;
		bool flag = false;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER {0} AUDIT SPECIFICATION {1}", new object[2] { text, FullQualifiedName });
		Property property = base.Properties.Get("AuditName");
		if (property.Dirty && !string.IsNullOrEmpty(property.Value as string))
		{
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "FOR SERVER AUDIT {0}", new object[1] { SqlSmoObject.MakeSqlBraket(property.Value.ToString()) });
			flag = true;
		}
		if (flag)
		{
			query.Add(stringBuilder.ToString());
		}
	}

	public void Enable()
	{
		EnableDisable(enable: true);
	}

	public void Disable()
	{
		EnableDisable(enable: false);
	}

	private void EnableDisable(bool enable)
	{
		this.ThrowIfNotSupported(typeof(DatabaseAuditSpecification));
		CheckObjectState();
		try
		{
			string text = GetType().InvokeMember("ParentType", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty, null, null, new object[0], SmoApplication.DefaultCulture) as string;
			if (!base.IsDesignMode)
			{
				StringCollection stringCollection = new StringCollection();
				AddDatabaseContext(stringCollection, new ScriptingPreferences(this));
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER {0} AUDIT SPECIFICATION {1}", new object[2] { text, FullQualifiedName });
				stringBuilder.AppendLine();
				if (enable)
				{
					stringBuilder.Append("WITH (STATE = ON)");
				}
				else
				{
					stringBuilder.Append("WITH (STATE = OFF)");
				}
				stringCollection.Add(stringBuilder.ToString());
				ExecutionManager.ExecuteNonQuery(stringCollection);
			}
			Property property = base.Properties.Get("Enabled");
			property.SetValue(enable);
			property.SetRetrieved(retrieved: true);
			if (!ExecutionManager.Recording && !SmoApplication.eventsSingleton.IsNullObjectAltered())
			{
				SmoApplication.eventsSingleton.CallObjectAltered(GetServerObject(), new ObjectAlteredEventArgs(base.Urn, this));
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			if (enable)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.Enable, this, ex);
			}
			throw new FailedOperationException(ExceptionTemplatesImpl.Disable, this, ex);
		}
	}
}
