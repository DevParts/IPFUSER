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

[PhysicalFacet]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public sealed class PartitionFunction : ScriptNameObjectBase, ISfcSupportsDesignMode, IDroppable, IDropIfExists, IAlterable, ICreatable, IScriptable, IExtendedProperties
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 4, 5, 5, 5, 5, 5, 5, 5 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 4 };

		private static int sqlDwPropertyCount = 4;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[4]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("NumberOfPartitions", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("RangeType", expensive: false, readOnly: false, typeof(RangeType))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[4]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("NumberOfPartitions", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("RangeType", expensive: false, readOnly: false, typeof(RangeType))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[5]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("NumberOfPartitions", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("RangeType", expensive: false, readOnly: false, typeof(RangeType)),
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
						"CreateDate" => 0, 
						"ID" => 1, 
						"NumberOfPartitions" => 2, 
						"RangeType" => 3, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"CreateDate" => 0, 
					"ID" => 1, 
					"NumberOfPartitions" => 2, 
					"RangeType" => 3, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"CreateDate" => 0, 
				"ID" => 1, 
				"NumberOfPartitions" => 2, 
				"RangeType" => 3, 
				"PolicyHealthState" => 4, 
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

	private PartitionFunctionEvents events;

	private PartitionFunctionParameterCollection m_PartitionFunctionParameters;

	private object[] rangeValues;

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
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int NumberOfPartitions => (int)base.Properties.GetValueWithNullReplacement("NumberOfPartitions");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public RangeType RangeType
	{
		get
		{
			return (RangeType)base.Properties.GetValueWithNullReplacement("RangeType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RangeType", value);
		}
	}

	public PartitionFunctionEvents Events
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
				events = new PartitionFunctionEvents(this);
			}
			return events;
		}
	}

	public static string UrnSuffix => "PartitionFunction";

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[SfcKey(0)]
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

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.OneToAny, typeof(PartitionFunctionParameter))]
	public PartitionFunctionParameterCollection PartitionFunctionParameters
	{
		get
		{
			CheckObjectState();
			if (m_PartitionFunctionParameters == null)
			{
				m_PartitionFunctionParameters = new PartitionFunctionParameterCollection(this);
				m_PartitionFunctionParameters.AcceptDuplicateNames = true;
			}
			return m_PartitionFunctionParameters;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public object[] RangeValues
	{
		get
		{
			try
			{
				CheckObjectState();
				if (rangeValues == null && base.State != SqlSmoState.Creating)
				{
					Request request = new Request(string.Concat(base.Urn, "/RangeValue"));
					request.Fields = new string[2] { "Value", "ID" };
					request.OrderByList = new OrderBy[1]
					{
						new OrderBy("ID", OrderBy.Direction.Asc)
					};
					DataTable enumeratorData = ExecutionManager.GetEnumeratorData(request);
					rangeValues = new object[enumeratorData.Rows.Count];
					int num = 0;
					foreach (DataRow row in enumeratorData.Rows)
					{
						rangeValues[num++] = row["Value"];
					}
				}
				return rangeValues;
			}
			catch (Exception ex)
			{
				SqlSmoObject.FilterException(ex);
				throw new FailedOperationException(ExceptionTemplatesImpl.GetRangeValues, this, ex);
			}
		}
		set
		{
			CheckObjectState();
			rangeValues = value;
		}
	}

	public PartitionFunction()
	{
	}

	public PartitionFunction(Database database, string name)
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
		return new string[1] { "RangeType" };
	}

	internal PartitionFunction(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
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
		string text = FormatFullNameForScripting(sp);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_PARTITION_FUNCTION, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE PARTITION FUNCTION {0}", new object[1] { text });
		if (PartitionFunctionParameters.Count == 0)
		{
			throw new SmoException(ExceptionTemplatesImpl.ObjectWithNoChildren(GetType().Name, "PartitionFunctionParameter"));
		}
		stringBuilder.Append("(");
		int num = 0;
		foreach (PartitionFunctionParameter partitionFunctionParameter in PartitionFunctionParameters)
		{
			if (0 < num++)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			if (UserDefinedDataType.TypeAllowsLength(partitionFunctionParameter.Name, partitionFunctionParameter.StringComparer) && partitionFunctionParameter.Length > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}({1})", new object[2] { partitionFunctionParameter.Name, partitionFunctionParameter.Length });
			}
			else if (UserDefinedDataType.TypeAllowsPrecisionScale(partitionFunctionParameter.Name, partitionFunctionParameter.StringComparer))
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}({1},{2})", new object[3] { partitionFunctionParameter.Name, partitionFunctionParameter.NumericPrecision, partitionFunctionParameter.NumericScale });
			}
			else if (UserDefinedDataType.TypeAllowsScale(partitionFunctionParameter.Name, partitionFunctionParameter.StringComparer))
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}({1})", new object[2] { partitionFunctionParameter.Name, partitionFunctionParameter.NumericScale });
			}
			else if (DataType.IsTypeFloatStateCreating(partitionFunctionParameter.Name, partitionFunctionParameter))
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}({1})", new object[2] { partitionFunctionParameter.Name, partitionFunctionParameter.NumericPrecision });
			}
			else
			{
				stringBuilder.Append(partitionFunctionParameter.Name);
			}
		}
		stringBuilder.Append(") AS ");
		stringBuilder.Append("RANGE ");
		if (base.Properties.Get("RangeType").Value != null)
		{
			RangeType rangeType = (RangeType)base.Properties["RangeType"].Value;
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} ", new object[1] { (rangeType == RangeType.Left) ? "LEFT" : "RIGHT" });
		}
		stringBuilder.Append("FOR VALUES (");
		if (RangeValues != null)
		{
			for (int i = 0; i < RangeValues.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(Globals.commaspace);
				}
				stringBuilder.Append(FormatSqlVariant(RangeValues[i]));
			}
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
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_PARTITION_FUNCTION, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP PARTITION FUNCTION {0}", new object[1] { FormatFullNameForScripting(sp) });
		queries.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		return new PropagateInfo[1]
		{
			new PropagateInfo(ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
		};
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (m_PartitionFunctionParameters != null)
		{
			m_PartitionFunctionParameters.MarkAllDropped();
		}
		if (m_ExtendedProperties != null)
		{
			m_ExtendedProperties.MarkAllDropped();
		}
	}

	public void MergeRangePartition(object boundaryValue)
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER PARTITION FUNCTION {0}() MERGE RANGE({1})", new object[2]
			{
				FullQualifiedName,
				FormatSqlVariant(boundaryValue)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.MergeRangePartition, this, ex);
		}
	}

	public void SplitRangePartition(object boundaryValue)
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER PARTITION FUNCTION {0}() SPLIT RANGE({1})", new object[2]
			{
				FullQualifiedName,
				FormatSqlVariant(boundaryValue)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SplitRangePartition, this, ex);
		}
	}

	public override void Refresh()
	{
		base.Refresh();
		rangeValues = null;
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		return new string[1] { "RangeType" };
	}
}
