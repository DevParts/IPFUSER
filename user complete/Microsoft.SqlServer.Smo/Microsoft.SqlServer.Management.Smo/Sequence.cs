using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[StateChangeEvent("CREATE_SEQUENCE", "SEQUENCE")]
[StateChangeEvent("ALTER_SEQUENCE", "SEQUENCE")]
[StateChangeEvent("RENAME", "SEQUENCE")]
[StateChangeEvent("ALTER_AUTHORIZATION_DATABASE", "SEQUENCE")]
[StateChangeEvent("ALTER_SCHEMA", "SEQUENCE")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[PhysicalFacet]
public sealed class Sequence : ScriptSchemaObjectBase, IObjectPermission, ICreatable, IDroppable, IDropIfExists, IRenamable, IExtendedProperties, IScriptable, IAlterable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 20, 20, 20, 20, 20 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 19 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[19]
		{
			new StaticMetadata("CacheSize", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("CurrentValue", expensive: false, readOnly: true, typeof(object)),
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IncrementValue", expensive: false, readOnly: false, typeof(object)),
			new StaticMetadata("IsCycleEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsExhausted", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("MaxValue", expensive: false, readOnly: false, typeof(object)),
			new StaticMetadata("MinValue", expensive: false, readOnly: false, typeof(object)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("SequenceCacheType", expensive: false, readOnly: false, typeof(SequenceCacheType)),
			new StaticMetadata("StartValue", expensive: false, readOnly: false, typeof(object)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[20]
		{
			new StaticMetadata("CacheSize", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("CurrentValue", expensive: false, readOnly: true, typeof(object)),
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IncrementValue", expensive: false, readOnly: false, typeof(object)),
			new StaticMetadata("IsCycleEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsExhausted", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("MaxValue", expensive: false, readOnly: false, typeof(object)),
			new StaticMetadata("MinValue", expensive: false, readOnly: false, typeof(object)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
			new StaticMetadata("SequenceCacheType", expensive: false, readOnly: false, typeof(SequenceCacheType)),
			new StaticMetadata("StartValue", expensive: false, readOnly: false, typeof(object)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string))
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
					"CacheSize" => 0, 
					"CreateDate" => 1, 
					"CurrentValue" => 2, 
					"DataType" => 3, 
					"DataTypeSchema" => 4, 
					"DateLastModified" => 5, 
					"ID" => 6, 
					"IncrementValue" => 7, 
					"IsCycleEnabled" => 8, 
					"IsExhausted" => 9, 
					"IsSchemaOwned" => 10, 
					"MaxValue" => 11, 
					"MinValue" => 12, 
					"NumericPrecision" => 13, 
					"NumericScale" => 14, 
					"Owner" => 15, 
					"SequenceCacheType" => 16, 
					"StartValue" => 17, 
					"SystemType" => 18, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"CacheSize" => 0, 
				"CreateDate" => 1, 
				"CurrentValue" => 2, 
				"DataType" => 3, 
				"DataTypeSchema" => 4, 
				"DateLastModified" => 5, 
				"ID" => 6, 
				"IncrementValue" => 7, 
				"IsCycleEnabled" => 8, 
				"IsExhausted" => 9, 
				"IsSchemaOwned" => 10, 
				"MaxValue" => 11, 
				"MinValue" => 12, 
				"NumericPrecision" => 13, 
				"NumericScale" => 14, 
				"Owner" => 15, 
				"PolicyHealthState" => 16, 
				"SequenceCacheType" => 17, 
				"StartValue" => 18, 
				"SystemType" => 19, 
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

	private SequenceEvents events;

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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int CacheSize
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("CacheSize");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("CacheSize", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public object CurrentValue => base.Properties.GetValueWithNullReplacement("CurrentValue");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public object IncrementValue
	{
		get
		{
			return base.Properties.GetValueWithNullReplacement("IncrementValue");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IncrementValue", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsCycleEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsCycleEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsCycleEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsExhausted => (bool)base.Properties.GetValueWithNullReplacement("IsExhausted");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsSchemaOwned => (bool)base.Properties.GetValueWithNullReplacement("IsSchemaOwned");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public object MaxValue
	{
		get
		{
			return base.Properties.GetValueWithNullReplacement("MaxValue");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaxValue", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public object MinValue
	{
		get
		{
			return base.Properties.GetValueWithNullReplacement("MinValue");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MinValue", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public SequenceCacheType SequenceCacheType
	{
		get
		{
			return (SequenceCacheType)base.Properties.GetValueWithNullReplacement("SequenceCacheType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SequenceCacheType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public object StartValue
	{
		get
		{
			return base.Properties.GetValueWithNullReplacement("StartValue");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("StartValue", value);
		}
	}

	public SequenceEvents Events
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (SqlContext.IsAvailable)
			{
				throw new SmoException(ExceptionTemplatesImpl.SmoSQLCLRUnAvailable);
			}
			if (events == null)
			{
				events = new SequenceEvents(this);
			}
			return events;
		}
	}

	public static string UrnSuffix => "Sequence";

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(ExtendedProperty))]
	public ExtendedPropertyCollection ExtendedProperties
	{
		get
		{
			this.ThrowIfNotSupported(typeof(Sequence));
			CheckObjectState();
			if (m_ExtendedProperties == null)
			{
				m_ExtendedProperties = new ExtendedPropertyCollection(this);
			}
			return m_ExtendedProperties;
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DataType DataType
	{
		get
		{
			return GetDataType(ref dataType);
		}
		set
		{
			if (value != null && value.SqlDataType == SqlDataType.UserDefinedTableType)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.SetDataType, this, null);
			}
			SetDataType(ref dataType, value);
		}
	}

	public Sequence()
	{
	}

	public Sequence(Database database, string name)
	{
		ValidateName(name);
		key = new SchemaObjectKey(name, null);
		Parent = database;
	}

	public Sequence(Database database, string name, string schema)
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
		return new string[5] { "DataType", "DataTypeSchema", "NumericPrecision", "NumericScale", "SystemType" };
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

	internal Sequence(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
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
		StringBuilder stringBuilder = new StringBuilder();
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader("Sequence", FormatFullNameForScripting(sp), DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.DatabaseContext)
		{
			AddDatabaseContext(dropQuery);
		}
		if (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal < SqlServerVersionInternal.Version130)
		{
			stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_SEQUENCE, new object[3]
			{
				string.Empty,
				FormatFullNameForScripting(sp, nameIsIndentifier: false),
				SqlSmoObject.MakeSqlString(GetSchema(sp))
			}));
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append("DROP SEQUENCE " + ((sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty) + FormatFullNameForScripting(sp));
		dropQuery.Add(stringBuilder.ToString());
	}

	public void Create()
	{
		CreateImpl();
		SetSchemaOwned();
	}

	private bool IsValidDoubleValue(string propertyName, string propertyValue)
	{
		if (double.TryParse(propertyValue, out var _))
		{
			return true;
		}
		throw new WrongPropertyValueException(ExceptionTemplatesImpl.InvalidSequenceValue(propertyName));
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder();
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader("Sequence", FormatFullNameForScripting(sp), DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.DatabaseContext)
		{
			AddDatabaseContext(createQuery);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_SEQUENCE, new object[3]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false),
				SqlSmoObject.MakeSqlString(GetSchema(sp))
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE SEQUENCE {0} ", new object[1] { FormatFullNameForScripting(sp) });
		if (DataType != null && !string.IsNullOrEmpty(dataType.Name))
		{
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append(" AS ");
			UserDefinedDataType.AppendScriptTypeDefinition(stringBuilder, sp, this, DataType.SqlDataType);
		}
		string text = Convert.ToString(GetPropValueOptional("StartValue"), SmoApplication.DefaultCulture);
		if (!string.IsNullOrEmpty(text) && IsValidDoubleValue("StartValue", text))
		{
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append(" START WITH ");
			stringBuilder.Append(text);
		}
		string text2 = Convert.ToString(GetPropValueOptional("IncrementValue"), SmoApplication.DefaultCulture);
		if (!string.IsNullOrEmpty(text2) && IsValidDoubleValue("IncrementValue", text2))
		{
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append(" INCREMENT BY ");
			stringBuilder.Append(text2);
		}
		string text3 = Convert.ToString(GetPropValueOptional("MinValue"), SmoApplication.DefaultCulture);
		if (!string.IsNullOrEmpty(text3) && IsValidDoubleValue("MinValue", text3))
		{
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append(" MINVALUE ");
			stringBuilder.Append(text3);
		}
		string text4 = Convert.ToString(GetPropValueOptional("MaxValue"), SmoApplication.DefaultCulture);
		if (!string.IsNullOrEmpty(text4) && IsValidDoubleValue("MaxValue", text4))
		{
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append(" MAXVALUE ");
			stringBuilder.Append(text4);
		}
		object propValueOptional = GetPropValueOptional("IsCycleEnabled");
		if (propValueOptional != null && (bool)propValueOptional)
		{
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append(" CYCLE ");
		}
		object propValueOptional2 = GetPropValueOptional("SequenceCacheType");
		if (propValueOptional2 != null)
		{
			SequenceCacheType sequenceCacheType = (SequenceCacheType)propValueOptional2;
			if (!Enum.IsDefined(typeof(SequenceCacheType), sequenceCacheType))
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration("SequenceCacheType"));
			}
			switch ((SequenceCacheType)propValueOptional2)
			{
			case SequenceCacheType.DefaultCache:
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append(" CACHE ");
				break;
			case SequenceCacheType.NoCache:
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append(" NO CACHE ");
				break;
			case SequenceCacheType.CacheWithSize:
			{
				int propValueOptional3 = GetPropValueOptional("CacheSize", 0);
				stringBuilder.Append(Globals.newline);
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " CACHE  {0} ", new object[1] { propValueOptional3.ToString(SmoApplication.DefaultCulture) });
				break;
			}
			}
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		createQuery.Add(stringBuilder.ToString());
		if (sp.IncludeScripts.Owner)
		{
			ScriptOwner(createQuery, sp);
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

	public void Alter()
	{
		AlterImpl();
		SetSchemaOwned();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		if (!IsObjectDirty() || base.State == SqlSmoState.Creating)
		{
			return;
		}
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.AppendFormat("ALTER SEQUENCE {0}", FullQualifiedName);
		int length = stringBuilder.Length;
		Property property;
		if ((property = base.Properties.Get("StartValue")).Value != null && property.Dirty)
		{
			stringBuilder.Append(sp.NewLine);
			if (string.IsNullOrEmpty(property.Value.ToString()))
			{
				stringBuilder.Append(" RESTART ");
			}
			else if (IsValidDoubleValue("StartValue", property.Value.ToString()))
			{
				stringBuilder.AppendFormat(" RESTART  WITH {0} ", property.Value.ToString());
			}
		}
		if ((property = base.Properties.Get("IncrementValue")).Value != null && property.Dirty && !string.IsNullOrEmpty(property.Value.ToString()) && IsValidDoubleValue("IncrementValue", property.Value.ToString()))
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.AppendFormat(" INCREMENT BY {0} ", property.Value.ToString());
		}
		if ((property = base.Properties.Get("MinValue")).Value != null && property.Dirty)
		{
			stringBuilder.Append(sp.NewLine);
			if (!string.IsNullOrEmpty(property.Value.ToString()) && IsValidDoubleValue("MinValue", property.Value.ToString()))
			{
				stringBuilder.AppendFormat(" MINVALUE {0} ", property.Value.ToString());
			}
			else
			{
				stringBuilder.Append(" NO MINVALUE ");
			}
		}
		if ((property = base.Properties.Get("MaxValue")).Value != null && property.Dirty)
		{
			stringBuilder.Append(sp.NewLine);
			if (!string.IsNullOrEmpty(property.Value.ToString()) && IsValidDoubleValue("MaxValue", property.Value.ToString()))
			{
				stringBuilder.AppendFormat(" MAXVALUE {0} ", property.Value.ToString());
			}
			else
			{
				stringBuilder.Append(" NO MAXVALUE ");
			}
		}
		if ((property = base.Properties.Get("IsCycleEnabled")).Value != null && property.Dirty)
		{
			stringBuilder.Append(sp.NewLine);
			if ((bool)property.Value)
			{
				stringBuilder.Append(" CYCLE ");
			}
			else
			{
				stringBuilder.Append(" NO CYCLE ");
			}
		}
		Property property2;
		if (((property = base.Properties.Get("SequenceCacheType")).Value != null && property.Dirty) || ((property2 = base.Properties.Get("CacheSize")).Value != null && property2.Dirty))
		{
			SequenceCacheType sequenceCacheType = (SequenceCacheType)property.Value;
			if (!Enum.IsDefined(typeof(SequenceCacheType), sequenceCacheType))
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration("SequenceCacheType"));
			}
			switch (sequenceCacheType)
			{
			case SequenceCacheType.DefaultCache:
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append(" CACHE ");
				break;
			case SequenceCacheType.NoCache:
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append(" NO CACHE ");
				break;
			case SequenceCacheType.CacheWithSize:
			{
				int num = (int)base.Properties.Get("CacheSize").Value;
				stringBuilder.Append(Globals.newline);
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " CACHE  {0} ", new object[1] { num.ToString(SmoApplication.DefaultCulture) });
				break;
			}
			}
		}
		if (stringBuilder.Length > length)
		{
			alterQuery.Add(stringBuilder.ToString());
		}
		if (sp.IncludeScripts.Owner)
		{
			ScriptChangeOwner(alterQuery, sp);
		}
	}

	public void Rename(string newname)
	{
		RenameImpl(newname);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.InternalName) }));
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_rename @objname = N'{0}', @newname = N'{1}', @objtype = N'OBJECT'", new object[2]
		{
			SqlSmoObject.SqlString(FullQualifiedName),
			SqlSmoObject.SqlString(newName)
		}));
	}

	public override void Refresh()
	{
		base.Refresh();
		dataType = null;
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		return new PropagateInfo[1]
		{
			new PropagateInfo((base.ServerVersion.Major < 8) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
		};
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[14]
		{
			"NumericPrecision", "NumericScale", "SystemType", "DataTypeSchema", "StartValue", "IncrementValue", "MinValue", "MaxValue", "IsCycleEnabled", "SequenceCacheType",
			"CacheSize", "ID", "Owner", "IsSchemaOwned"
		};
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		supportedScriptFields.Add("DataType");
		return supportedScriptFields.ToArray();
	}
}
