using System;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Smo;

internal class DynamicPropertyMetadataProvider : PropertyMetadataProvider
{
	private SortedList<string, StaticMetadata> m_listData = new SortedList<string, StaticMetadata>(System.StringComparer.Ordinal);

	public override int Count => m_listData.Count;

	public override int PropertyNameToIDLookup(string propertyName)
	{
		return m_listData.IndexOfKey(propertyName);
	}

	public override StaticMetadata GetStaticMetadata(int id)
	{
		return m_listData.Values[id];
	}

	public void AddMetadata(string name, bool readOnly, Type type)
	{
		m_listData.Add(name, new StaticMetadata(name, expensive: false, readOnly, type));
	}
}
