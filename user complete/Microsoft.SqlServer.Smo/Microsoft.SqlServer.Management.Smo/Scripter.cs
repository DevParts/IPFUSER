using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

public class Scripter : DependencyWalker
{
	private ProgressReportEventHandler scriptingProgress;

	private ScriptingErrorEventHandler scriptingError;

	private ScriptingOptions scriptingOptions;

	private bool prefetchObjects = true;

	public ScriptingOptions Options
	{
		get
		{
			if (scriptingOptions == null)
			{
				scriptingOptions = new ScriptingOptions();
			}
			return scriptingOptions;
		}
		set
		{
			if (value == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("Options"));
			}
			scriptingOptions = value;
		}
	}

	public bool PrefetchObjects
	{
		get
		{
			if (prefetchObjects)
			{
				return !base.Server.IsDesignMode;
			}
			return false;
		}
		set
		{
			prefetchObjects = value;
		}
	}

	public event ProgressReportEventHandler ScriptingProgress
	{
		add
		{
			if (!SqlContext.IsAvailable)
			{
				scriptingProgress = (ProgressReportEventHandler)Delegate.Combine(scriptingProgress, value);
			}
		}
		remove
		{
			scriptingProgress = (ProgressReportEventHandler)Delegate.Remove(scriptingProgress, value);
		}
	}

	public event ScriptingErrorEventHandler ScriptingError
	{
		add
		{
			if (!SqlContext.IsAvailable)
			{
				scriptingError = (ScriptingErrorEventHandler)Delegate.Combine(scriptingError, value);
			}
		}
		remove
		{
			scriptingError = (ScriptingErrorEventHandler)Delegate.Remove(scriptingError, value);
		}
	}

	public Scripter()
	{
		Init();
	}

	public Scripter(Server svr)
		: base(svr)
	{
		if (svr == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("svr"));
		}
		Init();
	}

	protected internal void Init()
	{
	}

	internal ScriptingOptions GetOptions()
	{
		ScriptingOptions options = Options;
		if (!options.GetScriptingPreferences().TargetVersionAndDatabaseEngineTypeDirty)
		{
			if (GetServerObject().ServerVersion == null)
			{
				string versionString = GetServerObject().Information.VersionString;
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, versionString);
			}
			options.SetTargetServerInfo(GetServerObject(), forced: false);
		}
		return options;
	}

	public StringCollection ScriptWithList(SqlSmoObject[] objects)
	{
		if (GetOptions().ScriptData)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ScriptDataNotSupportedByThisMethod);
		}
		return EnumerableContainer.IEnumerableToStringCollection(EnumScriptWithList(objects));
	}

	public IEnumerable<string> EnumScriptWithList(SqlSmoObject[] objects)
	{
		if (objects == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("objects"));
		}
		DependencyCollection dependencyCollection = new DependencyCollection();
		foreach (SqlSmoObject sqlSmoObject in objects)
		{
			dependencyCollection.Add(new DependencyCollectionNode(sqlSmoObject.Urn, isSchemaBound: true, fRoot: true));
		}
		return ScriptWithList(dependencyCollection, objects);
	}

	public StringCollection ScriptWithList(UrnCollection list)
	{
		if (GetOptions().ScriptData)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ScriptDataNotSupportedByThisMethod);
		}
		return EnumerableContainer.IEnumerableToStringCollection(EnumScriptWithList(list));
	}

	public IEnumerable<string> EnumScriptWithList(UrnCollection list)
	{
		if (list == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("list"));
		}
		return ScriptWithList(list, null);
	}

	internal IEnumerable<string> ScriptWithList(UrnCollection list, SqlSmoObject[] objects)
	{
		DependencyCollection dependencyCollection = new DependencyCollection();
		for (int i = 0; i < list.Count; i++)
		{
			dependencyCollection.Add(new DependencyCollectionNode(list[i], isSchemaBound: true, fRoot: true));
		}
		return ScriptWithList(dependencyCollection, objects);
	}

	public StringCollection ScriptWithList(Urn[] urns)
	{
		if (GetOptions().ScriptData)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ScriptDataNotSupportedByThisMethod);
		}
		return EnumerableContainer.IEnumerableToStringCollection(EnumScriptWithList(urns));
	}

	public IEnumerable<string> EnumScriptWithList(Urn[] urns)
	{
		if (urns == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("urns"));
		}
		return ScriptWithList(urns, null);
	}

	internal IEnumerable<string> ScriptWithList(Urn[] urns, SqlSmoObject[] objects)
	{
		DependencyCollection dependencyCollection = new DependencyCollection();
		foreach (Urn urn in urns)
		{
			dependencyCollection.Add(new DependencyCollectionNode(urn, isSchemaBound: true, fRoot: true));
		}
		return ScriptWithList(dependencyCollection, objects);
	}

	public StringCollection ScriptWithList(DependencyCollection depList)
	{
		if (GetOptions().ScriptData)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ScriptDataNotSupportedByThisMethod);
		}
		return EnumerableContainer.IEnumerableToStringCollection(EnumScriptWithList(depList));
	}

	public IEnumerable<string> EnumScriptWithList(DependencyCollection depList)
	{
		return ScriptWithList(depList, null);
	}

	internal IEnumerable<string> ScriptWithList(DependencyCollection depList, SqlSmoObject[] objects)
	{
		return ScriptWithList(depList, objects, discoveryRequired: false);
	}

	private IEnumerable<string> ScriptWithList(DependencyCollection depList, SqlSmoObject[] objects, bool discoveryRequired)
	{
		try
		{
			return ScriptWithListWorker(depList, objects, discoveryRequired);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			SqlSmoObject sqlSmoObject = base.Server;
			if (objects != null && objects.Length == 1 && objects[0] != null)
			{
				sqlSmoObject = objects[0];
				if (ex is PropertyCannotBeRetrievedException)
				{
					FailedOperationException ex2 = new FailedOperationException(ExceptionTemplatesImpl.FailedOperationExceptionTextScript(SqlSmoObject.GetTypeName(sqlSmoObject.GetType().Name), sqlSmoObject.ToString()), ex);
					ex2.Operation = ExceptionTemplatesImpl.Script;
					ex2.FailedObject = sqlSmoObject;
					throw ex2;
				}
			}
			else if (depList != null && depList.Count == 1 && depList[0] != null)
			{
				try
				{
					sqlSmoObject = base.Server.GetSmoObject(depList[0].Urn);
				}
				catch
				{
				}
			}
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sqlSmoObject, "null == mainObj");
			throw new FailedOperationException(ExceptionTemplatesImpl.Script, sqlSmoObject, ex);
		}
	}

	private IEnumerable<string> ScriptWithListWorker(DependencyCollection depList, SqlSmoObject[] objects, bool discoveryRequired)
	{
		ScriptingOptions options = GetOptions();
		CheckConflictingOptions();
		EnumerableContainer enumerableContainer = new EnumerableContainer();
		Server server = ((base.Server != null) ? base.Server : objects[0].GetServerObject());
		ScriptMaker scriptMaker = new ScriptMaker(server);
		scriptMaker.SourceDatabaseEngineEdition = ((objects == null || objects.Length == 0) ? server.DatabaseEngineEdition : objects[0].DatabaseEngineEdition);
		scriptMaker.Preferences = options.GetScriptingPreferences();
		scriptMaker.discoverer = GetDiscoverer(server, options, discoveryRequired);
		SmoUrnFilter smoUrnFilterForFiltering = options.GetSmoUrnFilterForFiltering(server);
		if (smoUrnFilterForFiltering != null)
		{
			scriptMaker.Filter = smoUrnFilterForFiltering;
		}
		if (scriptingError != null)
		{
			scriptMaker.ScriptingError += scriptingError;
		}
		if (scriptingProgress != null)
		{
			scriptMaker.ObjectScripting += scriptMaker_ObjectScripting;
		}
		scriptMaker.Prefetch = PrefetchObjects;
		if (!string.IsNullOrEmpty(options.FileName) && options.ToFileOnly)
		{
			if (SqlContext.IsAvailable)
			{
				throw new Exception(ExceptionTemplatesImpl.SmoSQLCLRUnAvailable);
			}
			using SingleFileWriter singleFileWriter = new SingleFileWriter(options.FileName, options.AppendToFile, options.Encoding);
			singleFileWriter.ScriptBatchTerminator = !options.NoCommandTerminator;
			singleFileWriter.BatchTerminator = Globals.Go;
			scriptMaker.Script(depList, objects, singleFileWriter);
		}
		else
		{
			SmoStringWriter smoStringWriter = new SmoStringWriter();
			scriptMaker.Script(depList, objects, smoStringWriter);
			enumerableContainer.Add(smoStringWriter.FinalStringCollection);
			if (!string.IsNullOrEmpty(options.FileName))
			{
				WriteToFile(options, enumerableContainer);
			}
		}
		return enumerableContainer;
	}

	private void scriptMaker_ObjectScripting(object sender, ObjectScriptingEventArgs e)
	{
		scriptingProgress(this, new ProgressReportEventArgs(e.Current, null, 1, 1, e.CurrentCount, e.Total));
	}

	private ISmoDependencyDiscoverer GetDiscoverer(Server server, ScriptingOptions so, bool discoveryRequired)
	{
		SmoDependencyDiscoverer smoDependencyDiscoverer = new SmoDependencyDiscoverer(base.Server);
		smoDependencyDiscoverer.Preferences = so.GetScriptingPreferences();
		smoDependencyDiscoverer.Preferences.DependentObjects = discoveryRequired;
		smoDependencyDiscoverer.filteredUrnTypes = so.GetSmoUrnFilterForDiscovery(server).filteredTypes;
		return smoDependencyDiscoverer;
	}

	private void WriteToFile(ScriptingOptions options, IEnumerable<string> queryEnumerable)
	{
		StreamWriter streamWriter = null;
		if (SqlContext.IsAvailable)
		{
			throw new Exception(ExceptionTemplatesImpl.SmoSQLCLRUnAvailable);
		}
		Stream stream = new FileStream(options.FileName, options.AppendToFile ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read);
		streamWriter = new StreamWriter(stream, options.Encoding);
		foreach (string item in queryEnumerable)
		{
			streamWriter.WriteLine(item);
			if (!options.NoCommandTerminator)
			{
				streamWriter.WriteLine(Globals.Go);
			}
		}
		streamWriter.Flush();
		streamWriter.Close();
	}

	private void CheckConflictingOptions()
	{
		ScriptingOptions options = GetOptions();
		if (options.DdlBodyOnly && options.DdlHeaderOnly)
		{
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.ConflictingScriptingOptions(Enum.GetName(typeof(EnumScriptOptions), EnumScriptOptions.DdlBodyOnly), Enum.GetName(typeof(EnumScriptOptions), EnumScriptOptions.DdlHeaderOnly)));
		}
		if (!options.ScriptData && !options.ScriptSchema)
		{
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.InvalidScriptingOutput(Enum.GetName(typeof(EnumScriptOptions), EnumScriptOptions.ScriptData), Enum.GetName(typeof(EnumScriptOptions), EnumScriptOptions.ScriptSchema)));
		}
		if (!options.ScriptSchema && options.ScriptForAlter)
		{
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.ConflictingScriptingOptions(Enum.GetName(typeof(EnumScriptOptions), EnumScriptOptions.ScriptSchema), "ScriptForAlter"));
		}
	}

	public StringCollection Script(SqlSmoObject[] objects)
	{
		if (GetOptions().ScriptData)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ScriptDataNotSupportedByThisMethod);
		}
		return EnumerableContainer.IEnumerableToStringCollection(EnumScript(objects));
	}

	public IEnumerable<string> EnumScript(SqlSmoObject[] objects)
	{
		if (objects == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("objects"));
		}
		Urn[] array = new Urn[objects.Length];
		for (int i = 0; i < objects.Length; i++)
		{
			array[i] = objects[i].Urn;
		}
		return Script(array, objects);
	}

	public StringCollection Script(UrnCollection list)
	{
		if (GetOptions().ScriptData)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ScriptDataNotSupportedByThisMethod);
		}
		return EnumerableContainer.IEnumerableToStringCollection(EnumScript(list));
	}

	public IEnumerable<string> EnumScript(UrnCollection list)
	{
		if (list == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("list"));
		}
		Urn[] array = new Urn[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			array[i] = list[i];
		}
		return Script(array, null);
	}

	public StringCollection Script(Urn[] urns)
	{
		if (GetOptions().ScriptData)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ScriptDataNotSupportedByThisMethod);
		}
		return EnumerableContainer.IEnumerableToStringCollection(EnumScript(urns));
	}

	public IEnumerable<string> EnumScript(Urn[] urns)
	{
		if (urns == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("urns"));
		}
		return Script(urns, null);
	}

	internal IEnumerable<string> Script(Urn[] urns, SqlSmoObject[] objects)
	{
		CheckConflictingOptions();
		IEnumerable<string> enumerable = null;
		if (GetOptions().WithDependencies)
		{
			DependencyCollection dependencyCollection = new DependencyCollection();
			foreach (Urn urn in urns)
			{
				dependencyCollection.Add(new DependencyCollectionNode(urn, isSchemaBound: true, fRoot: true));
			}
			return ScriptWithList(dependencyCollection, objects, discoveryRequired: true);
		}
		return ScriptWithList(urns, objects);
	}

	public static UrnCollection EnumDependencies(SqlSmoObject smoObject, DependencyType dependencyType)
	{
		if (smoObject == null)
		{
			throw new ArgumentNullException("smoObject");
		}
		DependencyRequest dependencyRequest = new DependencyRequest();
		dependencyRequest.Urns = new Urn[1] { smoObject.Urn };
		dependencyRequest.ParentDependencies = dependencyType == DependencyType.Parents;
		DependencyChainCollection dependencies = smoObject.ExecutionManager.GetDependencies(dependencyRequest);
		UrnCollection urnCollection = new UrnCollection();
		for (int i = 0; i < dependencies.Count; i++)
		{
			urnCollection.Add(dependencies[i].Urn);
		}
		return urnCollection;
	}

	internal StringCollection Script(SqlSmoObject sqlSmoObject)
	{
		StoreAndChangeOptions(sqlSmoObject, out var originalWithDependencies, out var sqlServerVersion);
		try
		{
			return Script(new SqlSmoObject[1] { sqlSmoObject });
		}
		finally
		{
			RestoreOptions(originalWithDependencies, sqlServerVersion);
		}
	}

	internal IEnumerable<string> EnumScript(SqlSmoObject sqlSmoObject)
	{
		StoreAndChangeOptions(sqlSmoObject, out var originalWithDependencies, out var sqlServerVersion);
		try
		{
			return EnumScript(new SqlSmoObject[1] { sqlSmoObject });
		}
		finally
		{
			RestoreOptions(originalWithDependencies, sqlServerVersion);
		}
	}

	private void RestoreOptions(bool originalWithDependencies, SqlServerVersion sqlServerVersion)
	{
		Options.WithDependencies = originalWithDependencies;
		if (Options.TargetDatabaseEngineType != DatabaseEngineType.Standalone && Options.TargetServerVersion != sqlServerVersion)
		{
			Options.TargetServerVersion = sqlServerVersion;
		}
	}

	private void StoreAndChangeOptions(SqlSmoObject sqlSmoObject, out bool originalWithDependencies, out SqlServerVersion sqlServerVersion)
	{
		if (!Options.WithDependencies)
		{
			PrefetchObjects = !sqlSmoObject.InitializedForScripting;
		}
		originalWithDependencies = Options.WithDependencies;
		if (sqlSmoObject.State == SqlSmoState.Creating || sqlSmoObject.IsDesignMode)
		{
			Options.WithDependencies = false;
		}
		sqlServerVersion = Options.TargetServerVersion;
		if (Options.TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			if (!sqlSmoObject.IsCloudSupported)
			{
				throw new UnsupportedEngineTypeException(ExceptionTemplatesImpl.UnsupportedEngineTypeException);
			}
			Options.TargetServerVersion = SqlServerVersion.Version120;
		}
	}
}
