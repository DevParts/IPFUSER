using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

public static class SmoUtility
{
	private static List<string> ObjectSupportedBySqlDw = new List<string>
	{
		"Certificate", "Column", "Database", "DatabaseRole", "DefaultConstraint", "DatabaseScopedCredential", "ExternalDataSource", "ExternalFileFormat", "Index", "IndexedColumn",
		"Login", "MasterKey", "Parameter", "PartitionFunctionParameter", "PartitionSchemeParameter", "PhysicalPartition", "Schema", "Server", "Statistic", "StatisticColumn",
		"StoredProcedure", "Table", "User", "UserDefinedFunction", "UserOptions", "View"
	};

	private static readonly ServerVersion[] supportedOnPremVersions = new ServerVersion[8]
	{
		new ServerVersion(9, 0),
		new ServerVersion(10, 0),
		new ServerVersion(10, 50),
		new ServerVersion(11, 0),
		new ServerVersion(12, 0),
		new ServerVersion(13, 0),
		new ServerVersion(14, 0),
		new ServerVersion(15, 0)
	};

	private static readonly ServerVersion[] supportedCloudVersions = new ServerVersion[1]
	{
		new ServerVersion(12, 0)
	};

	public static bool IsSupportedObject(Type type, ServerVersion serverVersion, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		switch (databaseEngineType)
		{
		case DatabaseEngineType.Standalone:
			if (serverVersion.Major < 14)
			{
				switch (type.Name)
				{
				case "ExternalLibrary":
				case "ExternalLibraryFile":
				case "ResumableIndex":
					return false;
				}
			}
			if (serverVersion.Major < 13)
			{
				switch (type.Name)
				{
				case "ColumnEncryptionKey":
				case "ColumnEncryptionKeyValue":
				case "ColumnMasterKey":
				case "ExternalDataSource":
				case "ExternalFileFormat":
				case "ExternalResourcePool":
				case "ExternalResourcePoolAffinityInfo":
				case "SecurityPolicy":
				case "SecurityPredicate":
				case "QueryStoreOptions":
				case "DatabaseScopedCredential":
				case "DatabaseScopedConfiguration":
					return false;
				}
			}
			if (serverVersion.Major == 11 && serverVersion.Minor == 0 && serverVersion.BuildNumber < 2813)
			{
				switch (type.Name)
				{
				case "IndexedXmlPath":
				case "IndexedXmlPathNamespace":
					return false;
				}
			}
			if (serverVersion.Major < 11)
			{
				switch (type.Name)
				{
				case "AffinityInfo":
					if ((serverVersion.Major >= 10 && serverVersion.Minor < 50) || serverVersion.Major < 10)
					{
						return false;
					}
					return true;
				case "AvailabilityDatabase":
				case "AvailabilityGroup":
				case "AvailabilityGroupListener":
				case "AvailabilityGroupListenerIPAddress":
				case "IndexedXmlPath":
				case "IndexedXmlPathNamespace":
				case "SearchProperty":
				case "SearchPropertyList":
				case "Sequence":
				case "SmartAdmin":
					return false;
				}
			}
			if (serverVersion.Major < 10)
			{
				switch (type.Name)
				{
				case "Audit":
				case "AuditSpecification":
				case "CryptographicProvider":
				case "DatabaseAuditSpecification":
				case "DatabaseEncryptionKey":
				case "FullTextStopList":
				case "OrderColumn":
				case "ResourceGovernor":
				case "ResourcePool":
				case "ServerAuditSpecification":
				case "UserDefinedTableType":
				case "WorkLoadGroup":
					return false;
				}
			}
			if (serverVersion.Major < 9)
			{
				switch (type.Name)
				{
				case "BrokerPriority":
				case "BrokerService":
				case "Certificate":
				case "Credential":
				case "DatabaseDdlTrigger":
				case "DatabaseMirroringPayload":
				case "Endpoint":
				case "EndpointPayload":
				case "EndpointProtocol":
				case "FullTextCatalog":
				case "MailAccount":
				case "MailProfile":
				case "MailServer":
				case "MessageType":
				case "MessageTypeMapping":
				case "PartitionFunction":
				case "PartitionFunctionParameter":
				case "PartitionScheme":
				case "PartitionSchemeParameter":
				case "PhysicalPartition":
				case "PlanGuide":
				case "RemoteServiceBinding":
				case "Schema":
				case "ServerDdlTrigger":
				case "ServiceBroker":
				case "ServiceContract":
				case "ServiceMasterKey":
				case "ServiceContractMapping":
				case "ServiceQueue":
				case "ServiceRoute":
				case "SqlAssembly":
				case "Synonym":
				case "SymmetricKey":
				case "UserDefinedAggregate":
				case "UserDefinedAggregateParameter":
				case "UserDefinedType":
				case "XmlSchemaCollection":
					return false;
				}
			}
			break;
		case DatabaseEngineType.SqlAzureDatabase:
			switch (type.Name)
			{
			case "Check":
			case "Column":
			case "Database":
			case "DatabaseDdlTrigger":
			case "DatabaseRole":
			case "DefaultConstraint":
			case "ForeignKey":
			case "ForeignKeyColumn":
			case "Index":
			case "IndexedColumn":
			case "Login":
			case "Parameter":
			case "Schema":
			case "Server":
			case "Statistic":
			case "StatisticColumn":
			case "StoredProcedure":
			case "Synonym":
			case "Table":
			case "Trigger":
			case "User":
			case "UserDefinedDataType":
			case "UserDefinedFunction":
			case "UserDefinedTableType":
			case "View":
				if (databaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return ObjectSupportedBySqlDw.Contains(type.Name);
				}
				return true;
			case "ApplicationRole":
			case "AsymmetricKey":
			case "Certificate":
			case "ColumnEncryptionKey":
			case "ColumnEncryptionKeyValue":
			case "ColumnMasterKey":
			case "DatabaseScopedCredential":
			case "Default":
			case "ExtendedProperty":
			case "ExternalDataSource":
			case "FullTextCatalog":
			case "FullTextIndex":
			case "FullTextIndexColumn":
			case "FullTextService":
			case "FullTextStopList":
			case "MasterKey":
			case "NumberedStoredProcedure":
			case "PartitionFunction":
			case "PartitionFunctionParameter":
			case "PartitionScheme":
			case "PartitionSchemeParameter":
			case "PhysicalPartition":
			case "PlanGuide":
			case "QueryStoreOptions":
			case "Rule":
			case "SecurityPolicy":
			case "SecurityPredicate":
			case "DatabaseScopedConfiguration":
			case "Sequence":
			case "SqlAssembly":
			case "SymmetricKey":
			case "UserDefinedAggregate":
			case "UserDefinedAggregateParameter":
			case "UserDefinedType":
			case "UserOptions":
			case "XmlSchemaCollection":
				if (databaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return ObjectSupportedBySqlDw.Contains(type.Name);
				}
				return serverVersion.Major >= 12;
			case "ExternalFileFormat":
				if (databaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return ObjectSupportedBySqlDw.Contains(type.Name);
				}
				return false;
			default:
				return false;
			}
		}
		return true;
	}

	public static bool IsSupportedObject<T>(this SqlSmoObject smoObject, ScriptingPreferences sp = null) where T : SqlSmoObject
	{
		if (IsSupportedObject(typeof(T), smoObject.ServerVersion, smoObject.DatabaseEngineType, smoObject.DatabaseEngineEdition))
		{
			if (sp != null)
			{
				return IsSupportedObject(typeof(T), ScriptingOptions.ConvertToServerVersion(sp.TargetServerVersion), sp.TargetDatabaseEngineType, sp.TargetDatabaseEngineEdition);
			}
			return true;
		}
		return false;
	}

	internal static bool IsSupportedObject(this SqlSmoObject smoObject, Type type, ScriptingPreferences sp = null)
	{
		if (IsSupportedObject(type, smoObject.ServerVersion, smoObject.DatabaseEngineType, smoObject.DatabaseEngineEdition))
		{
			if (sp != null)
			{
				return IsSupportedObject(type, ScriptingOptions.ConvertToServerVersion(sp.TargetServerVersion), sp.TargetDatabaseEngineType, sp.TargetDatabaseEngineEdition);
			}
			return true;
		}
		return false;
	}

	internal static void ThrowIfNotSupported(this SqlSmoObject smoObject, Type type, ScriptingPreferences sp = null)
	{
		smoObject.ThrowIfNotSupported(type, null, sp);
	}

	internal static void ThrowIfNotSupported(this SqlSmoObject smoObject, Type type, string message, ScriptingPreferences sp = null)
	{
		if (IsSupportedObject(type, smoObject.ServerVersion, smoObject.DatabaseEngineType, smoObject.DatabaseEngineEdition) && (sp == null || IsSupportedObject(type, ScriptingOptions.ConvertToServerVersion(sp.TargetServerVersion), sp.TargetDatabaseEngineType, sp.TargetDatabaseEngineEdition)))
		{
			return;
		}
		if (smoObject.DatabaseEngineType == DatabaseEngineType.Standalone)
		{
			ServerVersion minimumSupportedVersion = GetMinimumSupportedVersion(type, smoObject.DatabaseEngineType, smoObject.DatabaseEngineEdition);
			if (minimumSupportedVersion.Major == 15)
			{
				message = (string.IsNullOrEmpty(message) ? ExceptionTemplatesImpl.SupportedOnlyOn150 : message);
				throw new UnsupportedVersionException(message).SetHelpContext("SupportedOnlyOn150");
			}
			if (minimumSupportedVersion.Major == 14)
			{
				message = (string.IsNullOrEmpty(message) ? ExceptionTemplatesImpl.SupportedOnlyOn140 : message);
				throw new UnsupportedVersionException(message).SetHelpContext("SupportedOnlyOn140");
			}
			if (minimumSupportedVersion.Major == 13)
			{
				message = (string.IsNullOrEmpty(message) ? ExceptionTemplatesImpl.SupportedOnlyOn130 : message);
				throw new UnsupportedVersionException(message).SetHelpContext("SupportedOnlyOn130");
			}
			if (minimumSupportedVersion.Major == 12)
			{
				message = (string.IsNullOrEmpty(message) ? ExceptionTemplatesImpl.SupportedOnlyOn120 : message);
				throw new UnsupportedVersionException(message).SetHelpContext("SupportedOnlyOn120");
			}
			if (minimumSupportedVersion.Major == 11)
			{
				message = (string.IsNullOrEmpty(message) ? ExceptionTemplatesImpl.SupportedOnlyOn110 : message);
				throw new UnsupportedVersionException(message).SetHelpContext("SupportedOnlyOn110");
			}
			if (minimumSupportedVersion.Major == 10)
			{
				if (minimumSupportedVersion.Minor == 5)
				{
					message = (string.IsNullOrEmpty(message) ? ExceptionTemplatesImpl.SupportedOnlyOn105 : message);
					throw new UnsupportedVersionException(message).SetHelpContext("SupportedOnlyOn105");
				}
				message = (string.IsNullOrEmpty(message) ? ExceptionTemplatesImpl.SupportedOnlyOn100 : message);
				throw new UnsupportedVersionException(message).SetHelpContext("SupportedOnlyOn100");
			}
			if (minimumSupportedVersion.Major == 9)
			{
				message = (string.IsNullOrEmpty(message) ? ExceptionTemplatesImpl.SupportedOnlyOn90 : message);
				throw new UnsupportedVersionException(message).SetHelpContext("SupportedOnlyOn90");
			}
			throw new UnsupportedFeatureException(ExceptionTemplatesImpl.NotSupportedOnStandaloneWithDetails(type.Name)).SetHelpContext("NotSupportedOnStandaloneWithDetails");
		}
		if (smoObject.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			if (smoObject.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse && !IsSupportedObject(type, smoObject.ServerVersion, smoObject.DatabaseEngineType, smoObject.DatabaseEngineEdition))
			{
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.NotSupportedForSqlDw(type.Name)).SetHelpContext("NotSupportedForSqlDw");
			}
			throw new UnsupportedVersionException(ExceptionTemplatesImpl.NotSupportedOnCloudWithDetails(type.Name)).SetHelpContext("NotSupportedOnCloudWithDetails");
		}
	}

	internal static ServerVersion GetMinimumSupportedVersion(Type type, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		foreach (ServerVersion supportedVersion in GetSupportedVersions(databaseEngineType, databaseEngineEdition))
		{
			if (IsSupportedObject(type, supportedVersion, databaseEngineType, databaseEngineEdition))
			{
				return supportedVersion;
			}
		}
		return new ServerVersion(99, 99);
	}

	internal static IEnumerable<ServerVersion> GetSupportedVersions(DatabaseEngineType dbEngineType, DatabaseEngineEdition dbEngineEdition)
	{
		return dbEngineType switch
		{
			DatabaseEngineType.Standalone => supportedOnPremVersions, 
			DatabaseEngineType.SqlAzureDatabase => supportedCloudVersions, 
			_ => new ServerVersion[0], 
		};
	}

	internal static void EncodeStringCollectionAsComment(StringCollection stringCollection, string headComment = "")
	{
		if (!string.IsNullOrEmpty(headComment))
		{
			stringCollection.Insert(0, "/*** " + headComment + " ***/");
		}
		for (int i = 1; i < stringCollection.Count; i++)
		{
			stringCollection[i] = "-- " + stringCollection[i];
		}
	}
}
