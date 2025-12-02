using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class FilterNodeConstant : FilterNode
{
	public enum ObjectType
	{
		Number,
		Boolean,
		String
	}

	private object m_value;

	private ObjectType m_objtype;

	public override Type NodeType => Type.Constant;

	public object Value => m_value;

	public ObjectType ObjType => m_objtype;

	public string ValueAsString => Value.ToString();

	public FilterNodeConstant(object value, ObjectType type)
	{
		m_value = value;
		m_objtype = type;
	}

	public static implicit operator string(FilterNodeConstant fnc)
	{
		if (fnc == null)
		{
			return null;
		}
		return Urn.EscapeString(fnc.ValueAsString);
	}

	internal static bool Compare(FilterNodeConstant f1, FilterNodeConstant f2, CompareOptions compInfo, CultureInfo cultureInfo)
	{
		if (f1.ObjType != f2.ObjType)
		{
			return false;
		}
		switch (f1.ObjType)
		{
		case ObjectType.Number:
		case ObjectType.Boolean:
		case ObjectType.String:
			return 0 == cultureInfo.CompareInfo.Compare(f1.Value.ToString(), f2.Value.ToString(), compInfo);
		default:
			return false;
		}
	}

	public override string ToString()
	{
		if (ObjType == ObjectType.String)
		{
			return $"'{Urn.EscapeString(Value.ToString())}'";
		}
		if (ObjType == ObjectType.Boolean)
		{
			if (!(bool)m_value)
			{
				return "false()";
			}
			return "true()";
		}
		return Value.ToString();
	}

	public override int GetHashCode()
	{
		return m_value.GetHashCode();
	}
}
