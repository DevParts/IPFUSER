using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class Prefetch
{
	internal delegate void PrefetchEventHandler(object sender, PrefetchEventArgs e);

	private class UrnIterator : IEnumerable<Urn>, IEnumerable
	{
		private class Enumerator : IEnumerator<Urn>, IDisposable, IEnumerator
		{
			private Prefetch prefetch;

			private IList<Urn> urns;

			private IList<BatchBlock> batchBlocks;

			private int urnIndex;

			private int batchIndex;

			public Urn Current
			{
				get
				{
					if (urnIndex < 0 || urnIndex >= urns.Count)
					{
						throw new InvalidOperationException();
					}
					while (batchIndex < batchBlocks.Count)
					{
						BatchBlock batchBlock = batchBlocks[batchIndex];
						if (batchBlock.StartIndex > urnIndex)
						{
							break;
						}
						prefetch.OnBeforePrefetchObjects(batchBlock);
						batchBlock.PrefetchObjects();
						prefetch.OnAfterPrefetchObjects(batchBlock);
						batchIndex++;
					}
					return urns[urnIndex];
				}
			}

			object IEnumerator.Current => Current;

			internal Enumerator(Prefetch prefetch, IList<Urn> urns, IList<BatchBlock> batchBlocks)
			{
				this.prefetch = prefetch;
				this.urns = urns;
				this.batchBlocks = batchBlocks;
				Reset();
			}

			public void Reset()
			{
				urnIndex = -1;
				batchIndex = 0;
			}

			public bool MoveNext()
			{
				return ++urnIndex < urns.Count;
			}

			void IDisposable.Dispose()
			{
			}
		}

		private Prefetch prefetch;

		private IList<Urn> urnList;

		private List<BatchBlock> batchBlocks = new List<BatchBlock>();

		internal UrnIterator(Prefetch prefetch, IList<Urn> urns)
		{
			this.prefetch = prefetch;
			urnList = urns;
			BuildBatchBlocks();
		}

		IEnumerator<Urn> IEnumerable<Urn>.GetEnumerator()
		{
			return new Enumerator(prefetch, urnList, batchBlocks);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(prefetch, urnList, batchBlocks);
		}

		private void BuildBatchBlocks()
		{
			for (int i = 0; i < urnList.Count; i++)
			{
				Urn urn = urnList[i];
				string type = urn.Type;
				if (TryGetBatchBlock(type, out var batchBlock) && !batchBlock.TryAdd(prefetch, urn))
				{
					batchBlock = null;
				}
				if (batchBlock == null)
				{
					batchBlock = prefetch.CreateBatchBlock(type);
					if (batchBlock != null)
					{
						batchBlock.StartIndex = i;
						batchBlock.TryAdd(prefetch, urn);
						batchBlocks.Add(batchBlock);
					}
				}
			}
		}

		private bool TryGetBatchBlock(string type, out BatchBlock batchBlock)
		{
			batchBlock = null;
			for (int num = batchBlocks.Count - 1; num >= 0; num--)
			{
				if (batchBlocks[num].TypeName == type)
				{
					batchBlock = batchBlocks[num];
					return true;
				}
			}
			return false;
		}
	}

	private Database database;

	private ScriptingPreferences scriptingPreferences;

	internal Database Database => database;

	internal Server Server => database.Parent;

	internal ScriptingPreferences ScriptingPreferences => scriptingPreferences;

	internal event PrefetchEventHandler BeforePrefetch;

	internal event PrefetchEventHandler AfterPrefetch;

	internal Prefetch(Database database, ScriptingOptions scriptingOptions)
	{
		this.database = database;
		scriptingPreferences = scriptingOptions.GetScriptingPreferences();
	}

	public IEnumerable<Urn> EnumerateObjectUrns(IList<Urn> urns)
	{
		return new UrnIterator(this, urns);
	}

	private void OnBeforePrefetchObjects(BatchBlock block)
	{
		if (this.BeforePrefetch != null)
		{
			this.BeforePrefetch(this, new PrefetchEventArgs(block.TypeName, block.FilterConditionText));
		}
	}

	private void OnAfterPrefetchObjects(BatchBlock block)
	{
		if (this.AfterPrefetch != null)
		{
			this.AfterPrefetch(this, new PrefetchEventArgs(block.TypeName, block.FilterConditionText));
		}
	}

	private BatchBlock CreateBatchBlock(string typeName)
	{
		return typeName switch
		{
			"Database" => null, 
			"Table" => new LimitedBatchBlock(typeName, delegate(BatchBlock batchBlock)
			{
				Database.Tables.Clear();
				string text = string.Concat(Database.Urn, "/Table");
				string text2 = text + "[" + batchBlock.FilterConditionText + "]";
				foreach (string item in Database.EnumerateTableFiltersForPrefetch(string.Empty, ScriptingPreferences))
				{
					Server.InitQueryUrns(new Urn(text2 + item), null, null, null, ScriptingPreferences, new Urn(text + item), Database.DatabaseEngineEdition);
				}
			}), 
			"View" => new LimitedBatchBlock(typeName, delegate(BatchBlock batchBlock)
			{
				Database.Views.Clear();
				string text = string.Concat(Database.Urn, "/View");
				string text2 = text + "[" + batchBlock.FilterConditionText + "]";
				foreach (string item2 in Database.EnumerateViewFiltersForPrefetch(string.Empty, ScriptingPreferences))
				{
					Server.InitQueryUrns(new Urn(text2 + item2), null, null, null, ScriptingPreferences, new Urn(text + item2), Database.DatabaseEngineEdition);
				}
			}), 
			"StoredProcedure" => new UnlimitedBatchBlock(typeName, delegate
			{
				Database.PrefetchStoredProcedures(ScriptingPreferences);
			}), 
			"User" => new UnlimitedBatchBlock(typeName, delegate
			{
				Database.PrefetchUsers(ScriptingPreferences);
			}), 
			"DatabaseRole" => new UnlimitedBatchBlock(typeName, delegate
			{
				Database.PrefetchDatabaseRoles(ScriptingPreferences);
			}), 
			"Default" => new UnlimitedBatchBlock(typeName, delegate
			{
				Database.PrefetchDefaults(ScriptingPreferences);
			}), 
			"Rule" => new UnlimitedBatchBlock(typeName, delegate
			{
				Database.PrefetchRules(ScriptingPreferences);
			}), 
			"UserDefinedFunction" => new UnlimitedBatchBlock(typeName, delegate
			{
				Database.PrefetchUserDefinedFunctions(ScriptingPreferences);
			}), 
			"ExtendedStoredProcedure" => new UnlimitedBatchBlock(typeName, delegate
			{
				Database.PrefetchExtendedStoredProcedures(ScriptingPreferences);
			}), 
			"UserDefinedType" => new UnlimitedBatchBlock(typeName, delegate
			{
				Database.PrefetchUserDefinedTypes(ScriptingPreferences);
			}), 
			"UserDefinedTableType" => new UnlimitedBatchBlock(typeName, delegate
			{
				Database.PrefetchUserDefinedTableTypes(ScriptingPreferences);
			}), 
			"UserDefinedAggregate" => new UnlimitedBatchBlock(typeName, delegate
			{
				Database.PrefetchUserDefinedAggregates(ScriptingPreferences);
			}), 
			"UserDefinedDataType" => new UnlimitedBatchBlock(typeName, delegate
			{
				Database.PrefetchUDDT(ScriptingPreferences);
			}), 
			"XmlSchemaCollection" => new UnlimitedBatchBlock(typeName, delegate
			{
				Database.PrefetchXmlSchemaCollections(ScriptingPreferences);
			}), 
			"SqlAssembly" => new UnlimitedBatchBlock(typeName, delegate
			{
				Database.PrefetchSqlAssemblies(ScriptingPreferences);
			}), 
			"Schema" => new UnlimitedBatchBlock(typeName, delegate
			{
				Database.PrefetchSchemas(ScriptingPreferences);
			}), 
			"PartitionScheme" => new UnlimitedBatchBlock(typeName, delegate
			{
				Database.PrefetchPartitionSchemes(ScriptingPreferences);
			}), 
			"PartitionFunction" => new UnlimitedBatchBlock(typeName, delegate
			{
				Database.PrefetchPartitionFunctions(ScriptingPreferences);
			}), 
			_ => new UnlimitedBatchBlock(typeName, null), 
		};
	}
}
