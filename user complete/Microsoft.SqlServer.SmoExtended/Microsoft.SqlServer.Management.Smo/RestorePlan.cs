using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

public class RestorePlan
{
	private RestoreOptions restoreOptions;

	private Server server;

	private string databaseName;

	private RestoreActionType restoreAction;

	private List<Restore> restoreOperations = new List<Restore>();

	private AsyncStatus asyncStatus = new AsyncStatus(ExecutionStatus.Inactive, null);

	private int processCompleted = -1;

	private int maxProcessCompleted;

	private int executingRestoreOperationIndex;

	public Server Server
	{
		get
		{
			return server;
		}
		private set
		{
			if (value == null)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.InitObject, this, new ArgumentNullException("Server"));
			}
			server = value;
			Server.ExecutionManager.ConnectionContext.StatementTimeout = 0;
		}
	}

	public string DatabaseName
	{
		get
		{
			return databaseName;
		}
		set
		{
			if (RestoreOperations != null)
			{
				foreach (Restore restoreOperation in RestoreOperations)
				{
					restoreOperation.Database = value;
				}
			}
			databaseName = value;
		}
	}

	public RestoreActionType RestoreAction
	{
		get
		{
			return restoreAction;
		}
		set
		{
			restoreAction = value;
		}
	}

	public Backup TailLogBackupOperation { get; set; }

	public List<Restore> RestoreOperations => restoreOperations;

	public AsyncStatus AsyncStatus => asyncStatus;

	public bool CloseExistingConnections { get; set; }

	public event PercentCompleteEventHandler PercentComplete;

	public event ServerMessageEventHandler NextMedia;

	public event ServerMessageEventHandler Complete;

	public event ServerMessageEventHandler Information;

	public event NextRestoreEventHandler NextRestore;

	public RestorePlan(Server server)
	{
		Server = server;
	}

	public RestorePlan(Server server, string databaseName)
	{
		Server = server;
		DatabaseName = databaseName;
	}

	public RestorePlan(Database database)
		: this(database.GetServerObject(), database.Name)
	{
	}

	public void Execute()
	{
		bool flag = true;
		processCompleted = -1;
		maxProcessCompleted = RestoreOperations.Count - 1;
		if (TailLogBackupOperation != null)
		{
			maxProcessCompleted++;
		}
		executingRestoreOperationIndex = 0;
		StringCollection stringCollection = new StringCollection();
		try
		{
			DatabaseUserAccess? dbUserAccess = ScriptPreRestore(stringCollection);
			server.ExecutionManager.ExecuteNonQueryWithMessage(stringCollection, OnInfoMessage, errorsAsMessages: true);
			stringCollection.Clear();
			int num = 1;
			bool flag2 = restoreOperations[0].Action == RestoreActionType.OnlinePage;
			foreach (Restore restoreOperation in restoreOperations)
			{
				if (flag2 && restoreOperation.Action == RestoreActionType.Log && flag && TailLogBackupOperation != null)
				{
					StringCollection stringCollection2 = new StringCollection();
					stringCollection2.Add(TailLogBackupOperation.Script(server));
					server.ExecutionManager.ExecuteNonQueryWithMessage(stringCollection2, OnInfoMessage, errorsAsMessages: true);
					flag = false;
				}
				StringCollection queries = restoreOperation.Script(server);
				NextRestoreEventArgs e = ((restoreOperation.BackupSet != null) ? new NextRestoreEventArgs(restoreOperation.BackupSet.Name, restoreOperation.BackupSet.Description, restoreOperation.Devices[0].Name, num) : new NextRestoreEventArgs(restoreOperation.Devices[0].Name, string.Empty, restoreOperation.Devices[0].Name, num));
				num++;
				if (this.NextRestore != null)
				{
					this.NextRestore(this, e);
				}
				if (e.Continue)
				{
					server.ExecutionManager.ExecuteNonQueryWithMessage(queries, OnInfoMessage, errorsAsMessages: true);
					continue;
				}
				throw new SmoException(ExceptionTemplatesImpl.OperationCancelledByUser);
			}
			ScriptPostRestore(stringCollection, dbUserAccess);
			server.ExecutionManager.ExecuteNonQueryWithMessage(stringCollection, OnInfoMessage, errorsAsMessages: true);
		}
		catch (Exception ex)
		{
			if (ex.InnerException != null)
			{
				if (!(ex.InnerException is SqlException ex2))
				{
					return;
				}
				bool flag3 = false;
				foreach (SqlError error in ex2.Errors)
				{
					if (error.Number == 3014)
					{
						flag3 = true;
					}
				}
				if (!flag3)
				{
					throw ex;
				}
				return;
			}
			throw ex;
		}
		finally
		{
			if (!server.ExecutionManager.Recording && restoreOperations[0].Action != RestoreActionType.OnlinePage)
			{
				RefreshOENode(DatabaseName);
			}
			if (!server.ExecutionManager.Recording && restoreOperations[0].Action != RestoreActionType.OnlinePage && TailLogBackupOperation != null)
			{
				RefreshOENode(TailLogBackupOperation.Database);
			}
		}
	}

	private void RefreshOENode(string databaseName)
	{
		try
		{
			server.Databases.Refresh();
			if (server.Databases[databaseName] != null && !SmoApplication.eventsSingleton.IsNullDatabaseEvent())
			{
				SmoApplication.eventsSingleton.CallDatabaseEvent(server, new DatabaseEventArgs(server.Databases[databaseName].Urn, server.Databases[databaseName], databaseName, DatabaseEventType.Restore));
			}
		}
		catch
		{
		}
	}

	public void ExecuteAsync()
	{
		executingRestoreOperationIndex = 0;
		StringCollection queries = Script();
		try
		{
			server.ExecutionManager.ExecuteNonQueryCompleted += OnExecuteNonQueryCompleted;
			asyncStatus = new AsyncStatus(ExecutionStatus.InProgress, null);
			if (this.Complete != null || this.PercentComplete != null || this.Information != null || this.NextMedia != null)
			{
				server.ExecutionManager.ExecuteNonQueryWithMessageAsync(queries, OnInfoMessage, errorsAsMessages: true);
			}
			else
			{
				server.ExecutionManager.ExecuteNonQueryAsync(queries);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RestoreAsync, server, ex);
		}
	}

	public StringCollection Script()
	{
		if (Server.Version.Major > 8)
		{
			Verify(checkBackupMediaIntegrity: false);
		}
		StringCollection stringCollection = new StringCollection();
		DatabaseUserAccess? dbUserAccess = ScriptPreRestore(stringCollection);
		ScriptRestore(stringCollection);
		ScriptPostRestore(stringCollection, dbUserAccess);
		return stringCollection;
	}

	private DatabaseUserAccess? ScriptPreRestore(StringCollection script)
	{
		bool flag = Server.DatabaseEngineEdition == DatabaseEngineEdition.SqlManagedInstance;
		script.Add(Scripts.USEMASTER);
		DatabaseUserAccess? result = null;
		bool flag2 = false;
		Database database = Server.Databases[DatabaseName];
		if (database != null && CloseExistingConnections && CanDropExistingConnections(database.Name) && database.DatabaseOptions.UserAccess != DatabaseUserAccess.Single)
		{
			result = database.DatabaseOptions.UserAccess;
			if (!flag)
			{
				if (restoreOperations[0].Action != RestoreActionType.OnlinePage && TailLogBackupOperation != null && TailLogBackupOperation.Database != DatabaseName)
				{
					script.Add(TailLogBackupOperation.Script(server));
					flag2 = true;
				}
				string value = string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] SET {1} WITH ROLLBACK IMMEDIATE", new object[2] { DatabaseName, "SINGLE_USER" });
				script.Add(value);
			}
		}
		if (!flag && !flag2 && restoreOperations[0].Action != RestoreActionType.OnlinePage && TailLogBackupOperation != null)
		{
			script.Add(TailLogBackupOperation.Script(server));
		}
		return result;
	}

	private void ScriptRestore(StringCollection script)
	{
		bool flag = true;
		bool flag2 = restoreOperations[0].Action == RestoreActionType.OnlinePage;
		foreach (Restore restoreOperation in restoreOperations)
		{
			if (flag2 && restoreOperation.Action == RestoreActionType.Log && flag && TailLogBackupOperation != null)
			{
				script.Add(TailLogBackupOperation.Script(server));
				flag = false;
			}
			StringCollection stringCollection = restoreOperation.Script(server);
			StringEnumerator enumerator2 = stringCollection.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					string current2 = enumerator2.Current;
					if (!string.IsNullOrEmpty(current2))
					{
						script.Add(current2);
					}
				}
			}
			finally
			{
				if (enumerator2 is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}
	}

	private void ScriptPostRestore(StringCollection script, DatabaseUserAccess? dbUserAccess)
	{
		if (dbUserAccess.HasValue && restoreOptions != null && restoreOptions.RecoveryState != DatabaseRecoveryState.WithNoRecovery)
		{
			string text = "MULTI_USER";
			if (dbUserAccess == DatabaseUserAccess.Restricted)
			{
				text = "RESTRICTED_USER";
			}
			string value = string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] SET {1}", new object[2] { DatabaseName, text });
			script.Add(value);
		}
	}

	public bool CanDropExistingConnections(string dbName)
	{
		if (string.IsNullOrEmpty(dbName))
		{
			return false;
		}
		Database database = Server.Databases[dbName];
		if (database == null)
		{
			return false;
		}
		CompatibilityLevel compatibilityLevel = database.GetCompatibilityLevel();
		if (compatibilityLevel < CompatibilityLevel.Version90)
		{
			return false;
		}
		if (database.Status != DatabaseStatus.Normal)
		{
			return false;
		}
		return true;
	}

	private void OnExecuteNonQueryCompleted(object sender, ExecuteNonQueryCompletedEventArgs args)
	{
		asyncStatus = new AsyncStatus(args.ExecutionStatus, args.LastException);
		server.ExecutionManager.ExecuteNonQueryCompleted -= OnExecuteNonQueryCompleted;
		if (!server.ExecutionManager.Recording && !SmoApplication.eventsSingleton.IsNullDatabaseEvent())
		{
			SmoApplication.eventsSingleton.CallDatabaseEvent(server, new DatabaseEventArgs(server.Databases[DatabaseName].Urn, server.Databases[DatabaseName], DatabaseName, DatabaseEventType.Restore));
		}
	}

	private void OnInfoMessage(object sender, ServerMessageEventArgs e)
	{
		switch (e.Error.Number)
		{
		case 3211:
			if (this.PercentComplete != null)
			{
				string empty = string.Empty;
				empty = ((TailLogBackupOperation != null && processCompleted == -1) ? ExceptionTemplatesImpl.BackupTailLog : ((TailLogBackupOperation != null && RestoreOperations[processCompleted].BackupSet == null) ? ExceptionTemplatesImpl.Restoring(ExceptionTemplatesImpl.TailLog) : ((TailLogBackupOperation == null) ? ExceptionTemplatesImpl.Restoring(RestoreOperations[processCompleted + 1].BackupSet.Name) : ExceptionTemplatesImpl.Restoring(RestoreOperations[processCompleted].BackupSet.Name))));
				this.PercentComplete(this, new PercentCompleteEventArgs(e.Error, empty));
			}
			return;
		case 3014:
			processCompleted++;
			if (this.Complete != null && processCompleted == maxProcessCompleted)
			{
				this.Complete(this, e);
			}
			return;
		case 3247:
		case 3249:
		case 4027:
		case 4028:
			if (this.NextMedia != null)
			{
				this.NextMedia(this, e);
			}
			return;
		}
		if (this.Information != null)
		{
			this.Information(this, e);
			if (e != null)
			{
				executingRestoreOperationIndex++;
			}
		}
	}

	public void Verify(bool checkBackupMediaIntegrity)
	{
		if (server.Version.Major < 9)
		{
			throw new UnsupportedVersionException(ExceptionTemplatesImpl.UnsupportedServerVersion);
		}
		if (RestoreOperations == null || RestoreOperations.Count == 0)
		{
			throw new InvalidRestorePlanException(this, ExceptionTemplatesImpl.EmptyRestorePlan);
		}
		if (string.IsNullOrEmpty(databaseName))
		{
			databaseName = RestoreOperations[0].Database;
		}
		foreach (Restore restoreOperation in RestoreOperations)
		{
			if (Server.StringComparer.Compare(databaseName, restoreOperation.Database) != 0)
			{
				throw new InvalidRestorePlanException(restoreOperation, ExceptionTemplatesImpl.MultipleDatabaseSelectedToRestore);
			}
		}
		int num = RestoreOperations.Where((Restore restoreObj) => !restoreObj.NoRecovery).Count();
		if (num > 1 || (num == 1 && restoreOperations[restoreOperations.Count - 1].NoRecovery))
		{
			throw new InvalidRestorePlanException(this, ExceptionTemplatesImpl.OnlyLastRestoreWithNoRecovery);
		}
		if (RestoreAction == RestoreActionType.Database && RestoreOperations[0].BackupSet.BackupSetType != BackupSetType.Database)
		{
			throw new InvalidRestorePlanException(this, ExceptionTemplatesImpl.NoFullBackupSelected);
		}
		if (RestoreAction == RestoreActionType.Database || RestoreAction == RestoreActionType.Log || RestoreAction == RestoreActionType.OnlinePage)
		{
			for (int num2 = 0; num2 < RestoreOperations.Count - 1; num2++)
			{
				decimal stopAtLsn = 0m;
				if (RestoreOperations[num2].BackupSet != null && RestoreOperations[num2 + 1].BackupSet != null)
				{
					string errMsg;
					object errSource;
					bool flag = BackupSet.IsBackupSetsInSequence(RestoreOperations[num2].BackupSet, RestoreOperations[num2 + 1].BackupSet, out errMsg, out errSource, ref stopAtLsn);
					if (stopAtLsn != 0m && stopAtLsn != RestoreOperations[num2].BackupSet.StopAtLsn)
					{
						flag = false;
					}
					if (!flag)
					{
						new InvalidRestorePlanException(errSource, errMsg);
					}
				}
			}
		}
		if (!checkBackupMediaIntegrity)
		{
			return;
		}
		foreach (Restore restoreOperation2 in RestoreOperations)
		{
			if (restoreOperation2.BackupSet != null)
			{
				restoreOperation2.BackupSet.Verify();
			}
		}
	}

	public void CheckBackupSetsExistence()
	{
		foreach (Restore restoreOperation in RestoreOperations)
		{
			if (restoreOperation.BackupSet != null)
			{
				restoreOperation.BackupSet.CheckBackupFilesExistence();
			}
		}
	}

	public void SetRestoreOptions(RestoreOptions restoreOptions)
	{
		this.restoreOptions = restoreOptions;
		if (RestoreOperations == null || RestoreOperations.Count == 0)
		{
			return;
		}
		foreach (Restore restoreOperation in RestoreOperations)
		{
			restoreOperation.PercentCompleteNotification = restoreOptions.PercentCompleteNotification;
			restoreOperation.ContinueAfterError = restoreOptions.ContinueAfterError;
			restoreOperation.ClearSuspectPageTableAfterRestore = restoreOptions.ClearSuspectPageTableAfterRestore;
			restoreOperation.BlockSize = restoreOptions.Blocksize;
			restoreOperation.BufferCount = restoreOptions.BufferCount;
			restoreOperation.MaxTransferSize = restoreOptions.MaxTransferSize;
			restoreOperation.RestrictedUser = restoreOptions.SetRestrictedUser;
			restoreOperation.NoRecovery = true;
		}
		if (TailLogBackupOperation != null)
		{
			TailLogBackupOperation.PercentCompleteNotification = restoreOptions.PercentCompleteNotification;
			TailLogBackupOperation.ContinueAfterError = restoreOptions.ContinueAfterError;
			TailLogBackupOperation.BlockSize = restoreOptions.Blocksize;
			TailLogBackupOperation.BufferCount = restoreOptions.BufferCount;
			TailLogBackupOperation.MaxTransferSize = restoreOptions.MaxTransferSize;
		}
		Restore restore = restoreOperations[0];
		restore.ReplaceDatabase = restoreOptions.ReplaceDatabase;
		int index = restoreOperations.Count - 1;
		Restore restore2 = restoreOperations[index];
		switch (restoreOptions.RecoveryState)
		{
		case DatabaseRecoveryState.WithNoRecovery:
			restore2.NoRecovery = true;
			break;
		case DatabaseRecoveryState.WithRecovery:
			restore2.NoRecovery = false;
			break;
		case DatabaseRecoveryState.WithStandBy:
			restore2.NoRecovery = false;
			restore2.StandbyFile = restoreOptions.StandByFile;
			break;
		}
		restore2.KeepReplication = restoreOptions.KeepReplication;
		restore2.KeepTemporalRetention = restoreOptions.KeepTemporalRetention;
	}

	public void AddRestoreOperation(BackupSet backupSet)
	{
		Restore item = new Restore(DatabaseName, backupSet);
		RestoreOperations.Add(item);
	}

	public void AddRestoreOperation(List<BackupSet> backupSets)
	{
		foreach (BackupSet backupSet in backupSets)
		{
			try
			{
				Restore item = new Restore(DatabaseName, backupSet);
				RestoreOperations.Add(item);
			}
			catch (SmoException)
			{
			}
		}
	}
}
