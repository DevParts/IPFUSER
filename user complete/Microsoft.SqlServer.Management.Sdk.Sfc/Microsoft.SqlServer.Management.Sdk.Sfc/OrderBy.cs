using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
[ComVisible(false)]
public class OrderBy
{
	public enum Direction
	{
		Asc,
		Desc
	}

	private string m_field;

	private Direction m_direction;

	[XmlAttribute]
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

	[XmlAttribute]
	public Direction Dir
	{
		get
		{
			return m_direction;
		}
		set
		{
			m_direction = value;
		}
	}

	public OrderBy()
	{
	}

	public OrderBy(string field, Direction dir)
	{
		Field = field;
		Dir = dir;
	}
}
