namespace Microsoft.SqlServer.Management.Smo;

public sealed class ScriptOption
{
	private EnumScriptOptions m_value;

	internal EnumScriptOptions Value => m_value;

	public static ScriptOption AppendToFile => new ScriptOption(EnumScriptOptions.AppendToFile);

	public static ScriptOption ToFileOnly => new ScriptOption(EnumScriptOptions.ToFileOnly);

	public static ScriptOption SchemaQualify => new ScriptOption(EnumScriptOptions.SchemaQualify);

	public static ScriptOption IncludeHeaders => new ScriptOption(EnumScriptOptions.IncludeHeaders);

	public static ScriptOption IncludeIfNotExists => new ScriptOption(EnumScriptOptions.IncludeIfNotExists);

	public static ScriptOption WithDependencies => new ScriptOption(EnumScriptOptions.WithDependencies);

	public static ScriptOption DriPrimaryKey => new ScriptOption(EnumScriptOptions.DriPrimaryKey);

	public static ScriptOption DriForeignKeys => new ScriptOption(EnumScriptOptions.DriForeignKeys);

	public static ScriptOption DriUniqueKeys => new ScriptOption(EnumScriptOptions.DriUniqueKeys);

	public static ScriptOption DriClustered => new ScriptOption(EnumScriptOptions.DriClustered);

	public static ScriptOption DriNonClustered => new ScriptOption(EnumScriptOptions.DriNonClustered);

	public static ScriptOption DriChecks => new ScriptOption(EnumScriptOptions.DriChecks);

	public static ScriptOption DriDefaults => new ScriptOption(EnumScriptOptions.DriDefaults);

	public static ScriptOption Triggers => new ScriptOption(EnumScriptOptions.Triggers);

	public static ScriptOption Bindings => new ScriptOption(EnumScriptOptions.Bindings);

	public static ScriptOption NoFileGroup => new ScriptOption(EnumScriptOptions.NoFileGroup);

	public static ScriptOption NoFileStream => new ScriptOption(EnumScriptOptions.NoFileStream);

	public static ScriptOption NoFileStreamColumn => new ScriptOption(EnumScriptOptions.NoFileStreamColumn);

	public static ScriptOption NoCollation => new ScriptOption(EnumScriptOptions.NoCollation);

	public static ScriptOption ContinueScriptingOnError => new ScriptOption(EnumScriptOptions.ContinueScriptingOnError);

	public static ScriptOption Permissions => new ScriptOption(EnumScriptOptions.Permissions);

	public static ScriptOption AllowSystemObjects => new ScriptOption(EnumScriptOptions.AllowSystemObjects);

	public static ScriptOption NoIdentities => new ScriptOption(EnumScriptOptions.NoIdentities);

	public static ScriptOption ConvertUserDefinedDataTypesToBaseType => new ScriptOption(EnumScriptOptions.ConvertUserDefinedDataTypesToBaseType);

	public static ScriptOption TimestampToBinary => new ScriptOption(EnumScriptOptions.TimestampToBinary);

	public static ScriptOption AnsiPadding => new ScriptOption(EnumScriptOptions.AnsiPadding);

	public static ScriptOption ExtendedProperties => new ScriptOption(EnumScriptOptions.ExtendedProperties);

	public static ScriptOption DdlHeaderOnly => new ScriptOption(EnumScriptOptions.DdlHeaderOnly);

	public static ScriptOption DdlBodyOnly => new ScriptOption(EnumScriptOptions.DdlBodyOnly);

	public static ScriptOption NoViewColumns => new ScriptOption(EnumScriptOptions.NoViewColumns);

	public static ScriptOption Statistics => new ScriptOption(EnumScriptOptions.Statistics);

	public static ScriptOption SchemaQualifyForeignKeysReferences => new ScriptOption(EnumScriptOptions.SchemaQualifyForeignKeysReferences);

	public static ScriptOption ClusteredIndexes => new ScriptOption(EnumScriptOptions.ClusteredIndexes);

	public static ScriptOption NonClusteredIndexes => new ScriptOption(EnumScriptOptions.NonClusteredIndexes);

	public static ScriptOption AnsiFile => new ScriptOption(EnumScriptOptions.AnsiFile);

	public static ScriptOption AgentAlertJob => new ScriptOption(EnumScriptOptions.AgentAlertJob);

	public static ScriptOption AgentJobId => new ScriptOption(EnumScriptOptions.AgentJobId);

	public static ScriptOption AgentNotify => new ScriptOption(EnumScriptOptions.AgentNotify);

	public static ScriptOption LoginSid => new ScriptOption(EnumScriptOptions.LoginSid);

	public static ScriptOption NoCommandTerminator => new ScriptOption(EnumScriptOptions.NoCommandTerminator);

	public static ScriptOption NoIndexPartitioningSchemes => new ScriptOption(EnumScriptOptions.NoIndexPartitioningSchemes);

	public static ScriptOption NoTablePartitioningSchemes => new ScriptOption(EnumScriptOptions.NoTablePartitioningSchemes);

	public static ScriptOption IncludeDatabaseContext => new ScriptOption(EnumScriptOptions.IncludeDatabaseContext);

	public static ScriptOption FullTextCatalogs => new ScriptOption(EnumScriptOptions.FullTextCatalogs);

	public static ScriptOption FullTextStopLists => new ScriptOption(EnumScriptOptions.FullTextStopLists);

	public static ScriptOption FullTextIndexes => new ScriptOption(EnumScriptOptions.FullTextIndexes);

	public static ScriptOption NoXmlNamespaces => new ScriptOption(EnumScriptOptions.NoXmlNamespaces);

	public static ScriptOption NoAssemblies => new ScriptOption(EnumScriptOptions.NoAssemblies);

	public static ScriptOption PrimaryObject => new ScriptOption(EnumScriptOptions.PrimaryObject);

	public static ScriptOption DriIncludeSystemNames => new ScriptOption(EnumScriptOptions.DriIncludeSystemNames);

	public static ScriptOption Default => new ScriptOption(EnumScriptOptions.PrimaryObject);

	public static ScriptOption XmlIndexes => new ScriptOption(EnumScriptOptions.XmlIndexes);

	public static ScriptOption OptimizerData => new ScriptOption(EnumScriptOptions.OptimizerData);

	public static ScriptOption NoExecuteAs => new ScriptOption(EnumScriptOptions.NoExecuteAs);

	public static ScriptOption EnforceScriptingOptions => new ScriptOption(EnumScriptOptions.EnforceScriptingOptions);

	public static ScriptOption NoMailProfileAccounts => new ScriptOption(EnumScriptOptions.NoMailProfileAccounts);

	public static ScriptOption NoMailProfilePrincipals => new ScriptOption(EnumScriptOptions.NoMailProfilePrincipals);

	public static ScriptOption DriWithNoCheck => new ScriptOption(EnumScriptOptions.DriWithNoCheck);

	public static ScriptOption DriAllKeys => new ScriptOption(EnumScriptOptions.DriAllKeys);

	public static ScriptOption Indexes => new ScriptOption(EnumScriptOptions.Indexes);

	public static ScriptOption DriIndexes => new ScriptOption(EnumScriptOptions.DriIndexes);

	public static ScriptOption DriAllConstraints => new ScriptOption(EnumScriptOptions.DriAllConstraints);

	public static ScriptOption DriAll => new ScriptOption(EnumScriptOptions.DriAll);

	public static ScriptOption NoVardecimal => new ScriptOption(EnumScriptOptions.NoVardecimal);

	public static ScriptOption IncludeDatabaseRoleMemberships => new ScriptOption(EnumScriptOptions.IncludeDatabaseRoleMemberships);

	public static ScriptOption ChangeTracking => new ScriptOption(EnumScriptOptions.ChangeTracking);

	public static ScriptOption ScriptOwner => new ScriptOption(EnumScriptOptions.ScriptOwner);

	public static ScriptOption IncludeFullTextCatalogRootPath => new ScriptOption(EnumScriptOptions.IncludeFullTextCatalogRootPath);

	public static ScriptOption ScriptSchema => new ScriptOption(EnumScriptOptions.ScriptSchema);

	public static ScriptOption ScriptData => new ScriptOption(EnumScriptOptions.ScriptData);

	public static ScriptOption ScriptBatchTerminator => new ScriptOption(EnumScriptOptions.ScriptBatchTerminator);

	public static ScriptOption ScriptDataCompression => new ScriptOption(EnumScriptOptions.ScriptDataCompression);

	internal ScriptOption(EnumScriptOptions optionValue)
	{
		m_value = optionValue;
	}

	public static implicit operator ScriptingOptions(ScriptOption scriptOption)
	{
		return new ScriptingOptions(scriptOption);
	}

	public static ScriptingOptions operator |(ScriptOption leftOption, ScriptOption rightOption)
	{
		return new ScriptingOptions(leftOption.Value, rightOption.Value);
	}

	public static ScriptingOptions BitwiseOr(ScriptOption leftOption, ScriptOption rightOption)
	{
		return leftOption | rightOption;
	}

	public static ScriptingOptions operator +(ScriptOption leftOption, ScriptOption rightOption)
	{
		return new ScriptingOptions(leftOption.Value, rightOption.Value);
	}

	public static ScriptingOptions Add(ScriptOption leftOption, ScriptOption rightOption)
	{
		return leftOption + rightOption;
	}

	public override string ToString()
	{
		return m_value.ToString();
	}

	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}
		return m_value == ((ScriptOption)obj).m_value;
	}

	public override int GetHashCode()
	{
		return m_value.GetHashCode();
	}
}
