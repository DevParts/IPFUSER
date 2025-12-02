using System;
using System.Collections;
using System.Data;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcObjectQuery
{
	private SfcQueryExpression query;

	private ISfcDomain domain;

	private ISfcConnection domainConn;

	private IAlienRoot nonSfcRoot;

	private string[] fields;

	private OrderBy[] orderByFields;

	private SfcObjectQueryMode activeQueriesMode;

	public SfcQueryExpression SfcQueryExpression => query;

	public SfcObjectQueryMode ActiveQueriesMode
	{
		get
		{
			return activeQueriesMode;
		}
		set
		{
			activeQueriesMode = value;
		}
	}

	public SfcObjectQuery(ISfcDomain domain, SfcObjectQueryMode activeQueriesMode)
	{
		Init(domain, activeQueriesMode);
	}

	public SfcObjectQuery(ISfcDomain domain)
	{
		Init(domain, SfcObjectQueryMode.CachedQuery);
	}

	public SfcObjectQuery(IAlienRoot root)
	{
		nonSfcRoot = root;
	}

	public DataTable ExecuteDataTable(SfcQueryExpression query, string[] fields, OrderBy[] orderByFields)
	{
		this.query = query;
		this.fields = fields;
		this.orderByFields = orderByFields;
		ValidateQueryExpression(this.query);
		if (nonSfcRoot != null)
		{
			return nonSfcRoot.SfcHelper_GetDataTable(nonSfcRoot.ConnectionContext, query.ToString(), fields, orderByFields);
		}
		if (domain.ConnectionContext.Mode == SfcConnectionContextMode.Offline)
		{
			return new DataTable();
		}
		return Enumerator.GetData(domainConn.ToEnumeratorObject(), query.ToString(), fields, orderByFields);
	}

	public IEnumerable ExecuteIterator(SfcQueryExpression query, string[] fields, OrderBy[] orderByFields)
	{
		this.query = query;
		this.fields = fields;
		this.orderByFields = orderByFields;
		ValidateQueryExpression(this.query);
		if (nonSfcRoot != null)
		{
			return new NonSfcObjectIterator(nonSfcRoot, activeQueriesMode, this.query, this.fields, this.orderByFields);
		}
		return new SfcObjectIterator(domain, activeQueriesMode, this.query, this.fields, this.orderByFields);
	}

	private void Init(ISfcDomain domain, SfcObjectQueryMode mode)
	{
		this.domain = domain;
		activeQueriesMode = mode;
		TraceHelper.Assert(null != domain);
		domainConn = domain.GetConnection();
		TraceHelper.Assert(domainConn != null || this.domain.ConnectionContext.Mode == SfcConnectionContextMode.Offline);
	}

	private void ValidateQueryExpression(SfcQueryExpression query)
	{
		if (nonSfcRoot != null && !query.ToString().StartsWith("Server", StringComparison.OrdinalIgnoreCase))
		{
			throw new SfcInvalidQueryExpressionException(SfcStrings.InvalidSMOQuery(query.ToString()));
		}
	}
}
