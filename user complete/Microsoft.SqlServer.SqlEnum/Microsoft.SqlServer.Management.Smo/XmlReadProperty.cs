using System;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlReadProperty : XmlRead
{
	public string Name => base.Reader["name"];

	public bool ReadOnly
	{
		get
		{
			string text = base.Reader["read_only"];
			if (text != null)
			{
				return bool.Parse(text);
			}
			text = base.Reader["access"];
			if (text != null)
			{
				if (string.Compare("read", text, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
				return false;
			}
			string name = Name;
			if ("Urn" == name || "ID" == name || "Name" == name || "CreateDate" == name || "Schema" == name)
			{
				return true;
			}
			return false;
		}
	}

	public string ClrType
	{
		get
		{
			string text = base.Reader["report_type"];
			if (text != null)
			{
				return "Microsoft.SqlServer.Management.Smo." + text;
			}
			text = base.Reader["report_type2"];
			if (text != null)
			{
				return text;
			}
			return Util.DbTypeToClrType(base.Reader["type"]);
		}
	}

	public bool ExtendedType => null != base.Reader["report_type"];

	public string DbType => base.Reader["type"];

	public bool Expensive
	{
		get
		{
			string text = base.Reader["expensive"];
			if (text == null)
			{
				return false;
			}
			return bool.Parse(text);
		}
	}

	public bool ReadOnlyAfterCreation
	{
		get
		{
			string text = base.Reader["read_only_after_creation"];
			if (text == null)
			{
				return false;
			}
			return bool.Parse(text);
		}
	}

	public short KeyIndex
	{
		get
		{
			string text = base.Reader["key_index"];
			if (text == null)
			{
				return -1;
			}
			return short.Parse(text);
		}
	}

	public PropertyMode PropertyMode
	{
		get
		{
			string text = base.Reader["mode"];
			PropertyMode propertyMode = PropertyMode.None;
			if (text != null)
			{
				text = text.ToUpperInvariant();
				if (-1 != text.LastIndexOf("DESIGN", StringComparison.Ordinal))
				{
					propertyMode |= PropertyMode.Design;
				}
				if (-1 != text.LastIndexOf("DEPLOY", StringComparison.Ordinal))
				{
					propertyMode |= PropertyMode.Deploy;
				}
			}
			return propertyMode;
		}
	}

	public bool Cast
	{
		get
		{
			string text = base.Reader["cast"];
			if (text == null)
			{
				return false;
			}
			return bool.Parse(text);
		}
	}

	public bool Hidden => null != base.Reader["hidden"];

	public ObjectPropertyUsages Usage
	{
		get
		{
			string text = base.Reader["usage"];
			ObjectPropertyUsages result = ObjectPropertyUsages.Reserved1;
			if (text != null)
			{
				text = text.ToLowerInvariant();
				result = ObjectPropertyUsages.Reserved1;
				if (-1 != text.LastIndexOf("request", StringComparison.Ordinal))
				{
					result |= ObjectPropertyUsages.Request;
				}
				if (-1 != text.LastIndexOf("filter", StringComparison.Ordinal))
				{
					result |= ObjectPropertyUsages.Filter;
				}
				if (-1 != text.LastIndexOf("order", StringComparison.Ordinal))
				{
					result |= ObjectPropertyUsages.OrderBy;
				}
				return result;
			}
			text = base.Reader["notusage"];
			if (text != null)
			{
				text = text.ToLowerInvariant();
				result = ObjectPropertyUsages.All | ObjectPropertyUsages.Reserved1;
				if (-1 != text.LastIndexOf("request", StringComparison.Ordinal))
				{
					result &= ~ObjectPropertyUsages.Request;
				}
				if (-1 != text.LastIndexOf("filter", StringComparison.Ordinal))
				{
					result &= ~ObjectPropertyUsages.Filter;
				}
				if (-1 != text.LastIndexOf("order", StringComparison.Ordinal))
				{
					result &= ~ObjectPropertyUsages.OrderBy;
				}
				return result;
			}
			if (Hidden)
			{
				return result;
			}
			return ObjectPropertyUsages.All | ObjectPropertyUsages.Reserved1;
		}
	}

	public string Value => GetAliasString(GetTextOfElement());

	public string Table => base.Reader["table"];

	public string Link => base.Reader["link"];

	public string Size => base.Reader["size"];

	public bool HasPropertyLink
	{
		get
		{
			if (Table == null)
			{
				return null != Link;
			}
			return true;
		}
	}

	public XmlReadMultipleLink MultipleLink
	{
		get
		{
			if (XmlUtility.IsElement(base.Reader, "link_multiple"))
			{
				return new XmlReadMultipleLink(this);
			}
			return null;
		}
	}

	public XmlReadProperty(XmlRead xmlReader)
		: base(xmlReader)
	{
	}
}
