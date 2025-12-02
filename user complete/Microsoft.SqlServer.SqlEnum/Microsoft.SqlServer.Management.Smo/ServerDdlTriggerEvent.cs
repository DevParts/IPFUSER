namespace Microsoft.SqlServer.Management.Smo;

public sealed class ServerDdlTriggerEvent
{
	private ServerDdlTriggerEventValues m_value;

	internal ServerDdlTriggerEventValues Value => m_value;

	public static ServerDdlTriggerEvent AddRoleMember => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AddRoleMember);

	public static ServerDdlTriggerEvent AddSensitivityClassification => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AddSensitivityClassification);

	public static ServerDdlTriggerEvent AddServerRoleMember => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AddServerRoleMember);

	public static ServerDdlTriggerEvent AddSignature => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AddSignature);

	public static ServerDdlTriggerEvent AddSignatureSchemaObject => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AddSignatureSchemaObject);

	public static ServerDdlTriggerEvent AlterApplicationRole => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterApplicationRole);

	public static ServerDdlTriggerEvent AlterAssembly => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterAssembly);

	public static ServerDdlTriggerEvent AlterAsymmetricKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterAsymmetricKey);

	public static ServerDdlTriggerEvent AlterAudit => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterAudit);

	public static ServerDdlTriggerEvent AlterAuthorizationDatabase => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterAuthorizationDatabase);

	public static ServerDdlTriggerEvent AlterAuthorizationServer => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterAuthorizationServer);

	public static ServerDdlTriggerEvent AlterAvailabilityGroup => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterAvailabilityGroup);

	public static ServerDdlTriggerEvent AlterBrokerPriority => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterBrokerPriority);

	public static ServerDdlTriggerEvent AlterCertificate => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterCertificate);

	public static ServerDdlTriggerEvent AlterColumnEncryptionKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterColumnEncryptionKey);

	public static ServerDdlTriggerEvent AlterCredential => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterCredential);

	public static ServerDdlTriggerEvent AlterCryptographicProvider => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterCryptographicProvider);

	public static ServerDdlTriggerEvent AlterDatabase => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterDatabase);

	public static ServerDdlTriggerEvent AlterDatabaseAuditSpecification => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterDatabaseAuditSpecification);

	public static ServerDdlTriggerEvent AlterDatabaseEncryptionKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterDatabaseEncryptionKey);

	public static ServerDdlTriggerEvent AlterDatabaseScopedConfiguration => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterDatabaseScopedConfiguration);

	public static ServerDdlTriggerEvent AlterEndpoint => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterEndpoint);

	public static ServerDdlTriggerEvent AlterEventSession => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterEventSession);

	public static ServerDdlTriggerEvent AlterExtendedProperty => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterExtendedProperty);

	public static ServerDdlTriggerEvent AlterExternalLibrary => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterExternalLibrary);

	public static ServerDdlTriggerEvent AlterExternalResourcePool => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterExternalResourcePool);

	public static ServerDdlTriggerEvent AlterFulltextCatalog => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterFulltextCatalog);

	public static ServerDdlTriggerEvent AlterFulltextIndex => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterFulltextIndex);

	public static ServerDdlTriggerEvent AlterFulltextStoplist => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterFulltextStoplist);

	public static ServerDdlTriggerEvent AlterFunction => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterFunction);

	public static ServerDdlTriggerEvent AlterIndex => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterIndex);

	public static ServerDdlTriggerEvent AlterInstance => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterInstance);

	public static ServerDdlTriggerEvent AlterLinkedServer => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterLinkedServer);

	public static ServerDdlTriggerEvent AlterLogin => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterLogin);

	public static ServerDdlTriggerEvent AlterMasterKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterMasterKey);

	public static ServerDdlTriggerEvent AlterMessage => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterMessage);

	public static ServerDdlTriggerEvent AlterMessageType => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterMessageType);

	public static ServerDdlTriggerEvent AlterPartitionFunction => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterPartitionFunction);

	public static ServerDdlTriggerEvent AlterPartitionScheme => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterPartitionScheme);

	public static ServerDdlTriggerEvent AlterPlanGuide => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterPlanGuide);

	public static ServerDdlTriggerEvent AlterProcedure => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterProcedure);

	public static ServerDdlTriggerEvent AlterQueue => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterQueue);

	public static ServerDdlTriggerEvent AlterRemoteServer => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterRemoteServer);

	public static ServerDdlTriggerEvent AlterRemoteServiceBinding => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterRemoteServiceBinding);

	public static ServerDdlTriggerEvent AlterResourceGovernorConfig => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterResourceGovernorConfig);

	public static ServerDdlTriggerEvent AlterResourcePool => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterResourcePool);

	public static ServerDdlTriggerEvent AlterRole => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterRole);

	public static ServerDdlTriggerEvent AlterRoute => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterRoute);

	public static ServerDdlTriggerEvent AlterSchema => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterSchema);

	public static ServerDdlTriggerEvent AlterSearchPropertyList => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterSearchPropertyList);

	public static ServerDdlTriggerEvent AlterSecurityPolicy => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterSecurityPolicy);

	public static ServerDdlTriggerEvent AlterSequence => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterSequence);

	public static ServerDdlTriggerEvent AlterServerAudit => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterServerAudit);

	public static ServerDdlTriggerEvent AlterServerAuditSpecification => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterServerAuditSpecification);

	public static ServerDdlTriggerEvent AlterServerRole => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterServerRole);

	public static ServerDdlTriggerEvent AlterService => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterService);

	public static ServerDdlTriggerEvent AlterServiceMasterKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterServiceMasterKey);

	public static ServerDdlTriggerEvent AlterSymmetricKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterSymmetricKey);

	public static ServerDdlTriggerEvent AlterTable => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterTable);

	public static ServerDdlTriggerEvent AlterTrigger => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterTrigger);

	public static ServerDdlTriggerEvent AlterUser => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterUser);

	public static ServerDdlTriggerEvent AlterView => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterView);

	public static ServerDdlTriggerEvent AlterWorkloadGroup => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterWorkloadGroup);

	public static ServerDdlTriggerEvent AlterXmlSchemaCollection => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.AlterXmlSchemaCollection);

	public static ServerDdlTriggerEvent BindDefault => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.BindDefault);

	public static ServerDdlTriggerEvent BindRule => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.BindRule);

	public static ServerDdlTriggerEvent CreateApplicationRole => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateApplicationRole);

	public static ServerDdlTriggerEvent CreateAssembly => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateAssembly);

	public static ServerDdlTriggerEvent CreateAsymmetricKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateAsymmetricKey);

	public static ServerDdlTriggerEvent CreateAudit => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateAudit);

	public static ServerDdlTriggerEvent CreateAvailabilityGroup => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateAvailabilityGroup);

	public static ServerDdlTriggerEvent CreateBrokerPriority => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateBrokerPriority);

	public static ServerDdlTriggerEvent CreateCertificate => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateCertificate);

	public static ServerDdlTriggerEvent CreateColumnEncryptionKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateColumnEncryptionKey);

	public static ServerDdlTriggerEvent CreateColumnMasterKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateColumnMasterKey);

	public static ServerDdlTriggerEvent CreateContract => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateContract);

	public static ServerDdlTriggerEvent CreateCredential => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateCredential);

	public static ServerDdlTriggerEvent CreateCryptographicProvider => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateCryptographicProvider);

	public static ServerDdlTriggerEvent CreateDatabase => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateDatabase);

	public static ServerDdlTriggerEvent CreateDatabaseAuditSpecification => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateDatabaseAuditSpecification);

	public static ServerDdlTriggerEvent CreateDatabaseEncryptionKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateDatabaseEncryptionKey);

	public static ServerDdlTriggerEvent CreateDefault => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateDefault);

	public static ServerDdlTriggerEvent CreateEndpoint => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateEndpoint);

	public static ServerDdlTriggerEvent CreateEventNotification => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateEventNotification);

	public static ServerDdlTriggerEvent CreateEventSession => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateEventSession);

	public static ServerDdlTriggerEvent CreateExtendedProcedure => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateExtendedProcedure);

	public static ServerDdlTriggerEvent CreateExtendedProperty => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateExtendedProperty);

	public static ServerDdlTriggerEvent CreateExternalLibrary => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateExternalLibrary);

	public static ServerDdlTriggerEvent CreateExternalResourcePool => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateExternalResourcePool);

	public static ServerDdlTriggerEvent CreateFulltextCatalog => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateFulltextCatalog);

	public static ServerDdlTriggerEvent CreateFulltextIndex => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateFulltextIndex);

	public static ServerDdlTriggerEvent CreateFulltextStoplist => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateFulltextStoplist);

	public static ServerDdlTriggerEvent CreateFunction => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateFunction);

	public static ServerDdlTriggerEvent CreateIndex => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateIndex);

	public static ServerDdlTriggerEvent CreateLinkedServer => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateLinkedServer);

	public static ServerDdlTriggerEvent CreateLinkedServerLogin => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateLinkedServerLogin);

	public static ServerDdlTriggerEvent CreateLogin => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateLogin);

	public static ServerDdlTriggerEvent CreateMasterKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateMasterKey);

	public static ServerDdlTriggerEvent CreateMessage => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateMessage);

	public static ServerDdlTriggerEvent CreateMessageType => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateMessageType);

	public static ServerDdlTriggerEvent CreatePartitionFunction => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreatePartitionFunction);

	public static ServerDdlTriggerEvent CreatePartitionScheme => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreatePartitionScheme);

	public static ServerDdlTriggerEvent CreatePlanGuide => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreatePlanGuide);

	public static ServerDdlTriggerEvent CreateProcedure => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateProcedure);

	public static ServerDdlTriggerEvent CreateQueue => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateQueue);

	public static ServerDdlTriggerEvent CreateRemoteServer => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateRemoteServer);

	public static ServerDdlTriggerEvent CreateRemoteServiceBinding => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateRemoteServiceBinding);

	public static ServerDdlTriggerEvent CreateResourcePool => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateResourcePool);

	public static ServerDdlTriggerEvent CreateRole => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateRole);

	public static ServerDdlTriggerEvent CreateRoute => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateRoute);

	public static ServerDdlTriggerEvent CreateRule => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateRule);

	public static ServerDdlTriggerEvent CreateSchema => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateSchema);

	public static ServerDdlTriggerEvent CreateSearchPropertyList => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateSearchPropertyList);

	public static ServerDdlTriggerEvent CreateSecurityPolicy => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateSecurityPolicy);

	public static ServerDdlTriggerEvent CreateSequence => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateSequence);

	public static ServerDdlTriggerEvent CreateServerAudit => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateServerAudit);

	public static ServerDdlTriggerEvent CreateServerAuditSpecification => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateServerAuditSpecification);

	public static ServerDdlTriggerEvent CreateServerRole => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateServerRole);

	public static ServerDdlTriggerEvent CreateService => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateService);

	public static ServerDdlTriggerEvent CreateSpatialIndex => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateSpatialIndex);

	public static ServerDdlTriggerEvent CreateStatistics => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateStatistics);

	public static ServerDdlTriggerEvent CreateSymmetricKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateSymmetricKey);

	public static ServerDdlTriggerEvent CreateSynonym => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateSynonym);

	public static ServerDdlTriggerEvent CreateTable => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateTable);

	public static ServerDdlTriggerEvent CreateTrigger => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateTrigger);

	public static ServerDdlTriggerEvent CreateType => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateType);

	public static ServerDdlTriggerEvent CreateUser => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateUser);

	public static ServerDdlTriggerEvent CreateView => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateView);

	public static ServerDdlTriggerEvent CreateWorkloadGroup => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateWorkloadGroup);

	public static ServerDdlTriggerEvent CreateXmlIndex => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateXmlIndex);

	public static ServerDdlTriggerEvent CreateXmlSchemaCollection => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.CreateXmlSchemaCollection);

	public static ServerDdlTriggerEvent DenyDatabase => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DenyDatabase);

	public static ServerDdlTriggerEvent DenyServer => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DenyServer);

	public static ServerDdlTriggerEvent DropApplicationRole => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropApplicationRole);

	public static ServerDdlTriggerEvent DropAssembly => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropAssembly);

	public static ServerDdlTriggerEvent DropAsymmetricKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropAsymmetricKey);

	public static ServerDdlTriggerEvent DropAudit => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropAudit);

	public static ServerDdlTriggerEvent DropAvailabilityGroup => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropAvailabilityGroup);

	public static ServerDdlTriggerEvent DropBrokerPriority => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropBrokerPriority);

	public static ServerDdlTriggerEvent DropCertificate => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropCertificate);

	public static ServerDdlTriggerEvent DropColumnEncryptionKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropColumnEncryptionKey);

	public static ServerDdlTriggerEvent DropColumnMasterKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropColumnMasterKey);

	public static ServerDdlTriggerEvent DropContract => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropContract);

	public static ServerDdlTriggerEvent DropCredential => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropCredential);

	public static ServerDdlTriggerEvent DropCryptographicProvider => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropCryptographicProvider);

	public static ServerDdlTriggerEvent DropDatabase => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropDatabase);

	public static ServerDdlTriggerEvent DropDatabaseAuditSpecification => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropDatabaseAuditSpecification);

	public static ServerDdlTriggerEvent DropDatabaseEncryptionKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropDatabaseEncryptionKey);

	public static ServerDdlTriggerEvent DropDefault => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropDefault);

	public static ServerDdlTriggerEvent DropEndpoint => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropEndpoint);

	public static ServerDdlTriggerEvent DropEventNotification => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropEventNotification);

	public static ServerDdlTriggerEvent DropEventSession => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropEventSession);

	public static ServerDdlTriggerEvent DropExtendedProcedure => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropExtendedProcedure);

	public static ServerDdlTriggerEvent DropExtendedProperty => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropExtendedProperty);

	public static ServerDdlTriggerEvent DropExternalLibrary => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropExternalLibrary);

	public static ServerDdlTriggerEvent DropExternalResourcePool => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropExternalResourcePool);

	public static ServerDdlTriggerEvent DropFulltextCatalog => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropFulltextCatalog);

	public static ServerDdlTriggerEvent DropFulltextIndex => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropFulltextIndex);

	public static ServerDdlTriggerEvent DropFulltextStoplist => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropFulltextStoplist);

	public static ServerDdlTriggerEvent DropFunction => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropFunction);

	public static ServerDdlTriggerEvent DropIndex => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropIndex);

	public static ServerDdlTriggerEvent DropLinkedServer => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropLinkedServer);

	public static ServerDdlTriggerEvent DropLinkedServerLogin => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropLinkedServerLogin);

	public static ServerDdlTriggerEvent DropLogin => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropLogin);

	public static ServerDdlTriggerEvent DropMasterKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropMasterKey);

	public static ServerDdlTriggerEvent DropMessage => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropMessage);

	public static ServerDdlTriggerEvent DropMessageType => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropMessageType);

	public static ServerDdlTriggerEvent DropPartitionFunction => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropPartitionFunction);

	public static ServerDdlTriggerEvent DropPartitionScheme => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropPartitionScheme);

	public static ServerDdlTriggerEvent DropPlanGuide => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropPlanGuide);

	public static ServerDdlTriggerEvent DropProcedure => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropProcedure);

	public static ServerDdlTriggerEvent DropQueue => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropQueue);

	public static ServerDdlTriggerEvent DropRemoteServer => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropRemoteServer);

	public static ServerDdlTriggerEvent DropRemoteServiceBinding => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropRemoteServiceBinding);

	public static ServerDdlTriggerEvent DropResourcePool => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropResourcePool);

	public static ServerDdlTriggerEvent DropRole => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropRole);

	public static ServerDdlTriggerEvent DropRoleMember => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropRoleMember);

	public static ServerDdlTriggerEvent DropRoute => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropRoute);

	public static ServerDdlTriggerEvent DropRule => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropRule);

	public static ServerDdlTriggerEvent DropSchema => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropSchema);

	public static ServerDdlTriggerEvent DropSearchPropertyList => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropSearchPropertyList);

	public static ServerDdlTriggerEvent DropSecurityPolicy => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropSecurityPolicy);

	public static ServerDdlTriggerEvent DropSensitivityClassification => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropSensitivityClassification);

	public static ServerDdlTriggerEvent DropSequence => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropSequence);

	public static ServerDdlTriggerEvent DropServerAudit => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropServerAudit);

	public static ServerDdlTriggerEvent DropServerAuditSpecification => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropServerAuditSpecification);

	public static ServerDdlTriggerEvent DropServerRole => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropServerRole);

	public static ServerDdlTriggerEvent DropServerRoleMember => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropServerRoleMember);

	public static ServerDdlTriggerEvent DropService => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropService);

	public static ServerDdlTriggerEvent DropSignature => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropSignature);

	public static ServerDdlTriggerEvent DropSignatureSchemaObject => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropSignatureSchemaObject);

	public static ServerDdlTriggerEvent DropStatistics => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropStatistics);

	public static ServerDdlTriggerEvent DropSymmetricKey => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropSymmetricKey);

	public static ServerDdlTriggerEvent DropSynonym => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropSynonym);

	public static ServerDdlTriggerEvent DropTable => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropTable);

	public static ServerDdlTriggerEvent DropTrigger => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropTrigger);

	public static ServerDdlTriggerEvent DropType => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropType);

	public static ServerDdlTriggerEvent DropUser => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropUser);

	public static ServerDdlTriggerEvent DropView => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropView);

	public static ServerDdlTriggerEvent DropWorkloadGroup => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropWorkloadGroup);

	public static ServerDdlTriggerEvent DropXmlSchemaCollection => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.DropXmlSchemaCollection);

	public static ServerDdlTriggerEvent GrantDatabase => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.GrantDatabase);

	public static ServerDdlTriggerEvent GrantServer => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.GrantServer);

	public static ServerDdlTriggerEvent Logon => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.Logon);

	public static ServerDdlTriggerEvent Rename => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.Rename);

	public static ServerDdlTriggerEvent RevokeDatabase => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.RevokeDatabase);

	public static ServerDdlTriggerEvent RevokeServer => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.RevokeServer);

	public static ServerDdlTriggerEvent UnbindDefault => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.UnbindDefault);

	public static ServerDdlTriggerEvent UnbindRule => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.UnbindRule);

	public static ServerDdlTriggerEvent UpdateStatistics => new ServerDdlTriggerEvent(ServerDdlTriggerEventValues.UpdateStatistics);

	internal ServerDdlTriggerEvent(ServerDdlTriggerEventValues eventValue)
	{
		m_value = eventValue;
	}

	public static implicit operator ServerDdlTriggerEventSet(ServerDdlTriggerEvent eventValue)
	{
		return new ServerDdlTriggerEventSet(eventValue);
	}

	public static ServerDdlTriggerEventSet operator +(ServerDdlTriggerEvent eventLeft, ServerDdlTriggerEvent eventRight)
	{
		ServerDdlTriggerEventSet serverDdlTriggerEventSet = new ServerDdlTriggerEventSet(eventLeft);
		serverDdlTriggerEventSet.SetBit(eventRight);
		return serverDdlTriggerEventSet;
	}

	public static ServerDdlTriggerEventSet Add(ServerDdlTriggerEvent eventLeft, ServerDdlTriggerEvent eventRight)
	{
		return eventLeft + eventRight;
	}

	public static ServerDdlTriggerEventSet operator |(ServerDdlTriggerEvent eventLeft, ServerDdlTriggerEvent eventRight)
	{
		ServerDdlTriggerEventSet serverDdlTriggerEventSet = new ServerDdlTriggerEventSet(eventLeft);
		serverDdlTriggerEventSet.SetBit(eventRight);
		return serverDdlTriggerEventSet;
	}

	public static ServerDdlTriggerEventSet BitwiseOr(ServerDdlTriggerEvent eventLeft, ServerDdlTriggerEvent eventRight)
	{
		return eventLeft | eventRight;
	}

	public override string ToString()
	{
		return m_value.ToString();
	}
}
