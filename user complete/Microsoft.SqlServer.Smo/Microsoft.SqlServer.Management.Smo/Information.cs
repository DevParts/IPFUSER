using System;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class Information : SqlSmoObject
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 23, 28, 38, 38, 38, 40, 40, 41, 45, 45 };

		private static int[] cloudVersionCount = new int[3] { 20, 22, 23 };

		private static int sqlDwPropertyCount = 23;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[23]
		{
			new StaticMetadata("BuildNumber", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("Collation", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("CollationID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ComparisonStyle", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Edition", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("EngineEdition", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("HostPlatform", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("IsCaseSensitive", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsFullTextInstalled", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSingleUser", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsXTPSupported", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("MaxPrecision", expensive: false, readOnly: true, typeof(byte)),
			new StaticMetadata("PathSeparator", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ProductLevel", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ResourceLastUpdateDateTime", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ResourceVersionString", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("SqlCharSet", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("SqlCharSetName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("SqlSortOrder", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("SqlSortOrderName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("VersionMajor", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("VersionMinor", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("VersionString", expensive: false, readOnly: true, typeof(string))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[23]
		{
			new StaticMetadata("BuildNumber", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("Collation", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("CollationID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ComparisonStyle", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Edition", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("EngineEdition", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsCaseSensitive", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSingleUser", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsXTPSupported", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("MaxPrecision", expensive: false, readOnly: true, typeof(byte)),
			new StaticMetadata("ProductLevel", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ResourceLastUpdateDateTime", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ResourceVersionString", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("SqlCharSet", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("SqlCharSetName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("SqlSortOrder", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("SqlSortOrderName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("VersionMajor", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("VersionMinor", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("VersionString", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("HostPlatform", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("PathSeparator", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("IsFullTextInstalled", expensive: false, readOnly: true, typeof(bool))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[45]
		{
			new StaticMetadata("BuildNumber", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("Edition", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ErrorLogPath", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("HasNullSaPassword", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("HostPlatform", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("IsCaseSensitive", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsFullTextInstalled", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsXTPSupported", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("Language", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("MasterDBLogPath", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("MasterDBPath", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("MaxPrecision", expensive: false, readOnly: true, typeof(byte)),
			new StaticMetadata("NetName", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("OSVersion", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("PathSeparator", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("PhysicalMemory", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("Platform", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("Processors", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("Product", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("RootDirectory", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("VersionMajor", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("VersionMinor", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("VersionString", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("Collation", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("EngineEdition", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsClustered", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSingleUser", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ProductLevel", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("BuildClrVersionString", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("CollationID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ComparisonStyle", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ComputerNamePhysicalNetBIOS", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ResourceLastUpdateDateTime", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ResourceVersionString", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("SqlCharSet", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("SqlCharSetName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("SqlSortOrder", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("SqlSortOrderName", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("FullyQualifiedNetName", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("IsHadrEnabled", expensive: true, readOnly: true, typeof(bool)),
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
						"IsCaseSensitive" => 7, 
						"IsFullTextInstalled" => 8, 
						"IsSingleUser" => 9, 
						"IsXTPSupported" => 10, 
						"MaxPrecision" => 11, 
						"PathSeparator" => 12, 
						"ProductLevel" => 13, 
						"ResourceLastUpdateDateTime" => 14, 
						"ResourceVersionString" => 15, 
						"SqlCharSet" => 16, 
						"SqlCharSetName" => 17, 
						"SqlSortOrder" => 18, 
						"SqlSortOrderName" => 19, 
						"VersionMajor" => 20, 
						"VersionMinor" => 21, 
						"VersionString" => 22, 
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
					"IsCaseSensitive" => 6, 
					"IsSingleUser" => 7, 
					"IsXTPSupported" => 8, 
					"MaxPrecision" => 9, 
					"ProductLevel" => 10, 
					"ResourceLastUpdateDateTime" => 11, 
					"ResourceVersionString" => 12, 
					"SqlCharSet" => 13, 
					"SqlCharSetName" => 14, 
					"SqlSortOrder" => 15, 
					"SqlSortOrderName" => 16, 
					"VersionMajor" => 17, 
					"VersionMinor" => 18, 
					"VersionString" => 19, 
					"HostPlatform" => 20, 
					"PathSeparator" => 21, 
					"IsFullTextInstalled" => 22, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"BuildNumber" => 0, 
				"Edition" => 1, 
				"ErrorLogPath" => 2, 
				"HasNullSaPassword" => 3, 
				"HostPlatform" => 4, 
				"IsCaseSensitive" => 5, 
				"IsFullTextInstalled" => 6, 
				"IsXTPSupported" => 7, 
				"Language" => 8, 
				"MasterDBLogPath" => 9, 
				"MasterDBPath" => 10, 
				"MaxPrecision" => 11, 
				"NetName" => 12, 
				"OSVersion" => 13, 
				"PathSeparator" => 14, 
				"PhysicalMemory" => 15, 
				"Platform" => 16, 
				"Processors" => 17, 
				"Product" => 18, 
				"RootDirectory" => 19, 
				"VersionMajor" => 20, 
				"VersionMinor" => 21, 
				"VersionString" => 22, 
				"Collation" => 23, 
				"EngineEdition" => 24, 
				"IsClustered" => 25, 
				"IsSingleUser" => 26, 
				"ProductLevel" => 27, 
				"BuildClrVersionString" => 28, 
				"CollationID" => 29, 
				"ComparisonStyle" => 30, 
				"ComputerNamePhysicalNetBIOS" => 31, 
				"ResourceLastUpdateDateTime" => 32, 
				"ResourceVersionString" => 33, 
				"SqlCharSet" => 34, 
				"SqlCharSetName" => 35, 
				"SqlSortOrder" => 36, 
				"SqlSortOrderName" => 37, 
				"FullyQualifiedNetName" => 38, 
				"IsHadrEnabled" => 39, 
				"IsPolyBaseInstalled" => 40, 
				"HostDistribution" => 41, 
				"HostRelease" => 42, 
				"HostServicePackLevel" => 43, 
				"HostSku" => 44, 
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

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string BuildClrVersionString => (string)base.Properties.GetValueWithNullReplacement("BuildClrVersionString");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int BuildNumber => (int)base.Properties.GetValueWithNullReplacement("BuildNumber");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Collation => (string)base.Properties.GetValueWithNullReplacement("Collation");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int CollationID => (int)base.Properties.GetValueWithNullReplacement("CollationID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ComparisonStyle => (int)base.Properties.GetValueWithNullReplacement("ComparisonStyle");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ComputerNamePhysicalNetBIOS => (string)base.Properties.GetValueWithNullReplacement("ComputerNamePhysicalNetBIOS");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Edition => (string)base.Properties.GetValueWithNullReplacement("Edition");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ErrorLogPath => (string)base.Properties.GetValueWithNullReplacement("ErrorLogPath");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string FullyQualifiedNetName => (string)base.Properties.GetValueWithNullReplacement("FullyQualifiedNetName");

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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsCaseSensitive => (bool)base.Properties.GetValueWithNullReplacement("IsCaseSensitive");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsClustered => (bool)base.Properties.GetValueWithNullReplacement("IsClustered");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsFullTextInstalled => (bool)base.Properties.GetValueWithNullReplacement("IsFullTextInstalled");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public bool IsHadrEnabled => (bool)base.Properties.GetValueWithNullReplacement("IsHadrEnabled");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsPolyBaseInstalled => (bool)base.Properties.GetValueWithNullReplacement("IsPolyBaseInstalled");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsSingleUser => (bool)base.Properties.GetValueWithNullReplacement("IsSingleUser");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsXTPSupported => (bool)base.Properties.GetValueWithNullReplacement("IsXTPSupported");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string Language => (string)base.Properties.GetValueWithNullReplacement("Language");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string MasterDBLogPath => (string)base.Properties.GetValueWithNullReplacement("MasterDBLogPath");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string MasterDBPath => (string)base.Properties.GetValueWithNullReplacement("MasterDBPath");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public byte MaxPrecision => (byte)base.Properties.GetValueWithNullReplacement("MaxPrecision");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string NetName => (string)base.Properties.GetValueWithNullReplacement("NetName");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string OSVersion => (string)base.Properties.GetValueWithNullReplacement("OSVersion");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string PathSeparator => (string)base.Properties.GetValueWithNullReplacement("PathSeparator");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int PhysicalMemory => (int)base.Properties.GetValueWithNullReplacement("PhysicalMemory");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string Platform => (string)base.Properties.GetValueWithNullReplacement("Platform");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int Processors => (int)base.Properties.GetValueWithNullReplacement("Processors");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string Product => (string)base.Properties.GetValueWithNullReplacement("Product");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ProductLevel => (string)base.Properties.GetValueWithNullReplacement("ProductLevel");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime ResourceLastUpdateDateTime => (DateTime)base.Properties.GetValueWithNullReplacement("ResourceLastUpdateDateTime");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ResourceVersionString => (string)base.Properties.GetValueWithNullReplacement("ResourceVersionString");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string RootDirectory => (string)base.Properties.GetValueWithNullReplacement("RootDirectory");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public short SqlCharSet => (short)base.Properties.GetValueWithNullReplacement("SqlCharSet");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string SqlCharSetName => (string)base.Properties.GetValueWithNullReplacement("SqlCharSetName");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public short SqlSortOrder => (short)base.Properties.GetValueWithNullReplacement("SqlSortOrder");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string SqlSortOrderName => (string)base.Properties.GetValueWithNullReplacement("SqlSortOrderName");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int VersionMajor => (int)base.Properties.GetValueWithNullReplacement("VersionMajor");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int VersionMinor => (int)base.Properties.GetValueWithNullReplacement("VersionMinor");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string VersionString => (string)base.Properties.GetValueWithNullReplacement("VersionString");

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Server Parent
	{
		get
		{
			CheckObjectState();
			return singletonParent as Server;
		}
	}

	public static string UrnSuffix => "Information";

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public Version Version
	{
		get
		{
			ServerVersion serverVersion = base.ServerVersion;
			return new Version(serverVersion.Major, serverVersion.Minor, serverVersion.BuildNumber);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public Edition EngineEdition => (Edition)(int)base.Properties.GetValueWithNullReplacement("EngineEdition");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public Version ResourceVersion => new Version(ResourceVersionString);

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public Version BuildClrVersion => new Version(BuildClrVersionString.Substring(1));

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal Information(Server parentsrv, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentsrv;
		SetServerObject(parentsrv.GetServerObject());
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}
}
