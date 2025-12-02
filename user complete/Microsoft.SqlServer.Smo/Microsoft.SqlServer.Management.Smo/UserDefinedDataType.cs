using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
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
public sealed class UserDefinedDataType : ScriptSchemaObjectBase, ISfcSupportsDesignMode, IObjectPermission, ICreatable, IAlterable, IDroppable, IDropIfExists, IRenamable, IExtendedProperties, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 14, 15, 16, 17, 17, 17, 17, 17, 17, 17 };

		private static int[] cloudVersionCount = new int[3] { 16, 16, 16 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[16]
		{
			new StaticMetadata("AllowIdentity", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Collation", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Default", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DefaultSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("MaxLength", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("Nullable", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Rule", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("RuleSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("SystemType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("VariableLength", expensive: false, readOnly: true, typeof(bool))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[17]
		{
			new StaticMetadata("AllowIdentity", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Default", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DefaultSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("MaxLength", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("Nullable", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Rule", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("RuleSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("SystemType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("VariableLength", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Collation", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState))
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
					"AllowIdentity" => 0, 
					"Collation" => 1, 
					"Default" => 2, 
					"DefaultSchema" => 3, 
					"ID" => 4, 
					"IsSchemaOwned" => 5, 
					"Length" => 6, 
					"MaxLength" => 7, 
					"Nullable" => 8, 
					"NumericPrecision" => 9, 
					"NumericScale" => 10, 
					"Owner" => 11, 
					"Rule" => 12, 
					"RuleSchema" => 13, 
					"SystemType" => 14, 
					"VariableLength" => 15, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"AllowIdentity" => 0, 
				"Default" => 1, 
				"DefaultSchema" => 2, 
				"ID" => 3, 
				"Length" => 4, 
				"MaxLength" => 5, 
				"Nullable" => 6, 
				"NumericPrecision" => 7, 
				"NumericScale" => 8, 
				"Owner" => 9, 
				"Rule" => 10, 
				"RuleSchema" => 11, 
				"SystemType" => 12, 
				"VariableLength" => 13, 
				"Collation" => 14, 
				"IsSchemaOwned" => 15, 
				"PolicyHealthState" => 16, 
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

	private const string AddUddt = "EXEC dbo.sp_addtype @typename={0}, @phystype='{1}'";

	private const string AddUddtNullOption = ", @nulltype='{0}'";

	private const string AddUddtOwnerOption80 = ", @owner=N'{0}'";

	private const string IfUddtNotExists = "IF NOT EXISTS (SELECT * FROM dbo.systypes WHERE name = N'{0}')\r\nBEGIN\r\n{1}\r\nEND\r\n";

	[SfcObject(SfcObjectRelationship.ParentObject, SfcObjectFlags.Design)]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool AllowIdentity => (bool)base.Properties.GetValueWithNullReplacement("AllowIdentity");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Collation => (string)base.Properties.GetValueWithNullReplacement("Collation");

	[SfcReference(typeof(Default), "Server[@Name = '{0}']/Database[@Name = '{1}']/Default[@Name='{2}' and @Schema='{3}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Default", "DefaultSchema" })]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[CLSCompliant(false)]
	public string Default
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Default");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Default", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase, "dbo")]
	public string DefaultSchema
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DefaultSchema");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DefaultSchema", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsSchemaOwned => (bool)base.Properties.GetValueWithNullReplacement("IsSchemaOwned");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public int Length
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("Length");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Length", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public short MaxLength => (short)base.Properties.GetValueWithNullReplacement("MaxLength");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool Nullable
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Nullable");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Nullable", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public int NumericPrecision
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("NumericPrecision");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NumericPrecision", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public int NumericScale
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("NumericScale");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NumericScale", value);
		}
	}

	[SfcReference(typeof(ApplicationRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/ApplicationRole[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[CLSCompliant(false)]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcReference(typeof(DatabaseRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/Role[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
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

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[SfcReference(typeof(Rule), "Server[@Name = '{0}']/Database[@Name = '{1}']/Rule[@Name='{2}' and @Schema='{3}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Rule", "RuleSchema" })]
	[CLSCompliant(false)]
	public string Rule
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Rule");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Rule", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase, "dbo")]
	public string RuleSchema
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("RuleSchema");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RuleSchema", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string SystemType
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("SystemType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SystemType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool VariableLength => (bool)base.Properties.GetValueWithNullReplacement("VariableLength");

	public static string UrnSuffix => "UserDefinedDataType";

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcKey(1)]
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

	[SfcKey(0)]
	[SfcReference(typeof(Schema), typeof(SchemaCustomResolver), "Resolve", new string[] { })]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[CLSCompliant(false)]
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

	public UserDefinedDataType()
	{
	}

	public UserDefinedDataType(Database database, string name)
	{
		ValidateName(name);
		key = new SchemaObjectKey(name, null);
		Parent = database;
	}

	public UserDefinedDataType(Database database, string name, string schema)
	{
		ValidateName(name);
		key = new SchemaObjectKey(name, schema);
		Parent = database;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[9] { "Default", "DefaultSchema", "Length", "Nullable", "NumericPrecision", "NumericScale", "Rule", "RuleSchema", "SystemType" };
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		return propname switch
		{
			"DefaultSchema" => "dbo", 
			"RuleSchema" => "dbo", 
			_ => base.GetPropertyDefaultValue(propname), 
		};
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

	internal UserDefinedDataType(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		CreateImpl();
		SetSchemaOwned();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			if (base.State == SqlSmoState.Existing)
			{
				InitializeKeepDirtyValues();
			}
			string name = FormatFullNameForScripting(sp);
			if (sp.IncludeScripts.Header)
			{
				stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, name, DateTime.Now.ToString(GetDbCulture())));
				stringBuilder.Append(sp.NewLine);
			}
			if (sp.IncludeScripts.ExistenceCheck)
			{
				AddExistsCheck(stringBuilder, "NOT", sp);
			}
			ScriptDdlGreaterEqual9(stringBuilder, sp);
			queries.Add(stringBuilder.ToString());
			AddBindings(queries, sp);
			if (sp.IncludeScripts.Owner)
			{
				ScriptOwner(queries, sp);
			}
			return;
		}
		string name2 = Name;
		string empty = string.Empty;
		string text = "NULL";
		empty = GetTypeDefinitionScript(sp, this, "SystemType", bSquareBraketsForNative: false);
		stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_addtype @typename={0}, @phystype='{1}'", new object[2]
		{
			FormatFullNameForScripting(sp, nameIsIndentifier: false),
			empty
		}));
		text = ((base.State == SqlSmoState.Creating || Nullable) ? "NULL" : "NOT NULL");
		stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, ", @nulltype='{0}'", new object[1] { text }));
		if (!string.IsNullOrEmpty(Schema) && sp.IncludeScripts.Owner)
		{
			stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, ", @owner=N'{0}'", new object[1] { Schema }));
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			queries.Add(string.Format(SmoApplication.DefaultCulture, "IF NOT EXISTS (SELECT * FROM dbo.systypes WHERE name = N'{0}')\r\nBEGIN\r\n{1}\r\nEND\r\n", new object[2]
			{
				name2,
				stringBuilder.ToString()
			}));
		}
		else
		{
			queries.Add(stringBuilder.ToString());
		}
	}

	internal override void ScriptOwnerForShiloh(StringBuilder sb, ScriptingPreferences sp, string newOwner)
	{
		if (base.ServerVersion.Major > 8)
		{
			throw new UnsupportedFeatureException(ExceptionTemplatesImpl.PropertyCannotBeSetForVersion("Owner", "User Defined Data Type", SqlSmoObject.GetSqlServerName(sp)));
		}
	}

	private void ScriptDdlGreaterEqual9(StringBuilder sb, ScriptingPreferences sp)
	{
		sb.AppendFormat("CREATE TYPE {0} FROM ", FormatFullNameForScripting(sp));
		sb.Append(GetTypeDefinitionScript(sp, this, "SystemType", bSquareBraketsForNative: true));
		object value = base.Properties.Get("Nullable").Value;
		if (value != null)
		{
			if ((bool)value)
			{
				sb.Append(" NULL");
			}
			else
			{
				sb.Append(" NOT NULL");
			}
		}
	}

	private void ScriptDdlLess9(StringBuilder sb, ScriptingPreferences sp)
	{
		sb.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_addtype {0}, ", new object[1] { FormatFullNameForScripting(sp, nameIsIndentifier: false) });
		sb.AppendFormat("N'{0}'", SqlSmoObject.SqlString(GetTypeDefinitionScript(sp, this, "SystemType", bSquareBraketsForNative: false)));
		object propValueOptional = GetPropValueOptional("Nullable");
		if (propValueOptional != null)
		{
			if ((bool)propValueOptional)
			{
				sb.Append(",N'null'");
			}
			else
			{
				sb.Append(",N'not null'");
			}
		}
		else
		{
			sb.Append(",null ");
		}
		if (base.StringComparer.Compare("dbo", Schema) != 0)
		{
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.TypeSchemaMustBeDbo("Schema", Schema));
		}
		sb.Append(sp.NewLine);
	}

	internal void AddBindings(StringCollection queries, ScriptingPreferences sp)
	{
		if (SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType) || (!sp.OldOptions.Bindings && !sp.ScriptForCreateDrop))
		{
			return;
		}
		object value = base.Properties.Get("Default").Value;
		if (value != null && string.Empty != (string)value)
		{
			object value2 = base.Properties.Get("DefaultSchema").Value;
			if (sp.IncludeScripts.SchemaQualify)
			{
				queries.Add(GetBindDefaultScript(sp, (string)value2, (string)value, futureOnly: true));
			}
			else
			{
				queries.Add(GetBindDefaultScript(sp, null, (string)value, futureOnly: true));
			}
		}
		object value3 = base.Properties.Get("Rule").Value;
		if (value3 != null && string.Empty != (string)value3)
		{
			object value4 = base.Properties.Get("RuleSchema").Value;
			if (sp.IncludeScripts.SchemaQualify)
			{
				queries.Add(GetBindRuleScript(sp, (string)value4, (string)value3, futureOnly: true));
			}
			else
			{
				queries.Add(GetBindRuleScript(sp, null, (string)value3, futureOnly: true));
			}
		}
	}

	private static string GetTypeDefinitionScript(ScriptingPreferences sp, SqlSmoObject oObj, string sTypeNameProperty, bool bSquareBraketsForNative)
	{
		StringBuilder stringBuilder = new StringBuilder();
		PropertyCollection propertyCollection = oObj.Properties;
		if (oObj.State == SqlSmoState.Creating && oObj.Properties.Get(sTypeNameProperty).Value == null)
		{
			throw new PropertyNotSetException(sTypeNameProperty);
		}
		string text = (string)propertyCollection[sTypeNameProperty].Value;
		if (sp.DataType.TimestampToBinary && oObj.StringComparer.Compare("timestamp", text) == 0)
		{
			if (bSquareBraketsForNative)
			{
				stringBuilder.Append("[binary](8)");
			}
			else
			{
				stringBuilder.Append("binary(8)");
			}
		}
		else if (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version70 && oObj.StringComparer.Compare("bigint", text) == 0)
		{
			if (bSquareBraketsForNative)
			{
				stringBuilder.Append("[int]");
			}
			else
			{
				stringBuilder.Append("int");
			}
		}
		else
		{
			if (bSquareBraketsForNative)
			{
				stringBuilder.AppendFormat("[{0}]", text);
			}
			else
			{
				stringBuilder.AppendFormat("{0}", text);
			}
			SqlDataType sqlDataType = DataType.SqlToEnum(text);
			if (!DataType.IsDataTypeSupportedOnTargetVersion(sqlDataType, sp.TargetServerVersion))
			{
				SqlSmoObject.ThrowIfBelowVersion100(sp.TargetServerVersionInternal);
			}
			if (!DataType.IsDataTypeSupportedOnCloud(sqlDataType))
			{
				SqlSmoObject.ThrowIfCloud(sp.TargetDatabaseEngineType, string.Format(CultureInfo.CurrentCulture, ExceptionTemplatesImpl.PropertyNotSupportedOnCloud(sqlDataType.ToString())));
			}
			if (string.Compare(text, "xml", StringComparison.OrdinalIgnoreCase) == 0)
			{
				SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal);
				if (sp.DataType.XmlNamespaces && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90 && oObj.ServerVersion.Major >= 9 && oObj.Properties.Get("XmlSchemaNamespace").Value is string { Length: >0 } text2)
				{
					stringBuilder.Append("(");
					switch (oObj.GetPropValueOptional("XmlDocumentConstraint", XmlDocumentConstraint.Default))
					{
					case XmlDocumentConstraint.Content:
						stringBuilder.Append("CONTENT ");
						break;
					case XmlDocumentConstraint.Document:
						stringBuilder.Append("DOCUMENT ");
						break;
					default:
						throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration("XmlDocumentConstraint"));
					case XmlDocumentConstraint.Default:
						break;
					}
					if (oObj.Properties.Get("XmlSchemaNamespaceSchema").Value is string { Length: >0 } text3 && sp.IncludeScripts.SchemaQualify)
					{
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}.", new object[1] { SqlSmoObject.MakeSqlBraket(text3) });
					}
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0})", new object[1] { SqlSmoObject.MakeSqlBraket(text2) });
				}
			}
			else if (TypeAllowsLength(text, oObj.StringComparer))
			{
				object value = propertyCollection.Get("Length").Value;
				if (value == null)
				{
					throw new PropertyNotSetException("Length");
				}
				if ((int)value != 0)
				{
					switch (text)
					{
					case "varchar":
					case "nvarchar":
					case "varbinary":
						if ((int)value < 0)
						{
							stringBuilder.Append("(max)");
							break;
						}
						goto default;
					default:
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "({0})", new object[1] { value });
						break;
					}
				}
			}
			else if (TypeAllowsPrecisionScale(text, oObj.StringComparer))
			{
				object value2 = propertyCollection.Get("NumericPrecision").Value;
				if (value2 != null)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "({0}", new object[1] { value2 });
					object value3 = propertyCollection.Get("NumericScale").Value;
					if (value3 != null)
					{
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", {0}", new object[1] { value3 });
					}
					stringBuilder.Append(")");
				}
			}
			else if (TypeAllowsScale(text, oObj.StringComparer))
			{
				object value4 = propertyCollection.Get("NumericScale").Value;
				if (value4 != null)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "({0})", new object[1] { value4 });
				}
			}
			else if (DataType.IsTypeFloatStateCreating(text, oObj))
			{
				object value5 = propertyCollection.Get("NumericPrecision").Value;
				if (value5 != null)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "({0})", new object[1] { value5 });
				}
			}
		}
		return stringBuilder.ToString();
	}

	internal static void AppendScriptTypeDefinition(StringBuilder sb, ScriptingPreferences sp, SqlSmoObject oObj, SqlDataType sqlDataType)
	{
		if (sqlDataType == SqlDataType.UserDefinedType)
		{
			SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal);
		}
		if (!DataType.IsDataTypeSupportedOnCloud(sqlDataType))
		{
			SqlSmoObject.ThrowIfCloud(sp.TargetDatabaseEngineType, string.Format(CultureInfo.CurrentCulture, ExceptionTemplatesImpl.PropertyNotSupportedOnCloud(sqlDataType.ToString())));
		}
		if (oObj.State == SqlSmoState.Creating && oObj.Properties.Get("DataType").Value == null)
		{
			throw new PropertyNotSetException("DataType");
		}
		string text = (string)oObj.Properties["DataType"].Value;
		if (text == null || text.Length == 0)
		{
			throw new PropertyNotSetException("DataType");
		}
		if (IsSystemType(oObj, sp))
		{
			sb.Append(GetTypeDefinitionScript(sp, oObj, "DataType", bSquareBraketsForNative: true));
			return;
		}
		string text2 = (string)oObj.Properties.Get("DataTypeSchema").Value;
		string text3 = oObj.GetPropValueOptional("SystemType") as string;
		if (sp.DataType.UserDefinedDataTypesToBaseType)
		{
			if (text3 == null)
			{
				Database database = oObj.GetServerObject().Databases[oObj.GetDBName()];
				UserDefinedDataType userDefinedDataType = ((text2 != null && text2.Length != 0) ? database.UserDefinedDataTypes[text, text2] : database.UserDefinedDataTypes[text]);
				if (userDefinedDataType != null)
				{
					oObj.Properties.Get("SystemType").SetValue(userDefinedDataType.GetPropValueOptional("SystemType") as string);
					oObj.Properties.Get("SystemType").SetRetrieved(retrieved: true);
					text3 = (string)oObj.Properties.Get("SystemType").Value;
				}
			}
			else if (text3 != null && text3.Length > 0)
			{
				sb.Append(GetTypeDefinitionScript(sp, oObj, "SystemType", bSquareBraketsForNative: true));
				return;
			}
		}
		if (SqlServerVersionInternal.Version80 == sp.TargetServerVersionInternal || sp.TargetServerVersionInternal == SqlServerVersionInternal.Version70 || oObj.ServerVersion.Major < 9 || text2 == null)
		{
			sb.AppendFormat("[{0}]", SqlSmoObject.SqlBraket(text));
		}
		else
		{
			sb.AppendFormat("[{0}].[{1}]", SqlSmoObject.SqlBraket(text2), SqlSmoObject.SqlBraket(text));
		}
	}

	internal static bool IsSystemType(SqlSmoObject oObj, ScriptingPreferences sp)
	{
		string sqlTypeName = oObj.Properties.Get("DataType").Value as string;
		return DataType.IsSystemDataType(DataType.SqlToEnum(sqlTypeName), sp.TargetServerVersion);
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
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string name = FormatFullNameForScripting(sp);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, name, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal < SqlServerVersionInternal.Version130)
		{
			AddExistsCheck(stringBuilder, "", sp);
		}
		if (SqlServerVersionInternal.Version90 > sp.TargetServerVersionInternal)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_droptype @typename={0}", new object[1] { FormatFullNameForScripting(sp, nameIsIndentifier: false) });
		}
		else
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP TYPE {0}{1}", new object[2]
			{
				(sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty,
				FormatFullNameForScripting(sp)
			});
		}
		queries.Add(stringBuilder.ToString());
	}

	private void AddExistsCheck(StringBuilder sb, string prefix, ScriptingPreferences sp)
	{
		if (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90 || !sp.IncludeScripts.SchemaQualify)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_UDDT80, new object[2]
			{
				prefix,
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
		}
		else
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_UDDT90, new object[3]
			{
				prefix,
				SqlSmoObject.SqlString((ScriptName.Length > 0) ? ScriptName : Name),
				SqlSmoObject.SqlString((ScriptSchema.Length > 0) ? ScriptSchema : Schema)
			});
		}
		sb.Append(sp.NewLine);
	}

	public void Alter()
	{
		AlterImpl();
		SetSchemaOwned();
	}

	public void BindRule(string ruleSchema, string ruleName)
	{
		BindRule(ruleSchema, ruleName, bindColumns: false);
	}

	public void BindRule(string ruleSchema, string ruleName, bool bindColumns)
	{
		CheckObjectState(throwIfNotCreated: true);
		BindRuleImpl(ruleSchema, ruleName, bindColumns);
	}

	public void UnbindRule()
	{
		UnbindRule(bindColumns: false);
	}

	public void UnbindRule(bool bindColumns)
	{
		CheckObjectState(throwIfNotCreated: true);
		UnbindRuleImpl(bindColumns);
	}

	public void BindDefault(string defaultSchema, string defaultName)
	{
		BindDefault(defaultSchema, defaultName, bindColumns: false);
	}

	public void BindDefault(string defaultSchema, string defaultName, bool bindColumns)
	{
		CheckObjectState(throwIfNotCreated: true);
		BindDefaultImpl(defaultSchema, defaultName, bindColumns);
	}

	public void UnbindDefault()
	{
		UnbindDefault(bindColumns: false);
	}

	public void UnbindDefault(bool bindColumns)
	{
		CheckObjectState(throwIfNotCreated: true);
		UnbindDefaultImpl(bindColumns);
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		if (sp.IncludeScripts.Owner)
		{
			ScriptOwner(alterQuery, sp);
		}
	}

	protected override void PostCreate()
	{
		if (!base.IsDesignMode)
		{
			return;
		}
		string x = (string)base.Properties["SystemType"].Value;
		if (base.StringComparer.Compare(x, "nvarchar") == 0 || base.StringComparer.Compare(x, "varchar") == 0 || base.StringComparer.Compare(x, "varbinary") == 0)
		{
			int num = Length;
			if (Length < 0)
			{
				num = -1;
				Length = -1;
			}
			else if (base.StringComparer.Compare(x, "nvarchar") == 0)
			{
				num = 2 * num;
			}
			int index = base.Properties.LookupID("MaxLength", PropertyAccessPurpose.Write);
			base.Properties.SetValue(index, (short)num);
			base.Properties.SetRetrieved(index, val: true);
		}
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		if (DatabaseEngineType.SqlAzureDatabase == DatabaseEngineType)
		{
			return null;
		}
		return new PropagateInfo[1]
		{
			new PropagateInfo((base.ServerVersion.Major < 8) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
		};
	}

	public SqlSmoObject[] EnumBoundColumns()
	{
		try
		{
			CheckObjectState();
			DataTable enumeratorData = ExecutionManager.GetEnumeratorData(new Request(string.Concat(base.Urn, "/BoundColumn")));
			SqlSmoObject[] array = new SqlSmoObject[enumeratorData.Rows.Count];
			Urn urn = base.Urn;
			int num = 0;
			foreach (DataRow row in enumeratorData.Rows)
			{
				string text = string.Format(SmoApplication.DefaultCulture, "{0}/Table[@Name='{1}' and @Schema='{2}']/Column[@Name='{3}']", urn.Parent, Urn.EscapeString((string)row["ObjectName"]), Urn.EscapeString((string)row["ObjectSchema"]), Urn.EscapeString((string)row["Name"]));
				array[num++] = GetServerObject().GetSmoObject(text);
			}
			return array;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumBoundColumns, this, ex);
		}
	}

	internal static bool TypeAllowsLength(string type, StringComparer comparer)
	{
		if (comparer.Compare(type, "nvarchar") == 0 || comparer.Compare(type, "varchar") == 0 || comparer.Compare(type, "binary") == 0 || comparer.Compare(type, "varbinary") == 0 || comparer.Compare(type, "nchar") == 0 || comparer.Compare(type, "char") == 0)
		{
			return true;
		}
		return false;
	}

	internal static bool TypeAllowsCollation(string type, StringComparer comparer)
	{
		if (comparer.Compare(type, "nvarchar") == 0 || comparer.Compare(type, "varchar") == 0 || comparer.Compare(type, "nchar") == 0 || comparer.Compare(type, "text") == 0 || comparer.Compare(type, "ntext") == 0 || comparer.Compare(type, "char") == 0 || comparer.Compare(type, "sysname") == 0)
		{
			return true;
		}
		return false;
	}

	internal static bool TypeAllowsPrecisionScale(string type, StringComparer comparer)
	{
		if (comparer.Compare(type, "numeric") == 0 || comparer.Compare(type, "decimal") == 0)
		{
			return true;
		}
		return false;
	}

	internal static bool TypeAllowsScale(string type, StringComparer comparer)
	{
		if (comparer.Compare(type, "time") == 0 || comparer.Compare(type, "datetimeoffset") == 0 || comparer.Compare(type, "datetime2") == 0)
		{
			return true;
		}
		return false;
	}

	public void Rename(string newname)
	{
		RenameImpl(newname);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "EXEC {0}.dbo.sp_rename @objname = N'{1}', @newname = N'{2}', @objtype = N'USERDATATYPE'", new object[3]
		{
			SqlSmoObject.MakeSqlBraket(Parent.Name),
			SqlSmoObject.SqlString(FullQualifiedName),
			SqlSmoObject.SqlString(newName)
		}));
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	internal override void AddScriptPermission(StringCollection query, ScriptingPreferences sp)
	{
		if (sp.TargetServerVersionInternal != SqlServerVersionInternal.Version80 && base.ServerVersion.Major != 8)
		{
			base.AddScriptPermission(query, sp);
		}
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[13]
		{
			"Default", "DefaultSchema", "Rule", "RuleSchema", "Nullable", "Length", "MaxLength", "NumericPrecision", "NumericScale", "SystemType",
			"ID", "Owner", "IsSchemaOwned"
		};
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}
