using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class Alert : AgentObjectBase, ICreatable, IDroppable, IDropIfExists, IAlterable, IRenamable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 20, 20, 22, 22, 22, 22, 22, 22, 22, 22 };

		private static int[] cloudVersionCount;

		private static int sqlDwPropertyCount;

		internal static StaticMetadata[] sqlDwStaticMetadata;

		internal static StaticMetadata[] cloudStaticMetadata;

		internal static StaticMetadata[] staticMetadata;

		public override int Count
		{
			get
			{
				if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
					if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
					{
						return sqlDwPropertyCount;
					}
					int num = ((currentVersionIndex < cloudVersionCount.Length) ? currentVersionIndex : (cloudVersionCount.Length - 1));
					return cloudVersionCount[num];
				}
				int num2 = ((currentVersionIndex < versionCount.Length) ? currentVersionIndex : (versionCount.Length - 1));
				return versionCount[num2];
			}
		}

		protected override int[] VersionCount
		{
			get
			{
				if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
					if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
					{
						return new int[1] { sqlDwPropertyCount };
					}
					return cloudVersionCount;
				}
				return versionCount;
			}
		}

		internal PropertyMetadataProvider(ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
			: base(version, databaseEngineType, databaseEngineEdition)
		{
		}

		public override int PropertyNameToIDLookup(string propertyName)
		{
			if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return -1;
				}
				return -1;
			}
			return propertyName switch
			{
				"AlertType" => 0, 
				"CategoryName" => 1, 
				"CountResetDate" => 2, 
				"DatabaseName" => 3, 
				"DelayBetweenResponses" => 4, 
				"EventDescriptionKeyword" => 5, 
				"EventSource" => 6, 
				"HasNotification" => 7, 
				"ID" => 8, 
				"IncludeEventDescription" => 9, 
				"IsEnabled" => 10, 
				"JobID" => 11, 
				"JobName" => 12, 
				"LastOccurrenceDate" => 13, 
				"LastResponseDate" => 14, 
				"MessageID" => 15, 
				"NotificationMessage" => 16, 
				"OccurrenceCount" => 17, 
				"PerformanceCondition" => 18, 
				"Severity" => 19, 
				"WmiEventNamespace" => 20, 
				"WmiEventQuery" => 21, 
				_ => -1, 
			};
		}

		internal new static int[] GetVersionArray(DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
		{
			if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (databaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return new int[1] { sqlDwPropertyCount };
				}
				return cloudVersionCount;
			}
			return versionCount;
		}

		public override StaticMetadata GetStaticMetadata(int id)
		{
			if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return sqlDwStaticMetadata[id];
				}
				return cloudStaticMetadata[id];
			}
			return staticMetadata[id];
		}

		internal new static StaticMetadata[] GetStaticMetadataArray(DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
		{
			if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (databaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return sqlDwStaticMetadata;
				}
				return cloudStaticMetadata;
			}
			return staticMetadata;
		}

		static PropertyMetadataProvider()
		{
			int[] array = new int[3];
			cloudVersionCount = array;
			sqlDwPropertyCount = 0;
			sqlDwStaticMetadata = new StaticMetadata[0];
			cloudStaticMetadata = new StaticMetadata[0];
			staticMetadata = new StaticMetadata[22]
			{
				new StaticMetadata("AlertType", expensive: false, readOnly: true, typeof(AlertType)),
				new StaticMetadata("CategoryName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("CountResetDate", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("DatabaseName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("DelayBetweenResponses", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("EventDescriptionKeyword", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("EventSource", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("HasNotification", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("IncludeEventDescription", expensive: false, readOnly: false, typeof(NotifyMethods)),
				new StaticMetadata("IsEnabled", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("JobID", expensive: false, readOnly: false, typeof(Guid)),
				new StaticMetadata("JobName", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("LastOccurrenceDate", expensive: false, readOnly: false, typeof(DateTime)),
				new StaticMetadata("LastResponseDate", expensive: false, readOnly: false, typeof(DateTime)),
				new StaticMetadata("MessageID", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("NotificationMessage", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("OccurrenceCount", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("PerformanceCondition", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("Severity", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("WmiEventNamespace", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("WmiEventQuery", expensive: false, readOnly: false, typeof(string))
			};
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public JobServer Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as JobServer;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AlertType AlertType => (AlertType)base.Properties.GetValueWithNullReplacement("AlertType");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string CategoryName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("CategoryName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("CategoryName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime CountResetDate => (DateTime)base.Properties.GetValueWithNullReplacement("CountResetDate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string DatabaseName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DatabaseName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DatabaseName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int DelayBetweenResponses
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("DelayBetweenResponses");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DelayBetweenResponses", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string EventDescriptionKeyword
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("EventDescriptionKeyword");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EventDescriptionKeyword", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string EventSource => (string)base.Properties.GetValueWithNullReplacement("EventSource");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int HasNotification => (int)base.Properties.GetValueWithNullReplacement("HasNotification");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public NotifyMethods IncludeEventDescription
	{
		get
		{
			return (NotifyMethods)base.Properties.GetValueWithNullReplacement("IncludeEventDescription");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IncludeEventDescription", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public Guid JobID
	{
		get
		{
			return (Guid)base.Properties.GetValueWithNullReplacement("JobID");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("JobID", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string JobName => (string)base.Properties.GetValueWithNullReplacement("JobName");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime LastOccurrenceDate
	{
		get
		{
			return (DateTime)base.Properties.GetValueWithNullReplacement("LastOccurrenceDate");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("LastOccurrenceDate", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime LastResponseDate
	{
		get
		{
			return (DateTime)base.Properties.GetValueWithNullReplacement("LastResponseDate");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("LastResponseDate", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int MessageID
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("MessageID");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MessageID", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string NotificationMessage
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("NotificationMessage");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NotificationMessage", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int OccurrenceCount => (int)base.Properties.GetValueWithNullReplacement("OccurrenceCount");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string PerformanceCondition
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("PerformanceCondition");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PerformanceCondition", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int Severity
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("Severity");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Severity", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string WmiEventNamespace
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("WmiEventNamespace");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("WmiEventNamespace", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string WmiEventQuery
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("WmiEventQuery");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("WmiEventQuery", value);
		}
	}

	public static string UrnSuffix => "Alert";

	public Alert()
	{
	}

	public Alert(JobServer jobServer, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = jobServer;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal Alert(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AGENT_ALERT, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_add_alert @name=N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
		int count = 1;
		GetAllParams(stringBuilder, sp, ref count);
		queries.Add(stringBuilder.ToString());
		if (!sp.Agent.Notify)
		{
			return;
		}
		DataTable dataTable = EnumNotifications();
		for (int i = 0; i < dataTable.Rows.Count; i++)
		{
			DataRow dataRow = dataTable.Rows[i];
			bool flag = Convert.ToBoolean(dataRow["HasEmail"], SmoApplication.DefaultCulture) && Convert.ToBoolean(dataRow["UseEmail"], SmoApplication.DefaultCulture);
			bool flag2 = Convert.ToBoolean(dataRow["HasPager"], SmoApplication.DefaultCulture) && Convert.ToBoolean(dataRow["UsePager"], SmoApplication.DefaultCulture);
			bool flag3 = Convert.ToBoolean(dataRow["HasNetSend"], SmoApplication.DefaultCulture) && Convert.ToBoolean(dataRow["UseNetSend"], SmoApplication.DefaultCulture);
			if (!flag && !flag2 && !flag3)
			{
				continue;
			}
			string s = Convert.ToString(dataRow["OperatorName"], SmoApplication.DefaultCulture);
			string text = string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_add_notification @alert_name=N'{0}', @operator_name=N'{1}', @notification_method =", new object[2]
			{
				SqlSmoObject.SqlString(Name),
				SqlSmoObject.SqlString(s)
			});
			if (flag && flag2 && flag3)
			{
				queries.Add(text + 7);
				continue;
			}
			if (flag)
			{
				queries.Add(text + 1);
			}
			if (flag2)
			{
				queries.Add(text + 2);
			}
			if (flag3)
			{
				queries.Add(text + 4);
			}
		}
	}

	private void GetAllParams(StringBuilder sb, ScriptingPreferences sp, ref int count)
	{
		GetParameter(sb, sp, "MessageID", "@message_id={0}", ref count);
		GetParameter(sb, sp, "Severity", "@severity={0}", ref count);
		GetBoolParameter(sb, sp, "IsEnabled", "@enabled={0}", ref count);
		GetParameter(sb, sp, "DelayBetweenResponses", "@delay_between_responses={0}", ref count);
		GetEnumParameter(sb, sp, "IncludeEventDescription", "@include_event_description_in={0}", typeof(NotifyMethods), ref count);
		GetStringParameter(sb, sp, "DatabaseName", "@database_name=N'{0}'", ref count);
		GetStringParameter(sb, sp, "NotificationMessage", "@notification_message=N'{0}'", ref count);
		GetStringParameter(sb, sp, "EventDescriptionKeyword", "@event_description_keyword=N'{0}'", ref count);
		GetStringParameter(sb, sp, "CategoryName", "@category_name=N'{0}'", ref count);
		GetStringParameter(sb, sp, "PerformanceCondition", "@performance_condition=N'{0}'", ref count);
		if (base.ServerVersion.Major >= 9)
		{
			GetStringParameter(sb, sp, "WmiEventNamespace", "@wmi_namespace=N'{0}'", ref count);
			GetStringParameter(sb, sp, "WmiEventQuery", "@wmi_query=N'{0}'", ref count);
		}
		if (sp.Agent.JobId && ((!sp.Agent.AlertJob && !sp.ScriptForAlter && !sp.ScriptForCreateDrop) || !GetStringParameter(sb, sp, "JobName", "@job_name=N'{0}'", ref count)))
		{
			GetGuidParameter(sb, sp, "JobID", "@job_id=N'{0}'", ref count);
		}
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AGENT_ALERT, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_alert @name=N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
		queries.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_alert @name=N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
		int count = 1;
		GetAllParams(stringBuilder, sp, ref count);
		GetDateTimeParameter(stringBuilder, sp, "CountResetDate", "@count_reset_{0}={1}", ref count);
		GetDateTimeParameter(stringBuilder, sp, "LastOccurrenceDate", "@last_occurrence_{0}={1}", ref count);
		GetDateTimeParameter(stringBuilder, sp, "LastResponseDate", "@last_response_{0}={1}", ref count);
		if (count > 1)
		{
			queries.Add(stringBuilder.ToString());
		}
	}

	public void Rename(string newName)
	{
		RenameImpl(newName);
	}

	internal override void ScriptRename(StringCollection queries, ScriptingPreferences sp, string newName)
	{
		queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_alert @name=N'{0}', @new_name=N'{1}'", new object[2]
		{
			SqlSmoObject.SqlString(Name),
			SqlSmoObject.SqlString(newName)
		}));
	}

	public void ResetOccurrenceCount()
	{
		try
		{
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("DECLARE @curr_date INT, \t@curr_time INT ");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("SELECT @curr_date = CONVERT(INT, CONVERT(CHAR, GETDATE(), 112)), @curr_time = (DATEPART(hh, GETDATE()) * 10000) + (DATEPART(mi, GETDATE()) * 100) + (DATEPART(ss, GETDATE())) ");
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXECUTE msdb.dbo.sp_update_alert @name = N'{0}', @count_reset_date = @curr_date, @count_reset_time = @curr_time, @occurrence_count = 0", new object[1] { SqlSmoObject.SqlString(Name) });
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ResetOccurrenceCount, this, ex);
		}
	}

	public void AddNotification(string operatorName, NotifyMethods notifymethod)
	{
		try
		{
			if (operatorName == null)
			{
				throw new ArgumentNullException("operatorName");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_add_notification @alert_name=N'{0}', @operator_name=N'{1}', @notification_method = {2}", new object[3]
			{
				SqlSmoObject.SqlString(Name),
				SqlSmoObject.SqlString(operatorName),
				Enum.Format(typeof(NotifyMethods), notifymethod, "d")
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddNotification, this, ex);
		}
	}

	public void RemoveNotification(string operatorName)
	{
		try
		{
			if (operatorName == null)
			{
				throw new ArgumentNullException("operatorName");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_notification @alert_name=N'{0}', @operator_name=N'{1}'", new object[2]
			{
				SqlSmoObject.SqlString(Name),
				SqlSmoObject.SqlString(operatorName)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveNotification, this, ex);
		}
	}

	public void UpdateNotification(string operatorName, NotifyMethods notifymethod)
	{
		try
		{
			if (operatorName == null)
			{
				throw new ArgumentNullException("operatorName");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_notification @alert_name=N'{0}', @operator_name=N'{1}', @notification_method = {2}", new object[3]
			{
				SqlSmoObject.SqlString(Name),
				SqlSmoObject.SqlString(operatorName),
				Enum.Format(typeof(NotifyMethods), notifymethod, "d")
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.UpdateNotification, this, ex);
		}
	}

	public DataTable EnumNotifications()
	{
		return EnumNotifications(NotifyMethods.NotifyAll, string.Empty);
	}

	public DataTable EnumNotifications(NotifyMethods notifyMethod)
	{
		return EnumNotifications(notifyMethod, string.Empty);
	}

	public DataTable EnumNotifications(string operatorName)
	{
		return EnumNotifications(NotifyMethods.NotifyAll, operatorName);
	}

	public DataTable EnumNotifications(NotifyMethods notifyMethod, string operatorName)
	{
		try
		{
			string text = base.Urn.Value + "/Notification";
			bool flag = false;
			switch (notifyMethod)
			{
			case NotifyMethods.NotifyEmail:
				text += "[@UseEmail=1";
				flag = true;
				break;
			case NotifyMethods.Pager:
				text += "[@UsePager=1";
				flag = true;
				break;
			case NotifyMethods.NetSend:
				text += "[@UseNetSend=1";
				flag = true;
				break;
			default:
				return null;
			case NotifyMethods.NotifyAll:
				break;
			}
			if (operatorName != null && 0 < operatorName.Length)
			{
				if (flag)
				{
					text += " and ";
				}
				else
				{
					text += "[";
					flag = true;
				}
				text += "@OperatorName='";
				text += Urn.EscapeString(operatorName);
				text += "'";
			}
			if (flag)
			{
				text += "]";
			}
			Request request = new Request(text);
			request.OrderByList = new OrderBy[1]
			{
				new OrderBy("OperatorName", OrderBy.Direction.Asc)
			};
			return ExecutionManager.GetEnumeratorData(request);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumNotifications, this, ex);
		}
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}
}
