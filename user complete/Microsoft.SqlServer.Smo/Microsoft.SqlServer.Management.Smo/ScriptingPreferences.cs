using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;

namespace Microsoft.SqlServer.Management.Smo;

public class ScriptingPreferences
{
	private SqlServerVersionInternal m_eTargetServerVersion;

	private DatabaseEngineType m_eTargetDatabaseEngineType;

	private DatabaseEngineEdition m_eTargetDatabaseEngineEdition;

	private AgentPreferences agentPreferences;

	private MailPreferences mailPreferences;

	public ScriptBehavior Behavior { get; set; }

	public bool ScriptForAlter { get; set; }

	internal bool ContinueOnScriptingError { get; set; }

	internal bool SystemObjects { get; set; }

	internal bool IgnoreDependencyError { get; set; }

	internal bool DependentObjects { get; set; }

	internal bool SfcChildren { get; set; }

	internal string NewLine { get; set; }

	internal bool ScriptForCreateDrop { get; set; }

	internal bool ForDirectExecution { get; set; }

	internal bool SuppressDirtyCheck { get; set; }

	internal bool VersionDirty { get; private set; }

	internal bool DatabaseEngineTypeDirty { get; private set; }

	internal bool DatabaseEngineEditionDirty { get; private set; }

	internal bool TargetVersionAndDatabaseEngineTypeDirty
	{
		get
		{
			if (VersionDirty)
			{
				return DatabaseEngineTypeDirty;
			}
			return false;
		}
	}

	internal SqlServerVersionInternal TargetServerVersionInternal
	{
		get
		{
			return m_eTargetServerVersion;
		}
		set
		{
			m_eTargetServerVersion = value;
			VersionDirty = true;
		}
	}

	internal SqlServerVersion TargetServerVersion
	{
		get
		{
			switch (m_eTargetServerVersion)
			{
			case SqlServerVersionInternal.Version70:
			case SqlServerVersionInternal.Version80:
				return SqlServerVersion.Version80;
			case SqlServerVersionInternal.Version90:
				return SqlServerVersion.Version90;
			case SqlServerVersionInternal.Version100:
				return SqlServerVersion.Version100;
			case SqlServerVersionInternal.Version105:
				return SqlServerVersion.Version105;
			case SqlServerVersionInternal.Version110:
				return SqlServerVersion.Version110;
			case SqlServerVersionInternal.Version120:
				return SqlServerVersion.Version120;
			case SqlServerVersionInternal.Version130:
				return SqlServerVersion.Version130;
			case SqlServerVersionInternal.Version140:
				return SqlServerVersion.Version140;
			case SqlServerVersionInternal.Version150:
				return SqlServerVersion.Version150;
			default:
				TraceHelper.Assert(condition: false, "unexpected server version");
				return SqlServerVersion.Version150;
			}
		}
		set
		{
			switch (value)
			{
			case SqlServerVersion.Version80:
				m_eTargetServerVersion = SqlServerVersionInternal.Version80;
				break;
			case SqlServerVersion.Version90:
				m_eTargetServerVersion = SqlServerVersionInternal.Version90;
				break;
			case SqlServerVersion.Version100:
				m_eTargetServerVersion = SqlServerVersionInternal.Version100;
				break;
			case SqlServerVersion.Version105:
				m_eTargetServerVersion = SqlServerVersionInternal.Version105;
				break;
			case SqlServerVersion.Version110:
				m_eTargetServerVersion = SqlServerVersionInternal.Version110;
				break;
			case SqlServerVersion.Version120:
				m_eTargetServerVersion = SqlServerVersionInternal.Version120;
				break;
			case SqlServerVersion.Version130:
				m_eTargetServerVersion = SqlServerVersionInternal.Version130;
				break;
			case SqlServerVersion.Version140:
				m_eTargetServerVersion = SqlServerVersionInternal.Version140;
				break;
			case SqlServerVersion.Version150:
				m_eTargetServerVersion = SqlServerVersionInternal.Version150;
				break;
			default:
				TraceHelper.Assert(condition: false, "unexpected server version");
				m_eTargetServerVersion = SqlServerVersionInternal.Version150;
				break;
			}
			VersionDirty = true;
		}
	}

	internal DatabaseEngineType TargetDatabaseEngineType
	{
		get
		{
			return m_eTargetDatabaseEngineType;
		}
		set
		{
			m_eTargetDatabaseEngineType = value;
			DatabaseEngineTypeDirty = true;
		}
	}

	internal DatabaseEngineEdition TargetDatabaseEngineEdition
	{
		get
		{
			return m_eTargetDatabaseEngineEdition;
		}
		set
		{
			m_eTargetDatabaseEngineEdition = value;
			DatabaseEngineEditionDirty = true;
		}
	}

	internal IncludeScriptPreferences IncludeScripts { get; private set; }

	internal SecurityPreferences Security { get; private set; }

	internal StoragePreferences Storage { get; private set; }

	internal TablePreferences Table { get; private set; }

	internal DataTypePreferences DataType { get; private set; }

	internal DataPreferences Data { get; private set; }

	internal OldScriptingOptions OldOptions { get; private set; }

	internal AgentPreferences Agent
	{
		get
		{
			if (agentPreferences == null)
			{
				agentPreferences = new AgentPreferences();
			}
			return agentPreferences;
		}
	}

	internal MailPreferences Mail
	{
		get
		{
			if (mailPreferences == null)
			{
				mailPreferences = new MailPreferences();
			}
			return mailPreferences;
		}
	}

	internal void SetTargetServerVersion(ServerVersion ver)
	{
		VersionDirty = true;
		switch (ver.Major)
		{
		case 8:
			m_eTargetServerVersion = SqlServerVersionInternal.Version80;
			break;
		case 9:
			m_eTargetServerVersion = SqlServerVersionInternal.Version90;
			break;
		case 10:
			if (ver.Minor == 0)
			{
				m_eTargetServerVersion = SqlServerVersionInternal.Version100;
				break;
			}
			TraceHelper.Assert(ver.Minor == 50, "unexpected server version");
			m_eTargetServerVersion = SqlServerVersionInternal.Version105;
			break;
		case 11:
			m_eTargetServerVersion = SqlServerVersionInternal.Version110;
			break;
		case 12:
			m_eTargetServerVersion = SqlServerVersionInternal.Version120;
			break;
		case 13:
			m_eTargetServerVersion = SqlServerVersionInternal.Version130;
			break;
		case 14:
			m_eTargetServerVersion = SqlServerVersionInternal.Version140;
			break;
		case 15:
			m_eTargetServerVersion = SqlServerVersionInternal.Version150;
			break;
		default:
			TraceHelper.Assert(condition: false, "unexpected server version");
			break;
		}
	}

	internal void SetTargetDatabaseEngineType(DatabaseEngineType databaseEngineType)
	{
		TargetDatabaseEngineType = databaseEngineType;
	}

	internal void SetTargetDatabaseEngineEdition(DatabaseEngineEdition databaseEngineEdition)
	{
		TargetDatabaseEngineEdition = databaseEngineEdition;
	}

	internal void SetTargetServerVersion(SqlSmoObject o)
	{
		SetTargetServerVersion(o.ServerVersion);
	}

	internal void SetTargetDatabaseEngineType(SqlSmoObject o)
	{
		SetTargetDatabaseEngineType(o.DatabaseEngineType);
	}

	internal void SetTargetDatabaseEngineEdition(SqlSmoObject o)
	{
		SetTargetDatabaseEngineEdition(o.DatabaseEngineEdition);
	}

	internal bool TargetEngineIsAzureStretchDb()
	{
		if (TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			return TargetDatabaseEngineEdition == DatabaseEngineEdition.SqlStretchDatabase;
		}
		return false;
	}

	internal bool TargetEngineIsAzureSqlDw()
	{
		if (TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			return TargetDatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse;
		}
		return false;
	}

	internal void SetTargetServerInfo(SqlSmoObject o)
	{
		SetTargetServerInfo(o, forced: true);
	}

	internal void SetTargetServerInfo(SqlSmoObject o, bool forced)
	{
		if (forced || !DatabaseEngineTypeDirty)
		{
			SetTargetDatabaseEngineType(o.DatabaseEngineType);
		}
		if (forced || !VersionDirty)
		{
			SetTargetServerVersion(o.ServerVersion);
		}
		if (forced || !DatabaseEngineEditionDirty)
		{
			SetTargetDatabaseEngineEdition(o.DatabaseEngineEdition);
		}
	}

	internal ScriptingPreferences()
	{
		Init();
	}

	internal ScriptingPreferences(SqlSmoObject Obj)
	{
		Init();
		SetTargetServerInfo(Obj);
	}

	private void Init()
	{
		Behavior = ScriptBehavior.Create;
		SystemObjects = true;
		NewLine = Globals.newline;
		SuppressDirtyCheck = true;
		SfcChildren = true;
		m_eTargetServerVersion = SqlServerVersionInternal.Version140;
		m_eTargetDatabaseEngineType = DatabaseEngineType.Standalone;
		IncludeScripts = new IncludeScriptPreferences();
		Table = new TablePreferences();
		Security = new SecurityPreferences();
		Storage = new StoragePreferences();
		DataType = new DataTypePreferences();
		Data = new DataPreferences();
		OldOptions = new OldScriptingOptions();
	}

	internal object Clone()
	{
		ScriptingPreferences scriptingPreferences = (ScriptingPreferences)MemberwiseClone();
		scriptingPreferences.IncludeScripts = (IncludeScriptPreferences)IncludeScripts.Clone();
		scriptingPreferences.agentPreferences = (AgentPreferences)Agent.Clone();
		scriptingPreferences.mailPreferences = (MailPreferences)Mail.Clone();
		scriptingPreferences.Data = (DataPreferences)Data.Clone();
		scriptingPreferences.DataType = (DataTypePreferences)DataType.Clone();
		scriptingPreferences.OldOptions = (OldScriptingOptions)OldOptions.Clone();
		scriptingPreferences.Security = (SecurityPreferences)Security.Clone();
		scriptingPreferences.Storage = (StoragePreferences)Storage.Clone();
		scriptingPreferences.Table = (TablePreferences)Table.Clone();
		return scriptingPreferences;
	}
}
