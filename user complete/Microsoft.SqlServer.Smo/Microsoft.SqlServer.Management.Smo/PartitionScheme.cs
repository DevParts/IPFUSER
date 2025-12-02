using System;
using System.Collections.Specialized;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[PhysicalFacet]
public sealed class PartitionScheme : ScriptNameObjectBase, ISfcSupportsDesignMode, IDroppable, IDropIfExists, IAlterable, ICreatable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 3, 4, 4, 4, 4, 4, 4, 4 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 3 };

		private static int sqlDwPropertyCount = 2;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[2]
		{
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("PartitionFunction", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[3]
		{
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("NextUsedFileGroup", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PartitionFunction", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[4]
		{
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("NextUsedFileGroup", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PartitionFunction", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState))
		};

		public override int Count
		{
			get
			{
				if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
					if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
					{
						return sqlDwPropertyCount;
					}
					int num = ((currentVersionIndex < cloudVersionCount.Length) ? currentVersionIndex : (cloudVersionCount.Length - 1));
					return cloudVersionCount[num];
				}
				int num2 = ((currentVersionIndex < versionCount.Length) ? currentVersionIndex : (versionCount.Length - 1));
				return versionCount[num2];
			}
		}

		protected override int[] VersionCount
		{
			get
			{
				if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
					if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
					{
						return new int[1] { sqlDwPropertyCount };
					}
					return cloudVersionCount;
				}
				return versionCount;
			}
		}

		internal PropertyMetadataProvider(ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
			: base(version, databaseEngineType, databaseEngineEdition)
		{
		}

		public override int PropertyNameToIDLookup(string propertyName)
		{
			if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return propertyName switch
					{
						"ID" => 0, 
						"PartitionFunction" => 1, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"ID" => 0, 
					"NextUsedFileGroup" => 1, 
					"PartitionFunction" => 2, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"ID" => 0, 
				"NextUsedFileGroup" => 1, 
				"PartitionFunction" => 2, 
				"PolicyHealthState" => 3, 
				_ => -1, 
			};
		}

		internal new static int[] GetVersionArray(DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
		{
			if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (databaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return new int[1] { sqlDwPropertyCount };
				}
				return cloudVersionCount;
			}
			return versionCount;
		}

		public override StaticMetadata GetStaticMetadata(int id)
		{
			if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return sqlDwStaticMetadata[id];
				}
				return cloudStaticMetadata[id];
			}
			return staticMetadata[id];
		}

		internal new static StaticMetadata[] GetStaticMetadataArray(DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
		{
			if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (databaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return sqlDwStaticMetadata;
				}
				return cloudStaticMetadata;
			}
			return staticMetadata;
		}
	}

	private PartitionSchemeEvents events;

	private StringCollection fileGroups;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Database Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as Database;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string NextUsedFileGroup
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("NextUsedFileGroup");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NextUsedFileGroup", value);
		}
	}

	[CLSCompliant(false)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[SfcReference(typeof(PartitionFunction), "Server[@Name = '{0}']/Database[@Name = '{1}']/PartitionFunction[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "PartitionFunction" })]
	public string PartitionFunction
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("PartitionFunction");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PartitionFunction", value);
		}
	}

	public PartitionSchemeEvents Events
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (SqlContext.IsAvailable)
			{
				throw new SmoException(ExceptionTemplatesImpl.SmoSQLCLRUnAvailable);
			}
			if (events == null)
			{
				events = new PartitionSchemeEvents(this);
			}
			return events;
		}
	}

	public static string UrnSuffix => "PartitionScheme";

	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public override string Name
	{
		get
		{
			return base.Name;
		}
		set
		{
			base.Name = value;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(ExtendedProperty))]
	public ExtendedPropertyCollection ExtendedProperties
	{
		get
		{
			ThrowIfBelowVersion80();
			CheckObjectState();
			if (m_ExtendedProperties == null)
			{
				m_ExtendedProperties = new ExtendedPropertyCollection(this);
			}
			return m_ExtendedProperties;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public StringCollection FileGroups
	{
		get
		{
			try
			{
				CheckObjectState();
				if (fileGroups == null)
				{
					if (base.State == SqlSmoState.Creating)
					{
						fileGroups = new StringCollection();
					}
					else
					{
						Request request = new Request(string.Concat(base.Urn, "/FileGroup"));
						request.Fields = new string[2] { "Name", "ID" };
						request.OrderByList = new OrderBy[1]
						{
							new OrderBy("ID", OrderBy.Direction.Asc)
						};
						DataTable enumeratorData = ExecutionManager.GetEnumeratorData(request);
						fileGroups = new StringCollection();
						foreach (DataRow row in enumeratorData.Rows)
						{
							fileGroups.Add((string)row["Name"]);
						}
					}
				}
				return fileGroups;
			}
			catch (Exception ex)
			{
				SqlSmoObject.FilterException(ex);
				throw new FailedOperationException(ExceptionTemplatesImpl.GetFileGroups, this, ex);
			}
		}
	}

	public PartitionScheme()
	{
	}

	public PartitionScheme(Database database, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = database;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[1] { "PartitionFunction" };
	}

	internal PartitionScheme(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_PARTITION_SCHEME, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE PARTITION SCHEME {0}", new object[1] { FormatFullNameForScripting(sp) });
		if (FileGroups.Count == 0)
		{
			throw new SmoException(ExceptionTemplatesImpl.ObjectWithNoChildren(GetType().Name, "FileGroup"));
		}
		string s = (string)GetPropValue("PartitionFunction");
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " AS PARTITION [{0}] TO (", new object[1] { SqlSmoObject.SqlBraket(s) });
		int num = 0;
		StringEnumerator enumerator = FileGroups.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				if (0 < num++)
				{
					stringBuilder.Append(Globals.commaspace);
				}
				stringBuilder.Append(SqlSmoObject.MakeSqlBraket((!sp.Storage.FileGroup) ? "PRIMARY" : current));
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		string propValueIfSupportedWithThrowOnTarget = GetPropValueIfSupportedWithThrowOnTarget("NextUsedFileGroup", string.Empty, sp);
		if (!string.IsNullOrEmpty(propValueIfSupportedWithThrowOnTarget))
		{
			stringBuilder.Append(Globals.commaspace);
			stringBuilder.Append(SqlSmoObject.MakeSqlBraket((!sp.Storage.FileGroup) ? "PRIMARY" : propValueIfSupportedWithThrowOnTarget));
		}
		stringBuilder.Append(")");
		queries.Add(stringBuilder.ToString());
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_PARTITION_SCHEME, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP PARTITION SCHEME {0}", new object[1] { FormatFullNameForScripting(sp) });
		queries.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		Property property = base.Properties.Get("NextUsedFileGroup");
		if (property.Value != null && (property.Dirty || sp.ScriptForCreateDrop))
		{
			ThrowIfPropertyNotSupported("NextUsedFileGroup", sp);
			string text = (string)property.Value;
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER PARTITION SCHEME {0} NEXT USED", new object[1] { FullQualifiedName });
			if (text.Length > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " [{0}]", new object[1] { SqlSmoObject.SqlBraket(text) });
			}
			alterQuery.Add(stringBuilder.ToString());
		}
	}

	public void ResetNextUsed()
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER PARTITION FUNCTION {0} NEXT USED", new object[1] { FullQualifiedName }));
			ExecutionManager.ExecuteNonQuery(stringCollection);
			if (!ExecutionManager.Recording)
			{
				base.Properties.Get("NextUsedFileGroup").SetValue(string.Empty);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ResetNextUsed, this, ex);
		}
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		return new PropagateInfo[1]
		{
			new PropagateInfo(ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
		};
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (m_ExtendedProperties != null)
		{
			m_ExtendedProperties.MarkAllDropped();
		}
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public override void Refresh()
	{
		base.Refresh();
		fileGroups = null;
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		return new string[2] { "NextUsedFileGroup", "PartitionFunction" };
	}
}
