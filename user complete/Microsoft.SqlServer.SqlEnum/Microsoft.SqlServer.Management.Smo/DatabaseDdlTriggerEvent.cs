namespace Microsoft.SqlServer.Management.Smo;

public sealed class DatabaseDdlTriggerEvent
{
	private DatabaseDdlTriggerEventValues m_value;

	internal DatabaseDdlTriggerEventValues Value => m_value;

	public static DatabaseDdlTriggerEvent AddRoleMember => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AddRoleMember);

	public static DatabaseDdlTriggerEvent AddSensitivityClassification => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AddSensitivityClassification);

	public static DatabaseDdlTriggerEvent AddSignature => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AddSignature);

	public static DatabaseDdlTriggerEvent AddSignatureSchemaObject => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AddSignatureSchemaObject);

	public static DatabaseDdlTriggerEvent AlterApplicationRole => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterApplicationRole);

	public static DatabaseDdlTriggerEvent AlterAssembly => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterAssembly);

	public static DatabaseDdlTriggerEvent AlterAsymmetricKey => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterAsymmetricKey);

	public static DatabaseDdlTriggerEvent AlterAudit => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterAudit);

	public static DatabaseDdlTriggerEvent AlterAuthorizationDatabase => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterAuthorizationDatabase);

	public static DatabaseDdlTriggerEvent AlterBrokerPriority => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterBrokerPriority);

	public static DatabaseDdlTriggerEvent AlterCertificate => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterCertificate);

	public static DatabaseDdlTriggerEvent AlterColumnEncryptionKey => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterColumnEncryptionKey);

	public static DatabaseDdlTriggerEvent AlterDatabaseAuditSpecification => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterDatabaseAuditSpecification);

	public static DatabaseDdlTriggerEvent AlterDatabaseEncryptionKey => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterDatabaseEncryptionKey);

	public static DatabaseDdlTriggerEvent AlterDatabaseScopedConfiguration => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterDatabaseScopedConfiguration);

	public static DatabaseDdlTriggerEvent AlterExtendedProperty => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterExtendedProperty);

	public static DatabaseDdlTriggerEvent AlterExternalLibrary => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterExternalLibrary);

	public static DatabaseDdlTriggerEvent AlterFulltextCatalog => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterFulltextCatalog);

	public static DatabaseDdlTriggerEvent AlterFulltextIndex => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterFulltextIndex);

	public static DatabaseDdlTriggerEvent AlterFulltextStoplist => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterFulltextStoplist);

	public static DatabaseDdlTriggerEvent AlterFunction => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterFunction);

	public static DatabaseDdlTriggerEvent AlterIndex => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterIndex);

	public static DatabaseDdlTriggerEvent AlterMasterKey => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterMasterKey);

	public static DatabaseDdlTriggerEvent AlterMessageType => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterMessageType);

	public static DatabaseDdlTriggerEvent AlterPartitionFunction => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterPartitionFunction);

	public static DatabaseDdlTriggerEvent AlterPartitionScheme => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterPartitionScheme);

	public static DatabaseDdlTriggerEvent AlterPlanGuide => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterPlanGuide);

	public static DatabaseDdlTriggerEvent AlterProcedure => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterProcedure);

	public static DatabaseDdlTriggerEvent AlterQueue => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterQueue);

	public static DatabaseDdlTriggerEvent AlterRemoteServiceBinding => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterRemoteServiceBinding);

	public static DatabaseDdlTriggerEvent AlterRole => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterRole);

	public static DatabaseDdlTriggerEvent AlterRoute => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterRoute);

	public static DatabaseDdlTriggerEvent AlterSchema => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterSchema);

	public static DatabaseDdlTriggerEvent AlterSearchPropertyList => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterSearchPropertyList);

	public static DatabaseDdlTriggerEvent AlterSecurityPolicy => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterSecurityPolicy);

	public static DatabaseDdlTriggerEvent AlterSequence => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterSequence);

	public static DatabaseDdlTriggerEvent AlterService => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterService);

	public static DatabaseDdlTriggerEvent AlterSymmetricKey => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterSymmetricKey);

	public static DatabaseDdlTriggerEvent AlterTable => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterTable);

	public static DatabaseDdlTriggerEvent AlterTrigger => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterTrigger);

	public static DatabaseDdlTriggerEvent AlterUser => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterUser);

	public static DatabaseDdlTriggerEvent AlterView => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterView);

	public static DatabaseDdlTriggerEvent AlterXmlSchemaCollection => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.AlterXmlSchemaCollection);

	public static DatabaseDdlTriggerEvent BindDefault => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.BindDefault);

	public static DatabaseDdlTriggerEvent BindRule => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.BindRule);

	public static DatabaseDdlTriggerEvent CreateApplicationRole => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateApplicationRole);

	public static DatabaseDdlTriggerEvent CreateAssembly => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateAssembly);

	public static DatabaseDdlTriggerEvent CreateAsymmetricKey => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateAsymmetricKey);

	public static DatabaseDdlTriggerEvent CreateAudit => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateAudit);

	public static DatabaseDdlTriggerEvent CreateBrokerPriority => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateBrokerPriority);

	public static DatabaseDdlTriggerEvent CreateCertificate => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateCertificate);

	public static DatabaseDdlTriggerEvent CreateColumnEncryptionKey => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateColumnEncryptionKey);

	public static DatabaseDdlTriggerEvent CreateColumnMasterKey => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateColumnMasterKey);

	public static DatabaseDdlTriggerEvent CreateContract => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateContract);

	public static DatabaseDdlTriggerEvent CreateDatabaseAuditSpecification => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateDatabaseAuditSpecification);

	public static DatabaseDdlTriggerEvent CreateDatabaseEncryptionKey => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateDatabaseEncryptionKey);

	public static DatabaseDdlTriggerEvent CreateDefault => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateDefault);

	public static DatabaseDdlTriggerEvent CreateEventNotification => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateEventNotification);

	public static DatabaseDdlTriggerEvent CreateExtendedProperty => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateExtendedProperty);

	public static DatabaseDdlTriggerEvent CreateExternalLibrary => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateExternalLibrary);

	public static DatabaseDdlTriggerEvent CreateFulltextCatalog => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateFulltextCatalog);

	public static DatabaseDdlTriggerEvent CreateFulltextIndex => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateFulltextIndex);

	public static DatabaseDdlTriggerEvent CreateFulltextStoplist => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateFulltextStoplist);

	public static DatabaseDdlTriggerEvent CreateFunction => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateFunction);

	public static DatabaseDdlTriggerEvent CreateIndex => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateIndex);

	public static DatabaseDdlTriggerEvent CreateMasterKey => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateMasterKey);

	public static DatabaseDdlTriggerEvent CreateMessageType => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateMessageType);

	public static DatabaseDdlTriggerEvent CreatePartitionFunction => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreatePartitionFunction);

	public static DatabaseDdlTriggerEvent CreatePartitionScheme => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreatePartitionScheme);

	public static DatabaseDdlTriggerEvent CreatePlanGuide => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreatePlanGuide);

	public static DatabaseDdlTriggerEvent CreateProcedure => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateProcedure);

	public static DatabaseDdlTriggerEvent CreateQueue => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateQueue);

	public static DatabaseDdlTriggerEvent CreateRemoteServiceBinding => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateRemoteServiceBinding);

	public static DatabaseDdlTriggerEvent CreateRole => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateRole);

	public static DatabaseDdlTriggerEvent CreateRoute => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateRoute);

	public static DatabaseDdlTriggerEvent CreateRule => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateRule);

	public static DatabaseDdlTriggerEvent CreateSchema => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateSchema);

	public static DatabaseDdlTriggerEvent CreateSearchPropertyList => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateSearchPropertyList);

	public static DatabaseDdlTriggerEvent CreateSecurityPolicy => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateSecurityPolicy);

	public static DatabaseDdlTriggerEvent CreateSequence => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateSequence);

	public static DatabaseDdlTriggerEvent CreateService => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateService);

	public static DatabaseDdlTriggerEvent CreateSpatialIndex => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateSpatialIndex);

	public static DatabaseDdlTriggerEvent CreateStatistics => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateStatistics);

	public static DatabaseDdlTriggerEvent CreateSymmetricKey => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateSymmetricKey);

	public static DatabaseDdlTriggerEvent CreateSynonym => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateSynonym);

	public static DatabaseDdlTriggerEvent CreateTable => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateTable);

	public static DatabaseDdlTriggerEvent CreateTrigger => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateTrigger);

	public static DatabaseDdlTriggerEvent CreateType => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateType);

	public static DatabaseDdlTriggerEvent CreateUser => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateUser);

	public static DatabaseDdlTriggerEvent CreateView => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateView);

	public static DatabaseDdlTriggerEvent CreateXmlIndex => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateXmlIndex);

	public static DatabaseDdlTriggerEvent CreateXmlSchemaCollection => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.CreateXmlSchemaCollection);

	public static DatabaseDdlTriggerEvent DenyDatabase => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DenyDatabase);

	public static DatabaseDdlTriggerEvent DropApplicationRole => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropApplicationRole);

	public static DatabaseDdlTriggerEvent DropAssembly => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropAssembly);

	public static DatabaseDdlTriggerEvent DropAsymmetricKey => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropAsymmetricKey);

	public static DatabaseDdlTriggerEvent DropAudit => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropAudit);

	public static DatabaseDdlTriggerEvent DropBrokerPriority => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropBrokerPriority);

	public static DatabaseDdlTriggerEvent DropCertificate => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropCertificate);

	public static DatabaseDdlTriggerEvent DropColumnEncryptionKey => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropColumnEncryptionKey);

	public static DatabaseDdlTriggerEvent DropColumnMasterKey => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropColumnMasterKey);

	public static DatabaseDdlTriggerEvent DropContract => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropContract);

	public static DatabaseDdlTriggerEvent DropDatabaseAuditSpecification => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropDatabaseAuditSpecification);

	public static DatabaseDdlTriggerEvent DropDatabaseEncryptionKey => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropDatabaseEncryptionKey);

	public static DatabaseDdlTriggerEvent DropDefault => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropDefault);

	public static DatabaseDdlTriggerEvent DropEventNotification => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropEventNotification);

	public static DatabaseDdlTriggerEvent DropExtendedProperty => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropExtendedProperty);

	public static DatabaseDdlTriggerEvent DropExternalLibrary => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropExternalLibrary);

	public static DatabaseDdlTriggerEvent DropFulltextCatalog => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropFulltextCatalog);

	public static DatabaseDdlTriggerEvent DropFulltextIndex => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropFulltextIndex);

	public static DatabaseDdlTriggerEvent DropFulltextStoplist => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropFulltextStoplist);

	public static DatabaseDdlTriggerEvent DropFunction => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropFunction);

	public static DatabaseDdlTriggerEvent DropIndex => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropIndex);

	public static DatabaseDdlTriggerEvent DropMasterKey => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropMasterKey);

	public static DatabaseDdlTriggerEvent DropMessageType => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropMessageType);

	public static DatabaseDdlTriggerEvent DropPartitionFunction => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropPartitionFunction);

	public static DatabaseDdlTriggerEvent DropPartitionScheme => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropPartitionScheme);

	public static DatabaseDdlTriggerEvent DropPlanGuide => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropPlanGuide);

	public static DatabaseDdlTriggerEvent DropProcedure => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropProcedure);

	public static DatabaseDdlTriggerEvent DropQueue => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropQueue);

	public static DatabaseDdlTriggerEvent DropRemoteServiceBinding => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropRemoteServiceBinding);

	public static DatabaseDdlTriggerEvent DropRole => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropRole);

	public static DatabaseDdlTriggerEvent DropRoleMember => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropRoleMember);

	public static DatabaseDdlTriggerEvent DropRoute => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropRoute);

	public static DatabaseDdlTriggerEvent DropRule => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropRule);

	public static DatabaseDdlTriggerEvent DropSchema => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropSchema);

	public static DatabaseDdlTriggerEvent DropSearchPropertyList => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropSearchPropertyList);

	public static DatabaseDdlTriggerEvent DropSecurityPolicy => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropSecurityPolicy);

	public static DatabaseDdlTriggerEvent DropSensitivityClassification => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropSensitivityClassification);

	public static DatabaseDdlTriggerEvent DropSequence => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropSequence);

	public static DatabaseDdlTriggerEvent DropService => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropService);

	public static DatabaseDdlTriggerEvent DropSignature => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropSignature);

	public static DatabaseDdlTriggerEvent DropSignatureSchemaObject => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropSignatureSchemaObject);

	public static DatabaseDdlTriggerEvent DropStatistics => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropStatistics);

	public static DatabaseDdlTriggerEvent DropSymmetricKey => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropSymmetricKey);

	public static DatabaseDdlTriggerEvent DropSynonym => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropSynonym);

	public static DatabaseDdlTriggerEvent DropTable => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropTable);

	public static DatabaseDdlTriggerEvent DropTrigger => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropTrigger);

	public static DatabaseDdlTriggerEvent DropType => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropType);

	public static DatabaseDdlTriggerEvent DropUser => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropUser);

	public static DatabaseDdlTriggerEvent DropView => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropView);

	public static DatabaseDdlTriggerEvent DropXmlSchemaCollection => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.DropXmlSchemaCollection);

	public static DatabaseDdlTriggerEvent GrantDatabase => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.GrantDatabase);

	public static DatabaseDdlTriggerEvent Rename => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.Rename);

	public static DatabaseDdlTriggerEvent RevokeDatabase => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.RevokeDatabase);

	public static DatabaseDdlTriggerEvent UnbindDefault => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.UnbindDefault);

	public static DatabaseDdlTriggerEvent UnbindRule => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.UnbindRule);

	public static DatabaseDdlTriggerEvent UpdateStatistics => new DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues.UpdateStatistics);

	internal DatabaseDdlTriggerEvent(DatabaseDdlTriggerEventValues eventValue)
	{
		m_value = eventValue;
	}

	public static implicit operator DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent eventValue)
	{
		return new DatabaseDdlTriggerEventSet(eventValue);
	}

	public static DatabaseDdlTriggerEventSet operator +(DatabaseDdlTriggerEvent eventLeft, DatabaseDdlTriggerEvent eventRight)
	{
		DatabaseDdlTriggerEventSet databaseDdlTriggerEventSet = new DatabaseDdlTriggerEventSet(eventLeft);
		databaseDdlTriggerEventSet.SetBit(eventRight);
		return databaseDdlTriggerEventSet;
	}

	public static DatabaseDdlTriggerEventSet Add(DatabaseDdlTriggerEvent eventLeft, DatabaseDdlTriggerEvent eventRight)
	{
		return eventLeft + eventRight;
	}

	public static DatabaseDdlTriggerEventSet operator |(DatabaseDdlTriggerEvent eventLeft, DatabaseDdlTriggerEvent eventRight)
	{
		DatabaseDdlTriggerEventSet databaseDdlTriggerEventSet = new DatabaseDdlTriggerEventSet(eventLeft);
		databaseDdlTriggerEventSet.SetBit(eventRight);
		return databaseDdlTriggerEventSet;
	}

	public static DatabaseDdlTriggerEventSet BitwiseOr(DatabaseDdlTriggerEvent eventLeft, DatabaseDdlTriggerEvent eventRight)
	{
		return eventLeft | eventRight;
	}

	public override string ToString()
	{
		return m_value.ToString();
	}
}
