using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo.Agent;

[SfcElementType("Step")]
public sealed class JobStep : AgentObjectBase, IAlterable, ICreatable, IDroppable, IDropIfExists, IRenamable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 20, 20, 21, 21, 21, 21, 21, 21, 21, 21 };

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
				"Command" => 0, 
				"CommandExecutionSuccessCode" => 1, 
				"DatabaseName" => 2, 
				"DatabaseUserName" => 3, 
				"ID" => 4, 
				"JobStepFlags" => 5, 
				"LastRunDate" => 6, 
				"LastRunDuration" => 7, 
				"LastRunOutcome" => 8, 
				"LastRunRetries" => 9, 
				"OnFailAction" => 10, 
				"OnFailStep" => 11, 
				"OnSuccessAction" => 12, 
				"OnSuccessStep" => 13, 
				"OSRunPriority" => 14, 
				"OutputFileName" => 15, 
				"RetryAttempts" => 16, 
				"RetryInterval" => 17, 
				"Server" => 18, 
				"SubSystem" => 19, 
				"ProxyName" => 20, 
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
			staticMetadata = new StaticMetadata[21]
			{
				new StaticMetadata("Command", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("CommandExecutionSuccessCode", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("DatabaseName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("DatabaseUserName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ID", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("JobStepFlags", expensive: false, readOnly: false, typeof(JobStepFlags)),
				new StaticMetadata("LastRunDate", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("LastRunDuration", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("LastRunOutcome", expensive: false, readOnly: true, typeof(CompletionResult)),
				new StaticMetadata("LastRunRetries", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("OnFailAction", expensive: false, readOnly: false, typeof(StepCompletionAction)),
				new StaticMetadata("OnFailStep", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("OnSuccessAction", expensive: false, readOnly: false, typeof(StepCompletionAction)),
				new StaticMetadata("OnSuccessStep", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("OSRunPriority", expensive: false, readOnly: false, typeof(OSRunPriority)),
				new StaticMetadata("OutputFileName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("RetryAttempts", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("RetryInterval", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("Server", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("SubSystem", expensive: false, readOnly: false, typeof(AgentSubSystem)),
				new StaticMetadata("ProxyName", expensive: false, readOnly: false, typeof(string))
			};
		}
	}

	private int stepIDInternal;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Job Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as Job;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string Command
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Command");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Command", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int CommandExecutionSuccessCode
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("CommandExecutionSuccessCode");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("CommandExecutionSuccessCode", value);
		}
	}

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
	public string DatabaseUserName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DatabaseUserName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DatabaseUserName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public int ID
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("ID");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ID", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public JobStepFlags JobStepFlags
	{
		get
		{
			return (JobStepFlags)base.Properties.GetValueWithNullReplacement("JobStepFlags");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("JobStepFlags", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime LastRunDate => (DateTime)base.Properties.GetValueWithNullReplacement("LastRunDate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int LastRunDuration => (int)base.Properties.GetValueWithNullReplacement("LastRunDuration");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public CompletionResult LastRunOutcome => (CompletionResult)base.Properties.GetValueWithNullReplacement("LastRunOutcome");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int LastRunRetries => (int)base.Properties.GetValueWithNullReplacement("LastRunRetries");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public StepCompletionAction OnFailAction
	{
		get
		{
			return (StepCompletionAction)base.Properties.GetValueWithNullReplacement("OnFailAction");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("OnFailAction", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int OnFailStep
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("OnFailStep");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("OnFailStep", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public StepCompletionAction OnSuccessAction
	{
		get
		{
			return (StepCompletionAction)base.Properties.GetValueWithNullReplacement("OnSuccessAction");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("OnSuccessAction", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int OnSuccessStep
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("OnSuccessStep");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("OnSuccessStep", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public OSRunPriority OSRunPriority
	{
		get
		{
			return (OSRunPriority)base.Properties.GetValueWithNullReplacement("OSRunPriority");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("OSRunPriority", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string OutputFileName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("OutputFileName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("OutputFileName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ProxyName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ProxyName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ProxyName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int RetryAttempts
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("RetryAttempts");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RetryAttempts", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int RetryInterval
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("RetryInterval");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RetryInterval", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string Server
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Server");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Server", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AgentSubSystem SubSystem
	{
		get
		{
			return (AgentSubSystem)base.Properties.GetValueWithNullReplacement("SubSystem");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SubSystem", value);
		}
	}

	public static string UrnSuffix => "Step";

	internal int StepIDInternal => (int)base.Properties["ID"].Value;

	public JobStep()
	{
	}

	public JobStep(Job job, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = job;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[1] { "ID" };
	}

	internal JobStep(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
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
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AGENT_JOBSTEP, new object[3]
			{
				"NOT",
				((Job)base.ParentColl.ParentInstance).JobIdOrJobNameParameter(sp, prefixAssignmentCode: false),
				StepIDInternal
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC {0}msdb.dbo.sp_add_jobstep {1}, @step_name=N'{2}'", new object[3]
		{
			Job.GetReturnCode(sp),
			((Job)base.ParentColl.ParentInstance).JobIdOrJobNameParameter(sp),
			SqlSmoObject.SqlString(Name)
		});
		int count = 2;
		GetParameter(stringBuilder, sp, "ID", "@step_id={0}", ref count);
		GetAllParams(stringBuilder, sp, ref count);
		queries.Add(stringBuilder.ToString());
	}

	private void GetAllParams(StringBuilder sb, ScriptingPreferences sp, ref int count)
	{
		GetParameter(sb, sp, "CommandExecutionSuccessCode", "@cmdexec_success_code={0}", ref count);
		GetEnumParameter(sb, sp, "OnSuccessAction", "@on_success_action={0}", typeof(StepCompletionAction), ref count);
		GetParameter(sb, sp, "OnSuccessStep", "@on_success_step_id={0}", ref count);
		GetEnumParameter(sb, sp, "OnFailAction", "@on_fail_action={0}", typeof(StepCompletionAction), ref count);
		GetParameter(sb, sp, "OnFailStep", "@on_fail_step_id={0}", ref count);
		GetParameter(sb, sp, "RetryAttempts", "@retry_attempts={0}", ref count);
		GetParameter(sb, sp, "RetryInterval", "@retry_interval={0}", ref count);
		GetEnumParameter(sb, sp, "OSRunPriority", "@os_run_priority={0}", typeof(OSRunPriority), ref count);
		Property property = base.Properties.Get("SubSystem");
		if (property.Value != null && (!sp.ScriptForAlter || property.Dirty))
		{
			if (count++ > 0)
			{
				sb.Append(Globals.commaspace);
			}
			string s = "TSQL";
			switch ((AgentSubSystem)property.Value)
			{
			case AgentSubSystem.ActiveScripting:
				s = "ActiveScripting";
				break;
			case AgentSubSystem.AnalysisCommand:
				s = "ANALYSISCOMMAND";
				GetPropValue("Server");
				break;
			case AgentSubSystem.AnalysisQuery:
				s = "ANALYSISQUERY";
				break;
			case AgentSubSystem.CmdExec:
				s = "CmdExec";
				break;
			case AgentSubSystem.Distribution:
				s = "Distribution";
				break;
			case AgentSubSystem.Ssis:
				s = "SSIS";
				break;
			case AgentSubSystem.LogReader:
				s = "LogReader";
				break;
			case AgentSubSystem.Merge:
				s = "Merge";
				break;
			case AgentSubSystem.QueueReader:
				s = "QueueReader";
				break;
			case AgentSubSystem.Snapshot:
				s = "Snapshot";
				break;
			case AgentSubSystem.TransactSql:
				s = "TSQL";
				break;
			case AgentSubSystem.PowerShell:
				s = "PowerShell";
				break;
			}
			sb.AppendFormat(SmoApplication.DefaultCulture, "@subsystem=N'{0}'", new object[1] { SqlSmoObject.SqlString(s) });
		}
		GetStringParameter(sb, sp, "Command", "@command=N'{0}'", ref count);
		if (sp.TargetDatabaseEngineEdition != DatabaseEngineEdition.SqlManagedInstance)
		{
			GetStringParameter(sb, sp, "Server", "@server=N'{0}'", ref count);
		}
		GetStringParameter(sb, sp, "DatabaseName", "@database_name=N'{0}'", ref count);
		GetStringParameter(sb, sp, "DatabaseUserName", "@database_user_name=N'{0}'", ref count);
		GetStringParameter(sb, sp, "OutputFileName", "@output_file_name=N'{0}'", ref count);
		GetEnumParameter(sb, sp, "JobStepFlags", "@flags={0}", typeof(JobStepFlags), ref count);
		if (base.ServerVersion.Major >= 9 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			GetStringParameter(sb, sp, "ProxyName", "@proxy_name=N'{0}'", ref count);
		}
		if (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90)
		{
			Property property2 = base.Properties.Get("JobStepFlags");
			if (property2.Value != null && (JobStepFlags)property2.Value >= JobStepFlags.LogToTableWithOverwrite)
			{
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.InvalidPropertyValueForVersion(GetType().Name, "JobStepFlags", property2.Value.ToString(), GetSqlServerVersionName()));
			}
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
		stepIDInternal = StepIDInternal;
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AGENT_JOBSTEP, new object[3]
			{
				"",
				((Job)base.ParentColl.ParentInstance).JobIdOrJobNameParameter(sp, prefixAssignmentCode: false),
				stepIDInternal
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_jobstep {0}, @step_id={1}", new object[2]
		{
			((Job)base.ParentColl.ParentInstance).JobIdOrJobNameParameter(sp),
			stepIDInternal
		});
		queries.Add(stringBuilder.ToString());
	}

	protected override void PostDrop()
	{
		if (ExecutionManager.Recording)
		{
			return;
		}
		JobStepCollection jobStepCollection = (JobStepCollection)base.ParentColl;
		if (!jobStepCollection.initialized)
		{
			return;
		}
		Property property = null;
		foreach (JobStep item in jobStepCollection)
		{
			property = item.Properties.Get("ID");
			if (property.Value != null && (int)property.Value >= stepIDInternal)
			{
				property.SetValue((int)property.Value - 1);
			}
		}
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_jobstep {0}, @step_id={1} ", new object[2]
		{
			((Job)base.ParentColl.ParentInstance).JobIdOrJobNameParameter(),
			StepIDInternal
		});
		int count = 2;
		GetAllParams(stringBuilder, sp, ref count);
		if (count > 2)
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
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_jobstep {0}, @step_id={1}, @step_name=N'{2}'", new object[3]
		{
			((Job)base.ParentColl.ParentInstance).JobIdOrJobNameParameter(),
			StepIDInternal,
			SqlSmoObject.SqlString(newName)
		});
		queries.Add(stringBuilder.ToString());
	}

	public DataTable EnumLogs()
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState(throwIfNotCreated: true);
			return ExecutionManager.GetEnumeratorData(new Request(string.Concat(base.Urn, "/OutputLog")));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumJobStepOutputLogs, this, ex);
		}
	}

	public void DeleteLogs(DateTime olderThan)
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_jobsteplog @job_name=N'{0}', @step_name=N'{1}', @older_than='{2}'", new object[3]
			{
				SqlSmoObject.SqlString(Parent.Name),
				SqlSmoObject.SqlString(Name),
				olderThan.ToString("MM/dd/yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo)
			});
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DeleteJobStepLogs, this, ex);
		}
	}

	public void DeleteLogs(int largerThan)
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_jobsteplog @job_name=N'{0}', @step_name=N'{1}', @larger_than='{2}'", new object[3]
			{
				SqlSmoObject.SqlString(Parent.Name),
				SqlSmoObject.SqlString(Name),
				largerThan
			});
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DeleteJobStepLogs, this, ex);
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
