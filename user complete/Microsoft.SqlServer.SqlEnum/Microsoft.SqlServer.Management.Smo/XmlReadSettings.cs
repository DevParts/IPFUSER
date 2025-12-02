using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlReadSettings : XmlRead
{
	public string MainTable => base.Reader["main_table"];

	public string AdditionalFilter => base.Reader["aditional_filter"];

	public bool Distinct
	{
		get
		{
			string text = base.Reader["distinct"];
			if (text == null)
			{
				return false;
			}
			return bool.Parse(text);
		}
	}

	public bool HasPropertyLink
	{
		get
		{
			if (MainTable == null)
			{
				return null != AdditionalFilter;
			}
			return true;
		}
	}

	public XmlReadParentLink ParentLink
	{
		get
		{
			if (!XmlUtility.SelectNextElement(base.Reader))
			{
				return null;
			}
			if (IsElementWithCheckVersion("parent_link"))
			{
				return new XmlReadParentLink(this);
			}
			return null;
		}
	}

	public XmlReadConditionedStatementFailCondition FailCondition
	{
		get
		{
			if (IsElementWithCheckVersion("fail_condition"))
			{
				return new XmlReadConditionedStatementFailCondition(this);
			}
			return null;
		}
	}

	public XmlRequestParentSelect RequestParentSelect
	{
		get
		{
			if (IsElementWithCheckVersion("request_parent_select"))
			{
				return new XmlRequestParentSelect(this);
			}
			return null;
		}
	}

	public XmlReadInclude Include
	{
		get
		{
			if (IsElementWithCheckVersion("include"))
			{
				return new XmlReadInclude(this);
			}
			return null;
		}
	}

	public XmlReadPropertyLink PropertyLink
	{
		get
		{
			if (IsElementWithCheckVersion("property_link"))
			{
				return new XmlReadPropertyLink(this);
			}
			return null;
		}
	}

	public XmlReadConditionedStatementPrefix Prefix
	{
		get
		{
			if (IsElementWithCheckVersion("prefix"))
			{
				return new XmlReadConditionedStatementPrefix(this);
			}
			return null;
		}
	}

	public XmlReadConditionedStatementPostfix Postfix
	{
		get
		{
			if (IsElementWithCheckVersion("postfix"))
			{
				return new XmlReadConditionedStatementPostfix(this);
			}
			return null;
		}
	}

	public XmlReadConditionedStatementPostProcess PostProcess
	{
		get
		{
			if (IsElementWithCheckVersion("post_process"))
			{
				return new XmlReadConditionedStatementPostProcess(this);
			}
			return null;
		}
	}

	public XmlReadOrderByRedirect OrderByRedirect
	{
		get
		{
			if (IsElementWithCheckVersion("orderby_redirect"))
			{
				return new XmlReadOrderByRedirect(this);
			}
			return null;
		}
	}

	public XmlReadSpecialQuery SpecialQuery
	{
		get
		{
			if (IsElementWithCheckVersion("special_query"))
			{
				return new XmlReadSpecialQuery(this);
			}
			return null;
		}
	}

	public XmlReadSettings(XmlRead xmlReader)
		: base(xmlReader)
	{
	}
}
