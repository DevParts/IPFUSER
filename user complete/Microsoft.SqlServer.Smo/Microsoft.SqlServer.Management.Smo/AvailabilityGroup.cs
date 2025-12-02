using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Diagnostics.STrace;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[PhysicalFacet]
[SfcElement(SfcElementFlags.Standalone)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public sealed class AvailabilityGroup : NamedSmoObject, IObjectPermission, ICreatable, IAlterable, IDroppable, IDropIfExists, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 8, 8, 12, 14, 14 };

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
				"AutomatedBackupPreference" => 0, 
				"FailureConditionLevel" => 1, 
				"HealthCheckTimeout" => 2, 
				"ID" => 3, 
				"LocalReplicaRole" => 4, 
				"PolicyHealthState" => 5, 
				"PrimaryReplicaServerName" => 6, 
				"UniqueId" => 7, 
				"BasicAvailabilityGroup" => 8, 
				"DatabaseHealthTrigger" => 9, 
				"DtcSupportEnabled" => 10, 
				"IsDistributedAvailabilityGroup" => 11, 
				"ClusterType" => 12, 
				"RequiredSynchronizedSecondariesToCommit" => 13, 
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
			staticMetadata = new StaticMetadata[14]
			{
				new StaticMetadata("AutomatedBackupPreference", expensive: false, readOnly: false, typeof(AvailabilityGroupAutomatedBackupPreference)),
				new StaticMetadata("FailureConditionLevel", expensive: false, readOnly: false, typeof(AvailabilityGroupFailureConditionLevel)),
				new StaticMetadata("HealthCheckTimeout", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("LocalReplicaRole", expensive: false, readOnly: true, typeof(AvailabilityReplicaRole)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
				new StaticMetadata("PrimaryReplicaServerName", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("UniqueId", expensive: false, readOnly: true, typeof(Guid)),
				new StaticMetadata("BasicAvailabilityGroup", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("DatabaseHealthTrigger", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("DtcSupportEnabled", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("IsDistributedAvailabilityGroup", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("ClusterType", expensive: false, readOnly: false, typeof(AvailabilityGroupClusterType)),
				new StaticMetadata("RequiredSynchronizedSecondariesToCommit", expensive: false, readOnly: false, typeof(int))
			};
		}
	}

	internal const string AutomatedBackupPreferencePropertyName = "AutomatedBackupPreference";

	internal const string BasicAvailabilityGroupPropertyName = "BasicAvailabilityGroup";

	internal const string DatabaseHealthTriggerPropertyName = "DatabaseHealthTrigger";

	internal const string DtcSupportEnabledPropertyName = "DtcSupportEnabled";

	internal const string FailureConditionLevelPropertyName = "FailureConditionLevel";

	internal const string HealthCheckTimeoutPropertyName = "HealthCheckTimeout";

	internal const string ClusterTypePropertyName = "ClusterType";

	internal const string RequiredSynchronizedSecondariesToCommitPropertyName = "RequiredSynchronizedSecondariesToCommit";

	private AvailabilityReplicaCollection m_AvailabilityReplicas;

	private AvailabilityDatabaseCollection m_AvailabilityDatabases;

	private DatabaseReplicaStateCollection m_DatabaseReplicaStates;

	private AvailabilityGroupListenerCollection m_AvailabilityGroupListeners;

	internal static readonly string ForceFailoverAllowDataLossScript = "FORCE_FAILOVER_ALLOW_DATA_LOSS";

	internal static readonly string FailoverScript = "FAILOVER";

	internal static readonly string AvailabilityGroupScript = "AVAILABILITY GROUP";

	internal static readonly string DatabaseScript = "DATABASE";

	internal static readonly string ReplicaOn = "REPLICA ON";

	internal static readonly string AutomatedBackupPreferenceScript = "AUTOMATED_BACKUP_PREFERENCE";

	internal static readonly string BasicAvailabilityGroupScript = "BASIC";

	internal static readonly string DatabaseHealthTriggerScript = "DB_FAILOVER";

	internal static readonly string DtcSupportEnabledScript = "DTC_SUPPORT";

	internal static readonly string DtcSupportEnabledOnScript = "PER_DB";

	internal static readonly string DtcSupportEnabledOffScript = "NONE";

	internal static readonly string FailureConditionLevelScript = "FAILURE_CONDITION_LEVEL";

	internal static readonly string HealthCheckTimeoutScript = "HEALTH_CHECK_TIMEOUT";

	internal static readonly string ListenerScript = "LISTENER";

	internal static readonly string PortScript = "PORT";

	internal static readonly string ClusterTypeScript = "CLUSTER_TYPE";

	internal static readonly string RequiredSynchronizedSecondariesToCommitScript = "REQUIRED_SYNCHRONIZED_SECONDARIES_TO_COMMIT";

	internal static readonly string RoleScript = "ROLE";

	internal static readonly string PrimaryScript = "PRIMARY";

	internal static readonly string SecondaryOnlyScript = "SECONDARY_ONLY";

	internal static readonly string SecondaryScript = "SECONDARY";

	internal static readonly string NoneScript = "NONE";

	internal static readonly string WsfcScript = "WSFC";

	internal static readonly string ExternalScript = "EXTERNAL";

	internal static readonly Dictionary<SqlServerVersionInternal, string[]> CreatableGroupPropertyNames = new Dictionary<SqlServerVersionInternal, string[]>
	{
		{
			SqlServerVersionInternal.Version120,
			new string[3] { "AutomatedBackupPreference", "FailureConditionLevel", "HealthCheckTimeout" }
		},
		{
			SqlServerVersionInternal.Version130,
			new string[3] { "BasicAvailabilityGroup", "DatabaseHealthTrigger", "DtcSupportEnabled" }
		},
		{
			SqlServerVersionInternal.Version140,
			new string[2] { "ClusterType", "RequiredSynchronizedSecondariesToCommit" }
		}
	};

	internal static readonly string[] BasicAlterableGroupPropertyNames = new string[4] { "DatabaseHealthTrigger", "FailureConditionLevel", "HealthCheckTimeout", "DtcSupportEnabled" };

	internal static readonly Dictionary<SqlServerVersionInternal, string[]> AlterableGroupPropertyNames = new Dictionary<SqlServerVersionInternal, string[]>
	{
		{
			SqlServerVersionInternal.Version120,
			new string[3] { "AutomatedBackupPreference", "FailureConditionLevel", "HealthCheckTimeout" }
		},
		{
			SqlServerVersionInternal.Version130,
			new string[2] { "DatabaseHealthTrigger", "DtcSupportEnabled" }
		},
		{
			SqlServerVersionInternal.Version140,
			new string[1] { "RequiredSynchronizedSecondariesToCommit" }
		}
	};

	private static readonly TraceContext tc = TraceContext.GetTraceContext(SmoApplication.ModuleName, "AvailabilityGroup");

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Server Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as Server;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	internal override UserPermissionCollection Permissions => GetUserPermissions();

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityGroupAutomatedBackupPreference AutomatedBackupPreference
	{
		get
		{
			return (AvailabilityGroupAutomatedBackupPreference)base.Properties.GetValueWithNullReplacement("AutomatedBackupPreference");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AutomatedBackupPreference", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone, "false")]
	public bool BasicAvailabilityGroup
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("BasicAvailabilityGroup");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BasicAvailabilityGroup", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityGroupClusterType ClusterType
	{
		get
		{
			return (AvailabilityGroupClusterType)base.Properties.GetValueWithNullReplacement("ClusterType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ClusterType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone, "false")]
	public bool DatabaseHealthTrigger
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("DatabaseHealthTrigger");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DatabaseHealthTrigger", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone, "false")]
	public bool DtcSupportEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("DtcSupportEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DtcSupportEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityGroupFailureConditionLevel FailureConditionLevel
	{
		get
		{
			return (AvailabilityGroupFailureConditionLevel)base.Properties.GetValueWithNullReplacement("FailureConditionLevel");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FailureConditionLevel", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int HealthCheckTimeout
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("HealthCheckTimeout");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("HealthCheckTimeout", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone, "false")]
	public bool IsDistributedAvailabilityGroup
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsDistributedAvailabilityGroup");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsDistributedAvailabilityGroup", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityReplicaRole LocalReplicaRole => (AvailabilityReplicaRole)base.Properties.GetValueWithNullReplacement("LocalReplicaRole");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string PrimaryReplicaServerName => (string)base.Properties.GetValueWithNullReplacement("PrimaryReplicaServerName");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int RequiredSynchronizedSecondariesToCommit
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("RequiredSynchronizedSecondariesToCommit");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RequiredSynchronizedSecondariesToCommit", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public Guid UniqueId => (Guid)base.Properties.GetValueWithNullReplacement("UniqueId");

	public static string UrnSuffix => "AvailabilityGroup";

	public AvailabilityGroupClusterType ClusterTypeWithDefault
	{
		get
		{
			if (!IsSupportedProperty("ClusterType"))
			{
				return AvailabilityGroupClusterType.Wsfc;
			}
			return ClusterType;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.OneToAny, typeof(AvailabilityReplica))]
	public AvailabilityReplicaCollection AvailabilityReplicas
	{
		get
		{
			if (m_AvailabilityReplicas == null)
			{
				m_AvailabilityReplicas = new AvailabilityReplicaCollection(this, GetComparerFromCollation("Latin1_General_CI_AS"));
			}
			return m_AvailabilityReplicas;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(AvailabilityDatabase))]
	public AvailabilityDatabaseCollection AvailabilityDatabases
	{
		get
		{
			if (m_AvailabilityDatabases == null)
			{
				m_AvailabilityDatabases = new AvailabilityDatabaseCollection(this);
			}
			return m_AvailabilityDatabases;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(DatabaseReplicaState))]
	public DatabaseReplicaStateCollection DatabaseReplicaStates
	{
		get
		{
			if (m_DatabaseReplicaStates == null)
			{
				m_DatabaseReplicaStates = new DatabaseReplicaStateCollection(this);
			}
			return m_DatabaseReplicaStates;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(AvailabilityGroupListener))]
	public AvailabilityGroupListenerCollection AvailabilityGroupListeners
	{
		get
		{
			if (m_AvailabilityGroupListeners == null)
			{
				m_AvailabilityGroupListeners = new AvailabilityGroupListenerCollection(this);
			}
			return m_AvailabilityGroupListeners;
		}
	}

	public AvailabilityGroup()
	{
	}

	public AvailabilityGroup(Server server, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = server;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		return propname switch
		{
			"BasicAvailabilityGroup" => false, 
			"DatabaseHealthTrigger" => false, 
			"DtcSupportEnabled" => false, 
			"IsDistributedAvailabilityGroup" => false, 
			_ => base.GetPropertyDefaultValue(propname), 
		};
	}

	public void Deny(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string[] granteeNames, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, null, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, revokeGrant, cascade, asRole);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, revokeGrant, cascade, asRole);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions()
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, null, null);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, granteeName, null);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, null, permissions);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName, ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, granteeName, permissions);
	}

	internal AvailabilityGroup(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Failover()
	{
		CheckObjectState(!ExecutionManager.Recording);
		string script = Scripts.ALTER + Globals.space + AvailabilityGroupScript + Globals.space + SqlSmoObject.MakeSqlBraket(Name) + Globals.space + FailoverScript + Globals.statementTerminator;
		DoCustomAction(script, ExceptionTemplatesImpl.ManualFailoverFailed(Parent.Name, Name));
	}

	public void FailoverWithPotentialDataLoss()
	{
		CheckObjectState(!ExecutionManager.Recording);
		string script = Scripts.ALTER + Globals.space + AvailabilityGroupScript + Globals.space + SqlSmoObject.MakeSqlBraket(Name) + Globals.space + ForceFailoverAllowDataLossScript + Globals.statementTerminator;
		DoCustomAction(script, ExceptionTemplatesImpl.ForceFailoverFailed(Parent.Name, Name));
	}

	public void DemoteAsSecondary()
	{
		ThrowIfBelowVersion130();
		CheckObjectState(!ExecutionManager.Recording);
		string script = Scripts.ALTER + Globals.space + AvailabilityGroupScript + Globals.space + SqlSmoObject.MakeSqlBraket(Name) + Globals.space + Scripts.SET + Globals.space + Globals.LParen + RoleScript + Globals.space + Globals.EqualSign + Globals.space + SecondaryScript + Globals.RParen + Globals.statementTerminator;
		DoCustomAction(script, ExceptionTemplatesImpl.ForceFailoverFailed(Parent.Name, Name));
	}

	public void Create()
	{
		CreateImpl();
	}

	public void Alter()
	{
		AlterImpl();
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

	public DataTable EnumReplicaClusterNodes()
	{
		try
		{
			Request req = new Request(base.Urn.Value + "/ReplicaClusterNode");
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumReplicaClusterNodes(Name), this, ex);
		}
	}

	internal override void ScriptCreate(StringCollection query, ScriptingPreferences sp)
	{
		tc.Assert(null != query, "String collection should not be null");
		SqlSmoObject.ThrowIfBelowVersion110(sp.TargetServerVersionInternal);
		SqlSmoObject.ThrowIfCloud(sp.TargetDatabaseEngineType, ExceptionTemplatesImpl.UnsupportedEngineTypeException);
		if (AvailabilityReplicas == null || AvailabilityReplicas.Count < 1)
		{
			throw new SmoException(ExceptionTemplatesImpl.ObjectWithNoChildren(UrnSuffix, AvailabilityReplica.UrnSuffix));
		}
		if (AvailabilityReplicas[Parent.ConnectionContext.TrueName] == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.CannotCreateAvailabilityGroupWithoutCurrentIntance(Parent.Name, Name));
		}
		if (AvailabilityGroupListeners != null && AvailabilityGroupListeners.Count > 1)
		{
			throw new SmoException(ExceptionTemplatesImpl.ObjectWithMoreChildren(UrnSuffix, AvailabilityGroupListener.UrnSuffix));
		}
		if (AvailabilityGroupListeners != null && AvailabilityGroupListeners.Count == 1)
		{
			AvailabilityGroupListeners[0].ValidateIPAddresses();
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AVAILABILITY_GROUP, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append(Scripts.CREATE + Globals.space + AvailabilityGroupScript + Globals.space);
		stringBuilder.Append(SqlSmoObject.MakeSqlBraket(Name) + Globals.newline);
		string text = ScriptCreateGroupOptions(sp.TargetServerVersionInternal);
		if (!string.IsNullOrEmpty(text))
		{
			stringBuilder.Append(Globals.With + Globals.space + Globals.LParen + text + Globals.RParen + Globals.newline);
		}
		stringBuilder.Append(Globals.For + Globals.space);
		bool flag = true;
		foreach (AvailabilityDatabase availabilityDatabase in AvailabilityDatabases)
		{
			if (flag)
			{
				stringBuilder.Append(DatabaseScript + Globals.space);
				flag = false;
			}
			else
			{
				stringBuilder.Append(Globals.comma + Globals.space);
			}
			stringBuilder.Append(SqlSmoObject.MakeSqlBraket(availabilityDatabase.Name));
		}
		stringBuilder.Append(Globals.newline);
		bool flag2 = true;
		stringBuilder.Append(ReplicaOn + Globals.space);
		foreach (AvailabilityReplica availabilityReplica in AvailabilityReplicas)
		{
			if (flag2)
			{
				flag2 = false;
			}
			else
			{
				stringBuilder.Append(Globals.comma + Globals.newline + Globals.tab);
			}
			stringBuilder.Append(SqlSmoObject.MakeSqlString(availabilityReplica.Name) + Globals.space + Globals.With + Globals.space + Globals.LParen);
			stringBuilder.Append(availabilityReplica.ScriptReplicaOptions(sp));
			stringBuilder.Append(Globals.RParen);
		}
		if (AvailabilityGroupListeners != null && AvailabilityGroupListeners.Count == 1)
		{
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append(ListenerScript + Globals.space);
			stringBuilder.Append(SqlSmoObject.MakeSqlString(AvailabilityGroupListeners[0].Name) + Globals.space + Globals.LParen);
			stringBuilder.Append(AvailabilityGroupListeners[0].ScriptListenerOptions());
			stringBuilder.Append(Globals.RParen);
		}
		stringBuilder.Append(Globals.statementTerminator);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		string text2 = stringBuilder.ToString();
		query.Add(text2);
		tc.TraceInformation("Generated create script: " + text2);
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		tc.Assert(null != alterQuery, "String collection should not be null");
		SqlSmoObject.ThrowIfBelowVersion110(sp.TargetServerVersionInternal);
		SqlSmoObject.ThrowIfCloud(sp.TargetDatabaseEngineType, ExceptionTemplatesImpl.UnsupportedEngineTypeException);
		IEnumerable<string> enumerable = ((!IsSupportedProperty("BasicAvailabilityGroup") || !BasicAvailabilityGroup) ? AlterableGroupPropertyNames.Where((KeyValuePair<SqlServerVersionInternal, string[]> kvp) => kvp.Key <= sp.TargetServerVersionInternal).SelectMany((KeyValuePair<SqlServerVersionInternal, string[]> kvp) => kvp.Value) : BasicAlterableGroupPropertyNames);
		foreach (string item in enumerable)
		{
			string value = ScriptAlterOneOption(item, sp);
			if (!string.IsNullOrEmpty(value))
			{
				alterQuery.Add(value);
			}
		}
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		tc.Assert(null != dropQuery, "String collection for the drop query should not be null");
		SqlSmoObject.ThrowIfBelowVersion110(sp.TargetServerVersionInternal);
		SqlSmoObject.ThrowIfCloud(sp.TargetDatabaseEngineType, ExceptionTemplatesImpl.UnsupportedEngineTypeException);
		StringBuilder stringBuilder = new StringBuilder();
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AVAILABILITY_GROUP, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append(Scripts.DROP + Globals.space + AvailabilityGroupScript + Globals.space + SqlSmoObject.MakeSqlBraket(Name) + Globals.statementTerminator);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		string text = stringBuilder.ToString();
		dropQuery.Add(text);
		tc.TraceInformation("Generated drop script: " + text);
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		if (action == PropagateAction.Create)
		{
			return new PropagateInfo[3]
			{
				new PropagateInfo(AvailabilityReplicas, bWithScript: false),
				new PropagateInfo(AvailabilityDatabases, bWithScript: false),
				new PropagateInfo(AvailabilityGroupListeners, bWithScript: false)
			};
		}
		return null;
	}

	protected override void PostCreate()
	{
		base.PostCreate();
		if (AvailabilityGroupListeners == null)
		{
			return;
		}
		foreach (AvailabilityGroupListener availabilityGroupListener in AvailabilityGroupListeners)
		{
			availabilityGroupListener.FetchIpAddressKeysPostCreate();
		}
	}

	internal static string GetAvailabilityGroupClusterType(AvailabilityGroupClusterType availabilityGroupClusterType)
	{
		return availabilityGroupClusterType switch
		{
			AvailabilityGroupClusterType.Wsfc => WsfcScript, 
			AvailabilityGroupClusterType.None => NoneScript, 
			AvailabilityGroupClusterType.External => ExternalScript, 
			_ => throw new ArgumentException(ExceptionTemplatesImpl.UnknownEnumerationWithValue("AvailabilityGroupClusterType", availabilityGroupClusterType)), 
		};
	}

	private bool IsDirty(string property)
	{
		return base.Properties.IsDirty(base.Properties.LookupID(property, PropertyAccessPurpose.Read));
	}

	private AvailabilityGroupAutomatedBackupPreference GetEffectiveAutomatedBackupPreference(SqlServerVersionInternal targetVersion)
	{
		if (targetVersion >= SqlServerVersionInternal.Version130)
		{
			Property propertyOptional = GetPropertyOptional("BasicAvailabilityGroup");
			if (!propertyOptional.IsNull && propertyOptional.Value.Equals(true))
			{
				return AvailabilityGroupAutomatedBackupPreference.Primary;
			}
		}
		return AutomatedBackupPreference;
	}

	private string GetAutomatedBackupPreferenceScript(AvailabilityGroupAutomatedBackupPreference preference)
	{
		return preference switch
		{
			AvailabilityGroupAutomatedBackupPreference.Primary => PrimaryScript, 
			AvailabilityGroupAutomatedBackupPreference.SecondaryOnly => SecondaryOnlyScript, 
			AvailabilityGroupAutomatedBackupPreference.Secondary => SecondaryScript, 
			AvailabilityGroupAutomatedBackupPreference.None => NoneScript, 
			_ => throw new ArgumentException(ExceptionTemplatesImpl.UnknownEnumeration("AvailabilityGroupAutomatedBackupPreference")), 
		};
	}

	private string ScriptGroupOption(bool scriptAll, string propertyName, SqlServerVersionInternal targetServerVersion)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		Property propertyOptional = GetPropertyOptional(propertyName);
		if (!propertyOptional.IsNull && (scriptAll || IsDirty(propertyName)))
		{
			switch (propertyName)
			{
			case "AutomatedBackupPreference":
				stringBuilder.Append(AutomatedBackupPreferenceScript + Globals.space + Globals.EqualSign + Globals.space);
				stringBuilder.Append(GetAutomatedBackupPreferenceScript(GetEffectiveAutomatedBackupPreference(targetServerVersion)));
				break;
			case "BasicAvailabilityGroup":
				if (BasicAvailabilityGroup)
				{
					stringBuilder.Append(BasicAvailabilityGroupScript);
				}
				break;
			case "DatabaseHealthTrigger":
				stringBuilder.Append(DatabaseHealthTriggerScript + Globals.space + Globals.EqualSign + Globals.space);
				stringBuilder.Append(DatabaseHealthTrigger ? "ON" : "OFF");
				break;
			case "DtcSupportEnabled":
				stringBuilder.Append(DtcSupportEnabledScript + Globals.space + Globals.EqualSign + Globals.space);
				stringBuilder.Append(DtcSupportEnabled ? DtcSupportEnabledOnScript : DtcSupportEnabledOffScript);
				break;
			case "FailureConditionLevel":
				stringBuilder.Append(FailureConditionLevelScript + Globals.space + Globals.EqualSign + Globals.space);
				stringBuilder.Append((int)FailureConditionLevel);
				break;
			case "HealthCheckTimeout":
				stringBuilder.Append(HealthCheckTimeoutScript + Globals.space + Globals.EqualSign + Globals.space);
				stringBuilder.Append(HealthCheckTimeout);
				break;
			case "ClusterType":
				if (ClusterType != AvailabilityGroupClusterType.Wsfc)
				{
					stringBuilder.Append(ClusterTypeScript + Globals.space + Globals.EqualSign + Globals.space);
					stringBuilder.Append(GetAvailabilityGroupClusterType(ClusterType));
				}
				break;
			case "RequiredSynchronizedSecondariesToCommit":
				stringBuilder.Append(RequiredSynchronizedSecondariesToCommitScript + Globals.space + Globals.EqualSign + Globals.space);
				stringBuilder.Append(RequiredSynchronizedSecondariesToCommit);
				break;
			}
		}
		return stringBuilder.ToString();
	}

	private string ScriptCreateGroupOptions(SqlServerVersionInternal targetVersion)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		bool flag = true;
		foreach (string item in CreatableGroupPropertyNames.Where((KeyValuePair<SqlServerVersionInternal, string[]> kvp) => kvp.Key <= targetVersion).SelectMany((KeyValuePair<SqlServerVersionInternal, string[]> kvp) => kvp.Value))
		{
			string value = ScriptGroupOption(scriptAll: true, item, targetVersion);
			if (!string.IsNullOrEmpty(value))
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(Globals.comma + Globals.newline);
				}
				stringBuilder.Append(value);
			}
		}
		return stringBuilder.ToString();
	}

	private string ScriptAlterOneOption(string propertyName, ScriptingPreferences sp)
	{
		string value = ScriptGroupOption(scriptAll: false, propertyName, sp.TargetServerVersionInternal);
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AVAILABILITY_GROUP, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append(Scripts.ALTER + Globals.space + AvailabilityGroupScript + Globals.space);
		stringBuilder.Append(SqlSmoObject.MakeSqlBraket(Name) + Globals.space + Scripts.SET + Globals.LParen + Globals.newline);
		stringBuilder.Append(value);
		stringBuilder.Append(Globals.newline + Globals.RParen);
		stringBuilder.Append(Globals.statementTerminator);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		return stringBuilder.ToString();
	}
}
