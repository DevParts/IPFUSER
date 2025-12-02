using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[AttributeUsage(AttributeTargets.All)]
public class PropertyOrderAttribute : Attribute
{
	private int m_iOrder = -1;

	public int Order => m_iOrder;

	public PropertyOrderAttribute(int iOrder)
	{
		m_iOrder = iOrder;
	}
}
