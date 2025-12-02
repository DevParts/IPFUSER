using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class LimitedBatchBlock : BatchBlock
{
	private const int MaximumObjectsPerBatch = 5000;

	private List<int> ids = new List<int>(5000);

	private string filterConditionText;

	public override string FilterConditionText
	{
		get
		{
			if (filterConditionText == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (ids.Count > 0)
				{
					foreach (int id in ids)
					{
						if (stringBuilder.Length == 0)
						{
							stringBuilder.Append("in(@ID, '");
						}
						else
						{
							stringBuilder.Append(",");
						}
						stringBuilder.Append(id);
					}
					stringBuilder.Append("')");
				}
				filterConditionText = stringBuilder.ToString();
			}
			return filterConditionText;
		}
	}

	internal LimitedBatchBlock(string typeName, PrefetchObjectsFunc prefetchFunc)
		: base(typeName, prefetchFunc)
	{
	}

	public override bool TryAdd(Prefetch prefetch, Urn urn)
	{
		if (ids.Count == 5000)
		{
			return false;
		}
		int item = (int)prefetch.Server.GetSmoObject(urn).Properties["ID"].Value;
		ids.Add(item);
		filterConditionText = null;
		return true;
	}
}
