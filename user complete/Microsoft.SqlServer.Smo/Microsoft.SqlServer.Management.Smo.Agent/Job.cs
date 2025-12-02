using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class Job : AgentObjectBase, IAlterable, ICreatable, IDroppable, IDropIfExists, IRenamable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 31, 31, 31, 31, 31, 31, 31, 31, 31, 31 };

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
				"Category" => 0, 
				"CategoryID" => 1, 
				"CategoryType" => 2, 
				"CurrentRunRetryAttempt" => 3, 
				"CurrentRunStatus" => 4, 
				"CurrentRunStep" => 5, 
				"DateCreated" => 6, 
				"DateLastModified" => 7, 
				"DeleteLevel" => 8, 
				"Description" => 9, 
				"EmailLevel" => 10, 
				"EventLogLevel" => 11, 
				"HasSchedule" => 12, 
				"HasServer" => 13, 
				"HasStep" => 14, 
				"IsEnabled" => 15, 
				"JobID" => 16, 
				"JobType" => 17, 
				"LastRunDate" => 18, 
				"LastRunOutcome" => 19, 
				"NetSendLevel" => 20, 
				"NextRunDate" => 21, 
				"NextRunScheduleID" => 22, 
				"OperatorToEmail" => 23, 
				"OperatorToNetSend" => 24, 
				"OperatorToPage" => 25, 
				"OriginatingServer" => 26, 
				"OwnerLoginName" => 27, 
				"PageLevel" => 28, 
				"StartStepID" => 29, 
				"VersionNumber" => 30, 
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
			staticMetadata = new StaticMetadata[31]
			{
				new StaticMetadata("Category", expensive: true, readOnly: false, typeof(string)),
				new StaticMetadata("CategoryID", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("CategoryType", expensive: true, readOnly: false, typeof(byte)),
				new StaticMetadata("CurrentRunRetryAttempt", expensive: true, readOnly: true, typeof(int)),
				new StaticMetadata("CurrentRunStatus", expensive: true, readOnly: true, typeof(JobExecutionStatus)),
				new StaticMetadata("CurrentRunStep", expensive: true, readOnly: true, typeof(string)),
				new StaticMetadata("DateCreated", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("DeleteLevel", expensive: false, readOnly: false, typeof(CompletionAction)),
				new StaticMetadata("Description", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("EmailLevel", expensive: false, readOnly: false, typeof(CompletionAction)),
				new StaticMetadata("EventLogLevel", expensive: false, readOnly: false, typeof(CompletionAction)),
				new StaticMetadata("HasSchedule", expensive: true, readOnly: true, typeof(bool)),
				new StaticMetadata("HasServer", expensive: true, readOnly: true, typeof(bool)),
				new StaticMetadata("HasStep", expensive: true, readOnly: true, typeof(bool)),
				new StaticMetadata("IsEnabled", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("JobID", expensive: false, readOnly: true, typeof(Guid)),
				new StaticMetadata("JobType", expensive: true, readOnly: true, typeof(JobType)),
				new StaticMetadata("LastRunDate", expensive: true, readOnly: true, typeof(DateTime)),
				new StaticMetadata("LastRunOutcome", expensive: true, readOnly: true, typeof(CompletionResult)),
				new StaticMetadata("NetSendLevel", expensive: false, readOnly: false, typeof(CompletionAction)),
				new StaticMetadata("NextRunDate", expensive: true, readOnly: true, typeof(DateTime)),
				new StaticMetadata("NextRunScheduleID", expensive: true, readOnly: true, typeof(int)),
				new StaticMetadata("OperatorToEmail", expensive: true, readOnly: false, typeof(string)),
				new StaticMetadata("OperatorToNetSend", expensive: true, readOnly: false, typeof(string)),
				new StaticMetadata("OperatorToPage", expensive: true, readOnly: false, typeof(string)),
				new StaticMetadata("OriginatingServer", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("OwnerLoginName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("PageLevel", expensive: false, readOnly: false, typeof(CompletionAction)),
				new StaticMetadata("StartStepID", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("VersionNumber", expensive: false, readOnly: true, typeof(int))
			};
		}
	}

	private bool keepUnusedSchedules;

	private JobStepCollection jobSteps;

	private JobScheduleCollection jobSchedules;

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

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string Category
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Category");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Category", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public byte CategoryType
	{
		get
		{
			return (byte)base.Properties.GetValueWithNullReplacement("CategoryType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("CategoryType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int CurrentRunRetryAttempt => (int)base.Properties.GetValueWithNullReplacement("CurrentRunRetryAttempt");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public JobExecutionStatus CurrentRunStatus => (JobExecutionStatus)base.Properties.GetValueWithNullReplacement("CurrentRunStatus");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string CurrentRunStep => (string)base.Properties.GetValueWithNullReplacement("CurrentRunStep");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime DateCreated => (DateTime)base.Properties.GetValueWithNullReplacement("DateCreated");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public CompletionAction DeleteLevel
	{
		get
		{
			return (CompletionAction)base.Properties.GetValueWithNullReplacement("DeleteLevel");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DeleteLevel", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string Description
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Description");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Description", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public CompletionAction EmailLevel
	{
		get
		{
			return (CompletionAction)base.Properties.GetValueWithNullReplacement("EmailLevel");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EmailLevel", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public CompletionAction EventLogLevel
	{
		get
		{
			return (CompletionAction)base.Properties.GetValueWithNullReplacement("EventLogLevel");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EventLogLevel", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public bool HasSchedule => (bool)base.Properties.GetValueWithNullReplacement("HasSchedule");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public bool HasServer => (bool)base.Properties.GetValueWithNullReplacement("HasServer");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public bool HasStep => (bool)base.Properties.GetValueWithNullReplacement("HasStep");

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
	public Guid JobID => (Guid)base.Properties.GetValueWithNullReplacement("JobID");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public JobType JobType => (JobType)base.Properties.GetValueWithNullReplacement("JobType");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public DateTime LastRunDate => (DateTime)base.Properties.GetValueWithNullReplacement("LastRunDate");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public CompletionResult LastRunOutcome => (CompletionResult)base.Properties.GetValueWithNullReplacement("LastRunOutcome");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public CompletionAction NetSendLevel
	{
		get
		{
			return (CompletionAction)base.Properties.GetValueWithNullReplacement("NetSendLevel");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NetSendLevel", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public DateTime NextRunDate => (DateTime)base.Properties.GetValueWithNullReplacement("NextRunDate");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int NextRunScheduleID => (int)base.Properties.GetValueWithNullReplacement("NextRunScheduleID");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string OperatorToEmail
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("OperatorToEmail");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("OperatorToEmail", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string OperatorToNetSend
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("OperatorToNetSend");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("OperatorToNetSend", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string OperatorToPage
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("OperatorToPage");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("OperatorToPage", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string OriginatingServer => (string)base.Properties.GetValueWithNullReplacement("OriginatingServer");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string OwnerLoginName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("OwnerLoginName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("OwnerLoginName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public CompletionAction PageLevel
	{
		get
		{
			return (CompletionAction)base.Properties.GetValueWithNullReplacement("PageLevel");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PageLevel", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int StartStepID
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("StartStepID");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("StartStepID", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int VersionNumber => (int)base.Properties.GetValueWithNullReplacement("VersionNumber");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	[SfcKey(0)]
	public override string Name
	{
		get
		{
			return ((JobObjectKey)key).Name;
		}
		set
		{
			try
			{
				ValidateName(value);
				((JobObjectKey)key).Name = value;
				UpdateObjectState();
			}
			catch (Exception ex)
			{
				SqlSmoObject.FilterException(ex);
				throw new FailedOperationException(ExceptionTemplatesImpl.SetName, this, ex);
			}
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	[SfcKey(1)]
	public int CategoryID => ((JobObjectKey)key).CategoryID;

	public static string UrnSuffix => "Job";

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(JobStep))]
	public JobStepCollection JobSteps
	{
		get
		{
			CheckObjectState();
			if (jobSteps == null)
			{
				jobSteps = new JobStepCollection(this);
			}
			return jobSteps;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(JobSchedule))]
	public JobScheduleCollection JobSchedules
	{
		get
		{
			CheckObjectState();
			if (jobSchedules == null)
			{
				jobSchedules = new JobScheduleCollection(this);
				jobSchedules.AcceptDuplicateNames = true;
			}
			return jobSchedules;
		}
	}

	internal Guid JobIDInternal => (Guid)base.Properties["JobID"].Value;

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	public Job()
	{
		key = new JobObjectKey(null, 0);
	}

	public Job(JobServer jobServer, string name)
	{
		ValidateName(name);
		key = new JobObjectKey(name, 0);
		Parent = jobServer;
	}

	public Job(JobServer jobServer, string name, int categoryID)
	{
		ValidateName(name);
		key = new JobObjectKey(name, categoryID);
		Parent = jobServer;
	}

	internal Job(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		jobSteps = null;
		jobSchedules = null;
	}

	private void UpdateCategoryIDFromCategoryProperty()
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(Parent != null);
		string text = (string)base.Properties.Get("Category").Value;
		if (text != null)
		{
			JobCategory jobCategory = Parent.JobCategories[text];
			if (jobCategory == null)
			{
				Parent.JobCategories.Refresh();
				jobCategory = Parent.JobCategories[text];
			}
			if (jobCategory == null)
			{
				throw new InternalSmoErrorException(ExceptionTemplatesImpl.UnknownCategoryName(text));
			}
			jobCategory.Initialize(allProperties: true);
			((JobObjectKey)key).CategoryID = jobCategory.ID;
		}
	}

	private void UpdateCategoryIDFromServer()
	{
		if (ExecutionManager.Recording)
		{
			return;
		}
		string text = base.Urn;
		int categoryID = ((JobObjectKey)key).CategoryID;
		int num = (int)ExecutionManager.ExecuteScalar(string.Format(SmoApplication.DefaultCulture, "select category_id from msdb.dbo.sysjobs_view where job_id='{0}'", new object[1] { JobID }));
		if (num != categoryID)
		{
			((JobObjectKey)key).CategoryID = num;
			if (!SmoApplication.eventsSingleton.IsNullObjectCreated() && !SmoApplication.eventsSingleton.IsNullObjectDropped())
			{
				SmoApplication.eventsSingleton.CallObjectDropped(GetServerObject(), new ObjectDroppedEventArgs(text));
				SmoApplication.eventsSingleton.CallObjectCreated(GetServerObject(), new ObjectCreatedEventArgs(base.Urn, this));
			}
		}
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}[{1}]", new object[2]
		{
			UrnSuffix,
			((JobObjectKey)key).UrnFilter
		});
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.ForDirectExecution)
		{
			GetJobCreationScript(stringBuilder, sp);
			UpdateCategoryIDFromCategoryProperty();
		}
		else
		{
			GetJobScriptingScript(stringBuilder, sp);
		}
		queries.Add(stringBuilder.ToString());
	}

	private void DumpStringCollectionToBuilder(StringCollection coll, StringBuilder script)
	{
		StringEnumerator enumerator = coll.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				script.Append(current);
				script.Append(Globals.newline);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	private void GetJobScriptingScript(StringBuilder createQuery, ScriptingPreferences sp)
	{
		Initialize(allProperties: true);
		createQuery.Append("BEGIN TRANSACTION");
		createQuery.Append(Globals.newline);
		createQuery.Append("DECLARE @ReturnCode INT");
		createQuery.Append(Globals.newline);
		createQuery.Append("SELECT @ReturnCode = 0");
		createQuery.Append(Globals.newline);
		bool inScriptJob = sp.Agent.InScriptJob;
		sp.Agent.InScriptJob = true;
		StringCollection stringCollection = new StringCollection();
		if (Category.Length > 0)
		{
			JobCategory jobCategory = Parent.JobCategories[Category];
			if (jobCategory != null)
			{
				jobCategory.Initialize(allProperties: true);
				bool existenceCheck = sp.IncludeScripts.ExistenceCheck;
				sp.IncludeScripts.ExistenceCheck = true;
				jobCategory.ScriptCreateInternal(stringCollection, sp);
				sp.IncludeScripts.ExistenceCheck = existenceCheck;
			}
			DumpStringCollectionToBuilder(stringCollection, createQuery);
			stringCollection.Clear();
		}
		GetJobCreationScript(createQuery, sp);
		foreach (JobStep jobStep in JobSteps)
		{
			jobStep.Initialize(allProperties: true);
			jobStep.ScriptCreateInternal(stringCollection, sp);
			DumpStringCollectionToBuilder(stringCollection, createQuery);
			stringCollection.Clear();
			AddCheckErrorCode(createQuery);
		}
		object propValueOptional = GetPropValueOptional("StartStepID");
		if (propValueOptional != null)
		{
			createQuery.AppendFormat("EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = {0}", (int)propValueOptional);
			createQuery.Append(Globals.newline);
			AddCheckErrorCode(createQuery);
		}
		foreach (JobSchedule jobSchedule in JobSchedules)
		{
			jobSchedule.Initialize(allProperties: true);
			jobSchedule.ScriptCreateInternal(stringCollection, sp);
			DumpStringCollectionToBuilder(stringCollection, createQuery);
			stringCollection.Clear();
			AddCheckErrorCode(createQuery);
		}
		if (base.State == SqlSmoState.Existing)
		{
			DataTable dataTable = EnumTargetServers();
			foreach (DataRow row in dataTable.Rows)
			{
				string text = (string)row["ServerName"];
				if (string.Compare(text, ExecutionManager.TrueServerName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					text = "(local)";
				}
				createQuery.AppendFormat(SmoApplication.DefaultCulture, "EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'{0}'", new object[1] { SqlSmoObject.SqlString(text) });
				createQuery.Append(Globals.newline);
				AddCheckErrorCode(createQuery);
			}
		}
		createQuery.Append("COMMIT TRANSACTION");
		createQuery.Append(Globals.newline);
		createQuery.Append("GOTO EndSave");
		createQuery.Append(Globals.newline);
		createQuery.Append("QuitWithRollback:");
		createQuery.Append(Globals.newline);
		createQuery.Append("    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION");
		createQuery.Append(Globals.newline);
		createQuery.Append("EndSave:");
		createQuery.Append(Globals.newline);
		stringCollection.Add(createQuery.ToString());
		sp.Agent.InScriptJob = inScriptJob;
	}

	internal static void AddCheckErrorCode(StringBuilder query)
	{
		query.Append("IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback");
		query.Append(Globals.newline);
	}

	internal static string GetReturnCode(ScriptingPreferences sp)
	{
		if (sp.Agent.InScriptJob)
		{
			return "@ReturnCode = ";
		}
		return string.Empty;
	}

	private void GetJobCreationScript(StringBuilder createQuery, ScriptingPreferences sp)
	{
		if (sp.OldOptions.PrimaryObject)
		{
			createQuery.Append("DECLARE @jobId BINARY(16)");
			createQuery.Append(Globals.newline);
			if (sp.IncludeScripts.ExistenceCheck)
			{
				createQuery.AppendFormat("select @jobId = job_id from msdb.dbo.sysjobs where (name = N'{0}')", SqlSmoObject.SqlString(Name));
				createQuery.Append(Globals.newline);
				createQuery.Append("if (@jobId is NULL)");
				createQuery.Append(Globals.newline);
				createQuery.Append("BEGIN");
				createQuery.Append(Globals.newline);
			}
			createQuery.AppendFormat(SmoApplication.DefaultCulture, "EXEC {0} msdb.dbo.sp_add_job @job_name=N'{1}'", new object[2]
			{
				GetReturnCode(sp),
				SqlSmoObject.SqlString(Name)
			});
			int count = 1;
			GetAllParams(createQuery, sp, forAlter: false, ref count);
			createQuery.Append(", @job_id = @jobId OUTPUT");
			if (sp.Agent.InScriptJob)
			{
				createQuery.Append(Globals.newline);
				AddCheckErrorCode(createQuery);
			}
			if (sp.IncludeScripts.ExistenceCheck)
			{
				createQuery.Append(Globals.newline);
				createQuery.Append("END");
				createQuery.Append(Globals.newline);
			}
			if (sp.ForDirectExecution)
			{
				createQuery.Append(Globals.newline);
				createQuery.Append("select @jobId");
			}
		}
	}

	private void GetAllParams(StringBuilder sb, ScriptingPreferences sp, bool forAlter, ref int count)
	{
		GetBoolParameter(sb, sp, "IsEnabled", "@enabled={0}", ref count);
		if (sp.ScriptForAlter || sp.ForDirectExecution)
		{
			GetParameter(sb, sp, "StartStepID", "@start_step_id={0}", ref count);
		}
		GetEnumParameter(sb, sp, "EventLogLevel", "@notify_level_eventlog={0}", typeof(CompletionAction), ref count);
		GetEnumParameter(sb, sp, "EmailLevel", "@notify_level_email={0}", typeof(CompletionAction), ref count);
		GetEnumParameter(sb, sp, "NetSendLevel", "@notify_level_netsend={0}", typeof(CompletionAction), ref count);
		GetEnumParameter(sb, sp, "PageLevel", "@notify_level_page={0}", typeof(CompletionAction), ref count);
		GetEnumParameter(sb, sp, "DeleteLevel", "@delete_level={0}", typeof(CompletionAction), ref count);
		GetStringParameter(sb, sp, "Description", "@description=N'{0}'", ref count);
		if (base.Properties.Get("Category").Value != null)
		{
			GetStringParameter(sb, sp, "Category", "@category_name=N'{0}'", ref count);
		}
		else if (!forAlter && count++ > 0)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, "{0}{1}{2}{3}@category_id={4}", Globals.commaspace, Globals.newline, Globals.tab, Globals.tab, CategoryID);
		}
		GetStringParameter(sb, sp, "OwnerLoginName", "@owner_login_name=N'{0}'", ref count);
		GetPropValueOptional("OperatorToEmail");
		GetStringParameter(sb, sp, "OperatorToEmail", "@notify_email_operator_name=N'{0}'", ref count);
		GetPropValueOptional("OperatorToNetSend");
		GetStringParameter(sb, sp, "OperatorToNetSend", "@notify_netsend_operator_name=N'{0}'", ref count);
		GetPropValueOptional("OperatorToPage");
		GetStringParameter(sb, sp, "OperatorToPage", "@notify_page_operator_name=N'{0}'", ref count);
	}

	public void Drop(bool keepUnusedSchedules)
	{
		ThrowIfBelowVersion90();
		try
		{
			this.keepUnusedSchedules = keepUnusedSchedules;
			DropImpl();
		}
		finally
		{
			this.keepUnusedSchedules = false;
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
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AGENT_JOB, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.TargetServerVersionInternal <= SqlServerVersionInternal.Version80)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_job {0}", new object[1] { JobIdOrJobNameParameter(sp) });
		}
		else
		{
			int num = ((!keepUnusedSchedules) ? 1 : 0);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_job {0}, @delete_unused_schedule={1}", new object[2]
			{
				JobIdOrJobNameParameter(sp),
				num
			});
		}
		queries.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		try
		{
			string name = Name;
			string oldUrn = base.Urn;
			bool dirty = base.Properties.Get("Category").Dirty;
			AlterImplWorker();
			if (ExecutionManager.Recording || SmoApplication.eventsSingleton.IsNullObjectAltered())
			{
				return;
			}
			if (dirty)
			{
				if (!SmoApplication.eventsSingleton.IsNullObjectRenamed())
				{
					SmoApplication.eventsSingleton.CallObjectRenamed(GetServerObject(), new ObjectRenamedEventArgs(base.Urn, this, name, Name, oldUrn));
				}
			}
			else if (!SmoApplication.eventsSingleton.IsNullObjectAltered())
			{
				SmoApplication.eventsSingleton.CallObjectAltered(GetServerObject(), new ObjectAlteredEventArgs(base.Urn, this));
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Alter, this, ex);
		}
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_job {0}", new object[1] { JobIdOrJobNameParameter() });
		int count = 1;
		GetAllParams(stringBuilder, sp, forAlter: true, ref count);
		if (count > 1)
		{
			queries.Add(stringBuilder.ToString());
			if (sp.ForDirectExecution)
			{
				UpdateCategoryIDFromCategoryProperty();
			}
		}
	}

	public void Rename(string newName)
	{
		RenameImpl(newName);
	}

	internal override void ScriptRename(StringCollection queries, ScriptingPreferences sp, string newName)
	{
		queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_job {0}, @new_name=N'{1}'", new object[2]
		{
			JobIdOrJobNameParameter(),
			SqlSmoObject.SqlString(newName)
		}));
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (jobSchedules != null)
		{
			jobSchedules.MarkAllDropped();
		}
		if (jobSteps != null)
		{
			jobSteps.MarkAllDropped();
		}
	}

	internal string JobIdOrJobNameParameter()
	{
		return JobIdOrJobNameParameter(null);
	}

	internal string JobIdOrJobNameParameter(ScriptingPreferences sp)
	{
		return JobIdOrJobNameParameter(sp, prefixAssignmentCode: true);
	}

	internal string JobIdOrJobNameParameter(ScriptingPreferences sp, bool prefixAssignmentCode)
	{
		if (sp != null && sp.Agent.InScriptJob)
		{
			if (!prefixAssignmentCode)
			{
				return "@jobId";
			}
			return "@job_id=@jobId";
		}
		if (sp == null || sp.ScriptForCreateDrop || sp.Agent.JobId)
		{
			string format = ((!prefixAssignmentCode) ? "N'{0}'" : "@job_id=N'{0}'");
			object propValueOptional = GetPropValueOptional("JobID");
			if (propValueOptional != null)
			{
				return string.Format(SmoApplication.DefaultCulture, format, new object[1] { SqlSmoObject.SqlString(JobIDInternal.ToString("D", SmoApplication.DefaultCulture)) });
			}
		}
		return string.Format(SmoApplication.DefaultCulture, "@job_name=N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
	}

	public void ApplyToTargetServer(string serverName)
	{
		try
		{
			CheckObjectState();
			ExecutionManager.ExecuteNonQuery(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_add_jobserver {0}, @server_name = {1}", new object[2]
			{
				JobIdOrJobNameParameter(),
				string.IsNullOrEmpty(serverName) ? "NULL" : SqlSmoObject.MakeSqlString(serverName)
			}));
			UpdateCategoryIDFromServer();
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ApplyToTargetServer, this, ex);
		}
	}

	public void ApplyToTargetServerGroup(string groupName)
	{
		try
		{
			if (groupName == null)
			{
				throw new ArgumentNullException("groupName");
			}
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_apply_job_to_targets {0}, @target_server_groups = N'{1}'", new object[2]
			{
				JobIdOrJobNameParameter(),
				SqlSmoObject.SqlString(groupName)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
			UpdateCategoryIDFromServer();
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ApplyToTargetServerGroup, this, ex);
		}
	}

	public DataTable EnumAlerts()
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			Request req = new Request(string.Concat(base.Urn.Parent, "/Alert[@JobID='", JobID.ToString(), "']"));
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumAlerts, this, ex);
		}
	}

	public DataTable EnumHistory(JobHistoryFilter filter)
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			return ExecutionManager.GetEnumeratorData(filter.GetEnumRequest(this));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumHistory, this, ex);
		}
	}

	public DataTable EnumHistory()
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			Request req = new Request(string.Concat(base.Urn, "/History"));
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumHistory, this, ex);
		}
	}

	public DataTable EnumJobStepLogs()
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState(throwIfNotCreated: true);
			return ExecutionManager.GetEnumeratorData(new Request(string.Concat(base.Urn, "/Step/OutputLog")));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumJobStepOutputLogs, this, ex);
		}
	}

	public DataTable EnumJobStepLogs(int stepId)
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState(throwIfNotCreated: true);
			return ExecutionManager.GetEnumeratorData(new Request(string.Concat(base.Urn, string.Format(SmoApplication.DefaultCulture, "/Step[@ID={0}]/OutputLog", new object[1] { stepId }))));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumJobStepOutputLogs, this, ex);
		}
	}

	public DataTable EnumJobStepLogs(string stepName)
	{
		try
		{
			if (stepName == null)
			{
				throw new ArgumentNullException("stepName");
			}
			ThrowIfBelowVersion90();
			CheckObjectState(throwIfNotCreated: true);
			return ExecutionManager.GetEnumeratorData(new Request(string.Concat(base.Urn, string.Format(SmoApplication.DefaultCulture, "/Step[@Name='{0}']/OutputLog", new object[1] { Urn.EscapeString(stepName) }))));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumJobStepOutputLogs, this, ex);
		}
	}

	public DataTable EnumTargetServers()
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			return ExecutionManager.GetEnumeratorData(new Request(string.Concat(base.Urn, "/TargetServer")));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumTargetServers, this, ex);
		}
	}

	public JobStep[] EnumJobStepsByID()
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			DataTable enumeratorData = ExecutionManager.GetEnumeratorData(new Request(string.Concat(base.Urn, "/Step"), new string[1] { "Name" }, new OrderBy[1]
			{
				new OrderBy("ID", OrderBy.Direction.Asc)
			}));
			JobStep[] array = new JobStep[enumeratorData.Rows.Count];
			int num = 0;
			foreach (DataRow row in enumeratorData.Rows)
			{
				array[num++] = JobSteps[(string)row["Name"]];
			}
			return array;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumJobSteps, this, ex);
		}
	}

	public void DeleteJobStepLogs(DateTime olderThan)
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_jobsteplog @job_name=N'{0}', @older_than='{1}'", new object[2]
			{
				SqlSmoObject.SqlString(Name),
				olderThan.ToString("MM/dd/yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo)
			});
			ExecutionManager.ExecuteNonQuery(stringBuilder.ToString());
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DeleteJobStepLogs, this, ex);
		}
	}

	public void DeleteJobStepLogs(int largerThan)
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_jobsteplog @job_name=N'{0}', @larger_than={1}", new object[2]
			{
				SqlSmoObject.SqlString(Name),
				largerThan
			});
			ExecutionManager.ExecuteNonQuery(stringBuilder.ToString());
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DeleteJobStepLogs, this, ex);
		}
	}

	public void Invoke()
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_sqlagent_notify @op_type = N'J', @job_id = N'{0}', @schedule_id = NULL, @alert_id = NULL, @action_type = N'S'", new object[1] { SqlSmoObject.SqlString(JobIDInternal.ToString("D", SmoApplication.DefaultCulture)) }));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Invoke, this, ex);
		}
	}

	public void PurgeHistory()
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_purge_jobhistory {0}", new object[1] { JobIdOrJobNameParameter() }));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.PurgeHistory, this, ex);
		}
	}

	public void AddSharedSchedule(int scheduleId)
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState(throwIfNotCreated: true);
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_attach_schedule {0},@schedule_id={1}", new object[2]
			{
				JobIdOrJobNameParameter(),
				scheduleId
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddSharedSchedule, this, ex);
		}
	}

	private JobSchedule GetJobScheduleByID(int scheduleId)
	{
		JobSchedules.Refresh();
		JobSchedule jobSchedule = JobSchedules.ItemById(scheduleId);
		if (jobSchedule == null)
		{
			throw new MissingObjectException(ExceptionTemplatesImpl.ObjectDoesNotExist("ScheduleId", scheduleId.ToString()));
		}
		return jobSchedule;
	}

	public void RemoveSharedSchedule(int scheduleId)
	{
		try
		{
			GetJobScheduleByID(scheduleId).Drop();
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveSharedSchedule, this, ex);
		}
	}

	public void RemoveSharedSchedule(int scheduleId, bool keepUnusedSchedules)
	{
		try
		{
			GetJobScheduleByID(scheduleId).Drop(keepUnusedSchedules);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveSharedSchedule, this, ex);
		}
	}

	public void RemoveAllJobSchedules()
	{
		try
		{
			while (JobSchedules != null && JobSchedules.Count > 0)
			{
				JobSchedules[0].Drop();
			}
			jobSchedules = null;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveAllJobSchedules, this, ex);
		}
	}

	public void RemoveAllJobSchedules(bool keepUnusedSchedules)
	{
		try
		{
			while (JobSchedules != null && JobSchedules.Count > 0)
			{
				JobSchedules[0].Drop(keepUnusedSchedules);
			}
			jobSchedules = null;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveAllJobSchedules, this, ex);
		}
	}

	public void RemoveAllJobSteps()
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_jobstep {0}, @step_id = 0", new object[1] { JobIdOrJobNameParameter() }));
			ExecutionManager.ExecuteNonQuery(stringCollection);
			jobSteps = null;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveAllJobSteps, this, ex);
		}
	}

	public void RemoveFromTargetServer(string serverName)
	{
		try
		{
			if (serverName == null)
			{
				throw new ArgumentNullException("serverName");
			}
			CheckObjectState(throwIfNotCreated: true);
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_jobserver {0}, @server_name = N'{1}'", new object[2]
			{
				JobIdOrJobNameParameter(),
				SqlSmoObject.SqlString(serverName)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
			UpdateCategoryIDFromServer();
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveFromTargetServer, this, ex);
		}
	}

	public void RemoveFromTargetServerGroup(string groupName)
	{
		try
		{
			if (groupName == null)
			{
				throw new ArgumentNullException("groupName");
			}
			CheckObjectState(throwIfNotCreated: true);
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_remove_job_from_targets {0}, @target_server_groups = N'{1}'", new object[2]
			{
				JobIdOrJobNameParameter(),
				SqlSmoObject.SqlString(groupName)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveFromTargetServerGroup, this, ex);
		}
	}

	public void Start(string jobStepName)
	{
		try
		{
			if (jobStepName == null)
			{
				throw new ArgumentNullException("jobStepName");
			}
			CheckObjectState(throwIfNotCreated: true);
			StartImpl(jobStepName);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Start, this, ex);
		}
	}

	public void Start()
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			StartImpl(null);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Start, this, ex);
		}
	}

	internal void StartImpl(string jobStepName)
	{
		StringCollection stringCollection = new StringCollection();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_start_job {0}", new object[1] { JobIdOrJobNameParameter() });
		if (jobStepName != null)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", @step_name=N'{0}' ", new object[1] { SqlSmoObject.SqlString(jobStepName) });
		}
		stringCollection.Add(stringBuilder.ToString());
		ExecutionManager.ExecuteNonQuery(stringCollection);
	}

	public void Stop()
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_stop_job {0}", new object[1] { JobIdOrJobNameParameter() }));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Stop, this, ex);
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

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[1] { "JobID" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}
