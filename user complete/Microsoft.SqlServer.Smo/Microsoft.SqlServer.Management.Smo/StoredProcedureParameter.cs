using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[SfcElementType("Param")]
public sealed class StoredProcedureParameter : Parameter, ISfcSupportsDesignMode
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 9, 9, 14, 16, 16, 16, 16, 16, 16, 16 };

		private static int[] cloudVersionCount = new int[3] { 16, 16, 16 };

		private static int sqlDwPropertyCount = 13;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[13]
		{
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DefaultValue", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("HasDefaultValue", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsCursorParameter", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsOutputParameter", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsReadOnly", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("UserType", expensive: false, readOnly: true, typeof(string))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[16]
		{
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DefaultValue", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("HasDefaultValue", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsCursorParameter", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsOutputParameter", expensive: false, readOnly: false, typeof(bool)),
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

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[16]
		{
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DefaultValue", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsOutputParameter", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("HasDefaultValue", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsCursorParameter", expensive: false, readOnly: false, typeof(bool)),
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
						"IsCursorParameter" => 5, 
						"IsOutputParameter" => 6, 
						"IsReadOnly" => 7, 
						"Length" => 8, 
						"NumericPrecision" => 9, 
						"NumericScale" => 10, 
						"SystemType" => 11, 
						"UserType" => 12, 
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
					"IsCursorParameter" => 5, 
					"IsOutputParameter" => 6, 
					"IsReadOnly" => 7, 
					"Length" => 8, 
					"NumericPrecision" => 9, 
					"NumericScale" => 10, 
					"SystemType" => 11, 
					"UserType" => 12, 
					"XmlDocumentConstraint" => 13, 
					"XmlSchemaNamespace" => 14, 
					"XmlSchemaNamespaceSchema" => 15, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"DataType" => 0, 
				"DataTypeSchema" => 1, 
				"DefaultValue" => 2, 
				"ID" => 3, 
				"IsOutputParameter" => 4, 
				"Length" => 5, 
				"NumericPrecision" => 6, 
				"NumericScale" => 7, 
				"SystemType" => 8, 
				"HasDefaultValue" => 9, 
				"IsCursorParameter" => 10, 
				"XmlDocumentConstraint" => 11, 
				"XmlSchemaNamespace" => 12, 
				"XmlSchemaNamespaceSchema" => 13, 
				"IsReadOnly" => 14, 
				"UserType" => 15, 
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
	public StoredProcedure Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as StoredProcedure;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsCursorParameter
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsCursorParameter");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsCursorParameter", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsOutputParameter
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsOutputParameter");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsOutputParameter", value);
		}
	}

	public StoredProcedureParameter()
	{
	}

	public StoredProcedureParameter(StoredProcedure storedProcedure, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = storedProcedure;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	public StoredProcedureParameter(StoredProcedure storedProcedure, string name, DataType dataType)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = storedProcedure;
		DataType = dataType;
	}

	internal StoredProcedureParameter(AbstractCollectionBase parent, ObjectKeyBase key, SqlSmoState state)
		: base(parent, key, state)
	{
	}

	internal override void ValidateProperty(Property prop, object value)
	{
		if (prop.Name == "DefaultValue" || prop.Name == "IsOutputParameter")
		{
			ScriptNameObjectBase.Validate_set_ChildTextObjectDDLProperty(prop, value);
		}
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		if (!defaultTextMode)
		{
			string[] fields = new string[13]
			{
				"NumericPrecision", "NumericScale", "Length", "DataType", "DataTypeSchema", "SystemType", "IsOutputParameter", "DefaultValue", "HasDefaultValue", "XmlSchemaNamespace",
				"XmlSchemaNamespaceSchema", "XmlDocumentConstraint", "IsReadOnly"
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
