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

[PhysicalFacet]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public sealed class UserDefinedAggregate : ScriptSchemaObjectBase, ISfcSupportsDesignMode, IObjectPermission, ICreatable, IDroppable, IDropIfExists, IExtendedProperties, IScriptable, IAlterable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 16, 18, 18, 18, 18, 18, 18, 18 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 17 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[17]
		{
			new StaticMetadata("AssemblyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ClassName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("UserType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("XmlDocumentConstraint", expensive: false, readOnly: false, typeof(XmlDocumentConstraint)),
			new StaticMetadata("XmlSchemaNamespace", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("XmlSchemaNamespaceSchema", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[18]
		{
			new StaticMetadata("AssemblyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ClassName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("XmlDocumentConstraint", expensive: false, readOnly: false, typeof(XmlDocumentConstraint)),
			new StaticMetadata("XmlSchemaNamespace", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("XmlSchemaNamespaceSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
			new StaticMetadata("UserType", expensive: false, readOnly: true, typeof(string))
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
					"AssemblyName" => 0, 
					"ClassName" => 1, 
					"CreateDate" => 2, 
					"DataType" => 3, 
					"DataTypeSchema" => 4, 
					"DateLastModified" => 5, 
					"ID" => 6, 
					"IsSchemaOwned" => 7, 
					"Length" => 8, 
					"NumericPrecision" => 9, 
					"NumericScale" => 10, 
					"Owner" => 11, 
					"SystemType" => 12, 
					"UserType" => 13, 
					"XmlDocumentConstraint" => 14, 
					"XmlSchemaNamespace" => 15, 
					"XmlSchemaNamespaceSchema" => 16, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"AssemblyName" => 0, 
				"ClassName" => 1, 
				"CreateDate" => 2, 
				"DataType" => 3, 
				"DataTypeSchema" => 4, 
				"DateLastModified" => 5, 
				"ID" => 6, 
				"IsSchemaOwned" => 7, 
				"Length" => 8, 
				"NumericPrecision" => 9, 
				"NumericScale" => 10, 
				"Owner" => 11, 
				"SystemType" => 12, 
				"XmlDocumentConstraint" => 13, 
				"XmlSchemaNamespace" => 14, 
				"XmlSchemaNamespaceSchema" => 15, 
				"PolicyHealthState" => 16, 
				"UserType" => 17, 
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

	private UserDefinedAggregateParameterCollection parameters;

	private DataType dataType;

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

	internal override UserPermissionCollection Permissions => GetUserPermissions();

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[CLSCompliant(false)]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[SfcReference(typeof(SqlAssembly), "Server[@Name = '{0}']/Database[@Name = '{1}']/SqlAssembly[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "AssemblyName" })]
	public string AssemblyName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("AssemblyName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AssemblyName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ClassName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ClassName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ClassName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsSchemaOwned => (bool)base.Properties.GetValueWithNullReplacement("IsSchemaOwned");

	[CLSCompliant(false)]
	[SfcReference(typeof(ApplicationRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/ApplicationRole[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcReference(typeof(DatabaseRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/Role[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	public string Owner
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Owner");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Owner", value);
		}
	}

	public static string UrnSuffix => "UserDefinedAggregate";

	[CLSCompliant(false)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[SfcReference(typeof(Schema), typeof(SchemaCustomResolver), "Resolve", new string[] { })]
	[SfcKey(0)]
	public override string Schema
	{
		get
		{
			return base.Schema;
		}
		set
		{
			base.Schema = value;
		}
	}

	[SfcKey(1)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.OneToAny, typeof(UserDefinedAggregateParameter))]
	public UserDefinedAggregateParameterCollection Parameters
	{
		get
		{
			CheckObjectState();
			if (parameters == null)
			{
				parameters = new UserDefinedAggregateParameterCollection(this);
			}
			return parameters;
		}
	}

	[SfcReference(typeof(UserDefinedDataType), typeof(UserDefinedDataTypeResolver), "Resolve", new string[] { })]
	[CLSCompliant(false)]
	[SfcReference(typeof(UserDefinedType), typeof(UserDefinedTypeResolver), "Resolve", new string[] { })]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DataType DataType
	{
		get
		{
			return GetDataType(ref dataType);
		}
		set
		{
			SetDataType(ref dataType, value);
		}
	}

	public UserDefinedAggregate()
	{
	}

	public UserDefinedAggregate(Database database, string name)
	{
		ValidateName(name);
		key = new SchemaObjectKey(name, null);
		Parent = database;
	}

	public UserDefinedAggregate(Database database, string name, string schema)
	{
		ValidateName(name);
		key = new SchemaObjectKey(name, schema);
		Parent = database;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	public void Deny(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string[] granteeNames, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, null, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, revokeGrant, cascade, asRole);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, revokeGrant, cascade, asRole);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions()
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, null, null);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, granteeName, null);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, null, permissions);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName, ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, granteeName, permissions);
	}

	private void init()
	{
		parameters = null;
	}

	internal UserDefinedAggregate(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		init();
	}

	public void ChangeSchema(string newSchema)
	{
		CheckObjectState();
		ChangeSchema(newSchema, bCheckExisting: true);
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		return new PropagateInfo[1]
		{
			new PropagateInfo((base.ServerVersion.Major < 8) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
		};
	}

	private void AddParam(StringBuilder sb, ScriptingPreferences sp, UserDefinedAggregateParameter spp)
	{
		StringCollection stringCollection = new StringCollection();
		spp.UseOutput = false;
		spp.UseDefault = false;
		spp.ScriptDdlInternal(stringCollection, sp);
		sb.Append(stringCollection[0]);
		stringCollection.Clear();
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		if (sp.IncludeScripts.Owner)
		{
			ScriptChangeOwner(alterQuery, sp);
		}
	}

	public void Create()
	{
		CreateImpl();
		SetSchemaOwned();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = FormatFullNameForScripting(sp);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		StringBuilder stringBuilder2 = new StringBuilder(1024);
		stringBuilder2.AppendFormat("CREATE AGGREGATE {0}", text);
		stringBuilder2.Append(sp.NewLine);
		if (Parameters.Count < 1)
		{
			throw new SmoException(ExceptionTemplatesImpl.MustSpecifyOneParameter);
		}
		stringBuilder2.Append(Globals.LParen);
		AddParam(stringBuilder2, sp, parameters[0]);
		for (int i = 1; i < Parameters.Count; i++)
		{
			stringBuilder2.Append(Globals.comma);
			stringBuilder2.Append(Globals.space);
			AddParam(stringBuilder2, sp, parameters[i]);
		}
		stringBuilder2.Append(Globals.RParen);
		stringBuilder2.Append(sp.NewLine);
		stringBuilder2.Append("RETURNS");
		UserDefinedDataType.AppendScriptTypeDefinition(stringBuilder2, sp, this, DataType.SqlDataType);
		stringBuilder2.Append(sp.NewLine);
		stringBuilder2.Append("EXTERNAL NAME ");
		string text2 = (string)GetPropValue("AssemblyName");
		if (string.Empty == text2)
		{
			throw new PropertyNotSetException("AssemblyName");
		}
		stringBuilder2.AppendFormat("[{0}]", SqlSmoObject.SqlBraket(text2));
		Property property = base.Properties.Get("ClassName");
		if (property.Value != null && ((string)property.Value).Length > 0)
		{
			stringBuilder2.AppendFormat(".[{0}]", SqlSmoObject.SqlBraket((string)property.Value));
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_UDA, new object[2]
			{
				"NOT",
				SqlSmoObject.SqlString(text)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append("EXEC dbo.sp_executesql @statement =");
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append("N'");
			stringBuilder.Append(SqlSmoObject.SqlString(stringBuilder2.ToString()));
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append("'");
		}
		else
		{
			stringBuilder.Append(stringBuilder2.ToString());
		}
		queries.Add(stringBuilder.ToString());
		if (sp.IncludeScripts.Owner)
		{
			ScriptOwner(queries, sp);
		}
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = FormatFullNameForScripting(sp);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal < SqlServerVersionInternal.Version130)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_UDA, new object[2]
			{
				"",
				SqlSmoObject.SqlString(text)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat("DROP AGGREGATE {0}{1}", (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty, text);
		stringBuilder.Append(sp.NewLine);
		dropQuery.Add(stringBuilder.ToString());
	}

	public override void Refresh()
	{
		base.Refresh();
		dataType = null;
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
		string[] fields = new string[13]
		{
			"DataTypeSchema", "SystemType", "Length", "NumericPrecision", "NumericScale", "XmlSchemaNamespace", "XmlSchemaNamespaceSchema", "XmlDocumentConstraint", "AssemblyName", "ClassName",
			"ID", "Owner", "IsSchemaOwned"
		};
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		supportedScriptFields.Add("DataType");
		return supportedScriptFields.ToArray();
	}
}
