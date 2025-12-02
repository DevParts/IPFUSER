using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public abstract class SfcInstance : ISfcDiscoverObject, ISfcPropertyProvider, ISfcNotifyPropertyMetadataChanged, INotifyPropertyChanged
{
	internal class PopulatorFromDataReader : IPropertyCollectionPopulator
	{
		private IDataReader m_reader;

		public PopulatorFromDataReader(IDataReader reader)
		{
			m_reader = reader;
		}

		void IPropertyCollectionPopulator.Populate(SfcPropertyCollection properties)
		{
			FillPropertyCollectionFromDataReader(properties, m_reader);
		}
	}

	private SfcObjectState m_state = SfcObjectState.Pending;

	private ISfcPropertyStorageProvider propertiesStorage;

	private SfcKey m_key;

	private SfcKeyChain m_keychain;

	private SfcInstance m_parent;

	private SfcPropertyCollection m_properties;

	public Urn Urn
	{
		get
		{
			if (!(null == KeyChain))
			{
				return KeyChain.Urn;
			}
			return null;
		}
	}

	protected internal SfcObjectState State
	{
		get
		{
			return m_state;
		}
		internal set
		{
			m_state = value;
		}
	}

	[XmlIgnore]
	public SfcPropertyCollection Properties
	{
		get
		{
			if (m_properties == null)
			{
				m_properties = new SfcPropertyCollection(new PropertyDataDispatcher(this));
			}
			return m_properties;
		}
	}

	[XmlIgnore]
	public virtual SfcMetadataDiscovery Metadata => new SfcMetadataDiscovery(GetType());

	protected virtual ISfcPropertyStorageProvider PropertyStorageProvider
	{
		get
		{
			if (propertiesStorage == null)
			{
				propertiesStorage = new SfcDefaultStorage(this);
			}
			return propertiesStorage;
		}
	}

	protected internal SfcKey AbstractIdentityKey
	{
		get
		{
			if (m_key == null)
			{
				m_key = CreateIdentityKey();
				if (m_key == null)
				{
					return null;
				}
			}
			return m_key;
		}
		set
		{
			m_key = value;
		}
	}

	[XmlIgnore]
	public SfcKeyChain KeyChain
	{
		get
		{
			if (m_keychain == null)
			{
				if (AbstractIdentityKey == null)
				{
					return null;
				}
				if (this is ISfcDomain)
				{
					TraceHelper.Assert(m_parent == null);
					if (!(AbstractIdentityKey is DomainRootKey topKey))
					{
						return null;
					}
					m_keychain = new SfcKeyChain(topKey);
				}
				else if (m_parent != null && m_parent.KeyChain != null)
				{
					m_keychain = new SfcKeyChain(AbstractIdentityKey, m_parent.KeyChain);
				}
			}
			return m_keychain;
		}
		internal set
		{
			TraceHelper.Assert(value != null);
			if (m_parent != null && value.Parent.GetObject() != Parent)
			{
				throw new SfcInvalidKeyChainException();
			}
			if (m_key != null)
			{
				throw new InvalidOperationException(SfcStrings.KeyAlreadySet);
			}
			m_keychain = value;
		}
	}

	[XmlIgnore]
	public SfcInstance Parent
	{
		get
		{
			if (m_parent == null)
			{
				if (KeyChain == null)
				{
					return null;
				}
				SfcKeyChain parent = KeyChain.Parent;
				if (parent != null)
				{
					m_parent = parent.GetObject();
				}
			}
			return m_parent;
		}
		protected internal set
		{
			if (m_keychain != null)
			{
				if (State != SfcObjectState.Pending)
				{
					if (m_keychain.Parent != value.KeyChain)
					{
						throw new InvalidOperationException(SfcStrings.KeyChainAlreadySet);
					}
				}
				else
				{
					m_keychain.Parent = value.KeyChain;
				}
			}
			m_parent = value;
		}
	}

	[CLSCompliant(false)]
	protected event EventHandler<SfcPropertyMetadataChangedEventArgs> propertyMetadataChanged;

	[CLSCompliant(false)]
	protected event PropertyChangedEventHandler propertyChanged;

	public event EventHandler<SfcPropertyMetadataChangedEventArgs> PropertyMetadataChanged
	{
		add
		{
			propertyMetadataChanged += value;
			InitializeUIPropertyState();
		}
		remove
		{
			propertyMetadataChanged -= value;
		}
	}

	public event PropertyChangedEventHandler PropertyChanged
	{
		add
		{
			propertyChanged += value;
		}
		remove
		{
			propertyChanged -= value;
		}
	}

	internal void Initialize()
	{
		if (GetConnectionContextMode() == SfcConnectionContextMode.Offline)
		{
			return;
		}
		ResultType[] supportedResultTypes = GetSupportedResultTypes(GetConnectionContext(), Urn);
		bool flag = false;
		ResultType[] array = supportedResultTypes;
		foreach (ResultType resultType in array)
		{
			if (resultType == ResultType.IDataReader)
			{
				flag = true;
			}
		}
		if (Properties.Count == 0)
		{
			return;
		}
		if (flag)
		{
			using (IDataReader dataReader = GetInitDataReader(GetConnectionContext(), Urn, null, null))
			{
				if (!dataReader.Read())
				{
					dataReader.Close();
					throw new SfcObjectInitializationException(Urn);
				}
				FillPropertyCollectionFromDataReader(Properties, dataReader);
				return;
			}
		}
		throw new SfcObjectInitializationException(Urn);
	}

	public virtual void Refresh()
	{
		if (State == SfcObjectState.Recreate)
		{
			State = SfcObjectState.Existing;
		}
		CheckObjectCreated();
		Initialize();
	}

	public override string ToString()
	{
		return AbstractIdentityKey.ToString();
	}

	public void Serialize(XmlWriter writer)
	{
		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}
		SfcSerializer sfcSerializer = new SfcSerializer();
		sfcSerializer.Serialize(this);
		sfcSerializer.Write(writer);
	}

	public virtual ISfcPropertySet GetPropertySet()
	{
		return Properties;
	}

	internal void InternalOnPropertyValueChanges(PropertyChangedEventArgs args)
	{
		if (this.propertyChanged != null || Properties.DynamicMetaDataEnabled)
		{
			UpdateUIPropertyState();
			OnPropertyValueChanges(args);
		}
	}

	internal void InternalOnPropertyMetadataChanges(SfcPropertyMetadataChangedEventArgs args)
	{
		if (this.propertyMetadataChanged != null)
		{
			OnPropertyMetadataChanges(args);
		}
	}

	protected internal virtual void OnPropertyValueChanges(PropertyChangedEventArgs args)
	{
		if (this.propertyChanged != null)
		{
			this.propertyChanged(this, args);
		}
	}

	protected internal virtual void OnPropertyMetadataChanges(SfcPropertyMetadataChangedEventArgs args)
	{
		if (this.propertyMetadataChanged != null)
		{
			this.propertyMetadataChanged(this, args);
		}
	}

	protected internal abstract ISfcCollection GetChildCollection(string elementType);

	internal object GetPropertyValueImpl(string propertyName)
	{
		return PropertyStorageProvider.GetPropertyValueImpl(propertyName);
	}

	internal void SetPropertyValueImpl(string propertyName, object value)
	{
		PropertyStorageProvider.SetPropertyValueImpl(propertyName, value);
	}

	protected internal abstract SfcKey CreateIdentityKey();

	protected virtual ValidationState Validate()
	{
		ValidationState validationState = new ValidationState();
		foreach (SfcProperty property in Properties)
		{
			if (property.Required)
			{
				Exception error = null;
				object obj = null;
				string text = null;
				obj = property.Value;
				if (obj == null || obj == DBNull.Value)
				{
					text = SfcStrings.PropertyNotSet(property.Name);
					validationState.AddError(text, error, property.Name);
				}
			}
		}
		return validationState;
	}

	protected internal virtual void InitializeUIPropertyState()
	{
	}

	protected virtual void UpdateUIPropertyState()
	{
	}

	protected internal void ResetKey()
	{
		m_key = CreateIdentityKey();
		m_keychain = null;
	}

	private ISfcConnection GetConnectionContext()
	{
		return KeyChain.Domain.GetConnection();
	}

	private SfcConnectionContextMode GetConnectionContextMode()
	{
		return KeyChain.Domain.ConnectionContext.Mode;
	}

	private static void FillPropertyCollectionFromDataReader(SfcPropertyCollection properties, IDataReader reader)
	{
		DataTable schemaTable = reader.GetSchemaTable();
		int columnIndex = schemaTable.Columns.IndexOf("ColumnName");
		for (int i = 0; i < schemaTable.Rows.Count; i++)
		{
			string propertyName = schemaTable.Rows[i][columnIndex] as string;
			int num = properties.LookupID(propertyName);
			if (num >= 0)
			{
				SfcProperty sfcProperty = properties[propertyName];
				sfcProperty.Retrieved = true;
				object value = reader.GetValue(i);
				sfcProperty.Value = value;
				sfcProperty.Dirty = false;
			}
		}
	}

	internal void MergeObjectPropsFromPropertyCollection(SfcPropertyCollection mergeProperties)
	{
		foreach (SfcProperty mergeProperty in mergeProperties)
		{
			if (!Properties[mergeProperty.Name].Dirty)
			{
				Properties[mergeProperty.Name].Retrieved = true;
				Properties[mergeProperty.Name].Value = mergeProperties[mergeProperty.Name].Value;
				Properties[mergeProperty.Name].Dirty = false;
			}
		}
	}

	private static IDataReader GetInitDataReader(ISfcConnection connection, Urn urn, string[] fields, OrderBy[] orderby)
	{
		Request request = new Request(urn);
		request.ResultType = ResultType.IDataReader;
		request.Fields = fields;
		request.OrderByList = orderby;
		return EnumResult.ConvertToDataReader(Enumerator.GetData(connection.ToEnumeratorObject(), request));
	}

	private static ResultType[] GetSupportedResultTypes(ISfcConnection connection, Urn urn)
	{
		RequestObjectInfo requestObjectInfo = new RequestObjectInfo(urn, RequestObjectInfo.Flags.ResultTypes);
		Enumerator enumerator = new Enumerator();
		ObjectInfo objectInfo = enumerator.Process(connection.ToEnumeratorObject(), requestObjectInfo);
		return objectInfo.ResultTypes;
	}

	internal void InitReferenceLevel(ISfcCollection refColl)
	{
		Urn urn = refColl.GetCollectionElementNameImpl();
		Urn urn2 = new Urn(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2] { Urn, urn }));
		using IDataReader reader = GetInitDataReader(GetConnectionContext(), urn2, null, null);
		InitObjectsFromEnumResults(refColl, reader);
	}

	internal void InitChildLevel(ISfcCollection childColl)
	{
		Urn arg = childColl.GetCollectionElementNameImpl();
		Urn urn = new Urn($"{Urn}/{arg}");
		using IDataReader reader = GetInitDataReader(GetConnectionContext(), urn, null, null);
		InitObjectsFromEnumResults(childColl, reader);
	}

	private void InitObjectsFromEnumResults(ISfcCollection childColl, IDataReader reader)
	{
		if (!reader.Read())
		{
			reader.Close();
			return;
		}
		bool flag = true;
		for (int i = 0; i < reader.FieldCount; i++)
		{
			if (!(reader[i] is DBNull))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			reader.Close();
			return;
		}
		object[] array = new object[reader.FieldCount];
		reader.GetValues(array);
		InitObjectsFromEnumResultsRec(childColl, reader, 0, array);
	}

	private void InitObjectsFromEnumResultsRec(ISfcCollection childColl, IDataReader reader, int columnIdx, object[] parentRow)
	{
		TraceHelper.Assert(!reader.IsClosed);
		if (childColl.Count > 0)
		{
			while (CompareRows(reader, parentRow, 0, columnIdx))
			{
				MergeOrCreateObjectFromRow(childColl, reader);
				if (!reader.Read())
				{
					reader.Close();
					break;
				}
			}
			return;
		}
		while (CompareRows(reader, parentRow, 0, columnIdx))
		{
			CreateNewObjectFromRow(childColl, reader);
			if (!reader.Read())
			{
				reader.Close();
				break;
			}
		}
	}

	private static SfcInstance MergeOrCreateObjectFromRow(ISfcCollection childColl, IDataReader reader)
	{
		SfcInstance sfcInstance = childColl.GetElementFactory().Create(childColl.Parent, new PopulatorFromDataReader(reader), SfcObjectState.Existing);
		if (childColl.GetExisting(sfcInstance.AbstractIdentityKey, out var sfcInstance2))
		{
			sfcInstance2.MergeObjectPropsFromPropertyCollection(sfcInstance.Properties);
		}
		else
		{
			sfcInstance2 = sfcInstance;
		}
		childColl.AddShadow(sfcInstance2);
		return sfcInstance2;
	}

	private static SfcInstance CreateNewObjectFromRow(ISfcCollection childColl, IDataReader reader)
	{
		SfcInstance sfcInstance = childColl.GetElementFactory().Create(childColl.Parent, new PopulatorFromDataReader(reader), SfcObjectState.Existing);
		childColl.AddShadow(sfcInstance);
		return sfcInstance;
	}

	private bool AdvanceInitRec(IDataReader reader, int columnIdx)
	{
		if (reader.FieldCount - columnIdx > 1)
		{
			FillPropertyCollectionFromDataReader(Properties, reader);
		}
		if (!reader.Read())
		{
			reader.Close();
			return false;
		}
		return true;
	}

	private static bool CompareRows(IDataReader reader, object[] parentRow, int columnStartIdx, int columnStopIdx)
	{
		TraceHelper.Assert(!reader.IsClosed);
		for (int i = columnStartIdx; i < columnStopIdx; i++)
		{
			if (!reader.GetValue(i).Equals(parentRow[i]))
			{
				return false;
			}
		}
		return true;
	}

	public virtual void Discover(ISfcDependencyDiscoveryObjectSink sink)
	{
	}

	public ISfcDomain GetDomain()
	{
		SfcInstance sfcInstance = this;
		while (sfcInstance.Parent != null)
		{
			sfcInstance = sfcInstance.Parent;
		}
		if (!(sfcInstance is ISfcDomain result))
		{
			throw new SfcMissingParentException(SfcStrings.MissingParent);
		}
		return result;
	}

	protected internal virtual SfcTypeMetadata GetTypeMetadataImpl()
	{
		return GetDomain().GetTypeMetadata(GetType().Name);
	}

	protected void MarkRootAsConnected()
	{
		if (GetDomain().UseSfcStateManagement())
		{
			TraceHelper.Assert(this is ISfcDomain);
			State = SfcObjectState.Existing;
			Initialize();
		}
	}

	private ISfcCollection GetParentCollection()
	{
		return Parent.GetChildCollection(GetType().Name);
	}

	protected void CheckObjectState()
	{
		if (GetDomain().UseSfcStateManagement())
		{
			TraceHelper.Assert(KeyChain != null);
			if (State == SfcObjectState.Dropped)
			{
				throw new SfcInvalidStateException(SfcStrings.InvalidState(State, SfcObjectState.Existing));
			}
		}
	}

	protected void CheckObjectCreated()
	{
		CheckObjectState();
		CheckObjectStateAndParent(SfcObjectState.Existing);
	}

	private void CheckObjectStateAndParent(SfcObjectState required_state)
	{
		if (Parent == null && !(this is ISfcDomain))
		{
			throw new SfcMissingParentException(SfcStrings.MissingParent);
		}
		if (!GetDomain().UseSfcStateManagement() || State == required_state)
		{
			return;
		}
		throw new SfcInvalidStateException(SfcStrings.InvalidState(State, required_state));
	}

	private List<SfcInstance> GetDependentObjects(SfcDependencyAction action)
	{
		SfcDependencyEngine sfcDependencyEngine = new SfcDependencyEngine(SfcDependencyDiscoveryMode.Children, action);
		SfcDependencyRootList sfcDependencyRootList = new SfcDependencyRootList();
		sfcDependencyRootList.Add(this);
		sfcDependencyEngine.SfcDependencyRootList = sfcDependencyRootList;
		sfcDependencyEngine.Discover();
		SfcDependencyEngine.DependencyListEnumerator dependencyListEnumerator = new SfcDependencyEngine.DependencyListEnumerator(sfcDependencyEngine);
		List<SfcInstance> list = new List<SfcInstance>();
		foreach (SfcDependencyNode item in (IEnumerable<SfcDependencyNode>)dependencyListEnumerator)
		{
			list.Add(item.Instance);
		}
		dependencyListEnumerator.Dispose();
		return list;
	}

	private void PostCrud(SfcDependencyAction depAction, SfcKeyChain oldKeyChain, object extraParam, object executionResult)
	{
		UpdateUIPropertyState();
		switch (depAction)
		{
		case SfcDependencyAction.Create:
		{
			PostCreate(executionResult);
			ISfcCollection parentCollection4 = GetParentCollection();
			if (parentCollection4.GetExisting(AbstractIdentityKey, out var sfcInstance4))
			{
				TraceHelper.Assert(object.ReferenceEquals(sfcInstance4, this));
			}
			else
			{
				GetParentCollection().Add(this);
			}
			{
				foreach (SfcProperty property in Properties)
				{
					property.Dirty = false;
				}
				break;
			}
		}
		case SfcDependencyAction.Rename:
		{
			TraceHelper.Assert(extraParam is SfcKey);
			SfcKey sfcKey = extraParam as SfcKey;
			SfcKeyChain sfcKeyChain2 = new SfcKeyChain(sfcKey, KeyChain.Parent);
			SfcApplication.Events.OnBeforeObjectRenamed(this, new SfcBeforeObjectRenamedEventArgs(KeyChain.Urn, this, sfcKeyChain2.Urn, sfcKey));
			SfcKey leafKey = KeyChain.LeafKey;
			ISfcCollection parentCollection3 = GetParentCollection();
			TraceHelper.Assert(parentCollection3.GetExisting(AbstractIdentityKey, out var sfcInstance3));
			TraceHelper.Assert(object.ReferenceEquals(this, sfcInstance3));
			parentCollection3.Rename(this, sfcKey);
			TraceHelper.Assert(!parentCollection3.GetExisting(leafKey, out sfcInstance3));
			TraceHelper.Assert(parentCollection3.GetExisting(AbstractIdentityKey, out sfcInstance3));
			TraceHelper.Assert(object.ReferenceEquals(this, sfcInstance3));
			PostRename(executionResult);
			SfcApplication.Events.OnAfterObjectRenamed(this, new SfcAfterObjectRenamedEventArgs(KeyChain.Urn, this, oldKeyChain.Urn, leafKey));
			break;
		}
		case SfcDependencyAction.Move:
		{
			TraceHelper.Assert(extraParam is SfcInstance);
			SfcInstance sfcInstance = extraParam as SfcInstance;
			SfcKeyChain sfcKeyChain = new SfcKeyChain(AbstractIdentityKey, sfcInstance.KeyChain);
			SfcApplication.Events.OnBeforeObjectMoved(this, new SfcBeforeObjectMovedEventArgs(KeyChain.Urn, this, sfcKeyChain.Urn, sfcInstance));
			SfcInstance parent = Parent;
			ISfcCollection parentCollection = GetParentCollection();
			parentCollection.RemoveElement(this);
			m_keychain.Parent = sfcInstance.m_keychain;
			m_parent = sfcInstance;
			ISfcCollection parentCollection2 = GetParentCollection();
			parentCollection2.Add(this);
			TraceHelper.Assert(!parentCollection.GetExisting(AbstractIdentityKey, out var sfcInstance2));
			TraceHelper.Assert(parentCollection2.GetExisting(AbstractIdentityKey, out sfcInstance2));
			TraceHelper.Assert(object.ReferenceEquals(this, sfcInstance2));
			PostMove(executionResult);
			SfcApplication.Events.OnAfterObjectMoved(this, new SfcAfterObjectMovedEventArgs(KeyChain.Urn, this, oldKeyChain.Urn, parent));
			break;
		}
		case SfcDependencyAction.Alter:
			PostAlter(executionResult);
			{
				foreach (SfcProperty property2 in Properties)
				{
					property2.Dirty = false;
				}
				break;
			}
		case SfcDependencyAction.Drop:
			PostDrop(executionResult);
			GetParentCollection().RemoveElement(this);
			break;
		default:
			TraceHelper.Assert(condition: false);
			break;
		}
	}

	protected virtual void PostCreate(object executionResult)
	{
	}

	protected virtual void PostRename(object executionResult)
	{
	}

	protected virtual void PostMove(object executionResult)
	{
	}

	protected virtual void PostAlter(object executionResult)
	{
	}

	protected virtual void PostDrop(object executionResult)
	{
	}

	private ISfcScript ScriptCRUD(SfcDependencyAction depAction, object extraParam)
	{
		try
		{
			switch (depAction)
			{
			case SfcDependencyAction.Create:
				return ((ISfcCreatable)this).ScriptCreate();
			case SfcDependencyAction.Rename:
				return ((ISfcRenamable)this).ScriptRename((SfcKey)extraParam);
			case SfcDependencyAction.Move:
				return ((ISfcMovable)this).ScriptMove((SfcInstance)extraParam);
			case SfcDependencyAction.Alter:
				return ((ISfcAlterable)this).ScriptAlter();
			case SfcDependencyAction.Drop:
				return ((ISfcDroppable)this).ScriptDrop();
			}
		}
		catch (InvalidCastException)
		{
			throw new SfcObjectNotScriptableException(SfcStrings.ObjectNotScriptabe(ToString(), GetType().Name));
		}
		return null;
	}

	private ISfcScript AccumulateScript(List<SfcInstance> objList, SfcDependencyAction depAction, object extraParam)
	{
		ISfcScript sfcScript = null;
		foreach (SfcInstance obj in objList)
		{
			if (objList.Contains(obj.Parent))
			{
				SfcTypeMetadata typeMetadataImpl = obj.GetTypeMetadataImpl();
				if (typeMetadataImpl != null && typeMetadataImpl.IsCrudActionHandledByParent(depAction))
				{
					continue;
				}
			}
			ISfcScript sfcScript2 = obj.ScriptCRUD(depAction, extraParam);
			if (sfcScript2 != null)
			{
				if (sfcScript == null)
				{
					sfcScript = sfcScript2;
				}
				else
				{
					sfcScript.Add(sfcScript2);
				}
			}
		}
		return sfcScript;
	}

	private void CRUDImpl(string operationName, SfcObjectState requiredState, SfcDependencyAction depAction, SfcObjectState finalState)
	{
		CRUDImplWorker(operationName, requiredState, depAction, finalState, null);
	}

	private void CRUDImplWorker(string operationName, SfcObjectState requiredState, SfcDependencyAction depAction, SfcObjectState finalState, object extraParam)
	{
		CheckObjectStateAndParent(requiredState);
		List<SfcInstance> dependentObjects = GetDependentObjects(depAction);
		TraceHelper.Assert(dependentObjects.Count > 0);
		SfcKeyChain oldKeyChain = new SfcKeyChain(KeyChain.LeafKey, KeyChain.Parent);
		object executionResult = null;
		if (GetDomain().ConnectionContext.Mode == SfcConnectionContextMode.Offline)
		{
			if (depAction == SfcDependencyAction.Create && Parent != null)
			{
				SfcKey sfcKey = CreateIdentityKey();
				if (!sfcKey.Equals(AbstractIdentityKey))
				{
					ResetKey();
					TraceHelper.Assert(sfcKey.Equals(AbstractIdentityKey));
				}
				if (GetParentCollection().GetExisting(sfcKey, out var _))
				{
					throw new SfcCRUDOperationFailedException(SfcStrings.CannotCreateDestinationHasDuplicate(this));
				}
			}
		}
		else
		{
			ISfcScript script = AccumulateScript(dependentObjects, depAction, extraParam);
			ISfcExecutionEngine executionEngine = GetDomain().GetExecutionEngine();
			try
			{
				executionResult = executionEngine.Execute(script);
			}
			catch (Exception innerException)
			{
				throw new SfcCRUDOperationFailedException(SfcStrings.CRUDOperationFailed(operationName, ToString()), innerException);
			}
		}
		if (GetDomain().UseSfcStateManagement())
		{
			foreach (SfcInstance item in dependentObjects)
			{
				if (SfcObjectState.ToBeDropped != item.State)
				{
					item.State = finalState;
				}
				else
				{
					item.State = SfcObjectState.Dropped;
					if (item.Parent.State != SfcObjectState.Dropped)
					{
						item.GetParentCollection().RemoveElement(item);
					}
				}
				if (depAction == SfcDependencyAction.Create || depAction == SfcDependencyAction.Alter)
				{
					foreach (SfcProperty property in item.Properties)
					{
						property.Dirty = false;
					}
				}
				switch (depAction)
				{
				case SfcDependencyAction.Create:
					SfcApplication.Events.OnObjectCreated(this, new SfcObjectCreatedEventArgs(item.Urn, item));
					break;
				case SfcDependencyAction.Alter:
					SfcApplication.Events.OnObjectAltered(this, new SfcObjectAlteredEventArgs(item.Urn, item));
					break;
				case SfcDependencyAction.Drop:
					SfcApplication.Events.OnObjectDropped(this, new SfcObjectDroppedEventArgs(item.Urn, item));
					break;
				}
			}
		}
		PostCrud(depAction, oldKeyChain, extraParam, executionResult);
	}

	protected void CreateImpl()
	{
		CRUDImpl(SfcStrings.opCreate, SfcObjectState.Pending, SfcDependencyAction.Create, SfcObjectState.Existing);
	}

	protected void RenameImpl(SfcKey newKey)
	{
		if (newKey == null)
		{
			throw new SfcInvalidRenameException(SfcStrings.CannotRenameNoKey(this));
		}
		ISfcCollection parentCollection = GetParentCollection();
		bool initialized = parentCollection.Initialized;
		try
		{
			parentCollection.Initialized = true;
			if (GetParentCollection().GetExisting(newKey, out var _))
			{
				throw new SfcInvalidRenameException(SfcStrings.CannotRenameDestinationHasDuplicate(this, newKey));
			}
			CRUDImplWorker(SfcStrings.opRename, SfcObjectState.Existing, SfcDependencyAction.Rename, SfcObjectState.Existing, newKey);
		}
		finally
		{
			parentCollection.Initialized = initialized;
		}
	}

	protected void MoveImpl(SfcInstance newParent)
	{
		if (newParent == null)
		{
			throw new SfcInvalidMoveException(SfcStrings.CannotMoveNoDestination(this));
		}
		if (KeyChain.IsClientAncestorOf(newParent.KeyChain))
		{
			throw new SfcInvalidMoveException(SfcStrings.CannotMoveDestinationIsDescendant(this, newParent));
		}
		if (newParent.GetChildCollection(GetType().Name).GetExisting(AbstractIdentityKey, out var _))
		{
			throw new SfcInvalidMoveException(SfcStrings.CannotMoveDestinationHasDuplicate(this, newParent));
		}
		CRUDImplWorker(SfcStrings.opMove, newParent.State, SfcDependencyAction.Move, newParent.State, newParent);
	}

	protected void AlterImpl()
	{
		CRUDImpl(SfcStrings.opAlter, SfcObjectState.Existing, SfcDependencyAction.Alter, SfcObjectState.Existing);
	}

	protected void DropImpl()
	{
		CRUDImpl(SfcStrings.opDrop, SfcObjectState.Existing, SfcDependencyAction.Drop, SfcObjectState.Dropped);
	}

	protected void MarkForDropImpl(bool dropOnAlter)
	{
		CheckObjectState();
		if (State != SfcObjectState.Existing && State != SfcObjectState.ToBeDropped)
		{
			throw new SfcInvalidStateException(SfcStrings.InvalidState(State, SfcObjectState.Existing));
		}
		if (dropOnAlter)
		{
			State = SfcObjectState.ToBeDropped;
		}
		else if (State == SfcObjectState.ToBeDropped)
		{
			State = SfcObjectState.Existing;
		}
	}
}
public abstract class SfcInstance<K, T> : SfcInstance where K : SfcKey where T : SfcInstance, new()
{
	private sealed class ObjectFactory : SfcObjectFactory
	{
		private static readonly ObjectFactory instance;

		public static ObjectFactory Instance => instance;

		static ObjectFactory()
		{
			instance = new ObjectFactory();
		}

		private ObjectFactory()
		{
		}

		protected override SfcInstance CreateImpl()
		{
			return new T();
		}
	}

	private sealed class TypeMetadata : SfcTypeMetadata
	{
		private string typeName = typeof(T).Name;

		private static readonly TypeMetadata instance;

		public static TypeMetadata Instance => instance;

		static TypeMetadata()
		{
			instance = new TypeMetadata();
		}

		private TypeMetadata()
		{
		}

		public override bool IsCrudActionHandledByParent(SfcDependencyAction depAction)
		{
			switch (depAction)
			{
			case SfcDependencyAction.Create:
			case SfcDependencyAction.Alter:
			case SfcDependencyAction.Rename:
			case SfcDependencyAction.Move:
				return false;
			case SfcDependencyAction.Drop:
				return true;
			default:
				throw new InvalidOperationException(SfcStrings.UnsupportedAction(depAction.ToString(), typeName));
			}
		}
	}

	[SfcIgnore]
	public K IdentityKey => (K)base.AbstractIdentityKey;

	public SfcInstance()
	{
	}

	public static SfcTypeMetadata GetTypeMetadata()
	{
		return TypeMetadata.Instance;
	}

	protected internal override SfcTypeMetadata GetTypeMetadataImpl()
	{
		return GetTypeMetadata();
	}

	public static SfcObjectFactory GetObjectFactory()
	{
		return ObjectFactory.Instance;
	}

	protected internal abstract K CreateKey();

	protected internal override SfcKey CreateIdentityKey()
	{
		return CreateKey();
	}
}
