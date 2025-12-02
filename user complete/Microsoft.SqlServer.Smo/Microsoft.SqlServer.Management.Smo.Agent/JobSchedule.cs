using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo.Agent;

[SfcElementType("Schedule")]
public sealed class JobSchedule : ScheduleBase, IAlterable, ICreatable, IDroppable, IDropIfExists, IRenamable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 14, 14, 15, 15, 15, 15, 15, 15, 15, 15 };

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
				"ActiveEndDate" => 0, 
				"ActiveEndTimeOfDay" => 1, 
				"ActiveStartDate" => 2, 
				"ActiveStartTimeOfDay" => 3, 
				"DateCreated" => 4, 
				"FrequencyInterval" => 5, 
				"FrequencyRecurrenceFactor" => 6, 
				"FrequencyRelativeIntervals" => 7, 
				"FrequencySubDayInterval" => 8, 
				"FrequencySubDayTypes" => 9, 
				"FrequencyTypes" => 10, 
				"ID" => 11, 
				"IsEnabled" => 12, 
				"JobCount" => 13, 
				"ScheduleUid" => 14, 
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
			staticMetadata = new StaticMetadata[15]
			{
				new StaticMetadata("ActiveEndDate", expensive: false, readOnly: false, typeof(DateTime)),
				new StaticMetadata("ActiveEndTimeOfDay", expensive: false, readOnly: false, typeof(TimeSpan)),
				new StaticMetadata("ActiveStartDate", expensive: false, readOnly: false, typeof(DateTime)),
				new StaticMetadata("ActiveStartTimeOfDay", expensive: false, readOnly: false, typeof(TimeSpan)),
				new StaticMetadata("DateCreated", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("FrequencyInterval", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("FrequencyRecurrenceFactor", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("FrequencyRelativeIntervals", expensive: false, readOnly: false, typeof(FrequencyRelativeIntervals)),
				new StaticMetadata("FrequencySubDayInterval", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("FrequencySubDayTypes", expensive: false, readOnly: false, typeof(FrequencySubDayTypes)),
				new StaticMetadata("FrequencyTypes", expensive: false, readOnly: false, typeof(FrequencyTypes)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("IsEnabled", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("JobCount", expensive: true, readOnly: true, typeof(int)),
				new StaticMetadata("ScheduleUid", expensive: false, readOnly: false, typeof(Guid))
			};
		}
	}

	private bool keepUnusedSchedule;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	[SfcParent("Job")]
	public SqlSmoObject Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime ActiveEndDate
	{
		get
		{
			return (DateTime)base.Properties.GetValueWithNullReplacement("ActiveEndDate");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ActiveEndDate", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public TimeSpan ActiveEndTimeOfDay
	{
		get
		{
			return (TimeSpan)base.Properties.GetValueWithNullReplacement("ActiveEndTimeOfDay");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ActiveEndTimeOfDay", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime ActiveStartDate
	{
		get
		{
			return (DateTime)base.Properties.GetValueWithNullReplacement("ActiveStartDate");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ActiveStartDate", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public TimeSpan ActiveStartTimeOfDay
	{
		get
		{
			return (TimeSpan)base.Properties.GetValueWithNullReplacement("ActiveStartTimeOfDay");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ActiveStartTimeOfDay", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime DateCreated => (DateTime)base.Properties.GetValueWithNullReplacement("DateCreated");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int FrequencyInterval
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("FrequencyInterval");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FrequencyInterval", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int FrequencyRecurrenceFactor
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("FrequencyRecurrenceFactor");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FrequencyRecurrenceFactor", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public FrequencyRelativeIntervals FrequencyRelativeIntervals
	{
		get
		{
			return (FrequencyRelativeIntervals)base.Properties.GetValueWithNullReplacement("FrequencyRelativeIntervals");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FrequencyRelativeIntervals", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int FrequencySubDayInterval
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("FrequencySubDayInterval");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FrequencySubDayInterval", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public FrequencySubDayTypes FrequencySubDayTypes
	{
		get
		{
			return (FrequencySubDayTypes)base.Properties.GetValueWithNullReplacement("FrequencySubDayTypes");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FrequencySubDayTypes", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public FrequencyTypes FrequencyTypes
	{
		get
		{
			return (FrequencyTypes)base.Properties.GetValueWithNullReplacement("FrequencyTypes");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FrequencyTypes", value);
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

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int JobCount => (int)base.Properties.GetValueWithNullReplacement("JobCount");

	public static string UrnSuffix => "Schedule";

	private bool IsShared => base.ParentColl.ParentInstance is JobServer;

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public Guid ScheduleUid
	{
		get
		{
			return (Guid)base.Properties.GetValueWithNullReplacement("ScheduleUid");
		}
		set
		{
			if (base.State != SqlSmoState.Creating)
			{
				throw new PropertyReadOnlyException("ScheduleUid");
			}
			base.Properties.SetValueWithConsistencyCheck("ScheduleUid", value);
		}
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	public JobSchedule()
	{
	}

	public JobSchedule(SqlSmoObject parent, string name)
	{
		ValidateName(name);
		key = new ScheduleObjectKey(name, JobScheduleCollectionBase.GetDefaultID());
		Parent = parent;
	}

	internal JobSchedule(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
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
		bool isShared = IsShared;
		base.ExecuteForScalar = true;
		int count;
		if (isShared)
		{
			SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal);
			if (sp.ForDirectExecution)
			{
				stringBuilder.Append("DECLARE @schedule_id int");
				stringBuilder.Append(Globals.newline);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_add_schedule @schedule_name=N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
			count = 1;
		}
		else if (sp.TargetServerVersionInternal > SqlServerVersionInternal.Version80)
		{
			if (sp.ForDirectExecution)
			{
				stringBuilder.Append("DECLARE @schedule_id int");
				stringBuilder.Append(Globals.newline);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC {0}msdb.dbo.sp_add_jobschedule {1}, @name=N'{2}'", new object[3]
			{
				Job.GetReturnCode(sp),
				((Job)base.ParentColl.ParentInstance).JobIdOrJobNameParameter(sp),
				SqlSmoObject.SqlString(Name)
			});
			count = 2;
		}
		else
		{
			if (sp.ForDirectExecution)
			{
				stringBuilder.Append("begin transaction");
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append("create table #tmp_sp_help_jobschedule1 (schedule_id int null, schedule_name nvarchar(128) null, enabled int null, freq_type int null, freq_interval int null, freq_subday_type int null, freq_subday_interval int null, freq_relative_interval int null, freq_recurrence_factor int null, active_start_date int null, active_end_date int null, active_start_time int null, active_end_time int null, date_created datetime null, schedule_description nvarchar(4000) null, next_run_date int null, next_run_time int null, job_id uniqueidentifier null)");
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append("DECLARE @schedule_id int");
				stringBuilder.Append(Globals.newline);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC {0}msdb.dbo.sp_add_jobschedule {1}, @name=N'{2}'", new object[3]
			{
				Job.GetReturnCode(sp),
				((Job)base.ParentColl.ParentInstance).JobIdOrJobNameParameter(sp),
				SqlSmoObject.SqlString(Name)
			});
			count = 2;
		}
		GetAllParams(stringBuilder, sp, ref count);
		if (base.ServerVersion.Major >= 9 && (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version100 || (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version90 && isShared)))
		{
			GetParameter(stringBuilder, sp, "ScheduleUid", "@schedule_uid=N'{0}'", ref count);
		}
		if (sp.ForDirectExecution)
		{
			if (sp.TargetServerVersionInternal > SqlServerVersionInternal.Version80)
			{
				stringBuilder.Append(", @schedule_id = @schedule_id OUTPUT");
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append("select @schedule_id");
			}
			else
			{
				stringBuilder.Append(Globals.newline);
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "insert into #tmp_sp_help_jobschedule1 (schedule_id, schedule_name, enabled, freq_type, freq_interval, freq_subday_type, freq_subday_interval, freq_relative_interval, freq_recurrence_factor, active_start_date, active_end_date, active_start_time, active_end_time, date_created, schedule_description, next_run_date, next_run_time) \texec msdb.dbo.sp_help_jobschedule  {0}", new object[1] { ((Job)base.ParentColl.ParentInstance).JobIdOrJobNameParameter(sp) });
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append("select max(schedule_id) from #tmp_sp_help_jobschedule1");
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append("drop table #tmp_sp_help_jobschedule1");
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append("commit transaction");
			}
		}
		queries.Add(stringBuilder.ToString());
	}

	private void GetAllParams(StringBuilder sb, ScriptingPreferences sp, ref int count)
	{
		GetBoolParameter(sb, sp, "IsEnabled", "@enabled={0}", ref count);
		GetEnumParameter(sb, sp, "FrequencyTypes", "@freq_type={0}", typeof(FrequencyTypes), ref count);
		GetParameter(sb, sp, "FrequencyInterval", "@freq_interval={0}", ref count);
		GetEnumParameter(sb, sp, "FrequencySubDayTypes", "@freq_subday_type={0}", typeof(FrequencySubDayTypes), ref count);
		GetParameter(sb, sp, "FrequencySubDayInterval", "@freq_subday_interval={0}", ref count);
		GetEnumParameter(sb, sp, "FrequencyRelativeIntervals", "@freq_relative_interval={0}", typeof(FrequencyRelativeIntervals), ref count);
		GetParameter(sb, sp, "FrequencyRecurrenceFactor", "@freq_recurrence_factor={0}", ref count);
		GetDateTimeParameterAsInt(sb, sp, "ActiveStartDate", "@active_start_date={0}", ref count);
		GetDateTimeParameterAsInt(sb, sp, "ActiveEndDate", "@active_end_date={0}", ref count);
		GetTimeSpanParameterAsInt(sb, sp, "ActiveStartTimeOfDay", "@active_start_time={0}", ref count);
		GetTimeSpanParameterAsInt(sb, sp, "ActiveEndTimeOfDay", "@active_end_time={0}", ref count);
	}

	protected override void PostCreate()
	{
		if (!ExecutionManager.Recording)
		{
			SetId((int)base.ScalarResult[1]);
		}
	}

	public void Drop(bool keepUnusedSchedule)
	{
		ThrowIfBelowVersion90();
		try
		{
			this.keepUnusedSchedule = keepUnusedSchedule;
			DropImpl();
		}
		finally
		{
			this.keepUnusedSchedule = false;
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
		if (IsShared)
		{
			SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal);
			if (keepUnusedSchedule)
			{
				return;
			}
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AGENT_SCHEDULE, new object[2] { "", base.ID });
				stringBuilder.Append(sp.NewLine);
			}
			if (!IsShared)
			{
				int num = ((!keepUnusedSchedule) ? 1 : 0);
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_detach_schedule {0}, @schedule_id={1}, @delete_unused_schedule={2}", new object[3]
				{
					((Job)base.ParentColl.ParentInstance).JobIdOrJobNameParameter(sp),
					base.ID,
					num
				});
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_schedule @schedule_id={0}", new object[1] { base.ID });
			}
		}
		else
		{
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendFormat(Scripts.INCLUDE_EXISTS_AGENT_JOBSCHEDULE, "", base.ID);
				stringBuilder.Append(sp.NewLine);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_jobschedule {0}, @name=N'{1}'", new object[2]
			{
				((Job)base.ParentColl.ParentInstance).JobIdOrJobNameParameter(sp),
				SqlSmoObject.SqlString(Name)
			});
		}
		queries.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		int count;
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_schedule @schedule_id={0}", new object[1] { base.ID });
			count = 1;
			GetAllParams(stringBuilder, sp, ref count);
			if (count > 1)
			{
				queries.Add(stringBuilder.ToString());
			}
			return;
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_jobschedule {0}, @name=N'{1}'", new object[2]
		{
			((Job)base.ParentColl.ParentInstance).JobIdOrJobNameParameter(),
			SqlSmoObject.SqlString(Name)
		});
		count = 2;
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
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_schedule @schedule_id={0}, @new_name=N'{1}'", new object[2]
			{
				base.ID,
				SqlSmoObject.SqlString(newName)
			}));
		}
		else
		{
			queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_jobschedule {0}, @name=N'{1}', @new_name=N'{2}'", new object[3]
			{
				((Job)base.ParentColl.ParentInstance).JobIdOrJobNameParameter(),
				SqlSmoObject.SqlString(Name),
				SqlSmoObject.SqlString(newName)
			}));
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

	public Guid[] EnumJobReferences()
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState(throwIfNotCreated: true);
			Request request;
			if (IsShared)
			{
				string text = string.Format(SmoApplication.DefaultCulture, "{0}/Job/Schedule[@ID={1}]", new object[2]
				{
					base.Urn.Parent,
					base.ID
				});
				request = new Request(text);
			}
			else
			{
				string text2 = string.Format(SmoApplication.DefaultCulture, "{0}/Job/Schedule[@ID={1}]", new object[2]
				{
					Parent.Urn.Parent,
					base.ID
				});
				request = new Request(text2);
			}
			request.Fields = new string[0];
			request.ParentPropertiesRequests = new PropertiesRequest[1];
			PropertiesRequest propertiesRequest = new PropertiesRequest();
			propertiesRequest.Fields = new string[1] { "JobID" };
			request.ParentPropertiesRequests[0] = propertiesRequest;
			DataTable enumeratorData = ExecutionManager.GetEnumeratorData(request);
			Guid[] array = new Guid[enumeratorData.Rows.Count];
			for (int i = 0; i < enumeratorData.Rows.Count; i++)
			{
				DataRow dataRow = enumeratorData.Rows[i];
				ref Guid reference = ref array[i];
				reference = (Guid)dataRow[0];
			}
			return array;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumReferences, this, ex);
		}
	}
}
