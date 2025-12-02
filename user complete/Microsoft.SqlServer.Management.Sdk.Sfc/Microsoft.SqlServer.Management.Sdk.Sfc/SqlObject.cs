using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public abstract class SqlObject : SqlObjectBase, ISupportInitDatabaseEngineData
{
	public abstract Assembly ResourceAssembly { get; }

	public override void Initialize(object ci, XPathExpressionBlock block)
	{
		base.Initialize(ci, block);
	}

	public void LoadInitData(string file, ServerVersion ver, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		LoadInitDataFromAssembly(ResourceAssembly, file, ver, databaseEngineType, databaseEngineEdition);
	}

	public void LoadInitDataFromAssembly(Assembly assemblyObject, string file, ServerVersion ver, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		LoadInitDataFromAssemblyInternal(assemblyObject, file, ver, null, null, store: true, databaseEngineType, databaseEngineEdition);
	}

	private void LoadInitDataFromAssemblyInternal(Assembly assemblyObject, string file, ServerVersion ver, string alias, StringCollection requestedFields, bool store, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		XmlReadDoc xmlReadDoc = new XmlReadDoc(ver, alias, databaseEngineType, databaseEngineEdition);
		xmlReadDoc.LoadFile(assemblyObject, file);
		if (store)
		{
			LoadAndStore(xmlReadDoc, assemblyObject, requestedFields);
		}
		else
		{
			Load(xmlReadDoc, assemblyObject, requestedFields);
		}
		xmlReadDoc.Close();
	}

	protected internal virtual void LoadAndStore(XmlReadDoc xrd, Assembly assemblyObject, StringCollection requestedFields)
	{
		Load(xrd, assemblyObject, requestedFields);
		StoreInitialState();
	}

	internal virtual void Load(XmlReadDoc xrd, Assembly assembly, StringCollection requestedFields)
	{
		XmlReadSettings settings = xrd.Settings;
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
				IncludeFile(include, assembly, requestedFields);
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
					SqlObjectProperty op = new SqlObjectProperty(property);
					AddProperty(op);
					property.Close();
				}
				else
				{
					property.Skip();
				}
			}
			else if (include != null)
			{
				IncludeFile(include, assembly, requestedFields);
			}
		}
		while (property != null || include != null);
	}

	private void IncludeFile(XmlReadInclude xri, Assembly assembly, StringCollection requestedFields)
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
		LoadInitDataFromAssemblyInternal(assembly, xri.File, xri.Version, xri.TableAlias, stringCollection, store: false, xri.DatabaseEngineType, xri.DatabaseEngineEdition);
		xri.Close();
	}
}
