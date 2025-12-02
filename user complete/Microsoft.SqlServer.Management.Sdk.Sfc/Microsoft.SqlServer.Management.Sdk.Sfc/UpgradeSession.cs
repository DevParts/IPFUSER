using System;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class UpgradeSession
{
	public virtual List<KeyValuePair<string, object>> UpgradeInstance(List<SfcInstanceSerializedData> sfcInstanceData, int fileVersion, string smlUri, Dictionary<string, object> sfcCache)
	{
		return null;
	}

	public object UpgradeInstance(Type newInstanceType, List<SfcInstanceSerializedData> sfcInstanceData)
	{
		SfcSerializer sfcSerializer = new SfcSerializer();
		return sfcSerializer.CreateInstanceFromSerializedData(newInstanceType, string.Empty, sfcInstanceData);
	}

	public virtual void PostProcessUpgrade(Dictionary<string, object> sfcCache, int fileVersion)
	{
	}

	public virtual bool IsUpgradeRequiredOnType(string instanceType, int fileVersion)
	{
		return false;
	}
}
