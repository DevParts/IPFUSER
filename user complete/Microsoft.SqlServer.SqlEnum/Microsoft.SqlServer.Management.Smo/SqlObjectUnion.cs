using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class SqlObjectUnion : SqlObject
{
	private ArrayList m_listObjects;

	public override ResultType[] ResultTypes => new ResultType[2]
	{
		ResultType.DataTable,
		ResultType.DataSet
	};

	private SqlObjectUnion()
	{
		m_listObjects = new ArrayList();
	}

	public override Request RetrieveParentRequest()
	{
		SqlRequest sqlRequest = (SqlRequest)base.RetrieveParentRequest();
		foreach (SqlObject listObject in m_listObjects)
		{
			listObject.Request = base.Request;
			listObject.SetUrn(base.Urn);
			sqlRequest.MergeLinkFileds((SqlRequest)listObject.RetrieveParentRequest());
		}
		return sqlRequest;
	}

	private void ProcessStatementBuilder(SqlEnumResult ser, SqlObject o, StringBuilder sql)
	{
		StatementBuilder statementBuilder = ser.StatementBuilder;
		ser.StatementBuilder = statementBuilder.MakeCopy();
		o.PrepareGetData(ser);
		ser.StatementBuilder = statementBuilder;
		o.StatementBuilder.AddStoredProperties();
		sql.Append(o.StatementBuilder.InternalSelect());
	}

	public override EnumResult GetData(EnumResult erParent)
	{
		StringBuilder stringBuilder = new StringBuilder();
		SqlEnumResult sqlEnumResult = (SqlEnumResult)erParent;
		ProcessStatementBuilder(sqlEnumResult, this, stringBuilder);
		foreach (SqlObject listObject in m_listObjects)
		{
			stringBuilder.Append("\nUNION\n");
			ProcessStatementBuilder(sqlEnumResult, listObject, stringBuilder);
		}
		base.StatementBuilder.SetInternalSelect(stringBuilder);
		sqlEnumResult.StatementBuilder = base.StatementBuilder;
		return BuildResult(erParent);
	}

	public override void PostProcess(EnumResult erChildren)
	{
		base.PostProcess(erChildren);
		foreach (SqlObject listObject in m_listObjects)
		{
			listObject.PostProcess(erChildren);
		}
	}

	public override void Initialize(object ci, XPathExpressionBlock block)
	{
		base.Initialize(ci, block);
		foreach (SqlObject listObject in m_listObjects)
		{
			listObject.Initialize(ci, block);
		}
	}

	protected internal override void LoadAndStore(XmlReadDoc xrd, Assembly assembly, StringCollection requestedFields, StringCollection roAfterCreation)
	{
		xrd.ReadUnion();
		base.LoadAndStore(xrd, assembly, requestedFields, roAfterCreation);
		while (xrd.ReadUnion())
		{
			SqlObject sqlObject = new SqlObject();
			sqlObject.LoadAndStore(xrd, assembly, requestedFields, roAfterCreation);
			m_listObjects.Add(sqlObject);
		}
	}
}
