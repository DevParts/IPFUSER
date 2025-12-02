using System;
using System.Collections.Specialized;
using System.Data.SqlTypes;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class ExtendedProperty : ScriptNameObjectBase, ICreatable, IAlterable, IDroppable, IDropIfExists, IMarkForDrop, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 1 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[1]
		{
			new StaticMetadata("Value", expensive: false, readOnly: false, typeof(object))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[1]
		{
			new StaticMetadata("Value", expensive: false, readOnly: false, typeof(object))
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
				string text;
				if ((text = propertyName) != null && text == "Value")
				{
					return 0;
				}
				return -1;
			}
			string text2;
			if ((text2 = propertyName) != null && text2 == "Value")
			{
				return 0;
			}
			return -1;
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

	internal class ScriptingParameters
	{
		private struct TypeNamePair
		{
			internal string type;

			internal string name;
		}

		private TypeNamePair[] typeNames_;

		private void SetTypeNamePair(int level, string type, string name)
		{
			TraceHelper.Assert(level >= 0 && level <= 2);
			typeNames_[level].type = type;
			typeNames_[level].name = SqlSmoObject.SqlString(name);
		}

		private void SetSchema(SqlSmoObject objParent, ScriptingPreferences sp)
		{
			if (!(objParent is ScriptSchemaObjectBase scriptSchemaObjectBase))
			{
				throw new SmoException(ExceptionTemplatesImpl.CannotCreateExtendedPropertyWithoutSchema);
			}
			string type = ((sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90) ? "SCHEMA" : "USER");
			string schema = scriptSchemaObjectBase.Schema;
			SetTypeNamePair(0, type, schema);
		}

		internal ScriptingParameters(SqlSmoObject objParent, ScriptingPreferences sp)
		{
			typeNames_ = new TypeNamePair[3];
			for (int i = 0; i < typeNames_.Length; i++)
			{
				typeNames_[i].type = null;
				typeNames_[i].name = null;
			}
			string text = objParent.GetType().ToString();
			string text2 = "Microsoft.SqlServer.Management.Smo.";
			if (!text.StartsWith(text2, StringComparison.Ordinal))
			{
				return;
			}
			switch (text.Remove(0, text2.Length))
			{
			case "Schema":
				SetTypeNamePair(0, "SCHEMA", objParent.InternalName);
				break;
			case "DatabaseRole":
			case "ApplicationRole":
			case "User":
				SetTypeNamePair(0, "USER", objParent.InternalName);
				break;
			case "UserDefinedDataType":
				if (objParent is ScriptSchemaObjectBase scriptSchemaObjectBase)
				{
					if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
					{
						SetTypeNamePair(0, "SCHEMA", scriptSchemaObjectBase.Schema);
						SetTypeNamePair(1, "TYPE", objParent.InternalName);
					}
					else
					{
						SetTypeNamePair(0, "TYPE", objParent.InternalName);
					}
					break;
				}
				throw new SmoException(ExceptionTemplatesImpl.CannotCreateExtendedPropertyWithoutSchema);
			case "UserDefinedTableType":
				if (objParent is ScriptSchemaObjectBase scriptSchemaObjectBase2)
				{
					SetTypeNamePair(0, "SCHEMA", scriptSchemaObjectBase2.Schema);
					SetTypeNamePair(1, "TYPE", objParent.InternalName);
					break;
				}
				throw new SmoException(ExceptionTemplatesImpl.CannotCreateExtendedPropertyWithoutSchema);
			case "DdlTrigger":
			case "DatabaseDdlTrigger":
			case "ServerDdlTrigger":
				SetTypeNamePair(0, "TRIGGER", objParent.InternalName);
				break;
			case "PlanGuide":
				SetTypeNamePair(0, "PLAN GUIDE", objParent.InternalName);
				break;
			case "Table":
				SetSchema(objParent, sp);
				SetTypeNamePair(1, "TABLE", objParent.InternalName);
				break;
			case "View":
				SetSchema(objParent, sp);
				SetTypeNamePair(1, "VIEW", objParent.InternalName);
				break;
			case "ExtendedStoredProcedure":
			case "StoredProcedure":
				SetSchema(objParent, sp);
				SetTypeNamePair(1, "PROCEDURE", objParent.InternalName);
				break;
			case "Synonym":
				SetSchema(objParent, sp);
				SetTypeNamePair(1, "SYNONYM", objParent.InternalName);
				break;
			case "Sequence":
				SetSchema(objParent, sp);
				SetTypeNamePair(1, "SEQUENCE", objParent.InternalName);
				break;
			case "Rule":
				SetSchema(objParent, sp);
				SetTypeNamePair(1, "RULE", objParent.InternalName);
				break;
			case "Default":
				SetSchema(objParent, sp);
				SetTypeNamePair(1, "DEFAULT", objParent.InternalName);
				break;
			case "UserDefinedFunction":
				SetSchema(objParent, sp);
				SetTypeNamePair(1, "FUNCTION", objParent.InternalName);
				break;
			case "Column":
				CopyFrom(new ScriptingParameters(objParent.ParentColl.ParentInstance, sp));
				SetTypeNamePair(2, "COLUMN", objParent.InternalName);
				break;
			case "Index":
			{
				Index index = (Index)objParent;
				CopyFrom(new ScriptingParameters(objParent.ParentColl.ParentInstance, sp));
				string type = ((index.IndexKeyType != IndexKeyType.None) ? "CONSTRAINT" : "INDEX");
				SetTypeNamePair(2, type, objParent.InternalName);
				break;
			}
			case "Trigger":
				CopyFrom(new ScriptingParameters(objParent.ParentColl.ParentInstance, sp));
				SetTypeNamePair(2, "TRIGGER", objParent.InternalName);
				break;
			case "UserDefinedFunctionParameter":
			case "UserDefinedAggregateParameter":
			case "StoredProcedureParameter":
				CopyFrom(new ScriptingParameters(objParent.ParentColl.ParentInstance, sp));
				SetTypeNamePair(2, "PARAMETER", objParent.InternalName);
				break;
			case "Check":
				CopyFrom(new ScriptingParameters(objParent.ParentColl.ParentInstance, sp));
				SetTypeNamePair(2, "CONSTRAINT", objParent.InternalName);
				break;
			case "ForeignKey":
				CopyFrom(new ScriptingParameters(objParent.ParentColl.ParentInstance, sp));
				SetTypeNamePair(2, "CONSTRAINT", objParent.InternalName);
				break;
			case "DefaultConstraint":
				CopyFrom(new ScriptingParameters(((DefaultConstraint)objParent).Parent.Parent, sp));
				SetTypeNamePair(2, "CONSTRAINT", objParent.InternalName);
				break;
			case "XmlSchemaCollection":
				SetSchema(objParent, sp);
				SetTypeNamePair(1, "XML SCHEMA COLLECTION", objParent.InternalName);
				break;
			case "PartitionFunction":
				SetTypeNamePair(0, "PARTITION FUNCTION", objParent.InternalName);
				break;
			case "PartitionScheme":
				SetTypeNamePair(0, "PARTITION SCHEME", objParent.InternalName);
				break;
			case "SqlAssembly":
				SetTypeNamePair(0, "ASSEMBLY", objParent.InternalName);
				break;
			case "ExternalLibrary":
				SetTypeNamePair(0, "EXTERNAL LIBRARY", objParent.InternalName);
				break;
			case "UserDefinedType":
				SetSchema(objParent, sp);
				SetTypeNamePair(1, "TYPE", objParent.InternalName);
				break;
			case "UserDefinedAggregate":
				SetSchema(objParent, sp);
				SetTypeNamePair(1, "AGGREGATE", objParent.InternalName);
				break;
			case "Broker.MessageType":
				SetTypeNamePair(0, "MESSAGE TYPE", objParent.InternalName);
				break;
			case "Broker.ServiceContract":
				SetTypeNamePair(0, "CONTRACT", objParent.InternalName);
				break;
			case "Broker.BrokerService":
				SetTypeNamePair(0, "SERVICE", objParent.InternalName);
				break;
			case "Broker.RemoteServiceBinding":
				SetTypeNamePair(0, "REMOTE SERVICE BINDING", objParent.InternalName);
				break;
			case "Broker.ServiceRoute":
				SetTypeNamePair(0, "ROUTE", objParent.InternalName);
				break;
			case "Broker.ServiceQueue":
				SetSchema(objParent, sp);
				SetTypeNamePair(1, "QUEUE", objParent.InternalName);
				break;
			case "SecurityPolicy":
				SetSchema(objParent, sp);
				SetTypeNamePair(1, "SECURITY POLICY", objParent.InternalName);
				break;
			case "Database":
				break;
			}
		}

		private void CopyFrom(ScriptingParameters other)
		{
			for (int i = 0; i < typeNames_.Length; i++)
			{
				typeNames_[i].type = other.typeNames_[i].type;
				typeNames_[i].name = other.typeNames_[i].name;
			}
		}

		internal string GetDDLParam()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < typeNames_.Length; i++)
			{
				if (typeNames_[i].type != null)
				{
					stringBuilder.AppendFormat(", @level{0}type=N'{1}',@level{0}name=N'{2}'", i, typeNames_[i].type, typeNames_[i].name);
				}
			}
			return stringBuilder.ToString();
		}

		internal string GetIfNotExistParam()
		{
			StringBuilder stringBuilder = new StringBuilder();
			TypeNamePair[] array = typeNames_;
			for (int i = 0; i < array.Length; i++)
			{
				TypeNamePair typeNamePair = array[i];
				if (typeNamePair.type != null)
				{
					stringBuilder.AppendFormat(", N'{0}',N'{1}'", typeNamePair.type, typeNamePair.name);
				}
				else
				{
					stringBuilder.AppendFormat(", NULL,NULL");
				}
			}
			return stringBuilder.ToString();
		}
	}

	[SfcParent("Table")]
	[SfcParent("StoredProcedure")]
	[SfcParent("StoredProcedureParameter")]
	[SfcParent("NumberedStoredProcedureParameter")]
	[SfcParent("ExtendedStoredProcedure")]
	[SfcParent("UserDefinedDataType")]
	[SfcParent("UserDefinedTableType")]
	[SfcParent("UserDefinedFunction")]
	[SfcParent("UserDefinedFunctionParameter")]
	[SfcParent("View")]
	[SfcParent("XmlSchemaCollection")]
	[SfcParent("PartitionFunction")]
	[SfcParent("PartitionScheme")]
	[SfcParent("DatabaseDdlTrigger")]
	[SfcParent("SecurityPolicy")]
	[SfcParent("Sequence")]
	[SfcParent("ApplicationRole")]
	[SfcParent("Trigger")]
	[SfcParent("Check")]
	[SfcParent("Index")]
	[SfcParent("ForeignKey")]
	[SfcParent("Default")]
	[SfcParent("Rule")]
	[SfcParent("Synonym")]
	[SfcParent("Column")]
	[SfcObject(SfcObjectRelationship.ParentObject)]
	[SfcParent("Database")]
	[SfcParent("SqlAssembly")]
	[SfcParent("UserDefinedAggregate")]
	[SfcParent("UserDefinedAggregateParameter")]
	[SfcParent("UserDefinedType")]
	[SfcParent("PlanGuide")]
	[SfcParent("User")]
	[SfcParent("Schema")]
	[SfcParent("DatabaseRole")]
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

	public static string UrnSuffix => "ExtendedProperty";

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public object Value
	{
		get
		{
			return base.Properties.GetValueWithNullReplacement("Value", throwOnNullValue: false, useDefaultOnMissingValue: false);
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Value", value, allowNull: true);
		}
	}

	public ExtendedProperty()
	{
	}

	public ExtendedProperty(SqlSmoObject parent, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = parent;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal ExtendedProperty(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public ExtendedProperty(SqlSmoObject parent, string name, object propertyValue)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = parent;
		Value = propertyValue;
	}

	protected internal override string GetDBName()
	{
		return Parent.GetDBName();
	}

	private string GetParams(SqlSmoObject objParent, ScriptingPreferences sp)
	{
		ScriptingParameters scriptingParameters = new ScriptingParameters(objParent, sp);
		return scriptingParameters.GetDDLParam();
	}

	public void Create()
	{
		CreateImpl();
	}

	private string GetPrefix(ScriptingPreferences sp)
	{
		Database database = Parent as Database;
		string empty = string.Empty;
		empty = ((sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90) ? "sys" : "dbo");
		if (sp.IncludeScripts.DatabaseContext && database != null)
		{
			empty = database.FormatFullNameForScripting(sp) + "." + empty;
		}
		return empty;
	}

	private string GetIfNotExistString(bool bCreate, string name, string param)
	{
		return string.Format(SmoApplication.DefaultCulture, "IF {0} EXISTS (SELECT * FROM sys.fn_listextendedproperty({1} {2}))", new object[3]
		{
			bCreate ? "NOT" : "",
			name,
			param
		});
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		object propValueOptionalAllowNull = GetPropValueOptionalAllowNull("Value");
		bool flag = propValueOptionalAllowNull is DateTime || propValueOptionalAllowNull is SqlDateTime;
		ScriptingParameters scriptingParameters = new ScriptingParameters(base.ParentColl.ParentInstance, sp);
		bool flag2 = !sp.OldOptions.DdlHeaderOnly && !sp.OldOptions.DdlBodyOnly && sp.IncludeScripts.ExistenceCheck;
		if (flag2)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, GetIfNotExistString(bCreate: true, FormatFullNameForScripting(sp, nameIsIndentifier: false), scriptingParameters.GetIfNotExistParam()));
			stringBuilder.AppendLine();
		}
		if (flag)
		{
			if (flag2)
			{
				stringBuilder.AppendLine(Scripts.BEGIN);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "declare @datetime_val datetime \n set @datetime_val=cast({0} as datetime) \n EXEC {1}.sp_addextendedproperty @name={2}, @value=@datetime_val {3}", FormatSqlVariant(propValueOptionalAllowNull), GetPrefix(sp), FormatFullNameForScripting(sp, nameIsIndentifier: false), scriptingParameters.GetDDLParam());
			if (flag2)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(Scripts.END);
			}
		}
		else
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC {0}.sp_addextendedproperty @name={1}, @value={2} {3}", GetPrefix(sp), FormatFullNameForScripting(sp, nameIsIndentifier: false), FormatSqlVariant(propValueOptionalAllowNull), scriptingParameters.GetDDLParam());
		}
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
		CheckObjectState();
		ScriptingParameters scriptingParameters = new ScriptingParameters(base.ParentColl.ParentInstance, sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (!sp.OldOptions.DdlHeaderOnly && !sp.OldOptions.DdlBodyOnly && sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, GetIfNotExistString(bCreate: false, FormatFullNameForScripting(sp, nameIsIndentifier: false), scriptingParameters.GetIfNotExistParam()));
			stringBuilder.AppendLine();
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC {2}.sp_dropextendedproperty @name={0} {1}", new object[3]
		{
			FormatFullNameForScripting(sp, nameIsIndentifier: false),
			scriptingParameters.GetDDLParam(),
			GetPrefix(sp)
		});
		stringBuilder.Append(sp.NewLine);
		queries.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		if (IsObjectDirty())
		{
			this.ThrowIfNotSupported(GetType(), sp);
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			object propValueOptional = GetPropValueOptional("Value");
			bool flag = propValueOptional is DateTime || propValueOptional is SqlDateTime;
			string empty = string.Empty;
			empty = ((!flag) ? string.Format(SmoApplication.DefaultCulture, "EXEC {0}.sp_updateextendedproperty @name={1}, @value={2} {3}", GetPrefix(sp), FormatFullNameForScripting(sp, nameIsIndentifier: false), FormatSqlVariant(propValueOptional), GetParams(base.ParentColl.ParentInstance, sp)) : string.Format(SmoApplication.DefaultCulture, "declare @datetime_val datetime \n set @datetime_val=cast({0} as datetime) \n EXEC {1}.sp_updateextendedproperty @name={2}, @value=@datetime_val {3}", FormatSqlVariant(propValueOptional), GetPrefix(sp), FormatFullNameForScripting(sp, nameIsIndentifier: false), GetParams(base.ParentColl.ParentInstance, sp)));
			stringBuilder.Append(empty);
			stringBuilder.Append(sp.NewLine);
			alterQuery.Add(stringBuilder.ToString());
		}
	}

	public void MarkForDrop(bool dropOnAlter)
	{
		MarkForDropImpl(dropOnAlter);
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
		return new string[1] { "Value" };
	}
}
