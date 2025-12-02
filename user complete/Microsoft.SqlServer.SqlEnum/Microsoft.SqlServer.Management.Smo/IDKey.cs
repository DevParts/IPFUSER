using System;

namespace Microsoft.SqlServer.Management.Smo;

internal class IDKey : IComparable
{
	private int m_id;

	private int m_type;

	public int id
	{
		get
		{
			return m_id;
		}
		set
		{
			m_id = value;
		}
	}

	public int type
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

	internal IDKey(int id, int type)
	{
		m_id = id;
		m_type = type;
	}

	public int CompareTo(object o)
	{
		IDKey iDKey = (IDKey)o;
		if (type > iDKey.type)
		{
			return 1;
		}
		if (type < iDKey.type)
		{
			return -1;
		}
		if (id == iDKey.id)
		{
			return 0;
		}
		if (id <= iDKey.id)
		{
			return -1;
		}
		return 1;
	}
}
