using System;
using System.Collections;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Smo.Agent;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ScriptingOptions
{
	private string m_sFileName = string.Empty;

	private Encoding m_encoding;

	private bool skipClusteredIndexes;

	private ScriptingPreferences scriptingPreferences;

	private int m_batchSize = 1;

	private BitArray m_options;

	internal string NewLine
	{
		get
		{
			return scriptingPreferences.NewLine;
		}
		set
		{
			scriptingPreferences.NewLine = value;
		}
	}

	public string FileName
	{
		get
		{
			return m_sFileName;
		}
		set
		{
			m_sFileName = value;
		}
	}

	public Encoding Encoding
	{
		get
		{
			if (m_encoding == null)
			{
				m_encoding = new UnicodeEncoding();
			}
			return m_encoding;
		}
		set
		{
			m_encoding = value;
			this[EnumScriptOptions.AnsiFile] = m_encoding == Encoding.GetEncoding(1252);
		}
	}

	public bool ScriptForCreateDrop
	{
		get
		{
			return scriptingPreferences.ScriptForCreateDrop;
		}
		set
		{
			scriptingPreferences.ScriptForCreateDrop = value;
		}
	}

	public bool ScriptForAlter
	{
		get
		{
			return scriptingPreferences.ScriptForAlter;
		}
		set
		{
			scriptingPreferences.ScriptForAlter = value;
		}
	}

	internal bool SkipClusteredIndexes
	{
		get
		{
			return skipClusteredIndexes;
		}
		set
		{
			skipClusteredIndexes = value;
		}
	}

	public bool DriWithNoCheck
	{
		get
		{
			return scriptingPreferences.Table.ConstraintsWithNoCheck;
		}
		set
		{
			scriptingPreferences.Table.ConstraintsWithNoCheck = value;
		}
	}

	public bool IncludeFullTextCatalogRootPath
	{
		get
		{
			return scriptingPreferences.OldOptions.IncludeFullTextCatalogRootPath;
		}
		set
		{
			scriptingPreferences.OldOptions.IncludeFullTextCatalogRootPath = value;
		}
	}

	public bool SpatialIndexes
	{
		get
		{
			return this[EnumScriptOptions.SpatialIndexes];
		}
		set
		{
			this[EnumScriptOptions.SpatialIndexes] = value;
		}
	}

	public bool ColumnStoreIndexes
	{
		get
		{
			return this[EnumScriptOptions.ColumnStoreIndexes];
		}
		set
		{
			this[EnumScriptOptions.ColumnStoreIndexes] = value;
		}
	}

	public int BatchSize
	{
		get
		{
			return m_batchSize;
		}
		set
		{
			m_batchSize = value;
		}
	}

	public bool ScriptDrops
	{
		get
		{
			return scriptingPreferences.Behavior != ScriptBehavior.Create;
		}
		set
		{
			if (value)
			{
				scriptingPreferences.Behavior = ScriptBehavior.Drop;
			}
			else
			{
				scriptingPreferences.Behavior = ScriptBehavior.Create;
			}
		}
	}

	internal SqlServerVersionInternal TargetServerVersionInternal
	{
		get
		{
			return scriptingPreferences.TargetServerVersionInternal;
		}
		set
		{
			scriptingPreferences.TargetServerVersionInternal = value;
		}
	}

	public SqlServerVersion TargetServerVersion
	{
		get
		{
			return scriptingPreferences.TargetServerVersion;
		}
		set
		{
			scriptingPreferences.TargetServerVersion = value;
		}
	}

	public DatabaseEngineType TargetDatabaseEngineType
	{
		get
		{
			return scriptingPreferences.TargetDatabaseEngineType;
		}
		set
		{
			scriptingPreferences.TargetDatabaseEngineType = value;
		}
	}

	public DatabaseEngineEdition TargetDatabaseEngineEdition
	{
		get
		{
			return scriptingPreferences.TargetDatabaseEngineEdition;
		}
		set
		{
			scriptingPreferences.TargetDatabaseEngineEdition = value;
		}
	}

	public bool AnsiFile
	{
		get
		{
			return this[EnumScriptOptions.AnsiFile];
		}
		set
		{
			this[EnumScriptOptions.AnsiFile] = value;
			if (value)
			{
				m_encoding = Encoding.GetEncoding(1252);
			}
			else if (m_encoding == Encoding.GetEncoding(1252))
			{
				m_encoding = new UnicodeEncoding();
			}
		}
	}

	public bool AppendToFile
	{
		get
		{
			return this[EnumScriptOptions.AppendToFile];
		}
		set
		{
			this[EnumScriptOptions.AppendToFile] = value;
		}
	}

	public bool ToFileOnly
	{
		get
		{
			return this[EnumScriptOptions.ToFileOnly];
		}
		set
		{
			this[EnumScriptOptions.ToFileOnly] = value;
		}
	}

	public bool SchemaQualify
	{
		get
		{
			return scriptingPreferences.IncludeScripts.SchemaQualify;
		}
		set
		{
			scriptingPreferences.IncludeScripts.SchemaQualify = value;
		}
	}

	public bool IncludeHeaders
	{
		get
		{
			return scriptingPreferences.IncludeScripts.Header;
		}
		set
		{
			scriptingPreferences.IncludeScripts.Header = value;
		}
	}

	public bool IncludeScriptingParametersHeader
	{
		get
		{
			return scriptingPreferences.IncludeScripts.ScriptingParameterHeader;
		}
		set
		{
			scriptingPreferences.IncludeScripts.ScriptingParameterHeader = value;
		}
	}

	public bool IncludeIfNotExists
	{
		get
		{
			return scriptingPreferences.IncludeScripts.ExistenceCheck;
		}
		set
		{
			scriptingPreferences.IncludeScripts.ExistenceCheck = value;
		}
	}

	public bool WithDependencies
	{
		get
		{
			return scriptingPreferences.DependentObjects;
		}
		set
		{
			scriptingPreferences.DependentObjects = value;
		}
	}

	public bool DriPrimaryKey
	{
		get
		{
			return this[EnumScriptOptions.DriPrimaryKey];
		}
		set
		{
			this[EnumScriptOptions.DriPrimaryKey] = value;
		}
	}

	public bool DriForeignKeys
	{
		get
		{
			return this[EnumScriptOptions.DriForeignKeys];
		}
		set
		{
			this[EnumScriptOptions.DriForeignKeys] = value;
		}
	}

	public bool DriUniqueKeys
	{
		get
		{
			return this[EnumScriptOptions.DriUniqueKeys];
		}
		set
		{
			this[EnumScriptOptions.DriUniqueKeys] = value;
		}
	}

	public bool DriClustered
	{
		get
		{
			return this[EnumScriptOptions.DriClustered];
		}
		set
		{
			this[EnumScriptOptions.DriClustered] = value;
		}
	}

	public bool DriNonClustered
	{
		get
		{
			return this[EnumScriptOptions.DriNonClustered];
		}
		set
		{
			this[EnumScriptOptions.DriNonClustered] = value;
		}
	}

	public bool DriChecks
	{
		get
		{
			return this[EnumScriptOptions.DriChecks];
		}
		set
		{
			this[EnumScriptOptions.DriChecks] = value;
		}
	}

	public bool DriDefaults
	{
		get
		{
			return this[EnumScriptOptions.DriDefaults];
		}
		set
		{
			this[EnumScriptOptions.DriDefaults] = value;
		}
	}

	public bool Triggers
	{
		get
		{
			return this[EnumScriptOptions.Triggers];
		}
		set
		{
			this[EnumScriptOptions.Triggers] = value;
		}
	}

	public bool Statistics
	{
		get
		{
			return this[EnumScriptOptions.Statistics];
		}
		set
		{
			this[EnumScriptOptions.Statistics] = value;
		}
	}

	public bool ClusteredIndexes
	{
		get
		{
			return this[EnumScriptOptions.ClusteredIndexes];
		}
		set
		{
			this[EnumScriptOptions.ClusteredIndexes] = value;
		}
	}

	public bool NonClusteredIndexes
	{
		get
		{
			return this[EnumScriptOptions.NonClusteredIndexes];
		}
		set
		{
			this[EnumScriptOptions.NonClusteredIndexes] = value;
		}
	}

	public bool NoAssemblies
	{
		get
		{
			return this[EnumScriptOptions.NoAssemblies];
		}
		set
		{
			this[EnumScriptOptions.NoAssemblies] = value;
		}
	}

	public bool PrimaryObject
	{
		get
		{
			return scriptingPreferences.OldOptions.PrimaryObject;
		}
		set
		{
			scriptingPreferences.OldOptions.PrimaryObject = value;
		}
	}

	public bool Default
	{
		get
		{
			return scriptingPreferences.OldOptions.PrimaryObject;
		}
		set
		{
			scriptingPreferences.OldOptions.PrimaryObject = value;
		}
	}

	public bool XmlIndexes
	{
		get
		{
			return this[EnumScriptOptions.XmlIndexes];
		}
		set
		{
			this[EnumScriptOptions.XmlIndexes] = value;
		}
	}

	public bool FullTextCatalogs
	{
		get
		{
			return this[EnumScriptOptions.FullTextCatalogs];
		}
		set
		{
			this[EnumScriptOptions.FullTextCatalogs] = value;
		}
	}

	public bool FullTextIndexes
	{
		get
		{
			return this[EnumScriptOptions.FullTextIndexes];
		}
		set
		{
			this[EnumScriptOptions.FullTextIndexes] = value;
		}
	}

	public bool FullTextStopLists
	{
		get
		{
			return this[EnumScriptOptions.FullTextStopLists];
		}
		set
		{
			this[EnumScriptOptions.FullTextStopLists] = value;
		}
	}

	public bool Indexes
	{
		get
		{
			return this[EnumScriptOptions.Indexes];
		}
		set
		{
			this[EnumScriptOptions.Indexes] = value;
		}
	}

	public bool DriIndexes
	{
		get
		{
			return this[EnumScriptOptions.DriIndexes];
		}
		set
		{
			this[EnumScriptOptions.DriIndexes] = value;
		}
	}

	public bool DriAllKeys
	{
		get
		{
			return this[EnumScriptOptions.DriAllKeys];
		}
		set
		{
			this[EnumScriptOptions.DriAllKeys] = value;
		}
	}

	public bool DriAllConstraints
	{
		get
		{
			return this[EnumScriptOptions.DriAllConstraints];
		}
		set
		{
			this[EnumScriptOptions.DriAllConstraints] = value;
		}
	}

	public bool DriAll
	{
		get
		{
			return this[EnumScriptOptions.DriAll];
		}
		set
		{
			this[EnumScriptOptions.DriAll] = value;
		}
	}

	public bool Bindings
	{
		get
		{
			return scriptingPreferences.OldOptions.Bindings;
		}
		set
		{
			scriptingPreferences.OldOptions.Bindings = value;
		}
	}

	public bool NoFileGroup
	{
		get
		{
			return !scriptingPreferences.Storage.FileGroup;
		}
		set
		{
			scriptingPreferences.Storage.FileGroup = !value;
		}
	}

	public bool NoFileStream
	{
		get
		{
			return !scriptingPreferences.Storage.FileStreamFileGroup;
		}
		set
		{
			scriptingPreferences.Storage.FileStreamFileGroup = !value;
		}
	}

	public bool NoFileStreamColumn
	{
		get
		{
			return !scriptingPreferences.Storage.FileStreamColumn;
		}
		set
		{
			scriptingPreferences.Storage.FileStreamColumn = !value;
		}
	}

	public bool NoCollation
	{
		get
		{
			return !scriptingPreferences.IncludeScripts.Collation;
		}
		set
		{
			scriptingPreferences.IncludeScripts.Collation = !value;
		}
	}

	public bool ContinueScriptingOnError
	{
		get
		{
			return scriptingPreferences.ContinueOnScriptingError;
		}
		set
		{
			scriptingPreferences.ContinueOnScriptingError = value;
		}
	}

	public bool IncludeDatabaseRoleMemberships
	{
		get
		{
			return scriptingPreferences.IncludeScripts.Associations;
		}
		set
		{
			scriptingPreferences.IncludeScripts.Associations = value;
		}
	}

	public bool Permissions
	{
		get
		{
			return scriptingPreferences.IncludeScripts.Permissions;
		}
		set
		{
			scriptingPreferences.IncludeScripts.Permissions = value;
		}
	}

	public bool AllowSystemObjects
	{
		get
		{
			return scriptingPreferences.SystemObjects;
		}
		set
		{
			scriptingPreferences.SystemObjects = value;
		}
	}

	public bool NoIdentities
	{
		get
		{
			return !scriptingPreferences.Table.Identities;
		}
		set
		{
			scriptingPreferences.Table.Identities = !value;
		}
	}

	public bool ConvertUserDefinedDataTypesToBaseType
	{
		get
		{
			return scriptingPreferences.DataType.UserDefinedDataTypesToBaseType;
		}
		set
		{
			scriptingPreferences.DataType.UserDefinedDataTypesToBaseType = value;
		}
	}

	public bool TimestampToBinary
	{
		get
		{
			return scriptingPreferences.DataType.TimestampToBinary;
		}
		set
		{
			scriptingPreferences.DataType.TimestampToBinary = value;
		}
	}

	public bool AnsiPadding
	{
		get
		{
			return scriptingPreferences.IncludeScripts.AnsiPadding;
		}
		set
		{
			scriptingPreferences.IncludeScripts.AnsiPadding = value;
		}
	}

	public bool ExtendedProperties
	{
		get
		{
			return scriptingPreferences.IncludeScripts.ExtendedProperties;
		}
		set
		{
			scriptingPreferences.IncludeScripts.ExtendedProperties = value;
		}
	}

	public bool DdlHeaderOnly
	{
		get
		{
			return scriptingPreferences.OldOptions.DdlHeaderOnly;
		}
		set
		{
			scriptingPreferences.OldOptions.DdlHeaderOnly = value;
		}
	}

	public bool DdlBodyOnly
	{
		get
		{
			return scriptingPreferences.OldOptions.DdlBodyOnly;
		}
		set
		{
			scriptingPreferences.OldOptions.DdlBodyOnly = value;
		}
	}

	public bool NoViewColumns
	{
		get
		{
			return scriptingPreferences.OldOptions.NoViewColumns;
		}
		set
		{
			scriptingPreferences.OldOptions.NoViewColumns = value;
		}
	}

	public bool SchemaQualifyForeignKeysReferences
	{
		get
		{
			return scriptingPreferences.IncludeScripts.SchemaQualifyForeignKeysReferences;
		}
		set
		{
			scriptingPreferences.IncludeScripts.SchemaQualifyForeignKeysReferences = value;
		}
	}

	public bool AgentAlertJob
	{
		get
		{
			return scriptingPreferences.Agent.AlertJob;
		}
		set
		{
			scriptingPreferences.Agent.AlertJob = value;
		}
	}

	public bool AgentJobId
	{
		get
		{
			return scriptingPreferences.Agent.JobId;
		}
		set
		{
			scriptingPreferences.Agent.JobId = value;
		}
	}

	public bool AgentNotify
	{
		get
		{
			return scriptingPreferences.Agent.Notify;
		}
		set
		{
			scriptingPreferences.Agent.Notify = value;
		}
	}

	public bool LoginSid
	{
		get
		{
			return scriptingPreferences.Security.Sid;
		}
		set
		{
			scriptingPreferences.Security.Sid = value;
		}
	}

	public bool NoCommandTerminator
	{
		get
		{
			return this[EnumScriptOptions.NoCommandTerminator];
		}
		set
		{
			this[EnumScriptOptions.NoCommandTerminator] = value;
		}
	}

	public bool NoIndexPartitioningSchemes
	{
		get
		{
			return (scriptingPreferences.Storage.PartitionSchemeInternal & PartitioningScheme.Index) != PartitioningScheme.Index;
		}
		set
		{
			if (value)
			{
				scriptingPreferences.Storage.PartitionSchemeInternal &= ~PartitioningScheme.Index;
			}
			else
			{
				scriptingPreferences.Storage.PartitionSchemeInternal |= PartitioningScheme.Index;
			}
		}
	}

	public bool NoTablePartitioningSchemes
	{
		get
		{
			return (scriptingPreferences.Storage.PartitionSchemeInternal & PartitioningScheme.Table) != PartitioningScheme.Table;
		}
		set
		{
			if (value)
			{
				scriptingPreferences.Storage.PartitionSchemeInternal &= ~PartitioningScheme.Table;
			}
			else
			{
				scriptingPreferences.Storage.PartitionSchemeInternal |= PartitioningScheme.Table;
			}
		}
	}

	public bool IncludeDatabaseContext
	{
		get
		{
			return scriptingPreferences.IncludeScripts.DatabaseContext;
		}
		set
		{
			scriptingPreferences.IncludeScripts.DatabaseContext = value;
		}
	}

	public bool NoXmlNamespaces
	{
		get
		{
			return !scriptingPreferences.DataType.XmlNamespaces;
		}
		set
		{
			scriptingPreferences.DataType.XmlNamespaces = !value;
		}
	}

	public bool DriIncludeSystemNames
	{
		get
		{
			return scriptingPreferences.Table.SystemNamesForConstraints;
		}
		set
		{
			scriptingPreferences.Table.SystemNamesForConstraints = value;
		}
	}

	public bool OptimizerData
	{
		get
		{
			return scriptingPreferences.Data.OptimizerData;
		}
		set
		{
			scriptingPreferences.Data.OptimizerData = value;
		}
	}

	public bool NoExecuteAs
	{
		get
		{
			return !scriptingPreferences.Security.ExecuteAs;
		}
		set
		{
			scriptingPreferences.Security.ExecuteAs = !value;
		}
	}

	public bool EnforceScriptingOptions
	{
		get
		{
			return scriptingPreferences.OldOptions.EnforceScriptingPreferences;
		}
		set
		{
			scriptingPreferences.OldOptions.EnforceScriptingPreferences = value;
		}
	}

	public bool NoMailProfileAccounts
	{
		get
		{
			return !scriptingPreferences.Mail.Accounts;
		}
		set
		{
			scriptingPreferences.Mail.Accounts = !value;
		}
	}

	public bool NoMailProfilePrincipals
	{
		get
		{
			return !scriptingPreferences.Mail.Principals;
		}
		set
		{
			scriptingPreferences.Mail.Principals = !value;
		}
	}

	public bool NoVardecimal
	{
		get
		{
			return scriptingPreferences.OldOptions.NoVardecimal;
		}
		set
		{
			scriptingPreferences.OldOptions.NoVardecimal = value;
		}
	}

	public bool ChangeTracking
	{
		get
		{
			return scriptingPreferences.Data.ChangeTracking;
		}
		set
		{
			scriptingPreferences.Data.ChangeTracking = value;
		}
	}

	public bool ScriptDataCompression
	{
		get
		{
			return scriptingPreferences.Storage.DataCompression;
		}
		set
		{
			scriptingPreferences.Storage.DataCompression = value;
		}
	}

	public bool ScriptSchema
	{
		get
		{
			return scriptingPreferences.IncludeScripts.Ddl;
		}
		set
		{
			scriptingPreferences.IncludeScripts.Ddl = value;
		}
	}

	public bool ScriptData
	{
		get
		{
			return scriptingPreferences.IncludeScripts.Data;
		}
		set
		{
			scriptingPreferences.IncludeScripts.Data = value;
		}
	}

	public bool ScriptBatchTerminator
	{
		get
		{
			return this[EnumScriptOptions.ScriptBatchTerminator];
		}
		set
		{
			this[EnumScriptOptions.ScriptBatchTerminator] = value;
		}
	}

	public bool ScriptOwner
	{
		get
		{
			return scriptingPreferences.IncludeScripts.Owner;
		}
		set
		{
			scriptingPreferences.IncludeScripts.Owner = value;
		}
	}

	private bool this[EnumScriptOptions eso]
	{
		get
		{
			if (eso <= EnumScriptOptions.TotalFilterOptions)
			{
				return m_options[Convert.ToInt32(eso, SmoApplication.DefaultCulture)];
			}
			return GetScriptingPreference(eso);
		}
		set
		{
			if (eso <= EnumScriptOptions.TotalFilterOptions)
			{
				m_options[Convert.ToInt32(eso, SmoApplication.DefaultCulture)] = value;
			}
			else
			{
				SetScriptingPreference(eso, value);
			}
		}
	}

	public ScriptingOptions()
	{
		Init();
	}

	public ScriptingOptions(ScriptingOptions so)
	{
		m_options = (BitArray)so.m_options.Clone();
		scriptingPreferences = (ScriptingPreferences)so.scriptingPreferences.Clone();
		m_sFileName = so.m_sFileName;
		m_encoding = so.m_encoding;
	}

	public ScriptingOptions(ScriptOption so)
	{
		InitializeOptionsAsFalse();
		this[so.Value] = true;
		if (so.Value != EnumScriptOptions.ScriptData)
		{
			this[EnumScriptOptions.ScriptSchema] = true;
		}
	}

	private void InitializeOptionsAsFalse()
	{
		m_options = InitializeBitArray();
		scriptingPreferences = new ScriptingPreferences();
		AllowSystemObjects = false;
		SchemaQualify = false;
		PrimaryObject = false;
		ScriptDataCompression = false;
		AgentJobId = false;
		NoVardecimal = false;
		ScriptSchema = false;
		DriWithNoCheck = false;
	}

	private BitArray InitializeBitArray()
	{
		return new BitArray(Convert.ToInt32(EnumScriptOptions.TotalFilterOptions, SmoApplication.DefaultCulture));
	}

	internal ScriptingOptions(SqlSmoObject parent)
	{
		Init();
		SetTargetServerInfo(parent);
	}

	internal ScriptingOptions(params EnumScriptOptions[] options)
	{
		InitializeOptionsAsFalse();
		bool flag = false;
		foreach (EnumScriptOptions enumScriptOptions in options)
		{
			this[enumScriptOptions] = true;
			if (enumScriptOptions == EnumScriptOptions.ScriptData)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			this[EnumScriptOptions.ScriptSchema] = true;
		}
	}

	private void Init()
	{
		m_options = InitializeBitArray();
		scriptingPreferences = new ScriptingPreferences();
		AllowSystemObjects = true;
		DriWithNoCheck = false;
		NoIdentities = false;
		ConvertUserDefinedDataTypesToBaseType = false;
		TimestampToBinary = false;
		SchemaQualify = true;
		AnsiPadding = false;
		ExtendedProperties = false;
		DdlHeaderOnly = false;
		DdlBodyOnly = false;
		NoViewColumns = false;
		PrimaryObject = true;
		DriIncludeSystemNames = false;
		NoExecuteAs = false;
		EnforceScriptingOptions = false;
		NoMailProfileAccounts = false;
		ScriptDataCompression = true;
		NoMailProfilePrincipals = false;
		OptimizerData = false;
		AgentJobId = true;
		NoVardecimal = true;
		IncludeDatabaseRoleMemberships = false;
		ChangeTracking = false;
		ScriptSchema = true;
		ScriptData = false;
		ScriptBatchTerminator = false;
		IncludeFullTextCatalogRootPath = false;
		ScriptOwner = false;
	}

	public ScriptingOptions Add(ScriptOption scriptOption)
	{
		this[scriptOption.Value] = true;
		return this;
	}

	public ScriptingOptions Remove(ScriptOption scriptOption)
	{
		this[scriptOption.Value] = false;
		return this;
	}

	public static ScriptingOptions operator +(ScriptingOptions options, ScriptOption scriptOption)
	{
		ScriptingOptions scriptingOptions = new ScriptingOptions(options);
		scriptingOptions[scriptOption.Value] = true;
		return scriptingOptions;
	}

	public static ScriptingOptions Add(ScriptingOptions options, ScriptOption scriptOption)
	{
		return options + scriptOption;
	}

	public static ScriptingOptions operator -(ScriptingOptions options, ScriptOption scriptOption)
	{
		ScriptingOptions scriptingOptions = new ScriptingOptions(options);
		scriptingOptions[scriptOption.Value] = false;
		return scriptingOptions;
	}

	public static ScriptingOptions Subtract(ScriptingOptions options, ScriptOption scriptOption)
	{
		return options - scriptOption;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GetType().Name + ": ");
		int num = 0;
		bool flag = true;
		foreach (bool option in m_options)
		{
			if (option)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(((EnumScriptOptions)num).ToString());
			}
			num++;
		}
		for (int i = 29; i < 74; i++)
		{
			if (this[(EnumScriptOptions)i])
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(((EnumScriptOptions)i).ToString());
			}
		}
		return stringBuilder.ToString();
	}

	public void SetTargetServerVersion(ServerVersion ver)
	{
		scriptingPreferences.SetTargetServerVersion(ver);
	}

	internal static ServerVersion ConvertToServerVersion(SqlServerVersion ver)
	{
		switch (ver)
		{
		case SqlServerVersion.Version80:
			return new ServerVersion(8, 0, 0);
		case SqlServerVersion.Version90:
			return new ServerVersion(9, 0, 0);
		case SqlServerVersion.Version100:
			return new ServerVersion(10, 0, 0);
		case SqlServerVersion.Version105:
			return new ServerVersion(10, 50, 0);
		case SqlServerVersion.Version110:
			return new ServerVersion(11, 0, 0);
		case SqlServerVersion.Version120:
			return new ServerVersion(12, 0, 0);
		case SqlServerVersion.Version130:
			return new ServerVersion(13, 0, 0);
		case SqlServerVersion.Version140:
			return new ServerVersion(14, 0, 0);
		default:
			TraceHelper.Assert(ver == SqlServerVersion.Version150, "unexpected server version");
			return new ServerVersion(15, 0, 0);
		}
	}

	public void SetTargetDatabaseEngineType(DatabaseEngineType databaseEngineType)
	{
		TargetDatabaseEngineType = databaseEngineType;
	}

	internal void SetTargetDatabaseEngineType(SqlSmoObject o)
	{
		SetTargetDatabaseEngineType(o.DatabaseEngineType);
	}

	internal void SetTargetServerVersion(SqlSmoObject o)
	{
		scriptingPreferences.SetTargetServerVersion(o);
	}

	internal void SetTargetServerInfo(SqlSmoObject o)
	{
		scriptingPreferences.SetTargetServerInfo(o);
	}

	internal void SetTargetServerInfo(SqlSmoObject o, bool forced)
	{
		scriptingPreferences.SetTargetServerInfo(o, forced);
	}

	internal bool ScriptDriPrimaryKey()
	{
		if (!DriPrimaryKey && !Indexes && !DriIndexes && !DriAllKeys && !DriAllConstraints)
		{
			return DriAll;
		}
		return true;
	}

	internal bool ScriptDriForeignKeys()
	{
		if (!DriForeignKeys && !DriAllKeys && !DriAllConstraints)
		{
			return DriAll;
		}
		return true;
	}

	internal bool ScriptDriUniqueKeys()
	{
		if (!DriUniqueKeys && !Indexes && !DriIndexes && !DriAllKeys && !DriAllConstraints)
		{
			return DriAll;
		}
		return true;
	}

	internal bool ScriptDriClustered()
	{
		if (!DriClustered && !Indexes && !DriIndexes)
		{
			return DriAll;
		}
		return true;
	}

	internal bool ScriptDriNonClustered()
	{
		if (!DriNonClustered && !Indexes && !DriIndexes)
		{
			return DriAll;
		}
		return true;
	}

	internal bool ScriptDriChecks()
	{
		if (!DriChecks && !DriAllConstraints)
		{
			return DriAll;
		}
		return true;
	}

	internal bool ScriptDriDefaults()
	{
		if (!DriDefaults && !DriAllConstraints)
		{
			return DriAll;
		}
		return true;
	}

	internal bool ScriptClusteredIndexes()
	{
		if (!ClusteredIndexes)
		{
			return Indexes;
		}
		return true;
	}

	internal bool ScriptNonClusteredIndexes()
	{
		if (!NonClusteredIndexes)
		{
			return Indexes;
		}
		return true;
	}

	internal bool ScriptXmlIndexes()
	{
		if (!XmlIndexes)
		{
			return Indexes;
		}
		return true;
	}

	private void SetScriptingPreference(EnumScriptOptions eso, bool value)
	{
		switch (eso)
		{
		case EnumScriptOptions.NoTablePartitioningSchemes:
			NoTablePartitioningSchemes = value;
			break;
		case EnumScriptOptions.NoIndexPartitioningSchemes:
			NoIndexPartitioningSchemes = value;
			break;
		case EnumScriptOptions.SchemaQualifyForeignKeysReferences:
			SchemaQualifyForeignKeysReferences = value;
			break;
		case EnumScriptOptions.IncludeDatabaseRoleMemberships:
			IncludeDatabaseRoleMemberships = value;
			break;
		case EnumScriptOptions.SchemaQualify:
			SchemaQualify = value;
			break;
		case EnumScriptOptions.IncludeHeaders:
			IncludeHeaders = value;
			break;
		case EnumScriptOptions.IncludeIfNotExists:
			IncludeIfNotExists = value;
			break;
		case EnumScriptOptions.WithDependencies:
			WithDependencies = value;
			break;
		case EnumScriptOptions.Bindings:
			Bindings = value;
			break;
		case EnumScriptOptions.ContinueScriptingOnError:
			ContinueScriptingOnError = value;
			break;
		case EnumScriptOptions.Permissions:
			Permissions = value;
			break;
		case EnumScriptOptions.AllowSystemObjects:
			AllowSystemObjects = value;
			break;
		case EnumScriptOptions.DriWithNoCheck:
			DriWithNoCheck = value;
			break;
		case EnumScriptOptions.ConvertUserDefinedDataTypesToBaseType:
			ConvertUserDefinedDataTypesToBaseType = value;
			break;
		case EnumScriptOptions.TimestampToBinary:
			TimestampToBinary = value;
			break;
		case EnumScriptOptions.AnsiPadding:
			AnsiPadding = value;
			break;
		case EnumScriptOptions.ExtendedProperties:
			ExtendedProperties = value;
			break;
		case EnumScriptOptions.DdlHeaderOnly:
			DdlHeaderOnly = value;
			break;
		case EnumScriptOptions.DdlBodyOnly:
			DdlBodyOnly = value;
			break;
		case EnumScriptOptions.NoViewColumns:
			NoViewColumns = value;
			break;
		case EnumScriptOptions.LoginSid:
			LoginSid = value;
			break;
		case EnumScriptOptions.IncludeDatabaseContext:
			IncludeDatabaseContext = value;
			break;
		case EnumScriptOptions.AgentAlertJob:
			AgentAlertJob = value;
			break;
		case EnumScriptOptions.AgentJobId:
			AgentJobId = value;
			break;
		case EnumScriptOptions.AgentNotify:
			AgentNotify = value;
			break;
		case EnumScriptOptions.DriIncludeSystemNames:
			DriIncludeSystemNames = value;
			break;
		case EnumScriptOptions.OptimizerData:
			OptimizerData = value;
			break;
		case EnumScriptOptions.NoExecuteAs:
			NoExecuteAs = value;
			break;
		case EnumScriptOptions.EnforceScriptingOptions:
			EnforceScriptingOptions = value;
			break;
		case EnumScriptOptions.NoVardecimal:
			NoVardecimal = value;
			break;
		case EnumScriptOptions.ScriptSchema:
			ScriptSchema = value;
			break;
		case EnumScriptOptions.ScriptData:
			ScriptData = value;
			break;
		case EnumScriptOptions.IncludeFullTextCatalogRootPath:
			IncludeFullTextCatalogRootPath = value;
			break;
		case EnumScriptOptions.ChangeTracking:
			ChangeTracking = value;
			break;
		case EnumScriptOptions.ScriptDataCompression:
			ScriptDataCompression = value;
			break;
		case EnumScriptOptions.ScriptOwner:
			ScriptOwner = value;
			break;
		case EnumScriptOptions.NoFileGroup:
			NoFileGroup = value;
			break;
		case EnumScriptOptions.NoFileStream:
			NoFileStream = value;
			break;
		case EnumScriptOptions.NoFileStreamColumn:
			NoFileStreamColumn = value;
			break;
		case EnumScriptOptions.NoCollation:
			NoCollation = value;
			break;
		case EnumScriptOptions.NoIdentities:
			NoIdentities = value;
			break;
		case EnumScriptOptions.NoXmlNamespaces:
			NoXmlNamespaces = value;
			break;
		case EnumScriptOptions.NoMailProfileAccounts:
			NoMailProfileAccounts = value;
			break;
		case EnumScriptOptions.NoMailProfilePrincipals:
			NoMailProfilePrincipals = value;
			break;
		case EnumScriptOptions.PrimaryObject:
			PrimaryObject = value;
			break;
		default:
			TraceHelper.Assert(condition: false, "incorrect index specified");
			break;
		}
	}

	private bool GetScriptingPreference(EnumScriptOptions eso)
	{
		switch (eso)
		{
		case EnumScriptOptions.NoTablePartitioningSchemes:
			return NoTablePartitioningSchemes;
		case EnumScriptOptions.NoIndexPartitioningSchemes:
			return NoIndexPartitioningSchemes;
		case EnumScriptOptions.SchemaQualifyForeignKeysReferences:
			return SchemaQualifyForeignKeysReferences;
		case EnumScriptOptions.IncludeDatabaseRoleMemberships:
			return IncludeDatabaseRoleMemberships;
		case EnumScriptOptions.SchemaQualify:
			return SchemaQualify;
		case EnumScriptOptions.IncludeHeaders:
			return IncludeHeaders;
		case EnumScriptOptions.IncludeIfNotExists:
			return IncludeIfNotExists;
		case EnumScriptOptions.WithDependencies:
			return WithDependencies;
		case EnumScriptOptions.Bindings:
			return Bindings;
		case EnumScriptOptions.ContinueScriptingOnError:
			return ContinueScriptingOnError;
		case EnumScriptOptions.Permissions:
			return Permissions;
		case EnumScriptOptions.AllowSystemObjects:
			return AllowSystemObjects;
		case EnumScriptOptions.DriWithNoCheck:
			return DriWithNoCheck;
		case EnumScriptOptions.ConvertUserDefinedDataTypesToBaseType:
			return ConvertUserDefinedDataTypesToBaseType;
		case EnumScriptOptions.TimestampToBinary:
			return TimestampToBinary;
		case EnumScriptOptions.AnsiPadding:
			return AnsiPadding;
		case EnumScriptOptions.ExtendedProperties:
			return ExtendedProperties;
		case EnumScriptOptions.DdlHeaderOnly:
			return DdlHeaderOnly;
		case EnumScriptOptions.DdlBodyOnly:
			return DdlBodyOnly;
		case EnumScriptOptions.NoViewColumns:
			return NoViewColumns;
		case EnumScriptOptions.LoginSid:
			return LoginSid;
		case EnumScriptOptions.IncludeDatabaseContext:
			return IncludeDatabaseContext;
		case EnumScriptOptions.AgentAlertJob:
			return AgentAlertJob;
		case EnumScriptOptions.AgentJobId:
			return AgentJobId;
		case EnumScriptOptions.AgentNotify:
			return AgentNotify;
		case EnumScriptOptions.DriIncludeSystemNames:
			return DriIncludeSystemNames;
		case EnumScriptOptions.OptimizerData:
			return OptimizerData;
		case EnumScriptOptions.NoExecuteAs:
			return NoExecuteAs;
		case EnumScriptOptions.EnforceScriptingOptions:
			return EnforceScriptingOptions;
		case EnumScriptOptions.NoVardecimal:
			return NoVardecimal;
		case EnumScriptOptions.ScriptSchema:
			return ScriptSchema;
		case EnumScriptOptions.ScriptData:
			return ScriptData;
		case EnumScriptOptions.IncludeFullTextCatalogRootPath:
			return IncludeFullTextCatalogRootPath;
		case EnumScriptOptions.ChangeTracking:
			return ChangeTracking;
		case EnumScriptOptions.ScriptDataCompression:
			return ScriptDataCompression;
		case EnumScriptOptions.ScriptOwner:
			return ScriptOwner;
		case EnumScriptOptions.NoFileGroup:
			return NoFileGroup;
		case EnumScriptOptions.NoFileStream:
			return NoFileStream;
		case EnumScriptOptions.NoFileStreamColumn:
			return NoFileStreamColumn;
		case EnumScriptOptions.NoCollation:
			return NoCollation;
		case EnumScriptOptions.NoIdentities:
			return NoIdentities;
		case EnumScriptOptions.NoXmlNamespaces:
			return NoXmlNamespaces;
		case EnumScriptOptions.NoMailProfileAccounts:
			return NoMailProfileAccounts;
		case EnumScriptOptions.NoMailProfilePrincipals:
			return NoMailProfilePrincipals;
		case EnumScriptOptions.PrimaryObject:
			return PrimaryObject;
		default:
			TraceHelper.Assert(condition: false, "incorrect index specified");
			return false;
		}
	}

	public static SqlServerVersion ConvertVersion(Version version)
	{
		return ConvertToSqlServerVersion(version.Major, version.Minor);
	}

	public static SqlServerVersion ConvertToSqlServerVersion(ServerVersion serverVersion)
	{
		return ConvertToSqlServerVersion(serverVersion.Major, serverVersion.Minor);
	}

	public static SqlServerVersion ConvertToSqlServerVersion(int majorVersion, int minorVersion)
	{
		switch (majorVersion)
		{
		case 8:
			return SqlServerVersion.Version80;
		case 9:
			return SqlServerVersion.Version90;
		case 10:
			if (minorVersion == 0)
			{
				return SqlServerVersion.Version100;
			}
			return SqlServerVersion.Version105;
		case 11:
			return SqlServerVersion.Version110;
		case 12:
			return SqlServerVersion.Version120;
		case 13:
			return SqlServerVersion.Version130;
		case 14:
			return SqlServerVersion.Version140;
		case 15:
			return SqlServerVersion.Version150;
		default:
			throw new SmoException(ExceptionTemplatesImpl.InvalidVersion(majorVersion.ToString()));
		}
	}

	internal ScriptingPreferences GetScriptingPreferences()
	{
		return scriptingPreferences;
	}

	internal SmoUrnFilter GetSmoUrnFilterForDiscovery(Server srv)
	{
		SmoUrnFilter smoUrnFilter = new SmoUrnFilter(srv);
		if (!ExtendedProperties)
		{
			smoUrnFilter.AddFilteredType(ExtendedProperty.UrnSuffix, null);
		}
		if (!ScriptDriChecks())
		{
			smoUrnFilter.AddFilteredType(Check.UrnSuffix, null);
		}
		if (!ScriptDriDefaults())
		{
			smoUrnFilter.AddFilteredType(DefaultConstraint.UrnSuffix, Column.UrnSuffix);
		}
		if (!ScriptDriForeignKeys())
		{
			smoUrnFilter.AddFilteredType(ForeignKey.UrnSuffix, null);
		}
		if (!FullTextCatalogs)
		{
			smoUrnFilter.AddFilteredType(FullTextCatalog.UrnSuffix, null);
		}
		if (!FullTextIndexes)
		{
			smoUrnFilter.AddFilteredType(FullTextIndex.UrnSuffix, null);
		}
		if (!FullTextStopLists)
		{
			smoUrnFilter.AddFilteredType(FullTextStopList.UrnSuffix, null);
		}
		AddIndexFilters(smoUrnFilter);
		if (NoAssemblies)
		{
			smoUrnFilter.AddFilteredType(SqlAssembly.UrnSuffix, null);
		}
		if (!Statistics)
		{
			smoUrnFilter.AddFilteredType(Statistic.UrnSuffix, null);
		}
		if (!Triggers)
		{
			smoUrnFilter.AddFilteredType(Trigger.UrnSuffix, null);
		}
		return smoUrnFilter;
	}

	internal static SmoUrnFilter GetAllFilters(Server srv)
	{
		SmoUrnFilter smoUrnFilter = new SmoUrnFilter(srv);
		smoUrnFilter.AddFilteredType(ExtendedProperty.UrnSuffix, null);
		smoUrnFilter.AddFilteredType(Check.UrnSuffix, null);
		smoUrnFilter.AddFilteredType(DefaultConstraint.UrnSuffix, Column.UrnSuffix);
		smoUrnFilter.AddFilteredType(ForeignKey.UrnSuffix, null);
		smoUrnFilter.AddFilteredType(FullTextCatalog.UrnSuffix, null);
		smoUrnFilter.AddFilteredType(FullTextIndex.UrnSuffix, null);
		smoUrnFilter.AddFilteredType(FullTextStopList.UrnSuffix, null);
		smoUrnFilter.AddFilteredType("XmlIndex", null);
		smoUrnFilter.AddFilteredType("SpatialIndex", null);
		smoUrnFilter.AddFilteredType("ColumnstoreIndex", null);
		smoUrnFilter.AddFilteredType("ClusteredIndex", null);
		smoUrnFilter.AddFilteredType("NonclusteredIndex", null);
		smoUrnFilter.AddFilteredType("ClusteredPrimaryKey", null);
		smoUrnFilter.AddFilteredType("NonclusteredPrimaryKey", null);
		smoUrnFilter.AddFilteredType("ClusteredUniqueKey", null);
		smoUrnFilter.AddFilteredType("NonclusteredUniqueKey", null);
		smoUrnFilter.AddFilteredType(SqlAssembly.UrnSuffix, null);
		smoUrnFilter.AddFilteredType(Statistic.UrnSuffix, null);
		smoUrnFilter.AddFilteredType(Trigger.UrnSuffix, null);
		return smoUrnFilter;
	}

	internal SmoUrnFilter GetSmoUrnFilterForFiltering(Server srv)
	{
		SmoUrnFilter smoUrnFilter = null;
		if (!PrimaryObject || NoAssemblies)
		{
			smoUrnFilter = new SmoUrnFilter(srv);
			if (!PrimaryObject)
			{
				smoUrnFilter.AddFilteredType(Table.UrnSuffix, null);
				smoUrnFilter.AddFilteredType(View.UrnSuffix, null);
				smoUrnFilter.AddFilteredType(UserDefinedFunction.UrnSuffix, null);
				smoUrnFilter.AddFilteredType(Job.UrnSuffix, null);
			}
			if (NoAssemblies)
			{
				smoUrnFilter.AddFilteredType(SqlAssembly.UrnSuffix, null);
			}
		}
		return smoUrnFilter;
	}

	private void AddIndexFilters(SmoUrnFilter smoUrnFilter)
	{
		if (TargetDatabaseEngineType != DatabaseEngineType.Standalone)
		{
			smoUrnFilter.AddFilteredType("XmlIndex", null);
			smoUrnFilter.AddFilteredType("ColumnstoreIndex", null);
		}
		else if (TargetServerVersion < SqlServerVersion.Version110)
		{
			smoUrnFilter.AddFilteredType("ColumnstoreIndex", null);
		}
		if (Indexes)
		{
			return;
		}
		if (!SpatialIndexes)
		{
			smoUrnFilter.AddFilteredType("SpatialIndex", null);
		}
		if (!ColumnStoreIndexes)
		{
			smoUrnFilter.AddFilteredType("ColumnstoreIndex", null);
		}
		if (!ClusteredIndexes)
		{
			smoUrnFilter.AddFilteredType("ClusteredIndex", null);
		}
		if (!NonClusteredIndexes)
		{
			smoUrnFilter.AddFilteredType("NonclusteredIndex", null);
		}
		if (!XmlIndexes)
		{
			smoUrnFilter.AddFilteredType("XmlIndex", null);
		}
		if (DriIndexes || DriAll || DriAllConstraints || DriAllKeys)
		{
			return;
		}
		if (!DriPrimaryKey)
		{
			if (!DriClustered)
			{
				smoUrnFilter.AddFilteredType("ClusteredPrimaryKey", null);
			}
			if (!DriNonClustered)
			{
				smoUrnFilter.AddFilteredType("NonclusteredPrimaryKey", null);
			}
		}
		if (!DriUniqueKeys)
		{
			if (!DriClustered)
			{
				smoUrnFilter.AddFilteredType("ClusteredUniqueKey", null);
			}
			if (!DriNonClustered)
			{
				smoUrnFilter.AddFilteredType("NonclusteredUniqueKey", null);
			}
		}
	}
}
