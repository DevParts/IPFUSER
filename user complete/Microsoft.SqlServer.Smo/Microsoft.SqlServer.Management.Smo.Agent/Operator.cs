using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class Operator : AgentObjectBase, ICreatable, IDroppable, IDropIfExists, IAlterable, IRenamable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 16, 16, 16, 16, 16, 16, 16, 16, 16, 16 };

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
				"CategoryName" => 0, 
				"EmailAddress" => 1, 
				"Enabled" => 2, 
				"ID" => 3, 
				"LastEmailDate" => 4, 
				"LastNetSendDate" => 5, 
				"LastPagerDate" => 6, 
				"NetSendAddress" => 7, 
				"PagerAddress" => 8, 
				"PagerDays" => 9, 
				"SaturdayPagerEndTime" => 10, 
				"SaturdayPagerStartTime" => 11, 
				"SundayPagerEndTime" => 12, 
				"SundayPagerStartTime" => 13, 
				"WeekdayPagerEndTime" => 14, 
				"WeekdayPagerStartTime" => 15, 
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
			staticMetadata = new StaticMetadata[16]
			{
				new StaticMetadata("CategoryName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("EmailAddress", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("Enabled", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("LastEmailDate", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("LastNetSendDate", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("LastPagerDate", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("NetSendAddress", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("PagerAddress", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("PagerDays", expensive: false, readOnly: false, typeof(WeekDays)),
				new StaticMetadata("SaturdayPagerEndTime", expensive: false, readOnly: false, typeof(TimeSpan)),
				new StaticMetadata("SaturdayPagerStartTime", expensive: false, readOnly: false, typeof(TimeSpan)),
				new StaticMetadata("SundayPagerEndTime", expensive: false, readOnly: false, typeof(TimeSpan)),
				new StaticMetadata("SundayPagerStartTime", expensive: false, readOnly: false, typeof(TimeSpan)),
				new StaticMetadata("WeekdayPagerEndTime", expensive: false, readOnly: false, typeof(TimeSpan)),
				new StaticMetadata("WeekdayPagerStartTime", expensive: false, readOnly: false, typeof(TimeSpan))
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
	public string EmailAddress
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("EmailAddress");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EmailAddress", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool Enabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Enabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Enabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime LastEmailDate => (DateTime)base.Properties.GetValueWithNullReplacement("LastEmailDate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime LastNetSendDate => (DateTime)base.Properties.GetValueWithNullReplacement("LastNetSendDate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime LastPagerDate => (DateTime)base.Properties.GetValueWithNullReplacement("LastPagerDate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string NetSendAddress
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("NetSendAddress");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NetSendAddress", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string PagerAddress
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("PagerAddress");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PagerAddress", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public WeekDays PagerDays
	{
		get
		{
			return (WeekDays)base.Properties.GetValueWithNullReplacement("PagerDays");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PagerDays", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public TimeSpan SaturdayPagerEndTime
	{
		get
		{
			return (TimeSpan)base.Properties.GetValueWithNullReplacement("SaturdayPagerEndTime");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SaturdayPagerEndTime", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public TimeSpan SaturdayPagerStartTime
	{
		get
		{
			return (TimeSpan)base.Properties.GetValueWithNullReplacement("SaturdayPagerStartTime");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SaturdayPagerStartTime", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public TimeSpan SundayPagerEndTime
	{
		get
		{
			return (TimeSpan)base.Properties.GetValueWithNullReplacement("SundayPagerEndTime");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SundayPagerEndTime", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public TimeSpan SundayPagerStartTime
	{
		get
		{
			return (TimeSpan)base.Properties.GetValueWithNullReplacement("SundayPagerStartTime");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SundayPagerStartTime", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public TimeSpan WeekdayPagerEndTime
	{
		get
		{
			return (TimeSpan)base.Properties.GetValueWithNullReplacement("WeekdayPagerEndTime");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("WeekdayPagerEndTime", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public TimeSpan WeekdayPagerStartTime
	{
		get
		{
			return (TimeSpan)base.Properties.GetValueWithNullReplacement("WeekdayPagerStartTime");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("WeekdayPagerStartTime", value);
		}
	}

	public static string UrnSuffix => "Operator";

	public Operator()
	{
	}

	public Operator(JobServer jobServer, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = jobServer;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal Operator(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
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
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AGENT_OPERATOR, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_add_operator @name=N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
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
			string s = Convert.ToString(dataRow["AlertName"], SmoApplication.DefaultCulture);
			string text = string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_add_notification @alert_name=N'{0}', @operator_name=N'{1}', @notification_method =", new object[2]
			{
				SqlSmoObject.SqlString(s),
				SqlSmoObject.SqlString(Name)
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
		bool flag = DatabaseEngineEdition.SqlManagedInstance == sp.TargetDatabaseEngineEdition || (base.ServerInfo != null && base.ServerInfo.DatabaseEngineEdition == DatabaseEngineEdition.SqlManagedInstance);
		GetBoolParameter(sb, sp, "Enabled", "@enabled={0}", ref count);
		GetTimeSpanParameterAsInt(sb, sp, "WeekdayPagerStartTime", "@weekday_pager_start_time={0}", ref count);
		GetTimeSpanParameterAsInt(sb, sp, "WeekdayPagerEndTime", "@weekday_pager_end_time={0}", ref count);
		GetTimeSpanParameterAsInt(sb, sp, "SaturdayPagerStartTime", "@saturday_pager_start_time={0}", ref count);
		GetTimeSpanParameterAsInt(sb, sp, "SaturdayPagerEndTime", "@saturday_pager_end_time={0}", ref count);
		GetTimeSpanParameterAsInt(sb, sp, "SundayPagerStartTime", "@sunday_pager_start_time={0}", ref count);
		GetTimeSpanParameterAsInt(sb, sp, "SundayPagerEndTime", "@sunday_pager_end_time={0}", ref count);
		if (!flag)
		{
			GetEnumParameter(sb, sp, "PagerDays", "@pager_days={0}", typeof(WeekDays), ref count);
		}
		GetStringParameter(sb, sp, "EmailAddress", "@email_address=N'{0}'", ref count);
		GetStringParameter(sb, sp, "PagerAddress", "@pager_address=N'{0}'", ref count);
		GetStringParameter(sb, sp, "CategoryName", "@category_name=N'{0}'", ref count);
		GetStringParameter(sb, sp, "NetSendAddress", "@netsend_address=N'{0}'", ref count);
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
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AGENT_OPERATOR, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_operator @name=N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
		queries.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_operator @name=N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
		int count = 1;
		GetAllParams(stringBuilder, sp, ref count);
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
		queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_operator @name=N'{0}', @new_name=N'{1}'", new object[2]
		{
			SqlSmoObject.SqlString(Name),
			SqlSmoObject.SqlString(newName)
		}));
	}

	public void AddNotification(string alertName, NotifyMethods notifymethod)
	{
		try
		{
			if (alertName == null)
			{
				throw new ArgumentNullException("alertName");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_add_notification @alert_name=N'{0}', @operator_name=N'{1}', @notification_method = {2}", new object[3]
			{
				SqlSmoObject.SqlString(alertName),
				SqlSmoObject.SqlString(Name),
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

	public void RemoveNotification(string alertName)
	{
		try
		{
			if (alertName == null)
			{
				throw new ArgumentNullException("alertName");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_notification @alert_name=N'{0}', @operator_name=N'{1}'", new object[2]
			{
				SqlSmoObject.SqlString(alertName),
				SqlSmoObject.SqlString(Name)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveNotification, this, ex);
		}
	}

	public void UpdateNotification(string alertName, NotifyMethods notifymethod)
	{
		try
		{
			if (alertName == null)
			{
				throw new ArgumentNullException("alertName");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_notification @alert_name=N'{0}', @operator_name=N'{1}', @notification_method = {2}", new object[3]
			{
				SqlSmoObject.SqlString(alertName),
				SqlSmoObject.SqlString(Name),
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

	public DataTable EnumNotifications(string alertName)
	{
		return EnumNotifications(NotifyMethods.NotifyAll, alertName);
	}

	public DataTable EnumNotifications(NotifyMethods notifyMethod, string alertName)
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
			if (alertName != null && 0 < alertName.Length)
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
				text += "@AlertName='";
				text += Urn.EscapeString(alertName);
				text += "'";
			}
			if (flag)
			{
				text += "]";
			}
			Request request = new Request(text);
			request.OrderByList = new OrderBy[1]
			{
				new OrderBy("AlertName", OrderBy.Direction.Asc)
			};
			return ExecutionManager.GetEnumeratorData(request);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumNotifications, this, ex);
		}
	}

	public DataTable EnumJobNotifications()
	{
		try
		{
			Request request = new Request(base.Urn.Value + "/JobNotification");
			request.OrderByList = new OrderBy[1]
			{
				new OrderBy("JobName", OrderBy.Direction.Asc)
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
