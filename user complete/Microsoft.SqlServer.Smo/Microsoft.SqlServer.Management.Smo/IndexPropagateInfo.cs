using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Smo;

internal class IndexPropagateInfo
{
	private IndexCollection Indexes;

	private Index clusteredPrimaryKey;

	private Index nonclusteredPrimaryKey;

	private List<Index> clusteredUniqueKeys;

	private List<Index> nonclusteredUniqueKeys;

	private List<Index> clusteredIndexes;

	private List<Index> nonclusteredIndexes;

	private List<Index> xmlIndexes;

	private List<Index> spatialIndexes;

	public IndexPropagateInfo(IndexCollection indexCollection)
	{
		Indexes = indexCollection;
	}

	private void SetupIndexPropagation()
	{
		ResetIndexPropagation();
		if (Indexes == null)
		{
			return;
		}
		foreach (Index index in Indexes)
		{
			bool? propValueOptional = index.GetPropValueOptional<bool>("IsClustered");
			if (propValueOptional.HasValue && propValueOptional.Value)
			{
				CheckKeyAndAdd(index, ref clusteredPrimaryKey, clusteredUniqueKeys, clusteredIndexes);
			}
			else if (index.HasXmlColumn(throwIfNotSet: true))
			{
				xmlIndexes.Add(index);
			}
			else if (index.HasSpatialColumn(throwIfNotSet: true))
			{
				spatialIndexes.Add(index);
			}
			else
			{
				CheckKeyAndAdd(index, ref nonclusteredPrimaryKey, nonclusteredUniqueKeys, nonclusteredIndexes);
			}
		}
	}

	private void CheckKeyAndAdd(Index index, ref Index primaryKey, List<Index> uniqueKeys, List<Index> indexes)
	{
		IndexKeyType? indexKeyType = index.GetPropValueOptional<IndexKeyType>("IndexKeyType");
		if (!indexKeyType.HasValue)
		{
			indexKeyType = IndexKeyType.None;
		}
		IndexKeyType valueOrDefault = indexKeyType.GetValueOrDefault();
		if (indexKeyType.HasValue)
		{
			switch (valueOrDefault)
			{
			case IndexKeyType.DriPrimaryKey:
				primaryKey = index;
				break;
			case IndexKeyType.DriUniqueKey:
				uniqueKeys.Add(index);
				break;
			case IndexKeyType.None:
				indexes.Add(index);
				break;
			}
		}
	}

	private void ResetIndexPropagation()
	{
		clusteredPrimaryKey = null;
		nonclusteredPrimaryKey = null;
		clusteredUniqueKeys = new List<Index>();
		nonclusteredUniqueKeys = new List<Index>();
		clusteredIndexes = new List<Index>();
		nonclusteredIndexes = new List<Index>();
		xmlIndexes = new List<Index>();
		spatialIndexes = new List<Index>();
	}

	public void PropagateInfo(ArrayList propInfo)
	{
		SetupIndexPropagation();
		propInfo.Add(new SqlSmoObject.PropagateInfo(clusteredPrimaryKey, bWithScript: true, "ClusteredPrimaryKey"));
		propInfo.Add(new SqlSmoObject.PropagateInfo(nonclusteredPrimaryKey, bWithScript: true, "NonclusteredPrimaryKey"));
		propInfo.Add(new SqlSmoObject.PropagateInfo(clusteredUniqueKeys, bWithScript: true, "ClusteredUniqueKey"));
		propInfo.Add(new SqlSmoObject.PropagateInfo(nonclusteredUniqueKeys, bWithScript: true, "NonclusteredUniqueKey"));
		propInfo.Add(new SqlSmoObject.PropagateInfo(clusteredIndexes, bWithScript: true, "ClusteredIndex"));
		propInfo.Add(new SqlSmoObject.PropagateInfo(nonclusteredIndexes, bWithScript: true, "NonclusteredIndex"));
		propInfo.Add(new SqlSmoObject.PropagateInfo(xmlIndexes, bWithScript: true, "XmlIndex"));
		propInfo.Add(new SqlSmoObject.PropagateInfo(spatialIndexes, bWithScript: true, "SpatialIndex"));
	}
}
