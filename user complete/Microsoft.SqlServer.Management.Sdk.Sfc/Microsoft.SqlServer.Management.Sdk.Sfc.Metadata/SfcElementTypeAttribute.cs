using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class SfcElementTypeAttribute : Attribute
{
	private string m_elementTypeName;

	public string ElementTypeName
	{
		get
		{
			return m_elementTypeName;
		}
		set
		{
			m_elementTypeName = value;
		}
	}

	public SfcElementTypeAttribute(string elementTypeName)
	{
		m_elementTypeName = elementTypeName;
	}
}
