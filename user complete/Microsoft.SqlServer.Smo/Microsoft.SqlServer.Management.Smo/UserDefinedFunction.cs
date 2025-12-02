using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

[StateChangeEvent("RENAME", "FUNCTION")]
[StateChangeEvent("CREATE_FUNCTION", "FUNCTION")]
[StateChangeEvent("ALTER_FUNCTION", "FUNCTION")]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[StateChangeEvent("ALTER_AUTHORIZATION_DATABASE", "FUNCTION")]
[StateChangeEvent("ALTER_SCHEMA", "FUNCTION")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[PhysicalFacet(PhysicalFacetOptions.ReadOnly)]
public sealed class UserDefinedFunction : ScriptSchemaObjectBase, ISfcSupportsDesignMode, IColumnPermission, IObjectPermission, ICreatable, IAlterable, ICreateOrAlterable, IRenamable, IDroppable, IDropIfExists, IExtendedProperties, IScriptable, ITextObject
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 21, 32, 34, 34, 34, 34, 35, 35, 36 };

		private static int[] cloudVersionCount = new int[3] { 29, 29, 35 };

		private static int sqlDwPropertyCount = 32;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[32]
		{
			new StaticMetadata("AnsiNullsStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AssemblyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("BodyStartIndex", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("ClassName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ExecutionContext", expensive: false, readOnly: false, typeof(ExecutionContext)),
			new StaticMetadata("ExecutionContextPrincipal", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FunctionType", expensive: false, readOnly: false, typeof(UserDefinedFunctionType)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ImplementationType", expensive: false, readOnly: false, typeof(ImplementationType)),
			new StaticMetadata("InlineType", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsDeterministic", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsEncrypted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsInlineable", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsNativelyCompiled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSchemaBound", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("MethodName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("QuotedIdentifierStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ReturnsNullOnNullInput", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("TableVariableName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Text", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("UserType", expensive: false, readOnly: true, typeof(string))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[35]
		{
			new StaticMetadata("AnsiNullsStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("BodyStartIndex", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ExecutionContext", expensive: false, readOnly: false, typeof(ExecutionContext)),
			new StaticMetadata("ExecutionContextPrincipal", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FunctionType", expensive: false, readOnly: false, typeof(UserDefinedFunctionType)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ImplementationType", expensive: false, readOnly: false, typeof(ImplementationType)),
			new StaticMetadata("IsDeterministic", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsEncrypted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSchemaBound", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("QuotedIdentifierStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ReturnsNullOnNullInput", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("TableVariableName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Text", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("UserType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("XmlDocumentConstraint", expensive: false, readOnly: false, typeof(XmlDocumentConstraint)),
			new StaticMetadata("XmlSchemaNamespace", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("XmlSchemaNamespaceSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("AssemblyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ClassName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("InlineType", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsInlineable", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsNativelyCompiled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("MethodName", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[36]
		{
			new StaticMetadata("AnsiNullsStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("BodyStartIndex", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FunctionType", expensive: false, readOnly: false, typeof(UserDefinedFunctionType)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ImplementationType", expensive: false, readOnly: false, typeof(ImplementationType)),
			new StaticMetadata("IsDeterministic", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsEncrypted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsInlineable", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSchemaBound", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("QuotedIdentifierStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("TableVariableName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Text", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("AssemblyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ClassName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ExecutionContext", expensive: false, readOnly: false, typeof(ExecutionContext)),
			new StaticMetadata("ExecutionContextPrincipal", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("MethodName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ReturnsNullOnNullInput", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("XmlDocumentConstraint", expensive: false, readOnly: false, typeof(XmlDocumentConstraint)),
			new StaticMetadata("XmlSchemaNamespace", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("XmlSchemaNamespaceSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
			new StaticMetadata("UserType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("IsNativelyCompiled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("InlineType", expensive: false, readOnly: false, typeof(bool))
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
						"DataType" => 5, 
						"DataTypeSchema" => 6, 
						"DateLastModified" => 7, 
						"ExecutionContext" => 8, 
						"ExecutionContextPrincipal" => 9, 
						"FunctionType" => 10, 
						"ID" => 11, 
						"ImplementationType" => 12, 
						"InlineType" => 13, 
						"IsDeterministic" => 14, 
						"IsEncrypted" => 15, 
						"IsInlineable" => 16, 
						"IsNativelyCompiled" => 17, 
						"IsSchemaBound" => 18, 
						"IsSchemaOwned" => 19, 
						"IsSystemObject" => 20, 
						"Length" => 21, 
						"MethodName" => 22, 
						"NumericPrecision" => 23, 
						"NumericScale" => 24, 
						"Owner" => 25, 
						"QuotedIdentifierStatus" => 26, 
						"ReturnsNullOnNullInput" => 27, 
						"SystemType" => 28, 
						"TableVariableName" => 29, 
						"Text" => 30, 
						"UserType" => 31, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"AnsiNullsStatus" => 0, 
					"BodyStartIndex" => 1, 
					"CreateDate" => 2, 
					"DataType" => 3, 
					"DataTypeSchema" => 4, 
					"DateLastModified" => 5, 
					"ExecutionContext" => 6, 
					"ExecutionContextPrincipal" => 7, 
					"FunctionType" => 8, 
					"ID" => 9, 
					"ImplementationType" => 10, 
					"IsDeterministic" => 11, 
					"IsEncrypted" => 12, 
					"IsSchemaBound" => 13, 
					"IsSchemaOwned" => 14, 
					"IsSystemObject" => 15, 
					"Length" => 16, 
					"NumericPrecision" => 17, 
					"NumericScale" => 18, 
					"Owner" => 19, 
					"QuotedIdentifierStatus" => 20, 
					"ReturnsNullOnNullInput" => 21, 
					"SystemType" => 22, 
					"TableVariableName" => 23, 
					"Text" => 24, 
					"UserType" => 25, 
					"XmlDocumentConstraint" => 26, 
					"XmlSchemaNamespace" => 27, 
					"XmlSchemaNamespaceSchema" => 28, 
					"AssemblyName" => 29, 
					"ClassName" => 30, 
					"InlineType" => 31, 
					"IsInlineable" => 32, 
					"IsNativelyCompiled" => 33, 
					"MethodName" => 34, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"AnsiNullsStatus" => 0, 
				"BodyStartIndex" => 1, 
				"CreateDate" => 2, 
				"DataType" => 3, 
				"DataTypeSchema" => 4, 
				"FunctionType" => 5, 
				"ID" => 6, 
				"ImplementationType" => 7, 
				"IsDeterministic" => 8, 
				"IsEncrypted" => 9, 
				"IsInlineable" => 10, 
				"IsSchemaBound" => 11, 
				"IsSystemObject" => 12, 
				"Length" => 13, 
				"NumericPrecision" => 14, 
				"NumericScale" => 15, 
				"Owner" => 16, 
				"QuotedIdentifierStatus" => 17, 
				"SystemType" => 18, 
				"TableVariableName" => 19, 
				"Text" => 20, 
				"AssemblyName" => 21, 
				"ClassName" => 22, 
				"DateLastModified" => 23, 
				"ExecutionContext" => 24, 
				"ExecutionContextPrincipal" => 25, 
				"IsSchemaOwned" => 26, 
				"MethodName" => 27, 
				"ReturnsNullOnNullInput" => 28, 
				"XmlDocumentConstraint" => 29, 
				"XmlSchemaNamespace" => 30, 
				"XmlSchemaNamespaceSchema" => 31, 
				"PolicyHealthState" => 32, 
				"UserType" => 33, 
				"IsNativelyCompiled" => 34, 
				"InlineType" => 35, 
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

	private UserDefinedFunctionEvents events;

	private UserDefinedFunctionParameterCollection m_UserDefinedFunctionParams;

	private IndexCollection m_Indexes;

	private ColumnCollection m_Columns;

	private OrderColumnCollection m_OrderColumns;

	private CheckCollection m_Checks;

	private DataType dataType;

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

	[CLSCompliant(false)]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public UserDefinedFunctionType FunctionType
	{
		get
		{
			return (UserDefinedFunctionType)base.Properties.GetValueWithNullReplacement("FunctionType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FunctionType", value);
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

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool InlineType
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("InlineType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("InlineType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDeterministic => (bool)base.Properties.GetValueWithNullReplacement("IsDeterministic");

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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsInlineable => (bool)base.Properties.GetValueWithNullReplacement("IsInlineable");

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

	[CLSCompliant(false)]
	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcReference(typeof(ApplicationRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/ApplicationRole[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
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
	public bool ReturnsNullOnNullInput
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("ReturnsNullOnNullInput");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ReturnsNullOnNullInput", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string TableVariableName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("TableVariableName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("TableVariableName", value);
		}
	}

	public UserDefinedFunctionEvents Events
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
				events = new UserDefinedFunctionEvents(this);
			}
			return events;
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

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(UserDefinedFunctionParameter), SfcObjectFlags.NaturalOrder | SfcObjectFlags.Design)]
	public UserDefinedFunctionParameterCollection Parameters
	{
		get
		{
			CheckObjectState();
			if (m_UserDefinedFunctionParams == null)
			{
				m_UserDefinedFunctionParams = new UserDefinedFunctionParameterCollection(this);
				SetCollectionTextMode(TextMode, m_UserDefinedFunctionParams);
			}
			return m_UserDefinedFunctionParams;
		}
	}

	public static string UrnSuffix => "UserDefinedFunction";

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(Index), SfcObjectFlags.Design)]
	public IndexCollection Indexes
	{
		get
		{
			CheckObjectState();
			if (m_Indexes == null)
			{
				m_Indexes = new IndexCollection(this);
			}
			return m_Indexes;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(Column), SfcObjectFlags.NaturalOrder | SfcObjectFlags.Design)]
	public ColumnCollection Columns
	{
		get
		{
			CheckObjectState();
			if (m_Columns == null)
			{
				m_Columns = new ColumnCollection(this);
				SetCollectionTextMode(TextMode, m_Columns);
			}
			return m_Columns;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(OrderColumn))]
	public OrderColumnCollection OrderColumns
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(OrderColumn));
			if (m_OrderColumns == null)
			{
				m_OrderColumns = new OrderColumnCollection(this);
				SetCollectionTextMode(TextMode, m_OrderColumns);
			}
			return m_OrderColumns;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(Check), SfcObjectFlags.Design)]
	public CheckCollection Checks
	{
		get
		{
			CheckObjectState();
			if (m_Checks == null)
			{
				m_Checks = new CheckCollection(this);
				SetCollectionTextMode(TextMode, m_Checks);
			}
			return m_Checks;
		}
	}

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

	[CLSCompliant(false)]
	[SfcReference(typeof(Schema), typeof(SchemaCustomResolver), "Resolve", new string[] { })]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
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

	[CLSCompliant(false)]
	[SfcReference(typeof(UserDefinedType), typeof(UserDefinedTypeResolver), "Resolve", new string[] { })]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcReference(typeof(UserDefinedDataType), typeof(UserDefinedDataTypeResolver), "Resolve", new string[] { })]
	public DataType DataType
	{
		get
		{
			CheckObjectState();
			if (base.State == SqlSmoState.Existing && UserDefinedFunctionType.Scalar != (UserDefinedFunctionType)GetPropValue("FunctionType"))
			{
				return null;
			}
			return GetDataType(ref dataType);
		}
		set
		{
			SetDataType(ref dataType, value);
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
			SetTextMode(value, new SmoCollectionBase[3] { Parameters, Columns, Checks });
		}
	}

	public UserDefinedFunction()
	{
	}

	public UserDefinedFunction(Database database, string name)
	{
		ValidateName(name);
		key = new SchemaObjectKey(name, null);
		Parent = database;
	}

	public UserDefinedFunction(Database database, string name, string schema)
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
		return new string[2] { "InlineType", "IsNativelyCompiled" };
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

	public void Deny(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, columnNames, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, columnNames, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, columnNames, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, columnNames, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, columnNames, revokeGrant, cascade, asRole);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName, string[] columnNames, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, columnNames, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, string[] columnNames, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, columnNames, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, string[] columnNames, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, columnNames, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, string[] columnNames, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, columnNames, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, string[] columnNames, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, columnNames, revokeGrant, cascade, asRole);
	}

	public ObjectPermissionInfo[] EnumColumnPermissions()
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Column, this, null, null);
	}

	public ObjectPermissionInfo[] EnumColumnPermissions(string granteeName)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Column, this, granteeName, null);
	}

	public ObjectPermissionInfo[] EnumColumnPermissions(ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Column, this, null, permissions);
	}

	public ObjectPermissionInfo[] EnumColumnPermissions(string granteeName, ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Column, this, granteeName, permissions);
	}

	internal UserDefinedFunction(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void ChangeSchema(string newSchema)
	{
		CheckObjectState();
		ChangeSchema(newSchema, bCheckExisting: true);
	}

	internal override void AddScriptPermission(StringCollection query, ScriptingPreferences sp)
	{
		AddScriptPermissions(query, PermissionWorker.PermissionEnumKind.Object, sp);
		foreach (Column column in Columns)
		{
			column.AddScriptPermissions(query, PermissionWorker.PermissionEnumKind.Column, sp);
		}
	}

	private void AddParam(StringBuilder sb, ScriptingPreferences sp, UserDefinedFunctionParameter spp)
	{
		StringCollection stringCollection = new StringCollection();
		spp.UseOutput = false;
		spp.ScriptDdlInternal(stringCollection, sp);
		sb.Append(stringCollection[0]);
		stringCollection.Clear();
	}

	private void ScriptUDF(StringCollection queries, ScriptingPreferences sp, ScriptHeaderType scriptHeaderType)
	{
		if (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version70)
		{
			return;
		}
		UserDefinedFunctionType propValueOptional = GetPropValueOptional("FunctionType", UserDefinedFunctionType.Unknown);
		if (propValueOptional != UserDefinedFunctionType.Scalar && sp.TargetEngineIsAzureSqlDw())
		{
			throw new UnsupportedEngineEditionException(ExceptionTemplatesImpl.PropertyValueNotSupportedForSqlDw(typeof(UserDefinedFunctionType).Name, propValueOptional.ToString())).SetHelpContext("PropertyValueNotSupportedForSqlDw");
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		bool flag = false;
		bool flag2 = false;
		string text = FormatFullNameForScripting(sp);
		if (!sp.OldOptions.DdlHeaderOnly && !sp.OldOptions.DdlBodyOnly)
		{
			if (sp.IncludeScripts.Header)
			{
				stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
				stringBuilder.Append(sp.NewLine);
			}
			flag = null != base.Properties.Get("AnsiNullsStatus").Value;
			flag2 = null != base.Properties.Get("QuotedIdentifierStatus").Value;
			Server server = (Server)base.ParentColl.ParentInstance.ParentColl.ParentInstance;
			if (DatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
			{
				_ = server.UserOptions.AnsiNulls;
				_ = server.UserOptions.QuotedIdentifier;
			}
			if (flag)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.SET_ANSI_NULLS, new object[1] { ((bool)base.Properties["AnsiNullsStatus"].Value) ? Globals.On : Globals.Off });
				queries.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
			if (flag2)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.SET_QUOTED_IDENTIFIER, new object[1] { ((bool)base.Properties["QuotedIdentifierStatus"].Value) ? Globals.On : Globals.Off });
				queries.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
		}
		bool flag3 = true;
		Property property;
		if ((property = base.Properties.Get("ImplementationType")).Value != null && ImplementationType.SqlClr == (ImplementationType)property.Value)
		{
			if (base.ServerVersion.Major < 9 || (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase && base.ServerVersion.Major < 12))
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.ClrNotSupported("ImplementationType", base.ServerVersion.ToString()));
			}
			if (sp.TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				SqlSmoObject.ThrowIfBelowVersion120(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.ClrUserDefinedFunctionDownlevel(FormatFullNameForScripting(sp, nameIsIndentifier: true), SqlSmoObject.GetSqlServerName(sp)));
			}
			else
			{
				SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.ClrUserDefinedFunctionDownlevel(FormatFullNameForScripting(sp, nameIsIndentifier: true), SqlSmoObject.GetSqlServerName(sp)));
			}
			flag3 = false;
		}
		if (!sp.OldOptions.DdlHeaderOnly && !sp.OldOptions.DdlBodyOnly && sp.IncludeScripts.ExistenceCheck)
		{
			string arg = (sp.ScriptForAlter ? string.Empty : "NOT");
			if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90 && base.ServerVersion.Major >= 9)
			{
				stringBuilder.AppendFormat(Scripts.INCLUDE_EXISTS_FUNCTION90, arg, SqlSmoObject.SqlString(text));
			}
			else
			{
				stringBuilder.AppendFormat(Scripts.INCLUDE_EXISTS_FUNCTION80, arg, SqlSmoObject.SqlString(text));
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
				switch (scriptHeaderType)
				{
				case ScriptHeaderType.ScriptHeaderForCreate:
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "{0} FUNCTION {1}", new object[2]
					{
						Scripts.CREATE,
						text
					});
					break;
				case ScriptHeaderType.ScriptHeaderForAlter:
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "{0} FUNCTION {1}", new object[2]
					{
						Scripts.ALTER,
						text
					});
					break;
				case ScriptHeaderType.ScriptHeaderForCreateOrAlter:
					SqlSmoObject.ThrowIfCreateOrAlterUnsupported(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.CreateOrAlterDownlevel("Function", SqlSmoObject.GetSqlServerName(sp)));
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "{0} FUNCTION {1}", new object[2]
					{
						Scripts.CREATE_OR_ALTER,
						text
					});
					break;
				default:
					throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration(scriptHeaderType.ToString()));
				}
				stringBuilder2.Append(Globals.LParen);
				bool flag4 = true;
				foreach (UserDefinedFunctionParameter parameter in Parameters)
				{
					if (parameter.State != SqlSmoState.ToBeDropped && !(string.Empty == parameter.Name))
					{
						if (!flag4)
						{
							stringBuilder2.Append(", ");
						}
						flag4 = false;
						AddParam(stringBuilder2, sp, parameter);
					}
				}
				stringBuilder2.Append(Globals.RParen);
				stringBuilder2.Append(sp.NewLine);
				UserDefinedFunctionType userDefinedFunctionType = (UserDefinedFunctionType)GetPropValue("FunctionType");
				ScriptReturnType(sp, stringBuilder2, userDefinedFunctionType);
				bool needsComma = false;
				if (flag3)
				{
					AppendWithOption(stringBuilder2, "IsSchemaBound", Scripts.SP_SCHEMABINDING, ref needsComma);
					if (!SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType))
					{
						if (base.ServerVersion.Major >= 13 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130 && IsSupportedProperty("IsNativelyCompiled", sp))
						{
							AppendWithOption(stringBuilder2, "IsNativelyCompiled", Scripts.NATIVELY_COMPILED, ref needsComma);
						}
						AppendWithOption(stringBuilder2, "IsEncrypted", "ENCRYPTION", ref needsComma);
					}
					if (IsSupportedProperty("InlineType", sp))
					{
						AppendWithOption(stringBuilder2, "InlineType", Scripts.INLINE_TYPE, ref needsComma);
					}
				}
				if (base.ServerVersion.Major >= 9 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
				{
					if (userDefinedFunctionType != UserDefinedFunctionType.Inline)
					{
						AddScriptExecuteAs(stringBuilder2, sp, base.Properties, ref needsComma);
					}
					AppendWithOption(stringBuilder2, "ReturnsNullOnNullInput", "RETURNS NULL ON NULL INPUT", ref needsComma);
				}
				if (needsComma)
				{
					stringBuilder2.Append(sp.NewLine);
				}
				if (!flag3 && m_OrderColumns != null && m_OrderColumns.Count > 0)
				{
					stringBuilder2.Append("ORDER ");
					stringBuilder2.Append(Globals.LParen);
					bool flag5 = true;
					foreach (OrderColumn orderColumn in OrderColumns)
					{
						if (!flag5)
						{
							stringBuilder2.Append(Globals.comma);
							stringBuilder2.Append(Globals.space);
						}
						else
						{
							flag5 = false;
						}
						Column column = m_Columns[orderColumn.Name];
						if (column == null)
						{
							throw new SmoException(ExceptionTemplatesImpl.OrderHintRefsNonexCol(Name, "[" + SqlSmoObject.SqlStringBraket(orderColumn.Name) + "]"));
						}
						stringBuilder2.Append(SqlSmoObject.MakeSqlBraket(orderColumn.GetName(sp)));
						stringBuilder2.Append(Globals.space);
						if (orderColumn.Descending)
						{
							stringBuilder2.Append("DESC");
						}
						else
						{
							stringBuilder2.Append("ASC");
						}
					}
					stringBuilder2.Append(Globals.RParen);
					stringBuilder2.Append(sp.NewLine);
				}
				stringBuilder2.Append("AS ");
			}
			if (!sp.OldOptions.DdlHeaderOnly)
			{
				if (!sp.OldOptions.DdlBodyOnly)
				{
					stringBuilder2.Append(sp.NewLine);
				}
				if (flag3)
				{
					stringBuilder2.Append(GetTextBody(forScripting: true));
				}
				else
				{
					SqlSmoObject.ThrowIfCloudAndBelowVersion120(sp.TargetDatabaseEngineType, sp.TargetServerVersionInternal, ExceptionTemplatesImpl.ClrUserDefinedFunctionDownlevel(FormatFullNameForScripting(sp, nameIsIndentifier: true), SqlSmoObject.GetSqlServerName(sp)));
					stringBuilder2.Append("EXTERNAL NAME ");
					string text2 = (string)GetPropValue("AssemblyName");
					if (string.Empty == text2)
					{
						throw new PropertyNotSetException("AssemblyName");
					}
					stringBuilder2.AppendFormat("[{0}]", SqlSmoObject.SqlBraket(text2));
					text2 = (string)GetPropValue("ClassName");
					if (string.Empty == text2)
					{
						throw new PropertyNotSetException("ClassName");
					}
					stringBuilder2.AppendFormat(".[{0}]", SqlSmoObject.SqlBraket(text2));
					text2 = (string)GetPropValue("MethodName");
					if (string.Empty == text2)
					{
						throw new PropertyNotSetException(text2);
					}
					stringBuilder2.AppendFormat(".[{0}]", SqlSmoObject.SqlBraket(text2));
				}
			}
		}
		else
		{
			stringBuilder2.Append(GetTextForScript(sp, new string[1] { "function" }, forceCheckNameAndManipulateIfRequired: true, scriptHeaderType));
		}
		if (!sp.OldOptions.DdlHeaderOnly && !sp.OldOptions.DdlBodyOnly && sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "execute dbo.sp_executesql @statement = {0} ", new object[1] { SqlSmoObject.MakeSqlString(stringBuilder2.ToString()) });
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("END");
			stringBuilder.Append(Globals.newline);
		}
		else
		{
			stringBuilder.Append(stringBuilder2.ToString());
		}
		queries.Add(stringBuilder.ToString());
		stringBuilder.Length = 0;
	}

	public void Create()
	{
		CreateImpl();
		SetSchemaOwned();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		SqlSmoObject.ThrowIfBelowVersion80(sp.TargetServerVersionInternal);
		ThrowIfCompatibilityLevelBelow80();
		if (base.State != SqlSmoState.Creating && IsEncrypted && ImplementationType == ImplementationType.TransactSql)
		{
			SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.EncryptedUserDefinedFunctionsDownlevel(FormatFullNameForScripting(sp, nameIsIndentifier: true), SqlSmoObject.GetSqlServerName(sp)));
		}
		if (sp.OldOptions.PrimaryObject)
		{
			InitializeKeepDirtyValues();
			ScriptUDF(queries, sp, ScriptHeaderType.ScriptHeaderForCreate);
			if (sp.IncludeScripts.Owner)
			{
				ScriptOwner(queries, sp);
			}
		}
	}

	public void CreateOrAlter()
	{
		CreateOrAlterImpl();
		SetSchemaOwned();
	}

	internal override void ScriptCreateOrAlter(StringCollection queries, ScriptingPreferences sp)
	{
		SqlSmoObject.ThrowIfBelowVersion80(sp.TargetServerVersionInternal);
		ThrowIfCompatibilityLevelBelow80();
		InitializeKeepDirtyValues();
		ScriptUDF(queries, sp, ScriptHeaderType.ScriptHeaderForCreateOrAlter);
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

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = FormatFullNameForScripting(sp);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal < SqlServerVersionInternal.Version130)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90) ? Scripts.INCLUDE_EXISTS_FUNCTION80 : Scripts.INCLUDE_EXISTS_FUNCTION90, new object[2]
			{
				"",
				SqlSmoObject.SqlString(text)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP FUNCTION {0}{1}", new object[2]
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

	private bool ShouldScriptBodyAtAlter()
	{
		if (GetIsTextDirty())
		{
			return true;
		}
		foreach (Property property in base.Properties)
		{
			if (string.Compare("Owner", property.Name, StringComparison.OrdinalIgnoreCase) != 0 && property.Writable && property.Dirty)
			{
				return true;
			}
		}
		return false;
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		if (IsObjectDirty())
		{
			InitializeKeepDirtyValues();
			if (ShouldScriptBodyAtAlter())
			{
				ScriptUDF(alterQuery, sp, ScriptHeaderType.ScriptHeaderForAlter);
			}
			if (sp.IncludeScripts.Owner)
			{
				ScriptOwner(alterQuery, sp);
			}
		}
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
		return new PropagateInfo[3]
		{
			new PropagateInfo(Parameters, bWithScript: false),
			new PropagateInfo((action != PropagateAction.Create) ? null : Indexes, bWithScript: false),
			new PropagateInfo((base.ServerVersion.Major < 8 || DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
		};
	}

	public void Rename(string newname)
	{
		RenameImpl(newname);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "EXEC {0}.dbo.sp_rename @objname = N'{1}', @newname = N'{2}', @objtype = N'OBJECT'", new object[3]
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

	private void ScriptReturnType(ScriptingPreferences sp, StringBuilder sb, UserDefinedFunctionType type)
	{
		sb.Append("RETURNS ");
		if (UserDefinedFunctionType.Scalar == type)
		{
			UserDefinedDataType.AppendScriptTypeDefinition(sb, sp, this, DataType.SqlDataType);
		}
		else if (UserDefinedFunctionType.Inline == type)
		{
			sb.Append("TABLE");
		}
		else if (UserDefinedFunctionType.Table == type)
		{
			sb.Append(GetPropValue("TableVariableName"));
			sb.Append(" TABLE ");
			bool systemNamesForConstraints = sp.Table.SystemNamesForConstraints;
			sp.Table.SystemNamesForConstraints = false;
			try
			{
				Table.ScriptTableInternal(sp, sb, Columns, Indexes);
				foreach (Check check in Checks)
				{
					sb.Append(Globals.comma);
					sb.Append(sp.NewLine);
					sb.Append(check.ScriptDdlBodyWithoutName(sp));
				}
			}
			finally
			{
				sp.Table.SystemNamesForConstraints = systemNamesForConstraints;
			}
			sb.Append(sp.NewLine);
			sb.Append(Globals.RParen);
		}
		sb.Append(" ");
	}

	public override void Refresh()
	{
		base.Refresh();
		dataType = null;
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		if (defaultTextMode)
		{
			string[] fields = new string[9] { "ImplementationType", "ID", "IsSystemObject", "QuotedIdentifierStatus", "AnsiNullsStatus", "FunctionType", "IsSchemaBound", "IsNativelyCompiled", "IsEncrypted" };
			List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
			supportedScriptFields.Add("Text");
			return supportedScriptFields.ToArray();
		}
		string[] fields2 = new string[26]
		{
			"ImplementationType", "ID", "DataTypeSchema", "SystemType", "Length", "NumericPrecision", "NumericScale", "IsSystemObject", "QuotedIdentifierStatus", "FunctionType",
			"IsSchemaBound", "IsNativelyCompiled", "IsEncrypted", "AnsiNullsStatus", "TableVariableName", "XmlSchemaNamespace", "XmlSchemaNamespaceSchema", "XmlDocumentConstraint", "ExecutionContext", "ExecutionContextPrincipal",
			"ReturnsNullOnNullInput", "AssemblyName", "MethodName", "ClassName", "Owner", "IsSchemaOwned"
		};
		List<string> supportedScriptFields2 = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields2, version, databaseEngineType, databaseEngineEdition);
		supportedScriptFields2.Add("Text");
		supportedScriptFields2.Add("DataType");
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
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(condition: false, "ServerVersion.Major == 7");
			break;
		case 8:
			switch (prop.Name)
			{
			case "FunctionType":
			case "IsSchemaBound":
			case "IsEncrypted":
			case "TableVariableName":
				ScriptNameObjectBase.Validate_set_TextObjectDDLProperty(prop, value);
				break;
			}
			break;
		default:
			switch (prop.Name)
			{
			case "FunctionType":
			case "IsSchemaBound":
			case "IsEncrypted":
			case "ExecutionContext":
			case "TableVariableName":
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
}
