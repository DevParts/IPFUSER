using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class DatabaseLevel : SqlObject, ISupportInitDatabaseEngineData
{
	private string m_PropertyNameForDatabase;

	private string m_XmlAssembly;

	private bool m_bLastDatabaseLevel;

	private bool m_bForChildren;

	public string NameProperty => m_PropertyNameForDatabase;

	public override Assembly ResourceAssembly
	{
		get
		{
			if (m_XmlAssembly == null)
			{
				return base.ResourceAssembly;
			}
			return Util.LoadAssembly(m_XmlAssembly);
		}
	}

	public DatabaseLevel()
	{
		m_PropertyNameForDatabase = "Name";
		m_bLastDatabaseLevel = false;
		m_bForChildren = false;
		m_XmlAssembly = null;
	}

	public override Request RetrieveParentRequest()
	{
		if (ResultType.Reserved2 == base.Request.ResultType || ResultType.Reserved1 == base.Request.ResultType)
		{
			m_bForChildren = true;
		}
		return base.RetrieveParentRequest();
	}

	public override EnumResult GetData(EnumResult res)
	{
		if (ResultType.Reserved2 == base.Request.ResultType)
		{
			return res;
		}
		if (ResultType.Reserved1 != base.Request.ResultType)
		{
			return base.GetData(res);
		}
		if (!base.SqlRequest.ResolveDatabases)
		{
			m_bForChildren = false;
			return base.GetData(res);
		}
		SqlEnumResult sqlEnumResult = (SqlEnumResult)res;
		sqlEnumResult.Databases = GetRequestedDatabases(sqlEnumResult);
		sqlEnumResult.NameProperties.Add(NameProperty);
		CleanupFilter();
		SqlObjectProperty sqlProperty = GetSqlProperty(NameProperty, ObjectPropertyUsages.Request | ObjectPropertyUsages.Reserved1);
		string value = sqlProperty.GetValue(this);
		StringCollection stringCollection = null;
		int num = sqlEnumResult.Level - 1;
		if (!m_bLastDatabaseLevel)
		{
			sqlProperty.Value = "{" + (2 * num).ToString(CultureInfo.InvariantCulture) + "}";
		}
		else
		{
			sqlProperty.Value = "db_name()";
			sqlEnumResult.LastDbLevelSet = true;
		}
		if (sqlEnumResult.Level > 1)
		{
			stringCollection = new StringCollection();
			for (int i = 0; i < base.PropertyLinkList.Count; i++)
			{
				SqlPropertyLink sqlPropertyLink = (SqlPropertyLink)base.PropertyLinkList[i];
				stringCollection.Add(sqlPropertyLink.Table);
				sqlPropertyLink.Table = "[{" + (2 * num - 1).ToString(CultureInfo.InvariantCulture) + "}]." + sqlPropertyLink.Table;
			}
		}
		try
		{
			SqlEnumResult sqlEnumResult2 = (SqlEnumResult)base.GetData((EnumResult)sqlEnumResult);
			sqlEnumResult2.StatementBuilder.ClearFailCondition();
			int i2 = 0;
			if (base.PropertyLinkList[i2].IsUsed)
			{
				sqlEnumResult2.StatementBuilder.AddWhere(value + "=" + sqlProperty.Value);
			}
			return sqlEnumResult2;
		}
		finally
		{
			sqlProperty.Value = value;
			if (stringCollection != null)
			{
				for (int j = 0; j < base.PropertyLinkList.Count; j++)
				{
					SqlPropertyLink sqlPropertyLink2 = (SqlPropertyLink)base.PropertyLinkList[j];
					sqlPropertyLink2.Table = stringCollection[j];
				}
			}
			m_bForChildren = false;
		}
	}

	protected override string AddLinkProperty(string name)
	{
		if (m_bForChildren && IsDatabaseNameOrDerivate(name))
		{
			AddConditionalsJustPropDependencies(name);
			SqlObjectProperty sqlObjectProperty = (SqlObjectProperty)GetProperty(name, ObjectPropertyUsages.Reserved1);
			return sqlObjectProperty.GetValue(this);
		}
		return base.AddLinkProperty(name);
	}

	internal DataTable GetRequestedDatabases(SqlEnumResult serParent)
	{
		TraceHelper.Assert(m_bForChildren, "should only be called when this is an intermediate level");
		string fixedStringProperty = GetFixedStringProperty(NameProperty, removeEscape: true);
		if (fixedStringProperty != null)
		{
			DataTable dataTable = new DataTable();
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add("Name", Type.GetType("System.String"));
			DataRow dataRow = dataTable.NewRow();
			dataRow[0] = fixedStringProperty;
			dataTable.Rows.Add(dataRow);
			return dataTable;
		}
		Request request = new Request();
		request.RequestFieldsTypes = RequestFieldsTypes.Request;
		request.ResultType = ResultType.DataTable;
		request.Urn = base.Urn;
		request.Fields = new string[1] { NameProperty };
		int count = serParent.NameProperties.Count;
		if (count > 0)
		{
			request.ParentPropertiesRequests = new PropertiesRequest[count];
			for (int i = 0; i < count; i++)
			{
				request.ParentPropertiesRequests[i] = new PropertiesRequest();
				request.ParentPropertiesRequests[i].Fields = new string[1] { serParent.NameProperties[count - i - 1] };
			}
		}
		return new Enumerator().Process(base.ConnectionInfo, request);
	}

	public new void LoadInitData(string init_data, ServerVersion ver, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml("<d " + init_data + " />");
		XmlElement documentElement = xmlDocument.DocumentElement;
		XmlAttribute xmlAttribute = documentElement.Attributes["db_name"];
		if (xmlAttribute != null)
		{
			m_PropertyNameForDatabase = xmlAttribute.Value;
		}
		xmlAttribute = documentElement.Attributes["xml"];
		string file = null;
		if (xmlAttribute != null)
		{
			file = xmlAttribute.Value;
		}
		xmlAttribute = documentElement.Attributes["assemly"];
		if (xmlAttribute != null)
		{
			m_XmlAssembly = xmlAttribute.Value;
		}
		xmlAttribute = documentElement.Attributes["last"];
		if (xmlAttribute != null)
		{
			m_bLastDatabaseLevel = true;
		}
		base.LoadInitData(file, ver, databaseEngineType, databaseEngineEdition);
	}

	private bool IsDatabaseNameOrDerivate(string fieldName)
	{
		TraceHelper.Assert(m_bForChildren, "should only be called when this is an intermediate level");
		if (fieldName == NameProperty)
		{
			return true;
		}
		SqlObjectProperty sqlProperty = GetSqlProperty(fieldName, ObjectPropertyUsages.Reserved1);
		if (sqlProperty.LinkFields == null)
		{
			return false;
		}
		foreach (LinkField linkField in sqlProperty.LinkFields)
		{
			if (LinkFieldType.Local == linkField.Type && NameProperty != linkField.Field)
			{
				return false;
			}
		}
		return true;
	}

	private void CleanupFilter()
	{
		TraceHelper.Assert(m_bForChildren, "should only be called when this is an intermediate level");
		if ((base.SqlRequest.Fields == null || base.SqlRequest.Fields.Length == 0 || 2 > base.SqlRequest.Fields.Length) && (base.SqlRequest.LinkFields == null || base.SqlRequest.LinkFields.Count == 0 || 2 > base.SqlRequest.LinkFields.Count))
		{
			base.Filter = null;
			return;
		}
		if (base.SqlRequest.Fields != null)
		{
			string[] fields = base.SqlRequest.Fields;
			foreach (string fieldName in fields)
			{
				if (!IsDatabaseNameOrDerivate(fieldName))
				{
					return;
				}
			}
		}
		else if (base.SqlRequest.LinkFields != null)
		{
			foreach (LinkField linkField in base.SqlRequest.LinkFields)
			{
				if (!IsDatabaseNameOrDerivate(linkField.Field))
				{
					return;
				}
			}
		}
		base.Filter = null;
	}

	public override void PostProcess(EnumResult erChildren)
	{
		m_bForChildren = false;
		base.PostProcess(erChildren);
	}
}
