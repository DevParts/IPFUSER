using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XPathExpressionBlock
{
	private string m_name;

	private FilterNode m_filter;

	private KeyValuePair<string, object>[] m_fixedProperties;

	private static Dictionary<string, string> CachedNames = new Dictionary<string, string>(StringComparer.Ordinal);

	public string Name
	{
		get
		{
			return m_name;
		}
		set
		{
			m_name = value;
		}
	}

	public FilterNode Filter
	{
		get
		{
			return m_filter;
		}
		set
		{
			m_fixedProperties = null;
			m_filter = value;
		}
	}

	private KeyValuePair<string, object>[] FixedPropertiesInternal
	{
		get
		{
			if (m_fixedProperties == null)
			{
				ComputeFixedProperties(Filter);
				if (m_fixedProperties == null)
				{
					m_fixedProperties = new KeyValuePair<string, object>[0];
				}
			}
			return m_fixedProperties;
		}
	}

	public SortedList FixedProperties
	{
		get
		{
			SortedList sortedList = new SortedList(StringComparer.Ordinal, FixedPropertiesInternal.Length);
			KeyValuePair<string, object>[] fixedPropertiesInternal = FixedPropertiesInternal;
			for (int i = 0; i < fixedPropertiesInternal.Length; i++)
			{
				KeyValuePair<string, object> keyValuePair = fixedPropertiesInternal[i];
				sortedList.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return sortedList;
		}
	}

	public XPathExpressionBlock()
	{
	}

	public XPathExpressionBlock(string name, FilterNode filter)
	{
		if (!CachedNames.TryGetValue(name, out m_name))
		{
			CachedNames.Add(name, name);
			m_name = name;
		}
		Filter = filter;
	}

	public XPathExpressionBlock Copy()
	{
		XPathExpressionBlock xPathExpressionBlock = new XPathExpressionBlock();
		xPathExpressionBlock.m_name = m_name;
		xPathExpressionBlock.m_filter = m_filter;
		xPathExpressionBlock.m_fixedProperties = m_fixedProperties;
		return xPathExpressionBlock;
	}

	public override string ToString()
	{
		string text = Name;
		if (Filter != null)
		{
			text += $"[{Filter}]";
		}
		return text;
	}

	public override int GetHashCode()
	{
		int num = m_name.GetHashCode();
		if (m_filter != null)
		{
			num ^= m_filter.GetHashCode();
		}
		return num;
	}

	private int IndexOfFixedProperty(string name)
	{
		for (int i = 0; i < FixedPropertiesInternal.Length; i++)
		{
			if (string.Compare(FixedPropertiesInternal[i].Key, name, StringComparison.Ordinal) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	private void AddFixedProperty(string name, object fnc)
	{
		KeyValuePair<string, object> keyValuePair = new KeyValuePair<string, object>(name, fnc);
		if (m_fixedProperties == null || m_fixedProperties.Length == 0)
		{
			m_fixedProperties = new KeyValuePair<string, object>[1] { keyValuePair };
		}
		else if (IndexOfFixedProperty(name) == -1)
		{
			int num = m_fixedProperties.Length;
			KeyValuePair<string, object>[] fixedProperties = m_fixedProperties;
			m_fixedProperties = new KeyValuePair<string, object>[num + 1];
			Array.Copy(fixedProperties, m_fixedProperties, num);
			m_fixedProperties[num] = keyValuePair;
		}
	}

	private void AddFixedProperty(FilterNodeOperator fno)
	{
		object obj = null;
		FilterNode.Type type = FilterNode.Type.Operator;
		FilterNodeAttribute filterNodeAttribute;
		if (fno.Left.NodeType == FilterNode.Type.Attribute)
		{
			filterNodeAttribute = (FilterNodeAttribute)fno.Left;
			obj = fno.Right;
			type = fno.Right.NodeType;
		}
		else
		{
			filterNodeAttribute = (FilterNodeAttribute)fno.Right;
			obj = fno.Left;
			type = fno.Left.NodeType;
		}
		if (FilterNode.Type.Constant == type)
		{
			AddFixedProperty(filterNodeAttribute.Name, obj);
		}
		else if (FilterNode.Type.Function == type)
		{
			FilterNodeFunction filterNodeFunction = (FilterNodeFunction)obj;
			if (filterNodeFunction.FunctionType == FilterNodeFunction.Type.True || filterNodeFunction.FunctionType == FilterNodeFunction.Type.False)
			{
				AddFixedProperty(filterNodeAttribute.Name, obj);
			}
		}
	}

	private bool ComputeFixedProperties(FilterNode node)
	{
		if (node == null)
		{
			return true;
		}
		if (FilterNode.Type.Operator == node.NodeType)
		{
			FilterNodeOperator filterNodeOperator = (FilterNodeOperator)node;
			if (FilterNodeOperator.Type.EQ == filterNodeOperator.OpType)
			{
				AddFixedProperty(filterNodeOperator);
			}
			else
			{
				if (FilterNodeOperator.Type.And != filterNodeOperator.OpType)
				{
					return false;
				}
				if (!ComputeFixedProperties(filterNodeOperator.Left))
				{
					return false;
				}
				if (!ComputeFixedProperties(filterNodeOperator.Right))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public static string GetUniqueAttribute(FilterNode filter)
	{
		if (filter == null)
		{
			return null;
		}
		FilterNodeOperator filterNodeOperator = (FilterNodeOperator)filter;
		if (FilterNodeOperator.Type.EQ == filterNodeOperator.OpType)
		{
			if (filterNodeOperator.Left.NodeType == FilterNode.Type.Attribute && FilterNode.Type.Constant == filterNodeOperator.Right.NodeType)
			{
				return ((FilterNodeConstant)filterNodeOperator.Right).Value.ToString();
			}
			if (filterNodeOperator.Right.NodeType == FilterNode.Type.Attribute && FilterNode.Type.Constant == filterNodeOperator.Left.NodeType)
			{
				return ((FilterNodeConstant)filterNodeOperator.Left).Value.ToString();
			}
		}
		return null;
	}

	public string GetAttributeFromFilter(string attributeName)
	{
		string result = null;
		int num = IndexOfFixedProperty(attributeName);
		if (num >= 0)
		{
			result = ((FilterNodeConstant)FixedPropertiesInternal[num].Value).Value.ToString();
		}
		return result;
	}
}
