using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class SmoDependencyOrderer : ISmoDependencyOrderer
{
	private Dictionary<UrnTypeKey, List<Urn>> urnTypeDictionary;

	internal CreatingObjectDictionary creatingDictionary;

	private static string CERTIFICATEKEYLOGIN = "certificatekeylogin";

	private static string CERTIFICATEKEYUSER = "certificatekeyuser";

	private static string MASTERASSEMBLY = "masterassembly";

	private static string MASTERCERTIFICATE = "mastercertificate";

	private static string MASTERASYMMETRICKEY = "masterasymmetrickey";

	private static string USERASSEMBLY = "userassembly";

	private static string USERCERTIFICATE = "usercertificate";

	private static string USERASYMMETRICKEY = "userasymmetrickey";

	private static string CLUSTEREDINDEX = "clusteredindex";

	private static string NONCLUSTEREDINDEX = "nonclusteredindex";

	private static string PRIMARYXMLINDEX = "primaryxmlindex";

	private static string SECONDARYXMLINDEX = "secondaryxmlindex";

	private static string SELECTIVEXMLINDEX = "selectivexmlindex";

	private static string SECONDARYSELECTIVEXMLINDEX = "secondaryselectivexmlindex";

	private static string SPATIALINDEX = "spatialindex";

	private static string COLUMNSTOREINDEX = "columnstoreindex";

	private static string CLUSTEREDCOLUMNSTOREINDEX = "clusteredcolumnstoreindex";

	private static string DATA = "data";

	private static string SERVERPERMISSION = "serverpermission";

	private static string DATABASEPERMISSION = "databasepermission";

	private static string SERVERASSOCIATION = "serverassociation";

	private static string DATABASEASSOCIATION = "databaseassociation";

	private static string SERVEROWNERSHIP = "serverownership";

	private static string DATABASEOWNERSHIP = "databaseownership";

	private static string DATABASEREADONLY = "databasereadonly";

	private static string CREATINGUDF = "creatingudf";

	private static string SCALARUDF = "scalarudf";

	private static string TABLEVIEWUDF = "tableviewudf";

	private static string CREATINGSPROC = "creatingsproc";

	private static string NONSCHEMABOUNDSPROC = "nonschemaboundsproc";

	private static string CREATINGVIEW = "creatingview";

	private static string CREATINGTABLE = "creatingtable";

	private static string SERVERROLESUFFIX = "roleserver";

	private static string DATABASEROLESUFFIX = "roledatabase";

	private static string DATABASEDDLTRIGGERSUFFIX = "ddltriggerdatabase";

	private static string DATABASEDDLTRIGGERENABLE = "ddltriggerdatabaseenable";

	private static string DATABASEDDLTRIGGERDISABLE = "ddltriggerdatabasedisable";

	private static string SERVERDDLTRIGGERSUFFIX = "ddltriggerserver";

	private static string SERVERDDLTRIGGERENABLE = "ddltriggerserverenable";

	private static string SERVERDDLTRIGGERDISABLE = "ddltriggerserverdisable";

	public ScriptingPreferences ScriptingPreferences { get; set; }

	internal ScriptContainerFactory ScriptContainerFactory { get; set; }

	public Server Server { get; set; }

	public List<Urn> Order(IEnumerable<Urn> urns)
	{
		Urn outUrn = null;
		int num = StoreInDictionary(urns, out outUrn);
		if (num > 1)
		{
			ResolveDependencies();
			return SortedList();
		}
		if (num > 0)
		{
			return ResolveSingleUrn(urns, outUrn);
		}
		return new List<Urn>(urns);
	}

	private List<Urn> ResolveSingleUrn(IEnumerable<Urn> urns, Urn outUrn)
	{
		if (outUrn.Type.Equals(Table.UrnSuffix))
		{
			List<Urn> list = new List<Urn>();
			if (ScriptingPreferences.IncludeScripts.Ddl)
			{
				list.Add(outUrn);
			}
			if (ScriptingPreferences.IncludeScripts.Data && !creatingDictionary.ContainsKey(outUrn))
			{
				list.Add(ConvertUrn(outUrn, "Data"));
			}
			return list;
		}
		if (ScriptingPreferences.IncludeScripts.Ddl)
		{
			if (outUrn.Type.Equals(Login.UrnSuffix) || (outUrn.Type.Equals(ServerRole.UrnSuffix) && outUrn.Parent.Type.Equals(Server.UrnSuffix)))
			{
				List<Urn> list2 = new List<Urn>();
				list2.Add(outUrn);
				if (!creatingDictionary.ContainsKey(outUrn) || creatingDictionary.SmoObjectFromUrn(outUrn).State == SqlSmoState.Existing)
				{
					list2.Add(ConvertUrn(outUrn, "Associations"));
				}
				return list2;
			}
			if (outUrn.Type.Equals(Database.UrnSuffix))
			{
				List<Urn> list3 = new List<Urn>();
				list3.Add(outUrn);
				list3.Add(ConvertUrn(outUrn, "databasereadonly"));
				return list3;
			}
			return new List<Urn>(urns);
		}
		return new List<Urn>();
	}

	private List<Urn> SortedList()
	{
		List<UrnTypeKey> list = new List<UrnTypeKey>(urnTypeDictionary.Keys);
		list.Sort();
		List<Urn> list2 = new List<Urn>();
		foreach (UrnTypeKey item in list)
		{
			if (ObjectOrder.clusteredindex != item.CreateOrder)
			{
				list2.AddRange(urnTypeDictionary[item]);
				continue;
			}
			List<Urn> list3 = urnTypeDictionary[item];
			foreach (Urn item2 in list3)
			{
				int num = list2.IndexOf(item2.Parent);
				if (num >= 0)
				{
					list2.Insert(num + 1, item2);
				}
				else
				{
					list2.Add(item2);
				}
			}
		}
		return list2;
	}

	private int StoreInDictionary(IEnumerable<Urn> urns, out Urn outUrn)
	{
		outUrn = null;
		int num = 0;
		urnTypeDictionary.Clear();
		if (ScriptingPreferences.IncludeScripts.Ddl)
		{
			foreach (Urn urn in urns)
			{
				AddToDictionary(urn);
				num++;
				outUrn = urn;
			}
		}
		else
		{
			foreach (Urn urn2 in urns)
			{
				if (urn2.Type.Equals(Table.UrnSuffix))
				{
					AddToDictionary(urn2);
					num++;
					outUrn = urn2;
				}
			}
		}
		return num;
	}

	private void AddToDictionary(Urn urn)
	{
		UrnTypeKey key = new UrnTypeKey(urn);
		if (urnTypeDictionary.ContainsKey(key))
		{
			urnTypeDictionary[key].Add(urn);
			return;
		}
		List<Urn> list = new List<Urn>();
		list.Add(urn);
		urnTypeDictionary.Add(key, list);
	}

	public SmoDependencyOrderer(Server srv)
	{
		Server = srv;
		urnTypeDictionary = new Dictionary<UrnTypeKey, List<Urn>>();
		creatingDictionary = null;
	}

	private void ResolveDependencies()
	{
		if (ScriptingPreferences.IncludeScripts.Ddl)
		{
			ResolveSqlAssemblyDependencies();
			ResolveDdlTriggerDependencies();
			ResolveSecurityObjectDependencies();
			if (ScriptingPreferences.IncludeScripts.Data)
			{
				AddTableData();
			}
			ResolveIndexDependencies();
			if (ScriptingPreferences.Behavior != ScriptBehavior.Drop)
			{
				EmbedForeignKeysChecksDefaultConstraints();
			}
			else
			{
				AddForeignKeys();
			}
			ResolveTableViewUDFSprocDependencies();
		}
		else
		{
			ResolveTableOnlyDependencies();
			urnTypeDictionary.Remove(new UrnTypeKey(Table.UrnSuffix));
		}
	}

	private void ResolveSqlAssemblyDependencies()
	{
		List<SqlAssembly> list = GetList<SqlAssembly>(SqlAssembly.UrnSuffix);
		if (list == null || list.Count <= 1)
		{
			return;
		}
		List<Urn> list2 = new List<Urn>();
		List<SqlAssembly> list3 = new List<SqlAssembly>();
		foreach (SqlAssembly item in list)
		{
			if (item.State == SqlSmoState.Creating)
			{
				list2.Add(item.Urn);
			}
			else
			{
				list3.Add(item);
			}
		}
		List<Urn> list4 = new List<Urn>();
		string query = "select assembly_id,referenced_assembly_id from sys.assembly_references where assembly_id in ({0}) and referenced_assembly_id in ({0})";
		ExecuteQuery(list4, list3.ConvertAll((Converter<SqlAssembly, SqlSmoObject>)((SqlAssembly p) => p)), query);
		list4.AddRange(list2);
		urnTypeDictionary[new UrnTypeKey(SqlAssembly.UrnSuffix)] = list4;
	}

	private void ResolveDdlTriggerDependencies()
	{
		ResolveServerDdlTriggerDependencies();
		ResolveDatabaseDdlTriggerDependencies();
	}

	private void ResolveServerDdlTriggerDependencies()
	{
		if (!urnTypeDictionary.TryGetValue(new UrnTypeKey(SERVERDDLTRIGGERSUFFIX), out var value) || value.Count <= 1)
		{
			return;
		}
		if ((ScriptingPreferences.Behavior & ScriptBehavior.Create) == ScriptBehavior.Create)
		{
			List<Urn> value2 = value.ConvertAll((Urn p) => ConvertUrn(p, SERVERDDLTRIGGERENABLE));
			urnTypeDictionary.Add(new UrnTypeKey(SERVERDDLTRIGGERENABLE), value2);
		}
		if ((ScriptingPreferences.Behavior & ScriptBehavior.Drop) == ScriptBehavior.Drop)
		{
			List<Urn> value3 = value.ConvertAll((Urn p) => ConvertUrn(p, SERVERDDLTRIGGERDISABLE));
			urnTypeDictionary.Add(new UrnTypeKey(SERVERDDLTRIGGERDISABLE), value3);
		}
		MarkUrnListSpecial(SERVERDDLTRIGGERSUFFIX);
	}

	private void ResolveDatabaseDdlTriggerDependencies()
	{
		if (!urnTypeDictionary.TryGetValue(new UrnTypeKey(DATABASEDDLTRIGGERSUFFIX), out var value) || value.Count <= 1)
		{
			return;
		}
		if ((ScriptingPreferences.Behavior & ScriptBehavior.Create) == ScriptBehavior.Create)
		{
			List<Urn> value2 = value.ConvertAll((Urn p) => ConvertUrn(p, DATABASEDDLTRIGGERENABLE));
			urnTypeDictionary.Add(new UrnTypeKey(DATABASEDDLTRIGGERENABLE), value2);
		}
		if ((ScriptingPreferences.Behavior & ScriptBehavior.Drop) == ScriptBehavior.Drop)
		{
			List<Urn> value3 = value.ConvertAll((Urn p) => ConvertUrn(p, DATABASEDDLTRIGGERDISABLE));
			urnTypeDictionary.Add(new UrnTypeKey(DATABASEDDLTRIGGERDISABLE), value3);
		}
		MarkUrnListSpecial(DATABASEDDLTRIGGERSUFFIX);
	}

	private void ResolveIndexDependencies()
	{
		if (ScriptContainerFactory == null)
		{
			ResolveIndexDependenciesWithoutFactory();
		}
		else
		{
			ResolveIndexDependenciesWithFactory();
		}
	}

	private void ResolveIndexDependenciesWithoutFactory()
	{
		List<Index> list = GetList<Index>(Index.UrnSuffix);
		if (list == null)
		{
			return;
		}
		urnTypeDictionary.TryGetValue(new UrnTypeKey(Table.UrnSuffix), out var value);
		HashSet<Urn> hashSet = new HashSet<Urn>();
		HashSet<Urn> hashSet2 = new HashSet<Urn>();
		if (value != null)
		{
			hashSet = new HashSet<Urn>(value);
			if (ScriptingPreferences.IncludeScripts.Data)
			{
				hashSet2 = new HashSet<Urn>(value.Where((Urn p) => IsFilestreamTable((Table)creatingDictionary.SmoObjectFromUrn(p), ScriptingPreferences)));
			}
		}
		List<Index> list2 = new List<Index>();
		List<Index> list3 = new List<Index>();
		List<Index> list4 = new List<Index>();
		List<Index> list5 = new List<Index>();
		List<Index> list6 = new List<Index>();
		List<Index> list7 = new List<Index>();
		List<Index> list8 = new List<Index>();
		List<Index> list9 = new List<Index>();
		List<Index> list10 = new List<Index>();
		foreach (Index item in list)
		{
			if (ScriptingPreferences.IncludeScripts.Data && hashSet2.Contains(item.Urn.Parent) && IsKey(item))
			{
				AddToTable(item);
				continue;
			}
			switch (item.InferredIndexType)
			{
			case IndexType.ClusteredIndex:
				if (IsKey(item) && hashSet.Contains(item.Urn.Parent))
				{
					AddToTable(item);
				}
				else
				{
					list2.Add(item);
				}
				break;
			case IndexType.NonClusteredIndex:
				if (!item.IsMemoryOptimizedIndex)
				{
					if (IsKey(item) && !ScriptingPreferences.IncludeScripts.Data && hashSet.Contains(item.Urn.Parent))
					{
						AddToTable(item);
					}
					else
					{
						list3.Add(item);
					}
				}
				break;
			case IndexType.PrimaryXmlIndex:
				list4.Add(item);
				break;
			case IndexType.SecondaryXmlIndex:
				list5.Add(item);
				break;
			case IndexType.SpatialIndex:
				list8.Add(item);
				break;
			case IndexType.NonClusteredColumnStoreIndex:
				list9.Add(item);
				break;
			case IndexType.SelectiveXmlIndex:
				list6.Add(item);
				break;
			case IndexType.SecondarySelectiveXmlIndex:
				list7.Add(item);
				break;
			case IndexType.ClusteredColumnStoreIndex:
				list10.Add(item);
				break;
			default:
				throw new WrongPropertyValueException(item.Properties["IndexType"]);
			case IndexType.NonClusteredHashIndex:
			case IndexType.HeapIndex:
				break;
			}
		}
		urnTypeDictionary.Remove(new UrnTypeKey(Index.UrnSuffix));
		urnTypeDictionary.Add(new UrnTypeKey(CLUSTEREDINDEX), list2.ConvertAll((Index p) => p.Urn));
		urnTypeDictionary.Add(new UrnTypeKey(NONCLUSTEREDINDEX), list3.ConvertAll((Index p) => p.Urn));
		urnTypeDictionary.Add(new UrnTypeKey(PRIMARYXMLINDEX), list4.ConvertAll((Index p) => p.Urn));
		urnTypeDictionary.Add(new UrnTypeKey(SECONDARYXMLINDEX), list5.ConvertAll((Index p) => p.Urn));
		urnTypeDictionary.Add(new UrnTypeKey(SELECTIVEXMLINDEX), list6.ConvertAll((Index p) => p.Urn));
		urnTypeDictionary.Add(new UrnTypeKey(SECONDARYSELECTIVEXMLINDEX), list7.ConvertAll((Index p) => p.Urn));
		urnTypeDictionary.Add(new UrnTypeKey(SPATIALINDEX), list8.ConvertAll((Index p) => p.Urn));
		urnTypeDictionary.Add(new UrnTypeKey(COLUMNSTOREINDEX), list9.ConvertAll((Index p) => p.Urn));
		urnTypeDictionary.Add(new UrnTypeKey(CLUSTEREDCOLUMNSTOREINDEX), list10.ConvertAll((Index p) => p.Urn));
	}

	private void ResolveIndexDependenciesWithFactory()
	{
		if (!urnTypeDictionary.TryGetValue(new UrnTypeKey(Index.UrnSuffix), out var value))
		{
			return;
		}
		List<Urn> list = new List<Urn>();
		List<Urn> list2 = new List<Urn>();
		List<Urn> list3 = new List<Urn>();
		List<Urn> list4 = new List<Urn>();
		List<Urn> list5 = new List<Urn>();
		List<Urn> list6 = new List<Urn>();
		List<Urn> list7 = new List<Urn>();
		List<Urn> list8 = new List<Urn>();
		List<Urn> list9 = new List<Urn>();
		foreach (Urn item in value)
		{
			if (!ScriptContainerFactory.TryGetValue(item, out var value2))
			{
				continue;
			}
			IndexScriptContainer indexScriptContainer = (IndexScriptContainer)value2;
			switch (indexScriptContainer.IndexType)
			{
			case IndexType.ClusteredIndex:
				list.Add(item);
				break;
			case IndexType.NonClusteredIndex:
				if (!indexScriptContainer.IsMemoryOptimizedIndex)
				{
					list2.Add(item);
				}
				break;
			case IndexType.PrimaryXmlIndex:
				list3.Add(item);
				break;
			case IndexType.SecondaryXmlIndex:
				list4.Add(item);
				break;
			case IndexType.SelectiveXmlIndex:
				list5.Add(item);
				break;
			case IndexType.SecondarySelectiveXmlIndex:
				list6.Add(item);
				break;
			case IndexType.SpatialIndex:
				list7.Add(item);
				break;
			case IndexType.NonClusteredColumnStoreIndex:
				list8.Add(item);
				break;
			case IndexType.ClusteredColumnStoreIndex:
				list9.Add(item);
				break;
			default:
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(condition: false, "Invalid IndexType");
				break;
			case IndexType.NonClusteredHashIndex:
			case IndexType.HeapIndex:
				break;
			}
		}
		urnTypeDictionary.Remove(new UrnTypeKey(Index.UrnSuffix));
		urnTypeDictionary.Add(new UrnTypeKey(CLUSTEREDINDEX), list);
		urnTypeDictionary.Add(new UrnTypeKey(NONCLUSTEREDINDEX), list2);
		urnTypeDictionary.Add(new UrnTypeKey(PRIMARYXMLINDEX), list3);
		urnTypeDictionary.Add(new UrnTypeKey(SECONDARYXMLINDEX), list4);
		urnTypeDictionary.Add(new UrnTypeKey(SELECTIVEXMLINDEX), list5);
		urnTypeDictionary.Add(new UrnTypeKey(SECONDARYSELECTIVEXMLINDEX), list6);
		urnTypeDictionary.Add(new UrnTypeKey(SPATIALINDEX), list7);
		urnTypeDictionary.Add(new UrnTypeKey(COLUMNSTOREINDEX), list8);
		urnTypeDictionary.Add(new UrnTypeKey(CLUSTEREDCOLUMNSTOREINDEX), list9);
	}

	internal static bool IsSecondaryXmlIndex(Index index)
	{
		SecondaryXmlIndexType? propValueOptional = index.GetPropValueOptional<SecondaryXmlIndexType>("SecondaryXmlIndexType");
		if (propValueOptional.HasValue && propValueOptional.Value != SecondaryXmlIndexType.None)
		{
			return true;
		}
		return false;
	}

	private void AddToTable(Index index)
	{
		Table table = creatingDictionary.SmoObjectFromUrn(index.Urn.Parent) as Table;
		table.AddToIndexPropagationList(index);
	}

	internal static bool IsClustered(Index index)
	{
		bool? propValueOptional = index.GetPropValueOptional<bool>("IsClustered");
		if (propValueOptional.HasValue && propValueOptional.Value)
		{
			return true;
		}
		return false;
	}

	internal static bool IsKey(Index index)
	{
		IndexKeyType? propValueOptional = index.GetPropValueOptional<IndexKeyType>("IndexKeyType");
		if (propValueOptional.HasValue && propValueOptional.Value != IndexKeyType.None)
		{
			return true;
		}
		return false;
	}

	internal static bool IsFilestreamTable(Table table, ScriptingPreferences sp)
	{
		if (sp.Storage.FileStreamColumn && table.IsSupportedProperty("FileStreamFileGroup", sp))
		{
			if (string.IsNullOrEmpty(table.GetPropValueOptional("FileStreamFileGroup", string.Empty)))
			{
				return !string.IsNullOrEmpty(table.GetPropValueOptional("FileStreamPartitionScheme", string.Empty));
			}
			return true;
		}
		return false;
	}

	private void EmbedForeignKeysChecksDefaultConstraints()
	{
		if (!Server.IsDesignMode)
		{
			return;
		}
		urnTypeDictionary.TryGetValue(new UrnTypeKey(Table.UrnSuffix), out var value);
		if (value == null || value.Count != 1)
		{
			return;
		}
		Table table = (Table)creatingDictionary.SmoObjectFromUrn(value.First());
		List<ForeignKey> list = GetList<ForeignKey>(ForeignKey.UrnSuffix);
		if (list != null)
		{
			foreach (ForeignKey item in list)
			{
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(item.Urn.Parent.Equals(table.Urn), "invalid call");
				table.AddToEmbeddedForeignKeyChecksList(item);
			}
		}
		List<Check> list2 = GetList<Check>(Check.UrnSuffix);
		if (list2 != null)
		{
			foreach (Check item2 in list2)
			{
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(item2.Urn.Parent.Equals(table.Urn), "invalid call");
				table.AddToEmbeddedForeignKeyChecksList(item2);
			}
		}
		List<DefaultConstraint> list3 = GetList<DefaultConstraint>("DefaultColumn");
		if (list3 != null)
		{
			foreach (DefaultConstraint item3 in list3)
			{
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(item3.Urn.Parent.Parent.Equals(table.Urn), "invalid call");
				item3.forceEmbedDefaultConstraint = true;
			}
		}
		urnTypeDictionary.Remove(new UrnTypeKey(ForeignKey.UrnSuffix));
		urnTypeDictionary.Remove(new UrnTypeKey(Check.UrnSuffix));
		urnTypeDictionary.Remove(new UrnTypeKey("DefaultColumn"));
	}

	private void AddTableData(List<Urn> tableList)
	{
		List<Urn> value = tableList.ConvertAll((Urn p) => ConvertUrn(p, "Data"));
		urnTypeDictionary.Add(new UrnTypeKey(DATA), value);
	}

	private Urn ConvertUrn(Urn p, string type)
	{
		return new Urn(string.Format(SmoApplication.DefaultCulture, "{0}/{1}/Special", new object[2] { p, type }));
	}

	private void AddTableData()
	{
		if (urnTypeDictionary.TryGetValue(new UrnTypeKey(Table.UrnSuffix), out var value))
		{
			value = value.FindAll((Urn p) => !creatingDictionary.ContainsKey(p));
			AddTableData(value);
		}
	}

	private void AddForeignKeys()
	{
		List<Table> list = GetList<Table>(Table.UrnSuffix);
		if (list == null)
		{
			return;
		}
		urnTypeDictionary.TryGetValue(new UrnTypeKey(ForeignKey.UrnSuffix), out var value);
		HashSet<Urn> hashSet;
		if (value == null)
		{
			hashSet = new HashSet<Urn>();
			value = new List<Urn>();
			urnTypeDictionary.Add(new UrnTypeKey(ForeignKey.UrnSuffix), value);
		}
		else
		{
			hashSet = new HashSet<Urn>(value);
		}
		foreach (Table item in list)
		{
			foreach (ForeignKey foreignKey in item.ForeignKeys)
			{
				if (hashSet.Add(foreignKey.Urn))
				{
					value.Add(foreignKey.Urn);
				}
			}
		}
	}

	private void ResolveTableViewUDFSprocDependencies()
	{
		List<Urn> list = new List<Urn>();
		ResolveUDFDependencies(list);
		ResolveSprocDependencies(list);
		ResolveViewDependencies(list);
		if (VersionUtils.IsSql13Azure12OrLater(Server.DatabaseEngineType, Server.ServerVersion, ScriptingPreferences))
		{
			ResolveTemporalHistoryTableDependencies(list);
		}
		if (list.Count > 0)
		{
			OrderAndStoreSchemaBound(list);
		}
	}

	private void ResolveUDFDependencies(List<Urn> schemaboundList)
	{
		List<UserDefinedFunction> list = GetList<UserDefinedFunction>(UserDefinedFunction.UrnSuffix);
		if (list == null)
		{
			return;
		}
		List<Urn> list2 = new List<Urn>();
		List<Urn> list3 = new List<Urn>();
		bool flag = false;
		foreach (UserDefinedFunction item in list)
		{
			if (item.State == SqlSmoState.Creating)
			{
				list3.Add(item.Urn);
				continue;
			}
			if (!item.IsSchemaBound && item.FunctionType != UserDefinedFunctionType.Inline)
			{
				list2.Add(item.Urn);
				continue;
			}
			schemaboundList.Add(item.Urn);
			if (item.FunctionType == UserDefinedFunctionType.Scalar)
			{
				flag = true;
			}
		}
		if (list3.Count > 0)
		{
			urnTypeDictionary.Add(new UrnTypeKey(CREATINGUDF), list3);
		}
		if (list2.Count > 0)
		{
			urnTypeDictionary.Add(new UrnTypeKey(SCALARUDF), list2);
		}
		if (flag)
		{
			ResolveTableDependencies(schemaboundList);
		}
		urnTypeDictionary.Remove(new UrnTypeKey(UserDefinedFunction.UrnSuffix));
	}

	private void ResolveSprocDependencies(List<Urn> schemaboundList)
	{
		List<StoredProcedure> list = GetList<StoredProcedure>(StoredProcedure.UrnSuffix);
		if (list == null)
		{
			return;
		}
		List<Urn> list2 = new List<Urn>();
		List<Urn> list3 = new List<Urn>();
		new List<Urn>();
		bool flag = false;
		foreach (StoredProcedure item in list)
		{
			if (item.State == SqlSmoState.Creating)
			{
				list2.Add(item.Urn);
			}
			else if (item.IsSchemaBound)
			{
				schemaboundList.Add(item.Urn);
				flag = true;
			}
			else
			{
				list3.Add(item.Urn);
			}
		}
		if (list2.Count > 0)
		{
			urnTypeDictionary.Add(new UrnTypeKey(CREATINGSPROC), list2);
		}
		if (list3.Count > 0)
		{
			urnTypeDictionary.Add(new UrnTypeKey(NONSCHEMABOUNDSPROC), list3);
		}
		if (flag)
		{
			ResolveTableDependencies(schemaboundList);
		}
		urnTypeDictionary.Remove(new UrnTypeKey(StoredProcedure.UrnSuffix));
	}

	private void ResolveTableDependencies(List<Urn> schemaboundList)
	{
		if (!urnTypeDictionary.TryGetValue(new UrnTypeKey(Table.UrnSuffix), out var value))
		{
			return;
		}
		List<Urn> list = new List<Urn>();
		List<Urn> list2 = new List<Urn>();
		foreach (Urn item in value)
		{
			if (creatingDictionary.ContainsKey(item))
			{
				list2.Add(item);
			}
			else
			{
				list.Add(item);
			}
		}
		if (list2.Count > 0)
		{
			urnTypeDictionary.Add(new UrnTypeKey(CREATINGTABLE), list2);
		}
		if (list.Count > 0)
		{
			schemaboundList.AddRange(ReturnComputedColumnTables(list));
		}
		urnTypeDictionary.Remove(new UrnTypeKey(Table.UrnSuffix));
	}

	private void ResolveViewDependencies(List<Urn> schemaboundList)
	{
		if (!urnTypeDictionary.TryGetValue(new UrnTypeKey(View.UrnSuffix), out var value))
		{
			return;
		}
		List<Urn> list = new List<Urn>();
		List<Urn> list2 = new List<Urn>();
		foreach (Urn item in value)
		{
			if (creatingDictionary.ContainsKey(item))
			{
				list2.Add(item);
			}
			else
			{
				list.Add(item);
			}
		}
		if (list.Count > 0)
		{
			schemaboundList.AddRange(list);
		}
		if (list2.Count > 0)
		{
			urnTypeDictionary.Add(new UrnTypeKey(CREATINGVIEW), list2);
		}
		urnTypeDictionary.Remove(new UrnTypeKey(View.UrnSuffix));
	}

	private void ResolveTemporalHistoryTableDependencies(List<Urn> schemaboundList)
	{
		if (!urnTypeDictionary.TryGetValue(new UrnTypeKey(Table.UrnSuffix), out var value))
		{
			return;
		}
		List<Urn> list = new List<Urn>();
		List<Urn> list2 = new List<Urn>();
		foreach (Urn item in value)
		{
			if (creatingDictionary.ContainsKey(item))
			{
				list2.Add(item);
			}
			else
			{
				list.Add(item);
			}
		}
		if (list2.Count > 0)
		{
			urnTypeDictionary.Add(new UrnTypeKey(CREATINGTABLE), list2);
		}
		if (list.Count > 0)
		{
			schemaboundList.AddRange(list);
		}
		urnTypeDictionary.Remove(new UrnTypeKey(Table.UrnSuffix));
	}

	private void OrderAndStoreSchemaBound(List<Urn> schemaboundList)
	{
		urnTypeDictionary.Add(new UrnTypeKey(TABLEVIEWUDF), new List<Urn>());
		Dictionary<Urn, List<Urn>> dictionary = new Dictionary<Urn, List<Urn>>();
		foreach (Urn schemabound in schemaboundList)
		{
			if (dictionary.ContainsKey(schemabound.Parent))
			{
				dictionary[schemabound.Parent].Add(schemabound);
				continue;
			}
			List<Urn> list = new List<Urn>();
			list.Add(schemabound);
			dictionary.Add(schemabound.Parent, list);
		}
		string query = (VersionUtils.IsSql13Azure12OrLater(Server.DatabaseEngineType, Server.ServerVersion, ScriptingPreferences) ? "select dep.referencing_id,dep.referenced_id from sys.sql_expression_dependencies as dep join #tempordering as t1 on dep.referenced_id = t1.ID join #tempordering as t2 on dep.referencing_id = t2.ID where dep.referenced_id != dep.referencing_id UNION select [object_id], [history_table_id] from sys.tables where [temporal_type] = 2" : ((Server.VersionMajor > 9) ? "select dep.referencing_id,dep.referenced_id from sys.sql_expression_dependencies as dep join #tempordering as t1 on dep.referenced_id = t1.ID join #tempordering as t2 on dep.referencing_id = t2.ID where dep.referenced_id != dep.referencing_id " : ((Server.VersionMajor <= 8) ? "select dep.id,dep.depid from dbo.sysdepends as dep join #tempordering as t1 on dep.depid = t1.ID join #tempordering as t2 on dep.id = t2.ID where dep.depid != dep.id" : "select dep.object_id,dep.referenced_major_id from sys.sql_dependencies as dep join #tempordering as t1 on dep.referenced_major_id = t1.ID join #tempordering as t2 on dep.object_id = t2.ID where dep.referenced_major_id != dep.object_id")));
		foreach (KeyValuePair<Urn, List<Urn>> item in dictionary)
		{
			OrderAndStoreSchemaBoundInSingleDatabase(item.Value, query);
		}
	}

	private void OrderAndStoreSchemaBoundInSingleDatabase(List<Urn> list, string query)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(list.Count > 0);
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(urnTypeDictionary.ContainsKey(new UrnTypeKey(TABLEVIEWUDF)));
		List<Urn> list2 = urnTypeDictionary[new UrnTypeKey(TABLEVIEWUDF)];
		if (list.Count > 1)
		{
			ExecuteQueryUsingTempTable(list2, list, query);
		}
		else
		{
			list2.Add(list[0]);
		}
	}

	private void ExecuteQuery(List<Urn> objectList, List<SqlSmoObject> list, string query)
	{
		Dictionary<int, Urn> dictionary = new Dictionary<int, Urn>();
		StringBuilder stringBuilder = new StringBuilder();
		foreach (SqlSmoObject item in list)
		{
			int num = (int)item.Properties.GetValueWithNullReplacement("ID");
			dictionary.Add(num, item.Urn);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0},", new object[1] { num });
		}
		stringBuilder.Remove(stringBuilder.Length - 1, 1);
		string sqlCommand = string.Format(SmoApplication.DefaultCulture, query, new object[1] { stringBuilder.ToString() });
		Database database = (Database)creatingDictionary.SmoObjectFromUrn(list[0].Urn.Parent);
		DataSet ds = database.ExecuteWithResults(sqlCommand);
		SortDataSet(objectList, dictionary, ds);
	}

	private void ExecuteQueryUsingTempTable(List<Urn> objectList, List<Urn> list, string query)
	{
		Database database = (Database)creatingDictionary.SmoObjectFromUrn(list[0].Parent);
		Dictionary<int, Urn> dictionary = new Dictionary<int, Urn>();
		StringBuilder stringBuilder = new StringBuilder();
		int num = ((Server.VersionMajor <= 9) ? 1 : 1000);
		StringCollection stringCollection = new StringCollection();
		if (database.IsSqlDw)
		{
			stringCollection.Add("create table #tempordering(ID int)");
			num = 1;
		}
		else
		{
			stringCollection.Add("create table #tempordering(ID int primary key)");
		}
		int num2 = 0;
		StringBuilder stringBuilder2 = new StringBuilder();
		foreach (Urn item in list)
		{
			int idFromUrn = GetIdFromUrn(item);
			dictionary.Add(idFromUrn, item);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "({0}),", new object[1] { idFromUrn });
			num2++;
			if (num2 % num == 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "insert into #tempordering(ID) values {0};", new object[1] { stringBuilder.ToString() });
				stringBuilder.Length = 0;
				num2 = 0;
			}
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "insert into #tempordering(ID) values {0};", new object[1] { stringBuilder.ToString() });
			stringBuilder.Length = 0;
		}
		stringCollection.Add(stringBuilder2.ToString());
		stringCollection.Add(query);
		stringCollection.Add("drop table #tempordering");
		DataSet ds = database.ExecuteWithResults(stringCollection);
		SortDataSet(objectList, dictionary, ds);
	}

	private int GetIdFromUrn(Urn urn)
	{
		if (ScriptContainerFactory != null && ScriptContainerFactory.TryGetValue(urn, out var value) && value is IdBasedObjectScriptContainer idBasedObjectScriptContainer)
		{
			return idBasedObjectScriptContainer.ID;
		}
		return (int)creatingDictionary.SmoObjectFromUrn(urn).Properties.GetValueWithNullReplacement("ID");
	}

	private void SortDataSet(List<Urn> objectList, Dictionary<int, Urn> idDictionary, DataSet ds)
	{
		List<int> list = SortDataSet(ds);
		foreach (int item in list)
		{
			if (idDictionary.ContainsKey(item))
			{
				objectList.Add(idDictionary[item]);
				idDictionary.Remove(item);
			}
		}
		foreach (KeyValuePair<int, Urn> item2 in idDictionary)
		{
			objectList.Add(item2.Value);
		}
	}

	private List<int> SortDataSet(DataSet ds)
	{
		DataTable dataTable = ds.Tables[0];
		Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
		foreach (DataRow row in dataTable.Rows)
		{
			if (dictionary.ContainsKey((int)row[0]))
			{
				dictionary[(int)row[0]].Add((int)row[1]);
				continue;
			}
			List<int> list = new List<int>();
			list.Add((int)row[1]);
			dictionary.Add((int)row[0], list);
		}
		ds.Dispose();
		return SortDictionary(dictionary);
	}

	private List<int> SortDictionary(Dictionary<int, List<int>> dictionary)
	{
		List<int> list = new List<int>();
		HashSet<int> visited = new HashSet<int>();
		HashSet<int> current = new HashSet<int>();
		foreach (int key in dictionary.Keys)
		{
			DependencyGraphTraversal(key, dictionary, list, visited, current);
		}
		return list;
	}

	private void DependencyGraphTraversal(int num, Dictionary<int, List<int>> dictionary, List<int> sortedList, HashSet<int> visited, HashSet<int> current)
	{
		if (visited.Contains(num))
		{
			return;
		}
		if (!current.Add(num))
		{
			throw new SmoException(ExceptionTemplatesImpl.OrderingCycleDetected);
		}
		if (dictionary.ContainsKey(num))
		{
			foreach (int item in dictionary[num])
			{
				DependencyGraphTraversal(item, dictionary, sortedList, visited, current);
			}
		}
		sortedList.Add(num);
		visited.Add(num);
		current.Remove(num);
	}

	private List<Urn> ReturnComputedColumnTables(List<Urn> existingTableList)
	{
		return existingTableList;
	}

	private void ResolveTableOnlyDependencies()
	{
		if (urnTypeDictionary.TryGetValue(new UrnTypeKey(Table.UrnSuffix), out var value))
		{
			value = value.FindAll((Urn p) => !creatingDictionary.ContainsKey(p));
			if (value.Count > 1)
			{
				List<Urn> list = new List<Urn>();
				string query = ((Server.VersionMajor <= 8) ? "select fk.fkeyid,fk.rkeyid from dbo.sysreferences as fk join #tempordering as t1 on fk.rkeyid = t1.ID join #tempordering as t2 on fk.fkeyid = t2.ID where fk.rkeyid != fk.fkeyid" : "select fk.parent_object_id,fk.referenced_object_id from sys.foreign_key_columns as fk join #tempordering as t1 on fk.referenced_object_id = t1.ID join #tempordering as t2 on fk.parent_object_id = t2.ID where fk.referenced_object_id != fk.parent_object_id");
				ExecuteQueryUsingTempTable(list, value, query);
				AddTableData(list);
			}
			else
			{
				AddTableData(value);
			}
		}
	}

	private List<T> GetList<T>(string UrnSuffix) where T : SqlSmoObject
	{
		List<T> result = null;
		if (urnTypeDictionary.ContainsKey(new UrnTypeKey(UrnSuffix)))
		{
			result = urnTypeDictionary[new UrnTypeKey(UrnSuffix)].ConvertAll((Urn p) => creatingDictionary.SmoObjectFromUrn(p) as T);
		}
		return result;
	}

	private void ResolveSecurityObjectDependencies()
	{
		ResolveServerSecurityObjectDependencies();
		ResolveDatabaseSecurityObjectDependencies();
		if (ScriptingPreferences.Behavior == ScriptBehavior.Drop)
		{
			return;
		}
		bool flag = AddServerAssociations();
		flag = AddDatabaseReadOnly() || flag;
		if (ScriptingPreferences.IncludeScripts.Associations)
		{
			AddDatabaseAssociations();
			flag = true;
		}
		if (ScriptingPreferences.IncludeScripts.Owner)
		{
			AddOwner();
			flag = true;
		}
		if (ScriptingPreferences.IncludeScripts.Permissions)
		{
			AddPermissions();
			flag = true;
		}
		if (flag)
		{
			ChangeUrns();
		}
		if (urnTypeDictionary.ContainsKey(new UrnTypeKey("UnresolvedEntity")))
		{
			urnTypeDictionary[new UrnTypeKey("UnresolvedEntity")] = urnTypeDictionary[new UrnTypeKey("UnresolvedEntity")].ConvertAll((Urn p) => ConvertUrn(p, "UnresolvedEntity"));
		}
	}

	private void ChangeUrns()
	{
		MarkUrnListSpecial(Database.UrnSuffix);
		MarkUrnListSpecial(Login.UrnSuffix);
		MarkUrnListSpecial(MASTERASSEMBLY);
		MarkUrnListSpecial(MASTERCERTIFICATE);
		MarkUrnListSpecial(MASTERASYMMETRICKEY);
		MarkUrnListSpecial(CERTIFICATEKEYLOGIN);
		MarkUrnListSpecial(SERVERROLESUFFIX);
		MarkUrnListSpecial(ApplicationRole.UrnSuffix);
		MarkUrnListSpecial(User.UrnSuffix);
		MarkUrnListSpecial(USERASSEMBLY);
		MarkUrnListSpecial(USERCERTIFICATE);
		MarkUrnListSpecial(USERASYMMETRICKEY);
		MarkUrnListSpecial(CERTIFICATEKEYUSER);
		MarkUrnListSpecial(DATABASEROLESUFFIX);
	}

	private void MarkUrnListSpecial(string UrnSuffix)
	{
		if (urnTypeDictionary.ContainsKey(new UrnTypeKey(UrnSuffix)))
		{
			urnTypeDictionary[new UrnTypeKey(UrnSuffix)] = urnTypeDictionary[new UrnTypeKey(UrnSuffix)].ConvertAll((Urn p) => ConvertUrn(p, "Object"));
		}
	}

	private void AddPermissions()
	{
		List<Urn> list = new List<Urn>();
		AddConvertedUrnsToList(list, Login.UrnSuffix, "Permission");
		AddConvertedUrnsToList(list, CERTIFICATEKEYLOGIN, "Permission");
		AddConvertedUrnsToList(list, SERVERROLESUFFIX, "Permission");
		urnTypeDictionary.Add(new UrnTypeKey(SERVEROWNERSHIP), list);
		list = new List<Urn>();
		AddConvertedUrnsToList(list, Database.UrnSuffix, "Permission");
		AddConvertedUrnsToList(list, User.UrnSuffix, "Permission");
		AddConvertedUrnsToList(list, CERTIFICATEKEYUSER, "Permission");
		AddConvertedUrnsToList(list, MASTERASSEMBLY, "Permission");
		AddConvertedUrnsToList(list, MASTERCERTIFICATE, "Permission");
		AddConvertedUrnsToList(list, MASTERASYMMETRICKEY, "Permission");
		AddConvertedUrnsToList(list, USERASSEMBLY, "Permission");
		AddConvertedUrnsToList(list, USERCERTIFICATE, "Permission");
		AddConvertedUrnsToList(list, USERASYMMETRICKEY, "Permission");
		AddConvertedUrnsToList(list, DATABASEROLESUFFIX, "Permission");
		urnTypeDictionary.Add(new UrnTypeKey(DATABASEOWNERSHIP), list);
	}

	private void AddConvertedUrnsToList(List<Urn> List, string UrnSuffix, string type)
	{
		if (urnTypeDictionary.ContainsKey(new UrnTypeKey(UrnSuffix)))
		{
			List.AddRange(urnTypeDictionary[new UrnTypeKey(UrnSuffix)].ConvertAll((Urn p) => ConvertUrn(p, type)));
		}
	}

	private void AddOwner()
	{
		List<Urn> list = new List<Urn>();
		AddConvertedUrnsToList(list, SERVERROLESUFFIX, "Ownership");
		AddConvertedUrnsToList(list, Database.UrnSuffix, "Ownership");
		urnTypeDictionary.Add(new UrnTypeKey(SERVERPERMISSION), list);
		list = new List<Urn>();
		AddConvertedUrnsToList(list, MASTERASSEMBLY, "Ownership");
		AddConvertedUrnsToList(list, MASTERCERTIFICATE, "Ownership");
		AddConvertedUrnsToList(list, MASTERASYMMETRICKEY, "Ownership");
		AddConvertedUrnsToList(list, USERASSEMBLY, "Ownership");
		AddConvertedUrnsToList(list, USERCERTIFICATE, "Ownership");
		AddConvertedUrnsToList(list, USERASYMMETRICKEY, "Ownership");
		AddConvertedUrnsToList(list, DATABASEROLESUFFIX, "Ownership");
		urnTypeDictionary.Add(new UrnTypeKey(DATABASEPERMISSION), list);
	}

	private bool AddServerAssociations()
	{
		List<Urn> list = new List<Urn>();
		AddConvertedUrnsToList(list, Login.UrnSuffix, "Associations");
		AddConvertedUrnsToList(list, CERTIFICATEKEYLOGIN, "Associations");
		AddConvertedUrnsToList(list, SERVERROLESUFFIX, "Associations");
		urnTypeDictionary.Add(new UrnTypeKey(SERVERASSOCIATION), list);
		return list.Count > 0;
	}

	private bool AddDatabaseReadOnly()
	{
		List<Urn> list = new List<Urn>();
		AddConvertedUrnsToList(list, Database.UrnSuffix, "databasereadonly");
		urnTypeDictionary.Add(new UrnTypeKey(DATABASEREADONLY), list);
		return list.Count > 0;
	}

	private void AddDatabaseAssociations()
	{
		List<Urn> list = new List<Urn>();
		AddConvertedUrnsToList(list, User.UrnSuffix, "Associations");
		AddConvertedUrnsToList(list, CERTIFICATEKEYUSER, "Associations");
		AddConvertedUrnsToList(list, DATABASEROLESUFFIX, "Associations");
		urnTypeDictionary.Add(new UrnTypeKey(DATABASEASSOCIATION), list);
	}

	private void ResolveServerSecurityObjectDependencies()
	{
		List<Login> list = GetList<Login>(Login.UrnSuffix);
		if (list == null)
		{
			return;
		}
		List<Login> list2 = new List<Login>();
		List<Login> list3 = new List<Login>();
		foreach (Login item in list)
		{
			if (item.LoginType == LoginType.AsymmetricKey || item.LoginType == LoginType.Certificate)
			{
				list2.Add(item);
			}
			else
			{
				list3.Add(item);
			}
		}
		if (list2.Count > 0)
		{
			urnTypeDictionary[new UrnTypeKey(Login.UrnSuffix)] = list3.ConvertAll((Login p) => p.Urn);
			urnTypeDictionary.Add(new UrnTypeKey(CERTIFICATEKEYLOGIN), list2.ConvertAll((Login p) => p.Urn));
			FindAndAddMasterSecurityObjects(SqlAssembly.UrnSuffix, MASTERASSEMBLY);
			FindAndAddMasterSecurityObjects(Certificate.UrnSuffix, MASTERCERTIFICATE);
			FindAndAddMasterSecurityObjects(AsymmetricKey.UrnSuffix, MASTERASYMMETRICKEY);
		}
	}

	private void FindAndAddMasterSecurityObjects(string UrnSuffix, string urnTypeKey)
	{
		List<Urn> list = new List<Urn>();
		List<Urn> list2 = new List<Urn>();
		if (!urnTypeDictionary.ContainsKey(new UrnTypeKey(UrnSuffix)))
		{
			return;
		}
		foreach (Urn item in urnTypeDictionary[new UrnTypeKey(UrnSuffix)])
		{
			if (item.Parent.GetAttribute("Name").Equals("master", StringComparison.Ordinal))
			{
				list.Add(item);
			}
			else
			{
				list2.Add(item);
			}
		}
		if (list.Count > 0)
		{
			urnTypeDictionary[new UrnTypeKey(UrnSuffix)] = list2;
			urnTypeDictionary.Add(new UrnTypeKey(urnTypeKey), list);
		}
	}

	private void ResolveDatabaseSecurityObjectDependencies()
	{
		List<User> list = GetList<User>(User.UrnSuffix);
		if (list == null)
		{
			return;
		}
		List<User> list2 = new List<User>();
		List<User> list3 = new List<User>();
		foreach (User item in list)
		{
			if (item.UserType == UserType.AsymmetricKey || item.UserType == UserType.Certificate)
			{
				list2.Add(item);
			}
			else
			{
				list3.Add(item);
			}
		}
		if (list2.Count > 0)
		{
			urnTypeDictionary[new UrnTypeKey(User.UrnSuffix)] = list3.ConvertAll((User p) => p.Urn);
			urnTypeDictionary.Add(new UrnTypeKey(CERTIFICATEKEYUSER), list2.ConvertAll((User p) => p.Urn));
			AddCertificateKeyUserDependencies(SqlAssembly.UrnSuffix, USERASSEMBLY);
			AddCertificateKeyUserDependencies(Certificate.UrnSuffix, USERCERTIFICATE);
			AddCertificateKeyUserDependencies(AsymmetricKey.UrnSuffix, USERASYMMETRICKEY);
		}
	}

	private void AddCertificateKeyUserDependencies(string UrnSuffix, string urnKeyType)
	{
		if (urnTypeDictionary.ContainsKey(new UrnTypeKey(UrnSuffix)))
		{
			urnTypeDictionary.Add(new UrnTypeKey(urnKeyType), urnTypeDictionary[new UrnTypeKey(UrnSuffix)]);
			urnTypeDictionary.Remove(new UrnTypeKey(UrnSuffix));
		}
	}
}
