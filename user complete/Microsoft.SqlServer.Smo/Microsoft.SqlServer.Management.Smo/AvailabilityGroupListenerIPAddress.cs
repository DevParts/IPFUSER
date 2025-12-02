using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Text;
using Microsoft.SqlServer.Diagnostics.STrace;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone)]
public sealed class AvailabilityGroupListenerIPAddress : SqlSmoObject, ICreatable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 8, 8, 8, 8, 8 };

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
				"IPAddress" => 0, 
				"IPAddressState" => 1, 
				"IPAddressStateDescription" => 2, 
				"IsDHCP" => 3, 
				"SubnetIP" => 4, 
				"SubnetIPv4Mask" => 5, 
				"SubnetMask" => 6, 
				"SubnetPrefixLength" => 7, 
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
			staticMetadata = new StaticMetadata[8]
			{
				new StaticMetadata("IPAddress", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("IPAddressState", expensive: false, readOnly: true, typeof(AvailabilityGroupListenerIPState)),
				new StaticMetadata("IPAddressStateDescription", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("IsDHCP", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("SubnetIP", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("SubnetIPv4Mask", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("SubnetMask", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("SubnetPrefixLength", expensive: false, readOnly: true, typeof(int))
			};
		}
	}

	internal const string IpAddressPropertyName = "IPAddress";

	internal const string SubnetMaskPropertyName = "SubnetMask";

	internal const string SubnetIpPropertyName = "SubnetIP";

	internal const string IsDHCPPropertyName = "IsDHCP";

	private const string IPScript = "IP";

	private static readonly TraceContext tc = TraceContext.GetTraceContext(SmoApplication.ModuleName, "AvailabilityGroupListenerIPAddress");

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public AvailabilityGroupListener Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as AvailabilityGroupListener;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityGroupListenerIPState IPAddressState => (AvailabilityGroupListenerIPState)base.Properties.GetValueWithNullReplacement("IPAddressState");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string IPAddressStateDescription => (string)base.Properties.GetValueWithNullReplacement("IPAddressStateDescription");

	[SfcProperty(SfcPropertyFlags.Standalone, "false")]
	public bool IsDHCP
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsDHCP");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsDHCP", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone, "string.empty")]
	public string SubnetIPv4Mask => (string)base.Properties.GetValueWithNullReplacement("SubnetIPv4Mask");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int SubnetPrefixLength => (int)base.Properties.GetValueWithNullReplacement("SubnetPrefixLength");

	public bool IsIPv6 => string.IsNullOrEmpty(SubnetMask);

	[SfcProperty(SfcPropertyFlags.Standalone)]
	[SfcKey(0)]
	public string IPAddress
	{
		get
		{
			return ((AvailabilityGroupListenerIPAddressObjectKey)key).IPAddress;
		}
		set
		{
			try
			{
				((AvailabilityGroupListenerIPAddressObjectKey)key).IPAddress = value;
				UpdateObjectState();
			}
			catch (Exception ex)
			{
				SqlSmoObject.FilterException(ex);
				throw new FailedOperationException(ExceptionTemplatesImpl.SetIpAddress, this, ex);
			}
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	[SfcKey(1)]
	public string SubnetMask
	{
		get
		{
			return ((AvailabilityGroupListenerIPAddressObjectKey)key).SubnetMask;
		}
		set
		{
			try
			{
				((AvailabilityGroupListenerIPAddressObjectKey)key).SubnetMask = value;
				UpdateObjectState();
			}
			catch (Exception ex)
			{
				SqlSmoObject.FilterException(ex);
				throw new FailedOperationException(ExceptionTemplatesImpl.SetSubnetMask, this, ex);
			}
		}
	}

	[SfcKey(2)]
	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string SubnetIP
	{
		get
		{
			return ((AvailabilityGroupListenerIPAddressObjectKey)key).SubnetIP;
		}
		set
		{
			try
			{
				((AvailabilityGroupListenerIPAddressObjectKey)key).SubnetIP = value;
				UpdateObjectState();
			}
			catch (Exception ex)
			{
				SqlSmoObject.FilterException(ex);
				throw new FailedOperationException(ExceptionTemplatesImpl.SetSubnetIp, this, ex);
			}
		}
	}

	public static string UrnSuffix => "AvailabilityGroupListenerIPAddress";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		return propname switch
		{
			"IsDHCP" => false, 
			"SubnetIP" => "string.empty", 
			"SubnetIPv4Mask" => "string.empty", 
			"SubnetMask" => "string.empty", 
			_ => base.GetPropertyDefaultValue(propname), 
		};
	}

	public AvailabilityGroupListenerIPAddress()
	{
	}

	public AvailabilityGroupListenerIPAddress(AvailabilityGroupListener availabilityGroupListener)
	{
		Parent = availabilityGroupListener;
	}

	internal AvailabilityGroupListenerIPAddress(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		CreateImpl();
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	internal override void ScriptCreate(StringCollection query, ScriptingPreferences sp)
	{
		tc.Assert(null != query, "String collection should not be null");
		SqlSmoObject.ThrowIfBelowVersion110(sp.TargetServerVersionInternal);
		SqlSmoObject.ThrowIfCloud(sp.TargetDatabaseEngineType, ExceptionTemplatesImpl.UnsupportedEngineTypeException);
		if (GetPropValueOptional("IsDHCP") != null && IsDHCP)
		{
			throw new InvalidOperationException(ExceptionTemplatesImpl.CannotAddDHCPIPAddress(UrnSuffix, "IsDHCP"));
		}
		if (string.IsNullOrEmpty(IPAddress))
		{
			throw new PropertyNotSetException("IPAddress");
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		stringBuilder.Append(Scripts.ALTER + Globals.space + AvailabilityGroup.AvailabilityGroupScript + Globals.space);
		stringBuilder.Append(SqlSmoObject.MakeSqlBraket(Parent.Parent.Name) + Globals.newline);
		stringBuilder.Append("MODIFY LISTENER" + Globals.space + SqlSmoObject.MakeSqlString(Parent.Name) + Globals.newline);
		stringBuilder.Append(Globals.LParen + Scripts.ADD + Globals.space + "IP" + Globals.space + Globals.LParen);
		if (IsIPv6)
		{
			stringBuilder.Append(SqlSmoObject.MakeSqlString(IPAddress));
		}
		else
		{
			stringBuilder.Append(SqlSmoObject.MakeSqlString(IPAddress) + Globals.comma + Globals.space + SqlSmoObject.MakeSqlString(SubnetMask));
		}
		stringBuilder.Append(Globals.RParen);
		stringBuilder.Append(Globals.newline + Globals.RParen);
		stringBuilder.Append(Globals.statementTerminator);
		query.Add(stringBuilder.ToString());
	}

	internal override ObjectKeyBase GetEmptyKey()
	{
		return new AvailabilityGroupListenerIPAddressObjectKey();
	}

	internal override void UpdateObjectState()
	{
		if (base.State == SqlSmoState.Pending && base.ParentColl != null)
		{
			SetState(SqlSmoState.Creating);
		}
	}

	internal void FetchKeyPostCreate()
	{
		if (ExecutionManager.Recording || !IsDHCP)
		{
			return;
		}
		try
		{
			string[] fields = new string[3] { "IPAddress", "SubnetMask", "SubnetIP" };
			Urn urn = string.Format(CultureInfo.InvariantCulture, "{0}/{1}[@{2}={3}]", Parent.Urn, UrnSuffix, "IsDHCP", 1);
			Request req = new Request(urn, fields);
			DataTable enumeratorData = ExecutionManager.GetEnumeratorData(req);
			if (enumeratorData != null && enumeratorData.Rows.Count == 1)
			{
				DataRow dataRow = enumeratorData.Rows[0];
				IPAddress = dataRow["IPAddress"] as string;
				SubnetIP = dataRow["SubnetMask"] as string;
				SubnetMask = dataRow["SubnetIP"] as string;
				return;
			}
			throw new SmoException(ExceptionTemplatesImpl.GetDHCPAddressFailed(Parent.Name, enumeratorData.Rows.Count));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.GetDHCPAddress, Parent, ex);
		}
	}
}
