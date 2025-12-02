using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Reflection;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class SfcObjectIterator : IEnumerable, IEnumerator, IDisposable
{
	private ISfcDomain _root;

	private string[] _fields;

	private OrderBy[] _orderByFields;

	private Type _type;

	private IDataReader _ResultsDataReader;

	private DataTable _ResultsDataTable;

	private DataTableReader _ResultsDataTableReader;

	private Urn _urn;

	private int _urnColIndex;

	private bool _closeConnection;

	private bool _cacheReader;

	private SfcObjectQueryMode _activeQueriesMode;

	private ISfcConnection _connection;

	private SfcInstance _currentInstance;

	object IEnumerator.Current
	{
		get
		{
			Urn urn = _ResultsDataReader.GetString(_urnColIndex);
			SfcInstance sfcInstance = _root as SfcInstance;
			if (IsDomainHop(sfcInstance.Urn, urn))
			{
				object obj = CreateObjectHierarchy(urn);
				if (obj is ISqlSmoObjectInitialize)
				{
					ISqlSmoObjectInitialize sqlSmoObjectInitialize = obj as ISqlSmoObjectInitialize;
					sqlSmoObjectInitialize.InitializeFromDataReader(_ResultsDataReader);
					return sqlSmoObjectInitialize;
				}
				throw new SfcMetadataException(SfcStrings.DomainNotFound(urn.XPathExpression[0].ToString()));
			}
			if (_currentInstance == null)
			{
				SfcKeyChain sfcKeyChain = new SfcKeyChain(_ResultsDataReader.GetString(_urnColIndex), _root);
				if (sfcKeyChain.Parent == null)
				{
					TraceHelper.Assert(sfcKeyChain.LeafKey is DomainRootKey);
					_currentInstance = sfcKeyChain.GetObject();
				}
				else
				{
					SfcInstance sfcInstance2 = sfcKeyChain.Parent.GetObject();
					TraceHelper.Assert(sfcInstance2 != null);
					string name = sfcKeyChain.LeafKey.InstanceType.Name;
					ISfcCollection childCollection = sfcInstance2.GetChildCollection(name);
					if (!childCollection.GetExisting(sfcKeyChain.LeafKey, out _currentInstance))
					{
						SfcObjectFactory elementFactory = childCollection.GetElementFactory();
						_currentInstance = elementFactory.Create(sfcInstance2, new SfcInstance.PopulatorFromDataReader(_ResultsDataReader), SfcObjectState.Existing);
						childCollection.Add(_currentInstance);
					}
				}
			}
			return _currentInstance;
		}
	}

	public SfcObjectIterator(ISfcDomain root, SfcObjectQueryMode activeQueriesMode, SfcQueryExpression query, string[] fields, OrderBy[] orderByFields)
	{
		_root = root;
		_fields = fields;
		_urn = query.ToUrn();
		_activeQueriesMode = activeQueriesMode;
		_orderByFields = orderByFields;
		_type = _root.GetType(_urn.Type);
		TraceHelper.Assert(_type != null);
		GetConnection();
		MakeDataReader();
	}

	public void Close()
	{
		CloseDataReader();
		CloseClonedConnection();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this;
	}

	private bool IsDomainHop(Urn source, Urn destination)
	{
		return source.XPathExpression[0].Name != destination.XPathExpression[0].Name;
	}

	private object CreateObjectHierarchy(Urn urn)
	{
		object obj = null;
		if (null != urn.Parent)
		{
			obj = CreateObjectHierarchy(urn.Parent);
		}
		Type type = _root.GetType(urn.Type);
		object obj2 = SfcRegistration.CreateObject(type.FullName);
		TraceHelper.Assert(obj2 != null);
		if (obj2 is IAlienRoot)
		{
			IAlienRoot alienRoot = obj2 as IAlienRoot;
			alienRoot.DesignModeInitialize();
			alienRoot.ConnectionContext.TrueName = urn.GetNameForType(urn.Type);
			alienRoot.ConnectionContext.ServerInstance = urn.GetNameForType(urn.Type);
		}
		if (obj != null && obj2 is IAlienObject)
		{
			IAlienObject alienObject = obj2 as IAlienObject;
			PropertyInfo property = alienObject.GetType().GetProperty("Parent", BindingFlags.Instance | BindingFlags.Public);
			if (property != null && property.CanWrite)
			{
				property.SetValue(alienObject, obj, null);
			}
			property = alienObject.GetType().GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);
			if (property != null && property.CanWrite)
			{
				property.SetValue(alienObject, urn.GetNameForType(urn.Type), null);
			}
		}
		return obj2;
	}

	bool IEnumerator.MoveNext()
	{
		_currentInstance = null;
		return _ResultsDataReader.Read();
	}

	void IEnumerator.Reset()
	{
		CloseDataReader();
		MakeDataReader();
	}

	void IDisposable.Dispose()
	{
		Close();
	}

	private void CloseDataReader()
	{
		if (_cacheReader)
		{
			if (_ResultsDataTableReader != null)
			{
				_ResultsDataTableReader.Close();
				_ResultsDataTableReader = null;
				_ResultsDataReader = null;
			}
			if (_ResultsDataTable != null)
			{
				_ResultsDataTable.Clear();
				_ResultsDataTable = null;
			}
		}
		else if (_ResultsDataReader != null && !_ResultsDataReader.IsClosed)
		{
			_ResultsDataReader.Close();
			_ResultsDataReader = null;
		}
	}

	private void CloseClonedConnection()
	{
		if (_closeConnection && _connection != null)
		{
			_connection.Disconnect();
			_connection = null;
		}
	}

	private void GetConnection()
	{
		ISfcConnection connection = _root.GetConnection();
		if (_root.ConnectionContext.Mode == SfcConnectionContextMode.Offline || _activeQueriesMode == SfcObjectQueryMode.CachedQuery)
		{
			_connection = null;
		}
		else
		{
			try
			{
				_connection = _root.GetConnection(_activeQueriesMode);
			}
			catch
			{
				_connection = null;
			}
		}
		if (_connection == null)
		{
			_cacheReader = true;
			_connection = connection;
		}
		_closeConnection = connection != _connection;
	}

	private void MakeDataReader()
	{
		_currentInstance = null;
		Request request = new Request(_urn);
		request.Fields = _fields;
		request.ResultType = ResultType.IDataReader;
		request.OrderByList = _orderByFields;
		if (_cacheReader)
		{
			_ResultsDataTable = new DataTable();
			_ResultsDataTable.Locale = CultureInfo.InvariantCulture;
			if (_root.ConnectionContext.Mode != SfcConnectionContextMode.Offline)
			{
				using (_ResultsDataReader = EnumResult.ConvertToDataReader(Enumerator.GetData(_connection.ToEnumeratorObject(), request)))
				{
					_ResultsDataTable.Load(_ResultsDataReader);
				}
			}
			_ResultsDataTableReader = _ResultsDataTable.CreateDataReader();
			_ResultsDataReader = _ResultsDataTableReader;
		}
		else
		{
			_ResultsDataReader = EnumResult.ConvertToDataReader(Enumerator.GetData(_connection.ToEnumeratorObject(), request));
		}
		DataTable schemaTable = _ResultsDataReader.GetSchemaTable();
		int columnIndex = schemaTable.Columns.IndexOf("ColumnName");
		for (int i = 0; i < schemaTable.Rows.Count; i++)
		{
			string strA = schemaTable.Rows[i][columnIndex] as string;
			if (string.Compare(strA, "Urn", StringComparison.OrdinalIgnoreCase) == 0)
			{
				_urnColIndex = i;
				break;
			}
		}
	}
}
