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
[PhysicalFacet]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public sealed class UserDefinedType : ScriptSchemaObjectBase, ISfcSupportsDesignMode, IObjectPermission, ICreatable, IDroppable, IDropIfExists, IExtendedProperties, IScriptable, IAlterable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 15, 16, 16, 16, 16, 16, 16, 16 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 15 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[15]
		{
			new StaticMetadata("AssemblyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("BinaryTypeIdentifier", expensive: false, readOnly: true, typeof(byte[])),
			new StaticMetadata("ClassName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Collation", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsBinaryOrdered", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsComVisible", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsFixedLength", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsNullable", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("MaxLength", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("UserDefinedTypeFormat", expensive: false, readOnly: true, typeof(UserDefinedTypeFormat))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[16]
		{
			new StaticMetadata("AssemblyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("BinaryTypeIdentifier", expensive: false, readOnly: true, typeof(byte[])),
			new StaticMetadata("ClassName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Collation", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsBinaryOrdered", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsComVisible", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsFixedLength", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsNullable", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("MaxLength", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("UserDefinedTypeFormat", expensive: false, readOnly: true, typeof(UserDefinedTypeFormat)),
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
					"AssemblyName" => 0, 
					"BinaryTypeIdentifier" => 1, 
					"ClassName" => 2, 
					"Collation" => 3, 
					"ID" => 4, 
					"IsBinaryOrdered" => 5, 
					"IsComVisible" => 6, 
					"IsFixedLength" => 7, 
					"IsNullable" => 8, 
					"IsSchemaOwned" => 9, 
					"MaxLength" => 10, 
					"NumericPrecision" => 11, 
					"NumericScale" => 12, 
					"Owner" => 13, 
					"UserDefinedTypeFormat" => 14, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"AssemblyName" => 0, 
				"BinaryTypeIdentifier" => 1, 
				"ClassName" => 2, 
				"Collation" => 3, 
				"ID" => 4, 
				"IsBinaryOrdered" => 5, 
				"IsComVisible" => 6, 
				"IsFixedLength" => 7, 
				"IsNullable" => 8, 
				"IsSchemaOwned" => 9, 
				"MaxLength" => 10, 
				"NumericPrecision" => 11, 
				"NumericScale" => 12, 
				"Owner" => 13, 
				"UserDefinedTypeFormat" => 14, 
				"PolicyHealthState" => 15, 
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

	private UserDefinedTypeEvents events;

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

	[SfcReference(typeof(SqlAssembly), "Server[@Name = '{0}']/Database[@Name = '{1}']/SqlAssembly[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "AssemblyName" })]
	[CLSCompliant(false)]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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
	public byte[] BinaryTypeIdentifier => (byte[])base.Properties.GetValueWithNullReplacement("BinaryTypeIdentifier");

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
	public string Collation => (string)base.Properties.GetValueWithNullReplacement("Collation");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsBinaryOrdered => (bool)base.Properties.GetValueWithNullReplacement("IsBinaryOrdered");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsComVisible => (bool)base.Properties.GetValueWithNullReplacement("IsComVisible");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsFixedLength => (bool)base.Properties.GetValueWithNullReplacement("IsFixedLength");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsNullable => (bool)base.Properties.GetValueWithNullReplacement("IsNullable");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsSchemaOwned => (bool)base.Properties.GetValueWithNullReplacement("IsSchemaOwned");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int MaxLength => (int)base.Properties.GetValueWithNullReplacement("MaxLength");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int NumericPrecision => (int)base.Properties.GetValueWithNullReplacement("NumericPrecision");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int NumericScale => (int)base.Properties.GetValueWithNullReplacement("NumericScale");

	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcReference(typeof(ApplicationRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/ApplicationRole[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[CLSCompliant(false)]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public UserDefinedTypeFormat UserDefinedTypeFormat => (UserDefinedTypeFormat)base.Properties.GetValueWithNullReplacement("UserDefinedTypeFormat");

	public UserDefinedTypeEvents Events
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
				events = new UserDefinedTypeEvents(this);
			}
			return events;
		}
	}

	public static string UrnSuffix => "UserDefinedType";

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

	public UserDefinedType()
	{
	}

	public UserDefinedType(Database database, string name)
	{
		ValidateName(name);
		key = new SchemaObjectKey(name, null);
		Parent = database;
	}

	public UserDefinedType(Database database, string name, string schema)
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
		m_ExtendedProperties = null;
	}

	internal UserDefinedType(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
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
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_UDT, new object[3]
			{
				"NOT",
				SqlSmoObject.SqlString(GetName(sp)),
				SqlSmoObject.SqlString(GetSchema(sp))
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat("CREATE TYPE {0}", text);
		stringBuilder.Append(sp.NewLine);
		stringBuilder.Append("EXTERNAL NAME ");
		string text2 = (string)GetPropValue("AssemblyName");
		if (string.Empty == text2)
		{
			throw new PropertyNotSetException("AssemblyName");
		}
		stringBuilder.AppendFormat("[{0}]", SqlSmoObject.SqlBraket(text2));
		Property property = base.Properties.Get("ClassName");
		if (property.Value != null && ((string)property.Value).Length > 0)
		{
			stringBuilder.AppendFormat(".[{0}]", SqlSmoObject.SqlBraket((string)property.Value));
		}
		stringBuilder.Append(sp.NewLine);
		queries.Add(stringBuilder.ToString());
		if (sp.IncludeScripts.Owner)
		{
			ScriptOwner(queries, sp);
		}
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		if (sp.IncludeScripts.Owner)
		{
			ScriptOwner(query, sp);
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
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_UDT, new object[3]
			{
				"",
				SqlSmoObject.SqlString(GetName(sp)),
				SqlSmoObject.SqlString(GetSchema(sp))
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat("DROP TYPE {0}{1}", (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty, text);
		stringBuilder.Append(sp.NewLine);
		dropQuery.Add(stringBuilder.ToString());
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
		string[] fields = new string[4] { "AssemblyName", "ClassName", "ID", "Owner" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}
