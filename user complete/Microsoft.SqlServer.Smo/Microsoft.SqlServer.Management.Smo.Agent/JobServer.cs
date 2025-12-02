using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class JobServer : SqlSmoObject, IAlterable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 22, 22, 28, 29, 29, 29, 29, 29, 29, 29 };

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
				"AgentLogLevel" => 0, 
				"AgentShutdownWaitTime" => 1, 
				"ErrorLogFile" => 2, 
				"HostLoginName" => 3, 
				"IdleCpuDuration" => 4, 
				"IdleCpuPercentage" => 5, 
				"IsCpuPollingEnabled" => 6, 
				"JobServerType" => 7, 
				"LocalHostAlias" => 8, 
				"LoginTimeout" => 9, 
				"MaximumHistoryRows" => 10, 
				"MaximumJobHistoryRows" => 11, 
				"MsxAccountName" => 12, 
				"MsxServerName" => 13, 
				"NetSendRecipient" => 14, 
				"SaveInSentFolder" => 15, 
				"SqlAgentAutoStart" => 16, 
				"SqlAgentMailProfile" => 17, 
				"SqlAgentRestart" => 18, 
				"SqlServerRestart" => 19, 
				"SysAdminOnly" => 20, 
				"WriteOemErrorLog" => 21, 
				"AgentMailType" => 22, 
				"DatabaseMailProfile" => 23, 
				"MsxAccountCredentialName" => 24, 
				"ReplaceAlertTokensEnabled" => 25, 
				"ServiceAccount" => 26, 
				"ServiceStartMode" => 27, 
				"AgentDomainGroup" => 28, 
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
			staticMetadata = new StaticMetadata[29]
			{
				new StaticMetadata("AgentLogLevel", expensive: false, readOnly: false, typeof(AgentLogLevels)),
				new StaticMetadata("AgentShutdownWaitTime", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("ErrorLogFile", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("HostLoginName", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("IdleCpuDuration", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("IdleCpuPercentage", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("IsCpuPollingEnabled", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("JobServerType", expensive: false, readOnly: true, typeof(JobServerType)),
				new StaticMetadata("LocalHostAlias", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("LoginTimeout", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("MaximumHistoryRows", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("MaximumJobHistoryRows", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("MsxAccountName", expensive: true, readOnly: true, typeof(string)),
				new StaticMetadata("MsxServerName", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("NetSendRecipient", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("SaveInSentFolder", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("SqlAgentAutoStart", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("SqlAgentMailProfile", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("SqlAgentRestart", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("SqlServerRestart", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("SysAdminOnly", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("WriteOemErrorLog", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("AgentMailType", expensive: false, readOnly: false, typeof(AgentMailType)),
				new StaticMetadata("DatabaseMailProfile", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("MsxAccountCredentialName", expensive: true, readOnly: true, typeof(string)),
				new StaticMetadata("ReplaceAlertTokensEnabled", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("ServiceAccount", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("ServiceStartMode", expensive: false, readOnly: true, typeof(ServiceStartMode)),
				new StaticMetadata("AgentDomainGroup", expensive: false, readOnly: true, typeof(string))
			};
		}
	}

	private JobCategoryCollection jobCategories;

	private OperatorCategoryCollection operatorCategories;

	private AlertCategoryCollection alertCategories;

	private AlertSystem alertSystem;

	private AlertCollection alerts;

	private OperatorCollection operators;

	private TargetServerCollection targetServers;

	private TargetServerGroupCollection targetServerGroups;

	private JobCollection jobs;

	private JobScheduleCollection sharedSchedules;

	private ProxyAccountCollection proxyAccounts;

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string AgentDomainGroup => (string)base.Properties.GetValueWithNullReplacement("AgentDomainGroup");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AgentLogLevels AgentLogLevel
	{
		get
		{
			return (AgentLogLevels)base.Properties.GetValueWithNullReplacement("AgentLogLevel");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AgentLogLevel", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AgentMailType AgentMailType
	{
		get
		{
			return (AgentMailType)base.Properties.GetValueWithNullReplacement("AgentMailType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AgentMailType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int AgentShutdownWaitTime
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("AgentShutdownWaitTime");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AgentShutdownWaitTime", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string DatabaseMailProfile
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DatabaseMailProfile");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DatabaseMailProfile", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ErrorLogFile
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ErrorLogFile");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ErrorLogFile", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string HostLoginName => (string)base.Properties.GetValueWithNullReplacement("HostLoginName");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int IdleCpuDuration
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("IdleCpuDuration");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IdleCpuDuration", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int IdleCpuPercentage
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("IdleCpuPercentage");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IdleCpuPercentage", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsCpuPollingEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsCpuPollingEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsCpuPollingEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public JobServerType JobServerType => (JobServerType)base.Properties.GetValueWithNullReplacement("JobServerType");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string LocalHostAlias
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("LocalHostAlias");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("LocalHostAlias", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int LoginTimeout
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("LoginTimeout");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("LoginTimeout", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int MaximumHistoryRows
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("MaximumHistoryRows");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaximumHistoryRows", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int MaximumJobHistoryRows
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("MaximumJobHistoryRows");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaximumJobHistoryRows", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string MsxAccountCredentialName => (string)base.Properties.GetValueWithNullReplacement("MsxAccountCredentialName");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string MsxAccountName => (string)base.Properties.GetValueWithNullReplacement("MsxAccountName");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string MsxServerName => (string)base.Properties.GetValueWithNullReplacement("MsxServerName");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string NetSendRecipient
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("NetSendRecipient");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NetSendRecipient", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool ReplaceAlertTokensEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("ReplaceAlertTokensEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ReplaceAlertTokensEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool SaveInSentFolder
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("SaveInSentFolder");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SaveInSentFolder", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ServiceAccount => (string)base.Properties.GetValueWithNullReplacement("ServiceAccount");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public ServiceStartMode ServiceStartMode => (ServiceStartMode)base.Properties.GetValueWithNullReplacement("ServiceStartMode");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool SqlAgentAutoStart
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("SqlAgentAutoStart");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SqlAgentAutoStart", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string SqlAgentMailProfile
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("SqlAgentMailProfile");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SqlAgentMailProfile", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool SqlAgentRestart
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("SqlAgentRestart");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SqlAgentRestart", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool SqlServerRestart
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("SqlServerRestart");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SqlServerRestart", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool WriteOemErrorLog
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("WriteOemErrorLog");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("WriteOemErrorLog", value);
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Server Parent
	{
		get
		{
			CheckObjectState();
			return singletonParent as Server;
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public string Name => ((SimpleObjectKey)key).Name;

	public static string UrnSuffix => "JobServer";

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(JobCategory))]
	public JobCategoryCollection JobCategories
	{
		get
		{
			CheckObjectState();
			if (jobCategories == null)
			{
				jobCategories = new JobCategoryCollection(this);
			}
			return jobCategories;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(OperatorCategory))]
	public OperatorCategoryCollection OperatorCategories
	{
		get
		{
			CheckObjectState();
			if (operatorCategories == null)
			{
				operatorCategories = new OperatorCategoryCollection(this);
			}
			return operatorCategories;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(AlertCategory))]
	public AlertCategoryCollection AlertCategories
	{
		get
		{
			CheckObjectState();
			if (alertCategories == null)
			{
				alertCategories = new AlertCategoryCollection(this);
			}
			return alertCategories;
		}
	}

	[SfcObject(SfcObjectRelationship.Object, SfcObjectCardinality.One)]
	public AlertSystem AlertSystem
	{
		get
		{
			CheckObjectState();
			if (alertSystem == null)
			{
				alertSystem = new AlertSystem(this, new SimpleObjectKey(Name), SqlSmoState.Existing);
			}
			return alertSystem;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(Alert))]
	public AlertCollection Alerts
	{
		get
		{
			CheckObjectState();
			if (alerts == null)
			{
				alerts = new AlertCollection(this);
			}
			return alerts;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(Operator))]
	public OperatorCollection Operators
	{
		get
		{
			CheckObjectState();
			if (operators == null)
			{
				operators = new OperatorCollection(this);
			}
			return operators;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(TargetServer))]
	public TargetServerCollection TargetServers
	{
		get
		{
			CheckObjectState();
			if (targetServers == null)
			{
				targetServers = new TargetServerCollection(this);
			}
			return targetServers;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(TargetServerGroup))]
	public TargetServerGroupCollection TargetServerGroups
	{
		get
		{
			CheckObjectState();
			if (targetServerGroups == null)
			{
				targetServerGroups = new TargetServerGroupCollection(this);
			}
			return targetServerGroups;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(Job))]
	public JobCollection Jobs
	{
		get
		{
			CheckObjectState();
			if (jobs == null)
			{
				jobs = new JobCollection(this);
			}
			return jobs;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(JobSchedule))]
	public JobScheduleCollection SharedSchedules
	{
		get
		{
			CheckObjectState();
			ThrowIfBelowVersion90();
			if (sharedSchedules == null)
			{
				sharedSchedules = new JobScheduleCollection(this);
				sharedSchedules.AcceptDuplicateNames = true;
			}
			return sharedSchedules;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(ProxyAccount))]
	public ProxyAccountCollection ProxyAccounts
	{
		get
		{
			CheckObjectState();
			ThrowIfBelowVersion90();
			if (proxyAccounts == null)
			{
				proxyAccounts = new ProxyAccountCollection(this);
			}
			return proxyAccounts;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool SysAdminOnly
	{
		get
		{
			if (base.ServerVersion.Major >= 9)
			{
				throw new PropertyCannotBeRetrievedException("SysAdminOnly", this, ExceptionTemplatesImpl.ReasonPropertyIsNotSupportedOnCurrentServerVersion);
			}
			return (bool)base.Properties.GetValueWithNullReplacement("SysAdminOnly");
		}
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal JobServer(Server parentsrv, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentsrv;
		SetServerObject(parentsrv.GetServerObject());
		m_comparer = parentsrv.Databases["msdb"].StringComparer;
		jobCategories = null;
		alertCategories = null;
		operatorCategories = null;
		alertSystem = null;
		alerts = null;
		operators = null;
		targetServers = null;
		targetServerGroups = null;
		jobs = null;
		sharedSchedules = null;
	}

	public Job GetJobByID(Guid jobId)
	{
		try
		{
			foreach (Job job in Jobs)
			{
				if (jobId == job.JobID)
				{
					return job;
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.GetJobByID, this, ex);
		}
	}

	public void RemoveJobByID(Guid jobId)
	{
		try
		{
			foreach (Job job in Jobs)
			{
				if (jobId == job.JobID)
				{
					job.Drop();
					break;
				}
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveJobByID, this, ex);
		}
	}

	public void RemoveJobsByLogin(string login)
	{
		if (login == null)
		{
			throw new ArgumentNullException("login");
		}
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("EXEC msdb.dbo.sp_manage_jobs_by_login @action = N'DELETE', @current_owner_login_name = ");
			stringBuilder.Append(SqlSmoObject.MakeSqlString(login));
			ExecutionManager.ExecuteNonQuery(stringBuilder.ToString());
			Jobs.Refresh();
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveJobsByLogin, this, ex);
		}
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}

	protected internal override string GetDBName()
	{
		return "msdb";
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		ScriptProperties(queries, sp);
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		ScriptProperties(queries, sp);
	}

	private void ScriptProperties(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.Append("EXEC msdb.dbo.sp_set_sqlagent_properties ");
		int count = 0;
		GetBoolParameter(stringBuilder, sp, "SqlServerRestart", "@sqlserver_restart={0}", ref count);
		GetBoolParameter(stringBuilder, sp, "SqlAgentRestart", "@monitor_autostart={0}", ref count);
		GetBoolParameter(stringBuilder, sp, "SqlAgentAutoStart", "@auto_start={0}", ref count);
		GetParameter(stringBuilder, sp, "MaximumHistoryRows", "@jobhistory_max_rows={0}", ref count);
		GetParameter(stringBuilder, sp, "MaximumJobHistoryRows", "@jobhistory_max_rows_per_job={0}", ref count);
		GetStringParameter(stringBuilder, sp, "ErrorLogFile", "@errorlog_file=N'{0}'", ref count);
		GetEnumParameter(stringBuilder, sp, "AgentLogLevel", "@errorlogging_level={0}", typeof(AgentLogLevels), ref count);
		GetStringParameter(stringBuilder, sp, "NetSendRecipient", "@error_recipient=N'{0}'", ref count);
		GetParameter(stringBuilder, sp, "AgentShutdownWaitTime", "@job_shutdown_timeout={0}", ref count);
		GetStringParameter(stringBuilder, sp, "SqlAgentMailProfile", "@email_profile=N'{0}'", ref count);
		GetBoolParameter(stringBuilder, sp, "SaveInSentFolder", "@email_save_in_sent_folder={0}", ref count);
		GetBoolParameter(stringBuilder, sp, "WriteOemErrorLog", "@oem_errorlog={0}", ref count);
		GetBoolParameter(stringBuilder, sp, "IsCpuPollingEnabled", "@cpu_poller_enabled={0}", ref count);
		GetParameter(stringBuilder, sp, "IdleCpuPercentage", "@idle_cpu_percent={0}", ref count);
		GetParameter(stringBuilder, sp, "IdleCpuDuration", "@idle_cpu_duration={0}", ref count);
		GetParameter(stringBuilder, sp, "LoginTimeout", "@login_timeout={0}", ref count);
		GetStringParameter(stringBuilder, sp, "LocalHostAlias", "@local_host_server=N'{0}'", ref count);
		if (base.ServerVersion.Major >= 9 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			GetBoolParameter(stringBuilder, sp, "ReplaceAlertTokensEnabled", "@alert_replace_runtime_tokens={0}", ref count);
		}
		if (base.ServerVersion.Major >= 11 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version110)
		{
			GetStringParameter(stringBuilder, sp, "DatabaseMailProfile", "@databasemail_profile=N'{0}'", ref count);
			GetEnumParameter(stringBuilder, sp, "AgentMailType", "@use_databasemail={0}", typeof(AgentMailType), ref count);
		}
		else
		{
			Property property = base.Properties.Get("AgentMailType");
			if (property.Value != null && (property.Dirty || !sp.ScriptForAlter))
			{
				queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC master.dbo.xp_instance_regwrite N'HKEY_LOCAL_MACHINE', N'SOFTWARE\\Microsoft\\MSSQLServer\\SQLServerAgent', N'UseDatabaseMail', N'REG_DWORD', {0}", new object[1] { Enum.Format(typeof(AgentMailType), (AgentMailType)property.Value, "d") }));
			}
			property = base.Properties.Get("DatabaseMailProfile");
			if (property.Value != null && (property.Dirty || !sp.ScriptForAlter))
			{
				queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC master.dbo.xp_instance_regwrite N'HKEY_LOCAL_MACHINE', N'SOFTWARE\\Microsoft\\MSSQLServer\\SQLServerAgent', N'DatabaseMailProfile', N'REG_SZ', N'{0}'", new object[1] { SqlSmoObject.SqlString(property.Value.ToString()) }));
			}
		}
		if (count > 0)
		{
			queries.Add(stringBuilder.ToString());
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

	protected override bool ImplInitialize(string[] fields, OrderBy[] orderby)
	{
		string[] array = fields;
		OrderBy[] array2 = orderby;
		bool flag = false;
		if (fields != null)
		{
			foreach (string text in fields)
			{
				if (text == "Name")
				{
					flag = true;
					break;
				}
			}
			if (flag && 1 < fields.Length)
			{
				array = new string[fields.Length - 1];
				int num = 0;
				foreach (string text2 in fields)
				{
					if (text2 != "Name")
					{
						array[num++] = text2;
					}
				}
			}
		}
		flag = false;
		if (orderby != null)
		{
			foreach (OrderBy orderBy in orderby)
			{
				if (orderBy.Field == "Name")
				{
					flag = true;
				}
			}
			if (flag && 1 < orderby.Length)
			{
				array2 = new OrderBy[orderby.Length - 1];
				int num2 = 0;
				foreach (OrderBy orderBy2 in orderby)
				{
					if (orderBy2.Field != "Name")
					{
						array2[num2++] = orderBy2;
					}
				}
			}
		}
		return base.ImplInitialize(array, array2);
	}

	public void TestMailProfile(string profileName)
	{
		ThrowIfAboveVersion100();
		try
		{
			if (profileName == null || profileName.Length == 0)
			{
				profileName = "Outlook";
			}
			ExecutionManager.ExecuteNonQuery(string.Format(SmoApplication.DefaultCulture, "EXECUTE master.dbo.xp_sqlagent_notify N'N',null,null,null,N'M',N'{0}' ", new object[1] { SqlSmoObject.SqlString(profileName) }));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.TestMailProfile, this, ex);
		}
	}

	public void TestNetSend()
	{
		try
		{
			ThrowIfBelowVersion90();
			string text = base.Properties["NetSendRecipient"].Value as string;
			if (text.Length == 0)
			{
				throw new PropertyNotSetException("NetSendRecipient");
			}
			ExecutionManager.ExecuteNonQuery(string.Format(SmoApplication.DefaultCulture, "EXECUTE master.dbo.xp_sqlagent_notify N'N',null,null,null,N'N',N'{0}' ", new object[1] { SqlSmoObject.SqlString(text) }));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.TestNetSend, this, ex);
		}
	}

	public void PurgeJobHistory()
	{
		try
		{
			ExecutionManager.ExecuteNonQuery("EXEC msdb.dbo.sp_purge_jobhistory");
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.PurgeJobHistory, this, ex);
		}
	}

	public void SetHostLoginAccount(string loginName, string password)
	{
		ThrowIfAboveVersion80();
		try
		{
			if (loginName == null)
			{
				throw new ArgumentNullException("loginName");
			}
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			stringBuilder.Append("declare @arg varbinary(512)");
			stringBuilder.Append(Globals.newline);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "set @arg = cast (N'{0}' as varbinary(512))", new object[1] { SqlSmoObject.SqlString(password) });
			stringBuilder.Append(Globals.newline);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_set_sqlagent_properties @host_login_name=N'{0}', @host_login_password=@arg, @regular_connections = 1", new object[1] { SqlSmoObject.SqlString(loginName) });
			ExecutionManager.ExecuteNonQuery(stringBuilder.ToString());
			if (!ExecutionManager.Recording)
			{
				base.Properties.Get("HostLoginName").SetValue(loginName);
			}
			stringBuilder.Length = 0;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SetHostLoginAccount, this, ex);
		}
	}

	public void ClearHostLoginAccount()
	{
		try
		{
			ExecutionManager.ExecuteNonQuery("EXEC msdb.dbo.sp_set_sqlagent_properties @regular_connections = 0");
			if (!ExecutionManager.Recording)
			{
				base.Properties.Get("HostLoginName").SetValue(string.Empty);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ClearHostLoginAccount, this, ex);
		}
	}

	public void SetMsxAccount(string account, string password)
	{
		try
		{
			ThrowIfBelowVersion80SP3();
			if (base.ServerVersion.Major >= 9)
			{
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.UnsupportedVersion(base.ServerVersion.ToString()));
			}
			if (account == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("account"));
			}
			if (password == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("password"));
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			ParseAccountName(account, stringBuilder, stringBuilder2);
			ExecutionManager.ExecuteNonQuery(string.Format(SmoApplication.DefaultCulture, "EXEC master.dbo.xp_sqlagent_msx_account N'SET', N'{0}', N'{1}', N'{2}'", new object[3]
			{
				SqlSmoObject.SqlString(stringBuilder.ToString()),
				SqlSmoObject.SqlString(stringBuilder2.ToString()),
				SqlSmoObject.SqlString(password)
			}));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SetMsxAccount, this, ex);
		}
	}

	public void SetMsxAccount(string credentialName)
	{
		try
		{
			ThrowIfBelowVersion90();
			if (credentialName == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("credentialName"));
			}
			ExecutionManager.ExecuteNonQuery(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_msx_set_account @credential_name = {0}", new object[1] { SqlSmoObject.MakeSqlString(credentialName) }));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SetMsxAccount, this, ex);
		}
	}

	public void ClearMsxAccount()
	{
		try
		{
			if (base.ServerVersion.Major >= 9)
			{
				ExecutionManager.ExecuteNonQuery("EXEC msdb.dbo.sp_msx_set_account");
				return;
			}
			ThrowIfBelowVersion80SP3();
			ExecutionManager.ExecuteNonQuery("EXEC master.dbo.xp_sqlagent_msx_account N'DEL'");
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ClearMsxAccount, this, ex);
		}
	}

	private void ParseAccountName(string accountName, StringBuilder domainName, StringBuilder userName)
	{
		string[] array = accountName.Split('\\');
		if (array.Length == 2)
		{
			domainName.Append(array[0]);
			userName.Append(array[1]);
			return;
		}
		if (array.Length == 1)
		{
			userName.Append(array[0]);
			return;
		}
		throw new SmoException(ExceptionTemplatesImpl.InvalidAcctName);
	}

	public void CycleErrorLog()
	{
		try
		{
			ThrowIfBelowVersion90();
			ExecutionManager.ExecuteNonQuery("EXEC msdb.dbo.sp_cycle_agent_errorlog");
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CycleErrorLog, this, ex);
		}
	}

	public DataTable EnumErrorLogs()
	{
		try
		{
			Request req = new Request(base.Urn.Value + "/ErrorLog");
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumErrorLogs, this, ex);
		}
	}

	public DataTable ReadErrorLog()
	{
		try
		{
			return ReadErrorLog(0);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ReadErrorLog, this, ex);
		}
	}

	public DataTable ReadErrorLog(int logNumber)
	{
		try
		{
			Request req = new Request(string.Format(SmoApplication.DefaultCulture, "{0}/ErrorLog[@ArchiveNo='{1}']/LogEntry", new object[2] { base.Urn, logNumber }));
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ReadErrorLog, this, ex);
		}
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (alertSystem != null)
		{
			alertSystem.MarkDroppedInternal();
		}
		if (jobCategories != null)
		{
			jobCategories.MarkAllDropped();
		}
		if (alertCategories != null)
		{
			alertCategories.MarkAllDropped();
		}
		if (operatorCategories != null)
		{
			operatorCategories.MarkAllDropped();
		}
		if (alerts != null)
		{
			alerts.MarkAllDropped();
		}
		if (operators != null)
		{
			operators.MarkAllDropped();
		}
		if (targetServers != null)
		{
			targetServers.MarkAllDropped();
		}
		if (targetServerGroups != null)
		{
			targetServerGroups.MarkAllDropped();
		}
		if (jobs != null)
		{
			jobs.MarkAllDropped();
		}
		if (sharedSchedules != null)
		{
			sharedSchedules.MarkAllDropped();
		}
	}

	public DataTable EnumJobHistory(JobHistoryFilter filter)
	{
		try
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			return ExecutionManager.GetEnumeratorData(filter.GetEnumRequest(this));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumJobHistory, this, ex);
		}
	}

	public DataTable EnumJobHistory()
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			Request req = new Request(string.Concat(base.Urn, "/Job/History"));
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumJobHistory, this, ex);
		}
	}

	public DataTable EnumJobs(JobFilter filter)
	{
		DataTable dataTable = null;
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		try
		{
			dataTable = ExecutionManager.GetEnumeratorData(filter.GetEnumRequest(this));
			if (dataTable != null)
			{
				FilterJobsByExecutionStatus(filter, ref dataTable);
				FilterJobsByJobType(filter, ref dataTable);
				FilterJobsBySubSystem(filter, ref dataTable);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumJobs, this, ex);
		}
		return dataTable;
	}

	private void FilterJobsByExecutionStatus(JobFilter filter, ref DataTable jobsTable)
	{
		if (!filter.currentExecutionStatusDirty)
		{
			return;
		}
		List<DataRow> list = new List<DataRow>();
		foreach (DataRow row in jobsTable.Rows)
		{
			Job job = Jobs.ItemById((Guid)row["JobID"]);
			if (job != null && job.CurrentRunStatus != filter.CurrentExecutionStatus)
			{
				list.Add(row);
			}
		}
		foreach (DataRow item in list)
		{
			jobsTable.Rows.Remove(item);
		}
	}

	private void FilterJobsByJobType(JobFilter filter, ref DataTable jobsTable)
	{
		if (!filter.jobTypeDirty)
		{
			return;
		}
		List<DataRow> list = new List<DataRow>();
		foreach (DataRow row in jobsTable.Rows)
		{
			Job job = Jobs.ItemById((Guid)row["JobID"]);
			if (job != null && job.JobType != filter.JobType)
			{
				list.Add(row);
			}
		}
		foreach (DataRow item in list)
		{
			jobsTable.Rows.Remove(item);
		}
	}

	private void FilterJobsBySubSystem(JobFilter filter, ref DataTable jobsTable)
	{
		if (!filter.stepSubsystemDirty)
		{
			return;
		}
		List<DataRow> list = new List<DataRow>();
		foreach (DataRow row in jobsTable.Rows)
		{
			Job job = Jobs.ItemById((Guid)row["JobID"]);
			bool flag = false;
			if (job != null)
			{
				foreach (JobStep jobStep in job.JobSteps)
				{
					if (jobStep.SubSystem == filter.StepSubsystem)
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				list.Add(row);
			}
		}
		foreach (DataRow item in list)
		{
			jobsTable.Rows.Remove(item);
		}
	}

	public DataTable EnumJobs()
	{
		try
		{
			Request req = new Request(string.Concat(base.Urn, "/Job"));
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumJobs, this, ex);
		}
	}

	public DataTable EnumSubSystems()
	{
		try
		{
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add("EXEC msdb.dbo.sp_enum_sqlagent_subsystems");
			return ExecutionManager.ExecuteWithResults(stringCollection).Tables[0];
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumSubSystems, this, ex);
		}
	}

	public void MsxDefect()
	{
		try
		{
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add("EXEC msdb.dbo.sp_msx_defect");
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.MsxDefect, this, ex);
		}
	}

	public void MsxDefect(bool forceDefection)
	{
		try
		{
			StringCollection stringCollection = new StringCollection();
			string text = (forceDefection ? "1" : "0");
			string value = "EXEC msdb.dbo.sp_msx_defect @forced_defection = " + text;
			stringCollection.Add(value);
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.MsxDefect, this, ex);
		}
	}

	public void MsxEnlist(string masterServer, string location)
	{
		try
		{
			if (masterServer == null)
			{
				throw new ArgumentNullException("masterServer");
			}
			if (location == null)
			{
				throw new ArgumentNullException("location");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_msx_enlist @msx_server_name = N'{0}', @location = N'{1}'", new object[2]
			{
				SqlSmoObject.SqlString(masterServer),
				SqlSmoObject.SqlString(location)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.MsxEnlist, this, ex);
		}
	}

	public void PurgeJobHistory(JobHistoryFilter filter)
	{
		try
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add("EXEC msdb.dbo.sp_purge_jobhistory " + filter.GetPurgeFilter());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.PurgeJobHistory, this, ex);
		}
	}

	public void ReassignJobsByLogin(string oldLogin, string newLogin)
	{
		try
		{
			if (oldLogin == null)
			{
				throw new ArgumentNullException("oldLogin");
			}
			if (newLogin == null)
			{
				throw new ArgumentNullException("newLogin");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_manage_jobs_by_login @action = N'REASSIGN', @current_owner_login_name = N'{0}', @new_owner_login_name = N'{1}'", new object[2]
			{
				SqlSmoObject.SqlString(oldLogin),
				SqlSmoObject.SqlString(newLogin)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ReassignJobsByLogin, this, ex);
		}
	}

	public void DropJobByID(Guid jobid)
	{
		for (int i = 0; i < Jobs.Count; i++)
		{
			Job job = Jobs[i];
			if (jobid == job.JobID)
			{
				job.Drop();
				break;
			}
		}
	}

	public void DropJobsByLogin(string login)
	{
		try
		{
			if (login == null)
			{
				throw new ArgumentNullException("login");
			}
			for (int i = 0; i < Jobs.Count; i++)
			{
				Job job = Jobs[i];
				if (login == job.OwnerLoginName)
				{
					job.Drop();
					i--;
				}
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DropJobsByLogin, this, ex);
		}
	}

	public void DropJobsByServer(string serverName)
	{
		try
		{
			if (serverName == null)
			{
				throw new ArgumentNullException("serverName");
			}
			for (int i = 0; i < Jobs.Count; i++)
			{
				Job job = Jobs[i];
				if (string.Compare(serverName, job.OriginatingServer, StringComparison.OrdinalIgnoreCase) == 0)
				{
					job.Drop();
					i--;
				}
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DropJobsByServer, this, ex);
		}
	}

	public void StartMonitor(string netSendAddress, int restartAttempts)
	{
		try
		{
			if (netSendAddress == null)
			{
				throw new ArgumentNullException("netSendAddress");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC master.dbo.xp_sqlagent_monitor N'START', N'{0}', {1}", new object[2]
			{
				SqlSmoObject.SqlString(netSendAddress),
				restartAttempts
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.StartMonitor, this, ex);
		}
	}

	public void StopMonitor()
	{
		try
		{
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add("EXEC master.dbo.xp_sqlagent_monitor N'STOP'");
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.StopMonitor, this, ex);
		}
	}

	internal DataTable EnumPerfInfoInternal(string objectName, string counterName, string instanceName)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		if (objectName != null)
		{
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "@ObjectName = '{0}'", new object[1] { Urn.EscapeString(objectName) });
			flag = true;
		}
		if (counterName != null)
		{
			if (flag)
			{
				stringBuilder.Append(" and ");
			}
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "@CounterName = '{0}'", new object[1] { Urn.EscapeString(counterName) });
			flag = true;
		}
		if (instanceName != null)
		{
			if (flag)
			{
				stringBuilder.Append(" and ");
			}
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "@InstanceName = '{0}'", new object[1] { Urn.EscapeString(instanceName) });
			flag = true;
		}
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.Append(base.Urn.Value);
		stringBuilder2.Append("/PerfInfo");
		if (stringBuilder.Length > 0)
		{
			stringBuilder2.Append("[");
			stringBuilder2.Append(stringBuilder.ToString());
			stringBuilder2.Append("]");
		}
		Request req = new Request(stringBuilder2.ToString());
		return ExecutionManager.GetEnumeratorData(req);
	}

	public DataTable EnumPerformanceCounters()
	{
		try
		{
			return EnumPerfInfoInternal(null, null, null);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumPerformanceCounters, this, ex);
		}
	}

	public DataTable EnumPerformanceCounters(string objectName)
	{
		try
		{
			if (objectName == null)
			{
				throw new ArgumentNullException("objectName ");
			}
			return EnumPerfInfoInternal(objectName, null, null);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumPerformanceCounters, this, ex);
		}
	}

	public DataTable EnumPerformanceCounters(string objectName, string counterName)
	{
		try
		{
			if (objectName == null)
			{
				throw new ArgumentNullException("objectName ");
			}
			if (counterName == null)
			{
				throw new ArgumentNullException("objectName ");
			}
			return EnumPerfInfoInternal(objectName, counterName, null);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumPerformanceCounters, this, ex);
		}
	}

	public DataTable EnumPerformanceCounters(string objectName, string counterName, string instanceName)
	{
		try
		{
			if (objectName == null)
			{
				throw new ArgumentNullException("objectName ");
			}
			if (counterName == null)
			{
				throw new ArgumentNullException("objectName ");
			}
			if (instanceName == null)
			{
				throw new ArgumentNullException("objectName ");
			}
			return EnumPerfInfoInternal(objectName, counterName, instanceName);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumPerformanceCounters, this, ex);
		}
	}
}
