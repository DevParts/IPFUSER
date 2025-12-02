using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
internal class ParentLink
{
	private LinkMultiple m_LinkMultiple;

	public ArrayList LinkFields => m_LinkMultiple.LinkFields;

	public LinkMultiple LinkMultiple => m_LinkMultiple;

	public ParentLink(XmlReadParentLink xrpl)
	{
		m_LinkMultiple = new LinkMultiple();
		XmlReadSimpleParentLink simpleParentLink = xrpl.SimpleParentLink;
		if (simpleParentLink != null)
		{
			Init(simpleParentLink);
			return;
		}
		XmlReadMultipleLink multipleLink = xrpl.MultipleLink;
		if (multipleLink != null)
		{
			m_LinkMultiple.Init(multipleLink);
		}
	}

	internal void Init(XmlReadSimpleParentLink xrspl)
	{
		m_LinkMultiple.SetLinkFields(new ArrayList());
		string text = string.Empty;
		int num = 0;
		do
		{
			LinkField linkField = new LinkField();
			linkField.Type = LinkFieldType.Local;
			linkField.Field = xrspl.Local;
			m_LinkMultiple.LinkFields.Add(linkField);
			linkField = new LinkField();
			linkField.Type = LinkFieldType.Parent;
			linkField.Field = xrspl.Parent;
			m_LinkMultiple.LinkFields.Add(linkField);
			if (num > 0)
			{
				text += " AND ";
			}
			text += string.Format(CultureInfo.InvariantCulture, "{{{0}}}={{{1}}}", new object[2]
			{
				num++,
				num++
			});
		}
		while (xrspl.Next());
		m_LinkMultiple.SetSqlExpression(text);
		m_LinkMultiple.No = num.ToString(CultureInfo.InvariantCulture);
	}
}
