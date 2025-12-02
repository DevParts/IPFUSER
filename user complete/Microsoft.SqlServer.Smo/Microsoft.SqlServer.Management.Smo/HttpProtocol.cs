using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElementType("Http")]
[SfcElement(SfcElementFlags.Standalone)]
public sealed class HttpProtocol : EndpointProtocol
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 10, 10, 10, 10, 10, 10, 10, 10 };

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
				"AuthenticationRealm" => 0, 
				"ClearPort" => 1, 
				"DefaultLogonDomain" => 2, 
				"HttpAuthenticationModes" => 3, 
				"HttpPortTypes" => 4, 
				"IsCompressionEnabled" => 5, 
				"IsSystemObject" => 6, 
				"SslPort" => 7, 
				"WebSite" => 8, 
				"WebSiteUrlPath" => 9, 
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
			staticMetadata = new StaticMetadata[10]
			{
				new StaticMetadata("AuthenticationRealm", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ClearPort", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("DefaultLogonDomain", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("HttpAuthenticationModes", expensive: false, readOnly: false, typeof(HttpAuthenticationModes)),
				new StaticMetadata("HttpPortTypes", expensive: false, readOnly: false, typeof(HttpPortTypes)),
				new StaticMetadata("IsCompressionEnabled", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("SslPort", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("WebSite", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("WebSiteUrlPath", expensive: false, readOnly: false, typeof(string))
			};
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string AuthenticationRealm
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("AuthenticationRealm");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AuthenticationRealm", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ClearPort
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("ClearPort");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ClearPort", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string DefaultLogonDomain
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DefaultLogonDomain");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DefaultLogonDomain", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public HttpAuthenticationModes HttpAuthenticationModes
	{
		get
		{
			return (HttpAuthenticationModes)base.Properties.GetValueWithNullReplacement("HttpAuthenticationModes");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("HttpAuthenticationModes", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public HttpPortTypes HttpPortTypes
	{
		get
		{
			return (HttpPortTypes)base.Properties.GetValueWithNullReplacement("HttpPortTypes");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("HttpPortTypes", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsCompressionEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsCompressionEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsCompressionEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int SslPort
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("SslPort");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SslPort", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string WebSite
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("WebSite");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("WebSite", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string WebSiteUrlPath
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("WebSiteUrlPath");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("WebSiteUrlPath", value);
		}
	}

	public static string UrnSuffix => "Http";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal HttpProtocol(Endpoint parentEndpoint, ObjectKeyBase key, SqlSmoState state)
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
		sb.AppendFormat(SmoApplication.DefaultCulture, "PATH=N'{0}'", new object[1] { SqlSmoObject.SqlString((string)GetPropValue("WebSiteUrlPath")) });
		bool flag = false;
		HttpPortTypes httpPortTypes = (HttpPortTypes)GetPropValue("HttpPortTypes");
		sb.Append(", PORTS = (");
		bool flag2 = false;
		if ((HttpPortTypes.Ssl & httpPortTypes) != HttpPortTypes.None)
		{
			flag2 = true;
			flag = true;
			sb.Append("SSL");
		}
		if ((HttpPortTypes.Clear & httpPortTypes) != HttpPortTypes.None)
		{
			flag2 = true;
			if (flag)
			{
				sb.Append(Globals.commaspace);
			}
			sb.Append("CLEAR");
		}
		if (!flag2)
		{
			throw new WrongPropertyValueException(base.Properties.Get("HttpPortTypes"));
		}
		sb.Append(Globals.RParen);
		HttpAuthenticationModes httpAuthenticationModes = (HttpAuthenticationModes)GetPropValue("HttpAuthenticationModes");
		sb.Append(", AUTHENTICATION = (");
		flag = false;
		if ((HttpAuthenticationModes.Anonymous & httpAuthenticationModes) != 0)
		{
			flag = true;
			sb.Append("ANONYMOUS");
		}
		if ((HttpAuthenticationModes.Basic & httpAuthenticationModes) != 0)
		{
			if (flag)
			{
				sb.Append(Globals.commaspace);
			}
			flag = true;
			sb.Append("BASIC");
		}
		if ((HttpAuthenticationModes.Digest & httpAuthenticationModes) != 0)
		{
			if (flag)
			{
				sb.Append(Globals.commaspace);
			}
			flag = true;
			sb.Append("DIGEST");
		}
		if ((HttpAuthenticationModes.Ntlm & httpAuthenticationModes) != 0)
		{
			if (flag)
			{
				sb.Append(Globals.commaspace);
			}
			flag = true;
			sb.Append("NTLM");
		}
		if ((HttpAuthenticationModes.Kerberos & httpAuthenticationModes) != 0)
		{
			if (flag)
			{
				sb.Append(Globals.commaspace);
			}
			flag = true;
			sb.Append("KERBEROS");
		}
		if ((HttpAuthenticationModes.Integrated & httpAuthenticationModes) != 0)
		{
			if (flag)
			{
				sb.Append(Globals.commaspace);
			}
			flag = true;
			sb.Append("INTEGRATED");
		}
		sb.Append(Globals.RParen);
		object propValueOptional = GetPropValueOptional("WebSite");
		if (propValueOptional != null)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, ", SITE=N'{0}'", new object[1] { SqlSmoObject.SqlString((string)propValueOptional) });
		}
		if ((HttpPortTypes.Clear & httpPortTypes) != HttpPortTypes.None)
		{
			object propValueOptional2 = GetPropValueOptional("ClearPort");
			if (propValueOptional2 != null && (!sp.ScriptForAlter || base.Properties.Get("ClearPort").Dirty))
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, ", CLEAR_PORT = {0}", new object[1] { (int)propValueOptional2 });
			}
		}
		if ((HttpPortTypes.Ssl & httpPortTypes) != HttpPortTypes.None)
		{
			object propValueOptional3 = GetPropValueOptional("SslPort");
			if (propValueOptional3 != null && (!sp.ScriptForAlter || base.Properties.Get("SslPort").Dirty))
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, ", SSL_PORT = {0}", new object[1] { (int)propValueOptional3 });
			}
		}
		if ((HttpAuthenticationModes.Digest & httpAuthenticationModes) != 0 && GetPropValueOptional("AuthenticationRealm") is string { Length: >0 } text)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, ", AUTH_REALM=N'{0}'", new object[1] { SqlSmoObject.SqlString(text) });
		}
		if ((HttpAuthenticationModes.Basic & httpAuthenticationModes) != 0 && GetPropValueOptional("DefaultLogonDomain") is string { Length: >0 } text2)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, ", DEFAULT_LOGON_DOMAIN=N'{0}'", new object[1] { SqlSmoObject.SqlString(text2) });
		}
		object propValueOptional4 = GetPropValueOptional("IsCompressionEnabled");
		if (propValueOptional4 != null)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, ", COMPRESSION={0}", new object[1] { ((bool)propValueOptional4) ? "ENABLED" : "DISABLED" });
		}
	}
}
