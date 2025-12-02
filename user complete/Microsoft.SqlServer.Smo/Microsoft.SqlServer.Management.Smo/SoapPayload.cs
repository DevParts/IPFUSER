using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone)]
[SfcElementType("Soap")]
public sealed class SoapPayload : EndpointPayload
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 11, 11, 11, 11, 11, 11, 11, 11 };

		private static int[] cloudVersionCount;

		private static int sqlDwPropertyCount;

		internal static StaticMetadata[] sqlDwStaticMetadata;

		internal static StaticMetadata[] cloudStaticMetadata;

		internal static StaticMetadata[] staticMetadata;

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
				return -1;
			}
			return propertyName switch
			{
				"DefaultDatabase" => 0, 
				"DefaultNamespace" => 1, 
				"IsSessionEnabled" => 2, 
				"IsSqlBatchesEnabled" => 3, 
				"IsSystemObject" => 4, 
				"SessionNeverTimesOut" => 5, 
				"SessionTimeout" => 6, 
				"WsdlGeneratorOption" => 7, 
				"WsdlGeneratorProcedure" => 8, 
				"XmlFormatOption" => 9, 
				"XsdSchemaOption" => 10, 
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

		static PropertyMetadataProvider()
		{
			int[] array = new int[3];
			cloudVersionCount = array;
			sqlDwPropertyCount = 0;
			sqlDwStaticMetadata = new StaticMetadata[0];
			cloudStaticMetadata = new StaticMetadata[0];
			staticMetadata = new StaticMetadata[11]
			{
				new StaticMetadata("DefaultDatabase", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("DefaultNamespace", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("IsSessionEnabled", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("IsSqlBatchesEnabled", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("SessionNeverTimesOut", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("SessionTimeout", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("WsdlGeneratorOption", expensive: false, readOnly: false, typeof(WsdlGeneratorOption)),
				new StaticMetadata("WsdlGeneratorProcedure", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("XmlFormatOption", expensive: false, readOnly: false, typeof(XmlFormatOption)),
				new StaticMetadata("XsdSchemaOption", expensive: false, readOnly: false, typeof(XsdSchemaOption))
			};
		}
	}

	private SoapPayloadMethodCollection m_soapPayloadMethodCollection;

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string DefaultDatabase
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DefaultDatabase");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DefaultDatabase", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string DefaultNamespace
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DefaultNamespace");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DefaultNamespace", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsSessionEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsSessionEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsSessionEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsSqlBatchesEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsSqlBatchesEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsSqlBatchesEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool SessionNeverTimesOut
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("SessionNeverTimesOut");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SessionNeverTimesOut", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int SessionTimeout
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("SessionTimeout");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SessionTimeout", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public WsdlGeneratorOption WsdlGeneratorOption
	{
		get
		{
			return (WsdlGeneratorOption)base.Properties.GetValueWithNullReplacement("WsdlGeneratorOption");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("WsdlGeneratorOption", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string WsdlGeneratorProcedure
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("WsdlGeneratorProcedure");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("WsdlGeneratorProcedure", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public XmlFormatOption XmlFormatOption
	{
		get
		{
			return (XmlFormatOption)base.Properties.GetValueWithNullReplacement("XmlFormatOption");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("XmlFormatOption", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public XsdSchemaOption XsdSchemaOption
	{
		get
		{
			return (XsdSchemaOption)base.Properties.GetValueWithNullReplacement("XsdSchemaOption");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("XsdSchemaOption", value);
		}
	}

	public static string UrnSuffix => "Soap";

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(SoapPayloadMethod))]
	public SoapPayloadMethodCollection SoapPayloadMethods
	{
		get
		{
			if (m_soapPayloadMethodCollection == null)
			{
				m_soapPayloadMethodCollection = new SoapPayloadMethodCollection(this);
			}
			return m_soapPayloadMethodCollection;
		}
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal SoapPayload(Endpoint parentEndpoint, ObjectKeyBase key, SqlSmoState state)
		: base(parentEndpoint, key, state)
	{
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		base.Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (m_soapPayloadMethodCollection != null)
		{
			m_soapPayloadMethodCollection.MarkAllDropped();
		}
	}

	internal override void Script(StringBuilder sb, ScriptingPreferences sp)
	{
		bool flag = false;
		if (SoapPayloadMethods.Count > 0)
		{
			foreach (SoapPayloadMethod soapPayloadMethod in SoapPayloadMethods)
			{
				if (!soapPayloadMethod.IgnoreForScripting)
				{
					if (flag)
					{
						sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
					}
					flag = true;
					sb.Append(Globals.newline);
					soapPayloadMethod.Script(sb, sp);
				}
			}
		}
		object propValueOptional = GetPropValueOptional("IsSqlBatchesEnabled");
		if (propValueOptional != null)
		{
			if (flag)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
			}
			flag = true;
			sb.AppendFormat(SmoApplication.DefaultCulture, "BATCHES={0}", new object[1] { ((bool)propValueOptional) ? "ENABLED" : "DISABLED" });
		}
		object propValueOptional2 = GetPropValueOptional("WsdlGeneratorOption");
		if (propValueOptional2 != null)
		{
			string empty = string.Empty;
			empty = (WsdlGeneratorOption)propValueOptional2 switch
			{
				WsdlGeneratorOption.None => "NONE", 
				WsdlGeneratorOption.DefaultProcedure => "DEFAULT", 
				WsdlGeneratorOption.Procedure => SqlSmoObject.MakeSqlString(GetPropValue("WsdlGeneratorProcedure") as string), 
				_ => throw new WrongPropertyValueException(base.Properties.Get("WsdlGeneratorOption")), 
			};
			if (empty.Length > 0)
			{
				if (flag)
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
				}
				flag = true;
				sb.AppendFormat(SmoApplication.DefaultCulture, "WSDL={0}", new object[1] { empty });
			}
		}
		object propValueOptional3 = GetPropValueOptional("IsSessionEnabled");
		if (propValueOptional3 != null)
		{
			if (flag)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
			}
			flag = true;
			sb.AppendFormat(SmoApplication.DefaultCulture, "SESSIONS={0}", new object[1] { ((bool)propValueOptional3) ? "ENABLED" : "DISABLED" });
		}
		if (GetPropValueOptional("SessionNeverTimesOut", defaultValue: false))
		{
			if (flag)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
			}
			sb.Append("SESSION_TIMEOUT=NEVER");
		}
		else
		{
			object propValueOptional4 = GetPropValueOptional("SessionTimeout");
			if (propValueOptional4 != null)
			{
				if (flag)
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
				}
				flag = true;
				if ((int)propValueOptional4 < 0)
				{
					sb.Append("SESSION_TIMEOUT=NEVER");
				}
				else
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, "SESSION_TIMEOUT={0}", new object[1] { (int)propValueOptional4 });
				}
			}
		}
		object propValueOptional5 = GetPropValueOptional("DefaultDatabase");
		if (propValueOptional5 != null)
		{
			if (flag)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
			}
			flag = true;
			sb.AppendFormat(SmoApplication.DefaultCulture, "DATABASE={0}", new object[1] { (((string)propValueOptional5).Length > 0) ? SqlSmoObject.MakeSqlString((string)propValueOptional5) : "DEFAULT" });
		}
		object propValueOptional6 = GetPropValueOptional("DefaultNamespace");
		if (propValueOptional6 != null)
		{
			if (flag)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
			}
			flag = true;
			sb.AppendFormat(SmoApplication.DefaultCulture, "NAMESPACE={0}", new object[1] { (((string)propValueOptional6).Length > 0) ? SqlSmoObject.MakeSqlString((string)propValueOptional6) : "DEFAULT" });
		}
		object propValueOptional7 = GetPropValueOptional("XsdSchemaOption");
		if (propValueOptional7 != null)
		{
			string text = string.Empty;
			switch ((XsdSchemaOption)propValueOptional7)
			{
			case XsdSchemaOption.None:
				text = "NONE";
				break;
			case XsdSchemaOption.Standard:
				text = "STANDARD";
				break;
			}
			if (text.Length > 0)
			{
				if (flag)
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
				}
				flag = true;
				sb.AppendFormat(SmoApplication.DefaultCulture, "SCHEMA={0}", new object[1] { text });
			}
		}
		object propValueOptional8 = GetPropValueOptional("XmlFormatOption");
		if (propValueOptional8 == null)
		{
			return;
		}
		string text2 = string.Empty;
		switch ((XmlFormatOption)propValueOptional8)
		{
		case XmlFormatOption.XmlFormat:
			text2 = "XML";
			break;
		case XmlFormatOption.SqlFormat:
			text2 = "SQL";
			break;
		}
		if (text2.Length > 0)
		{
			if (flag)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
			}
			flag = true;
			sb.AppendFormat(SmoApplication.DefaultCulture, "CHARACTER_SET={0}", new object[1] { text2 });
		}
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		return new PropagateInfo[1]
		{
			new PropagateInfo(SoapPayloadMethods, bWithScript: false)
		};
	}
}
