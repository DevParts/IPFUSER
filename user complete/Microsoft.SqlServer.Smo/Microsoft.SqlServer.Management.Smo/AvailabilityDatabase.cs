using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using Microsoft.SqlServer.Diagnostics.STrace;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[EvaluationMode(/*Could not decode attribute arguments.*/)]
[PhysicalFacet]
[SfcElement(SfcElementFlags.Standalone)]
public sealed class AvailabilityDatabase : NamedSmoObject, ICreatable, IDroppable, IDropIfExists, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 9, 9, 9, 9, 9 };

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
				"IsFailoverReady" => 0, 
				"IsJoined" => 1, 
				"IsPendingSecondarySuspend" => 2, 
				"IsSuspended" => 3, 
				"PolicyHealthState" => 4, 
				"RecoveryLSN" => 5, 
				"SynchronizationState" => 6, 
				"TruncationLSN" => 7, 
				"UniqueId" => 8, 
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
			staticMetadata = new StaticMetadata[9]
			{
				new StaticMetadata("IsFailoverReady", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("IsJoined", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("IsPendingSecondarySuspend", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("IsSuspended", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
				new StaticMetadata("RecoveryLSN", expensive: false, readOnly: true, typeof(decimal)),
				new StaticMetadata("SynchronizationState", expensive: false, readOnly: true, typeof(AvailabilityDatabaseSynchronizationState)),
				new StaticMetadata("TruncationLSN", expensive: false, readOnly: true, typeof(decimal)),
				new StaticMetadata("UniqueId", expensive: false, readOnly: true, typeof(Guid))
			};
		}
	}

	private static readonly TraceContext tc = TraceContext.GetTraceContext(SmoApplication.ModuleName, "AvailabilityDatabase");

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public AvailabilityGroup Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as AvailabilityGroup;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsFailoverReady => (bool)base.Properties.GetValueWithNullReplacement("IsFailoverReady");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsJoined => (bool)base.Properties.GetValueWithNullReplacement("IsJoined");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsPendingSecondarySuspend => (bool)base.Properties.GetValueWithNullReplacement("IsPendingSecondarySuspend");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsSuspended => (bool)base.Properties.GetValueWithNullReplacement("IsSuspended");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public decimal RecoveryLSN => (decimal)base.Properties.GetValueWithNullReplacement("RecoveryLSN");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityDatabaseSynchronizationState SynchronizationState => (AvailabilityDatabaseSynchronizationState)base.Properties.GetValueWithNullReplacement("SynchronizationState");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public decimal TruncationLSN => (decimal)base.Properties.GetValueWithNullReplacement("TruncationLSN");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public Guid UniqueId => (Guid)base.Properties.GetValueWithNullReplacement("UniqueId");

	public static string UrnSuffix => "AvailabilityDatabase";

	public AvailabilityDatabase()
	{
	}

	public AvailabilityDatabase(AvailabilityGroup availabilityGroup, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = availabilityGroup;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal AvailabilityDatabase(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		CreateImpl();
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public void JoinAvailablityGroup()
	{
		CheckObjectState(!ExecutionManager.Recording);
		string name = Parent.Name;
		string format = "\r\n-- Wait for the replica to start communicating\r\nbegin try\r\ndeclare @conn bit\r\ndeclare @count int\r\ndeclare @replica_id uniqueidentifier \r\ndeclare @group_id uniqueidentifier\r\nset @conn = 0\r\nset @count = 30 -- wait for 5 minutes \r\n\r\nif (serverproperty('IsHadrEnabled') = 1)\r\n\tand (isnull((select member_state from master.sys.dm_hadr_cluster_members where upper(member_name COLLATE Latin1_General_CI_AS) = upper(cast(serverproperty('ComputerNamePhysicalNetBIOS') as nvarchar(256)) COLLATE Latin1_General_CI_AS)), 0) <> 0)\r\n\tand (isnull((select state from master.sys.database_mirroring_endpoints), 1) = 0)\r\nbegin\r\n    select @group_id = ags.group_id from master.sys.availability_groups as ags where name = N'{0}'\r\n\tselect @replica_id = replicas.replica_id from master.sys.availability_replicas as replicas where upper(replicas.replica_server_name COLLATE Latin1_General_CI_AS) = upper(@@SERVERNAME COLLATE Latin1_General_CI_AS) and group_id = @group_id\r\n\twhile @conn <> 1 and @count > 0\r\n\tbegin\r\n\t\tset @conn = isnull((select connected_state from master.sys.dm_hadr_availability_replica_states as states where states.replica_id = @replica_id), 1)\r\n\t\tif @conn = 1\r\n\t\tbegin\r\n\t\t\t-- exit loop when the replica is connected, or if the query cannot find the replica status\r\n\t\t\tbreak\r\n\t\tend\r\n\t\twaitfor delay '00:00:10'\r\n\t\tset @count = @count - 1\r\n\tend\r\nend\r\nend try\r\nbegin catch\r\n\t-- If the wait loop fails, do not stop execution of the alter database statement\r\nend catch\r\n";
		format = string.Format(CultureInfo.InvariantCulture, format, new object[1] { SqlSmoObject.EscapeString(name, '\'') });
		string script = format + Scripts.ALTER + Globals.space + AvailabilityGroup.DatabaseScript + Globals.space + SqlSmoObject.MakeSqlBraket(Name) + Globals.space + Scripts.SET + Globals.space + Scripts.HADR + Globals.space + AvailabilityGroup.AvailabilityGroupScript + Globals.space + Globals.EqualSign + Globals.space + SqlSmoObject.MakeSqlBraket(name) + Globals.statementTerminator;
		string toplevelExceptionMessage = ExceptionTemplatesImpl.DatabaseJoinAvailabilityGroupFailed(Parent.Parent.Name, name, Name);
		DoCustomAction(script, toplevelExceptionMessage);
	}

	public void LeaveAvailabilityGroup()
	{
		CheckObjectState(!ExecutionManager.Recording);
		string script = Scripts.ALTER + Globals.space + AvailabilityGroup.DatabaseScript + Globals.space + SqlSmoObject.MakeSqlBraket(Name) + Globals.space + Scripts.SET + Globals.space + Scripts.HADR + Globals.space + Globals.Off + Globals.statementTerminator;
		string toplevelExceptionMessage = ExceptionTemplatesImpl.DatabaseLeaveAvailabilityGroupFailed(Parent.Parent.Name, Parent.Name, Name);
		DoCustomAction(script, toplevelExceptionMessage);
	}

	public void SuspendDataMovement()
	{
		CheckObjectState(!ExecutionManager.Recording);
		string script = Scripts.ALTER + Globals.space + AvailabilityGroup.DatabaseScript + Globals.space + SqlSmoObject.MakeSqlBraket(Name) + Globals.space + Scripts.SET + Globals.space + Scripts.HADR + Globals.space + Scripts.SUSPEND + Globals.statementTerminator;
		string toplevelExceptionMessage = ExceptionTemplatesImpl.SuspendDataMovementFailed(Parent.Parent.Name, Parent.Name, Name);
		DoCustomAction(script, toplevelExceptionMessage);
	}

	public void ResumeDataMovement()
	{
		CheckObjectState(!ExecutionManager.Recording);
		string script = Scripts.ALTER + Globals.space + AvailabilityGroup.DatabaseScript + Globals.space + SqlSmoObject.MakeSqlBraket(Name) + Globals.space + Scripts.SET + Globals.space + Scripts.HADR + Globals.space + Scripts.RESUME + Globals.statementTerminator;
		string toplevelExceptionMessage = ExceptionTemplatesImpl.ResumeDataMovementFailed(Parent.Parent.Name, Parent.Name, Name);
		DoCustomAction(script, toplevelExceptionMessage);
	}

	internal override void ScriptCreate(StringCollection query, ScriptingPreferences sp)
	{
		tc.Assert(null != query, "String collection should not be null");
		SqlSmoObject.ThrowIfBelowVersion110(sp.TargetServerVersionInternal);
		SqlSmoObject.ThrowIfCloud(sp.TargetDatabaseEngineType, ExceptionTemplatesImpl.UnsupportedEngineTypeException);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			string text = FormatFullNameForScripting(sp, nameIsIndentifier: false);
			string text2 = Parent.FormatFullNameForScripting(sp, nameIsIndentifier: false);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AVAILABILITY_DATABASE, new object[3] { "NOT", text, text2 });
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append(Scripts.ALTER + Globals.space + AvailabilityGroup.AvailabilityGroupScript + Globals.space);
		stringBuilder.Append(SqlSmoObject.MakeSqlBraket(Parent.Name) + Globals.newline);
		stringBuilder.Append("ADD" + Globals.space + "DATABASE" + Globals.space);
		stringBuilder.Append(SqlSmoObject.MakeSqlBraket(Name));
		stringBuilder.Append(Globals.statementTerminator);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		query.Add(stringBuilder.ToString());
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		tc.Assert(null != dropQuery, "String collection should not be null");
		SqlSmoObject.ThrowIfBelowVersion110(sp.TargetServerVersionInternal);
		SqlSmoObject.ThrowIfCloud(sp.TargetDatabaseEngineType, ExceptionTemplatesImpl.UnsupportedEngineTypeException);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			string text = FormatFullNameForScripting(sp, nameIsIndentifier: false);
			string text2 = Parent.FormatFullNameForScripting(sp, nameIsIndentifier: false);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AVAILABILITY_DATABASE, new object[3] { "", text, text2 });
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append(Scripts.ALTER + Globals.space + AvailabilityGroup.AvailabilityGroupScript + Globals.space);
		stringBuilder.Append(SqlSmoObject.MakeSqlBraket(Parent.Name) + Globals.newline);
		stringBuilder.Append("REMOVE" + Globals.space + "DATABASE" + Globals.space);
		stringBuilder.Append(SqlSmoObject.MakeSqlBraket(Name));
		stringBuilder.Append(Globals.statementTerminator);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		dropQuery.Add(stringBuilder.ToString());
	}
}
