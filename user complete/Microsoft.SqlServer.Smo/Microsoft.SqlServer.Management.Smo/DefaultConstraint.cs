using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[SfcElementType("Default")]
public sealed class DefaultConstraint : ScriptNameObjectBase, ISfcSupportsDesignMode, ICreatable, IDroppable, IDropIfExists, IRenamable, IAlterable, IExtendedProperties, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 5, 5, 6, 6, 6, 6, 6, 6, 6, 6 };

		private static int[] cloudVersionCount = new int[3] { 5, 5, 6 };

		private static int sqlDwPropertyCount = 6;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[6]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsFileTableDefined", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemNamed", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Text", expensive: true, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[6]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsSystemNamed", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Text", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("IsFileTableDefined", expensive: false, readOnly: true, typeof(bool))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[6]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsFileTableDefined", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemNamed", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Text", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime))
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
						"IsFileTableDefined" => 3, 
						"IsSystemNamed" => 4, 
						"Text" => 5, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"CreateDate" => 0, 
					"DateLastModified" => 1, 
					"ID" => 2, 
					"IsSystemNamed" => 3, 
					"Text" => 4, 
					"IsFileTableDefined" => 5, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"CreateDate" => 0, 
				"ID" => 1, 
				"IsFileTableDefined" => 2, 
				"IsSystemNamed" => 3, 
				"Text" => 4, 
				"DateLastModified" => 5, 
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

	private const string IfExistsDefaultConstraint = "IF {0} EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{1}') AND type = 'D')\r\nBEGIN\r\n{2}\r\nEND\r\n";

	internal bool forceEmbedDefaultConstraint;

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsFileTableDefined => (bool)base.Properties.GetValueWithNullReplacement("IsFileTableDefined");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
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

	[SfcObject(SfcObjectRelationship.ParentObject, SfcObjectFlags.Design)]
	public Column Parent
	{
		get
		{
			return singletonParent as Column;
		}
		internal set
		{
			SetServerObject(value.GetServerObject());
			SetParentImpl(value);
			value.DefaultConstraint = this;
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public override string Name
	{
		get
		{
			if (base.IsDesignMode && GetIsSystemNamed() && singletonParent.State == SqlSmoState.Creating)
			{
				return null;
			}
			return base.Name;
		}
		set
		{
			base.Name = value;
			if (singletonParent != null)
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
			if (singletonParent != null && base.IsDesignMode && singletonParent.State != SqlSmoState.Existing)
			{
				throw new PropertyNotSetException("IsSystemNamed");
			}
			return (bool)base.Properties.GetValueWithNullReplacement("IsSystemNamed");
		}
	}

	public static string UrnSuffix => "Default";

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(ExtendedProperty))]
	public ExtendedPropertyCollection ExtendedProperties
	{
		get
		{
			ThrowIfBelowVersion80();
			CheckObjectState();
			if (m_ExtendedProperties == null)
			{
				if (m_comparer == null)
				{
					m_comparer = Parent.StringComparer;
				}
				m_ExtendedProperties = new ExtendedPropertyCollection(this);
			}
			return m_ExtendedProperties;
		}
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal DefaultConstraint()
	{
	}

	internal DefaultConstraint(Column parentColumn, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentColumn;
		SetServerObject(parentColumn.GetServerObject());
	}

	internal override void UpdateObjectState()
	{
		if (base.State == SqlSmoState.Pending && singletonParent != null && (!key.IsNull || base.IsDesignMode))
		{
			SetState(SqlSmoState.Existing);
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

	internal override void ValidateName(string name)
	{
		if (name == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("Name"));
		}
		if (base.IsDesignMode || singletonParent.State == SqlSmoState.Pending || singletonParent.State == SqlSmoState.Creating)
		{
			return;
		}
		throw new InvalidSmoOperationException(ExceptionTemplatesImpl.OperationOnlyInPendingState);
	}

	public StringCollection Script()
	{
		if (Parent.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Script, this, null, ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfUDTT);
		}
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		if (Parent.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Script, this, null, ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfUDTT);
		}
		return ScriptImpl(scriptingOptions);
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}[{1}]", new object[2] { UrnSuffix, key.UrnFilter });
	}

	public void Create()
	{
		if (Parent.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Script, this, null, ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfUDTT);
		}
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		string text = ((ScriptSchemaObjectBase)Parent.ParentColl.ParentInstance).FormatFullNameForScripting(sp);
		string script = string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} ADD {1} FOR [{2}]", new object[3]
		{
			text,
			ScriptDdl(sp),
			SqlSmoObject.SqlBraket(Parent.Name)
		});
		queries.Add(AddIfExistsCheck(script, sp, "NOT"));
	}

	public void Drop()
	{
		if (Parent.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Script, this, null, ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfUDTT);
		}
		DropImpl();
		if (base.State == SqlSmoState.Dropped)
		{
			Parent.RemoveDefaultConstraint();
		}
	}

	public void DropIfExists()
	{
		if (Parent.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Script, this, null, ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfUDTT);
		}
		DropImpl(isDropIfExists: true);
		if (base.State == SqlSmoState.Dropped)
		{
			Parent.RemoveDefaultConstraint();
		}
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		CheckObjectState();
		string text = ((ScriptSchemaObjectBase)Parent.ParentColl.ParentInstance).FormatFullNameForScripting(sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		bool flag = VersionUtils.IsTargetServerVersionSQl13OrLater(sp.TargetServerVersionInternal);
		if (sp.IncludeScripts.ExistenceCheck && flag)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_TABLE90, new object[2]
			{
				"",
				SqlSmoObject.SqlString(text)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER TABLE {0} DROP CONSTRAINT {1}[{2}]", new object[3]
		{
			text,
			(sp.IncludeScripts.ExistenceCheck && flag) ? "IF EXISTS " : string.Empty,
			SqlSmoObject.SqlBraket(Name)
		});
		string text2 = stringBuilder.ToString();
		dropQuery.Add(flag ? text2 : AddIfExistsCheck(text2, sp, ""));
	}

	private string AddIfExistsCheck(string script, ScriptingPreferences sp, string qualifier)
	{
		if (!sp.IncludeScripts.ExistenceCheck)
		{
			return script;
		}
		return string.Format(SmoApplication.DefaultCulture, "IF {0} EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{1}') AND type = 'D')\r\nBEGIN\r\n{2}\r\nEND\r\n", new object[3]
		{
			qualifier,
			Util.EscapeString(FormatFullNameForScripting(sp), '\''),
			script
		});
	}

	public void Alter()
	{
		if (Parent.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Script, this, null, ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfUDTT);
		}
		AlterImpl();
	}

	internal string ScriptDdl(ScriptingPreferences sp)
	{
		if (!string.IsNullOrEmpty(Name))
		{
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			if (Parent.ParentColl.ParentInstance is Table && ScriptConstraintWithName(sp))
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " CONSTRAINT {0} ", new object[1] { SqlSmoObject.MakeSqlBraket(Name) });
			}
			stringBuilder.Append(" DEFAULT ");
			stringBuilder.Append(GetTextProperty("Text"));
			return stringBuilder.ToString();
		}
		return " DEFAULT " + GetTextProperty("Text");
	}

	public void Rename(string newname)
	{
		if (Parent.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Script, this, null, ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfUDTT);
		}
		RenameImpl(newname);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "EXEC {0}.dbo.sp_rename @objname = N'{1}', @newname = N'{2}', @objtype = N'OBJECT'", new object[3]
		{
			SqlSmoObject.MakeSqlBraket(GetDBName()),
			SqlSmoObject.SqlString(FormatFullNameForScripting(sp)),
			SqlSmoObject.SqlString(newName)
		}));
	}

	protected override string GetServerName()
	{
		return Parent.ParentColl.ParentInstance.ParentColl.ParentInstance.ParentColl.ParentInstance.InternalName;
	}

	protected internal override string GetDBName()
	{
		return Parent.ParentColl.ParentInstance.ParentColl.ParentInstance.InternalName;
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		forceEmbedDefaultConstraint = false;
		if (DatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
		{
			return new PropagateInfo[1]
			{
				new PropagateInfo((base.ServerVersion.Major < 8) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
			};
		}
		return null;
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[2] { "IsSystemNamed", "IsFileTableDefined" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		supportedScriptFields.Add("Text");
		return supportedScriptFields.ToArray();
	}

	internal override string FormatFullNameForScripting(ScriptingPreferences sp)
	{
		CheckObjectState();
		string text = string.Empty;
		if (sp.IncludeScripts.SchemaQualify)
		{
			string schema = ((ScriptSchemaObjectBase)Parent.ParentColl.ParentInstance).Schema;
			if (schema.Length > 0)
			{
				text = SqlSmoObject.MakeSqlBraket(schema);
				text += Globals.Dot;
			}
		}
		return text + base.FormatFullNameForScripting(sp);
	}
}
