namespace Microsoft.SqlServer.Management.Smo;

internal class SqlTypeConvert
{
	private string m_typeName;

	private int m_typeNo;

	private string m_path;

	public string Name
	{
		get
		{
			return m_typeName;
		}
		set
		{
			m_typeName = value;
		}
	}

	public int No
	{
		get
		{
			return m_typeNo;
		}
		set
		{
			m_typeNo = value;
		}
	}

	public string Path
	{
		get
		{
			return m_path;
		}
		set
		{
			m_path = value;
		}
	}

	public SqlTypeConvert(string name, int no, string path)
	{
		Name = name;
		No = no;
		Path = path;
	}
}
