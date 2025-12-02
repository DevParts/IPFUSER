using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class FilterNodeFunction : FilterNodeChildren
{
	public new enum Type
	{
		True,
		False,
		String,
		Contains,
		UserDefined,
		Not,
		Boolean,
		Like,
		In
	}

	private Type m_funcType;

	private string m_name;

	public override FilterNode.Type NodeType => FilterNode.Type.Function;

	public string Name => m_name;

	public Type FunctionType => m_funcType;

	public int ParameterCount => base.Children.Length;

	internal FilterNodeFunction(Type funcType, string name)
	{
		m_funcType = funcType;
		m_name = name;
	}

	public FilterNodeFunction(Type funcType, string name, params FilterNode[] args)
		: base(args)
	{
		m_funcType = funcType;
		m_name = name;
	}

	public FilterNode GetParameter(int index)
	{
		return base.Children[index];
	}

	internal static bool Compare(FilterNodeFunction f1, FilterNodeFunction f2, CompareOptions compInfo, CultureInfo cultureInfo)
	{
		if (f1.FunctionType != f2.FunctionType)
		{
			return false;
		}
		return FilterNodeChildren.Compare(f1, f2, compInfo, cultureInfo);
	}

	public static string FuncTypeToString(Type type)
	{
		return type.ToString().ToLowerInvariant();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(FuncTypeToString(FunctionType));
		stringBuilder.Append("(");
		if (ParameterCount > 0)
		{
			stringBuilder.Append(GetParameter(0).ToString());
		}
		for (int i = 1; i < ParameterCount; i++)
		{
			stringBuilder.Append(", ");
			stringBuilder.Append(GetParameter(i).ToString());
		}
		stringBuilder.Append(")");
		return stringBuilder.ToString();
	}

	public override int GetHashCode()
	{
		return FunctionType.GetHashCode() ^ base.GetHashCode();
	}
}
