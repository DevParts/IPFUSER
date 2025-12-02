using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[StateChangeEvent("ALTER_AUTHORIZATION_SERVER", "ENDPOINT")]
[PhysicalFacet]
[StateChangeEvent("CREATE_ENDPOINT", "ENDPOINT")]
[StateChangeEvent("ALTER_ENDPOINT", "ENDPOINT")]
[SfcElement(SfcElementFlags.Standalone)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public sealed class Endpoint : ScriptNameObjectBase, IObjectPermission, ICreatable, IAlterable, IDroppable, IDropIfExists, IScriptable
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
				"EndpointState" => 0, 
				"EndpointType" => 1, 
				"ID" => 2, 
				"IsAdminEndpoint" => 3, 
				"IsSystemObject" => 4, 
				"Owner" => 5, 
				"ProtocolType" => 6, 
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
				new StaticMetadata("EndpointState", expensive: false, readOnly: true, typeof(EndpointState)),
				new StaticMetadata("EndpointType", expensive: false, readOnly: false, typeof(EndpointType)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("IsAdminEndpoint", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ProtocolType", expensive: false, readOnly: false, typeof(ProtocolType)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState))
			};
		}
	}

	private Payload m_payload;

	private Protocol m_protocol;

	internal object oldEndpointTypeValue;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Server Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as Server;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	internal override UserPermissionCollection Permissions => GetUserPermissions();

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public EndpointState EndpointState => (EndpointState)base.Properties.GetValueWithNullReplacement("EndpointState");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public EndpointType EndpointType
	{
		get
		{
			return (EndpointType)base.Properties.GetValueWithNullReplacement("EndpointType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EndpointType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsAdminEndpoint => (bool)base.Properties.GetValueWithNullReplacement("IsAdminEndpoint");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcProperty(SfcPropertyFlags.Standalone)]
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
	public ProtocolType ProtocolType
	{
		get
		{
			return (ProtocolType)base.Properties.GetValueWithNullReplacement("ProtocolType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ProtocolType", value);
		}
	}

	public static string UrnSuffix => "Endpoint";

	[SfcObject(SfcObjectRelationship.ChildObject, SfcObjectCardinality.One)]
	public Payload Payload
	{
		get
		{
			if (m_payload == null)
			{
				m_payload = new Payload(this);
			}
			return m_payload;
		}
	}

	[SfcObject(SfcObjectRelationship.ChildObject, SfcObjectCardinality.One)]
	public Protocol Protocol
	{
		get
		{
			if (m_protocol == null)
			{
				m_protocol = new Protocol(this);
			}
			return m_protocol;
		}
	}

	public Endpoint()
	{
	}

	public Endpoint(Server server, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = server;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
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

	internal Endpoint(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string name = FormatFullNameForScripting(sp);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, name, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_ENDPOINT, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		ScriptEndpoint(stringBuilder, sp);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
		}
		queries.Add(stringBuilder.ToString());
	}

	private void ScriptEndpoint(StringBuilder sb, ScriptingPreferences sp)
	{
		string text = FormatFullNameForScripting(sp);
		sb.AppendFormat(SmoApplication.DefaultCulture, "{0} ENDPOINT {1} ", new object[2]
		{
			sp.ScriptForAlter ? "ALTER" : "CREATE",
			text
		});
		if (!sp.ScriptForAlter && sp.IncludeScripts.Owner)
		{
			object propValueOptional = GetPropValueOptional("Owner");
			if (propValueOptional != null)
			{
				sb.Append(Globals.newline);
				sb.Append(Globals.tab);
				sb.AppendFormat(SmoApplication.DefaultCulture, "AUTHORIZATION {0}", new object[1] { SqlSmoObject.MakeSqlBraket((string)propValueOptional) });
			}
		}
		object propValueOptional2 = GetPropValueOptional("EndpointState");
		if (propValueOptional2 != null)
		{
			string text2 = string.Empty;
			switch ((EndpointState)propValueOptional2)
			{
			case EndpointState.Started:
				text2 = "STARTED";
				break;
			case EndpointState.Stopped:
				text2 = "STOPPED";
				break;
			case EndpointState.Disabled:
				text2 = "DISABLED";
				break;
			}
			if (text2.Length > 0)
			{
				sb.Append(Globals.newline);
				sb.Append(Globals.tab);
				sb.AppendFormat(SmoApplication.DefaultCulture, "STATE={0}", new object[1] { text2 });
			}
		}
		sb.Append(Globals.newline);
		sb.Append(Globals.tab);
		sb.Append("AS ");
		Protocol.Script(sb, sp);
		sb.Append(Globals.newline);
		sb.Append(Globals.tab);
		sb.Append("FOR ");
		Payload.Script(sb, sp);
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		if (IsDllDirty() || SqlSmoObject.IsCollectionDirty(Payload.Soap.SoapPayloadMethods))
		{
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			ScriptEndpoint(stringBuilder, sp);
			string value = stringBuilder.ToString();
			if (!string.IsNullOrEmpty(value))
			{
				alterQuery.Add(value);
			}
		}
		ScriptChangeOwner(alterQuery, sp);
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = FormatFullNameForScripting(sp);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_ENDPOINT, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP ENDPOINT {0}", new object[1] { text });
		dropQuery.Add(stringBuilder.ToString());
	}

	public void Start()
	{
		try
		{
			SetEndpointState(EndpointState.Started);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Start, this, ex);
		}
	}

	public void Stop()
	{
		try
		{
			SetEndpointState(EndpointState.Stopped);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Stop, this, ex);
		}
	}

	public void Disable()
	{
		try
		{
			SetEndpointState(EndpointState.Disabled);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Disable, this, ex);
		}
	}

	private void SetEndpointState(EndpointState newState)
	{
		string text = newState switch
		{
			EndpointState.Started => "STARTED", 
			EndpointState.Stopped => "STOPPED", 
			EndpointState.Disabled => "DISABLED", 
			_ => throw new InternalSmoErrorException(ExceptionTemplatesImpl.UnknownEnumeration("EndpointState")), 
		};
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, "IF (SELECT state FROM sys.endpoints WHERE name = {0}) <> {1}", new object[2]
		{
			SqlSmoObject.MakeSqlString(Name),
			(int)newState
		}));
		stringBuilder.Append(Globals.newline);
		stringBuilder.Append(Scripts.BEGIN);
		stringBuilder.Append(Globals.newline);
		stringBuilder.Append(Globals.tab);
		stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, "ALTER ENDPOINT {0} STATE = {1}", new object[2] { FullQualifiedName, text }));
		stringBuilder.Append(Globals.newline);
		stringBuilder.Append(Scripts.END);
		stringBuilder.Append(Globals.newline);
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(stringBuilder.ToString());
		ExecutionManager.ExecuteNonQuery(stringCollection);
		if (!ExecutionManager.Recording)
		{
			base.Properties["EndpointState"].SetValue(newState);
		}
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	private bool IsDllDirty()
	{
		foreach (Property property in base.Properties)
		{
			if (property.Name != "Owner" && property.Dirty)
			{
				return true;
			}
		}
		if ((Protocol.EndpointProtocol != null && Protocol.EndpointProtocol.InternalIsObjectDirty) || (Payload.EndpointPayload != null && Payload.EndpointPayload.InternalIsObjectDirty))
		{
			return true;
		}
		return false;
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		return new PropagateInfo[2]
		{
			new PropagateInfo(Protocol.EndpointProtocol, bWithScript: false),
			new PropagateInfo(Payload.EndpointPayload, bWithScript: false)
		};
	}

	protected override void MarkDropped()
	{
		Protocol.MarkDropped();
		Payload.MarkDropped();
		base.MarkDropped();
	}

	internal override void ValidateProperty(Property prop, object value)
	{
		if (prop.Name == "EndpointType" && !prop.Dirty)
		{
			oldEndpointTypeValue = prop.Value;
		}
	}

	public override void Refresh()
	{
		base.Refresh();
		oldEndpointTypeValue = null;
	}
}
