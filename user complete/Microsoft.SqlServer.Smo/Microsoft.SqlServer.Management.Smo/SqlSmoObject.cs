using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Management.Smo.Agent;
using Microsoft.SqlServer.Management.Smo.Broker;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(LocalizableTypeConverter))]
public abstract class SqlSmoObject : SmoObjectBase, ISfcPropertyProvider, ISfcNotifyPropertyMetadataChanged, INotifyPropertyChanged, IRefreshable, IAlienObject, ISqlSmoObjectInitialize
{
	internal class PropagateInfo
	{
		public ICollection col;

		public SqlSmoObject obj;

		public bool bWithScript;

		public bool bPropagateScriptToChildLevel;

		private string urnTypeKey;

		internal string UrnTypeKey
		{
			get
			{
				return urnTypeKey;
			}
			set
			{
				urnTypeKey = value;
			}
		}

		private void Init(ICollection col, bool bWithScript, string urnTypeKey, bool bPropagateScriptToChildLevel)
		{
			this.col = col;
			this.bWithScript = bWithScript;
			this.urnTypeKey = urnTypeKey;
			this.bPropagateScriptToChildLevel = bPropagateScriptToChildLevel;
		}

		private void Init(SqlSmoObject obj, bool bWithScript, string urnTypeKey, bool bPropagateScriptToChildLevel)
		{
			this.obj = obj;
			this.bWithScript = bWithScript;
			this.urnTypeKey = urnTypeKey;
			this.bPropagateScriptToChildLevel = bPropagateScriptToChildLevel;
		}

		internal PropagateInfo(ICollection col)
		{
			Init(col, bWithScript: true, null, bPropagateScriptToChildLevel: true);
		}

		internal PropagateInfo(ICollection col, bool bWithScript)
		{
			Init(col, bWithScript, null, bPropagateScriptToChildLevel: true);
		}

		internal PropagateInfo(ICollection col, bool bWithScript, bool bPropagateScriptToChildLevel)
		{
			Init(col, bWithScript, null, bPropagateScriptToChildLevel);
		}

		internal PropagateInfo(ICollection col, bool bWithScript, string urnTypeKey)
		{
			Init(col, bWithScript, urnTypeKey, bPropagateScriptToChildLevel: true);
		}

		internal PropagateInfo(SqlSmoObject obj)
		{
			Init(obj, bWithScript: true, null, bPropagateScriptToChildLevel: true);
		}

		internal PropagateInfo(SqlSmoObject obj, bool bWithScript)
		{
			Init(obj, bWithScript, null, bPropagateScriptToChildLevel: true);
		}

		internal PropagateInfo(SqlSmoObject obj, bool bWithScript, string urnTypeKey)
		{
			Init(obj, bWithScript, urnTypeKey, bPropagateScriptToChildLevel: true);
		}
	}

	internal enum PropagateAction
	{
		Create,
		Alter,
		Drop,
		CreateOrAlter
	}

	internal const BindingFlags UrnSuffixBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty;

	private static Dictionary<Type, string> s_SingletonTypeToProperty;

	private static Dictionary<Type, string[]> s_TypeToKeyFields;

	private bool initializedForScripting;

	internal bool objectInSpace;

	protected ExtendedPropertyCollection m_ExtendedProperties;

	private AbstractCollectionBase parentColl;

	internal ObjectKeyBase key;

	protected SqlSmoObject singletonParent;

	internal SqlPropertyCollection properties;

	private Server m_server;

	private bool executeForScalar;

	private object[] scalarResult;

	private UserPermissionCollection userPermissions;

	internal StringComparer m_comparer;

	private ObjectComparerBase keyComparer;

	private bool m_bIgnoreForScripting;

	private IRenewableToken accessToken;

	private bool isTouched;

	private static readonly IList<string> DATABASE_SPECIAL_PROPS;

	private Dictionary<string, StringCollection> roleToLoginCache = new Dictionary<string, StringCollection>();

	internal bool InitializedForScripting
	{
		get
		{
			return initializedForScripting;
		}
		set
		{
			initializedForScripting = value;
		}
	}

	protected bool ObjectInSpace => objectInSpace;

	internal AbstractCollectionBase ParentColl
	{
		get
		{
			return parentColl;
		}
		set
		{
			parentColl = value;
		}
	}

	public AbstractCollectionBase ParentCollection => ParentColl;

	internal virtual string FullQualifiedName => key.ToString();

	internal virtual string InternalName => key.ToString();

	internal string UrnSkeleton
	{
		get
		{
			CheckObjectState();
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			GetUrnSkellRecursive(stringBuilder);
			return stringBuilder.ToString();
		}
	}

	public Urn Urn
	{
		get
		{
			CheckObjectStateImpl(throwIfNotCreated: false);
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			GetUrnRecursive(stringBuilder);
			return new Urn(stringBuilder.ToString());
		}
	}

	internal Urn UrnWithId
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			GetUrnRecursive(stringBuilder, UrnIdOption.WithId);
			return new Urn(stringBuilder.ToString());
		}
	}

	internal Urn UrnOnlyId
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			GetUrnRecursive(stringBuilder, UrnIdOption.OnlyId);
			return new Urn(stringBuilder.ToString());
		}
	}

	public SqlPropertyCollection Properties
	{
		get
		{
			CheckObjectStateImpl(throwIfNotCreated: false);
			if (properties == null)
			{
				properties = new SqlPropertyCollection(this, GetPropertyMetadataProvider());
			}
			return properties;
		}
	}

	protected bool ExecuteForScalar
	{
		get
		{
			return executeForScalar;
		}
		set
		{
			executeForScalar = value;
		}
	}

	protected object[] ScalarResult => scalarResult;

	internal virtual UserPermissionCollection Permissions => null;

	internal StringComparer StringComparer
	{
		get
		{
			InitializeStringComparer();
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(m_comparer != null);
			return m_comparer;
		}
	}

	internal ObjectComparerBase KeyComparer
	{
		get
		{
			if (keyComparer == null)
			{
				StringComparer stringComparer = null;
				stringComparer = ((ParentColl != null && ParentColl.ParentInstance != null) ? ParentColl.ParentInstance.StringComparer : ((!(this is Database)) ? StringComparer : GetDbComparer(inServer: true)));
				keyComparer = key.GetComparer(stringComparer);
			}
			return keyComparer;
		}
	}

	internal bool IgnoreForScripting
	{
		get
		{
			CheckObjectState();
			return m_bIgnoreForScripting;
		}
		set
		{
			CheckObjectState();
			m_bIgnoreForScripting = value;
		}
	}

	public ServerVersion ServerVersion
	{
		get
		{
			if (TryGetServerObject() != null)
			{
				return GetServerObject().ExecutionManager.GetServerVersion();
			}
			return VersionUtils.HighestKnownServerVersion;
		}
	}

	public virtual DatabaseEngineType DatabaseEngineType => TryGetServerObject()?.ExecutionManager.GetDatabaseEngineType() ?? DatabaseEngineType.Unknown;

	public virtual DatabaseEngineEdition DatabaseEngineEdition => GetParentObject(throwIfParentIsCreating: false, throwIfParentNotExist: false)?.DatabaseEngineEdition ?? ExecutionManager.GetDatabaseEngineEdition();

	internal virtual bool IsCloudSupported => IsSupportedOnSqlAzure(GetType());

	internal ServerInformation ServerInfo
	{
		get
		{
			Server server = TryGetServerObject();
			return new ServerInformation(ExecutionManager.GetServerVersion(), ExecutionManager.GetProductVersion(), ExecutionManager.GetDatabaseEngineType(), ExecutionManager.GetDatabaseEngineEdition(), (server == null) ? "Windows" : server.HostPlatform, ExecutionManager.GetConnectionProtocol());
		}
	}

	internal bool InternalIsObjectDirty => IsObjectDirty();

	protected bool IsTouched => isTouched;

	public virtual ExecutionManager ExecutionManager
	{
		get
		{
			SqlSmoObject sqlSmoObject = null;
			if (parentColl != null)
			{
				sqlSmoObject = parentColl.ParentInstance;
			}
			else if (singletonParent != null)
			{
				sqlSmoObject = singletonParent;
			}
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sqlSmoObject, "parent == null");
			return sqlSmoObject.ExecutionManager;
		}
	}

	internal bool IsDesignMode
	{
		get
		{
			bool result = false;
			if (this is ISfcSupportsDesignMode)
			{
				ISfcHasConnection sfcHasConnection = TryGetServerObject();
				if (sfcHasConnection != null)
				{
					result = sfcHasConnection.ConnectionContext.Mode == SfcConnectionContextMode.Offline;
				}
			}
			return result;
		}
	}

	internal bool SupportsDesignMode => this is ISfcSupportsDesignMode;

	internal override bool ShouldNotifyPropertyMetadataChange => this.PropertyMetadataChanged != null;

	internal override bool ShouldNotifyPropertyChange => this.PropertyChanged != null;

	public static event EventHandler<PropertyMissingEventArgs> PropertyMissing;

	[CLSCompliant(false)]
	public event EventHandler<SfcPropertyMetadataChangedEventArgs> PropertyMetadataChanged;

	public event PropertyChangedEventHandler PropertyChanged;

	internal SqlSmoObject(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
	{
		SetObjectKey(key);
		this.parentColl = parentColl;
		Init();
		SetState(state);
	}

	internal virtual SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return null;
	}

	internal SqlSmoObject(ObjectKeyBase key, SqlSmoState state)
	{
		SetObjectKey(key);
		Init();
		SetState(state);
	}

	protected internal SqlSmoObject()
	{
		Init();
		SetState(SqlSmoState.Pending);
		objectInSpace = true;
		key = GetEmptyKey();
	}

	internal virtual ObjectKeyBase GetEmptyKey()
	{
		return new ObjectKeyBase();
	}

	private void Init()
	{
		propertyBagState = PropertyBagState.Empty;
		properties = null;
		m_bIgnoreForScripting = false;
		m_comparer = null;
		m_ExtendedProperties = null;
	}

	protected internal bool IsObjectInSpace()
	{
		if (base.State == SqlSmoState.Pending)
		{
			return true;
		}
		SqlSmoObject sqlSmoObject = this;
		while (sqlSmoObject != null && !(sqlSmoObject is Server))
		{
			if (sqlSmoObject.ObjectInSpace)
			{
				return true;
			}
			if (sqlSmoObject.ParentColl == null || sqlSmoObject.ParentColl.ParentInstance == null)
			{
				PropertyInfo property = sqlSmoObject.GetType().GetProperty("Parent", BindingFlags.Instance | BindingFlags.Public);
				if (null == property)
				{
					throw new InternalSmoErrorException(ExceptionTemplatesImpl.GetParentFailed);
				}
				sqlSmoObject = property.GetValue(sqlSmoObject, null) as SqlSmoObject;
			}
			else
			{
				sqlSmoObject = sqlSmoObject.ParentColl.ParentInstance;
			}
		}
		return false;
	}

	protected virtual void MarkDropped()
	{
		SetState(SqlSmoState.Dropped);
		if (userPermissions != null)
		{
			userPermissions.MarkAllDropped();
		}
	}

	internal void MarkDroppedInternal()
	{
		MarkDropped();
	}

	protected void MarkForDropImpl(bool dropOnAlter)
	{
		CheckObjectState();
		if (base.State != SqlSmoState.Existing && base.State != SqlSmoState.ToBeDropped)
		{
			throw new InvalidSmoOperationException("MarkForDrop", base.State);
		}
		if (dropOnAlter)
		{
			SetState(SqlSmoState.ToBeDropped);
		}
		else if (base.State == SqlSmoState.ToBeDropped)
		{
			SetState(SqlSmoState.Existing);
		}
	}

	protected void CheckObjectState()
	{
		CheckObjectState(throwIfNotCreated: false);
	}

	protected virtual void CheckObjectState(bool throwIfNotCreated)
	{
		CheckObjectStateImpl(throwIfNotCreated);
	}

	protected void CheckObjectStateImpl(bool throwIfNotCreated)
	{
		CheckPendingState();
		if (base.State == SqlSmoState.Dropped)
		{
			throw new SmoException(ExceptionTemplatesImpl.ObjectDroppedExceptionText(GetType().ToString(), key.ToString()));
		}
		if (throwIfNotCreated && base.State == SqlSmoState.Creating)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.ErrorInCreatingState);
		}
	}

	public override string ToString()
	{
		if (key.GetType().Name == "ObjectKeyBase")
		{
			return base.ToString();
		}
		return key.ToString();
	}

	public static Type GetTypeFromUrnSkeleton(Urn urn)
	{
		XPathExpression xPathExpression = urn.XPathExpression;
		Type type = null;
		string parentName = null;
		for (int i = 0; i < xPathExpression.Length; i++)
		{
			type = GetChildType(xPathExpression[i].Name, parentName);
			if (null == type)
			{
				break;
			}
			parentName = type.Name;
		}
		return type;
	}

	internal void GetUrnSkellRecursive(StringBuilder urnbuilder)
	{
		string text = null;
		text = ((!(GetType().GetBaseType() == typeof(Parameter))) ? (GetType().InvokeMember("UrnSuffix", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty, null, null, new object[0], SmoApplication.DefaultCulture) as string) : (GetType().GetBaseType().GetBaseType().InvokeMember("UrnSuffix", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty, null, null, new object[0], SmoApplication.DefaultCulture) as string));
		if (text.Length != 0)
		{
			if (ParentColl != null)
			{
				ParentColl.ParentInstance.GetUrnSkellRecursive(urnbuilder);
			}
			else if (!(this is Server))
			{
				SqlSmoObject sqlSmoObject = (SqlSmoObject)GetType().InvokeMember("Parent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, this, new object[0], SmoApplication.DefaultCulture);
				sqlSmoObject.GetUrnSkellRecursive(urnbuilder);
			}
			if (urnbuilder.Length != 0)
			{
				urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { text });
			}
			else
			{
				urnbuilder.Append("Server");
			}
		}
	}

	internal void GetUrnRecImpl(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		GetUrnRecursive(urnbuilder, idOption);
	}

	protected virtual void GetUrnRecursive(StringBuilder urnbuilder)
	{
		GetUrnRecursive(urnbuilder, UrnIdOption.NoId);
	}

	protected virtual void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		string text = null;
		if (GetType().GetBaseType() == typeof(Parameter))
		{
			PropertyInfo property = GetType().GetBaseType().GetBaseType().GetProperty("UrnSuffix", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty);
			text = property.GetValue(this, null) as string;
		}
		else
		{
			PropertyInfo property2 = GetType().GetProperty("UrnSuffix", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty);
			text = property2.GetValue(this, null) as string;
		}
		if (text.Length == 0)
		{
			return;
		}
		if (ParentColl != null)
		{
			ParentColl.ParentInstance.GetUrnRecursive(urnbuilder, idOption);
		}
		if (urnbuilder.Length != 0)
		{
			switch (idOption)
			{
			default:
				return;
			case UrnIdOption.WithId:
				if (Properties.Contains("ID"))
				{
					urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}[{1} and @ID={2}]", new object[3]
					{
						text,
						key.UrnFilter,
						GetPropValueOptional("ID", 0).ToString(SmoApplication.DefaultCulture)
					});
					return;
				}
				break;
			case UrnIdOption.OnlyId:
				if (Properties.Contains("ID"))
				{
					urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}[@ID={1}]", new object[2]
					{
						text,
						GetPropValueOptional("ID", 0).ToString(SmoApplication.DefaultCulture)
					});
					return;
				}
				break;
			case UrnIdOption.NoId:
				break;
			}
			urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}[{1}]", new object[2] { text, key.UrnFilter });
		}
		else if (GetServerObject().ExecutionManager.TrueServerName == null)
		{
			urnbuilder.Append(text);
		}
		else
		{
			urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}[@Name='{1}']", new object[2]
			{
				text,
				Urn.EscapeString(GetServerObject().ExecutionManager.TrueServerName)
			});
		}
	}

	internal void SetObjectKey(ObjectKeyBase key)
	{
		this.key = key;
	}

	internal virtual void ValidateParent(SqlSmoObject newParent)
	{
		if (!(GetType().InvokeMember("UrnSuffix", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty, null, null, new object[0], SmoApplication.DefaultCulture) is string { Length: not 0 } text))
		{
			throw new InternalSmoErrorException(ExceptionTemplatesImpl.NoUrnSuffix);
		}
		try
		{
			parentColl = GetChildCollection(newParent, text, null, newParent.ServerVersion);
		}
		catch (ArgumentException)
		{
			PropertyInfo property;
			try
			{
				property = newParent.GetType().GetProperty(GetType().Name);
			}
			catch (MissingMethodException)
			{
				throw new ArgumentException(ExceptionTemplatesImpl.InvalidPathChildCollectionNotFound(text, newParent.GetType().Name));
			}
			if (null != property)
			{
				singletonParent = newParent;
				return;
			}
		}
		if (parentColl == null && singletonParent == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.SetParent, this, null, ExceptionTemplatesImpl.InvalidType(newParent.GetType().ToString()));
		}
	}

	protected internal void SetParentImpl(SqlSmoObject newParent)
	{
		try
		{
			if (newParent == null)
			{
				throw new ArgumentNullException("newParent");
			}
			if (base.State != SqlSmoState.Pending)
			{
				throw new InvalidSmoOperationException(ExceptionTemplatesImpl.OperationOnlyInPendingState);
			}
			if (newParent.State == SqlSmoState.Pending)
			{
				throw new InvalidSmoOperationException(ExceptionTemplatesImpl.OperationOnlyInPendingState);
			}
			ValidateParent(newParent);
			if (this is ScriptSchemaObjectBase scriptSchemaObjectBase && key != null && parentColl != null && (scriptSchemaObjectBase.Schema == null || scriptSchemaObjectBase.Schema.Length == 0))
			{
				scriptSchemaObjectBase.ChangeSchema(((SchemaCollectionBase)parentColl).GetDefaultSchema(), bCheckExisting: false);
			}
			UpdateObjectState();
		}
		catch (Exception ex)
		{
			FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SetParent, this, ex);
		}
	}

	internal virtual void UpdateObjectState()
	{
		if (base.State == SqlSmoState.Pending && !key.IsNull && parentColl != null)
		{
			SetState(SqlSmoState.Creating);
		}
	}

	private ScriptingPreferences GetScriptingPreferencesForAlter()
	{
		ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
		scriptingPreferences.SuppressDirtyCheck = false;
		scriptingPreferences.SetTargetServerInfo(this);
		scriptingPreferences.ScriptForAlter = true;
		scriptingPreferences.ForDirectExecution = true;
		scriptingPreferences.IncludeScripts.Associations = true;
		scriptingPreferences.OldOptions.Bindings = true;
		scriptingPreferences.Data.ChangeTracking = true;
		scriptingPreferences.IncludeScripts.Owner = true;
		return scriptingPreferences;
	}

	internal void AddDatabaseContext(StringCollection queries, ScriptingPreferences sp)
	{
		if (DatabaseEngineType.SqlAzureDatabase == sp.TargetDatabaseEngineType)
		{
			return;
		}
		string text = GetDBName();
		if (string.IsNullOrEmpty(text) && parentColl != null && ParentColl.ParentInstance is Server)
		{
			text = "master";
		}
		if (text.Length != 0)
		{
			string text2 = string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlBraket(text) });
			if (queries.Count == 0 || string.Compare(queries[0], text2, StringComparison.Ordinal) != 0)
			{
				queries.Add(text2);
			}
		}
	}

	protected void AddDatabaseContext(StringCollection queries)
	{
		AddDatabaseContext(queries, new ScriptingPreferences(this));
	}

	protected void AlterImplWorker()
	{
		CheckObjectState();
		AlterImplInit(out var alterQuery, out var sp);
		sp.IncludeScripts.DatabaseContext = true;
		ScriptAlterInternal(alterQuery, sp);
		AlterImplFinish(alterQuery, sp);
	}

	private void CheckNonAlterableProperties()
	{
		string[] nonAlterableProperties = GetNonAlterableProperties();
		foreach (string text in nonAlterableProperties)
		{
			if (Properties.Contains(text))
			{
				Property property = Properties.Get(text);
				if (property.Value != null && property.Dirty)
				{
					throw new SmoException(ExceptionTemplatesImpl.PropNotModifiable(text, GetType().Name));
				}
			}
		}
	}

	internal void AlterImplInit(out StringCollection alterQuery, out ScriptingPreferences sp)
	{
		if (!ExecutionManager.Recording && base.State != SqlSmoState.Existing)
		{
			throw new InvalidSmoOperationException("Alter", base.State);
		}
		if (!ExecutionManager.Recording)
		{
			CheckNonAlterableProperties();
		}
		InitializeKeepDirtyValues();
		sp = GetScriptingPreferencesForAlter();
		alterQuery = new StringCollection();
	}

	internal void AlterImplFinish(StringCollection alterQuery, ScriptingPreferences sp)
	{
		if (!IsDesignMode)
		{
			ExecuteNonQuery(alterQuery, executeForAlter: true);
		}
		PostAlter();
		if (!ExecutionManager.Recording)
		{
			CleanObject();
			PropagateStateAndCleanUp(alterQuery, sp, PropagateAction.Alter);
		}
	}

	protected void AlterImpl()
	{
		try
		{
			AlterImplWorker();
			GenerateAlterEvent();
		}
		catch (Exception ex)
		{
			FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Alter, this, ex);
		}
	}

	protected void GenerateAlterEvent(Urn urn, object innerObject)
	{
		if (!ExecutionManager.Recording && !SmoApplication.eventsSingleton.IsNullObjectAltered())
		{
			SmoApplication.eventsSingleton.CallObjectAltered(GetServerObject(), new ObjectAlteredEventArgs(urn, innerObject));
		}
	}

	protected void GenerateAlterEvent()
	{
		GenerateAlterEvent(Urn, this);
	}

	internal virtual void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
	}

	internal void ScriptAlterInternal(StringCollection alterQuery, ScriptingPreferences sp)
	{
		ScriptAlter(alterQuery, sp);
		PropagateScript(alterQuery, sp, PropagateAction.Alter);
	}

	public virtual void Refresh()
	{
		try
		{
			CheckObjectStateImpl(throwIfNotCreated: false);
			if (base.State == SqlSmoState.Creating && Initialize())
			{
				SetState(SqlSmoState.Existing);
			}
			else if (base.State == SqlSmoState.Existing)
			{
				IDataReader dataReader = null;
				try
				{
					Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != key, "null == key");
					StringCollection fieldNames = key.GetFieldNames();
					if (fieldNames.Count > 0)
					{
						string[] array = new string[fieldNames.Count];
						fieldNames.CopyTo(array, 0);
						dataReader = GetInitDataReader(array, null);
						if (dataReader == null)
						{
							SetState(SqlSmoState.Dropped);
						}
					}
				}
				finally
				{
					if (dataReader != null)
					{
						dataReader.Close();
						dataReader = null;
					}
				}
			}
			properties = null;
			propertyBagState = PropertyBagState.Empty;
			initializedForScripting = false;
			userPermissions = null;
			m_ExtendedProperties = null;
		}
		catch (Exception ex)
		{
			FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Refresh, this, ex);
		}
	}

	internal void ReCompile(string name, string schema)
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlBraket(GetDBName()) }));
			if (string.IsNullOrEmpty(schema))
			{
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_recompile @objname=N'[{0}]'", new object[1] { SqlString(name) }));
			}
			else
			{
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_recompile @objname=N'[{0}].[{1}]'", new object[2]
				{
					SqlString(schema),
					SqlString(name)
				}));
			}
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ReCompileReferences, this, ex);
		}
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		if (IsDesignMode && base.State == SqlSmoState.Existing)
		{
			Property property = Properties.Get(propname);
			if (null != property)
			{
				object obj = null;
				SfcMetadataDiscovery sfcMetadataDiscovery = new SfcMetadataDiscovery(GetType());
				SfcMetadataRelation sfcMetadataRelation = sfcMetadataDiscovery.FindProperty(propname);
				if (sfcMetadataRelation != null)
				{
					obj = sfcMetadataRelation.PropertyDefaultValue;
					if (obj != null && obj.ToString() == "string.empty")
					{
						obj = string.Empty;
					}
				}
				if (obj == null)
				{
					switch (property.Type.FullName)
					{
					case "System.Boolean":
						obj = false;
						break;
					case "System.DateTime":
						obj = DateTime.MinValue;
						break;
					case "System.DateTimeOffset":
						obj = DateTimeOffset.MinValue;
						break;
					case "System.TimeSpan":
						obj = TimeSpan.MinValue;
						break;
					case "System.Int32":
						obj = 0;
						break;
					case "System.Int64":
						obj = 0L;
						break;
					case "System.UInt32":
						obj = 0u;
						break;
					case "System.UInt64":
						obj = 0uL;
						break;
					case "System.Single":
						obj = 0f;
						break;
					case "System.Double":
						obj = 0.0;
						break;
					}
				}
				if (obj != null)
				{
					return obj;
				}
			}
			Trace("DesignMode Missing " + (Properties.Get(propname).Expensive ? "expensive" : "regular") + " property " + propname + " for type " + GetType().Name);
		}
		return base.GetPropertyDefaultValue(propname);
	}

	internal override object OnPropertyMissing(string propname, bool useDefaultValue)
	{
		if (useDefaultValue)
		{
			switch (base.State)
			{
			case SqlSmoState.Pending:
			case SqlSmoState.Creating:
				return GetPropertyDefaultValue(propname);
			case SqlSmoState.Existing:
				if (IsDesignMode)
				{
					return GetPropertyDefaultValue(propname);
				}
				break;
			}
		}
		else
		{
			switch (base.State)
			{
			case SqlSmoState.Pending:
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(condition: false);
				break;
			case SqlSmoState.Creating:
				throw new PropertyNotSetException(propname);
			}
		}
		Trace(string.Concat("Missing ", Properties.Get(propname).Expensive ? "expensive" : "regular", " property ", propname, " property bag state ", propertyBagState, " for type ", GetType().Name));
		PropertyMissingEventArgs e = new PropertyMissingEventArgs(propname, GetType().Name);
		Property property = Properties.Get(propname);
		if (property.Expensive)
		{
			string[] fields = new string[1] { propname };
			if (!IsDesignMode)
			{
				SqlSmoObject.PropertyMissing(this, e);
				ImplInitialize(fields, null);
			}
			return property.Value;
		}
		switch (propertyBagState)
		{
		case PropertyBagState.Empty:
		{
			bool flag = !GetServerObject().IsInitField(GetType(), propname);
			if (flag)
			{
				SqlSmoObject.PropertyMissing(this, e);
			}
			Initialize(flag);
			return property.Value;
		}
		case PropertyBagState.Lazy:
			SqlSmoObject.PropertyMissing(this, e);
			Initialize(allProperties: true);
			return property.Value;
		case PropertyBagState.Full:
			throw new InternalSmoErrorException(ExceptionTemplatesImpl.FullPropertyBag(propname));
		default:
			return null;
		}
	}

	public bool Initialize()
	{
		return Initialize(allProperties: false);
	}

	public bool Initialize(bool allProperties)
	{
		CheckObjectState();
		if (IsDesignMode || (allProperties && IsObjectInitialized()))
		{
			return false;
		}
		string[] fields = null;
		OrderBy[] array = null;
		if (!allProperties)
		{
			fields = GetServerObject().GetDefaultInitFieldsInternal(GetType(), DatabaseEngineEdition);
		}
		bool flag = ImplInitialize(fields, array);
		if (allProperties)
		{
			if (flag)
			{
				propertyBagState = PropertyBagState.Full;
			}
		}
		else if (flag && propertyBagState != PropertyBagState.Full)
		{
			propertyBagState = PropertyBagState.Lazy;
		}
		return flag;
	}

	protected internal bool IsObjectInitialized()
	{
		if ((base.State != SqlSmoState.Existing && base.State != SqlSmoState.ToBeDropped) || propertyBagState == PropertyBagState.Full)
		{
			return true;
		}
		return false;
	}

	internal bool InitializeKeepDirtyValues()
	{
		CheckObjectState();
		if (IsObjectInitialized() || InitializedForScripting || IsDesignMode)
		{
			return false;
		}
		if (Properties.Count <= 0 && parentColl == null)
		{
			return true;
		}
		try
		{
			IDataReader dataReader = null;
			try
			{
				dataReader = GetInitDataReader(null, null);
				AddObjectPropsFromDataReader(dataReader, skipIfDirty: true);
			}
			finally
			{
				dataReader?.Close();
			}
		}
		catch (FailedOperationException)
		{
			return false;
		}
		propertyBagState = PropertyBagState.Full;
		return true;
	}

	private IDataReader GetInitDataReader(string[] fields, OrderBy[] orderby)
	{
		Urn urn = Urn;
		Request request = new Request();
		request.Urn = urn;
		if (fields == null)
		{
			request.Fields = GetRejectFields();
			request.RequestFieldsTypes = RequestFieldsTypes.Reject;
		}
		else
		{
			request.Fields = fields;
		}
		request.OrderByList = orderby;
		IDataReader enumeratorDataReader = ExecutionManager.GetEnumeratorDataReader(request);
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != enumeratorDataReader, "reader == null");
		if (!enumeratorDataReader.Read())
		{
			enumeratorDataReader.Close();
			Trace("Failed to Initialize urn " + urn);
			return null;
		}
		return enumeratorDataReader;
	}

	internal virtual string[] GetRejectFields()
	{
		return new string[1] { "Urn" };
	}

	protected virtual bool ImplInitialize(string[] fields, OrderBy[] orderby)
	{
		if ((Properties.Count <= 0 && parentColl == null) || (fields != null && fields.Length == 0))
		{
			return true;
		}
		IDataReader dataReader = null;
		try
		{
			dataReader = GetInitDataReader(fields, orderby);
			if (dataReader == null)
			{
				return false;
			}
			AddObjectPropsFromDataReader(dataReader, skipIfDirty: true);
		}
		finally
		{
			dataReader?.Close();
		}
		return true;
	}

	void ISqlSmoObjectInitialize.InitializeFromDataReader(IDataReader reader)
	{
		AddObjectPropsFromDataReader(reader, skipIfDirty: false);
	}

	internal void AddObjectPropsFromDataReader(IDataReader reader, bool skipIfDirty)
	{
		AddObjectPropsFromDataReader(reader, skipIfDirty, 0, -1);
	}

	internal void AddObjectPropsFromDataReader(IDataReader reader, bool skipIfDirty, int startColIdx, int endColIdx)
	{
		DataTable schemaTable = reader.GetSchemaTable();
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != schemaTable, "reader.GetSchemaTable()==null");
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(schemaTable.Rows.Count == reader.FieldCount, "schemaTable.Rows.Count != reader.FieldCount");
		int num = schemaTable.Columns.IndexOf("ColumnName");
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(num > -1, "IndexOf(\"ColumnName\")==-1");
		for (int i = startColIdx; i < ((endColIdx >= 0) ? endColIdx : schemaTable.Rows.Count); i++)
		{
			string text = schemaTable.Rows[i][num] as string;
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != text, "schemaTable.Rows[i][\"ColumnName\"]==null");
			object value = reader.GetValue(i);
			int num2 = Properties.PropertiesMetadata.PropertyNameToIDLookup(text);
			if (num2 < 0)
			{
				continue;
			}
			Property property = Properties.Get(num2);
			property.SetRetrieved(retrieved: true);
			if (skipIfDirty && property.Dirty)
			{
				continue;
			}
			if (property.Enumeration)
			{
				if (DBNull.Value.Equals(value))
				{
					property.SetValue(null);
				}
				else if (property.Type.Equals(typeof(Guid)))
				{
					property.SetValue(new Guid((string)value));
				}
				else
				{
					property.SetValue(Enum.ToObject(property.Type, Convert.ToInt32(value, SmoApplication.DefaultCulture)));
				}
			}
			else if (DBNull.Value.Equals(value))
			{
				if (property.Type.Equals(typeof(DateTime)))
				{
					property.SetValue(DateTime.MinValue);
				}
				else if (property.Type.Equals(typeof(DateTimeOffset)))
				{
					property.SetValue(DateTimeOffset.MinValue);
				}
				else if (property.Type.Equals(typeof(TimeSpan)))
				{
					property.SetValue(TimeSpan.MinValue);
				}
				else
				{
					property.SetValue(null);
				}
			}
			else
			{
				property.SetValue(value);
			}
		}
	}

	internal virtual string[] GetNonAlterableProperties()
	{
		return new string[0];
	}

	protected internal object GetPropValue(string propName)
	{
		if (base.State == SqlSmoState.Creating && Properties.Get(propName).Value == null)
		{
			throw new PropertyNotSetException(propName);
		}
		return Properties[propName].Value;
	}

	protected internal object GetPropValueOptional(string propName)
	{
		return GetPropertyOptional(propName).Value;
	}

	internal T? GetPropValueOptional<T>(string propName) where T : struct
	{
		object propValueOptional = GetPropValueOptional(propName);
		if (propValueOptional == null)
		{
			return null;
		}
		return (T)propValueOptional;
	}

	public T GetPropValueOptional<T>(string propName, T defaultValue)
	{
		object propValueOptional = GetPropValueOptional(propName);
		if (propValueOptional != null)
		{
			return (T)propValueOptional;
		}
		return defaultValue;
	}

	protected internal object GetPropValueOptionalAllowNull(string propName)
	{
		if (base.State == SqlSmoState.Creating)
		{
			return Properties.Get(propName).Value;
		}
		return Properties.GetPropertyObjectAllowNull(propName).Value;
	}

	internal T GetPropValueIfSupported<T>(string propertyName, T defaultValue, ScriptingPreferences sp = null)
	{
		if (IsSupportedProperty(propertyName))
		{
			T propValueOptional = GetPropValueOptional(propertyName, defaultValue);
			if (sp != null && !IsSupportedProperty(propertyName, sp))
			{
				return defaultValue;
			}
			return propValueOptional;
		}
		return defaultValue;
	}

	internal T GetPropValueIfSupportedWithThrowOnTarget<T>(string propertyName, T defaultValue, ScriptingPreferences sp = null)
	{
		if (IsSupportedProperty(propertyName))
		{
			T propValueOptional = GetPropValueOptional(propertyName, defaultValue);
			if (sp != null && !propValueOptional.Equals(defaultValue))
			{
				ThrowIfPropertyNotSupported(propertyName, sp);
			}
			return propValueOptional;
		}
		return defaultValue;
	}

	internal Property GetPropertyOptional(string propName)
	{
		if (base.State == SqlSmoState.Creating)
		{
			return Properties.Get(propName);
		}
		if (IsDesignMode)
		{
			return Properties.GetPropertyObject(propName, doNotLoadPropertyValues: true);
		}
		return Properties[propName];
	}

	protected object GetRealValue(Property prop, object oldValue)
	{
		if (oldValue != null)
		{
			return oldValue;
		}
		Request req = new Request(Urn, new string[1] { prop.Name });
		DataTable enumeratorData = ExecutionManager.GetEnumeratorData(req);
		if (enumeratorData.Rows.Count == 0 || enumeratorData.Columns.Count == 0)
		{
			return prop.Value;
		}
		return enumeratorData.Rows[0][0];
	}

	protected virtual string GetServerName()
	{
		return GetServerObject().Name;
	}

	private Server TryGetServerObject()
	{
		if (m_server == null)
		{
			m_server = this as Server;
			if (m_server == null)
			{
				if (ParentColl != null && ParentColl.ParentInstance != null)
				{
					m_server = ParentColl.ParentInstance.TryGetServerObject();
				}
				else
				{
					if (ParentColl != null)
					{
						return null;
					}
					m_server = GetParentObject(throwIfParentIsCreating: false).TryGetServerObject();
				}
			}
		}
		return m_server;
	}

	protected internal Server GetServerObject()
	{
		Server server = TryGetServerObject();
		if (server != null)
		{
			return server;
		}
		throw new InternalSmoErrorException(ExceptionTemplatesImpl.ObjectNotUnderServer(GetType().ToString()));
	}

	internal void SetServerObject(Server server)
	{
		m_server = server;
	}

	protected internal virtual string GetDBName()
	{
		SqlSmoObject sqlSmoObject = this;
		while (sqlSmoObject.ParentColl != null && !(sqlSmoObject is Database))
		{
			sqlSmoObject = sqlSmoObject.ParentColl.ParentInstance;
		}
		if (sqlSmoObject.ParentColl == null)
		{
			if (sqlSmoObject is ServiceBroker)
			{
				return ((ServiceBroker)sqlSmoObject).Parent.GetDBName();
			}
			if (sqlSmoObject is FullTextIndex)
			{
				return ((FullTextIndex)sqlSmoObject).Parent.GetDBName();
			}
			return string.Empty;
		}
		return ((Database)sqlSmoObject).Name;
	}

	protected internal Database GetContextDB()
	{
		SqlSmoObject sqlSmoObject = this;
		while (sqlSmoObject.ParentColl != null && !(sqlSmoObject is Database))
		{
			sqlSmoObject = sqlSmoObject.ParentColl.ParentInstance;
		}
		if (sqlSmoObject.ParentColl != null)
		{
			return (Database)sqlSmoObject;
		}
		return null;
	}

	protected internal static void Trace(string traceText)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, traceText);
	}

	internal static void FilterException(Exception e)
	{
		if (e is OutOfMemoryException)
		{
			throw e;
		}
	}

	protected void CreateImpl()
	{
		try
		{
			CreateImplInit(out var createQuery, out var sp);
			ScriptCreateInternal(createQuery, sp);
			CreateImplFinish(createQuery, sp);
		}
		catch (Exception ex)
		{
			FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
	}

	protected void CreateOrAlterImpl()
	{
		try
		{
			CreateOrAlterImplInit(out var createOrAlterQuery, out var sp);
			ScriptCreateOrAlterInternal(createOrAlterQuery, sp);
			CreateOrAlterImplFinish(createOrAlterQuery, sp);
		}
		catch (Exception ex)
		{
			FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CreateOrAlter, this, ex);
		}
	}

	internal virtual void ScriptCreateOrAlter(StringCollection query, ScriptingPreferences sp)
	{
		throw new NotSupportedException(ExceptionTemplatesImpl.CreateOrAlterNotSupported(GetType().Name));
	}

	internal virtual void ScriptCreateOrAlterInternal(StringCollection query, ScriptingPreferences sp)
	{
		ScriptCreateOrAlterInternal(query, sp, skipPropagateScript: false);
	}

	internal virtual void ScriptCreateOrAlterInternal(StringCollection query, ScriptingPreferences sp, bool skipPropagateScript)
	{
		ScriptCreateOrAlter(query, sp);
		if (base.State != SqlSmoState.Existing && sp.IncludeScripts.Permissions)
		{
			AddScriptPermission(query, sp);
		}
		if (!skipPropagateScript)
		{
			PropagateScript(query, sp, PropagateAction.CreateOrAlter);
		}
	}

	internal ScriptingPreferences GetScriptingPreferencesForCreate()
	{
		ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
		scriptingPreferences.SuppressDirtyCheck = false;
		scriptingPreferences.IncludeScripts.Associations = true;
		scriptingPreferences.OldOptions.Bindings = true;
		scriptingPreferences.ScriptForCreateDrop = true;
		scriptingPreferences.ForDirectExecution = true;
		scriptingPreferences.IncludeScripts.ExistenceCheck = false;
		scriptingPreferences.Data.ChangeTracking = true;
		scriptingPreferences.IncludeScripts.Owner = true;
		scriptingPreferences.SetTargetServerInfo(this);
		return scriptingPreferences;
	}

	private SqlSmoObject GetParentObject(bool throwIfParentIsCreating = true, bool throwIfParentNotExist = true)
	{
		AbstractCollectionBase abstractCollectionBase = parentColl;
		SqlSmoObject sqlSmoObject = null;
		if (abstractCollectionBase != null)
		{
			sqlSmoObject = abstractCollectionBase.ParentInstance;
		}
		else if (singletonParent != null)
		{
			sqlSmoObject = singletonParent;
		}
		if (sqlSmoObject != null)
		{
			if (sqlSmoObject.State == SqlSmoState.Creating && throwIfParentIsCreating)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.ParentMustExist(GetType().Name, FullQualifiedName));
			}
		}
		else if (throwIfParentNotExist)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.NeedToSetParent);
		}
		return sqlSmoObject;
	}

	private void ExecuteNonQuery(StringCollection queries, bool executeForAlter)
	{
		ExecuteNonQuery(queries, includeDbContext: true, executeForAlter);
	}

	protected void ExecuteNonQuery(StringCollection queries, bool includeDbContext, bool executeForAlter)
	{
		if (queries.Count <= 0)
		{
			return;
		}
		string dBName = GetDBName();
		if (dBName != null && DatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
		{
			string value = string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlBraket(dBName) });
			if (includeDbContext && !queries[0].StartsWith(value) && !queries[0].StartsWith(Scripts.USEMASTER))
			{
				if (dBName.Length > 0 && !typeof(Database).Equals(GetType()))
				{
					queries.Insert(0, value);
				}
				else
				{
					queries.Insert(0, Scripts.USEMASTER);
				}
			}
		}
		ExecutionManager executionManager = ExecutionManager;
		if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase && typeof(Database).Equals(GetType()) && !executeForAlter)
		{
			executionManager = GetServerObject().ExecutionManager;
		}
		if (executeForScalar)
		{
			scalarResult = executionManager.ExecuteScalar(queries);
		}
		else
		{
			executionManager.ExecuteNonQuery(queries);
		}
	}

	internal void CreateImplInit(out StringCollection createQuery, out ScriptingPreferences sp)
	{
		scalarResult = null;
		if (base.State == SqlSmoState.Pending)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.InvalidOperationInDisconnectedMode);
		}
		if (!ExecutionManager.Recording)
		{
			CheckObjectState();
			if (base.State != SqlSmoState.Creating)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.ObjectAlreadyExists(Urn.Type, key.ToString()));
			}
			GetParentObject();
		}
		createQuery = new StringCollection();
		sp = GetScriptingPreferencesForCreate();
	}

	internal void CreateImplFinish(StringCollection createQuery, ScriptingPreferences sp)
	{
		if (createQuery.Count <= 0)
		{
			Urn urn = Urn;
			throw new FailedOperationException(ExceptionTemplatesImpl.NoSqlGen(urn.ToString()));
		}
		if (!IsDesignMode)
		{
			ExecuteNonQuery(createQuery, !(this is Database), executeForAlter: false);
		}
		PostCreate();
		if (!ExecutionManager.Recording)
		{
			SetState(SqlSmoState.Existing);
			if (objectInSpace)
			{
				if (ParentColl != null)
				{
					ParentColl.AddExisting(this);
				}
				objectInSpace = false;
			}
			CleanObject();
			PropagateStateAndCleanUp(createQuery, sp, PropagateAction.Create);
			PostPropagate();
		}
		if (!ExecutionManager.Recording && !SmoApplication.eventsSingleton.IsNullObjectCreated())
		{
			SmoApplication.eventsSingleton.CallObjectCreated(GetServerObject(), new ObjectCreatedEventArgs(Urn, this));
		}
	}

	internal void CreateOrAlterImplInit(out StringCollection createOrAlterQuery, out ScriptingPreferences sp)
	{
		CheckObjectState();
		if (base.State != SqlSmoState.Existing)
		{
			CreateImplInit(out createOrAlterQuery, out sp);
			return;
		}
		AlterImplInit(out createOrAlterQuery, out sp);
		sp.IncludeScripts.DatabaseContext = true;
	}

	internal void CreateOrAlterImplFinish(StringCollection createOrAlterQuery, ScriptingPreferences sp)
	{
		CheckObjectState();
		if (base.State != SqlSmoState.Existing)
		{
			CreateImplFinish(createOrAlterQuery, sp);
			return;
		}
		AlterImplFinish(createOrAlterQuery, sp);
		GenerateAlterEvent();
	}

	protected virtual void PostCreate()
	{
	}

	protected virtual void PostAlter()
	{
	}

	protected virtual void PostDrop()
	{
	}

	internal virtual void PostPropagate()
	{
	}

	internal virtual void ScriptDdl(StringCollection query, ScriptingPreferences sp)
	{
		throw new InvalidOperationException();
	}

	internal void ScriptDdlInternal(StringCollection query, ScriptingPreferences sp)
	{
		ScriptDdl(query, sp);
	}

	internal virtual void ScriptAssociations(StringCollection query, ScriptingPreferences sp)
	{
		throw new InvalidOperationException();
	}

	internal void ScriptAssociationsInternal(StringCollection query, ScriptingPreferences sp)
	{
		ScriptAssociations(query, sp);
	}

	internal virtual void ScriptCreate(StringCollection query, ScriptingPreferences sp)
	{
		throw new InvalidOperationException();
	}

	internal virtual void ScriptCreateInternal(StringCollection query, ScriptingPreferences sp)
	{
		ScriptCreateInternal(query, sp, skipPropagateScript: false);
	}

	internal virtual void ScriptCreateInternal(StringCollection query, ScriptingPreferences sp, bool skipPropagateScript)
	{
		ScriptCreate(query, sp);
		if (sp.IncludeScripts.Permissions)
		{
			AddScriptPermission(query, sp);
		}
		if (!skipPropagateScript)
		{
			PropagateScript(query, sp, PropagateAction.Create);
		}
	}

	internal virtual void AddScriptPermission(StringCollection query, ScriptingPreferences sp)
	{
		if (this is IObjectPermission)
		{
			AddScriptPermissions(query, PermissionWorker.PermissionEnumKind.Object, sp);
		}
	}

	private PermissionInfo[] GetPermissionsFromCache(PermissionWorker.PermissionEnumKind kind)
	{
		UserPermissionCollection userPermissionCollection = GetUserPermissions();
		PermissionInfo[] permissionInfoArray = PermissionWorker.GetPermissionInfoArray(kind, userPermissionCollection.Count);
		PermissionInfo.ObjIdent objIdent = null;
		string text = null;
		int num = 0;
		foreach (UserPermission item in userPermissionCollection)
		{
			if (objIdent == null)
			{
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null == text, "null == columnName");
				objIdent = new PermissionInfo.ObjIdent(item.ObjectClass);
				if (kind == PermissionWorker.PermissionEnumKind.Column)
				{
					Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(this is Column, "this is Column");
					objIdent.SetData(((Column)this).Parent);
					text = ((Column)this).Name;
				}
				else
				{
					objIdent.SetData(this);
					text = null;
				}
			}
			PermissionInfo permissionInfo = PermissionWorker.GetPermissionInfo(kind);
			permissionInfo.SetPermissionInfoData(item.Grantee, item.GranteeType, item.Grantor, item.GrantorType, item.PermissionState, PermissionWorker.GetPermissionSetBase(kind, (int)item.Code), text, objIdent);
			permissionInfoArray[num++] = permissionInfo;
		}
		return permissionInfoArray;
	}

	internal UserPermissionCollection GetUserPermissions()
	{
		CheckObjectState();
		if (userPermissions == null)
		{
			userPermissions = new UserPermissionCollection(this);
		}
		return userPermissions;
	}

	internal void ClearUserPemissions()
	{
		userPermissions = null;
	}

	internal void AddScriptPermissions(StringCollection sc, PermissionWorker.PermissionEnumKind kind, ScriptingPreferences sp)
	{
		PermissionInfo[] array = null;
		try
		{
			array = GetPermissionsFromCache(kind);
		}
		catch (EnumeratorException ex)
		{
			if (ex.InnerException is InvalidVersionEnumeratorException)
			{
				return;
			}
			throw;
		}
		PermissionInfo[] array2 = array;
		foreach (PermissionInfo permissionInfo in array2)
		{
			if ((sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90 || ((permissionInfo.ObjectClass == ObjectClass.ObjectOrColumn || permissionInfo.ObjectClass == ObjectClass.Database) && permissionInfo.PermissionTypeInternal.IsValidPermissionForVersion(sp.TargetServerVersionInternal))) && !(permissionInfo.Grantee == "dbo") && !(permissionInfo.Grantee == "information_schema") && !(permissionInfo.Grantee == "sys") && (sp.TargetServerVersionInternal != SqlServerVersionInternal.Version70 || !(this is View) || !((ObjectPermissionSet)permissionInfo.PermissionTypeInternal).References))
			{
				sc.Add(ScriptPermissionInfo(permissionInfo, sp));
			}
		}
	}

	internal virtual string ScriptPermissionInfo(PermissionInfo pi, ScriptingPreferences sp)
	{
		return PermissionWorker.ScriptPermissionInfo(GetPermTargetObject(), pi, sp);
	}

	internal virtual SqlSmoObject GetPermTargetObject()
	{
		return this;
	}

	protected void DropImpl(bool isDropIfExists = false)
	{
		if (isDropIfExists)
		{
			ThrowIfBelowVersion130();
			if (base.State == SqlSmoState.Dropped || (base.State == SqlSmoState.Creating && !ExecutionManager.Recording) || (base.State == SqlSmoState.Pending && !IsDesignMode))
			{
				return;
			}
		}
		try
		{
			Urn urn = null;
			DropImplWorker(ref urn, isDropIfExists);
			if (!ExecutionManager.Recording && !SmoApplication.eventsSingleton.IsNullObjectDropped())
			{
				SmoApplication.eventsSingleton.CallObjectDropped(GetServerObject(), new ObjectDroppedEventArgs(urn));
			}
		}
		catch (Exception ex)
		{
			FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Drop, this, ex);
		}
	}

	protected void DropImplWorker(ref Urn urn, bool isDropIfExists = false)
	{
		if (!ExecutionManager.Recording)
		{
			CheckObjectState(throwIfNotCreated: true);
		}
		urn = Urn;
		StringCollection stringCollection = new StringCollection();
		ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
		scriptingPreferences.IncludeScripts.Header = true;
		scriptingPreferences.Behavior = ScriptBehavior.Drop;
		scriptingPreferences.ForDirectExecution = true;
		scriptingPreferences.ScriptForCreateDrop = true;
		scriptingPreferences.SetTargetServerInfo(this);
		scriptingPreferences.IncludeScripts.ExistenceCheck = isDropIfExists;
		ScriptDrop(stringCollection, scriptingPreferences);
		if (!IsDesignMode && stringCollection.Count > 0)
		{
			ExecuteNonQuery(stringCollection, executeForAlter: false);
		}
		if (parentColl != null)
		{
			parentColl.RemoveObject(key);
		}
		if (!ExecutionManager.Recording)
		{
			MarkDropped();
		}
		PropagateStateAndCleanUp(stringCollection, scriptingPreferences, PropagateAction.Drop);
		PostDrop();
	}

	internal virtual void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		throw new InvalidOperationException();
	}

	internal void ScriptDropInternal(StringCollection dropQuery, ScriptingPreferences sp)
	{
		ScriptDrop(dropQuery, sp);
	}

	internal void ScriptIncludeHeaders(StringBuilder sb, ScriptingPreferences sp, string objectType)
	{
		if (sp.IncludeScripts.Header)
		{
			sb.Append(ExceptionTemplates.IncludeHeader(objectType, MakeSqlBraketNoEscape(InternalName), DateTime.Now.ToString(GetDbCulture())));
			sb.Append(sp.NewLine);
		}
	}

	public IComparer<string> GetStringComparer()
	{
		return StringComparer;
	}

	internal void InitializeStringComparer()
	{
		if (m_comparer == null)
		{
			if (this is Server || this is Settings)
			{
				m_comparer = GetDbComparer(inServer: true);
			}
			else if (this is Database)
			{
				m_comparer = GetDbComparer(inServer: false);
			}
			else if (ParentColl == null || ParentColl.ParentInstance == null)
			{
				m_comparer = GetDbComparer(inServer: true);
			}
			else
			{
				m_comparer = ParentColl.ParentInstance.StringComparer;
			}
		}
	}

	private bool TryGetProperty<T>(string propertyName, ref T value)
	{
		Property property = null;
		if (IsSupportedProperty(propertyName) && Properties.Contains(propertyName))
		{
			property = Properties.Get(propertyName);
		}
		if (null != property && property.Value != null && property.Retrieved)
		{
			value = (T)property.Value;
			return true;
		}
		return false;
	}

	private string GetCollationRelatedProperties(string dbName, out ContainmentType containmentType, out CatalogCollationType catalogCollation)
	{
		List<string> list = new List<string>();
		ContainmentType value = ContainmentType.None;
		CatalogCollationType value2 = CatalogCollationType.DatabaseDefault;
		string value3 = null;
		if (!TryGetProperty("ContainmentType", ref value) && dbName != "master" && dbName != "msdb" && IsSupportedProperty("ContainmentType"))
		{
			list.Add("ContainmentType");
		}
		if (!TryGetProperty("CatalogCollation", ref value2) && IsSupportedProperty("CatalogCollation"))
		{
			list.Add("CatalogCollation");
		}
		if (!TryGetProperty("Collation", ref value3))
		{
			if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase && string.Compare(dbName, "master", StringComparison.OrdinalIgnoreCase) == 0)
			{
				value3 = "SQL_Latin1_General_CP1_CI_AS";
			}
			else
			{
				list.Add("Collation");
			}
		}
		if (list.Any())
		{
			Request req = new Request(string.Format(SmoApplication.DefaultCulture, "Server/Database[@Name='{0}']", new object[1] { Urn.EscapeString(dbName) }), list.ToArray());
			DataTable enumeratorData = GetServerObject().ExecutionManager.GetEnumeratorData(req);
			if (enumeratorData.Rows.Count > 0)
			{
				if (enumeratorData.Columns.Contains("Collation") && enumeratorData.Rows[0]["Collation"] != DBNull.Value)
				{
					value3 = (string)enumeratorData.Rows[0]["Collation"];
				}
				if (enumeratorData.Columns.Contains("CatalogCollation") && enumeratorData.Rows[0]["CatalogCollation"] != DBNull.Value)
				{
					value2 = (CatalogCollationType)enumeratorData.Rows[0]["CatalogCollation"];
				}
				if (enumeratorData.Columns.Contains("ContainmentType") && enumeratorData.Rows[0]["ContainmentType"] != DBNull.Value)
				{
					value = (ContainmentType)enumeratorData.Rows[0]["ContainmentType"];
				}
			}
		}
		catalogCollation = value2;
		containmentType = value;
		return value3 ?? string.Empty;
	}

	internal StringComparer GetDbComparer(bool inServer)
	{
		if (ServerVersion.Major <= 7)
		{
			return new StringComparer(string.Empty, 1033);
		}
		string text = (inServer ? "master" : ((SimpleObjectKey)key).Name);
		ContainmentType containmentType;
		CatalogCollationType catalogCollation;
		string text2 = GetCollationRelatedProperties(text, out containmentType, out catalogCollation);
		if (containmentType != ContainmentType.None)
		{
			text2 = "Latin1_General_100_CI_AS_KS_WS";
		}
		else if (Enum.IsDefined(typeof(CatalogCollationType), catalogCollation) && catalogCollation != CatalogCollationType.DatabaseDefault)
		{
			TypeConverter typeConverter = SmoManagementUtil.GetTypeConverter(typeof(CatalogCollationType));
			text2 = typeConverter.ConvertToInvariantString(catalogCollation);
		}
		else if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			text2 = "SQL_Latin1_General_CP1_CI_AS";
		}
		if (text2 == string.Empty && !IsDesignMode)
		{
			text2 = GetServerObject().Collation;
			Trace($"Got null/empty DB Collation for DB {text}, falling back to using server collation {text2}");
		}
		return GetComparerFromCollation(text2);
	}

	internal StringComparer GetComparerFromCollation(string collationName)
	{
		StringComparer stringComparer = null;
		try
		{
			return new StringComparer(collationName, GetServerObject().GetLCIDCollation(collationName));
		}
		catch (DisconnectedConnectionException)
		{
			return new StringComparer(collationName, 1033);
		}
	}

	internal CultureInfo GetDbCulture()
	{
		return Thread.CurrentThread.CurrentCulture;
	}

	public void SetAccessToken(IRenewableToken token)
	{
		accessToken = token;
		if (ExecutionManager != null)
		{
			ExecutionManager.ConnectionContext.AccessToken = token;
		}
	}

	public static bool IsSupportedOnSqlAzure(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		object[] customAttributes = type.GetCustomAttributes(typeof(SfcElementAttribute), inherit: true);
		foreach (object obj in customAttributes)
		{
			if (obj is SfcElementAttribute sfcElementAttribute)
			{
				return sfcElementAttribute.SqlAzureDatabase;
			}
		}
		return false;
	}

	internal static string EscapeString(string s, char cEsc)
	{
		if (s == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in s)
		{
			stringBuilder.Append(c);
			if (cEsc == c)
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static string SqlString(string s)
	{
		return EscapeString(s, '\'');
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static string MakeSqlString(string s)
	{
		return string.Format(SmoApplication.DefaultCulture, "N'{0}'", new object[1] { EscapeString(s, '\'') });
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static string QuoteString(string name, char cStart = '\'', char cEnd = '\'')
	{
		return string.Format(SmoApplication.DefaultCulture, "{1}{0}{2}", new object[3]
		{
			EscapeString(name, cEnd),
			cStart,
			cEnd
		});
	}

	internal static string MakeSqlStringForInsert(string s)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(s != null);
		return MakeSqlString(s).Replace("\\" + System.Environment.NewLine, "\\' + N'" + System.Environment.NewLine);
	}

	internal static string SqlBraket(string s)
	{
		return EscapeString(s, ']');
	}

	internal static string MakeSqlBraket(string s)
	{
		return string.Format(SmoApplication.DefaultCulture, "[{0}]", new object[1] { EscapeString(s, ']') });
	}

	internal static string SqlStringBraket(string s)
	{
		return SqlBraket(SqlString(s));
	}

	internal static string MakeSqlBraketNoEscape(string s)
	{
		return string.Format(SmoApplication.DefaultCulture, "[{0}]", new object[1] { s });
	}

	protected virtual bool IsObjectDirty()
	{
		if (!Properties.Dirty)
		{
			return isTouched;
		}
		return true;
	}

	protected virtual void CleanObject()
	{
		if (IsDesignMode)
		{
			Properties.SetAllDirtyAsRetrieved(val: true);
		}
		Properties.SetAllDirty(val: false);
		isTouched = false;
	}

	public void Touch()
	{
		isTouched = true;
		TouchImpl();
	}

	protected virtual void TouchImpl()
	{
	}

	protected static bool IsCollectionDirty(ICollection col)
	{
		if (col is AbstractCollectionBase { IsDirty: not false })
		{
			return true;
		}
		foreach (SqlSmoObject item in col)
		{
			if (item.State == SqlSmoState.Creating)
			{
				return true;
			}
			if (item.State == SqlSmoState.Existing)
			{
				if (item.IsObjectDirty())
				{
					return true;
				}
			}
			else if (item.State == SqlSmoState.ToBeDropped)
			{
				return true;
			}
		}
		return false;
	}

	internal virtual PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		return null;
	}

	internal virtual PropagateInfo[] GetPropagateInfoForDiscovery(PropagateAction action)
	{
		return GetPropagateInfo(action);
	}

	internal void PropagateScript(StringCollection query, ScriptingPreferences sp, PropagateAction action)
	{
		if (PropagateAction.Drop == action)
		{
			return;
		}
		PropagateInfo[] propagateInfo = GetPropagateInfo(action);
		if (propagateInfo == null)
		{
			return;
		}
		PropagateInfo[] array = propagateInfo;
		foreach (PropagateInfo propagateInfo2 in array)
		{
			ICollection collection = null;
			if (propagateInfo2.col != null)
			{
				collection = propagateInfo2.col;
			}
			else
			{
				if (propagateInfo2.obj == null)
				{
					continue;
				}
				collection = new SqlSmoObject[1] { propagateInfo2.obj };
			}
			if (!propagateInfo2.bWithScript && !propagateInfo2.bPropagateScriptToChildLevel)
			{
				continue;
			}
			foreach (SqlSmoObject item in collection)
			{
				if (sp.TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase && !item.IsCloudSupported)
				{
					continue;
				}
				if (propagateInfo2.bWithScript)
				{
					if (action == PropagateAction.Create && !sp.ScriptForCreateDrop && item is ICreatable)
					{
						item.ScriptCreateInternal(query, sp);
					}
					else if (action == PropagateAction.Create || PropagateAction.Alter == action || PropagateAction.CreateOrAlter == action)
					{
						if (item.State == SqlSmoState.Existing)
						{
							bool scriptForAlter = sp.ScriptForAlter;
							sp.ScriptForAlter = true;
							item.ScriptAlterInternal(query, sp);
							sp.ScriptForAlter = scriptForAlter;
						}
						else if (item.State == SqlSmoState.Creating)
						{
							bool scriptForAlter2 = sp.ScriptForAlter;
							bool scriptForCreateDrop = sp.ScriptForCreateDrop;
							sp.ScriptForAlter = false;
							sp.ScriptForCreateDrop = true;
							item.ScriptCreateInternal(query, sp);
							sp.ScriptForAlter = scriptForAlter2;
							sp.ScriptForCreateDrop = scriptForCreateDrop;
						}
						else if (item.State == SqlSmoState.ToBeDropped)
						{
							item.ScriptDropInternal(query, sp);
						}
					}
				}
				else if (propagateInfo2.bPropagateScriptToChildLevel)
				{
					item.PropagateScript(query, sp, action);
				}
			}
		}
	}

	private void PropagateStateAndCleanUp(StringCollection query, ScriptingPreferences sp, PropagateAction action)
	{
		if (!sp.ScriptForCreateDrop)
		{
			if (action == PropagateAction.Create || PropagateAction.Drop == action)
			{
				return;
			}
		}
		else if (!sp.ScriptForAlter && PropagateAction.Alter == action)
		{
			return;
		}
		if (PropagateAction.Drop == action || ExecutionManager.Recording)
		{
			return;
		}
		PropagateInfo[] propagateInfo = GetPropagateInfo(action);
		if (propagateInfo == null)
		{
			return;
		}
		PropagateInfo[] array = propagateInfo;
		foreach (PropagateInfo propagateInfo2 in array)
		{
			ArrayList arrayList = new ArrayList();
			ICollection collection;
			if (propagateInfo2.col != null)
			{
				if (propagateInfo2.col is AbstractCollectionBase { NoFaultCount: >0 } abstractCollectionBase)
				{
					collection = propagateInfo2.col;
					abstractCollectionBase.IsDirty = false;
				}
				else
				{
					collection = ((!(propagateInfo2.col is List<SqlSmoObject>)) ? new SqlSmoObject[0] : propagateInfo2.col);
				}
			}
			else
			{
				if (propagateInfo2.obj == null)
				{
					continue;
				}
				collection = new SqlSmoObject[1] { propagateInfo2.obj };
			}
			if (action == PropagateAction.Create && collection is AbstractCollectionBase abstractCollectionBase2)
			{
				abstractCollectionBase2.initialized = true;
			}
			foreach (SqlSmoObject item in collection)
			{
				if (action == PropagateAction.Create)
				{
					if (item.State != SqlSmoState.Existing)
					{
						item.PostCreate();
					}
					item.PropagateStateAndCleanUp(query, sp, action);
					item.SetState(SqlSmoState.Existing);
					item.CleanObject();
				}
				else
				{
					if (PropagateAction.Alter != action)
					{
						continue;
					}
					if (item.State == SqlSmoState.ToBeDropped)
					{
						arrayList.Add(item);
						continue;
					}
					if (item.State == SqlSmoState.Creating)
					{
						item.PostCreate();
						item.SetState(SqlSmoState.Existing);
					}
					item.CleanObject();
					item.PropagateStateAndCleanUp(query, sp, action);
				}
			}
			foreach (SqlSmoObject item2 in arrayList)
			{
				item2.SetState(SqlSmoState.Dropped);
				if (item2.parentColl != null)
				{
					item2.parentColl.RemoveObject(item2.key);
				}
			}
		}
	}

	protected internal static void UpdateCollectionState2(ICollection col)
	{
		foreach (SqlSmoObject item in col)
		{
			if (item.State == SqlSmoState.Creating)
			{
				item.SetState(SqlSmoState.Existing);
			}
			else if (item.State == SqlSmoState.ToBeDropped)
			{
				item.SetState(SqlSmoState.Dropped);
				item.ParentColl.RemoveObject(item.key);
			}
		}
	}

	internal virtual void PreInitChildLevel()
	{
	}

	internal List<string> InitQueryUrns(Urn levelFilter, string[] queryFields, OrderBy[] orderByFields, string[] infrastructureFields)
	{
		return InitQueryUrns(levelFilter, queryFields, orderByFields, infrastructureFields, null, null, DatabaseEngineEdition);
	}

	internal List<string> InitQueryUrns(Urn levelFilter, string[] queryFields, OrderBy[] orderByFields, string[] infrastructureFields, ScriptingPreferences sp, Urn initializeCollectionsFilter, DatabaseEngineEdition edition)
	{
		bool flag = sp != null;
		PreInitChildLevel();
		_ = Urn;
		InitializeStringComparer();
		XPathExpression xPathExpression = levelFilter.XPathExpression;
		int length = xPathExpression.Length;
		if (length < 1)
		{
			throw new InternalSmoErrorException(ExceptionTemplatesImpl.CallingInitQueryUrnsWithWrongUrn(levelFilter));
		}
		Type childType = GetChildType(xPathExpression[length - 1].Name, (length > 1) ? xPathExpression[length - 2].Name : GetType().Name);
		Request request = new Request(levelFilter, queryFields, orderByFields);
		Type type = null;
		int num = 0;
		if (length >= 2)
		{
			Type type2 = childType;
			int num2 = length - 2;
			request.ParentPropertiesRequests = new PropertiesRequest[length - 1];
			while (num2 >= 0)
			{
				type2 = GetChildType(xPathExpression[num2].Name, (num2 > 0) ? xPathExpression[num2 - 1].Name : GetType().Name);
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != type2, "currType == null");
				if (null == type)
				{
					type = type2;
				}
				PropertiesRequest propertiesRequest = new PropertiesRequest();
				propertiesRequest.Fields = GetQueryTypeKeyFields(type2);
				propertiesRequest.OrderByList = GetOrderByList(type2);
				if (propertiesRequest.OrderByList.Length == 1 && propertiesRequest.OrderByList[0].Field == "Name" && !type2.IsSubclassOf(typeof(NamedSmoObject)))
				{
					propertiesRequest.OrderByList = null;
				}
				request.ParentPropertiesRequests[length - 2 - num2] = propertiesRequest;
				num += ((propertiesRequest.Fields != null) ? propertiesRequest.Fields.Length : 0);
				num2--;
			}
		}
		type = type ?? GetType();
		if (request.Fields == null || request.Fields.Length == 0)
		{
			if (flag)
			{
				request.Fields = GetServerObject().GetScriptInitFieldsInternal(childType, type, sp, edition);
			}
			else
			{
				request.Fields = GetServerObject().GetDefaultInitFieldsInternal(childType, edition);
			}
		}
		StringCollection stringCollection = new StringCollection();
		stringCollection.AddRange(GetQueryTypeKeyFields(childType));
		string[] array = new string[stringCollection.Count + ((request.Fields != null) ? request.Fields.Length : 0) + ((infrastructureFields != null) ? infrastructureFields.Length : 0)];
		if (stringCollection.Count > 0)
		{
			stringCollection.CopyTo(array, 0);
		}
		int num3 = stringCollection.Count;
		if (request.Fields != null)
		{
			string[] fields = request.Fields;
			foreach (string text in fields)
			{
				bool flag2 = false;
				string[] array2 = array;
				foreach (string strB in array2)
				{
					if (string.CompareOrdinal(text, strB) == 0)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					array[num3] = text;
					num3++;
				}
			}
		}
		if (infrastructureFields != null)
		{
			foreach (string text2 in infrastructureFields)
			{
				bool flag3 = false;
				string[] array3 = array;
				foreach (string strB2 in array3)
				{
					if (string.CompareOrdinal(text2, strB2) == 0)
					{
						flag3 = true;
						break;
					}
				}
				if (!flag3)
				{
					array[num3] = text2;
					num3++;
				}
			}
		}
		if (num3 == 0)
		{
			request.Fields = null;
		}
		else
		{
			request.Fields = new string[num3];
			for (int m = 0; m < num3; m++)
			{
				request.Fields[m] = array[m];
			}
		}
		if (request.OrderByList == null || request.OrderByList.Length == 0)
		{
			request.OrderByList = GetOrderByList(childType);
			if (request.OrderByList.Length == 1 && request.OrderByList[0].Field == "Name" && !childType.IsSubclassOf(typeof(NamedSmoObject)))
			{
				request.OrderByList = null;
			}
		}
		List<string> list = new List<string>();
		if (IsDatabaseSpecialCase(flag, xPathExpression, request.Fields))
		{
			DoDatabaseSpecialCase(request, levelFilter, flag, list, num);
		}
		else
		{
			using (DataTable dataTable = ExecutionManager.GetEnumeratorData(request))
			{
				using DataTableReader reader = dataTable.CreateDataReader();
				InitObjectsFromEnumResults(levelFilter, reader, flag, list, num, skipServer: true);
			}
			if (flag && GetServerObject().GetScriptInitExpensiveFieldsInternal(childType, type, sp, out var fields2, edition))
			{
				request.Fields = fields2;
				using IDataReader reader2 = ExecutionManager.GetEnumeratorDataReader(request);
				InitObjectsFromEnumResults(levelFilter, reader2, flag, list, num, skipServer: true);
			}
			if (initializeCollectionsFilter != null)
			{
				MarkChildCollRetrieved(initializeCollectionsFilter, 1);
			}
		}
		return list;
	}

	internal bool IsSupportedProperty(string propertyName, ScriptingPreferences sp)
	{
		if (IsSupportedProperty(propertyName))
		{
			return PropertyMetadataProvider.CheckPropertyValid(GetPropertyMetadataProvider().GetType(), propertyName, ScriptingOptions.ConvertToServerVersion(sp.TargetServerVersion), sp.TargetDatabaseEngineType, sp.TargetDatabaseEngineEdition);
		}
		return false;
	}

	public bool IsSupportedProperty(string propertyName)
	{
		if (string.IsNullOrEmpty(propertyName))
		{
			throw new ArgumentNullException("propertyName");
		}
		if (!Properties.Contains(propertyName))
		{
			return false;
		}
		return true;
	}

	internal void ThrowIfPropertyNotSupported(string propertyName, ScriptingPreferences sp = null)
	{
		if (!IsSupportedProperty(propertyName))
		{
			throw new UnsupportedVersionException(ExceptionTemplatesImpl.PropertyNotSupportedWithDetails(propertyName, DatabaseEngineType.ToString(), ServerVersion.ToString(), DatabaseEngineEdition.ToString()));
		}
		if (sp != null && !IsSupportedProperty(propertyName, sp))
		{
			throw new UnsupportedVersionException(ExceptionTemplatesImpl.PropertyNotSupportedWithDetails(propertyName, sp.TargetDatabaseEngineType.ToString(), ScriptingOptions.ConvertToServerVersion(sp.TargetServerVersion).ToString(), sp.TargetDatabaseEngineEdition.ToString()));
		}
	}

	internal static List<string> GetSupportedScriptFields(Type type, string[] fields, ServerVersion serverVersion, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		List<string> list = new List<string>();
		foreach (string text in fields)
		{
			if (PropertyMetadataProvider.CheckPropertyValid(type, text, serverVersion, databaseEngineType, databaseEngineEdition))
			{
				list.Add(text);
			}
		}
		return list;
	}

	internal void InitChildLevel(Urn levelDescription, ScriptingPreferences sp)
	{
		InitChildLevel(levelDescription, sp, forScripting: false);
	}

	public void InitChildCollection(Urn childType, bool forScripting)
	{
		InitChildLevel(childType, new ScriptingPreferences(), forScripting);
	}

	internal void InitChildLevel(Urn levelFilter, ScriptingPreferences sp, bool forScripting)
	{
		InitChildLevel(levelFilter, sp, forScripting, null);
	}

	internal void InitChildLevel(Urn levelFilter, ScriptingPreferences sp, bool forScripting, IEnumerable<string> extraFields)
	{
		PreInitChildLevel();
		XPathExpression xPathExpression = levelFilter.XPathExpression;
		int length = xPathExpression.Length;
		if (length < 1)
		{
			throw new InternalSmoErrorException(ExceptionTemplatesImpl.CallingInitChildLevelWithWrongUrn(levelFilter));
		}
		Type childType = GetChildType(xPathExpression[length - 1].Name, (length > 1) ? xPathExpression[length - 2].Name : GetType().Name);
		Request request = new Request(string.Format(SmoApplication.DefaultCulture, "{0}/{1}", new object[2] { Urn, levelFilter }));
		Type type = null;
		if (length >= 2)
		{
			Type type2 = childType;
			int num = length - 2;
			request.ParentPropertiesRequests = new PropertiesRequest[length - 1];
			while (num >= 0)
			{
				type2 = GetChildType(xPathExpression[num].Name, (num > 0) ? xPathExpression[num - 1].Name : GetType().Name);
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != type2, "currType == null");
				if (null == type)
				{
					type = type2;
				}
				PropertiesRequest propertiesRequest = new PropertiesRequest();
				if (IsOrderedByID(type2))
				{
					propertiesRequest.Fields = new string[2] { "ID", "Name" };
				}
				else
				{
					StringCollection fieldNames = ObjectKeyBase.GetFieldNames(type2);
					propertiesRequest.Fields = new string[fieldNames.Count];
					fieldNames.CopyTo(propertiesRequest.Fields, 0);
				}
				propertiesRequest.OrderByList = GetOrderByList(type2);
				request.ParentPropertiesRequests[length - 2 - num] = propertiesRequest;
				num--;
			}
		}
		type = type ?? GetType();
		if (forScripting)
		{
			request.Fields = GetServerObject().GetScriptInitFieldsInternal(childType, type, sp, DatabaseEngineEdition);
		}
		else
		{
			request.Fields = GetServerObject().GetDefaultInitFieldsInternal(childType, DatabaseEngineEdition);
		}
		if (extraFields != null && extraFields.Count() > 0)
		{
			request.Fields = request.Fields.Union(extraFields).ToArray();
		}
		request.OrderByList = GetOrderByList(childType);
		if (IsDatabaseSpecialCase(forScripting, xPathExpression, request.Fields))
		{
			DoDatabaseSpecialCase(request, levelFilter, forScripting, null, 0);
			return;
		}
		using (IDataReader reader = ExecutionManager.GetEnumeratorDataReader(request))
		{
			InitObjectsFromEnumResults(levelFilter, reader, forScripting, null, 0, skipServer: false);
		}
		if (forScripting && GetServerObject().GetScriptInitExpensiveFieldsInternal(childType, type, sp, out var fields, sp.TargetDatabaseEngineEdition))
		{
			request.Fields = fields;
			using IDataReader reader2 = ExecutionManager.GetEnumeratorDataReader(request);
			InitObjectsFromEnumResults(levelFilter, reader2, forScripting, null, 0, skipServer: false);
		}
		MarkChildCollRetrieved(levelFilter, 0);
	}

	private bool IsDatabaseSpecialCase(bool forScripting, XPathExpression parsedUrn, string[] fields)
	{
		if (!forScripting && parsedUrn[parsedUrn.Length - 1].Name == "Database")
		{
			foreach (string text in fields)
			{
				if (text == "Status")
				{
					return true;
				}
			}
		}
		return false;
	}

	private void DoDatabaseSpecialCase(Request levelQuery, Urn levelFilter, bool forScripting, List<string> urnList, int startLeafIdx)
	{
		string[] fields = levelQuery.Fields;
		levelQuery.Fields = new string[2] { "Name", "Status" };
		using (IDataReader reader = ExecutionManager.GetEnumeratorDataReader(levelQuery))
		{
			InitObjectsFromEnumResults(levelFilter, reader, forScripting, null, 0, urnList != null);
		}
		if (urnList == null)
		{
			MarkChildCollRetrieved(levelFilter, 0);
		}
		levelQuery.Fields = fields;
		string text = levelQuery.Urn.ToString();
		if (text.EndsWith("]", StringComparison.Ordinal))
		{
			levelQuery.Urn = text.Insert(text.Length - 1, " and @Status=1");
		}
		else
		{
			levelQuery.Urn = string.Concat(levelQuery.Urn, "[@Status=1]");
		}
		text = levelFilter.ToString();
		levelFilter = ((!text.EndsWith("]", StringComparison.Ordinal)) ? ((Urn)string.Concat(levelFilter, "[@Status=1]")) : ((Urn)text.Insert(text.Length - 1, " and @Status=1")));
		using IDataReader reader2 = ExecutionManager.GetEnumeratorDataReader(levelQuery);
		InitObjectsFromEnumResults(levelFilter, reader2, forScripting, urnList, startLeafIdx, urnList != null);
	}

	private void MarkChildCollRetrieved(Urn levelFilter, int filterIdx)
	{
		MarkChildCollRetrievedRec(this, levelFilter.XPathExpression, filterIdx);
	}

	private void MarkChildCollRetrievedRec(SqlSmoObject currentSmoObject, XPathExpression levelFilter, int filterIdx)
	{
		if (currentSmoObject is Column && levelFilter[filterIdx].Name == "Default")
		{
			if (filterIdx == levelFilter.Length - 1)
			{
				((Column)currentSmoObject).m_bDefaultInitialized = true;
				return;
			}
			DefaultConstraint defaultConstraint = ((Column)currentSmoObject).DefaultConstraint;
			if (defaultConstraint != null)
			{
				MarkChildCollRetrievedRec(defaultConstraint, levelFilter, filterIdx + 1);
			}
			return;
		}
		if (currentSmoObject is TableViewBase && levelFilter[filterIdx].Name == "FullTextIndex")
		{
			if (filterIdx == levelFilter.Length - 1)
			{
				((TableViewBase)currentSmoObject).m_bFullTextIndexInitialized = true;
				return;
			}
			FullTextIndex fullTextIndex = ((TableViewBase)currentSmoObject).FullTextIndex;
			if (fullTextIndex != null)
			{
				MarkChildCollRetrievedRec(fullTextIndex, levelFilter, filterIdx + 1);
			}
			return;
		}
		if (currentSmoObject is Endpoint)
		{
			if (filterIdx != levelFilter.Length - 1)
			{
				SqlSmoObject sqlSmoObject = null;
				Endpoint endpoint = (Endpoint)currentSmoObject;
				switch (levelFilter[filterIdx].Name)
				{
				case "Soap":
					sqlSmoObject = endpoint.Payload.Soap;
					break;
				case "DatabaseMirroring":
					sqlSmoObject = endpoint.Payload.DatabaseMirroring;
					break;
				case "ServiceBroker":
					sqlSmoObject = endpoint.Payload.ServiceBroker;
					break;
				case "Http":
					sqlSmoObject = endpoint.Protocol.Http;
					break;
				case "Tcp":
					sqlSmoObject = endpoint.Protocol.Tcp;
					break;
				}
				if (sqlSmoObject != null)
				{
					MarkChildCollRetrievedRec(sqlSmoObject, levelFilter, filterIdx + 1);
				}
			}
			return;
		}
		AbstractCollectionBase childCollection = GetChildCollection(currentSmoObject, levelFilter, filterIdx, GetServerObject().ServerVersion);
		if (filterIdx == levelFilter.Length - 1)
		{
			if (levelFilter[filterIdx].Filter == null)
			{
				childCollection.initialized = true;
			}
			return;
		}
		HashSet<string> idSet = new HashSet<string>();
		string attributeFromFilter = levelFilter[filterIdx].GetAttributeFromFilter("Name");
		if (childCollection is DatabaseCollection && !string.IsNullOrEmpty(attributeFromFilter))
		{
			MarkChildCollRetrievedRec(GetServerObject().Databases.GetObjectByName(attributeFromFilter), levelFilter, filterIdx + 1);
			return;
		}
		IEnumerator enumerator = ((IEnumerable)childCollection).GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (ObjectInFilter((SqlSmoObject)enumerator.Current, levelFilter[filterIdx], idSet))
			{
				MarkChildCollRetrievedRec((SqlSmoObject)enumerator.Current, levelFilter, filterIdx + 1);
			}
		}
	}

	private bool ObjectInFilter(SqlSmoObject current, XPathExpressionBlock levelFilterBlock, HashSet<string> idSet)
	{
		FilterNode filter = levelFilterBlock.Filter;
		if (filter == null)
		{
			return true;
		}
		if (filter.NodeType == FilterNode.Type.Operator)
		{
			FilterNodeOperator opNode = (FilterNodeOperator)filter;
			return ObjectInFilterRec(current, opNode);
		}
		if (filter.NodeType == FilterNode.Type.Function)
		{
			FilterNodeFunction filterNodeFunction = (FilterNodeFunction)filter;
			if (filterNodeFunction.FunctionType == FilterNodeFunction.Type.In)
			{
				if (idSet.Count == 0)
				{
					string valueAsString = ((FilterNodeConstant)filterNodeFunction.GetParameter(1)).ValueAsString;
					idSet.UnionWith(valueAsString.Split(','));
				}
				int? propValueOptional = current.GetPropValueOptional<int>("ID");
				if (propValueOptional.HasValue && idSet.Contains(propValueOptional.ToString()))
				{
					return true;
				}
				return false;
			}
		}
		throw new SmoException(ExceptionTemplatesImpl.UnknownFilter(levelFilterBlock.ToString()));
	}

	private bool ObjectInFilterRec(SqlSmoObject current, FilterNodeOperator opNode)
	{
		if (opNode.OpType == FilterNodeOperator.Type.OR)
		{
			if (!ObjectInFilterRec(current, (FilterNodeOperator)opNode.Left))
			{
				return ObjectInFilterRec(current, (FilterNodeOperator)opNode.Right);
			}
			return true;
		}
		if (opNode.OpType == FilterNodeOperator.Type.And)
		{
			if (CompareAttributeToObject(current, (FilterNodeOperator)opNode.Left))
			{
				return CompareAttributeToObject(current, (FilterNodeOperator)opNode.Right);
			}
			return false;
		}
		if (opNode.OpType == FilterNodeOperator.Type.EQ || opNode.OpType == FilterNodeOperator.Type.LT || opNode.OpType == FilterNodeOperator.Type.GT)
		{
			return CompareAttributeToObject(current, opNode);
		}
		throw new SmoException(ExceptionTemplatesImpl.UnknownFilter(opNode.ToString()));
	}

	private bool CompareAttributeToObject(SqlSmoObject current, FilterNodeOperator oper)
	{
		FilterNodeAttribute filterNodeAttribute = (FilterNodeAttribute)oper.Left;
		object obj = null;
		if (oper.Right.NodeType == FilterNode.Type.Constant)
		{
			FilterNodeConstant filterNodeConstant = (FilterNodeConstant)oper.Right;
			obj = filterNodeConstant.Value;
		}
		StringComparer parentStringComparer = GetParentStringComparer(current);
		switch (oper.OpType)
		{
		case FilterNodeOperator.Type.EQ:
			switch (oper.Right.NodeType)
			{
			case FilterNode.Type.Constant:
				return filterNodeAttribute.Name switch
				{
					"Name" => 0 == parentStringComparer.Compare(((SimpleObjectKey)current.key).Name, Urn.UnEscapeString((string)obj)), 
					"Schema" => 0 == parentStringComparer.Compare(((SchemaObjectKey)current.key).Schema, Urn.UnEscapeString((string)obj)), 
					"ID" => ((MessageObjectKey)current.key).ID == (int)obj, 
					"Language" => 0 == parentStringComparer.Compare(((MessageObjectKey)current.key).Language, Urn.UnEscapeString((string)obj)), 
					_ => throw new UnknownPropertyException(filterNodeAttribute.Name), 
				};
			case FilterNode.Type.Function:
				if (filterNodeAttribute.Name == "IsSystemObject")
				{
					FilterNodeFunction filterNodeFunction = (FilterNodeFunction)oper.Right;
					if (filterNodeFunction.FunctionType != FilterNodeFunction.Type.True && filterNodeFunction.FunctionType != FilterNodeFunction.Type.False)
					{
						throw new InternalSmoErrorException(ExceptionTemplatesImpl.UnsupportedUrnFilter(filterNodeAttribute.Name, filterNodeFunction.FunctionType.ToString()));
					}
					bool flag = filterNodeFunction.FunctionType == FilterNodeFunction.Type.True;
					return flag == (bool)current.GetPropValue("IsSystemObject");
				}
				throw new InternalSmoErrorException(ExceptionTemplatesImpl.UnsupportedUrnAttrib(filterNodeAttribute.Name));
			}
			break;
		case FilterNodeOperator.Type.LT:
		{
			FilterNode.Type nodeType2 = oper.Right.NodeType;
			if (nodeType2 == FilterNode.Type.Constant)
			{
				return filterNodeAttribute.Name switch
				{
					"Name" => -1 == parentStringComparer.Compare(((SimpleObjectKey)current.key).Name, Urn.UnEscapeString((string)obj)), 
					"Schema" => -1 == parentStringComparer.Compare(((SchemaObjectKey)current.key).Schema, Urn.UnEscapeString((string)obj)), 
					"ID" => ((MessageObjectKey)current.key).ID < (int)obj, 
					"Language" => -1 == parentStringComparer.Compare(((MessageObjectKey)current.key).Language, Urn.UnEscapeString((string)obj)), 
					_ => throw new UnknownPropertyException(filterNodeAttribute.Name), 
				};
			}
			break;
		}
		case FilterNodeOperator.Type.GT:
		{
			FilterNode.Type nodeType = oper.Right.NodeType;
			if (nodeType == FilterNode.Type.Constant)
			{
				return filterNodeAttribute.Name switch
				{
					"Name" => 1 == parentStringComparer.Compare(((SimpleObjectKey)current.key).Name, Urn.UnEscapeString((string)obj)), 
					"Schema" => 1 == parentStringComparer.Compare(((SchemaObjectKey)current.key).Schema, Urn.UnEscapeString((string)obj)), 
					"ID" => ((MessageObjectKey)current.key).ID > (int)obj, 
					"Language" => 1 == parentStringComparer.Compare(((MessageObjectKey)current.key).Language, Urn.UnEscapeString((string)obj)), 
					_ => throw new UnknownPropertyException(filterNodeAttribute.Name), 
				};
			}
			break;
		}
		}
		throw new SmoException(ExceptionTemplatesImpl.UnknownFilter(oper.ToString()));
	}

	private StringComparer GetParentStringComparer(SqlSmoObject Object)
	{
		SqlSmoObject sqlSmoObject = ((Object.ParentColl != null && Object.ParentColl.ParentInstance != null) ? Object.ParentColl.ParentInstance : Object);
		return sqlSmoObject.StringComparer;
	}

	private void InitObjectsFromEnumResults(Urn levelFilter, IDataReader reader, bool forScripting, List<string> urnList, int startLeafIdx, bool skipServer)
	{
		if (!reader.Read())
		{
			reader.Close();
			return;
		}
		object[] array = new object[reader.FieldCount];
		reader.GetValues(array);
		bool flag = skipServer && levelFilter.XPathExpression.Length > 1;
		int filterIdx = (flag ? 1 : 0);
		int columnIdx = (flag ? 1 : 0);
		InitObjectsFromEnumResultsRec(this, levelFilter.XPathExpression, filterIdx, reader, columnIdx, array, forScripting, urnList, startLeafIdx);
	}

	private void InitObjectsFromEnumResultsRec(SqlSmoObject currentSmoObject, XPathExpression levelFilter, int filterIdx, IDataReader reader, int columnIdx, object[] parentRow, bool forScripting, List<string> urnList, int startLeafIdx)
	{
		if (reader.IsClosed)
		{
			return;
		}
		Type childType;
		if (urnList != null)
		{
			int length = levelFilter.Length;
			childType = GetChildType((length > filterIdx) ? levelFilter[filterIdx].Name : currentSmoObject.GetType().Name, currentSmoObject.GetType().Name);
		}
		else
		{
			childType = GetChildType(levelFilter[filterIdx].Name, currentSmoObject.GetType().Name);
		}
		if (childType.Equals(typeof(DefaultConstraint)))
		{
			if (filterIdx == levelFilter.Length - 1)
			{
				((Column)currentSmoObject).InitializeDefault(reader, columnIdx, forScripting);
				urnList?.Add(((Column)currentSmoObject).DefaultConstraint.Urn.ToString());
				if (!reader.Read())
				{
					reader.Close();
				}
			}
			else
			{
				DefaultConstraint defaultConstraint = ((Column)currentSmoObject).DefaultConstraint;
				if (defaultConstraint != null)
				{
					InitObjectsFromEnumResultsRec(defaultConstraint, levelFilter, filterIdx + 1, reader, columnIdx + 1, parentRow, forScripting, urnList, startLeafIdx);
				}
			}
			return;
		}
		if (childType.Equals(typeof(FullTextIndex)))
		{
			if (filterIdx == levelFilter.Length - 1)
			{
				currentSmoObject = ((TableViewBase)currentSmoObject).InitializeFullTextIndexNoEnum();
				if (reader.FieldCount - columnIdx > 1)
				{
					currentSmoObject.AddObjectPropsFromDataReader(reader, skipIfDirty: true, columnIdx, -1);
				}
				if (forScripting)
				{
					currentSmoObject.InitializedForScripting = true;
				}
				urnList?.Add(currentSmoObject.Urn.ToString());
				if (!reader.Read())
				{
					reader.Close();
				}
			}
			else
			{
				FullTextIndex fullTextIndex = ((TableViewBase)currentSmoObject).FullTextIndex;
				if (fullTextIndex != null)
				{
					InitObjectsFromEnumResultsRec(fullTextIndex, levelFilter, filterIdx + 1, reader, columnIdx + 1, parentRow, forScripting, urnList, startLeafIdx);
				}
			}
			return;
		}
		if (currentSmoObject is Endpoint)
		{
			if (filterIdx == levelFilter.Length - 1)
			{
				if (!reader.Read())
				{
					reader.Close();
				}
				return;
			}
			SqlSmoObject sqlSmoObject = null;
			Endpoint endpoint = (Endpoint)currentSmoObject;
			if (childType.Equals(typeof(SoapPayload)))
			{
				sqlSmoObject = endpoint.Payload.Soap;
			}
			else if (childType.Equals(typeof(DatabaseMirroringPayload)))
			{
				sqlSmoObject = endpoint.Payload.DatabaseMirroring;
			}
			else if (childType.Equals(typeof(ServiceBrokerPayload)))
			{
				sqlSmoObject = endpoint.Payload.ServiceBroker;
			}
			else if (childType.Equals(typeof(HttpProtocol)))
			{
				sqlSmoObject = endpoint.Protocol.Http;
			}
			else if (childType.Equals(typeof(TcpProtocol)))
			{
				sqlSmoObject = endpoint.Protocol.Tcp;
			}
			if (sqlSmoObject != null)
			{
				InitObjectsFromEnumResultsRec(sqlSmoObject, levelFilter, filterIdx + 1, reader, columnIdx, parentRow, forScripting, urnList, startLeafIdx);
			}
			return;
		}
		AbstractCollectionBase abstractCollectionBase = null;
		bool flag = false;
		try
		{
			abstractCollectionBase = GetChildCollection(currentSmoObject, levelFilter, filterIdx, GetServerObject().ServerVersion);
		}
		catch (Exception ex)
		{
			if (urnList == null)
			{
				throw;
			}
			if (!(ex is ArgumentException) && !(ex is InvalidCastException))
			{
				throw;
			}
			flag = true;
		}
		int queryTypeKeyFieldsCount = GetQueryTypeKeyFieldsCount(childType);
		if (flag)
		{
			SqlSmoObject childSingleton = GetChildSingleton(currentSmoObject, levelFilter, filterIdx, GetServerObject().ServerVersion);
			if (AdvanceInitRec(childSingleton, levelFilter, filterIdx, reader, columnIdx, queryTypeKeyFieldsCount, parentRow, forScripting, urnList, startLeafIdx))
			{
			}
			return;
		}
		bool flag2 = IsOrderedByID(childType);
		if (abstractCollectionBase.initialized)
		{
			IEnumerator enumerator = ((IEnumerable)abstractCollectionBase).GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (reader.IsClosed)
				{
					return;
				}
				SqlSmoObject sqlSmoObject2 = (SqlSmoObject)enumerator.Current;
				if (!CompareRows(reader, parentRow, 0, columnIdx))
				{
					return;
				}
				int num = CompareObjectToRow(sqlSmoObject2, reader, columnIdx, flag2, levelFilter, filterIdx);
				if (num == 0)
				{
					if (!AdvanceInitRec(sqlSmoObject2, levelFilter, filterIdx, reader, columnIdx, queryTypeKeyFieldsCount, parentRow, forScripting, urnList, startLeafIdx))
					{
						return;
					}
				}
				else if (num > 0)
				{
					SqlSmoObject existingOrCreateNewObject = GetExistingOrCreateNewObject(reader, columnIdx, childType, abstractCollectionBase, flag2);
					enumerator = ((IEnumerable)abstractCollectionBase).GetEnumerator();
					while (enumerator.MoveNext() && enumerator.Current != existingOrCreateNewObject)
					{
					}
					sqlSmoObject2 = (SqlSmoObject)enumerator.Current;
					if (!AdvanceInitRec(existingOrCreateNewObject, levelFilter, filterIdx, reader, columnIdx, queryTypeKeyFieldsCount, parentRow, forScripting, urnList, startLeafIdx))
					{
						return;
					}
				}
			}
			if (reader.IsClosed)
			{
				return;
			}
			while (CompareRows(reader, parentRow, 0, columnIdx))
			{
				SqlSmoObject existingOrCreateNewObject2 = GetExistingOrCreateNewObject(reader, columnIdx, childType, abstractCollectionBase, flag2);
				if (!AdvanceInitRec(existingOrCreateNewObject2, levelFilter, filterIdx, reader, columnIdx, queryTypeKeyFieldsCount, parentRow, forScripting, urnList, startLeafIdx))
				{
					return;
				}
			}
		}
		else
		{
			bool flag3 = 0 != abstractCollectionBase.NoFaultCount;
			bool skipOrderChecking = flag2 && abstractCollectionBase.NoFaultCount == 0 && levelFilter[filterIdx].Filter == null && filterIdx == levelFilter.Length - 1;
			if (!reader.IsClosed)
			{
				SqlSmoObject sqlSmoObject3 = null;
				while (CompareRows(reader, parentRow, 0, columnIdx))
				{
					if (flag3)
					{
						ObjectKeyBase objectKeyBase = ObjectKeyBase.CreateKeyOffset(childType, reader, columnIdx);
						sqlSmoObject3 = abstractCollectionBase.NoFaultLookup(objectKeyBase);
					}
					else
					{
						sqlSmoObject3 = null;
					}
					if (sqlSmoObject3 == null)
					{
						sqlSmoObject3 = CreateNewObjectFromRow(abstractCollectionBase, childType, reader, columnIdx, flag2, skipOrderChecking);
					}
					if (!AdvanceInitRec(sqlSmoObject3, levelFilter, filterIdx, reader, columnIdx, queryTypeKeyFieldsCount, parentRow, forScripting, urnList, startLeafIdx))
					{
						break;
					}
				}
			}
		}
		if (levelFilter[filterIdx].Filter == null && filterIdx == levelFilter.Length - 1)
		{
			abstractCollectionBase.initialized = true;
		}
	}

	private SqlSmoObject CreateNewObjectFromRow(AbstractCollectionBase childColl, Type childType, IDataReader reader, int columnIdx, bool isOrderedByID, bool skipOrderChecking)
	{
		ObjectKeyBase objectKeyBase = ObjectKeyBase.CreateKeyOffset(childType, reader, columnIdx);
		object[] args = new object[3]
		{
			childColl,
			objectKeyBase,
			SqlSmoState.Existing
		};
		SqlSmoObject sqlSmoObject = (SqlSmoObject)Activator.CreateInstance(childType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, args, null);
		ISqlSmoObjectInitialize sqlSmoObjectInitialize = sqlSmoObject;
		sqlSmoObjectInitialize.InitializeFromDataReader(reader);
		sqlSmoObject.SetState(PropertyBagState.Lazy);
		if (isOrderedByID)
		{
			sqlSmoObject.Properties.Get("ID").SetValue(reader.GetValue(columnIdx));
			sqlSmoObject.Properties.Get("ID").SetRetrieved(retrieved: true);
			if (skipOrderChecking && childColl is ParameterCollectionBase parameterCollectionBase)
			{
				parameterCollectionBase.InternalStorage.InsertAt(parameterCollectionBase.InternalStorage.Count, sqlSmoObject);
				return sqlSmoObject;
			}
		}
		childColl.AddExisting(sqlSmoObject);
		return sqlSmoObject;
	}

	private SqlSmoObject GetExistingOrCreateNewObject(IDataReader reader, int columnIdx, Type childType, AbstractCollectionBase childColl, bool isOrderedByID)
	{
		SqlSmoObject sqlSmoObject = null;
		if (!isOrderedByID && childColl is SortedListCollectionBase)
		{
			sqlSmoObject = ((SortedListCollectionBase)childColl).NoFaultLookup(ObjectKeyBase.CreateKeyOffset(childType, reader, columnIdx));
		}
		if (sqlSmoObject == null)
		{
			sqlSmoObject = CreateNewObjectFromRow(childColl, childType, reader, columnIdx, isOrderedByID, skipOrderChecking: false);
		}
		return sqlSmoObject;
	}

	private bool AdvanceInitRec(SqlSmoObject currentSmoObject, XPathExpression levelFilter, int filterIdx, IDataReader reader, int columnIdx, int columnOffset, object[] parentRow, bool forScripting, List<string> urnList, int startLeafIdx)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != parentRow, "parentRow == null");
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(parentRow.Length == reader.FieldCount, "parentRow.Length != reader.FieldCount");
		if (filterIdx == levelFilter.Length - 1)
		{
			if (reader.FieldCount - columnIdx > 1)
			{
				currentSmoObject.AddObjectPropsFromDataReader(reader, skipIfDirty: true, columnIdx, -1);
			}
			if (forScripting)
			{
				currentSmoObject.InitializedForScripting = true;
			}
			urnList?.Add(currentSmoObject.Urn.ToString());
			if (!reader.Read())
			{
				reader.Close();
				return false;
			}
		}
		else
		{
			object[] array = new object[reader.FieldCount];
			reader.GetValues(array);
			InitObjectsFromEnumResultsRec(currentSmoObject, levelFilter, filterIdx + 1, reader, columnIdx + columnOffset, array, forScripting, urnList, startLeafIdx);
		}
		return true;
	}

	internal static AbstractCollectionBase GetChildCollection(SqlSmoObject parent, XPathExpression levelFilter, int filterIdx, ServerVersion srvVer)
	{
		return GetChildCollection(parent, levelFilter[filterIdx].Name, levelFilter[filterIdx].GetAttributeFromFilter("CategoryClass"), srvVer);
	}

	internal static string GetPluralName(string name, SqlSmoObject parent)
	{
		switch (name)
		{
		case "Index":
			return "Indexes";
		case "Numbered":
			return "NumberedStoredProcedures";
		case "Method":
			return "SoapPayloadMethods";
		case "Param":
			return "Parameters";
		case "ExtendedProperty":
			return "ExtendedProperties";
		case "JobCategory":
			return "JobCategories";
		case "AlertCategory":
			return "AlertCategories";
		case "OperatorCategory":
			return "OperatorCategories";
		case "Column":
			if (parent is Statistic)
			{
				return "StatisticColumns";
			}
			return "Columns";
		case "Step":
			return "JobSteps";
		case "Schedule":
			if (parent is Job)
			{
				return "JobSchedules";
			}
			return "SharedSchedules";
		case "Login":
			if (parent is LinkedServer)
			{
				return "LinkedServerLogins";
			}
			return "Logins";
		case "SqlAssembly":
			return "Assemblies";
		case "ExternalLibrary":
			return "ExternalLibraries";
		case "FullTextIndexColumn":
			return "IndexedColumns";
		case "DdlTrigger":
			return "Triggers";
		case "MailProfile":
			return "Profiles";
		case "MailAccount":
			return "Accounts";
		case "ServiceQueue":
			return "Queues";
		case "BrokerService":
			return "Services";
		case "BrokerPriority":
			return "Priorities";
		case "ServiceRoute":
			return "Routes";
		case "EventClass":
			return "EventClasses";
		case "NotificationClass":
			return "NotificationClasses";
		case "SubscriptionClass":
			return "SubscriptionClasses";
		case "SearchProperty":
			return "SearchProperties";
		case "SecurityPolicy":
			return "SecurityPolicies";
		case "ExternalDataSource":
			return "ExternalDataSources";
		case "ExternalFileFormat":
			return "ExternalFileFormats";
		case "AvailabilityGroupListenerIPAddress":
			return "AvailabilityGroupListenerIPAddresses";
		case "ResumableIndex":
			return "ResumableIndexes";
		default:
			return name + "s";
		}
	}

	internal static AbstractCollectionBase GetChildCollection(SqlSmoObject parent, string childUrnSuffix, string categorystr, ServerVersion srvVer)
	{
		string pluralName = GetPluralName(childUrnSuffix, parent);
		object obj = null;
		if (pluralName != "Permissions")
		{
			try
			{
				obj = parent.GetType().InvokeMember(pluralName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, parent, new object[0], SmoApplication.DefaultCulture);
			}
			catch (MissingMethodException)
			{
				throw new ArgumentException(ExceptionTemplatesImpl.InvalidPathChildCollectionNotFound(childUrnSuffix, parent.GetType().Name));
			}
		}
		else
		{
			obj = parent.GetType().InvokeMember(pluralName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty, null, parent, new object[0], SmoApplication.DefaultCulture);
		}
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != obj, "null == childCollection");
		return (AbstractCollectionBase)obj;
	}

	private SqlSmoObject GetChildSingleton(SqlSmoObject parent, XPathExpression levelFilter, int filterIdx, ServerVersion srvVer)
	{
		string value = null;
		object obj = null;
		int length = levelFilter.Length;
		Type childType = GetChildType((length > filterIdx) ? levelFilter[filterIdx].Name : parent.GetType().Name, (filterIdx > 0) ? levelFilter[filterIdx - 1].Name : parent.GetType().Name);
		if (childType == typeof(Server))
		{
			return parent;
		}
		if (!s_SingletonTypeToProperty.TryGetValue(childType, out value))
		{
			SfcMetadataDiscovery sfcMetadataDiscovery = new SfcMetadataDiscovery(parent.GetType());
			foreach (SfcMetadataRelation relation in sfcMetadataDiscovery.Relations)
			{
				if ((relation.Relationship == SfcRelationship.ChildObject || relation.Relationship == SfcRelationship.Object) && childType == relation.Type)
				{
					value = relation.PropertyName;
					break;
				}
			}
			lock (((ICollection)s_SingletonTypeToProperty).SyncRoot)
			{
				s_SingletonTypeToProperty[childType] = value;
			}
		}
		if (value == null)
		{
			throw new ArgumentException(ExceptionTemplatesImpl.InvalidPathChildSingletonNotFound(childType.Name, parent.GetType().Name));
		}
		try
		{
			obj = parent.GetType().InvokeMember(value, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, parent, new object[0], SmoApplication.DefaultCulture);
		}
		catch (MissingMethodException)
		{
			throw new ArgumentException(ExceptionTemplatesImpl.InvalidPathChildSingletonNotFound(childType.Name, parent.GetType().Name));
		}
		return (SqlSmoObject)obj;
	}

	private int CompareObjectToRow(SqlSmoObject currObj, IDataReader currentRow, int colIdx, bool isOrderedByID, XPathExpression xpath, int xpathIdx)
	{
		if (isOrderedByID)
		{
			string text = ((xpathIdx != xpath.Length - 1) ? (xpath[xpathIdx].Name + "_ID") : "ID");
			int num = Convert.ToInt32(currObj.Properties["ID"].Value, SmoApplication.DefaultCulture);
			int i = -1;
			try
			{
				i = currentRow.GetOrdinal(text);
			}
			catch (IndexOutOfRangeException)
			{
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(condition: false, "currentRow.GetOrdinal(" + text + ") failed");
			}
			int num2 = Convert.ToInt32(currentRow.GetValue(i), SmoApplication.DefaultCulture);
			return num - num2;
		}
		ObjectKeyBase obj = ObjectKeyBase.CreateKeyOffset(currObj.GetType(), currentRow, colIdx);
		return currObj.KeyComparer.Compare(currObj.key, obj);
	}

	private bool CompareRows(IDataReader reader, object[] parentRow, int columnStartIdx, int columnStopIdx)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != reader, "reader == null");
		if (reader.IsClosed)
		{
			return false;
		}
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != parentRow, "parentRow == null");
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(columnStartIdx >= 0, "columnStartIdx < 0");
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(columnStopIdx < reader.FieldCount, "columnStopIdx >= reader.FieldCount");
		for (int i = columnStartIdx; i < columnStopIdx; i++)
		{
			if (!reader.GetValue(i).Equals(parentRow[i]))
			{
				return false;
			}
		}
		return true;
	}

	internal static string[] GetQueryTypeInfrastructureFields(Type t)
	{
		switch (t.Name)
		{
		case "Information":
			return new string[1] { "Edition" };
		case "Database":
			return new string[2] { "CompatibilityLevel", "Collation" };
		case "StoredProcedure":
		case "UserDefinedFunction":
		case "Trigger":
		case "DdlTrigger":
		case "DatabaseDdlTrigger":
			return new string[1] { "ImplementationType" };
		default:
			return new string[0];
		}
	}

	private static OrderBy[] GetOrderByList(Type objType)
	{
		if (objType.IsSubclassOf(typeof(ScriptSchemaObjectBase)))
		{
			return new OrderBy[2]
			{
				new OrderBy("Schema", OrderBy.Direction.Asc),
				new OrderBy("Name", OrderBy.Direction.Asc)
			};
		}
		if (objType.Equals(typeof(NumberedStoredProcedure)))
		{
			return new OrderBy[1]
			{
				new OrderBy("Number", OrderBy.Direction.Asc)
			};
		}
		if (IsOrderedByID(objType) || objType.IsSubclassOf(typeof(MessageObjectBase)))
		{
			return new OrderBy[1]
			{
				new OrderBy("ID", OrderBy.Direction.Asc)
			};
		}
		if (objType.Equals(typeof(FullTextIndex)))
		{
			return new OrderBy[0];
		}
		if (objType.IsSubclassOf(typeof(SoapMethodObject)))
		{
			return new OrderBy[2]
			{
				new OrderBy("Namespace", OrderBy.Direction.Asc),
				new OrderBy("Name", OrderBy.Direction.Asc)
			};
		}
		if (objType.Equals(typeof(PhysicalPartition)))
		{
			return new OrderBy[1]
			{
				new OrderBy("PartitionNumber", OrderBy.Direction.Asc)
			};
		}
		if (objType.Equals(typeof(DatabaseReplicaState)))
		{
			return new OrderBy[2]
			{
				new OrderBy("AvailabilityReplicaServerName", OrderBy.Direction.Asc),
				new OrderBy("AvailabilityDatabaseName", OrderBy.Direction.Asc)
			};
		}
		if (objType.Equals(typeof(AvailabilityGroupListenerIPAddress)))
		{
			return new OrderBy[3]
			{
				new OrderBy("IPAddress", OrderBy.Direction.Asc),
				new OrderBy("SubnetMask", OrderBy.Direction.Asc),
				new OrderBy("SubnetIP", OrderBy.Direction.Asc)
			};
		}
		if (objType.Equals(typeof(SecurityPredicate)))
		{
			return new OrderBy[1]
			{
				new OrderBy("SecurityPredicateID", OrderBy.Direction.Asc)
			};
		}
		if (objType.Equals(typeof(ColumnEncryptionKeyValue)))
		{
			return new OrderBy[1]
			{
				new OrderBy("ColumnMasterKeyID", OrderBy.Direction.Asc)
			};
		}
		return new OrderBy[1]
		{
			new OrderBy("Name", OrderBy.Direction.Asc)
		};
	}

	internal static bool IsOrderedByID(Type t)
	{
		if (!t.IsSubclassOf(typeof(ParameterBase)) && !t.Equals(typeof(Column)) && !t.Equals(typeof(ForeignKeyColumn)) && !t.Equals(typeof(OrderColumn)) && !t.Equals(typeof(IndexedColumn)) && !t.Equals(typeof(IndexedXmlPath)) && !t.Equals(typeof(StatisticColumn)) && !t.Equals(typeof(JobStep)) && !t.Equals(typeof(PartitionFunctionParameter)))
		{
			return t.Equals(typeof(PartitionSchemeParameter));
		}
		return true;
	}

	internal static string[] GetQueryTypeKeyFields(Type t)
	{
		if (!s_TypeToKeyFields.TryGetValue(t, out var value))
		{
			if (IsOrderedByID(t))
			{
				value = new string[2] { "ID", "Name" };
			}
			else if (t.IsSubclassOf(typeof(NamedSmoObject)) || t == typeof(Server) || t == typeof(FullTextIndex) || t == typeof(DatabaseReplicaState) || t == typeof(AvailabilityGroupListenerIPAddress))
			{
				StringCollection fieldNames = ObjectKeyBase.GetFieldNames(t);
				value = new string[fieldNames.Count];
				fieldNames.CopyTo(value, 0);
			}
			else
			{
				value = new string[0];
			}
			lock (((ICollection)s_TypeToKeyFields).SyncRoot)
			{
				s_TypeToKeyFields[t] = value;
			}
		}
		return value;
	}

	internal static int GetQueryTypeKeyFieldsCount(Type t)
	{
		return GetQueryTypeKeyFields(t).Length;
	}

	public static Type GetChildType(string objectName, string parentName)
	{
		if (objectName == "Server")
		{
			return typeof(Server);
		}
		string empty = string.Empty;
		empty = objectName switch
		{
			"Server" => "Server", 
			"Role" => (!(parentName == "Server")) ? "DatabaseRole" : "ServerRole", 
			"Default" => (!(parentName == "Column")) ? "Default" : "DefaultConstraint", 
			"File" => "DataFile", 
			"Column" => (!(parentName == "ForeignKey")) ? ((!(parentName == "Statistic")) ? "Column" : "StatisticColumn") : "ForeignKeyColumn", 
			"Mail" => "SqlMail", 
			"Schedule" => "JobSchedule", 
			"Step" => "JobStep", 
			"Login" => (!(parentName == "LinkedServer")) ? "Login" : "LinkedServerLogin", 
			"Param" => (!(parentName == "Numbered")) ? (parentName + "Parameter") : "NumberedStoredProcedureParameter", 
			"Numbered" => "NumberedStoredProcedure", 
			"Setting" => "Settings", 
			"Option" => "DatabaseOptions", 
			"Method" => "SoapPayloadMethod", 
			"Soap" => "SoapPayload", 
			"OleDbProviderSetting" => "OleDbProviderSettings", 
			"DdlTrigger" => (!(parentName == "Server")) ? "DatabaseDdlTrigger" : "ServerDdlTrigger", 
			"Permission" => "UserPermission", 
			"UserOption" => "UserOptions", 
			"DatabaseMirroring" => "DatabaseMirroringPayload", 
			"Http" => "HttpProtocol", 
			"Tcp" => "TcpProtocol", 
			"MasterKey" => (!(parentName == "Server")) ? "MasterKey" : "ServiceMasterKey", 
			_ => objectName, 
		};
		Assembly assembly = typeof(Server).GetAssembly();
		Type type;
		switch (objectName)
		{
		case "RegisteredServer":
		case "ServerGroup":
		{
			string name = "Microsoft.SqlServer.Management.Smo.RegisteredServers." + empty + ",Microsoft.SqlServer.SmoExtended";
			type = assembly.GetType(name, throwOnError: true);
			break;
		}
		default:
		{
			string name = "Microsoft.SqlServer.Management.Smo." + empty;
			type = assembly.GetType(name, throwOnError: false);
			if (!(type == null))
			{
				break;
			}
			type = assembly.GetType("Microsoft.SqlServer.Management.Smo.Agent." + empty, throwOnError: false);
			if (type == null)
			{
				type = assembly.GetType("Microsoft.SqlServer.Management.Smo.Mail." + empty, throwOnError: false);
				if (type == null)
				{
					type = assembly.GetType("Microsoft.SqlServer.Management.Smo.Broker." + empty, throwOnError: false);
				}
			}
			break;
		}
		}
		return type;
	}

	protected void ThrowIfAboveVersion80(string exceptionMessage = null)
	{
		if (ServerVersion.Major > 8)
		{
			throw new UnsupportedVersionException(string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.SupportedOnlyBelow90 : exceptionMessage).SetHelpContext("SupportedOnlyBelow90");
		}
	}

	protected void ThrowIfBelowVersion80(string exceptionMessage = null)
	{
		if (ServerVersion.Major < 8)
		{
			throw new UnsupportedVersionException(string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.SupportedOnlyOn80 : exceptionMessage).SetHelpContext("SupportedOnlyOn80");
		}
	}

	internal void ThrowIfSourceOrDestBelowVersion80(SqlServerVersionInternal targetVersion, string exceptionMessage = null)
	{
		ThrowIfBelowVersion80(exceptionMessage);
		ThrowIfBelowVersion80(targetVersion, exceptionMessage);
	}

	protected void ThrowIfBelowVersion90(string exceptionMessage = null)
	{
		if (ServerVersion.Major < 9)
		{
			throw new UnsupportedVersionException(string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.SupportedOnlyOn90 : exceptionMessage).SetHelpContext("SupportedOnlyOn90");
		}
	}

	internal void ThrowIfSourceOrDestBelowVersion90(SqlServerVersionInternal targetVersion, string exceptionMessage = null)
	{
		ThrowIfBelowVersion90(exceptionMessage);
		ThrowIfBelowVersion90(targetVersion, exceptionMessage);
	}

	protected void ThrowIfAboveVersion100(string exceptionMessage = null)
	{
		if (ServerVersion.Major > 10)
		{
			throw new UnsupportedFeatureException(string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.SupportedOnlyBelow110 : exceptionMessage).SetHelpContext("SupportedOnlyBelow110");
		}
	}

	protected void ThrowIfBelowVersion100(string exceptionMessage = null)
	{
		if (ServerVersion.Major < 10)
		{
			throw new UnsupportedVersionException(string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.SupportedOnlyOn100 : exceptionMessage).SetHelpContext("SupportedOnlyOn100");
		}
	}

	internal void ThrowIfSourceOrDestBelowVersion100(SqlServerVersionInternal targetVersion, string exceptionMessage = null)
	{
		ThrowIfBelowVersion100(exceptionMessage);
		ThrowIfBelowVersion100(targetVersion, exceptionMessage);
	}

	protected void ThrowIfBelowVersion110(string exceptionMessage = null)
	{
		if (ServerVersion.Major < 11)
		{
			throw new UnsupportedVersionException(string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.SupportedOnlyOn110 : exceptionMessage).SetHelpContext("SupportedOnlyOn110");
		}
	}

	protected void ThrowIfBelowVersion110Prop(string propertyName)
	{
		if (ServerVersion.Major < 11)
		{
			throw new UnknownPropertyException(propertyName, ExceptionTemplatesImpl.NotSupportedForVersionEarlierThan110);
		}
	}

	internal void ThrowIfSourceOrDestBelowVersion110(SqlServerVersionInternal targetVersion, string exceptionMessage = null)
	{
		ThrowIfBelowVersion110(exceptionMessage);
		ThrowIfBelowVersion110(targetVersion, exceptionMessage);
	}

	protected void ThrowIfBelowVersion120(string exceptionMessage = null)
	{
		if (ServerVersion.Major < 12)
		{
			throw new UnsupportedVersionException(string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.SupportedOnlyOn120 : exceptionMessage).SetHelpContext("SupportedOnlyOn120");
		}
	}

	protected void ThrowIfBelowVersion120Prop(string propertyName)
	{
		if (ServerVersion.Major < 12)
		{
			throw new UnknownPropertyException(propertyName, ExceptionTemplatesImpl.NotSupportedForVersionEarlierThan120);
		}
	}

	internal void ThrowIfSourceOrDestBelowVersion120(SqlServerVersionInternal targetVersion, string exceptionMessage = null)
	{
		ThrowIfBelowVersion120(exceptionMessage);
		ThrowIfBelowVersion120(targetVersion, exceptionMessage);
	}

	protected void ThrowIfBelowVersion130(string exceptionMessage = null)
	{
		if (ServerVersion.Major < 13)
		{
			throw new UnsupportedVersionException(string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.SupportedOnlyOn130 : exceptionMessage).SetHelpContext("SupportedOnlyOn130");
		}
	}

	protected void ThrowIfBelowVersion130Prop(string propertyName)
	{
		if (ServerVersion.Major < 13)
		{
			throw new UnknownPropertyException(propertyName, ExceptionTemplatesImpl.NotSupportedForVersionEarlierThan130);
		}
	}

	protected void ThrowIfBelowVersion140(string exceptionMessage = null)
	{
		if (ServerVersion.Major < 14)
		{
			throw new UnsupportedVersionException(string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.SupportedOnlyOn140 : exceptionMessage).SetHelpContext("SupportedOnlyOn140");
		}
	}

	protected void ThrowIfBelowVersion140Prop(string propertyName)
	{
		if (ServerVersion.Major < 14)
		{
			throw new UnknownPropertyException(propertyName, ExceptionTemplatesImpl.NotSupportedForVersionEarlierThan140);
		}
	}

	internal void ThrowIfSourceOrDestBelowVersion130(SqlServerVersionInternal targetVersion, string exceptionMessage = null)
	{
		ThrowIfBelowVersion130(exceptionMessage);
		ThrowIfBelowVersion130(targetVersion, exceptionMessage);
	}

	internal void ThrowIfExpressSku(string uft)
	{
		if (IsExpressSku())
		{
			throw new UnsupportedFeatureException(ExceptionTemplatesImpl.UnsupportedFeature(uft));
		}
	}

	internal void ThrowIfCloudProp(string propertyName)
	{
		if (DatabaseEngineType.SqlAzureDatabase == DatabaseEngineType)
		{
			throw new UnknownPropertyException(propertyName, ExceptionTemplatesImpl.PropertyNotSupportedOnCloud(propertyName));
		}
	}

	internal void ThrowIfCloud(string exceptionMessage = null)
	{
		if (DatabaseEngineType.SqlAzureDatabase == DatabaseEngineType)
		{
			throw new UnsupportedFeatureException(string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.NotSupportedOnCloud : exceptionMessage);
		}
	}

	internal void ThrowIfCloudAndVersionBelow12(string propertyName)
	{
		if (DatabaseEngineType.SqlAzureDatabase == DatabaseEngineType && ServerVersion.Major < 12)
		{
			throw new UnsupportedFeatureException(ExceptionTemplatesImpl.PropertyNotSupportedForCloudVersion(propertyName, ServerVersion.ToString()));
		}
	}

	public bool IsExpressSku()
	{
		if (IsDesignMode)
		{
			return false;
		}
		Server serverObject = GetServerObject();
		if (serverObject.Information.ServerVersion.Major >= 9)
		{
			return serverObject.Information.Edition.StartsWith("Express Edition", StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	internal static void ThrowIfCloud(DatabaseEngineType targetEngineType)
	{
		ThrowIfCloud(targetEngineType, ExceptionTemplatesImpl.NotSupportedOnCloud);
	}

	internal static void ThrowIfCloud(DatabaseEngineType targetDatabaseEngineType, string exceptionMessage)
	{
		if (targetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			throw new UnsupportedEngineTypeException(exceptionMessage);
		}
	}

	internal static void ThrowIfNotCloud(DatabaseEngineType targetDatabaseEngineType, string exceptionMessage)
	{
		if (targetDatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
		{
			throw new UnsupportedEngineTypeException(exceptionMessage);
		}
	}

	internal static void ThrowIfNotSqlDw(DatabaseEngineEdition targetDatabaseEngineEdition, string exceptionMessage)
	{
		if (targetDatabaseEngineEdition != DatabaseEngineEdition.SqlDataWarehouse)
		{
			throw new UnsupportedEngineEditionException(exceptionMessage);
		}
	}

	internal static void ThrowIfCloudAndBelowVersion120(DatabaseEngineType targetDatabaseEngineType, SqlServerVersionInternal targetVersion, string exceptionMessage)
	{
		if (targetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			ThrowIfBelowVersionLimit(targetVersion, SqlServerVersionInternal.Version120, exceptionMessage);
		}
	}

	internal static void ThrowIfBelowVersion90(SqlServerVersionInternal targetVersion, string exceptionMessage = null)
	{
		ThrowIfBelowVersionLimit(targetVersion, SqlServerVersionInternal.Version90, string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.UnsupportedVersionException : exceptionMessage);
	}

	internal static void ThrowIfBelowVersion100(SqlServerVersionInternal targetVersion, string exceptionMessage = null)
	{
		ThrowIfBelowVersionLimit(targetVersion, SqlServerVersionInternal.Version100, string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.UnsupportedVersionException : exceptionMessage);
	}

	internal static void ThrowIfBelowVersion105(SqlServerVersionInternal targetVersion, string exceptionMessage)
	{
		ThrowIfBelowVersionLimit(targetVersion, SqlServerVersionInternal.Version105, string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.UnsupportedVersionException : exceptionMessage);
	}

	internal static void ThrowIfBelowVersion110(SqlServerVersionInternal targetVersion, string exceptionMessage = null)
	{
		ThrowIfBelowVersionLimit(targetVersion, SqlServerVersionInternal.Version110, string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.UnsupportedVersionException : exceptionMessage);
	}

	internal static void ThrowIfBelowVersion120(SqlServerVersionInternal targetVersion, string exceptionMessage = null)
	{
		ThrowIfBelowVersionLimit(targetVersion, SqlServerVersionInternal.Version120, string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.UnsupportedVersionException : exceptionMessage);
	}

	internal static void ThrowIfBelowVersion130(SqlServerVersionInternal targetVersion, string exceptionMessage = null)
	{
		ThrowIfBelowVersionLimit(targetVersion, SqlServerVersionInternal.Version130, string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.UnsupportedVersionException : exceptionMessage);
	}

	internal static void ThrowIfBelowVersion80(SqlServerVersionInternal targetVersion, string exceptionMessage = null)
	{
		ThrowIfBelowVersionLimit(targetVersion, SqlServerVersionInternal.Version80, string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.UnsupportedVersionException : exceptionMessage);
	}

	private static void ThrowIfBelowVersionLimit(SqlServerVersionInternal targetVersion, SqlServerVersionInternal upperLimit, string exceptionText)
	{
		if (targetVersion < upperLimit)
		{
			throw new UnsupportedVersionException(exceptionText).SetHelpContext("UnsupportedVersionException");
		}
	}

	internal CompatibilityLevel GetCompatibilityLevel()
	{
		Server serverObject = GetServerObject();
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != serverObject, "srv == null");
		string dBName = GetDBName();
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != dBName, "dbName == null");
		if (dBName.Length != 0)
		{
			Database database = serverObject.Databases[dBName];
			if (database != null)
			{
				return database.GetPropValueOptional("CompatibilityLevel", GetCompatibilityLevel(ServerVersion));
			}
		}
		return GetCompatibilityLevel(ServerVersion);
	}

	internal static CompatibilityLevel GetCompatibilityLevel(ServerVersion ver)
	{
		return ver.Major switch
		{
			8 => CompatibilityLevel.Version80, 
			9 => CompatibilityLevel.Version90, 
			10 => CompatibilityLevel.Version100, 
			11 => CompatibilityLevel.Version110, 
			12 => CompatibilityLevel.Version120, 
			13 => CompatibilityLevel.Version130, 
			14 => CompatibilityLevel.Version140, 
			_ => CompatibilityLevel.Version150, 
		};
	}

	internal void ThrowIfCompatibilityLevelBelow130()
	{
		ThrowIfCompatibilityLevelBelowLimit(GetCompatibilityLevel(), CompatibilityLevel.Version130);
	}

	internal void ThrowIfCompatibilityLevelBelow120()
	{
		ThrowIfCompatibilityLevelBelowLimit(GetCompatibilityLevel(), CompatibilityLevel.Version120);
	}

	internal void ThrowIfCompatibilityLevelBelow100()
	{
		ThrowIfCompatibilityLevelBelowLimit(GetCompatibilityLevel(), CompatibilityLevel.Version100);
	}

	internal void ThrowIfCompatibilityLevelBelow90()
	{
		ThrowIfCompatibilityLevelBelowLimit(GetCompatibilityLevel(), CompatibilityLevel.Version90);
	}

	internal void ThrowIfCompatibilityLevelBelow80()
	{
		ThrowIfCompatibilityLevelBelowLimit(GetCompatibilityLevel(), CompatibilityLevel.Version80);
	}

	private static void ThrowIfCompatibilityLevelBelowLimit(CompatibilityLevel targetCompatLevel, CompatibilityLevel upperLimit)
	{
		if (targetCompatLevel < upperLimit)
		{
			throw new UnsupportedCompatLevelException(ExceptionTemplatesImpl.UnsupportedCompatLevelException((long)targetCompatLevel, (long)upperLimit)).SetHelpContext("UnsupportedCompatLevelException");
		}
	}

	internal static void ThrowIfCreateOrAlterUnsupported(SqlServerVersionInternal targetVersion, string exceptionMessage = null)
	{
		ThrowIfBelowVersionLimit(targetVersion, SqlServerVersionInternal.Version130, string.IsNullOrEmpty(exceptionMessage) ? ExceptionTemplatesImpl.UnsupportedVersionException : exceptionMessage);
	}

	internal static string GetSqlServerName(ScriptingPreferences sp)
	{
		if (sp.TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			return LocalizableResources.EngineCloud;
		}
		return sp.TargetServerVersion switch
		{
			SqlServerVersion.Version80 => LocalizableResources.ServerShiloh, 
			SqlServerVersion.Version90 => LocalizableResources.ServerYukon, 
			SqlServerVersion.Version100 => LocalizableResources.ServerKatmai, 
			SqlServerVersion.Version105 => LocalizableResources.ServerKilimanjaro, 
			SqlServerVersion.Version110 => LocalizableResources.ServerDenali, 
			SqlServerVersion.Version120 => LocalizableResources.ServerSQL14, 
			SqlServerVersion.Version130 => LocalizableResources.ServerSQL15, 
			SqlServerVersion.Version140 => LocalizableResources.ServerSQL2017, 
			SqlServerVersion.Version150 => LocalizableResources.ServerSQLv150, 
			_ => string.Empty, 
		};
	}

	internal static string GetSqlServerName(SqlSmoObject srv)
	{
		if (srv.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			return LocalizableResources.EngineCloud;
		}
		switch (srv.ServerVersion.Major)
		{
		case 7:
			return LocalizableResources.ServerSphinx;
		case 8:
			return LocalizableResources.ServerShiloh;
		case 9:
			return LocalizableResources.ServerYukon;
		case 10:
			if (srv.ServerVersion.Minor == 0)
			{
				return LocalizableResources.ServerKatmai;
			}
			if (srv.ServerVersion.Minor == 50)
			{
				return LocalizableResources.ServerKilimanjaro;
			}
			return string.Empty;
		case 11:
			return LocalizableResources.ServerDenali;
		case 12:
			return LocalizableResources.ServerSQL14;
		case 13:
			return LocalizableResources.ServerSQL15;
		case 14:
			return LocalizableResources.ServerSQL2017;
		case 15:
			return LocalizableResources.ServerSQLv150;
		default:
			return string.Empty;
		}
	}

	internal static bool IsCloudAtSrcOrDest(DatabaseEngineType srcEngineType, DatabaseEngineType destEngineType)
	{
		if (srcEngineType == DatabaseEngineType.SqlAzureDatabase || destEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			return true;
		}
		return false;
	}

	internal static string GetDatabaseEngineName(ScriptingPreferences sp)
	{
		return sp.TargetDatabaseEngineType switch
		{
			DatabaseEngineType.SqlAzureDatabase => LocalizableResources.EngineCloud, 
			DatabaseEngineType.Standalone => LocalizableResources.EngineSingleton, 
			_ => string.Empty, 
		};
	}

	public string GetSqlServerVersionName()
	{
		return GetSqlServerName(this);
	}

	protected string GetFragOptionString(FragmentationOption fragmentationOption)
	{
		string text = null;
		switch (fragmentationOption)
		{
		case FragmentationOption.Fast:
			return "FragmentationFast";
		case FragmentationOption.Sampled:
			if (ServerVersion.Major < 9)
			{
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.InvalidOptionForVersion("EnumFragmentation", fragmentationOption.ToString(), GetSqlServerVersionName()));
			}
			return "FragmentationSampled";
		case FragmentationOption.Detailed:
			if (ServerVersion.Major < 8)
			{
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.InvalidOptionForVersion("EnumFragmentation", fragmentationOption.ToString(), GetSqlServerVersionName()));
			}
			return "FragmentationDetailed";
		default:
			throw new InternalSmoErrorException(ExceptionTemplatesImpl.UnknownEnumeration("FragmentationOption"));
		}
	}

	protected internal string FormatSqlVariant(object sqlVariant)
	{
		if (sqlVariant == null)
		{
			return "NULL";
		}
		Type type = sqlVariant.GetType();
		if (type == typeof(int))
		{
			return ((int)sqlVariant).ToString(SmoApplication.DefaultCulture);
		}
		if (type == typeof(byte))
		{
			return ByteArrayToString(new byte[1] { (byte)sqlVariant });
		}
		if (type == typeof(decimal))
		{
			return ((decimal)sqlVariant).ToString(SmoApplication.DefaultCulture);
		}
		if (type == typeof(string))
		{
			return string.Format(SmoApplication.DefaultCulture, "N'{0}'", new object[1] { SqlString((string)sqlVariant) });
		}
		if (type == typeof(short))
		{
			return ((short)sqlVariant).ToString(SmoApplication.DefaultCulture);
		}
		if (type == typeof(long))
		{
			return ((long)sqlVariant).ToString(SmoApplication.DefaultCulture);
		}
		if (type == typeof(double))
		{
			return ((double)sqlVariant).ToString(SmoApplication.DefaultCulture);
		}
		if (type == typeof(float))
		{
			return ((float)sqlVariant).ToString(SmoApplication.DefaultCulture);
		}
		if (type == typeof(DateTime))
		{
			return string.Format(SmoApplication.DefaultCulture, "N'{0}'", new object[1] { SqlDateString((DateTime)sqlVariant, "yyyy-MM-ddTHH:mm:ss.fff") });
		}
		if (type == typeof(DateTimeOffset))
		{
			return string.Format(SmoApplication.DefaultCulture, "N'{0}'", new object[1] { SqlDateString((DateTimeOffset)sqlVariant) });
		}
		if (type == typeof(SqlDateTime))
		{
			if (((SqlDateTime)sqlVariant).IsNull)
			{
				return "NULL";
			}
			return string.Format(SmoApplication.DefaultCulture, "N'{0}'", new object[1] { SqlDateString(((SqlDateTime)sqlVariant).Value, "yyyy-MM-ddTHH:mm:ss.fff") });
		}
		if (type == typeof(byte[]))
		{
			return ByteArrayToString((byte[])sqlVariant);
		}
		if (type == typeof(SqlBinary))
		{
			if (((SqlBinary)sqlVariant).IsNull)
			{
				return "NULL";
			}
			return ByteArrayToString(((SqlBinary)sqlVariant).Value);
		}
		if (type == typeof(bool))
		{
			return string.Format(SmoApplication.DefaultCulture, ((bool)sqlVariant) ? "1" : "0");
		}
		if (type == typeof(Guid))
		{
			return string.Format(SmoApplication.DefaultCulture, "{0}", new object[1] { MakeSqlString(sqlVariant.ToString()) });
		}
		return sqlVariant.ToString();
	}

	internal static string SqlDateString(DateTime date)
	{
		return SqlDateString(date, "s");
	}

	internal static string SqlDateString(DateTime date, string format)
	{
		return date.ToString(format, SmoApplication.DefaultCulture);
	}

	internal static string SqlDateString(DateTimeOffset date)
	{
		return date.ToString(SmoApplication.DefaultCulture);
	}

	private string ByteArrayToString(byte[] bytes)
	{
		if (bytes.Length == 0)
		{
			return "NULL";
		}
		int num = bytes.Length;
		StringBuilder stringBuilder = new StringBuilder("0x", 2 * (num + 1));
		for (int i = 0; i < num; i++)
		{
			stringBuilder.Append(bytes[i].ToString("X2", SmoApplication.DefaultCulture));
		}
		return stringBuilder.ToString();
	}

	internal void GenerateDataSpaceScript(StringBuilder parentScript, ScriptingPreferences sp)
	{
		if (sp.TargetDatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse || ParentColl.ParentInstance is UserDefinedTableType)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = string.Empty;
		if (GetPropValueOptional("FileGroup") != null)
		{
			text = GetPropValueOptional("FileGroup").ToString();
			if (sp.TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase && text.Length > 0)
			{
				text = "PRIMARY";
			}
		}
		if (!IsSupportedProperty("PartitionScheme", sp))
		{
			if (sp.Storage.FileGroup && text.Length > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " ON [{0}]", new object[1] { SqlBraket(text) });
			}
		}
		else
		{
			string text2 = string.Empty;
			if (Properties.Get("PartitionScheme").Value != null)
			{
				text2 = (string)Properties["PartitionScheme"].Value;
			}
			if (text.Length > 0 && text2.Length > 0)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.MutuallyExclusiveProperties("PartitionScheme", "FileGroup"));
			}
			if (sp.Storage.FileGroup && text.Length > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " ON [{0}]", new object[1] { SqlBraket(text) });
			}
			else if (text2.Length > 0 && ((this is Table && (sp.Storage.PartitionSchemeInternal & PartitioningScheme.Table) == PartitioningScheme.Table) || (this is Index && (sp.Storage.PartitionSchemeInternal & PartitioningScheme.Index) == PartitioningScheme.Index)))
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " ON [{0}]", new object[1] { SqlBraket(text2) });
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "(");
				PartitionSchemeParameterCollection partitionSchemeParameterCollection = null;
				ColumnCollection columnCollection = null;
				if (this is Table)
				{
					partitionSchemeParameterCollection = ((Table)this).PartitionSchemeParameters;
					columnCollection = ((Table)this).Columns;
				}
				else if (this is Index)
				{
					partitionSchemeParameterCollection = ((Index)this).PartitionSchemeParameters;
					columnCollection = ((TableViewBase)((Index)this).Parent).Columns;
				}
				if (partitionSchemeParameterCollection.Count == 0)
				{
					throw new FailedOperationException(ExceptionTemplatesImpl.NeedPSParams);
				}
				int num = 0;
				foreach (PartitionSchemeParameter item in partitionSchemeParameterCollection)
				{
					Column column = columnCollection[item.Name];
					if (column == null && (ParentColl.ParentInstance.State != SqlSmoState.Creating || !(ParentColl.ParentInstance is View)))
					{
						throw new SmoException(ExceptionTemplatesImpl.ObjectRefsNonexCol(PartitionScheme.UrnSuffix, text2, ToString() + ".[" + SqlStringBraket(item.Name) + "]"));
					}
					if (column.IgnoreForScripting)
					{
						IgnoreForScripting = true;
						return;
					}
					if (0 < num++)
					{
						stringBuilder.Append(Globals.commaspace);
					}
					stringBuilder.Append(column.FormatFullNameForScripting(sp));
				}
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ")");
			}
		}
		parentScript.Append(stringBuilder.ToString());
	}

	internal void GenerateDataSpaceFileStreamScript(StringBuilder parentScript, ScriptingPreferences sp, bool alterTable)
	{
		if (!IsSupportedProperty("FileStreamFileGroup", sp))
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = string.Empty;
		Property property = Properties.Get("FileStreamFileGroup");
		if (property.Value != null)
		{
			text = (string)property.Value;
		}
		string text2 = string.Empty;
		Property property2 = Properties.Get("FileStreamPartitionScheme");
		if (property2.Value != null)
		{
			text2 = (string)property2.Value;
		}
		if (text.Length > 0 && text2.Length > 0)
		{
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.MutuallyExclusiveProperties("FileStreamPartitionScheme", "FileStreamFileGroup"));
		}
		if (sp.Storage.FileStreamFileGroup && sp.Storage.FileStreamColumn && text.Length > 0)
		{
			if (!alterTable)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " FILESTREAM_ON [{0}]", new object[1] { SqlBraket(text) });
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " SET (FILESTREAM_ON = [{0}])", new object[1] { SqlBraket(text) });
			}
		}
		else if (sp.Storage.FileStreamColumn && text2.Length > 0 && ((this is Table && (sp.Storage.PartitionSchemeInternal & PartitioningScheme.Table) == PartitioningScheme.Table) || (this is Index && (sp.Storage.PartitionSchemeInternal & PartitioningScheme.Index) == PartitioningScheme.Index)))
		{
			if (!alterTable)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " FILESTREAM_ON [{0}]", new object[1] { SqlBraket(text2) });
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " SET (FILESTREAM_ON = [{0}])", new object[1] { SqlBraket(text2) });
			}
		}
		parentScript.Append(stringBuilder.ToString());
	}

	protected StringCollection ScriptImpl()
	{
		CheckObjectState(throwIfNotCreated: false);
		ScriptingPreferences scriptingPreferences = new ScriptingPreferences(this);
		scriptingPreferences.SfcChildren = false;
		return ScriptImpl(scriptingPreferences);
	}

	internal StringCollection ScriptImpl(ScriptingPreferences sp)
	{
		if (sp.IncludeScripts.Data)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ScriptDataNotSupportedByThisMethod);
		}
		return EnumerableContainer.IEnumerableToStringCollection(EnumScriptImpl(sp));
	}

	protected StringCollection ScriptImpl(ScriptingOptions so)
	{
		if (so == null)
		{
			throw new ArgumentNullException("scriptingOptions");
		}
		if (so.ScriptData)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ScriptDataNotSupportedByThisMethod);
		}
		Scripter scripter = new Scripter(GetServerObject());
		if (!so.GetScriptingPreferences().TargetVersionAndDatabaseEngineTypeDirty)
		{
			so.SetTargetServerInfo(this, forced: false);
		}
		scripter.Options = so;
		return scripter.Script(this);
	}

	internal IEnumerable<string> EnumScriptImpl(ScriptingPreferences sp)
	{
		CheckObjectState(throwIfNotCreated: false);
		try
		{
			return EnumScriptImplWorker(sp);
		}
		catch (PropertyCannotBeRetrievedException innerException)
		{
			FailedOperationException ex = new FailedOperationException(ExceptionTemplatesImpl.FailedOperationExceptionTextScript(GetTypeName(GetType().Name), ToString()), innerException);
			ex.Operation = ExceptionTemplatesImpl.Script;
			ex.FailedObject = this;
			throw ex;
		}
		catch (Exception ex2)
		{
			FilterException(ex2);
			throw new FailedOperationException(ExceptionTemplatesImpl.Script, this, ex2);
		}
	}

	internal StringCollection ScriptImplWorker(ScriptingPreferences sp)
	{
		if (sp.IncludeScripts.Data)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ScriptDataNotSupportedByThisMethod);
		}
		return EnumerableContainer.IEnumerableToStringCollection(EnumScriptImplWorker(sp));
	}

	internal IEnumerable<string> EnumScriptImplWorker(ScriptingPreferences sp)
	{
		if (sp == null)
		{
			throw new ArgumentNullException("scriptingPreferences");
		}
		ScriptMaker scriptMaker = new ScriptMaker(GetServerObject());
		if (!sp.DependentObjects)
		{
			scriptMaker.Prefetch = !InitializedForScripting;
		}
		if (!sp.TargetVersionAndDatabaseEngineTypeDirty)
		{
			sp.SetTargetServerInfo(this, forced: false);
		}
		scriptMaker.Preferences = sp;
		EnumerableContainer enumerableContainer = new EnumerableContainer();
		enumerableContainer.Add(scriptMaker.Script(new SqlSmoObject[1] { this }));
		return enumerableContainer;
	}

	protected bool IsVersion80SP3()
	{
		if (ServerVersion.Major != 8 || ServerVersion.BuildNumber < 760)
		{
			return ServerVersion.Major > 8;
		}
		return true;
	}

	protected bool IsVersion90AndAbove()
	{
		return ServerVersion.Major >= 9;
	}

	protected void ThrowIfBelowVersion80SP3()
	{
		if (!IsVersion80SP3())
		{
			throw new SmoException(ExceptionTemplatesImpl.SupportedOnlyOn80SP3);
		}
	}

	internal string GetBindRuleScript(ScriptingPreferences sp, string ruleSchema, string ruleName, bool futureOnly)
	{
		return GetBindScript(sp, ruleSchema, ruleName, futureOnly, forRule: true);
	}

	internal string GetBindDefaultScript(ScriptingPreferences sp, string defSchema, string defName, bool futureOnly)
	{
		return GetBindScript(sp, defSchema, defName, futureOnly, forRule: false);
	}

	private string GetBindScript(ScriptingPreferences sp, string schema, string name, bool futureOnly, bool forRule)
	{
		string empty = string.Empty;
		empty = ((this is Column) ? string.Format(SmoApplication.DefaultCulture, "{0}.{1}", new object[2]
		{
			((ScriptSchemaObjectBase)ParentColl.ParentInstance).FormatFullNameForScripting(sp),
			((Column)this).FormatFullNameForScripting(sp)
		}) : ((this is UserDefinedDataType && sp.TargetServerVersionInternal <= SqlServerVersionInternal.Version80) ? MakeSqlBraket(((UserDefinedDataType)this).GetName(sp)) : ((!(this is ScriptSchemaObjectBase)) ? ((ScriptNameObjectBase)this).FormatFullNameForScripting(sp) : ((ScriptSchemaObjectBase)this).FormatFullNameForScripting(sp))));
		string text = ((sp.TargetServerVersionInternal <= SqlServerVersionInternal.Version80) ? "dbo" : "sys");
		schema = ((schema == null || schema.Length <= 0) ? string.Empty : (MakeSqlBraket(SqlString(schema)) + Globals.Dot));
		if (forRule)
		{
			return string.Format(SmoApplication.DefaultCulture, "EXEC {0}.sp_bindrule @rulename=N'{1}[{2}]', @objname=N'{3}' {4}", text, schema, SqlStringBraket(name), SqlString(empty), futureOnly ? ", @futureonly='futureonly'" : "");
		}
		return string.Format(SmoApplication.DefaultCulture, "EXEC {0}.sp_bindefault @defname=N'{1}[{2}]', @objname=N'{3}' {4}", text, schema, SqlStringBraket(name), SqlString(empty), futureOnly ? ", @futureonly='futureonly'" : "");
	}

	protected void BindRuleImpl(string ruleSchema, string rule, bool bindColumns)
	{
		try
		{
			if (rule == null)
			{
				throw new ArgumentNullException("rule");
			}
			if (rule.Length == 0)
			{
				throw new ArgumentException(ExceptionTemplatesImpl.EmptyInputParam("rule", "string"));
			}
			if (ruleSchema == null)
			{
				throw new ArgumentNullException("ruleSchema");
			}
			if (!IsDesignMode)
			{
				StringCollection stringCollection = new StringCollection();
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlBraket(GetDBName()) }));
				ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
				scriptingPreferences.SetTargetServerInfo(this);
				scriptingPreferences.ForDirectExecution = true;
				stringCollection.Add(GetBindRuleScript(scriptingPreferences, ruleSchema, rule, bindColumns));
				ExecutionManager.ExecuteNonQuery(stringCollection);
			}
			if (!ExecutionManager.Recording)
			{
				Properties.Get("Rule").SetValue(rule);
				Properties.Get("RuleSchema").SetValue(ruleSchema);
			}
		}
		catch (Exception ex)
		{
			FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Bind, this, ex);
		}
	}

	protected void UnbindRuleImpl(bool bindColumns)
	{
		try
		{
			if (!IsDesignMode)
			{
				StringCollection stringCollection = new StringCollection();
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlBraket(GetDBName()) }));
				string empty = string.Empty;
				empty = ((!(this is Column)) ? FullQualifiedName : string.Format(SmoApplication.DefaultCulture, "{0}.{1}", new object[2]
				{
					ParentColl.ParentInstance.FullQualifiedName,
					FullQualifiedName
				}));
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_unbindrule @objname=N'{0}' {1}", new object[2]
				{
					SqlString(empty),
					bindColumns ? ", @futureonly='futureonly'" : ""
				}));
				ExecutionManager.ExecuteNonQuery(stringCollection);
			}
			if (!ExecutionManager.Recording)
			{
				Properties.Get("Rule").SetValue(string.Empty);
				Properties.Get("RuleSchema").SetValue(string.Empty);
			}
		}
		catch (Exception ex)
		{
			FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Unbind, this, ex);
		}
	}

	protected void BindDefaultImpl(string defaultSchema, string defaultName, bool bindColumns)
	{
		try
		{
			if (defaultName == null)
			{
				throw new ArgumentNullException("defaultName");
			}
			if (defaultName.Length == 0)
			{
				throw new ArgumentException(ExceptionTemplatesImpl.EmptyInputParam("defaultName", "string"));
			}
			if (defaultSchema == null)
			{
				throw new ArgumentNullException("defaultSchema");
			}
			if (!IsDesignMode)
			{
				StringCollection stringCollection = new StringCollection();
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlBraket(GetDBName()) }));
				ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
				scriptingPreferences.SetTargetServerInfo(this);
				scriptingPreferences.ForDirectExecution = true;
				stringCollection.Add(GetBindDefaultScript(scriptingPreferences, defaultSchema, defaultName, bindColumns));
				ExecutionManager.ExecuteNonQuery(stringCollection);
			}
			if (!ExecutionManager.Recording)
			{
				Properties.Get("Default").SetValue(defaultName);
				Properties.Get("DefaultSchema").SetValue(defaultSchema);
			}
		}
		catch (Exception ex)
		{
			FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Bind, this, ex);
		}
	}

	protected void UnbindDefaultImpl(bool bindColumns)
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			if (!IsDesignMode)
			{
				StringCollection stringCollection = new StringCollection();
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlBraket(GetDBName()) }));
				string empty = string.Empty;
				empty = ((!(this is Column)) ? FullQualifiedName : string.Format(SmoApplication.DefaultCulture, "{0}.{1}", new object[2]
				{
					ParentColl.ParentInstance.FullQualifiedName,
					FullQualifiedName
				}));
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_unbindefault @objname=N'{0}' {1}", new object[2]
				{
					SqlString(empty),
					bindColumns ? ", @futureonly='futureonly'" : ""
				}));
				ExecutionManager.ExecuteNonQuery(stringCollection);
			}
			if (!ExecutionManager.Recording)
			{
				Properties.Get("Default").SetValue(string.Empty);
				Properties.Get("DefaultSchema").SetValue(string.Empty);
			}
		}
		catch (Exception ex)
		{
			FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Unbind, this, ex);
		}
	}

	internal void CheckCollation(string collationName, ScriptingPreferences sp)
	{
		if (!IsDesignMode)
		{
			CollationVersion collationVersion = GetServerObject().GetCollationVersion(collationName);
			CollationVersion collationVersion2 = CollationVersion.Version150;
			switch (sp.TargetServerVersionInternal)
			{
			case SqlServerVersionInternal.Version80:
				collationVersion2 = CollationVersion.Version80;
				break;
			case SqlServerVersionInternal.Version90:
				collationVersion2 = CollationVersion.Version90;
				break;
			case SqlServerVersionInternal.Version100:
				collationVersion2 = CollationVersion.Version100;
				break;
			case SqlServerVersionInternal.Version105:
				collationVersion2 = CollationVersion.Version105;
				break;
			case SqlServerVersionInternal.Version110:
				collationVersion2 = CollationVersion.Version110;
				break;
			case SqlServerVersionInternal.Version120:
				collationVersion2 = CollationVersion.Version120;
				break;
			case SqlServerVersionInternal.Version130:
				collationVersion2 = CollationVersion.Version130;
				break;
			case SqlServerVersionInternal.Version140:
				collationVersion2 = CollationVersion.Version140;
				break;
			}
			if (collationVersion > collationVersion2)
			{
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.UnsupportedCollation(collationName, GetSqlServerName(sp)));
			}
		}
	}

	private void AddNewLineFormat(StringBuilder buffer)
	{
		buffer.Append(Globals.commaspace);
		buffer.Append(Globals.newline);
		buffer.Append(Globals.tab);
		buffer.Append(Globals.tab);
	}

	internal void GetBoolParameter(StringBuilder buffer, ScriptingPreferences sp, string propName, string sqlPropScript, ref int count)
	{
		GetBoolParameter(buffer, sp, propName, sqlPropScript, ref count, valueAsTrueFalse: false);
	}

	internal void GetBoolParameter(StringBuilder buffer, ScriptingPreferences sp, string propName, string sqlPropScript, ref int count, bool valueAsTrueFalse)
	{
		Property property = Properties.Get(propName);
		if (property.Value != null && (!sp.ScriptForAlter || property.Dirty))
		{
			if (count++ > 0)
			{
				AddNewLineFormat(buffer);
			}
			string text = ((!valueAsTrueFalse) ? (((bool)property.Value) ? "1" : "0") : property.Value.ToString().ToLower(SmoApplication.DefaultCulture));
			buffer.AppendFormat(SmoApplication.DefaultCulture, sqlPropScript, new object[1] { text });
		}
	}

	internal void GetEnumParameter(StringBuilder buffer, ScriptingPreferences sp, string propName, string sqlPropScript, Type enumtype, ref int count)
	{
		Property property = Properties.Get(propName);
		if (property.Value != null && (!sp.ScriptForAlter || property.Dirty))
		{
			if (count++ > 0)
			{
				AddNewLineFormat(buffer);
			}
			buffer.AppendFormat(SmoApplication.DefaultCulture, sqlPropScript, new object[1] { Enum.Format(enumtype, property.Value, "d") });
		}
	}

	internal bool GetDateTimeParameterAsInt(StringBuilder buffer, ScriptingPreferences sp, string propName, string sqlPropScript, ref int count)
	{
		Property property = Properties.Get(propName);
		if (property.Value != null && (!sp.ScriptForAlter || property.Dirty))
		{
			if (count++ > 0)
			{
				AddNewLineFormat(buffer);
			}
			DateTime dateTime = (DateTime)property.Value;
			int num = dateTime.Year * 10000 + dateTime.Month * 100 + dateTime.Day;
			buffer.AppendFormat(SmoApplication.DefaultCulture, sqlPropScript, new object[1] { SqlString(num.ToString(SmoApplication.DefaultCulture)) });
			return true;
		}
		return false;
	}

	internal bool GetTimeSpanParameterAsInt(StringBuilder buffer, ScriptingPreferences sp, string propName, string sqlPropScript, ref int count)
	{
		Property property = Properties.Get(propName);
		if (property.Value != null && (!sp.ScriptForAlter || property.Dirty))
		{
			if (count++ > 0)
			{
				AddNewLineFormat(buffer);
			}
			TimeSpan timeSpan = (TimeSpan)property.Value;
			int num = timeSpan.Hours * 10000 + timeSpan.Minutes * 100 + timeSpan.Seconds;
			buffer.AppendFormat(SmoApplication.DefaultCulture, sqlPropScript, new object[1] { SqlString(num.ToString(SmoApplication.DefaultCulture)) });
			return true;
		}
		return false;
	}

	internal void GetDateTimeParameter(StringBuilder buffer, ScriptingPreferences sp, string propName, string sqlPropScript, ref int count)
	{
		Property property = Properties.Get(propName);
		if (property.Value != null && (!sp.ScriptForAlter || property.Dirty))
		{
			if (count++ > 0)
			{
				AddNewLineFormat(buffer);
			}
			DateTime dateTime = (DateTime)property.Value;
			int num;
			int num2;
			if (dateTime == DateTime.MinValue)
			{
				num = 0;
				num2 = 0;
			}
			else
			{
				num = dateTime.Year * 10000 + dateTime.Month * 100 + dateTime.Day;
				num2 = dateTime.Hour * 10000 + dateTime.Minute * 100 + dateTime.Second;
			}
			buffer.AppendFormat(SmoApplication.DefaultCulture, sqlPropScript, new object[2] { "date", num });
			buffer.Append(Globals.commaspace);
			buffer.AppendFormat(SmoApplication.DefaultCulture, sqlPropScript, new object[2] { "time", num2 });
		}
	}

	internal bool GetGuidParameter(StringBuilder buffer, ScriptingPreferences sp, string propName, string sqlPropScript, ref int count)
	{
		Property property = Properties.Get(propName);
		if (property.Value != null && (!sp.ScriptForAlter || property.Dirty))
		{
			if (count++ > 0)
			{
				AddNewLineFormat(buffer);
			}
			Guid guid = (Guid)property.Value;
			buffer.AppendFormat(SmoApplication.DefaultCulture, sqlPropScript, new object[1] { SqlString(guid.ToString()) });
			return true;
		}
		return false;
	}

	internal bool GetStringParameter(StringBuilder buffer, ScriptingPreferences sp, string propName, string sqlPropScript, ref int count)
	{
		return GetStringParameter(buffer, sp, propName, sqlPropScript, ref count, throwIfNotSet: false);
	}

	internal bool GetStringParameter(StringBuilder buffer, ScriptingPreferences sp, string propName, string sqlPropScript, ref int count, bool throwIfNotSet)
	{
		Property propertyOptional = GetPropertyOptional(propName);
		if (throwIfNotSet && propertyOptional.Value == null)
		{
			throw new PropertyNotSetException(propName);
		}
		if (propertyOptional.Value != null && (!sp.ScriptForAlter || propertyOptional.Dirty) && (sp.ScriptForAlter || ((string)propertyOptional.Value).Length > 0))
		{
			if (count++ > 0)
			{
				AddNewLineFormat(buffer);
			}
			buffer.AppendFormat(SmoApplication.DefaultCulture, sqlPropScript, new object[1] { SqlString((string)propertyOptional.Value) });
			return true;
		}
		return false;
	}

	internal void GetParameter(StringBuilder buffer, ScriptingPreferences sp, string propName, string sqlPropScript, ref int count)
	{
		GetParameter(buffer, sp, propName, sqlPropScript, ref count, throwIfNotSet: false);
	}

	internal void GetParameter(StringBuilder buffer, ScriptingPreferences sp, string propName, string sqlPropScript, ref int count, bool throwIfNotSet)
	{
		Property property = Properties.Get(propName);
		if (throwIfNotSet && property.Value == null)
		{
			throw new PropertyNotSetException(propName);
		}
		if (property.Value != null && (!sp.ScriptForAlter || property.Dirty))
		{
			if (count++ > 0)
			{
				AddNewLineFormat(buffer);
			}
			buffer.AppendFormat(SmoApplication.DefaultCulture, sqlPropScript, new object[1] { property.Value });
		}
	}

	internal void CheckPendingState()
	{
		if (IsDesignMode || base.State != SqlSmoState.Pending)
		{
			return;
		}
		if (parentColl == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.NeedToSetParent);
		}
		StringCollection fieldNames = GetEmptyKey().GetFieldNames();
		if (fieldNames.Count == 1)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.OperationNotInPendingState1(fieldNames[0]));
		}
		if (fieldNames.Count == 2)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.OperationNotInPendingState2(fieldNames[0], fieldNames[1]));
		}
		if (fieldNames.Count == 3)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.OperationNotInPendingState3(fieldNames[0], fieldNames[1], fieldNames[2]));
		}
		throw new FailedOperationException(ExceptionTemplatesImpl.OperationNotInPendingState);
	}

	internal DataType GetDataType(ref DataType dataType)
	{
		try
		{
			CheckPendingState();
			if (dataType == null)
			{
				dataType = new DataType();
				dataType.Parent = this;
				dataType.ReadFromPropBag(this);
			}
			return dataType;
		}
		catch (Exception ex)
		{
			FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.GetDataType, this, ex);
		}
	}

	internal void SetDataType(ref DataType targetDataType, DataType sourceDataType)
	{
		try
		{
			CheckPendingState();
			if (sourceDataType == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("DataType"));
			}
			targetDataType = sourceDataType.Clone();
			targetDataType.Parent = this;
			WriteToPropBag(targetDataType);
		}
		catch (Exception ex)
		{
			FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SetDataType, this, ex);
		}
	}

	internal void WriteToPropBag(DataType dataType)
	{
		switch (dataType.SqlDataType)
		{
		case SqlDataType.BigInt:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.BigInt);
			break;
		case SqlDataType.Binary:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.Binary);
			Properties.Get("Length").Value = dataType.MaximumLength;
			break;
		case SqlDataType.Bit:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.Bit);
			break;
		case SqlDataType.Char:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.Char);
			Properties.Get("Length").Value = dataType.MaximumLength;
			break;
		case SqlDataType.DateTime:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.DateTime);
			break;
		case SqlDataType.Decimal:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.Decimal);
			Properties.Get("NumericPrecision").Value = dataType.NumericPrecision;
			Properties.Get("NumericScale").Value = dataType.NumericScale;
			break;
		case SqlDataType.Numeric:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.Numeric);
			Properties.Get("NumericPrecision").Value = dataType.NumericPrecision;
			Properties.Get("NumericScale").Value = dataType.NumericScale;
			break;
		case SqlDataType.Float:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.Float);
			Properties.Get("NumericPrecision").Value = dataType.NumericPrecision;
			break;
		case SqlDataType.Geography:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.Geography);
			break;
		case SqlDataType.Geometry:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.Geometry);
			break;
		case SqlDataType.Image:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.Image);
			Properties.Get("Length").Value = dataType.MaximumLength;
			break;
		case SqlDataType.Int:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.Int);
			break;
		case SqlDataType.Money:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.Money);
			break;
		case SqlDataType.NChar:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.NChar);
			Properties.Get("Length").Value = dataType.MaximumLength;
			break;
		case SqlDataType.NText:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.NText);
			Properties.Get("Length").Value = dataType.MaximumLength;
			break;
		case SqlDataType.NVarChar:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.NVarChar);
			Properties.Get("Length").Value = dataType.MaximumLength;
			break;
		case SqlDataType.NVarCharMax:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.NVarCharMax);
			Properties.Get("Length").Value = -1;
			break;
		case SqlDataType.Real:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.Real);
			break;
		case SqlDataType.SmallDateTime:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.SmallDateTime);
			break;
		case SqlDataType.SmallInt:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.SmallInt);
			break;
		case SqlDataType.SmallMoney:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.SmallMoney);
			break;
		case SqlDataType.Text:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.Text);
			Properties.Get("Length").Value = dataType.MaximumLength;
			break;
		case SqlDataType.Timestamp:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.Timestamp);
			break;
		case SqlDataType.TinyInt:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.TinyInt);
			break;
		case SqlDataType.UniqueIdentifier:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.UniqueIdentifier);
			break;
		case SqlDataType.UserDefinedDataType:
			Properties.Get("DataType").Value = dataType.Name;
			if (dataType.Schema.Length > 0)
			{
				Properties.Get("DataTypeSchema").Value = dataType.Schema;
			}
			break;
		case SqlDataType.UserDefinedTableType:
			Properties.Get("DataType").Value = dataType.Name;
			if (dataType.Schema.Length > 0)
			{
				Properties.Get("DataTypeSchema").Value = dataType.Schema;
			}
			break;
		case SqlDataType.UserDefinedType:
			Properties.Get("DataType").Value = dataType.Name;
			if (dataType.Schema.Length > 0)
			{
				Properties.Get("DataTypeSchema").Value = dataType.Schema;
			}
			break;
		case SqlDataType.VarBinary:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.VarBinary);
			Properties.Get("Length").Value = dataType.MaximumLength;
			break;
		case SqlDataType.VarBinaryMax:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.VarBinaryMax);
			Properties.Get("Length").Value = -1;
			break;
		case SqlDataType.VarChar:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.VarChar);
			Properties.Get("Length").Value = dataType.MaximumLength;
			break;
		case SqlDataType.VarCharMax:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.VarCharMax);
			Properties.Get("Length").Value = -1;
			break;
		case SqlDataType.Variant:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.Variant);
			break;
		case SqlDataType.Xml:
			Properties.Get("DataType").Value = "xml";
			if (dataType.Name.Length > 0)
			{
				Properties.Get("XmlSchemaNamespace").Value = dataType.Name;
			}
			if (dataType.Schema.Length > 0)
			{
				Properties.Get("XmlSchemaNamespaceSchema").Value = dataType.Schema;
			}
			Properties.Get("XmlDocumentConstraint").Value = dataType.XmlDocumentConstraint;
			break;
		case SqlDataType.SysName:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.SysName);
			break;
		case SqlDataType.Date:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.Date);
			break;
		case SqlDataType.Time:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.Time);
			Properties.Get("NumericScale").Value = dataType.NumericScale;
			break;
		case SqlDataType.DateTimeOffset:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.DateTimeOffset);
			Properties.Get("NumericScale").Value = dataType.NumericScale;
			break;
		case SqlDataType.DateTime2:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.DateTime2);
			Properties.Get("NumericScale").Value = dataType.NumericScale;
			break;
		case SqlDataType.HierarchyId:
			Properties.Get("DataType").Value = dataType.GetSqlName(SqlDataType.HierarchyId);
			break;
		case (SqlDataType)5:
		case (SqlDataType)26:
		case (SqlDataType)27:
			break;
		}
	}

	internal static string GetTypeName(string typeName)
	{
		return typeName switch
		{
			"Table" => ExceptionTemplatesImpl.Table, 
			"View" => ExceptionTemplatesImpl.View, 
			"Server" => ExceptionTemplatesImpl.Server, 
			"Database" => ExceptionTemplatesImpl.Database, 
			"ExtendedProperty" => ExceptionTemplatesImpl.ExtendedProperty, 
			"DatabaseOptions" => ExceptionTemplatesImpl.DatabaseOptions, 
			"Synonym" => ExceptionTemplatesImpl.Synonym, 
			"Sequence" => ExceptionTemplatesImpl.Sequence, 
			"FullTextIndex" => ExceptionTemplatesImpl.FullTextIndex, 
			"FullTextIndexColumn" => ExceptionTemplatesImpl.FullTextIndexColumn, 
			"Check" => ExceptionTemplatesImpl.Check, 
			"ForeignKey" => ExceptionTemplatesImpl.ForeignKey, 
			"ForeignKeyColumn" => ExceptionTemplatesImpl.ForeignKeyColumn, 
			"PartitionSchemeParameter" => ExceptionTemplatesImpl.PartitionSchemeParameter, 
			"PlanGuide" => ExceptionTemplatesImpl.PlanGuide, 
			"Trigger" => ExceptionTemplatesImpl.Trigger, 
			"Index" => ExceptionTemplatesImpl.Index, 
			"BrokerPriority" => ExceptionTemplatesImpl.BrokerPriority, 
			"IndexedColumn" => ExceptionTemplatesImpl.Index, 
			"Statistic" => ExceptionTemplatesImpl.Statistic, 
			"StatisticColumn" => ExceptionTemplatesImpl.StatisticColumn, 
			"Column" => ExceptionTemplatesImpl.Column, 
			"DefaultConstraint" => ExceptionTemplatesImpl.DefaultConstraint, 
			"StoredProcedure" => ExceptionTemplatesImpl.StoredProcedure, 
			"StoredProcedureParameter" => ExceptionTemplatesImpl.StoredProcedureParameter, 
			"SqlAssembly" => ExceptionTemplatesImpl.SqlAssembly, 
			"SqlAssemblyFile" => ExceptionTemplatesImpl.SqlAssemblyFile, 
			"UserDefinedType" => ExceptionTemplatesImpl.UserDefinedType, 
			"UserDefinedAggregate" => ExceptionTemplatesImpl.UserDefinedAggregate, 
			"UserDefinedAggregateParameter" => ExceptionTemplatesImpl.UserDefinedAggregateParameter, 
			"FullTextCatalog" => ExceptionTemplatesImpl.FullTextCatalog, 
			"FullTextStopList" => ExceptionTemplatesImpl.FullTextStopList, 
			"SearchPropertyList" => ExceptionTemplatesImpl.SearchPropertyList, 
			"ExtendedStoredProcedure" => ExceptionTemplatesImpl.ExtendedStoredProcedure, 
			"UserDefinedFunction" => ExceptionTemplatesImpl.UserDefinedFunction, 
			"UserDefinedFunctionParameter" => ExceptionTemplatesImpl.UserDefinedFunctionParameter, 
			"User" => ExceptionTemplatesImpl.User, 
			"Schema" => ExceptionTemplatesImpl.Schema, 
			"DatabaseRole" => ExceptionTemplatesImpl.DatabaseRole, 
			"ApplicationRole" => ExceptionTemplatesImpl.ApplicationRole, 
			"LogFile" => ExceptionTemplatesImpl.LogFile, 
			"FileGroup" => ExceptionTemplatesImpl.FileGroup, 
			"DataFile" => ExceptionTemplatesImpl.DataFile, 
			"Default" => ExceptionTemplatesImpl.Default, 
			"Rule" => ExceptionTemplatesImpl.Rule, 
			"UserDefinedDataType" => ExceptionTemplatesImpl.UserDefinedDataType, 
			"UserDefinedTableType" => ExceptionTemplatesImpl.UserDefinedTableType, 
			"XmlSchemaCollection" => ExceptionTemplatesImpl.XmlSchemaCollection, 
			"PartitionFunction" => ExceptionTemplatesImpl.PartitionFunction, 
			"PartitionScheme" => ExceptionTemplatesImpl.PartitionScheme, 
			"DatabaseActiveDirectory" => ExceptionTemplatesImpl.DatabaseActiveDirectory, 
			"Language" => ExceptionTemplatesImpl.Language, 
			"Login" => ExceptionTemplatesImpl.Login, 
			"ServerRole" => ExceptionTemplatesImpl.ServerRole, 
			"LinkedServer" => ExceptionTemplatesImpl.LinkedServer, 
			"LinkedServerLogin" => ExceptionTemplatesImpl.LinkedServerLogin, 
			"SystemDataType" => ExceptionTemplatesImpl.SystemDataType, 
			"JobServer" => ExceptionTemplatesImpl.JobServer, 
			"Category" => ExceptionTemplatesImpl.Category, 
			"AlertSystem" => ExceptionTemplatesImpl.AlertSystem, 
			"Alert" => ExceptionTemplatesImpl.Alert, 
			"Operator" => ExceptionTemplatesImpl.Operator, 
			"TargetServer" => ExceptionTemplatesImpl.TargetServer, 
			"TargetServerGroup" => ExceptionTemplatesImpl.TargetServerGroup, 
			"Job" => ExceptionTemplatesImpl.Job, 
			"JobStep" => ExceptionTemplatesImpl.JobStep, 
			"JobSchedule" => ExceptionTemplatesImpl.JobSchedule, 
			"Settings" => ExceptionTemplatesImpl.Settings, 
			"Information" => ExceptionTemplatesImpl.Information, 
			"UserOptions" => ExceptionTemplatesImpl.UserOptions, 
			"BackupDevice" => ExceptionTemplatesImpl.BackupDevice, 
			"FullTextService" => ExceptionTemplatesImpl.FullTextService, 
			"ServerActiveDirectory" => ExceptionTemplatesImpl.ServerActiveDirectory, 
			"HttpEndpoint" => ExceptionTemplatesImpl.HttpEndpoint, 
			"SoapConfiguration" => ExceptionTemplatesImpl.SoapConfiguration, 
			"SoapMethod" => ExceptionTemplatesImpl.SoapMethod, 
			"ServerAlias" => ExceptionTemplatesImpl.ServerAlias, 
			"PhysicalPartition" => ExceptionTemplatesImpl.PhysicalPartition, 
			"Audit" => ExceptionTemplatesImpl.Audit, 
			"ServerAuditSpecification" => ExceptionTemplatesImpl.ServerAuditSpecification, 
			"DatabaseAuditSpecification" => ExceptionTemplatesImpl.DatabaseAuditSpecification, 
			"AvailabilityGroup" => ExceptionTemplatesImpl.AvailabilityGroup, 
			"AvailabilityReplica" => ExceptionTemplatesImpl.AvailabilityReplica, 
			"AvailabilityDatabase" => ExceptionTemplatesImpl.AvailabilityDatabase, 
			"AvailabilityGroupListener" => ExceptionTemplatesImpl.AvailabilityGroupListener, 
			"AvailabilityGroupListenerIPAddress" => ExceptionTemplatesImpl.AvailabilityGroupListenerIPAddress, 
			_ => typeName, 
		};
	}

	ISfcPropertySet ISfcPropertyProvider.GetPropertySet()
	{
		return Properties;
	}

	internal override void OnPropertyMetadataChanged(string propname)
	{
		if (this.PropertyMetadataChanged != null)
		{
			this.PropertyMetadataChanged(this, new SfcPropertyMetadataChangedEventArgs(propname));
		}
	}

	internal override void OnPropertyChanged(string propname)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propname));
		}
	}

	internal bool IsSystemObjectInternal()
	{
		int index = 0;
		bool result = false;
		if (Properties.PropertiesMetadata.TryPropertyNameToIDLookup("IsSystemObject", out index))
		{
			object value = Properties.Get(index).Value;
			if (value != null)
			{
				result = (bool)value;
			}
		}
		return result;
	}

	public List<object> Discover()
	{
		List<object> list = new List<object>();
		list.Add(this);
		Queue<object> queue = new Queue<object>();
		queue.Enqueue(this);
		if (this is Database && !IsDesignMode)
		{
			((Database)this).PrefetchObjects();
		}
		while (queue.Count > 0)
		{
			object obj = queue.Dequeue();
			SfcMetadataDiscovery sfcMetadataDiscovery = new SfcMetadataDiscovery(obj.GetType());
			foreach (SfcMetadataRelation relation in sfcMetadataDiscovery.Relations)
			{
				if ((relation.Relationship != SfcRelationship.ChildContainer && relation.Relationship != SfcRelationship.ObjectContainer && relation.Relationship != SfcRelationship.ChildObject && relation.Relationship != SfcRelationship.Object) || (IsDesignMode && relation.Type.GetInterface(typeof(ISfcSupportsDesignMode).Name) == null))
				{
					continue;
				}
				object obj2 = null;
				try
				{
					PropertyInfo property = obj.GetType().GetProperty(relation.PropertyName);
					obj2 = property.GetValue(obj, null);
				}
				catch (TargetInvocationException)
				{
				}
				if (obj2 == null)
				{
					continue;
				}
				if (relation.Relationship == SfcRelationship.ChildContainer || relation.Relationship == SfcRelationship.ObjectContainer)
				{
					try
					{
						SmoCollectionBase smoCollectionBase = obj2 as SmoCollectionBase;
						foreach (object item in smoCollectionBase)
						{
							if (item != null && item.GetType().Name != "SystemMessage" && !DiscoveryHelper.IsSystemObject(item))
							{
								queue.Enqueue(item);
								list.Add(item);
							}
						}
					}
					catch (EnumeratorException)
					{
						continue;
					}
				}
				else if (relation.Relationship == SfcRelationship.ChildObject || relation.Relationship == SfcRelationship.Object)
				{
					if (DiscoveryHelper.IsSystemObject(obj2))
					{
						continue;
					}
					queue.Enqueue(obj2);
					list.Add(obj2);
				}
				if (IsDesignMode)
				{
					continue;
				}
				foreach (Attribute relationshipAttribute in relation.RelationshipAttributes)
				{
					if (relationshipAttribute is SfcReferenceAttribute)
					{
						SfcReferenceAttribute sfcReferenceAttribute = relationshipAttribute as SfcReferenceAttribute;
						object obj3 = sfcReferenceAttribute.Resolve(this);
						if (obj3 != null)
						{
							queue.Enqueue(obj3);
							list.Add(obj3);
						}
					}
				}
			}
		}
		return list;
	}

	object IAlienObject.Resolve(string urnString)
	{
		SqlSmoObject sqlSmoObject = this;
		Server server = null;
		SqlStoreConnection sqlStoreConnection = null;
		SfcObjectQuery sfcObjectQuery = null;
		object result = null;
		while (true)
		{
			Type type = sqlSmoObject.GetType();
			PropertyInfo property = type.GetProperty("Parent");
			if (property == null)
			{
				break;
			}
			sqlSmoObject = type.InvokeMember("Parent", BindingFlags.GetProperty, null, sqlSmoObject, null) as SqlSmoObject;
		}
		server = sqlSmoObject as Server;
		sqlStoreConnection = new SqlStoreConnection(server.ConnectionContext.SqlConnectionObject);
		if (server == null)
		{
			server = new Server(sqlStoreConnection.ServerConnection);
		}
		sfcObjectQuery = new SfcObjectQuery(server);
		int num = 0;
		foreach (object item in sfcObjectQuery.ExecuteIterator(new SfcQueryExpression(urnString), null, null))
		{
			result = item;
			num++;
		}
		_ = 1;
		return result;
	}

	List<object> IAlienObject.Discover()
	{
		return Discover();
	}

	void IAlienObject.SetPropertyValue(string propertyName, Type propertyType, object value)
	{
		int index = 0;
		if (Properties.PropertiesMetadata.TryPropertyNameToIDLookup(propertyName, out index) && Properties.PropertiesMetadata.GetStaticMetadata(index).PropertyType == propertyType)
		{
			Property property = Properties.Get(index);
			property.SetValue(value);
			property.SetRetrieved(retrieved: true);
			return;
		}
		PropertyInfo property2 = GetType().GetProperty(propertyName);
		if (property2 != null && property2.PropertyType == propertyType)
		{
			if (property2.CanWrite)
			{
				property2.SetValue(this, value, null);
			}
			return;
		}
		throw new FailedOperationException(ExceptionTemplatesImpl.PropertyNotFound(propertyName, propertyType.Name));
	}

	object IAlienObject.GetParent()
	{
		return GetParentObject();
	}

	Urn IAlienObject.GetUrn()
	{
		return Urn;
	}

	object IAlienObject.GetPropertyValue(string propertyName, Type propertyType)
	{
		object obj = null;
		int index = 0;
		if (Properties.PropertiesMetadata.TryPropertyNameToIDLookup(propertyName, out index) && Properties.PropertiesMetadata.GetStaticMetadata(index).PropertyType == propertyType)
		{
			obj = Properties.Get(index).Value;
			if (obj == null && IsSpeciallyLoaded(GetType(), propertyName))
			{
				obj = GetPropertyValueByReflection(propertyName, propertyType);
			}
		}
		else
		{
			obj = GetPropertyValueByReflection(propertyName, propertyType);
		}
		return obj;
	}

	private object GetPropertyValueByReflection(string propertyName, Type propertyType)
	{
		object obj = null;
		PropertyInfo pInfo = null;
		if (!SfcMetadataDiscovery.TryGetCachedPropertyInfo(GetType().TypeHandle, propertyName, out pInfo))
		{
			pInfo = GetType().GetProperty(propertyName);
		}
		if (pInfo != null && pInfo.PropertyType == propertyType)
		{
			return pInfo.GetValue(this, null);
		}
		throw new FailedOperationException(ExceptionTemplatesImpl.PropertyNotFound(propertyName, propertyType.Name));
	}

	private bool IsSpeciallyLoaded(Type t, string propertyName)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(t != null, "Expect non-null type");
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(propertyName != null, "Expect non-null property name");
		string name;
		if ((name = t.Name) != null && name == "Database")
		{
			return DATABASE_SPECIAL_PROPS.Contains(propertyName);
		}
		return System.StringComparer.Ordinal.Compare("IsSystemObject", propertyName) == 0;
	}

	Type IAlienObject.GetPropertyType(string propertyName)
	{
		PropertyInfo property = GetType().GetProperty(propertyName);
		if (property != null)
		{
			return property.PropertyType;
		}
		throw new FailedOperationException("Cannot discover the property " + propertyName);
	}

	void IAlienObject.SetObjectState(SfcObjectState state)
	{
		switch (state)
		{
		case SfcObjectState.Dropped:
			SetState(SqlSmoState.Dropped);
			break;
		case SfcObjectState.Existing:
			SetState(SqlSmoState.Existing);
			break;
		case SfcObjectState.Pending:
			SetState(SqlSmoState.Pending);
			break;
		case SfcObjectState.ToBeDropped:
			SetState(SqlSmoState.ToBeDropped);
			break;
		default:
			throw new FailedOperationException(string.Format(SmoApplication.DefaultCulture, "Object state cannot be set. Cannot find an equivalent state for '{0}' in SMO", new object[1] { state }));
		}
	}

	ISfcDomainLite IAlienObject.GetDomainRoot()
	{
		return GetServerObject();
	}

	internal void AddLoginToRole(string roleName, string loginName)
	{
		if (roleToLoginCache.ContainsKey(roleName))
		{
			if (!roleToLoginCache[roleName].Contains(loginName))
			{
				roleToLoginCache[roleName].Add(loginName);
			}
		}
		else
		{
			roleToLoginCache.Add(roleName, new StringCollection());
			roleToLoginCache[roleName].Add(loginName);
		}
	}

	internal void RemoveLoginFromRole(string roleName, string loginName)
	{
		if (roleToLoginCache.ContainsKey(roleName))
		{
			if (roleToLoginCache[roleName].Contains(loginName))
			{
				roleToLoginCache[roleName].Remove(loginName);
				return;
			}
			throw new MissingObjectException(ExceptionTemplatesImpl.ObjectDoesNotExist("Login", loginName));
		}
		throw new MissingObjectException(ExceptionTemplatesImpl.ObjectDoesNotExist("Role", roleName));
	}

	internal StringCollection EnumLoginsForRole(string roleName)
	{
		if (roleToLoginCache.ContainsKey(roleName))
		{
			return roleToLoginCache[roleName];
		}
		return new StringCollection();
	}

	internal StringCollection EnumRolesForLogin(string loginName)
	{
		StringCollection stringCollection = new StringCollection();
		foreach (KeyValuePair<string, StringCollection> item in roleToLoginCache)
		{
			if (item.Value.Contains(loginName))
			{
				stringCollection.Add(item.Key);
			}
		}
		return stringCollection;
	}

	internal void DoCustomAction(string script, string toplevelExceptionMessage)
	{
		try
		{
			ExecutionManager.ExecuteNonQuery(script);
			Properties.SetAllRetrieved(val: false);
			SetState(PropertyBagState.Empty);
			GenerateAlterEvent();
		}
		catch (Exception ex)
		{
			FilterException(ex);
			throw new FailedOperationException(toplevelExceptionMessage, ex);
		}
	}

	public void ExecuteWithModes(SqlExecutionModes modes, Action action)
	{
		SqlExecutionModes sqlExecutionModes = ExecutionManager.ConnectionContext.SqlExecutionModes;
		ExecutionManager.ConnectionContext.SqlExecutionModes = modes;
		try
		{
			action();
		}
		finally
		{
			ExecutionManager.ConnectionContext.SqlExecutionModes = sqlExecutionModes;
		}
	}

	static SqlSmoObject()
	{
		SqlSmoObject.PropertyMissing = delegate
		{
		};
		s_SingletonTypeToProperty = new Dictionary<Type, string>(20);
		s_TypeToKeyFields = new Dictionary<Type, string[]>(150);
		DATABASE_SPECIAL_PROPS = new List<string>(new string[6] { "CompatibilityLevel", "Collation", "AnsiPaddingEnabled", "DatabaseEncryptionKey", "IsSystemObject", "DefaultSchema" });
	}
}
