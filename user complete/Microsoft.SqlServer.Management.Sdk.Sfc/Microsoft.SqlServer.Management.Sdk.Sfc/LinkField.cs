using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class LinkField
{
	private string m_field;

	private string m_value;

	private LinkFieldType m_type;

	public LinkFieldType Type
	{
		get
		{
			return m_type;
		}
		set
		{
			m_type = value;
		}
	}

	public string Field
	{
		get
		{
			return m_field;
		}
		set
		{
			m_field = value;
		}
	}

	public string Value
	{
		get
		{
			return m_value;
		}
		set
		{
			m_value = value;
		}
	}
}
