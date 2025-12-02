using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[SfcElementType("Param")]
public sealed class UserDefinedFunctionParameter : Parameter, ISfcSupportsDesignMode
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 8, 8, 12, 14, 14, 14, 14, 14, 14, 14 };

		private static int[] cloudVersionCount = new int[3] { 14, 14, 14 };

		private static int sqlDwPropertyCount = 11;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[11]
		{
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DefaultValue", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("HasDefaultValue", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsReadOnly", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("UserType", expensive: false, readOnly: true, typeof(string))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[14]
		{
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DefaultValue", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("HasDefaultValue", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsReadOnly", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("UserType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("XmlDocumentConstraint", expensive: false, readOnly: false, typeof(XmlDocumentConstraint)),
			new StaticMetadata("XmlSchemaNamespace", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("XmlSchemaNamespaceSchema", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[14]
		{
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DefaultValue", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("HasDefaultValue", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("XmlDocumentConstraint", expensive: false, readOnly: false, typeof(XmlDocumentConstraint)),
			new StaticMetadata("XmlSchemaNamespace", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("XmlSchemaNamespaceSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("IsReadOnly", expensive: false, readOnly: false, typeof(bool)),
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
					return propertyName switch
					{
						"DataType" => 0, 
						"DataTypeSchema" => 1, 
						"DefaultValue" => 2, 
						"HasDefaultValue" => 3, 
						"ID" => 4, 
						"IsReadOnly" => 5, 
						"Length" => 6, 
						"NumericPrecision" => 7, 
						"NumericScale" => 8, 
						"SystemType" => 9, 
						"UserType" => 10, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"DataType" => 0, 
					"DataTypeSchema" => 1, 
					"DefaultValue" => 2, 
					"HasDefaultValue" => 3, 
					"ID" => 4, 
					"IsReadOnly" => 5, 
					"Length" => 6, 
					"NumericPrecision" => 7, 
					"NumericScale" => 8, 
					"SystemType" => 9, 
					"UserType" => 10, 
					"XmlDocumentConstraint" => 11, 
					"XmlSchemaNamespace" => 12, 
					"XmlSchemaNamespaceSchema" => 13, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"DataType" => 0, 
				"DataTypeSchema" => 1, 
				"DefaultValue" => 2, 
				"ID" => 3, 
				"Length" => 4, 
				"NumericPrecision" => 5, 
				"NumericScale" => 6, 
				"SystemType" => 7, 
				"HasDefaultValue" => 8, 
				"XmlDocumentConstraint" => 9, 
				"XmlSchemaNamespace" => 10, 
				"XmlSchemaNamespaceSchema" => 11, 
				"IsReadOnly" => 12, 
				"UserType" => 13, 
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

	[SfcObject(SfcObjectRelationship.ParentObject, SfcObjectFlags.Design)]
	public UserDefinedFunction Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as UserDefinedFunction;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	public UserDefinedFunctionParameter()
	{
	}

	public UserDefinedFunctionParameter(UserDefinedFunction userDefinedFunction, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = userDefinedFunction;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	public UserDefinedFunctionParameter(UserDefinedFunction userDefinedFunction, string name, DataType dataType)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = userDefinedFunction;
		DataType = dataType;
	}

	internal UserDefinedFunctionParameter(AbstractCollectionBase parent, ObjectKeyBase key, SqlSmoState state)
		: base(parent, key, state)
	{
	}

	internal override void ValidateProperty(Property prop, object value)
	{
		if (prop.Name == "DefaultValue")
		{
			ScriptNameObjectBase.Validate_set_ChildTextObjectDDLProperty(prop, value);
		}
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		if (!defaultTextMode)
		{
			string[] fields = new string[12]
			{
				"NumericPrecision", "NumericScale", "Length", "DataType", "DataTypeSchema", "SystemType", "DefaultValue", "HasDefaultValue", "XmlSchemaNamespace", "XmlSchemaNamespaceSchema",
				"XmlDocumentConstraint", "IsReadOnly"
			};
			List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
			return supportedScriptFields.ToArray();
		}
		return new string[0];
	}

	protected override bool isParentClrImplemented()
	{
		if (base.ServerVersion.Major > 8)
		{
			return Parent.GetPropValueOptional<ImplementationType>("ImplementationType") == ImplementationType.SqlClr;
		}
		return false;
	}
}
