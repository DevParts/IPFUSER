using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

[EvaluationMode(/*Could not decode attribute arguments.*/)]
[PhysicalFacet]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class Statistic : ScriptNameObjectBase, ISfcSupportsDesignMode, ICreatable, IDroppable, IDropIfExists, IMarkForDrop, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 6, 6, 7, 10, 10, 11, 11, 11, 11, 11 };

		private static int[] cloudVersionCount = new int[3] { 9, 9, 9 };

		private static int sqlDwPropertyCount = 9;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[9]
		{
			new StaticMetadata("FileGroup", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FilterDefinition", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("HasFilter", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsAutoCreated", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsFromIndexCreation", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("LastUpdated", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("NoAutomaticRecomputation", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Stream", expensive: true, readOnly: true, typeof(byte[]))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[9]
		{
			new StaticMetadata("FileGroup", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FilterDefinition", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("HasFilter", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsAutoCreated", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsFromIndexCreation", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("LastUpdated", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("NoAutomaticRecomputation", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Stream", expensive: true, readOnly: true, typeof(byte[]))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[11]
		{
			new StaticMetadata("FileGroup", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsAutoCreated", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsFromIndexCreation", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("LastUpdated", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("NoAutomaticRecomputation", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Stream", expensive: true, readOnly: true, typeof(byte[])),
			new StaticMetadata("FilterDefinition", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("HasFilter", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
			new StaticMetadata("IsTemporary", expensive: false, readOnly: true, typeof(bool))
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
						"FileGroup" => 0, 
						"FilterDefinition" => 1, 
						"HasFilter" => 2, 
						"ID" => 3, 
						"IsAutoCreated" => 4, 
						"IsFromIndexCreation" => 5, 
						"LastUpdated" => 6, 
						"NoAutomaticRecomputation" => 7, 
						"Stream" => 8, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"FileGroup" => 0, 
					"FilterDefinition" => 1, 
					"HasFilter" => 2, 
					"ID" => 3, 
					"IsAutoCreated" => 4, 
					"IsFromIndexCreation" => 5, 
					"LastUpdated" => 6, 
					"NoAutomaticRecomputation" => 7, 
					"Stream" => 8, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"FileGroup" => 0, 
				"ID" => 1, 
				"IsAutoCreated" => 2, 
				"IsFromIndexCreation" => 3, 
				"LastUpdated" => 4, 
				"NoAutomaticRecomputation" => 5, 
				"Stream" => 6, 
				"FilterDefinition" => 7, 
				"HasFilter" => 8, 
				"PolicyHealthState" => 9, 
				"IsTemporary" => 10, 
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

	private StatisticEvents events;

	private StatisticsScanType m_ScanType = StatisticsScanType.Default;

	private int m_sampleValue;

	private bool m_bIsOnComputed;

	private StatisticColumnCollection m_StatisticColumn;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	[SfcParent("Table")]
	[SfcParent("View")]
	public SqlSmoObject Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string FileGroup
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FileGroup");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FileGroup", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string FilterDefinition
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FilterDefinition");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FilterDefinition", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasFilter => (bool)base.Properties.GetValueWithNullReplacement("HasFilter");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsAutoCreated => (bool)base.Properties.GetValueWithNullReplacement("IsAutoCreated");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsFromIndexCreation => (bool)base.Properties.GetValueWithNullReplacement("IsFromIndexCreation");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsTemporary => (bool)base.Properties.GetValueWithNullReplacement("IsTemporary");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime LastUpdated => (DateTime)base.Properties.GetValueWithNullReplacement("LastUpdated");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool NoAutomaticRecomputation
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("NoAutomaticRecomputation");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NoAutomaticRecomputation", value);
		}
	}

	public StatisticEvents Events
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
				events = new StatisticEvents(this);
			}
			return events;
		}
	}

	public static string UrnSuffix => "Statistic";

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
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

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.OneToAny, typeof(StatisticColumn))]
	public StatisticColumnCollection StatisticColumns
	{
		get
		{
			CheckObjectState();
			if (m_StatisticColumn == null)
			{
				m_StatisticColumn = new StatisticColumnCollection(this);
			}
			return m_StatisticColumn;
		}
	}

	public Statistic()
	{
	}

	public Statistic(SqlSmoObject parent, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = parent;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal Statistic(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		m_StatisticColumn = null;
	}

	public void SetScanOptions(StatisticsScanType type, int no)
	{
		m_ScanType = type;
		m_sampleValue = no;
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (m_StatisticColumn != null)
		{
			m_StatisticColumn.MarkAllDropped();
		}
	}

	public void Create()
	{
		CreateImpl();
		((TableViewBase)base.ParentColl.ParentInstance).Indexes.MarkOutOfSync();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		CheckObjectState();
		if (!sp.ForDirectExecution && base.IgnoreForScripting)
		{
			return;
		}
		bool flag = sp.Data.OptimizerData && base.ServerVersion.Major >= 9 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90;
		if ((!GetPropValueOptional("IsFromIndexCreation", defaultValue: false) || flag) && (!GetPropValueOptional("IsAutoCreated", defaultValue: false) || flag))
		{
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			GetDDL(stringBuilder, sp, creating: true);
			if (stringBuilder.Length > 0)
			{
				AddSetOptionsForStats(queries);
			}
			queries.Add(stringBuilder.ToString());
		}
	}

	private void GetDDL(StringBuilder sb, ScriptingPreferences sp, bool creating)
	{
		ScriptIncludeHeaders(sb, sp, UrnSuffix);
		bool propValueOptional = GetPropValueOptional("IsFromIndexCreation", defaultValue: false);
		GetPropValueOptional("IsAutoCreated", defaultValue: false);
		string text = ((sp.Data.OptimizerData && propValueOptional) ? string.Empty : "not");
		m_bIsOnComputed = false;
		if (sp.IncludeScripts.ExistenceCheck)
		{
			TableViewBase tableViewBase = (TableViewBase)Parent;
			if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_STATISTIC90, new object[3]
				{
					text,
					FormatFullNameForScripting(sp, nameIsIndentifier: false),
					SqlSmoObject.SqlString(tableViewBase.FormatFullNameForScripting(sp))
				});
				sb.Append(sp.NewLine);
			}
			else
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_STATISTIC80, new object[3]
				{
					text,
					SqlSmoObject.SqlString(Name),
					SqlSmoObject.SqlString(tableViewBase.Name)
				});
				sb.Append(sp.NewLine);
			}
		}
		if (sp.Data.OptimizerData && propValueOptional)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, "UPDATE STATISTICS {0}({1})", new object[2]
			{
				GetTableName(sp),
				FormatFullNameForScripting(sp)
			});
		}
		else
		{
			string scriptName = string.Empty;
			bool flag = false;
			if (IsSupportedProperty("IsTemporary"))
			{
				flag = GetPropValueOptional("IsTemporary", defaultValue: false);
				if (flag)
				{
					scriptName = ScriptName;
					ScriptName = Name + "_scripted";
				}
			}
			try
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "CREATE STATISTICS {0}", new object[1] { FormatFullNameForScripting(sp) });
			}
			finally
			{
				if (flag)
				{
					ScriptName = scriptName;
				}
			}
			sb.Append(" ON ");
			sb.Append(GetTableName(sp));
			sb.Append(Globals.LParen);
			if (!GetColumnList(sb, sp))
			{
				return;
			}
			sb.Append(Globals.RParen);
		}
		if (base.ServerVersion.Major >= 10 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version100 && base.Properties.Get("FilterDefinition").Value is string text2 && 0 < text2.Length)
		{
			sb.Append(sp.NewLine);
			sb.AppendFormat(SmoApplication.DefaultCulture, "WHERE {0}", new object[1] { ParseFilterDefinition(text2) });
			sb.Append(sp.NewLine);
		}
		if (sp.Data.OptimizerData && base.ServerVersion.Major >= 9 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			Property property = base.Properties.Get("Stream");
			object obj = null;
			object obj2 = null;
			object obj3 = null;
			if (property.Value != null)
			{
				obj = GetPropValueOptional("Stream");
			}
			else
			{
				string query = string.Format(SmoApplication.DefaultCulture, "DBCC SHOW_STATISTICS(N'{0}.{1}', N'{2}') WITH STATS_STREAM", new object[3]
				{
					SqlSmoObject.SqlString(Parent.ParentColl.ParentInstance.FullQualifiedName),
					SqlSmoObject.SqlString(Parent.FullQualifiedName),
					SqlSmoObject.SqlString(Name)
				});
				DataSet dataSet = null;
				SqlExecutionModes sqlExecutionModes = ExecutionManager.ConnectionContext.SqlExecutionModes;
				ExecutionManager.ConnectionContext.SqlExecutionModes = SqlExecutionModes.ExecuteSql;
				try
				{
					dataSet = ExecutionManager.ExecuteWithResults(query);
				}
				finally
				{
					ExecutionManager.ConnectionContext.SqlExecutionModes = sqlExecutionModes;
				}
				if (dataSet.Tables.Count > 0)
				{
					DataTable dataTable = dataSet.Tables[0];
					if (dataTable.Rows.Count > 0)
					{
						obj = dataTable.Rows[0]["Stats_Stream"];
						obj2 = dataTable.Rows[0]["Rows"];
						obj3 = dataTable.Rows[0]["Data Pages"];
					}
				}
			}
			if (obj == null)
			{
				return;
			}
			sb.Append(" WITH STATS_STREAM = 0x");
			byte[] array = (byte[])obj;
			foreach (byte b in array)
			{
				sb.Append(b.ToString("X2", SmoApplication.DefaultCulture));
			}
			bool flag2 = false;
			if (Parent.IsSupportedProperty("IsMemoryOptimized") && Parent.GetPropValueOptional("IsMemoryOptimized", defaultValue: false))
			{
				flag2 = true;
			}
			if (flag2)
			{
				sb.Append(", NORECOMPUTE");
				return;
			}
			if (obj2 != null && !(obj2 is DBNull))
			{
				sb.Append(", ROWCOUNT = ");
				sb.Append(Convert.ToInt64(obj2, SmoApplication.DefaultCulture).ToString(SmoApplication.DefaultCulture));
			}
			if (obj3 != null && !(obj3 is DBNull))
			{
				sb.Append(", PAGECOUNT = ");
				sb.Append(Convert.ToInt64(obj3, SmoApplication.DefaultCulture).ToString(SmoApplication.DefaultCulture));
			}
		}
		else
		{
			GetDDLBody(sb, sp, creating: true);
		}
	}

	private static string replaceOR(Match m)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "([{0}] IN (", new object[1] { m.Groups["column"].Captures[0].Value });
		for (int i = 0; i < m.Groups["value"].Captures.Count; i++)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0},", new object[1] { m.Groups["value"].Captures[i].Value });
		}
		stringBuilder.Replace(',', ')', stringBuilder.Length - 1, 1);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ")");
		return stringBuilder.ToString();
	}

	private static string ParseFilterDefinition(string p)
	{
		Regex regex = new Regex("\\((\\[(?<column>(([^\\[])|((?<=')\\[))*?)\\](=)(\\()?(?<value>([^\\[])*?)(\\))?)(\\sOR\\s(\\[(?<column>(([^\\[])|((?<=')\\[))*?)\\](=)(\\()?(?<value>([^\\[])*?)(\\))?))+\\)");
		return regex.Replace(p, replaceOR);
	}

	private bool GetColumnList(StringBuilder sb, ScriptingPreferences sp)
	{
		bool flag = true;
		if (StatisticColumns.Count <= 0)
		{
			throw new SmoException(ExceptionTemplatesImpl.ColumnsMustBeSpecified);
		}
		foreach (StatisticColumn statisticColumn in StatisticColumns)
		{
			Column column = null;
			column = ((TableViewBase)base.ParentColl.ParentInstance).Columns[statisticColumn.Name];
			if (column == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.ObjectRefsNonexCol(UrnSuffix, Name, "[" + SqlSmoObject.SqlStringBraket(base.ParentColl.ParentInstance.InternalName) + "].[" + SqlSmoObject.SqlStringBraket(statisticColumn.Name) + "]"));
			}
			if (column.IgnoreForScripting)
			{
				base.IgnoreForScripting = true;
				return false;
			}
			if (!m_bIsOnComputed)
			{
				object propValueOptional = column.GetPropValueOptional("Computed");
				if (propValueOptional != null && (bool)propValueOptional)
				{
					m_bIsOnComputed = true;
				}
			}
			if (flag)
			{
				flag = false;
			}
			else
			{
				sb.Append(Globals.comma);
				sb.Append(" ");
			}
			if (column != null && column.ScriptName.Length > 0)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "[{0}]", new object[1] { SqlSmoObject.SqlBraket(column.ScriptName) });
			}
			else
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "[{0}]", new object[1] { SqlSmoObject.SqlBraket(statisticColumn.Name) });
			}
		}
		return true;
	}

	internal bool AddSetOptionsForStats(StringCollection queries)
	{
		if (m_bIsOnComputed || Parent is View)
		{
			queries.Add("SET ARITHABORT ON");
			queries.Add("SET CONCAT_NULL_YIELDS_NULL ON");
			queries.Add("SET QUOTED_IDENTIFIER ON");
			queries.Add("SET ANSI_NULLS ON");
			queries.Add("SET ANSI_PADDING ON");
			queries.Add("SET ANSI_WARNINGS ON");
			queries.Add("SET NUMERIC_ROUNDABORT OFF");
			return true;
		}
		return false;
	}

	public void Drop()
	{
		DropImpl();
		((TableViewBase)base.ParentColl.ParentInstance).Indexes.RemoveObject(new SimpleObjectKey(Name));
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		CheckObjectState();
		if (GetPropValueOptional("IsFromIndexCreation", defaultValue: false))
		{
			return;
		}
		if (GetPropValueOptional("IsAutoCreated", defaultValue: false))
		{
			bool flag = false;
			if (IsSupportedProperty("IsTemporary"))
			{
				flag = GetPropValueOptional("IsTemporary", defaultValue: false);
			}
			if (!flag)
			{
				return;
			}
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			TableViewBase tableViewBase = (TableViewBase)Parent;
			if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_STATISTIC90, new object[3]
				{
					"",
					FormatFullNameForScripting(sp, nameIsIndentifier: false),
					SqlSmoObject.SqlString(tableViewBase.FormatFullNameForScripting(sp))
				});
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_STATISTIC80, new object[3]
				{
					"",
					SqlSmoObject.SqlString(Name),
					SqlSmoObject.SqlString(tableViewBase.Name)
				});
			}
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP STATISTICS {0}.{1}", new object[2]
		{
			GetTableName(),
			FormatFullNameForScripting(sp)
		});
		queries.Add(stringBuilder.ToString());
	}

	public void MarkForDrop(bool dropOnAlter)
	{
		MarkForDropImpl(dropOnAlter);
	}

	public void Update()
	{
		ScriptingPreferences sp = new ScriptingPreferences();
		ExecutionManager.ExecuteNonQuery(UpdateStatistics(GetDatabaseName(), GetTableName(), FormatFullNameForScripting(sp), m_ScanType, StatisticsTarget.All, GetNoRecompute(), m_sampleValue));
	}

	public void Update(StatisticsScanType scanType)
	{
		ScriptingPreferences sp = new ScriptingPreferences();
		ExecutionManager.ExecuteNonQuery(UpdateStatistics(GetDatabaseName(), GetTableName(), FormatFullNameForScripting(sp), scanType, StatisticsTarget.All, GetNoRecompute(), m_sampleValue));
	}

	public void Update(StatisticsScanType scanType, int sampleValue)
	{
		ScriptingPreferences sp = new ScriptingPreferences();
		ExecutionManager.ExecuteNonQuery(UpdateStatistics(GetDatabaseName(), GetTableName(), FormatFullNameForScripting(sp), scanType, StatisticsTarget.All, GetNoRecompute(), sampleValue));
	}

	public void Update(StatisticsScanType scanType, int sampleValue, bool recompute)
	{
		ScriptingPreferences sp = new ScriptingPreferences();
		ExecutionManager.ExecuteNonQuery(UpdateStatistics(GetDatabaseName(), GetTableName(), FormatFullNameForScripting(sp), scanType, StatisticsTarget.All, !recompute, sampleValue));
	}

	private string GetTableName()
	{
		return GetTableName(null);
	}

	private string GetTableName(ScriptingPreferences sp)
	{
		if (sp == null)
		{
			sp = new ScriptingPreferences();
		}
		TableViewBase tableViewBase = (TableViewBase)base.ParentColl.ParentInstance;
		return tableViewBase.FormatFullNameForScripting(sp);
	}

	private string GetDatabaseName()
	{
		return string.Format(SmoApplication.DefaultCulture, "[{0}]", new object[1] { SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.ParentColl.ParentInstance.InternalName) });
	}

	private bool GetNoRecompute()
	{
		bool result = false;
		if (base.Properties.Get("NoAutomaticRecomputation").Value != null && (bool)base.Properties.Get("NoAutomaticRecomputation").Value)
		{
			result = true;
		}
		return result;
	}

	private void GetDDLBody(StringBuilder sb, ScriptingPreferences sp, bool creating)
	{
		if (creating && StatisticsScanType.Resample == m_ScanType)
		{
			throw new SmoException(ExceptionTemplatesImpl.InvalidScanType(StatisticsScanType.Resample.ToString()));
		}
		UpdateStatisticsBody(sb, sp, m_ScanType, StatisticsTarget.All, GetNoRecompute(), m_sampleValue);
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	internal static StringCollection UpdateStatistics(string dbName, string tableName, string statisticName, StatisticsScanType scanType, StatisticsTarget affectType, bool bIsNorecompute, int sampleValue)
	{
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "use {0}", new object[1] { dbName }));
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptingPreferences sp = new ScriptingPreferences();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "UPDATE STATISTICS {0} {1}", new object[2] { tableName, statisticName });
		UpdateStatisticsBody(stringBuilder, sp, scanType, affectType, bIsNorecompute, sampleValue);
		stringCollection.Add(stringBuilder.ToString());
		return stringCollection;
	}

	private static void UpdateStatisticsBody(StringBuilder sb, ScriptingPreferences sp, StatisticsScanType scanType, StatisticsTarget affectType, bool bIsNorecompute, int sampleValue)
	{
		StringCollection stringCollection = new StringCollection();
		switch (scanType)
		{
		case StatisticsScanType.FullScan:
			stringCollection.Add("FULLSCAN");
			break;
		case StatisticsScanType.Percent:
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "SAMPLE {0} PERCENT", new object[1] { sampleValue }));
			break;
		case StatisticsScanType.Rows:
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "SAMPLE {0} ROWS", new object[1] { sampleValue }));
			break;
		case StatisticsScanType.Resample:
			stringCollection.Add("RESAMPLE");
			break;
		}
		switch (affectType)
		{
		case StatisticsTarget.Column:
			stringCollection.Add("COLUMNS");
			break;
		case StatisticsTarget.Index:
			stringCollection.Add("INDEX");
			break;
		}
		if (bIsNorecompute)
		{
			stringCollection.Add("NORECOMPUTE");
		}
		for (int i = 0; i < stringCollection.Count; i++)
		{
			if (i == 0)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "{0}WITH{1}{2}", new object[3]
				{
					sp.NewLine,
					Globals.space,
					stringCollection[i]
				});
			}
			else
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "{0}{1}", new object[2]
				{
					Globals.comma,
					stringCollection[i]
				});
			}
		}
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[5] { "NoAutomaticRecomputation", "FilterDefinition", "IsAutoCreated", "IsFromIndexCreation", "IsTemporary" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}

	public DataSet EnumStatistics()
	{
		DataSet dataSet = null;
		CheckObjectState(throwIfNotCreated: true);
		StringCollection stringCollection = new StringCollection();
		ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
		scriptingPreferences.SetTargetDatabaseEngineType(base.ServerInfo.DatabaseEngineType);
		scriptingPreferences.ScriptForCreateDrop = true;
		AddDatabaseContext(stringCollection, scriptingPreferences);
		string s = ((ScriptSchemaObjectBase)base.ParentColl.ParentInstance).FormatFullNameForScripting(scriptingPreferences);
		stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DBCC SHOW_STATISTICS ({0}, {1})", new object[2]
		{
			SqlSmoObject.MakeSqlString(s),
			SqlSmoObject.MakeSqlString(Name)
		}));
		return ExecutionManager.ExecuteWithResults(stringCollection);
	}
}
