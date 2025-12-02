using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public abstract class SqlObjectBase : EnumObject, ISqlFilterDecoderCallback
{
	private StatementBuilder m_sb;

	private ParentLink m_parentLink;

	private ConditionedSqlList m_conditionedSqlList;

	private ConditionedSqlList m_propertyLinkList;

	private ConditionedSqlList m_postProcessList;

	private RequestParentSelect m_RequestParentSelect;

	private SortedList m_OrderByRedirect;

	private SortedList m_SpecialQuery;

	private ArrayList m_LinkFields;

	private bool m_distinct;

	public StatementBuilder StatementBuilder
	{
		get
		{
			return m_sb;
		}
		set
		{
			m_sb = value;
		}
	}

	internal ParentLink ParentLink
	{
		get
		{
			return m_parentLink;
		}
		set
		{
			m_parentLink = value;
		}
	}

	protected RequestParentSelect RequestParentSelect
	{
		get
		{
			return m_RequestParentSelect;
		}
		set
		{
			m_RequestParentSelect = value;
		}
	}

	protected SortedList OrderByRedirect
	{
		get
		{
			if (m_OrderByRedirect == null)
			{
				m_OrderByRedirect = new SortedList(StringComparer.Ordinal);
			}
			return m_OrderByRedirect;
		}
	}

	protected SortedList SpecialQuery => m_SpecialQuery;

	protected ConditionedSqlList ConditionedSqlList
	{
		get
		{
			return m_conditionedSqlList;
		}
		set
		{
			m_conditionedSqlList = value;
		}
	}

	protected ConditionedSqlList PropertyLinkList
	{
		get
		{
			return m_propertyLinkList;
		}
		set
		{
			m_propertyLinkList = value;
		}
	}

	protected ConditionedSqlList PostProcessList
	{
		get
		{
			return m_postProcessList;
		}
		set
		{
			m_postProcessList = value;
		}
	}

	protected bool Distinct
	{
		get
		{
			return m_distinct;
		}
		set
		{
			m_distinct = value;
		}
	}

	protected SqlRequest SqlRequest
	{
		get
		{
			if (base.Request == null)
			{
				return new SqlRequest();
			}
			if (base.Request is SqlRequest)
			{
				return (SqlRequest)base.Request;
			}
			return new SqlRequest(base.Request);
		}
	}

	internal string ObjectName => base.Urn.Type;

	public override ResultType[] ResultTypes => new ResultType[3]
	{
		ResultType.IDataReader,
		ResultType.DataTable,
		ResultType.DataSet
	};

	private bool CalculateParentRequest => null != m_LinkFields;

	public virtual bool SupportsParameterization => ServerConnection.ParameterizationMode >= QueryParameterizationMode.ForcedParameterization;

	public SqlObjectBase()
	{
		m_conditionedSqlList = new ConditionedSqlList();
		m_propertyLinkList = new ConditionedSqlList();
		m_postProcessList = new ConditionedSqlList();
	}

	internal void SetUrn(Urn urn)
	{
		base.Urn = urn;
	}

	public SqlObjectProperty GetSqlProperty(string field, ObjectPropertyUsages usage)
	{
		return (SqlObjectProperty)GetProperty(field, usage);
	}

	private void AddRequestProperty(SqlObjectProperty prop, bool triggered)
	{
		if (CalculateParentRequest)
		{
			AddLinkFields(prop.LinkFields);
			PostProcessList.AddHits(this, prop.Name, null);
		}
		else
		{
			prop.Alias = GetAliasPropertyName(prop.Name);
			prop.Add(this, triggered);
			AddLinkProperty(prop.Name);
		}
	}

	private void RegisterPostProcessHits()
	{
		foreach (SqlPostProcess postProcess in PostProcessList)
		{
			if (postProcess.Used)
			{
				postProcess.Register(this);
			}
		}
	}

	private void AddRequestProperties()
	{
		if (base.Request != null && base.Request.Fields != null)
		{
			string[] fields = base.Request.Fields;
			foreach (string field in fields)
			{
				ObjectPropertyUsages usage = ((ResultType.Reserved2 == base.Request.ResultType) ? ObjectPropertyUsages.Reserved1 : ObjectPropertyUsages.Request);
				SqlObjectProperty sqlProperty = GetSqlProperty(field, usage);
				AddRequestProperty(sqlProperty, triggered: false);
			}
			AddPostProcessTriggers();
		}
	}

	public virtual string AddFilterProperty(string name)
	{
		SqlObjectProperty sqlProperty = GetSqlProperty(name, ObjectPropertyUsages.Filter);
		if (CalculateParentRequest)
		{
			AddLinkFields(sqlProperty.LinkFields);
		}
		else
		{
			AddLinkProperty(name);
		}
		return sqlProperty.GetValueWithCast(this);
	}

	public virtual string AddOrderByProperty(string name)
	{
		return AddOrderByProperty(name, overrideFlags: false);
	}

	public virtual string AddOrderByProperty(string name, bool overrideFlags)
	{
		SqlObjectProperty sqlObjectProperty = ((!overrideFlags) ? GetSqlProperty(name, ObjectPropertyUsages.OrderBy) : GetSqlProperty(name, ObjectPropertyUsages.Reserved1));
		if (CalculateParentRequest)
		{
			AddLinkFields(sqlObjectProperty.LinkFields);
		}
		else
		{
			AddLinkProperty(name);
		}
		return sqlObjectProperty.GetValueWithCast(this);
	}

	protected virtual string AddLinkProperty(string name)
	{
		SqlObjectProperty sqlProperty = GetSqlProperty(name, ObjectPropertyUsages.Reserved1);
		AddConditionals(name);
		return sqlProperty.GetValueWithCast(this);
	}

	protected void AddConditionalsJustPropDependencies(string name)
	{
		if (m_sb == null)
		{
			return;
		}
		SqlObjectProperty sqlProperty = GetSqlProperty(name, ObjectPropertyUsages.Reserved1);
		if (sqlProperty.LinkFields == null)
		{
			return;
		}
		foreach (LinkField linkField in sqlProperty.LinkFields)
		{
			if (LinkFieldType.Local == linkField.Type)
			{
				AddLinkProperty(linkField.Field);
			}
		}
	}

	protected virtual void AddConditionals(string field)
	{
		if (m_sb != null)
		{
			ConditionedSqlList.AddHits(this, field, m_sb);
			PropertyLinkList.AddHits(this, field, m_sb);
			PostProcessList.AddHits(this, field, m_sb);
			AddConditionalsJustPropDependencies(field);
		}
	}

	private void RetrieveParentRequestLinks(SqlRequest sr)
	{
		m_LinkFields = new ArrayList();
		if (ParentLink != null)
		{
			AddLinkFields(ParentLink.LinkFields);
		}
		foreach (ConditionedSql propertyLink in PropertyLinkList)
		{
			if (propertyLink.LinkMultiple != null)
			{
				AddLinkFields(propertyLink.LinkMultiple.LinkFields);
			}
		}
		foreach (ConditionedSql conditionedSql in ConditionedSqlList)
		{
			if (conditionedSql.LinkMultiple != null)
			{
				AddLinkFields(conditionedSql.LinkMultiple.LinkFields);
			}
		}
		AddRequestProperties();
		if (SqlRequest != null && SqlRequest.LinkFields != null)
		{
			foreach (LinkField linkField in SqlRequest.LinkFields)
			{
				if (linkField.Type == LinkFieldType.Parent)
				{
					SqlObjectProperty sqlProperty = GetSqlProperty(linkField.Field, ObjectPropertyUsages.Reserved1);
					AddLinkFields(sqlProperty.LinkFields);
				}
			}
		}
		if (m_LinkFields.Count > 0)
		{
			sr.SetLinkFields(m_LinkFields);
		}
	}

	private void PropagateRequestedParentProperties(SqlRequest sr)
	{
		if (base.Request == null || base.Request.ParentPropertiesRequests == null)
		{
			return;
		}
		if (base.Request.ParentPropertiesRequests.Length > 0)
		{
			PropertiesRequest propertiesRequest = base.Request.ParentPropertiesRequests[0];
			if (propertiesRequest != null)
			{
				if (propertiesRequest.Fields != null)
				{
					if (sr.Fields == null)
					{
						sr.Fields = new string[propertiesRequest.Fields.Length];
					}
					propertiesRequest.Fields.CopyTo(sr.Fields, 0);
					sr.PropertyAlias = propertiesRequest.PropertyAlias;
				}
				if (propertiesRequest.OrderByList != null)
				{
					if (sr.OrderByList == null)
					{
						sr.OrderByList = new OrderBy[propertiesRequest.OrderByList.Length];
					}
					propertiesRequest.OrderByList.CopyTo(sr.OrderByList, 0);
				}
			}
		}
		if (base.Request.ParentPropertiesRequests.Length > 1)
		{
			sr.ParentPropertiesRequests = new PropertiesRequest[base.Request.ParentPropertiesRequests.Length - 1];
			Array.Copy(base.Request.ParentPropertiesRequests, 1, sr.ParentPropertiesRequests, 0, sr.ParentPropertiesRequests.Length);
		}
		else
		{
			sr.ParentPropertiesRequests = null;
		}
	}

	public override Request RetrieveParentRequest()
	{
		SqlRequest sqlRequest = new SqlRequest();
		sqlRequest.RequestFieldsTypes = RequestFieldsTypes.Request;
		if (ResultType.Reserved2 == base.Request.ResultType)
		{
			sqlRequest.ResultType = ResultType.Reserved2;
		}
		else
		{
			sqlRequest.ResultType = ResultType.Reserved1;
		}
		RetrieveParentRequestLinks(sqlRequest);
		AddXpathFilter();
		AddOrderByInDatabase();
		if (RequestParentSelect != null)
		{
			sqlRequest.PrefixPostfixFields = RequestParentSelect.Fields;
		}
		PropagateRequestedParentProperties(sqlRequest);
		m_LinkFields = null;
		return sqlRequest;
	}

	internal void AddLinkProperties(LinkFieldType lft, ArrayList linkFields)
	{
		if (linkFields == null)
		{
			return;
		}
		foreach (LinkField linkField in linkFields)
		{
			if (lft == linkField.Type)
			{
				SqlObjectProperty sqlObjectProperty = (SqlObjectProperty)GetProperty(linkField.Field, ObjectPropertyUsages.Reserved1);
				AddLinkProperties(LinkFieldType.Local, sqlObjectProperty.LinkFields);
				linkField.Value = AddLinkProperty(linkField.Field);
			}
		}
	}

	private void AddParentLinkProperties()
	{
		AddParentLinkPropertiesParent();
		AddParentLinkPropertiesLocal();
	}

	private void AddParentLinkPropertiesParent()
	{
		if (SqlRequest != null)
		{
			AddLinkProperties(LinkFieldType.Parent, SqlRequest.LinkFields);
		}
	}

	private void AddParentLinkPropertiesLocal()
	{
		if (m_parentLink != null)
		{
			AddLinkProperties(LinkFieldType.Local, m_parentLink.LinkFields);
		}
	}

	protected virtual void IntegrateParentResult(EnumResult erParent)
	{
		if (erParent != null)
		{
			SqlEnumResult sqlEnumResult = (SqlEnumResult)erParent;
			StatementBuilder.Merge(sqlEnumResult.StatementBuilder);
			sqlEnumResult.StatementBuilder = StatementBuilder;
		}
	}

	private void AddXpathFilter()
	{
		FilterDecoder filterDecoder = new FilterDecoder(this);
		string sql = filterDecoder.GetSql(base.Filter);
		if (sql.Length != 0 && !CalculateParentRequest)
		{
			StatementBuilder.AddWhere(sql);
		}
	}

	internal void PrepareGetData(EnumResult erParent)
	{
		BuildStatement(erParent);
		IntegrateParentResult(erParent);
		RegisterPostProcessHits();
	}

	public override EnumResult GetData(EnumResult erParent)
	{
		PrepareGetData(erParent);
		BeforeStatementExecuted(ObjectName);
		return BuildResult(erParent);
	}

	protected virtual void BeforeStatementExecuted(string levelName)
	{
	}

	protected void BuildStatement(EnumResult erParent)
	{
		m_sb = new StatementBuilder();
		AddParentLinkProperties();
		AddRequestProperties();
		FillPrefixPostfix();
		AddXpathFilter();
		if (Distinct)
		{
			StatementBuilder.Distinct = true;
		}
		if (erParent != null && ((SqlEnumResult)erParent).MultipleDatabases)
		{
			AddOrderByAcrossDatabases();
		}
		else
		{
			AddOrderByInDatabase();
		}
	}

	internal void AddSpecialQuery(string database, string query)
	{
		if (m_SpecialQuery == null)
		{
			m_SpecialQuery = new SortedList(StringComparer.Ordinal);
		}
		m_SpecialQuery.Add(database, query);
	}

	private void AddSpecialQueryToResult(SqlEnumResult result)
	{
		if (SpecialQuery == null)
		{
			return;
		}
		foreach (DictionaryEntry item in SpecialQuery)
		{
			result.AddSpecialQuery((string)item.Key, (string)item.Value);
		}
	}

	internal EnumResult BuildResult(EnumResult result)
	{
		if (result == null)
		{
			DatabaseEngineType databaseEngineType = DatabaseEngineType.Standalone;
			if (this is ISupportDatabaseEngineTypes)
			{
				databaseEngineType = ExecuteSql.GetDatabaseEngineType(base.ConnectionInfo);
			}
			result = new SqlEnumResult(StatementBuilder, ResultType.Reserved1, databaseEngineType);
		}
		SqlEnumResult sqlEnumResult = (SqlEnumResult)result;
		AddSpecialQueryToResult(sqlEnumResult);
		if (base.Request == null || ResultType.Reserved1 == base.Request.ResultType || ResultType.Reserved2 == base.Request.ResultType)
		{
			return sqlEnumResult;
		}
		ResultType resultType = base.Request.ResultType;
		if (base.Request.Urn.ToString().StartsWith("SqlServerCe", StringComparison.OrdinalIgnoreCase) || base.Request.ResultType == ResultType.Default)
		{
			resultType = ResultType.DataTable;
		}
		if (StatementBuilder.PostProcessList.Count > 0)
		{
			bool flag = false;
			{
				IDictionaryEnumerator dictionaryEnumerator = StatementBuilder.PostProcessList.GetEnumerator();
				try
				{
					if (dictionaryEnumerator.MoveNext())
					{
						_ = (DictionaryEntry)dictionaryEnumerator.Current;
						flag = true;
					}
				}
				finally
				{
					IDisposable disposable = dictionaryEnumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			if (flag)
			{
				if (resultType == ResultType.IDataReader)
				{
					TraceHelper.Trace("w", 268435456u, "IDataReader will be returned from a DataTable because post processing is needed");
				}
				resultType = ResultType.DataTable;
			}
		}
		object ob = FillDataWithUseFailure(sqlEnumResult, resultType);
		return new EnumResult(ob, resultType);
	}

	protected object FillDataWithUseFailure(SqlEnumResult sqlresult, ResultType resultType)
	{
		StringCollection stringCollection = sqlresult.BuildSql();
		try
		{
			return FillData(resultType, stringCollection, base.ConnectionInfo, sqlresult.StatementBuilder);
		}
		catch (ExecutionFailureException ex)
		{
			if (!(ex.InnerException is SqlException { Class: 16, State: 1, Number: 911 }))
			{
				throw;
			}
			stringCollection.Clear();
			stringCollection.Add(sqlresult.StatementBuilder.GetCreateTemporaryTableSqlConnect("#empty_result"));
			stringCollection.Add("select * from #empty_result\nDROP TABLE #empty_result");
			return FillData(resultType, stringCollection, base.ConnectionInfo, sqlresult.StatementBuilder);
		}
	}

	protected virtual object FillData(ResultType resultType, StringCollection sql, object connectionInfo, StatementBuilder sb)
	{
		int condition;
		switch (resultType)
		{
		case ResultType.IDataReader:
			return ExecuteSql.GetDataProvider(sql, connectionInfo, sb);
		default:
			condition = ((resultType == ResultType.DataSet) ? 1 : 0);
			break;
		case ResultType.DataTable:
			condition = 1;
			break;
		}
		TraceHelper.Assert((byte)condition != 0);
		DataTable dataTable = ExecuteSql.ExecuteWithResults(sql, connectionInfo, sb);
		if (resultType == ResultType.DataTable)
		{
			return dataTable;
		}
		DataSet dataSet = new DataSet();
		dataSet.Locale = CultureInfo.InvariantCulture;
		dataSet.Tables.Add(dataTable);
		return dataSet;
	}

	protected void ClearHits()
	{
		ConditionedSqlList.ClearHits();
		PropertyLinkList.ClearHits();
		PostProcessList.ClearHits();
	}

	public string AddPropertyForFilter(string name)
	{
		return AddFilterProperty(name);
	}

	public string AddConstantForFilter(string constantValue)
	{
		return constantValue;
	}

	private void AddLinkFields(ArrayList linkfields)
	{
		if (linkfields == null)
		{
			return;
		}
		foreach (LinkField linkfield in linkfields)
		{
			if (linkfield.Type == LinkFieldType.Parent)
			{
				m_LinkFields.Add(linkfield);
			}
		}
	}

	protected void ResolveLocalLinkLinks()
	{
		if (ParentLink != null)
		{
			AddLinkProperties(LinkFieldType.Local, ParentLink.LinkFields);
		}
		foreach (ConditionedSql propertyLink in PropertyLinkList)
		{
			AddLinkProperties(LinkFieldType.Local, propertyLink.LinkFields);
		}
		foreach (ConditionedSql conditionedSql in ConditionedSqlList)
		{
			AddLinkProperties(LinkFieldType.Local, conditionedSql.LinkFields);
		}
		ObjectProperty[] properties = GetProperties(ObjectPropertyUsages.Reserved1);
		ObjectProperty[] array = properties;
		for (int i = 0; i < array.Length; i++)
		{
			SqlObjectProperty sqlObjectProperty = (SqlObjectProperty)array[i];
			AddLinkProperties(LinkFieldType.Local, sqlObjectProperty.LinkFields);
		}
	}

	private void AddPostProcessTriggers()
	{
		foreach (SqlPostProcess postProcess in PostProcessList)
		{
			if (!postProcess.Used)
			{
				continue;
			}
			StringEnumerator enumerator2 = postProcess.TriggeredFields.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					string current = enumerator2.Current;
					SqlObjectProperty sqlProperty = GetSqlProperty(current, ObjectPropertyUsages.Reserved1);
					AddRequestProperty(sqlProperty, triggered: true);
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

	protected void StoreInitialState()
	{
		m_sb = null;
		ResolveLocalLinkLinks();
	}

	protected void RestoreInitialState()
	{
		m_sb = null;
		ClearHits();
		ResolveLocalLinkLinks();
	}

	private string GetRequestedParentSelect()
	{
		Request request = new Request();
		request.RequestFieldsTypes = RequestFieldsTypes.Request;
		request.ResultType = ResultType.Reserved2;
		request.Urn = base.Urn.Parent;
		request.Fields = new string[RequestParentSelect.Fields.Count];
		RequestParentSelect.Fields.CopyTo(request.Fields, 0);
		SqlEnumResult sqlEnumResult = (SqlEnumResult)new Enumerator().Process(base.ConnectionInfo, request);
		StatementBuilder statementBuilder = sqlEnumResult.StatementBuilder;
		statementBuilder.AddStoredProperties();
		statementBuilder.ClearPrefixPostfix();
		return statementBuilder.SqlStatement;
	}

	private void FillPrefixPostfix()
	{
		if (RequestParentSelect == null)
		{
			return;
		}
		StringEnumerator enumerator = RequestParentSelect.Fields.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				ConditionedSqlList.AddHits(this, current, m_sb);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	protected internal virtual string ResolveComputedField(string fieldName)
	{
		return fieldName switch
		{
			"ParentSelect" => GetRequestedParentSelect(), 
			"NType" => ObjectName, 
			_ => null, 
		};
	}

	public override void PostProcess(EnumResult erChildren)
	{
		RestoreInitialState();
	}

	internal new string GetAliasPropertyName(string prop)
	{
		return base.GetAliasPropertyName(prop);
	}

	protected void AddOrderByInDatabase()
	{
		AddOrderByDatabase(bAcrossDatabases: false);
	}

	protected void AddOrderByAcrossDatabases()
	{
		AddOrderByDatabase(bAcrossDatabases: true);
	}

	private void AddOrderByDatabase(bool bAcrossDatabases)
	{
		if (base.Request != null && base.Request.OrderByList != null && StatementBuilder != null)
		{
			OrderBy[] orderByList = base.Request.OrderByList;
			foreach (OrderBy orderBy in orderByList)
			{
				AddOrderByDatabase(orderBy.Field, orderBy.Dir, bAcrossDatabases, bHiddenField: false);
			}
		}
	}

	private void AddOrderByDatabase(string field, OrderBy.Direction dir, bool bAcrossDatabases, bool bHiddenField)
	{
		string orderByValue = AddOrderByProperty(field, bHiddenField);
		StringCollection stringCollection = (StringCollection)OrderByRedirect[field];
		if (stringCollection != null)
		{
			StringEnumerator enumerator = stringCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					AddOrderByDatabase(current, dir, bAcrossDatabases, bHiddenField: true);
				}
				return;
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}
		string text;
		if (bAcrossDatabases)
		{
			text = GetAliasPropertyName(field);
			orderByValue = text;
		}
		else
		{
			text = field;
		}
		StatementBuilder.AddOrderBy(text, orderByValue, dir);
	}

	internal string GetFixedFilterValue(string field)
	{
		FilterNodeConstant filterNodeConstant = (FilterNodeConstant)base.FixedProperties[field];
		if (filterNodeConstant == null)
		{
			return null;
		}
		if (filterNodeConstant.ObjType == FilterNodeConstant.ObjectType.String)
		{
			return string.Format(CultureInfo.InvariantCulture, "N'{0}'", new object[1] { filterNodeConstant.ValueAsString });
		}
		return filterNodeConstant.ValueAsString;
	}
}
