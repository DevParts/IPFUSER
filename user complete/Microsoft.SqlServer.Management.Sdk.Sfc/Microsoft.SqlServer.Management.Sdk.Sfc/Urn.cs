using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public class Urn
{
	private IUrn impl;

	private int hashCode;

	private static readonly CultureInfo DefaultComparisonCulture = new CultureInfo("");

	private static readonly CompareOptions[] DefaultComparisonOptions;

	public XPathExpression XPathExpression => impl.XPathExpression;

	[XmlAttribute]
	public string Value
	{
		get
		{
			return impl.Value;
		}
		set
		{
			impl.Value = value;
			hashCode = 0;
		}
	}

	public string DomainInstanceName => impl.DomainInstanceName;

	public string Type
	{
		get
		{
			if (XPathExpression.Length > 0)
			{
				return XPathExpression[XPathExpression.Length - 1].Name;
			}
			return string.Empty;
		}
	}

	public Urn Parent
	{
		get
		{
			bool flag = false;
			int num = -1;
			for (int num2 = Value.Length - 1; num2 >= 0; num2--)
			{
				if ('/' == Value[num2] && !flag)
				{
					num = num2;
					break;
				}
				if ('\'' == Value[num2])
				{
					if (num2 > 0 && '\'' == Value[num2 - 1])
					{
						num2--;
					}
					else
					{
						flag = !flag;
					}
				}
			}
			if (-1 == num)
			{
				return null;
			}
			return Value.Substring(0, num);
		}
	}

	public Urn()
	{
		impl = new UrnImpl();
	}

	public Urn(string value)
	{
		impl = new UrnImpl(value);
	}

	internal Urn(IUrn keychain)
	{
		impl = keychain;
	}

	internal IUrn GetIUrn()
	{
		return impl;
	}

	public static bool operator ==(Urn u1, Urn u2)
	{
		return Compare(u1, u2, null, null);
	}

	public static bool operator !=(Urn urn1, Urn urn2)
	{
		return !Compare(urn1, urn2, null, null);
	}

	public override bool Equals(object o)
	{
		if (o == null)
		{
			return false;
		}
		return Compare(this, (Urn)o, null, null);
	}

	public static implicit operator string(Urn urn)
	{
		if (null == urn)
		{
			return null;
		}
		return urn.Value;
	}

	public static implicit operator Urn(string str)
	{
		return new Urn(str);
	}

	public override string ToString()
	{
		return Value;
	}

	public override int GetHashCode()
	{
		if (string.IsNullOrEmpty(Value))
		{
			if (Value == string.Empty)
			{
				return string.Empty.GetHashCode();
			}
			return 0;
		}
		if (hashCode == 0)
		{
			hashCode = XPathExpression.GetHashCode();
		}
		return hashCode;
	}

	public bool Fixed(object ci)
	{
		Enumerator enumerator = new Enumerator();
		_ = XPathExpression;
		for (string text = Value; text != null; text = ((Urn)text).Parent)
		{
			RequestObjectInfo requestObjectInfo = new RequestObjectInfo(text, RequestObjectInfo.Flags.UrnProperties);
			ObjectInfo objectInfo = enumerator.Process(ci, requestObjectInfo);
			if (objectInfo.UrnProperties == null || objectInfo.UrnProperties.Length < 1)
			{
				return false;
			}
			ObjectProperty[] urnProperties = objectInfo.UrnProperties;
			foreach (ObjectProperty objectProperty in urnProperties)
			{
				if (requestObjectInfo.Urn.GetAttribute(objectProperty.Name) == null)
				{
					return false;
				}
			}
		}
		return true;
	}

	public string GetAttribute(string attributeName, string type)
	{
		_ = XPathExpression;
		return XPathExpression.GetAttribute(attributeName, type);
	}

	public string GetAttribute(string attributeName)
	{
		return GetAttribute(attributeName, Type);
	}

	public string GetNameForType(string type)
	{
		return GetAttribute("Name", type);
	}

	[ComVisible(false)]
	public static string EscapeString(string value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in value)
		{
			stringBuilder.Append(c);
			if ('\'' == c)
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	[ComVisible(false)]
	public static string UnEscapeString(string escapedValue)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		foreach (char c in escapedValue)
		{
			if ('\'' == c)
			{
				if (flag)
				{
					flag = false;
					continue;
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
			stringBuilder.Append(c);
		}
		return stringBuilder.ToString();
	}

	public static bool Compare(Urn u1, Urn u2, CompareOptions[] compInfoList, CultureInfo cultureInfo)
	{
		if ((object)u1 == null && (object)u2 == null)
		{
			return true;
		}
		if ((object)u1 == null || (object)u2 == null)
		{
			return false;
		}
		if (cultureInfo == null)
		{
			cultureInfo = DefaultComparisonCulture;
		}
		if (compInfoList == null)
		{
			compInfoList = DefaultComparisonOptions;
		}
		if (compInfoList == DefaultComparisonOptions && u1.GetHashCode() != u2.GetHashCode())
		{
			return false;
		}
		return XPathExpression.Compare(u1.XPathExpression, u2.XPathExpression, compInfoList, cultureInfo);
	}

	public bool IsValidUrn()
	{
		try
		{
			_ = XPathExpression;
			return true;
		}
		catch (XPathException)
		{
			return false;
		}
		catch (InvalidQueryExpressionEnumeratorException)
		{
			return false;
		}
	}

	public bool IsValidUrnSkeleton()
	{
		try
		{
			XPathExpression xPathExpression = XPathExpression;
			for (int i = 0; i < xPathExpression.Length; i++)
			{
				XPathExpressionBlock xPathExpressionBlock = xPathExpression[i];
				if (xPathExpressionBlock.Filter != null)
				{
					return false;
				}
			}
			return true;
		}
		catch (XPathException)
		{
			return false;
		}
		catch (InvalidQueryExpressionEnumeratorException)
		{
			return false;
		}
	}

	static Urn()
	{
		CompareOptions[] defaultComparisonOptions = new CompareOptions[1];
		DefaultComparisonOptions = defaultComparisonOptions;
	}
}
