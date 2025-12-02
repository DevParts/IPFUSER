using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.SqlServer.Management.Common;

[CompilerGenerated]
internal class StringConnectionInfo
{
	[CompilerGenerated]
	public class Keys
	{
		public const string ConnectionCannotBeChanged = "ConnectionCannotBeChanged";

		public const string NotInTransaction = "NotInTransaction";

		public const string ConnectionFailure = "ConnectionFailure";

		public const string InvalidPropertyValue = "InvalidPropertyValue";

		public const string InvalidPropertyValueReasonString = "InvalidPropertyValueReasonString";

		public const string InvalidIntegratedSecureValue = "InvalidIntegratedSecureValue";

		public const string InvalidPropertyValueReasonInt = "InvalidPropertyValueReasonInt";

		public const string ExecutionFailure = "ExecutionFailure";

		public const string PropertyNotSetException = "PropertyNotSetException";

		public const string CannotSetWhenLoginSecure = "CannotSetWhenLoginSecure";

		public const string PropertyNotAvailable = "PropertyNotAvailable";

		public const string ConnectToInvalidVersion = "ConnectToInvalidVersion";

		public const string ClassDefaulConstructorCannotBeUsed = "ClassDefaulConstructorCannotBeUsed";

		public const string MethodNotSupported = "MethodNotSupported";

		public const string ParseError = "ParseError";

		public const string PasswordCouldNotBeChanged = "PasswordCouldNotBeChanged";

		public const string InvalidLockTimeout = "InvalidLockTimeout";

		public const string InvalidArgumentCacheCapacity = "InvalidArgumentCacheCapacity";

		public const string InvalidArgumentCacheNullKey = "InvalidArgumentCacheNullKey";

		public const string InvalidArgumentCacheDuplicateKey = "InvalidArgumentCacheDuplicateKey";

		public const string SqlAzureDatabaseEdition = "SqlAzureDatabaseEdition";

		public const string SqlDataWarehouseEdition = "SqlDataWarehouseEdition";

		public const string EnterpriseEdition = "EnterpriseEdition";

		public const string ExpressEdition = "ExpressEdition";

		public const string PersonalEdition = "PersonalEdition";

		public const string StandardEdition = "StandardEdition";

		public const string StretchEdition = "StretchEdition";

		public const string SqlManagedInstanceEdition = "SqlManagedInstanceEdition";

		public const string SqlAzureDatabase = "SqlAzureDatabase";

		public const string Standalone = "Standalone";

		public const string CannotCloseTraceController = "CannotCloseTraceController";

		public const string CannotRetrieveSchemaTable = "CannotRetrieveSchemaTable";

		public const string CannotReadNextEvent = "CannotReadNextEvent";

		public const string CannotGetColumnName = "CannotGetColumnName";

		public const string CannotGetColumnType = "CannotGetColumnType";

		public const string CannotGetColumnValue = "CannotGetColumnValue";

		public const string CannotSetColumnValue = "CannotSetColumnValue";

		public const string CannotWriteEvent = "CannotWriteEvent";

		public const string CannotCloseWriter = "CannotCloseWriter";

		public const string CannotInitializeAsReader = "CannotInitializeAsReader";

		public const string CannotInitializeAsWriter = "CannotInitializeAsWriter";

		public const string CannotInitializeAsReplayOutputWriter = "CannotInitializeAsReplayOutputWriter";

		public const string CannotPause = "CannotPause";

		public const string CannotStop = "CannotStop";

		public const string CannotRestart = "CannotRestart";

		public const string CannotStartReplay = "CannotStartReplay";

		public const string CannotPauseReplay = "CannotPauseReplay";

		public const string CannotStopReplay = "CannotStopReplay";

		public const string CannotLoadType = "CannotLoadType";

		public const string FailedToGetSQLToolsDirPathFromInstAPI = "FailedToGetSQLToolsDirPathFromInstAPI";

		public const string InstAPIIsNotInstalled = "InstAPIIsNotInstalled";

		public const string CouldNotInstantiateInstAPI = "CouldNotInstantiateInstAPI";

		public const string CouldNotLoadMethodInfoFromInstAPI = "CouldNotLoadMethodInfoFromInstAPI";

		public const string CannotTranslateSubclass = "CannotTranslateSubclass";

		public const string CannotGetOrdinal = "CannotGetOrdinal";

		public const string CannotLoadInAppDomain = "CannotLoadInAppDomain";

		public const string AssemblyLoadFailed = "AssemblyLoadFailed";

		public const string SmoSQLCLRUnAvailable = "SmoSQLCLRUnAvailable";

		public const string CannotBeSetWhileConnected = "CannotBeSetWhileConnected";

		public const string CannotPerformOperationWhileDisconnected = "CannotPerformOperationWhileDisconnected";

		public const string CannotSetTrueName = "CannotSetTrueName";

		public const string TrueNameMustBeSet = "TrueNameMustBeSet";

		public const string UnableToSavePasswordFormat = "UnableToSavePasswordFormat";

		private static ResourceManager resourceManager = new ResourceManager(typeof(StringConnectionInfo).FullName, typeof(StringConnectionInfo).Module.Assembly);

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

	public static string ConnectionCannotBeChanged => Keys.GetString("ConnectionCannotBeChanged");

	public static string NotInTransaction => Keys.GetString("NotInTransaction");

	public static string InvalidPropertyValueReasonString => Keys.GetString("InvalidPropertyValueReasonString");

	public static string ExecutionFailure => Keys.GetString("ExecutionFailure");

	public static string PasswordCouldNotBeChanged => Keys.GetString("PasswordCouldNotBeChanged");

	public static string InvalidArgumentCacheNullKey => Keys.GetString("InvalidArgumentCacheNullKey");

	public static string SqlAzureDatabaseEdition => Keys.GetString("SqlAzureDatabaseEdition");

	public static string SqlDataWarehouseEdition => Keys.GetString("SqlDataWarehouseEdition");

	public static string EnterpriseEdition => Keys.GetString("EnterpriseEdition");

	public static string ExpressEdition => Keys.GetString("ExpressEdition");

	public static string PersonalEdition => Keys.GetString("PersonalEdition");

	public static string StandardEdition => Keys.GetString("StandardEdition");

	public static string StretchEdition => Keys.GetString("StretchEdition");

	public static string SqlManagedInstanceEdition => Keys.GetString("SqlManagedInstanceEdition");

	public static string SqlAzureDatabase => Keys.GetString("SqlAzureDatabase");

	public static string Standalone => Keys.GetString("Standalone");

	public static string CannotCloseTraceController => Keys.GetString("CannotCloseTraceController");

	public static string CannotRetrieveSchemaTable => Keys.GetString("CannotRetrieveSchemaTable");

	public static string CannotReadNextEvent => Keys.GetString("CannotReadNextEvent");

	public static string CannotGetColumnName => Keys.GetString("CannotGetColumnName");

	public static string CannotGetColumnType => Keys.GetString("CannotGetColumnType");

	public static string CannotGetColumnValue => Keys.GetString("CannotGetColumnValue");

	public static string CannotSetColumnValue => Keys.GetString("CannotSetColumnValue");

	public static string CannotWriteEvent => Keys.GetString("CannotWriteEvent");

	public static string CannotCloseWriter => Keys.GetString("CannotCloseWriter");

	public static string CannotInitializeAsReader => Keys.GetString("CannotInitializeAsReader");

	public static string CannotInitializeAsWriter => Keys.GetString("CannotInitializeAsWriter");

	public static string CannotInitializeAsReplayOutputWriter => Keys.GetString("CannotInitializeAsReplayOutputWriter");

	public static string CannotPause => Keys.GetString("CannotPause");

	public static string CannotStop => Keys.GetString("CannotStop");

	public static string CannotRestart => Keys.GetString("CannotRestart");

	public static string CannotStartReplay => Keys.GetString("CannotStartReplay");

	public static string CannotPauseReplay => Keys.GetString("CannotPauseReplay");

	public static string CannotStopReplay => Keys.GetString("CannotStopReplay");

	public static string FailedToGetSQLToolsDirPathFromInstAPI => Keys.GetString("FailedToGetSQLToolsDirPathFromInstAPI");

	public static string InstAPIIsNotInstalled => Keys.GetString("InstAPIIsNotInstalled");

	public static string CouldNotInstantiateInstAPI => Keys.GetString("CouldNotInstantiateInstAPI");

	public static string CouldNotLoadMethodInfoFromInstAPI => Keys.GetString("CouldNotLoadMethodInfoFromInstAPI");

	public static string CannotTranslateSubclass => Keys.GetString("CannotTranslateSubclass");

	public static string CannotGetOrdinal => Keys.GetString("CannotGetOrdinal");

	public static string CannotLoadInAppDomain => Keys.GetString("CannotLoadInAppDomain");

	public static string SmoSQLCLRUnAvailable => Keys.GetString("SmoSQLCLRUnAvailable");

	public static string CannotBeSetWhileConnected => Keys.GetString("CannotBeSetWhileConnected");

	public static string CannotPerformOperationWhileDisconnected => Keys.GetString("CannotPerformOperationWhileDisconnected");

	public static string CannotSetTrueName => Keys.GetString("CannotSetTrueName");

	public static string TrueNameMustBeSet => Keys.GetString("TrueNameMustBeSet");

	protected StringConnectionInfo()
	{
	}

	public static string ConnectionFailure(string server)
	{
		return Keys.GetString("ConnectionFailure", server);
	}

	public static string InvalidPropertyValue(string value, string property, string reason)
	{
		return Keys.GetString("InvalidPropertyValue", value, property, reason);
	}

	public static string InvalidIntegratedSecureValue(string property)
	{
		return Keys.GetString("InvalidIntegratedSecureValue", property);
	}

	public static string InvalidPropertyValueReasonInt(string value)
	{
		return Keys.GetString("InvalidPropertyValueReasonInt", value);
	}

	public static string PropertyNotSetException(string property)
	{
		return Keys.GetString("PropertyNotSetException", property);
	}

	public static string CannotSetWhenLoginSecure(string property)
	{
		return Keys.GetString("CannotSetWhenLoginSecure", property);
	}

	public static string PropertyNotAvailable(string property)
	{
		return Keys.GetString("PropertyNotAvailable", property);
	}

	public static string ConnectToInvalidVersion(string serverVersion)
	{
		return Keys.GetString("ConnectToInvalidVersion", serverVersion);
	}

	public static string ClassDefaulConstructorCannotBeUsed(string className)
	{
		return Keys.GetString("ClassDefaulConstructorCannotBeUsed", className);
	}

	public static string MethodNotSupported(string methodName)
	{
		return Keys.GetString("MethodNotSupported", methodName);
	}

	public static string ParseError(int line)
	{
		return Keys.GetString("ParseError", line);
	}

	public static string InvalidLockTimeout(int lockTimeout)
	{
		return Keys.GetString("InvalidLockTimeout", lockTimeout);
	}

	public static string InvalidArgumentCacheCapacity(int capacity)
	{
		return Keys.GetString("InvalidArgumentCacheCapacity", capacity);
	}

	public static string InvalidArgumentCacheDuplicateKey(object key)
	{
		return Keys.GetString("InvalidArgumentCacheDuplicateKey", key);
	}

	public static string CannotLoadType(string typeName)
	{
		return Keys.GetString("CannotLoadType", typeName);
	}

	public static string AssemblyLoadFailed(string assemblyName, string originalerror)
	{
		return Keys.GetString("AssemblyLoadFailed", assemblyName, originalerror);
	}

	public static string UnableToSavePasswordFormat(string userName)
	{
		return Keys.GetString("UnableToSavePasswordFormat", userName);
	}
}
