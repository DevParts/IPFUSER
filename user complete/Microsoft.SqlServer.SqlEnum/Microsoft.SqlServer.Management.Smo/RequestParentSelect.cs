using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class RequestParentSelect
{
	private StringCollection m_Fields;

	public StringCollection Fields => m_Fields;

	public RequestParentSelect(XmlRequestParentSelect xrrps)
	{
		m_Fields = new StringCollection();
		XmlRequestParentSelectField field = xrrps.Field;
		do
		{
			m_Fields.Add(field.Name);
		}
		while (field.Next());
	}
}
