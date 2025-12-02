using System;
using System.Collections.Specialized;
using System.Security;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Management.Smo.Internal;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone)]
[SfcElementType("Login")]
public sealed class LinkedServerLogin : NamedSmoObject, ISfcSupportsDesignMode, ICreatable, IDroppable, IDropIfExists, IAlterable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 2, 2, 3, 3, 3, 3, 3, 3, 3, 3 };

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
				"Impersonate" => 0, 
				"RemoteUser" => 1, 
				"DateLastModified" => 2, 
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
			staticMetadata = new StaticMetadata[3]
			{
				new StaticMetadata("Impersonate", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("RemoteUser", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime))
			};
		}
	}

	private SqlSecureString remoteUserPassword;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public LinkedServer Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as LinkedServer;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool Impersonate
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Impersonate");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Impersonate", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string RemoteUser
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("RemoteUser");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RemoteUser", value);
		}
	}

	public static string UrnSuffix => "Login";

	[SfcReference(typeof(Login), "Server[@Name = '{0}']/Login[@Name = '{1}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Name" })]
	[CLSCompliant(false)]
	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public override string Name
	{
		get
		{
			return base.Name;
		}
		set
		{
			base.Name = value;
		}
	}

	public LinkedServerLogin()
	{
	}

	public LinkedServerLogin(LinkedServer linkedServer, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = linkedServer;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal LinkedServerLogin(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection query, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder();
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		ScriptIncludeIfNotExists(stringBuilder, sp, "NOT");
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_addlinkedsrvlogin @rmtsrvname = N'{0}'", new object[1] { SqlSmoObject.SqlString(base.ParentColl.ParentInstance.InternalName) });
		stringBuilder.Append(", @locallogin = ");
		if (Name.Length == 0)
		{
			stringBuilder.Append("NULL ");
		}
		else
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
		}
		Property property = base.Properties.Get("Impersonate");
		if (property.Value != null && (property.Dirty || sp.ScriptForCreateDrop))
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", @useself = N'{0}'", new object[1] { SqlSmoObject.SqlString(property.Value.ToString()) });
		}
		property = base.Properties.Get("RemoteUser");
		if (property.Value != null && (property.Dirty || sp.ScriptForCreateDrop))
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", @rmtuser = N'{0}'", new object[1] { SqlSmoObject.SqlString(property.Value.ToString()) });
			if (null != remoteUserPassword)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", @rmtpassword = N'{0}'", new object[1] { SqlSmoObject.SqlString((string)remoteUserPassword) });
			}
		}
		query.Add(stringBuilder.ToString());
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
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		ScriptIncludeIfNotExists(stringBuilder, sp, string.Empty);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_droplinkedsrvlogin @rmtsrvname = N'{0}'", new object[1] { SqlSmoObject.SqlString(base.ParentColl.ParentInstance.InternalName) });
		stringBuilder.Append(", @locallogin = ");
		if (Name.Length == 0)
		{
			stringBuilder.Append("NULL ");
		}
		else
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
		}
		dropQuery.Add(stringBuilder.ToString());
	}

	private void ScriptIncludeIfNotExists(StringBuilder sb, ScriptingPreferences sp, string predicate)
	{
		if (sp.IncludeScripts.ExistenceCheck)
		{
			if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_LINKED_SERVER_LOGIN90, new object[2]
				{
					predicate,
					FormatFullNameForScripting(sp, nameIsIndentifier: false)
				});
			}
			else
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_LINKED_SERVER_LOGIN80, new object[2]
				{
					predicate,
					FormatFullNameForScripting(sp, nameIsIndentifier: false)
				});
			}
		}
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		ScriptCreate(alterQuery, sp);
	}

	public void SetRemotePassword(string password)
	{
		remoteUserPassword = ((password != null) ? new SqlSecureString(password) : null);
		base.Properties.Get("Impersonate").SetDirty(dirty: true);
		base.Properties.Get("RemoteUser").SetDirty(dirty: true);
	}

	public void SetRemotePassword(SecureString password)
	{
		remoteUserPassword = password;
		base.Properties.Get("Impersonate").SetDirty(dirty: true);
		base.Properties.Get("RemoteUser").SetDirty(dirty: true);
	}
}
