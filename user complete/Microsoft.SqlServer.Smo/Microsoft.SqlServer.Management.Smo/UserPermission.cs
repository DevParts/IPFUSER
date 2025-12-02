using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElementType("Permission")]
internal sealed class UserPermission : NamedSmoObject
{
	private class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 7, 7, 7, 7, 7, 7, 7, 7, 7, 7 };

		private static int[] cloudVersionCount = new int[3] { 7, 7, 7 };

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[7]
		{
			new StaticMetadata("Code", expensive: false, readOnly: false, typeof(ObjectPermissionSetValue)),
			new StaticMetadata("Grantee", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("GranteeType", expensive: false, readOnly: false, typeof(PrincipalType)),
			new StaticMetadata("Grantor", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("GrantorType", expensive: false, readOnly: false, typeof(PrincipalType)),
			new StaticMetadata("ObjectClass", expensive: false, readOnly: false, typeof(ObjectClass)),
			new StaticMetadata("PermissionState", expensive: false, readOnly: false, typeof(PermissionState))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[7]
		{
			new StaticMetadata("Code", expensive: false, readOnly: false, typeof(ObjectPermissionSetValue)),
			new StaticMetadata("Grantee", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("GranteeType", expensive: false, readOnly: false, typeof(PrincipalType)),
			new StaticMetadata("Grantor", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("GrantorType", expensive: false, readOnly: false, typeof(PrincipalType)),
			new StaticMetadata("ObjectClass", expensive: false, readOnly: false, typeof(ObjectClass)),
			new StaticMetadata("PermissionState", expensive: false, readOnly: false, typeof(PermissionState))
		};

		public override int Count
		{
			get
			{
				if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
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
				return propertyName switch
				{
					"Code" => 0, 
					"Grantee" => 1, 
					"GranteeType" => 2, 
					"Grantor" => 3, 
					"GrantorType" => 4, 
					"ObjectClass" => 5, 
					"PermissionState" => 6, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"Code" => 0, 
				"Grantee" => 1, 
				"GranteeType" => 2, 
				"Grantor" => 3, 
				"GrantorType" => 4, 
				"ObjectClass" => 5, 
				"PermissionState" => 6, 
				_ => -1, 
			};
		}

		public override StaticMetadata GetStaticMetadata(int id)
		{
			if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				return cloudStaticMetadata[id];
			}
			return staticMetadata[id];
		}

		internal new static int[] GetVersionArray(DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
		{
			if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				return cloudVersionCount;
			}
			return versionCount;
		}

		internal new static StaticMetadata[] GetStaticMetadataArray(DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
		{
			if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				return cloudStaticMetadata;
			}
			return staticMetadata;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public ObjectPermissionSetValue Code
	{
		get
		{
			return (ObjectPermissionSetValue)base.Properties.GetValueWithNullReplacement("Code");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Code", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Grantee
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Grantee");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Grantee", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public PrincipalType GranteeType
	{
		get
		{
			return (PrincipalType)base.Properties.GetValueWithNullReplacement("GranteeType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("GranteeType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Grantor
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Grantor");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Grantor", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public PrincipalType GrantorType
	{
		get
		{
			return (PrincipalType)base.Properties.GetValueWithNullReplacement("GrantorType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("GrantorType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int IntCode
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("IntCode");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IntCode", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public ObjectClass ObjectClass
	{
		get
		{
			return (ObjectClass)base.Properties.GetValueWithNullReplacement("ObjectClass");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ObjectClass", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public PermissionState PermissionState
	{
		get
		{
			return (PermissionState)base.Properties.GetValueWithNullReplacement("PermissionState");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PermissionState", value);
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	internal SqlSmoObject Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	public static string UrnSuffix => "Permission";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal UserPermission(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	internal UserPermission()
	{
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[7] { "Code", "Grantee", "GranteeType", "Grantor", "GrantorType", "ObjectClass", "PermissionState" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}

	internal override string[] GetRejectFields()
	{
		return new string[2] { "Urn", "ColumnName" };
	}
}
