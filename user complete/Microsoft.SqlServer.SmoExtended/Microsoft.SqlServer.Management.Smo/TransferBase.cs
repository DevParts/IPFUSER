using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.Internal;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class TransferBase
{
	private Database database;

	private ArrayList objectList;

	private string destinationLogin = string.Empty;

	private SqlSecureString destinationPassword = string.Empty;

	private string destinationServer = string.Empty;

	private DatabaseFileMappingsDictionary databaseFileMappings = new DatabaseFileMappingsDictionary();

	private Scripter scripter;

	public Database Database
	{
		get
		{
			return database;
		}
		set
		{
			SetDatabase(value);
		}
	}

	public ArrayList ObjectList
	{
		get
		{
			if (objectList == null)
			{
				objectList = new ArrayList();
			}
			return objectList;
		}
		set
		{
			if (value == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("ObjectList"));
			}
			objectList = value;
		}
	}

	public bool CopyAllObjects { get; set; }

	public bool CopyAllFullTextCatalogs { get; set; }

	public bool CopyAllFullTextStopLists { get; set; }

	public bool CopyAllSearchPropertyLists { get; set; }

	public bool CopyAllTables { get; set; }

	public bool CopyAllViews { get; set; }

	public bool CopyAllStoredProcedures { get; set; }

	public bool CopyAllUserDefinedFunctions { get; set; }

	public bool CopyAllUserDefinedDataTypes { get; set; }

	public bool CopyAllUserDefinedTableTypes { get; set; }

	public bool CopyAllPlanGuides { get; set; }

	public bool CopyAllRules { get; set; }

	public bool CopyAllDefaults { get; set; }

	public bool CopyAllUsers { get; set; }

	public bool CopyAllRoles { get; set; }

	public bool CopyAllPartitionSchemes { get; set; }

	public bool CopyAllPartitionFunctions { get; set; }

	public bool CopyAllXmlSchemaCollections { get; set; }

	public bool CopyAllSqlAssemblies { get; set; }

	public bool CopyAllUserDefinedAggregates { get; set; }

	public bool CopyAllUserDefinedTypes { get; set; }

	public bool CopyAllSchemas { get; set; }

	public bool CopyAllSynonyms { get; set; }

	public bool CopyAllSequences { get; set; }

	public bool CopyAllDatabaseTriggers { get; set; }

	public bool CopyAllDatabaseScopedCredentials { get; set; }

	public bool CopyAllExternalFileFormats { get; set; }

	public bool CopyAllExternalDataSources { get; set; }

	public bool CopyAllLogins { get; set; }

	public bool CopyAllExternalLibraries { get; set; }

	public bool CopySchema { get; set; }

	public bool CopyData { get; set; }

	public bool DropDestinationObjectsFirst { get; set; }

	public bool CreateTargetDatabase { get; set; }

	public bool DestinationTranslateChar { get; set; }

	public bool SourceTranslateChar { get; set; }

	public bool UseDestinationTransaction { get; set; }

	public bool PreserveLogins { get; set; }

	public bool PrefetchObjects { get; set; }

	public bool PreserveDbo { get; set; }

	public bool DestinationLoginSecure { get; set; }

	public string DestinationDatabase { get; set; }

	public string DestinationLogin
	{
		get
		{
			return destinationLogin;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("DestinationLogin");
			}
			destinationLogin = value;
		}
	}

	public string DestinationPassword
	{
		get
		{
			return (string)destinationPassword;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("DestinationPassword");
			}
			destinationPassword = new SqlSecureString(value);
		}
	}

	public string DestinationServer
	{
		get
		{
			return destinationServer;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("DestinationServer");
			}
			destinationServer = value;
		}
	}

	public string TargetDatabaseFilePath { get; set; }

	public string TargetLogFilePath { get; set; }

	public DatabaseFileMappingsDictionary DatabaseFileMappings
	{
		get
		{
			return databaseFileMappings;
		}
		set
		{
			if (value == null)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.WrongPropertyValueException("DatabaseFileMappings", "NULL"));
			}
			databaseFileMappings = value;
		}
	}

	protected Scripter Scripter
	{
		get
		{
			if (scripter == null)
			{
				if (Database == null)
				{
					throw new PropertyNotSetException("Database");
				}
				scripter = new Scripter(Database.Parent);
				scripter.PrefetchObjects = false;
				scripter.Options.WithDependencies = true;
			}
			return scripter;
		}
	}

	public ScriptingOptions Options
	{
		get
		{
			return Scripter.GetOptions();
		}
		set
		{
			Scripter.Options = value;
		}
	}

	public event ProgressReportEventHandler DiscoveryProgress
	{
		add
		{
			Scripter.DiscoveryProgress += value;
		}
		remove
		{
			Scripter.DiscoveryProgress -= value;
		}
	}

	public event ProgressReportEventHandler ScriptingProgress
	{
		add
		{
			Scripter.ScriptingProgress += value;
		}
		remove
		{
			Scripter.ScriptingProgress -= value;
		}
	}

	public event ScriptingErrorEventHandler ScriptingError
	{
		add
		{
			Scripter.ScriptingError += value;
		}
		remove
		{
			Scripter.ScriptingError -= value;
		}
	}

	public TransferBase()
	{
		Init();
	}

	public TransferBase(Database database)
		: this()
	{
		SetDatabase(database);
	}

	private void SetDatabase(Database database)
	{
		if (database == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.SetDatabase, this, new ArgumentNullException("database"));
		}
		if (database.State == SqlSmoState.Pending || database.State == SqlSmoState.Creating)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.TransferCtorErr, new ArgumentException("database"));
		}
		this.database = database;
	}

	private void Init()
	{
		CopyAllObjects = true;
		CopySchema = true;
		CopyData = true;
		DestinationDatabase = string.Empty;
		DestinationLoginSecure = true;
		DestinationTranslateChar = true;
		SourceTranslateChar = true;
		TargetDatabaseFilePath = string.Empty;
		TargetLogFilePath = string.Empty;
		PrefetchObjects = true;
	}

	protected void SetTargetServerInfo()
	{
		ServerConnection serverConnection = new ServerConnection(DestinationServer);
		serverConnection.LoginSecure = DestinationLoginSecure;
		ServerConnection serverConnection2 = serverConnection;
		try
		{
			if (!DestinationLoginSecure)
			{
				serverConnection2.Login = DestinationLogin;
				serverConnection2.Password = DestinationPassword;
			}
			serverConnection2.NonPooledConnection = true;
			serverConnection2.Connect();
			Scripter.Options.SetTargetServerVersion(serverConnection2.ServerVersion);
		}
		finally
		{
			serverConnection2.Disconnect();
		}
	}

	public StringCollection ScriptTransfer()
	{
		if (Database == null)
		{
			throw new PropertyNotSetException("Database");
		}
		if (Options.ScriptData)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ScriptDataNotSupportedByThisMethod);
		}
		try
		{
			return EnumerableContainer.IEnumerableToStringCollection(EnumScriptTransfer());
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ScriptTransfer, this, ex);
		}
	}

	public IEnumerable<string> EnumScriptTransfer()
	{
		if (Database == null)
		{
			throw new PropertyNotSetException("Database");
		}
		bool withDependencies = Scripter.Options.WithDependencies;
		bool includeDatabaseContext = Options.IncludeDatabaseContext;
		try
		{
			SqlSmoObject.Trace("Transfer: Entering");
			SqlSmoObject.Trace("Transfer: Script all discovered objects");
			EnumerableContainer enumerableContainer = new EnumerableContainer();
			Scripter.PrefetchObjects = false;
			Options.IncludeDatabaseContext = false;
			if (!string.IsNullOrEmpty(DestinationServer))
			{
				SetTargetServerInfo();
			}
			if (includeDatabaseContext)
			{
				enumerableContainer.Add(new List<string> { string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(string.IsNullOrEmpty(DestinationDatabase) ? Database.Name : DestinationDatabase) }) });
			}
			enumerableContainer.Add(Scripter.EnumScriptWithList(GetObjectList(ordered: false)));
			return enumerableContainer;
		}
		catch (PropertyCannotBeRetrievedException innerException)
		{
			FailedOperationException ex = new FailedOperationException(ExceptionTemplatesImpl.FailedOperationExceptionTextScript(SqlSmoObject.GetTypeName(Database.GetType().Name), Database.ToString()), innerException);
			ex.Operation = ExceptionTemplatesImpl.ScriptTransfer;
			ex.FailedObject = this;
			throw ex;
		}
		catch (Exception ex2)
		{
			SqlSmoObject.FilterException(ex2);
			throw new FailedOperationException(ExceptionTemplatesImpl.ScriptTransfer, this, ex2);
		}
		finally
		{
			Options.IncludeDatabaseContext = includeDatabaseContext;
			Scripter.Options.WithDependencies = withDependencies;
		}
	}

	public UrnCollection EnumObjects()
	{
		return EnumObjects(ordered: true);
	}

	public UrnCollection EnumObjects(bool ordered)
	{
		DependencyCollection dependencyCollection = GetObjectList(ordered);
		UrnCollection urnCollection = new UrnCollection();
		foreach (DependencyCollectionNode item in dependencyCollection)
		{
			urnCollection.Add(item.Urn);
		}
		return urnCollection;
	}

	internal DependencyCollection GetObjectList(bool ordered)
	{
		DependencyCollection dependencyCollection = null;
		HashSet<Urn> hashSet = new HashSet<Urn>();
		HashSet<Urn> nonDepDiscList = new HashSet<Urn>();
		Dictionary<string, HashSet<Urn>> dictionary = new Dictionary<string, HashSet<Urn>>();
		dictionary.Add(User.UrnSuffix, new HashSet<Urn>());
		dictionary.Add(DatabaseRole.UrnSuffix, new HashSet<Urn>());
		dictionary.Add(ApplicationRole.UrnSuffix, new HashSet<Urn>());
		dictionary.Add(Login.UrnSuffix, new HashSet<Urn>());
		dictionary.Add(Endpoint.UrnSuffix, new HashSet<Urn>());
		HashSet<Urn> hashSet2 = new HashSet<Urn>();
		ScriptingPreferences scriptingPreferences = Options.GetScriptingPreferences();
		SeparateDiscoverySupportedObjects(hashSet, nonDepDiscList, dictionary, hashSet2);
		Dictionary<Type, StringCollection> originalDefaultFields = new Dictionary<Type, StringCollection>();
		try
		{
			if (scriptingPreferences.IncludeScripts.ExtendedProperties && IsSupportedObject<ExtendedProperty>(scriptingPreferences) && CopyAllObjects)
			{
				AddAllObjects<ExtendedProperty>(hashSet2, Database.ExtendedProperties);
			}
			AddDiscoverableObjects(hashSet, originalDefaultFields);
			dependencyCollection = new DependencyCollection();
			bool flag = Options.WithDependencies && !CopyAllObjects;
			if (0 < hashSet.Count && flag)
			{
				hashSet = DiscoverDependencies(hashSet);
			}
			PrefetchSecurityObjects(originalDefaultFields);
			AddDiscoveryUnsupportedObjects(hashSet, nonDepDiscList);
			if (ordered)
			{
				dependencyCollection = GetDependencyOrderedCollection(hashSet);
				AddSecurityObjectsInOrder(dependencyCollection, dictionary);
			}
			else
			{
				foreach (Urn item in hashSet)
				{
					dependencyCollection.Add(new DependencyCollectionNode(item, isSchemaBound: true, fRoot: true));
				}
				AddSecurityObjectsWithoutOrder(dependencyCollection, dictionary);
			}
			CheckDownLevelScripting(dependencyCollection);
			foreach (Urn item2 in hashSet2)
			{
				dependencyCollection.Insert(0, new DependencyCollectionNode(item2, isSchemaBound: true, fRoot: true));
			}
			return dependencyCollection;
		}
		finally
		{
			RestoreDefaultInitFields(originalDefaultFields);
		}
	}

	private HashSet<Urn> DiscoverDependencies(HashSet<Urn> depDiscInputList)
	{
		HashSet<Urn> hashSet = new HashSet<Urn>();
		SqlSmoObject.Trace("Transfer: Discovering dependencies");
		TraceHelper.Assert(null != Database, "null == this.Database");
		bool flag = Options.ScriptSchema && Options.ScriptDrops;
		Urn[] array = new Urn[depDiscInputList.Count];
		depDiscInputList.CopyTo(array);
		DependencyTree dependencyTree = Scripter.DiscoverDependencies(array, !flag);
		DependencyChainCollection dependencies = dependencyTree.Dependencies;
		TraceHelper.Assert(null != dependencies, "GetDependencies() returned null");
		HashSet<Urn> hashSet2 = new HashSet<Urn>();
		if (dependencyTree.FirstChild != null)
		{
			TreeTraversal(dependencyTree.FirstChild, hashSet2);
		}
		Urn urn = "Server/Database[@Name='" + Urn.EscapeString(Database.Name) + "']";
		foreach (Urn item in hashSet2)
		{
			if (!item.Type.Equals("Default", StringComparison.OrdinalIgnoreCase) || !item.Parent.Type.Equals("Column", StringComparison.OrdinalIgnoreCase))
			{
				Urn parent = item.Parent;
				while (parent.XPathExpression.Length > 2)
				{
					parent = parent.Parent;
				}
				if (Database.Parent.CompareUrn(urn, parent) == 0 && item.Type != "UnresolvedEntity")
				{
					hashSet.Add(item);
				}
			}
		}
		return hashSet;
	}

	private void AddWithoutDependencyDiscovery(DependencyCollection depList, DependencyCollectionNode node)
	{
		depList.Insert(0, node);
	}

	private void AddWithoutDependencyDiscoveryCollection(DependencyCollection depList, HashSet<Urn> nonDepHashSet)
	{
		foreach (Urn item in nonDepHashSet)
		{
			AddWithoutDependencyDiscovery(depList, new DependencyCollectionNode(item, isSchemaBound: true, fRoot: true));
		}
	}

	private void AddDiscoveryUnsupportedObjects(HashSet<Urn> list, HashSet<Urn> nonDepDiscList)
	{
		ScriptingPreferences scriptingPreferences = Options.GetScriptingPreferences();
		if (IsSupportedObject<FullTextCatalog>(scriptingPreferences) && (CopyAllObjects || CopyAllFullTextCatalogs || Options.FullTextCatalogs))
		{
			SqlSmoObject.Trace("Transfer: Adding FullTextCatalogs");
			foreach (FullTextCatalog fullTextCatalog in Database.FullTextCatalogs)
			{
				list.Add(fullTextCatalog.Urn);
			}
		}
		if (IsSupportedObject<FullTextStopList>(scriptingPreferences) && (CopyAllObjects || CopyAllFullTextStopLists || Options.FullTextStopLists))
		{
			SqlSmoObject.Trace("Transfer: Adding all FullTextStopLists");
			foreach (FullTextStopList fullTextStopList in Database.FullTextStopLists)
			{
				list.Add(fullTextStopList.Urn);
			}
		}
		if (IsSupportedObject<SearchPropertyList>(scriptingPreferences) && (CopyAllObjects || CopyAllSearchPropertyLists))
		{
			SqlSmoObject.Trace("Transfer: Adding all SearchPropertyLists");
			foreach (SearchPropertyList searchPropertyList in Database.SearchPropertyLists)
			{
				list.Add(searchPropertyList.Urn);
			}
		}
		if (IsSupportedObject<Schema>(scriptingPreferences) && (CopyAllObjects || CopyAllSchemas))
		{
			SqlSmoObject.Trace("Transfer: Adding all user Schemas to dependency list");
			foreach (Schema schema in Database.Schemas)
			{
				if (schema.ID > 4 && (schema.ID < 16384 || schema.ID >= 16400))
				{
					list.Add(schema.Urn);
				}
			}
		}
		list.UnionWith(nonDepDiscList);
	}

	private void PrefetchSecurityObjects(Dictionary<Type, StringCollection> originalDefaultFields)
	{
		ScriptingPreferences scriptingPreferences = Options.GetScriptingPreferences();
		if (PrefetchObjects && CopySchema)
		{
			if (CopyAllObjects || CopyAllUsers)
			{
				Database.PrefetchUsers(scriptingPreferences);
			}
			if (CopyAllObjects || CopyAllRoles)
			{
				Database.PrefetchDatabaseRoles(scriptingPreferences);
			}
			if (CopyAllObjects || CopyAllSchemas)
			{
				Database.PrefetchSchemas(scriptingPreferences);
			}
		}
		else
		{
			SetDefaultInitFields(originalDefaultFields, typeof(User), "IsSystemObject");
			SetDefaultInitFields(originalDefaultFields, typeof(Schema), "ID");
			SetDefaultInitFields(originalDefaultFields, typeof(DatabaseRole), "Owner", "IsFixedRole");
		}
	}

	private void AddSecurityObjectsInOrder(DependencyCollection depList, Dictionary<string, HashSet<Urn>> nonDepListDictionary)
	{
		if (CopyAllObjects || CopyAllRoles)
		{
			SqlSmoObject.Trace("Transfer: Adding DatabaseRoles");
			DependencyObjects dependencyObjects = new DependencyObjects();
			foreach (DatabaseRole role in Database.Roles)
			{
				dependencyObjects.Add(role);
				string owner = role.Owner;
				if (Database.Roles.Contains(owner))
				{
					if (!Database.Roles[owner].IsFixedRole)
					{
						dependencyObjects.Add(Database.Roles[owner], role);
					}
				}
				else if (Database.Users.Contains(owner) && !Database.Users[owner].IsSystemObject)
				{
					dependencyObjects.Add(Database.Users[owner], role);
				}
				StringEnumerator enumerator2 = role.EnumMembers().GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						string current = enumerator2.Current;
						if (Database.Roles.Contains(current))
						{
							dependencyObjects.Add(role, Database.Roles[current]);
						}
						else if (Database.Users.Contains(current))
						{
							dependencyObjects.Add(role, Database.Users[current]);
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
			List<SqlSmoObject> dependencies = dependencyObjects.GetDependencies();
			foreach (SqlSmoObject item in dependencies)
			{
				if (!(item is DatabaseRole))
				{
					continue;
				}
				DatabaseRole databaseRole2 = (DatabaseRole)item;
				if (!databaseRole2.IsFixedRole && databaseRole2.Name != "public")
				{
					AddWithoutDependencyDiscovery(depList, new DependencyCollectionNode(databaseRole2.Urn, isSchemaBound: true, fRoot: true));
					if (nonDepListDictionary[DatabaseRole.UrnSuffix].Contains(databaseRole2.Urn))
					{
						nonDepListDictionary[DatabaseRole.UrnSuffix].Remove(databaseRole2.Urn);
					}
				}
			}
		}
		AddWithoutDependencyDiscoveryCollection(depList, nonDepListDictionary[DatabaseRole.UrnSuffix]);
		AddWithoutDependencyDiscoveryCollection(depList, nonDepListDictionary[ApplicationRole.UrnSuffix]);
		ScriptingPreferences scriptingPreferences = Options.GetScriptingPreferences();
		if ((CopyAllObjects || CopyAllRoles) && IsSupportedObject<ApplicationRole>(scriptingPreferences))
		{
			SqlSmoObject.Trace("Transfer: Adding ApplicationRoles");
			foreach (ApplicationRole applicationRole in Database.ApplicationRoles)
			{
				if (!nonDepListDictionary[ApplicationRole.UrnSuffix].Contains(applicationRole.Urn))
				{
					AddWithoutDependencyDiscovery(depList, new DependencyCollectionNode(applicationRole.Urn, isSchemaBound: true, fRoot: true));
				}
			}
		}
		AddWithoutDependencyDiscoveryCollection(depList, nonDepListDictionary[User.UrnSuffix]);
		if (CopyAllObjects || CopyAllUsers)
		{
			SqlSmoObject.Trace("Transfer: Adding Users");
			foreach (User user in Database.Users)
			{
				if (!user.IsSystemObject && string.Compare(user.Name, "guest", StringComparison.OrdinalIgnoreCase) != 0 && !nonDepListDictionary[User.UrnSuffix].Contains(user.Urn))
				{
					AddWithoutDependencyDiscovery(depList, new DependencyCollectionNode(user.Urn, isSchemaBound: true, fRoot: true));
				}
			}
		}
		AddWithoutDependencyDiscoveryCollection(depList, nonDepListDictionary[Endpoint.UrnSuffix]);
		AddWithoutDependencyDiscoveryCollection(depList, nonDepListDictionary[Login.UrnSuffix]);
		if (!CopyAllLogins)
		{
			return;
		}
		SqlSmoObject.Trace("Transfer: Adding Logins");
		foreach (Login login in Database.Parent.Logins)
		{
			if (!login.IsSystemObject && !nonDepListDictionary[Login.UrnSuffix].Contains(login.Urn))
			{
				AddWithoutDependencyDiscovery(depList, new DependencyCollectionNode(login.Urn, isSchemaBound: true, fRoot: true));
			}
		}
	}

	private void AddSecurityObjectsWithoutOrder(DependencyCollection depList, Dictionary<string, HashSet<Urn>> nonDepListDictionary)
	{
		if (CopyAllObjects || CopyAllRoles)
		{
			SqlSmoObject.Trace("Transfer: Adding DatabaseRoles");
			foreach (DatabaseRole role in Database.Roles)
			{
				if (!role.IsFixedRole && role.Name != "public")
				{
					nonDepListDictionary[DatabaseRole.UrnSuffix].Add(role.Urn);
				}
			}
		}
		ScriptingPreferences scriptingPreferences = Options.GetScriptingPreferences();
		if ((CopyAllObjects || CopyAllRoles) && IsSupportedObject<ApplicationRole>(scriptingPreferences))
		{
			SqlSmoObject.Trace("Transfer: Adding ApplicationRoles");
			foreach (ApplicationRole applicationRole in Database.ApplicationRoles)
			{
				nonDepListDictionary[ApplicationRole.UrnSuffix].Add(applicationRole.Urn);
			}
		}
		if (CopyAllObjects || CopyAllUsers)
		{
			SqlSmoObject.Trace("Transfer: Adding Users");
			foreach (User user in Database.Users)
			{
				if (!user.IsSystemObject && string.Compare(user.Name, "guest", StringComparison.OrdinalIgnoreCase) != 0)
				{
					nonDepListDictionary[User.UrnSuffix].Add(user.Urn);
				}
			}
		}
		if (CopyAllLogins)
		{
			SqlSmoObject.Trace("Transfer: Adding Logins");
			foreach (Login login in Database.Parent.Logins)
			{
				if (!login.IsSystemObject)
				{
					nonDepListDictionary[Login.UrnSuffix].Add(login.Urn);
				}
			}
		}
		AddWithoutDependencyDiscoveryCollection(depList, nonDepListDictionary[User.UrnSuffix]);
		AddWithoutDependencyDiscoveryCollection(depList, nonDepListDictionary[DatabaseRole.UrnSuffix]);
		AddWithoutDependencyDiscoveryCollection(depList, nonDepListDictionary[ApplicationRole.UrnSuffix]);
		AddWithoutDependencyDiscoveryCollection(depList, nonDepListDictionary[Endpoint.UrnSuffix]);
		AddWithoutDependencyDiscoveryCollection(depList, nonDepListDictionary[Login.UrnSuffix]);
	}

	private void AddDiscoverableObjects(HashSet<Urn> depDiscInputList, Dictionary<Type, StringCollection> originalDefaultFields)
	{
		ScriptingPreferences scriptingPreferences = Options.GetScriptingPreferences();
		if (IsSupportedObject<PartitionFunction>(scriptingPreferences))
		{
			AddAllObjects<PartitionFunction>(depDiscInputList, Database.PartitionFunctions, CopyAllPartitionFunctions, Database.PrefetchPartitionFunctions, originalDefaultFields, new string[0]);
		}
		if (IsSupportedObject<PartitionScheme>(scriptingPreferences))
		{
			AddAllObjects<PartitionScheme>(depDiscInputList, Database.PartitionSchemes, CopyAllPartitionSchemes, Database.PrefetchPartitionSchemes, originalDefaultFields, new string[0]);
		}
		if (IsSupportedObject<DatabaseScopedCredential>(scriptingPreferences))
		{
			AddAllObjects<DatabaseScopedCredential>(depDiscInputList, Database.DatabaseScopedCredentials, CopyAllDatabaseScopedCredentials, Database.PrefetchDatabaseScopedCredentials, null, new string[0]);
		}
		if (IsSupportedObject<ExternalFileFormat>(scriptingPreferences))
		{
			AddAllObjects<ExternalFileFormat>(depDiscInputList, Database.ExternalFileFormats, CopyAllExternalFileFormats, Database.PrefetchExternalFileFormats, null, new string[0]);
		}
		if (IsSupportedObject<ExternalDataSource>(scriptingPreferences))
		{
			AddAllObjects<ExternalDataSource>(depDiscInputList, Database.ExternalDataSources, CopyAllExternalDataSources, Database.PrefetchExternalDataSources, null, new string[0]);
		}
		if (CopyAllObjects || CopyAllTables)
		{
			if (PrefetchObjects)
			{
				Database.PrefetchTables(scriptingPreferences);
			}
			else
			{
				SetDefaultInitFields(originalDefaultFields, typeof(Table), "ID", "IsSystemObject");
			}
			AddAllNonSystemObjects<Table>(depDiscInputList, Database.Tables);
		}
		if (IsSupportedObject<View>(scriptingPreferences))
		{
			AddAllNonSystemObjects<View>(depDiscInputList, Database.Views, CopyAllViews, Database.PrefetchViews, originalDefaultFields, new string[2] { "ID", "IsSystemObject" });
		}
		if (IsSupportedObject<StoredProcedure>(scriptingPreferences))
		{
			AddAllNonSystemObjects<StoredProcedure>(depDiscInputList, Database.StoredProcedures, CopyAllStoredProcedures, Database.PrefetchStoredProcedures, originalDefaultFields, new string[1] { "IsSystemObject" });
		}
		if (IsSupportedObject<UserDefinedFunction>(scriptingPreferences))
		{
			AddAllNonSystemObjects<UserDefinedFunction>(depDiscInputList, Database.UserDefinedFunctions, CopyAllUserDefinedFunctions, Database.PrefetchUserDefinedFunctions, originalDefaultFields, new string[1] { "IsSystemObject" });
		}
		if (IsSupportedObject<XmlSchemaCollection>(scriptingPreferences))
		{
			AddAllObjects<XmlSchemaCollection>(depDiscInputList, Database.XmlSchemaCollections, CopyAllXmlSchemaCollections, Database.PrefetchXmlSchemaCollections, null, new string[0]);
		}
		if (IsSupportedObject<SqlAssembly>(scriptingPreferences))
		{
			AddAllNonSystemObjects<SqlAssembly>(depDiscInputList, Database.Assemblies, CopyAllSqlAssemblies, Database.PrefetchSqlAssemblies, originalDefaultFields, new string[1] { "IsSystemObject" });
		}
		if (IsSupportedObject<UserDefinedAggregate>(scriptingPreferences))
		{
			AddAllObjects<UserDefinedAggregate>(depDiscInputList, Database.UserDefinedAggregates, CopyAllUserDefinedAggregates, Database.PrefetchUserDefinedAggregates, null, new string[0]);
		}
		if (IsSupportedObject<UserDefinedType>(scriptingPreferences))
		{
			AddAllObjects<UserDefinedType>(depDiscInputList, Database.UserDefinedTypes, CopyAllUserDefinedTypes, Database.PrefetchUserDefinedTypes, null, new string[0]);
		}
		if (IsSupportedObject<PlanGuide>(scriptingPreferences))
		{
			AddAllObjects<PlanGuide>(depDiscInputList, Database.PlanGuides, CopyAllPlanGuides, null, null, new string[0]);
		}
		if (IsSupportedObject<UserDefinedTableType>(scriptingPreferences))
		{
			AddAllObjects<UserDefinedTableType>(depDiscInputList, Database.UserDefinedTableTypes, CopyAllUserDefinedTableTypes, Database.PrefetchUserDefinedTableTypes, null, new string[0]);
		}
		if (IsSupportedObject<Synonym>(scriptingPreferences))
		{
			AddAllObjects<Synonym>(depDiscInputList, Database.Synonyms, CopyAllSynonyms, null, null, new string[0]);
		}
		if (IsSupportedObject<Sequence>(scriptingPreferences))
		{
			AddAllObjects<Sequence>(depDiscInputList, Database.Sequences, CopyAllSequences, null, null, new string[0]);
		}
		if (IsSupportedObject<DatabaseDdlTrigger>(scriptingPreferences))
		{
			AddAllObjects<DatabaseDdlTrigger>(depDiscInputList, Database.Triggers, CopyAllDatabaseTriggers, null, null, new string[0]);
		}
		if (IsSupportedObject<ExternalLibrary>(scriptingPreferences))
		{
			AddAllObjects<ExternalLibrary>(depDiscInputList, Database.ExternalLibraries, CopyAllExternalLibraries, Database.PrefetchExternalLibraries, null, new string[0]);
		}
		if (IsSupportedObject<UserDefinedDataType>(scriptingPreferences))
		{
			AddAllObjects<UserDefinedDataType>(depDiscInputList, Database.UserDefinedDataTypes, CopyAllUserDefinedDataTypes, null, null, new string[0]);
		}
		if (IsSupportedObject<Rule>(scriptingPreferences))
		{
			AddAllObjects<Rule>(depDiscInputList, Database.Rules, CopyAllRules, Database.PrefetchRules, null, new string[0]);
		}
		if (IsSupportedObject<Default>(scriptingPreferences))
		{
			AddAllObjects<Default>(depDiscInputList, Database.Defaults, CopyAllDefaults, Database.PrefetchDefaults, null, new string[0]);
		}
	}

	private void AddAllObjects<T>(ICollection<Urn> List, SmoCollectionBase collection) where T : SqlSmoObject
	{
		SqlSmoObject.Trace(string.Format(SmoApplication.DefaultCulture, "Transfer: Adding all objects {0} to dependency list", new object[1] { collection.GetType().Name }));
		foreach (T item in collection)
		{
			T val = item;
			List.Add(val.Urn);
		}
	}

	private void AddAllObjects<T>(ICollection<Urn> List, SmoCollectionBase collection, bool copyAll, Action<ScriptingPreferences> prefetch, Dictionary<Type, StringCollection> originalDefaultFields, params string[] fields) where T : SqlSmoObject
	{
		if (CopyAllObjects || copyAll)
		{
			if (PrefetchObjects && CopySchema)
			{
				prefetch?.Invoke(Options.GetScriptingPreferences());
			}
			else if (originalDefaultFields != null)
			{
				SetDefaultInitFields(originalDefaultFields, typeof(T), fields);
			}
			AddAllObjects<T>(List, collection);
		}
	}

	private void AddAllNonSystemObjects<T>(ICollection<Urn> List, SmoCollectionBase collection) where T : SqlSmoObject
	{
		SqlSmoObject.Trace(string.Format(SmoApplication.DefaultCulture, "Transfer: Adding all objects in {0} to dependency list", new object[1] { collection.GetType().Name }));
		foreach (T item in collection)
		{
			T val = item;
			if (!val.GetPropValueOptional("IsSystemObject", defaultValue: false))
			{
				List.Add(val.Urn);
			}
		}
	}

	private void AddAllNonSystemObjects<T>(ICollection<Urn> List, SmoCollectionBase collection, bool copyAll, Action<ScriptingPreferences> prefetch, Dictionary<Type, StringCollection> originalDefaultFields, params string[] fields) where T : SqlSmoObject
	{
		if (CopyAllObjects || copyAll)
		{
			if (PrefetchObjects && CopySchema)
			{
				prefetch?.Invoke(Options.GetScriptingPreferences());
			}
			else if (originalDefaultFields != null)
			{
				SetDefaultInitFields(originalDefaultFields, typeof(T), fields);
			}
			AddAllNonSystemObjects<T>(List, collection);
		}
	}

	private bool IsSupportedObject<T>(ScriptingPreferences sp) where T : SqlSmoObject
	{
		if (SmoUtility.IsSupportedObject(typeof(T), Database.ServerVersion, Database.DatabaseEngineType, Database.DatabaseEngineEdition))
		{
			return SmoUtility.IsSupportedObject(typeof(T), ScriptingOptions.ConvertToServerVersion(sp.TargetServerVersion), sp.TargetDatabaseEngineType, sp.TargetDatabaseEngineEdition);
		}
		return false;
	}

	private void SeparateDiscoverySupportedObjects(HashSet<Urn> depDiscInputList, HashSet<Urn> nonDepDiscList, Dictionary<string, HashSet<Urn>> nonDepListDictionary, HashSet<Urn> nonDepList)
	{
		foreach (object @object in ObjectList)
		{
			if (!(@object is SqlSmoObject))
			{
				if (!Scripter.Options.ContinueScriptingOnError)
				{
					if (@object == null)
					{
						throw new ArgumentNullException();
					}
					throw new SmoException(ExceptionTemplatesImpl.NeedToPassObject(@object.GetType().ToString()));
				}
				continue;
			}
			switch (@object.GetType().Name)
			{
			case "View":
			case "StoredProcedure":
			case "UserDefinedFunction":
			case "PartitionScheme":
			case "PartitionFunction":
			case "XmlSchemaCollection":
			case "UserDefinedAggregate":
			case "UserDefinedType":
			case "SqlAssembly":
			case "Synonym":
			case "Sequence":
			case "PlanGuide":
			case "UserDefinedTableType":
			case "Rule":
			case "Default":
			case "DdlTrigger":
			case "Trigger":
			case "UserDefinedDataType":
			case "Table":
				depDiscInputList.Add(((SqlSmoObject)@object).Urn);
				break;
			case "FullTextCatalog":
			case "FullTextStopList":
			case "SearchPropertyList":
			case "Schema":
				nonDepDiscList.Add(((SqlSmoObject)@object).Urn);
				break;
			case "User":
			case "ApplicationRole":
			case "Role":
			case "Login":
			case "Endpoint":
				nonDepListDictionary[((SqlSmoObject)@object).Urn.Type].Add(((SqlSmoObject)@object).Urn);
				break;
			default:
				nonDepList.Add(((SqlSmoObject)@object).Urn);
				break;
			}
		}
	}

	private void CheckDownLevelScripting(DependencyCollection depList)
	{
		if (Options.TargetServerVersionInternal >= SqlServerVersionInternal.Version100 || Database.CompatibilityLevel < CompatibilityLevel.Version90)
		{
			return;
		}
		ArrayList arrayList = new ArrayList();
		foreach (DependencyCollectionNode dep in depList)
		{
			if (!CanScriptDownlevel(dep.Urn, Options.TargetServerVersionInternal))
			{
				if (!Options.ContinueScriptingOnError)
				{
					throw new SmoException(ExceptionTemplatesImpl.UnsupportedVersionException);
				}
				arrayList.Add(dep);
			}
		}
		foreach (DependencyCollectionNode item in arrayList)
		{
			depList.Remove(item);
		}
	}

	private void TreeTraversal(DependencyTreeNode node, HashSet<Urn> visitedUrns)
	{
		if (visitedUrns.Add(node.Urn))
		{
			if (node.NextSibling != null)
			{
				TreeTraversal(node.NextSibling, visitedUrns);
			}
			if (node.HasChildNodes)
			{
				TreeTraversal(node.FirstChild, visitedUrns);
			}
		}
	}

	private void SetDefaultInitFields(Dictionary<Type, StringCollection> originalDefaultFields, Type type, params string[] fields)
	{
		originalDefaultFields[type] = Database.GetServerObject().GetDefaultInitFields(type);
		Database.GetServerObject().SetDefaultInitFields(type, fields);
	}

	private void RestoreDefaultInitFields(Dictionary<Type, StringCollection> originalDefaultFields)
	{
		if (originalDefaultFields == null)
		{
			return;
		}
		foreach (KeyValuePair<Type, StringCollection> originalDefaultField in originalDefaultFields)
		{
			Database.GetServerObject().SetDefaultInitFields(originalDefaultField.Key, originalDefaultField.Value);
		}
	}

	private bool CanScriptDownlevel(Urn urn, SqlServerVersionInternal targetVersion)
	{
		TraceHelper.Assert(null != urn, "null == urn");
		SqlSmoObject smoObject = Database.Parent.GetSmoObject(urn);
		if (smoObject is Table && ContainsUnsupportedType(((Table)smoObject).Columns, targetVersion))
		{
			return false;
		}
		if (smoObject is View && ContainsUnsupportedType(((View)smoObject).Columns, targetVersion))
		{
			return false;
		}
		if (smoObject is StoredProcedure)
		{
			StoredProcedure storedProcedure = (StoredProcedure)smoObject;
			if (ContainsUnsupportedType(storedProcedure.Parameters, targetVersion) || (storedProcedure.ImplementationType == ImplementationType.SqlClr && targetVersion < SqlServerVersionInternal.Version90))
			{
				return false;
			}
		}
		if (smoObject is UserDefinedFunction)
		{
			UserDefinedFunction userDefinedFunction = (UserDefinedFunction)smoObject;
			if (ContainsUnsupportedType(userDefinedFunction.Parameters, targetVersion) || (userDefinedFunction.DataType != null && userDefinedFunction.FunctionType == UserDefinedFunctionType.Scalar && IsUnsupportedType(userDefinedFunction.DataType.SqlDataType, targetVersion)) || (userDefinedFunction.ImplementationType == ImplementationType.SqlClr && targetVersion < SqlServerVersionInternal.Version90))
			{
				return false;
			}
		}
		if (smoObject is UserDefinedDataType && ((UserDefinedDataType)smoObject).SystemType == "xml" && targetVersion < SqlServerVersionInternal.Version90)
		{
			return false;
		}
		return true;
	}

	private bool IsUnsupportedType(SqlDataType type, SqlServerVersionInternal targetVersion)
	{
		if (targetVersion == SqlServerVersionInternal.Version80)
		{
			switch (type)
			{
			case SqlDataType.NVarCharMax:
			case SqlDataType.VarBinaryMax:
			case SqlDataType.VarCharMax:
			case SqlDataType.Xml:
			case SqlDataType.UserDefinedTableType:
			case SqlDataType.HierarchyId:
			case SqlDataType.Geometry:
			case SqlDataType.Geography:
				return true;
			default:
				return false;
			}
		}
		switch (type)
		{
		case SqlDataType.UserDefinedTableType:
		case SqlDataType.HierarchyId:
		case SqlDataType.Geometry:
		case SqlDataType.Geography:
			return true;
		default:
			return false;
		}
	}

	private bool ContainsUnsupportedType(ParameterCollectionBase parms, SqlServerVersionInternal targetVersion)
	{
		foreach (Parameter parm in parms)
		{
			if (IsUnsupportedType(parm.DataType.SqlDataType, targetVersion))
			{
				return true;
			}
		}
		return false;
	}

	private bool ContainsUnsupportedType(ColumnCollection cols, SqlServerVersionInternal targetVersion)
	{
		foreach (Column col in cols)
		{
			if (IsUnsupportedType(col.DataType.SqlDataType, targetVersion))
			{
				return true;
			}
		}
		return false;
	}

	private DependencyCollection GetDependencyOrderedCollection(HashSet<Urn> transferSet)
	{
		SmoDependencyOrderer smoDependencyOrderer = new SmoDependencyOrderer(Database.GetServerObject());
		smoDependencyOrderer.ScriptingPreferences = (ScriptingPreferences)Options.GetScriptingPreferences().Clone();
		smoDependencyOrderer.ScriptingPreferences.IncludeScripts.Data = CopyData;
		smoDependencyOrderer.ScriptingPreferences.IncludeScripts.Ddl = CopySchema;
		smoDependencyOrderer.ScriptingPreferences.Behavior = ScriptBehavior.Create;
		smoDependencyOrderer.ScriptingPreferences.IncludeScripts.Associations = false;
		smoDependencyOrderer.ScriptingPreferences.IncludeScripts.Permissions = false;
		smoDependencyOrderer.ScriptingPreferences.IncludeScripts.Owner = false;
		smoDependencyOrderer.creatingDictionary = new CreatingObjectDictionary(Database.GetServerObject());
		List<Urn> list = new List<Urn>();
		list.AddRange(transferSet);
		List<Urn> list2 = smoDependencyOrderer.Order(list);
		DependencyCollection dependencyCollection = new DependencyCollection();
		if (CopySchema)
		{
			foreach (Urn item in list2)
			{
				if (!item.Type.Equals("Special"))
				{
					dependencyCollection.Add(new DependencyCollectionNode(item, isSchemaBound: true, fRoot: true));
				}
				else if (item.Parent.Type == "Object")
				{
					dependencyCollection.Add(new DependencyCollectionNode(item.Parent.Parent, isSchemaBound: true, fRoot: true));
				}
			}
		}
		else
		{
			foreach (Urn item2 in list2)
			{
				TraceHelper.Assert(item2.Type.Equals("Special") && item2.Parent.Type == "Data", "only data entries expected");
				dependencyCollection.Add(new DependencyCollectionNode(item2.Parent.Parent, isSchemaBound: true, fRoot: true));
			}
		}
		return dependencyCollection;
	}
}
