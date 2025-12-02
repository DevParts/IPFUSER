using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[EvaluationMode(/*Could not decode attribute arguments.*/)]
[SfcElement(SfcElementFlags.Standalone)]
[PhysicalFacet]
public sealed class BackupDevice : ScriptNameObjectBase, ICreatable, IDroppable, IDropIfExists, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 3, 3, 3, 4, 4, 4, 4, 4, 4, 4 };

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
				"BackupDeviceType" => 0, 
				"PhysicalLocation" => 1, 
				"SkipTapeLabel" => 2, 
				"PolicyHealthState" => 3, 
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
				new StaticMetadata("BackupDeviceType", expensive: false, readOnly: false, typeof(BackupDeviceType)),
				new StaticMetadata("PhysicalLocation", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("SkipTapeLabel", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState))
			};
		}
	}

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

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public BackupDeviceType BackupDeviceType
	{
		get
		{
			return (BackupDeviceType)base.Properties.GetValueWithNullReplacement("BackupDeviceType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BackupDeviceType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public string PhysicalLocation
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("PhysicalLocation");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PhysicalLocation", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public bool SkipTapeLabel
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("SkipTapeLabel");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SkipTapeLabel", value);
		}
	}

	public static string UrnSuffix => "BackupDevice";

	public BackupDevice()
	{
	}

	public BackupDevice(Server server, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = server;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[3] { "BackupDeviceType", "PhysicalLocation", "SkipTapeLabel" };
	}

	internal BackupDevice(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		InitializeKeepDirtyValues();
		Property property = base.Properties.Get("BackupDeviceType");
		if (property.Value == null)
		{
			throw new PropertyNotSetException("BackupDeviceType");
		}
		Property property2 = base.Properties.Get("PhysicalLocation");
		if (property2.Value == null)
		{
			throw new PropertyNotSetException("PhysicalLocation");
		}
		bool flag = false;
		string empty = string.Empty;
		switch ((BackupDeviceType)property.Value)
		{
		case BackupDeviceType.Disk:
			empty = "disk";
			break;
		case BackupDeviceType.FloppyA:
		case BackupDeviceType.FloppyB:
			empty = "disk";
			break;
		case BackupDeviceType.Tape:
		{
			empty = "tape";
			Property property3 = base.Properties.Get("SkipTapeLabel");
			if (property3.Value != null)
			{
				flag = (bool)property3.Value;
			}
			break;
		}
		case BackupDeviceType.Pipe:
			if (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version90)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.BackupToPipesNotSupported(sp.TargetServerVersionInternal.ToString()));
			}
			empty = "pipe";
			break;
		default:
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnsupportedBackupDeviceType(((BackupDeviceType)property.Value).ToString()));
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = FormatFullNameForScripting(sp, nameIsIndentifier: false);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_BACKUP_DEVICE, new object[2] { "NOT", text });
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_addumpdevice  @devtype = N'{0}', @logicalname = {1}, @physicalname = N'{2}'", new object[3]
		{
			empty,
			text,
			SqlSmoObject.SqlString((string)property2.Value)
		});
		if (flag)
		{
			stringBuilder.Append(", @devstatus = N'skip'");
		}
		queries.Add(stringBuilder.ToString());
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
		string text = FormatFullNameForScripting(sp, nameIsIndentifier: false);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_BACKUP_DEVICE, new object[2] { "", text });
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_dropdevice @logicalname = {0}", new object[1] { text });
		dropQuery.Add(stringBuilder.ToString());
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public DataTable ReadBackupHeader()
	{
		try
		{
			return ExecutionManager.ExecuteWithResults("RESTORE HEADERONLY FROM " + FormatFullNameForScripting(new ScriptingPreferences()) + " WITH NOUNLOAD").Tables[0];
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ReadBackupHeader, this, ex);
		}
	}

	public DataTable ReadMediaHeader()
	{
		try
		{
			return ExecutionManager.ExecuteWithResults("RESTORE LABELONLY FROM " + FormatFullNameForScripting(new ScriptingPreferences()) + " WITH NOUNLOAD").Tables[0];
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ReadMediaHeader, this, ex);
		}
	}
}
