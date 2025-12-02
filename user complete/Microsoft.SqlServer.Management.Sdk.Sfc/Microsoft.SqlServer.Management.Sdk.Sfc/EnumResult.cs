using System;
using System.Data;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class EnumResult
{
	private ResultType m_resultType;

	private object m_data;

	public ResultType Type => m_resultType;

	public object Data
	{
		get
		{
			return m_data;
		}
		set
		{
			m_data = value;
		}
	}

	public string CommandText { get; set; }

	public TimeSpan CommandElapsedTime { get; set; }

	protected void SetType(ResultType type)
	{
		m_resultType = type;
	}

	public EnumResult(object ob, ResultType resultType)
	{
		m_data = ob;
		m_resultType = resultType;
	}

	public EnumResult()
	{
	}

	public static implicit operator DataSet(EnumResult er)
	{
		if (er.m_resultType == ResultType.DataSet)
		{
			return (DataSet)er.m_data;
		}
		if (er.m_resultType == ResultType.DataTable)
		{
			DataSet dataSet = new DataSet();
			dataSet.Locale = CultureInfo.InvariantCulture;
			dataSet.Tables.Add((DataTable)er.m_data);
			return dataSet;
		}
		throw new ResultTypeNotSupportedEnumeratorException(ResultType.DataSet);
	}

	public static DataSet ConvertToDataSet(EnumResult er)
	{
		return er;
	}

	public static implicit operator DataTable(EnumResult er)
	{
		if (er.m_resultType == ResultType.DataTable)
		{
			return (DataTable)er.m_data;
		}
		if (er.m_resultType == ResultType.DataSet)
		{
			if (((DataSet)er.Data).Tables[0] != null)
			{
				return ((DataSet)er.Data).Tables[0];
			}
			return null;
		}
		throw new ResultTypeNotSupportedEnumeratorException(ResultType.DataTable);
	}

	public static DataTable ConvertToDataTable(EnumResult er)
	{
		return er;
	}

	public static implicit operator XmlDocument(EnumResult er)
	{
		if (er.m_resultType != ResultType.XmlDocument)
		{
			throw new ResultTypeNotSupportedEnumeratorException(ResultType.XmlDocument);
		}
		return (XmlDocument)er.m_data;
	}

	public static XmlDocument ConvertToXmlDocument(EnumResult er)
	{
		return er;
	}

	public static IDataReader ConvertToDataReader(EnumResult er)
	{
		return er.m_resultType switch
		{
			ResultType.DataTable => ((DataTable)er.Data).CreateDataReader(), 
			ResultType.IDataReader => (IDataReader)er.Data, 
			_ => throw new ResultTypeNotSupportedEnumeratorException(ResultType.IDataReader), 
		};
	}
}
