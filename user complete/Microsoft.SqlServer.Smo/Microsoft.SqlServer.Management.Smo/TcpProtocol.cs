using System.Net;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElementType("Tcp")]
[SfcElement(SfcElementFlags.Standalone)]
public sealed class TcpProtocol : EndpointProtocol
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 4, 4, 4, 4, 4, 4, 4, 4 };

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
				"IsDynamicPort" => 0, 
				"IsSystemObject" => 1, 
				"ListenerIPAddress" => 2, 
				"ListenerPort" => 3, 
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
			staticMetadata = new StaticMetadata[4]
			{
				new StaticMetadata("IsDynamicPort", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("ListenerIPAddress", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ListenerPort", expensive: false, readOnly: false, typeof(int))
			};
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsDynamicPort => (bool)base.Properties.GetValueWithNullReplacement("IsDynamicPort");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ListenerPort
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("ListenerPort");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ListenerPort", value);
		}
	}

	public static string UrnSuffix => "Tcp";

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public IPAddress ListenerIPAddress
	{
		get
		{
			IPAddress iPAddress = null;
			object valueWithNullReplacement = base.Properties.GetValueWithNullReplacement("ListenerIPAddress");
			if (valueWithNullReplacement != null)
			{
				iPAddress = valueWithNullReplacement as IPAddress;
				if (iPAddress != null)
				{
					return iPAddress;
				}
				if (valueWithNullReplacement is string { Length: >0 } text)
				{
					iPAddress = IPAddress.Parse(text);
				}
			}
			if (iPAddress == null)
			{
				iPAddress = IPAddress.Any;
			}
			ListenerIPAddress = iPAddress;
			return iPAddress;
		}
		set
		{
			base.Properties.SetValue(base.Properties.LookupID("ListenerIPAddress", PropertyAccessPurpose.Read), value);
			base.Properties.SetDirty(base.Properties.LookupID("ListenerIPAddress", PropertyAccessPurpose.Read), val: true);
		}
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal TcpProtocol(Endpoint parentEndpoint, ObjectKeyBase key, SqlSmoState state)
		: base(parentEndpoint, key, state)
	{
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		base.Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void Script(StringBuilder sb, ScriptingPreferences sp)
	{
		object propValue = GetPropValue("ListenerPort");
		sb.AppendFormat(SmoApplication.DefaultCulture, "LISTENER_PORT = {0}", new object[1] { propValue });
		propValue = GetPropValueOptional("ListenerIPAddress");
		if (propValue != null)
		{
			sb.Append(Globals.commaspace);
			IPAddress listenerIPAddress = ListenerIPAddress;
			if (IPAddress.Any == listenerIPAddress)
			{
				sb.Append("LISTENER_IP = ALL");
				return;
			}
			sb.AppendFormat(SmoApplication.DefaultCulture, "LISTENER_IP = ({0})", new object[1] { listenerIPAddress.ToString() });
		}
	}
}
