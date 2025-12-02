using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[PhysicalFacet(PhysicalFacetOptions.ReadOnly)]
public sealed class View : TableViewBase, ISfcSupportsDesignMode, IColumnPermission, IObjectPermission, ICreatable, ICreateOrAlterable, IAlterable, IDroppable, IDropIfExists, IRenamable, IExtendedProperties, ITextObject, IViewOptions, IDmfFacet
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 14, 21, 24, 25, 25, 25, 25, 25, 25, 25 };

		private static int[] cloudVersionCount = new int[3] { 24, 24, 24 };

		private static int sqlDwPropertyCount = 19;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[19]
		{
			new StaticMetadata("AnsiNullsStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("BodyStartIndex", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("HasClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasColumnSpecification", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("HasIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasNonClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasPrimaryClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsEncrypted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsIndexable", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSchemaBound", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("QuotedIdentifierStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ReturnsViewMetadata", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Text", expensive: true, readOnly: true, typeof(string))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[24]
		{
			new StaticMetadata("AnsiNullsStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("BodyStartIndex", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("HasAfterTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasColumnSpecification", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("HasDeleteTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasInsertTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasInsteadOfTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasNonClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasPrimaryClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasUpdateTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsEncrypted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsIndexable", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSchemaBound", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("QuotedIdentifierStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ReturnsViewMetadata", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Text", expensive: true, readOnly: true, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[25]
		{
			new StaticMetadata("AnsiNullsStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("BodyStartIndex", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("HasClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasColumnSpecification", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("HasNonClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasPrimaryClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsEncrypted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSchemaBound", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("QuotedIdentifierStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Text", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("HasAfterTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasDeleteTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasInsertTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasInsteadOfTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasUpdateTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsIndexable", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ReturnsViewMetadata", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState))
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
					return propertyName switch
					{
						"AnsiNullsStatus" => 0, 
						"BodyStartIndex" => 1, 
						"CreateDate" => 2, 
						"DateLastModified" => 3, 
						"HasClusteredIndex" => 4, 
						"HasColumnSpecification" => 5, 
						"HasIndex" => 6, 
						"HasNonClusteredIndex" => 7, 
						"HasPrimaryClusteredIndex" => 8, 
						"ID" => 9, 
						"IsEncrypted" => 10, 
						"IsIndexable" => 11, 
						"IsSchemaBound" => 12, 
						"IsSchemaOwned" => 13, 
						"IsSystemObject" => 14, 
						"Owner" => 15, 
						"QuotedIdentifierStatus" => 16, 
						"ReturnsViewMetadata" => 17, 
						"Text" => 18, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"AnsiNullsStatus" => 0, 
					"BodyStartIndex" => 1, 
					"CreateDate" => 2, 
					"DateLastModified" => 3, 
					"HasAfterTrigger" => 4, 
					"HasClusteredIndex" => 5, 
					"HasColumnSpecification" => 6, 
					"HasDeleteTrigger" => 7, 
					"HasIndex" => 8, 
					"HasInsertTrigger" => 9, 
					"HasInsteadOfTrigger" => 10, 
					"HasNonClusteredIndex" => 11, 
					"HasPrimaryClusteredIndex" => 12, 
					"HasUpdateTrigger" => 13, 
					"ID" => 14, 
					"IsEncrypted" => 15, 
					"IsIndexable" => 16, 
					"IsSchemaBound" => 17, 
					"IsSchemaOwned" => 18, 
					"IsSystemObject" => 19, 
					"Owner" => 20, 
					"QuotedIdentifierStatus" => 21, 
					"ReturnsViewMetadata" => 22, 
					"Text" => 23, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"AnsiNullsStatus" => 0, 
				"BodyStartIndex" => 1, 
				"CreateDate" => 2, 
				"HasClusteredIndex" => 3, 
				"HasColumnSpecification" => 4, 
				"HasNonClusteredIndex" => 5, 
				"HasPrimaryClusteredIndex" => 6, 
				"ID" => 7, 
				"IsEncrypted" => 8, 
				"IsSchemaBound" => 9, 
				"IsSystemObject" => 10, 
				"Owner" => 11, 
				"QuotedIdentifierStatus" => 12, 
				"Text" => 13, 
				"HasAfterTrigger" => 14, 
				"HasDeleteTrigger" => 15, 
				"HasIndex" => 16, 
				"HasInsertTrigger" => 17, 
				"HasInsteadOfTrigger" => 18, 
				"HasUpdateTrigger" => 19, 
				"IsIndexable" => 20, 
				"DateLastModified" => 21, 
				"IsSchemaOwned" => 22, 
				"ReturnsViewMetadata" => 23, 
				"PolicyHealthState" => 24, 
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

	private ViewEvents events;

	private ResumableIndexCollection m_ResumableIndexes;

	[SfcObject(SfcObjectRelationship.ParentObject, SfcObjectFlags.Design)]
	public Database Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as Database;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	internal override UserPermissionCollection Permissions => GetUserPermissions();

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool AnsiNullsStatus
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AnsiNullsStatus");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AnsiNullsStatus", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasAfterTrigger => (bool)base.Properties.GetValueWithNullReplacement("HasAfterTrigger");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasClusteredIndex => (bool)base.Properties.GetValueWithNullReplacement("HasClusteredIndex");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasColumnSpecification => (bool)base.Properties.GetValueWithNullReplacement("HasColumnSpecification");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasDeleteTrigger => (bool)base.Properties.GetValueWithNullReplacement("HasDeleteTrigger");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasIndex => (bool)base.Properties.GetValueWithNullReplacement("HasIndex");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasInsertTrigger => (bool)base.Properties.GetValueWithNullReplacement("HasInsertTrigger");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasInsteadOfTrigger => (bool)base.Properties.GetValueWithNullReplacement("HasInsteadOfTrigger");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasNonClusteredIndex => (bool)base.Properties.GetValueWithNullReplacement("HasNonClusteredIndex");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasPrimaryClusteredIndex => (bool)base.Properties.GetValueWithNullReplacement("HasPrimaryClusteredIndex");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasUpdateTrigger => (bool)base.Properties.GetValueWithNullReplacement("HasUpdateTrigger");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy, "false")]
	public bool IsEncrypted
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsEncrypted");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsEncrypted", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsIndexable => (bool)base.Properties.GetValueWithNullReplacement("IsIndexable");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsSchemaBound
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsSchemaBound");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsSchemaBound", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsSchemaOwned => (bool)base.Properties.GetValueWithNullReplacement("IsSchemaOwned");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcReference(typeof(ApplicationRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/ApplicationRole[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[CLSCompliant(false)]
	[SfcReference(typeof(DatabaseRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/Role[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool QuotedIdentifierStatus
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("QuotedIdentifierStatus");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("QuotedIdentifierStatus", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool ReturnsViewMetadata
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("ReturnsViewMetadata");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ReturnsViewMetadata", value);
		}
	}

	public ViewEvents Events
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
				events = new ViewEvents(this);
			}
			return events;
		}
	}

	public static string UrnSuffix => "View";

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(Index), SfcObjectFlags.Deploy)]
	public override IndexCollection Indexes => base.Indexes;

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(Index), SfcObjectFlags.Deploy)]
	public ResumableIndexCollection ResumableIndexes
	{
		get
		{
			CheckObjectState();
			if (m_ResumableIndexes == null)
			{
				m_ResumableIndexes = new ResumableIndexCollection(this);
			}
			return m_ResumableIndexes;
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string TextBody
	{
		get
		{
			CheckObjectState();
			return GetTextBody();
		}
		set
		{
			CheckObjectState();
			SetTextBody(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string TextHeader
	{
		get
		{
			CheckObjectState();
			return GetTextHeader(forAlter: false);
		}
		set
		{
			CheckObjectState();
			SetTextHeader(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool TextMode
	{
		get
		{
			CheckObjectState();
			return GetTextMode();
		}
		set
		{
			CheckObjectState();
			SetTextMode(value, new SmoCollectionBase[1] { base.Columns });
		}
	}

	public View()
	{
	}

	public View(Database database, string name)
	{
		ValidateName(name);
		key = new SchemaObjectKey(name, null);
		Parent = database;
	}

	public View(Database database, string name, string schema)
	{
		ValidateName(name);
		key = new SchemaObjectKey(name, schema);
		Parent = database;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		string text;
		if ((text = propname) != null && text == "IsEncrypted")
		{
			return false;
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

	public void Deny(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, columnNames, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, columnNames, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, columnNames, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, columnNames, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, columnNames, revokeGrant, cascade, asRole);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName, string[] columnNames, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, columnNames, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, string[] columnNames, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, columnNames, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, string[] columnNames, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, columnNames, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, string[] columnNames, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, columnNames, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, string[] columnNames, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, columnNames, revokeGrant, cascade, asRole);
	}

	public ObjectPermissionInfo[] EnumColumnPermissions()
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Column, this, null, null);
	}

	public ObjectPermissionInfo[] EnumColumnPermissions(string granteeName)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Column, this, granteeName, null);
	}

	public ObjectPermissionInfo[] EnumColumnPermissions(ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Column, this, null, permissions);
	}

	public ObjectPermissionInfo[] EnumColumnPermissions(string granteeName, ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Column, this, granteeName, permissions);
	}

	internal View(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void ChangeSchema(string newSchema)
	{
		CheckObjectState();
		ChangeSchema(newSchema, bCheckExisting: true);
	}

	public void Create()
	{
		CreateImpl();
		SetSchemaOwned();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		if (base.State != SqlSmoState.Creating && IsEncrypted)
		{
			SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.EncryptedViewsFunctionsDownlevel(FormatFullNameForScripting(sp, nameIsIndentifier: true), SqlSmoObject.GetSqlServerName(sp)));
			SqlSmoObject.ThrowIfCloud(sp.TargetDatabaseEngineType, ExceptionTemplatesImpl.EncryptedViewsFunctionsDownlevel(FormatFullNameForScripting(sp, nameIsIndentifier: true), SqlSmoObject.GetDatabaseEngineName(sp)));
		}
		if (sp.OldOptions.PrimaryObject)
		{
			GetDDL(queries, sp, ScriptHeaderType.ScriptHeaderForCreate);
			if (sp.IncludeScripts.Owner)
			{
				ScriptOwner(queries, sp);
			}
		}
	}

	public void CreateOrAlter()
	{
		CreateOrAlterImpl();
		SetSchemaOwned();
	}

	internal override void ScriptCreateOrAlter(StringCollection queries, ScriptingPreferences sp)
	{
		GetDDL(queries, sp, ScriptHeaderType.ScriptHeaderForCreateOrAlter);
		if (sp.IncludeScripts.Owner)
		{
			ScriptOwner(queries, sp);
		}
	}

	public void Alter()
	{
		AlterImpl();
		SetSchemaOwned();
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		if (IsObjectDirty())
		{
			if (ShouldScriptBodyAtAlter())
			{
				GetDDL(queries, sp, ScriptHeaderType.ScriptHeaderForAlter);
			}
			if (sp.IncludeScripts.Owner)
			{
				ScriptOwner(queries, sp);
			}
		}
	}

	protected override bool IsObjectDirty()
	{
		if (!base.IsObjectDirty())
		{
			return SqlSmoObject.IsCollectionDirty(base.Columns);
		}
		return true;
	}

	private string GetIfNotExistString(bool forCreate, ScriptingPreferences sp)
	{
		return string.Format(format: (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90) ? Scripts.INCLUDE_EXISTS_VIEW80 : Scripts.INCLUDE_EXISTS_VIEW90, provider: SmoApplication.DefaultCulture, args: new object[2]
		{
			forCreate ? "NOT" : "",
			SqlSmoObject.SqlString(FormatFullNameForScripting(sp))
		});
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
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = FormatFullNameForScripting(sp);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		bool flag = ((sp.TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase) ? (!sp.TargetEngineIsAzureSqlDw()) : (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130));
		if (sp.IncludeScripts.ExistenceCheck && !flag)
		{
			stringBuilder.AppendLine(GetIfNotExistString(forCreate: false, sp));
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP VIEW {0}{1}", new object[2]
		{
			(sp.IncludeScripts.ExistenceCheck && flag) ? "IF EXISTS " : string.Empty,
			text
		});
		queries.Add(stringBuilder.ToString());
	}

	public void Rename(string newName)
	{
		RenameImpl(newName);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		AddDatabaseContext(renameQuery, sp);
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_rename N'{0}', N'{1}', 'OBJECT'", new object[2]
		{
			SqlSmoObject.SqlString(FullQualifiedName),
			SqlSmoObject.SqlString(newName)
		}));
	}

	private void GetDDL(StringCollection queries, ScriptingPreferences sp, ScriptHeaderType scriptHeaderType)
	{
		bool flag = IsCreate(scriptHeaderType);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = FormatFullNameForScripting(sp);
		bool flag2 = false;
		bool flag3 = false;
		if (!sp.OldOptions.DdlHeaderOnly && !sp.OldOptions.DdlBodyOnly)
		{
			if (sp.IncludeScripts.Header)
			{
				stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
				stringBuilder.Append(sp.NewLine);
			}
			flag2 = null != base.Properties.Get("AnsiNullsStatus").Value;
			flag3 = null != base.Properties.Get("QuotedIdentifierStatus").Value;
			if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
			{
				Server server = (Server)base.ParentColl.ParentInstance.ParentColl.ParentInstance;
				_ = server.UserOptions.AnsiNulls;
				_ = server.UserOptions.QuotedIdentifier;
			}
			if (flag2)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.SET_ANSI_NULLS, new object[1] { ((bool)base.Properties["AnsiNullsStatus"].Value) ? Globals.On : Globals.Off });
				queries.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
			if (flag3)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.SET_QUOTED_IDENTIFIER, new object[1] { ((bool)base.Properties["QuotedIdentifierStatus"].Value) ? Globals.On : Globals.Off });
				queries.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
		}
		if (!TextMode || (sp.OldOptions.EnforceScriptingPreferences && sp.OldOptions.NoViewColumns))
		{
			StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			if (!sp.OldOptions.DdlBodyOnly)
			{
				if (flag && sp.IncludeScripts.ExistenceCheck)
				{
					stringBuilder.AppendLine(GetIfNotExistString(forCreate: true, sp));
					stringBuilder.AppendLine("EXECUTE dbo.sp_executesql N'");
				}
				switch (scriptHeaderType)
				{
				case ScriptHeaderType.ScriptHeaderForCreate:
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "{0} VIEW {1} ", new object[2]
					{
						Scripts.CREATE,
						text
					});
					break;
				case ScriptHeaderType.ScriptHeaderForAlter:
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "{0} VIEW {1} ", new object[2]
					{
						Scripts.ALTER,
						text
					});
					break;
				case ScriptHeaderType.ScriptHeaderForCreateOrAlter:
					SqlSmoObject.ThrowIfCreateOrAlterUnsupported(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.CreateOrAlterDownlevel("View", SqlSmoObject.GetSqlServerName(sp)));
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "{0} VIEW {1} ", new object[2]
					{
						Scripts.CREATE_OR_ALTER,
						text
					});
					break;
				default:
					throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration(scriptHeaderType.ToString()));
				}
				if (base.Columns.Count > 0 && !sp.OldOptions.NoViewColumns && ((flag && (GetPropValueOptional("HasColumnSpecification") == null || (bool)GetPropValueOptional("HasColumnSpecification"))) || (!flag && (bool)GetPropValueOptional("HasColumnSpecification"))))
				{
					StringBuilder stringBuilder3 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
					int num = 0;
					foreach (Column column in base.Columns)
					{
						if (sp.ScriptForCreateDrop || !column.IgnoreForScripting)
						{
							if (num++ > 0)
							{
								stringBuilder3.Append(Globals.comma);
								stringBuilder3.Append(sp.NewLine);
							}
							stringBuilder3.Append(Globals.tab);
							stringBuilder3.Append(column.FormatFullNameForScripting(sp));
						}
					}
					if (stringBuilder3.Length > 0)
					{
						stringBuilder2.AppendFormat("({0})", stringBuilder3.ToString());
						stringBuilder2.AppendLine();
					}
				}
				bool needsComma = false;
				if (base.ServerVersion.Major > 7 && sp.TargetServerVersionInternal > SqlServerVersionInternal.Version70)
				{
					AppendWithOption(stringBuilder2, "IsSchemaBound", "SCHEMABINDING", ref needsComma);
				}
				if (!SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType))
				{
					AppendWithOption(stringBuilder2, "IsEncrypted", "ENCRYPTION", ref needsComma);
				}
				if (base.ServerVersion.Major >= 9 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
				{
					AppendWithOption(stringBuilder2, "ReturnsViewMetadata", "VIEW_METADATA", ref needsComma);
				}
				if (needsComma)
				{
					stringBuilder2.AppendLine();
				}
				stringBuilder2.AppendLine(" AS ");
			}
			if (!sp.OldOptions.DdlHeaderOnly)
			{
				stringBuilder2.AppendLine(GetTextBody(forScripting: true));
			}
			if (flag && sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendLine(SqlSmoObject.SqlString(stringBuilder2.ToString()));
				stringBuilder.Append("'");
			}
			else
			{
				stringBuilder.Append(stringBuilder2.ToString());
			}
		}
		else
		{
			string textForScript = GetTextForScript(sp, new string[1] { "view" }, forceCheckNameAndManipulateIfRequired: true, scriptHeaderType);
			if (flag && sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendLine(GetIfNotExistString(forCreate: true, sp));
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_executesql @statement = {0} ", new object[1] { SqlSmoObject.MakeSqlString(textForScript) });
			}
			else
			{
				stringBuilder.Append(textForScript);
			}
		}
		queries.Add(stringBuilder.ToString());
		stringBuilder.Length = 0;
	}

	private bool ShouldScriptBodyAtAlter()
	{
		if (GetIsTextDirty())
		{
			return true;
		}
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add("AnsiNullsStatus");
		stringCollection.Add("QuotedIdentifierStatus");
		stringCollection.Add("IsSchemaBound");
		stringCollection.Add("IsEncrypted");
		if (base.ServerVersion.Major > 8)
		{
			stringCollection.Add("ReturnsViewMetadata");
		}
		if (base.Properties.ArePropertiesDirty(stringCollection))
		{
			return true;
		}
		if (SqlSmoObject.IsCollectionDirty(base.Columns))
		{
			return true;
		}
		return false;
	}

	public DataTable EnumColumns()
	{
		try
		{
			CheckObjectState();
			Request request = new Request(base.Urn.Value + "/Column");
			request.OrderByList = new OrderBy[1]
			{
				new OrderBy("Name", OrderBy.Direction.Asc)
			};
			return ExecutionManager.GetEnumeratorData(request);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumColumns, this, ex);
		}
	}

	public void Refresh(bool refreshViewMetadata)
	{
		Refresh();
		try
		{
			if (refreshViewMetadata)
			{
				Database database = (Database)base.ParentColl.ParentInstance;
				database.ExecuteNonQuery(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_refreshview @viewname=N'{0}'", new object[1] { SqlSmoObject.SqlString(FullQualifiedName) }));
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Refresh, this, ex);
		}
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		return GetPropagateInfoImpl(forDiscovery: false);
	}

	private PropagateInfo[] GetPropagateInfoImpl(bool forDiscovery)
	{
		ArrayList arrayList = new ArrayList();
		arrayList.Add(new PropagateInfo(base.Columns, bWithScript: false, bPropagateScriptToChildLevel: true));
		arrayList.Add(new PropagateInfo(base.Statistics, bWithScript: true, Statistic.UrnSuffix));
		if (forDiscovery)
		{
			new IndexPropagateInfo(Indexes).PropagateInfo(arrayList);
		}
		else
		{
			arrayList.Add(new PropagateInfo(Indexes, bWithScript: true, Index.UrnSuffix));
		}
		arrayList.Add(new PropagateInfo(base.Triggers, bWithScript: true, Trigger.UrnSuffix));
		if (DatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
		{
			arrayList.Add(new PropagateInfo(base.ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix));
		}
		if (DatabaseEngineType == DatabaseEngineType.Standalone && base.ServerVersion.Major >= 9)
		{
			arrayList.Add(new PropagateInfo(base.FullTextIndex, bWithScript: true, FullTextIndex.UrnSuffix));
		}
		PropagateInfo[] array = new PropagateInfo[arrayList.Count];
		arrayList.CopyTo(array, 0);
		return array;
	}

	internal override PropagateInfo[] GetPropagateInfoForDiscovery(PropagateAction action)
	{
		return GetPropagateInfoImpl(forDiscovery: true);
	}

	internal override void AddScriptPermission(StringCollection query, ScriptingPreferences sp)
	{
		if (sp.TargetServerVersionInternal != SqlServerVersionInternal.Version70 && base.ServerVersion.Major != 7)
		{
			base.AddScriptPermission(query, sp);
		}
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		if (!defaultTextMode)
		{
			string[] fields = new string[10] { "QuotedIdentifierStatus", "IsSystemObject", "AnsiNullsStatus", "IsSchemaBound", "IsEncrypted", "HasColumnSpecification", "ReturnsViewMetadata", "ID", "Owner", "IsSchemaOwned" };
			List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
			supportedScriptFields.Add("Text");
			return supportedScriptFields.ToArray();
		}
		string[] fields2 = new string[4] { "QuotedIdentifierStatus", "IsSystemObject", "AnsiNullsStatus", "ID" };
		List<string> supportedScriptFields2 = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields2, version, databaseEngineType, databaseEngineEdition);
		supportedScriptFields2.Add("Text");
		return supportedScriptFields2.ToArray();
	}

	public string ScriptHeader(bool forAlter)
	{
		CheckObjectState();
		return GetTextHeader(forAlter);
	}

	public string ScriptHeader(ScriptHeaderType scriptHeaderType)
	{
		CheckObjectState();
		return GetTextHeader(scriptHeaderType);
	}

	internal override void ValidateProperty(Property prop, object value)
	{
		switch (base.ServerVersion.Major)
		{
		case 7:
			switch (prop.Name)
			{
			case "IsEncrypted":
			case "HasColumnSpecification":
				ScriptNameObjectBase.Validate_set_TextObjectDDLProperty(prop, value);
				break;
			}
			break;
		default:
			switch (prop.Name)
			{
			case "IsSchemaBound":
			case "IsEncrypted":
			case "HasColumnSpecification":
				ScriptNameObjectBase.Validate_set_TextObjectDDLProperty(prop, value);
				break;
			}
			break;
		}
	}
}
