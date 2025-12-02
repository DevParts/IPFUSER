using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[PhysicalFacet]
public sealed class FullTextIndex : SqlSmoObject, ISfcSupportsDesignMode, ICreatable, IAlterable, IDroppable, IDropIfExists, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 5, 5, 9, 13, 13, 14, 14, 14, 14, 14 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 13 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[13]
		{
			new StaticMetadata("CatalogName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ChangeTracking", expensive: false, readOnly: false, typeof(ChangeTracking)),
			new StaticMetadata("DocumentsProcessed", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("FilegroupName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("IsEnabled", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ItemCount", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("NumberOfFailures", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("PendingChanges", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("PopulationStatus", expensive: false, readOnly: true, typeof(IndexPopulationStatus)),
			new StaticMetadata("SearchPropertyListName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("StopListName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("StopListOption", expensive: false, readOnly: false, typeof(StopListOption)),
			new StaticMetadata("UniqueIndexName", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[14]
		{
			new StaticMetadata("CatalogName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ChangeTracking", expensive: false, readOnly: false, typeof(ChangeTracking)),
			new StaticMetadata("IsEnabled", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("PopulationStatus", expensive: false, readOnly: true, typeof(IndexPopulationStatus)),
			new StaticMetadata("UniqueIndexName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DocumentsProcessed", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ItemCount", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("NumberOfFailures", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("PendingChanges", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("FilegroupName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
			new StaticMetadata("StopListName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("StopListOption", expensive: false, readOnly: false, typeof(StopListOption)),
			new StaticMetadata("SearchPropertyListName", expensive: false, readOnly: false, typeof(string))
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
					return -1;
				}
				return propertyName switch
				{
					"CatalogName" => 0, 
					"ChangeTracking" => 1, 
					"DocumentsProcessed" => 2, 
					"FilegroupName" => 3, 
					"IsEnabled" => 4, 
					"ItemCount" => 5, 
					"NumberOfFailures" => 6, 
					"PendingChanges" => 7, 
					"PopulationStatus" => 8, 
					"SearchPropertyListName" => 9, 
					"StopListName" => 10, 
					"StopListOption" => 11, 
					"UniqueIndexName" => 12, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"CatalogName" => 0, 
				"ChangeTracking" => 1, 
				"IsEnabled" => 2, 
				"PopulationStatus" => 3, 
				"UniqueIndexName" => 4, 
				"DocumentsProcessed" => 5, 
				"ItemCount" => 6, 
				"NumberOfFailures" => 7, 
				"PendingChanges" => 8, 
				"FilegroupName" => 9, 
				"PolicyHealthState" => 10, 
				"StopListName" => 11, 
				"StopListOption" => 12, 
				"SearchPropertyListName" => 13, 
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

	private FullTextIndexColumnCollection fullTextIndexColumns;

	internal bool noPopulation;

	internal object oldChangeTrackingValue;

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string CatalogName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("CatalogName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("CatalogName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public ChangeTracking ChangeTracking
	{
		get
		{
			return (ChangeTracking)base.Properties.GetValueWithNullReplacement("ChangeTracking");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ChangeTracking", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int DocumentsProcessed => (int)base.Properties.GetValueWithNullReplacement("DocumentsProcessed");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string FilegroupName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FilegroupName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FilegroupName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsEnabled => (bool)base.Properties.GetValueWithNullReplacement("IsEnabled");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ItemCount => (int)base.Properties.GetValueWithNullReplacement("ItemCount");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int NumberOfFailures => (int)base.Properties.GetValueWithNullReplacement("NumberOfFailures");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int PendingChanges => (int)base.Properties.GetValueWithNullReplacement("PendingChanges");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public IndexPopulationStatus PopulationStatus => (IndexPopulationStatus)base.Properties.GetValueWithNullReplacement("PopulationStatus");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string SearchPropertyListName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("SearchPropertyListName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SearchPropertyListName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string StopListName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("StopListName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("StopListName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public StopListOption StopListOption
	{
		get
		{
			return (StopListOption)base.Properties.GetValueWithNullReplacement("StopListOption");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("StopListOption", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string UniqueIndexName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("UniqueIndexName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("UniqueIndexName", value);
		}
	}

	[SfcParent("View")]
	[SfcParent("Table")]
	[SfcObject(SfcObjectRelationship.ParentObject)]
	public TableViewBase Parent
	{
		get
		{
			CheckObjectState();
			return singletonParent as TableViewBase;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	public static string UrnSuffix => "FullTextIndex";

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.OneToAny, typeof(FullTextIndexColumn))]
	public FullTextIndexColumnCollection IndexedColumns
	{
		get
		{
			CheckObjectState();
			if (fullTextIndexColumns == null)
			{
				fullTextIndexColumns = new FullTextIndexColumnCollection(this);
			}
			return fullTextIndexColumns;
		}
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[3] { "CatalogName", "FilegroupName", "UniqueIndexName" };
	}

	public FullTextIndex()
	{
	}

	public FullTextIndex(TableViewBase parent)
		: base(new SimpleObjectKey(parent.Name), SqlSmoState.Creating)
	{
		singletonParent = parent;
		SetServerObject(parent.GetServerObject());
		m_comparer = parent.StringComparer;
	}

	internal FullTextIndex(TableViewBase parent, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parent;
		SetServerObject(parent.GetServerObject());
		m_comparer = parent.StringComparer;
	}

	internal override void ValidateParent(SqlSmoObject newParent)
	{
		singletonParent = (TableViewBase)newParent;
		m_comparer = newParent.StringComparer;
		SetServerObject(newParent.GetServerObject());
		if (newParent is View)
		{
			ThrowIfBelowVersion90();
		}
	}

	internal override void UpdateObjectState()
	{
		if (base.State == SqlSmoState.Pending && singletonParent != null)
		{
			SetState(SqlSmoState.Creating);
		}
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (fullTextIndexColumns != null)
		{
			fullTextIndexColumns.MarkAllDropped();
		}
	}

	public void Create()
	{
		CreateImpl();
	}

	public void Create(bool noPopulation)
	{
		try
		{
			this.noPopulation = noPopulation;
			CreateImpl();
		}
		finally
		{
			noPopulation = false;
		}
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		ScriptCreateFullTextIndex(queries, sp);
		Property property = properties.Get("IsEnabled");
		if (property.Value != null && !(bool)property.Value)
		{
			ScriptDisable(queries, sp);
		}
	}

	protected override void PostCreate()
	{
		Parent.m_bFullTextIndexInitialized = true;
		Parent.m_FullTextIndex = this;
	}

	public void Alter()
	{
		AlterImpl();
	}

	public void Alter(bool noPopulation)
	{
		try
		{
			this.noPopulation = noPopulation;
			AlterImpl();
		}
		finally
		{
			noPopulation = false;
		}
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		if (IsObjectDirty())
		{
			ScriptAlterFullTextIndex(alterQuery, sp);
		}
	}

	private void ScriptCreateFullTextIndex(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, string.Empty, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		TableViewBase parent = Parent;
		string text = parent.FormatFullNameForScripting(sp);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			if (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_FT_INDEX, new object[2]
				{
					SqlSmoObject.SqlString(text),
					"="
				});
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_FT_INDEX90, new object[2]
				{
					"not",
					SqlSmoObject.SqlString(text)
				});
			}
			stringBuilder.Append(sp.NewLine);
		}
		Property property;
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE FULLTEXT INDEX ON {0}", new object[1] { text });
			if (IndexedColumns.Count > 0)
			{
				stringBuilder.Append("(");
			}
			int num = 0;
			foreach (FullTextIndexColumn indexedColumn in IndexedColumns)
			{
				if (indexedColumn.IgnoreForScripting)
				{
					continue;
				}
				if (num++ != 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(sp.NewLine);
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { indexedColumn.FormatFullNameForScripting(sp) });
				object propValueOptional = indexedColumn.GetPropValueOptional("TypeColumnName");
				if (propValueOptional != null)
				{
					string text2 = (string)propValueOptional;
					if (text2.Length > 0)
					{
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " TYPE COLUMN [{0}]", new object[1] { SqlSmoObject.SqlBraket(text2) });
					}
				}
				if (base.ServerVersion.Major >= 8 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version80)
				{
					object propValueOptional2 = indexedColumn.GetPropValueOptional("Language");
					if (propValueOptional2 != null)
					{
						string text3 = (string)propValueOptional2;
						if (text3.Length > 0)
						{
							stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " LANGUAGE '{0}'", new object[1] { SqlSmoObject.SqlBraket(text3) });
						}
					}
				}
				if (base.ServerVersion.Major < 11 || sp.TargetServerVersionInternal < SqlServerVersionInternal.Version110)
				{
					continue;
				}
				object propValueOptional3 = indexedColumn.GetPropValueOptional("StatisticalSemantics");
				if (propValueOptional3 != null)
				{
					int num2 = (int)propValueOptional3;
					if (num2 > 0)
					{
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " STATISTICAL_SEMANTICS");
					}
				}
			}
			if (IndexedColumns.Count > 0)
			{
				if (num == 0)
				{
					stringBuilder.Length--;
				}
				else
				{
					stringBuilder.Append(")");
				}
			}
			stringBuilder.Append(sp.NewLine);
			if (!(GetPropValue("UniqueIndexName") is string { Length: >0 } text4))
			{
				throw new PropertyNotSetException("UniqueIndexName");
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "KEY INDEX [{0}]", new object[1] { SqlSmoObject.SqlBraket(text4) });
			if (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version90)
			{
				if ((property = base.Properties.Get("CatalogName")).Value != null && property.Value.ToString().Length > 0)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " ON [{0}]", new object[1] { SqlSmoObject.SqlBraket(property.Value.ToString()) });
				}
				stringBuilder.Append(sp.NewLine);
				if ((property = base.Properties.Get("ChangeTracking")).Value != null)
				{
					stringBuilder.Append("WITH CHANGE_TRACKING ");
					switch ((ChangeTracking)property.Value)
					{
					case ChangeTracking.Automatic:
						stringBuilder.Append("AUTO");
						break;
					case ChangeTracking.Manual:
						stringBuilder.Append("MANUAL");
						break;
					case ChangeTracking.Off:
						stringBuilder.Append("OFF");
						if (noPopulation)
						{
							stringBuilder.Append(", NO POPULATION");
						}
						break;
					default:
						throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration("Change Tracking"));
					}
					stringBuilder.Append(sp.NewLine);
				}
			}
			else
			{
				StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
				if (base.Properties.Get("CatalogName").Value is string { Length: >0 } text5)
				{
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { SqlSmoObject.MakeSqlBraket(text5) });
				}
				if (base.ServerVersion.Major >= 10 && base.Properties.Get("FilegroupName").Value is string { Length: >0 } text6)
				{
					if (stringBuilder2.Length > 0)
					{
						stringBuilder2.Append(Globals.commaspace);
					}
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "FILEGROUP {0}", new object[1] { SqlSmoObject.MakeSqlBraket(text6) });
				}
				if (stringBuilder2.Length > 0)
				{
					stringBuilder.Append("ON (");
					stringBuilder.Append(stringBuilder2);
					stringBuilder.Append(")");
					stringBuilder.Append(sp.NewLine);
				}
				StringBuilder stringBuilder3 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
				Property property2 = base.Properties.Get("ChangeTracking");
				if (property2.Value != null)
				{
					stringBuilder3.Append("CHANGE_TRACKING = ");
					switch ((ChangeTracking)property2.Value)
					{
					case ChangeTracking.Automatic:
						stringBuilder3.Append("AUTO");
						break;
					case ChangeTracking.Manual:
						stringBuilder3.Append("MANUAL");
						break;
					case ChangeTracking.Off:
						stringBuilder3.Append("OFF");
						if (noPopulation)
						{
							stringBuilder3.Append(", NO POPULATION");
						}
						break;
					default:
						throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration("Change Tracking"));
					}
				}
				if (base.ServerVersion.Major >= 10)
				{
					Property property3 = base.Properties.Get("StopListOption");
					string text7 = base.Properties.Get("StopListName").Value as string;
					if (property3.Value != null)
					{
						if (stringBuilder3.Length > 0)
						{
							stringBuilder3.Append(Globals.commaspace);
						}
						stringBuilder3.Append("STOPLIST = ");
						switch ((StopListOption)property3.Value)
						{
						case StopListOption.Off:
							stringBuilder3.Append("OFF");
							if (text7 != null && text7.Length > 0)
							{
								throw new SmoException(ExceptionTemplatesImpl.PropertyNotValidException("StopListName", "StopListOption", "OFF"));
							}
							break;
						case StopListOption.System:
							stringBuilder3.Append("SYSTEM");
							if (text7 != null && text7.Length > 0)
							{
								throw new SmoException(ExceptionTemplatesImpl.PropertyNotValidException("StopListName", "StopListOption", "SYSTEM"));
							}
							break;
						case StopListOption.Name:
							if (text7 != null && text7.Length > 0)
							{
								stringBuilder3.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { SqlSmoObject.MakeSqlBraket(text7) });
								break;
							}
							throw new PropertyNotSetException("StopListName");
						default:
							throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration("StopList Name"));
						}
					}
					else if (text7 != null && text7.Length > 0)
					{
						if (stringBuilder3.Length > 0)
						{
							stringBuilder3.Append(Globals.commaspace);
						}
						stringBuilder3.Append("STOPLIST = ");
						stringBuilder3.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { SqlSmoObject.MakeSqlBraket(text7) });
					}
				}
				if (VersionUtils.IsSql11OrLater(sp.TargetServerVersionInternal, base.ServerVersion) && base.Properties.Get("SearchPropertyListName").Value is string { Length: >0 } text8)
				{
					if (stringBuilder3.Length > 0)
					{
						stringBuilder3.Append(Globals.commaspace);
					}
					stringBuilder3.Append("SEARCH PROPERTY LIST = ");
					stringBuilder3.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { SqlSmoObject.MakeSqlBraket(text8) });
				}
				if (stringBuilder3.Length > 0)
				{
					stringBuilder.Append("WITH (");
					stringBuilder.Append(stringBuilder3);
					stringBuilder.Append(")");
					stringBuilder.Append(sp.NewLine);
				}
			}
			stringBuilder.Append(sp.NewLine);
			queries.Add(stringBuilder.ToString());
			return;
		}
		_ = (Database)parent.ParentColl.ParentInstance;
		if (!(GetPropValue("UniqueIndexName") is string { Length: >0 } text9))
		{
			throw new PropertyNotSetException("UniqueIndexName");
		}
		string text10 = string.Empty;
		if ((property = base.Properties.Get("CatalogName")).Value != null)
		{
			text10 = (string)property.Value;
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_fulltext_table @tabname=N'{0}', @action=N'create', @keyname=N'{1}'", new object[2]
		{
			SqlSmoObject.SqlString(text),
			SqlSmoObject.SqlString(text9)
		});
		if (text10.Length > 0)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", @ftcat=N'{0}'", new object[1] { SqlSmoObject.SqlString(text10) });
		}
		queries.Add(stringBuilder.ToString());
		stringBuilder.Length = 0;
		foreach (FullTextIndexColumn indexedColumn2 in IndexedColumns)
		{
			if (!indexedColumn2.IgnoreForScripting)
			{
				indexedColumn2.ScriptCreateFullTextIndexColumn(queries, sp);
			}
		}
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version80 && (property = base.Properties.Get("ChangeTracking")).Value != null)
		{
			switch ((ChangeTracking)property.Value)
			{
			case ChangeTracking.Automatic:
				queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_fulltext_table @tabname=N'{0}', @action=N'start_change_tracking'", new object[1] { SqlSmoObject.SqlString(text) }));
				queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_fulltext_table @tabname=N'{0}', @action=N'start_background_updateindex'", new object[1] { SqlSmoObject.SqlString(text) }));
				break;
			case ChangeTracking.Manual:
				queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_fulltext_table @tabname=N'{0}', @action=N'start_change_tracking'", new object[1] { SqlSmoObject.SqlString(text) }));
				break;
			default:
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration("Change Tracking"));
			case ChangeTracking.Off:
				break;
			}
		}
	}

	private void ScriptAlterFullTextIndex(StringCollection queries, ScriptingPreferences sp)
	{
		TableViewBase parent = Parent;
		Property property;
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			if ((property = base.Properties.Get("ChangeTracking")).Value != null && property.Dirty)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER FULLTEXT INDEX ON {0} SET CHANGE_TRACKING ", new object[1] { parent.FullQualifiedName });
				if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version100)
				{
					stringBuilder.Append("= ");
				}
				switch ((ChangeTracking)property.Value)
				{
				case ChangeTracking.Automatic:
					stringBuilder.Append("AUTO");
					break;
				case ChangeTracking.Manual:
					stringBuilder.Append("MANUAL");
					break;
				case ChangeTracking.Off:
					stringBuilder.Append("OFF");
					break;
				default:
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration("Change Tracking"));
				}
				queries.Add(stringBuilder.ToString());
			}
		}
		else if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version80 && (property = base.Properties.Get("ChangeTracking")).Value != null && property.Dirty)
		{
			ChangeTracking changeTracking = (ChangeTracking)property.Value;
			ChangeTracking changeTracking2 = (ChangeTracking)GetRealValue(property, oldChangeTrackingValue);
			if (changeTracking != changeTracking2)
			{
				StringCollection stringCollection = new StringCollection();
				if (changeTracking2 == ChangeTracking.Automatic && changeTracking == ChangeTracking.Manual)
				{
					stringCollection.Add("stop_background_updateindex");
				}
				else if (changeTracking2 == ChangeTracking.Automatic && changeTracking == ChangeTracking.Off)
				{
					stringCollection.Add("stop_background_updateindex");
					stringCollection.Add("stop_change_tracking");
				}
				else if (changeTracking2 == ChangeTracking.Manual && changeTracking == ChangeTracking.Automatic)
				{
					stringCollection.Add("start_background_updateindex");
				}
				else if (changeTracking2 == ChangeTracking.Manual && changeTracking == ChangeTracking.Off)
				{
					stringCollection.Add("stop_change_tracking");
				}
				else if (changeTracking2 == ChangeTracking.Off && changeTracking == ChangeTracking.Automatic)
				{
					stringCollection.Add("start_change_tracking");
					stringCollection.Add("start_background_updateindex");
				}
				else if (changeTracking2 == ChangeTracking.Off && changeTracking == ChangeTracking.Manual)
				{
					stringCollection.Add("start_change_tracking");
				}
				StringEnumerator enumerator = stringCollection.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_fulltext_table @tabname=N'{0}', @action=N'{1}'", new object[2]
						{
							SqlSmoObject.SqlString(parent.FormatFullNameForScripting(sp)),
							current
						}));
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
		}
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version100 && base.ServerVersion.Major >= 10)
		{
			StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			Property property2 = base.Properties.Get("StopListOption");
			Property property3 = base.Properties.Get("StopListName");
			string text = base.Properties.Get("StopListName").Value as string;
			StopListOption stopListOption = (StopListOption)property2.Value;
			if (property2.Value != null && property2.Dirty)
			{
				stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "ALTER FULLTEXT INDEX ON {0} SET STOPLIST = ", new object[1] { parent.FullQualifiedName });
				switch (stopListOption)
				{
				case StopListOption.Off:
					stringBuilder2.Append("OFF");
					if (text != null && property3.Dirty && text.Length > 0)
					{
						throw new SmoException(ExceptionTemplatesImpl.PropertyNotValidException("StopListName", "StopListOption", "OFF"));
					}
					break;
				case StopListOption.System:
					stringBuilder2.Append("SYSTEM");
					if (text != null && property3.Dirty && text.Length > 0)
					{
						throw new SmoException(ExceptionTemplatesImpl.PropertyNotValidException("StopListName", "StopListOption", "SYSTEM"));
					}
					break;
				case StopListOption.Name:
					if (text != null && text.Length > 0)
					{
						stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { SqlSmoObject.MakeSqlBraket(text) });
						break;
					}
					throw new PropertyNotSetException("StopListName");
				default:
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration("StopList NAme"));
				}
				stringBuilder2.Append(sp.NewLine);
				if (noPopulation)
				{
					stringBuilder2.Append("WITH NO POPULATION");
					stringBuilder2.Append(sp.NewLine);
				}
			}
			else if (text != null && property3.Dirty && text.Length > 0)
			{
				switch (stopListOption)
				{
				case StopListOption.Off:
					throw new SmoException(ExceptionTemplatesImpl.PropertyNotValidException("StopListName", "StopListOption", "OFF"));
				case StopListOption.System:
					throw new SmoException(ExceptionTemplatesImpl.PropertyNotValidException("StopListName", "StopListOption", "SYSTEM"));
				case StopListOption.Name:
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "ALTER FULLTEXT INDEX ON {0} SET STOPLIST = {1}", new object[2]
					{
						parent.FullQualifiedName,
						SqlSmoObject.MakeSqlBraket(text)
					});
					stringBuilder2.Append(sp.NewLine);
					if (noPopulation)
					{
						stringBuilder2.Append("WITH NO POPULATION");
						stringBuilder2.Append(sp.NewLine);
					}
					break;
				}
			}
			if (stringBuilder2.Length > 0)
			{
				queries.Add(stringBuilder2.ToString());
			}
		}
		if (!VersionUtils.IsSql11OrLater(sp.TargetServerVersionInternal, base.ServerVersion))
		{
			return;
		}
		StringBuilder stringBuilder3 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		Property property4 = base.Properties.Get("SearchPropertyListName");
		string text2 = property4.Value as string;
		if (property4.Dirty && text2 != null)
		{
			stringBuilder3.AppendFormat(SmoApplication.DefaultCulture, "ALTER FULLTEXT INDEX ON {0} SET {1} = ", new object[2] { parent.FullQualifiedName, "SEARCH PROPERTY LIST" });
			if (text2.Length == 0)
			{
				stringBuilder3.Append("OFF");
			}
			else
			{
				stringBuilder3.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { SqlSmoObject.MakeSqlBraket(text2) });
			}
			stringBuilder3.Append(sp.NewLine);
			if (noPopulation)
			{
				stringBuilder3.Append("WITH NO POPULATION");
				stringBuilder3.Append(sp.NewLine);
			}
			queries.Add(stringBuilder3.ToString());
		}
	}

	public void Drop()
	{
		if (!ExecutionManager.Recording)
		{
			TableViewBase parent = Parent;
			parent.DropFullTextIndexRef();
		}
		DropImpl();
	}

	public void DropIfExists()
	{
		bool flag = base.State == SqlSmoState.Dropped || (base.State == SqlSmoState.Creating && !ExecutionManager.Recording) || (base.State == SqlSmoState.Pending && !base.IsDesignMode);
		if (!ExecutionManager.Recording && !flag)
		{
			TableViewBase parent = Parent;
			parent.DropFullTextIndexRef();
		}
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, string.Empty, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		TableViewBase parent = Parent;
		string text = parent.FormatFullNameForScripting(sp);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			if (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_FT_INDEX, new object[2]
				{
					SqlSmoObject.SqlString(text),
					"<>"
				});
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_FT_INDEX90, new object[2]
				{
					string.Empty,
					SqlSmoObject.SqlString(text)
				});
			}
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP FULLTEXT INDEX ON {0}", new object[1] { text });
		}
		else
		{
			_ = (Database)parent.ParentColl.ParentInstance;
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_fulltext_table @tabname={0}, @action=N'drop'", new object[1] { parent.FormatFullNameForScripting(sp, nameIsIndentifier: false) });
		}
		stringBuilder.Append(sp.NewLine);
		dropQuery.Add(stringBuilder.ToString());
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		bool bWithScript = action != PropagateAction.Create;
		return new PropagateInfo[1]
		{
			new PropagateInfo(IndexedColumns, bWithScript)
		};
	}

	public void Disable()
	{
		try
		{
			StringCollection queries = new StringCollection();
			ScriptDisable(queries);
			ExecutionManager.ExecuteNonQuery(queries);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Disable, this, ex);
		}
	}

	internal void ScriptDisable(StringCollection queries)
	{
		ScriptDisable(queries, null);
	}

	internal void ScriptDisable(StringCollection queries, ScriptingPreferences sp)
	{
		TableViewBase parent = Parent;
		Database database = (Database)parent.ParentColl.ParentInstance;
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp != null && sp.IncludeScripts.DatabaseContext)
		{
			queries.Add(string.Format(SmoApplication.DefaultCulture, "USE [{0}]", new object[1] { SqlSmoObject.SqlBraket(database.Name) }));
		}
		if (sp != null && sp.IncludeScripts.ExistenceCheck)
		{
			string s = parent.FormatFullNameForScripting(sp);
			if (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_FT_INDEX, new object[2]
				{
					SqlSmoObject.SqlString(s),
					"<>"
				});
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_FT_INDEX90, new object[2]
				{
					string.Empty,
					SqlSmoObject.SqlString(s)
				});
			}
			stringBuilder.Append(sp.NewLine);
		}
		if (sp != null && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER FULLTEXT INDEX ON {0} DISABLE", new object[1] { parent.FullQualifiedName });
		}
		else
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_fulltext_table @tabname=N'{0}', @action=N'deactivate'", new object[1] { SqlSmoObject.SqlString(parent.FullQualifiedName) });
		}
		queries.Add(stringBuilder.ToString());
	}

	public void Enable()
	{
		try
		{
			StringCollection stringCollection = new StringCollection();
			TableViewBase parent = Parent;
			Database database = (Database)parent.ParentColl.ParentInstance;
			if (base.ServerVersion.Major >= 9)
			{
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "USE [{0}]", new object[1] { SqlSmoObject.SqlBraket(database.Name) }));
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER FULLTEXT INDEX ON {0} ENABLE", new object[1] { parent.FullQualifiedName }));
			}
			else
			{
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "USE [{0}]", new object[1] { SqlSmoObject.SqlBraket(database.Name) }));
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_fulltext_table @tabname=N'{0}', @action=N'activate'", new object[1] { SqlSmoObject.SqlString(parent.FullQualifiedName) }));
			}
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Enable, this, ex);
		}
	}

	public void StartPopulation(IndexPopulationAction action)
	{
		try
		{
			if (base.ServerVersion.Major >= 8)
			{
				StringCollection stringCollection = new StringCollection();
				TableViewBase parent = Parent;
				Database database = (Database)parent.ParentColl.ParentInstance;
				if (base.ServerVersion.Major >= 9)
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "USE [{0}]", new object[1] { SqlSmoObject.SqlBraket(database.Name) }));
					string empty = string.Empty;
					empty = action switch
					{
						IndexPopulationAction.Full => "FULL", 
						IndexPopulationAction.Incremental => "INCREMENTAL", 
						_ => "UPDATE", 
					};
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER FULLTEXT INDEX ON {0} START {1} POPULATION", new object[2] { parent.FullQualifiedName, empty }));
				}
				else
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "USE [{0}]", new object[1] { SqlSmoObject.SqlBraket(database.Name) }));
					string empty2 = string.Empty;
					empty2 = action switch
					{
						IndexPopulationAction.Full => "start_full", 
						IndexPopulationAction.Incremental => "start_incremental", 
						_ => "update_index", 
					};
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_fulltext_table @tabname=N'{0}', @action=N'{1}'", new object[2]
					{
						SqlSmoObject.SqlString(parent.FullQualifiedName),
						empty2
					}));
				}
				ExecutionManager.ExecuteNonQuery(stringCollection);
				return;
			}
			throw new SmoException(ExceptionTemplatesImpl.UnsupportedVersion(base.ServerVersion.ToString()));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.StartPopulation, this, ex);
		}
	}

	public void StopPopulation()
	{
		try
		{
			if (base.ServerVersion.Major >= 8)
			{
				StringCollection stringCollection = new StringCollection();
				TableViewBase parent = Parent;
				Database database = (Database)parent.ParentColl.ParentInstance;
				if (base.ServerVersion.Major >= 9)
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "USE [{0}]", new object[1] { SqlSmoObject.SqlBraket(database.Name) }));
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER FULLTEXT INDEX ON {0} STOP POPULATION", new object[1] { parent.FullQualifiedName }));
				}
				else
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "USE [{0}]", new object[1] { SqlSmoObject.SqlBraket(database.Name) }));
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_fulltext_table @tabname=N'{0}', @action=N'stop'", new object[1] { SqlSmoObject.SqlString(parent.FullQualifiedName) }));
				}
				ExecutionManager.ExecuteNonQuery(stringCollection);
				return;
			}
			throw new SmoException(ExceptionTemplatesImpl.UnsupportedVersion(base.ServerVersion.ToString()));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.StopPopulation, this, ex);
		}
	}

	public override void Refresh()
	{
		base.Refresh();
		oldChangeTrackingValue = null;
	}

	internal override void ValidateProperty(Property prop, object value)
	{
		if (prop.Name == "ChangeTracking" && !prop.Dirty)
		{
			oldChangeTrackingValue = prop.Value;
		}
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[8] { "IsEnabled", "UniqueIndexName", "CatalogName", "ChangeTracking", "FilegroupName", "StopListName", "StopListOption", "SearchPropertyListName" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}
