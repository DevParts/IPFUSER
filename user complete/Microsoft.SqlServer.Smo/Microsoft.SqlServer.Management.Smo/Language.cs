using System;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class Language : NamedSmoObject
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 10 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[10]
		{
			new StaticMetadata("Alias", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("DateFormat", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Days", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("FirstDayOfWeek", expensive: false, readOnly: true, typeof(byte)),
			new StaticMetadata("LangID", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("LocaleID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Months", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("MsgLangID", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("ShortMonths", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Upgrade", expensive: false, readOnly: true, typeof(int))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[10]
		{
			new StaticMetadata("Alias", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("DateFormat", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Days", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("FirstDayOfWeek", expensive: false, readOnly: true, typeof(byte)),
			new StaticMetadata("LangID", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("LocaleID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Months", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("MsgLangID", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("ShortMonths", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Upgrade", expensive: false, readOnly: true, typeof(int))
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
					"Alias" => 0, 
					"DateFormat" => 1, 
					"Days" => 2, 
					"FirstDayOfWeek" => 3, 
					"LangID" => 4, 
					"LocaleID" => 5, 
					"Months" => 6, 
					"MsgLangID" => 7, 
					"ShortMonths" => 8, 
					"Upgrade" => 9, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"Alias" => 0, 
				"DateFormat" => 1, 
				"Days" => 2, 
				"FirstDayOfWeek" => 3, 
				"LangID" => 4, 
				"LocaleID" => 5, 
				"Months" => 6, 
				"MsgLangID" => 7, 
				"ShortMonths" => 8, 
				"Upgrade" => 9, 
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Alias => (string)base.Properties.GetValueWithNullReplacement("Alias");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string DateFormat => (string)base.Properties.GetValueWithNullReplacement("DateFormat");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Days => (string)base.Properties.GetValueWithNullReplacement("Days");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public byte FirstDayOfWeek => (byte)base.Properties.GetValueWithNullReplacement("FirstDayOfWeek");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public short LangID => (short)base.Properties.GetValueWithNullReplacement("LangID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int LocaleID => (int)base.Properties.GetValueWithNullReplacement("LocaleID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Months => (string)base.Properties.GetValueWithNullReplacement("Months");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public short MsgLangID => (short)base.Properties.GetValueWithNullReplacement("MsgLangID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ShortMonths => (string)base.Properties.GetValueWithNullReplacement("ShortMonths");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int Upgrade => (int)base.Properties.GetValueWithNullReplacement("Upgrade");

	public static string UrnSuffix => "Language";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal Language(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public string Day(DayOfWeek day)
	{
		int num = (int)(day - 1);
		if (-1 == num)
		{
			num = 6;
		}
		return ((string)base.Properties["Days"].Value).Split(',')[num];
	}

	public string ShortMonth(Month month)
	{
		int num = (int)(month - 1);
		return ((string)base.Properties["ShortMonths"].Value).Split(',')[num];
	}

	public string Month(Month month)
	{
		int num = (int)(month - 1);
		return ((string)base.Properties["Months"].Value).Split(',')[num];
	}
}
