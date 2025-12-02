using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class SqlObject : SqlObjectBase, ISupportInitDatabaseEngineData
{
	public override void Initialize(object ci, XPathExpressionBlock block)
	{
		base.Initialize(ci, block);
	}

	public void LoadInitData(string file, ServerVersion ver, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		LoadInitDataFromAssembly(ResourceAssembly, file, ver, databaseEngineType, databaseEngineEdition);
	}

	public void LoadInitData(Stream xml, ServerVersion ver, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		LoadInitDataFromAssemblyInternal(ResourceAssembly, null, ver, null, null, store: true, null, databaseEngineType, databaseEngineEdition, xml);
	}

	public void LoadInitDataFromAssembly(Assembly assemblyObject, string file, ServerVersion ver, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		LoadInitDataFromAssemblyInternal(assemblyObject, file, ver, null, null, store: true, null, databaseEngineType, databaseEngineEdition);
	}

	private void LoadInitDataFromAssemblyInternal(Assembly assemblyObject, string file, ServerVersion ver, string alias, StringCollection requestedFields, bool store, StringCollection roAfterCreation, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, Stream configXml = null)
	{
		XmlReadDoc xmlReadDoc = new XmlReadDoc(ver, alias, databaseEngineType, databaseEngineEdition);
		if (configXml == null)
		{
			xmlReadDoc.LoadFile(assemblyObject, file);
		}
		else
		{
			xmlReadDoc.LoadXml(configXml);
		}
		if (store)
		{
			LoadAndStore(xmlReadDoc, assemblyObject, requestedFields, roAfterCreation);
		}
		else
		{
			Load(xmlReadDoc, assemblyObject, requestedFields, roAfterCreation);
		}
		xmlReadDoc.Close();
	}

	protected internal virtual void LoadAndStore(XmlReadDoc xrd, Assembly assemblyObject, StringCollection requestedFields, StringCollection roAfterCreation)
	{
		Load(xrd, assemblyObject, requestedFields, roAfterCreation);
		StoreInitialState();
	}

	internal virtual void Load(XmlReadDoc xrd, Assembly assembly, StringCollection requestedFields, StringCollection roAfterCreation)
	{
		XmlReadSettings settings = xrd.Settings;
		Hashtable hashtable = new Hashtable();
		XmlReadInclude include;
		if (settings != null)
		{
			base.Distinct = settings.Distinct;
			SqlPropertyLink.Add(base.PropertyLinkList, settings);
			XmlReadParentLink parentLink = settings.ParentLink;
			if (parentLink != null)
			{
				base.ParentLink = new ParentLink(parentLink);
				parentLink.Close();
			}
			XmlReadConditionedStatementFailCondition failCondition = settings.FailCondition;
			if (failCondition != null)
			{
				SqlConditionedStatementFailCondition.AddAll(base.ConditionedSqlList, failCondition);
				failCondition.Close();
			}
			XmlRequestParentSelect requestParentSelect = settings.RequestParentSelect;
			if (requestParentSelect != null)
			{
				base.RequestParentSelect = new RequestParentSelect(requestParentSelect);
				requestParentSelect.Close();
			}
			include = settings.Include;
			if (include != null)
			{
				IncludeFile(include, assembly, requestedFields, roAfterCreation);
			}
			XmlReadPropertyLink propertyLink = settings.PropertyLink;
			if (propertyLink != null)
			{
				SqlPropertyLink.AddAll(base.PropertyLinkList, propertyLink);
				propertyLink.Close();
			}
			XmlReadConditionedStatementPrefix prefix = settings.Prefix;
			if (prefix != null)
			{
				SqlConditionedStatementPrefix.AddAll(base.ConditionedSqlList, prefix);
				prefix.Close();
			}
			XmlReadConditionedStatementPostfix postfix = settings.Postfix;
			if (postfix != null)
			{
				SqlConditionedStatementPostfix.AddAll(base.ConditionedSqlList, postfix);
				postfix.Close();
			}
			XmlReadConditionedStatementPostProcess postProcess = settings.PostProcess;
			if (postProcess != null)
			{
				SqlPostProcess.AddAll(base.PostProcessList, postProcess, ResourceAssembly);
				postProcess.Close();
				foreach (ConditionedSql postProcess2 in base.PostProcessList)
				{
					StringEnumerator enumerator2 = postProcess2.Fields.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							string current = enumerator2.Current;
							hashtable.Add(current, current);
						}
					}
					finally
					{
						if (enumerator2 is IDisposable disposable)
						{
							disposable.Dispose();
						}
					}
				}
			}
			XmlReadOrderByRedirect orderByRedirect = settings.OrderByRedirect;
			if (orderByRedirect != null)
			{
				do
				{
					base.OrderByRedirect[orderByRedirect.Field] = orderByRedirect.RedirectFields;
				}
				while (orderByRedirect.Next());
				orderByRedirect.Close();
			}
			XmlReadSpecialQuery specialQuery = settings.SpecialQuery;
			if (specialQuery != null)
			{
				AddSpecialQuery(specialQuery.Database, specialQuery.Query);
				AddQueryHint(specialQuery.Hint);
				specialQuery.Close();
			}
		}
		XmlReadProperties properties = xrd.Properties;
		XmlReadProperty property = properties.Property;
		do
		{
			property = properties.Property;
			include = properties.Include;
			if (property != null)
			{
				if (requestedFields == null || requestedFields.Contains(property.Name) || property.Hidden)
				{
					SqlPropertyLink.Add(base.PropertyLinkList, property);
					SqlObjectProperty sqlObjectProperty = new SqlObjectProperty(property);
					if (roAfterCreation != null && roAfterCreation.Contains(sqlObjectProperty.Name))
					{
						sqlObjectProperty.ReadOnlyAfterCreation = true;
					}
					if (hashtable.ContainsKey(sqlObjectProperty.Name))
					{
						sqlObjectProperty.Usage &= ~ObjectPropertyUsages.Filter;
						sqlObjectProperty.Usage &= ~ObjectPropertyUsages.OrderBy;
					}
					AddProperty(sqlObjectProperty);
					property.Close();
				}
				else
				{
					property.Skip();
				}
			}
			else if (include != null)
			{
				IncludeFile(include, assembly, requestedFields, roAfterCreation);
			}
		}
		while (property != null || include != null);
	}

	private void IncludeFile(XmlReadInclude xri, Assembly assembly, StringCollection requestedFields, StringCollection roAfterCreation)
	{
		StringCollection stringCollection = xri.RequestedFields;
		if (requestedFields != null)
		{
			StringEnumerator enumerator = requestedFields.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					stringCollection.Add(current);
				}
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}
		if (stringCollection.Count == 0)
		{
			stringCollection = null;
		}
		StringCollection stringCollection2 = xri.ROAfterCreation;
		if (roAfterCreation != null)
		{
			StringEnumerator enumerator2 = roAfterCreation.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					string current2 = enumerator2.Current;
					stringCollection2.Add(current2);
				}
			}
			finally
			{
				if (enumerator2 is IDisposable disposable2)
				{
					disposable2.Dispose();
				}
			}
		}
		if (stringCollection2.Count == 0)
		{
			stringCollection2 = null;
		}
		LoadInitDataFromAssemblyInternal(assembly, xri.File, xri.Version, xri.TableAlias, stringCollection, store: false, stringCollection2, xri.DatabaseEngineType, xri.DatabaseEngineEdition);
		xri.Close();
	}
}
