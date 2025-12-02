using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone)]
public sealed class SearchProperty : ScriptNameObjectBase, ICreatable, IDroppable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 4, 4, 4, 4, 4 };

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
				"Description" => 0, 
				"ID" => 1, 
				"IntID" => 2, 
				"PropertySetGuid" => 3, 
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
			staticMetadata = new StaticMetadata[4]
			{
				new StaticMetadata("Description", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("IntID", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("PropertySetGuid", expensive: false, readOnly: false, typeof(Guid))
			};
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public SearchPropertyList Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as SearchPropertyList;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public string Description
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Description");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Description", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public int IntID
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("IntID");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IntID", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public Guid PropertySetGuid
	{
		get
		{
			return (Guid)base.Properties.GetValueWithNullReplacement("PropertySetGuid");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PropertySetGuid", value);
		}
	}

	public static string UrnSuffix => "SearchProperty";

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

	public SearchProperty()
	{
	}

	public SearchProperty(SearchPropertyList searchPropertyList, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = searchPropertyList;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[3] { "Description", "IntID", "PropertySetGuid" };
	}

	internal SearchProperty(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public SearchProperty(SearchPropertyList parent, string name, string propertySetGuid, int intID, string description)
	{
		Name = name;
		Parent = parent;
		PropertySetGuid = new Guid(propertySetGuid);
		IntID = intID;
		Description = description;
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		SqlSmoObject.ThrowIfBelowVersion110(sp.TargetServerVersionInternal);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, Name, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_SEARCH_PROPERTY, new object[3]
			{
				"NOT",
				Parent.ID,
				SqlSmoObject.SqlString(Name)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER {0} {1}", new object[2]
		{
			"SEARCH PROPERTY LIST",
			SqlSmoObject.MakeSqlBraket(Parent.Name)
		});
		stringBuilder.Append(sp.NewLine);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ADD {0}{1}WITH (", new object[2]
		{
			SqlSmoObject.MakeSqlString(Name),
			sp.NewLine
		});
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "PROPERTY_SET_GUID = {0}", new object[1] { SqlSmoObject.MakeSqlString(((Guid)GetPropValue("PropertySetGuid")/*cast due to .constrained prefix*/).ToString()) });
		object propValue = GetPropValue("IntID");
		if (propValue != null)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", PROPERTY_INT_ID = {0} ", new object[1] { propValue });
		}
		object propValueOptional = GetPropValueOptional("Description");
		if (propValueOptional != null)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", PROPERTY_DESCRIPTION = {0} ", new object[1] { SqlSmoObject.MakeSqlString(propValueOptional as string) });
		}
		stringBuilder.Append(");");
		stringBuilder.Append(sp.NewLine);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		createQuery.Add(stringBuilder.ToString());
	}

	public void Drop()
	{
		DropImpl();
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		SqlSmoObject.ThrowIfBelowVersion110(sp.TargetServerVersionInternal);
		dropQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER {0} {1} DROP {2};", new object[3]
		{
			"SEARCH PROPERTY LIST",
			SqlSmoObject.MakeSqlBraket(Parent.Name),
			SqlSmoObject.MakeSqlString(Name)
		}));
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		return new string[3] { "IntID", "Description", "PropertySetGuid" };
	}
}
