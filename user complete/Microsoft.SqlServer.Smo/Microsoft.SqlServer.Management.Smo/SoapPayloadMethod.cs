using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElementType("Method")]
[SfcElement(SfcElementFlags.Standalone)]
public sealed class SoapPayloadMethod : SoapMethodObject, ICreatable, IDroppable, IAlterable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 6, 6, 6, 6, 6, 6, 6, 6 };

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
				"IsSystemObject" => 0, 
				"MethodLoginType" => 1, 
				"MethodXsdSchemaOption" => 2, 
				"Namespace" => 3, 
				"ResultFormat" => 4, 
				"SqlMethod" => 5, 
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
			staticMetadata = new StaticMetadata[6]
			{
				new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("MethodLoginType", expensive: false, readOnly: false, typeof(MethodLoginType)),
				new StaticMetadata("MethodXsdSchemaOption", expensive: false, readOnly: false, typeof(MethodXsdSchemaOption)),
				new StaticMetadata("Namespace", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ResultFormat", expensive: false, readOnly: false, typeof(ResultFormat)),
				new StaticMetadata("SqlMethod", expensive: false, readOnly: true, typeof(string))
			};
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public SoapPayload Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as SoapPayload;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public MethodLoginType MethodLoginType
	{
		get
		{
			return (MethodLoginType)base.Properties.GetValueWithNullReplacement("MethodLoginType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MethodLoginType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public MethodXsdSchemaOption MethodXsdSchemaOption
	{
		get
		{
			return (MethodXsdSchemaOption)base.Properties.GetValueWithNullReplacement("MethodXsdSchemaOption");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MethodXsdSchemaOption", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public ResultFormat ResultFormat
	{
		get
		{
			return (ResultFormat)base.Properties.GetValueWithNullReplacement("ResultFormat");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ResultFormat", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string SqlMethod => (string)base.Properties.GetValueWithNullReplacement("SqlMethod");

	public static string UrnSuffix => "Method";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	public SoapPayloadMethod()
	{
	}

	public SoapPayloadMethod(SoapPayload soapPayload, string name)
	{
		ValidateName(name);
		key = new SoapMethodKey(name, SoapMethodCollectionBase.GetDefaultNamespace());
		Parent = soapPayload;
	}

	public SoapPayloadMethod(SoapPayload soapPayload, string name, string methodNamespace)
	{
		ValidateName(name);
		key = new SoapMethodKey(name, methodNamespace);
		Parent = soapPayload;
	}

	public SoapPayloadMethod(SoapPayload soapPayload, string name, string database, string schema, string sqlMethod)
	{
		ValidateName(name);
		key = new SoapMethodKey(name, SoapMethodCollectionBase.GetDefaultNamespace());
		Parent = soapPayload;
		SetSqlMethod(database, schema, sqlMethod);
	}

	public SoapPayloadMethod(SoapPayload soapPayload, string name, string database, string schema, string sqlMethod, string methodNamespace)
	{
		ValidateName(name);
		key = new SoapMethodKey(name, methodNamespace);
		Parent = soapPayload;
		SetSqlMethod(database, schema, sqlMethod);
	}

	internal SoapPayloadMethod(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		CreateImpl();
	}

	private void AddEndpointPart(StringBuilder sb, ScriptingPreferences sp)
	{
		Endpoint parent = ((SoapPayload)base.ParentColl.ParentInstance).Parent;
		_ = parent.FullQualifiedName;
		sb.AppendFormat(SmoApplication.DefaultCulture, "ALTER ENDPOINT {0} FOR SOAP", new object[1] { parent.FormatFullNameForScripting(sp) });
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		AddEndpointPart(stringBuilder, sp);
		stringBuilder.Append("( ADD ");
		Script(stringBuilder, sp);
		stringBuilder.Append(Globals.RParen);
		queries.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		AddEndpointPart(stringBuilder, sp);
		stringBuilder.Append("( ");
		Script(stringBuilder, sp);
		stringBuilder.Append(Globals.RParen);
		alterQuery.Add(stringBuilder.ToString());
	}

	public void Drop()
	{
		DropImpl();
	}

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		AddEndpointPart(stringBuilder, sp);
		stringBuilder.Append("( DROP ");
		Script(stringBuilder, sp);
		stringBuilder.Append(Globals.RParen);
		queries.Add(stringBuilder.ToString());
	}

	public void SetSqlMethod(string database, string schema, string name)
	{
		if (database == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("database"));
		}
		if (schema == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("schema"));
		}
		if (name == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("name"));
		}
		base.Properties.Get("SqlMethod").SetValue(string.Format(SmoApplication.DefaultCulture, "{0}.{1}.{2}", new object[3]
		{
			SqlSmoObject.MakeSqlBraket(database),
			SqlSmoObject.MakeSqlBraket(schema),
			SqlSmoObject.MakeSqlBraket(name)
		}));
		base.Properties.Get("SqlMethod").SetDirty(dirty: true);
	}

	internal void Script(StringBuilder sb, ScriptingPreferences sp)
	{
		if (sp.ScriptForAlter)
		{
			if (base.State == SqlSmoState.Creating)
			{
				sb.Append("ADD ");
			}
			else if (base.State == SqlSmoState.ToBeDropped)
			{
				sb.Append("DROP ");
			}
			else
			{
				sb.Append("ALTER ");
			}
		}
		sb.Append("WEBMETHOD ");
		string text = base.Namespace;
		if (text.Length > 0)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, "'{0}'.", new object[1] { SqlSmoObject.SqlString(text) });
		}
		sb.AppendFormat(SmoApplication.DefaultCulture, "'{0}'", new object[1] { SqlSmoObject.SqlString(GetName(sp)) });
		if (sp.Behavior == ScriptBehavior.Drop)
		{
			return;
		}
		string s = (string)GetPropValue("SqlMethod");
		sb.Append(Globals.LParen);
		sb.AppendFormat(SmoApplication.DefaultCulture, " NAME={0}", new object[1] { SqlSmoObject.MakeSqlString(s) });
		object propValueOptional = GetPropValueOptional("MethodXsdSchemaOption");
		if (propValueOptional != null && (!sp.ScriptForAlter || base.Properties.Get("MethodXsdSchemaOption").Dirty))
		{
			string text2 = string.Empty;
			switch ((MethodXsdSchemaOption)propValueOptional)
			{
			case MethodXsdSchemaOption.None:
				text2 = "NONE";
				break;
			case MethodXsdSchemaOption.Standard:
				text2 = "STANDARD";
				break;
			case MethodXsdSchemaOption.Default:
				text2 = "DEFAULT";
				break;
			}
			if (text2.Length > 0)
			{
				sb.Append(Globals.newline);
				sb.AppendFormat(SmoApplication.DefaultCulture, ", SCHEMA={0}", new object[1] { text2 });
			}
		}
		object propValueOptional2 = GetPropValueOptional("ResultFormat");
		if (propValueOptional2 != null && (!sp.ScriptForAlter || base.Properties.Get("ResultFormat").Dirty))
		{
			string text3 = string.Empty;
			switch ((ResultFormat)propValueOptional2)
			{
			case ResultFormat.AllResults:
				text3 = "ALL_RESULTS";
				break;
			case ResultFormat.RowSets:
				text3 = "ROWSETS_ONLY";
				break;
			}
			if (text3.Length > 0)
			{
				sb.Append(Globals.newline);
				sb.AppendFormat(SmoApplication.DefaultCulture, ", FORMAT={0}", new object[1] { text3 });
			}
		}
		sb.Append(Globals.RParen);
	}
}
