using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcSqlPathUtilities
{
	public static string ConvertUrnToPath(Urn urn)
	{
		if (urn == null)
		{
			throw new ArgumentNullException("urn");
		}
		if (urn.ToString().Length == 0)
		{
			throw new ArgumentException(SfcStrings.InvalidUrn, "urn");
		}
		string[] array = urn.XPathExpression.ExpressionSkeleton.Split('/');
		StringBuilder stringBuilder = new StringBuilder(255);
		SfcDomainInfo sfcDomainInfo = null;
		foreach (SfcDomainInfo domain in SfcRegistration.Domains)
		{
			if (domain.Name.Equals(array[0], StringComparison.Ordinal))
			{
				sfcDomainInfo = domain;
				break;
			}
		}
		if (sfcDomainInfo == null)
		{
			throw new SfcPathConversionException(SfcStrings.UnknownDomain(array[0]));
		}
		stringBuilder.Append(sfcDomainInfo.PSDriveName);
		stringBuilder.Append("\\");
		string attribute = urn.GetAttribute("Name", array[0]);
		stringBuilder.Append(attribute);
		if (!attribute.Contains("\\"))
		{
			stringBuilder.Append("\\");
			stringBuilder.Append("DEFAULT");
		}
		SfcMetadataDiscovery sfcMetadataDiscovery = new SfcMetadataDiscovery(sfcDomainInfo.RootType);
		List<SfcMetadataRelation> list = sfcMetadataDiscovery.Objects;
		for (int i = 1; i < array.Length; i++)
		{
			SfcMetadataRelation sfcMetadataRelation = null;
			foreach (SfcMetadataRelation item in list)
			{
				if (array[i].Equals(item.ElementTypeName, StringComparison.InvariantCulture))
				{
					sfcMetadataRelation = item;
					break;
				}
			}
			if (sfcMetadataRelation == null)
			{
				throw new SfcPathConversionException(SfcStrings.LevelNotFound(array[i], urn.ToString()));
			}
			if (sfcMetadataRelation.Relationship == SfcRelationship.ChildContainer || sfcMetadataRelation.Relationship == SfcRelationship.ObjectContainer || sfcMetadataRelation.Relationship == SfcRelationship.ChildObject || sfcMetadataRelation.Relationship == SfcRelationship.Object)
			{
				stringBuilder.Append("\\");
				stringBuilder.Append(sfcMetadataRelation.PropertyName);
			}
			if (sfcMetadataRelation.ReadOnlyKeys.Count > 0)
			{
				bool flag = true;
				int num = 0;
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append("\\");
				foreach (SfcMetadataRelation readOnlyKey in sfcMetadataRelation.ReadOnlyKeys)
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						stringBuilder2.Append(".");
					}
					string attribute2 = urn.GetAttribute(readOnlyKey.PropertyName, array[i]);
					if (string.IsNullOrEmpty(attribute2))
					{
						if (i < array.Length - 1)
						{
							throw new SfcPathConversionException(SfcStrings.InvalidKeyValue(readOnlyKey.PropertyName, urn.ToString()));
						}
					}
					else
					{
						stringBuilder2.Append(EncodeSqlName(Urn.UnEscapeString(attribute2)));
						num++;
					}
				}
				if (i == array.Length - 1)
				{
					if (num != 0)
					{
						if (num != sfcMetadataRelation.ReadOnlyKeys.Count)
						{
							throw new SfcPathConversionException(SfcStrings.MissingKeys(urn.ToString(), array[i]));
						}
						stringBuilder.Append(stringBuilder2.ToString());
					}
				}
				else
				{
					stringBuilder.Append(stringBuilder2.ToString());
				}
			}
			list = sfcMetadataRelation.Relations;
		}
		return stringBuilder.ToString();
	}

	public static string EncodeSqlName(string name)
	{
		StringBuilder stringBuilder = new StringBuilder(255);
		for (int i = 0; i < name.Length; i++)
		{
			if (char.IsControl(name[i]) || name[i] == '.' || "\\/:%<>*?[]|".Contains(name[i].ToString()))
			{
				stringBuilder.Append('%');
				stringBuilder.Append($"{Convert.ToByte(name[i]):X2}");
			}
			else
			{
				stringBuilder.Append(name[i]);
			}
		}
		return stringBuilder.ToString();
	}

	public static string DecodeSqlName(string name)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < name.Length; i++)
		{
			if (name[i] == '%' && i + 2 < name.Length)
			{
				string value = name.Substring(i + 1, 2);
				stringBuilder.Append(Convert.ToChar(Convert.ToInt32(value, 16)));
				i += 2;
			}
			else
			{
				stringBuilder.Append(name[i]);
			}
		}
		return stringBuilder.ToString();
	}

	public static string[] DecodeSqlName(string[] names)
	{
		string[] array = new string[names.Length];
		for (int i = 0; i < names.Length; i++)
		{
			array[i] = DecodeSqlName(names[i]);
		}
		return array;
	}
}
