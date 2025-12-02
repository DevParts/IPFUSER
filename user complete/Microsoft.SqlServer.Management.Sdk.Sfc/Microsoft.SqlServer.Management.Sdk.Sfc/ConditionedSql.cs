using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public abstract class ConditionedSql
{
	private bool m_used;

	private LinkMultiple m_LinkMultiple;

	private StringCollection m_cols;

	protected StringCollection Fields => m_cols;

	public bool IsUsed => m_used;

	protected virtual bool AcceptsMultipleHits => false;

	public bool Used => m_used;

	public LinkMultiple LinkMultiple
	{
		get
		{
			return m_LinkMultiple;
		}
		set
		{
			m_LinkMultiple = value;
		}
	}

	public ArrayList LinkFields
	{
		get
		{
			if (m_LinkMultiple == null)
			{
				return null;
			}
			return m_LinkMultiple.LinkFields;
		}
	}

	protected ConditionedSql()
	{
		m_used = false;
		m_cols = new StringCollection();
	}

	public void SetFields(StringCollection fields)
	{
		m_cols = fields;
	}

	public bool IsHit(string field)
	{
		if (!AcceptsMultipleHits && m_used)
		{
			return false;
		}
		return TestHit(field);
	}

	protected bool TestHit(string field)
	{
		if (!IsDefault())
		{
			return m_cols.Contains(field);
		}
		return true;
	}

	public bool IsDefault()
	{
		return 0 == m_cols.Count;
	}

	public void MarkHit()
	{
		m_used = true;
	}

	public void ClearHit()
	{
		m_used = false;
	}

	public void AddLinkMultiple(XmlReadMultipleLink xrmpl)
	{
		if (xrmpl != null)
		{
			m_LinkMultiple = new LinkMultiple();
			m_LinkMultiple.Init(xrmpl);
		}
	}

	public abstract void AddHit(string field, SqlObjectBase obj, StatementBuilder sb);
}
