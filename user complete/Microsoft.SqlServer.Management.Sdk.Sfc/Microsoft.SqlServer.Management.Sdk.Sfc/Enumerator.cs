using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class Enumerator : MarshalByRefObject
{
	[Conditional("DEBUG")]
	public static void TraceInfo(string trace)
	{
		TraceHelper.Trace("w", 1u, trace);
	}

	[Conditional("DEBUG")]
	public static void TraceInfo(string strFormat, params object[] arg)
	{
		TraceHelper.Trace("w", 1u, strFormat, arg);
	}

	public static EnumResult GetData(object connectionInfo, Request request)
	{
		if (request == null)
		{
			throw new ArgumentNullException("request");
		}
		if (null == request.Urn)
		{
			throw new ArgumentNullException("request.Urn");
		}
		ConnectionHelpers.UpdateConnectionInfoIfContainedAuthentication(ref connectionInfo, request.Urn);
		Request request2 = request.ShallowClone();
		int num = 0;
		request2.Fields = FixPropertyList(connectionInfo, request2.Urn, request2.Fields, request.RequestFieldsTypes);
		request2.RequestFieldsTypes = RequestFieldsTypes.Request;
		num += request2.Fields.Length;
		if (request2.ParentPropertiesRequests != null)
		{
			Urn urn = request2.Urn;
			PropertiesRequest[] parentPropertiesRequests = request2.ParentPropertiesRequests;
			foreach (PropertiesRequest propertiesRequest in parentPropertiesRequests)
			{
				if (null == urn)
				{
					break;
				}
				urn = urn.Parent;
				if (propertiesRequest != null)
				{
					propertiesRequest.Fields = FixPropertyList(connectionInfo, urn, propertiesRequest.Fields, propertiesRequest.RequestFieldsTypes);
					propertiesRequest.RequestFieldsTypes = RequestFieldsTypes.Request;
					num += propertiesRequest.Fields.Length;
				}
			}
		}
		if (num == 0)
		{
			throw new QueryNotSupportedEnumeratorException(SfcStrings.NoPropertiesRequested);
		}
		return new Environment().GetData(request2, connectionInfo);
	}

	public static void RegisterExtension(Urn urn, string name, Assembly assembly, string implementsType)
	{
		ObjectLoadInfoManager.AddExtension(urn, name, assembly, implementsType);
	}

	public static EnumResult GetData(object connectionInfo, Urn urn)
	{
		return new Enumerator().Process(connectionInfo, new Request(urn));
	}

	public static EnumResult GetData(object connectionInfo, Urn urn, string[] requestedFields)
	{
		return new Enumerator().Process(connectionInfo, new Request(urn, requestedFields));
	}

	public static EnumResult GetData(object connectionInfo, Urn urn, string[] requestedFields, OrderBy[] orderBy)
	{
		return new Enumerator().Process(connectionInfo, new Request(urn, requestedFields, orderBy));
	}

	public static EnumResult GetData(object connectionInfo, Urn urn, string[] requestedFields, OrderBy orderBy)
	{
		return new Enumerator().Process(connectionInfo, new Request(urn, requestedFields, new OrderBy[1] { orderBy }));
	}

	public EnumResult Process(object connectionInfo, Request request)
	{
		bool flag = false;
		object connectionInfo2 = connectionInfo;
		try
		{
			flag = ConnectionHelpers.UpdateConnectionInfoIfCloud(ref connectionInfo, request.Urn);
			return GetData(connectionInfo, request);
		}
		catch (Exception ex)
		{
			EnumeratorException.FilterException(ex);
			if (flag)
			{
				return VanillaProcess(connectionInfo2, request);
			}
			throw new EnumeratorException(SfcStrings.FailedRequest, ex);
		}
	}

	private EnumResult VanillaProcess(object connectionInfo, Request request)
	{
		try
		{
			return GetData(connectionInfo, request);
		}
		catch (Exception ex)
		{
			EnumeratorException.FilterException(ex);
			throw new EnumeratorException(SfcStrings.FailedRequest, ex);
		}
	}

	internal static ObjectInfo GetObjectInfo(object connectionInfo, Urn urn)
	{
		return GetObjectInfo(connectionInfo, new RequestObjectInfo(urn, RequestObjectInfo.Flags.All));
	}

	internal static ObjectInfo GetObjectInfo(object connectionInfo, Urn urn, RequestObjectInfo.Flags flags)
	{
		return GetObjectInfo(connectionInfo, new RequestObjectInfo(urn, flags));
	}

	internal static ObjectInfo GetObjectInfo(object connectionInfo, RequestObjectInfo requestObjectInfo)
	{
		if (requestObjectInfo == null)
		{
			throw new ArgumentNullException("requestObjectInfo");
		}
		if (null == requestObjectInfo.Urn)
		{
			throw new ArgumentNullException("requestObjectInfo.Urn");
		}
		return new Environment().GetObjectInfo(connectionInfo, requestObjectInfo);
	}

	public ObjectInfo Process(object connectionInfo, RequestObjectInfo requestObjectInfo)
	{
		ConnectionHelpers.UpdateConnectionInfoIfCloud(ref connectionInfo, requestObjectInfo.Urn);
		return GetObjectInfo(connectionInfo, requestObjectInfo);
	}

	public ObjectInfo Process(ServerVersion version, RequestObjectInfo requestObjectInfo)
	{
		return new Environment().GetObjectInfo(version, requestObjectInfo);
	}

	public DependencyChainCollection EnumDependencies(object connectionInfo, DependencyRequest dependencyRequest)
	{
		if (connectionInfo == null)
		{
			throw new ArgumentNullException("connectionInfo");
		}
		if (dependencyRequest == null)
		{
			throw new ArgumentNullException("dependencyRequest");
		}
		if (dependencyRequest.Urns == null || dependencyRequest.Urns.Length == 0)
		{
			return new DependencyChainCollection();
		}
		IEnumDependencies enumDependencies = ObjectCache.CreateObjectInstance("Microsoft.SqlServer.SqlEnum", "Microsoft.SqlServer.Management.Smo.SqlEnumDependencies") as IEnumDependencies;
		return enumDependencies.EnumDependencies(connectionInfo, dependencyRequest);
	}

	private static string[] FixPropertyList(object connectionInfo, Urn urn, string[] fields, RequestFieldsTypes requestFieldsType)
	{
		if (fields != null && (RequestFieldsTypes.Request & requestFieldsType) != RequestFieldsTypes.Reject)
		{
			return fields;
		}
		RequestObjectInfo requestObjectInfo = new RequestObjectInfo();
		requestObjectInfo.Urn = urn;
		requestObjectInfo.InfoType = RequestObjectInfo.Flags.Properties;
		ObjectInfo objectInfo = GetObjectInfo(connectionInfo, requestObjectInfo);
		ArrayList arrayList = new ArrayList();
		bool flag = 0 != (RequestFieldsTypes.IncludeExpensiveInResult & requestFieldsType);
		if (fields == null)
		{
			ObjectProperty[] properties = objectInfo.Properties;
			foreach (ObjectProperty objectProperty in properties)
			{
				if ((!objectProperty.Expensive || flag) && ObjectPropertyUsages.Request == (objectProperty.Usage & ObjectPropertyUsages.Request))
				{
					arrayList.Add(objectProperty.Name);
				}
			}
		}
		else
		{
			Hashtable hashtable = new Hashtable();
			foreach (string key in fields)
			{
				hashtable[key] = null;
			}
			ObjectProperty[] properties2 = objectInfo.Properties;
			foreach (ObjectProperty objectProperty2 in properties2)
			{
				if (!hashtable.ContainsKey(objectProperty2.Name) && (!objectProperty2.Expensive || flag))
				{
					arrayList.Add(objectProperty2.Name);
				}
			}
		}
		string[] array = new string[arrayList.Count];
		arrayList.CopyTo(array, 0);
		return array;
	}
}
