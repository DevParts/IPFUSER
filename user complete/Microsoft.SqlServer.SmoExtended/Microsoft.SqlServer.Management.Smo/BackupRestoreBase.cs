using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.Internal;

namespace Microsoft.SqlServer.Management.Smo;

public class BackupRestoreBase
{
	protected enum AsyncOperation
	{
		None,
		Backup,
		Restore
	}

	protected AsyncOperation currentAsyncOperation;

	protected BackupTruncateLogType m_LogTruncation;

	protected BackupActionType m_BackupAction;

	protected RestoreActionType m_RestoreAction;

	private Server server;

	private int blockSize;

	private int bufferCount;

	private int maxTransferSize;

	private bool m_retryFailedQueries = true;

	private bool processCompleted;

	private readonly object syncRoot = new object();

	private AsyncStatus asyncStatus = new AsyncStatus();

	private BackupDeviceList backupDevices;

	private StringCollection databaseFiles;

	private StringCollection databaseFileGroups;

	private string database;

	private string credentialName;

	private bool checksum;

	private bool continueAfterError;

	private string mediaName;

	private SqlSecureString mediaPassword;

	internal bool m_checkForHADRMaintPlan;

	private bool m_ignoreReplicaType;

	private bool noRewind;

	internal bool NorewindValueSetByUser;

	private int percentCompleteNotification = 10;

	private SqlSecureString password;

	private bool restart;

	private bool unloadTapeAfter;

	internal bool UnloadValueSetByUser;

	private bool m_NoRecovery;

	public int BlockSize
	{
		get
		{
			return blockSize;
		}
		set
		{
			blockSize = value;
		}
	}

	public int BufferCount
	{
		get
		{
			return bufferCount;
		}
		set
		{
			bufferCount = value;
		}
	}

	public int MaxTransferSize
	{
		get
		{
			return maxTransferSize;
		}
		set
		{
			maxTransferSize = value;
		}
	}

	public bool RetryFailedQueries
	{
		get
		{
			return m_retryFailedQueries;
		}
		set
		{
			m_retryFailedQueries = value;
		}
	}

	public AsyncStatus AsyncStatus => asyncStatus;

	[CLSCompliant(false)]
	public BackupDeviceList Devices => backupDevices;

	public StringCollection DatabaseFiles => databaseFiles;

	public StringCollection DatabaseFileGroups => databaseFileGroups;

	public string Database
	{
		get
		{
			return database;
		}
		set
		{
			database = value;
		}
	}

	public string CredentialName
	{
		get
		{
			return credentialName;
		}
		set
		{
			credentialName = value;
		}
	}

	public bool Checksum
	{
		get
		{
			return checksum;
		}
		set
		{
			checksum = value;
		}
	}

	public bool ContinueAfterError
	{
		get
		{
			return continueAfterError;
		}
		set
		{
			continueAfterError = value;
		}
	}

	public string MediaName
	{
		get
		{
			return mediaName;
		}
		set
		{
			mediaName = value;
		}
	}

	internal static ServerVersion BackupUrlDeviceSupportedServerVersion => new ServerVersion(11, 0, 3339);

	internal bool IgnoreReplicaType
	{
		get
		{
			return m_ignoreReplicaType;
		}
		set
		{
			m_ignoreReplicaType = value;
		}
	}

	public bool NoRewind
	{
		get
		{
			return noRewind;
		}
		set
		{
			NorewindValueSetByUser = true;
			noRewind = value;
		}
	}

	public int PercentCompleteNotification
	{
		get
		{
			return percentCompleteNotification;
		}
		set
		{
			percentCompleteNotification = value;
		}
	}

	public bool Restart
	{
		get
		{
			return restart;
		}
		set
		{
			restart = value;
		}
	}

	public bool UnloadTapeAfter
	{
		get
		{
			return unloadTapeAfter;
		}
		set
		{
			UnloadValueSetByUser = true;
			unloadTapeAfter = value;
		}
	}

	public bool NoRecovery
	{
		get
		{
			return m_NoRecovery;
		}
		set
		{
			m_NoRecovery = value;
		}
	}

	public event PercentCompleteEventHandler PercentComplete;

	public event ServerMessageEventHandler NextMedia;

	public event ServerMessageEventHandler Complete;

	public event ServerMessageEventHandler Information;

	public BackupRestoreBase()
	{
		backupDevices = new BackupDeviceList();
		databaseFiles = new StringCollection();
		databaseFileGroups = new StringCollection();
		server = null;
		m_NoRecovery = false;
		blockSize = -1;
		bufferCount = -1;
		maxTransferSize = -1;
	}

	private void SetServer(Server server)
	{
		lock (syncRoot)
		{
			if (this.server == null)
			{
				this.server = server;
				return;
			}
			throw new InvalidOperationException(ExceptionTemplatesImpl.OperationInProgress);
		}
	}

	private void ResetServer()
	{
		lock (syncRoot)
		{
			server = null;
		}
	}

	protected void ExecuteSql(Server server, StringCollection queries)
	{
		SetServer(server);
		try
		{
			processCompleted = false;
			if (this.Complete != null || this.PercentComplete != null || this.Information != null || this.NextMedia != null)
			{
				this.server.ExecutionManager.ExecuteNonQueryWithMessage(queries, OnInfoMessage, errorsAsMessages: true, RetryFailedQueries);
			}
			else
			{
				this.server.ExecutionManager.ExecuteNonQuery(queries, RetryFailedQueries);
			}
		}
		catch (Exception ex)
		{
			if (!processCompleted && ex.InnerException != null && ex.InnerException is SqlException ex2)
			{
				foreach (SqlError error in ex2.Errors)
				{
					if (error.Number == 3014)
					{
						processCompleted = true;
						break;
					}
				}
			}
			if (!processCompleted)
			{
				throw;
			}
		}
		finally
		{
			ResetServer();
		}
	}

	protected void ExecuteSqlAsync(Server server, StringCollection queries)
	{
		SetServer(server);
		this.server.ExecutionManager.ExecuteNonQueryCompleted += OnExecuteNonQueryCompleted;
		asyncStatus = new AsyncStatus(ExecutionStatus.InProgress, null);
		if (this.Complete != null || this.PercentComplete != null || this.Information != null || this.NextMedia != null)
		{
			this.server.ExecutionManager.ExecuteNonQueryWithMessageAsync(queries, OnInfoMessage, errorsAsMessages: true, RetryFailedQueries);
		}
		else
		{
			this.server.ExecutionManager.ExecuteNonQueryAsync(queries, RetryFailedQueries);
		}
	}

	protected DataSet ExecuteSqlWithResults(Server server, string cmd)
	{
		SetServer(server);
		try
		{
			if (this.Complete != null || this.PercentComplete != null || this.Information != null || this.NextMedia != null)
			{
				return this.server.ExecutionManager.ExecuteWithResultsAndMessages(cmd, OnInfoMessage, errorsAsMessages: true, RetryFailedQueries);
			}
			return this.server.ExecutionManager.ExecuteWithResults(cmd, RetryFailedQueries);
		}
		finally
		{
			ResetServer();
		}
	}

	protected StringBuilder CheckForHADRMaintPlan(Server targetServer, StringBuilder sb)
	{
		ServerVersion serverVersion = targetServer.ServerVersion;
		DatabaseEngineType databaseEngineType = targetServer.DatabaseEngineType;
		if (m_checkForHADRMaintPlan && 11 <= serverVersion.Major && databaseEngineType == DatabaseEngineType.Standalone && targetServer.IsHadrEnabled && !string.IsNullOrEmpty(targetServer.Databases[Database].AvailabilityGroupName) && !m_ignoreReplicaType)
		{
			sb = GetMaintPlanTSQLForRightReplica(sb);
		}
		return sb;
	}

	private StringBuilder GetMaintPlanTSQLForRightReplica(StringBuilder SqlStatement)
	{
		if (string.IsNullOrEmpty(SqlStatement.ToString()))
		{
			return SqlStatement;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("DECLARE @preferredReplica int");
		stringBuilder.AppendFormat("\r\nSET @preferredReplica = (SELECT [master].sys.fn_hadr_backup_is_preferred_replica('{0}'))", Database);
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("\r\nIF (@preferredReplica = 1)\r\nBEGIN\r\n    {0}\r\nEND", SqlStatement.ToString());
		return stringBuilder;
	}

	public void Abort()
	{
		try
		{
			if (server == null)
			{
				return;
			}
			lock (syncRoot)
			{
				if (server != null)
				{
					server.ExecutionManager.Abort();
				}
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Abort, this, ex);
		}
	}

	public void Wait()
	{
		Server server = null;
		if (this.server != null)
		{
			lock (syncRoot)
			{
				if (this.server != null)
				{
					server = this.server;
				}
			}
		}
		server?.ExecutionManager.AsyncWaitHandle.WaitOne();
	}

	private void OnExecuteNonQueryCompleted(object sender, ExecuteNonQueryCompletedEventArgs args)
	{
		asyncStatus = new AsyncStatus(args.ExecutionStatus, args.LastException);
		server.ExecutionManager.ExecuteNonQueryCompleted -= OnExecuteNonQueryCompleted;
		if (currentAsyncOperation == AsyncOperation.Backup)
		{
			if (!server.ExecutionManager.Recording && m_BackupAction == BackupActionType.Log && m_LogTruncation == BackupTruncateLogType.NoTruncate && !SmoApplication.eventsSingleton.IsNullDatabaseEvent())
			{
				SmoApplication.eventsSingleton.CallDatabaseEvent(server, new DatabaseEventArgs(server.Databases[Database].Urn, server.Databases[Database], Database, DatabaseEventType.Backup));
			}
		}
		else if (currentAsyncOperation == AsyncOperation.Restore && !server.ExecutionManager.Recording && (m_RestoreAction == RestoreActionType.Database || m_RestoreAction == RestoreActionType.Log) && !SmoApplication.eventsSingleton.IsNullDatabaseEvent())
		{
			SmoApplication.eventsSingleton.CallDatabaseEvent(server, new DatabaseEventArgs(server.Databases[Database].Urn, server.Databases[Database], Database, DatabaseEventType.Restore));
		}
		currentAsyncOperation = AsyncOperation.None;
		ResetServer();
	}

	private void OnInfoMessage(object sender, ServerMessageEventArgs e)
	{
		switch (e.Error.Number)
		{
		case 3211:
			if (this.PercentComplete != null)
			{
				this.PercentComplete(this, new PercentCompleteEventArgs(e.Error));
			}
			break;
		case 3014:
			if (this.Complete != null)
			{
				this.Complete(this, e);
			}
			processCompleted = true;
			break;
		case 3247:
		case 3249:
		case 4027:
		case 4028:
			if (this.NextMedia != null)
			{
				this.NextMedia(this, e);
			}
			break;
		default:
			if (this.Information != null)
			{
				this.Information(this, e);
			}
			break;
		}
	}

	[CLSCompliant(false)]
	protected void GetDevicesScript(StringBuilder query, BackupDeviceList devices, ServerVersion targetVersion)
	{
		TraceHelper.Assert(null != devices);
		string empty = string.Empty;
		bool flag = false;
		bool flag2 = true;
		foreach (BackupDeviceItem device in devices)
		{
			switch (device.DeviceType)
			{
			case DeviceType.Tape:
				empty = " TAPE = N'{0}'";
				flag = false;
				break;
			case DeviceType.File:
				empty = " DISK = N'{0}'";
				flag = false;
				break;
			case DeviceType.LogicalDevice:
				empty = " [{0}]";
				flag = true;
				break;
			case DeviceType.VirtualDevice:
				empty = " VIRTUAL_DEVICE = N'{0}'";
				flag = false;
				break;
			case DeviceType.Pipe:
				if (targetVersion.Major >= 9)
				{
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.BackupToPipesNotSupported(targetVersion.ToString()));
				}
				empty = " PIPE = N'{0}'";
				flag = false;
				break;
			case DeviceType.Url:
				if (!IsBackupUrlDeviceSupported(targetVersion))
				{
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.BackupToUrlNotSupported(targetVersion.ToString(), BackupUrlDeviceSupportedServerVersion.ToString()));
				}
				empty = " URL = N'{0}'";
				flag = false;
				break;
			default:
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration("DeviceType"));
			}
			if (!flag2)
			{
				query.Append(Globals.commaspace);
			}
			else
			{
				flag2 = false;
			}
			TraceHelper.Assert(null != device.Name);
			query.AppendFormat(SmoApplication.DefaultCulture, empty, new object[1] { flag ? SqlSmoObject.SqlBraket(device.Name) : SqlSmoObject.SqlString(device.Name) });
		}
	}

	[Obsolete]
	public void SetMediaPassword(string value)
	{
		mediaPassword = ((value != null) ? new SqlSecureString(value) : null);
	}

	[Obsolete]
	public void SetMediaPassword(SecureString value)
	{
		mediaPassword = value;
	}

	internal SqlSecureString GetMediaPassword()
	{
		return mediaPassword;
	}

	public static bool IsBackupUrlDeviceSupported(ServerVersion currentServerVersion)
	{
		bool result = false;
		if (currentServerVersion.Major > BackupUrlDeviceSupportedServerVersion.Major)
		{
			result = true;
		}
		else if (currentServerVersion.Major == BackupUrlDeviceSupportedServerVersion.Major && currentServerVersion.Minor >= BackupUrlDeviceSupportedServerVersion.Minor && currentServerVersion.BuildNumber >= BackupUrlDeviceSupportedServerVersion.BuildNumber)
		{
			result = true;
		}
		return result;
	}

	public static bool IsBackupFileDeviceSupported(DatabaseEngineEdition serverEdition)
	{
		return serverEdition != DatabaseEngineEdition.SqlManagedInstance;
	}

	public void SetPassword(string value)
	{
		password = ((value != null) ? new SqlSecureString(value) : null);
	}

	public void SetPassword(SecureString value)
	{
		password = value;
	}

	internal SqlSecureString GetPassword()
	{
		return password;
	}

	protected bool IsStringValid(string s)
	{
		if (s != null)
		{
			return s.Length > 0;
		}
		return false;
	}

	internal bool IsStringValid(SqlSecureString s)
	{
		if (null != s)
		{
			return s.Length > 0;
		}
		return false;
	}

	internal bool AddCredential(ServerVersion targetVersion, StringBuilder sb, bool withCommaStart, bool withCommaEnd)
	{
		bool result = false;
		if (!string.IsNullOrEmpty(CredentialName))
		{
			if (!IsBackupUrlDeviceSupported(targetVersion))
			{
				throw new UnsupportedFeatureException(ExceptionTemplatesImpl.CredentialNotSupportedError(CredentialName, targetVersion.ToString(), BackupUrlDeviceSupportedServerVersion.ToString()));
			}
			if (withCommaStart)
			{
				sb.Append(Globals.commaspace);
			}
			sb.AppendFormat(SmoApplication.DefaultCulture, " CREDENTIAL = N'{0}' ", new object[1] { SqlSmoObject.SqlString(CredentialName) });
			if (withCommaEnd)
			{
				sb.Append(Globals.commaspace);
			}
			result = true;
		}
		return result;
	}

	internal bool AddMediaPassword(ServerVersion targetVersion, StringBuilder sb, bool withCommaStart, bool withCommaEnd)
	{
		bool result = false;
		if (IsStringValid(GetMediaPassword()) && 7 != targetVersion.Major)
		{
			if (VersionUtils.IsSql11OrLater(targetVersion))
			{
				throw new UnsupportedFeatureException(ExceptionTemplatesImpl.MediaPasswordError);
			}
			if (withCommaStart)
			{
				sb.Append(Globals.commaspace);
			}
			sb.AppendFormat(SmoApplication.DefaultCulture, " MEDIAPASSWORD = N'{0}'", new object[1] { SqlSmoObject.SqlString((string)GetMediaPassword()) });
			if (withCommaEnd)
			{
				sb.Append(Globals.commaspace);
			}
			result = true;
		}
		return result;
	}

	internal bool AddPassword(ServerVersion targetVersion, StringBuilder sb, bool withCommaStart, bool withCommaEnd)
	{
		bool result = false;
		if (IsStringValid(GetPassword()) && 7 != targetVersion.Major)
		{
			if (VersionUtils.IsSql11OrLater(targetVersion))
			{
				throw new UnsupportedFeatureException(ExceptionTemplatesImpl.PasswordError);
			}
			if (withCommaStart)
			{
				sb.Append(Globals.commaspace);
			}
			sb.AppendFormat(SmoApplication.DefaultCulture, " PASSWORD = N'{0}'", new object[1] { SqlSmoObject.SqlString((string)GetPassword()) });
			if (withCommaEnd)
			{
				sb.Append(Globals.commaspace);
			}
			result = true;
		}
		return result;
	}

	internal static bool CheckNewBackupFile(Server server, string file)
	{
		if (string.IsNullOrEmpty(file))
		{
			return false;
		}
		Enumerator enumerator = new Enumerator();
		try
		{
			if (!file.StartsWith("\\\\", StringComparison.Ordinal) && string.IsNullOrEmpty(Path.GetDirectoryName(file)))
			{
				file = server.BackupDirectory + "\\" + file;
			}
		}
		catch (Exception)
		{
			return false;
		}
		Request request = new Request(new Urn("Server/File[@FullName='" + Urn.EscapeString(Path.GetFullPath(file)) + "']"));
		DataSet dataSet = enumerator.Process(server.ConnectionContext, request);
		if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
		{
			return true;
		}
		return false;
	}

	internal static bool IsBackupDeviceUrl(string url)
	{
		if (Uri.TryCreate(url, UriKind.Absolute, out var result))
		{
			if (!(result.Scheme == Uri.UriSchemeHttps))
			{
				return result.Scheme == Uri.UriSchemeHttp;
			}
			return true;
		}
		return false;
	}
}
