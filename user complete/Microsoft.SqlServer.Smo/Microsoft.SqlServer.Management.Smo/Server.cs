using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Management.Smo.Agent;
using Microsoft.SqlServer.Management.Smo.Broker;
using Microsoft.SqlServer.Management.Smo.Mail;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[RootFacet(typeof(Server))]
[PhysicalFacet]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public sealed class Server : SqlSmoObject, ISfcSupportsDesignMode, IAlterable, IScriptable, IServerSettings, IServerInformation, IDmfFacet, IAlienRoot, ISfcDomainLite, ISfcHasConnection
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 32, 44, 62, 67, 68, 75, 75, 76, 80, 80 };

		private static int[] cloudVersionCount = new int[3] { 25, 27, 28 };

		private static int sqlDwPropertyCount = 27;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[27]
		{
			new StaticMetadata("BuildNumber", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("Collation", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("CollationID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ComparisonStyle", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Edition", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("EngineEdition", expensive: false, readOnly: true, typeof(Edition)),
			new StaticMetadata("HostPlatform", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("InstanceName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("IsCaseSensitive", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsContainedAuthentication", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSingleUser", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsXTPSupported", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("MaxPrecision", expensive: false, readOnly: true, typeof(byte)),
			new StaticMetadata("PathSeparator", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ProductLevel", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ProductUpdateLevel", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ResourceLastUpdateDateTime", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ResourceVersionString", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ServerType", expensive: false, readOnly: true, typeof(DatabaseEngineType)),
			new StaticMetadata("SqlCharSet", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("SqlCharSetName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("SqlSortOrder", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("SqlSortOrderName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Status", expensive: false, readOnly: true, typeof(ServerStatus)),
			new StaticMetadata("VersionMajor", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("VersionMinor", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("VersionString", expensive: false, readOnly: true, typeof(string))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[28]
		{
			new StaticMetadata("BuildNumber", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("Collation", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("CollationID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ComparisonStyle", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Edition", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("EngineEdition", expensive: false, readOnly: true, typeof(Edition)),
			new StaticMetadata("InstanceName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("IsCaseSensitive", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsContainedAuthentication", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSingleUser", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsXTPSupported", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("MaxPrecision", expensive: false, readOnly: true, typeof(byte)),
			new StaticMetadata("ProductLevel", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ProductUpdateLevel", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ResourceLastUpdateDateTime", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ResourceVersionString", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ServerType", expensive: false, readOnly: true, typeof(DatabaseEngineType)),
			new StaticMetadata("SqlCharSet", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("SqlCharSetName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("SqlSortOrder", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("SqlSortOrderName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Status", expensive: false, readOnly: true, typeof(ServerStatus)),
			new StaticMetadata("VersionMajor", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("VersionMinor", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("VersionString", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("HostPlatform", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("PathSeparator", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("IsFullTextInstalled", expensive: false, readOnly: true, typeof(bool))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[80]
		{
			new StaticMetadata("AuditLevel", expensive: false, readOnly: false, typeof(AuditLevel)),
			new StaticMetadata("BackupDirectory", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("BuildNumber", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("DefaultFile", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DefaultLog", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ErrorLogPath", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("HasNullSaPassword", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("HostPlatform", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("InstallDataDirectory", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("IsCaseSensitive", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsContainedAuthentication", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsFullTextInstalled", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsXTPSupported", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("Language", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("LoginMode", expensive: false, readOnly: false, typeof(ServerLoginMode)),
			new StaticMetadata("MailProfile", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("MasterDBLogPath", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("MasterDBPath", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("MaxPrecision", expensive: false, readOnly: true, typeof(byte)),
			new StaticMetadata("NumberOfLogFiles", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("OSVersion", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("PathSeparator", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("PerfMonMode", expensive: false, readOnly: false, typeof(PerfMonMode)),
			new StaticMetadata("PhysicalMemory", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("Platform", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("Processors", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("Product", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("RootDirectory", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ServerType", expensive: false, readOnly: true, typeof(DatabaseEngineType)),
			new StaticMetadata("ServiceName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("VersionMajor", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("VersionMinor", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("Collation", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Edition", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("EngineEdition", expensive: false, readOnly: true, typeof(Edition)),
			new StaticMetadata("InstanceName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("IsClustered", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSingleUser", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("NetName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ProductLevel", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ProductUpdateLevel", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Status", expensive: false, readOnly: true, typeof(ServerStatus)),
			new StaticMetadata("TapeLoadWaitTime", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("VersionString", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("BrowserServiceAccount", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("BrowserStartMode", expensive: true, readOnly: true, typeof(ServiceStartMode)),
			new StaticMetadata("BuildClrVersionString", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("CollationID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ComparisonStyle", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ComputerNamePhysicalNetBIOS", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("InstallSharedDirectory", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("NamedPipesEnabled", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ResourceLastUpdateDateTime", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ResourceVersionString", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ServiceAccount", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ServiceInstanceId", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("ServiceStartMode", expensive: false, readOnly: true, typeof(ServiceStartMode)),
			new StaticMetadata("SqlCharSet", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("SqlCharSetName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("SqlSortOrder", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("SqlSortOrderName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("TcpEnabled", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("FilestreamLevel", expensive: false, readOnly: true, typeof(FileStreamEffectiveLevel)),
			new StaticMetadata("FilestreamShareName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("PhysicalMemoryUsageInKB", expensive: true, readOnly: true, typeof(long)),
			new StaticMetadata("PolicyHealthState", expensive: false, readOnly: false, typeof(PolicyHealthState)),
			new StaticMetadata("SqlDomainGroup", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ProcessorUsage", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("ClusterName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ClusterQuorumState", expensive: false, readOnly: true, typeof(ClusterQuorumState)),
			new StaticMetadata("ClusterQuorumType", expensive: false, readOnly: true, typeof(ClusterQuorumType)),
			new StaticMetadata("ErrorLogSizeKb", expensive: true, readOnly: false, typeof(int)),
			new StaticMetadata("HadrManagerStatus", expensive: false, readOnly: true, typeof(HadrManagerStatus)),
			new StaticMetadata("IsHadrEnabled", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ServiceAccountSid", expensive: false, readOnly: false, typeof(byte[])),
			new StaticMetadata("IsPolyBaseInstalled", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HostDistribution", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("HostRelease", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("HostServicePackLevel", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("HostSku", expensive: true, readOnly: true, typeof(int))
		};

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
					return propertyName switch
					{
						"BuildNumber" => 0, 
						"Collation" => 1, 
						"CollationID" => 2, 
						"ComparisonStyle" => 3, 
						"Edition" => 4, 
						"EngineEdition" => 5, 
						"HostPlatform" => 6, 
						"InstanceName" => 7, 
						"IsCaseSensitive" => 8, 
						"IsContainedAuthentication" => 9, 
						"IsSingleUser" => 10, 
						"IsXTPSupported" => 11, 
						"MaxPrecision" => 12, 
						"PathSeparator" => 13, 
						"ProductLevel" => 14, 
						"ProductUpdateLevel" => 15, 
						"ResourceLastUpdateDateTime" => 16, 
						"ResourceVersionString" => 17, 
						"ServerType" => 18, 
						"SqlCharSet" => 19, 
						"SqlCharSetName" => 20, 
						"SqlSortOrder" => 21, 
						"SqlSortOrderName" => 22, 
						"Status" => 23, 
						"VersionMajor" => 24, 
						"VersionMinor" => 25, 
						"VersionString" => 26, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"BuildNumber" => 0, 
					"Collation" => 1, 
					"CollationID" => 2, 
					"ComparisonStyle" => 3, 
					"Edition" => 4, 
					"EngineEdition" => 5, 
					"InstanceName" => 6, 
					"IsCaseSensitive" => 7, 
					"IsContainedAuthentication" => 8, 
					"IsSingleUser" => 9, 
					"IsXTPSupported" => 10, 
					"MaxPrecision" => 11, 
					"ProductLevel" => 12, 
					"ProductUpdateLevel" => 13, 
					"ResourceLastUpdateDateTime" => 14, 
					"ResourceVersionString" => 15, 
					"ServerType" => 16, 
					"SqlCharSet" => 17, 
					"SqlCharSetName" => 18, 
					"SqlSortOrder" => 19, 
					"SqlSortOrderName" => 20, 
					"Status" => 21, 
					"VersionMajor" => 22, 
					"VersionMinor" => 23, 
					"VersionString" => 24, 
					"HostPlatform" => 25, 
					"PathSeparator" => 26, 
					"IsFullTextInstalled" => 27, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"AuditLevel" => 0, 
				"BackupDirectory" => 1, 
				"BuildNumber" => 2, 
				"DefaultFile" => 3, 
				"DefaultLog" => 4, 
				"ErrorLogPath" => 5, 
				"HasNullSaPassword" => 6, 
				"HostPlatform" => 7, 
				"InstallDataDirectory" => 8, 
				"IsCaseSensitive" => 9, 
				"IsContainedAuthentication" => 10, 
				"IsFullTextInstalled" => 11, 
				"IsXTPSupported" => 12, 
				"Language" => 13, 
				"LoginMode" => 14, 
				"MailProfile" => 15, 
				"MasterDBLogPath" => 16, 
				"MasterDBPath" => 17, 
				"MaxPrecision" => 18, 
				"NumberOfLogFiles" => 19, 
				"OSVersion" => 20, 
				"PathSeparator" => 21, 
				"PerfMonMode" => 22, 
				"PhysicalMemory" => 23, 
				"Platform" => 24, 
				"Processors" => 25, 
				"Product" => 26, 
				"RootDirectory" => 27, 
				"ServerType" => 28, 
				"ServiceName" => 29, 
				"VersionMajor" => 30, 
				"VersionMinor" => 31, 
				"Collation" => 32, 
				"Edition" => 33, 
				"EngineEdition" => 34, 
				"InstanceName" => 35, 
				"IsClustered" => 36, 
				"IsSingleUser" => 37, 
				"NetName" => 38, 
				"ProductLevel" => 39, 
				"ProductUpdateLevel" => 40, 
				"Status" => 41, 
				"TapeLoadWaitTime" => 42, 
				"VersionString" => 43, 
				"BrowserServiceAccount" => 44, 
				"BrowserStartMode" => 45, 
				"BuildClrVersionString" => 46, 
				"CollationID" => 47, 
				"ComparisonStyle" => 48, 
				"ComputerNamePhysicalNetBIOS" => 49, 
				"InstallSharedDirectory" => 50, 
				"NamedPipesEnabled" => 51, 
				"ResourceLastUpdateDateTime" => 52, 
				"ResourceVersionString" => 53, 
				"ServiceAccount" => 54, 
				"ServiceInstanceId" => 55, 
				"ServiceStartMode" => 56, 
				"SqlCharSet" => 57, 
				"SqlCharSetName" => 58, 
				"SqlSortOrder" => 59, 
				"SqlSortOrderName" => 60, 
				"TcpEnabled" => 61, 
				"FilestreamLevel" => 62, 
				"FilestreamShareName" => 63, 
				"PhysicalMemoryUsageInKB" => 64, 
				"PolicyHealthState" => 65, 
				"SqlDomainGroup" => 66, 
				"ProcessorUsage" => 67, 
				"ClusterName" => 68, 
				"ClusterQuorumState" => 69, 
				"ClusterQuorumType" => 70, 
				"ErrorLogSizeKb" => 71, 
				"HadrManagerStatus" => 72, 
				"IsHadrEnabled" => 73, 
				"ServiceAccountSid" => 74, 
				"IsPolyBaseInstalled" => 75, 
				"HostDistribution" => 76, 
				"HostRelease" => 77, 
				"HostServicePackLevel" => 78, 
				"HostSku" => 79, 
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
	}

	private const string DomainName = "SMO";

	private ExecutionManager m_ExecutionManager;

	private ServerConnection serverConnection;

	private SortedList collationCache;

	private bool overrideValueChecking;

	private bool defaultTextMode = true;

	private Configuration m_config;

	private AffinityInfo affinityInfo;

	private ServerProxyAccount proxyAccount;

	private SqlMail mail;

	private DatabaseCollection m_Databases;

	private EndpointCollection m_Endpoints;

	private LanguageCollection m_Languages;

	private SystemMessageCollection systemMessages;

	private UserDefinedMessageCollection userDefinedMessages;

	private CredentialCollection credentials;

	private CryptographicProviderCollection cryptographicProviders;

	private LoginCollection m_Logins;

	private ServerRoleCollection m_Roles;

	private LinkedServerCollection m_LinkedServers;

	private SystemDataTypeCollection systemDataTypes;

	private JobServer jobServer;

	private ResourceGovernor resourceGovernor;

	private ServiceMasterKey masterKey;

	private SmartAdmin smartAdmin;

	private Settings m_Settings;

	private Information m_Information;

	private UserOptions m_UserOption;

	private BackupDeviceCollection m_BackupDevices;

	private FullTextService fullTextService;

	private ServerActiveDirectory serverActiveDirectory;

	private ServerDdlTriggerCollection serverDdlTriggerCollection;

	private AuditCollection auditCollection;

	private ServerAuditSpecificationCollection serverAuditSpecificationCollection;

	private AvailabilityGroupCollection m_AvailabilityGroups;

	private DataTable collations;

	private Dictionary<string, CollationVersion> collationVersionDictionary;

	private Hashtable hashInitFields;

	private Hashtable collInitFields;

	private bool useAllFieldsForInit;

	private Hashtable objectMetadataHash = new Hashtable();

	private static HashSet<string> typesToIgnore = new HashSet<string> { "NumberedStoredProcedure", "QueryStoreOptions" };

	private object syncRoot = new object();

	private ServerEvents events;

	private OleDbProviderSettingsCollection m_OleDbProviderSettings;

	private SfcConnectionContext sfcConnectionContext;

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AuditLevel AuditLevel
	{
		get
		{
			return (AuditLevel)base.Properties.GetValueWithNullReplacement("AuditLevel");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AuditLevel", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string BackupDirectory
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("BackupDirectory");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BackupDirectory", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string BrowserServiceAccount => (string)base.Properties.GetValueWithNullReplacement("BrowserServiceAccount");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public ServiceStartMode BrowserStartMode => (ServiceStartMode)base.Properties.GetValueWithNullReplacement("BrowserStartMode");

	[SfcProperty(SfcPropertyFlags.Standalone, "v2.0.50727")]
	public string BuildClrVersionString => (string)base.Properties.GetValueWithNullReplacement("BuildClrVersionString");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int BuildNumber => (int)base.Properties.GetValueWithNullReplacement("BuildNumber");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ClusterName => (string)base.Properties.GetValueWithNullReplacement("ClusterName");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public ClusterQuorumState ClusterQuorumState => (ClusterQuorumState)base.Properties.GetValueWithNullReplacement("ClusterQuorumState");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public ClusterQuorumType ClusterQuorumType => (ClusterQuorumType)base.Properties.GetValueWithNullReplacement("ClusterQuorumType");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Collation => (string)base.Properties.GetValueWithNullReplacement("Collation");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int CollationID => (int)base.Properties.GetValueWithNullReplacement("CollationID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ComparisonStyle => (int)base.Properties.GetValueWithNullReplacement("ComparisonStyle");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ComputerNamePhysicalNetBIOS => (string)base.Properties.GetValueWithNullReplacement("ComputerNamePhysicalNetBIOS");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string DefaultFile
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DefaultFile");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DefaultFile", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string DefaultLog
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DefaultLog");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DefaultLog", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Edition => (string)base.Properties.GetValueWithNullReplacement("Edition");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ErrorLogPath => (string)base.Properties.GetValueWithNullReplacement("ErrorLogPath");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int ErrorLogSizeKb
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("ErrorLogSizeKb");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ErrorLogSizeKb", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public FileStreamEffectiveLevel FilestreamLevel => (FileStreamEffectiveLevel)base.Properties.GetValueWithNullReplacement("FilestreamLevel");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string FilestreamShareName => (string)base.Properties.GetValueWithNullReplacement("FilestreamShareName");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public HadrManagerStatus HadrManagerStatus => (HadrManagerStatus)base.Properties.GetValueWithNullReplacement("HadrManagerStatus");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string HostDistribution => (string)base.Properties.GetValueWithNullReplacement("HostDistribution");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string HostPlatform => (string)base.Properties.GetValueWithNullReplacement("HostPlatform");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string HostRelease => (string)base.Properties.GetValueWithNullReplacement("HostRelease");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string HostServicePackLevel => (string)base.Properties.GetValueWithNullReplacement("HostServicePackLevel");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int HostSku => (int)base.Properties.GetValueWithNullReplacement("HostSku");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string InstallDataDirectory => (string)base.Properties.GetValueWithNullReplacement("InstallDataDirectory");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string InstallSharedDirectory => (string)base.Properties.GetValueWithNullReplacement("InstallSharedDirectory");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string InstanceName => (string)base.Properties.GetValueWithNullReplacement("InstanceName");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsCaseSensitive => (bool)base.Properties.GetValueWithNullReplacement("IsCaseSensitive");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsClustered => (bool)base.Properties.GetValueWithNullReplacement("IsClustered");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsFullTextInstalled => (bool)base.Properties.GetValueWithNullReplacement("IsFullTextInstalled");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsHadrEnabled => (bool)base.Properties.GetValueWithNullReplacement("IsHadrEnabled");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsPolyBaseInstalled => (bool)base.Properties.GetValueWithNullReplacement("IsPolyBaseInstalled");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsSingleUser => (bool)base.Properties.GetValueWithNullReplacement("IsSingleUser");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsXTPSupported => (bool)base.Properties.GetValueWithNullReplacement("IsXTPSupported");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string Language => (string)base.Properties.GetValueWithNullReplacement("Language");

	[DmfIgnoreProperty]
	[SfcProperty(SfcPropertyFlags.Standalone)]
	public ServerLoginMode LoginMode
	{
		get
		{
			return (ServerLoginMode)base.Properties.GetValueWithNullReplacement("LoginMode");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("LoginMode", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string MailProfile
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("MailProfile");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MailProfile", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string MasterDBLogPath => (string)base.Properties.GetValueWithNullReplacement("MasterDBLogPath");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string MasterDBPath => (string)base.Properties.GetValueWithNullReplacement("MasterDBPath");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public byte MaxPrecision => (byte)base.Properties.GetValueWithNullReplacement("MaxPrecision");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool NamedPipesEnabled => (bool)base.Properties.GetValueWithNullReplacement("NamedPipesEnabled");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string NetName => (string)base.Properties.GetValueWithNullReplacement("NetName");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int NumberOfLogFiles
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("NumberOfLogFiles");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NumberOfLogFiles", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string OSVersion => (string)base.Properties.GetValueWithNullReplacement("OSVersion");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string PathSeparator => (string)base.Properties.GetValueWithNullReplacement("PathSeparator");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public PerfMonMode PerfMonMode
	{
		get
		{
			return (PerfMonMode)base.Properties.GetValueWithNullReplacement("PerfMonMode");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PerfMonMode", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int PhysicalMemory => (int)base.Properties.GetValueWithNullReplacement("PhysicalMemory");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public long PhysicalMemoryUsageInKB => (long)base.Properties.GetValueWithNullReplacement("PhysicalMemoryUsageInKB");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string Platform => (string)base.Properties.GetValueWithNullReplacement("Platform");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int Processors => (int)base.Properties.GetValueWithNullReplacement("Processors");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int ProcessorUsage => (int)base.Properties.GetValueWithNullReplacement("ProcessorUsage");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string Product => (string)base.Properties.GetValueWithNullReplacement("Product");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ProductLevel => (string)base.Properties.GetValueWithNullReplacement("ProductLevel");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ProductUpdateLevel => (string)base.Properties.GetValueWithNullReplacement("ProductUpdateLevel");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime ResourceLastUpdateDateTime => (DateTime)base.Properties.GetValueWithNullReplacement("ResourceLastUpdateDateTime");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase, "10.50.0000")]
	public string ResourceVersionString => (string)base.Properties.GetValueWithNullReplacement("ResourceVersionString");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string RootDirectory => (string)base.Properties.GetValueWithNullReplacement("RootDirectory");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DatabaseEngineType ServerType => (DatabaseEngineType)base.Properties.GetValueWithNullReplacement("ServerType");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ServiceAccount => (string)base.Properties.GetValueWithNullReplacement("ServiceAccount");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string ServiceInstanceId => (string)base.Properties.GetValueWithNullReplacement("ServiceInstanceId");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ServiceName => (string)base.Properties.GetValueWithNullReplacement("ServiceName");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public ServiceStartMode ServiceStartMode => (ServiceStartMode)base.Properties.GetValueWithNullReplacement("ServiceStartMode");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public short SqlCharSet => (short)base.Properties.GetValueWithNullReplacement("SqlCharSet");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string SqlCharSetName => (string)base.Properties.GetValueWithNullReplacement("SqlCharSetName");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string SqlDomainGroup => (string)base.Properties.GetValueWithNullReplacement("SqlDomainGroup");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public short SqlSortOrder => (short)base.Properties.GetValueWithNullReplacement("SqlSortOrder");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string SqlSortOrderName => (string)base.Properties.GetValueWithNullReplacement("SqlSortOrderName");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public ServerStatus Status => (ServerStatus)base.Properties.GetValueWithNullReplacement("Status");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int TapeLoadWaitTime
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("TapeLoadWaitTime");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("TapeLoadWaitTime", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool TcpEnabled => (bool)base.Properties.GetValueWithNullReplacement("TcpEnabled");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int VersionMajor => (int)base.Properties.GetValueWithNullReplacement("VersionMajor");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int VersionMinor => (int)base.Properties.GetValueWithNullReplacement("VersionMinor");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string VersionString => (string)base.Properties.GetValueWithNullReplacement("VersionString");

	public override ExecutionManager ExecutionManager => GetExecutionManager();

	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Name
	{
		get
		{
			try
			{
				if (!SqlContext.IsAvailable)
				{
					return NormalizeServerName(ConnectionContext.ServerInstance);
				}
				return ConnectionContext.ServerInstance;
			}
			catch (PropertyNotAvailableException)
			{
				try
				{
					return ConnectionContext.TrueName;
				}
				catch (ExecutionFailureException)
				{
				}
			}
			return string.Empty;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[SfcSerializationAdapter(typeof(VersionSerializationAdapter))]
	public Version Version
	{
		get
		{
			ServerVersion serverVersion = base.ServerVersion;
			return new Version(serverVersion.Major, serverVersion.Minor, serverVersion.BuildNumber);
		}
		internal set
		{
			if (base.IsDesignMode)
			{
				if (value != null)
				{
					ConnectionContext.ServerVersion = new ServerVersion(value.Major, value.Minor, value.Build);
				}
				else
				{
					ConnectionContext.ServerVersion = null;
				}
			}
			else
			{
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(condition: false, "Version property of Server can only be set in design mode");
			}
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public Edition EngineEdition => (Edition)(int)base.Properties.GetValueWithNullReplacement("EngineEdition");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public Version ResourceVersion => new Version(ResourceVersionString);

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public Version BuildClrVersion => new Version(BuildClrVersionString.Substring(1));

	public static string UrnSuffix => "Server";

	internal override UserPermissionCollection Permissions => GetUserPermissions();

	public bool DefaultTextMode
	{
		get
		{
			return defaultTextMode;
		}
		set
		{
			defaultTextMode = value;
		}
	}

	[SfcObject(SfcObjectRelationship.ChildObject, SfcObjectCardinality.One)]
	public Configuration Configuration
	{
		get
		{
			if (m_config == null)
			{
				m_config = new Configuration(this);
			}
			return m_config;
		}
	}

	[SfcObject(SfcObjectRelationship.ChildObject, SfcObjectCardinality.One)]
	public AffinityInfo AffinityInfo
	{
		get
		{
			if (affinityInfo == null)
			{
				affinityInfo = new AffinityInfo(this);
			}
			return affinityInfo;
		}
	}

	[SfcObject(SfcObjectRelationship.Object, SfcObjectCardinality.One)]
	public ServerProxyAccount ProxyAccount
	{
		get
		{
			this.ThrowIfNotSupported(typeof(ServerProxyAccount));
			if (proxyAccount == null)
			{
				proxyAccount = new ServerProxyAccount(this, new SimpleObjectKey(Name), SqlSmoState.Existing);
			}
			return proxyAccount;
		}
	}

	[SfcObject(SfcObjectRelationship.Object, SfcObjectCardinality.One)]
	public SqlMail Mail
	{
		get
		{
			ThrowIfExpressSku(ExceptionTemplatesImpl.UnsupportedFeatureSqlMail);
			this.ThrowIfNotSupported(typeof(SqlMail));
			if (mail == null)
			{
				mail = new SqlMail(this, new SimpleObjectKey(Name), SqlSmoState.Existing);
			}
			return mail;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(Database), SfcObjectFlags.Design)]
	public DatabaseCollection Databases
	{
		get
		{
			if (m_Databases == null)
			{
				m_Databases = new DatabaseCollection(this);
			}
			return m_Databases;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(Endpoint))]
	public EndpointCollection Endpoints
	{
		get
		{
			if (m_Endpoints == null)
			{
				m_Endpoints = new EndpointCollection(this);
			}
			return m_Endpoints;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(Language))]
	public LanguageCollection Languages
	{
		get
		{
			if (m_Languages == null)
			{
				m_Languages = new LanguageCollection(this);
			}
			return m_Languages;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(SystemMessage))]
	public SystemMessageCollection SystemMessages
	{
		get
		{
			if (systemMessages == null)
			{
				systemMessages = new SystemMessageCollection(this);
			}
			return systemMessages;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(UserDefinedMessage))]
	public UserDefinedMessageCollection UserDefinedMessages
	{
		get
		{
			if (userDefinedMessages == null)
			{
				userDefinedMessages = new UserDefinedMessageCollection(this);
			}
			return userDefinedMessages;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(Credential))]
	public CredentialCollection Credentials
	{
		get
		{
			if (credentials == null)
			{
				credentials = new CredentialCollection(this);
			}
			return credentials;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(CryptographicProvider))]
	public CryptographicProviderCollection CryptographicProviders
	{
		get
		{
			this.ThrowIfNotSupported(typeof(CryptographicProvider));
			if (cryptographicProviders == null)
			{
				cryptographicProviders = new CryptographicProviderCollection(this);
			}
			return cryptographicProviders;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(Login))]
	public LoginCollection Logins
	{
		get
		{
			if (m_Logins == null)
			{
				m_Logins = new LoginCollection(this);
			}
			return m_Logins;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(ServerRole))]
	public ServerRoleCollection Roles
	{
		get
		{
			if (m_Roles == null)
			{
				m_Roles = new ServerRoleCollection(this);
			}
			return m_Roles;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(LinkedServer))]
	public LinkedServerCollection LinkedServers
	{
		get
		{
			this.ThrowIfNotSupported(typeof(LinkedServer));
			if (m_LinkedServers == null)
			{
				m_LinkedServers = new LinkedServerCollection(this);
			}
			return m_LinkedServers;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(SystemDataType))]
	public SystemDataTypeCollection SystemDataTypes
	{
		get
		{
			if (systemDataTypes == null)
			{
				systemDataTypes = new SystemDataTypeCollection(this);
			}
			return systemDataTypes;
		}
	}

	[SfcObject(SfcObjectRelationship.Object, SfcObjectCardinality.One)]
	public JobServer JobServer
	{
		get
		{
			ThrowIfExpressSku(ExceptionTemplatesImpl.UnsupportedFeatureSqlAgent);
			this.ThrowIfNotSupported(typeof(JobServer));
			if (jobServer == null)
			{
				jobServer = new JobServer(this, new SimpleObjectKey(Name), SqlSmoState.Existing);
			}
			return jobServer;
		}
	}

	[SfcObject(SfcObjectRelationship.Object, SfcObjectCardinality.One)]
	public ResourceGovernor ResourceGovernor
	{
		get
		{
			ThrowIfExpressSku(ExceptionTemplatesImpl.UnsupportedFeatureResourceGovernor);
			this.ThrowIfNotSupported(typeof(ResourceGovernor));
			if (resourceGovernor == null)
			{
				resourceGovernor = new ResourceGovernor(this, new ObjectKeyBase(), SqlSmoState.Existing);
			}
			return resourceGovernor;
		}
	}

	[SfcObject(SfcObjectRelationship.Object, SfcObjectCardinality.One)]
	public ServiceMasterKey ServiceMasterKey
	{
		get
		{
			this.ThrowIfNotSupported(typeof(ServiceMasterKey));
			if (masterKey == null)
			{
				masterKey = new ServiceMasterKey(this, new ObjectKeyBase(), SqlSmoState.Existing);
			}
			return masterKey;
		}
	}

	[SfcObject(SfcObjectRelationship.Object, SfcObjectCardinality.One)]
	public SmartAdmin SmartAdmin
	{
		get
		{
			ThrowIfExpressSku(ExceptionTemplatesImpl.UnsupportedFeatureSmartAdmin);
			this.ThrowIfNotSupported(typeof(SmartAdmin));
			if (smartAdmin == null)
			{
				smartAdmin = new SmartAdmin(this, new ObjectKeyBase(), SqlSmoState.Existing);
			}
			return smartAdmin;
		}
	}

	[SfcObject(SfcObjectRelationship.ChildObject, SfcObjectCardinality.One)]
	public Settings Settings
	{
		get
		{
			if (m_Settings == null)
			{
				m_Settings = new Settings(this, new ObjectKeyBase(), SqlSmoState.Existing);
			}
			return m_Settings;
		}
	}

	[SfcObject(SfcObjectRelationship.ChildObject, SfcObjectCardinality.One)]
	public Information Information
	{
		get
		{
			if (m_Information == null)
			{
				m_Information = new Information(this, new ObjectKeyBase(), SqlSmoState.Existing);
			}
			return m_Information;
		}
	}

	[SfcObject(SfcObjectRelationship.ChildObject, SfcObjectCardinality.One)]
	public UserOptions UserOptions
	{
		get
		{
			this.ThrowIfNotSupported(typeof(UserOptions));
			if (m_UserOption == null)
			{
				m_UserOption = new UserOptions(this, new ObjectKeyBase(), SqlSmoState.Existing);
			}
			return m_UserOption;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(BackupDevice))]
	public BackupDeviceCollection BackupDevices
	{
		get
		{
			if (m_BackupDevices == null)
			{
				m_BackupDevices = new BackupDeviceCollection(this);
			}
			return m_BackupDevices;
		}
	}

	[SfcObject(SfcObjectRelationship.Object, SfcObjectCardinality.One)]
	public FullTextService FullTextService
	{
		get
		{
			this.ThrowIfNotSupported(typeof(FullTextService));
			if (fullTextService == null)
			{
				fullTextService = new FullTextService(this, new SimpleObjectKey(Name), SqlSmoState.Existing);
			}
			return fullTextService;
		}
	}

	[SfcObject(SfcObjectRelationship.Object, SfcObjectCardinality.One)]
	[Obsolete]
	public ServerActiveDirectory ActiveDirectory
	{
		get
		{
			ThrowIfAboveVersion100();
			this.ThrowIfNotSupported(typeof(ServerActiveDirectory));
			if (serverActiveDirectory == null)
			{
				serverActiveDirectory = new ServerActiveDirectory(this, new ObjectKeyBase(), SqlSmoState.Existing);
			}
			return serverActiveDirectory;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(ServerDdlTrigger))]
	public ServerDdlTriggerCollection Triggers
	{
		get
		{
			this.ThrowIfNotSupported(typeof(ServerDdlTrigger));
			if (serverDdlTriggerCollection == null)
			{
				serverDdlTriggerCollection = new ServerDdlTriggerCollection(this);
			}
			return serverDdlTriggerCollection;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(Audit))]
	public AuditCollection Audits
	{
		get
		{
			this.ThrowIfNotSupported(typeof(Audit));
			if (auditCollection == null)
			{
				auditCollection = new AuditCollection(this);
			}
			return auditCollection;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(ServerAuditSpecification))]
	public ServerAuditSpecificationCollection ServerAuditSpecifications
	{
		get
		{
			this.ThrowIfNotSupported(typeof(ServerAuditSpecification));
			if (serverAuditSpecificationCollection == null)
			{
				serverAuditSpecificationCollection = new ServerAuditSpecificationCollection(this);
			}
			return serverAuditSpecificationCollection;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(AvailabilityGroup))]
	public AvailabilityGroupCollection AvailabilityGroups
	{
		get
		{
			if (m_AvailabilityGroups == null)
			{
				m_AvailabilityGroups = new AvailabilityGroupCollection(this);
			}
			return m_AvailabilityGroups;
		}
	}

	internal Hashtable HashInitFields => hashInitFields;

	internal Hashtable CollInitFields => collInitFields;

	public ServerConnection ConnectionContext => serverConnection;

	public ServerEvents Events
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (SqlContext.IsAvailable)
			{
				throw new SmoException(ExceptionTemplatesImpl.SmoSQLCLRUnAvailable);
			}
			if (events == null)
			{
				events = new ServerEvents(this);
			}
			return events;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(OleDbProviderSettings))]
	public OleDbProviderSettingsCollection OleDbProviderSettings
	{
		get
		{
			CheckObjectState();
			if (m_OleDbProviderSettings == null)
			{
				m_OleDbProviderSettings = new OleDbProviderSettingsCollection(this);
			}
			return m_OleDbProviderSettings;
		}
	}

	SfcConnectionContext ISfcHasConnection.ConnectionContext
	{
		get
		{
			if (sfcConnectionContext == null)
			{
				sfcConnectionContext = new SfcConnectionContext(this);
			}
			return sfcConnectionContext;
		}
	}

	string ISfcDomainLite.DomainName => "SMO";

	string ISfcDomainLite.DomainInstanceName => m_ExecutionManager.TrueServerName;

	public AvailabilityGroupClusterType[] SupportedAvailabilityGroupClusterTypes
	{
		get
		{
			List<AvailabilityGroupClusterType> list = new List<AvailabilityGroupClusterType>();
			if (IsMemberOfWsfcCluster)
			{
				list.Add(AvailabilityGroupClusterType.Wsfc);
			}
			if (VersionMajor >= 14)
			{
				list.Add(AvailabilityGroupClusterType.External);
				list.Add(AvailabilityGroupClusterType.None);
			}
			return list.ToArray();
		}
	}

	public AvailabilityGroupClusterType DefaultAvailabilityGroupClusterType
	{
		get
		{
			if (!IsMemberOfWsfcCluster)
			{
				return AvailabilityGroupClusterType.External;
			}
			return AvailabilityGroupClusterType.Wsfc;
		}
	}

	public bool IsMemberOfWsfcCluster
	{
		get
		{
			if (HostPlatform == "Windows" && !string.IsNullOrEmpty(ClusterName))
			{
				return ClusterName[0] != '\0';
			}
			return false;
		}
	}

	public bool IsConfigurationOnlyAvailabilityReplicaSupported
	{
		get
		{
			if (VersionMajor <= 14)
			{
				if (VersionMajor == 14)
				{
					return BuildNumber >= 3000;
				}
				return false;
			}
			return true;
		}
	}

	public bool IsAvailabilityReplicaSeedingModeSupported => VersionMajor >= 13;

	public bool IsCrossPlatformAvailabilityGroupSupported => VersionMajor >= 14;

	public bool IsReadOnlyListWithLoadBalancingSupported => VersionMajor >= 13;

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[1] { "ServiceAccountSid" };
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		return propname switch
		{
			"BuildClrVersionString" => "v2.0.50727", 
			"ResourceVersionString" => "10.50.0000", 
			_ => base.GetPropertyDefaultValue(propname), 
		};
	}

	public void Deny(ServerPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Deny(ServerPermissionSet permission, string[] granteeNames, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, null, grantGrant: false, cascade, null);
	}

	public void Grant(ServerPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Grant(ServerPermissionSet permission, string[] granteeNames, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant, cascade: false, null);
	}

	public void Grant(ServerPermissionSet permission, string[] granteeNames, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ServerPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ServerPermissionSet permission, string[] granteeNames, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, revokeGrant, cascade, null);
	}

	public void Revoke(ServerPermissionSet permission, string[] granteeNames, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, revokeGrant, cascade, asRole);
	}

	public void Deny(ServerPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Deny(ServerPermissionSet permission, string granteeName, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade, null);
	}

	public void Grant(ServerPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Grant(ServerPermissionSet permission, string granteeName, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant, cascade: false, null);
	}

	public void Grant(ServerPermissionSet permission, string granteeName, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ServerPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ServerPermissionSet permission, string granteeName, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, revokeGrant, cascade, null);
	}

	public void Revoke(ServerPermissionSet permission, string granteeName, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, revokeGrant, cascade, asRole);
	}

	public ServerPermissionInfo[] EnumServerPermissions()
	{
		return (ServerPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Server, this, null, null);
	}

	public ServerPermissionInfo[] EnumServerPermissions(string granteeName)
	{
		return (ServerPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Server, this, granteeName, null);
	}

	public ServerPermissionInfo[] EnumServerPermissions(ServerPermissionSet permissions)
	{
		return (ServerPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Server, this, null, permissions);
	}

	public ServerPermissionInfo[] EnumServerPermissions(string granteeName, ServerPermissionSet permissions)
	{
		return (ServerPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Server, this, granteeName, permissions);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions()
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumAllPermissions(this, null, null);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumAllPermissions(this, granteeName, null);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumAllPermissions(this, null, permissions);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName, ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumAllPermissions(this, granteeName, permissions);
	}

	public Server(string name)
	{
		m_ExecutionManager = new ExecutionManager(name);
		m_ExecutionManager.Parent = this;
		Init();
	}

	public Server()
	{
		m_ExecutionManager = new ExecutionManager(".");
		m_ExecutionManager.Parent = this;
		Init();
	}

	public Server(ServerConnection serverConnection)
	{
		this.serverConnection = serverConnection;
		Init();
	}

	private bool IsAzureDbScopedConnection(ServerConnection sc)
	{
		if ((!string.IsNullOrEmpty(sc.DatabaseName) && sc.DatabaseName.ToUpperInvariant() != "MASTER") || (!string.IsNullOrEmpty(sc.InitialCatalog) && sc.InitialCatalog.ToUpperInvariant() != "MASTER"))
		{
			return sc.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase;
		}
		return false;
	}

	private void Init()
	{
		if (serverConnection == null)
		{
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(m_ExecutionManager != null, "m_ExecutionManager == null");
			serverConnection = m_ExecutionManager.ConnectionContext;
		}
		hashInitFields = new Hashtable(257);
		collInitFields = new Hashtable(257);
		SetState(SqlSmoState.Existing);
		objectInSpace = false;
		SetObjectKey(new SimpleObjectKey(Name));
	}

	internal override void AddScriptPermission(StringCollection query, ScriptingPreferences sp)
	{
		AddScriptPermissions(query, PermissionWorker.PermissionEnumKind.Server, sp);
	}

	internal void SetServerNameFromConnectionInfo()
	{
		SetObjectKey(new SimpleObjectKey(NormalizeServerName(ConnectionContext.ServerInstance)));
	}

	private string NormalizeServerName(string name)
	{
		switch (name)
		{
		default:
			if (name.Length != 0)
			{
				if (name.StartsWith(".\\", StringComparison.Ordinal))
				{
					return name.Replace(".", System.Environment.MachineName);
				}
				if (name.StartsWith("(local)\\", StringComparison.Ordinal))
				{
					return name.Replace("(local)", System.Environment.MachineName);
				}
				return name;
			}
			goto case ".";
		case ".":
		case "(local)":
		case "localhost":
			return System.Environment.MachineName;
		}
	}

	public IComparer GetStringComparer(string collationName)
	{
		int lCIDCollation = GetLCIDCollation(collationName);
		return new StringComparer(collationName, lCIDCollation);
	}

	internal int GetLCIDCollation(string collationName)
	{
		if (collationName.Contains("Latin1") && !collationName.Contains("SQL_Latin1_General_CP1254"))
		{
			return 1033;
		}
		if (collationName.StartsWith("Japanese_Unicode", StringComparison.Ordinal))
		{
			return 1041;
		}
		if (collationCache == null)
		{
			collationCache = new SortedList(System.StringComparer.Ordinal);
		}
		if (!collationCache.Contains(collationName))
		{
			Request req = new Request("Server/Collation[@Name = '" + Urn.EscapeString(collationName) + "']", new string[1] { "LocaleID" });
			DataTable enumeratorData = ExecutionManager.GetEnumeratorData(req);
			if (enumeratorData.Rows.Count == 1 && !(enumeratorData.Rows[0][0] is DBNull))
			{
				collationCache[collationName] = (int)enumeratorData.Rows[0][0];
			}
			else
			{
				collationCache[collationName] = 1033;
			}
		}
		return (int)collationCache[collationName];
	}

	object IAlienRoot.SfcHelper_GetSmoObject(string urn)
	{
		return GetSmoObject(new Urn(urn));
	}

	DataTable IAlienRoot.SfcHelper_GetDataTable(object connection, string urn, string[] fields, OrderBy[] orderByFields)
	{
		OrderBy[] array = null;
		if (orderByFields != null)
		{
			array = new OrderBy[orderByFields.Length];
			orderByFields.CopyTo(array, 0);
		}
		return Enumerator.GetData(connection, new Urn(urn), fields, array);
	}

	void IAlienRoot.DesignModeInitialize()
	{
		FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(GetType().GetAssembly().Location);
		ConnectionContext.ServerVersion = new ServerVersion(versionInfo.FileMajorPart, versionInfo.FileMinorPart);
		ConnectionContext.TrueName = "DesignMode";
		((ISfcHasConnection)this).ConnectionContext.Mode = SfcConnectionContextMode.Offline;
	}

	List<string> IAlienRoot.SfcHelper_GetSmoObjectQuery(string urn, string[] fields, OrderBy[] orderByFields)
	{
		return GetSmoObjectQuery(new Urn(urn), fields, orderByFields);
	}

	private List<string> GetSmoObjectQuery(string queryString, string[] fields, OrderBy[] orderByFields)
	{
		_ = Information.Edition;
		Urn urn = new Urn(queryString);
		XPathExpression xPathExpression = urn.XPathExpression;
		int length = xPathExpression.Length;
		if (length < 1)
		{
			throw new InternalSmoErrorException(ExceptionTemplatesImpl.CallingInitChildLevelWithWrongUrn(urn));
		}
		GetSmoObjectQueryRec(new Urn(queryString));
		List<string> list = null;
		try
		{
			string[] infrastructureFields = null;
			_ = xPathExpression[xPathExpression.Length - 1].Name;
			return InitQueryUrns(urn, fields, orderByFields, infrastructureFields);
		}
		catch (ArgumentException innerException)
		{
			throw new ArgumentException(ExceptionTemplatesImpl.UnsupportedObjectQueryUrn(queryString), innerException);
		}
	}

	private void GetSmoObjectQueryRec(Urn urn)
	{
		urn = urn.Parent;
		if (!(urn == null) && !(urn.Parent == null))
		{
			GetSmoObjectQueryRec(urn);
			XPathExpression xPathExpression = urn.XPathExpression;
			int length = xPathExpression.Length;
			Type childType = SqlSmoObject.GetChildType(xPathExpression[length - 1].Name, (length > 1) ? xPathExpression[length - 2].Name : GetType().Name);
			string[] queryTypeInfrastructureFields = SqlSmoObject.GetQueryTypeInfrastructureFields(childType);
			InitQueryUrns(urn, queryTypeInfrastructureFields, null, null);
		}
	}

	public SqlSmoObject GetSmoObject(Urn urn)
	{
		try
		{
			if (null == urn)
			{
				throw new ArgumentNullException("urn");
			}
			if ("Default" == urn.Type && "Column" == urn.Parent.Type)
			{
				Column column = GetSmoObjectRec(urn.Parent) as Column;
				return column.GetDefaultConstraintBaseByName(urn.GetAttribute("Name"));
			}
			if ("DatabaseOption" == urn.Type)
			{
				Database database = GetSmoObjectRec(urn.Parent) as Database;
				return database.DatabaseOptions;
			}
			return GetSmoObjectRec(urn);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.GetSmoObject, this, ex);
		}
	}

	private SqlSmoObject GetSmoObjectRec(Urn urn)
	{
		if (null == urn.Parent)
		{
			if (urn.Type == "Server")
			{
				CheckValidUrnServerLevel(urn.XPathExpression[0]);
				return this;
			}
			throw new SmoException("For the moment we don't support Urn's that do not start with Server");
		}
		SqlSmoObject smoObjectRec = GetSmoObjectRec(urn.Parent);
		string type = urn.Type;
		type = string.Intern(type);
		if (type == JobServer.UrnSuffix)
		{
			return ((Server)smoObjectRec).JobServer;
		}
		if (type == AlertSystem.UrnSuffix)
		{
			return ((JobServer)smoObjectRec).AlertSystem;
		}
		if (type == UserOptions.UrnSuffix)
		{
			return ((Server)smoObjectRec).UserOptions;
		}
		if (type == Information.UrnSuffix)
		{
			return ((Server)smoObjectRec).Information;
		}
		if (type == Settings.UrnSuffix)
		{
			return ((Server)smoObjectRec).Settings;
		}
		if (type == FullTextIndex.UrnSuffix)
		{
			return ((TableViewBase)smoObjectRec).FullTextIndex;
		}
		if (type == ServerActiveDirectory.UrnSuffix)
		{
			return ((Server)smoObjectRec).ActiveDirectory;
		}
		if (type == DatabaseActiveDirectory.UrnSuffix)
		{
			return ((Database)smoObjectRec).ActiveDirectory;
		}
		if (type == DefaultConstraint.UrnSuffix && smoObjectRec is Column)
		{
			return ((Column)smoObjectRec).DefaultConstraint;
		}
		if (type == DatabaseOptions.UrnSuffix)
		{
			return ((Database)smoObjectRec).DatabaseOptions;
		}
		if (type == SqlMail.UrnSuffix)
		{
			return ((Server)smoObjectRec).Mail;
		}
		if (type == SoapPayload.UrnSuffix)
		{
			return ((Endpoint)smoObjectRec).Payload.Soap;
		}
		if (type == DatabaseMirroringPayload.UrnSuffix)
		{
			return ((Endpoint)smoObjectRec).Payload.DatabaseMirroring;
		}
		if (type == ServiceBrokerPayload.UrnSuffix && smoObjectRec is Endpoint)
		{
			return ((Endpoint)smoObjectRec).Payload.ServiceBroker;
		}
		if (type == HttpProtocol.UrnSuffix)
		{
			return ((Endpoint)smoObjectRec).Protocol.Http;
		}
		if (type == TcpProtocol.UrnSuffix)
		{
			return ((Endpoint)smoObjectRec).Protocol.Tcp;
		}
		if (type == ServiceBroker.UrnSuffix)
		{
			return ((Database)smoObjectRec).ServiceBroker;
		}
		if (type == DatabaseEncryptionKey.UrnSuffix)
		{
			return ((Database)smoObjectRec).DatabaseEncryptionKey;
		}
		if (type == ServiceMasterKey.UrnSuffix && smoObjectRec is Server)
		{
			return ((Server)smoObjectRec).ServiceMasterKey;
		}
		if (type == MasterKey.UrnSuffix && smoObjectRec is Database)
		{
			return ((Database)smoObjectRec).MasterKey;
		}
		if (type == ResourceGovernor.UrnSuffix)
		{
			return ((Server)smoObjectRec).ResourceGovernor;
		}
		if (type == ServerProxyAccount.UrnSuffix)
		{
			return ((Server)smoObjectRec).ProxyAccount;
		}
		if (type == SmartAdmin.UrnSuffix)
		{
			return ((Server)smoObjectRec).SmartAdmin;
		}
		if (type == FullTextService.UrnSuffix)
		{
			return ((Server)smoObjectRec).FullTextService;
		}
		if (type == QueryStoreOptions.UrnSuffix)
		{
			return ((Database)smoObjectRec).QueryStoreOptions;
		}
		object childCollection = SqlSmoObject.GetChildCollection(smoObjectRec, urn.XPathExpression, urn.XPathExpression.Length - 1, base.ServerVersion);
		ObjectKeyBase objectKeyBase = ((AbstractCollectionBase)childCollection).CreateKeyFromUrn(urn);
		SqlSmoObject objectByKey = ((SmoCollectionBase)childCollection).GetObjectByKey(objectKeyBase);
		if (objectByKey == null)
		{
			throw new MissingObjectException(ExceptionTemplatesImpl.ObjectDoesNotExist(SqlSmoObject.GetTypeName(type), objectKeyBase.ToString()));
		}
		return objectByKey;
	}

	public void Alter()
	{
		overrideValueChecking = false;
		AlterImpl();
	}

	public void Alter(bool overrideValueChecking)
	{
		this.overrideValueChecking = overrideValueChecking;
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		if (m_config != null)
		{
			m_config.ScriptAlter(query, sp, overrideValueChecking);
		}
		if (affinityInfo != null)
		{
			affinityInfo.Alter();
		}
		ScriptProperties(query, sp);
	}

	public void DetachDatabase(string databaseName, bool updateStatistics)
	{
		try
		{
			if (databaseName == null)
			{
				throw new ArgumentNullException("databaseName");
			}
			DetachDatabaseWorker(databaseName, updateStatistics, emitFT: false, dropFulltextIndexFile: false);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DetachDatabase, this, ex);
		}
	}

	public void DetachDatabase(string databaseName, bool updateStatistics, bool removeFulltextIndexFile)
	{
		try
		{
			ThrowIfBelowVersion90();
			if (databaseName == null)
			{
				throw new ArgumentNullException("databaseName");
			}
			DetachDatabaseWorker(databaseName, updateStatistics, emitFT: true, removeFulltextIndexFile);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DetachDatabase, this, ex);
		}
	}

	private void DetachDatabaseWorker(string name, bool updateStatistics, bool emitFT, bool dropFulltextIndexFile)
	{
		if (!Databases.Contains(name))
		{
			throw new MissingObjectException(ExceptionTemplatesImpl.ObjectDoesNotExist(ExceptionTemplatesImpl.Database, name));
		}
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add("USE [master]");
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_detach_db @dbname = N'{0}'", new object[1] { SqlSmoObject.SqlString(name) });
		if (updateStatistics)
		{
			stringBuilder.Append(", @skipchecks = 'false'");
		}
		if (emitFT)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", @keepfulltextindexfile=N'{0}'", new object[1] { dropFulltextIndexFile ? "false" : "true" });
		}
		stringCollection.Add(stringBuilder.ToString());
		ExecutionManager.ExecuteNonQuery(stringCollection);
		Databases[name].MarkDroppedInternal();
		Databases.RemoveObject(new SimpleObjectKey(name));
		if (!ExecutionManager.Recording && !SmoApplication.eventsSingleton.IsNullDatabaseEvent())
		{
			Urn urn = new Urn(string.Concat(base.Urn, string.Format(SmoApplication.DefaultCulture, "/Database[@Name='{0}']", new object[1] { Urn.EscapeString(name) })));
			SmoApplication.eventsSingleton.CallDatabaseEvent(this, new DatabaseEventArgs(urn, null, name, DatabaseEventType.Detach));
		}
	}

	private void AttachDatabaseWorker(string name, StringCollection files, string owner, AttachOptions attachOptions)
	{
		StringCollection stringCollection = new StringCollection();
		if (files.Count < 1)
		{
			throw new ArgumentException(ExceptionTemplatesImpl.TooFewFiles);
		}
		if (Databases.Contains(name))
		{
			throw new SmoException(ExceptionTemplatesImpl.DatabaseAlreadyExists);
		}
		if (name.Length == 0)
		{
			throw new ArgumentException(ExceptionTemplatesImpl.EmptyInputParam("name", "string"));
		}
		if (owner != null && owner.Length == 0)
		{
			throw new ArgumentException(ExceptionTemplatesImpl.EmptyInputParam("owner", "string"));
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE DATABASE [{0}] ON ", new object[1] { SqlSmoObject.SqlBraket(name) });
		for (int i = 0; i < files.Count; i++)
		{
			string s = files[i];
			if (i != 0)
			{
				stringBuilder.Append(Globals.comma);
			}
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append(Globals.LParen);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " FILENAME = N'{0}' ", new object[1] { SqlSmoObject.SqlString(s) });
			stringBuilder.Append(Globals.RParen);
		}
		stringBuilder.Append(Globals.newline);
		if (base.ServerVersion.Major >= 9)
		{
			if (attachOptions == AttachOptions.RebuildLog)
			{
				stringBuilder.Append(" FOR ATTACH_REBUILD_LOG");
			}
			else
			{
				stringBuilder.Append(" FOR ATTACH");
				switch (attachOptions)
				{
				case AttachOptions.EnableBroker:
					stringBuilder.Append(" WITH ENABLE_BROKER");
					break;
				case AttachOptions.NewBroker:
					stringBuilder.Append(" WITH NEW_BROKER");
					break;
				case AttachOptions.ErrorBrokerConversations:
					stringBuilder.Append(" WITH ERROR_BROKER_CONVERSATIONS");
					break;
				default:
					throw new ArgumentException(ExceptionTemplatesImpl.UnknownEnumeration("AttachOptions"));
				case AttachOptions.None:
					break;
				}
			}
		}
		else
		{
			stringBuilder.Append(" FOR ATTACH");
		}
		stringCollection.Add(Scripts.USEMASTER);
		stringCollection.Add(stringBuilder.ToString());
		if (owner != null)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			if (base.ServerVersion.Major <= 8)
			{
				empty = "master.dbo.sysdatabases";
				empty2 = "sid";
			}
			else
			{
				empty = "master.sys.databases";
				empty2 = "owner_sid";
			}
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "if exists (select name from {0} sd where name = N'{1}' and SUSER_SNAME(sd.{2}) = SUSER_SNAME() ) EXEC [{3}].dbo.sp_changedbowner @loginame=N'{4}', @map=false", empty, SqlSmoObject.SqlString(name), empty2, SqlSmoObject.SqlBraket(name), SqlSmoObject.SqlString(owner)));
		}
		ExecutionManager.ExecuteNonQuery(stringCollection);
		Databases.InitializeChildObject(new SimpleObjectKey(name));
		if (!ExecutionManager.Recording && !SmoApplication.eventsSingleton.IsNullDatabaseEvent())
		{
			SmoApplication.eventsSingleton.CallDatabaseEvent(this, new DatabaseEventArgs(Databases[name].Urn, Databases[name], name, DatabaseEventType.Attach));
		}
	}

	public void AttachDatabase(string name, StringCollection files, string owner)
	{
		try
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (files == null)
			{
				throw new ArgumentNullException("files");
			}
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			AttachDatabaseWorker(name, files, owner, AttachOptions.None);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AttachDatabase, this, ex);
		}
	}

	public void AttachDatabase(string name, StringCollection files)
	{
		try
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (files == null)
			{
				throw new ArgumentNullException("files");
			}
			AttachDatabaseWorker(name, files, null, AttachOptions.None);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AttachDatabase, this, ex);
		}
	}

	public void AttachDatabase(string name, StringCollection files, AttachOptions attachOptions)
	{
		ThrowIfBelowVersion90();
		try
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (files == null)
			{
				throw new ArgumentNullException("files");
			}
			AttachDatabaseWorker(name, files, null, attachOptions);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AttachDatabase, this, ex);
		}
	}

	public void AttachDatabase(string name, StringCollection files, string owner, AttachOptions attachOptions)
	{
		ThrowIfBelowVersion90();
		try
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (files == null)
			{
				throw new ArgumentNullException("files");
			}
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			AttachDatabaseWorker(name, files, owner, attachOptions);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AttachDatabase, this, ex);
		}
	}

	private void CheckValidUrnServerLevel(XPathExpressionBlock xb)
	{
		if (xb.Name != "Server")
		{
			throw new SmoException(ExceptionTemplatesImpl.ServerLevelMustBePresent);
		}
		if (xb.Filter != null)
		{
			string attributeFromFilter = xb.GetAttributeFromFilter("Name");
			string trueServerName = ExecutionManager.TrueServerName;
			if (attributeFromFilter == null || trueServerName == null || string.Compare(attributeFromFilter, Urn.EscapeString(trueServerName), StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new SmoException(ExceptionTemplatesImpl.InvalidUrnServerLevel);
			}
		}
	}

	public int CompareUrn(Urn urn1, Urn urn2)
	{
		try
		{
			return CompareUrnWorker(urn1, urn2);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CompareUrn, this, ex);
		}
	}

	private int CompareUrnWorker(Urn urn1, Urn urn2)
	{
		if (null == urn1)
		{
			throw new ArgumentNullException("urn1");
		}
		if (null == urn2)
		{
			throw new ArgumentNullException("urn2");
		}
		XPathExpression xPathExpression = urn1.XPathExpression;
		XPathExpression xPathExpression2 = urn2.XPathExpression;
		if (xPathExpression.Length != xPathExpression2.Length)
		{
			return xPathExpression.Length - xPathExpression2.Length;
		}
		if (xPathExpression.Length == 0)
		{
			return -1;
		}
		if (xPathExpression[0].Name != "Server" || xPathExpression2[0].Name != "Server")
		{
			throw new SmoException(ExceptionTemplatesImpl.ServerLevelMustBePresent);
		}
		string empty = string.Empty;
		empty = ((xPathExpression[0].Filter != null) ? xPathExpression[0].GetAttributeFromFilter("Name") : Urn.EscapeString(ExecutionManager.TrueServerName));
		string empty2 = string.Empty;
		empty2 = ((xPathExpression2[0].Filter != null) ? xPathExpression2[0].GetAttributeFromFilter("Name") : Urn.EscapeString(ExecutionManager.TrueServerName));
		int num = string.Compare(empty, empty2, StringComparison.OrdinalIgnoreCase);
		if (num != 0)
		{
			return num;
		}
		if (xPathExpression.Length == 1)
		{
			return 0;
		}
		if (xPathExpression[1].Name == xPathExpression2[1].Name)
		{
			string attributeFromFilter = xPathExpression[1].GetAttributeFromFilter("Name");
			string attributeFromFilter2 = xPathExpression2[1].GetAttributeFromFilter("Name");
			int num2 = -1;
			num2 = base.StringComparer.Compare(attributeFromFilter, attributeFromFilter2);
			if (num2 != 0)
			{
				return num2;
			}
			string text = attributeFromFilter;
			if (xPathExpression.Length == 2)
			{
				return 0;
			}
			StringComparer stringComparer = base.StringComparer;
			if (xPathExpression[1].Name == "Database" && text != null && Databases[text] != null)
			{
				stringComparer = Databases[text].StringComparer;
			}
			for (int i = 2; i < xPathExpression.Length; i++)
			{
				if (xPathExpression[i].Name == xPathExpression2[i].Name)
				{
					string attributeFromFilter3 = xPathExpression[i].GetAttributeFromFilter("Name");
					string attributeFromFilter4 = xPathExpression2[i].GetAttributeFromFilter("Name");
					int num3 = stringComparer.Compare(attributeFromFilter3, attributeFromFilter4);
					if (num3 != 0)
					{
						return num3;
					}
					string attributeFromFilter5 = xPathExpression[i].GetAttributeFromFilter("Schema");
					string attributeFromFilter6 = xPathExpression2[i].GetAttributeFromFilter("Schema");
					if (attributeFromFilter5 != null && attributeFromFilter6 == null)
					{
						return stringComparer.Compare(attributeFromFilter5, string.Empty);
					}
					if (attributeFromFilter5 == null && attributeFromFilter6 != null)
					{
						return stringComparer.Compare(string.Empty, attributeFromFilter6);
					}
					if (attributeFromFilter5 != null && attributeFromFilter6 != null)
					{
						num3 = stringComparer.Compare(attributeFromFilter5, attributeFromFilter6);
					}
					if (num3 != 0)
					{
						return num3;
					}
					continue;
				}
				return string.Compare(xPathExpression[i].Name, xPathExpression2[i].Name);
			}
			return 0;
		}
		return string.Compare(xPathExpression[1].Name, xPathExpression2[1].Name);
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (m_Databases != null)
		{
			m_Databases.MarkAllDropped();
		}
		if (m_Languages != null)
		{
			m_Languages.MarkAllDropped();
		}
		if (m_Logins != null)
		{
			m_Logins.MarkAllDropped();
		}
		if (m_Roles != null)
		{
			m_Roles.MarkAllDropped();
		}
		if (m_LinkedServers != null)
		{
			m_LinkedServers.MarkAllDropped();
		}
		if (jobServer != null)
		{
			jobServer.MarkDroppedInternal();
		}
		if (m_BackupDevices != null)
		{
			m_BackupDevices.MarkAllDropped();
		}
		if (m_Settings != null)
		{
			m_Settings.MarkDroppedInternal();
		}
		if (m_Information != null)
		{
			m_Information.MarkDroppedInternal();
		}
		if (m_UserOption != null)
		{
			m_UserOption.MarkDroppedInternal();
		}
		if (systemDataTypes != null)
		{
			systemDataTypes.MarkAllDropped();
		}
		if (Endpoints != null)
		{
			Endpoints.MarkAllDropped();
		}
		if (auditCollection != null)
		{
			auditCollection.MarkAllDropped();
		}
		if (serverAuditSpecificationCollection != null)
		{
			serverAuditSpecificationCollection.MarkAllDropped();
		}
	}

	public DataTable EnumCollations()
	{
		try
		{
			if (collations == null)
			{
				Request request = new Request(base.Urn.Value + "/Collation");
				request.OrderByList = new OrderBy[1]
				{
					new OrderBy("Name", OrderBy.Direction.Asc)
				};
				collations = ExecutionManager.GetEnumeratorData(request);
			}
			return collations;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumCollations, this, ex);
		}
	}

	internal CollationVersion GetCollationVersion(string collationName)
	{
		if (collationVersionDictionary == null)
		{
			InitializeCollationVersionDictionary();
		}
		if (collationVersionDictionary.ContainsKey(collationName))
		{
			return collationVersionDictionary[collationName];
		}
		CollationVersion collationVersion = FindCollationVersion(collationName);
		collationVersionDictionary.Add(collationName, collationVersion);
		return collationVersion;
	}

	private void InitializeCollationVersionDictionary()
	{
		collationVersionDictionary = new Dictionary<string, CollationVersion>(System.StringComparer.OrdinalIgnoreCase);
	}

	private CollationVersion FindCollationVersion(string collationName)
	{
		SqlExecutionModes sqlExecutionModes = ExecutionManager.ConnectionContext.SqlExecutionModes;
		if (ExecutionManager.ConnectionContext.SqlExecutionModes == SqlExecutionModes.CaptureSql)
		{
			ExecutionManager.ConnectionContext.SqlExecutionModes = SqlExecutionModes.ExecuteSql;
		}
		DataTable dataTable;
		try
		{
			dataTable = ((base.ServerVersion.Major >= 9) ? ExecutionManager.ExecuteWithResults("SELECT COLLATIONPROPERTY('" + collationName + "', 'Version') as CollationVersion").Tables[0] : ExecutionManager.ExecuteWithResults("SELECT CollationVersion = CASE WHEN COLLATIONPROPERTY('" + collationName + "', 'lcid') IS NOT NULL THEN '0' END").Tables[0]);
		}
		finally
		{
			ExecutionManager.ConnectionContext.SqlExecutionModes = sqlExecutionModes;
		}
		DataRow dataRow = dataTable.Rows[0];
		if (!string.IsNullOrEmpty(dataRow["CollationVersion"].ToString()))
		{
			int value = int.Parse(dataRow["CollationVersion"].ToString(), SmoApplication.DefaultCulture);
			return (CollationVersion)Enum.ToObject(typeof(CollationVersion), value);
		}
		throw new WrongPropertyValueException(ExceptionTemplatesImpl.InvalidCollation(collationName));
	}

	internal DataTable EnumPerfInfoInternal(string objectName, string counterName, string instanceName)
	{
		ThrowIfCloud();
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		if (objectName != null)
		{
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "@ObjectName = '{0}'", new object[1] { Urn.EscapeString(objectName) });
			flag = true;
		}
		if (counterName != null && flag)
		{
			stringBuilder.Append(" and ");
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "@CounterName = '{0}'", new object[1] { Urn.EscapeString(counterName) });
			flag = true;
		}
		if (instanceName != null && flag)
		{
			stringBuilder.Append(" and ");
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
				throw new ArgumentNullException("objectName");
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
				throw new ArgumentNullException("objectName");
			}
			if (counterName == null)
			{
				throw new ArgumentNullException("counterName");
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
				throw new ArgumentNullException("objectName");
			}
			if (counterName == null)
			{
				throw new ArgumentNullException("counterName");
			}
			if (instanceName == null)
			{
				throw new ArgumentNullException("instanceName");
			}
			return EnumPerfInfoInternal(objectName, counterName, instanceName);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumPerformanceCounters, this, ex);
		}
	}

	public DataTable EnumDatabaseMirrorWitnessRoles()
	{
		ThrowIfBelowVersion90();
		ThrowIfCloud();
		try
		{
			Request req = new Request(base.Urn.Value + "/DatabaseMirroringWitnessRole");
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumDatabaseMirrorWitnessRoles, this, ex);
		}
	}

	public DataTable EnumDatabaseMirrorWitnessRoles(string database)
	{
		ThrowIfBelowVersion90();
		ThrowIfCloud();
		if (database == null)
		{
			throw new ArgumentNullException("database");
		}
		try
		{
			Request req = new Request(base.Urn.Value + "/DatabaseMirroringWitnessRole[@Database='" + Urn.EscapeString(database) + "']");
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumDatabaseMirrorWitnessRoles, this, ex);
		}
	}

	public DataTable EnumErrorLogs()
	{
		ThrowIfCloud();
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
		return ReadErrorLog(0);
	}

	public DataTable ReadErrorLog(int logNumber)
	{
		ThrowIfCloud();
		try
		{
			new StringCollection();
			Request request = new Request(string.Format(SmoApplication.DefaultCulture, "{0}/ErrorLog[@ArchiveNo='{1}']/LogEntry", new object[2] { base.Urn, logNumber }));
			request.Fields = new string[3] { "LogDate", "ProcessInfo", "Text" };
			request.OrderByList = new OrderBy[1]
			{
				new OrderBy("LogDate", OrderBy.Direction.Asc)
			};
			return ExecutionManager.GetEnumeratorData(request);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ReadErrorLog, this, ex);
		}
	}

	public void KillDatabase(string database)
	{
		ThrowIfCloud();
		try
		{
			if (database == null)
			{
				throw new ArgumentNullException("database");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(Scripts.USEMASTER);
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE", new object[1] { SqlSmoObject.MakeSqlBraket(database) }));
			try
			{
				ExecutionManager.ExecuteNonQuery(stringCollection);
			}
			catch
			{
			}
			Databases[database].Drop();
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.KillDatabase, this, ex);
		}
	}

	public void KillProcess(int processId)
	{
		ThrowIfCloud();
		try
		{
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(Scripts.USEMASTER);
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "KILL {0}", new object[1] { processId }));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.KillProcess, this, ex);
		}
	}

	public int GetActiveDBConnectionCount(string dbName)
	{
		ThrowIfCloud();
		try
		{
			if (dbName == null)
			{
				throw new ArgumentNullException("dbName");
			}
			string query = string.Format(SmoApplication.DefaultCulture, "select count(*) from master.dbo.sysprocesses where dbid=db_id(N'{0}')", new object[1] { SqlSmoObject.SqlString(dbName) });
			return (int)ExecutionManager.ExecuteScalar(query);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.GetActiveDBConnectionCount, this, ex);
		}
	}

	public void KillAllProcesses(string databaseName)
	{
		ThrowIfCloud();
		try
		{
			if (databaseName == null)
			{
				throw new ArgumentNullException("databaseName");
			}
			if (ConnectionContext.SqlExecutionModes == SqlExecutionModes.CaptureSql)
			{
				return;
			}
			ExecutionManager.ExecuteNonQuery(Scripts.USEMASTER);
			DataTable dataTable = null;
			string text = null;
			if (base.ServerVersion.Major == 8)
			{
				dataTable = ExecutionManager.ExecuteWithResults(string.Format(SmoApplication.DefaultCulture, "SELECT DISTINCT req_spid FROM master.dbo.syslockinfo WHERE rsc_type = 2 AND rsc_dbid = db_id('{0}') AND req_spid > 50", new object[1] { SqlSmoObject.SqlString(databaseName) })).Tables[0];
				text = "req_spid";
			}
			else
			{
				dataTable = ExecutionManager.ExecuteWithResults(string.Format(SmoApplication.DefaultCulture, "SELECT DISTINCT request_session_id FROM master.sys.dm_tran_locks WHERE resource_type = 'DATABASE' AND resource_database_id = db_id(N'{0}') and request_session_id > 50", new object[1] { SqlSmoObject.SqlString(databaseName) })).Tables[0];
				text = "request_session_id";
			}
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != dataTable, "null == spids");
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != text, "null == spidColumn");
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(Scripts.USEMASTER);
			foreach (DataRow row in dataTable.Rows)
			{
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "KILL {0}", new object[1] { Convert.ToInt32(row[text], SmoApplication.DefaultCulture) }));
			}
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DropAllActiveDBConnections, this, ex);
		}
	}

	public DataTable EnumDirectories(string path)
	{
		ThrowIfCloud();
		try
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			Request request = new Request(base.Urn.Value + string.Format(SmoApplication.DefaultCulture, "/File[@Path='{0}']", new object[1] { Urn.EscapeString(path) }));
			request.Fields = new string[2] { "Name", "IsFile" };
			DataTable enumeratorData = ExecutionManager.GetEnumeratorData(request);
			ArrayList arrayList = new ArrayList();
			foreach (DataRow row in enumeratorData.Rows)
			{
				if ((bool)row[1])
				{
					arrayList.Add(row);
				}
			}
			foreach (DataRow item in arrayList)
			{
				item.Delete();
			}
			enumeratorData.Columns.Remove(enumeratorData.Columns[1]);
			enumeratorData.AcceptChanges();
			return enumeratorData;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumDirectories, this, ex);
		}
	}

	public DataTable EnumLocks()
	{
		ThrowIfCloud();
		try
		{
			Request req = new Request(base.Urn.Value + "/Lock");
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumLocks, this, ex);
		}
	}

	public DataTable EnumLocks(int processId)
	{
		ThrowIfCloud();
		try
		{
			Request req = new Request(base.Urn.Value + string.Format(SmoApplication.DefaultCulture, "/Lock[@RequestorSpid={0}]", new object[1] { processId }));
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumLocks, this, ex);
		}
	}

	public DataTable EnumWindowsDomainGroups()
	{
		ThrowIfCloud();
		try
		{
			Request req = new Request(base.Urn.Value + "/NTGroup");
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumWindowsDomainGroups, this, ex);
		}
	}

	public DataTable EnumWindowsDomainGroups(string domain)
	{
		ThrowIfCloud();
		try
		{
			if (domain == null)
			{
				throw new ArgumentNullException("domain");
			}
			Request req = new Request(base.Urn.Value + string.Format(SmoApplication.DefaultCulture, "/NTGroup[@Domain='{0}']", new object[1] { Urn.EscapeString(domain) }));
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumWindowsDomainGroups, this, ex);
		}
	}

	public DataTable EnumProcesses()
	{
		ThrowIfCloud();
		try
		{
			Request req = new Request(base.Urn.Value + "/Process");
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumProcesses, this, ex);
		}
	}

	public DataTable EnumProcesses(int processId)
	{
		ThrowIfCloud();
		try
		{
			Request req = new Request(base.Urn.Value + string.Format(SmoApplication.DefaultCulture, "/Process[@Spid={0}]", new object[1] { processId }));
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumProcesses, this, ex);
		}
	}

	public DataTable EnumProcesses(bool excludeSystemProcesses)
	{
		ThrowIfCloud();
		try
		{
			string text = string.Empty;
			if (excludeSystemProcesses)
			{
				text = "[@IsSystem = false()]";
			}
			Request req = new Request(base.Urn.Value + "/Process" + text);
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumProcesses, this, ex);
		}
	}

	public DataTable EnumProcesses(string loginName)
	{
		ThrowIfCloud();
		try
		{
			if (loginName == null)
			{
				throw new ArgumentNullException("loginName");
			}
			Request req = new Request(base.Urn.Value + string.Format(SmoApplication.DefaultCulture, "/Process[@Login='{0}']", new object[1] { Urn.EscapeString(loginName) }));
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumProcesses, this, ex);
		}
	}

	public DataTable EnumStartupProcedures()
	{
		try
		{
			Request request = new Request(base.Urn.Value + "/Database[@Name='master']/StoredProcedure[@Startup = true()]");
			request.Fields = new string[2] { "Name", "Schema" };
			return ExecutionManager.GetEnumeratorData(request);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumStartupProcedures, this, ex);
		}
	}

	public void SetTraceFlag(int number, bool isOn)
	{
		ThrowIfCloud();
		if (isOn)
		{
			ExecutionManager.ExecuteNonQuery($"DBCC TRACEON ({number})");
		}
		else
		{
			ExecutionManager.ExecuteNonQuery($"DBCC TRACEOFF ({number})");
		}
	}

	public bool IsTraceFlagOn(int traceFlag, bool isGlobalTraceFlag)
	{
		DataTable dataTable = (isGlobalTraceFlag ? EnumActiveGlobalTraceFlags() : EnumActiveCurrentSessionTraceFlags());
		return dataTable.Rows.Cast<DataRow>().Any(delegate(DataRow row)
		{
			int num = Convert.ToInt32(row[0]);
			bool flag = Convert.ToBoolean(row[1]);
			bool flag2 = Convert.ToBoolean(row[2]);
			bool flag3 = Convert.ToBoolean(row[3]);
			return traceFlag == num && flag && ((isGlobalTraceFlag && flag2) || flag3);
		});
	}

	public DataTable EnumActiveGlobalTraceFlags()
	{
		ThrowIfCloud();
		DataSet dataSet = null;
		dataSet = ExecutionManager.ExecuteWithResults("CREATE TABLE #tracestatus (TraceFlag INT, Status INT, Global INT, Session INT)\nINSERT INTO #tracestatus EXEC ('DBCC TRACESTATUS (-1) WITH NO_INFOMSGS')\nSELECT * FROM #tracestatus\nDROP TABLE #tracestatus");
		return dataSet.Tables[0];
	}

	public DataTable EnumActiveCurrentSessionTraceFlags()
	{
		ThrowIfBelowVersion90();
		ThrowIfCloud();
		DataSet dataSet = null;
		dataSet = ExecutionManager.ExecuteWithResults("CREATE TABLE #tracestatus (TraceFlag INT, Status INT, Global INT, Session INT)\nINSERT INTO #tracestatus EXEC ('DBCC TRACESTATUS () WITH NO_INFOMSGS')\nSELECT * FROM #tracestatus\nDROP TABLE #tracestatus");
		return dataSet.Tables[0];
	}

	internal DataTable EnumAccountInfo(string arguments, string filter)
	{
		ThrowIfCloud();
		if ("" != filter)
		{
			filter = "where " + filter;
		}
		return ExecutionManager.ExecuteWithResults("create table #t1 ([account name] sysname, type nvarchar(10), privilege nvarchar(10), [mapped login name] sysname, [permission path] nvarchar(512))\n" + string.Format(SmoApplication.DefaultCulture, "insert into #t1 exec xp_logininfo {0}\n", new object[1] { arguments }) + string.Format(SmoApplication.DefaultCulture, "select * from #t1 {0}\n", new object[1] { filter }) + "drop table #t1").Tables[0];
	}

	public DataTable EnumWindowsUserInfo()
	{
		ThrowIfCloud();
		try
		{
			return EnumAccountInfo("", "type = 'user'");
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumWindowsUserInfo, this, ex);
		}
	}

	public DataTable EnumWindowsUserInfo(string account)
	{
		ThrowIfCloud();
		try
		{
			return EnumWindowsUserInfo(account, listPermissionPaths: false);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumWindowsUserInfo, this, ex);
		}
	}

	public DataTable EnumWindowsUserInfo(string account, bool listPermissionPaths)
	{
		ThrowIfCloud();
		try
		{
			if (account == null)
			{
				throw new ArgumentNullException("account");
			}
			if (listPermissionPaths)
			{
				return EnumAccountInfo(string.Format(SmoApplication.DefaultCulture, "N'{0}', N'all'", new object[1] { SqlSmoObject.SqlString(account) }), "");
			}
			return EnumAccountInfo(string.Format(SmoApplication.DefaultCulture, "N'{0}'", new object[1] { SqlSmoObject.SqlString(account) }), "");
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumWindowsUserInfo, this, ex);
		}
	}

	public DataTable EnumWindowsGroupInfo()
	{
		ThrowIfCloud();
		try
		{
			return EnumAccountInfo("", "type = 'group'");
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumWindowsGroupInfo, this, ex);
		}
	}

	public DataTable EnumWindowsGroupInfo(string group)
	{
		ThrowIfCloud();
		try
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			return EnumWindowsGroupInfo(group, listMembers: false);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumWindowsGroupInfo, this, ex);
		}
	}

	public DataTable EnumWindowsGroupInfo(string group, bool listMembers)
	{
		ThrowIfCloud();
		try
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			if (listMembers)
			{
				return EnumAccountInfo(string.Format(SmoApplication.DefaultCulture, "N'{0}', N'members'", new object[1] { SqlSmoObject.SqlString(group) }), "");
			}
			return EnumAccountInfo(string.Format(SmoApplication.DefaultCulture, "N'{0}'", new object[1] { SqlSmoObject.SqlString(group) }), "");
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumWindowsGroupInfo, this, ex);
		}
	}

	public DataTable EnumAvailableMedia()
	{
		ThrowIfCloud();
		try
		{
			return EnumAvailableMedia(MediaTypes.All);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumAvailableMedia, this, ex);
		}
	}

	public DataTable EnumAvailableMedia(MediaTypes media)
	{
		ThrowIfCloud();
		try
		{
			string text = string.Empty;
			int num = (int)media;
			if (MediaTypes.SharedFixedDisk == (MediaTypes.SharedFixedDisk & media))
			{
				text = "@SharedDrive = true()";
				num -= 16;
			}
			if (num != 0)
			{
				if (text.Length > 0)
				{
					text += " and ";
				}
				text += string.Format(SmoApplication.DefaultCulture, "0 != BitWiseAnd(@MediaTypes, {0})", new object[1] { num });
			}
			Request request = ((text.Length <= 0) ? new Request(base.Urn.Value + string.Format(SmoApplication.DefaultCulture, "/AvailableMedia")) : new Request(base.Urn.Value + string.Format(SmoApplication.DefaultCulture, "/AvailableMedia[{0}]", new object[1] { text })));
			request.Fields = new string[4] { "Name", "LowFree", "HighFree", "MediaTypes" };
			return ExecutionManager.GetEnumeratorData(request);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumAvailableMedia, this, ex);
		}
	}

	public DataTable EnumServerAttributes()
	{
		ThrowIfCloud();
		try
		{
			return ExecutionManager.ExecuteWithResults("EXEC master.dbo.sp_server_info").Tables[0];
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumServerAttributes, this, ex);
		}
	}

	protected override void CleanObject()
	{
		base.CleanObject();
		if (m_config != null)
		{
			m_config.CleanObject();
		}
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		List<PropagateInfo> list = new List<PropagateInfo>();
		list.Add(new PropagateInfo(Settings));
		List<PropagateInfo> list2 = list;
		if (this.IsSupportedObject<UserOptions>())
		{
			list2.Add(new PropagateInfo(UserOptions));
		}
		return list2.ToArray();
	}

	public StringCollection GetDefaultInitFields(Type typeObject)
	{
		return GetDefaultInitFields(typeObject, DatabaseEngineEdition);
	}

	public StringCollection GetDefaultInitFields(Type typeObject, DatabaseEngineEdition databaseEngineEdition)
	{
		if (!typeObject.IsSubclassOf(typeof(SqlSmoObject)) || !typeObject.GetIsSealed())
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.CannotSetDefInitFlds(typeObject.Name)).SetHelpContext("CannotSetDefInitFlds");
		}
		StringCollection stringCollection = new StringCollection();
		string[] defaultInitFieldsInternal = GetDefaultInitFieldsInternal(typeObject, databaseEngineEdition);
		if (defaultInitFieldsInternal != null)
		{
			int num = 1;
			if (defaultInitFieldsInternal.Length > 1)
			{
				num = ((!(defaultInitFieldsInternal[0] == "Schema")) ? 1 : 2);
			}
			for (int i = num; i < defaultInitFieldsInternal.Length; i++)
			{
				stringCollection.Add(defaultInitFieldsInternal[i]);
			}
		}
		return stringCollection;
	}

	public void SetDefaultInitFields(Type typeObject, StringCollection fields)
	{
		SetDefaultInitFields(typeObject, fields, DatabaseEngineEdition);
	}

	public void SetDefaultInitFields(Type typeObject, StringCollection fields, DatabaseEngineEdition databaseEngineEdition)
	{
		if (null == typeObject)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.SetDefaultInitFields, this, new ArgumentNullException("typeObject"));
		}
		if (fields == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.SetDefaultInitFields, this, new ArgumentNullException("fields"));
		}
		if (!typeObject.IsSubclassOf(typeof(SqlSmoObject)) || !typeObject.GetIsSealed())
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.CannotSetDefInitFlds(typeObject.Name)).SetHelpContext("CannotSetDefInitFlds");
		}
		Hashtable hashtable = new Hashtable();
		HashInitFields[typeObject] = hashtable;
		StringCollection stringCollection = CreateInitFieldsColl(typeObject);
		CollInitFields[typeObject] = stringCollection;
		StringEnumerator enumerator = stringCollection.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				hashtable[current] = current;
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		StringEnumerator enumerator2 = fields.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				string current2 = enumerator2.Current;
				if (current2 == "DataType")
				{
					hashtable["DataType"] = "DataType";
					hashtable["SystemType"] = "SystemType";
					hashtable["Length"] = "Length";
					hashtable["NumericPrecision"] = "NumericPrecision";
					hashtable["NumericScale"] = "NumericScale";
					if (base.ServerVersion.Major > 8 && databaseEngineEdition != DatabaseEngineEdition.SqlDataWarehouse)
					{
						hashtable["XmlSchemaNamespace"] = "XmlSchemaNamespace";
						hashtable["XmlSchemaNamespaceSchema"] = "XmlSchemaNamespaceSchema";
						hashtable["XmlDocumentConstraint"] = "XmlDocumentConstraint";
					}
					hashtable["DataTypeSchema"] = "DataTypeSchema";
					stringCollection.Add("DataType");
					stringCollection.Add("SystemType");
					stringCollection.Add("Length");
					stringCollection.Add("NumericPrecision");
					stringCollection.Add("NumericScale");
					if (base.ServerVersion.Major > 8 && databaseEngineEdition != DatabaseEngineEdition.SqlDataWarehouse)
					{
						stringCollection.Add("XmlSchemaNamespace");
						stringCollection.Add("XmlSchemaNamespaceSchema");
						stringCollection.Add("XmlDocumentConstraint");
					}
					stringCollection.Add("DataTypeSchema");
				}
				if (!hashtable.ContainsKey(current2))
				{
					hashtable[current2] = current2;
					stringCollection.Add(current2);
				}
			}
		}
		finally
		{
			if (enumerator2 is IDisposable disposable2)
			{
				disposable2.Dispose();
			}
		}
	}

	public void SetDefaultInitFields(Type typeObject, params string[] fields)
	{
		SetDefaultInitFields(typeObject, DatabaseEngineEdition, fields);
	}

	public void SetDefaultInitFields(Type typeObject, DatabaseEngineEdition databaseEngineEdition, params string[] fields)
	{
		if (fields == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.SetDefaultInitFields, this, new ArgumentNullException("fields"));
		}
		StringCollection stringCollection = new StringCollection();
		stringCollection.AddRange(fields);
		SetDefaultInitFields(typeObject, stringCollection, databaseEngineEdition);
	}

	public StringCollection GetPropertyNames(Type typeObject, DatabaseEngineEdition databaseEngineEdition)
	{
		if (null == typeObject)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.GetPropertyNames, this, new ArgumentNullException("typeObject"));
		}
		if (!typeObject.IsSubclassOf(typeof(SqlSmoObject)) || !typeObject.GetIsSealed())
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.CannotSetDefInitFlds(typeObject.Name)).SetHelpContext("CannotSetDefInitFlds");
		}
		Type nestedType = typeObject.GetNestedType("PropertyMetadataProvider", BindingFlags.NonPublic);
		if (null == nestedType)
		{
			return null;
		}
		Type[] types = new Type[3]
		{
			typeof(ServerVersion),
			typeof(DatabaseEngineType),
			typeof(DatabaseEngineEdition)
		};
		ConstructorInfo constructor = nestedType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, CallingConventions.HasThis, types, null);
		if (null == constructor)
		{
			return null;
		}
		if (!(constructor.Invoke(new object[3] { base.ServerVersion, DatabaseEngineType, databaseEngineEdition }) is SqlPropertyMetadataProvider sqlPropertyMetadataProvider))
		{
			return null;
		}
		StringCollection stringCollection = new StringCollection();
		for (int i = 0; i < sqlPropertyMetadataProvider.Count; i++)
		{
			stringCollection.Add(sqlPropertyMetadataProvider.GetStaticMetadata(i).Name);
		}
		return stringCollection;
	}

	public void SetDefaultInitFields(Type typeObject, bool allFields)
	{
		SetDefaultInitFields(typeObject, allFields, DatabaseEngineEdition);
	}

	public void SetDefaultInitFields(Type typeObject, bool allFields, DatabaseEngineEdition databaseEngineEdition)
	{
		StringCollection fields = new StringCollection();
		if (allFields)
		{
			fields = GetPropertyNames(typeObject, databaseEngineEdition);
		}
		SetDefaultInitFields(typeObject, fields, databaseEngineEdition);
	}

	public void SetDefaultInitFields(bool allFields)
	{
		CollInitFields.Clear();
		useAllFieldsForInit = allFields;
	}

	internal bool IsInitField(Type typeObject, string fieldName)
	{
		if (fieldName == "Schema" || fieldName == "Name")
		{
			return true;
		}
		if (HashInitFields[typeObject] is Hashtable hashtable)
		{
			return hashtable.Contains(fieldName);
		}
		return false;
	}

	internal string[] GetDefaultInitFieldsInternal(Type typeObject, DatabaseEngineEdition databaseEngineEdition)
	{
		StringCollection stringCollection = CollInitFields[typeObject] as StringCollection;
		if (stringCollection == null)
		{
			stringCollection = CreateInitFieldsColl(typeObject);
			if (useAllFieldsForInit)
			{
				Hashtable hashtable = new Hashtable();
				StringEnumerator enumerator = stringCollection.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						hashtable.Add(current, null);
					}
				}
				finally
				{
					if (enumerator is IDisposable disposable)
					{
						disposable.Dispose();
					}
				}
				StringEnumerator enumerator2 = GetPropertyNames(typeObject, databaseEngineEdition).GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						string current2 = enumerator2.Current;
						try
						{
							hashtable.Add(current2, null);
							stringCollection.Add(current2);
						}
						catch (ArgumentException)
						{
						}
					}
				}
				finally
				{
					if (enumerator2 is IDisposable disposable2)
					{
						disposable2.Dispose();
					}
				}
			}
			CollInitFields[typeObject] = stringCollection;
		}
		string[] array = new string[stringCollection.Count];
		stringCollection.CopyTo(array, 0);
		return array;
	}

	private StringCollection CreateInitFieldsColl(Type typeObject)
	{
		StringCollection stringCollection = new StringCollection();
		if (typeObject.IsSubclassOf(typeof(ScriptSchemaObjectBase)))
		{
			stringCollection.Add("Schema");
			stringCollection.Add("Name");
			if (typeObject.IsSubclassOf(typeof(TableViewBase)))
			{
				stringCollection.Add("ID");
			}
		}
		else if (typeObject == typeof(NumberedStoredProcedure))
		{
			stringCollection.Add("Number");
		}
		else if (SqlSmoObject.IsOrderedByID(typeObject))
		{
			stringCollection.Add("ID");
			stringCollection.Add("Name");
		}
		else if (typeObject.IsSubclassOf(typeof(MessageObjectBase)))
		{
			stringCollection.Add("ID");
			stringCollection.Add("Language");
		}
		else if (typeObject.IsSubclassOf(typeof(SoapMethodObject)))
		{
			stringCollection.Add("Namespace");
			stringCollection.Add("Name");
		}
		else if (typeObject.IsSubclassOf(typeof(ScheduleBase)))
		{
			stringCollection.Add("Name");
			stringCollection.Add("ID");
		}
		else if (typeObject == typeof(Job))
		{
			stringCollection.Add("Name");
			stringCollection.Add("CategoryID");
			stringCollection.Add("JobID");
		}
		else if (typeObject.IsSubclassOf(typeof(NamedSmoObject)))
		{
			stringCollection.Add("Name");
		}
		else if (typeObject == typeof(PhysicalPartition))
		{
			stringCollection.Add("PartitionNumber");
		}
		else if (typeObject == typeof(DatabaseReplicaState))
		{
			stringCollection.Add("AvailabilityReplicaServerName");
			stringCollection.Add("AvailabilityDatabaseName");
		}
		else if (typeObject == typeof(AvailabilityGroupListenerIPAddress))
		{
			stringCollection.Add("IPAddress");
			stringCollection.Add("SubnetMask");
			stringCollection.Add("SubnetIP");
		}
		else if (typeObject == typeof(SecurityPredicate))
		{
			stringCollection.Add("SecurityPredicateID");
		}
		else if (typeObject == typeof(ColumnEncryptionKeyValue))
		{
			stringCollection.Add("ColumnMasterKeyID");
		}
		if (typeObject == typeof(Column))
		{
			stringCollection.Add("DefaultConstraintName");
		}
		if (typeObject == typeof(DefaultConstraint))
		{
			stringCollection.Add("IsFileTableDefined");
		}
		if (typeObject == typeof(Information))
		{
			stringCollection.Add("Edition");
		}
		return stringCollection;
	}

	internal string[] GetScriptInitFieldsInternal(Type childType, Type parentType, ScriptingPreferences sp, DatabaseEngineEdition databaseEngineEdition)
	{
		string[] scriptInitFieldsInternal = GetScriptInitFieldsInternal2(childType, parentType, sp, databaseEngineEdition);
		return AddNecessaryFields(childType, scriptInitFieldsInternal);
	}

	private static string[] AddNecessaryFields(Type childType, string[] res2)
	{
		if (typesToIgnore.Contains(childType.Name))
		{
			return res2;
		}
		bool flag = childType.IsSubclassOf(typeof(ScriptSchemaObjectBase));
		bool flag2 = SqlSmoObject.IsOrderedByID(childType);
		int num = ((!flag && !flag2) ? 1 : 2);
		string[] array = new string[res2.Length + num];
		if (num == 2)
		{
			array[0] = (flag ? "Schema" : "ID");
		}
		array[num - 1] = "Name";
		res2.CopyTo(array, num);
		return array;
	}

	internal string[] GetScriptInitFieldsInternal2(Type childType, Type parentType, ScriptingPreferences sp, DatabaseEngineEdition databaseEngineEdition)
	{
		MethodInfo method = childType.GetMethod("GetScriptFields", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		if (method != null)
		{
			return method.Invoke(null, new object[5]
			{
				parentType,
				base.ServerVersion,
				DatabaseEngineType,
				databaseEngineEdition,
				DefaultTextMode && !sp.OldOptions.EnforceScriptingPreferences
			}) as string[];
		}
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != method, childType.Name + " is missing GetScriptFields method!");
		return new string[0];
	}

	internal bool GetScriptInitExpensiveFieldsInternal(Type childType, Type parentType, ScriptingPreferences sp, out string[] fields, DatabaseEngineEdition databaseEngineEdition)
	{
		string[] scriptInitExpensiveFieldsInternal = GetScriptInitExpensiveFieldsInternal2(childType, parentType, sp, databaseEngineEdition);
		if (scriptInitExpensiveFieldsInternal.Length == 0)
		{
			fields = scriptInitExpensiveFieldsInternal;
			return false;
		}
		fields = AddNecessaryFields(childType, scriptInitExpensiveFieldsInternal);
		return true;
	}

	private string[] GetScriptInitExpensiveFieldsInternal2(Type childType, Type parentType, ScriptingPreferences sp, DatabaseEngineEdition databaseEngineEdition)
	{
		MethodInfo method = childType.GetMethod("GetScriptFields2", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		if (method != null)
		{
			return method.Invoke(null, new object[6]
			{
				parentType,
				base.ServerVersion,
				DatabaseEngineType,
				databaseEngineEdition,
				DefaultTextMode && !sp.OldOptions.EnforceScriptingPreferences,
				sp
			}) as string[];
		}
		return new string[0];
	}

	internal ExecutionManager GetExecutionManager()
	{
		if (m_ExecutionManager == null)
		{
			lock (syncRoot)
			{
				if (m_ExecutionManager == null)
				{
					if (IsAzureDbScopedConnection(serverConnection))
					{
						ServerConnection databaseConnection = serverConnection.GetDatabaseConnection("master");
						try
						{
							databaseConnection.Connect();
							serverConnection = databaseConnection;
							databaseConnection.Disconnect();
						}
						catch (ConnectionFailureException)
						{
						}
					}
					m_ExecutionManager = new ExecutionManager(serverConnection);
					m_ExecutionManager.Parent = this;
				}
			}
		}
		return m_ExecutionManager;
	}

	public void DeleteBackupHistory(DateTime oldestDate)
	{
		ThrowIfCloud();
		try
		{
			ExecutionManager.ExecuteNonQuery(string.Format(SmoApplication.DefaultCulture, "declare @dt datetime select @dt = cast(N'{0}' as datetime) exec msdb.dbo.sp_delete_backuphistory @dt", new object[1] { SqlSmoObject.SqlDateString(oldestDate) }));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DeleteBackupHistory, this, ex);
		}
	}

	public DataTable DetachedDatabaseInfo(string mdfName)
	{
		ThrowIfCloud();
		if (mdfName == null)
		{
			throw new ArgumentNullException("mdfName");
		}
		try
		{
			CheckObjectState();
			return ExecutionManager.GetEnumeratorData(new Request("Server/PrimaryFile[@Name='" + Urn.EscapeString(mdfName) + "']", new string[2] { "Property", "Value" }));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DetachedDatabaseInfo, this, ex);
		}
	}

	public StringCollection EnumDetachedDatabaseFiles(string mdfName)
	{
		ThrowIfCloud();
		if (mdfName == null)
		{
			throw new ArgumentNullException("mdfName");
		}
		try
		{
			CheckObjectState();
			DataTable enumeratorData = ExecutionManager.GetEnumeratorData(new Request("Server/PrimaryFile[@Name='" + Urn.EscapeString(mdfName) + "']/File[@IsFile=true()]", new string[1] { "FileName" }));
			StringCollection stringCollection = new StringCollection();
			foreach (DataRow row in enumeratorData.Rows)
			{
				stringCollection.Add((string)row["FileName"]);
			}
			return stringCollection;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumDetachedDatabaseFiles, this, ex);
		}
	}

	public StringCollection EnumDetachedLogFiles(string mdfName)
	{
		ThrowIfCloud();
		if (mdfName == null)
		{
			throw new ArgumentNullException("mdfName");
		}
		try
		{
			CheckObjectState();
			DataTable enumeratorData = ExecutionManager.GetEnumeratorData(new Request("Server/PrimaryFile[@Name='" + Urn.EscapeString(mdfName) + "']/File[@IsFile=false()]", new string[1] { "FileName" }));
			StringCollection stringCollection = new StringCollection();
			foreach (DataRow row in enumeratorData.Rows)
			{
				stringCollection.Add((string)row["FileName"]);
			}
			return stringCollection;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumDetachedLogFiles, this, ex);
		}
	}

	public bool IsDetachedPrimaryFile(string mdfName)
	{
		ThrowIfCloud();
		if (mdfName == null)
		{
			throw new ArgumentNullException("mdfName");
		}
		try
		{
			CheckObjectState();
			return 1 == (int)ExecutionManager.ExecuteScalar("dbcc checkprimaryfile (" + SqlSmoObject.MakeSqlString(mdfName) + ", 0)");
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.IsDetachedPrimaryFile, this, ex);
		}
	}

	public bool IsWindowsGroupMember(string windowsGroup, string windowsUser)
	{
		ThrowIfCloud();
		if (windowsGroup == null)
		{
			throw new ArgumentNullException("windowsGroup");
		}
		if (windowsUser == null)
		{
			throw new ArgumentNullException("windowsUser");
		}
		try
		{
			CheckObjectState();
			DataTable dataTable = EnumWindowsGroupInfo(windowsGroup, listMembers: true);
			foreach (DataRow row in dataTable.Rows)
			{
				if (string.Compare(windowsUser, row[0].ToString(), StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.IsWindowsGroupMember, this, ex);
		}
	}

	public StringCollection EnumMembers(RoleTypes roleType)
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			if ((RoleTypes.Server & roleType) == RoleTypes.Server && DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
			{
				Request request = new Request("Server/Role/Member[@Name='" + Urn.EscapeString(ConnectionContext.TrueLogin) + "']", new string[0]);
				request.ParentPropertiesRequests = new PropertiesRequest[1]
				{
					new PropertiesRequest(new string[1] { "Name" })
				};
				DataTable enumeratorData = ExecutionManager.GetEnumeratorData(request);
				foreach (DataRow row in enumeratorData.Rows)
				{
					if (!stringCollection.Contains(row[0].ToString()))
					{
						stringCollection.Add(row[0].ToString());
					}
				}
			}
			if ((RoleTypes.Database & roleType) == RoleTypes.Database)
			{
				DataTable dataTable = ExecutionManager.ExecuteWithResults("select user_name(), db_name()").Tables[0];
				if (0 < dataTable.Rows.Count)
				{
					string value = dataTable.Rows[0][0].ToString();
					string value2 = dataTable.Rows[0][1].ToString();
					Request request2 = new Request("Server/Database[@Name='" + Urn.EscapeString(value2) + "']/Role/Member[@Name='" + Urn.EscapeString(value) + "']", new string[0]);
					request2.ParentPropertiesRequests = new PropertiesRequest[1]
					{
						new PropertiesRequest(new string[1] { "Name" })
					};
					DataTable enumeratorData2 = ExecutionManager.GetEnumeratorData(request2);
					foreach (DataRow row2 in enumeratorData2.Rows)
					{
						if (!stringCollection.Contains(row2[0].ToString()))
						{
							stringCollection.Add(row2[0].ToString());
						}
					}
				}
			}
			return stringCollection;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ServerEnumMembers, this, ex);
		}
	}

	public ServerVersion PingSqlServerVersion(string serverName, string login, string password)
	{
		if (serverName == null)
		{
			throw new ArgumentNullException("serverName");
		}
		if (login == null)
		{
			throw new ArgumentNullException("login");
		}
		if (password == null)
		{
			throw new ArgumentNullException("password");
		}
		try
		{
			CheckObjectState();
			return new ServerConnection(serverName, login, password).ServerVersion;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.PingSqlServerVersion, this, ex);
		}
	}

	public ServerVersion PingSqlServerVersion(string serverName)
	{
		if (serverName == null)
		{
			throw new ArgumentNullException("serverName");
		}
		try
		{
			CheckObjectState();
			return new ServerConnection(serverName).ServerVersion;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.PingSqlServerVersion, this, ex);
		}
	}

	public void DeleteBackupHistory(int mediaSetId)
	{
		ThrowIfCloud();
		try
		{
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			stringBuilder.Append("begin transaction");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("declare @id as int");
			stringBuilder.Append(Globals.newline);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "select @id = {0}", new object[1] { mediaSetId });
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("delete from msdb.dbo.restorefilegroup");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("where restore_history_id in");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("(select restore_history_id from msdb.dbo.restorehistory");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("where backup_set_id in (select backup_set_id from msdb.dbo.backupset where media_set_id = @id))");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("if @@error <> 0 goto error");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("delete from msdb.dbo.restorefile");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("where restore_history_id in");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("(select restore_history_id from msdb.dbo.restorehistory");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("where backup_set_id in (select backup_set_id from msdb.dbo.backupset where media_set_id = @id))");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("if @@error <> 0 goto error");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("delete from msdb.dbo.restorehistory where backup_set_id in (select backup_set_id from msdb.dbo.backupset where media_set_id = @id)");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("if @@error <> 0 goto error");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("delete from msdb.dbo.backupmediafamily where media_set_id = @id");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("if @@error <> 0 goto error");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("delete from msdb.dbo.backupfile where backup_set_id in (select backup_set_id from msdb.dbo.backupset where media_set_id = @id)");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("if @@error <> 0 goto error");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("delete from msdb.dbo.backupset where media_set_id = @id");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("if @@error <> 0 goto error");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("commit transaction");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("goto end_of_batch");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("error:");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("rollback transaction");
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("end_of_batch:");
			stringBuilder.Append(Globals.newline);
			ExecutionManager.ExecuteNonQuery(stringBuilder.ToString());
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DeleteBackupHistory, this, ex);
		}
	}

	public void DeleteBackupHistory(string database)
	{
		ThrowIfCloud();
		try
		{
			if (database == null)
			{
				throw new ArgumentNullException("database");
			}
			ThrowIfBelowVersion80();
			StringBuilder stringBuilder = new StringBuilder("EXEC msdb.dbo.sp_delete_database_backuphistory ");
			if (8 == base.ServerVersion.Major)
			{
				stringBuilder.Append("@db_nm = ");
			}
			else
			{
				stringBuilder.Append("@database_name = ");
			}
			stringBuilder.Append(SqlSmoObject.MakeSqlString(database));
			ExecutionManager.ExecuteNonQuery(stringBuilder.ToString());
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DeleteBackupHistory, this, ex);
		}
	}

	public override void Refresh()
	{
		base.Refresh();
		Settings.Refresh();
		if (this.IsSupportedObject<UserOptions>())
		{
			UserOptions.Refresh();
		}
		Information.Refresh();
		Configuration.Refresh();
		if (affinityInfo != null)
		{
			AffinityInfo.Refresh();
		}
		if (this.IsSupportedObject<ResourceGovernor>() && !IsExpressSku())
		{
			ResourceGovernor.Refresh();
		}
		if (this.IsSupportedObject<SmartAdmin>() && !IsExpressSku())
		{
			SmartAdmin.Refresh();
		}
		collationCache = null;
	}

	protected override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		switch (idOption)
		{
		case UrnIdOption.WithId:
		case UrnIdOption.NoId:
			base.GetUrnRecursive(urnbuilder, UrnIdOption.NoId);
			break;
		default:
			urnbuilder.Append(UrnSuffix);
			break;
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

	internal override void ScriptCreate(StringCollection query, ScriptingPreferences sp)
	{
		if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
		{
			ScriptProperties(query, sp);
		}
	}

	internal static IEnumerable RegistryProperties(DatabaseEngineType engineType)
	{
		yield return new string[3] { "AuditLevel", "AuditLevel", "REG_DWORD" };
		yield return new string[3] { "BackupDirectory", "BackupDirectory", "REG_SZ" };
		yield return new string[3] { "DefaultFile", "DefaultData", "REG_SZ" };
		yield return new string[3] { "DefaultLog", "DefaultLog", "REG_SZ" };
		yield return new string[3] { "ErrorLogSizeKb", "ErrorLogSizeInKb", "REG_DWORD" };
		yield return new string[3] { "LoginMode", "LoginMode", "REG_DWORD" };
		yield return new string[3] { "MailProfile", "MailAccountName", "REG_SZ" };
		yield return new string[3] { "NumberOfLogFiles", "NumErrorLogs", "REG_DWORD" };
		yield return new string[3] { "PerfMonMode", "Performance", "REG_DWORD" };
		yield return new string[3] { "TapeLoadWaitTime", "Tapeloadwaittime", "REG_DWORD" };
		yield return new string[3] { "", "", "" };
	}

	private void ScriptProperties(StringCollection query, ScriptingPreferences sp)
	{
		Initialize(allProperties: true);
		new StringBuilder();
		object obj = null;
		foreach (string[] item in RegistryProperties(DatabaseEngineType))
		{
			if (item[0].Length == 0)
			{
				break;
			}
			if (!IsSupportedProperty(item[0], sp))
			{
				continue;
			}
			Property property = base.Properties.Get(item[0]);
			if (property.Value == null || (sp.ScriptForAlter && !property.Dirty))
			{
				continue;
			}
			obj = property.Value;
			if ((item[0] == "NumberOfLogFiles" && (int)obj < 6) || (obj is string && ((string)obj).Length == 0))
			{
				ScriptDeleteRegSetting(query, item);
				continue;
			}
			if (item[0] == "LoginMode")
			{
				ServerLoginMode serverLoginMode = (ServerLoginMode)obj;
				if (serverLoginMode != ServerLoginMode.Integrated && serverLoginMode != ServerLoginMode.Normal && serverLoginMode != ServerLoginMode.Mixed)
				{
					throw new SmoException(ExceptionTemplatesImpl.UnsupportedLoginMode(serverLoginMode.ToString()));
				}
				ScriptRegSetting(query, item, Enum.Format(typeof(ServerLoginMode), (ServerLoginMode)obj, "d"));
				continue;
			}
			if (item[0] == "AuditLevel")
			{
				AuditLevel auditLevel = (AuditLevel)obj;
				if (AuditLevel.None > auditLevel || AuditLevel.All < auditLevel)
				{
					throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration(typeof(AuditLevel).Name));
				}
				ScriptRegSetting(query, item, Enum.Format(typeof(AuditLevel), auditLevel, "d"));
				continue;
			}
			if (item[0] == "PerfMonMode")
			{
				PerfMonMode perfMonMode = (PerfMonMode)obj;
				switch (perfMonMode)
				{
				case PerfMonMode.Continuous:
				case PerfMonMode.OnDemand:
					ScriptRegSetting(query, item, Enum.Format(typeof(PerfMonMode), (PerfMonMode)obj, "d"));
					continue;
				case PerfMonMode.None:
					if (!sp.ForDirectExecution)
					{
						continue;
					}
					break;
				}
				throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration(perfMonMode.GetType().Name));
			}
			if (item[0] == "DefaultFile" || item[0] == "DefaultLog" || item[0] == "BackupDirectory")
			{
				string text = (string)obj;
				if (text.Length == 0)
				{
					ScriptDeleteRegSetting(query, item);
					continue;
				}
				if (text[text.Length - 1] == '\\')
				{
					text = text.Remove(text.Length - 1, 1);
				}
				ScriptRegSetting(query, item, text);
			}
			else
			{
				ScriptRegSetting(query, item, obj);
			}
		}
	}

	private void ScriptRegSetting(StringCollection query, string[] prop, object oValue)
	{
		string rEG_WRITE_WRITE_PROP = Scripts.REG_WRITE_WRITE_PROP;
		if ("REG_SZ" == prop[2])
		{
			query.Add(string.Format(SmoApplication.DefaultCulture, rEG_WRITE_WRITE_PROP, new object[3]
			{
				prop[1],
				prop[2],
				"N'" + SqlSmoObject.SqlString(oValue.ToString())
			}) + "'");
		}
		else if (oValue is bool)
		{
			query.Add(string.Format(SmoApplication.DefaultCulture, rEG_WRITE_WRITE_PROP, new object[3]
			{
				prop[1],
				prop[2],
				((bool)oValue) ? 1 : 0
			}));
		}
		else
		{
			query.Add(string.Format(SmoApplication.DefaultCulture, rEG_WRITE_WRITE_PROP, new object[3]
			{
				prop[1],
				prop[2],
				oValue.ToString()
			}));
		}
	}

	private void ScriptDeleteRegSetting(StringCollection query, string[] prop)
	{
		string rEG_DELETE = Scripts.REG_DELETE;
		query.Add(string.Format(SmoApplication.DefaultCulture, rEG_DELETE, new object[1] { prop[1] }));
	}

	ISfcConnection ISfcHasConnection.GetConnection(SfcObjectQueryMode activeQueriesMode)
	{
		return ConnectionContext;
	}

	ISfcConnection ISfcHasConnection.GetConnection()
	{
		return ConnectionContext;
	}

	void ISfcHasConnection.SetConnection(ISfcConnection connection)
	{
		throw new NotSupportedException();
	}

	int ISfcDomainLite.GetLogicalVersion()
	{
		return 1;
	}

	public void JoinAvailabilityGroup(string availabilityGroupName)
	{
		JoinAvailabilityGroup(availabilityGroupName, AvailabilityGroupClusterType.Wsfc);
	}

	public void JoinAvailabilityGroup(string availabilityGroupName, AvailabilityGroupClusterType availabilityGroupClusterType)
	{
		try
		{
			string text = Scripts.ALTER + Globals.space + AvailabilityGroup.AvailabilityGroupScript + Globals.space + SqlSmoObject.MakeSqlBraket(availabilityGroupName) + Globals.space + Scripts.JOIN;
			if (availabilityGroupClusterType != AvailabilityGroupClusterType.Wsfc)
			{
				ThrowIfBelowVersion140();
				string text2 = text;
				text = text2 + Globals.space + Globals.With + Globals.space + Globals.LParen + AvailabilityGroup.ClusterTypeScript + Globals.space + Globals.EqualSign + Globals.space + AvailabilityGroup.GetAvailabilityGroupClusterType(availabilityGroupClusterType) + Globals.RParen;
			}
			text += Globals.statementTerminator;
			ExecutionManager.ExecuteNonQuery(text);
			AvailabilityGroup availabilityGroup = AvailabilityGroups[availabilityGroupName];
			if (!ExecutionManager.Recording && !SmoApplication.eventsSingleton.IsNullObjectCreated())
			{
				SmoApplication.eventsSingleton.CallObjectCreated(GetServerObject(), new ObjectCreatedEventArgs(availabilityGroup.Urn, availabilityGroup));
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.JoinAvailabilityGroupFailed(Name, availabilityGroupName), ex);
		}
	}

	public void GrantAvailabilityGroupCreateDatabasePrivilege(string availabilityGroupName)
	{
		SetAvailabilityGroupCreateDatabasePrivilege(availabilityGroupName, grantPrivilege: true);
	}

	public void RevokeAvailabilityGroupCreateDatabasePrivilege(string availabilityGroupName)
	{
		SetAvailabilityGroupCreateDatabasePrivilege(availabilityGroupName, grantPrivilege: false);
	}

	private void SetAvailabilityGroupCreateDatabasePrivilege(string availabilityGroupName, bool grantPrivilege)
	{
		if (string.IsNullOrEmpty(availabilityGroupName))
		{
			throw new ArgumentNullException("availabilityGroupName");
		}
		try
		{
			string text = (grantPrivilege ? Scripts.GRANT : Scripts.DENY);
			string text2 = Scripts.ALTER + Globals.space + AvailabilityGroup.AvailabilityGroupScript + Globals.space + SqlSmoObject.MakeSqlBraket(availabilityGroupName) + Globals.space + text + Scripts.CREATE + Globals.space + Scripts.ANY + Globals.space + AvailabilityGroup.DatabaseScript;
			text2 += Globals.statementTerminator;
			ExecutionManager.ExecuteNonQuery(text2);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			string message = (grantPrivilege ? ExceptionTemplatesImpl.GrantAGCreateDatabasePrivilegeFailed(Name, availabilityGroupName) : ExceptionTemplatesImpl.RevokeAGCreateDatabasePrivilegeFailed(Name, availabilityGroupName));
			throw new FailedOperationException(message, ex);
		}
	}

	public DataTable EnumClusterSubnets()
	{
		ThrowIfBelowVersion110();
		ThrowIfCloud();
		try
		{
			Request req = new Request(base.Urn.Value + "/ClusterSubnet");
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumClusterSubnets(Name), this, ex);
		}
	}

	public DataTable EnumClusterMembersState()
	{
		ThrowIfBelowVersion110();
		ThrowIfCloud();
		try
		{
			Request req = new Request(base.Urn.Value + "/ClusterMemberState");
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumClusterMemberState(Name), this, ex);
		}
	}

	public bool FileExists(string filePath)
	{
		DataRow dataRow = QueryFileInformation(filePath);
		return Convert.ToBoolean(dataRow[0]);
	}

	public bool ParentDirectoryExists(string filePath)
	{
		DataRow dataRow = QueryFileInformation(filePath);
		return Convert.ToBoolean(dataRow[2]);
	}

	private DataRow QueryFileInformation(string filePath)
	{
		if (string.IsNullOrEmpty(filePath))
		{
			throw new ArgumentNullException("filePath");
		}
		string arg = $"<msparam>{filePath}</msparam>";
		string sqlCommand = ((!(HostPlatform == "Windows")) ? $"select file_exists, file_is_a_directory, parent_directory_exists from sys.dm_os_file_exists({arg})" : $"exec master..xp_fileexist {arg}");
		DataSet dataSet = ConnectionContext.ExecuteWithResults(sqlCommand);
		if (dataSet == null || dataSet.Tables.Count != 1 || dataSet.Tables[0].Rows.Count != 1 || dataSet.Tables[0].Columns.Count != 3)
		{
			throw new InternalSmoErrorException(ExceptionTemplatesImpl.InvalidFileInformationData);
		}
		return dataSet.Tables[0].Rows[0];
	}
}
