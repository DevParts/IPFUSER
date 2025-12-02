namespace Microsoft.SqlServer.Management.Smo;

public sealed class ServerEvent
{
	private ServerEventValues m_value;

	internal ServerEventValues Value => m_value;

	public static ServerEvent AddRoleMember => new ServerEvent(ServerEventValues.AddRoleMember);

	public static ServerEvent AddSensitivityClassification => new ServerEvent(ServerEventValues.AddSensitivityClassification);

	public static ServerEvent AddServerRoleMember => new ServerEvent(ServerEventValues.AddServerRoleMember);

	public static ServerEvent AddSignature => new ServerEvent(ServerEventValues.AddSignature);

	public static ServerEvent AddSignatureSchemaObject => new ServerEvent(ServerEventValues.AddSignatureSchemaObject);

	public static ServerEvent AlterApplicationRole => new ServerEvent(ServerEventValues.AlterApplicationRole);

	public static ServerEvent AlterAssembly => new ServerEvent(ServerEventValues.AlterAssembly);

	public static ServerEvent AlterAsymmetricKey => new ServerEvent(ServerEventValues.AlterAsymmetricKey);

	public static ServerEvent AlterAudit => new ServerEvent(ServerEventValues.AlterAudit);

	public static ServerEvent AlterAuthorizationDatabase => new ServerEvent(ServerEventValues.AlterAuthorizationDatabase);

	public static ServerEvent AlterAuthorizationServer => new ServerEvent(ServerEventValues.AlterAuthorizationServer);

	public static ServerEvent AlterAvailabilityGroup => new ServerEvent(ServerEventValues.AlterAvailabilityGroup);

	public static ServerEvent AlterBrokerPriority => new ServerEvent(ServerEventValues.AlterBrokerPriority);

	public static ServerEvent AlterCertificate => new ServerEvent(ServerEventValues.AlterCertificate);

	public static ServerEvent AlterColumnEncryptionKey => new ServerEvent(ServerEventValues.AlterColumnEncryptionKey);

	public static ServerEvent AlterCredential => new ServerEvent(ServerEventValues.AlterCredential);

	public static ServerEvent AlterCryptographicProvider => new ServerEvent(ServerEventValues.AlterCryptographicProvider);

	public static ServerEvent AlterDatabase => new ServerEvent(ServerEventValues.AlterDatabase);

	public static ServerEvent AlterDatabaseAuditSpecification => new ServerEvent(ServerEventValues.AlterDatabaseAuditSpecification);

	public static ServerEvent AlterDatabaseEncryptionKey => new ServerEvent(ServerEventValues.AlterDatabaseEncryptionKey);

	public static ServerEvent AlterDatabaseScopedConfiguration => new ServerEvent(ServerEventValues.AlterDatabaseScopedConfiguration);

	public static ServerEvent AlterEndpoint => new ServerEvent(ServerEventValues.AlterEndpoint);

	public static ServerEvent AlterEventSession => new ServerEvent(ServerEventValues.AlterEventSession);

	public static ServerEvent AlterExtendedProperty => new ServerEvent(ServerEventValues.AlterExtendedProperty);

	public static ServerEvent AlterExternalLibrary => new ServerEvent(ServerEventValues.AlterExternalLibrary);

	public static ServerEvent AlterExternalResourcePool => new ServerEvent(ServerEventValues.AlterExternalResourcePool);

	public static ServerEvent AlterFulltextCatalog => new ServerEvent(ServerEventValues.AlterFulltextCatalog);

	public static ServerEvent AlterFulltextIndex => new ServerEvent(ServerEventValues.AlterFulltextIndex);

	public static ServerEvent AlterFulltextStoplist => new ServerEvent(ServerEventValues.AlterFulltextStoplist);

	public static ServerEvent AlterFunction => new ServerEvent(ServerEventValues.AlterFunction);

	public static ServerEvent AlterIndex => new ServerEvent(ServerEventValues.AlterIndex);

	public static ServerEvent AlterInstance => new ServerEvent(ServerEventValues.AlterInstance);

	public static ServerEvent AlterLinkedServer => new ServerEvent(ServerEventValues.AlterLinkedServer);

	public static ServerEvent AlterLogin => new ServerEvent(ServerEventValues.AlterLogin);

	public static ServerEvent AlterMasterKey => new ServerEvent(ServerEventValues.AlterMasterKey);

	public static ServerEvent AlterMessage => new ServerEvent(ServerEventValues.AlterMessage);

	public static ServerEvent AlterMessageType => new ServerEvent(ServerEventValues.AlterMessageType);

	public static ServerEvent AlterPartitionFunction => new ServerEvent(ServerEventValues.AlterPartitionFunction);

	public static ServerEvent AlterPartitionScheme => new ServerEvent(ServerEventValues.AlterPartitionScheme);

	public static ServerEvent AlterPlanGuide => new ServerEvent(ServerEventValues.AlterPlanGuide);

	public static ServerEvent AlterProcedure => new ServerEvent(ServerEventValues.AlterProcedure);

	public static ServerEvent AlterQueue => new ServerEvent(ServerEventValues.AlterQueue);

	public static ServerEvent AlterRemoteServer => new ServerEvent(ServerEventValues.AlterRemoteServer);

	public static ServerEvent AlterRemoteServiceBinding => new ServerEvent(ServerEventValues.AlterRemoteServiceBinding);

	public static ServerEvent AlterResourceGovernorConfig => new ServerEvent(ServerEventValues.AlterResourceGovernorConfig);

	public static ServerEvent AlterResourcePool => new ServerEvent(ServerEventValues.AlterResourcePool);

	public static ServerEvent AlterRole => new ServerEvent(ServerEventValues.AlterRole);

	public static ServerEvent AlterRoute => new ServerEvent(ServerEventValues.AlterRoute);

	public static ServerEvent AlterSchema => new ServerEvent(ServerEventValues.AlterSchema);

	public static ServerEvent AlterSearchPropertyList => new ServerEvent(ServerEventValues.AlterSearchPropertyList);

	public static ServerEvent AlterSecurityPolicy => new ServerEvent(ServerEventValues.AlterSecurityPolicy);

	public static ServerEvent AlterSequence => new ServerEvent(ServerEventValues.AlterSequence);

	public static ServerEvent AlterServerAudit => new ServerEvent(ServerEventValues.AlterServerAudit);

	public static ServerEvent AlterServerAuditSpecification => new ServerEvent(ServerEventValues.AlterServerAuditSpecification);

	public static ServerEvent AlterServerConfiguration => new ServerEvent(ServerEventValues.AlterServerConfiguration);

	public static ServerEvent AlterServerRole => new ServerEvent(ServerEventValues.AlterServerRole);

	public static ServerEvent AlterService => new ServerEvent(ServerEventValues.AlterService);

	public static ServerEvent AlterServiceMasterKey => new ServerEvent(ServerEventValues.AlterServiceMasterKey);

	public static ServerEvent AlterSymmetricKey => new ServerEvent(ServerEventValues.AlterSymmetricKey);

	public static ServerEvent AlterTable => new ServerEvent(ServerEventValues.AlterTable);

	public static ServerEvent AlterTrigger => new ServerEvent(ServerEventValues.AlterTrigger);

	public static ServerEvent AlterUser => new ServerEvent(ServerEventValues.AlterUser);

	public static ServerEvent AlterView => new ServerEvent(ServerEventValues.AlterView);

	public static ServerEvent AlterWorkloadGroup => new ServerEvent(ServerEventValues.AlterWorkloadGroup);

	public static ServerEvent AlterXmlSchemaCollection => new ServerEvent(ServerEventValues.AlterXmlSchemaCollection);

	public static ServerEvent BindDefault => new ServerEvent(ServerEventValues.BindDefault);

	public static ServerEvent BindRule => new ServerEvent(ServerEventValues.BindRule);

	public static ServerEvent CreateApplicationRole => new ServerEvent(ServerEventValues.CreateApplicationRole);

	public static ServerEvent CreateAssembly => new ServerEvent(ServerEventValues.CreateAssembly);

	public static ServerEvent CreateAsymmetricKey => new ServerEvent(ServerEventValues.CreateAsymmetricKey);

	public static ServerEvent CreateAudit => new ServerEvent(ServerEventValues.CreateAudit);

	public static ServerEvent CreateAvailabilityGroup => new ServerEvent(ServerEventValues.CreateAvailabilityGroup);

	public static ServerEvent CreateBrokerPriority => new ServerEvent(ServerEventValues.CreateBrokerPriority);

	public static ServerEvent CreateCertificate => new ServerEvent(ServerEventValues.CreateCertificate);

	public static ServerEvent CreateColumnEncryptionKey => new ServerEvent(ServerEventValues.CreateColumnEncryptionKey);

	public static ServerEvent CreateColumnMasterKey => new ServerEvent(ServerEventValues.CreateColumnMasterKey);

	public static ServerEvent CreateContract => new ServerEvent(ServerEventValues.CreateContract);

	public static ServerEvent CreateCredential => new ServerEvent(ServerEventValues.CreateCredential);

	public static ServerEvent CreateCryptographicProvider => new ServerEvent(ServerEventValues.CreateCryptographicProvider);

	public static ServerEvent CreateDatabase => new ServerEvent(ServerEventValues.CreateDatabase);

	public static ServerEvent CreateDatabaseAuditSpecification => new ServerEvent(ServerEventValues.CreateDatabaseAuditSpecification);

	public static ServerEvent CreateDatabaseEncryptionKey => new ServerEvent(ServerEventValues.CreateDatabaseEncryptionKey);

	public static ServerEvent CreateDefault => new ServerEvent(ServerEventValues.CreateDefault);

	public static ServerEvent CreateEndpoint => new ServerEvent(ServerEventValues.CreateEndpoint);

	public static ServerEvent CreateEventNotification => new ServerEvent(ServerEventValues.CreateEventNotification);

	public static ServerEvent CreateEventSession => new ServerEvent(ServerEventValues.CreateEventSession);

	public static ServerEvent CreateExtendedProcedure => new ServerEvent(ServerEventValues.CreateExtendedProcedure);

	public static ServerEvent CreateExtendedProperty => new ServerEvent(ServerEventValues.CreateExtendedProperty);

	public static ServerEvent CreateExternalLibrary => new ServerEvent(ServerEventValues.CreateExternalLibrary);

	public static ServerEvent CreateExternalResourcePool => new ServerEvent(ServerEventValues.CreateExternalResourcePool);

	public static ServerEvent CreateFulltextCatalog => new ServerEvent(ServerEventValues.CreateFulltextCatalog);

	public static ServerEvent CreateFulltextIndex => new ServerEvent(ServerEventValues.CreateFulltextIndex);

	public static ServerEvent CreateFulltextStoplist => new ServerEvent(ServerEventValues.CreateFulltextStoplist);

	public static ServerEvent CreateFunction => new ServerEvent(ServerEventValues.CreateFunction);

	public static ServerEvent CreateIndex => new ServerEvent(ServerEventValues.CreateIndex);

	public static ServerEvent CreateLinkedServer => new ServerEvent(ServerEventValues.CreateLinkedServer);

	public static ServerEvent CreateLinkedServerLogin => new ServerEvent(ServerEventValues.CreateLinkedServerLogin);

	public static ServerEvent CreateLogin => new ServerEvent(ServerEventValues.CreateLogin);

	public static ServerEvent CreateMasterKey => new ServerEvent(ServerEventValues.CreateMasterKey);

	public static ServerEvent CreateMessage => new ServerEvent(ServerEventValues.CreateMessage);

	public static ServerEvent CreateMessageType => new ServerEvent(ServerEventValues.CreateMessageType);

	public static ServerEvent CreatePartitionFunction => new ServerEvent(ServerEventValues.CreatePartitionFunction);

	public static ServerEvent CreatePartitionScheme => new ServerEvent(ServerEventValues.CreatePartitionScheme);

	public static ServerEvent CreatePlanGuide => new ServerEvent(ServerEventValues.CreatePlanGuide);

	public static ServerEvent CreateProcedure => new ServerEvent(ServerEventValues.CreateProcedure);

	public static ServerEvent CreateQueue => new ServerEvent(ServerEventValues.CreateQueue);

	public static ServerEvent CreateRemoteServer => new ServerEvent(ServerEventValues.CreateRemoteServer);

	public static ServerEvent CreateRemoteServiceBinding => new ServerEvent(ServerEventValues.CreateRemoteServiceBinding);

	public static ServerEvent CreateResourcePool => new ServerEvent(ServerEventValues.CreateResourcePool);

	public static ServerEvent CreateRole => new ServerEvent(ServerEventValues.CreateRole);

	public static ServerEvent CreateRoute => new ServerEvent(ServerEventValues.CreateRoute);

	public static ServerEvent CreateRule => new ServerEvent(ServerEventValues.CreateRule);

	public static ServerEvent CreateSchema => new ServerEvent(ServerEventValues.CreateSchema);

	public static ServerEvent CreateSearchPropertyList => new ServerEvent(ServerEventValues.CreateSearchPropertyList);

	public static ServerEvent CreateSecurityPolicy => new ServerEvent(ServerEventValues.CreateSecurityPolicy);

	public static ServerEvent CreateSequence => new ServerEvent(ServerEventValues.CreateSequence);

	public static ServerEvent CreateServerAudit => new ServerEvent(ServerEventValues.CreateServerAudit);

	public static ServerEvent CreateServerAuditSpecification => new ServerEvent(ServerEventValues.CreateServerAuditSpecification);

	public static ServerEvent CreateServerRole => new ServerEvent(ServerEventValues.CreateServerRole);

	public static ServerEvent CreateService => new ServerEvent(ServerEventValues.CreateService);

	public static ServerEvent CreateSpatialIndex => new ServerEvent(ServerEventValues.CreateSpatialIndex);

	public static ServerEvent CreateStatistics => new ServerEvent(ServerEventValues.CreateStatistics);

	public static ServerEvent CreateSymmetricKey => new ServerEvent(ServerEventValues.CreateSymmetricKey);

	public static ServerEvent CreateSynonym => new ServerEvent(ServerEventValues.CreateSynonym);

	public static ServerEvent CreateTable => new ServerEvent(ServerEventValues.CreateTable);

	public static ServerEvent CreateTrigger => new ServerEvent(ServerEventValues.CreateTrigger);

	public static ServerEvent CreateType => new ServerEvent(ServerEventValues.CreateType);

	public static ServerEvent CreateUser => new ServerEvent(ServerEventValues.CreateUser);

	public static ServerEvent CreateView => new ServerEvent(ServerEventValues.CreateView);

	public static ServerEvent CreateWorkloadGroup => new ServerEvent(ServerEventValues.CreateWorkloadGroup);

	public static ServerEvent CreateXmlIndex => new ServerEvent(ServerEventValues.CreateXmlIndex);

	public static ServerEvent CreateXmlSchemaCollection => new ServerEvent(ServerEventValues.CreateXmlSchemaCollection);

	public static ServerEvent DenyDatabase => new ServerEvent(ServerEventValues.DenyDatabase);

	public static ServerEvent DenyServer => new ServerEvent(ServerEventValues.DenyServer);

	public static ServerEvent DropApplicationRole => new ServerEvent(ServerEventValues.DropApplicationRole);

	public static ServerEvent DropAssembly => new ServerEvent(ServerEventValues.DropAssembly);

	public static ServerEvent DropAsymmetricKey => new ServerEvent(ServerEventValues.DropAsymmetricKey);

	public static ServerEvent DropAudit => new ServerEvent(ServerEventValues.DropAudit);

	public static ServerEvent DropAvailabilityGroup => new ServerEvent(ServerEventValues.DropAvailabilityGroup);

	public static ServerEvent DropBrokerPriority => new ServerEvent(ServerEventValues.DropBrokerPriority);

	public static ServerEvent DropCertificate => new ServerEvent(ServerEventValues.DropCertificate);

	public static ServerEvent DropColumnEncryptionKey => new ServerEvent(ServerEventValues.DropColumnEncryptionKey);

	public static ServerEvent DropColumnMasterKey => new ServerEvent(ServerEventValues.DropColumnMasterKey);

	public static ServerEvent DropContract => new ServerEvent(ServerEventValues.DropContract);

	public static ServerEvent DropCredential => new ServerEvent(ServerEventValues.DropCredential);

	public static ServerEvent DropCryptographicProvider => new ServerEvent(ServerEventValues.DropCryptographicProvider);

	public static ServerEvent DropDatabase => new ServerEvent(ServerEventValues.DropDatabase);

	public static ServerEvent DropDatabaseAuditSpecification => new ServerEvent(ServerEventValues.DropDatabaseAuditSpecification);

	public static ServerEvent DropDatabaseEncryptionKey => new ServerEvent(ServerEventValues.DropDatabaseEncryptionKey);

	public static ServerEvent DropDefault => new ServerEvent(ServerEventValues.DropDefault);

	public static ServerEvent DropEndpoint => new ServerEvent(ServerEventValues.DropEndpoint);

	public static ServerEvent DropEventNotification => new ServerEvent(ServerEventValues.DropEventNotification);

	public static ServerEvent DropEventSession => new ServerEvent(ServerEventValues.DropEventSession);

	public static ServerEvent DropExtendedProcedure => new ServerEvent(ServerEventValues.DropExtendedProcedure);

	public static ServerEvent DropExtendedProperty => new ServerEvent(ServerEventValues.DropExtendedProperty);

	public static ServerEvent DropExternalLibrary => new ServerEvent(ServerEventValues.DropExternalLibrary);

	public static ServerEvent DropExternalResourcePool => new ServerEvent(ServerEventValues.DropExternalResourcePool);

	public static ServerEvent DropFulltextCatalog => new ServerEvent(ServerEventValues.DropFulltextCatalog);

	public static ServerEvent DropFulltextIndex => new ServerEvent(ServerEventValues.DropFulltextIndex);

	public static ServerEvent DropFulltextStoplist => new ServerEvent(ServerEventValues.DropFulltextStoplist);

	public static ServerEvent DropFunction => new ServerEvent(ServerEventValues.DropFunction);

	public static ServerEvent DropIndex => new ServerEvent(ServerEventValues.DropIndex);

	public static ServerEvent DropLinkedServer => new ServerEvent(ServerEventValues.DropLinkedServer);

	public static ServerEvent DropLinkedServerLogin => new ServerEvent(ServerEventValues.DropLinkedServerLogin);

	public static ServerEvent DropLogin => new ServerEvent(ServerEventValues.DropLogin);

	public static ServerEvent DropMasterKey => new ServerEvent(ServerEventValues.DropMasterKey);

	public static ServerEvent DropMessage => new ServerEvent(ServerEventValues.DropMessage);

	public static ServerEvent DropMessageType => new ServerEvent(ServerEventValues.DropMessageType);

	public static ServerEvent DropPartitionFunction => new ServerEvent(ServerEventValues.DropPartitionFunction);

	public static ServerEvent DropPartitionScheme => new ServerEvent(ServerEventValues.DropPartitionScheme);

	public static ServerEvent DropPlanGuide => new ServerEvent(ServerEventValues.DropPlanGuide);

	public static ServerEvent DropProcedure => new ServerEvent(ServerEventValues.DropProcedure);

	public static ServerEvent DropQueue => new ServerEvent(ServerEventValues.DropQueue);

	public static ServerEvent DropRemoteServer => new ServerEvent(ServerEventValues.DropRemoteServer);

	public static ServerEvent DropRemoteServiceBinding => new ServerEvent(ServerEventValues.DropRemoteServiceBinding);

	public static ServerEvent DropResourcePool => new ServerEvent(ServerEventValues.DropResourcePool);

	public static ServerEvent DropRole => new ServerEvent(ServerEventValues.DropRole);

	public static ServerEvent DropRoleMember => new ServerEvent(ServerEventValues.DropRoleMember);

	public static ServerEvent DropRoute => new ServerEvent(ServerEventValues.DropRoute);

	public static ServerEvent DropRule => new ServerEvent(ServerEventValues.DropRule);

	public static ServerEvent DropSchema => new ServerEvent(ServerEventValues.DropSchema);

	public static ServerEvent DropSearchPropertyList => new ServerEvent(ServerEventValues.DropSearchPropertyList);

	public static ServerEvent DropSecurityPolicy => new ServerEvent(ServerEventValues.DropSecurityPolicy);

	public static ServerEvent DropSensitivityClassification => new ServerEvent(ServerEventValues.DropSensitivityClassification);

	public static ServerEvent DropSequence => new ServerEvent(ServerEventValues.DropSequence);

	public static ServerEvent DropServerAudit => new ServerEvent(ServerEventValues.DropServerAudit);

	public static ServerEvent DropServerAuditSpecification => new ServerEvent(ServerEventValues.DropServerAuditSpecification);

	public static ServerEvent DropServerRole => new ServerEvent(ServerEventValues.DropServerRole);

	public static ServerEvent DropServerRoleMember => new ServerEvent(ServerEventValues.DropServerRoleMember);

	public static ServerEvent DropService => new ServerEvent(ServerEventValues.DropService);

	public static ServerEvent DropSignature => new ServerEvent(ServerEventValues.DropSignature);

	public static ServerEvent DropSignatureSchemaObject => new ServerEvent(ServerEventValues.DropSignatureSchemaObject);

	public static ServerEvent DropStatistics => new ServerEvent(ServerEventValues.DropStatistics);

	public static ServerEvent DropSymmetricKey => new ServerEvent(ServerEventValues.DropSymmetricKey);

	public static ServerEvent DropSynonym => new ServerEvent(ServerEventValues.DropSynonym);

	public static ServerEvent DropTable => new ServerEvent(ServerEventValues.DropTable);

	public static ServerEvent DropTrigger => new ServerEvent(ServerEventValues.DropTrigger);

	public static ServerEvent DropType => new ServerEvent(ServerEventValues.DropType);

	public static ServerEvent DropUser => new ServerEvent(ServerEventValues.DropUser);

	public static ServerEvent DropView => new ServerEvent(ServerEventValues.DropView);

	public static ServerEvent DropWorkloadGroup => new ServerEvent(ServerEventValues.DropWorkloadGroup);

	public static ServerEvent DropXmlSchemaCollection => new ServerEvent(ServerEventValues.DropXmlSchemaCollection);

	public static ServerEvent GrantDatabase => new ServerEvent(ServerEventValues.GrantDatabase);

	public static ServerEvent GrantServer => new ServerEvent(ServerEventValues.GrantServer);

	public static ServerEvent Rename => new ServerEvent(ServerEventValues.Rename);

	public static ServerEvent RevokeDatabase => new ServerEvent(ServerEventValues.RevokeDatabase);

	public static ServerEvent RevokeServer => new ServerEvent(ServerEventValues.RevokeServer);

	public static ServerEvent UnbindDefault => new ServerEvent(ServerEventValues.UnbindDefault);

	public static ServerEvent UnbindRule => new ServerEvent(ServerEventValues.UnbindRule);

	public static ServerEvent UpdateStatistics => new ServerEvent(ServerEventValues.UpdateStatistics);

	internal ServerEvent(ServerEventValues eventValue)
	{
		m_value = eventValue;
	}

	public static implicit operator ServerEventSet(ServerEvent eventValue)
	{
		return new ServerEventSet(eventValue);
	}

	public static ServerEventSet operator +(ServerEvent eventLeft, ServerEvent eventRight)
	{
		ServerEventSet serverEventSet = new ServerEventSet(eventLeft);
		serverEventSet.SetBit(eventRight);
		return serverEventSet;
	}

	public static ServerEventSet Add(ServerEvent eventLeft, ServerEvent eventRight)
	{
		return eventLeft + eventRight;
	}

	public static ServerEventSet operator |(ServerEvent eventLeft, ServerEvent eventRight)
	{
		ServerEventSet serverEventSet = new ServerEventSet(eventLeft);
		serverEventSet.SetBit(eventRight);
		return serverEventSet;
	}

	public static ServerEventSet BitwiseOr(ServerEvent eventLeft, ServerEvent eventRight)
	{
		return eventLeft | eventRight;
	}

	public override string ToString()
	{
		return m_value.ToString();
	}

	public static bool operator ==(ServerEvent a, ServerEvent b)
	{
		if ((object)a == null && (object)b == null)
		{
			return true;
		}
		if ((object)a == null || (object)b == null)
		{
			return false;
		}
		return a.m_value == b.m_value;
	}

	public static bool operator !=(ServerEvent a, ServerEvent b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return this == obj as ServerEvent;
	}

	public override int GetHashCode()
	{
		return m_value.GetHashCode();
	}
}
