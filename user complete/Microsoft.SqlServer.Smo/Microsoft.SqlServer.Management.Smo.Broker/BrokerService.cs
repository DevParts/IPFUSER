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

[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.Broker.BrokerLocalizableResources", true)]
[TypeConverter(typeof(LocalizableTypeConverter))]
[PhysicalFacet]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public sealed class BrokerService : BrokerObjectBase, IObjectPermission, IExtendedProperties, ICreatable, IAlterable, IDroppable, IDropIfExists
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 5, 6, 6, 6, 6, 6, 6, 6 };

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
				"ID" => 0, 
				"IsSystemObject" => 1, 
				"Owner" => 2, 
				"QueueName" => 3, 
				"QueueSchema" => 4, 
				"PolicyHealthState" => 5, 
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
			staticMetadata = new StaticMetadata[6]
			{
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("QueueName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("QueueSchema", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState))
			};
		}
	}

	private BrokerServiceEvents events;

	private ServiceContractMappingCollection m_ServiceContractMappings;

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
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[CLSCompliant(false)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.Parent.ConnectionContext.TrueName", "Parent.Parent.Name", "Owner" })]
	[SfcReference(typeof(DatabaseRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/Role[@Name = '{2}']", new string[] { "Parent.Parent.Parent.ConnectionContext.TrueName", "Parent.Parent.Name", "Owner" })]
	[SfcReference(typeof(ApplicationRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/ApplicationRole[@Name = '{2}']", new string[] { "Parent.Parent.Parent.ConnectionContext.TrueName", "Parent.Parent.Name", "Owner" })]
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
	[SfcReference(typeof(ServiceQueue), "Server[@Name = '{0}']/Database[@Name = '{1}']/ServiceBroker/ServiceQueue[@Name='{2}' and @Schema='{3}']", new string[] { "Parent.Parent.Parent.ConnectionContext.TrueName", "Parent.Parent.Name", "QueueName", "QueueSchema" })]
	[CLSCompliant(false)]
	public string QueueName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("QueueName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("QueueName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone, "dbo")]
	public string QueueSchema
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("QueueSchema");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("QueueSchema", value);
		}
	}

	public BrokerServiceEvents Events
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
				events = new BrokerServiceEvents(this);
			}
			return events;
		}
	}

	public static string UrnSuffix => "BrokerService";

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(ServiceContractMapping))]
	public ServiceContractMappingCollection ServiceContractMappings
	{
		get
		{
			CheckObjectState();
			if (m_ServiceContractMappings == null)
			{
				m_ServiceContractMappings = new ServiceContractMappingCollection(this);
			}
			return m_ServiceContractMappings;
		}
	}

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

	public BrokerService()
	{
	}

	public BrokerService(ServiceBroker serviceBroker, string name)
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

	internal override object GetPropertyDefaultValue(string propname)
	{
		string text;
		if ((text = propname) != null && text == "QueueSchema")
		{
			return "dbo";
		}
		return base.GetPropertyDefaultValue(propname);
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

	internal BrokerService(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	private void GetDDL(StringCollection queries, ScriptingPreferences sp, bool bCreate)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (bCreate && sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_BROKER_SERVICE, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} SERVICE {1} ", new object[2]
		{
			bCreate ? "CREATE" : "ALTER",
			FormatFullNameForScripting(sp)
		});
		if (bCreate && sp.IncludeScripts.Owner)
		{
			Property property = base.Properties.Get("Owner");
			if (property.Value != null)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " AUTHORIZATION [{0}] ", new object[1] { SqlSmoObject.SqlBraket(property.Value as string) });
			}
		}
		Property property2 = base.Properties["QueueName"];
		string propValueOptional = GetPropValueOptional("QueueSchema", string.Empty);
		if (property2.Value != null)
		{
			if (propValueOptional.Length > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " ON QUEUE [{0}].[{1}] ", new object[2]
				{
					SqlSmoObject.SqlBraket(propValueOptional),
					SqlSmoObject.SqlBraket(property2.Value as string)
				});
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " ON QUEUE [{0}] ", new object[1] { SqlSmoObject.SqlBraket(property2.Value as string) });
			}
		}
		if (bCreate && ServiceContractMappings.Count > 0)
		{
			stringBuilder.Append(Globals.LParen);
			bool flag = false;
			foreach (ServiceContractMapping serviceContractMapping3 in ServiceContractMappings)
			{
				if (flag)
				{
					stringBuilder.Append(Globals.comma);
					stringBuilder.Append(Globals.newline);
				}
				else
				{
					flag = true;
				}
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "[{0}]", new object[1] { SqlSmoObject.SqlBraket(serviceContractMapping3.Name) });
			}
			stringBuilder.Append(Globals.RParen);
		}
		else if (!bCreate)
		{
			bool flag2 = false;
			bool flag3 = false;
			foreach (ServiceContractMapping serviceContractMapping4 in ServiceContractMappings)
			{
				if (serviceContractMapping4.State == SqlSmoState.Creating)
				{
					if (!flag2)
					{
						stringBuilder.Append(Globals.LParen);
						flag2 = true;
					}
					if (flag3)
					{
						stringBuilder.Append(Globals.comma);
						stringBuilder.Append(Globals.newline);
					}
					else
					{
						flag3 = true;
					}
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ADD CONTRACT [{0}]", new object[1] { SqlSmoObject.SqlBraket(serviceContractMapping4.Name) });
				}
				else if (serviceContractMapping4.State == SqlSmoState.ToBeDropped)
				{
					if (!flag2)
					{
						stringBuilder.Append(Globals.LParen);
						flag2 = true;
					}
					if (flag3)
					{
						stringBuilder.Append(Globals.comma);
						stringBuilder.Append(Globals.newline);
					}
					else
					{
						flag3 = true;
					}
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP CONTRACT [{0}]", new object[1] { SqlSmoObject.SqlBraket(serviceContractMapping4.Name) });
				}
			}
			if (flag2)
			{
				stringBuilder.Append(Globals.RParen);
			}
		}
		queries.Add(stringBuilder.ToString());
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal);
		GetDDL(queries, sp, bCreate: true);
	}

	protected override void PostCreate()
	{
		SqlSmoObject.UpdateCollectionState2(ServiceContractMappings);
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
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_BROKER_SERVICE, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP SERVICE {0}", new object[1] { FormatFullNameForScripting(sp) });
		queries.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		if (IsObjectDirty())
		{
			GetDDL(alterQuery, sp, bCreate: false);
		}
	}

	protected override bool IsObjectDirty()
	{
		if (!base.IsObjectDirty())
		{
			return SqlSmoObject.IsCollectionDirty(ServiceContractMappings);
		}
		return true;
	}
}
