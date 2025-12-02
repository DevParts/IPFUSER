using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class UserDefinedAggregateParameter : ParameterBase, ISfcSupportsDesignMode
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 10, 12, 12, 12, 12, 12, 12, 12 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 12 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[12]
		{
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
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

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[12]
		{
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string)),
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
					return -1;
				}
				return propertyName switch
				{
					"DataType" => 0, 
					"DataTypeSchema" => 1, 
					"ID" => 2, 
					"IsReadOnly" => 3, 
					"Length" => 4, 
					"NumericPrecision" => 5, 
					"NumericScale" => 6, 
					"SystemType" => 7, 
					"UserType" => 8, 
					"XmlDocumentConstraint" => 9, 
					"XmlSchemaNamespace" => 10, 
					"XmlSchemaNamespaceSchema" => 11, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"DataType" => 0, 
				"DataTypeSchema" => 1, 
				"ID" => 2, 
				"Length" => 3, 
				"NumericPrecision" => 4, 
				"NumericScale" => 5, 
				"SystemType" => 6, 
				"XmlDocumentConstraint" => 7, 
				"XmlSchemaNamespace" => 8, 
				"XmlSchemaNamespaceSchema" => 9, 
				"IsReadOnly" => 10, 
				"UserType" => 11, 
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

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public UserDefinedAggregate Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as UserDefinedAggregate;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	internal new static string UrnSuffix => "Param";

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[SfcReference(typeof(UserDefinedType), typeof(UserDefinedTypeResolver), "Resolve", new string[] { })]
	[SfcReference(typeof(UserDefinedDataType), typeof(UserDefinedDataTypeResolver), "Resolve", new string[] { })]
	[SfcReference(typeof(UserDefinedTableType), typeof(UserDefinedTableTypeResolver), "Resolve", new string[] { })]
	public override DataType DataType
	{
		get
		{
			return base.DataType;
		}
		set
		{
			base.DataType = value;
		}
	}

	public UserDefinedAggregateParameter()
	{
	}

	public UserDefinedAggregateParameter(UserDefinedAggregate userDefinedAggregate, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = userDefinedAggregate;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal UserDefinedAggregateParameter(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public UserDefinedAggregateParameter(UserDefinedAggregate userDefinedAggregate, string name, DataType dataType)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = userDefinedAggregate;
		DataType = dataType;
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		if (!defaultTextMode)
		{
			string[] fields = new string[7] { "NumericPrecision", "NumericScale", "Length", "DataType", "DataTypeSchema", "SystemType", "IsReadOnly" };
			List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
			return supportedScriptFields.ToArray();
		}
		return new string[0];
	}

	protected override bool isParentClrImplemented()
	{
		return true;
	}
}
