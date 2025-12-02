using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Diagnostics.STrace;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone)]
public sealed class AvailabilityGroupListener : NamedSmoObject, ICreatable, IDroppable, IDropIfExists, IAlterable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 4, 4, 4, 4, 4 };

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
				"ClusterIPConfiguration" => 0, 
				"IsConformant" => 1, 
				"PortNumber" => 2, 
				"UniqueId" => 3, 
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
				new StaticMetadata("ClusterIPConfiguration", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("IsConformant", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("PortNumber", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("UniqueId", expensive: false, readOnly: true, typeof(string))
			};
		}
	}

	internal const string ModifyListenerScript = "MODIFY LISTENER";

	internal const string RestartListenerScript = "RESTART LISTENER";

	internal const string WithDHCPScript = "WITH DHCP";

	internal const string WithIPScript = "WITH IP";

	private static readonly TraceContext tc = TraceContext.GetTraceContext(SmoApplication.ModuleName, "AvailabilityGroupListener");

	private AvailabilityGroupListenerIPAddressCollection availabilityGroupListenerIPAddresses;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public AvailabilityGroup Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as AvailabilityGroup;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ClusterIPConfiguration => (string)base.Properties.GetValueWithNullReplacement("ClusterIPConfiguration");

	[SfcProperty(SfcPropertyFlags.Standalone, "true")]
	public bool IsConformant => (bool)base.Properties.GetValueWithNullReplacement("IsConformant");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int PortNumber
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("PortNumber");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PortNumber", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string UniqueId => (string)base.Properties.GetValueWithNullReplacement("UniqueId");

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.OneToAny, typeof(AvailabilityGroupListenerIPAddress))]
	public AvailabilityGroupListenerIPAddressCollection AvailabilityGroupListenerIPAddresses
	{
		get
		{
			if (availabilityGroupListenerIPAddresses == null)
			{
				availabilityGroupListenerIPAddresses = new AvailabilityGroupListenerIPAddressCollection(this);
			}
			return availabilityGroupListenerIPAddresses;
		}
	}

	public static string UrnSuffix => "AvailabilityGroupListener";

	public AvailabilityGroupListener()
	{
	}

	public AvailabilityGroupListener(AvailabilityGroup availabilityGroup, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = availabilityGroup;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		string text;
		if ((text = propname) != null && text == "IsConformant")
		{
			return true;
		}
		return base.GetPropertyDefaultValue(propname);
	}

	internal AvailabilityGroupListener(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void RestartListener()
	{
		CheckObjectState(!ExecutionManager.Recording);
		try
		{
			string cmd = Scripts.ALTER + Globals.space + AvailabilityGroup.AvailabilityGroupScript + Globals.space + SqlSmoObject.MakeSqlBraket(Parent.Name) + Globals.space + "RESTART LISTENER" + Globals.space + SqlSmoObject.MakeSqlString(Name) + Globals.statementTerminator;
			ExecutionManager.ExecuteNonQuery(cmd);
			GenerateAlterEvent();
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RestartListenerFailed(Name, Parent.Name), ex);
		}
	}

	public void Create()
	{
		CreateImpl();
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	public void Alter()
	{
		CheckObjectState(throwIfNotCreated: true);
		AlterImpl();
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
		ValidateIPAddresses();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			string text = FormatFullNameForScripting(sp, nameIsIndentifier: false);
			string text2 = Parent.FormatFullNameForScripting(sp, nameIsIndentifier: false);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AVAILABILITY_GROUP_LISTENER, new object[3] { "NOT", text, text2 });
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append(Scripts.ALTER + Globals.space + AvailabilityGroup.AvailabilityGroupScript + Globals.space);
		stringBuilder.Append(SqlSmoObject.MakeSqlBraket(Parent.Name) + Globals.newline);
		stringBuilder.Append(Scripts.ADD + Globals.space + AvailabilityGroup.ListenerScript + Globals.space);
		stringBuilder.Append(SqlSmoObject.MakeSqlString(Name) + Globals.space + Globals.LParen);
		stringBuilder.Append(ScriptListenerOptions());
		stringBuilder.Append(Globals.RParen);
		stringBuilder.Append(Globals.statementTerminator);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		query.Add(stringBuilder.ToString());
	}

	internal void ValidateIPAddresses()
	{
		if (AvailabilityGroupListenerIPAddresses == null || AvailabilityGroupListenerIPAddresses.Count < 1)
		{
			throw new SmoException(ExceptionTemplatesImpl.ObjectWithNoChildren(UrnSuffix, AvailabilityGroupListenerIPAddress.UrnSuffix));
		}
		bool isDHCP = AvailabilityGroupListenerIPAddresses[0].IsDHCP;
		foreach (AvailabilityGroupListenerIPAddress availabilityGroupListenerIPAddress in AvailabilityGroupListenerIPAddresses)
		{
			if (availabilityGroupListenerIPAddress.IsDHCP != isDHCP)
			{
				throw new SmoException(ExceptionTemplatesImpl.WrongHybridIPAddresses(UrnSuffix));
			}
		}
		if (isDHCP)
		{
			if (AvailabilityGroupListenerIPAddresses.Count > 1)
			{
				throw new SmoException(ExceptionTemplatesImpl.WrongMultiDHCPIPAddresses(UrnSuffix));
			}
			if (!string.IsNullOrEmpty(AvailabilityGroupListenerIPAddresses[0].IPAddress) && AvailabilityGroupListenerIPAddresses[0].IsIPv6)
			{
				throw new SmoException(ExceptionTemplatesImpl.WrongDHCPv6IPAddress(UrnSuffix));
			}
		}
	}

	internal string ScriptListenerOptions()
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.Append(Globals.newline);
		if (AvailabilityGroupListenerIPAddresses[0].IsDHCP)
		{
			stringBuilder.Append("WITH DHCP" + Globals.newline);
			if (!string.IsNullOrEmpty(AvailabilityGroupListenerIPAddresses[0].SubnetIP))
			{
				stringBuilder.Append(Globals.space + Globals.On + Globals.space + Globals.LParen);
				stringBuilder.Append(SqlSmoObject.MakeSqlString(AvailabilityGroupListenerIPAddresses[0].SubnetIP) + Globals.comma + Globals.space + SqlSmoObject.MakeSqlString(AvailabilityGroupListenerIPAddresses[0].SubnetMask));
				stringBuilder.Append(Globals.newline + Globals.RParen + Globals.newline);
			}
		}
		else
		{
			stringBuilder.Append("WITH IP" + Globals.newline + Globals.LParen);
			bool flag = false;
			foreach (AvailabilityGroupListenerIPAddress availabilityGroupListenerIPAddress in AvailabilityGroupListenerIPAddresses)
			{
				if (flag)
				{
					stringBuilder.Append(Globals.comma + Globals.newline);
				}
				if (availabilityGroupListenerIPAddress.IsIPv6)
				{
					stringBuilder.Append(Globals.LParen + SqlSmoObject.MakeSqlString(availabilityGroupListenerIPAddress.IPAddress) + Globals.RParen);
				}
				else
				{
					stringBuilder.Append(Globals.LParen + SqlSmoObject.MakeSqlString(availabilityGroupListenerIPAddress.IPAddress) + Globals.comma + Globals.space + SqlSmoObject.MakeSqlString(availabilityGroupListenerIPAddress.SubnetMask) + Globals.RParen);
				}
				flag = true;
			}
			stringBuilder.Append(Globals.newline + Globals.RParen + Globals.newline);
		}
		stringBuilder.Append(Globals.comma + Globals.space + AvailabilityGroup.PortScript + Globals.EqualSign + PortNumber);
		return stringBuilder.ToString();
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		tc.Assert(null != query, "String collection should not be null");
		SqlSmoObject.ThrowIfBelowVersion110(sp.TargetServerVersionInternal);
		SqlSmoObject.ThrowIfCloud(sp.TargetDatabaseEngineType, ExceptionTemplatesImpl.UnsupportedEngineTypeException);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			string text = FormatFullNameForScripting(sp, nameIsIndentifier: false);
			string text2 = Parent.FormatFullNameForScripting(sp, nameIsIndentifier: false);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AVAILABILITY_GROUP_LISTENER, new object[3]
			{
				string.Empty,
				text,
				text2
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append(Scripts.ALTER + Globals.space + AvailabilityGroup.AvailabilityGroupScript + Globals.space);
		stringBuilder.Append(SqlSmoObject.MakeSqlBraket(Parent.Name) + Globals.newline);
		stringBuilder.Append("MODIFY LISTENER" + Globals.space + SqlSmoObject.MakeSqlString(Name) + Globals.space + Globals.LParen);
		stringBuilder.Append(AvailabilityGroup.PortScript + Globals.EqualSign + PortNumber);
		stringBuilder.Append(Globals.RParen);
		stringBuilder.Append(Globals.statementTerminator);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		query.Add(stringBuilder.ToString());
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		tc.Assert(null != dropQuery, "String collection should not be null");
		SqlSmoObject.ThrowIfBelowVersion110(sp.TargetServerVersionInternal);
		SqlSmoObject.ThrowIfCloud(sp.TargetDatabaseEngineType, ExceptionTemplatesImpl.UnsupportedEngineTypeException);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			string text = FormatFullNameForScripting(sp, nameIsIndentifier: false);
			string text2 = Parent.FormatFullNameForScripting(sp, nameIsIndentifier: false);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AVAILABILITY_GROUP_LISTENER, new object[3]
			{
				string.Empty,
				text,
				text2
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append(Scripts.ALTER + Globals.space + AvailabilityGroup.AvailabilityGroupScript + Globals.space);
		stringBuilder.Append(SqlSmoObject.MakeSqlBraket(Parent.Name) + Globals.newline);
		stringBuilder.Append(Scripts.REMOVE + Globals.space + AvailabilityGroup.ListenerScript + Globals.space + SqlSmoObject.MakeSqlString(Name));
		stringBuilder.Append(Globals.statementTerminator);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		dropQuery.Add(stringBuilder.ToString());
	}

	internal void FetchIpAddressKeysPostCreate()
	{
		foreach (AvailabilityGroupListenerIPAddress availabilityGroupListenerIPAddress in AvailabilityGroupListenerIPAddresses)
		{
			availabilityGroupListenerIPAddress.FetchKeyPostCreate();
		}
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		if (action == PropagateAction.Create)
		{
			return new PropagateInfo[1]
			{
				new PropagateInfo(AvailabilityGroupListenerIPAddresses, bWithScript: false)
			};
		}
		return null;
	}

	protected override void PostCreate()
	{
		FetchIpAddressKeysPostCreate();
	}
}
