namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessBodyText : PostProcessText
{
	private int m_idx;

	private int m_idxEnd;

	private int IdxStart
	{
		get
		{
			if (-2 == m_idx)
			{
				m_idx = DdlTextParser.ParseDdlHeader((string)m_text);
			}
			return m_idx;
		}
	}

	private int IdxEnd
	{
		get
		{
			if ("View" == base.ObjectName && -2 == m_idxEnd)
			{
				m_idxEnd = DdlTextParser.ParseCheckOption((string)m_text);
			}
			return m_idxEnd;
		}
	}

	private bool HasParantesis
	{
		get
		{
			if (-2 == m_idx)
			{
				DdlTextParser.ParseDdlHeader((string)m_text);
			}
			return DdlTextParser.ddlTextParserSingleton.hasParanthesis;
		}
	}

	private string TableVariableName
	{
		get
		{
			if (-2 == m_idx)
			{
				DdlTextParser.ParseDdlHeader((string)m_text);
			}
			return DdlTextParser.ddlTextParserSingleton.returnTableVariableName;
		}
	}

	protected override bool SupportDataReader => false;

	public PostProcessBodyText()
	{
		CleanRowData();
	}

	public override void CleanRowData()
	{
		base.CleanRowData();
		m_idx = -2;
		m_idxEnd = -2;
	}

	protected override string GetTextFor90(object data, DataProvider dp)
	{
		return GetTriggeredString(dp, 0);
	}

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		if (!base.IsTextSet)
		{
			object triggeredObject = GetTriggeredObject(dp, 0);
			SetText(triggeredObject, dp);
		}
		if (IsNull(m_text))
		{
			return m_text;
		}
		switch (name)
		{
		case "Text":
			return m_text;
		case "TextBody":
			return ((string)m_text).Remove(0, IdxStart);
		case "BodyStartIndex":
			return IdxStart;
		case "HasColumnSpecification":
			return HasParantesis;
		case "TableVariableName":
			if (TableVariableName != null)
			{
				return TableVariableName;
			}
			return string.Empty;
		default:
			return data;
		}
	}
}
