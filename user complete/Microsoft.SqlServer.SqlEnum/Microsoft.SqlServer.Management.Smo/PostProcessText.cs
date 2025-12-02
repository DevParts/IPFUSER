using System.Data;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessText : PostProcess
{
	protected object m_text;

	private bool m_btextSet;

	private DataTable m_dtText;

	protected bool IsTextSet => m_btextSet;

	protected override bool SupportDataReader
	{
		get
		{
			if (ExecuteSql.GetServerVersion(base.ConnectionInfo).Major < 9)
			{
				return false;
			}
			return true;
		}
	}

	public PostProcessText()
	{
		CleanRowData();
		m_dtText = null;
	}

	protected DataTable GetTextForAllRows()
	{
		Request request = new Request();
		request.Urn = base.Request.Urn.Value + "/Text";
		request.ResultType = ResultType.DataTable;
		request.RequestFieldsTypes = RequestFieldsTypes.Request;
		request.Fields = new string[3] { "ObjectIdentifier", "ID", "Text" };
		request.OrderByList = new OrderBy[2]
		{
			new OrderBy("ObjectIdentifier", OrderBy.Direction.Asc),
			new OrderBy("ID", OrderBy.Direction.Asc)
		};
		return new Enumerator().Process(base.ConnectionInfo, request);
	}

	protected string GetTextForObject(string sObjectIdentifier)
	{
		int num = BinarySearchSetOnFirst(m_dtText.Rows, sObjectIdentifier, "ObjectIdentifier");
		if (0 > num)
		{
			return string.Empty;
		}
		string text = string.Empty;
		do
		{
			text += (string)m_dtText.Rows[num++]["Text"];
		}
		while (num < m_dtText.Rows.Count && "1" != m_dtText.Rows[num]["ID"].ToString());
		return text;
	}

	protected void SetText(object data, DataProvider dp)
	{
		m_btextSet = true;
		if (IsNull(data))
		{
			m_text = string.Empty;
			return;
		}
		if (ExecuteSql.GetServerVersion(base.ConnectionInfo).Major < 9)
		{
			if (m_dtText == null)
			{
				m_dtText = GetTextForAllRows();
			}
			m_text = GetTextForObject((string)data);
		}
		else
		{
			m_text = GetTextFor90(data, dp);
		}
		if (m_text == null)
		{
			m_text = string.Empty;
		}
	}

	protected virtual string GetTextFor90(object data, DataProvider dp)
	{
		return (string)data;
	}

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		if (!IsTextSet)
		{
			SetText(data, dp);
		}
		return m_text;
	}

	public override void CleanRowData()
	{
		m_btextSet = false;
		m_text = null;
	}
}
