using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal abstract class DatabasePrefetchBase : IDatabasePrefetch
{
	protected Database Database;

	protected ScriptingPreferences scriptingPreferences;

	protected HashSet<UrnTypeKey> filteredTypes;

	protected HashSet<string> prefetchableTypes;

	protected Dictionary<string, List<Urn>> batchedPrefetchDictionary;

	protected int batchSize;

	public CreatingObjectDictionary creatingDictionary { get; set; }

	public DatabasePrefetchBase(Database db, ScriptingPreferences scriptingPreferences, HashSet<UrnTypeKey> filteredTypes)
	{
		Database = db;
		this.scriptingPreferences = scriptingPreferences;
		this.filteredTypes = filteredTypes;
		InitializePrefetchableTypes();
		InitializeBatchedPrefetchDictionary();
	}

	private void InitializeObjectCollection(string type)
	{
		if (type.Equals(Table.UrnSuffix) && !Database.Tables.initialized)
		{
			Database.InitChildLevel(Table.UrnSuffix, scriptingPreferences, forScripting: false);
		}
		else if (type.Equals(View.UrnSuffix) && !Database.Views.initialized)
		{
			Database.InitChildLevel(View.UrnSuffix, scriptingPreferences, forScripting: false);
		}
	}

	protected abstract void InitializeBatchedPrefetchDictionary();

	protected abstract void InitializePrefetchableTypes();

	public virtual IEnumerable<Urn> PrefetchObjects(IEnumerable<Urn> input)
	{
		foreach (Urn item in input)
		{
			if (prefetchableTypes.Contains(item.Type) && Database.Parent.CompareUrn(Database.Urn, item.Parent) == 0 && !creatingDictionary.ContainsKey(item))
			{
				if (batchedPrefetchDictionary.TryGetValue(item.Type, out var urnList))
				{
					InitializeObjectCollection(item.Type);
					AddUrn(item);
					urnList.Add(item);
				}
				else
				{
					PrefetchAllObjects(item.Type);
					prefetchableTypes.Remove(item.Type);
					yield return item;
				}
			}
			else if (item.Type == "Database")
			{
				Database.PrefetchScriptingOnlyChildren(scriptingPreferences);
			}
			else
			{
				yield return item;
			}
		}
		foreach (Urn item4 in PrePrefetchBatches())
		{
			yield return item4;
		}
		foreach (KeyValuePair<string, List<Urn>> item2 in batchedPrefetchDictionary.SkipWhile((KeyValuePair<string, List<Urn>> kvp) => kvp.Value.Count > batchSize))
		{
			KeyValuePair<string, List<Urn>> keyValuePair = item2;
			if (keyValuePair.Value.Count <= 0)
			{
				continue;
			}
			KeyValuePair<string, List<Urn>> keyValuePair2 = item2;
			HashSet<Urn> urnBatch = new HashSet<Urn>(keyValuePair2.Value);
			DatabasePrefetchBase databasePrefetchBase = this;
			KeyValuePair<string, List<Urn>> keyValuePair3 = item2;
			databasePrefetchBase.PrefetchBatch(keyValuePair3.Key, urnBatch, 1, 1);
			KeyValuePair<string, List<Urn>> keyValuePair4 = item2;
			foreach (Urn item5 in keyValuePair4.Value)
			{
				yield return item5;
			}
			DatabasePrefetchBase databasePrefetchBase2 = this;
			KeyValuePair<string, List<Urn>> keyValuePair5 = item2;
			databasePrefetchBase2.PostPrefetchBatch(keyValuePair5.Key, urnBatch, 1, 1);
			KeyValuePair<string, List<Urn>> keyValuePair6 = item2;
			keyValuePair6.Value.Clear();
		}
		foreach (KeyValuePair<string, List<Urn>> item3 in batchedPrefetchDictionary)
		{
			int batchCount = 0;
			KeyValuePair<string, List<Urn>> keyValuePair7 = item3;
			int num = keyValuePair7.Value.Count / batchSize;
			int totalBatchCount = num;
			KeyValuePair<string, List<Urn>> keyValuePair8 = item3;
			if (keyValuePair8.Value.Count % batchSize != 0)
			{
				totalBatchCount++;
			}
			HashSet<Urn> urnBatch2 = new HashSet<Urn>();
			KeyValuePair<string, List<Urn>> keyValuePair9 = item3;
			foreach (Urn table in keyValuePair9.Value)
			{
				if (urnBatch2.Count >= batchSize)
				{
					DatabasePrefetchBase databasePrefetchBase3 = this;
					KeyValuePair<string, List<Urn>> keyValuePair10 = item3;
					string key = keyValuePair10.Key;
					int currentBatchCount;
					batchCount = (currentBatchCount = batchCount + 1);
					databasePrefetchBase3.PrefetchBatch(key, urnBatch2, currentBatchCount, totalBatchCount);
					foreach (Urn item6 in urnBatch2)
					{
						yield return item6;
					}
					DatabasePrefetchBase databasePrefetchBase4 = this;
					KeyValuePair<string, List<Urn>> keyValuePair11 = item3;
					databasePrefetchBase4.PostPrefetchBatch(keyValuePair11.Key, urnBatch2, batchCount, totalBatchCount);
					urnBatch2.Clear();
				}
				urnBatch2.Add(table);
			}
			if (urnBatch2.Count <= 0)
			{
				continue;
			}
			DatabasePrefetchBase databasePrefetchBase5 = this;
			KeyValuePair<string, List<Urn>> keyValuePair12 = item3;
			string key2 = keyValuePair12.Key;
			int currentBatchCount2;
			batchCount = (currentBatchCount2 = batchCount + 1);
			databasePrefetchBase5.PrefetchBatch(key2, urnBatch2, currentBatchCount2, totalBatchCount);
			foreach (Urn item7 in urnBatch2)
			{
				yield return item7;
			}
			DatabasePrefetchBase databasePrefetchBase6 = this;
			KeyValuePair<string, List<Urn>> keyValuePair13 = item3;
			databasePrefetchBase6.PostPrefetchBatch(keyValuePair13.Key, urnBatch2, batchCount, totalBatchCount);
		}
	}

	protected virtual IEnumerable<Urn> PrePrefetchBatches()
	{
		return Enumerable.Empty<Urn>();
	}

	protected virtual void PostPrefetchBatch(string urnType, HashSet<Urn> urnBatch, int currentBatchCount, int totalBatchCount)
	{
	}

	protected virtual void PrefetchBatch(string urnType, HashSet<Urn> urnBatch, int currentBatchCount, int totalBatchCount)
	{
		throw new NotImplementedException();
	}

	protected virtual void AddUrn(Urn item)
	{
		throw new NotImplementedException();
	}

	protected virtual void PrefetchAllObjects(string urnType)
	{
		switch (urnType)
		{
		case "StoredProcedure":
			Database.PrefetchStoredProcedures(scriptingPreferences);
			break;
		case "User":
			Database.PrefetchUsers(scriptingPreferences);
			break;
		case "DatabaseRole":
			Database.PrefetchDatabaseRoles(scriptingPreferences);
			break;
		case "Default":
			Database.PrefetchDefaults(scriptingPreferences);
			break;
		case "Rule":
			Database.PrefetchRules(scriptingPreferences);
			break;
		case "UserDefinedFunction":
			Database.PrefetchUserDefinedFunctions(scriptingPreferences);
			break;
		case "ExtendedStoredProcedure":
			Database.PrefetchExtendedStoredProcedures(scriptingPreferences);
			break;
		case "UserDefinedType":
			Database.PrefetchUserDefinedTypes(scriptingPreferences);
			break;
		case "UserDefinedTableType":
			Database.PrefetchUserDefinedTableTypes(scriptingPreferences);
			break;
		case "UserDefinedAggregate":
			Database.PrefetchUserDefinedAggregates(scriptingPreferences);
			break;
		case "UserDefinedDataType":
			Database.PrefetchUDDT(scriptingPreferences);
			break;
		case "XmlSchemaCollection":
			Database.PrefetchXmlSchemaCollections(scriptingPreferences);
			break;
		case "SqlAssembly":
			Database.PrefetchSqlAssemblies(scriptingPreferences);
			break;
		case "Schema":
			Database.PrefetchSchemas(scriptingPreferences);
			break;
		case "PartitionScheme":
			Database.PrefetchPartitionSchemes(scriptingPreferences);
			break;
		case "PartitionFunction":
			Database.PrefetchPartitionFunctions(scriptingPreferences);
			break;
		case "Table":
			Database.PrefetchTables(scriptingPreferences);
			break;
		case "View":
			Database.PrefetchViews(scriptingPreferences);
			break;
		}
	}

	protected List<string> GetTablePrefetchList()
	{
		List<string> list = new List<string>();
		list.Add(string.Empty);
		if (Database.IsSupportedObject<Column>(scriptingPreferences))
		{
			list.Add("/Column");
			if (!filteredTypes.Contains(new UrnTypeKey(ExtendedProperty.UrnSuffix)) && Database.IsSupportedObject<ExtendedProperty>(scriptingPreferences))
			{
				list.Add("/Column/ExtendedProperty");
			}
			if (scriptingPreferences.IncludeScripts.Permissions)
			{
				list.Add("/Column/Permission");
			}
			if (Database.IsSupportedObject<Default>(scriptingPreferences))
			{
				list.Add("/Column/Default");
				if (!filteredTypes.Contains(new UrnTypeKey("DefaultColumn")) && !filteredTypes.Contains(new UrnTypeKey(ExtendedProperty.UrnSuffix)) && Database.IsSupportedObject<ExtendedProperty>(scriptingPreferences))
				{
					list.Add("/Column/Default/ExtendedProperty");
				}
			}
		}
		if (Database.IsSupportedObject<Index>(scriptingPreferences))
		{
			list.Add("/Index");
			if (!filteredTypes.Contains(new UrnTypeKey(ExtendedProperty.UrnSuffix)) && Database.IsSupportedObject<ExtendedProperty>(scriptingPreferences))
			{
				list.Add("/Index/ExtendedProperty");
			}
			if (Database.IsSupportedObject<IndexedColumn>(scriptingPreferences))
			{
				list.Add("/Index/IndexedColumn");
			}
		}
		if (Database.IsSupportedObject<FullTextIndex>(scriptingPreferences))
		{
			list.Add("/FullTextIndex");
			if (Database.IsSupportedObject<FullTextIndexColumn>(scriptingPreferences))
			{
				list.Add("/FullTextIndex/FullTextIndexColumn");
			}
		}
		if (!filteredTypes.Contains(new UrnTypeKey(ExtendedProperty.UrnSuffix)) && Database.IsSupportedObject<ExtendedProperty>(scriptingPreferences))
		{
			list.Add("/ExtendedProperty");
		}
		if (!filteredTypes.Contains(new UrnTypeKey(Check.UrnSuffix)))
		{
			list.Add("/Check");
			if (!filteredTypes.Contains(new UrnTypeKey(ExtendedProperty.UrnSuffix)) && Database.IsSupportedObject<ExtendedProperty>(scriptingPreferences))
			{
				list.Add("/Check/ExtendedProperty");
			}
		}
		if (!filteredTypes.Contains(new UrnTypeKey(ForeignKey.UrnSuffix)) && Database.IsSupportedObject<ForeignKey>(scriptingPreferences))
		{
			list.Add("/ForeignKey");
			list.Add("/ForeignKey/Column");
			if (!filteredTypes.Contains(new UrnTypeKey(ExtendedProperty.UrnSuffix)) && Database.IsSupportedObject<ExtendedProperty>(scriptingPreferences))
			{
				list.Add("/ForeignKey/ExtendedProperty");
			}
		}
		if (!filteredTypes.Contains(new UrnTypeKey(Trigger.UrnSuffix)) && Database.IsSupportedObject<Trigger>(scriptingPreferences))
		{
			list.Add("/Trigger");
			if (!filteredTypes.Contains(new UrnTypeKey(ExtendedProperty.UrnSuffix)) && Database.IsSupportedObject<ExtendedProperty>(scriptingPreferences))
			{
				list.Add("/Trigger/ExtendedProperty");
			}
		}
		if (!filteredTypes.Contains(new UrnTypeKey(Statistic.UrnSuffix)) && Database.IsSupportedObject<Statistic>(scriptingPreferences))
		{
			list.Add("/Statistic");
			list.Add("/Statistic/Column");
		}
		if (scriptingPreferences.IncludeScripts.Permissions)
		{
			list.Add("/Permission");
		}
		return list;
	}

	protected List<string> GetViewPrefetchList()
	{
		List<string> list = new List<string>();
		list.Add(string.Empty);
		if (Database.IsSupportedObject<Column>(scriptingPreferences))
		{
			list.Add("/Column");
			if (!filteredTypes.Contains(new UrnTypeKey(ExtendedProperty.UrnSuffix)) && Database.IsSupportedObject<ExtendedProperty>(scriptingPreferences))
			{
				list.Add("/Column/ExtendedProperty");
			}
			if (scriptingPreferences.IncludeScripts.Permissions)
			{
				list.Add("/Column/Permission");
			}
			if (Database.IsSupportedObject<Default>(scriptingPreferences))
			{
				list.Add("/Column/Default");
				if (!filteredTypes.Contains(new UrnTypeKey("DefaultColumn")) && !filteredTypes.Contains(new UrnTypeKey(ExtendedProperty.UrnSuffix)) && Database.IsSupportedObject<ExtendedProperty>(scriptingPreferences))
				{
					list.Add("/Column/Default/ExtendedProperty");
				}
			}
		}
		if (Database.IsSupportedObject<Index>(scriptingPreferences))
		{
			list.Add("/Index");
			if (!filteredTypes.Contains(new UrnTypeKey(ExtendedProperty.UrnSuffix)) && Database.IsSupportedObject<ExtendedProperty>(scriptingPreferences))
			{
				list.Add("/Index/ExtendedProperty");
			}
			if (Database.IsSupportedObject<IndexedColumn>(scriptingPreferences))
			{
				list.Add("/Index/IndexedColumn");
			}
		}
		if (Database.ServerVersion.Major > 8 && Database.IsSupportedObject<FullTextIndex>(scriptingPreferences))
		{
			list.Add("/FullTextIndex");
			list.Add("/FullTextIndex/FullTextIndexColumn");
		}
		if (!filteredTypes.Contains(new UrnTypeKey(ExtendedProperty.UrnSuffix)) && Database.IsSupportedObject<ExtendedProperty>(scriptingPreferences))
		{
			list.Add("/ExtendedProperty");
		}
		if (!filteredTypes.Contains(new UrnTypeKey(Trigger.UrnSuffix)) && Database.IsSupportedObject<Trigger>(scriptingPreferences))
		{
			list.Add("/Trigger");
			if (!filteredTypes.Contains(new UrnTypeKey(ExtendedProperty.UrnSuffix)) && Database.IsSupportedObject<ExtendedProperty>(scriptingPreferences))
			{
				list.Add("/Trigger/ExtendedProperty");
			}
		}
		if (!filteredTypes.Contains(new UrnTypeKey(Statistic.UrnSuffix)) && Database.IsSupportedObject<Statistic>(scriptingPreferences))
		{
			list.Add("/Statistic");
			list.Add("/Statistic/Column");
		}
		if (scriptingPreferences.IncludeScripts.Permissions)
		{
			list.Add("/Permission");
		}
		return list;
	}
}
