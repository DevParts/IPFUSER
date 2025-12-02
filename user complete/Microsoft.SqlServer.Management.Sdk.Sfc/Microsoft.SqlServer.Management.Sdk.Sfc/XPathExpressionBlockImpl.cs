using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class XPathExpressionBlockImpl : IUrnFragment
{
	private XPathExpressionBlock xpBlock;

	private Dictionary<string, object> fieldDict;

	string IUrnFragment.Name => xpBlock.Name;

	Dictionary<string, object> IUrnFragment.FieldDictionary
	{
		get
		{
			if (fieldDict == null)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				foreach (DictionaryEntry fixedProperty in xpBlock.FixedProperties)
				{
					FilterNodeConstant filterNodeConstant = (FilterNodeConstant)fixedProperty.Value;
					dictionary.Add(fixedProperty.Key.ToString(), Urn.UnEscapeString((string)filterNodeConstant.Value));
				}
				fieldDict = dictionary;
			}
			return fieldDict;
		}
	}

	public XPathExpressionBlockImpl(XPathExpressionBlock xpBlock)
	{
		this.xpBlock = xpBlock;
	}
}
