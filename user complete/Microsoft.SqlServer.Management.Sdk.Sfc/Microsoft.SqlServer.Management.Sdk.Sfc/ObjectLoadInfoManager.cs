using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class ObjectLoadInfoManager
{
	private static SortedList m_Hierarchy;

	private static uint m_UniqueKey = 0u;

	private static object lock_obj = new object();

	public static ObjectLoadInfo GetObjectLoadInfo(Urn urn, object ci)
	{
		LoadHierarchy();
		StringCollection canonicUrn = GetCanonicUrn(urn, ci);
		return GetObjectLoadInfo(canonicUrn);
	}

	public static ObjectLoadInfo GetFirstObjectLoadInfo(Urn urn, object ci)
	{
		LoadHierarchy();
		StringCollection canonicUrn = GetCanonicUrn(urn, ci);
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(canonicUrn[0]);
		return GetObjectLoadInfo(stringCollection);
	}

	public static ArrayList GetAllObjectsLoadInfo(Urn urn, object ci)
	{
		LoadHierarchy();
		StringCollection canonicUrn = GetCanonicUrn(urn, ci);
		return GetAllObjectsLoadInfo(canonicUrn);
	}

	private static StringCollection GetCanonicUrn(Urn urn, object ci)
	{
		StringCollection stringCollection = new StringCollection();
		Urn urn2 = urn;
		while (null != urn2)
		{
			stringCollection.Insert(0, urn2.Type);
			urn2 = urn2.Parent;
		}
		return stringCollection;
	}

	private static ObjectLoadInfo GetHierarchyRoot(string s)
	{
		ObjectLoadInfo objectLoadInfo = (ObjectLoadInfo)m_Hierarchy[s];
		if (objectLoadInfo == null)
		{
			throw new InvalidQueryExpressionEnumeratorException(SfcStrings.UrnCouldNotBeResolvedAtLevel(s));
		}
		return objectLoadInfo;
	}

	private static ObjectLoadInfo GetNextLevel(ObjectLoadInfo curent_oli, string s)
	{
		ObjectLoadInfo objectLoadInfo = (ObjectLoadInfo)curent_oli.Children[s];
		if (objectLoadInfo == null)
		{
			if (curent_oli.typeAllowsRecursion && s == curent_oli.Name)
			{
				return curent_oli;
			}
			throw new InvalidQueryExpressionEnumeratorException(SfcStrings.UrnCouldNotBeResolvedAtLevel(s));
		}
		return objectLoadInfo;
	}

	private static ObjectLoadInfo GetObjectLoadInfo(StringCollection types)
	{
		ObjectLoadInfo objectLoadInfo = null;
		StringEnumerator enumerator = types.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				objectLoadInfo = ((objectLoadInfo != null) ? GetNextLevel(objectLoadInfo, current) : GetHierarchyRoot(current));
			}
			return objectLoadInfo;
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	private static ArrayList GetAllObjectsLoadInfo(StringCollection types)
	{
		ArrayList arrayList = new ArrayList();
		ObjectLoadInfo objectLoadInfo = null;
		StringEnumerator enumerator = types.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				objectLoadInfo = ((objectLoadInfo != null) ? GetNextLevel(objectLoadInfo, current) : GetHierarchyRoot(current));
				arrayList.Add(objectLoadInfo);
			}
			return arrayList;
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	private static void LoadHierarchy()
	{
		SmoManagementUtil.EnterMonitor(lock_obj);
		Stream stream = null;
		try
		{
			if (m_Hierarchy != null)
			{
				return;
			}
			m_Hierarchy = new SortedList(StringComparer.Ordinal);
			using (stream = SmoManagementUtil.LoadResourceFromAssembly(SmoManagementUtil.GetExecutingAssembly(), "Config.xml"))
			{
				if (stream == null)
				{
					Console.WriteLine("fs is null!");
					Console.ReadLine();
				}
				XmlTextReader xmlTextReader = new XmlTextReader(stream);
				xmlTextReader.DtdProcessing = DtdProcessing.Prohibit;
				XmlTextReader xmlTextReader2 = xmlTextReader;
				xmlTextReader2.MoveToContent();
				XmlUtility.SelectNextElement(xmlTextReader2);
				LoadChildren(xmlTextReader2, xmlTextReader2.Depth, m_Hierarchy);
			}
		}
		finally
		{
			SmoManagementUtil.ExitMonitor(lock_obj);
		}
	}

	private static bool LoadChildren(XmlTextReader reader, int nLevelDepth, SortedList list)
	{
		int num = nLevelDepth;
		ObjectLoadInfo objectLoadInfo = null;
		while (true)
		{
			num = reader.Depth;
			if (num < nLevelDepth)
			{
				return true;
			}
			if (num > nLevelDepth)
			{
				if (!LoadChildren(reader, num, objectLoadInfo.Children))
				{
					return false;
				}
				continue;
			}
			objectLoadInfo = Add(reader, list);
			if (!XmlUtility.SelectNextElement(reader))
			{
				break;
			}
		}
		return false;
	}

	private static ObjectLoadInfo Add(XmlTextReader reader, SortedList list)
	{
		ObjectLoadInfo objectLoadInfo = new ObjectLoadInfo();
		objectLoadInfo.Name = reader["type"];
		objectLoadInfo.Assembly = reader["assembly"];
		objectLoadInfo.InitData = reader["cfg"];
		objectLoadInfo.ImplementClass = reader["implement"];
		objectLoadInfo.UniqueKey = m_UniqueKey;
		m_UniqueKey += 2u;
		string text = reader["allow_recursion"];
		if (text != null)
		{
			objectLoadInfo.typeAllowsRecursion = Convert.ToBoolean(text);
		}
		list.Add(reader["type"], objectLoadInfo);
		return objectLoadInfo;
	}

	public static void AddExtension(Urn urn, string name, Assembly assembly, string implementsType)
	{
		ObjectLoadInfo objectLoadInfo = new ObjectLoadInfo();
		objectLoadInfo.Name = name;
		objectLoadInfo.Assembly = null;
		objectLoadInfo.InitData = null;
		objectLoadInfo.ImplementClass = implementsType;
		objectLoadInfo.UniqueKey = m_UniqueKey;
		m_UniqueKey += 2u;
		objectLoadInfo.AssemblyReference = assembly;
		if (urn == null)
		{
			LoadHierarchy();
			m_Hierarchy.Add(name, objectLoadInfo);
			return;
		}
		ArrayList allObjectsLoadInfo = GetAllObjectsLoadInfo(urn, null);
		if (allObjectsLoadInfo.Count > 0)
		{
			((ObjectLoadInfo)allObjectsLoadInfo[allObjectsLoadInfo.Count - 1]).Children.Add(name, objectLoadInfo);
		}
	}
}
