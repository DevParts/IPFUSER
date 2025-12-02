using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
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
[StateChangeEvent("CREATE_PROCEDURE", "PROCEDURE")]
[StateChangeEvent("ALTER_PROCEDURE", "PROCEDURE")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[PhysicalFacet(PhysicalFacetOptions.ReadOnly)]
[StateChangeEvent("RENAME", "PROCEDURE")]
[StateChangeEvent("ALTER_AUTHORIZATION_DATABASE", "PROCEDURE")]
[StateChangeEvent("ALTER_SCHEMA", "PROCEDURE")]
public sealed class StoredProcedure : ScriptSchemaObjectBase, ISfcSupportsDesignMode, IObjectPermission, ICreatable, ICreateOrAlterable, IAlterable, IDroppable, IDropIfExists, IRenamable, IExtendedProperties, IScriptable, ITextObject
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 14, 14, 21, 22, 22, 22, 23, 23, 23, 23 };

		private static int[] cloudVersionCount = new int[3] { 18, 18, 22 };

		private static int sqlDwPropertyCount = 22;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[22]
		{
			new StaticMetadata("AnsiNullsStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AssemblyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("BodyStartIndex", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("ClassName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ExecutionContext", expensive: false, readOnly: false, typeof(ExecutionContext)),
			new StaticMetadata("ExecutionContextPrincipal", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ForReplication", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ImplementationType", expensive: false, readOnly: false, typeof(ImplementationType)),
			new StaticMetadata("IsEncrypted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsNativelyCompiled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSchemaBound", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("MethodName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("QuotedIdentifierStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Recompile", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Startup", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Text", expensive: true, readOnly: true, typeof(string))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[22]
		{
			new StaticMetadata("AnsiNullsStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("BodyStartIndex", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ExecutionContext", expensive: false, readOnly: false, typeof(ExecutionContext)),
			new StaticMetadata("ExecutionContextPrincipal", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ForReplication", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ImplementationType", expensive: false, readOnly: false, typeof(ImplementationType)),
			new StaticMetadata("IsEncrypted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSchemaBound", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("QuotedIdentifierStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Recompile", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Startup", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Text", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("AssemblyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ClassName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("IsNativelyCompiled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("MethodName", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[23]
		{
			new StaticMetadata("AnsiNullsStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("BodyStartIndex", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ForReplication", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ImplementationType", expensive: false, readOnly: false, typeof(ImplementationType)),
			new StaticMetadata("IsEncrypted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSchemaBound", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("QuotedIdentifierStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Recompile", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Startup", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Text", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("AssemblyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ClassName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ExecutionContext", expensive: false, readOnly: false, typeof(ExecutionContext)),
			new StaticMetadata("ExecutionContextPrincipal", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("MethodName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
			new StaticMetadata("IsNativelyCompiled", expensive: false, readOnly: false, typeof(bool))
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
						"AnsiNullsStatus" => 0, 
						"AssemblyName" => 1, 
						"BodyStartIndex" => 2, 
						"ClassName" => 3, 
						"CreateDate" => 4, 
						"DateLastModified" => 5, 
						"ExecutionContext" => 6, 
						"ExecutionContextPrincipal" => 7, 
						"ForReplication" => 8, 
						"ID" => 9, 
						"ImplementationType" => 10, 
						"IsEncrypted" => 11, 
						"IsNativelyCompiled" => 12, 
						"IsSchemaBound" => 13, 
						"IsSchemaOwned" => 14, 
						"IsSystemObject" => 15, 
						"MethodName" => 16, 
						"Owner" => 17, 
						"QuotedIdentifierStatus" => 18, 
						"Recompile" => 19, 
						"Startup" => 20, 
						"Text" => 21, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"AnsiNullsStatus" => 0, 
					"BodyStartIndex" => 1, 
					"CreateDate" => 2, 
					"DateLastModified" => 3, 
					"ExecutionContext" => 4, 
					"ExecutionContextPrincipal" => 5, 
					"ForReplication" => 6, 
					"ID" => 7, 
					"ImplementationType" => 8, 
					"IsEncrypted" => 9, 
					"IsSchemaBound" => 10, 
					"IsSchemaOwned" => 11, 
					"IsSystemObject" => 12, 
					"Owner" => 13, 
					"QuotedIdentifierStatus" => 14, 
					"Recompile" => 15, 
					"Startup" => 16, 
					"Text" => 17, 
					"AssemblyName" => 18, 
					"ClassName" => 19, 
					"IsNativelyCompiled" => 20, 
					"MethodName" => 21, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"AnsiNullsStatus" => 0, 
				"BodyStartIndex" => 1, 
				"CreateDate" => 2, 
				"ForReplication" => 3, 
				"ID" => 4, 
				"ImplementationType" => 5, 
				"IsEncrypted" => 6, 
				"IsSchemaBound" => 7, 
				"IsSystemObject" => 8, 
				"Owner" => 9, 
				"QuotedIdentifierStatus" => 10, 
				"Recompile" => 11, 
				"Startup" => 12, 
				"Text" => 13, 
				"AssemblyName" => 14, 
				"ClassName" => 15, 
				"DateLastModified" => 16, 
				"ExecutionContext" => 17, 
				"ExecutionContextPrincipal" => 18, 
				"IsSchemaOwned" => 19, 
				"MethodName" => 20, 
				"PolicyHealthState" => 21, 
				"IsNativelyCompiled" => 22, 
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

	private StoredProcedureEvents events;

	private NumberedStoredProcedureCollection numberedStoredProcedureCollection;

	private StoredProcedureParameterCollection m_Params;

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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool AnsiNullsStatus
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AnsiNullsStatus");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AnsiNullsStatus", value);
		}
	}

	[SfcReference(typeof(SqlAssembly), "Server[@Name = '{0}']/Database[@Name = '{1}']/SqlAssembly[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "AssemblyName" })]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[CLSCompliant(false)]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
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
	[DmfIgnoreProperty]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public ExecutionContext ExecutionContext
	{
		get
		{
			return (ExecutionContext)base.Properties.GetValueWithNullReplacement("ExecutionContext");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ExecutionContext", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "ExecutionContextPrincipal" })]
	[CLSCompliant(false)]
	public string ExecutionContextPrincipal
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ExecutionContextPrincipal");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ExecutionContextPrincipal", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool ForReplication
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("ForReplication");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ForReplication", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design, "TransactSql")]
	public ImplementationType ImplementationType
	{
		get
		{
			return (ImplementationType)base.Properties.GetValueWithNullReplacement("ImplementationType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ImplementationType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy, "false")]
	public bool IsEncrypted
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsEncrypted");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsEncrypted", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsNativelyCompiled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsNativelyCompiled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsNativelyCompiled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsSchemaBound
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsSchemaBound");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsSchemaBound", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsSchemaOwned => (bool)base.Properties.GetValueWithNullReplacement("IsSchemaOwned");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string MethodName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("MethodName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MethodName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcReference(typeof(DatabaseRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/Role[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcReference(typeof(ApplicationRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/ApplicationRole[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[CLSCompliant(false)]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool QuotedIdentifierStatus
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("QuotedIdentifierStatus");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("QuotedIdentifierStatus", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool Recompile
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Recompile");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Recompile", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool Startup
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Startup");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Startup", value);
		}
	}

	public StoredProcedureEvents Events
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
				events = new StoredProcedureEvents(this);
			}
			return events;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(NumberedStoredProcedure))]
	public NumberedStoredProcedureCollection NumberedStoredProcedures
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(NumberedStoredProcedure));
			if (numberedStoredProcedureCollection == null)
			{
				numberedStoredProcedureCollection = new NumberedStoredProcedureCollection(this);
			}
			return numberedStoredProcedureCollection;
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

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(StoredProcedureParameter), SfcObjectFlags.NaturalOrder | SfcObjectFlags.Design)]
	public StoredProcedureParameterCollection Parameters
	{
		get
		{
			CheckObjectState();
			if (m_Params == null)
			{
				m_Params = new StoredProcedureParameterCollection(this);
				SetCollectionTextMode(TextMode, m_Params);
			}
			return m_Params;
		}
	}

	public static string UrnSuffix => "StoredProcedure";

	[SfcKey(1)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
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

	[SfcReference(typeof(Schema), typeof(SchemaCustomResolver), "Resolve", new string[] { })]
	[SfcKey(0)]
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

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string TextBody
	{
		get
		{
			CheckObjectState();
			return GetTextBody();
		}
		set
		{
			CheckObjectState();
			SetTextBody(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string TextHeader
	{
		get
		{
			CheckObjectState();
			return GetTextHeader(forAlter: false);
		}
		set
		{
			CheckObjectState();
			SetTextHeader(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool TextMode
	{
		get
		{
			CheckObjectState();
			return GetTextMode();
		}
		set
		{
			CheckObjectState();
			if (value && ImplementationType.SqlClr == GetPropValueOptional("ImplementationType", ImplementationType.TransactSql))
			{
				throw new PropertyWriteException("TextMode", GetType().Name, Name, ExceptionTemplatesImpl.ReasonNotIntextMode);
			}
			SetTextMode(value, new SmoCollectionBase[1] { Parameters });
		}
	}

	public StoredProcedure()
	{
	}

	public StoredProcedure(Database database, string name)
	{
		ValidateName(name);
		key = new SchemaObjectKey(name, null);
		Parent = database;
	}

	public StoredProcedure(Database database, string name, string schema)
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
		return new string[2] { "ForReplication", "IsNativelyCompiled" };
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		return propname switch
		{
			"ImplementationType" => ImplementationType.TransactSql, 
			"IsEncrypted" => false, 
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

	internal StoredProcedure(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		m_Params = null;
	}

	public void ChangeSchema(string newSchema)
	{
		CheckObjectState();
		ChangeSchema(newSchema, bCheckExisting: true);
	}

	protected override void TouchImpl()
	{
		if (!this.IsSupportedObject<NumberedStoredProcedure>())
		{
			return;
		}
		foreach (NumberedStoredProcedure numberedStoredProcedure in NumberedStoredProcedures)
		{
			numberedStoredProcedure.Touch();
		}
	}

	private bool ShouldScriptBodyAtAlter()
	{
		if (GetIsTextDirty())
		{
			return true;
		}
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add("AnsiNullsStatus");
		stringCollection.Add("QuotedIdentifierStatus");
		stringCollection.Add("Recompile");
		stringCollection.Add("IsEncrypted");
		stringCollection.Add("ForReplication");
		stringCollection.Add("ImplementationType");
		if (base.ServerVersion.Major > 8)
		{
			stringCollection.Add("ExecutionContext");
			stringCollection.Add("AssemblyName");
			stringCollection.Add("ClassName");
			stringCollection.Add("MethodName");
		}
		if (base.Properties.ArePropertiesDirty(stringCollection))
		{
			return true;
		}
		if (SqlSmoObject.IsCollectionDirty(Parameters))
		{
			return true;
		}
		return false;
	}

	private void ScriptSP(StringCollection queries, ScriptingPreferences sp, ScriptHeaderType scriptHeaderType, bool skipSetOptions = false)
	{
		bool flag = IsCreate(scriptHeaderType);
		if (flag || ShouldScriptBodyAtAlter())
		{
			ScriptInternal(queries, sp, scriptHeaderType, skipSetOptions);
		}
		if (!SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType))
		{
			ScriptExternal(queries, sp, flag);
		}
		if ((!IsSupportedProperty("IsNativelyCompiled", sp) || !GetPropValueOptional("IsNativelyCompiled", defaultValue: false)) && sp.IncludeScripts.Owner)
		{
			ScriptOwner(queries, sp);
		}
	}

	private void ScriptSPHeaderInternal(StringBuilder sb, ScriptingPreferences sp, ScriptHeaderType scriptHeaderType)
	{
		switch (scriptHeaderType)
		{
		case ScriptHeaderType.ScriptHeaderForCreate:
			sb.AppendFormat(SmoApplication.DefaultCulture, "{0} PROCEDURE {1}", new object[2]
			{
				Scripts.CREATE,
				FormatFullNameForScripting(sp)
			});
			break;
		case ScriptHeaderType.ScriptHeaderForAlter:
			sb.AppendFormat(SmoApplication.DefaultCulture, "{0} PROCEDURE {1}", new object[2]
			{
				Scripts.ALTER,
				FormatFullNameForScripting(sp)
			});
			break;
		case ScriptHeaderType.ScriptHeaderForCreateOrAlter:
			SqlSmoObject.ThrowIfCreateOrAlterUnsupported(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.CreateOrAlterDownlevel("Procedure", SqlSmoObject.GetSqlServerName(sp)));
			sb.AppendFormat(SmoApplication.DefaultCulture, "{0} PROCEDURE {1}", new object[2]
			{
				Scripts.CREATE_OR_ALTER,
				FormatFullNameForScripting(sp)
			});
			break;
		default:
			throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration(scriptHeaderType.ToString()));
		}
		sb.Append(sp.NewLine);
		bool flag = true;
		StringCollection stringCollection = new StringCollection();
		foreach (StoredProcedureParameter parameter in Parameters)
		{
			if (parameter.State != SqlSmoState.ToBeDropped && !(string.Empty == parameter.Name))
			{
				if (!flag)
				{
					sb.Append(",");
					sb.Append(sp.NewLine);
				}
				flag = false;
				parameter.ScriptDdlInternal(stringCollection, sp);
				sb.AppendFormat(SmoApplication.DefaultCulture, "\t");
				sb.Append(stringCollection[0]);
				stringCollection.Clear();
			}
		}
		if (!flag)
		{
			sb.Append(sp.NewLine);
		}
		bool needsComma = false;
		if (IsSupportedProperty("IsNativelyCompiled", sp))
		{
			AppendWithOption(sb, "IsNativelyCompiled", Scripts.NATIVELY_COMPILED, ref needsComma);
		}
		if (IsSupportedProperty("IsSchemaBound", sp))
		{
			AppendWithOption(sb, "IsSchemaBound", Scripts.SP_SCHEMABINDING, ref needsComma);
		}
		if (IsTransactSql(sp))
		{
			AppendWithOption(sb, "Recompile", "RECOMPILE", ref needsComma);
			if (!SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType))
			{
				AppendWithOption(sb, "IsEncrypted", "ENCRYPTION", ref needsComma);
			}
		}
		if (base.ServerVersion.Major >= 9 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			AddScriptExecuteAs(sb, sp, base.Properties, ref needsComma);
		}
		if (needsComma)
		{
			sb.Append(sp.NewLine);
		}
		if (IsSupportedProperty("ForReplication", sp) && !SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType) && object.Equals(base.Properties.Get("ForReplication").Value, true))
		{
			sb.Append("FOR REPLICATION");
			sb.Append(sp.NewLine);
		}
		sb.Append("AS");
	}

	private void ScriptSPBodyInternal(StringBuilder sb, ScriptingPreferences sp)
	{
		if (IsTransactSql(sp))
		{
			sb.Append(GetTextBody(forScripting: true));
			return;
		}
		SqlSmoObject.ThrowIfCloud(sp.TargetDatabaseEngineType, ExceptionTemplatesImpl.ClrStoredProcedureDownlevel(FormatFullNameForScripting(sp, nameIsIndentifier: true), SqlSmoObject.GetSqlServerName(sp)));
		SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.ClrStoredProcedureDownlevel(FormatFullNameForScripting(sp, nameIsIndentifier: true), SqlSmoObject.GetSqlServerName(sp)));
		sb.Append("EXTERNAL NAME ");
		string text = (string)GetPropValue("AssemblyName");
		if (string.Empty == text)
		{
			throw new PropertyNotSetException("AssemblyName");
		}
		sb.AppendFormat("[{0}]", SqlSmoObject.SqlBraket(text));
		text = (string)GetPropValue("ClassName");
		if (string.Empty == text)
		{
			throw new PropertyNotSetException("ClassName");
		}
		sb.AppendFormat(".[{0}]", SqlSmoObject.SqlBraket(text));
		text = (string)GetPropValue("MethodName");
		if (string.Empty == text)
		{
			throw new PropertyNotSetException(text);
		}
		sb.AppendFormat(".[{0}]", SqlSmoObject.SqlBraket(text));
	}

	private bool IsTransactSql(ScriptingPreferences sp)
	{
		bool result = true;
		object propValueOptional = GetPropValueOptional("ImplementationType");
		if (DatabaseEngineType != DatabaseEngineType.SqlAzureDatabase && propValueOptional != null && (ImplementationType)propValueOptional == ImplementationType.SqlClr)
		{
			if (base.ServerVersion.Major < 9)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.ClrNotSupported("ImplementationType", base.ServerVersion.ToString()));
			}
			SqlSmoObject.ThrowIfCloud(sp.TargetDatabaseEngineType, ExceptionTemplatesImpl.ClrStoredProcedureDownlevel(FormatFullNameForScripting(sp, nameIsIndentifier: true), SqlSmoObject.GetSqlServerName(sp)));
			SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.ClrStoredProcedureDownlevel(FormatFullNameForScripting(sp, nameIsIndentifier: true), SqlSmoObject.GetSqlServerName(sp)));
			result = false;
			if (base.Properties.Get("Text").Dirty && !TextMode)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoPropertyChangeForDotNet("TextBody"));
			}
		}
		return result;
	}

	private void ScriptAlterSkipSetOptions(StringCollection alterQuery, ScriptingPreferences sp)
	{
		InitializeKeepDirtyValues();
		ScriptSP(alterQuery, sp, ScriptHeaderType.ScriptHeaderForAlter, skipSetOptions: true);
	}

	private void ScriptInternal(StringCollection queries, ScriptingPreferences sp, ScriptHeaderType scriptHeaderType, bool skipSetOptions = false)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = SqlSmoObject.SqlString(FormatFullNameForScripting(sp));
		_ = Schema;
		ScriptInformativeHeaders(sp, stringBuilder);
		ScriptAnsiQI(this, sp, queries, stringBuilder, out var _, out var _, skipSetOptions);
		if (!sp.OldOptions.DdlHeaderOnly && !sp.OldOptions.DdlBodyOnly && sp.IncludeScripts.ExistenceCheck)
		{
			string text2 = (sp.ScriptForAlter ? string.Empty : "NOT");
			if (sp.TargetServerVersionInternal <= SqlServerVersionInternal.Version80)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_PROCEDURE80, new object[2] { text2, text });
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_PROCEDURE90, new object[2] { text2, text });
			}
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("BEGIN");
			stringBuilder.Append(Globals.newline);
		}
		StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (!TextMode || (sp.OldOptions.EnforceScriptingPreferences && sp.DataType.UserDefinedDataTypesToBaseType))
		{
			if (!sp.OldOptions.DdlBodyOnly)
			{
				ScriptSPHeaderInternal(stringBuilder2, sp, scriptHeaderType);
				stringBuilder2.Append(sp.NewLine);
			}
			if (!sp.OldOptions.DdlHeaderOnly)
			{
				ScriptSPBodyInternal(stringBuilder2, sp);
			}
		}
		else
		{
			if (base.State == SqlSmoState.Existing && IsSupportedProperty("ForReplication", sp))
			{
				object propValueOptional = GetPropValueOptional("ForReplication");
				if (DatabaseEngineType.SqlAzureDatabase == sp.TargetDatabaseEngineType && propValueOptional != null && (bool)propValueOptional)
				{
					throw new WrongPropertyValueException(string.Format(CultureInfo.CurrentCulture, ExceptionTemplatesImpl.ReplicationOptionNotSupportedForCloud, new object[1] { "FOR REPLICATION" }));
				}
			}
			stringBuilder2.Append(GetTextForScript(sp, new string[2] { "procedure", "proc" }, forceCheckNameAndManipulateIfRequired: true, scriptHeaderType));
		}
		if (!sp.OldOptions.DdlHeaderOnly && !sp.OldOptions.DdlBodyOnly && sp.IncludeScripts.ExistenceCheck)
		{
			if (IsCreate(scriptHeaderType))
			{
				string text3 = "CREATE PROCEDURE " + text + " AS";
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_executesql @statement = N'{0}' ", new object[1] { text3 });
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append("END");
				queries.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
				StringCollection stringCollection = new StringCollection();
				Touch();
				ScriptingPreferences scriptingPreferences = (ScriptingPreferences)sp.Clone();
				scriptingPreferences.IncludeScripts.ExistenceCheck = false;
				scriptingPreferences.IncludeScripts.Header = false;
				scriptingPreferences.OldOptions.EnforceScriptingPreferences = true;
				ScriptAlterSkipSetOptions(stringCollection, scriptingPreferences);
				queries.AddCollection(stringCollection);
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_executesql @statement = {0} ", new object[1] { SqlSmoObject.MakeSqlString(stringBuilder2.ToString()) });
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append("END");
			}
		}
		else
		{
			stringBuilder.Append(stringBuilder2.ToString());
		}
		if (stringBuilder.Length > 0)
		{
			queries.Add(stringBuilder.ToString());
			stringBuilder.Length = 0;
		}
	}

	private void ScriptExternal(StringCollection queries, ScriptingPreferences sp, bool bForCreate)
	{
		if (!sp.OldOptions.DdlHeaderOnly && !sp.OldOptions.DdlBodyOnly)
		{
			Property property = base.Properties.Get("Startup");
			if ((!bForCreate && property != null && property.Dirty) || (bForCreate && property != null && property.Value != null && (bool)property.Value))
			{
				StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
				_ = Schema;
				string s = FormatFullNameForScripting(sp);
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC sp_procoption N'{0}', 'startup', '{1}'", new object[2]
				{
					SqlSmoObject.SqlString(s),
					((bool)property.Value) ? 1 : 0
				});
				stringBuilder.Append(sp.NewLine);
				queries.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
		}
	}

	public void Create()
	{
		CreateImpl();
		SetSchemaOwned();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		if (base.State != SqlSmoState.Creating && IsEncrypted && ImplementationType == ImplementationType.TransactSql)
		{
			SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.EncryptedStoredProcedureDownlevel(FormatFullNameForScripting(sp, nameIsIndentifier: true), SqlSmoObject.GetSqlServerName(sp)));
		}
		ScriptSP(queries, sp, ScriptHeaderType.ScriptHeaderForCreate);
	}

	public void CreateOrAlter()
	{
		CreateOrAlterImpl();
		SetSchemaOwned();
	}

	internal override void ScriptCreateOrAlter(StringCollection queries, ScriptingPreferences sp)
	{
		InitializeKeepDirtyValues();
		ScriptSP(queries, sp, ScriptHeaderType.ScriptHeaderForCreateOrAlter);
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
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90) ? Scripts.INCLUDE_EXISTS_PROCEDURE80 : Scripts.INCLUDE_EXISTS_PROCEDURE90, new object[2]
			{
				"",
				SqlSmoObject.SqlString(text)
			});
			stringBuilder.Append(Globals.newline);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP PROCEDURE {0}{1}", new object[2]
		{
			(sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty,
			text
		});
		queries.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
		SetSchemaOwned();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		if (IsObjectDirty())
		{
			InitializeKeepDirtyValues();
			ScriptSP(alterQuery, sp, ScriptHeaderType.ScriptHeaderForAlter);
			ScriptNumberedStoredProcedures(alterQuery, sp);
		}
	}

	public void Rename(string newname)
	{
		Table.CheckTableName(newname);
		RenameImpl(newname);
	}

	public void ReCompileReferences()
	{
		ReCompile(Name, Schema);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.InternalName) }));
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_rename @objname = N'{0}', @newname = N'{1}', @objtype = N'OBJECT'", new object[2]
		{
			SqlSmoObject.SqlString(FullQualifiedName),
			SqlSmoObject.SqlString(newName)
		}));
	}

	protected override bool IsObjectDirty()
	{
		if (!base.IsObjectDirty())
		{
			return SqlSmoObject.IsCollectionDirty(Parameters);
		}
		return true;
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			return new PropagateInfo[1]
			{
				new PropagateInfo(Parameters, bWithScript: false)
			};
		}
		return new PropagateInfo[3]
		{
			new PropagateInfo((base.ServerVersion.Major < 8) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix),
			new PropagateInfo(Parameters, bWithScript: false),
			new PropagateInfo(NumberedStoredProcedures, bWithScript: false, bPropagateScriptToChildLevel: false)
		};
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (m_ExtendedProperties != null)
		{
			m_ExtendedProperties.MarkAllDropped();
		}
		if (numberedStoredProcedureCollection != null)
		{
			numberedStoredProcedureCollection.MarkAllDropped();
		}
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		if (defaultTextMode)
		{
			string[] fields = new string[8] { "AnsiNullsStatus", "ImplementationType", "IsEncrypted", "IsSystemObject", "ID", "QuotedIdentifierStatus", "Startup", "Text" };
			List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
			return supportedScriptFields.ToArray();
		}
		string[] fields2 = new string[19]
		{
			"AnsiNullsStatus", "AssemblyName", "ClassName", "ExecutionContext", "ExecutionContextPrincipal", "ForReplication", "ID", "ImplementationType", "IsEncrypted", "IsNativelyCompiled",
			"IsSchemaBound", "IsSchemaOwned", "IsSystemObject", "MethodName", "QuotedIdentifierStatus", "Owner", "Recompile", "Startup", "Text"
		};
		List<string> supportedScriptFields2 = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields2, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields2.ToArray();
	}

	public string ScriptHeader(bool forAlter)
	{
		CheckObjectState();
		return GetTextHeader(forAlter);
	}

	public string ScriptHeader(ScriptHeaderType scriptHeaderType)
	{
		CheckObjectState();
		return GetTextHeader(scriptHeaderType);
	}

	internal override void ValidateProperty(Property prop, object value)
	{
		switch (base.ServerVersion.Major)
		{
		case 7:
		case 8:
			switch (prop.Name)
			{
			case "Recompile":
			case "ForReplication":
			case "IsEncrypted":
				ScriptNameObjectBase.Validate_set_TextObjectDDLProperty(prop, value);
				break;
			}
			break;
		default:
			switch (prop.Name)
			{
			case "Recompile":
			case "ForReplication":
			case "IsEncrypted":
			case "ExecutionContext":
			case "ExecutionContextPrincipal":
			case "AssemblyName":
			case "ClassName":
			case "MethodName":
				ScriptNameObjectBase.Validate_set_TextObjectDDLProperty(prop, value);
				break;
			}
			break;
		}
	}

	protected override void PostCreate()
	{
		if (TextMode && !CheckTextModeSupport())
		{
			TextMode = false;
		}
	}

	internal override void ScriptCreateInternal(StringCollection query, ScriptingPreferences sp, bool skipPropagateScript)
	{
		ScriptCreate(query, sp);
		ScriptNumberedStoredProcedures(query, sp);
		if (sp.IncludeScripts.Permissions)
		{
			AddScriptPermission(query, sp);
		}
		if (!skipPropagateScript)
		{
			PropagateScript(query, sp, PropagateAction.Create);
		}
	}

	private void ScriptNumberedStoredProcedures(StringCollection queries, ScriptingPreferences sp)
	{
		if (!this.IsSupportedObject<NumberedStoredProcedure>(sp))
		{
			return;
		}
		foreach (NumberedStoredProcedure numberedStoredProcedure in NumberedStoredProcedures)
		{
			if (sp.ScriptForAlter)
			{
				numberedStoredProcedure.ScriptAlterInternal(queries, sp);
			}
			else
			{
				numberedStoredProcedure.ScriptCreateInternal(queries, sp);
			}
		}
	}
}
