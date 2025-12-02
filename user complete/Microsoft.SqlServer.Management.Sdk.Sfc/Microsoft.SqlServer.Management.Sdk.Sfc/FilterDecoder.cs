using System.Runtime.InteropServices;
using System.Text;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class FilterDecoder
{
	private ISqlFilterDecoderCallback m_isfdc;

	private StringBuilder m_sql;

	private bool m_bInFuncContains;

	private bool m_bInFuncLike;

	private string m_strPrefix;

	public string StringPrefix
	{
		get
		{
			return m_strPrefix;
		}
		set
		{
			m_strPrefix = value;
		}
	}

	public FilterDecoder(ISqlFilterDecoderCallback isfdc)
	{
		m_isfdc = isfdc;
		m_strPrefix = "N";
	}

	public string GetSql(FilterNode node)
	{
		m_sql = new StringBuilder();
		m_bInFuncContains = false;
		m_bInFuncLike = false;
		decode(node);
		return m_sql.ToString();
	}

	private string XPathOpToSqOp(FilterNodeOperator.Type op)
	{
		return op switch
		{
			FilterNodeOperator.Type.LT => "<", 
			FilterNodeOperator.Type.GT => ">", 
			FilterNodeOperator.Type.LE => "<=", 
			FilterNodeOperator.Type.GE => ">=", 
			FilterNodeOperator.Type.EQ => "=", 
			FilterNodeOperator.Type.NE => "<>", 
			FilterNodeOperator.Type.OR => " or ", 
			FilterNodeOperator.Type.And => " and ", 
			_ => throw new InvalidQueryExpressionEnumeratorException(SfcStrings.UnknownOperator), 
		};
	}

	private void decode(FilterNode node)
	{
		if (node == null)
		{
			return;
		}
		if (FilterNode.Type.Operator == node.NodeType)
		{
			FilterNodeOperator filterNodeOperator = (FilterNodeOperator)node;
			decode(filterNodeOperator.Left);
			m_sql.Append(XPathOpToSqOp(filterNodeOperator.OpType));
			decode(filterNodeOperator.Right);
		}
		else if (FilterNode.Type.Constant == node.NodeType)
		{
			FilterNodeConstant filterNodeConstant = (FilterNodeConstant)node;
			string text = m_isfdc.AddConstantForFilter(filterNodeConstant.Value.ToString());
			if (!m_isfdc.SupportsParameterization)
			{
				if (!m_bInFuncContains && !m_bInFuncLike && FilterNodeConstant.ObjectType.String == filterNodeConstant.ObjType)
				{
					m_sql.Append(m_strPrefix + "'");
				}
				if (m_bInFuncContains)
				{
					text = Util.EscapeLikePattern(text);
				}
				m_sql.Append(text);
				if (!m_bInFuncContains && !m_bInFuncLike && FilterNodeConstant.ObjectType.String == filterNodeConstant.ObjType)
				{
					m_sql.Append('\'');
				}
			}
			else
			{
				m_sql.AppendFormat("<msparam>{0}</msparam>", text);
			}
		}
		else if (FilterNode.Type.Group == node.NodeType)
		{
			FilterNodeGroup filterNodeGroup = (FilterNodeGroup)node;
			m_sql.Append('(');
			decode(filterNodeGroup.Node);
			m_sql.Append(')');
		}
		else if (node.NodeType == FilterNode.Type.Attribute)
		{
			decode((FilterNodeAttribute)node);
		}
		else if (FilterNode.Type.Function == node.NodeType)
		{
			decode((FilterNodeFunction)node);
		}
	}

	private void decode(FilterNodeFunction func)
	{
		QueryParameterizationMode parameterizationMode = ServerConnection.ParameterizationMode;
		switch (func.FunctionType)
		{
		case FilterNodeFunction.Type.True:
			m_sql.Append('1');
			break;
		case FilterNodeFunction.Type.False:
			m_sql.Append('0');
			break;
		case FilterNodeFunction.Type.String:
			decode(func.GetParameter(0));
			break;
		case FilterNodeFunction.Type.Contains:
			try
			{
				ServerConnection.ParameterizationMode = QueryParameterizationMode.None;
				decode(func.GetParameter(0));
				m_sql.Append(" like N'%");
				m_bInFuncContains = true;
				decode(func.GetParameter(1));
				m_bInFuncContains = false;
				m_sql.Append("%'");
				break;
			}
			finally
			{
				ServerConnection.ParameterizationMode = parameterizationMode;
			}
		case FilterNodeFunction.Type.Like:
			try
			{
				ServerConnection.ParameterizationMode = QueryParameterizationMode.None;
				decode(func.GetParameter(0));
				m_sql.Append(" like N'");
				m_bInFuncLike = true;
				decode(func.GetParameter(1));
				m_bInFuncLike = false;
				m_sql.Append("'");
				break;
			}
			finally
			{
				ServerConnection.ParameterizationMode = parameterizationMode;
			}
		case FilterNodeFunction.Type.In:
			try
			{
				ServerConnection.ParameterizationMode = QueryParameterizationMode.None;
				decode(func.GetParameter(0));
				m_sql.Append(" in (");
				string valueAsString = ((FilterNodeConstant)func.GetParameter(1)).ValueAsString;
				string[] array = valueAsString.Split(',');
				foreach (string text in array)
				{
					int.Parse(text.Trim());
				}
				m_sql.Append(valueAsString);
				m_sql.Append(")");
				break;
			}
			finally
			{
				ServerConnection.ParameterizationMode = parameterizationMode;
			}
		case FilterNodeFunction.Type.Not:
			m_sql.Append("not(");
			decode(func.GetParameter(0));
			m_sql.Append(")");
			break;
		case FilterNodeFunction.Type.Boolean:
			decode(func.GetParameter(0));
			break;
		case FilterNodeFunction.Type.UserDefined:
			switch (func.Name)
			{
			case "BitWiseAnd":
				m_sql.Append("(");
				decode(func.GetParameter(0));
				m_sql.Append(")");
				m_sql.Append(" & ");
				m_sql.Append("(");
				decode(func.GetParameter(1));
				m_sql.Append(")");
				break;
			case "is_null":
				m_sql.Append("(");
				decode(func.GetParameter(0));
				m_sql.Append(")");
				m_sql.Append(" is null");
				break;
			case "datetime":
				m_sql.Append("convert(datetime, ");
				decode(func.GetParameter(0));
				m_sql.Append(", 121)");
				break;
			case "datetime2":
				m_sql.Append("cast(");
				decode(func.GetParameter(0));
				m_sql.Append(" AS datetime2)");
				break;
			case "datetimeoffset":
				m_sql.Append("cast(");
				decode(func.GetParameter(0));
				m_sql.Append(" AS datetimeoffset)");
				break;
			case "timespan":
				m_sql.Append("cast(");
				decode(func.GetParameter(0));
				m_sql.Append(" AS bigint)");
				break;
			case "date":
				m_sql.Append("cast(");
				decode(func.GetParameter(0));
				m_sql.Append(" AS date)");
				break;
			case "time":
				m_sql.Append("cast(");
				decode(func.GetParameter(0));
				m_sql.Append(" AS time)");
				break;
			}
			break;
		}
	}

	private void decode(FilterNodeAttribute ax)
	{
		m_sql.Append(m_isfdc.AddPropertyForFilter(ax.Name));
	}
}
