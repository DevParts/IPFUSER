using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class ObjectCache
{
	public const uint MaxUsagePoints = 15u;

	public const uint PingsForAging = 1u;

	private const uint MaxCacheSize = 15u;

	internal const uint SameObjectNumber = 2u;

	private static uint m_CurrentPings = 0u;

	private static SortedList m_cache = new SortedList();

	private static object lock_obj = new object();

	private static Dictionary<string, Assembly> assemblyCache = new Dictionary<string, Assembly>();

	public static CacheElement GetElement(Urn urn, ServerVersion ver, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, object ci)
	{
		ObjectLoadInfo objectLoadInfo = ObjectLoadInfoManager.GetObjectLoadInfo(urn, ci);
		return GetElement(objectLoadInfo, ver, databaseEngineType, databaseEngineEdition);
	}

	public static void PutElement(CacheElement elem)
	{
		SmoManagementUtil.EnterMonitor(lock_obj);
		try
		{
			if ((long)m_cache.Count >= 15L)
			{
				if (++m_CurrentPings > 1)
				{
					TryInsert(elem, bWithAging: true);
					m_CurrentPings = 0u;
				}
				else
				{
					TryInsert(elem, bWithAging: false);
				}
			}
			else
			{
				InsertInCache(elem);
			}
		}
		finally
		{
			SmoManagementUtil.ExitMonitor(lock_obj);
		}
	}

	public static EnumObject LoadFirstElementVersionless(Urn urn, object ci)
	{
		ObjectLoadInfo firstObjectLoadInfo = ObjectLoadInfoManager.GetFirstObjectLoadInfo(urn, ci);
		return LoadElement(firstObjectLoadInfo);
	}

	public static ArrayList GetAllElements(Urn urn, ServerVersion ver, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, object ci)
	{
		ArrayList allObjectsLoadInfo = ObjectLoadInfoManager.GetAllObjectsLoadInfo(urn, ci);
		ArrayList arrayList = new ArrayList();
		foreach (ObjectLoadInfo item in allObjectsLoadInfo)
		{
			arrayList.Add(GetElement(item, ver, databaseEngineType, databaseEngineEdition));
		}
		return arrayList;
	}

	public static void PutAllElements(ArrayList list)
	{
		foreach (CacheElement item in list)
		{
			PutElement(item);
		}
	}

	private static uint GetNumberFromVersion(ServerVersion ver)
	{
		if (ver != null)
		{
			return (uint)(ver.Major * 100 + ver.Minor);
		}
		return 0u;
	}

	private static CacheElement GetElement(ObjectLoadInfo oli, ServerVersion ver, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		SmoManagementUtil.EnterMonitor(lock_obj);
		try
		{
			CacheElement cacheElement = FindInCacheAndRemove(oli, ver, databaseEngineType, databaseEngineEdition);
			if (cacheElement == null)
			{
				cacheElement = LoadElement(oli, ver, databaseEngineType, databaseEngineEdition);
			}
			cacheElement.IncrementUsage();
			return cacheElement;
		}
		finally
		{
			SmoManagementUtil.ExitMonitor(lock_obj);
		}
	}

	private static CacheElement FindInCacheAndRemove(ObjectLoadInfo oli, ServerVersion ver, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		CacheElement cacheElement = null;
		CacheKey cacheKey = new CacheKey(oli.UniqueKey, GetNumberFromVersion(ver), databaseEngineType, databaseEngineEdition);
		for (uint num = 0u; num < 2; num++)
		{
			cacheKey.SameObjKey = num;
			cacheElement = (CacheElement)m_cache[cacheKey];
			if (cacheElement != null)
			{
				m_cache.Remove(cacheKey);
				break;
			}
		}
		return cacheElement;
	}

	private static EnumObject LoadElement(ObjectLoadInfo oli)
	{
		object obj = ((!(oli.AssemblyReference != null)) ? CreateObjectInstance(oli.Assembly, oli.ImplementClass) : CreateObjectInstance(oli.AssemblyReference, oli.ImplementClass));
		if (!(obj is EnumObject result))
		{
			throw new InternalEnumeratorException(SfcStrings.NotDerivedFrom(oli.ImplementClass, "EnumObject"));
		}
		return result;
	}

	private static CacheElement LoadElement(ObjectLoadInfo oli, ServerVersion ver, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		EnumObject enumObject = LoadElement(oli);
		if (oli.InitData != null)
		{
			if (enumObject is ISupportInitDatabaseEngineData supportInitDatabaseEngineData)
			{
				supportInitDatabaseEngineData.LoadInitData(oli.InitData, ver, databaseEngineType, databaseEngineEdition);
			}
			else
			{
				if (!(enumObject is ISupportInitData supportInitData))
				{
					throw new InternalEnumeratorException(SfcStrings.ISupportInitDataNotImplement(oli.InitData));
				}
				supportInitData.LoadInitData(oli.InitData, ver);
			}
		}
		return new CacheElement(oli, enumObject, new CacheKey(oli.UniqueKey, GetNumberFromVersion(ver), databaseEngineType, databaseEngineEdition));
	}

	private static bool InsertInCache(CacheElement elem)
	{
		CacheElement cacheElement = null;
		for (uint num = 0u; num < 2; num++)
		{
			elem.CacheKey.SameObjKey = num;
			cacheElement = (CacheElement)m_cache[elem.CacheKey];
			if (cacheElement == null)
			{
				m_cache[elem.CacheKey] = elem;
				return true;
			}
		}
		cacheElement.IncrementUsage();
		return false;
	}

	private static void TryInsert(CacheElement elem, bool bWithAging)
	{
		uint num = 0u;
		int num2 = -1;
		int index = 0;
		foreach (CacheElement value in m_cache.Values)
		{
			num2++;
			if (bWithAging)
			{
				value.DecrementUsage();
			}
			if (num >= value.Usage || num2 == 0)
			{
				index = num2;
				num = value.Usage;
			}
		}
		if (elem.Usage >= num && InsertInCache(elem))
		{
			m_cache.RemoveAt(index);
		}
	}

	[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
	private static Assembly LoadAssembly(string fullName)
	{
		lock (assemblyCache)
		{
			Assembly value = null;
			if (assemblyCache.TryGetValue(fullName, out value))
			{
				return value;
			}
			AssemblyName assemblyName = new AssemblyName(fullName);
			_ = SmoManagementUtil.GetExecutingAssembly().GetName().Version;
			SmoManagementUtil.GetExecutingAssembly().GetName().GetPublicKey();
			try
			{
				if (assemblyName.Name.StartsWith("Microsoft.SqlServerCe", StringComparison.OrdinalIgnoreCase))
				{
					value = SfcUtility.LoadSqlCeAssembly(assemblyName.Name);
				}
				else
				{
					AssemblyName assemblyName2 = new AssemblyName(SmoManagementUtil.GetExecutingAssembly().FullName);
					assemblyName2.Name = assemblyName.Name;
					try
					{
						value = SmoManagementUtil.LoadAssembly(assemblyName2.FullName);
					}
					catch (FileLoadException)
					{
						assemblyName2.Version = null;
						return Assembly.Load(assemblyName2);
					}
					catch (FileNotFoundException)
					{
						assemblyName2.Version = null;
						return Assembly.Load(assemblyName2);
					}
				}
			}
			catch (FileNotFoundException innerException)
			{
				throw new InternalEnumeratorException(SfcStrings.FailedToLoadAssembly(fullName), innerException);
			}
			catch (BadImageFormatException innerException2)
			{
				throw new InternalEnumeratorException(SfcStrings.FailedToLoadAssembly(fullName), innerException2);
			}
			catch (SecurityException innerException3)
			{
				throw new InternalEnumeratorException(SfcStrings.FailedToLoadAssembly(fullName), innerException3);
			}
			if (null == value)
			{
				throw new InternalEnumeratorException(SfcStrings.FailedToLoadAssembly(fullName));
			}
			assemblyCache.Add(fullName, value);
			return value;
		}
	}

	[ReflectionPermission(SecurityAction.Assert, Unrestricted = true)]
	private static object CreateObjectInstance(Assembly assembly, string objectType)
	{
		object obj = SmoManagementUtil.CreateInstance(assembly, objectType);
		if (obj == null)
		{
			throw new InternalEnumeratorException(SfcStrings.CouldNotInstantiateObj(objectType));
		}
		return obj;
	}

	internal static object CreateObjectInstance(string assemblyName, string objectType)
	{
		return CreateObjectInstance(LoadAssembly(assemblyName), objectType);
	}
}
