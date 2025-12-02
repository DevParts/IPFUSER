using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcBrowsable(false)]
[SfcElementType("OleDbProviderSetting")]
[SfcElement(SfcElementFlags.Standalone)]
public sealed class OleDbProviderSettings : NamedSmoObject, IAlterable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 };

		private static int[] cloudVersionCount;

		private static int sqlDwPropertyCount;

		internal static StaticMetadata[] sqlDwStaticMetadata;

		internal static StaticMetadata[] cloudStaticMetadata;

		internal static StaticMetadata[] staticMetadata;

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
				return -1;
			}
			return propertyName switch
			{
				"AllowInProcess" => 0, 
				"Description" => 1, 
				"DisallowAdHocAccess" => 2, 
				"DynamicParameters" => 3, 
				"IndexAsAccessPath" => 4, 
				"LevelZeroOnly" => 5, 
				"NestedQueries" => 6, 
				"NonTransactedUpdates" => 7, 
				"SqlServerLike" => 8, 
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

		static PropertyMetadataProvider()
		{
			int[] array = new int[3];
			cloudVersionCount = array;
			sqlDwPropertyCount = 0;
			sqlDwStaticMetadata = new StaticMetadata[0];
			cloudStaticMetadata = new StaticMetadata[0];
			staticMetadata = new StaticMetadata[9]
			{
				new StaticMetadata("AllowInProcess", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("Description", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("DisallowAdHocAccess", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("DynamicParameters", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("IndexAsAccessPath", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("LevelZeroOnly", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("NestedQueries", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("NonTransactedUpdates", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("SqlServerLike", expensive: false, readOnly: false, typeof(bool))
			};
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Settings Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as Settings;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool AllowInProcess
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AllowInProcess");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AllowInProcess", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string Description => (string)base.Properties.GetValueWithNullReplacement("Description");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool DisallowAdHocAccess
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("DisallowAdHocAccess");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DisallowAdHocAccess", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool DynamicParameters
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("DynamicParameters");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DynamicParameters", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IndexAsAccessPath
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IndexAsAccessPath");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IndexAsAccessPath", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool LevelZeroOnly
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("LevelZeroOnly");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("LevelZeroOnly", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool NestedQueries
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("NestedQueries");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NestedQueries", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool NonTransactedUpdates
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("NonTransactedUpdates");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NonTransactedUpdates", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool SqlServerLike
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("SqlServerLike");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SqlServerLike", value);
		}
	}

	public static string UrnSuffix => "OleDbProviderSetting";

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
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

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal OleDbProviderSettings(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		ScriptProperties(query, sp);
	}

	internal override void ScriptCreate(StringCollection query, ScriptingPreferences sp)
	{
		ScriptProperties(query, sp);
	}

	private void ScriptProperties(StringCollection query, ScriptingPreferences sp)
	{
		new StringBuilder();
		InitializeKeepDirtyValues();
		string[][] array = new string[9][]
		{
			new string[3] { "AllowInProcess", "AllowInProcess", "REG_DWORD" },
			new string[3] { "DisallowAdHocAccess", "DisallowAdHocAccess", "REG_DWORD" },
			new string[3] { "DynamicParameters", "DynamicParameters", "REG_DWORD" },
			new string[3] { "IndexAsAccessPath", "IndexAsAccessPath", "REG_DWORD" },
			new string[3] { "LevelZeroOnly", "LevelZeroOnly", "REG_DWORD" },
			new string[3] { "NestedQueries", "NestedQueries", "REG_DWORD" },
			new string[3] { "NonTransactedUpdates", "NonTransactedUpdates", "REG_DWORD" },
			new string[3] { "SqlServerLike", "SqlServerLIKE", "REG_DWORD" },
			new string[3] { "", "", "" }
		};
		object obj = null;
		for (int i = 0; array[i][0].Length > 0; i++)
		{
			Property property = base.Properties.Get(array[i][0]);
			if (property.Value != null && (!sp.ScriptForAlter || property.Dirty))
			{
				obj = property.Value;
				int num = (((bool)obj) ? 1 : 0);
				if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
				{
					query.Add(string.Format(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_MSset_oledb_prop {0}, {1}, {2}", new object[3]
					{
						SqlSmoObject.MakeSqlString(Name),
						SqlSmoObject.MakeSqlString(array[i][1]),
						num
					}));
				}
				else if (!(bool)obj)
				{
					ScriptDeleteRegSetting(query, array[i]);
				}
				else
				{
					ScriptRegSetting(query, array[i], num);
				}
			}
		}
	}

	private void ScriptRegSetting(StringCollection query, string[] prop, object oValue)
	{
		string text = null;
		text = ((base.ServerVersion.Major > 7) ? ("EXEC xp_instance_regwrite N'HKEY_LOCAL_MACHINE', N'Software\\Microsoft\\MSSQLServer\\Providers\\" + SqlSmoObject.SqlString(Name) + "', N'{0}', {1}, {2}") : ("EXEC xp_regwrite N'HKEY_LOCAL_MACHINE', N'Software\\Microsoft\\MSSQLServer\\Providers\\" + SqlSmoObject.SqlString(Name) + "', N'{0}', {1}, {2}"));
		if ("REG_SZ" == prop[2])
		{
			query.Add(string.Format(SmoApplication.DefaultCulture, text, new object[3]
			{
				prop[1],
				prop[2],
				"N'" + SqlSmoObject.SqlString(oValue.ToString())
			}) + "'");
		}
		else
		{
			query.Add(string.Format(SmoApplication.DefaultCulture, text, new object[3]
			{
				prop[1],
				prop[2],
				oValue.ToString()
			}));
		}
	}

	private void ScriptDeleteRegSetting(StringCollection query, string[] prop)
	{
		string text = null;
		query.Add(string.Format(format: (base.ServerVersion.Major > 7) ? ("EXEC xp_instance_regdeletevalue N'HKEY_LOCAL_MACHINE', N'Software\\Microsoft\\MSSQLServer\\Providers\\" + SqlSmoObject.SqlString(Name) + "', N'{0}'") : ("EXEC xp_regdeletevalue N'HKEY_LOCAL_MACHINE', N'Software\\Microsoft\\MSSQLServer\\Providers\\" + SqlSmoObject.SqlString(Name) + "', N'{0}'"), provider: SmoApplication.DefaultCulture, args: new object[1] { prop[1] }));
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}
}
