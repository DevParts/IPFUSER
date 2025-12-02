using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class AlertSystem : AgentObjectBase, IAlterable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 12, 12, 12, 12, 12, 12, 12, 12, 12, 12 };

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
				"FailSafeEmailAddress" => 0, 
				"FailSafeNetSendAddress" => 1, 
				"FailSafeOperator" => 2, 
				"FailSafePagerAddress" => 3, 
				"ForwardingServer" => 4, 
				"ForwardingSeverity" => 5, 
				"IsForwardedAlways" => 6, 
				"NotificationMethod" => 7, 
				"PagerCCTemplate" => 8, 
				"PagerSendSubjectOnly" => 9, 
				"PagerSubjectTemplate" => 10, 
				"PagerToTemplate" => 11, 
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
			staticMetadata = new StaticMetadata[12]
			{
				new StaticMetadata("FailSafeEmailAddress", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("FailSafeNetSendAddress", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("FailSafeOperator", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("FailSafePagerAddress", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ForwardingServer", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ForwardingSeverity", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("IsForwardedAlways", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("NotificationMethod", expensive: false, readOnly: false, typeof(NotifyMethods)),
				new StaticMetadata("PagerCCTemplate", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("PagerSendSubjectOnly", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("PagerSubjectTemplate", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("PagerToTemplate", expensive: false, readOnly: false, typeof(string))
			};
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string FailSafeEmailAddress
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FailSafeEmailAddress");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FailSafeEmailAddress", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string FailSafeNetSendAddress
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FailSafeNetSendAddress");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FailSafeNetSendAddress", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string FailSafeOperator
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FailSafeOperator");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FailSafeOperator", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string FailSafePagerAddress
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FailSafePagerAddress");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FailSafePagerAddress", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ForwardingServer
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ForwardingServer");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ForwardingServer", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ForwardingSeverity
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("ForwardingSeverity");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ForwardingSeverity", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsForwardedAlways
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsForwardedAlways");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsForwardedAlways", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public NotifyMethods NotificationMethod
	{
		get
		{
			return (NotifyMethods)base.Properties.GetValueWithNullReplacement("NotificationMethod");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NotificationMethod", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string PagerCCTemplate
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("PagerCCTemplate");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PagerCCTemplate", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool PagerSendSubjectOnly
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("PagerSendSubjectOnly");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PagerSendSubjectOnly", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string PagerSubjectTemplate
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("PagerSubjectTemplate");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PagerSubjectTemplate", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string PagerToTemplate
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("PagerToTemplate");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PagerToTemplate", value);
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public JobServer Parent
	{
		get
		{
			CheckObjectState();
			return singletonParent as JobServer;
		}
	}

	public static string UrnSuffix => "AlertSystem";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal AlertSystem(JobServer parentsrv, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentsrv;
		SetServerObject(parentsrv.GetServerObject());
		m_comparer = parentsrv.Parent.Databases["msdb"].StringComparer;
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}

	protected internal override string GetDBName()
	{
		return "msdb";
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		ScriptProperties(queries, sp);
	}

	private void ScriptProperties(StringCollection queries, ScriptingPreferences sp)
	{
		Initialize(allProperties: true);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_MSsetalertinfo ");
		int count = 0;
		GetStringParameter(stringBuilder, sp, "FailSafeOperator", "@failsafeoperator=N'{0}'", ref count);
		GetEnumParameter(stringBuilder, sp, "NotificationMethod", "@notificationmethod={0}", typeof(NotifyMethods), ref count);
		GetStringParameter(stringBuilder, sp, "ForwardingServer", "@forwardingserver=N'{0}'", ref count);
		GetStringParameter(stringBuilder, sp, "PagerToTemplate", "@pagertotemplate=N'{0}'", ref count);
		GetStringParameter(stringBuilder, sp, "PagerCCTemplate", "@pagercctemplate=N'{0}'", ref count);
		GetStringParameter(stringBuilder, sp, "PagerSubjectTemplate", "@pagersubjecttemplate=N'{0}'", ref count);
		GetBoolParameter(stringBuilder, sp, "IsForwardedAlways", "@forwardalways={0}", ref count);
		GetBoolParameter(stringBuilder, sp, "PagerSendSubjectOnly", "@pagersendsubjectonly={0}", ref count);
		GetParameter(stringBuilder, sp, "ForwardingSeverity", "@forwardingseverity={0}", ref count);
		GetStringParameter(stringBuilder, sp, "FailSafeEmailAddress", "@failsafeemailaddress=N'{0}'", ref count);
		GetStringParameter(stringBuilder, sp, "FailSafePagerAddress", "@failsafepageraddress=N'{0}'", ref count);
		GetStringParameter(stringBuilder, sp, "FailSafeNetSendAddress", "@failsafenetsendaddress=N'{0}'", ref count);
		if (count > 0)
		{
			queries.Add(stringBuilder.ToString());
		}
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		ScriptProperties(queries, sp);
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
