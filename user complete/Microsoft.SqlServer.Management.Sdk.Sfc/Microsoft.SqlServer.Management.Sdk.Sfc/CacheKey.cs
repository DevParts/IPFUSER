using System;
using System.Globalization;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class CacheKey : IComparable
{
	private readonly uint base_key;

	private readonly uint version;

	private readonly DatabaseEngineType databaseEngineType;

	private readonly DatabaseEngineEdition databaseEngineEdition;

	private uint same_obj_key;

	public uint SameObjKey
	{
		get
		{
			return same_obj_key;
		}
		set
		{
			same_obj_key = value;
		}
	}

	public CacheKey(uint base_key, uint version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		this.base_key = base_key;
		this.version = version;
		same_obj_key = 0u;
		this.databaseEngineType = databaseEngineType;
		this.databaseEngineEdition = databaseEngineEdition;
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[2] { base_key, version });
	}

	public int CompareTo(object o)
	{
		CacheKey cacheKey = (CacheKey)o;
		if (base_key < cacheKey.base_key)
		{
			return -1;
		}
		if (base_key > cacheKey.base_key)
		{
			return 1;
		}
		if (version < cacheKey.version)
		{
			return -1;
		}
		if (version > cacheKey.version)
		{
			return 1;
		}
		if (same_obj_key < cacheKey.same_obj_key)
		{
			return -1;
		}
		if (same_obj_key > cacheKey.same_obj_key)
		{
			return 1;
		}
		if (databaseEngineType < cacheKey.databaseEngineType)
		{
			return -1;
		}
		if (databaseEngineType > cacheKey.databaseEngineType)
		{
			return 1;
		}
		if (databaseEngineEdition < cacheKey.databaseEngineEdition)
		{
			return -1;
		}
		if (databaseEngineEdition > cacheKey.databaseEngineEdition)
		{
			return 1;
		}
		return 0;
	}
}
