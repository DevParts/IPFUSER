using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class FullTextIndexColumn : ScriptNameObjectBase, IAlterable, ICreatable, IDroppable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 1, 2, 2, 2, 2, 3, 3, 3, 3, 3 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 3 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[3]
		{
			new StaticMetadata("Language", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("StatisticalSemantics", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("TypeColumnName", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[3]
		{
			new StaticMetadata("TypeColumnName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Language", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("StatisticalSemantics", expensive: false, readOnly: false, typeof(int))
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
					"Language" => 0, 
					"StatisticalSemantics" => 1, 
					"TypeColumnName" => 2, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"TypeColumnName" => 0, 
				"Language" => 1, 
				"StatisticalSemantics" => 2, 
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

	internal bool noPopulation;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public FullTextIndex Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as FullTextIndex;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Language
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Language");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Language", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int StatisticalSemantics
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("StatisticalSemantics");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("StatisticalSemantics", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string TypeColumnName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("TypeColumnName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("TypeColumnName", value);
		}
	}

	public static string UrnSuffix => "FullTextIndexColumn";

	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
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

	public FullTextIndexColumn()
	{
	}

	public FullTextIndexColumn(FullTextIndex fullTextIndex, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = fullTextIndex;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[3] { "Language", "StatisticalSemantics", "TypeColumnName" };
	}

	internal FullTextIndexColumn(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
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
		ScriptAlterFullTextIndexColumn(alterQuery, sp);
	}

	internal void ScriptAlterFullTextIndexColumn(StringCollection queries, ScriptingPreferences sp)
	{
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version110)
		{
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			FullTextIndex fullTextIndex = (FullTextIndex)base.ParentColl.ParentInstance;
			TableViewBase parent = fullTextIndex.Parent;
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER FULLTEXT INDEX ON {0} ALTER COLUMN ", new object[1] { parent.FormatFullNameForScripting(sp) });
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} ", new object[1] { FormatFullNameForScripting(sp) });
			Property property = base.Properties.Get("StatisticalSemantics");
			int num = 0;
			if (property.Value != null)
			{
				num = (int)property.Value;
			}
			if (num > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " ADD STATISTICAL_SEMANTICS");
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " DROP STATISTICAL_SEMANTICS");
			}
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
		ScriptCreateFullTextIndexColumn(queries, sp);
	}

	internal void ScriptCreateFullTextIndexColumn(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		FullTextIndex fullTextIndex = (FullTextIndex)base.ParentColl.ParentInstance;
		TableViewBase parent = fullTextIndex.Parent;
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER FULLTEXT INDEX ON {0} ADD (", new object[1] { parent.FormatFullNameForScripting(sp) });
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { FormatFullNameForScripting(sp) });
			Property property = base.Properties.Get("TypeColumnName");
			if (property.Value != null)
			{
				string text = (string)property.Value;
				if (text.Length > 0)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " TYPE COLUMN [{0}]", new object[1] { SqlSmoObject.SqlBraket(text) });
				}
			}
			if (base.ServerVersion.Major >= 8)
			{
				Property property2 = base.Properties.Get("Language");
				if (property2.Value != null)
				{
					string text2 = (string)property2.Value;
					if (text2.Length > 0)
					{
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " LANGUAGE [{0}]", new object[1] { SqlSmoObject.SqlBraket(text2) });
					}
				}
			}
			if (base.ServerVersion.Major >= 11)
			{
				Property property3 = base.Properties.Get("StatisticalSemantics");
				if (property3.Value != null)
				{
					int num = (int)property3.Value;
					if (num > 0)
					{
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " STATISTICAL_SEMANTICS");
					}
				}
			}
			stringBuilder.Append(")");
			if (noPopulation || fullTextIndex.noPopulation)
			{
				stringBuilder.Append("WITH NO POPULATION");
			}
			queries.Add(stringBuilder.ToString());
			return;
		}
		string text3 = string.Empty;
		if (base.ServerVersion.Major >= 8 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version80)
		{
			Property property4 = base.Properties.Get("Language");
			if (property4.Value != null)
			{
				text3 = (string)property4.Value;
				if (0 < text3.Length)
				{
					stringBuilder.Append("declare @lcid int ");
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "select @lcid=lcid from master.dbo.syslanguages where alias=N'{0}' ", new object[1] { SqlSmoObject.SqlString(text3) });
				}
			}
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_fulltext_column @tabname=N'{0}', @colname={1}, @action=N'add'", new object[2]
		{
			SqlSmoObject.SqlString(parent.FormatFullNameForScripting(sp)),
			FormatFullNameForScripting(sp, nameIsIndentifier: false)
		});
		Property property5 = base.Properties.Get("TypeColumnName");
		if (property5.Value != null)
		{
			string text4 = (string)property5.Value;
			if (text4.Length > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", @type_colname=N'{0}'", new object[1] { SqlSmoObject.SqlString(text4) });
			}
		}
		if (base.ServerVersion.Major >= 8 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version80 && text3.Length > 0)
		{
			stringBuilder.Append(", @language=@lcid");
		}
		queries.Add(stringBuilder.ToString());
	}

	public void Drop()
	{
		DropImpl();
	}

	public void Drop(bool noPopulation)
	{
		try
		{
			this.noPopulation = noPopulation;
			DropImpl();
		}
		finally
		{
			noPopulation = false;
		}
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		FullTextIndex fullTextIndex = (FullTextIndex)base.ParentColl.ParentInstance;
		TableViewBase parent = fullTextIndex.Parent;
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER FULLTEXT INDEX ON {0} DROP ({1}) ", new object[2]
			{
				parent.FormatFullNameForScripting(sp),
				FormatFullNameForScripting(sp)
			});
			if (noPopulation || fullTextIndex.noPopulation)
			{
				stringBuilder.Append("WITH NO POPULATION");
			}
			dropQuery.Add(stringBuilder.ToString());
		}
		else
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_fulltext_column @tabname=N'{0}', @colname={1}, @action=N'drop'", new object[2]
			{
				parent.FormatFullNameForScripting(sp),
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			dropQuery.Add(stringBuilder.ToString());
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

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[3] { "TypeColumnName", "Language", "StatisticalSemantics" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}
