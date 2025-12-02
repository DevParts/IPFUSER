using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlReadProperties : XmlRead
{
	public XmlReadProperty Property
	{
		get
		{
			if (IsElementWithCheckVersion("property"))
			{
				return new XmlReadProperty(this);
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

	public XmlReadProperties(XmlRead xmlReader)
		: base(xmlReader)
	{
	}
}
