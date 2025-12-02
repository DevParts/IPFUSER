using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Diagnostics.STrace;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone)]
[PhysicalFacet]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public sealed class AvailabilityReplica : NamedSmoObject, ICreatable, IDroppable, IDropIfExists, IAlterable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 25, 25, 26, 26, 26 };

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
				"AvailabilityMode" => 0, 
				"BackupPriority" => 1, 
				"ConnectionModeInPrimaryRole" => 2, 
				"ConnectionModeInSecondaryRole" => 3, 
				"ConnectionState" => 4, 
				"CreateDate" => 5, 
				"DateLastModified" => 6, 
				"EndpointUrl" => 7, 
				"FailoverMode" => 8, 
				"JoinState" => 9, 
				"LastConnectErrorDescription" => 10, 
				"LastConnectErrorNumber" => 11, 
				"LastConnectErrorTimestamp" => 12, 
				"MemberState" => 13, 
				"OperationalState" => 14, 
				"Owner" => 15, 
				"PolicyHealthState" => 16, 
				"QuorumVoteCount" => 17, 
				"ReadonlyRoutingConnectionUrl" => 18, 
				"ReadonlyRoutingListDelimited" => 19, 
				"Role" => 20, 
				"RollupRecoveryState" => 21, 
				"RollupSynchronizationState" => 22, 
				"SessionTimeout" => 23, 
				"UniqueId" => 24, 
				"SeedingMode" => 25, 
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
			staticMetadata = new StaticMetadata[26]
			{
				new StaticMetadata("AvailabilityMode", expensive: false, readOnly: false, typeof(AvailabilityReplicaAvailabilityMode)),
				new StaticMetadata("BackupPriority", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("ConnectionModeInPrimaryRole", expensive: false, readOnly: false, typeof(AvailabilityReplicaConnectionModeInPrimaryRole)),
				new StaticMetadata("ConnectionModeInSecondaryRole", expensive: false, readOnly: false, typeof(AvailabilityReplicaConnectionModeInSecondaryRole)),
				new StaticMetadata("ConnectionState", expensive: false, readOnly: true, typeof(AvailabilityReplicaConnectionState)),
				new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("EndpointUrl", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("FailoverMode", expensive: false, readOnly: false, typeof(AvailabilityReplicaFailoverMode)),
				new StaticMetadata("JoinState", expensive: false, readOnly: true, typeof(AvailabilityReplicaJoinState)),
				new StaticMetadata("LastConnectErrorDescription", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("LastConnectErrorNumber", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("LastConnectErrorTimestamp", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("MemberState", expensive: true, readOnly: true, typeof(ClusterMemberState)),
				new StaticMetadata("OperationalState", expensive: false, readOnly: true, typeof(AvailabilityReplicaOperationalState)),
				new StaticMetadata("Owner", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
				new StaticMetadata("QuorumVoteCount", expensive: true, readOnly: true, typeof(int)),
				new StaticMetadata("ReadonlyRoutingConnectionUrl", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ReadonlyRoutingListDelimited", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("Role", expensive: false, readOnly: true, typeof(AvailabilityReplicaRole)),
				new StaticMetadata("RollupRecoveryState", expensive: false, readOnly: true, typeof(AvailabilityReplicaRollupRecoveryState)),
				new StaticMetadata("RollupSynchronizationState", expensive: false, readOnly: true, typeof(AvailabilityReplicaRollupSynchronizationState)),
				new StaticMetadata("SessionTimeout", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("UniqueId", expensive: false, readOnly: true, typeof(Guid)),
				new StaticMetadata("SeedingMode", expensive: false, readOnly: false, typeof(AvailabilityReplicaSeedingMode))
			};
		}
	}

	[Flags]
	private enum PropertyType
	{
		Alterable = 1,
		PrimaryRoleProperty = 2,
		SecondaryRoleProperty = 4
	}

	private const char ReadOnlyRoutingListReplicaNameSeparator = ',';

	private const char ReadOnlyRoutingLoadBalancingGroupStartCharacter = '(';

	private const char ReadOnlyRoutingLoadBalancingGroupEndCharacter = ')';

	internal const string EndPointUrlPropertyName = "EndpointUrl";

	internal const string AvailabilityModePropertyName = "AvailabilityMode";

	internal const string FailoverModePropertyName = "FailoverMode";

	internal const string ConnectionModeInPrimaryRolePropertyName = "ConnectionModeInPrimaryRole";

	internal const string ConnectionModeInSecondaryRolePropertyName = "ConnectionModeInSecondaryRole";

	internal const string SessionTimeoutPropertyName = "SessionTimeout";

	internal const string BackupPriorityPropertyName = "BackupPriority";

	internal const string SeedingModePropertyName = "SeedingMode";

	internal const string ReadonlyRoutingConnectionUrlPropertyName = "ReadonlyRoutingConnectionUrl";

	internal const string ReadonlyRoutingListPropertyName = "ReadonlyRoutingList";

	internal static readonly string TargetNameScript;

	internal static readonly string ModifyReplicaScript;

	internal static readonly string EndpointUrlScript;

	internal static readonly string PrimaryRoleScript;

	internal static readonly string SecondaryRoleScript;

	internal static readonly string BackupPriorityScript;

	internal static readonly string SeedingModeScript;

	internal static readonly string ReadonlyRoutingConnectionUrlScript;

	internal static readonly string ReadonlyRoutingListScript;

	internal static readonly string NoneScript;

	internal static readonly string AllowConnectionsScript;

	internal static readonly string AvailabilityModeScript;

	internal static readonly string FailoverModeScript;

	internal static readonly string SessionTimeoutScript;

	internal static readonly string[] RequiredPropertyNames;

	internal static readonly string[] ConfigurationOnlyModeProperties;

	private static Dictionary<string, PropertyType> AlterableReplicaProperties;

	private static string[] OrderedAlterableReplicaProperties;

	private StringCollection readonlyRoutingList;

	private IList<IList<string>> loadBalancedReadonlyRoutingList;

	private static readonly TraceContext tc;

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
	public AvailabilityReplicaAvailabilityMode AvailabilityMode
	{
		get
		{
			return (AvailabilityReplicaAvailabilityMode)base.Properties.GetValueWithNullReplacement("AvailabilityMode");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AvailabilityMode", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int BackupPriority
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("BackupPriority");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BackupPriority", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityReplicaConnectionModeInPrimaryRole ConnectionModeInPrimaryRole
	{
		get
		{
			return (AvailabilityReplicaConnectionModeInPrimaryRole)base.Properties.GetValueWithNullReplacement("ConnectionModeInPrimaryRole");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ConnectionModeInPrimaryRole", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityReplicaConnectionModeInSecondaryRole ConnectionModeInSecondaryRole
	{
		get
		{
			return (AvailabilityReplicaConnectionModeInSecondaryRole)base.Properties.GetValueWithNullReplacement("ConnectionModeInSecondaryRole");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ConnectionModeInSecondaryRole", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityReplicaConnectionState ConnectionState => (AvailabilityReplicaConnectionState)base.Properties.GetValueWithNullReplacement("ConnectionState");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string EndpointUrl
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("EndpointUrl");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EndpointUrl", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityReplicaFailoverMode FailoverMode
	{
		get
		{
			return (AvailabilityReplicaFailoverMode)base.Properties.GetValueWithNullReplacement("FailoverMode");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FailoverMode", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityReplicaJoinState JoinState => (AvailabilityReplicaJoinState)base.Properties.GetValueWithNullReplacement("JoinState");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string LastConnectErrorDescription => (string)base.Properties.GetValueWithNullReplacement("LastConnectErrorDescription");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int LastConnectErrorNumber => (int)base.Properties.GetValueWithNullReplacement("LastConnectErrorNumber");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime LastConnectErrorTimestamp => (DateTime)base.Properties.GetValueWithNullReplacement("LastConnectErrorTimestamp");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public ClusterMemberState MemberState => (ClusterMemberState)base.Properties.GetValueWithNullReplacement("MemberState");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityReplicaOperationalState OperationalState => (AvailabilityReplicaOperationalState)base.Properties.GetValueWithNullReplacement("OperationalState");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string Owner => (string)base.Properties.GetValueWithNullReplacement("Owner");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int QuorumVoteCount => (int)base.Properties.GetValueWithNullReplacement("QuorumVoteCount");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ReadonlyRoutingConnectionUrl
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ReadonlyRoutingConnectionUrl");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ReadonlyRoutingConnectionUrl", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityReplicaRole Role => (AvailabilityReplicaRole)base.Properties.GetValueWithNullReplacement("Role");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityReplicaRollupRecoveryState RollupRecoveryState => (AvailabilityReplicaRollupRecoveryState)base.Properties.GetValueWithNullReplacement("RollupRecoveryState");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityReplicaRollupSynchronizationState RollupSynchronizationState => (AvailabilityReplicaRollupSynchronizationState)base.Properties.GetValueWithNullReplacement("RollupSynchronizationState");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityReplicaSeedingMode SeedingMode
	{
		get
		{
			return (AvailabilityReplicaSeedingMode)base.Properties.GetValueWithNullReplacement("SeedingMode");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SeedingMode", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int SessionTimeout
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("SessionTimeout");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SessionTimeout", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public Guid UniqueId => (Guid)base.Properties.GetValueWithNullReplacement("UniqueId");

	public static string UrnSuffix => "AvailabilityReplica";

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public StringCollection ReadonlyRoutingList
	{
		get
		{
			if (readonlyRoutingList == null)
			{
				readonlyRoutingList = new StringCollection();
				string text = string.Empty;
				if (base.State != SqlSmoState.Creating && ReadonlyRoutingListDelimited.IndexOf('(') == -1)
				{
					text = ReadonlyRoutingListDelimited;
				}
				if (!string.IsNullOrEmpty(text))
				{
					string[] array = text.Split(',');
					string[] array2 = array;
					foreach (string text2 in array2)
					{
						readonlyRoutingList.Add(text2.Substring(2, text2.Length - 3));
					}
				}
			}
			return readonlyRoutingList;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public IList<IList<string>> LoadBalancedReadOnlyRoutingList
	{
		get
		{
			if (loadBalancedReadonlyRoutingList == null)
			{
				if (base.State != SqlSmoState.Creating)
				{
					loadBalancedReadonlyRoutingList = ConvertToReadOnlyRoutingList(ReadonlyRoutingListDelimited);
				}
				else
				{
					loadBalancedReadonlyRoutingList = new List<IList<string>>();
				}
			}
			return loadBalancedReadonlyRoutingList;
		}
	}

	public string LoadBalancedReadOnlyRoutingListDisplayString => ConvertReadOnlyRoutingListToString(LoadBalancedReadOnlyRoutingList);

	public bool IsSeedingModeSupported => IsSupportedProperty("SeedingMode");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	private string ReadonlyRoutingListDelimited => (string)base.Properties.GetValueWithNullReplacement("ReadonlyRoutingListDelimited");

	public AvailabilityReplica()
	{
	}

	public AvailabilityReplica(AvailabilityGroup availabilityGroup, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = availabilityGroup;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal AvailabilityReplica(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	static AvailabilityReplica()
	{
		TargetNameScript = "TARGETNAME";
		ModifyReplicaScript = "MODIFY";
		EndpointUrlScript = "ENDPOINT_URL";
		PrimaryRoleScript = "PRIMARY_ROLE";
		SecondaryRoleScript = "SECONDARY_ROLE";
		BackupPriorityScript = "BACKUP_PRIORITY";
		SeedingModeScript = "SEEDING_MODE";
		ReadonlyRoutingConnectionUrlScript = "READ_ONLY_ROUTING_URL";
		ReadonlyRoutingListScript = "READ_ONLY_ROUTING_LIST";
		NoneScript = "NONE";
		AllowConnectionsScript = "ALLOW_CONNECTIONS";
		AvailabilityModeScript = "AVAILABILITY_MODE";
		FailoverModeScript = "FAILOVER_MODE";
		SessionTimeoutScript = "SESSION_TIMEOUT";
		RequiredPropertyNames = new string[3] { "EndpointUrl", "FailoverMode", "AvailabilityMode" };
		ConfigurationOnlyModeProperties = new string[2] { "AvailabilityMode", "EndpointUrl" };
		OrderedAlterableReplicaProperties = new string[10] { "EndpointUrl", "FailoverMode", "AvailabilityMode", "SessionTimeout", "BackupPriority", "ConnectionModeInPrimaryRole", "ReadonlyRoutingConnectionUrl", "ReadonlyRoutingList", "ConnectionModeInSecondaryRole", "SeedingMode" };
		tc = TraceContext.GetTraceContext(SmoApplication.ModuleName, "AvailabilityReplica");
		AlterableReplicaProperties = new Dictionary<string, PropertyType>();
		AddAlterableReplicaProperty("EndpointUrl", PropertyType.Alterable);
		AddAlterableReplicaProperty("FailoverMode", PropertyType.Alterable);
		AddAlterableReplicaProperty("AvailabilityMode", PropertyType.Alterable);
		AddAlterableReplicaProperty("SessionTimeout", PropertyType.Alterable);
		AddAlterableReplicaProperty("BackupPriority", PropertyType.Alterable);
		AddAlterableReplicaProperty("SeedingMode", PropertyType.Alterable);
		AddAlterableReplicaProperty("ConnectionModeInPrimaryRole", PropertyType.Alterable | PropertyType.PrimaryRoleProperty);
		AddAlterableReplicaProperty("ReadonlyRoutingList", PropertyType.Alterable | PropertyType.PrimaryRoleProperty);
		AddAlterableReplicaProperty("ConnectionModeInSecondaryRole", PropertyType.Alterable | PropertyType.SecondaryRoleProperty);
		AddAlterableReplicaProperty("ReadonlyRoutingConnectionUrl", PropertyType.Alterable | PropertyType.SecondaryRoleProperty);
	}

	public void SetLoadBalancedReadOnlyRoutingList(IList<IList<string>> routingList)
	{
		if (routingList == null)
		{
			throw new ArgumentNullException("routingList");
		}
		LoadBalancedReadOnlyRoutingList.Clear();
		if (routingList.Count == 0)
		{
			return;
		}
		foreach (IList<string> item in routingList.Where((IList<string> item) => item != null && item.Count != 0))
		{
			LoadBalancedReadOnlyRoutingList.Add(item);
		}
	}

	public override void Refresh()
	{
		base.Refresh();
		readonlyRoutingList = null;
		loadBalancedReadonlyRoutingList = null;
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

	public void Alter()
	{
		CheckObjectState(!ExecutionManager.Recording);
		AlterImpl();
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
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
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AVAILABILITY_REPLICA, new object[3] { "NOT", text, text2 });
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append(Scripts.ALTER + Globals.space + AvailabilityGroup.AvailabilityGroupScript + Globals.space);
		stringBuilder.Append(SqlSmoObject.MakeSqlBraket(Parent.Name) + Globals.newline);
		stringBuilder.Append(Scripts.ADD + Globals.space + AvailabilityGroup.ReplicaOn + Globals.space);
		stringBuilder.Append(SqlSmoObject.MakeSqlString(Name) + Globals.space + Globals.With + Globals.space + Globals.LParen);
		stringBuilder.Append(ScriptReplicaOptions(sp));
		stringBuilder.Append(Globals.RParen);
		stringBuilder.Append(Globals.statementTerminator);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		query.Add(stringBuilder.ToString());
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		tc.Assert(null != query, "String collection should not be null");
		SqlSmoObject.ThrowIfBelowVersion110(sp.TargetServerVersionInternal);
		SqlSmoObject.ThrowIfCloud(sp.TargetDatabaseEngineType, ExceptionTemplatesImpl.UnsupportedEngineTypeException);
		string text = null;
		string text2 = null;
		string[] orderedAlterableReplicaProperties = OrderedAlterableReplicaProperties;
		foreach (string text3 in orderedAlterableReplicaProperties)
		{
			string text4 = ScriptAlterOneOption(text3, sp);
			if (!string.IsNullOrEmpty(text4))
			{
				if (text3 == "AvailabilityMode")
				{
					text = text4;
				}
				else if (text3 == "FailoverMode")
				{
					text2 = text4;
				}
				else
				{
					query.Add(text4);
				}
			}
		}
		if (text != null && text2 != null && AvailabilityMode == AvailabilityReplicaAvailabilityMode.AsynchronousCommit)
		{
			query.Add(text2);
			query.Add(text);
			return;
		}
		if (text != null)
		{
			query.Add(text);
		}
		if (text2 != null)
		{
			query.Add(text2);
		}
	}

	internal string ScriptAlterOneOption(string propertyName, ScriptingPreferences sp)
	{
		string text = ScriptReplicaOption(scriptAll: false, propertyName, sp);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		if (IsPrimaryRoleProperty(propertyName))
		{
			text = PrimaryRoleScript + Globals.LParen + text + Globals.RParen;
		}
		else if (IsSecondaryRoleProperty(propertyName))
		{
			text = SecondaryRoleScript + Globals.LParen + text + Globals.RParen;
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		FormatFullNameForScripting(sp);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			string text2 = FormatFullNameForScripting(sp, nameIsIndentifier: false);
			string text3 = Parent.FormatFullNameForScripting(sp, nameIsIndentifier: false);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AVAILABILITY_REPLICA, new object[3] { "", text2, text3 });
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append(Scripts.ALTER + Globals.space + AvailabilityGroup.AvailabilityGroupScript + Globals.space);
		stringBuilder.Append(SqlSmoObject.MakeSqlBraket(Parent.Name) + Globals.newline);
		stringBuilder.Append(ModifyReplicaScript + Globals.space + AvailabilityGroup.ReplicaOn + Globals.space);
		stringBuilder.Append(SqlSmoObject.MakeSqlString(Name) + Globals.space + Globals.With + Globals.space + Globals.LParen);
		stringBuilder.Append(text + Globals.RParen);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		return stringBuilder.ToString();
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
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AVAILABILITY_REPLICA, new object[3] { "", text, text2 });
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append(Scripts.ALTER + Globals.space + AvailabilityGroup.AvailabilityGroupScript + Globals.space);
		stringBuilder.Append(SqlSmoObject.MakeSqlBraket(Parent.Name) + Globals.newline);
		stringBuilder.Append(Scripts.REMOVE + Globals.space + AvailabilityGroup.ReplicaOn + Globals.space + SqlSmoObject.MakeSqlString(Name));
		stringBuilder.Append(Globals.statementTerminator);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		dropQuery.Add(stringBuilder.ToString());
	}

	internal string ScriptReplicaOption(bool scriptAll, string propertyName, ScriptingPreferences scriptingPreferences)
	{
		if (AvailabilityMode == AvailabilityReplicaAvailabilityMode.ConfigurationOnly && !ConfigurationOnlyModeProperties.Contains(propertyName))
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (propertyName.Equals("ReadonlyRoutingList"))
		{
			stringBuilder.Append(ScriptReadonlyRoutingList());
		}
		else if (IsSupportedProperty(propertyName, scriptingPreferences))
		{
			Property propertyOptional = GetPropertyOptional(propertyName);
			if (!propertyOptional.IsNull && (scriptAll || IsDirty(propertyName)))
			{
				switch (propertyName)
				{
				case "EndpointUrl":
					stringBuilder.Append(EndpointUrlScript + Globals.space + Globals.EqualSign + Globals.space);
					stringBuilder.Append(SqlSmoObject.MakeSqlString(EndpointUrl));
					break;
				case "AvailabilityMode":
				{
					stringBuilder.Append(AvailabilityModeScript + Globals.space + Globals.EqualSign + Globals.space);
					TypeConverter typeConverter3 = SmoManagementUtil.GetTypeConverter(typeof(AvailabilityReplicaAvailabilityMode));
					stringBuilder.Append(typeConverter3.ConvertToInvariantString(AvailabilityMode));
					break;
				}
				case "FailoverMode":
				{
					stringBuilder.Append(FailoverModeScript + Globals.space + Globals.EqualSign + Globals.space);
					TypeConverter typeConverter4 = SmoManagementUtil.GetTypeConverter(typeof(AvailabilityReplicaFailoverMode));
					stringBuilder.Append(typeConverter4.ConvertToInvariantString(FailoverMode));
					break;
				}
				case "ConnectionModeInPrimaryRole":
				{
					stringBuilder.Append(AllowConnectionsScript + Globals.space + Globals.EqualSign + Globals.space);
					TypeConverter typeConverter5 = SmoManagementUtil.GetTypeConverter(typeof(AvailabilityReplicaConnectionModeInPrimaryRole));
					stringBuilder.Append(typeConverter5.ConvertToInvariantString(ConnectionModeInPrimaryRole));
					break;
				}
				case "ConnectionModeInSecondaryRole":
				{
					stringBuilder.Append(AllowConnectionsScript + Globals.space + Globals.EqualSign + Globals.space);
					TypeConverter typeConverter2 = SmoManagementUtil.GetTypeConverter(typeof(AvailabilityReplicaConnectionModeInSecondaryRole));
					stringBuilder.Append(typeConverter2.ConvertToInvariantString(ConnectionModeInSecondaryRole));
					break;
				}
				case "SessionTimeout":
					stringBuilder.Append(SessionTimeoutScript + Globals.space + Globals.EqualSign + Globals.space + SessionTimeout);
					break;
				case "ReadonlyRoutingConnectionUrl":
					if (!string.IsNullOrEmpty(ReadonlyRoutingConnectionUrl))
					{
						stringBuilder.Append(ReadonlyRoutingConnectionUrlScript + Globals.space + Globals.EqualSign + Globals.space);
						stringBuilder.Append(SqlSmoObject.MakeSqlString(ReadonlyRoutingConnectionUrl));
					}
					break;
				case "BackupPriority":
					if (scriptingPreferences.TargetServerVersionInternal >= SqlServerVersionInternal.Version130)
					{
						Property propertyOptional2 = Parent.GetPropertyOptional("BasicAvailabilityGroup");
						if (propertyOptional2.IsNull || propertyOptional2.Value.Equals(false))
						{
							stringBuilder.Append(BackupPriorityScript + Globals.space + Globals.EqualSign + Globals.space + BackupPriority);
						}
					}
					else
					{
						stringBuilder.Append(BackupPriorityScript + Globals.space + Globals.EqualSign + Globals.space + BackupPriority);
					}
					break;
				case "SeedingMode":
				{
					TypeConverter typeConverter = SmoManagementUtil.GetTypeConverter(typeof(AvailabilityReplicaSeedingMode));
					stringBuilder.Append(SeedingModeScript + Globals.space + Globals.EqualSign + Globals.space + typeConverter.ConvertToInvariantString(SeedingMode));
					break;
				}
				}
			}
		}
		return stringBuilder.ToString();
	}

	internal string ScriptReplicaOptions(ScriptingPreferences scriptingPreferences)
	{
		CheckRequiredPropertiesSetBeforeCreation();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		StringCollection stringCollection = new StringCollection();
		StringCollection stringCollection2 = new StringCollection();
		string[] orderedAlterableReplicaProperties = OrderedAlterableReplicaProperties;
		foreach (string propertyName in orderedAlterableReplicaProperties)
		{
			string text = ScriptReplicaOption(scriptAll: true, propertyName, scriptingPreferences);
			if (!string.IsNullOrEmpty(text))
			{
				if (IsPrimaryRoleProperty(propertyName))
				{
					stringCollection.Add(text);
				}
				else if (IsSecondaryRoleProperty(propertyName))
				{
					stringCollection2.Add(text);
				}
				else
				{
					stringBuilder.Append(text + Globals.comma + Globals.space);
				}
			}
		}
		AppendReplicaRoleScripts(stringCollection, PrimaryRoleScript, stringBuilder);
		AppendReplicaRoleScripts(stringCollection2, SecondaryRoleScript, stringBuilder);
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(stringBuilder.Length - 2, 2);
		}
		return stringBuilder.ToString();
	}

	private string ScriptReadonlyRoutingList()
	{
		string text = ConvertReadOnlyRoutingListToString(LoadBalancedReadOnlyRoutingList, tsqlCompatible: true);
		string b = ((base.State == SqlSmoState.Creating) ? string.Empty : ReadonlyRoutingListDelimited);
		string text2;
		if (base.State != SqlSmoState.Creating && ReadonlyRoutingListDelimited.IndexOf('(') != -1)
		{
			text2 = text;
		}
		else
		{
			string text3 = string.Join(",", (from string replica in ReadonlyRoutingList
				select SqlSmoObject.MakeSqlString(replica)).ToArray());
			if ((base.State != SqlSmoState.Creating && !string.Equals(text3, b, StringComparison.OrdinalIgnoreCase) && !string.Equals(text, b, StringComparison.OrdinalIgnoreCase)) || (base.State == SqlSmoState.Creating && ReadonlyRoutingList.Count != 0 && LoadBalancedReadOnlyRoutingList.Count != 0))
			{
				throw new SmoException(ExceptionTemplatesImpl.CannotUpdateBothReadOnlyRoutingLists);
			}
			text2 = (string.Equals(text3, b, StringComparison.OrdinalIgnoreCase) ? text : text3);
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (!string.Equals(text2, b, StringComparison.OrdinalIgnoreCase))
		{
			stringBuilder.Append(ReadonlyRoutingListScript + Globals.space + Globals.EqualSign + Globals.space);
			if (!string.IsNullOrEmpty(text2))
			{
				stringBuilder.Append(Globals.LParen + text2 + Globals.RParen);
			}
			else
			{
				stringBuilder.Append(NoneScript);
			}
		}
		return stringBuilder.ToString();
	}

	private void AppendReplicaRoleScripts(StringCollection replicaRolePropertyScriptCollection, string roleDDLScript, StringBuilder script)
	{
		if (replicaRolePropertyScriptCollection.Count <= 0)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(roleDDLScript + Globals.LParen);
		StringEnumerator enumerator = replicaRolePropertyScriptCollection.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				stringBuilder.Append(current + Globals.comma + Globals.space);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		stringBuilder.Remove(stringBuilder.Length - 2, 2);
		stringBuilder.Append(Globals.RParen);
		script.Append(stringBuilder.ToString() + Globals.comma + Globals.space);
	}

	private bool IsDirty(string property)
	{
		return base.Properties.IsDirty(base.Properties.LookupID(property, PropertyAccessPurpose.Read));
	}

	private void CheckRequiredPropertiesSetBeforeCreation()
	{
		string[] requiredPropertyNames = RequiredPropertyNames;
		foreach (string text in requiredPropertyNames)
		{
			if (GetPropValueOptional(text) == null)
			{
				throw new PropertyNotSetException(text);
			}
		}
	}

	private bool IsAlterableProperty(string propertyName)
	{
		PropertyType value;
		bool flag = AlterableReplicaProperties.TryGetValue(propertyName, out value);
		tc.Assert(flag, "Invalid property name: " + propertyName);
		return (value & PropertyType.Alterable) == PropertyType.Alterable;
	}

	private bool IsPrimaryRoleProperty(string propertyName)
	{
		PropertyType value;
		bool flag = AlterableReplicaProperties.TryGetValue(propertyName, out value);
		tc.Assert(flag, "Invalid property name: " + propertyName);
		return (value & PropertyType.PrimaryRoleProperty) == PropertyType.PrimaryRoleProperty;
	}

	private bool IsSecondaryRoleProperty(string propertyName)
	{
		PropertyType value;
		bool flag = AlterableReplicaProperties.TryGetValue(propertyName, out value);
		tc.Assert(flag, "Invalid property name: " + propertyName);
		return (value & PropertyType.SecondaryRoleProperty) == PropertyType.SecondaryRoleProperty;
	}

	private static void AddAlterableReplicaProperty(string propertyName, PropertyType propertyType)
	{
		AlterableReplicaProperties.Add(propertyName, propertyType);
	}

	private static IList<IList<string>> ConvertToReadOnlyRoutingList(string readOnlyRoutingListDisplayString)
	{
		List<IList<string>> list = new List<IList<string>>();
		if (!string.IsNullOrEmpty(readOnlyRoutingListDisplayString))
		{
			int num = 0;
			List<string> list2 = new List<string>();
			bool flag = false;
			while (num < readOnlyRoutingListDisplayString.Length)
			{
				switch (readOnlyRoutingListDisplayString[num])
				{
				case '(':
					flag = true;
					num++;
					break;
				case ')':
					flag = false;
					list.Add(list2);
					list2 = new List<string>();
					num++;
					break;
				case ',':
					num++;
					break;
				case 'N':
				{
					int num2 = readOnlyRoutingListDisplayString.IndexOf('\'', num + 2);
					string item = readOnlyRoutingListDisplayString.Substring(num + 2, num2 - num - 2);
					list2.Add(item);
					if (!flag)
					{
						list.Add(list2);
						list2 = new List<string>();
					}
					num = num2 + 1;
					break;
				}
				default:
					num++;
					break;
				}
			}
		}
		return list;
	}

	public static string ConvertReadOnlyRoutingListToString(IList<IList<string>> readOnlyRoutingList, bool tsqlCompatible = false)
	{
		if (readOnlyRoutingList.Any((IList<string> row) => row.Any((string replicaName) => string.IsNullOrEmpty(replicaName))))
		{
			throw new InvalidArgumentException(ExceptionTemplatesImpl.ReadOnlyRoutingListContainsEmptyReplicaName);
		}
		if (readOnlyRoutingList != null)
		{
			return string.Join(','.ToString(), readOnlyRoutingList.Select(delegate(IList<string> row)
			{
				string text = string.Join(','.ToString(), row.Select((string replica) => (!tsqlCompatible) ? replica : SqlSmoObject.MakeSqlString(replica)).ToArray());
				return (row.Count <= 1) ? text : $"{'('}{text}{')'}";
			}).ToArray());
		}
		return string.Empty;
	}
}
