using System.Collections.Specialized;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessSplitFourPartName : PostProcess
{
	private StringCollection m_listNames;

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		if (m_listNames == null)
		{
			m_listNames = Util.SplitNames(GetTriggeredString(dp, 0), '.');
		}
		int num = 0;
		switch (name)
		{
		case "BaseSchema":
			num = 1;
			break;
		case "BaseDatabase":
			num = 2;
			break;
		case "BaseServer":
			num = 3;
			break;
		}
		if (num >= m_listNames.Count)
		{
			return data;
		}
		return m_listNames[num];
	}

	public override void CleanRowData()
	{
		m_listNames = null;
	}
}
