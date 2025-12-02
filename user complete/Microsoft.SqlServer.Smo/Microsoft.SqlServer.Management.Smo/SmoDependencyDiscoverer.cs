using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class SmoDependencyDiscoverer : ISmoDependencyDiscoverer
{
	internal HashSet<UrnTypeKey> filteredUrnTypes;

	internal CreatingObjectDictionary creatingDictionary;

	private ChildrenDiscoveryEventHandler childrenDiscovery;

	internal IDatabasePrefetch DatabasePrefetch { get; set; }

	public ScriptingPreferences Preferences { get; set; }

	public Server Server { get; set; }

	internal event ChildrenDiscoveryEventHandler ChildrenDiscovery
	{
		add
		{
			childrenDiscovery = (ChildrenDiscoveryEventHandler)Delegate.Combine(childrenDiscovery, value);
		}
		remove
		{
			childrenDiscovery = (ChildrenDiscoveryEventHandler)Delegate.Remove(childrenDiscovery, value);
		}
	}

	public SmoDependencyDiscoverer(Server server)
	{
		Server = server;
		filteredUrnTypes = new HashSet<UrnTypeKey>();
		Preferences = new ScriptingPreferences();
		Preferences.DependentObjects = true;
	}

	public SmoDependencyDiscoverer(Server server, ScriptingOptions so)
		: this(server)
	{
		Preferences = so.GetScriptingPreferences();
		filteredUrnTypes = so.GetSmoUrnFilterForDiscovery(server).filteredTypes;
	}

	public IEnumerable<Urn> Discover(IEnumerable<Urn> urns)
	{
		HashSet<Urn> hashSet = new HashSet<Urn>(urns);
		if (Preferences.DependentObjects)
		{
			hashSet = ReferenceDiscovery(hashSet);
		}
		if (Preferences.SfcChildren)
		{
			hashSet = SfcChildrenDiscovery(hashSet);
		}
		else if (DatabasePrefetch != null)
		{
			return new HashSet<Urn>(DatabasePrefetch.PrefetchObjects(hashSet));
		}
		return hashSet;
	}

	private HashSet<Urn> SfcChildrenDiscovery(HashSet<Urn> discoveredUrns)
	{
		SqlSmoObject.PropagateAction propagateAction = GetPropagateAction();
		if (propagateAction == SqlSmoObject.PropagateAction.Drop || propagateAction == SqlSmoObject.PropagateAction.Alter)
		{
			return discoveredUrns;
		}
		IEnumerable<Urn> enumerable = ((DatabasePrefetch != null) ? DatabasePrefetch.PrefetchObjects(discoveredUrns) : discoveredUrns);
		HashSet<Urn> hashSet = new HashSet<Urn>();
		foreach (Urn item in enumerable)
		{
			if (!item.Type.Equals("UnresolvedEntity"))
			{
				List<SqlSmoObject.PropagateInfo> list = new List<SqlSmoObject.PropagateInfo>();
				SqlSmoObject sqlSmoObject = creatingDictionary.SmoObjectFromUrn(item);
				SqlSmoObject.PropagateInfo[] propagateInfoForDiscovery = sqlSmoObject.GetPropagateInfoForDiscovery(propagateAction);
				if (propagateInfoForDiscovery != null)
				{
					list.AddRange(propagateInfoForDiscovery);
				}
				List<Urn> scriptableChildren = GetScriptableChildren(list, propagateAction);
				hashSet.UnionWith(scriptableChildren);
				if (childrenDiscovery != null)
				{
					childrenDiscovery(this, new ChildrenDiscoveryEventArgs(item, scriptableChildren));
				}
			}
		}
		discoveredUrns.UnionWith(hashSet);
		return discoveredUrns;
	}

	private List<Urn> GetScriptableChildren(List<SqlSmoObject.PropagateInfo> propInfoList, SqlSmoObject.PropagateAction propagateAction)
	{
		List<Urn> list = new List<Urn>();
		foreach (SqlSmoObject.PropagateInfo propInfo in propInfoList)
		{
			ICollection collection = null;
			if (propInfo.col != null)
			{
				collection = propInfo.col;
			}
			else
			{
				if (propInfo.obj == null)
				{
					continue;
				}
				collection = new SqlSmoObject[1] { propInfo.obj };
			}
			if ((!string.IsNullOrEmpty(propInfo.UrnTypeKey) && filteredUrnTypes.Contains(new UrnTypeKey(propInfo.UrnTypeKey))) || (!propInfo.bWithScript && !propInfo.bPropagateScriptToChildLevel))
			{
				continue;
			}
			IEnumerator enumerator2 = ((!(collection is SmoCollectionBase smoCollectionBase)) ? collection.GetEnumerator() : smoCollectionBase.GetEnumerator(Preferences));
			while (enumerator2.MoveNext())
			{
				SqlSmoObject sqlSmoObject = (SqlSmoObject)enumerator2.Current;
				if (sqlSmoObject.IsSupportedObject(sqlSmoObject.GetType(), Preferences))
				{
					if (propInfo.bWithScript)
					{
						creatingDictionary.Add(sqlSmoObject);
						list.Add(sqlSmoObject.Urn);
					}
					SqlSmoObject.PropagateInfo[] propagateInfoForDiscovery = sqlSmoObject.GetPropagateInfoForDiscovery(propagateAction);
					if (propagateInfoForDiscovery != null)
					{
						list.AddRange(GetScriptableChildren(new List<SqlSmoObject.PropagateInfo>(propagateInfoForDiscovery), propagateAction));
					}
				}
			}
		}
		return list;
	}

	private SqlSmoObject.PropagateAction GetPropagateAction()
	{
		switch (Preferences.Behavior)
		{
		case ScriptBehavior.Create:
			return SqlSmoObject.PropagateAction.Create;
		case ScriptBehavior.Drop:
			return SqlSmoObject.PropagateAction.Drop;
		case ScriptBehavior.CreateOrAlter:
			return SqlSmoObject.PropagateAction.CreateOrAlter;
		case ScriptBehavior.DropAndCreate:
			return SqlSmoObject.PropagateAction.Create;
		default:
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(condition: false, "Invalid Condition");
			return SqlSmoObject.PropagateAction.Create;
		}
	}

	private HashSet<Urn> ReferenceDiscovery(HashSet<Urn> urns)
	{
		if (Preferences.IgnoreDependencyError)
		{
			List<Urn> list = new List<Urn>();
			foreach (Urn urn in urns)
			{
				if (DiscoverSupported(urn) && !creatingDictionary.ContainsKey(urn))
				{
					list.Add(urn);
				}
			}
			urns.UnionWith(CallDependencyWalker(list.ToArray()));
		}
		else
		{
			Urn[] array = new Urn[urns.Count];
			urns.CopyTo(array);
			urns = CallDependencyWalker(array);
		}
		return urns;
	}

	private HashSet<Urn> CallDependencyWalker(Urn[] urns)
	{
		HashSet<Urn> hashSet = new HashSet<Urn>();
		if (urns.Length > 0)
		{
			new DependencyCollection();
			bool parents = Preferences.Behavior == ScriptBehavior.Create;
			DependencyWalker dependencyWalker = new DependencyWalker(Server);
			DependencyTree dependencyTree = dependencyWalker.DiscoverDependencies(urns, parents);
			{
				foreach (Dependency dependency in dependencyTree.Dependencies)
				{
					hashSet.Add(dependency.Urn);
				}
				return hashSet;
			}
		}
		return hashSet;
	}

	private bool DiscoverSupported(Urn urn)
	{
		switch (urn.Type)
		{
		case "Table":
		case "UserDefinedFunction":
		case "View":
		case "StoredProcedure":
		case "Default":
		case "Rule":
		case "Trigger":
		case "UserDefinedAggregate":
		case "Synonym":
		case "Sequence":
		case "UserDefinedDataType":
		case "XmlSchemaCollection":
		case "UserDefinedType":
		case "SqlAssembly":
		case "ExternalLibrary":
		case "PartitionScheme":
		case "PartitionFunction":
		case "UserDefinedTableType":
		case "UnresolvedEntity":
		case "DdlTrigger":
		case "PlanGuide":
			return true;
		default:
			return false;
		}
	}
}
