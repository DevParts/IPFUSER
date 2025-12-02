using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class Check : ScriptNameObjectBase, ISfcSupportsDesignMode, ICreatable, IDroppable, IDropIfExists, IRenamable, IMarkForDrop, IAlterable, IExtendedProperties, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 7, 7, 8, 8, 8, 9, 9, 9, 9, 9 };

		private static int[] cloudVersionCount = new int[3] { 8, 8, 8 };

		private static int sqlDwPropertyCount = 8;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[8]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsChecked", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemNamed", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("NotForReplication", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Text", expensive: true, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[8]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsChecked", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemNamed", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("NotForReplication", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Text", expensive: true, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[9]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsChecked", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemNamed", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("NotForReplication", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Text", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("IsFileTableDefined", expensive: false, readOnly: true, typeof(bool))
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
						"DateLastModified" => 1, 
						"ID" => 2, 
						"IsChecked" => 3, 
						"IsEnabled" => 4, 
						"IsSystemNamed" => 5, 
						"NotForReplication" => 6, 
						"Text" => 7, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"CreateDate" => 0, 
					"DateLastModified" => 1, 
					"ID" => 2, 
					"IsChecked" => 3, 
					"IsEnabled" => 4, 
					"IsSystemNamed" => 5, 
					"NotForReplication" => 6, 
					"Text" => 7, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"CreateDate" => 0, 
				"ID" => 1, 
				"IsChecked" => 2, 
				"IsEnabled" => 3, 
				"IsSystemNamed" => 4, 
				"NotForReplication" => 5, 
				"Text" => 6, 
				"DateLastModified" => 7, 
				"IsFileTableDefined" => 8, 
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

	[SfcParent("UserDefinedFunction")]
	[SfcObject(SfcObjectRelationship.ParentObject, SfcObjectFlags.Design)]
	[SfcParent("Table")]
	[SfcParent("UserDefinedTableType")]
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
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsChecked
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsChecked");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsChecked", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsFileTableDefined => (bool)base.Properties.GetValueWithNullReplacement("IsFileTableDefined");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool NotForReplication
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("NotForReplication");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NotForReplication", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string Text
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Text");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Text", value);
		}
	}

	public static string UrnSuffix => "Check";

	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public override string Name
	{
		get
		{
			if (base.IsDesignMode && GetIsSystemNamed() && base.State == SqlSmoState.Creating)
			{
				return null;
			}
			return base.Name;
		}
		set
		{
			base.Name = value;
			if (base.ParentColl != null)
			{
				SetIsSystemNamed(flag: false);
			}
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsSystemNamed
	{
		get
		{
			if (base.ParentColl != null && base.IsDesignMode && base.State != SqlSmoState.Existing)
			{
				throw new PropertyNotSetException("IsSystemNamed");
			}
			return (bool)base.Properties.GetValueWithNullReplacement("IsSystemNamed");
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

	public Check()
	{
	}

	public Check(SqlSmoObject parent, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = parent;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[3] { "IsChecked", "NotForReplication", "Text" };
	}

	internal Check(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	internal override void UpdateObjectState()
	{
		if (base.State == SqlSmoState.Pending && base.ParentColl != null && (!key.IsNull || base.IsDesignMode))
		{
			SetState(SqlSmoState.Creating);
			if (key.IsNull)
			{
				AutoGenerateName();
			}
			else
			{
				SetIsSystemNamed(flag: false);
			}
		}
	}

	public StringCollection Script()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Script, this, null);
		}
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Script, this, null);
		}
		return ScriptImpl(scriptingOptions);
	}

	public void Create()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.UDTTChecksCannotBeModified);
		}
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		if (base.ParentColl.ParentInstance is UserDefinedFunction)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfAUDF);
		}
		ConstraintScriptCreate(ScriptDdlBodyWithName(sp), queries, sp);
	}

	public void Alter()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.UDTTChecksCannotBeModified);
		}
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		if (base.ParentColl.ParentInstance is UserDefinedFunction)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfAUDF);
		}
		ConstraintScriptAlter(queries, sp);
	}

	private string ScriptDdlBodyWithName(ScriptingPreferences sp)
	{
		return ScriptDdlBodyWorker(sp, withConstraintName: true);
	}

	internal string ScriptDdlBodyWithoutName(ScriptingPreferences sp)
	{
		return ScriptDdlBodyWorker(sp, withConstraintName: false);
	}

	internal string ScriptDdlBody(ScriptingPreferences sp)
	{
		if (string.IsNullOrEmpty(Name))
		{
			return ScriptDdlBodyWithoutName(sp);
		}
		return ScriptDdlBodyWithName(sp);
	}

	private string ScriptDdlBodyWorker(ScriptingPreferences sp, bool withConstraintName)
	{
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		bool flag = false;
		if (IsSupportedProperty("NotForReplication", sp))
		{
			Property property = base.Properties.Get("NotForReplication");
			if (property.Value != null && !SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType))
			{
				flag = (bool)property.Value;
			}
		}
		if (withConstraintName)
		{
			AddConstraintName(stringBuilder, sp);
		}
		string text = (string)GetPropValue("Text");
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CHECK {0} ({1})", new object[2]
		{
			flag ? "NOT FOR REPLICATION" : "",
			text
		});
		return stringBuilder.ToString();
	}

	internal override string GetScriptIncludeExists(ScriptingPreferences sp, string tableName, bool forCreate)
	{
		string text = FormatFullNameForScripting(sp);
		if (Parent != null && Parent is Table)
		{
			text = SqlSmoObject.MakeSqlBraket(((Table)Parent).Schema) + "." + text;
		}
		text = SqlSmoObject.MakeSqlString(text);
		if (sp.TargetServerVersionInternal > SqlServerVersionInternal.Version80)
		{
			return string.Format(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_CHECK90, new object[3]
			{
				forCreate ? "NOT" : "",
				text,
				SqlSmoObject.MakeSqlString(tableName)
			});
		}
		return string.Format(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_CHECK80, new object[2]
		{
			forCreate ? "NOT" : "",
			text
		});
	}

	public void Rename(string newname)
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.UDTTChecksCannotBeModified);
		}
		RenameImpl(newname);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "EXEC {0}.dbo.sp_rename @objname = N'{1}.{2}', @newname = N'{3}', @objtype = N'OBJECT'", SqlSmoObject.MakeSqlBraket(GetDBName()), SqlSmoObject.SqlString(SqlSmoObject.MakeSqlBraket(((ScriptSchemaObjectBase)base.ParentColl.ParentInstance).Schema)), SqlSmoObject.SqlString(FullQualifiedName), SqlSmoObject.SqlString(newName)));
	}

	public void Drop()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.UDTTChecksCannotBeModified);
		}
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		if (base.ParentColl.ParentInstance is UserDefinedFunction)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfAUDF);
		}
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		bool flag = VersionUtils.IsTargetServerVersionSQl13OrLater(sp.TargetServerVersionInternal);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			TableViewBase tableViewBase = (TableViewBase)base.ParentColl.ParentInstance;
			string text = tableViewBase.FormatFullNameForScripting(sp);
			if (flag)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_TABLE90, new object[2]
				{
					"",
					SqlSmoObject.SqlString(text)
				});
			}
			else
			{
				stringBuilder.Append(GetScriptIncludeExists(sp, text, forCreate: false));
			}
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER TABLE {0} DROP CONSTRAINT {1}{2}", new object[3]
		{
			((ScriptSchemaObjectBase)base.ParentColl.ParentInstance).FormatFullNameForScripting(sp),
			(sp.IncludeScripts.ExistenceCheck && flag) ? "IF EXISTS " : string.Empty,
			FormatFullNameForScripting(sp)
		});
		queries.Add(stringBuilder.ToString());
	}

	public void MarkForDrop(bool dropOnAlter)
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.UDTTChecksCannotBeModified);
		}
		MarkForDropImpl(dropOnAlter);
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		if (DatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
		{
			return new PropagateInfo[1]
			{
				new PropagateInfo((base.ServerVersion.Major < 8 || Parent is UserDefinedTableType) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
			};
		}
		return null;
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[5] { "NotForReplication", "IsChecked", "IsEnabled", "IsSystemNamed", "IsFileTableDefined" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		supportedScriptFields.Add("Text");
		return supportedScriptFields.ToArray();
	}
}
