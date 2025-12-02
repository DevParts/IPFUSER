using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class DataTransferBase : TransferBase
{
	internal bool destinationAnsiPadding;

	internal StringCollection compensationScript;

	internal bool LogTransferDumps { get; set; }

	public DataTransferBase()
	{
	}

	public DataTransferBase(Database database)
		: base(database)
	{
	}

	internal TransferWriter GetScriptLoadedTransferWriter()
	{
		if (string.IsNullOrEmpty(base.DestinationDatabase))
		{
			throw new PropertyNotSetException("DestinationDatabase");
		}
		double? modelSize = SetTargetServerInfoAndGetModelSize();
		Dictionary<string, string> strAryOldDbFileNames = null;
		Dictionary<string, string> strAryOldLogFileNames = null;
		Dictionary<string, double> oldFileSizes = null;
		string scriptName = base.Database.ScriptName;
		compensationScript = null;
		UrnCollection urnCollection = EnumObjects(ordered: false);
		if (base.CopySchema)
		{
			ProcessObjectList(urnCollection);
		}
		ScriptMaker scriptMaker = GetScriptMaker();
		TransferWriter transferWriter = new TransferWriter(this, scriptMaker);
		string fileStreamFolder = null;
		try
		{
			if (base.CreateTargetDatabase)
			{
				if (base.Options.TargetDatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
				{
					fileStreamFolder = SetupDatabaseForDestinationScripting(modelSize, out strAryOldDbFileNames, out strAryOldLogFileNames, out oldFileSizes);
				}
				base.Database.ScriptName = base.DestinationDatabase;
				urnCollection.Add(base.Database.Urn);
			}
			transferWriter.SetEvents();
			scriptMaker.Script(urnCollection, transferWriter);
			transferWriter.ResetEvents();
			ScriptCompensation(scriptMaker);
			return transferWriter;
		}
		finally
		{
			if (base.CreateTargetDatabase)
			{
				if (base.Options.TargetDatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
				{
					ResetDatabaseForDestinationScripting(modelSize, strAryOldDbFileNames, strAryOldLogFileNames, oldFileSizes, fileStreamFolder);
				}
				base.Database.ScriptName = scriptName;
			}
		}
	}

	private void ScriptCompensation(ScriptMaker scriptMaker)
	{
		if (base.CreateTargetDatabase)
		{
			scriptMaker.Preferences.IncludeScripts.ExistenceCheck = true;
			scriptMaker.Preferences.Behavior = ScriptBehavior.Drop;
			scriptMaker.Preferences.IncludeScripts.Ddl = true;
			scriptMaker.Preferences.IncludeScripts.Data = false;
			scriptMaker.Preferences.IncludeScripts.DatabaseContext = false;
			compensationScript = scriptMaker.Script(new Urn[1] { base.Database.Urn });
			compensationScript.Insert(0, string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket("master") }));
		}
	}

	private void ProcessObjectList(UrnCollection urnList)
	{
		foreach (Urn urn in urnList)
		{
			string type;
			if ((type = urn.Type) != null && type == "FullTextCatalog")
			{
				FullTextCatalog fullTextCatalog = base.Database.Parent.GetSmoObject(urn) as FullTextCatalog;
				fullTextCatalog.RootPath = string.Empty;
			}
		}
	}

	private void ResetDatabaseForDestinationScripting(double? modelSize, Dictionary<string, string> strAryOldDbFileNames, Dictionary<string, string> strAryOldLogFileNames, Dictionary<string, double> oldFileSizes, string fileStreamFolder)
	{
		if (fileStreamFolder != null)
		{
			base.Database.FilestreamDirectoryName = fileStreamFolder;
		}
		if (strAryOldDbFileNames.Count > 0 || oldFileSizes.Count > 0)
		{
			foreach (FileGroup fileGroup in base.Database.FileGroups)
			{
				foreach (DataFile file in fileGroup.Files)
				{
					string value = null;
					if (strAryOldDbFileNames.TryGetValue(file.FileName, out value))
					{
						file.FileName = value;
					}
					double value2 = 0.0;
					if (oldFileSizes.TryGetValue(file.FileName, out value2))
					{
						file.Size = value2;
					}
				}
			}
		}
		if (strAryOldLogFileNames.Count <= 0)
		{
			return;
		}
		foreach (LogFile logFile in base.Database.LogFiles)
		{
			string value3 = null;
			if (strAryOldLogFileNames.TryGetValue(logFile.FileName, out value3))
			{
				logFile.FileName = value3;
			}
		}
	}

	private double? SetTargetServerInfoAndGetModelSize()
	{
		double? result = null;
		try
		{
			ServerConnection serverConnection = new ServerConnection(base.DestinationServer);
			serverConnection.LoginSecure = base.DestinationLoginSecure;
			if (!base.DestinationLoginSecure)
			{
				serverConnection.Login = base.DestinationLogin;
				serverConnection.Password = base.DestinationPassword;
			}
			serverConnection.NonPooledConnection = true;
			serverConnection.Connect();
			Server server = new Server(serverConnection);
			destinationAnsiPadding = server.UserOptions.AnsiPadding;
			base.Scripter.Options.SetTargetServerInfo(server, forced: true);
			result = GetModelDatabasePrimaryFileSize(server);
			serverConnection.Disconnect();
		}
		catch (Exception ex)
		{
			if (ex is OutOfMemoryException)
			{
				throw;
			}
		}
		return result;
	}

	private double GetModelDatabasePrimaryFileSize(Server destServer)
	{
		Database database = destServer.Databases["model"];
		double result = 0.0;
		foreach (FileGroup fileGroup in database.FileGroups)
		{
			foreach (DataFile file in fileGroup.Files)
			{
				if (file.IsPrimaryFile)
				{
					result = file.Size;
					break;
				}
			}
		}
		return result;
	}

	private string SetupDatabaseForDestinationScripting(double? modelSize, out Dictionary<string, string> strAryOldDbFileNames, out Dictionary<string, string> strAryOldLogFileNames, out Dictionary<string, double> oldFileSizes)
	{
		strAryOldDbFileNames = new Dictionary<string, string>();
		strAryOldLogFileNames = new Dictionary<string, string>();
		oldFileSizes = new Dictionary<string, double>();
		string result = null;
		foreach (FileGroup fileGroup in base.Database.FileGroups)
		{
			bool flag = false;
			foreach (DataFile file in fileGroup.Files)
			{
				if (file.IsPrimaryFile && modelSize.HasValue && file.Size < modelSize.Value)
				{
					oldFileSizes.Add(file.FileName, file.Size);
					file.Size = modelSize.Value;
				}
				if (base.DatabaseFileMappings.ContainsKey(file.FileName))
				{
					strAryOldDbFileNames.Add(base.DatabaseFileMappings[file.FileName], file.FileName);
					file.FileName = base.DatabaseFileMappings[file.FileName];
					if (base.Database.IsSupportedProperty("FilestreamDirectoryName") && fileGroup.IsFileStream && !flag && base.Database.FilestreamDirectoryName != null)
					{
						result = base.Database.FilestreamDirectoryName;
						base.Database.FilestreamDirectoryName = Path.GetFileName(file.FileName);
						flag = true;
					}
				}
				else if (!string.IsNullOrEmpty(base.TargetDatabaseFilePath))
				{
					string fileName = Path.GetFileName(file.FileName);
					string text = PathWrapper.Combine(base.TargetDatabaseFilePath, fileName);
					strAryOldDbFileNames.Add(text, file.FileName);
					file.FileName = text;
				}
			}
		}
		foreach (LogFile logFile in base.Database.LogFiles)
		{
			if (base.DatabaseFileMappings.ContainsKey(logFile.FileName))
			{
				strAryOldLogFileNames.Add(base.DatabaseFileMappings[logFile.FileName], logFile.FileName);
				logFile.FileName = base.DatabaseFileMappings[logFile.FileName];
			}
			else if (!string.IsNullOrEmpty(base.TargetLogFilePath))
			{
				string fileName2 = Path.GetFileName(logFile.FileName);
				string text2 = PathWrapper.Combine(base.TargetLogFilePath, fileName2);
				strAryOldLogFileNames.Add(text2, logFile.FileName);
				logFile.FileName = text2;
			}
		}
		return result;
	}

	private ScriptMaker GetScriptMaker()
	{
		ScriptMaker scriptMaker = new ScriptMaker(base.Database.Parent);
		scriptMaker.Preferences = (ScriptingPreferences)base.Scripter.Options.GetScriptingPreferences().Clone();
		scriptMaker.Preferences.IncludeScripts.Ddl = base.CopySchema;
		scriptMaker.Preferences.IncludeScripts.Data = base.CopyData;
		scriptMaker.Preferences.IncludeScripts.DatabaseContext = false;
		scriptMaker.Prefetch = false;
		if (base.DropDestinationObjectsFirst)
		{
			scriptMaker.Preferences.Behavior = ScriptBehavior.DropAndCreate;
		}
		else
		{
			scriptMaker.Preferences.Behavior = ScriptBehavior.Create;
		}
		SmoDependencyDiscoverer smoDependencyDiscoverer = new SmoDependencyDiscoverer(base.Database.Parent);
		smoDependencyDiscoverer.Preferences = scriptMaker.Preferences;
		smoDependencyDiscoverer.Preferences.DependentObjects = false;
		smoDependencyDiscoverer.filteredUrnTypes = base.Options.GetSmoUrnFilterForDiscovery(base.Database.Parent).filteredTypes;
		scriptMaker.discoverer = smoDependencyDiscoverer;
		return scriptMaker;
	}
}
