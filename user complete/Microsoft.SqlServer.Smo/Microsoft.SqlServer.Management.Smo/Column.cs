using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class Column : ScriptNameObjectBase, ISfcSupportsDesignMode, IPropertyDataDispatch, ICreatable, IAlterable, IDroppable, IDropIfExists, IMarkForDrop, IExtendedProperties, IRenamable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 26, 27, 33, 37, 37, 38, 38, 46, 47, 47 };

		private static int[] cloudVersionCount = new int[3] { 37, 37, 46 };

		private static int sqlDwPropertyCount = 35;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[35]
		{
			new StaticMetadata("AnsiPaddingStatus", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Collation", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Computed", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ComputedText", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Default", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DefaultConstraintName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DefaultSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DistributionColumnName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("GeneratedAlwaysType", expensive: false, readOnly: false, typeof(GeneratedAlwaysType)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Identity", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IdentityIncrement", expensive: true, readOnly: false, typeof(long)),
			new StaticMetadata("IdentityIncrementAsDecimal", expensive: false, readOnly: false, typeof(decimal)),
			new StaticMetadata("IdentitySeed", expensive: true, readOnly: false, typeof(long)),
			new StaticMetadata("IdentitySeedAsDecimal", expensive: false, readOnly: false, typeof(decimal)),
			new StaticMetadata("InPrimaryKey", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsColumnSet", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsDistributedColumn", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsFileStream", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsForeignKey", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsHidden", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsPersisted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSparse", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NotForReplication", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Nullable", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("RowGuidCol", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Rule", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("RuleSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("UserType", expensive: false, readOnly: true, typeof(string))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[46]
		{
			new StaticMetadata("AnsiPaddingStatus", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Collation", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Computed", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ComputedText", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Default", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DefaultConstraintName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DefaultSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Identity", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IdentityIncrement", expensive: true, readOnly: false, typeof(long)),
			new StaticMetadata("IdentityIncrementAsDecimal", expensive: false, readOnly: false, typeof(decimal)),
			new StaticMetadata("IdentitySeed", expensive: true, readOnly: false, typeof(long)),
			new StaticMetadata("IdentitySeedAsDecimal", expensive: false, readOnly: false, typeof(decimal)),
			new StaticMetadata("InPrimaryKey", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsColumnSet", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsDeterministic", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsFileStream", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsForeignKey", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsFullTextIndexed", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsPersisted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsPrecise", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSparse", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NotForReplication", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Nullable", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("RowGuidCol", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Rule", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("RuleSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("UserType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("XmlDocumentConstraint", expensive: false, readOnly: false, typeof(XmlDocumentConstraint)),
			new StaticMetadata("XmlSchemaNamespace", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("XmlSchemaNamespaceSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ColumnEncryptionKeyID", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("ColumnEncryptionKeyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("EncryptionAlgorithm", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("EncryptionType", expensive: false, readOnly: false, typeof(ColumnEncryptionType)),
			new StaticMetadata("GeneratedAlwaysType", expensive: false, readOnly: false, typeof(GeneratedAlwaysType)),
			new StaticMetadata("GraphType", expensive: false, readOnly: false, typeof(GraphType)),
			new StaticMetadata("IsHidden", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsMasked", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("MaskingFunction", expensive: true, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[47]
		{
			new StaticMetadata("AnsiPaddingStatus", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Computed", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ComputedText", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Default", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DefaultConstraintName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DefaultSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Identity", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IdentityIncrement", expensive: true, readOnly: false, typeof(long)),
			new StaticMetadata("IdentityIncrementAsDecimal", expensive: false, readOnly: false, typeof(decimal)),
			new StaticMetadata("IdentitySeed", expensive: true, readOnly: false, typeof(long)),
			new StaticMetadata("IdentitySeedAsDecimal", expensive: false, readOnly: false, typeof(decimal)),
			new StaticMetadata("InPrimaryKey", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsForeignKey", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsFullTextIndexed", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NotForReplication", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Nullable", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("RowGuidCol", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Rule", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("RuleSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Collation", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("IsDeterministic", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsPersisted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsPrecise", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("XmlDocumentConstraint", expensive: false, readOnly: false, typeof(XmlDocumentConstraint)),
			new StaticMetadata("XmlSchemaNamespace", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("XmlSchemaNamespaceSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("IsColumnSet", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsFileStream", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSparse", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("UserType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("StatisticalSemantics", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ColumnEncryptionKeyID", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("ColumnEncryptionKeyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("EncryptionAlgorithm", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("EncryptionType", expensive: false, readOnly: false, typeof(ColumnEncryptionType)),
			new StaticMetadata("GeneratedAlwaysType", expensive: false, readOnly: false, typeof(GeneratedAlwaysType)),
			new StaticMetadata("IsHidden", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsMasked", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("MaskingFunction", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("GraphType", expensive: false, readOnly: false, typeof(GraphType))
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
						"AnsiPaddingStatus" => 0, 
						"Collation" => 1, 
						"Computed" => 2, 
						"ComputedText" => 3, 
						"DataType" => 4, 
						"DataTypeSchema" => 5, 
						"Default" => 6, 
						"DefaultConstraintName" => 7, 
						"DefaultSchema" => 8, 
						"DistributionColumnName" => 9, 
						"GeneratedAlwaysType" => 10, 
						"ID" => 11, 
						"Identity" => 12, 
						"IdentityIncrement" => 13, 
						"IdentityIncrementAsDecimal" => 14, 
						"IdentitySeed" => 15, 
						"IdentitySeedAsDecimal" => 16, 
						"InPrimaryKey" => 17, 
						"IsColumnSet" => 18, 
						"IsDistributedColumn" => 19, 
						"IsFileStream" => 20, 
						"IsForeignKey" => 21, 
						"IsHidden" => 22, 
						"IsPersisted" => 23, 
						"IsSparse" => 24, 
						"Length" => 25, 
						"NotForReplication" => 26, 
						"Nullable" => 27, 
						"NumericPrecision" => 28, 
						"NumericScale" => 29, 
						"RowGuidCol" => 30, 
						"Rule" => 31, 
						"RuleSchema" => 32, 
						"SystemType" => 33, 
						"UserType" => 34, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"AnsiPaddingStatus" => 0, 
					"Collation" => 1, 
					"Computed" => 2, 
					"ComputedText" => 3, 
					"DataType" => 4, 
					"DataTypeSchema" => 5, 
					"Default" => 6, 
					"DefaultConstraintName" => 7, 
					"DefaultSchema" => 8, 
					"ID" => 9, 
					"Identity" => 10, 
					"IdentityIncrement" => 11, 
					"IdentityIncrementAsDecimal" => 12, 
					"IdentitySeed" => 13, 
					"IdentitySeedAsDecimal" => 14, 
					"InPrimaryKey" => 15, 
					"IsColumnSet" => 16, 
					"IsDeterministic" => 17, 
					"IsFileStream" => 18, 
					"IsForeignKey" => 19, 
					"IsFullTextIndexed" => 20, 
					"IsPersisted" => 21, 
					"IsPrecise" => 22, 
					"IsSparse" => 23, 
					"Length" => 24, 
					"NotForReplication" => 25, 
					"Nullable" => 26, 
					"NumericPrecision" => 27, 
					"NumericScale" => 28, 
					"RowGuidCol" => 29, 
					"Rule" => 30, 
					"RuleSchema" => 31, 
					"SystemType" => 32, 
					"UserType" => 33, 
					"XmlDocumentConstraint" => 34, 
					"XmlSchemaNamespace" => 35, 
					"XmlSchemaNamespaceSchema" => 36, 
					"ColumnEncryptionKeyID" => 37, 
					"ColumnEncryptionKeyName" => 38, 
					"EncryptionAlgorithm" => 39, 
					"EncryptionType" => 40, 
					"GeneratedAlwaysType" => 41, 
					"GraphType" => 42, 
					"IsHidden" => 43, 
					"IsMasked" => 44, 
					"MaskingFunction" => 45, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"AnsiPaddingStatus" => 0, 
				"Computed" => 1, 
				"ComputedText" => 2, 
				"DataType" => 3, 
				"DataTypeSchema" => 4, 
				"Default" => 5, 
				"DefaultConstraintName" => 6, 
				"DefaultSchema" => 7, 
				"ID" => 8, 
				"Identity" => 9, 
				"IdentityIncrement" => 10, 
				"IdentityIncrementAsDecimal" => 11, 
				"IdentitySeed" => 12, 
				"IdentitySeedAsDecimal" => 13, 
				"InPrimaryKey" => 14, 
				"IsForeignKey" => 15, 
				"IsFullTextIndexed" => 16, 
				"Length" => 17, 
				"NotForReplication" => 18, 
				"Nullable" => 19, 
				"NumericPrecision" => 20, 
				"NumericScale" => 21, 
				"RowGuidCol" => 22, 
				"Rule" => 23, 
				"RuleSchema" => 24, 
				"SystemType" => 25, 
				"Collation" => 26, 
				"IsDeterministic" => 27, 
				"IsPersisted" => 28, 
				"IsPrecise" => 29, 
				"XmlDocumentConstraint" => 30, 
				"XmlSchemaNamespace" => 31, 
				"XmlSchemaNamespaceSchema" => 32, 
				"IsColumnSet" => 33, 
				"IsFileStream" => 34, 
				"IsSparse" => 35, 
				"UserType" => 36, 
				"StatisticalSemantics" => 37, 
				"ColumnEncryptionKeyID" => 38, 
				"ColumnEncryptionKeyName" => 39, 
				"EncryptionAlgorithm" => 40, 
				"EncryptionType" => 41, 
				"GeneratedAlwaysType" => 42, 
				"IsHidden" => 43, 
				"IsMasked" => 44, 
				"MaskingFunction" => 45, 
				"GraphType" => 46, 
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

	private sealed class XSchemaProps
	{
		private bool _AnsiPaddingStatus;

		private string _Collation;

		private bool _Computed;

		private string _ComputedText;

		private string _DataType;

		private string _DataTypeSchema;

		private string _Default;

		private string _DefaultSchema;

		private bool _Identity;

		private long _IdentityIncrement;

		private decimal _IdentityIncrementAsDecimal;

		private long _IdentitySeed;

		private decimal _IdentitySeedAsDecimal;

		private bool _IsColumnSet;

		private bool _IsPersisted;

		private bool _IsSparse;

		private int _Length;

		private bool _NotForReplication;

		private bool _Nullable;

		private int _NumericPrecision;

		private int _NumericScale;

		private bool _RowGuidCol;

		private string _Rule;

		private string _RuleSchema;

		private string _SystemType;

		private string _UserType;

		private XmlDocumentConstraint _XmlDocumentConstraint;

		private string _XmlSchemaNamespace;

		private string _XmlSchemaNamespaceSchema;

		private string _DistributionColumnName;

		private bool _IsDistributedColumn;

		internal bool AnsiPaddingStatus
		{
			get
			{
				return _AnsiPaddingStatus;
			}
			set
			{
				_AnsiPaddingStatus = value;
			}
		}

		internal string Collation
		{
			get
			{
				return _Collation;
			}
			set
			{
				_Collation = value;
			}
		}

		internal bool Computed
		{
			get
			{
				return _Computed;
			}
			set
			{
				_Computed = value;
			}
		}

		internal string ComputedText
		{
			get
			{
				return _ComputedText;
			}
			set
			{
				_ComputedText = value;
			}
		}

		internal string DataType
		{
			get
			{
				return _DataType;
			}
			set
			{
				_DataType = value;
			}
		}

		internal string DataTypeSchema
		{
			get
			{
				return _DataTypeSchema;
			}
			set
			{
				_DataTypeSchema = value;
			}
		}

		internal string Default
		{
			get
			{
				return _Default;
			}
			set
			{
				_Default = value;
			}
		}

		internal string DefaultSchema
		{
			get
			{
				return _DefaultSchema;
			}
			set
			{
				_DefaultSchema = value;
			}
		}

		internal bool Identity
		{
			get
			{
				return _Identity;
			}
			set
			{
				_Identity = value;
			}
		}

		internal long IdentityIncrement
		{
			get
			{
				return _IdentityIncrement;
			}
			set
			{
				_IdentityIncrement = value;
			}
		}

		internal decimal IdentityIncrementAsDecimal
		{
			get
			{
				return _IdentityIncrementAsDecimal;
			}
			set
			{
				_IdentityIncrementAsDecimal = value;
			}
		}

		internal long IdentitySeed
		{
			get
			{
				return _IdentitySeed;
			}
			set
			{
				_IdentitySeed = value;
			}
		}

		internal decimal IdentitySeedAsDecimal
		{
			get
			{
				return _IdentitySeedAsDecimal;
			}
			set
			{
				_IdentitySeedAsDecimal = value;
			}
		}

		internal bool IsColumnSet
		{
			get
			{
				return _IsColumnSet;
			}
			set
			{
				_IsColumnSet = value;
			}
		}

		internal bool IsPersisted
		{
			get
			{
				return _IsPersisted;
			}
			set
			{
				_IsPersisted = value;
			}
		}

		internal bool IsSparse
		{
			get
			{
				return _IsSparse;
			}
			set
			{
				_IsSparse = value;
			}
		}

		internal int Length
		{
			get
			{
				return _Length;
			}
			set
			{
				_Length = value;
			}
		}

		internal bool NotForReplication
		{
			get
			{
				return _NotForReplication;
			}
			set
			{
				_NotForReplication = value;
			}
		}

		internal bool Nullable
		{
			get
			{
				return _Nullable;
			}
			set
			{
				_Nullable = value;
			}
		}

		internal int NumericPrecision
		{
			get
			{
				return _NumericPrecision;
			}
			set
			{
				_NumericPrecision = value;
			}
		}

		internal int NumericScale
		{
			get
			{
				return _NumericScale;
			}
			set
			{
				_NumericScale = value;
			}
		}

		internal bool RowGuidCol
		{
			get
			{
				return _RowGuidCol;
			}
			set
			{
				_RowGuidCol = value;
			}
		}

		internal string Rule
		{
			get
			{
				return _Rule;
			}
			set
			{
				_Rule = value;
			}
		}

		internal string RuleSchema
		{
			get
			{
				return _RuleSchema;
			}
			set
			{
				_RuleSchema = value;
			}
		}

		internal string SystemType
		{
			get
			{
				return _SystemType;
			}
			set
			{
				_SystemType = value;
			}
		}

		internal string UserType
		{
			get
			{
				return _UserType;
			}
			set
			{
				_UserType = value;
			}
		}

		internal XmlDocumentConstraint XmlDocumentConstraint
		{
			get
			{
				return _XmlDocumentConstraint;
			}
			set
			{
				_XmlDocumentConstraint = value;
			}
		}

		internal string XmlSchemaNamespace
		{
			get
			{
				return _XmlSchemaNamespace;
			}
			set
			{
				_XmlSchemaNamespace = value;
			}
		}

		internal string XmlSchemaNamespaceSchema
		{
			get
			{
				return _XmlSchemaNamespaceSchema;
			}
			set
			{
				_XmlSchemaNamespaceSchema = value;
			}
		}

		internal string DistributionColumnName
		{
			get
			{
				return _DistributionColumnName;
			}
			set
			{
				_DistributionColumnName = value;
			}
		}

		internal bool IsDistributedColumn
		{
			get
			{
				return _IsDistributedColumn;
			}
			set
			{
				_IsDistributedColumn = value;
			}
		}
	}

	private sealed class XRuntimeProps
	{
		private int _ColumnEncryptionKeyID;

		private string _ColumnEncryptionKeyName;

		private string _DefaultConstraintName;

		private string _EncryptionAlgorithm;

		private ColumnEncryptionType _EncryptionType;

		private GeneratedAlwaysType _GeneratedAlwaysType;

		private GraphType _GraphType;

		private int _ID;

		private bool _InPrimaryKey;

		private bool _IsDeterministic;

		private bool _IsFileStream;

		private bool _IsForeignKey;

		private bool _IsFullTextIndexed;

		private bool _IsHidden;

		private bool _IsMasked;

		private bool _IsPrecise;

		private string _MaskingFunction;

		private int _StatisticalSemantics;

		internal int ColumnEncryptionKeyID
		{
			get
			{
				return _ColumnEncryptionKeyID;
			}
			set
			{
				_ColumnEncryptionKeyID = value;
			}
		}

		internal string ColumnEncryptionKeyName
		{
			get
			{
				return _ColumnEncryptionKeyName;
			}
			set
			{
				_ColumnEncryptionKeyName = value;
			}
		}

		internal string DefaultConstraintName
		{
			get
			{
				return _DefaultConstraintName;
			}
			set
			{
				_DefaultConstraintName = value;
			}
		}

		internal string EncryptionAlgorithm
		{
			get
			{
				return _EncryptionAlgorithm;
			}
			set
			{
				_EncryptionAlgorithm = value;
			}
		}

		internal ColumnEncryptionType EncryptionType
		{
			get
			{
				return _EncryptionType;
			}
			set
			{
				_EncryptionType = value;
			}
		}

		internal GeneratedAlwaysType GeneratedAlwaysType
		{
			get
			{
				return _GeneratedAlwaysType;
			}
			set
			{
				_GeneratedAlwaysType = value;
			}
		}

		internal GraphType GraphType
		{
			get
			{
				return _GraphType;
			}
			set
			{
				_GraphType = value;
			}
		}

		internal int ID
		{
			get
			{
				return _ID;
			}
			set
			{
				_ID = value;
			}
		}

		internal bool InPrimaryKey
		{
			get
			{
				return _InPrimaryKey;
			}
			set
			{
				_InPrimaryKey = value;
			}
		}

		internal bool IsDeterministic
		{
			get
			{
				return _IsDeterministic;
			}
			set
			{
				_IsDeterministic = value;
			}
		}

		internal bool IsFileStream
		{
			get
			{
				return _IsFileStream;
			}
			set
			{
				_IsFileStream = value;
			}
		}

		internal bool IsForeignKey
		{
			get
			{
				return _IsForeignKey;
			}
			set
			{
				_IsForeignKey = value;
			}
		}

		internal bool IsFullTextIndexed
		{
			get
			{
				return _IsFullTextIndexed;
			}
			set
			{
				_IsFullTextIndexed = value;
			}
		}

		internal bool IsHidden
		{
			get
			{
				return _IsHidden;
			}
			set
			{
				_IsHidden = value;
			}
		}

		internal bool IsMasked
		{
			get
			{
				return _IsMasked;
			}
			set
			{
				_IsMasked = value;
			}
		}

		internal bool IsPrecise
		{
			get
			{
				return _IsPrecise;
			}
			set
			{
				_IsPrecise = value;
			}
		}

		internal string MaskingFunction
		{
			get
			{
				return _MaskingFunction;
			}
			set
			{
				_MaskingFunction = value;
			}
		}

		internal int StatisticalSemantics
		{
			get
			{
				return _StatisticalSemantics;
			}
			set
			{
				_StatisticalSemantics = value;
			}
		}
	}

	internal class MaskingFunctionValidation
	{
		public string Pattern { get; set; }

		public bool IsValidForNumeric { get; set; }

		public bool IsValidForString { get; set; }

		public bool IsValidForOther { get; set; }
	}

	private XSchemaProps _XSchema;

	private XRuntimeProps _XRuntime;

	private DataType dataType;

	private DefaultConstraint defaultConstraint;

	internal bool m_bDefaultInitialized;

	internal object oldRowGuidColValue;

	private static MaskingFunctionValidation[] MaskingFunctionsValidationTable = new MaskingFunctionValidation[4]
	{
		new MaskingFunctionValidation
		{
			Pattern = "^default\\(\\s*\\)$",
			IsValidForNumeric = true,
			IsValidForString = true,
			IsValidForOther = true
		},
		new MaskingFunctionValidation
		{
			Pattern = "^email\\(\\s*\\)$",
			IsValidForNumeric = false,
			IsValidForString = true,
			IsValidForOther = false
		},
		new MaskingFunctionValidation
		{
			Pattern = "^partial\\(\\s*\\d+\\s*,\\s*\"[^\"]*\"\\s*,\\s*\\d+\\s*\\)$",
			IsValidForNumeric = false,
			IsValidForString = true,
			IsValidForOther = false
		},
		new MaskingFunctionValidation
		{
			Pattern = "^random\\(\\s*-?\\d+.?\\d*\\s*,\\s*-?\\d+.?\\d*\\s*\\)$",
			IsValidForNumeric = true,
			IsValidForString = false,
			IsValidForOther = false
		}
	};

	[SfcParent("Default")]
	[SfcParent("Statistic")]
	[SfcParent("ForeignKey")]
	[SfcParent("Table")]
	[SfcParent("View")]
	[SfcParent("Rule")]
	[SfcParent("UserDefinedTableType")]
	[SfcParent("UserDefinedFunction")]
	[SfcObject(SfcObjectRelationship.ParentObject, SfcObjectFlags.Design)]
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

	private XSchemaProps XSchema
	{
		get
		{
			if (_XSchema == null)
			{
				_XSchema = new XSchemaProps();
			}
			return _XSchema;
		}
	}

	private XRuntimeProps XRuntime
	{
		get
		{
			if (_XRuntime == null)
			{
				_XRuntime = new XRuntimeProps();
			}
			return _XRuntime;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool AnsiPaddingStatus => (bool)base.Properties.GetValueWithNullReplacement("AnsiPaddingStatus");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string Collation
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Collation");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Collation", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ColumnEncryptionKeyID
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("ColumnEncryptionKeyID");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ColumnEncryptionKeyID", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ColumnEncryptionKeyName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ColumnEncryptionKeyName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ColumnEncryptionKeyName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool Computed
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Computed");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Computed", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string ComputedText
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ComputedText");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ComputedText", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[CLSCompliant(false)]
	[SfcReference(typeof(Default), "Server[@Name = '{0}']/Database[@Name = '{1}']/Default[@Name='{2}' and @Schema='{3}']", new string[] { "Parent.Parent.Parent.ConnectionContext.TrueName", "Parent.Parent.Name", "Default", "DefaultSchema" })]
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

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string DefaultConstraintName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DefaultConstraintName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DefaultConstraintName", value);
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

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string EncryptionAlgorithm
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("EncryptionAlgorithm");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EncryptionAlgorithm", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public ColumnEncryptionType EncryptionType
	{
		get
		{
			return (ColumnEncryptionType)base.Properties.GetValueWithNullReplacement("EncryptionType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EncryptionType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public GeneratedAlwaysType GeneratedAlwaysType
	{
		get
		{
			return (GeneratedAlwaysType)base.Properties.GetValueWithNullReplacement("GeneratedAlwaysType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("GeneratedAlwaysType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public GraphType GraphType
	{
		get
		{
			return (GraphType)base.Properties.GetValueWithNullReplacement("GraphType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("GraphType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool Identity
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Identity");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Identity", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public long IdentityIncrement
	{
		get
		{
			return (long)base.Properties.GetValueWithNullReplacement("IdentityIncrement");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IdentityIncrement", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public decimal IdentityIncrementAsDecimal
	{
		get
		{
			return (decimal)base.Properties.GetValueWithNullReplacement("IdentityIncrementAsDecimal");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IdentityIncrementAsDecimal", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public long IdentitySeed
	{
		get
		{
			return (long)base.Properties.GetValueWithNullReplacement("IdentitySeed");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IdentitySeed", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public decimal IdentitySeedAsDecimal
	{
		get
		{
			return (decimal)base.Properties.GetValueWithNullReplacement("IdentitySeedAsDecimal");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IdentitySeedAsDecimal", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool InPrimaryKey => (bool)base.Properties.GetValueWithNullReplacement("InPrimaryKey");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsColumnSet
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsColumnSet");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsColumnSet", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDeterministic => (bool)base.Properties.GetValueWithNullReplacement("IsDeterministic");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsFileStream
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsFileStream");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsFileStream", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsForeignKey => (bool)base.Properties.GetValueWithNullReplacement("IsForeignKey");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsFullTextIndexed => (bool)base.Properties.GetValueWithNullReplacement("IsFullTextIndexed");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsHidden
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsHidden");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsHidden", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsMasked
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsMasked");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsMasked", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsPersisted
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsPersisted");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsPersisted", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsPrecise => (bool)base.Properties.GetValueWithNullReplacement("IsPrecise");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsSparse
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsSparse");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsSparse", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string MaskingFunction
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("MaskingFunction");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaskingFunction", value);
		}
	}

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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool RowGuidCol
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("RowGuidCol");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RowGuidCol", value);
		}
	}

	[SfcReference(typeof(Rule), "Server[@Name = '{0}']/Database[@Name = '{1}']/Rule[@Name='{2}' and @Schema='{3}']", new string[] { "Parent.Parent.Parent.ConnectionContext.TrueName", "Parent.Parent.Name", "Rule", "RuleSchema" })]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int StatisticalSemantics => (int)base.Properties.GetValueWithNullReplacement("StatisticalSemantics");

	[SfcProperty]
	public string DistributionColumnName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DistributionColumnName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DistributionColumnName", value);
		}
	}

	[SfcProperty]
	public bool IsDistributedColumn
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsDistributedColumn");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsDistributedColumn", value);
		}
	}

	public static string UrnSuffix => "Column";

	internal override UserPermissionCollection Permissions => GetUserPermissions();

	internal SqlDataType UnderlyingSqlDataType
	{
		get
		{
			SqlDataType sqlDataType = DataType.SqlDataType;
			if (sqlDataType != SqlDataType.UserDefinedDataType)
			{
				return DataType.SqlDataType;
			}
			Server serverObject = GetServerObject();
			Database database = serverObject.Databases[GetDBName()];
			UserDefinedDataType uddt = database.UserDefinedDataTypes[DataType.Name, DataType.Schema];
			return DataType.UserDefinedDataTypeToEnum(uddt);
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

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcKey(0)]
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
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcReference(typeof(UserDefinedType), typeof(UserDefinedTypeResolver), "Resolve", new string[] { })]
	[SfcReference(typeof(UserDefinedDataType), typeof(UserDefinedDataTypeResolver), "Resolve", new string[] { })]
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

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsEncrypted
	{
		get
		{
			if (IsSupportedProperty("ColumnEncryptionKeyID"))
			{
				return null != GetPropValueOptionalAllowNull("ColumnEncryptionKeyID");
			}
			return false;
		}
	}

	internal bool UserDefinedDefault
	{
		get
		{
			CheckObjectState();
			return null != DefaultConstraint;
		}
	}

	[SfcObject(SfcObjectRelationship.ChildObject, SfcObjectCardinality.ZeroToOne, SfcObjectFlags.Design)]
	public DefaultConstraint DefaultConstraint
	{
		get
		{
			InitDefaultConstraint();
			return defaultConstraint;
		}
		internal set
		{
			defaultConstraint = value;
			DefaultConstraintName = ((defaultConstraint == null) ? string.Empty : defaultConstraint.Name);
		}
	}

	internal override string ScriptName
	{
		get
		{
			return base.ScriptName;
		}
		set
		{
			((ScriptSchemaObjectBase)base.ParentColl.ParentInstance).CheckTextModeAccess("ScriptName");
			base.ScriptName = value;
		}
	}

	public Column()
	{
	}

	public Column(SqlSmoObject parent, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = parent;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	object IPropertyDataDispatch.GetPropertyValue(int index)
	{
		if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			if (DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
			{
				return index switch
				{
					0 => XSchema.AnsiPaddingStatus, 
					1 => XSchema.Collation, 
					2 => XSchema.Computed, 
					3 => XSchema.ComputedText, 
					4 => XSchema.DataType, 
					5 => XSchema.DataTypeSchema, 
					6 => XSchema.Default, 
					7 => XRuntime.DefaultConstraintName, 
					8 => XSchema.DefaultSchema, 
					9 => XSchema.DistributionColumnName, 
					10 => XRuntime.GeneratedAlwaysType, 
					11 => XRuntime.ID, 
					12 => XSchema.Identity, 
					13 => XSchema.IdentityIncrement, 
					14 => XSchema.IdentityIncrementAsDecimal, 
					15 => XSchema.IdentitySeed, 
					16 => XSchema.IdentitySeedAsDecimal, 
					17 => XRuntime.InPrimaryKey, 
					18 => XSchema.IsColumnSet, 
					19 => XSchema.IsDistributedColumn, 
					20 => XRuntime.IsFileStream, 
					21 => XRuntime.IsForeignKey, 
					22 => XRuntime.IsHidden, 
					23 => XSchema.IsPersisted, 
					24 => XSchema.IsSparse, 
					25 => XSchema.Length, 
					26 => XSchema.NotForReplication, 
					27 => XSchema.Nullable, 
					28 => XSchema.NumericPrecision, 
					29 => XSchema.NumericScale, 
					30 => XSchema.RowGuidCol, 
					31 => XSchema.Rule, 
					32 => XSchema.RuleSchema, 
					33 => XSchema.SystemType, 
					34 => XSchema.UserType, 
					_ => throw new IndexOutOfRangeException(), 
				};
			}
			return index switch
			{
				0 => XSchema.AnsiPaddingStatus, 
				1 => XSchema.Collation, 
				37 => XRuntime.ColumnEncryptionKeyID, 
				38 => XRuntime.ColumnEncryptionKeyName, 
				2 => XSchema.Computed, 
				3 => XSchema.ComputedText, 
				4 => XSchema.DataType, 
				5 => XSchema.DataTypeSchema, 
				6 => XSchema.Default, 
				7 => XRuntime.DefaultConstraintName, 
				8 => XSchema.DefaultSchema, 
				39 => XRuntime.EncryptionAlgorithm, 
				40 => XRuntime.EncryptionType, 
				41 => XRuntime.GeneratedAlwaysType, 
				42 => XRuntime.GraphType, 
				9 => XRuntime.ID, 
				10 => XSchema.Identity, 
				11 => XSchema.IdentityIncrement, 
				12 => XSchema.IdentityIncrementAsDecimal, 
				13 => XSchema.IdentitySeed, 
				14 => XSchema.IdentitySeedAsDecimal, 
				15 => XRuntime.InPrimaryKey, 
				16 => XSchema.IsColumnSet, 
				17 => XRuntime.IsDeterministic, 
				18 => XRuntime.IsFileStream, 
				19 => XRuntime.IsForeignKey, 
				20 => XRuntime.IsFullTextIndexed, 
				43 => XRuntime.IsHidden, 
				44 => XRuntime.IsMasked, 
				21 => XSchema.IsPersisted, 
				22 => XRuntime.IsPrecise, 
				23 => XSchema.IsSparse, 
				24 => XSchema.Length, 
				45 => XRuntime.MaskingFunction, 
				25 => XSchema.NotForReplication, 
				26 => XSchema.Nullable, 
				27 => XSchema.NumericPrecision, 
				28 => XSchema.NumericScale, 
				29 => XSchema.RowGuidCol, 
				30 => XSchema.Rule, 
				31 => XSchema.RuleSchema, 
				32 => XSchema.SystemType, 
				33 => XSchema.UserType, 
				34 => XSchema.XmlDocumentConstraint, 
				35 => XSchema.XmlSchemaNamespace, 
				36 => XSchema.XmlSchemaNamespaceSchema, 
				_ => throw new IndexOutOfRangeException(), 
			};
		}
		return index switch
		{
			0 => XSchema.AnsiPaddingStatus, 
			26 => XSchema.Collation, 
			38 => XRuntime.ColumnEncryptionKeyID, 
			39 => XRuntime.ColumnEncryptionKeyName, 
			1 => XSchema.Computed, 
			2 => XSchema.ComputedText, 
			3 => XSchema.DataType, 
			4 => XSchema.DataTypeSchema, 
			5 => XSchema.Default, 
			6 => XRuntime.DefaultConstraintName, 
			7 => XSchema.DefaultSchema, 
			40 => XRuntime.EncryptionAlgorithm, 
			41 => XRuntime.EncryptionType, 
			42 => XRuntime.GeneratedAlwaysType, 
			46 => XRuntime.GraphType, 
			8 => XRuntime.ID, 
			9 => XSchema.Identity, 
			10 => XSchema.IdentityIncrement, 
			11 => XSchema.IdentityIncrementAsDecimal, 
			12 => XSchema.IdentitySeed, 
			13 => XSchema.IdentitySeedAsDecimal, 
			14 => XRuntime.InPrimaryKey, 
			33 => XSchema.IsColumnSet, 
			27 => XRuntime.IsDeterministic, 
			34 => XRuntime.IsFileStream, 
			15 => XRuntime.IsForeignKey, 
			16 => XRuntime.IsFullTextIndexed, 
			43 => XRuntime.IsHidden, 
			44 => XRuntime.IsMasked, 
			28 => XSchema.IsPersisted, 
			29 => XRuntime.IsPrecise, 
			35 => XSchema.IsSparse, 
			17 => XSchema.Length, 
			45 => XRuntime.MaskingFunction, 
			18 => XSchema.NotForReplication, 
			19 => XSchema.Nullable, 
			20 => XSchema.NumericPrecision, 
			21 => XSchema.NumericScale, 
			22 => XSchema.RowGuidCol, 
			23 => XSchema.Rule, 
			24 => XSchema.RuleSchema, 
			37 => XRuntime.StatisticalSemantics, 
			25 => XSchema.SystemType, 
			36 => XSchema.UserType, 
			30 => XSchema.XmlDocumentConstraint, 
			31 => XSchema.XmlSchemaNamespace, 
			32 => XSchema.XmlSchemaNamespaceSchema, 
			_ => throw new IndexOutOfRangeException(), 
		};
	}

	void IPropertyDataDispatch.SetPropertyValue(int index, object value)
	{
		if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			if (DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
			{
				switch (index)
				{
				case 0:
					XSchema.AnsiPaddingStatus = (bool)value;
					break;
				case 1:
					XSchema.Collation = (string)value;
					break;
				case 2:
					XSchema.Computed = (bool)value;
					break;
				case 3:
					XSchema.ComputedText = (string)value;
					break;
				case 4:
					XSchema.DataType = (string)value;
					break;
				case 5:
					XSchema.DataTypeSchema = (string)value;
					break;
				case 6:
					XSchema.Default = (string)value;
					break;
				case 7:
					XRuntime.DefaultConstraintName = (string)value;
					break;
				case 8:
					XSchema.DefaultSchema = (string)value;
					break;
				case 9:
					XSchema.DistributionColumnName = (string)value;
					break;
				case 10:
					XRuntime.GeneratedAlwaysType = (GeneratedAlwaysType)value;
					break;
				case 11:
					XRuntime.ID = (int)value;
					break;
				case 12:
					XSchema.Identity = (bool)value;
					break;
				case 13:
					XSchema.IdentityIncrement = (long)value;
					break;
				case 14:
					XSchema.IdentityIncrementAsDecimal = (decimal)value;
					break;
				case 15:
					XSchema.IdentitySeed = (long)value;
					break;
				case 16:
					XSchema.IdentitySeedAsDecimal = (decimal)value;
					break;
				case 17:
					XRuntime.InPrimaryKey = (bool)value;
					break;
				case 18:
					XSchema.IsColumnSet = (bool)value;
					break;
				case 19:
					XSchema.IsDistributedColumn = (bool)value;
					break;
				case 20:
					XRuntime.IsFileStream = (bool)value;
					break;
				case 21:
					XRuntime.IsForeignKey = (bool)value;
					break;
				case 22:
					XRuntime.IsHidden = (bool)value;
					break;
				case 23:
					XSchema.IsPersisted = (bool)value;
					break;
				case 24:
					XSchema.IsSparse = (bool)value;
					break;
				case 25:
					XSchema.Length = (int)value;
					break;
				case 26:
					XSchema.NotForReplication = (bool)value;
					break;
				case 27:
					XSchema.Nullable = (bool)value;
					break;
				case 28:
					XSchema.NumericPrecision = (int)value;
					break;
				case 29:
					XSchema.NumericScale = (int)value;
					break;
				case 30:
					XSchema.RowGuidCol = (bool)value;
					break;
				case 31:
					XSchema.Rule = (string)value;
					break;
				case 32:
					XSchema.RuleSchema = (string)value;
					break;
				case 33:
					XSchema.SystemType = (string)value;
					break;
				case 34:
					XSchema.UserType = (string)value;
					break;
				default:
					throw new IndexOutOfRangeException();
				}
				return;
			}
			switch (index)
			{
			case 0:
				XSchema.AnsiPaddingStatus = (bool)value;
				break;
			case 1:
				XSchema.Collation = (string)value;
				break;
			case 37:
				XRuntime.ColumnEncryptionKeyID = (int)value;
				break;
			case 38:
				XRuntime.ColumnEncryptionKeyName = (string)value;
				break;
			case 2:
				XSchema.Computed = (bool)value;
				break;
			case 3:
				XSchema.ComputedText = (string)value;
				break;
			case 4:
				XSchema.DataType = (string)value;
				break;
			case 5:
				XSchema.DataTypeSchema = (string)value;
				break;
			case 6:
				XSchema.Default = (string)value;
				break;
			case 7:
				XRuntime.DefaultConstraintName = (string)value;
				break;
			case 8:
				XSchema.DefaultSchema = (string)value;
				break;
			case 39:
				XRuntime.EncryptionAlgorithm = (string)value;
				break;
			case 40:
				XRuntime.EncryptionType = (ColumnEncryptionType)value;
				break;
			case 41:
				XRuntime.GeneratedAlwaysType = (GeneratedAlwaysType)value;
				break;
			case 42:
				XRuntime.GraphType = (GraphType)value;
				break;
			case 9:
				XRuntime.ID = (int)value;
				break;
			case 10:
				XSchema.Identity = (bool)value;
				break;
			case 11:
				XSchema.IdentityIncrement = (long)value;
				break;
			case 12:
				XSchema.IdentityIncrementAsDecimal = (decimal)value;
				break;
			case 13:
				XSchema.IdentitySeed = (long)value;
				break;
			case 14:
				XSchema.IdentitySeedAsDecimal = (decimal)value;
				break;
			case 15:
				XRuntime.InPrimaryKey = (bool)value;
				break;
			case 16:
				XSchema.IsColumnSet = (bool)value;
				break;
			case 17:
				XRuntime.IsDeterministic = (bool)value;
				break;
			case 18:
				XRuntime.IsFileStream = (bool)value;
				break;
			case 19:
				XRuntime.IsForeignKey = (bool)value;
				break;
			case 20:
				XRuntime.IsFullTextIndexed = (bool)value;
				break;
			case 43:
				XRuntime.IsHidden = (bool)value;
				break;
			case 44:
				XRuntime.IsMasked = (bool)value;
				break;
			case 21:
				XSchema.IsPersisted = (bool)value;
				break;
			case 22:
				XRuntime.IsPrecise = (bool)value;
				break;
			case 23:
				XSchema.IsSparse = (bool)value;
				break;
			case 24:
				XSchema.Length = (int)value;
				break;
			case 45:
				XRuntime.MaskingFunction = (string)value;
				break;
			case 25:
				XSchema.NotForReplication = (bool)value;
				break;
			case 26:
				XSchema.Nullable = (bool)value;
				break;
			case 27:
				XSchema.NumericPrecision = (int)value;
				break;
			case 28:
				XSchema.NumericScale = (int)value;
				break;
			case 29:
				XSchema.RowGuidCol = (bool)value;
				break;
			case 30:
				XSchema.Rule = (string)value;
				break;
			case 31:
				XSchema.RuleSchema = (string)value;
				break;
			case 32:
				XSchema.SystemType = (string)value;
				break;
			case 33:
				XSchema.UserType = (string)value;
				break;
			case 34:
				XSchema.XmlDocumentConstraint = (XmlDocumentConstraint)value;
				break;
			case 35:
				XSchema.XmlSchemaNamespace = (string)value;
				break;
			case 36:
				XSchema.XmlSchemaNamespaceSchema = (string)value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
		else
		{
			switch (index)
			{
			case 0:
				XSchema.AnsiPaddingStatus = (bool)value;
				break;
			case 26:
				XSchema.Collation = (string)value;
				break;
			case 38:
				XRuntime.ColumnEncryptionKeyID = (int)value;
				break;
			case 39:
				XRuntime.ColumnEncryptionKeyName = (string)value;
				break;
			case 1:
				XSchema.Computed = (bool)value;
				break;
			case 2:
				XSchema.ComputedText = (string)value;
				break;
			case 3:
				XSchema.DataType = (string)value;
				break;
			case 4:
				XSchema.DataTypeSchema = (string)value;
				break;
			case 5:
				XSchema.Default = (string)value;
				break;
			case 6:
				XRuntime.DefaultConstraintName = (string)value;
				break;
			case 7:
				XSchema.DefaultSchema = (string)value;
				break;
			case 40:
				XRuntime.EncryptionAlgorithm = (string)value;
				break;
			case 41:
				XRuntime.EncryptionType = (ColumnEncryptionType)value;
				break;
			case 42:
				XRuntime.GeneratedAlwaysType = (GeneratedAlwaysType)value;
				break;
			case 46:
				XRuntime.GraphType = (GraphType)value;
				break;
			case 8:
				XRuntime.ID = (int)value;
				break;
			case 9:
				XSchema.Identity = (bool)value;
				break;
			case 10:
				XSchema.IdentityIncrement = (long)value;
				break;
			case 11:
				XSchema.IdentityIncrementAsDecimal = (decimal)value;
				break;
			case 12:
				XSchema.IdentitySeed = (long)value;
				break;
			case 13:
				XSchema.IdentitySeedAsDecimal = (decimal)value;
				break;
			case 14:
				XRuntime.InPrimaryKey = (bool)value;
				break;
			case 33:
				XSchema.IsColumnSet = (bool)value;
				break;
			case 27:
				XRuntime.IsDeterministic = (bool)value;
				break;
			case 34:
				XRuntime.IsFileStream = (bool)value;
				break;
			case 15:
				XRuntime.IsForeignKey = (bool)value;
				break;
			case 16:
				XRuntime.IsFullTextIndexed = (bool)value;
				break;
			case 43:
				XRuntime.IsHidden = (bool)value;
				break;
			case 44:
				XRuntime.IsMasked = (bool)value;
				break;
			case 28:
				XSchema.IsPersisted = (bool)value;
				break;
			case 29:
				XRuntime.IsPrecise = (bool)value;
				break;
			case 35:
				XSchema.IsSparse = (bool)value;
				break;
			case 17:
				XSchema.Length = (int)value;
				break;
			case 45:
				XRuntime.MaskingFunction = (string)value;
				break;
			case 18:
				XSchema.NotForReplication = (bool)value;
				break;
			case 19:
				XSchema.Nullable = (bool)value;
				break;
			case 20:
				XSchema.NumericPrecision = (int)value;
				break;
			case 21:
				XSchema.NumericScale = (int)value;
				break;
			case 22:
				XSchema.RowGuidCol = (bool)value;
				break;
			case 23:
				XSchema.Rule = (string)value;
				break;
			case 24:
				XSchema.RuleSchema = (string)value;
				break;
			case 37:
				XRuntime.StatisticalSemantics = (int)value;
				break;
			case 25:
				XSchema.SystemType = (string)value;
				break;
			case 36:
				XSchema.UserType = (string)value;
				break;
			case 30:
				XSchema.XmlDocumentConstraint = (XmlDocumentConstraint)value;
				break;
			case 31:
				XSchema.XmlSchemaNamespace = (string)value;
				break;
			case 32:
				XSchema.XmlSchemaNamespaceSchema = (string)value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[19]
		{
			"ColumnEncryptionKeyID", "ColumnEncryptionKeyName", "Computed", "ComputedText", "Default", "DefaultConstraintName", "DefaultSchema", "EncryptionAlgorithm", "EncryptionType", "Identity",
			"IdentityIncrement", "IdentityIncrementAsDecimal", "IdentitySeed", "IdentitySeedAsDecimal", "IsColumnSet", "IsFileStream", "NotForReplication", "Rule", "RuleSchema"
		};
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

	internal Column(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		m_bDefaultInitialized = false;
		defaultConstraint = null;
	}

	public Column(SqlSmoObject parent, string name, DataType dataType)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = parent;
		DataType = dataType;
	}

	public Column(SqlSmoObject parent, string name, DataType dataType, bool isFileStream)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = parent;
		if (IsSupportedProperty("IsFileStream"))
		{
			DataType = dataType;
			IsFileStream = isFileStream;
			if (isFileStream && dataType.SqlDataType != SqlDataType.VarBinaryMax)
			{
				throw new SmoException(ExceptionTemplatesImpl.ColumnNotVarbinaryMax);
			}
			return;
		}
		throw new UnsupportedVersionException(ExceptionTemplatesImpl.SupportedOnlyOn100).SetHelpContext("SupportedOnlyOn100");
	}

	internal override SqlSmoObject GetPermTargetObject()
	{
		return Parent;
	}

	private bool EmbedDefaultConstraints(ScriptingPreferences sp = null)
	{
		if (Parent is UserDefinedTableType || (Parent.State == SqlSmoState.Existing && base.State == SqlSmoState.Creating) || (!VersionUtils.IsSql13Azure12OrLater(DatabaseEngineType, base.ServerVersion, sp) && Parent.IsSupportedProperty("IsMemoryOptimized") && Parent.GetPropValueOptional("IsMemoryOptimized", defaultValue: false)))
		{
			return true;
		}
		return false;
	}

	public void Create()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, null, ExceptionTemplatesImpl.UDTTColumnsCannotBeModified);
		}
		if (base.ParentColl.ParentInstance is View)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, null, ExceptionTemplatesImpl.ViewColumnsCannotBeModified);
		}
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		if (base.ParentColl.ParentInstance is UserDefinedFunction)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfAUDF);
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		TableViewBase tableViewBase = (TableViewBase)base.ParentColl.ParentInstance;
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER TABLE {0} ADD ", new object[1] { tableViewBase.FormatFullNameForScripting(sp) });
		ScriptDdlCreateImpl(stringBuilder, sp);
		queries.Add(stringBuilder.ToString());
		ScriptDefaultAndRuleBinding(queries, sp);
	}

	internal override void ScriptDdl(StringCollection queries, ScriptingPreferences sp)
	{
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptDdlCreateImpl(stringBuilder, sp);
		if (stringBuilder.Length > 0)
		{
			queries.Add(stringBuilder.ToString());
		}
	}

	private void ScriptDdlCreateImpl(StringBuilder sb, ScriptingPreferences sp)
	{
		VersionValidate(sp);
		bool flag = IsGraphInternalColumn();
		bool flag2 = IsGraphComputedColumn();
		string text = FormatFullNameForScripting(sp);
		if (flag || flag2)
		{
			return;
		}
		sb.AppendFormat(SmoApplication.DefaultCulture, "{0} ", new object[1] { text });
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		if (Parent.IsSupportedProperty("IsMemoryOptimized") && Parent.GetPropValueOptional("IsMemoryOptimized", defaultValue: false))
		{
			flag5 = true;
		}
		bool flag6 = CheckIsExternalTableColumn(sp);
		bool flag7 = Parent is Table;
		bool flag8 = false;
		bool flag9 = false;
		GeneratedAlwaysType generatedAlwaysType = GeneratedAlwaysType.None;
		if (IsSupportedProperty("GeneratedAlwaysType", sp))
		{
			object propValueOptional = GetPropValueOptional("GeneratedAlwaysType");
			if (propValueOptional != null)
			{
				generatedAlwaysType = (GeneratedAlwaysType)propValueOptional;
			}
			flag8 = generatedAlwaysType != GeneratedAlwaysType.None;
			if (flag8 && !flag7)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoGeneratedAlwaysColumnsOnNonTables);
			}
			if (flag8 && dataType.SqlDataType != SqlDataType.DateTime2)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.InvalidType(dataType.ToString()));
			}
		}
		if (IsSupportedProperty("IsHidden", sp))
		{
			if (base.Properties.Get("IsHidden").Value != null)
			{
				flag9 = (bool)base.Properties.Get("IsHidden").Value;
			}
			if (flag9 && !flag8)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoHiddenColumnsOnNonGeneratedAlwaysColumns);
			}
		}
		bool flag10 = false;
		string text2 = null;
		if (IsSupportedProperty("IsMasked", sp) && base.Properties.Get("IsMasked").Value != null)
		{
			flag10 = (bool)base.Properties.Get("IsMasked").Value;
			if (flag10)
			{
				text2 = GetAndValidateMaskingFunction(flag10);
				if (!Util.IsNullOrWhiteSpace(text2) && !flag7)
				{
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoDataMaskingOnNonTables);
				}
			}
		}
		if (IsSupportedProperty("IsColumnSet", sp))
		{
			if (base.Properties.Get("IsSparse").Value != null)
			{
				flag3 = (bool)base.Properties["IsSparse"].Value;
			}
			if (base.Properties.Get("IsColumnSet").Value != null)
			{
				flag4 = (bool)base.Properties["IsColumnSet"].Value;
			}
			if (flag3 && flag4)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoSparseOnColumnSet);
			}
		}
		if ((flag3 || flag4) && flag8)
		{
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoSparseOrColumnSetOnTemporalColumns);
		}
		if (flag4 && flag10)
		{
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoDataMaskingOnColumnSet);
		}
		if (flag8 && flag10)
		{
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoDataMaskingOnTemporalColumns);
		}
		if (base.Properties.Get("Computed").Value != null && (bool)base.Properties["Computed"].Value)
		{
			string text3 = (string)GetPropValue("ComputedText");
			if (text3 != null && text3.Length > 0)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, " AS {0}", new object[1] { text3 });
				if (flag3)
				{
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoSparseOnComputed);
				}
				if (flag4)
				{
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoColumnSetOnComputed);
				}
				if (flag8)
				{
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.ComputedTemporalColumns);
				}
				if (flag10)
				{
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoDataMaskingOnComputedColumns);
				}
				if (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90 || base.ServerVersion.Major < 9)
				{
					return;
				}
				if (GetPropValueOptional("IsPersisted", defaultValue: false))
				{
					sb.Append(" PERSISTED");
					if (!GetPropValueOptional("Nullable", defaultValue: false))
					{
						sb.Append(" NOT NULL");
					}
				}
				if (DatabaseEngineType.SqlAzureDatabase != sp.TargetDatabaseEngineType && sp.Data.OptimizerData && RequiresCollate(sp) && base.Properties.Get("Collation").Value is string text4 && 0 < text4.Length)
				{
					CheckCollation(text4, sp);
					sb.AppendFormat(SmoApplication.DefaultCulture, " COLLATE {0}", new object[1] { text4 });
				}
				return;
			}
		}
		UserDefinedDataType.AppendScriptTypeDefinition(sb, sp, this, DataType.SqlDataType);
		if (!flag5 && !flag6 && DatabaseEngineType.SqlAzureDatabase != sp.TargetDatabaseEngineType && IsSupportedProperty("IsFileStream", sp))
		{
			Property property = base.Properties.Get("IsFileStream");
			if (property.Value != null && sp.Storage.FileStreamColumn && (bool)property.Value)
			{
				if (flag10)
				{
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoDataMaskingOnFileStreamColumns);
				}
				if (dataType.SqlDataType != SqlDataType.VarBinaryMax)
				{
					throw new SmoException(ExceptionTemplatesImpl.ColumnNotVarbinaryMax);
				}
				sb.Append(" FILESTREAM ");
			}
		}
		if (RequiresCollate(sp) && base.Properties.Get("Collation").Value is string text5 && 0 < text5.Length)
		{
			CheckCollation(text5, sp);
			sb.AppendFormat(SmoApplication.DefaultCulture, " COLLATE {0}", new object[1] { text5 });
		}
		if (!flag5 && !flag6)
		{
			if (flag3)
			{
				sb.Append(" SPARSE ");
			}
			else if (flag4)
			{
				sb.Append(" COLUMN_SET FOR ALL_SPARSE_COLUMNS ");
			}
		}
		if (flag10)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, " MASKED WITH (FUNCTION = '{0}')", new object[1] { text2 });
		}
		if (sp.Table.Identities && base.Properties.Get("Identity").Value != null && (bool)base.Properties["Identity"].Value)
		{
			if (flag6)
			{
				throw new SmoException(ExceptionTemplatesImpl.IdentityColumnForExternalTable);
			}
			if (flag8)
			{
				throw new SmoException(ExceptionTemplatesImpl.IdentityTemporalColumns);
			}
			sb.Append(" IDENTITY");
			Property property2 = base.Properties.Get("IdentitySeedAsDecimal");
			if (property2.Value == null)
			{
				property2 = base.Properties.Get("IdentitySeed");
			}
			Property property3 = base.Properties.Get("IdentityIncrementAsDecimal");
			if (property3.Value == null)
			{
				property3 = base.Properties.Get("IdentityIncrement");
			}
			if (property2.Value != null && property3.Value != null)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "({0},{1})", new object[2]
				{
					property2.Value.ToString(),
					property3.Value.ToString()
				});
			}
			if (DatabaseEngineType.SqlAzureDatabase != sp.TargetDatabaseEngineType && IsSupportedProperty("NotForReplication", sp))
			{
				Property property4 = base.Properties.Get("NotForReplication");
				if (property4.Value != null && (bool)property4.Value)
				{
					sb.Append(" NOT FOR REPLICATION");
				}
			}
		}
		switch (generatedAlwaysType)
		{
		case GeneratedAlwaysType.AsRowStart:
			sb.Append(" GENERATED ALWAYS AS ROW START");
			if (flag9)
			{
				sb.Append(" HIDDEN");
			}
			break;
		case GeneratedAlwaysType.AsRowEnd:
			sb.Append(" GENERATED ALWAYS AS ROW END");
			if (flag9)
			{
				sb.Append(" HIDDEN");
			}
			break;
		default:
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(condition: false, "Unknown 'GeneratedAlwaysType' property value encountered.");
			break;
		case GeneratedAlwaysType.None:
			break;
		}
		if (!flag5 && !flag6 && DatabaseEngineType.SqlAzureDatabase != sp.TargetDatabaseEngineType && base.Properties.Get("RowGuidCol").Value != null && (bool)base.Properties["RowGuidCol"].Value)
		{
			sb.Append(" ROWGUIDCOL ");
		}
		if (IsSupportedProperty("ColumnEncryptionKeyName") && IsSupportedProperty("EncryptionAlgorithm") && IsSupportedProperty("EncryptionType"))
		{
			object value = base.Properties.Get("EncryptionAlgorithm").Value;
			object value2 = base.Properties.Get("EncryptionType").Value;
			if (value != null || value2 != null)
			{
				object propValueOptional2 = GetPropValueOptional("ColumnEncryptionKeyName");
				if (propValueOptional2 != null)
				{
					ThrowIfPropertyNotSupported("ColumnEncryptionKeyID", sp);
					if (propValueOptional2 == null || value == null || value2 == null)
					{
						throw new WrongPropertyValueException(ExceptionTemplatesImpl.InvalidAlwaysEncryptedPropertyValues);
					}
					if (flag10)
					{
						throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoDataMaskingOnEncryptedColumns);
					}
					sb.AppendFormat(SmoApplication.DefaultCulture, " ENCRYPTED WITH (COLUMN_ENCRYPTION_KEY = {0}, ENCRYPTION_TYPE = {1}, ALGORITHM = '{2}')", new object[3]
					{
						SqlSmoObject.MakeSqlBraket(ColumnEncryptionKeyName),
						EncryptionType.ToString(),
						EncryptionAlgorithm.Replace("'", "''")
					});
				}
			}
		}
		if (base.Properties.Get("Nullable").Value != null)
		{
			if (!(bool)base.Properties["Nullable"].Value)
			{
				if (!(Parent is UserDefinedFunction))
				{
					sb.Append(" NOT NULL");
				}
			}
			else
			{
				if (flag8)
				{
					throw new SmoException(ExceptionTemplatesImpl.NullableTemporalColumns);
				}
				sb.Append(" NULL");
			}
		}
		else if (flag8)
		{
			throw new SmoException(ExceptionTemplatesImpl.NullableTemporalColumns);
		}
		ScriptDefaultConstraint(sb, sp);
	}

	private void ScriptDefaultConstraint(StringBuilder sb, ScriptingPreferences sp)
	{
		InitDefaultConstraint(forScripting: true);
		if (DefaultConstraint != null && (!DefaultConstraint.IgnoreForScripting || sp.ForDirectExecution) && (EmbedDefaultConstraints(sp) || DefaultConstraint.forceEmbedDefaultConstraint) && sb.Length > 0)
		{
			DefaultConstraint.forceEmbedDefaultConstraint = false;
			sb.Append(DefaultConstraint.ScriptDdl(sp));
		}
	}

	internal override string ScriptPermissionInfo(PermissionInfo pi, ScriptingPreferences sp)
	{
		if (Parent is TableViewBase tableViewBase && (pi.PermissionState == PermissionState.Grant || pi.PermissionState == PermissionState.Revoke))
		{
			List<string> keysForPermissionWithGrantOptionFromCache = tableViewBase.GetKeysForPermissionWithGrantOptionFromCache();
			if (pi.PermissionTypeInternal.GetPermissionCount() == 1)
			{
				string keyToMatchColumnPermissions = TableViewBase.GetKeyToMatchColumnPermissions(pi.ObjectClass.ToString(), pi.Grantee, pi.GranteeType.ToString(), pi.Grantor, pi.GrantorType.ToString(), pi.PermissionTypeInternal.ToString());
				if (keysForPermissionWithGrantOptionFromCache.Contains(keyToMatchColumnPermissions))
				{
					if (pi.PermissionState == PermissionState.Grant)
					{
						pi.SetPermissionState(PermissionState.Revoke);
						return PermissionWorker.ScriptPermissionInfo(GetPermTargetObject(), pi, sp, grantGrant: true, cascade: true);
					}
					if (pi.PermissionState == PermissionState.Revoke)
					{
						return PermissionWorker.ScriptPermissionInfo(GetPermTargetObject(), pi, sp, grantGrant: false, cascade: true);
					}
				}
			}
		}
		return base.ScriptPermissionInfo(pi, sp);
	}

	internal void VersionValidate(ScriptingPreferences sp)
	{
		CheckSupportedType(sp);
		if (sp.TargetServerVersion == SqlServerVersion.Version80 && base.Properties.Get("Computed").Value != null && (bool)base.Properties["Computed"].Value && IsForeignKey)
		{
			throw new SmoException(ExceptionTemplatesImpl.ComputedColumnDownlevelContraint(FormatFullNameForScripting(sp, nameIsIndentifier: true), SqlSmoObject.GetSqlServerName(sp)));
		}
	}

	private SqlDataType GetNativeDataType()
	{
		SqlDataType sqlDataType = DataType.SqlDataType;
		if (sqlDataType == SqlDataType.UserDefinedDataType)
		{
			Database database = (Database)Parent.ParentColl.ParentInstance;
			UserDefinedDataType uddt = database.UserDefinedDataTypes[DataType.Name, DataType.Schema];
			sqlDataType = DataType.UserDefinedDataTypeToEnum(uddt);
		}
		return sqlDataType;
	}

	private void CheckSupportedType(ScriptingPreferences options)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(options != null);
		SqlDataType nativeDataType = GetNativeDataType();
		if (!DataType.IsDataTypeSupportedOnTargetVersion(nativeDataType, options.TargetServerVersion))
		{
			throw new SmoException(ExceptionTemplatesImpl.UnsupportedColumnType(((NamedSmoObject)Parent).Name, Name, nativeDataType.ToString(), SqlSmoObject.GetSqlServerName(options)));
		}
		if (DatabaseEngineType.SqlAzureDatabase == options.TargetDatabaseEngineType && !DataType.IsDataTypeSupportedOnCloud(nativeDataType))
		{
			throw new SmoException(ExceptionTemplatesImpl.UnsupportedColumnTypeOnEngineType(((NamedSmoObject)Parent).Name, Name, nativeDataType.ToString(), SqlSmoObject.GetDatabaseEngineName(options)));
		}
	}

	protected override void PostCreate()
	{
		if (!ExecutionManager.Recording && DefaultConstraint != null)
		{
			DefaultConstraint.SetState(SqlSmoState.Existing);
			DefaultConstraint.Properties.SetAllDirty(val: false);
		}
	}

	public void Drop()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Drop, this, null, ExceptionTemplatesImpl.UDTTColumnsCannotBeModified);
		}
		if (base.ParentColl.ParentInstance is View)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Drop, this, null, ExceptionTemplatesImpl.ViewColumnsCannotBeModified);
		}
		DropImpl();
	}

	public void DropIfExists()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Drop, this, null, ExceptionTemplatesImpl.UDTTColumnsCannotBeModified);
		}
		if (base.ParentColl.ParentInstance is View)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Drop, this, null, ExceptionTemplatesImpl.ViewColumnsCannotBeModified);
		}
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		if (base.ParentColl.ParentInstance is UserDefinedFunction)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfAUDF);
		}
		CheckObjectState();
		TableViewBase tableViewBase = (TableViewBase)base.ParentColl.ParentInstance;
		string text = tableViewBase.FormatFullNameForScripting(sp);
		string text2 = FormatFullNameForScripting(sp);
		Property property = base.Properties.Get("Default");
		if (DefaultConstraint != null && DefaultConstraint.Name.Length > 0)
		{
			dropQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} DROP CONSTRAINT [{1}]", new object[2]
			{
				text,
				SqlSmoObject.SqlBraket(DefaultConstraint.Name)
			}));
		}
		else if (DatabaseEngineType.SqlAzureDatabase != sp.TargetDatabaseEngineType && property.Value != null && ((string)property.Value).Length > 0)
		{
			dropQuery.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_unbindefault N'{0}.{1}'", new object[2]
			{
				SqlSmoObject.SqlString(text),
				SqlSmoObject.SqlString(text2)
			}));
		}
		dropQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} DROP COLUMN {1}{2}", new object[3]
		{
			text,
			(sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty,
			text2
		}));
	}

	public void Alter()
	{
		AlterImpl();
	}

	protected override bool IsObjectDirty()
	{
		if (!base.IsObjectDirty())
		{
			return AddedDefCnstr();
		}
		return true;
	}

	private bool AddedDefCnstr()
	{
		if (DefaultConstraint != null)
		{
			return DefaultConstraint.State == SqlSmoState.Creating;
		}
		return false;
	}

	private bool IsParentMemoryOptimized()
	{
		bool result = false;
		if ((Parent is Table || Parent is UserDefinedTableType) && Parent.IsSupportedProperty("IsMemoryOptimized"))
		{
			object propValueOptional = Parent.GetPropValueOptional("IsMemoryOptimized");
			if (propValueOptional != null)
			{
				result = Convert.ToBoolean(propValueOptional);
			}
		}
		return result;
	}

	private bool RequiresCollate(ScriptingPreferences sp)
	{
		bool flag = sp.IncludeScripts.Collation || IsParentMemoryOptimized() || IsEncrypted;
		if (sp.TargetServerVersionInternal != SqlServerVersionInternal.Version70 && base.ServerVersion.Major > 7 && flag && CompatibilityLevel.Version70 < GetCompatibilityLevel() && UserDefinedDataType.IsSystemType(this, sp) && UserDefinedDataType.TypeAllowsCollation((string)GetPropValue("DataType"), base.StringComparer))
		{
			return true;
		}
		return false;
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		if (base.ParentColl.ParentInstance is UserDefinedFunction)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfAUDF);
		}
		if (!IsObjectDirty())
		{
			return;
		}
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Alter, this, null, ExceptionTemplatesImpl.UDTTColumnsCannotBeModified);
		}
		if (base.ParentColl.ParentInstance is View)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Alter, this, null, ExceptionTemplatesImpl.ViewColumnsCannotBeModified);
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		bool flag = false;
		if (base.Properties.Get("Collation").Dirty || base.Properties.Get("Nullable").Dirty || base.Properties.Get("DataType").Dirty || base.Properties.Get("DataTypeSchema").Dirty || base.Properties.Get("Length").Dirty || base.Properties.Get("NumericPrecision").Dirty || base.Properties.Get("NumericScale").Dirty || (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90 && base.Properties.Get("XmlSchemaNamespace").Dirty))
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER TABLE {0} ALTER COLUMN {1} ", new object[2]
			{
				base.ParentColl.ParentInstance.FullQualifiedName,
				FullQualifiedName
			});
			UserDefinedDataType.AppendScriptTypeDefinition(stringBuilder, sp, this, DataType.SqlDataType);
			if (RequiresCollate(sp))
			{
				Property property = base.Properties.Get("Collation");
				string text = property.Value as string;
				if (property.Dirty && text != null && 0 < text.Length)
				{
					CheckCollation(text, sp);
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " COLLATE {0}", new object[1] { text });
				}
			}
			if ((bool)base.Properties["Nullable"].Value)
			{
				stringBuilder.Append(" NULL");
			}
			else
			{
				stringBuilder.Append(" NOT NULL");
			}
			alterQuery.Add(stringBuilder.ToString());
			flag = true;
			stringBuilder.Length = 0;
		}
		Property property2 = base.Properties.Get("RowGuidCol");
		if (property2.Dirty && string.Compare((string)base.Properties["DataType"].Value, "uniqueidentifier", StringComparison.OrdinalIgnoreCase) == 0 && ((bool)property2.Value ^ (bool)GetRealValue(property2, oldRowGuidColValue)))
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER TABLE {0} ALTER COLUMN {1}", new object[2]
			{
				base.ParentColl.ParentInstance.FullQualifiedName,
				FullQualifiedName
			});
			if ((bool)property2.Value)
			{
				stringBuilder.Append(" ADD");
			}
			else
			{
				stringBuilder.Append(" DROP");
			}
			stringBuilder.Append(" ROWGUIDCOL ");
			alterQuery.Add(stringBuilder.ToString());
			stringBuilder.Length = 0;
		}
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			Property property3 = base.Properties.Get("IsPersisted");
			if (property3.Dirty)
			{
				alterQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} ALTER COLUMN {1} {2} PERSISTED", new object[3]
				{
					base.ParentColl.ParentInstance.FullQualifiedName,
					FullQualifiedName,
					((bool)property3.Value) ? "ADD" : "DROP"
				}));
			}
		}
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		if (IsSupportedProperty("IsColumnSet", sp))
		{
			if (base.Properties.Get("IsColumnSet").Value != null)
			{
				flag2 = (bool)base.Properties.Get("IsColumnSet").Value;
			}
			Property property4 = base.Properties.Get("IsSparse");
			if (property4.Dirty)
			{
				alterQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} ALTER COLUMN {1} {2} SPARSE", new object[3]
				{
					base.ParentColl.ParentInstance.FullQualifiedName,
					FullQualifiedName,
					((bool)property4.Value) ? "ADD" : "DROP"
				}));
			}
		}
		if (IsSupportedProperty("GeneratedAlwaysType", sp))
		{
			object propValueOptional = GetPropValueOptional("GeneratedAlwaysType");
			if (propValueOptional != null)
			{
				flag3 = (GeneratedAlwaysType)propValueOptional != GeneratedAlwaysType.None;
			}
		}
		if (base.Properties.Get("Computed").Value != null)
		{
			flag4 = (bool)base.Properties.Get("Computed").Value;
		}
		if (IsSupportedProperty("IsFileStream", sp) && base.Properties.Get("IsFileStream").Value != null)
		{
			flag5 = (bool)base.Properties.Get("IsFileStream").Value;
		}
		if (IsSupportedProperty("ColumnEncryptionKeyName", sp))
		{
			_ = base.Properties.Get("ColumnEncryptionKeyName").Value;
			if (base.Properties.Get("ColumnEncryptionKeyName").Value != null)
			{
				flag6 = true;
			}
		}
		if (IsSupportedProperty("IsMasked", sp))
		{
			bool flag7 = Parent is Table;
			bool flag8 = false;
			bool flag9 = false;
			string text2 = null;
			if (base.Properties.Get("IsMasked").Value != null)
			{
				Property property5 = base.Properties.Get("IsMasked");
				flag8 = (bool)property5.Value;
				flag9 = (property5.Dirty ? (!flag8) : flag8);
				if (flag8)
				{
					text2 = GetAndValidateMaskingFunction(flag8);
					if (!Util.IsNullOrWhiteSpace(text2))
					{
						if (!flag7)
						{
							throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoDataMaskingOnNonTables);
						}
						if (flag2)
						{
							throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoDataMaskingOnColumnSet);
						}
						if (flag3)
						{
							throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoDataMaskingOnTemporalColumns);
						}
						if (flag4)
						{
							throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoDataMaskingOnComputedColumns);
						}
						if (flag5)
						{
							throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoDataMaskingOnFileStreamColumns);
						}
						if (flag6)
						{
							throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoDataMaskingOnEncryptedColumns);
						}
					}
				}
			}
			if (!flag && flag9)
			{
				alterQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} ALTER COLUMN {1} DROP MASKED", new object[2]
				{
					base.ParentColl.ParentInstance.FullQualifiedName,
					FullQualifiedName
				}));
			}
			if (flag8)
			{
				alterQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} ALTER COLUMN {1} ADD MASKED WITH (FUNCTION = '{2}')", new object[3]
				{
					base.ParentColl.ParentInstance.FullQualifiedName,
					FullQualifiedName,
					text2
				}));
			}
		}
		ScriptDefaultAndRuleBinding(alterQuery, sp);
	}

	private string GetAndValidateMaskingFunction(bool isMaskedColumn)
	{
		string text = null;
		object propValueOptional = GetPropValueOptional("MaskingFunction");
		if (isMaskedColumn && propValueOptional != null)
		{
			bool isValidFunction = false;
			bool isValidOnDataType = false;
			text = (string)propValueOptional;
			ValidateMaskingFunctionsFormat(text, DataType, out isValidFunction, out isValidOnDataType);
			if (!isValidFunction)
			{
				string message = string.Format(ExceptionTemplatesImpl.InvalidMaskingFunctionFormat, FullQualifiedName);
				throw new WrongPropertyValueException(message);
			}
			if (!isValidOnDataType)
			{
				string message2 = string.Format(ExceptionTemplatesImpl.MaskingFunctionOnWrongType, FullQualifiedName, text);
				throw new WrongPropertyValueException(message2);
			}
		}
		return text;
	}

	public void Rename(string newname)
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Rename, this, null, ExceptionTemplatesImpl.UDTTColumnsCannotBeModified);
		}
		if (base.ParentColl.ParentInstance is View)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Rename, this, null, ExceptionTemplatesImpl.ViewColumnsCannotBeModified);
		}
		RenameImpl(newname);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		AddDatabaseContext(renameQuery, sp);
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_rename @objname=N'{0}.{1}', @newname=N'{2}', @objtype=N'COLUMN'", new object[3]
		{
			SqlSmoObject.SqlString(base.ParentColl.ParentInstance.FullQualifiedName),
			SqlSmoObject.SqlString(FullQualifiedName),
			SqlSmoObject.SqlString(newName)
		}));
	}

	internal void ScriptDefaultAndRuleBinding(StringCollection queries, ScriptingPreferences sp)
	{
		if (SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType) || ((!sp.OldOptions.Bindings || base.IgnoreForScripting) && !sp.ScriptForCreateDrop))
		{
			return;
		}
		_ = (TableViewBase)base.ParentColl.ParentInstance;
		if (!UserDefinedDefault)
		{
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

	public void BindRule(string ruleSchema, string ruleName)
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Bind, this, null, ExceptionTemplatesImpl.UDTTColumnsCannotBeModified);
		}
		if (base.ParentColl.ParentInstance is View)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Bind, this, null, ExceptionTemplatesImpl.ViewColumnsCannotBeModified);
		}
		BindRuleImpl(ruleSchema, ruleName, bindColumns: false);
	}

	public void UnbindRule()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Unbind, this, null, ExceptionTemplatesImpl.UDTTColumnsCannotBeModified);
		}
		if (base.ParentColl.ParentInstance is View)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Unbind, this, null, ExceptionTemplatesImpl.ViewColumnsCannotBeModified);
		}
		UnbindRuleImpl(bindColumns: false);
	}

	public void BindDefault(string defaultSchema, string defaultName)
	{
		if (base.ParentColl.ParentInstance is View)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Bind, this, null, ExceptionTemplatesImpl.ViewColumnsCannotBeModified);
		}
		BindDefaultImpl(defaultSchema, defaultName, bindColumns: false);
	}

	public void UnbindDefault()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Unbind, this, null, ExceptionTemplatesImpl.UDTTColumnsCannotBeModified);
		}
		if (base.ParentColl.ParentInstance is View)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Unbind, this, null, ExceptionTemplatesImpl.ViewColumnsCannotBeModified);
		}
		UnbindDefaultImpl(bindColumns: false);
	}

	public void MarkForDrop(bool dropOnAlter)
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Drop, this, null, ExceptionTemplatesImpl.UDTTColumnsCannotBeModified);
		}
		MarkForDropImpl(dropOnAlter);
	}

	private void InitDefaultConstraint(bool forScripting = false)
	{
		CheckObjectState();
		if (!m_bDefaultInitialized && base.State != SqlSmoState.Creating && !base.IsDesignMode)
		{
			if (!string.IsNullOrEmpty(DefaultConstraintName))
			{
				InitChildLevel(DefaultConstraint.UrnSuffix, new ScriptingPreferences(), forScripting);
			}
			m_bDefaultInitialized = true;
		}
	}

	internal DefaultConstraint GetDefaultConstraintBaseByName(string name)
	{
		DefaultConstraint defaultConstraint = DefaultConstraint;
		if (defaultConstraint == null)
		{
			AddDefaultConstraint(name);
			this.defaultConstraint.SetState(SqlSmoState.Creating);
			return this.defaultConstraint;
		}
		if (name != null && name.Length != 0 && defaultConstraint.Name != name)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentException(ExceptionTemplatesImpl.ColumnHasNoDefault(Name, name)));
		}
		return defaultConstraint;
	}

	internal void InitializeDefault(IDataReader reader, int colIdx, bool forScripting)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != reader, "reader == null");
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(colIdx < reader.FieldCount, "colIdx >= reader.FieldCount");
		DefaultConstraintName = reader.GetString(colIdx);
		defaultConstraint = new DefaultConstraint(this, new SimpleObjectKey(DefaultConstraintName), SqlSmoState.Existing);
		m_bDefaultInitialized = true;
		Property property = defaultConstraint.Properties.Get("Text");
		Property property2 = defaultConstraint.Properties.Get("IsSystemNamed");
		Property property3 = defaultConstraint.Properties.Get("IsFileTableDefined");
		if (forScripting || GetServerObject().IsInitField(typeof(DefaultConstraint), "Text"))
		{
			int i = -1;
			try
			{
				i = reader.GetOrdinal("Text");
			}
			catch (IndexOutOfRangeException)
			{
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(condition: false, "Text column should be present when initializing for scripting or if it is an init field");
			}
			object value = reader.GetValue(i);
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(property.Type.Equals(typeof(string)), "text for the default should be of type string");
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != value, "enumerator is expected to return DBNull instead of null");
			if (DBNull.Value.Equals(value))
			{
				property.SetValue(string.Empty);
			}
			else
			{
				property.SetValue(value);
			}
			property.SetRetrieved(retrieved: true);
		}
		if (forScripting || GetServerObject().IsInitField(typeof(DefaultConstraint), "IsSystemNamed"))
		{
			int i2 = -1;
			try
			{
				i2 = reader.GetOrdinal("IsSystemNamed");
			}
			catch (IndexOutOfRangeException)
			{
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(condition: false, "IsSystemNamed column should be present when initializing for scripting or if it is an init field");
			}
			object value2 = reader.GetValue(i2);
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(property2.Type.Equals(typeof(bool)), "IsSystemNamed should be of type bool");
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != value2, "enumerator is expected to return DBNull instead of null");
			if (DBNull.Value.Equals(value2))
			{
				property2.SetValue(null);
			}
			else
			{
				property2.SetValue(value2);
			}
			property2.SetRetrieved(retrieved: true);
		}
		if (forScripting || GetServerObject().IsInitField(typeof(DefaultConstraint), "IsFileTableDefined"))
		{
			int i3 = -1;
			try
			{
				i3 = reader.GetOrdinal("IsFileTableDefined");
			}
			catch (IndexOutOfRangeException)
			{
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(condition: false, "IsFileTableDefined column should be present when initializing for scripting or if it is an init field");
			}
			object value3 = reader.GetValue(i3);
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(property3.Type.Equals(typeof(bool)), "IsFileTableDefined should be of type bool");
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != value3, "enumerator is expected to return DBNull instead of null");
			if (DBNull.Value.Equals(value3))
			{
				property3.SetValue(null);
			}
			else
			{
				property3.SetValue(value3);
			}
			property3.SetRetrieved(retrieved: true);
		}
		defaultConstraint.InitializedForScripting = property.Retrieved && property2.Retrieved;
	}

	internal void RemoveDefaultConstraint()
	{
		defaultConstraint = null;
	}

	public DefaultConstraint AddDefaultConstraint()
	{
		if (base.ParentColl.ParentInstance is View)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddDefaultConstraint, this, null, ExceptionTemplatesImpl.ViewColumnsCannotBeModified);
		}
		CheckObjectState();
		return AddDefaultConstraint(null);
	}

	public DefaultConstraint AddDefaultConstraint(string name)
	{
		if (base.ParentColl.ParentInstance is View)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddDefaultConstraint, this, null, ExceptionTemplatesImpl.ViewColumnsCannotBeModified);
		}
		try
		{
			CheckObjectState();
			if (DefaultConstraint != null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentException(ExceptionTemplatesImpl.ColumnAlreadyHasDefault(Name)));
			}
			if (name == null)
			{
				name = string.Format(SmoApplication.DefaultCulture, "DF_{0}_{1}", new object[2]
				{
					base.ParentColl.ParentInstance.InternalName,
					InternalName
				});
			}
			DefaultConstraintName = name;
			defaultConstraint = new DefaultConstraint(this, new SimpleObjectKey(name), SqlSmoState.Creating);
			m_bDefaultInitialized = true;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddDefaultConstraint, this, ex);
		}
		return defaultConstraint;
	}

	public DataTable EnumUserPermissions(string username)
	{
		try
		{
			CheckObjectState();
			if (username == null)
			{
				username = "";
			}
			Request request = new Request(base.Urn.Value + string.Format(SmoApplication.DefaultCulture, "/Permission[@Grantee='{0}']", new object[1] { Urn.EscapeString(username) }));
			request.Fields = new string[8] { "Grantee", "Grantor", "PermissionState", "Code", "ObjectClass", "GranteeType", "GrantorType", "ColumnName" };
			return ExecutionManager.GetEnumeratorData(request);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumPermissions, this, ex);
		}
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		ArrayList arrayList = new ArrayList();
		if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
		{
			arrayList.Add(new PropagateInfo((base.ServerVersion.Major < 8) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix));
		}
		if (Parent is Table)
		{
			bool flag = false;
			InitDefaultConstraint(forScripting: true);
			if (DefaultConstraint != null && DefaultConstraint.IsSupportedProperty("IsFileTableDefined"))
			{
				flag = DefaultConstraint.GetPropValueOptional("IsFileTableDefined", defaultValue: false);
			}
			if (!flag)
			{
				arrayList.Add(new PropagateInfo(DefaultConstraint, !EmbedDefaultConstraints(), "DefaultColumn"));
			}
		}
		PropagateInfo[] array = new PropagateInfo[arrayList.Count];
		arrayList.CopyTo(array, 0);
		return array;
	}

	public DataTable EnumForeignKeys()
	{
		try
		{
			if (base.ParentColl.ParentInstance is UserDefinedTableType || base.ParentColl.ParentInstance is View)
			{
				DataTable dataTable = new DataTable();
				dataTable.Locale = CultureInfo.InvariantCulture;
				return dataTable;
			}
			Urn urn = base.ParentColl.ParentInstance.ParentColl.ParentInstance.Urn;
			Request req = new Request(string.Format(SmoApplication.DefaultCulture, "{0}/Table/ForeignKey[@ReferencedTable='{1}']/Column[@ReferencedColumn='{2}']", new object[3]
			{
				urn,
				Urn.EscapeString(base.ParentColl.ParentInstance.InternalName),
				Urn.EscapeString(Name)
			}), new string[1] { "Urn" });
			DataTable enumeratorData = ExecutionManager.GetEnumeratorData(req);
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			stringBuilder.Append(urn);
			stringBuilder.Append("/Table/ForeignKey[");
			bool flag = false;
			foreach (DataRow row in enumeratorData.Rows)
			{
				stringBuilder.Append(flag ? " or " : "");
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "@Name='{0}'", new object[1] { Urn.EscapeString(((Urn)(string)row[0]).GetNameForType("ForeignKey")) });
				flag = true;
			}
			stringBuilder.Append("]");
			if (flag)
			{
				req = new Request(stringBuilder.ToString(), new string[1] { "Name" }, new OrderBy[1]
				{
					new OrderBy("Name", OrderBy.Direction.Asc)
				});
				req.ParentPropertiesRequests = new PropertiesRequest[1];
				PropertiesRequest propertiesRequest = new PropertiesRequest();
				propertiesRequest.Fields = new string[2] { "Schema", "Name" };
				propertiesRequest.OrderByList = new OrderBy[2]
				{
					new OrderBy("Schema", OrderBy.Direction.Asc),
					new OrderBy("Name", OrderBy.Direction.Asc)
				};
				req.ParentPropertiesRequests[0] = propertiesRequest;
				return ExecutionManager.GetEnumeratorData(req);
			}
			DataTable dataTable2 = new DataTable();
			dataTable2.Locale = CultureInfo.InvariantCulture;
			return dataTable2;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumForeignKeys, this, ex);
		}
	}

	public DataTable EnumIndexes()
	{
		try
		{
			Request request = new Request(string.Concat(base.Urn.Parent, string.Format(SmoApplication.DefaultCulture, "/Index/IndexedColumn[@Name='{0}']", new object[1] { Name })));
			request.Fields = new string[1] { "Urn" };
			request.OrderByList = new OrderBy[1]
			{
				new OrderBy("Urn", OrderBy.Direction.Asc)
			};
			DataTable enumeratorData = ExecutionManager.GetEnumeratorData(request);
			enumeratorData.Columns[0].Caption = "Name";
			foreach (DataRow row in enumeratorData.Rows)
			{
				row[0] = new Urn((string)row[0]).GetAttribute("Name", "Index");
			}
			return enumeratorData;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumIndexes, this, ex);
		}
	}

	public override void Refresh()
	{
		base.Refresh();
		dataType = null;
		m_bDefaultInitialized = false;
		oldRowGuidColValue = null;
	}

	public void UpdateStatistics()
	{
		UpdateStatistics(StatisticsScanType.Default, 0, recompute: true);
	}

	public void UpdateStatistics(StatisticsScanType scanType)
	{
		UpdateStatistics(scanType, 0, recompute: true);
	}

	public void UpdateStatistics(StatisticsScanType scanType, int sampleValue)
	{
		UpdateStatistics(scanType, 0, recompute: true);
	}

	public void UpdateStatistics(StatisticsScanType scanType, int sampleValue, bool recompute)
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.UpdateStatistics, this, null, ExceptionTemplatesImpl.UDTTColumnsCannotBeModified);
		}
		CheckObjectState(throwIfNotCreated: true);
		if (!(base.ParentColl.ParentInstance is TableViewBase tableViewBase))
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.UpdateStatistics, this, null, ExceptionTemplatesImpl.TableOrViewParentForUpdateStatistics);
		}
		tableViewBase.UpdateStatistics(StatisticsTarget.All, scanType, sampleValue, recompute);
	}

	private void ValidatePropertyChangeForText(Property prop, object value)
	{
		if (base.ParentColl is SmoCollectionBase { IsCollectionLocked: not false })
		{
			ScriptNameObjectBase.Validate_set_ChildTextObjectDDLProperty(prop, value);
		}
	}

	internal override void ValidateProperty(Property prop, object value)
	{
		switch (base.ServerVersion.Major)
		{
		case 7:
			switch (prop.Name)
			{
			default:
				return;
			case "Computed":
			case "ComputedText":
			case "Default":
			case "DefaultSchema":
			case "Identity":
			case "IdentityIncrement":
			case "IdentityIncrementAsDecimal":
			case "IdentitySeed":
			case "IdentitySeedAsDecimal":
			case "NotForReplication":
			case "Nullable":
			case "Rule":
			case "RuleSchema":
				break;
			case "RowGuidCol":
				if (!prop.Dirty)
				{
					oldRowGuidColValue = prop.Value;
				}
				break;
			}
			ValidatePropertyChangeForText(prop, value);
			return;
		}
		switch (prop.Name)
		{
		default:
			return;
		case "Collation":
		case "Computed":
		case "ComputedText":
		case "Default":
		case "DefaultSchema":
		case "Identity":
		case "IdentityIncrement":
		case "IdentitySeed":
		case "IdentityIncrementAsDecimal":
		case "IdentitySeedAsDecimal":
		case "NotForReplication":
		case "Nullable":
		case "Rule":
		case "RuleSchema":
			break;
		case "RowGuidCol":
			if (!prop.Dirty)
			{
				oldRowGuidColValue = prop.Value;
			}
			break;
		}
		ValidatePropertyChangeForText(prop, value);
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != parentType, "null == parentType");
		if (!parentType.Equals(typeof(View)) || !defaultTextMode)
		{
			string[] fields = new string[38]
			{
				"AnsiPaddingStatus", "Collation", "ColumnEncryptionKeyID", "ColumnEncryptionKeyName", "Computed", "ComputedText", "DataTypeSchema", "Default", "DefaultConstraintName", "DefaultSchema",
				"DistributionColumnName", "EncryptionAlgorithm", "EncryptionType", "GeneratedAlwaysType", "GraphType", "Identity", "IdentitySeedAsDecimal", "IdentityIncrementAsDecimal", "IsDistributedColumn", "IsFileStream",
				"IsPersisted", "IsForeignKey", "IsMasked", "IsSparse", "IsColumnSet", "Length", "MaskingFunction", "NotForReplication", "Nullable", "NumericScale",
				"NumericPrecision", "RowGuidCol", "Rule", "RuleSchema", "SystemType", "XmlSchemaNamespace", "XmlSchemaNamespaceSchema", "XmlDocumentConstraint"
			};
			List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
			supportedScriptFields.Add("DataType");
			return supportedScriptFields.ToArray();
		}
		return new string[0];
	}

	private bool CheckIsExternalTableColumn(ScriptingPreferences sp)
	{
		bool result = false;
		if (Parent.IsSupportedProperty("IsExternal", sp) && Parent.GetPropValueOptional("IsExternal", defaultValue: false))
		{
			result = true;
		}
		return result;
	}

	private static void ValidateMaskingFunctionsFormat(string maskingFunction, DataType dataType, out bool isValidFunction, out bool isValidOnDataType)
	{
		isValidFunction = false;
		isValidOnDataType = false;
		MaskingFunctionValidation[] maskingFunctionsValidationTable = MaskingFunctionsValidationTable;
		foreach (MaskingFunctionValidation maskingFunctionValidation in maskingFunctionsValidationTable)
		{
			if (Regex.Match(maskingFunction, maskingFunctionValidation.Pattern, RegexOptions.IgnoreCase).Success)
			{
				isValidFunction = true;
				if ((dataType.IsNumericType && maskingFunctionValidation.IsValidForNumeric) || (dataType.IsStringType && maskingFunctionValidation.IsValidForString) || maskingFunctionValidation.IsValidForOther || dataType.SqlDataType == SqlDataType.UserDefinedDataType)
				{
					isValidOnDataType = true;
				}
				break;
			}
		}
	}

	internal bool IsGraphComputedColumn()
	{
		if (IsSupportedProperty("GraphType"))
		{
			Property property = base.Properties.Get("GraphType");
			if (property.IsNull)
			{
				return false;
			}
			if ((GraphType)property.Value != GraphType.GraphFromIdComputed && (GraphType)property.Value != GraphType.GraphToIdComputed)
			{
				return (GraphType)property.Value == GraphType.GraphIdComputed;
			}
			return true;
		}
		return false;
	}

	internal bool IsGraphInternalColumn()
	{
		if (IsSupportedProperty("GraphType"))
		{
			Property property = base.Properties.Get("GraphType");
			if (property.IsNull)
			{
				return false;
			}
			if ((GraphType)property.Value != GraphType.GraphId && (GraphType)property.Value != GraphType.GraphFromId && (GraphType)property.Value != GraphType.GraphFromObjId && (GraphType)property.Value != GraphType.GraphToId)
			{
				return (GraphType)property.Value == GraphType.GraphToObjId;
			}
			return true;
		}
		return false;
	}
}
