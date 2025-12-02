using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class SystemDataType : NamedSmoObject, ISfcSupportsDesignMode
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 9, 10, 11, 11, 11, 11, 11, 11, 11, 11 };

		private static int[] cloudVersionCount = new int[3] { 11, 11, 11 };

		private static int sqlDwPropertyCount = 11;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[11]
		{
			new StaticMetadata("AllowIdentity", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("AllowLength", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("AllowNulls", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Collation", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("MaximumLength", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Numeric", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("VariableLength", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("VariableMaxLength", expensive: false, readOnly: true, typeof(int))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[11]
		{
			new StaticMetadata("AllowIdentity", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("AllowLength", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("AllowNulls", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Collation", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("MaximumLength", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Numeric", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("VariableLength", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("VariableMaxLength", expensive: false, readOnly: true, typeof(int))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[11]
		{
			new StaticMetadata("AllowIdentity", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("AllowLength", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("AllowNulls", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("MaximumLength", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Numeric", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("VariableLength", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("VariableMaxLength", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Collation", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int))
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
						"AllowIdentity" => 0, 
						"AllowLength" => 1, 
						"AllowNulls" => 2, 
						"Collation" => 3, 
						"ID" => 4, 
						"MaximumLength" => 5, 
						"Numeric" => 6, 
						"NumericPrecision" => 7, 
						"NumericScale" => 8, 
						"VariableLength" => 9, 
						"VariableMaxLength" => 10, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"AllowIdentity" => 0, 
					"AllowLength" => 1, 
					"AllowNulls" => 2, 
					"Collation" => 3, 
					"ID" => 4, 
					"MaximumLength" => 5, 
					"Numeric" => 6, 
					"NumericPrecision" => 7, 
					"NumericScale" => 8, 
					"VariableLength" => 9, 
					"VariableMaxLength" => 10, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"AllowIdentity" => 0, 
				"AllowLength" => 1, 
				"AllowNulls" => 2, 
				"MaximumLength" => 3, 
				"Numeric" => 4, 
				"NumericPrecision" => 5, 
				"NumericScale" => 6, 
				"VariableLength" => 7, 
				"VariableMaxLength" => 8, 
				"Collation" => 9, 
				"ID" => 10, 
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
	public Server Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as Server;
		}
	}

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool AllowIdentity => (bool)base.Properties.GetValueWithNullReplacement("AllowIdentity");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool AllowLength => (bool)base.Properties.GetValueWithNullReplacement("AllowLength");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool AllowNulls => (bool)base.Properties.GetValueWithNullReplacement("AllowNulls");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Collation => (string)base.Properties.GetValueWithNullReplacement("Collation");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int MaximumLength => (int)base.Properties.GetValueWithNullReplacement("MaximumLength");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int NumericPrecision => (int)base.Properties.GetValueWithNullReplacement("NumericPrecision");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int NumericScale => (int)base.Properties.GetValueWithNullReplacement("NumericScale");

	public static string UrnSuffix => "SystemDataType";

	[SfcKey(0)]
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

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal SystemDataType(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}
}
