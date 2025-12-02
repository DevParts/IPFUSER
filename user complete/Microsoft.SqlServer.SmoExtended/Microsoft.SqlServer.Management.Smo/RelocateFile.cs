namespace Microsoft.SqlServer.Management.Smo;

public class RelocateFile
{
	private string m_LogicalFileName;

	private string m_PhysicalFileName;

	public string LogicalFileName
	{
		get
		{
			return m_LogicalFileName;
		}
		set
		{
			m_LogicalFileName = value;
		}
	}

	public string PhysicalFileName
	{
		get
		{
			return m_PhysicalFileName;
		}
		set
		{
			m_PhysicalFileName = value;
		}
	}

	public RelocateFile()
	{
	}

	public RelocateFile(string logicalFileName, string physicalFileName)
	{
		m_LogicalFileName = logicalFileName;
		m_PhysicalFileName = physicalFileName;
	}
}
