using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
[ComVisible(false)]
public class XPathExpression
{
	private XPathExpressionBlock[] xPathBlocks;

	public virtual XPathExpressionBlock this[int index] => xPathBlocks[index];

	public int Length => xPathBlocks.Length;

	public string ExpressionSkeleton => BlockExpressionSkeleton(Length - 1);

	internal XPathExpression()
	{
		xPathBlocks = new XPathExpressionBlock[0];
	}

	public XPathExpression(string strXPathExpression)
	{
		Compile(strXPathExpression);
	}

	public XPathExpression(IList<XPathExpressionBlock> blocks)
	{
		xPathBlocks = new XPathExpressionBlock[blocks.Count];
		blocks.CopyTo(xPathBlocks, 0);
	}

	internal void Compile(string strXPathExpression)
	{
		XPathHandler xPathHandler = new XPathHandler();
		XPathScanner xPathScanner = new XPathScanner();
		xPathScanner.SetParseString(strXPathExpression);
		AstNode ast = xPathHandler.Run(xPathScanner);
		Load(ast);
	}

	private void Load(AstNode ast)
	{
		List<XPathExpressionBlock> list = new List<XPathExpressionBlock>();
		while (ast != null)
		{
			XPathExpressionBlock xPathExpressionBlock = new XPathExpressionBlock();
			while (ast.TypeOfAst == AstNode.QueryType.Filter)
			{
				FilterNode filterNode = FilterTranslate.decode(ast);
				if (xPathExpressionBlock.Filter == null)
				{
					xPathExpressionBlock.Filter = filterNode;
				}
				else
				{
					xPathExpressionBlock.Filter = new FilterNodeOperator(FilterNodeOperator.Type.And, xPathExpressionBlock.Filter, filterNode);
				}
				ast = ((Filter)ast).Input;
			}
			if (ast.TypeOfAst != AstNode.QueryType.Axis)
			{
				throw new InvalidQueryExpressionEnumeratorException(SfcStrings.InvalidNode);
			}
			xPathExpressionBlock.Name = ((Axis)ast).Name;
			ast = ((Axis)ast).Input;
			list.Add(xPathExpressionBlock);
		}
		list.Reverse();
		xPathBlocks = list.ToArray();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < Length; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append("/");
			}
			stringBuilder.Append(this[i]);
		}
		return stringBuilder.ToString();
	}

	public override int GetHashCode()
	{
		int num = -1;
		for (int i = 0; i < Length; i++)
		{
			num ^= this[i].GetHashCode();
		}
		return num;
	}

	public string BlockExpressionSkeleton(int blockIndex)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < Length && i <= blockIndex; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append("/");
			}
			stringBuilder.Append(this[i].Name);
		}
		return stringBuilder.ToString();
	}

	internal static bool Compare(XPathExpression x1, XPathExpression x2, CompareOptions[] compInfoList, CultureInfo cultureInfo)
	{
		if (x1 == null && x2 == null)
		{
			return true;
		}
		if (x1 == null || x2 == null)
		{
			return false;
		}
		if (x1.Length != x2.Length)
		{
			return false;
		}
		for (int num = x1.Length - 1; num >= 0; num--)
		{
			if (string.Compare(x1[num].Name, x2[num].Name, StringComparison.Ordinal) != 0)
			{
				return false;
			}
			CompareOptions compInfo = ((num < compInfoList.Length) ? compInfoList[num] : CompareOptions.None);
			if (!FilterNode.Compare(x1[num].Filter, x2[num].Filter, compInfo, cultureInfo))
			{
				return false;
			}
		}
		return true;
	}

	public string GetAttribute(string attributeName, string type)
	{
		for (int i = 0; i < Length; i++)
		{
			if (this[i].Name == type)
			{
				return this[i].GetAttributeFromFilter(attributeName);
			}
		}
		return null;
	}
}
