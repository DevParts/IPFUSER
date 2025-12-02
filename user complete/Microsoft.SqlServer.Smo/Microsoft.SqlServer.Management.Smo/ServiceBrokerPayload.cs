using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone)]
public sealed class ServiceBrokerPayload : EndpointPayload
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 7, 7, 7, 7, 7, 7, 7, 7 };

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
				"Certificate" => 0, 
				"EndpointAuthenticationOrder" => 1, 
				"EndpointEncryption" => 2, 
				"EndpointEncryptionAlgorithm" => 3, 
				"IsMessageForwardingEnabled" => 4, 
				"IsSystemObject" => 5, 
				"MessageForwardingSize" => 6, 
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
			staticMetadata = new StaticMetadata[7]
			{
				new StaticMetadata("Certificate", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("EndpointAuthenticationOrder", expensive: false, readOnly: false, typeof(EndpointAuthenticationOrder)),
				new StaticMetadata("EndpointEncryption", expensive: false, readOnly: false, typeof(EndpointEncryption)),
				new StaticMetadata("EndpointEncryptionAlgorithm", expensive: false, readOnly: false, typeof(EndpointEncryptionAlgorithm)),
				new StaticMetadata("IsMessageForwardingEnabled", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("MessageForwardingSize", expensive: false, readOnly: false, typeof(int))
			};
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string Certificate
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Certificate");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Certificate", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public EndpointAuthenticationOrder EndpointAuthenticationOrder
	{
		get
		{
			return (EndpointAuthenticationOrder)base.Properties.GetValueWithNullReplacement("EndpointAuthenticationOrder");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EndpointAuthenticationOrder", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public EndpointEncryption EndpointEncryption
	{
		get
		{
			return (EndpointEncryption)base.Properties.GetValueWithNullReplacement("EndpointEncryption");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EndpointEncryption", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public EndpointEncryptionAlgorithm EndpointEncryptionAlgorithm
	{
		get
		{
			return (EndpointEncryptionAlgorithm)base.Properties.GetValueWithNullReplacement("EndpointEncryptionAlgorithm");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EndpointEncryptionAlgorithm", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsMessageForwardingEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsMessageForwardingEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsMessageForwardingEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int MessageForwardingSize
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("MessageForwardingSize");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MessageForwardingSize", value);
		}
	}

	public static string UrnSuffix => "ServiceBroker";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal ServiceBrokerPayload(Endpoint parentEndpoint, ObjectKeyBase key, SqlSmoState state)
		: base(parentEndpoint, key, state)
	{
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		base.Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}

	internal override void Script(StringBuilder sb, ScriptingPreferences sp)
	{
		bool flag = false;
		object obj = null;
		obj = GetPropValueOptional("IsMessageForwardingEnabled");
		if (obj != null)
		{
			if (flag)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
			}
			flag = true;
			sb.Append("MESSAGE_FORWARDING = ");
			if ((bool)obj)
			{
				sb.Append("ENABLED");
			}
			else
			{
				sb.Append("DISABLED");
			}
			sb.Append(Globals.newline);
		}
		obj = GetPropValueOptional("MessageForwardingSize");
		if (obj != null)
		{
			if (flag)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
			}
			flag = true;
			sb.AppendFormat(SmoApplication.DefaultCulture, "MESSAGE_FORWARD_SIZE = {0}", new object[1] { obj });
			sb.Append(Globals.newline);
		}
		ScriptAuthenticationAndEncryption(sb, sp, flag);
	}
}
