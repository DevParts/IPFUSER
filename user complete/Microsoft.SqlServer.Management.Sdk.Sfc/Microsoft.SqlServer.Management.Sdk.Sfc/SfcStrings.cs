using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[CompilerGenerated]
internal class SfcStrings
{
	[CompilerGenerated]
	public class Keys
	{
		public const string PropertyReadOnly = "PropertyReadOnly";

		public const string PropertyNotSet = "PropertyNotSet";

		public const string OperationValidOnlyInPendingState = "OperationValidOnlyInPendingState";

		public const string SfcObjectInitFailed = "SfcObjectInitFailed";

		public const string SfcInvalidConnectionContextModeChange = "SfcInvalidConnectionContextModeChange";

		public const string InvalidKey = "InvalidKey";

		public const string InvalidKeyChain = "InvalidKeyChain";

		public const string InvalidRename = "InvalidRename";

		public const string InvalidMove = "InvalidMove";

		public const string KeyExists = "KeyExists";

		public const string KeyNotFound = "KeyNotFound";

		public const string KeyAlreadySet = "KeyAlreadySet";

		public const string KeyChainAlreadySet = "KeyChainAlreadySet";

		public const string SfcInvalidArgument = "SfcInvalidArgument";

		public const string SfcInvalidReaderStream = "SfcInvalidReaderStream";

		public const string SfcInvalidWriterStream = "SfcInvalidWriterStream";

		public const string SfcInvalidSerialization = "SfcInvalidSerialization";

		public const string SfcInvalidDeserialization = "SfcInvalidDeserialization";

		public const string SfcInvalidDeserializationMissingParent = "SfcInvalidDeserializationMissingParent";

		public const string SfcInvalidSerializationInstance = "SfcInvalidSerializationInstance";

		public const string SfcInvalidDeserializationInstance = "SfcInvalidDeserializationInstance";

		public const string SfcNullArgumentToSerialize = "SfcNullArgumentToSerialize";

		public const string SfcNullArgumentToResolve = "SfcNullArgumentToResolve";

		public const string SfcNullArgumentToResolveCollection = "SfcNullArgumentToResolveCollection";

		public const string SfcNullArgumentToSfcReferenceAttribute = "SfcNullArgumentToSfcReferenceAttribute";

		public const string SfcNullInvalidSfcReferenceResolver = "SfcNullInvalidSfcReferenceResolver";

		public const string SfcNullArgumentToViewAttribute = "SfcNullArgumentToViewAttribute";

		public const string SfcNullArgumentToProxyInstance = "SfcNullArgumentToProxyInstance";

		public const string SfcNullWriterToSerialize = "SfcNullWriterToSerialize";

		public const string SfcNullReaderToSerialize = "SfcNullReaderToSerialize";

		public const string SfcNonSerializableType = "SfcNonSerializableType";

		public const string SfcInvalidWriteWithoutDiscovery = "SfcInvalidWriteWithoutDiscovery";

		public const string SfcNonSerializableProperty = "SfcNonSerializableProperty";

		public const string UnregisteredXmlSfcDomain = "UnregisteredXmlSfcDomain";

		public const string UnregisteredSfcXmlType = "UnregisteredSfcXmlType";

		public const string CannotDeserializeNonSerializableProperty = "CannotDeserializeNonSerializableProperty";

		public const string SfcUnsupportedVersion = "SfcUnsupportedVersion";

		public const string SfcUnsupportedDomainUpgrade = "SfcUnsupportedDomainUpgrade";

		public const string EmptySfcXml = "EmptySfcXml";

		public const string InvalidSfcXmlParentType = "InvalidSfcXmlParentType";

		public const string InvalidSMOQuery = "InvalidSMOQuery";

		public const string ParentHasNoConnecton = "ParentHasNoConnecton";

		public const string SfcQueryConnectionUnavailable = "SfcQueryConnectionUnavailable";

		public const string BadCreateIdentityKey = "BadCreateIdentityKey";

		public const string InvalidState = "InvalidState";

		public const string CRUDOperationFailed = "CRUDOperationFailed";

		public const string ObjectNotScriptabe = "ObjectNotScriptabe";

		public const string UnsupportedAction = "UnsupportedAction";

		public const string MissingParent = "MissingParent";

		public const string opCreate = "opCreate";

		public const string opRename = "opRename";

		public const string opMove = "opMove";

		public const string opAlter = "opAlter";

		public const string opDrop = "opDrop";

		public const string CannotMoveNoDestination = "CannotMoveNoDestination";

		public const string CannotMoveDestinationIsDescendant = "CannotMoveDestinationIsDescendant";

		public const string CannotMoveDestinationHasDuplicate = "CannotMoveDestinationHasDuplicate";

		public const string CannotRenameNoProperties = "CannotRenameNoProperties";

		public const string CannotRenameMissingProperty = "CannotRenameMissingProperty";

		public const string CannotRenameNoKey = "CannotRenameNoKey";

		public const string CannotRenameDestinationHasDuplicate = "CannotRenameDestinationHasDuplicate";

		public const string PermissionDenied = "PermissionDenied";

		public const string IncompatibleWithSfcListAdapterCollection = "IncompatibleWithSfcListAdapterCollection";

		public const string BadQueryForConnection = "BadQueryForConnection";

		public const string CannotCreateDestinationHasDuplicate = "CannotCreateDestinationHasDuplicate";

		public const string MissingSqlCeTools = "MissingSqlCeTools";

		public const string AttributeConflict = "AttributeConflict";

		public const string DomainNotFound = "DomainNotFound";

		public const string PropertyUsageError = "PropertyUsageError";

		public const string UsageRequest = "UsageRequest";

		public const string UsageFilter = "UsageFilter";

		public const string UsageOrderBy = "UsageOrderBy";

		public const string PropertyCannotHaveAlias = "PropertyCannotHaveAlias";

		public const string InvalidPrefixAlias = "InvalidPrefixAlias";

		public const string AliasNotSpecified = "AliasNotSpecified";

		public const string InvalidAlias = "InvalidAlias";

		public const string ResultNotSupported = "ResultNotSupported";

		public const string UnknownProperty = "UnknownProperty";

		public const string UnknownType = "UnknownType";

		public const string XPathUnclosedString = "XPathUnclosedString";

		public const string XPathSyntaxError = "XPathSyntaxError";

		public const string FailedToLoadAssembly = "FailedToLoadAssembly";

		public const string CouldNotInstantiateObj = "CouldNotInstantiateObj";

		public const string UnknowNodeType = "UnknowNodeType";

		public const string UnknownOperator = "UnknownOperator";

		public const string UnknownFunction = "UnknownFunction";

		public const string VariablesNotSupported = "VariablesNotSupported";

		public const string UnknownElemType = "UnknownElemType";

		public const string ChildrenNotSupported = "ChildrenNotSupported";

		public const string UnsupportedExpresion = "UnsupportedExpresion";

		public const string NotDerivedFrom = "NotDerivedFrom";

		public const string ISupportInitDataNotImplement = "ISupportInitDataNotImplement";

		public const string UrnCouldNotBeResolvedAtLevel = "UrnCouldNotBeResolvedAtLevel";

		public const string InvalidNode = "InvalidNode";

		public const string NoPropertiesRequested = "NoPropertiesRequested";

		public const string FailedRequest = "FailedRequest";

		public const string IncorrectVersionTag = "IncorrectVersionTag";

		public const string InvalidAttributeValue = "InvalidAttributeValue";

		public const string NullVersionOnLoadingCfgFile = "NullVersionOnLoadingCfgFile";

		public const string EnumObjectTagNotFound = "EnumObjectTagNotFound";

		public const string InvalidConnectionType = "InvalidConnectionType";

		public const string OnlyPathOrFullName = "OnlyPathOrFullName";

		public const string DatabaseNameMustBeSpecified = "DatabaseNameMustBeSpecified";

		public const string FailedToLoadResFile = "FailedToLoadResFile";

		public const string UnsupportedTypeDepDiscovery = "UnsupportedTypeDepDiscovery";

		public const string QueryNotSupportedPostProcess = "QueryNotSupportedPostProcess";

		public const string FailedToCreateUrn = "FailedToCreateUrn";

		public const string PropMustBeSpecified = "PropMustBeSpecified";

		public const string InvalidUrnForDepends = "InvalidUrnForDepends";

		public const string TooManyDbLevels = "TooManyDbLevels";

		public const string InvalidConfigurationFile = "InvalidConfigurationFile";

		public const string MissingSection = "MissingSection";

		public const string NotDbObject = "NotDbObject";

		public const string NotSingleDb = "NotSingleDb";

		public const string NoClassNamePostProcess = "NoClassNamePostProcess";

		public const string InvalidVersion = "InvalidVersion";

		public const string InvalidSqlServer = "InvalidSqlServer";

		public const string DatabaseNameMustBeSpecifiedinTheUrn = "DatabaseNameMustBeSpecifiedinTheUrn";

		public const string CouldNotGetInfoFromDependencyRow = "CouldNotGetInfoFromDependencyRow";

		public const string SqlServer90Name = "SqlServer90Name";

		public const string SqlServer80Name = "SqlServer80Name";

		public const string InvalidTypeForProperty = "InvalidTypeForProperty";

		public const string InvalidUrn = "InvalidUrn";

		public const string UnknownDomain = "UnknownDomain";

		public const string NoProvider = "NoProvider";

		public const string LevelNotFound = "LevelNotFound";

		public const string InvalidKeyValue = "InvalidKeyValue";

		public const string MissingKeys = "MissingKeys";

		public const string ServerNameMissing = "ServerNameMissing";

		public const string DomainRootUnknown = "DomainRootUnknown";

		public const string PropertyNotsupported = "PropertyNotsupported";

		public const string ObjectNotSupportedOnSqlDw = "ObjectNotSupportedOnSqlDw";

		private static ResourceManager resourceManager = new ResourceManager(typeof(SfcStrings).FullName, typeof(SfcStrings).Module.Assembly);

		private static CultureInfo _culture = null;

		public static CultureInfo Culture
		{
			get
			{
				return _culture;
			}
			set
			{
				_culture = value;
			}
		}

		private Keys()
		{
		}

		public static string GetString(string key)
		{
			return resourceManager.GetString(key, _culture);
		}

		public static string GetString(string key, object arg0)
		{
			return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), new object[1] { arg0 });
		}

		public static string GetString(string key, object arg0, object arg1)
		{
			return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), new object[2] { arg0, arg1 });
		}

		public static string GetString(string key, object arg0, object arg1, object arg2)
		{
			return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), new object[3] { arg0, arg1, arg2 });
		}

		public static string GetString(string key, object arg0, object arg1, object arg2, object arg3)
		{
			return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0, arg1, arg2, arg3);
		}
	}

	public static CultureInfo Culture
	{
		get
		{
			return Keys.Culture;
		}
		set
		{
			Keys.Culture = value;
		}
	}

	public static string OperationValidOnlyInPendingState => Keys.GetString("OperationValidOnlyInPendingState");

	public static string InvalidKeyChain => Keys.GetString("InvalidKeyChain");

	public static string InvalidRename => Keys.GetString("InvalidRename");

	public static string InvalidMove => Keys.GetString("InvalidMove");

	public static string KeyAlreadySet => Keys.GetString("KeyAlreadySet");

	public static string KeyChainAlreadySet => Keys.GetString("KeyChainAlreadySet");

	public static string SfcInvalidSerialization => Keys.GetString("SfcInvalidSerialization");

	public static string SfcInvalidDeserialization => Keys.GetString("SfcInvalidDeserialization");

	public static string SfcNullArgumentToSerialize => Keys.GetString("SfcNullArgumentToSerialize");

	public static string SfcNullArgumentToResolve => Keys.GetString("SfcNullArgumentToResolve");

	public static string SfcNullArgumentToResolveCollection => Keys.GetString("SfcNullArgumentToResolveCollection");

	public static string SfcNullArgumentToViewAttribute => Keys.GetString("SfcNullArgumentToViewAttribute");

	public static string SfcNullArgumentToProxyInstance => Keys.GetString("SfcNullArgumentToProxyInstance");

	public static string SfcNullWriterToSerialize => Keys.GetString("SfcNullWriterToSerialize");

	public static string SfcNullReaderToSerialize => Keys.GetString("SfcNullReaderToSerialize");

	public static string SfcInvalidWriteWithoutDiscovery => Keys.GetString("SfcInvalidWriteWithoutDiscovery");

	public static string SfcUnsupportedVersion => Keys.GetString("SfcUnsupportedVersion");

	public static string SfcUnsupportedDomainUpgrade => Keys.GetString("SfcUnsupportedDomainUpgrade");

	public static string EmptySfcXml => Keys.GetString("EmptySfcXml");

	public static string ParentHasNoConnecton => Keys.GetString("ParentHasNoConnecton");

	public static string SfcQueryConnectionUnavailable => Keys.GetString("SfcQueryConnectionUnavailable");

	public static string BadCreateIdentityKey => Keys.GetString("BadCreateIdentityKey");

	public static string MissingParent => Keys.GetString("MissingParent");

	public static string opCreate => Keys.GetString("opCreate");

	public static string opRename => Keys.GetString("opRename");

	public static string opMove => Keys.GetString("opMove");

	public static string opAlter => Keys.GetString("opAlter");

	public static string opDrop => Keys.GetString("opDrop");

	public static string PermissionDenied => Keys.GetString("PermissionDenied");

	public static string MissingSqlCeTools => Keys.GetString("MissingSqlCeTools");

	public static string UsageRequest => Keys.GetString("UsageRequest");

	public static string UsageFilter => Keys.GetString("UsageFilter");

	public static string UsageOrderBy => Keys.GetString("UsageOrderBy");

	public static string InvalidAlias => Keys.GetString("InvalidAlias");

	public static string ResultNotSupported => Keys.GetString("ResultNotSupported");

	public static string XPathUnclosedString => Keys.GetString("XPathUnclosedString");

	public static string XPathSyntaxError => Keys.GetString("XPathSyntaxError");

	public static string UnknowNodeType => Keys.GetString("UnknowNodeType");

	public static string UnknownOperator => Keys.GetString("UnknownOperator");

	public static string UnknownFunction => Keys.GetString("UnknownFunction");

	public static string VariablesNotSupported => Keys.GetString("VariablesNotSupported");

	public static string UnknownElemType => Keys.GetString("UnknownElemType");

	public static string ChildrenNotSupported => Keys.GetString("ChildrenNotSupported");

	public static string UnsupportedExpresion => Keys.GetString("UnsupportedExpresion");

	public static string InvalidNode => Keys.GetString("InvalidNode");

	public static string NoPropertiesRequested => Keys.GetString("NoPropertiesRequested");

	public static string FailedRequest => Keys.GetString("FailedRequest");

	public static string InvalidAttributeValue => Keys.GetString("InvalidAttributeValue");

	public static string NullVersionOnLoadingCfgFile => Keys.GetString("NullVersionOnLoadingCfgFile");

	public static string EnumObjectTagNotFound => Keys.GetString("EnumObjectTagNotFound");

	public static string InvalidConnectionType => Keys.GetString("InvalidConnectionType");

	public static string OnlyPathOrFullName => Keys.GetString("OnlyPathOrFullName");

	public static string DatabaseNameMustBeSpecified => Keys.GetString("DatabaseNameMustBeSpecified");

	public static string TooManyDbLevels => Keys.GetString("TooManyDbLevels");

	public static string InvalidConfigurationFile => Keys.GetString("InvalidConfigurationFile");

	public static string NotDbObject => Keys.GetString("NotDbObject");

	public static string NotSingleDb => Keys.GetString("NotSingleDb");

	public static string NoClassNamePostProcess => Keys.GetString("NoClassNamePostProcess");

	public static string SqlServer90Name => Keys.GetString("SqlServer90Name");

	public static string SqlServer80Name => Keys.GetString("SqlServer80Name");

	public static string InvalidUrn => Keys.GetString("InvalidUrn");

	public static string PropertyNotsupported => Keys.GetString("PropertyNotsupported");

	public static string ObjectNotSupportedOnSqlDw => Keys.GetString("ObjectNotSupportedOnSqlDw");

	protected SfcStrings()
	{
	}

	public static string PropertyReadOnly(string propertyName)
	{
		return Keys.GetString("PropertyReadOnly", propertyName);
	}

	public static string PropertyNotSet(string propertyName)
	{
		return Keys.GetString("PropertyNotSet", propertyName);
	}

	public static string SfcObjectInitFailed(string objName)
	{
		return Keys.GetString("SfcObjectInitFailed", objName);
	}

	public static string SfcInvalidConnectionContextModeChange(string fromMode, string toMode)
	{
		return Keys.GetString("SfcInvalidConnectionContextModeChange", fromMode, toMode);
	}

	public static string InvalidKey(string keyName)
	{
		return Keys.GetString("InvalidKey", keyName);
	}

	public static string KeyExists(string key)
	{
		return Keys.GetString("KeyExists", key);
	}

	public static string KeyNotFound(string key)
	{
		return Keys.GetString("KeyNotFound", key);
	}

	public static string SfcInvalidArgument(string argumentName)
	{
		return Keys.GetString("SfcInvalidArgument", argumentName);
	}

	public static string SfcInvalidReaderStream(string argumentName)
	{
		return Keys.GetString("SfcInvalidReaderStream", argumentName);
	}

	public static string SfcInvalidWriterStream(string argumentName)
	{
		return Keys.GetString("SfcInvalidWriterStream", argumentName);
	}

	public static string SfcInvalidDeserializationMissingParent(string instanceUri, string parentUri)
	{
		return Keys.GetString("SfcInvalidDeserializationMissingParent", instanceUri, parentUri);
	}

	public static string SfcInvalidSerializationInstance(string instanceName)
	{
		return Keys.GetString("SfcInvalidSerializationInstance", instanceName);
	}

	public static string SfcInvalidDeserializationInstance(string instanceName)
	{
		return Keys.GetString("SfcInvalidDeserializationInstance", instanceName);
	}

	public static string SfcNullArgumentToSfcReferenceAttribute(string attribute)
	{
		return Keys.GetString("SfcNullArgumentToSfcReferenceAttribute", attribute);
	}

	public static string SfcNullInvalidSfcReferenceResolver(string resolverType, string resolverInterface)
	{
		return Keys.GetString("SfcNullInvalidSfcReferenceResolver", resolverType, resolverInterface);
	}

	public static string SfcNonSerializableType(string typeName)
	{
		return Keys.GetString("SfcNonSerializableType", typeName);
	}

	public static string SfcNonSerializableProperty(string property)
	{
		return Keys.GetString("SfcNonSerializableProperty", property);
	}

	public static string UnregisteredXmlSfcDomain(string sfcDomainName)
	{
		return Keys.GetString("UnregisteredXmlSfcDomain", sfcDomainName);
	}

	public static string UnregisteredSfcXmlType(string sfcDomain, string sfcType)
	{
		return Keys.GetString("UnregisteredSfcXmlType", sfcDomain, sfcType);
	}

	public static string CannotDeserializeNonSerializableProperty(string propertyName, string sfcTypeName)
	{
		return Keys.GetString("CannotDeserializeNonSerializableProperty", propertyName, sfcTypeName);
	}

	public static string InvalidSfcXmlParentType(string sfcExpectedParentDomain, string sfcExpectedParentType, string sfcGivenParentType)
	{
		return Keys.GetString("InvalidSfcXmlParentType", sfcExpectedParentDomain, sfcExpectedParentType, sfcGivenParentType);
	}

	public static string InvalidSMOQuery(string query)
	{
		return Keys.GetString("InvalidSMOQuery", query);
	}

	public static string InvalidState(SfcObjectState current_state, SfcObjectState required_state)
	{
		return Keys.GetString("InvalidState", current_state, required_state);
	}

	public static string CRUDOperationFailed(string opname, string objname)
	{
		return Keys.GetString("CRUDOperationFailed", opname, objname);
	}

	public static string ObjectNotScriptabe(string objname, string className)
	{
		return Keys.GetString("ObjectNotScriptabe", objname, className);
	}

	public static string UnsupportedAction(string action, string className)
	{
		return Keys.GetString("UnsupportedAction", action, className);
	}

	public static string CannotMoveNoDestination(SfcInstance obj)
	{
		return Keys.GetString("CannotMoveNoDestination", obj);
	}

	public static string CannotMoveDestinationIsDescendant(SfcInstance obj, SfcInstance destObj)
	{
		return Keys.GetString("CannotMoveDestinationIsDescendant", obj, destObj);
	}

	public static string CannotMoveDestinationHasDuplicate(SfcInstance obj, SfcInstance destObj)
	{
		return Keys.GetString("CannotMoveDestinationHasDuplicate", obj, destObj);
	}

	public static string CannotRenameNoProperties(SfcInstance obj)
	{
		return Keys.GetString("CannotRenameNoProperties", obj);
	}

	public static string CannotRenameMissingProperty(SfcInstance obj, string missingProperty)
	{
		return Keys.GetString("CannotRenameMissingProperty", obj, missingProperty);
	}

	public static string CannotRenameNoKey(SfcInstance obj)
	{
		return Keys.GetString("CannotRenameNoKey", obj);
	}

	public static string CannotRenameDestinationHasDuplicate(SfcInstance obj, SfcKey key)
	{
		return Keys.GetString("CannotRenameDestinationHasDuplicate", obj, key);
	}

	public static string IncompatibleWithSfcListAdapterCollection(string type)
	{
		return Keys.GetString("IncompatibleWithSfcListAdapterCollection", type);
	}

	public static string BadQueryForConnection(string query, string rootName)
	{
		return Keys.GetString("BadQueryForConnection", query, rootName);
	}

	public static string CannotCreateDestinationHasDuplicate(SfcInstance obj)
	{
		return Keys.GetString("CannotCreateDestinationHasDuplicate", obj);
	}

	public static string AttributeConflict(string firstAttribute, string secondAttribute, string typeName, string propertyName)
	{
		return Keys.GetString("AttributeConflict", firstAttribute, secondAttribute, typeName, propertyName);
	}

	public static string DomainNotFound(string name)
	{
		return Keys.GetString("DomainNotFound", name);
	}

	public static string PropertyUsageError(string name, string usage)
	{
		return Keys.GetString("PropertyUsageError", name, usage);
	}

	public static string PropertyCannotHaveAlias(string name)
	{
		return Keys.GetString("PropertyCannotHaveAlias", name);
	}

	public static string InvalidPrefixAlias(string name)
	{
		return Keys.GetString("InvalidPrefixAlias", name);
	}

	public static string AliasNotSpecified(string name)
	{
		return Keys.GetString("AliasNotSpecified", name);
	}

	public static string UnknownProperty(string property)
	{
		return Keys.GetString("UnknownProperty", property);
	}

	public static string UnknownType(string type)
	{
		return Keys.GetString("UnknownType", type);
	}

	public static string FailedToLoadAssembly(string assembly)
	{
		return Keys.GetString("FailedToLoadAssembly", assembly);
	}

	public static string CouldNotInstantiateObj(string objType)
	{
		return Keys.GetString("CouldNotInstantiateObj", objType);
	}

	public static string NotDerivedFrom(string objType, string objName)
	{
		return Keys.GetString("NotDerivedFrom", objType, objName);
	}

	public static string ISupportInitDataNotImplement(string objType)
	{
		return Keys.GetString("ISupportInitDataNotImplement", objType);
	}

	public static string UrnCouldNotBeResolvedAtLevel(string level)
	{
		return Keys.GetString("UrnCouldNotBeResolvedAtLevel", level);
	}

	public static string IncorrectVersionTag(string elemContent)
	{
		return Keys.GetString("IncorrectVersionTag", elemContent);
	}

	public static string FailedToLoadResFile(string fileName)
	{
		return Keys.GetString("FailedToLoadResFile", fileName);
	}

	public static string UnsupportedTypeDepDiscovery(string objType, string suppList)
	{
		return Keys.GetString("UnsupportedTypeDepDiscovery", objType, suppList);
	}

	public static string QueryNotSupportedPostProcess(string propList)
	{
		return Keys.GetString("QueryNotSupportedPostProcess", propList);
	}

	public static string FailedToCreateUrn(string objCode)
	{
		return Keys.GetString("FailedToCreateUrn", objCode);
	}

	public static string PropMustBeSpecified(string prop, string obj)
	{
		return Keys.GetString("PropMustBeSpecified", prop, obj);
	}

	public static string InvalidUrnForDepends(string urn)
	{
		return Keys.GetString("InvalidUrnForDepends", urn);
	}

	public static string MissingSection(string section)
	{
		return Keys.GetString("MissingSection", section);
	}

	public static string InvalidVersion(string version)
	{
		return Keys.GetString("InvalidVersion", version);
	}

	public static string InvalidSqlServer(string productName)
	{
		return Keys.GetString("InvalidSqlServer", productName);
	}

	public static string DatabaseNameMustBeSpecifiedinTheUrn(string urn)
	{
		return Keys.GetString("DatabaseNameMustBeSpecifiedinTheUrn", urn);
	}

	public static string CouldNotGetInfoFromDependencyRow(string rowInformation)
	{
		return Keys.GetString("CouldNotGetInfoFromDependencyRow", rowInformation);
	}

	public static string InvalidTypeForProperty(string propertyName, string typeName)
	{
		return Keys.GetString("InvalidTypeForProperty", propertyName, typeName);
	}

	public static string UnknownDomain(string root)
	{
		return Keys.GetString("UnknownDomain", root);
	}

	public static string NoProvider(string urn)
	{
		return Keys.GetString("NoProvider", urn);
	}

	public static string LevelNotFound(string level, string urn)
	{
		return Keys.GetString("LevelNotFound", level, urn);
	}

	public static string InvalidKeyValue(string key, string urn)
	{
		return Keys.GetString("InvalidKeyValue", key, urn);
	}

	public static string MissingKeys(string urn, string level)
	{
		return Keys.GetString("MissingKeys", urn, level);
	}

	public static string ServerNameMissing(string urn)
	{
		return Keys.GetString("ServerNameMissing", urn);
	}

	public static string DomainRootUnknown(string fullTypeName)
	{
		return Keys.GetString("DomainRootUnknown", fullTypeName);
	}
}
