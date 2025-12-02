using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class TargetServerGroup : AgentObjectBase, ICreatable, IDroppable, IDropIfExists, IRenamable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

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
			string text;
			if ((text = propertyName) != null && text == "ID")
			{
				return 0;
			}
			return -1;
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
			staticMetadata = new StaticMetadata[1]
			{
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int))
			};
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public JobServer Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as JobServer;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	public static string UrnSuffix => "TargetServerGroup";

	public TargetServerGroup()
	{
	}

	public TargetServerGroup(JobServer jobServer, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = jobServer;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal TargetServerGroup(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AGENT_TARGETSERVERGROUP, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_add_targetservergroup @name=N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
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

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AGENT_TARGETSERVERGROUP, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_targetservergroup @name=N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
		queries.Add(stringBuilder.ToString());
	}

	public void Rename(string newName)
	{
		RenameImpl(newName);
	}

	internal override void ScriptRename(StringCollection queries, ScriptingPreferences sp, string newName)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AGENT_TARGETSERVERGROUP, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_targetservergroup @name=N'{0}', @new_name=N'{1}'", new object[2]
		{
			SqlSmoObject.SqlString(Name),
			SqlSmoObject.SqlString(newName)
		});
		queries.Add(stringBuilder.ToString());
	}

	public void AddMemberServer(string srvname)
	{
		try
		{
			if (srvname == null)
			{
				throw new ArgumentNullException("srvname");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_add_targetsvrgrp_member @group_name=N'{0}', @server_name=N'{1}'", new object[2]
			{
				SqlSmoObject.SqlString(Name),
				SqlSmoObject.SqlString(srvname)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddMemberServer, this, ex);
		}
	}

	public TargetServer[] EnumMemberServers()
	{
		try
		{
			Request req = new Request(string.Concat(base.Urn, "/Member"), new string[1] { "Urn" });
			DataTable enumeratorData = ExecutionManager.GetEnumeratorData(req);
			TargetServer[] array = new TargetServer[enumeratorData.Rows.Count];
			int num = 0;
			foreach (DataRow row in enumeratorData.Rows)
			{
				array[num++] = (TargetServer)GetServerObject().GetSmoObject((string)row["Urn"]);
			}
			return array;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumMemberServers, this, ex);
		}
	}

	public void RemoveMemberServer(string srvname)
	{
		try
		{
			if (srvname == null)
			{
				throw new ArgumentNullException("srvname");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_targetsvrgrp_member @group_name=N'{0}', @server_name=N'{1}'", new object[2]
			{
				SqlSmoObject.SqlString(Name),
				SqlSmoObject.SqlString(srvname)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveMemberServer, this, ex);
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
}
