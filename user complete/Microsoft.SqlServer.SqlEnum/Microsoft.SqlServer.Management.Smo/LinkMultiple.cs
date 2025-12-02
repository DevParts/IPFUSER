using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class LinkMultiple
{
	private string m_no;

	private string m_expression;

	private ArrayList m_listLink;

	private bool HasLinkFields => null != m_listLink;

	public ArrayList LinkFields => m_listLink;

	public string No
	{
		get
		{
			return m_no;
		}
		set
		{
			m_no = value;
		}
	}

	internal void Init(XmlReadMultipleLink xrpl)
	{
		m_listLink = new ArrayList();
		m_no = xrpl.No;
		m_expression = xrpl.Expression;
		XmlReadLinkFields linkFields = xrpl.LinkFields;
		do
		{
			LinkField linkField = new LinkField();
			linkField.Type = linkFields.Type;
			linkField.Field = linkFields.Field;
			linkField.Value = linkFields.DefaultValue;
			m_listLink.Add(linkField);
		}
		while (linkFields.Next());
	}

	public void SetLinkFields(ArrayList list)
	{
		m_listLink = list;
	}

	public string GetSqlExpression(SqlObjectBase obj)
	{
		if (!HasLinkFields)
		{
			return m_expression;
		}
		foreach (LinkField item in m_listLink)
		{
			if (LinkFieldType.Computed == item.Type)
			{
				item.Value = obj.ResolveComputedField(item.Field);
			}
			else if (LinkFieldType.Filter == item.Type)
			{
				string fixedFilterValue = obj.GetFixedFilterValue(item.Field);
				if (fixedFilterValue != null)
				{
					item.Value = fixedFilterValue;
				}
				else if (item.Value == null)
				{
					item.Value = string.Empty;
				}
			}
		}
		int num = int.Parse(m_no, CultureInfo.InvariantCulture);
		object[] array = new object[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ((LinkField)m_listLink[i]).Value;
		}
		return string.Format(CultureInfo.InvariantCulture, m_expression, array);
	}

	internal void SetSqlExpression(string expr)
	{
		m_expression = expr;
	}
}
