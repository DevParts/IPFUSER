using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class Backup : BackupRestoreBase
{
	private string m_BackupSetDescription;

	private string m_BackupSetName;

	private DateTime m_ExpirationDate;

	private bool m_FormatMedia;

	private bool m_Initialize;

	private string m_MediaDescription;

	private int m_RetainDays;

	private bool m_SkipTapeHeader;

	private bool copyOnly;

	private bool m_Incremental;

	private BackupDeviceList[] mirrors;

	private static int MIRRORS_COUNT = 3;

	private string m_UndoFileName;

	private BackupCompressionOptions m_CompressionOption;

	private bool backupCompValueSetByUser;

	private BackupEncryptionOptions m_EncryptionOption;

	public BackupActionType Action
	{
		get
		{
			return m_BackupAction;
		}
		set
		{
			m_BackupAction = value;
		}
	}

	public string BackupSetDescription
	{
		get
		{
			return m_BackupSetDescription;
		}
		set
		{
			m_BackupSetDescription = value;
		}
	}

	public string BackupSetName
	{
		get
		{
			return m_BackupSetName;
		}
		set
		{
			m_BackupSetName = value;
		}
	}

	public DateTime ExpirationDate
	{
		get
		{
			return m_ExpirationDate;
		}
		set
		{
			m_ExpirationDate = value;
		}
	}

	public bool FormatMedia
	{
		get
		{
			return m_FormatMedia;
		}
		set
		{
			m_FormatMedia = value;
		}
	}

	public bool Initialize
	{
		get
		{
			return m_Initialize;
		}
		set
		{
			m_Initialize = value;
		}
	}

	public string MediaDescription
	{
		get
		{
			return m_MediaDescription;
		}
		set
		{
			m_MediaDescription = value;
		}
	}

	public int RetainDays
	{
		get
		{
			return m_RetainDays;
		}
		set
		{
			m_RetainDays = value;
		}
	}

	public bool SkipTapeHeader
	{
		get
		{
			return m_SkipTapeHeader;
		}
		set
		{
			m_SkipTapeHeader = value;
		}
	}

	public BackupTruncateLogType LogTruncation
	{
		get
		{
			return m_LogTruncation;
		}
		set
		{
			m_LogTruncation = value;
		}
	}

	public bool CopyOnly
	{
		get
		{
			return copyOnly;
		}
		set
		{
			copyOnly = value;
		}
	}

	public bool Incremental
	{
		get
		{
			return m_Incremental;
		}
		set
		{
			m_Incremental = value;
		}
	}

	[CLSCompliant(false)]
	public BackupDeviceList[] Mirrors
	{
		get
		{
			return mirrors;
		}
		set
		{
			if (value == null)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.SetMirrors, this, new ArgumentNullException("Mirrors"));
			}
			mirrors = value;
		}
	}

	public string UndoFileName
	{
		get
		{
			return m_UndoFileName;
		}
		set
		{
			m_UndoFileName = value;
		}
	}

	public BackupCompressionOptions CompressionOption
	{
		get
		{
			return m_CompressionOption;
		}
		set
		{
			backupCompValueSetByUser = true;
			m_CompressionOption = value;
		}
	}

	public BackupEncryptionOptions EncryptionOption
	{
		get
		{
			return m_EncryptionOption;
		}
		set
		{
			m_EncryptionOption = value;
		}
	}

	public Backup()
	{
		m_BackupAction = BackupActionType.Database;
		m_RetainDays = -1;
		m_Initialize = false;
		m_SkipTapeHeader = false;
		m_ExpirationDate = DateTime.MinValue;
		m_LogTruncation = BackupTruncateLogType.Truncate;
		m_Incremental = false;
		copyOnly = false;
		mirrors = new BackupDeviceList[MIRRORS_COUNT];
		for (int i = 0; i < mirrors.Length; i++)
		{
			mirrors[i] = new BackupDeviceList();
		}
	}

	internal Backup(bool checkForHADRMaintPlan)
		: this()
	{
		m_checkForHADRMaintPlan = checkForHADRMaintPlan;
	}

	private void ThrowIfUsingRemovedFeature(Server srv)
	{
		if (m_LogTruncation == BackupTruncateLogType.TruncateOnly && 10 <= srv.ServerVersion.Major)
		{
			throw new UnsupportedVersionException(ExceptionTemplatesImpl.InvalidPropertyValueForVersion(GetType().Name, "LogTruncation", BackupTruncateLogType.TruncateOnly.ToString(), SqlSmoObject.GetSqlServerName(srv)));
		}
	}

	public void SqlBackup(Server srv)
	{
		if (srv == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.BackupFailed, new ArgumentException("srv"));
		}
		ThrowIfUsingRemovedFeature(srv);
		try
		{
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(Script(srv));
			ExecuteSql(srv, stringCollection);
			if (!srv.ExecutionManager.Recording && Action == BackupActionType.Log && LogTruncation == BackupTruncateLogType.NoTruncate && !SmoApplication.eventsSingleton.IsNullDatabaseEvent())
			{
				SmoApplication.eventsSingleton.CallDatabaseEvent(srv, new DatabaseEventArgs(srv.Databases[base.Database].Urn, srv.Databases[base.Database], base.Database, DatabaseEventType.Backup));
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Backup, srv, ex);
		}
	}

	public void SqlBackupAsync(Server srv)
	{
		if (srv == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.BackupFailed, new ArgumentException("srv"));
		}
		ThrowIfUsingRemovedFeature(srv);
		try
		{
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(Script(srv));
			currentAsyncOperation = AsyncOperation.Backup;
			ExecuteSqlAsync(srv, stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Backup, srv, ex);
		}
	}

	public string Script(Server targetServer)
	{
		ThrowIfUsingRemovedFeature(targetServer);
		ServerVersion serverVersion = targetServer.ServerVersion;
		_ = targetServer.DatabaseEngineType;
		DatabaseEngineEdition databaseEngineEdition = targetServer.DatabaseEngineEdition;
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (base.Database == null || base.Database.Length == 0)
		{
			throw new PropertyNotSetException("Database");
		}
		switch (m_BackupAction)
		{
		case BackupActionType.Database:
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "BACKUP DATABASE [{0}]", new object[1] { SqlSmoObject.SqlBraket(base.Database) });
			break;
		case BackupActionType.Files:
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "BACKUP DATABASE [{0}]", new object[1] { SqlSmoObject.SqlBraket(base.Database) });
			int num = 0;
			StringEnumerator enumerator = base.DatabaseFiles.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					if (num++ > 0)
					{
						stringBuilder.Append(Globals.commaspace);
					}
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " FILE = N'{0}'", new object[1] { SqlSmoObject.SqlString(current) });
				}
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			StringEnumerator enumerator2 = base.DatabaseFileGroups.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					string current2 = enumerator2.Current;
					if (num++ > 0)
					{
						stringBuilder.Append(Globals.commaspace);
					}
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " FILEGROUP = N'{0}'", new object[1] { SqlSmoObject.SqlString(current2) });
				}
			}
			finally
			{
				if (enumerator2 is IDisposable disposable2)
				{
					disposable2.Dispose();
				}
			}
			break;
		}
		case BackupActionType.Log:
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "BACKUP LOG [{0}]", new object[1] { SqlSmoObject.SqlBraket(base.Database) });
			if (LogTruncation == BackupTruncateLogType.TruncateOnly)
			{
				stringBuilder.Append(" WITH NO_LOG ");
				return stringBuilder.ToString();
			}
			break;
		}
		if (base.Devices.Count == 0)
		{
			throw new PropertyNotSetException("Devices");
		}
		stringBuilder.Append(" TO ");
		GetDevicesScript(stringBuilder, base.Devices, serverVersion);
		if (serverVersion.Major >= 9)
		{
			BackupDeviceList[] array = Mirrors;
			foreach (BackupDeviceList backupDeviceList in array)
			{
				if (backupDeviceList != null && backupDeviceList.Count > 0)
				{
					if (backupDeviceList.Count != base.Devices.Count)
					{
						throw new WrongPropertyValueException(ExceptionTemplatesImpl.MismatchingNumberOfMirrors(base.Devices.Count, backupDeviceList.Count));
					}
					stringBuilder.Append(" MIRROR TO ");
					GetDevicesScript(stringBuilder, backupDeviceList, serverVersion);
				}
			}
		}
		stringBuilder.Append(" WITH ");
		int num2 = 0;
		if (AddCredential(serverVersion, stringBuilder, num2 > 0, withCommaEnd: false))
		{
			num2++;
		}
		if ((m_BackupAction == BackupActionType.Database || m_BackupAction == BackupActionType.Files) && m_Incremental)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.Append(" DIFFERENTIAL ");
		}
		if (m_BackupAction == BackupActionType.Log && m_LogTruncation == BackupTruncateLogType.NoTruncate)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.Append(" NO_TRUNCATE ");
		}
		if (base.BlockSize <= 0 && databaseEngineEdition == DatabaseEngineEdition.SqlManagedInstance)
		{
			base.BlockSize = 65536;
		}
		if (base.BlockSize > 0)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " BLOCKSIZE = {0}", new object[1] { base.BlockSize });
		}
		if (base.BufferCount > 0)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " BUFFERCOUNT = {0}", new object[1] { base.BufferCount });
		}
		if (base.MaxTransferSize <= 0 && databaseEngineEdition == DatabaseEngineEdition.SqlManagedInstance)
		{
			base.MaxTransferSize = 4194304;
		}
		if (base.MaxTransferSize > 0)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " MAXTRANSFERSIZE = {0}", new object[1] { base.MaxTransferSize });
		}
		if (9 <= serverVersion.Major && copyOnly)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " COPY_ONLY");
		}
		if (m_BackupSetDescription != null && m_BackupSetDescription.Length > 0)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " DESCRIPTION = N'{0}'", new object[1] { SqlSmoObject.SqlString(m_BackupSetDescription) });
		}
		if (m_ExpirationDate != DateTime.MinValue)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " EXPIREDATE = N'{0}'", new object[1] { m_ExpirationDate });
		}
		else if (m_RetainDays > 0)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " RETAINDAYS = {0}", new object[1] { m_RetainDays });
		}
		if (AddPassword(serverVersion, stringBuilder, num2 > 0, withCommaEnd: false))
		{
			num2++;
		}
		if (num2++ > 0)
		{
			stringBuilder.Append(Globals.commaspace);
		}
		stringBuilder.Append(m_FormatMedia ? "FORMAT" : "NOFORMAT");
		if (m_FormatMedia)
		{
			if (!m_Initialize)
			{
				throw new SmoException(ExceptionTemplatesImpl.ConflictingSwitches("FormatMedia", "Initialize"));
			}
			if (!m_SkipTapeHeader)
			{
				throw new SmoException(ExceptionTemplatesImpl.ConflictingSwitches("FormatMedia", "SkipTapeHeader"));
			}
		}
		if (num2++ > 0)
		{
			stringBuilder.Append(Globals.commaspace);
		}
		stringBuilder.Append(m_Initialize ? "INIT" : "NOINIT");
		if (m_MediaDescription != null && m_MediaDescription.Length > 0)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " MEDIADESCRIPTION = N'{0}'", new object[1] { SqlSmoObject.SqlString(m_MediaDescription) });
		}
		if (base.MediaName != null && base.MediaName.Length > 0)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " MEDIANAME = N'{0}'", new object[1] { SqlSmoObject.SqlString(base.MediaName) });
		}
		if (AddMediaPassword(serverVersion, stringBuilder, num2 > 0, withCommaEnd: false))
		{
			num2++;
		}
		if (m_BackupSetName != null && m_BackupSetName.Length > 0)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " NAME = N'{0}'", new object[1] { SqlSmoObject.SqlString(m_BackupSetName) });
		}
		if (num2++ > 0)
		{
			stringBuilder.Append(Globals.commaspace);
		}
		stringBuilder.Append(m_SkipTapeHeader ? "SKIP" : "NOSKIP");
		if (7 != serverVersion.Major)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.Append(base.NoRewind ? "NOREWIND" : "REWIND");
		}
		if (num2++ > 0)
		{
			stringBuilder.Append(Globals.commaspace);
		}
		stringBuilder.Append(base.UnloadTapeAfter ? "UNLOAD" : "NOUNLOAD");
		bool flag = false;
		if (BackupActionType.Log == m_BackupAction && IsStringValid(m_UndoFileName))
		{
			flag = true;
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " STANDBY = N'{0}' ", new object[1] { SqlSmoObject.SqlString(m_UndoFileName) });
		}
		if (BackupActionType.Log == m_BackupAction && !flag && base.NoRecovery)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.Append(" NORECOVERY ");
		}
		if (base.Restart)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.Append("RESTART");
		}
		if (10 <= serverVersion.Major)
		{
			if (!Enum.IsDefined(typeof(BackupCompressionOptions), m_CompressionOption))
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration("BackupCompressionOptions"));
			}
			if (m_CompressionOption != BackupCompressionOptions.Default)
			{
				stringBuilder.Append(Globals.commaspace);
				stringBuilder.AppendFormat((m_CompressionOption == BackupCompressionOptions.On) ? "COMPRESSION" : "NO_COMPRESSION");
			}
		}
		else if (backupCompValueSetByUser)
		{
			string version = LocalizableResources.ServerYukon;
			if (serverVersion.Major == 8)
			{
				version = LocalizableResources.ServerShiloh;
			}
			throw new UnknownPropertyException("CompressionOption", ExceptionTemplatesImpl.PropertyNotAvailableToWrite("CompressionOption", version));
		}
		if (VersionUtils.IsSql12OrLater(serverVersion))
		{
			if (m_EncryptionOption != null)
			{
				if (num2++ > 0)
				{
					stringBuilder.Append(Globals.commaspace);
				}
				stringBuilder.Append(m_EncryptionOption.Script());
			}
		}
		else if (m_EncryptionOption != null)
		{
			throw new UnsupportedVersionException(ExceptionTemplatesImpl.BackupEncryptionNotSupported);
		}
		if (base.PercentCompleteNotification > 0 && base.PercentCompleteNotification <= 100)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " STATS = {0}", new object[1] { base.PercentCompleteNotification });
		}
		if (9 <= serverVersion.Major && base.Checksum)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.Append("CHECKSUM");
		}
		if (9 <= serverVersion.Major && base.ContinueAfterError)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.Append("CONTINUE_AFTER_ERROR");
		}
		try
		{
			stringBuilder = CheckForHADRMaintPlan(targetServer, stringBuilder);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Backup, targetServer, ex);
		}
		return stringBuilder.ToString();
	}
}
