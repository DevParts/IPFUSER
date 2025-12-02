using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone)]
[SfcElementType("Setting")]
public sealed class Settings : SqlSmoObject, IAlterable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 9, 9, 9, 9, 9, 10, 10, 10, 10, 10 };

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
				"AuditLevel" => 0, 
				"BackupDirectory" => 1, 
				"DefaultFile" => 2, 
				"DefaultLog" => 3, 
				"LoginMode" => 4, 
				"MailProfile" => 5, 
				"NumberOfLogFiles" => 6, 
				"PerfMonMode" => 7, 
				"TapeLoadWaitTime" => 8, 
				"ErrorLogSizeKb" => 9, 
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
				new StaticMetadata("AuditLevel", expensive: false, readOnly: false, typeof(AuditLevel)),
				new StaticMetadata("BackupDirectory", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("DefaultFile", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("DefaultLog", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("LoginMode", expensive: false, readOnly: false, typeof(ServerLoginMode)),
				new StaticMetadata("MailProfile", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("NumberOfLogFiles", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("PerfMonMode", expensive: false, readOnly: false, typeof(PerfMonMode)),
				new StaticMetadata("TapeLoadWaitTime", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("ErrorLogSizeKb", expensive: false, readOnly: false, typeof(int))
			};
		}
	}

	private OleDbProviderSettingsCollection m_OleDbProviderSettings;

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AuditLevel AuditLevel
	{
		get
		{
			return (AuditLevel)base.Properties.GetValueWithNullReplacement("AuditLevel");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AuditLevel", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string BackupDirectory
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("BackupDirectory");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BackupDirectory", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string DefaultFile
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DefaultFile");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DefaultFile", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string DefaultLog
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DefaultLog");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DefaultLog", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ErrorLogSizeKb
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("ErrorLogSizeKb");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ErrorLogSizeKb", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public ServerLoginMode LoginMode
	{
		get
		{
			return (ServerLoginMode)base.Properties.GetValueWithNullReplacement("LoginMode");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("LoginMode", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string MailProfile
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("MailProfile");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MailProfile", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int NumberOfLogFiles
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("NumberOfLogFiles");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NumberOfLogFiles", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public PerfMonMode PerfMonMode
	{
		get
		{
			return (PerfMonMode)base.Properties.GetValueWithNullReplacement("PerfMonMode");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PerfMonMode", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int TapeLoadWaitTime
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("TapeLoadWaitTime");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("TapeLoadWaitTime", value);
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Server Parent
	{
		get
		{
			CheckObjectState();
			return singletonParent as Server;
		}
	}

	public static string UrnSuffix => "Setting";

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(OleDbProviderSettings))]
	public OleDbProviderSettingsCollection OleDbProviderSettings
	{
		get
		{
			CheckObjectState();
			if (m_OleDbProviderSettings == null)
			{
				m_OleDbProviderSettings = new OleDbProviderSettingsCollection(this);
			}
			return m_OleDbProviderSettings;
		}
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal Settings(Server parentsrv, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentsrv;
		SetServerObject(parentsrv.GetServerObject());
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		ScriptProperties(query, sp);
	}

	internal override void ScriptCreate(StringCollection query, ScriptingPreferences sp)
	{
		if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
		{
			ScriptProperties(query, sp);
		}
	}

	private void ScriptProperties(StringCollection query, ScriptingPreferences sp)
	{
		Initialize(allProperties: true);
		new StringBuilder();
		object obj = null;
		foreach (string[] item in Server.RegistryProperties(DatabaseEngineType))
		{
			if (item[0].Length == 0)
			{
				break;
			}
			if (!IsSupportedProperty(item[0], sp))
			{
				continue;
			}
			Property property = base.Properties.Get(item[0]);
			if (property.Value == null || (sp.ScriptForAlter && !property.Dirty))
			{
				continue;
			}
			obj = property.Value;
			if ((item[0] == "NumberOfLogFiles" && (int)obj < 6) || (obj is string && ((string)obj).Length == 0))
			{
				ScriptDeleteRegSetting(query, item);
				continue;
			}
			if (item[0] == "LoginMode")
			{
				ServerLoginMode serverLoginMode = (ServerLoginMode)obj;
				if (serverLoginMode != ServerLoginMode.Integrated && serverLoginMode != ServerLoginMode.Normal && serverLoginMode != ServerLoginMode.Mixed)
				{
					throw new SmoException(ExceptionTemplatesImpl.UnsupportedLoginMode(serverLoginMode.ToString()));
				}
				ScriptRegSetting(query, item, Enum.Format(typeof(ServerLoginMode), (ServerLoginMode)obj, "d"));
				continue;
			}
			if (item[0] == "AuditLevel")
			{
				AuditLevel auditLevel = (AuditLevel)obj;
				if (AuditLevel.None > auditLevel || AuditLevel.All < auditLevel)
				{
					throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration(typeof(AuditLevel).Name));
				}
				ScriptRegSetting(query, item, Enum.Format(typeof(AuditLevel), auditLevel, "d"));
				continue;
			}
			if (item[0] == "PerfMonMode")
			{
				PerfMonMode perfMonMode = (PerfMonMode)obj;
				switch (perfMonMode)
				{
				case PerfMonMode.Continuous:
				case PerfMonMode.OnDemand:
					ScriptRegSetting(query, item, Enum.Format(typeof(PerfMonMode), (PerfMonMode)obj, "d"));
					continue;
				case PerfMonMode.None:
					if (!sp.ForDirectExecution)
					{
						continue;
					}
					break;
				}
				throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration(perfMonMode.GetType().Name));
			}
			if (item[0] == "DefaultFile" || item[0] == "DefaultLog" || item[0] == "BackupDirectory")
			{
				string text = (string)obj;
				if (text.Length == 0)
				{
					ScriptDeleteRegSetting(query, item);
					continue;
				}
				if (text[text.Length - 1] == '\\')
				{
					text = text.Remove(text.Length - 1, 1);
				}
				ScriptRegSetting(query, item, text);
			}
			else
			{
				ScriptRegSetting(query, item, obj);
			}
		}
	}

	private void ScriptRegSetting(StringCollection query, string[] prop, object oValue)
	{
		string format = Scripts.REG_WRITE_WRITE_PROP;
		if (base.ServerVersion.Major <= 7)
		{
			format = Scripts.REG_WRITE_WRITE_PROP70;
		}
		if ("REG_SZ" == prop[2])
		{
			query.Add(string.Format(SmoApplication.DefaultCulture, format, new object[3]
			{
				prop[1],
				prop[2],
				"N'" + SqlSmoObject.SqlString(oValue.ToString())
			}) + "'");
		}
		else if (oValue is bool)
		{
			query.Add(string.Format(SmoApplication.DefaultCulture, format, new object[3]
			{
				prop[1],
				prop[2],
				((bool)oValue) ? 1 : 0
			}));
		}
		else
		{
			query.Add(string.Format(SmoApplication.DefaultCulture, format, new object[3]
			{
				prop[1],
				prop[2],
				oValue.ToString()
			}));
		}
	}

	private void ScriptDeleteRegSetting(StringCollection query, string[] prop)
	{
		string format = Scripts.REG_DELETE;
		if (base.ServerVersion.Major <= 7)
		{
			format = Scripts.REG_DELETE70;
		}
		query.Add(string.Format(SmoApplication.DefaultCulture, format, new object[1] { prop[1] }));
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}
}
