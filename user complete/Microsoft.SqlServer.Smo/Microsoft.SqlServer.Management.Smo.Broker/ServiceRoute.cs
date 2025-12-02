using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo.Broker;

[PhysicalFacet]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.Broker.BrokerLocalizableResources", true)]
[TypeConverter(typeof(LocalizableTypeConverter))]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public sealed class ServiceRoute : BrokerObjectBase, IObjectPermission, IExtendedProperties, ICreatable, IAlterable, IDroppable, IDropIfExists
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 7, 8, 8, 8, 8, 8, 8, 8 };

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
				"Address" => 0, 
				"BrokerInstance" => 1, 
				"ExpirationDate" => 2, 
				"ID" => 3, 
				"MirrorAddress" => 4, 
				"Owner" => 5, 
				"RemoteService" => 6, 
				"PolicyHealthState" => 7, 
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
				new StaticMetadata("Address", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("BrokerInstance", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ExpirationDate", expensive: false, readOnly: false, typeof(DateTime)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("MirrorAddress", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("RemoteService", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState))
			};
		}
	}

	private ServiceRouteEvents events;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public ServiceBroker Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as ServiceBroker;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	internal override UserPermissionCollection Permissions => GetUserPermissions();

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string Address
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Address");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Address", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string BrokerInstance
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("BrokerInstance");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BrokerInstance", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime ExpirationDate
	{
		get
		{
			return (DateTime)base.Properties.GetValueWithNullReplacement("ExpirationDate");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ExpirationDate", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string MirrorAddress
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("MirrorAddress");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MirrorAddress", value);
		}
	}

	[SfcReference(typeof(DatabaseRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/Role[@Name = '{2}']", new string[] { "Parent.Parent.Parent.ConnectionContext.TrueName", "Parent.Parent.Name", "Owner" })]
	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.Parent.ConnectionContext.TrueName", "Parent.Parent.Name", "Owner" })]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	[SfcReference(typeof(ApplicationRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/ApplicationRole[@Name = '{2}']", new string[] { "Parent.Parent.Parent.ConnectionContext.TrueName", "Parent.Parent.Name", "Owner" })]
	[CLSCompliant(false)]
	public string Owner
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Owner");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Owner", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string RemoteService
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("RemoteService");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RemoteService", value);
		}
	}

	public ServiceRouteEvents Events
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (SqlContext.IsAvailable)
			{
				throw new SmoException(ExceptionTemplatesImpl.SmoSQLCLRUnAvailable);
			}
			if (events == null)
			{
				events = new ServiceRouteEvents(this);
			}
			return events;
		}
	}

	public static string UrnSuffix => "ServiceRoute";

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(ExtendedProperty))]
	public ExtendedPropertyCollection ExtendedProperties
	{
		get
		{
			ThrowIfBelowVersion80();
			CheckObjectState();
			if (m_ExtendedProperties == null)
			{
				m_ExtendedProperties = new ExtendedPropertyCollection(this);
			}
			return m_ExtendedProperties;
		}
	}

	public ServiceRoute()
	{
	}

	public ServiceRoute(ServiceBroker serviceBroker, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = serviceBroker;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[1] { "Owner" };
	}

	public void Deny(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string[] granteeNames, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, null, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, revokeGrant, cascade, asRole);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, revokeGrant, cascade, asRole);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions()
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, null, null);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, granteeName, null);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, null, permissions);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName, ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, granteeName, permissions);
	}

	internal ServiceRoute(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		CreateImpl();
	}

	private void GetDDL(StringCollection queries, ScriptingPreferences sp, bool bCreate)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (bCreate && sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_SERVICE_ROUTE, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
		}
		string empty = string.Empty;
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} ROUTE {1} ", new object[2]
		{
			bCreate ? "CREATE" : "ALTER",
			FormatFullNameForScripting(sp)
		});
		if (bCreate && sp.IncludeScripts.Owner)
		{
			empty = (string)GetPropValueOptional("Owner");
			if (empty != null && empty.Length > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "  AUTHORIZATION [{0}] ", new object[1] { SqlSmoObject.SqlBraket(empty) });
			}
		}
		bool flag = false;
		empty = (string)GetPropValueOptional("RemoteService");
		if (empty != null && empty.Length > 0)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "  WITH  SERVICE_NAME  = {0} ", new object[1] { SqlSmoObject.MakeSqlString(empty) });
			flag = true;
		}
		empty = (string)GetPropValueOptional("BrokerInstance");
		if (empty != null && empty.Length > 0)
		{
			if (flag)
			{
				stringBuilder.Append(", ");
			}
			else
			{
				stringBuilder.Append(" WITH ");
				flag = true;
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " BROKER_INSTANCE  = {0} ", new object[1] { SqlSmoObject.MakeSqlString(empty) });
		}
		Property property = base.Properties.Get("ExpirationDate");
		if (property.Value != null)
		{
			DateTime dateTime = (DateTime)property.Value;
			if (dateTime != DateTime.MinValue)
			{
				if (flag)
				{
					stringBuilder.Append(", ");
				}
				else
				{
					stringBuilder.Append(" WITH ");
					flag = true;
				}
				TimeSpan timeSpan = dateTime.ToLocalTime() - DateTime.Now;
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " LIFETIME  = {0} ", new object[1] { Convert.ToInt32(timeSpan.TotalSeconds) });
			}
		}
		empty = ((!bCreate) ? ((string)GetPropValueOptional("Address")) : ((string)GetPropValue("Address")));
		if (empty != null && empty.Length > 0)
		{
			if (flag)
			{
				stringBuilder.Append(", ");
			}
			else
			{
				stringBuilder.Append(" WITH ");
				flag = true;
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " ADDRESS  = {0} ", new object[1] { SqlSmoObject.MakeSqlString(empty) });
		}
		empty = (string)GetPropValueOptional("MirrorAddress");
		property = base.Properties.Get("MirrorAddress");
		if (empty != null && empty.Length > 0)
		{
			if (flag)
			{
				stringBuilder.Append(", ");
			}
			else
			{
				stringBuilder.Append(" WITH ");
				flag = true;
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " MIRROR_ADDRESS  = {0} ", new object[1] { SqlSmoObject.MakeSqlString(empty) });
		}
		queries.Add(stringBuilder.ToString());
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal);
		GetDDL(queries, sp, bCreate: true);
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		if (IsObjectDirty())
		{
			GetDDL(queries, sp, bCreate: false);
		}
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_SERVICE_ROUTE, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP ROUTE {0}", new object[1] { FormatFullNameForScripting(sp) });
		queries.Add(stringBuilder.ToString());
	}
}
