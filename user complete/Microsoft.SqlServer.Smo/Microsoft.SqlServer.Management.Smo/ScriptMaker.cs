using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

public class ScriptMaker
{
	private IDatabasePrefetch currentDatabasePrefetch;

	private string currentlyScriptingDatabase;

	private HashSet<Urn> inputList;

	private SortedList<string, HashSet<Urn>> perDatabaseUrns;

	private SortedList<string, bool> prefetchableObjects;

	private bool multipleDatabases;

	private ScriptingErrorEventHandler scriptingError;

	private int totalObjectsToScript;

	private HashSet<Urn> ObjectsToScript;

	private ObjectScriptingEventHandler objectScripting;

	private ScriptingProgressEventHandler scriptingProgress;

	private RetryRequestedEventArgs currentRetryArgs;

	private RetryRequestedEventHandler retry;

	private ISmoScriptWriter writer;

	internal ISmoDependencyDiscoverer discoverer;

	private CreatingObjectDictionary creatingDictionary;

	private ScriptContainerFactory scriptContainerFactory;

	public Server Server { get; set; }

	public bool Prefetch { get; set; }

	public DatabaseEngineEdition? SourceDatabaseEngineEdition { get; set; }

	internal IDatabasePrefetch DatabasePrefetch { get; set; }

	public ScriptingPreferences Preferences { get; set; }

	internal ISmoFilter Filter { get; set; }

	public ISmoDependencyDiscoverer Discoverer
	{
		get
		{
			return discoverer;
		}
		set
		{
			discoverer = value;
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

	internal event ObjectScriptingEventHandler ObjectScripting
	{
		add
		{
			objectScripting = (ObjectScriptingEventHandler)Delegate.Combine(objectScripting, value);
		}
		remove
		{
			objectScripting = (ObjectScriptingEventHandler)Delegate.Remove(objectScripting, value);
		}
	}

	internal event ScriptingProgressEventHandler ScriptingProgress
	{
		add
		{
			scriptingProgress = (ScriptingProgressEventHandler)Delegate.Combine(scriptingProgress, value);
		}
		remove
		{
			scriptingProgress = (ScriptingProgressEventHandler)Delegate.Remove(scriptingProgress, value);
		}
	}

	internal event RetryRequestedEventHandler Retry
	{
		add
		{
			retry = (RetryRequestedEventHandler)Delegate.Combine(retry, value);
		}
		remove
		{
			retry = (RetryRequestedEventHandler)Delegate.Remove(retry, value);
		}
	}

	public ScriptMaker()
	{
		Preferences = new ScriptingPreferences();
		discoverer = null;
		creatingDictionary = null;
		Prefetch = true;
	}

	public ScriptMaker(Server server)
	{
		if (server == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("server"));
		}
		Server = server;
		discoverer = null;
		Prefetch = true;
		creatingDictionary = new CreatingObjectDictionary(server);
		Preferences = new ScriptingPreferences(server);
	}

	public ScriptMaker(Server server, ScriptingOptions scriptingOptions)
		: this(server)
	{
		Preferences = scriptingOptions.GetScriptingPreferences();
	}

	private void Script(SqlSmoObject[] objects, ISmoScriptWriter writer)
	{
		if (objects == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("objects"));
		}
		StoreObjects(objects);
		List<Urn> urns = new List<SqlSmoObject>(objects).ConvertAll((SqlSmoObject p) => p.Urn);
		ScriptWorker(urns, writer);
	}

	public StringCollection Script(SqlSmoObject[] objects)
	{
		if (objects == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("objects"));
		}
		SmoStringWriter smoStringWriter = new SmoStringWriter();
		Script(objects, smoStringWriter);
		return smoStringWriter.FinalStringCollection;
	}

	internal void Script(Urn[] urns, ISmoScriptWriter writer)
	{
		if (urns == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("urns"));
		}
		List<Urn> urns2 = new List<Urn>(urns);
		ScriptWorker(urns2, writer);
	}

	public StringCollection Script(Urn[] urns)
	{
		if (urns == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("urns"));
		}
		SmoStringWriter smoStringWriter = new SmoStringWriter();
		Script(urns, smoStringWriter);
		return smoStringWriter.FinalStringCollection;
	}

	internal void Script(UrnCollection list, ISmoScriptWriter writer)
	{
		if (list == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("list"));
		}
		List<Urn> urns = new List<Urn>(list);
		ScriptWorker(urns, writer);
	}

	internal void Script(DependencyCollection depList, SqlSmoObject[] objects, ISmoScriptWriter writer)
	{
		if (depList == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("list"));
		}
		if (objects != null)
		{
			StoreObjects(objects);
		}
		List<Urn> list = new List<Urn>();
		foreach (DependencyCollectionNode dep in depList)
		{
			list.Add(dep.Urn);
		}
		ScriptWorker(list, writer);
	}

	private void StoreObjects(SqlSmoObject[] objects)
	{
		if (objects.Length > 0)
		{
			creatingDictionary = new CreatingObjectDictionary(objects[0].GetServerObject());
			foreach (SqlSmoObject obj in objects)
			{
				creatingDictionary.Add(obj);
			}
		}
	}

	public StringCollection Script(UrnCollection list)
	{
		if (list == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("list"));
		}
		SmoStringWriter smoStringWriter = new SmoStringWriter();
		Script(list, smoStringWriter);
		return smoStringWriter.FinalStringCollection;
	}

	internal StringCollection Script(SqlSmoObject obj)
	{
		throw new InvalidOperationException();
	}

	private void ScriptWorker(List<Urn> urns, ISmoScriptWriter writer)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Trace("ScriptMaker", "ScriptWorker invoked for {0} Urns: {1}", urns.Count, string.Join(System.Environment.NewLine + "\t", urns.Select((Urn urn) => urn.Value).ToArray()));
		if (writer == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("writer"));
		}
		this.writer = writer;
		currentRetryArgs = null;
		scriptContainerFactory = null;
		Verify(urns);
		OnScriptingProgress(ScriptingProgressStages.VerificationDone, urns);
		if (creatingDictionary == null)
		{
			creatingDictionary = new CreatingObjectDictionary(Server);
		}
		if (Preferences.IncludeScripts.ScriptingParameterHeader)
		{
			string header = string.Format(CultureInfo.CurrentUICulture, "/*    =={0}==\r\n\r\n    {1}{2} : {3}\r\n    {4} : {5}\r\n\r\n    {6}{7} : {8}\r\n    {9} : {10}\r\n*/", LocalizableResources.ScriptingParameters, (Server.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase) ? string.Empty : string.Format(CultureInfo.CurrentUICulture, "{0} : {1} ({2}){3}    ", LocalizableResources.SourceServerVersion, TypeConverters.SqlServerVersionTypeConverter.ConvertToString(ScriptingOptions.ConvertVersion(Server.Version)), Server.Version, System.Environment.NewLine), LocalizableResources.SourceDatabaseEngineEdition, Microsoft.SqlServer.Management.Common.TypeConverters.DatabaseEngineEditionTypeConverter.ConvertToString(SourceDatabaseEngineEdition ?? Server.DatabaseEngineEdition), LocalizableResources.SourceDatabaseEngineType, Microsoft.SqlServer.Management.Common.TypeConverters.DatabaseEngineTypeTypeConverter.ConvertToString(Server.DatabaseEngineType), (Preferences.TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase) ? string.Empty : string.Format(CultureInfo.CurrentUICulture, "{0} : {1}{2}    ", new object[3]
			{
				LocalizableResources.TargetServerVersion,
				TypeConverters.SqlServerVersionTypeConverter.ConvertToString(Preferences.TargetServerVersion),
				System.Environment.NewLine
			}), LocalizableResources.TargetDatabaseEngineEdition, Microsoft.SqlServer.Management.Common.TypeConverters.DatabaseEngineEditionTypeConverter.ConvertToString(Preferences.TargetDatabaseEngineEdition), LocalizableResources.TargetDatabaseEngineType, Microsoft.SqlServer.Management.Common.TypeConverters.DatabaseEngineTypeTypeConverter.ConvertToString(Preferences.TargetDatabaseEngineType));
			this.writer.Header = header;
		}
		if (Preferences.ScriptForAlter)
		{
			ScriptAlterObjects(urns);
			return;
		}
		InitializeCurrentDatabasePrefetch();
		if (!Server.IsDesignMode && Server.DatabaseEngineType != DatabaseEngineType.SqlAzureDatabase && Prefetch && currentDatabasePrefetch == null)
		{
			foreach (IEnumerable<Urn> item in SingleDatabaseUrns(urns))
			{
				DiscoverOrderScript(item);
			}
		}
		else
		{
			DiscoverOrderScript(urns);
		}
		CleanUp();
	}

	private void CleanUp()
	{
		creatingDictionary = null;
		scriptContainerFactory = null;
	}

	private void InitializeCurrentDatabasePrefetch()
	{
		multipleDatabases = false;
		inputList = new HashSet<Urn>();
		if (Prefetch)
		{
			currentDatabasePrefetch = DatabasePrefetch;
		}
		else
		{
			currentDatabasePrefetch = null;
		}
		if (currentDatabasePrefetch != null)
		{
			currentDatabasePrefetch.creatingDictionary = creatingDictionary;
			if (currentDatabasePrefetch is GswDatabasePrefetch gswDatabasePrefetch)
			{
				gswDatabasePrefetch.PrefetchBatchEvent += OnPrefetchBatchEvent;
			}
		}
	}

	private void OnPrefetchBatchEvent(object sender, PrefetchBatchEventArgs e)
	{
		if (e.TotalBatchCount > 1 && scriptContainerFactory == null)
		{
			HashSet<UrnTypeKey> filteredTypes = new HashSet<UrnTypeKey>();
			if (Filter is SmoUrnFilter smoUrnFilter)
			{
				filteredTypes = smoUrnFilter.filteredTypes;
			}
			scriptContainerFactory = new ScriptContainerFactory(Preferences, filteredTypes, retry);
		}
	}

	private void DiscoverOrderScript(IEnumerable<Urn> urns)
	{
		IEnumerable<Urn> enumerable = Discover(urns);
		OnScriptingProgress(ScriptingProgressStages.DiscoveryDone, enumerable);
		if (multipleDatabases && Preferences.DependentObjects)
		{
			enumerable = RemoveDuplicatesDiscovered(enumerable);
		}
		IEnumerable<Urn> enumerable2 = FilterUrns(enumerable);
		if (enumerable2 != null)
		{
			OnScriptingProgress(ScriptingProgressStages.FilteringDone, enumerable2);
			List<Urn> list = Order(enumerable2);
			OnScriptingProgress(ScriptingProgressStages.OrderingDone, list);
			if (objectScripting != null)
			{
				SetupObjectScriptingProgress(list);
			}
			ScriptUrns(list);
			OnScriptingProgress(ScriptingProgressStages.ScriptingCompleted, enumerable2);
		}
	}

	private IEnumerable<Urn> RemoveDuplicatesDiscovered(IEnumerable<Urn> discoveredUrns)
	{
		foreach (Urn urn in discoveredUrns)
		{
			if (!DuplicateUrn(urn))
			{
				yield return urn;
				inputList.Add(urn);
			}
		}
	}

	private bool DuplicateUrn(Urn urn)
	{
		XPathExpression xPathExpression = urn.XPathExpression;
		if (xPathExpression.Length < 3 || (xPathExpression.Length > 1 && xPathExpression[1].Name != "Database"))
		{
			return false;
		}
		string attributeFromFilter = xPathExpression[1].GetAttributeFromFilter("Name");
		if (!attributeFromFilter.Equals(currentlyScriptingDatabase) && inputList.Contains(urn))
		{
			return true;
		}
		return false;
	}

	private void ScriptUrns(List<Urn> orderedUrns)
	{
		if (Preferences.Behavior == ScriptBehavior.Drop || Preferences.Behavior == ScriptBehavior.DropAndCreate)
		{
			List<Urn> list = new List<Urn>(orderedUrns);
			list.Reverse();
			ScriptDropObjects(list);
		}
		if (Preferences.Behavior == ScriptBehavior.Create || Preferences.Behavior == ScriptBehavior.DropAndCreate)
		{
			bool existenceCheck = Preferences.IncludeScripts.ExistenceCheck;
			if (Preferences.Behavior == ScriptBehavior.DropAndCreate && existenceCheck)
			{
				Preferences.IncludeScripts.ExistenceCheck = false;
			}
			ScriptCreateObjects(orderedUrns);
			Preferences.IncludeScripts.ExistenceCheck = existenceCheck;
		}
		if (Preferences.Behavior == ScriptBehavior.CreateOrAlter)
		{
			ScriptAlterObjects(orderedUrns, isCreateOrAlter: true);
		}
	}

	private void Verify(List<Urn> urns)
	{
		CheckForConflictiongPreferences();
		VerifyInput(urns);
	}

	private IEnumerable<IEnumerable<Urn>> SingleDatabaseUrns(IEnumerable<Urn> urns)
	{
		List<Urn> serverObjectList = new List<Urn>();
		perDatabaseUrns = new SortedList<string, HashSet<Urn>>(Server.StringComparer);
		prefetchableObjects = new SortedList<string, bool>(Server.StringComparer);
		BucketizeUrns(urns, serverObjectList);
		if (perDatabaseUrns.Keys.Count < 1)
		{
			currentDatabasePrefetch = null;
			inputList.Clear();
			if (serverObjectList.Count > 0)
			{
				yield return serverObjectList;
			}
			yield break;
		}
		if (perDatabaseUrns.Keys.Count == 1)
		{
			currentDatabasePrefetch = GetDatabasePrefetch(perDatabaseUrns.Keys.First());
			inputList.Clear();
			yield return urns;
			yield break;
		}
		multipleDatabases = true;
		if (perDatabaseUrns.ContainsKey("master"))
		{
			currentlyScriptingDatabase = "master";
			perDatabaseUrns["master"].UnionWith(serverObjectList);
			currentDatabasePrefetch = GetDatabasePrefetch("master");
			yield return perDatabaseUrns["master"];
			perDatabaseUrns.Remove("master");
		}
		else
		{
			currentDatabasePrefetch = null;
			currentlyScriptingDatabase = string.Empty;
			if (serverObjectList.Count > 0)
			{
				yield return serverObjectList;
			}
		}
		foreach (string key in perDatabaseUrns.Keys)
		{
			string database = (currentlyScriptingDatabase = key);
			currentDatabasePrefetch = GetDatabasePrefetch(database);
			yield return perDatabaseUrns[database];
		}
	}

	private void BucketizeUrns(IEnumerable<Urn> urns, List<Urn> serverObjectList)
	{
		foreach (Urn urn in urns)
		{
			inputList.Add(urn);
			XPathExpression xPathExpression = urn.XPathExpression;
			if (xPathExpression.Length < 3 || (xPathExpression.Length > 1 && xPathExpression[1].Name != "Database"))
			{
				serverObjectList.Add(urn);
				continue;
			}
			string attributeFromFilter = xPathExpression[1].GetAttributeFromFilter("Name");
			if (!perDatabaseUrns.TryGetValue(attributeFromFilter, out var value))
			{
				value = new HashSet<Urn>();
				perDatabaseUrns.Add(attributeFromFilter, value);
				prefetchableObjects.Add(attributeFromFilter, value: false);
			}
			if (!creatingDictionary.ContainsKey(urn) && (xPathExpression[2].Name == Table.UrnSuffix || xPathExpression[2].Name == View.UrnSuffix))
			{
				prefetchableObjects[attributeFromFilter] = true;
			}
			perDatabaseUrns[attributeFromFilter].Add(urn);
		}
	}

	private IDatabasePrefetch GetDatabasePrefetch(string databaseName)
	{
		IDatabasePrefetch databasePrefetch = null;
		Database database = Server.Databases[databaseName];
		if (database != null && prefetchableObjects[databaseName])
		{
			HashSet<UrnTypeKey> filteredTypes = GetFilteredTypes();
			databasePrefetch = new DefaultDatabasePrefetch(database, Preferences, filteredTypes);
			databasePrefetch.creatingDictionary = creatingDictionary;
		}
		return databasePrefetch;
	}

	private HashSet<UrnTypeKey> GetFilteredTypes()
	{
		HashSet<UrnTypeKey> result = new HashSet<UrnTypeKey>();
		if (!Preferences.SfcChildren)
		{
			return ScriptingOptions.GetAllFilters(Server).filteredTypes;
		}
		if (discoverer != null)
		{
			if (!(discoverer is SmoDependencyDiscoverer smoDependencyDiscoverer))
			{
				return result;
			}
			return smoDependencyDiscoverer.filteredUrnTypes;
		}
		if (Filter != null)
		{
			if (!(Filter is SmoUrnFilter smoUrnFilter))
			{
				return result;
			}
			return smoUrnFilter.filteredTypes;
		}
		return result;
	}

	private void OnScriptingProgress(ScriptingProgressStages scriptingProgressStages, IEnumerable<Urn> urns)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Trace("ScriptMaker", "OnScriptingProgress {0}", scriptingProgressStages);
		if (scriptingProgress != null)
		{
			scriptingProgress(this, new ScriptingProgressEventArgs(scriptingProgressStages, new List<Urn>(urns)));
		}
	}

	private void SetupObjectScriptingProgress(List<Urn> orderedUrns)
	{
		HashSet<Urn> hashSet = new HashSet<Urn>();
		foreach (Urn orderedUrn in orderedUrns)
		{
			if (orderedUrn.Type.Equals("Special"))
			{
				hashSet.Add(orderedUrn.Parent.Parent);
			}
			else
			{
				hashSet.Add(orderedUrn);
			}
		}
		totalObjectsToScript = hashSet.Count;
		ObjectsToScript = hashSet;
	}

	private SmoDependencyDiscoverer SetupDiscoverer()
	{
		SmoDependencyDiscoverer smoDependencyDiscoverer = new SmoDependencyDiscoverer(Server);
		smoDependencyDiscoverer.Preferences = Preferences;
		smoDependencyDiscoverer.DatabasePrefetch = currentDatabasePrefetch;
		if (Filter is SmoUrnFilter smoUrnFilter)
		{
			smoDependencyDiscoverer.filteredUrnTypes = smoUrnFilter.filteredTypes;
		}
		smoDependencyDiscoverer.creatingDictionary = creatingDictionary;
		smoDependencyDiscoverer.ChildrenDiscovery += OnChildrenDiscovery;
		return smoDependencyDiscoverer;
	}

	private void OnChildrenDiscovery(object sender, ChildrenDiscoveryEventArgs e)
	{
		if (scriptContainerFactory == null)
		{
			return;
		}
		if (e.Parent.Type.Equals("Table"))
		{
			Table table = (Table)creatingDictionary.SmoObjectFromUrn(e.Parent);
			foreach (Urn child in e.Children)
			{
				if (!child.Type.Equals("Index") || !AddIndexToTablePropagationList((Index)creatingDictionary.SmoObjectFromUrn(child)))
				{
					scriptContainerFactory.AddContainer(creatingDictionary.SmoObjectFromUrn(child));
				}
				else
				{
					table.AddToIndexPropagationList((Index)creatingDictionary.SmoObjectFromUrn(child));
				}
			}
			scriptContainerFactory.AddContainer(creatingDictionary.SmoObjectFromUrn(e.Parent));
			return;
		}
		scriptContainerFactory.AddContainer(creatingDictionary.SmoObjectFromUrn(e.Parent));
		foreach (Urn child2 in e.Children)
		{
			scriptContainerFactory.AddContainer(creatingDictionary.SmoObjectFromUrn(child2));
		}
	}

	private bool AddIndexToTablePropagationList(Index index)
	{
		if (!Preferences.IncludeScripts.Ddl)
		{
			return false;
		}
		if (Filter is SmoUrnFilter smoUrnFilter && smoUrnFilter.filteredTypes.Contains(new UrnTypeKey(Table.UrnSuffix)))
		{
			return false;
		}
		IndexKeyType? propValueOptional = index.GetPropValueOptional<IndexKeyType>("IndexKeyType");
		if (propValueOptional.HasValue && propValueOptional.Value != IndexKeyType.None)
		{
			if (!Preferences.IncludeScripts.Data || SmoDependencyOrderer.IsFilestreamTable((Table)index.Parent, Preferences))
			{
				return true;
			}
			bool? propValueOptional2 = index.GetPropValueOptional<bool>("IsClustered");
			if (propValueOptional2.HasValue && propValueOptional2.Value)
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerable<Urn> FilterUrns(IEnumerable<Urn> discoveredObjects)
	{
		if (Filter == null)
		{
			return discoveredObjects;
		}
		return Filter.Filter(discoveredObjects);
	}

	private IEnumerable<Urn> Discover(IEnumerable<Urn> urns)
	{
		IEnumerable<Urn> enumerable;
		if (discoverer == null)
		{
			SmoDependencyDiscoverer smoDependencyDiscoverer = SetupDiscoverer();
			enumerable = smoDependencyDiscoverer.Discover(urns);
		}
		else
		{
			if (discoverer is SmoDependencyDiscoverer smoDependencyDiscoverer2)
			{
				smoDependencyDiscoverer2.creatingDictionary = creatingDictionary;
				smoDependencyDiscoverer2.DatabasePrefetch = currentDatabasePrefetch;
				smoDependencyDiscoverer2.ChildrenDiscovery += OnChildrenDiscovery;
			}
			enumerable = discoverer.Discover(urns);
		}
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Trace("ScriptMaker", "Discovered {0} Urns: {1}", enumerable.Count(), string.Join(System.Environment.NewLine + "\t", enumerable.Select((Urn urn) => urn.Value).ToArray()));
		return enumerable;
	}

	private List<Urn> Order(IEnumerable<Urn> filteredObjects)
	{
		SmoDependencyOrderer smoDependencyOrderer = SetupOrderer();
		return smoDependencyOrderer.Order(filteredObjects);
	}

	private SmoDependencyOrderer SetupOrderer()
	{
		SmoDependencyOrderer smoDependencyOrderer = new SmoDependencyOrderer(Server);
		smoDependencyOrderer.ScriptingPreferences = Preferences;
		smoDependencyOrderer.creatingDictionary = creatingDictionary;
		smoDependencyOrderer.ScriptContainerFactory = scriptContainerFactory;
		return smoDependencyOrderer;
	}

	private void VerifyInput(List<Urn> urns)
	{
		if (Preferences.ContinueOnScriptingError)
		{
			urns.RemoveAll((Urn p) => string.IsNullOrEmpty(p) || !Scriptable(p));
			return;
		}
		foreach (Urn urn in urns)
		{
			if (string.IsNullOrEmpty(urn) || !Scriptable(urn))
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.Script, this, new SmoException(ExceptionTemplatesImpl.CantScriptObject(urn ?? ((Urn)string.Empty))));
			}
		}
	}

	private void ScriptCreateObjects(IEnumerable<Urn> urns)
	{
		int num = 0;
		HashSet<Urn> hashSet = new HashSet<Urn>();
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Trace("ScriptMaker", "ScriptCreateObjects for {0} Urns: {1}", urns.Count(), string.Join(System.Environment.NewLine + "\t", urns.Select((Urn u) => u.Value).ToArray()));
		foreach (Urn urn2 in urns)
		{
			ObjectScriptingType scriptType = ObjectScriptingType.None;
			try
			{
				try
				{
					ScriptCreate(urn2, Preferences, ref scriptType);
				}
				catch (Exception ex)
				{
					if (ex is OutOfMemoryException || retry == null)
					{
						throw;
					}
					RetryRequestedEventArgs e = new RetryRequestedEventArgs(urn2, (ScriptingPreferences)Preferences.Clone());
					retry(this, e);
					if (e.ShouldRetry)
					{
						currentRetryArgs = e;
						ScriptCreate(urn2, e.ScriptingPreferences, ref scriptType);
					}
				}
				finally
				{
					currentRetryArgs = null;
				}
			}
			catch (Exception e2)
			{
				if (ThrowException(urn2, e2))
				{
					throw;
				}
			}
			if (objectScripting != null)
			{
				Urn urn = (urn2.Type.Equals("Special") ? urn2.Parent.Parent : urn2);
				if (hashSet.Add(urn))
				{
					num++;
				}
				objectScripting(this, new ObjectScriptingEventArgs(urn, urn2, num, totalObjectsToScript, scriptType));
			}
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Trace("ScriptMaker", "ScriptCreate complete for {0}", urn2);
		}
	}

	private void ScriptCreate(Urn urn, ScriptingPreferences sp, ref ObjectScriptingType scriptType)
	{
		ScriptContainer value;
		if (!urn.Type.Equals("Special"))
		{
			if (scriptContainerFactory != null && scriptContainerFactory.TryGetValue(urn, out value))
			{
				ScriptCreateStoredObject(urn, sp, ref scriptType, value);
			}
			else
			{
				ScriptCreateObject(urn, sp, ref scriptType);
			}
		}
		else if (urn.Parent.Type == "UnresolvedEntity")
		{
			ScriptCreateUnresolvedEntity(urn, ref scriptType);
		}
		else if (scriptContainerFactory != null && scriptContainerFactory.TryGetValue(urn.Parent.Parent, out value))
		{
			ScriptDataStoredObject(urn.Parent.Parent, sp, ref scriptType, value);
		}
		else
		{
			ScriptCreateSpecialUrn(urn, sp, ref scriptType);
		}
	}

	private static ScriptingPreferences CloneScriptingPreferencesForSpecialUrns(ScriptingPreferences sp)
	{
		ScriptingPreferences scriptingPreferences = (ScriptingPreferences)sp.Clone();
		scriptingPreferences.IncludeScripts.Owner = false;
		scriptingPreferences.IncludeScripts.Associations = false;
		scriptingPreferences.IncludeScripts.Permissions = false;
		scriptingPreferences.IncludeScripts.CreateDdlTriggerDisabled = true;
		return scriptingPreferences;
	}

	private void ScriptDataStoredObject(Urn urn, ScriptingPreferences sp, ref ObjectScriptingType scriptType, ScriptContainer scriptContainer)
	{
		TableScriptContainer tableScriptContainer = (TableScriptContainer)scriptContainer;
		if (sp.IncludeScripts.DatabaseContext)
		{
			writer.ScriptContext(tableScriptContainer.DatabaseContext, urn);
		}
		if (tableScriptContainer.DataScript != null)
		{
			ScriptDataToWriter(tableScriptContainer.DataScript, urn);
		}
		if (sp.IncludeScripts.Ddl && sp.OldOptions.Bindings)
		{
			ScriptObjectToWriter(tableScriptContainer.BindingsScript.Script, urn);
		}
		scriptType = ObjectScriptingType.Data;
	}

	private void ScriptCreateStoredObject(Urn urn, ScriptingPreferences sp, ref ObjectScriptingType scriptType, ScriptContainer scriptContainer)
	{
		if (scriptContainer.CreateScript.Script.Count > 0)
		{
			if (sp.IncludeScripts.DatabaseContext)
			{
				writer.ScriptContext(scriptContainer.DatabaseContext, urn);
			}
			ScriptObjectToWriter(scriptContainer.CreateScript.Script, urn);
		}
		scriptType = ObjectScriptingType.All;
	}

	private void ScriptCreateObject(Urn urn, ScriptingPreferences sp, ref ObjectScriptingType scriptType)
	{
		StringCollection stringCollection = new StringCollection();
		SqlSmoObject sqlSmoObject = creatingDictionary.SmoObjectFromUrn(urn);
		if (!IsFiltered(sqlSmoObject, sp))
		{
			CheckCloudSupport(sqlSmoObject, sp);
			sqlSmoObject.InitializeKeepDirtyValues();
			sqlSmoObject.ScriptCreateInternal(stringCollection, sp, skipPropagateScript: true);
			ScriptObjectToWriterWithContext(stringCollection, sp, sqlSmoObject);
			scriptType = ObjectScriptingType.All;
		}
	}

	private void ScriptCreateUnresolvedEntity(Urn urn, ref ObjectScriptingType scriptType)
	{
		StringCollection stringCollection = new StringCollection();
		string value = string.Format(SmoApplication.DefaultCulture, "/****** Cannot script Unresolved Entities : {0} ******/", new object[1] { urn.Parent.Parent.ToString() });
		stringCollection.Add(value);
		ScriptObjectToWriter(stringCollection, urn.Parent);
		scriptType = ObjectScriptingType.Comment;
	}

	private void ScriptCreateSpecialUrn(Urn urn, ScriptingPreferences sp, ref ObjectScriptingType scriptType)
	{
		StringCollection stringCollection = new StringCollection();
		SqlSmoObject sqlSmoObject = creatingDictionary.SmoObjectFromUrn(urn.Parent.Parent);
		if (IsFiltered(sqlSmoObject, sp))
		{
			return;
		}
		CheckCloudSupport(sqlSmoObject, sp);
		sqlSmoObject.InitializeKeepDirtyValues();
		switch (urn.Parent.Type)
		{
		case "Object":
			sqlSmoObject.ScriptCreateInternal(stringCollection, CloneScriptingPreferencesForSpecialUrns(sp), skipPropagateScript: true);
			ScriptObjectToWriterWithContext(stringCollection, sp, sqlSmoObject);
			scriptType = ObjectScriptingType.Object;
			break;
		case "Data":
			ScriptDatabaseContextToWriter(sqlSmoObject, sp, isScriptingPermission: false);
			if (sqlSmoObject is Table table)
			{
				ScriptDataToWriter(table.ScriptDataInternal(sp), table.Urn);
				if (sp.IncludeScripts.Ddl && sp.OldOptions.Bindings)
				{
					table.ScriptBindings(stringCollection, sp);
					ScriptObjectToWriterWithContext(stringCollection, sp, sqlSmoObject);
				}
			}
			scriptType = ObjectScriptingType.Data;
			break;
		case "Ownership":
			if (sqlSmoObject is NamedSmoObject namedSmoObject)
			{
				namedSmoObject.ScriptChangeOwner(stringCollection, sp);
				ScriptObjectToWriterWithContext(stringCollection, sp, sqlSmoObject);
			}
			scriptType = ObjectScriptingType.OwnerShip;
			break;
		case "Associations":
			sqlSmoObject.ScriptAssociationsInternal(stringCollection, sp);
			ScriptObjectToWriterWithContext(stringCollection, sp, sqlSmoObject);
			scriptType = ObjectScriptingType.Association;
			break;
		case "Permission":
			sqlSmoObject.AddScriptPermission(stringCollection, sp);
			if (stringCollection.Count > 0)
			{
				ScriptDatabaseContextToWriter(sqlSmoObject, sp, isScriptingPermission: true);
				ScriptObjectToWriter(stringCollection, sqlSmoObject.Urn);
			}
			scriptType = ObjectScriptingType.Permission;
			break;
		case "ddltriggerdatabaseenable":
		case "ddltriggerserverenable":
			if (sqlSmoObject is DdlTriggerBase ddlTriggerBase && ddlTriggerBase.GetPropValueOptional("IsEnabled", defaultValue: true))
			{
				stringCollection.Add(ddlTriggerBase.ScriptEnableDisableCommand(isEnabled: true, sp));
				ScriptObjectToWriterWithContext(stringCollection, sp, sqlSmoObject);
			}
			scriptType = ObjectScriptingType.Object;
			break;
		case "databasereadonly":
			if (sqlSmoObject is Database database)
			{
				bool? propValueOptional = database.GetPropValueOptional<bool>("ReadOnly");
				if (propValueOptional.HasValue)
				{
					database.ScriptAlterPropReadonly(stringCollection, sp, propValueOptional.Value);
					ScriptObjectToWriterWithContext(stringCollection, sp, sqlSmoObject);
				}
			}
			scriptType = ObjectScriptingType.Object;
			break;
		}
	}

	private void ScriptDataToWriter(IEnumerable<string> dataScripts, Urn urn)
	{
		if (currentRetryArgs != null)
		{
			writer.ScriptData(SurroundWithRetryTexts(dataScripts, currentRetryArgs), urn);
		}
		else
		{
			writer.ScriptData(dataScripts, urn);
		}
	}

	internal static IEnumerable<string> SurroundWithRetryTexts(IEnumerable<string> dataScripts, RetryRequestedEventArgs retryRequestedEventArgs)
	{
		EnumerableContainer enumerableContainer = new EnumerableContainer();
		if (!string.IsNullOrEmpty(retryRequestedEventArgs.PreText))
		{
			enumerableContainer.Add(new string[1] { retryRequestedEventArgs.PreText });
		}
		enumerableContainer.Add(dataScripts);
		if (!string.IsNullOrEmpty(retryRequestedEventArgs.PostText))
		{
			enumerableContainer.Add(new string[1] { retryRequestedEventArgs.PostText });
		}
		return enumerableContainer;
	}

	private void ScriptObjectToWriterWithContext(StringCollection scriptCollection, ScriptingPreferences sp, SqlSmoObject obj)
	{
		if (scriptCollection.Count > 0)
		{
			ScriptDatabaseContextToWriter(obj, sp, isScriptingPermission: false);
			ScriptObjectToWriter(scriptCollection, obj.Urn);
		}
	}

	private void ScriptDatabaseContextToWriter(SqlSmoObject obj, ScriptingPreferences sp, bool isScriptingPermission)
	{
		if (sp.TargetDatabaseEngineType != DatabaseEngineType.SqlAzureDatabase && sp.IncludeScripts.DatabaseContext)
		{
			string databaseContext = ScriptDatabaseContext(obj, isScriptingPermission);
			writer.ScriptContext(databaseContext, obj.Urn);
		}
	}

	internal static string ScriptDatabaseContext(SqlSmoObject obj, bool isScriptingPermission)
	{
		string text = obj.GetDBName();
		if ((obj is Database && !isScriptingPermission) || string.IsNullOrEmpty(text))
		{
			text = "master";
		}
		return string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(text) });
	}

	private void ScriptDropObjects(IEnumerable<Urn> urns)
	{
		int num = 0;
		HashSet<Urn> hashSet = new HashSet<Urn>();
		foreach (Urn urn2 in urns)
		{
			ObjectScriptingType scriptType = ObjectScriptingType.None;
			try
			{
				try
				{
					ScriptDrop(urn2, Preferences, ref scriptType);
				}
				catch (Exception ex)
				{
					if (ex is OutOfMemoryException || retry == null)
					{
						throw;
					}
					RetryRequestedEventArgs e = new RetryRequestedEventArgs(urn2, (ScriptingPreferences)Preferences.Clone());
					retry(this, e);
					if (e.ShouldRetry)
					{
						currentRetryArgs = e;
						ScriptDrop(urn2, e.ScriptingPreferences, ref scriptType);
					}
				}
				finally
				{
					currentRetryArgs = null;
				}
			}
			catch (Exception e2)
			{
				if (ThrowException(urn2, e2))
				{
					throw;
				}
			}
			if (objectScripting != null)
			{
				Urn urn = (urn2.Type.Equals("Special") ? urn2.Parent.Parent : urn2);
				if (hashSet.Add(urn))
				{
					num++;
				}
				objectScripting(this, new ObjectScriptingEventArgs(urn, urn2, num, totalObjectsToScript, scriptType));
			}
		}
	}

	private void ScriptDrop(Urn urn, ScriptingPreferences sp, ref ObjectScriptingType scriptType)
	{
		ScriptContainer value;
		if (!urn.Type.Equals("Special"))
		{
			if (scriptContainerFactory != null && scriptContainerFactory.TryGetValue(urn, out value))
			{
				ScriptDropStoredObject(urn, sp, ref scriptType, value);
			}
			else
			{
				ScriptDropObject(urn, sp, ref scriptType);
			}
		}
		else
		{
			if (!(urn.Parent.Type != "UnresolvedEntity"))
			{
				return;
			}
			if (scriptContainerFactory != null && scriptContainerFactory.TryGetValue(urn.Parent.Parent, out value))
			{
				if (!sp.IncludeScripts.Ddl && sp.IncludeScripts.Data)
				{
					ScriptDropStoredObject(urn.Parent.Parent, sp, ref scriptType, value);
					scriptType = ObjectScriptingType.Data;
				}
			}
			else
			{
				ScriptDropSpecialUrn(urn, sp, ref scriptType);
			}
		}
	}

	private void ScriptDropStoredObject(Urn urn, ScriptingPreferences sp, ref ObjectScriptingType scriptType, ScriptContainer scriptContainer)
	{
		if (scriptContainer.DropScript.Script.Count > 0)
		{
			if (sp.IncludeScripts.DatabaseContext)
			{
				writer.ScriptContext(scriptContainer.DatabaseContext, urn);
			}
			ScriptObjectToWriter(scriptContainer.DropScript.Script, urn);
		}
		scriptType = ObjectScriptingType.All;
	}

	private void ScriptDropObject(Urn urn, ScriptingPreferences sp, ref ObjectScriptingType scriptType)
	{
		StringCollection stringCollection = new StringCollection();
		SqlSmoObject sqlSmoObject = creatingDictionary.SmoObjectFromUrn(urn);
		if (!IsFiltered(sqlSmoObject, sp))
		{
			CheckCloudSupport(sqlSmoObject, sp);
			sqlSmoObject.ScriptDropInternal(stringCollection, sp);
			ScriptObjectToWriterWithContext(stringCollection, sp, sqlSmoObject);
			scriptType = ObjectScriptingType.All;
		}
	}

	private void CheckCloudSupport(SqlSmoObject obj, ScriptingPreferences sp)
	{
		if (sp.TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase && !obj.IsCloudSupported)
		{
			throw new UnsupportedEngineTypeException(ExceptionTemplatesImpl.UnsupportedEngineTypeException);
		}
	}

	private bool IsFiltered(SqlSmoObject obj, ScriptingPreferences sp)
	{
		if (sp.ForDirectExecution || !obj.IgnoreForScripting)
		{
			if (!sp.SystemObjects)
			{
				return IsSystemObject(obj);
			}
			return false;
		}
		return true;
	}

	private void ScriptDropSpecialUrn(Urn urn, ScriptingPreferences sp, ref ObjectScriptingType scriptType)
	{
		StringCollection stringCollection = new StringCollection();
		SqlSmoObject sqlSmoObject = creatingDictionary.SmoObjectFromUrn(urn.Parent.Parent);
		if (IsFiltered(sqlSmoObject, sp))
		{
			return;
		}
		CheckCloudSupport(sqlSmoObject, sp);
		switch (urn.Parent.Type)
		{
		case "Object":
			sqlSmoObject.ScriptDropInternal(stringCollection, sp);
			ScriptObjectToWriterWithContext(stringCollection, sp, sqlSmoObject);
			scriptType = ObjectScriptingType.Object;
			break;
		case "Data":
			if (!sp.IncludeScripts.Ddl && sp.IncludeScripts.Data)
			{
				if (sqlSmoObject is Table table)
				{
					ScriptObjectToWriterWithContext(table.ScriptDropData(sp), sp, sqlSmoObject);
					scriptType = ObjectScriptingType.Data;
				}
				scriptType = ObjectScriptingType.Data;
			}
			break;
		case "ddltriggerdatabasedisable":
		case "ddltriggerserverdisable":
			if (sqlSmoObject is DdlTriggerBase ddlTrigger)
			{
				stringCollection.Add(ScriptDdlTriggerDisable(ddlTrigger, sp));
				ScriptObjectToWriterWithContext(stringCollection, sp, sqlSmoObject);
			}
			scriptType = ObjectScriptingType.Object;
			break;
		}
	}

	private string ScriptDdlTriggerDisable(DdlTriggerBase ddlTrigger, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendLine(ddlTrigger.GetIfNotExistStatement(sp, string.Empty));
		}
		stringBuilder.AppendLine(ddlTrigger.ScriptEnableDisableCommand(isEnabled: false, sp));
		return stringBuilder.ToString();
	}

	private void ScriptAlterObjects(List<Urn> urns, bool isCreateOrAlter = false)
	{
		foreach (Urn urn in urns)
		{
			try
			{
				StringCollection stringCollection = new StringCollection();
				SqlSmoObject sqlSmoObject = creatingDictionary.SmoObjectFromUrn(urn);
				if (IsFiltered(sqlSmoObject, Preferences))
				{
					continue;
				}
				if (!isCreateOrAlter)
				{
					CheckCloudSupport(sqlSmoObject, Preferences);
					sqlSmoObject.InitializeKeepDirtyValues();
				}
				if (Preferences.IncludeScripts.Ddl)
				{
					if (isCreateOrAlter)
					{
						sqlSmoObject.ScriptCreateOrAlterInternal(stringCollection, Preferences);
					}
					else
					{
						sqlSmoObject.ScriptAlterInternal(stringCollection, Preferences);
					}
					ScriptObjectToWriterWithContext(stringCollection, Preferences, sqlSmoObject);
				}
			}
			catch (Exception e)
			{
				if (ThrowException(urn, e))
				{
					throw;
				}
			}
		}
	}

	private void ScriptObjectToWriter(StringCollection stringCollection, Urn obj)
	{
		if (stringCollection.Count > 0)
		{
			SurroundWithRetryTexts(stringCollection, currentRetryArgs);
			EnumerableContainer enumerableContainer = new EnumerableContainer();
			enumerableContainer.Add(stringCollection);
			writer.ScriptObject(enumerableContainer, obj);
		}
	}

	internal static void SurroundWithRetryTexts(StringCollection stringCollection, RetryRequestedEventArgs retryRequestedEventArgs)
	{
		if (retryRequestedEventArgs != null)
		{
			if (!string.IsNullOrEmpty(retryRequestedEventArgs.PreText))
			{
				stringCollection.Insert(0, retryRequestedEventArgs.PreText);
			}
			if (!string.IsNullOrEmpty(retryRequestedEventArgs.PostText))
			{
				stringCollection.Add(retryRequestedEventArgs.PostText);
			}
		}
	}

	private bool ThrowException(Urn urn, Exception e)
	{
		if (e is OutOfMemoryException || !Preferences.ContinueOnScriptingError)
		{
			return true;
		}
		if (scriptingError != null)
		{
			scriptingError(this, new ScriptingErrorEventArgs(urn.Type.Equals("Special") ? urn.Parent.Parent : urn, e));
		}
		return false;
	}

	private bool IsSystemObject(SqlSmoObject obj)
	{
		if (obj.Properties.Contains("IsSystemObject"))
		{
			object valueWithNullReplacement = obj.Properties.GetValueWithNullReplacement("IsSystemObject", throwOnNullValue: false, useDefaultOnMissingValue: true);
			if (valueWithNullReplacement != null)
			{
				return (bool)valueWithNullReplacement;
			}
			return false;
		}
		return false;
	}

	private bool Scriptable(Urn urn)
	{
		switch (urn.Type)
		{
		case "Table":
		case "View":
		case "Default":
		case "Rule":
		case "UserDefinedFunction":
		case "UserDefinedDataType":
		case "UserDefinedTableType":
		case "StoredProcedure":
		case "ExtendedStoredProcedure":
		case "User":
		case "Role":
		case "XmlSchemaCollection":
		case "FullTextCatalog":
		case "FullTextStopList":
		case "SearchPropertyList":
		case "SearchProperty":
		case "FullTextIndex":
		case "FullTextIndexColumn":
		case "Schema":
		case "ApplicationRole":
		case "PlanGuide":
		case "DatabaseAuditSpecification":
		case "Check":
		case "ForeignKey":
		case "Trigger":
		case "Index":
		case "Statistic":
		case "DefaultConstraint":
		case "DdlTrigger":
		case "Mail":
		case "MailProfile":
		case "MailAccount":
		case "MailServer":
		case "ConfigurationValue":
		case "Job":
		case "Step":
		case "Operator":
		case "OperatorCategory":
		case "JobCategory":
		case "AlertCategory":
		case "Schedule":
		case "TargetServerGroup":
		case "Alert":
		case "BackupDevice":
		case "ProxyAccount":
		case "JobServer":
		case "AlertSystem":
		case "Login":
		case "FullTextService":
		case "Database":
		case "UserDefinedMessage":
		case "LinkedServer":
		case "HttpEndpoint":
		case "Endpoint":
		case "Setting":
		case "OleDbProviderSetting":
		case "UserOption":
		case "FileStreamSettings":
		case "Audit":
		case "ServerAuditSpecification":
		case "Server":
		case "UserDefinedType":
		case "UserDefinedAggregate":
		case "SqlAssembly":
		case "ExternalLibrary":
		case "PartitionScheme":
		case "PartitionFunction":
		case "ExtendedProperty":
		case "Synonym":
		case "Sequence":
		case "MessageType":
		case "ServiceContract":
		case "BrokerService":
		case "BrokerPriority":
		case "ServiceQueue":
		case "ServiceRoute":
		case "RemoteServiceBinding":
		case "SymmetricKey":
		case "KeyEncryption":
		case "ResourceGovernor":
		case "ResourcePool":
		case "ExternalResourcePool":
		case "WorkloadGroup":
		case "CryptographicProvider":
		case "UnresolvedEntity":
		case "DatabaseEncryptionKey":
		case "AvailabilityGroup":
		case "AvailabilityReplica":
		case "AvailabilityDatabase":
		case "AvailabilityGroupListener":
		case "AvailabilityGroupListenerIPAddress":
		case "SmartAdmin":
		case "SecurityPolicy":
		case "ColumnMasterKey":
		case "ColumnEncryptionKey":
		case "ExternalDataSource":
		case "ExternalFileFormat":
		case "QueryStoreOptions":
		case "DatabaseScopedCredential":
			return true;
		default:
			return false;
		}
	}

	private void CheckForConflictiongPreferences()
	{
		if (Preferences.OldOptions.DdlBodyOnly && Preferences.OldOptions.DdlHeaderOnly)
		{
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.ConflictingScriptingOptions(Enum.GetName(typeof(EnumScriptOptions), EnumScriptOptions.DdlBodyOnly), Enum.GetName(typeof(EnumScriptOptions), EnumScriptOptions.DdlHeaderOnly)));
		}
		if (!Preferences.IncludeScripts.Data && !Preferences.IncludeScripts.Ddl)
		{
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.InvalidScriptingOutput(Enum.GetName(typeof(EnumScriptOptions), EnumScriptOptions.ScriptData), Enum.GetName(typeof(EnumScriptOptions), EnumScriptOptions.ScriptSchema)));
		}
		if (!Preferences.IncludeScripts.Ddl && Preferences.ScriptForAlter)
		{
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.ConflictingScriptingOptions(Enum.GetName(typeof(EnumScriptOptions), EnumScriptOptions.ScriptSchema), "ScriptForAlter"));
		}
	}
}
