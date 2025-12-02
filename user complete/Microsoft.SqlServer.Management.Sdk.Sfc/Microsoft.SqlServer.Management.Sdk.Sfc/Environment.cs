using System.Collections;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class Environment
{
	private XPathExpression m_xpath;

	private ArrayList m_listEnumObject;

	public int LastPos => m_listEnumObject.Count - 1;

	private void GetObjectsFromCache(Urn urn, object ci)
	{
		m_listEnumObject = ObjectCache.GetAllElements(urn, GetServerVersion(urn, ci), GetDatabaseEngineType(urn, ci), GetDatabaseEngineEdition(urn, ci), ci);
		int num = 0;
		foreach (CacheElement item in m_listEnumObject)
		{
			item.EnumObject.Initialize(ci, m_xpath[num++]);
		}
		for (num = m_listEnumObject.Count - 1; num >= 0; num--)
		{
			((CacheElement)m_listEnumObject[num]).EnumObject.Urn = urn;
			urn = urn.Parent;
		}
	}

	private void PostProcess(EnumResult er)
	{
		for (int num = m_listEnumObject.Count - 1; num >= 0; num--)
		{
			((CacheElement)m_listEnumObject[num]).EnumObject.PostProcess(er);
		}
	}

	private void PutObjectsInCache()
	{
		ObjectCache.PutAllElements(m_listEnumObject);
	}

	public void InitObjects(Request req)
	{
		Request request = req;
		for (int num = m_listEnumObject.Count - 1; num >= 0; num--)
		{
			EnumObject enumObject = ((CacheElement)m_listEnumObject[num]).EnumObject;
			enumObject.Request = request;
			request = enumObject.RetrieveParentRequest();
		}
	}

	private EnumResult GetData()
	{
		EnumResult enumResult = null;
		foreach (CacheElement item in m_listEnumObject)
		{
			enumResult = item.EnumObject.GetData(enumResult);
		}
		return enumResult;
	}

	public EnumResult GetData(Request req, object ci)
	{
		m_xpath = req.Urn.XPathExpression;
		GetObjectsFromCache(req.Urn, ci);
		InitObjects(req);
		EnumResult data = GetData();
		PostProcess(data);
		PutObjectsInCache();
		return data;
	}

	public ObjectInfo GetObjectInfo(object ci, RequestObjectInfo req)
	{
		m_xpath = req.Urn.XPathExpression;
		CacheElement element = ObjectCache.GetElement(req.Urn, GetServerVersion(req.Urn, ci), GetDatabaseEngineType(req.Urn, ci), GetDatabaseEngineEdition(req.Urn, ci), ci);
		EnumObject enumObject = element.EnumObject;
		ObjectInfo objectInfo = new ObjectInfo();
		if ((RequestObjectInfo.Flags.Children & req.InfoType) != RequestObjectInfo.Flags.None)
		{
			objectInfo.Children = element.GetChildren();
		}
		if ((RequestObjectInfo.Flags.Properties & req.InfoType) != RequestObjectInfo.Flags.None)
		{
			objectInfo.Properties = enumObject.GetProperties(ObjectPropertyUsages.All);
		}
		if ((RequestObjectInfo.Flags.UrnProperties & req.InfoType) != RequestObjectInfo.Flags.None)
		{
			objectInfo.UrnProperties = enumObject.GetUrnProperties();
		}
		if ((RequestObjectInfo.Flags.ResultTypes & req.InfoType) != RequestObjectInfo.Flags.None)
		{
			objectInfo.ResultTypes = enumObject.ResultTypes;
		}
		ObjectCache.PutElement(element);
		return objectInfo;
	}

	internal static ServerVersion GetServerVersion(Urn urn, object ci)
	{
		if (ci is ServerVersion)
		{
			return (ServerVersion)ci;
		}
		if (ci is ServerInformation)
		{
			return ((ServerInformation)ci).ServerVersion;
		}
		EnumObject enumObject = ObjectCache.LoadFirstElementVersionless(urn, ci);
		if (enumObject is ISupportVersions supportVersions)
		{
			return supportVersions.GetServerVersion(ci);
		}
		return null;
	}

	public ObjectInfo GetObjectInfo(ServerVersion version, RequestObjectInfo req)
	{
		m_xpath = req.Urn.XPathExpression;
		CacheElement element = ObjectCache.GetElement(req.Urn, version, DatabaseEngineType.Standalone, DatabaseEngineEdition.Unknown, version);
		EnumObject enumObject = element.EnumObject;
		ObjectInfo objectInfo = new ObjectInfo();
		if ((RequestObjectInfo.Flags.Children & req.InfoType) != RequestObjectInfo.Flags.None)
		{
			objectInfo.Children = element.GetChildren();
		}
		if ((RequestObjectInfo.Flags.Properties & req.InfoType) != RequestObjectInfo.Flags.None)
		{
			objectInfo.Properties = enumObject.GetProperties(ObjectPropertyUsages.All);
		}
		if ((RequestObjectInfo.Flags.UrnProperties & req.InfoType) != RequestObjectInfo.Flags.None)
		{
			objectInfo.UrnProperties = enumObject.GetUrnProperties();
		}
		if ((RequestObjectInfo.Flags.ResultTypes & req.InfoType) != RequestObjectInfo.Flags.None)
		{
			objectInfo.ResultTypes = enumObject.ResultTypes;
		}
		ObjectCache.PutElement(element);
		return objectInfo;
	}

	internal static DatabaseEngineType GetDatabaseEngineType(Urn urn, object ci)
	{
		if (ci is ServerInformation)
		{
			return ((ServerInformation)ci).DatabaseEngineType;
		}
		EnumObject enumObject = ObjectCache.LoadFirstElementVersionless(urn, ci);
		if (enumObject is ISupportDatabaseEngineTypes supportDatabaseEngineTypes)
		{
			return supportDatabaseEngineTypes.GetDatabaseEngineType(ci);
		}
		return DatabaseEngineType.Standalone;
	}

	internal static DatabaseEngineEdition GetDatabaseEngineEdition(Urn urn, object ci)
	{
		if (ci is ServerInformation)
		{
			return ((ServerInformation)ci).DatabaseEngineEdition;
		}
		EnumObject enumObject = ObjectCache.LoadFirstElementVersionless(urn, ci);
		if (enumObject is ISupportDatabaseEngineEditions supportDatabaseEngineEditions)
		{
			return supportDatabaseEngineEditions.GetDatabaseEngineEdition(ci);
		}
		return DatabaseEngineEdition.Unknown;
	}
}
