using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class Restore : BackupRestoreBase
{
	private bool m_bVerifySuccess;

	public VerifyCompleteEventHandler VerifyComplete;

	private bool m_Partial;

	private bool m_RestrictedUser;

	private int m_FileNumber;

	private ArrayList m_RelocateFiles;

	private bool m_KeepReplication;

	private bool m_KeepTemporalRetention;

	private string m_StandbyFile;

	private bool m_ReplaceDatabase;

	private string m_ToPointInTime;

	private string m_StopAtMarkName;

	private string m_StopAtMarkAfterDate;

	private string m_StopBeforeMarkName;

	private string m_StopBeforeMarkAfterDate;

	private List<SuspectPage> databasePages = new List<SuspectPage>();

	private long[] offset;

	private bool clearSuspectPageTableAfterRestore;

	public BackupSet BackupSet { get; private set; }

	public bool Partial
	{
		get
		{
			return m_Partial;
		}
		set
		{
			m_Partial = value;
		}
	}

	public bool RestrictedUser
	{
		get
		{
			return m_RestrictedUser;
		}
		set
		{
			m_RestrictedUser = value;
		}
	}

	public int FileNumber
	{
		get
		{
			return m_FileNumber;
		}
		set
		{
			m_FileNumber = value;
		}
	}

	public ArrayList RelocateFiles => m_RelocateFiles;

	public bool KeepReplication
	{
		get
		{
			return m_KeepReplication;
		}
		set
		{
			m_KeepReplication = value;
		}
	}

	public bool KeepTemporalRetention
	{
		get
		{
			return m_KeepTemporalRetention;
		}
		set
		{
			m_KeepTemporalRetention = value;
		}
	}

	public string StandbyFile
	{
		get
		{
			return m_StandbyFile;
		}
		set
		{
			m_StandbyFile = value;
		}
	}

	public bool ReplaceDatabase
	{
		get
		{
			return m_ReplaceDatabase;
		}
		set
		{
			m_ReplaceDatabase = value;
		}
	}

	public string ToPointInTime
	{
		get
		{
			return m_ToPointInTime;
		}
		set
		{
			m_ToPointInTime = value;
		}
	}

	public string StopAtMarkName
	{
		get
		{
			return m_StopAtMarkName;
		}
		set
		{
			m_StopAtMarkName = value;
		}
	}

	public string StopAtMarkAfterDate
	{
		get
		{
			return m_StopAtMarkAfterDate;
		}
		set
		{
			m_StopAtMarkAfterDate = value;
		}
	}

	public string StopBeforeMarkName
	{
		get
		{
			return m_StopBeforeMarkName;
		}
		set
		{
			m_StopBeforeMarkName = value;
		}
	}

	public string StopBeforeMarkAfterDate
	{
		get
		{
			return m_StopBeforeMarkAfterDate;
		}
		set
		{
			m_StopBeforeMarkAfterDate = value;
		}
	}

	public RestoreActionType Action
	{
		get
		{
			return m_RestoreAction;
		}
		set
		{
			m_RestoreAction = value;
		}
	}

	public List<SuspectPage> DatabasePages => databasePages;

	public long[] Offset
	{
		get
		{
			return offset;
		}
		set
		{
			offset = value;
		}
	}

	public bool ClearSuspectPageTableAfterRestore
	{
		get
		{
			return clearSuspectPageTableAfterRestore;
		}
		set
		{
			clearSuspectPageTableAfterRestore = value;
		}
	}

	public Restore()
	{
		m_RestoreAction = RestoreActionType.Database;
		m_Partial = false;
		m_RestrictedUser = false;
		m_FileNumber = 0;
		m_KeepReplication = false;
		m_KeepTemporalRetention = false;
		m_ReplaceDatabase = false;
		m_bVerifySuccess = false;
		m_RelocateFiles = new ArrayList();
	}

	public Restore(string DestinationDatabaseName, BackupSet backupSet)
	{
		base.Database = DestinationDatabaseName;
		m_Partial = false;
		m_RestrictedUser = false;
		m_KeepReplication = false;
		m_KeepTemporalRetention = false;
		m_ReplaceDatabase = false;
		m_bVerifySuccess = false;
		m_RelocateFiles = new ArrayList();
		switch (backupSet.BackupSetType)
		{
		case BackupSetType.Database:
		case BackupSetType.Differential:
			m_RestoreAction = RestoreActionType.Database;
			break;
		case BackupSetType.Log:
			m_RestoreAction = RestoreActionType.Log;
			break;
		case BackupSetType.FileOrFileGroup:
		case BackupSetType.FileOrFileGroupDifferential:
			m_RestoreAction = RestoreActionType.Files;
			break;
		}
		backupSet.BackupMediaSet.CheckMediaSetComplete();
		int i;
		for (i = 1; i <= backupSet.BackupMediaSet.FamilyCount; i++)
		{
			IOrderedEnumerable<BackupMedia> source = from BackupMedia bkMedia in backupSet.BackupMediaSet.BackupMediaList
				where bkMedia.FamilySequenceNumber == i
				orderby bkMedia.MirrorSequenceNumber
				select bkMedia;
			if (source.Count() > 0)
			{
				BackupMedia backupMedia = source.ElementAt(0);
				base.Devices.Add(new BackupDeviceItem(backupMedia.MediaName, backupMedia.MediaType));
			}
		}
		FileNumber = backupSet.Position;
		BackupSet = backupSet;
	}

	internal Restore(bool checkForHADRMaintPlan)
		: this()
	{
		m_checkForHADRMaintPlan = checkForHADRMaintPlan;
	}

	public bool SqlVerify(Server srv)
	{
		string errorMessage;
		return SqlVerify(srv, loadHistory: false, out errorMessage);
	}

	public bool SqlVerify(Server srv, bool loadHistory)
	{
		string errorMessage;
		return SqlVerify(srv, loadHistory, out errorMessage);
	}

	public bool SqlVerify(Server srv, out string errorMessage)
	{
		return SqlVerify(srv, loadHistory: false, out errorMessage);
	}

	public bool SqlVerify(Server srv, bool loadHistory, out string errorMessage)
	{
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(ScriptVerify(srv, loadHistory));
		return SqlVerifyWorker(srv, stringCollection, out errorMessage);
	}

	internal bool SqlVerifyWorker(Server srv, StringCollection queries, out string errorMessage)
	{
		try
		{
			errorMessage = null;
			srv.ExecutionManager.BeforeExecuteSql += OnBeforeSqlVerify;
			base.Information += OnInfoMessage;
			ExecuteSql(srv, queries);
		}
		catch (SmoException ex)
		{
			errorMessage = ex.Message;
		}
		catch (ConnectionException ex2)
		{
			if (ex2.InnerException != null && ex2.InnerException is SqlException)
			{
				errorMessage = ex2.InnerException.Message;
			}
			else
			{
				errorMessage = ex2.Message;
			}
		}
		finally
		{
			base.Information -= OnInfoMessage;
			srv.ExecutionManager.BeforeExecuteSql -= OnBeforeSqlVerify;
		}
		if (VerifyComplete != null)
		{
			VerifyComplete(this, new VerifyCompleteEventArgs(m_bVerifySuccess));
		}
		return m_bVerifySuccess;
	}

	public bool SqlVerifyLatest(Server srv)
	{
		string errorMessage;
		return SqlVerifyLatest(srv, out errorMessage);
	}

	public bool SqlVerifyLatest(Server srv, out string errorMessage)
	{
		StringCollection stringCollection = new StringCollection();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.Append("declare @backupSetId as int");
		stringBuilder.Append(Globals.newline);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "select @backupSetId = position from msdb..backupset where database_name={0} and backup_set_id=(select max(backup_set_id) from msdb..backupset where database_name={0} )", new object[1] { SqlSmoObject.MakeSqlString(base.Database) });
		stringBuilder.Append(Globals.newline);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "if @backupSetId is null begin raiserror({0}, 16, 1) end", new object[1] { SqlSmoObject.MakeSqlString(ExceptionTemplatesImpl.VerifyFailed0(base.Database)) });
		stringBuilder.Append(Globals.newline);
		stringBuilder.Append(ScriptVerify(srv, loadHistory: false, ignoreFileNumber: true));
		try
		{
			stringBuilder = CheckForHADRMaintPlan(srv, stringBuilder);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Restore, srv, ex);
		}
		stringCollection.Add(stringBuilder.ToString());
		return SqlVerifyWorker(srv, stringCollection, out errorMessage);
	}

	public bool SqlVerifyLatest(Server srv, SqlVerifyAction sqlVerifyAction)
	{
		string errorMessage;
		return SqlVerifyLatest(srv, sqlVerifyAction, out errorMessage);
	}

	public bool SqlVerifyLatest(Server srv, SqlVerifyAction sqlVerifyAction, out string errorMessage)
	{
		StringCollection stringCollection = new StringCollection();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.Append("declare @backupSetId as int");
		stringBuilder.Append(Globals.newline);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "select @backupSetId = position from msdb..backupset where database_name={0} and  type={1} and backup_set_id=(select max(backup_set_id) from msdb..backupset where database_name={0} and  type={1})", new object[2]
		{
			SqlSmoObject.MakeSqlString(base.Database),
			SqlSmoObject.MakeSqlString(GetBackupTypeName(sqlVerifyAction))
		});
		stringBuilder.Append(Globals.newline);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "if @backupSetId is null begin raiserror({0}, 16, 1) end", new object[1] { SqlSmoObject.MakeSqlString(ExceptionTemplatesImpl.VerifyFailed(base.Database, sqlVerifyAction.ToString())) });
		stringBuilder.Append(Globals.newline);
		stringBuilder.Append(ScriptVerify(srv, loadHistory: false, ignoreFileNumber: true));
		try
		{
			stringBuilder = CheckForHADRMaintPlan(srv, stringBuilder);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Restore, srv, ex);
		}
		stringCollection.Add(stringBuilder.ToString());
		return SqlVerifyWorker(srv, stringCollection, out errorMessage);
	}

	private string GetBackupTypeName(SqlVerifyAction sqlVerifyAction)
	{
		return sqlVerifyAction switch
		{
			SqlVerifyAction.VerifyDatabase => "D", 
			SqlVerifyAction.VerifyFile => "F", 
			SqlVerifyAction.VerifyIncremental => "I", 
			SqlVerifyAction.VerifyLog => "L", 
			_ => throw new InternalSmoErrorException(ExceptionTemplatesImpl.UnknownEnumeration("SqlVerifyAction")), 
		};
	}

	public void SqlVerifyAsync(Server srv)
	{
		SqlVerifyAsync(srv, loadHistory: false);
	}

	public void SqlVerifyAsync(Server srv, bool loadHistory)
	{
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(ScriptVerify(srv, loadHistory));
		srv.ExecutionManager.ExecuteNonQueryCompleted += OnExecuteSqlVerifyCompleted;
		srv.ExecutionManager.BeforeExecuteSql += OnBeforeSqlVerify;
		base.Information += OnInfoMessage;
		ExecuteSqlAsync(srv, stringCollection);
	}

	private string ScriptVerify(Server srv, bool loadHistory)
	{
		return ScriptVerify(srv, loadHistory, ignoreFileNumber: false);
	}

	private string ScriptVerify(Server srv, bool loadHistory, bool ignoreFileNumber)
	{
		ServerVersion serverVersion = srv.ServerVersion;
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.Append("RESTORE VERIFYONLY");
		stringBuilder.Append(" FROM ");
		GetDevicesScript(stringBuilder, base.Devices, serverVersion);
		stringBuilder.Append(" WITH ");
		AddCredential(serverVersion, stringBuilder, withCommaStart: false, withCommaEnd: true);
		if (!ignoreFileNumber)
		{
			if (0 < FileNumber)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " FILE = {0}, ", new object[1] { m_FileNumber });
			}
		}
		else
		{
			stringBuilder.Append(" FILE = @backupSetId, ");
		}
		if (base.UnloadTapeAfter)
		{
			stringBuilder.Append(" UNLOAD, ");
		}
		else
		{
			stringBuilder.Append(" NOUNLOAD, ");
		}
		if (loadHistory)
		{
			stringBuilder.Append(" LOADHISTORY, ");
		}
		AddPassword(serverVersion, stringBuilder, withCommaStart: false, withCommaEnd: true);
		AddMediaPassword(serverVersion, stringBuilder, withCommaStart: false, withCommaEnd: true);
		if (base.NoRewind && 7 != serverVersion.Major)
		{
			stringBuilder.Append(" NOREWIND, ");
		}
		RemoveLastComma(stringBuilder);
		return stringBuilder.ToString();
	}

	private void OnBeforeSqlVerify(object sender, EventArgs args)
	{
		m_bVerifySuccess = false;
	}

	private void OnInfoMessage(object sender, ServerMessageEventArgs e)
	{
		if (3262 == e.Error.Number)
		{
			m_bVerifySuccess = true;
		}
	}

	private void OnExecuteSqlVerifyCompleted(object sender, ExecuteNonQueryCompletedEventArgs args)
	{
		base.Information -= OnInfoMessage;
		if (sender is ExecutionManager executionManager)
		{
			executionManager.ExecuteNonQueryCompleted -= OnExecuteSqlVerifyCompleted;
			executionManager.BeforeExecuteSql -= OnBeforeSqlVerify;
		}
		if (VerifyComplete != null)
		{
			VerifyComplete(this, new VerifyCompleteEventArgs(m_bVerifySuccess));
		}
	}

	public DataTable ReadFileList(Server srv)
	{
		ServerVersion serverVersion = srv.ServerVersion;
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.Append("RESTORE FILELISTONLY");
		stringBuilder.Append(" FROM ");
		GetDevicesScript(stringBuilder, base.Devices, serverVersion);
		stringBuilder.Append(" WITH ");
		AddCredential(serverVersion, stringBuilder, withCommaStart: false, withCommaEnd: true);
		if (base.UnloadTapeAfter)
		{
			stringBuilder.Append(" UNLOAD, ");
		}
		else
		{
			stringBuilder.Append(" NOUNLOAD, ");
		}
		if (0 < FileNumber)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " FILE = {0}, ", new object[1] { m_FileNumber });
		}
		AddPassword(serverVersion, stringBuilder, withCommaStart: false, withCommaEnd: true);
		AddMediaPassword(serverVersion, stringBuilder, withCommaStart: false, withCommaEnd: true);
		RemoveLastComma(stringBuilder);
		return ExecuteSqlWithResults(srv, stringBuilder.ToString()).Tables[0];
	}

	public DataTable ReadMediaHeader(Server srv)
	{
		ServerVersion serverVersion = srv.ServerVersion;
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.Append("RESTORE LABELONLY");
		stringBuilder.Append(" FROM ");
		GetDevicesScript(stringBuilder, base.Devices, serverVersion);
		stringBuilder.Append(" WITH ");
		AddCredential(serverVersion, stringBuilder, withCommaStart: false, withCommaEnd: true);
		if (base.UnloadTapeAfter)
		{
			stringBuilder.Append(" UNLOAD, ");
		}
		else
		{
			stringBuilder.Append(" NOUNLOAD, ");
		}
		AddMediaPassword(serverVersion, stringBuilder, withCommaStart: false, withCommaEnd: true);
		RemoveLastComma(stringBuilder);
		return ExecuteSqlWithResults(srv, stringBuilder.ToString()).Tables[0];
	}

	public DataTable ReadBackupHeader(Server srv)
	{
		ServerVersion serverVersion = srv.ServerVersion;
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.Append("RESTORE HEADERONLY");
		stringBuilder.Append(" FROM ");
		GetDevicesScript(stringBuilder, base.Devices, serverVersion);
		stringBuilder.Append(" WITH ");
		AddCredential(serverVersion, stringBuilder, withCommaStart: false, withCommaEnd: true);
		if (base.UnloadTapeAfter)
		{
			stringBuilder.Append(" UNLOAD, ");
		}
		else
		{
			stringBuilder.Append(" NOUNLOAD, ");
		}
		if (0 < FileNumber)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " FILE = {0}, ", new object[1] { m_FileNumber });
		}
		AddPassword(serverVersion, stringBuilder, withCommaStart: false, withCommaEnd: true);
		AddMediaPassword(serverVersion, stringBuilder, withCommaStart: false, withCommaEnd: true);
		RemoveLastComma(stringBuilder);
		return ExecuteSqlWithResults(srv, stringBuilder.ToString()).Tables[0];
	}

	public DataTable ReadSuspectPageTable(Server server)
	{
		if (server.ServerVersion.Major < 9)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.UnsupportedVersion(server.ServerVersion.ToString())).SetHelpContext("UnsupportedVersion");
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.Append("select * from msdb.dbo.suspect_pages");
		GetDbFileFilter(stringBuilder);
		return ExecuteSqlWithResults(server, stringBuilder.ToString()).Tables[0];
	}

	public void ClearSuspectPageTable(Server srv)
	{
		if (srv.ServerVersion.Major < 9)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.UnsupportedVersion(srv.ServerVersion.ToString())).SetHelpContext("UnsupportedVersion");
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.Append("delete from msdb.dbo.suspect_pages");
		GetDbFileFilter(stringBuilder);
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(stringBuilder.ToString());
		ExecuteSql(srv, stringCollection);
	}

	private void GetDbFileFilter(StringBuilder selectStmt)
	{
		if (base.Database == null)
		{
			return;
		}
		selectStmt.AppendFormat(SmoApplication.DefaultCulture, " where database_id in (select dbid from master.dbo.sysdatabases where name = N'{0}')", new object[1] { SqlSmoObject.SqlString(base.Database) });
		if (base.DatabaseFiles.Count > 1 && Offset != null && 0 < Offset.Length)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.OneFilePageSupported).SetHelpContext("OneFilePageSupported");
		}
		if (base.DatabaseFiles.Count > 0)
		{
			selectStmt.AppendFormat(SmoApplication.DefaultCulture, " and file_id in ( select fileid from [{0}].dbo.sysfiles where name in ( ", new object[1] { SqlSmoObject.SqlBraket(base.Database) });
			int num = 0;
			StringEnumerator enumerator = base.DatabaseFiles.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					if (num++ > 0)
					{
						selectStmt.Append(Globals.commaspace);
					}
					selectStmt.AppendFormat(SmoApplication.DefaultCulture, "N'{0}'", new object[1] { SqlSmoObject.SqlString(current) });
				}
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			selectStmt.Append(" )  )");
		}
		if (Offset == null || 0 >= Offset.Length)
		{
			return;
		}
		selectStmt.Append(" and page_id in (");
		int num2 = 0;
		long[] array = Offset;
		foreach (long num3 in array)
		{
			if (num2++ > 0)
			{
				selectStmt.Append(Globals.commaspace);
			}
			selectStmt.AppendFormat(SmoApplication.DefaultCulture, "0x{0:x}", new object[1] { num3 });
		}
		selectStmt.Append(" ) ");
	}

	public StringCollection Script(Server server)
	{
		ServerVersion serverVersion = server.ServerVersion;
		StringCollection stringCollection = new StringCollection();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (base.Database == null || base.Database.Length == 0)
		{
			throw new PropertyNotSetException("Database");
		}
		bool flag = false;
		switch (m_RestoreAction)
		{
		case RestoreActionType.Database:
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "RESTORE DATABASE [{0}]", new object[1] { SqlSmoObject.SqlBraket(base.Database) });
			break;
		case RestoreActionType.OnlineFiles:
			flag = true;
			goto case RestoreActionType.Files;
		case RestoreActionType.Files:
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "RESTORE DATABASE [{0}]", new object[1] { SqlSmoObject.SqlBraket(base.Database) });
			int num = 0;
			StringEnumerator enumerator2 = base.DatabaseFiles.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					string current2 = enumerator2.Current;
					if (num++ > 0)
					{
						stringBuilder.Append(Globals.commaspace);
					}
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " FILE = N'{0}'", new object[1] { SqlSmoObject.SqlString(current2) });
				}
			}
			finally
			{
				if (enumerator2 is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			StringEnumerator enumerator3 = base.DatabaseFileGroups.GetEnumerator();
			try
			{
				while (enumerator3.MoveNext())
				{
					string current3 = enumerator3.Current;
					if (num++ > 0)
					{
						stringBuilder.Append(Globals.commaspace);
					}
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " FILEGROUP = N'{0}'", new object[1] { SqlSmoObject.SqlString(current3) });
				}
			}
			finally
			{
				if (enumerator3 is IDisposable disposable2)
				{
					disposable2.Dispose();
				}
			}
			break;
		}
		case RestoreActionType.Log:
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "RESTORE LOG [{0}]", new object[1] { SqlSmoObject.SqlBraket(base.Database) });
			break;
		case RestoreActionType.OnlinePage:
		{
			Database database = server.Databases[base.Database];
			if (database == null)
			{
				throw new PropertyNotSetException("Database");
			}
			if (DatabasePages.Count == 0)
			{
				throw new PropertyNotSetException("DatabasePages");
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "RESTORE DATABASE [{0}] PAGE=", new object[1] { SqlSmoObject.SqlBraket(base.Database) });
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (SuspectPage databasePage in DatabasePages)
			{
				stringBuilder2.Append(databasePage.ToString() + ", ");
			}
			RemoveLastComma(stringBuilder2);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "'{0}'", new object[1] { stringBuilder2.ToString() });
			break;
		}
		}
		if (base.Devices.Count == 0)
		{
			if (m_RestoreAction == RestoreActionType.Log && FileNumber == 0)
			{
				stringCollection.Add(stringBuilder.ToString());
				return stringCollection;
			}
			throw new PropertyNotSetException("Devices");
		}
		stringBuilder.Append(" FROM ");
		GetDevicesScript(stringBuilder, base.Devices, serverVersion);
		if (server.DatabaseEngineEdition != DatabaseEngineEdition.SqlManagedInstance)
		{
			stringBuilder.Append(" WITH ");
			AddCredential(serverVersion, stringBuilder, withCommaStart: false, withCommaEnd: true);
			if (flag)
			{
				stringBuilder.Append(" ONLINE, ");
			}
			bool flag2 = false;
			if (Partial && 7 != serverVersion.Major && RestoreActionType.Log != m_RestoreAction)
			{
				flag2 = true;
				stringBuilder.Append(" PARTIAL, ");
			}
			if (m_RestrictedUser && 7 != serverVersion.Major)
			{
				stringBuilder.Append(" RESTRICTED_USER, ");
			}
			if (m_RestrictedUser && 7 == serverVersion.Major)
			{
				stringBuilder.Append(" DBO_ONLY, ");
			}
			if (0 < FileNumber)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " FILE = {0}, ", new object[1] { m_FileNumber });
			}
			AddPassword(serverVersion, stringBuilder, withCommaStart: false, withCommaEnd: true);
			if (IsStringValid(base.MediaName))
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " MEDIANAME = N'{0}', ", new object[1] { SqlSmoObject.SqlString(base.MediaName) });
			}
			AddMediaPassword(serverVersion, stringBuilder, withCommaStart: false, withCommaEnd: true);
			if (m_RelocateFiles.Count > 0)
			{
				foreach (RelocateFile relocateFile in m_RelocateFiles)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " MOVE N'{0}' TO N'{1}', ", new object[2]
					{
						SqlSmoObject.SqlString(relocateFile.LogicalFileName),
						SqlSmoObject.SqlString(relocateFile.PhysicalFileName)
					});
				}
			}
			if (!flag2 && RestoreActionType.Files != m_RestoreAction && m_KeepReplication && 7 != serverVersion.Major)
			{
				stringBuilder.Append(" KEEP_REPLICATION, ");
			}
			if (!flag2 && RestoreActionType.Files != m_RestoreAction && m_KeepTemporalRetention && 12 < serverVersion.Major)
			{
				stringBuilder.Append(" KEEP_TEMPORAL_RETENTION, ");
			}
			bool flag3 = false;
			if ((m_RestoreAction == RestoreActionType.Database || RestoreActionType.Log == m_RestoreAction) && IsStringValid(m_StandbyFile))
			{
				flag3 = true;
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " STANDBY = N'{0}', ", new object[1] { SqlSmoObject.SqlString(m_StandbyFile) });
			}
			if (!flag3 && base.NoRecovery)
			{
				stringBuilder.Append(" NORECOVERY, ");
			}
			if (base.NoRewind && 7 != serverVersion.Major)
			{
				stringBuilder.Append(" NOREWIND, ");
			}
			if (base.UnloadTapeAfter)
			{
				stringBuilder.Append(" UNLOAD, ");
			}
			else
			{
				stringBuilder.Append(" NOUNLOAD, ");
			}
			if (m_ReplaceDatabase && RestoreActionType.Log != m_RestoreAction)
			{
				stringBuilder.Append(" REPLACE, ");
			}
			if (base.Restart)
			{
				stringBuilder.Append(" RESTART, ");
			}
			if (base.PercentCompleteNotification > 0 && base.PercentCompleteNotification <= 100)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " STATS = {0}, ", new object[1] { base.PercentCompleteNotification });
			}
			if (RestoreActionType.Log == m_RestoreAction || (serverVersion.Major >= 9 && m_RestoreAction == RestoreActionType.Database))
			{
				if (IsStringValid(m_ToPointInTime))
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " STOPAT = N'{0}', ", new object[1] { SqlSmoObject.SqlString(m_ToPointInTime) });
				}
				else
				{
					if (BackupSet != null && BackupSet.StopAtLsn != 0m)
					{
						m_StopBeforeMarkName = string.Format(SmoApplication.DefaultCulture, "lsn:{0}", new object[1] { BackupSet.StopAtLsn });
					}
					if (7 < serverVersion.Major)
					{
						if (IsStringValid(m_StopAtMarkName))
						{
							stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " STOPATMARK = N'{0}'", new object[1] { SqlSmoObject.SqlString(m_StopAtMarkName) });
							if (IsStringValid(m_StopAtMarkAfterDate))
							{
								stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " AFTER N'{0}'", new object[1] { SqlSmoObject.SqlString(m_StopAtMarkAfterDate) });
							}
							stringBuilder.Append(", ");
						}
						else if (IsStringValid(m_StopBeforeMarkName))
						{
							stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " STOPBEFOREMARK = N'{0}'", new object[1] { SqlSmoObject.SqlString(m_StopBeforeMarkName) });
							if (IsStringValid(m_StopBeforeMarkAfterDate))
							{
								stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " AFTER N'{0}'", new object[1] { SqlSmoObject.SqlString(m_StopBeforeMarkAfterDate) });
							}
							stringBuilder.Append(", ");
						}
					}
				}
			}
			if (base.BlockSize > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " BLOCKSIZE = {0}", new object[1] { base.BlockSize });
				stringBuilder.Append(Globals.commaspace);
			}
			if (base.BufferCount > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " BUFFERCOUNT = {0}", new object[1] { base.BufferCount });
				stringBuilder.Append(Globals.commaspace);
			}
			if (base.MaxTransferSize > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " MAXTRANSFERSIZE = {0}", new object[1] { base.MaxTransferSize });
				stringBuilder.Append(Globals.commaspace);
			}
			if (9 <= serverVersion.Major && base.Checksum)
			{
				stringBuilder.Append(" CHECKSUM, ");
			}
			if (9 <= serverVersion.Major && base.ContinueAfterError)
			{
				stringBuilder.Append(" CONTINUE_AFTER_ERROR, ");
			}
			RemoveLastComma(stringBuilder);
		}
		stringCollection.Add(stringBuilder.ToString());
		if (clearSuspectPageTableAfterRestore)
		{
			if (server.ServerVersion.Major < 9)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.UnsupportedVersion(server.ServerVersion.ToString())).SetHelpContext("UnsupportedVersion");
			}
			StringBuilder stringBuilder3 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			stringBuilder3.Append("delete from msdb.dbo.suspect_pages");
			GetDbFileFilter(stringBuilder3);
			stringCollection.Add(stringBuilder3.ToString());
		}
		return stringCollection;
	}

	public void SqlRestore(Server srv)
	{
		try
		{
			ExecuteSql(srv, Script(srv));
			if (!srv.ExecutionManager.Recording && (Action == RestoreActionType.Database || Action == RestoreActionType.Log) && !SmoApplication.eventsSingleton.IsNullDatabaseEvent())
			{
				SmoApplication.eventsSingleton.CallDatabaseEvent(srv, new DatabaseEventArgs(srv.Databases[base.Database].Urn, srv.Databases[base.Database], base.Database, DatabaseEventType.Restore));
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Restore, srv, ex);
		}
	}

	public void SqlRestoreAsync(Server srv)
	{
		try
		{
			currentAsyncOperation = AsyncOperation.Restore;
			ExecuteSqlAsync(srv, Script(srv));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RestoreAsync, srv, ex);
		}
	}

	private void RemoveLastComma(StringBuilder sb)
	{
		if (0 <= sb.Length - 2 && ',' == sb[sb.Length - 2])
		{
			sb.Remove(sb.Length - 2, 2);
		}
	}
}
