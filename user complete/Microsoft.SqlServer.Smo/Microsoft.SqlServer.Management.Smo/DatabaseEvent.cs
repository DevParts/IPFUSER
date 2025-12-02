namespace Microsoft.SqlServer.Management.Smo;

public sealed class DatabaseEvent
{
	private DatabaseEventValues m_value;

	internal DatabaseEventValues Value => m_value;

	public static DatabaseEvent AddRoleMember => new DatabaseEvent(DatabaseEventValues.AddRoleMember);

	public static DatabaseEvent AddSensitivityClassification => new DatabaseEvent(DatabaseEventValues.AddSensitivityClassification);

	public static DatabaseEvent AddSignature => new DatabaseEvent(DatabaseEventValues.AddSignature);

	public static DatabaseEvent AddSignatureSchemaObject => new DatabaseEvent(DatabaseEventValues.AddSignatureSchemaObject);

	public static DatabaseEvent AlterApplicationRole => new DatabaseEvent(DatabaseEventValues.AlterApplicationRole);

	public static DatabaseEvent AlterAssembly => new DatabaseEvent(DatabaseEventValues.AlterAssembly);

	public static DatabaseEvent AlterAsymmetricKey => new DatabaseEvent(DatabaseEventValues.AlterAsymmetricKey);

	public static DatabaseEvent AlterAudit => new DatabaseEvent(DatabaseEventValues.AlterAudit);

	public static DatabaseEvent AlterAuthorizationDatabase => new DatabaseEvent(DatabaseEventValues.AlterAuthorizationDatabase);

	public static DatabaseEvent AlterBrokerPriority => new DatabaseEvent(DatabaseEventValues.AlterBrokerPriority);

	public static DatabaseEvent AlterCertificate => new DatabaseEvent(DatabaseEventValues.AlterCertificate);

	public static DatabaseEvent AlterColumnEncryptionKey => new DatabaseEvent(DatabaseEventValues.AlterColumnEncryptionKey);

	public static DatabaseEvent AlterDatabaseAuditSpecification => new DatabaseEvent(DatabaseEventValues.AlterDatabaseAuditSpecification);

	public static DatabaseEvent AlterDatabaseEncryptionKey => new DatabaseEvent(DatabaseEventValues.AlterDatabaseEncryptionKey);

	public static DatabaseEvent AlterDatabaseScopedConfiguration => new DatabaseEvent(DatabaseEventValues.AlterDatabaseScopedConfiguration);

	public static DatabaseEvent AlterExtendedProperty => new DatabaseEvent(DatabaseEventValues.AlterExtendedProperty);

	public static DatabaseEvent AlterExternalLibrary => new DatabaseEvent(DatabaseEventValues.AlterExternalLibrary);

	public static DatabaseEvent AlterFulltextCatalog => new DatabaseEvent(DatabaseEventValues.AlterFulltextCatalog);

	public static DatabaseEvent AlterFulltextIndex => new DatabaseEvent(DatabaseEventValues.AlterFulltextIndex);

	public static DatabaseEvent AlterFulltextStoplist => new DatabaseEvent(DatabaseEventValues.AlterFulltextStoplist);

	public static DatabaseEvent AlterFunction => new DatabaseEvent(DatabaseEventValues.AlterFunction);

	public static DatabaseEvent AlterIndex => new DatabaseEvent(DatabaseEventValues.AlterIndex);

	public static DatabaseEvent AlterMasterKey => new DatabaseEvent(DatabaseEventValues.AlterMasterKey);

	public static DatabaseEvent AlterMessageType => new DatabaseEvent(DatabaseEventValues.AlterMessageType);

	public static DatabaseEvent AlterPartitionFunction => new DatabaseEvent(DatabaseEventValues.AlterPartitionFunction);

	public static DatabaseEvent AlterPartitionScheme => new DatabaseEvent(DatabaseEventValues.AlterPartitionScheme);

	public static DatabaseEvent AlterPlanGuide => new DatabaseEvent(DatabaseEventValues.AlterPlanGuide);

	public static DatabaseEvent AlterProcedure => new DatabaseEvent(DatabaseEventValues.AlterProcedure);

	public static DatabaseEvent AlterQueue => new DatabaseEvent(DatabaseEventValues.AlterQueue);

	public static DatabaseEvent AlterRemoteServiceBinding => new DatabaseEvent(DatabaseEventValues.AlterRemoteServiceBinding);

	public static DatabaseEvent AlterRole => new DatabaseEvent(DatabaseEventValues.AlterRole);

	public static DatabaseEvent AlterRoute => new DatabaseEvent(DatabaseEventValues.AlterRoute);

	public static DatabaseEvent AlterSchema => new DatabaseEvent(DatabaseEventValues.AlterSchema);

	public static DatabaseEvent AlterSearchPropertyList => new DatabaseEvent(DatabaseEventValues.AlterSearchPropertyList);

	public static DatabaseEvent AlterSecurityPolicy => new DatabaseEvent(DatabaseEventValues.AlterSecurityPolicy);

	public static DatabaseEvent AlterSequence => new DatabaseEvent(DatabaseEventValues.AlterSequence);

	public static DatabaseEvent AlterService => new DatabaseEvent(DatabaseEventValues.AlterService);

	public static DatabaseEvent AlterSymmetricKey => new DatabaseEvent(DatabaseEventValues.AlterSymmetricKey);

	public static DatabaseEvent AlterTable => new DatabaseEvent(DatabaseEventValues.AlterTable);

	public static DatabaseEvent AlterTrigger => new DatabaseEvent(DatabaseEventValues.AlterTrigger);

	public static DatabaseEvent AlterUser => new DatabaseEvent(DatabaseEventValues.AlterUser);

	public static DatabaseEvent AlterView => new DatabaseEvent(DatabaseEventValues.AlterView);

	public static DatabaseEvent AlterXmlSchemaCollection => new DatabaseEvent(DatabaseEventValues.AlterXmlSchemaCollection);

	public static DatabaseEvent BindDefault => new DatabaseEvent(DatabaseEventValues.BindDefault);

	public static DatabaseEvent BindRule => new DatabaseEvent(DatabaseEventValues.BindRule);

	public static DatabaseEvent CreateApplicationRole => new DatabaseEvent(DatabaseEventValues.CreateApplicationRole);

	public static DatabaseEvent CreateAssembly => new DatabaseEvent(DatabaseEventValues.CreateAssembly);

	public static DatabaseEvent CreateAsymmetricKey => new DatabaseEvent(DatabaseEventValues.CreateAsymmetricKey);

	public static DatabaseEvent CreateAudit => new DatabaseEvent(DatabaseEventValues.CreateAudit);

	public static DatabaseEvent CreateBrokerPriority => new DatabaseEvent(DatabaseEventValues.CreateBrokerPriority);

	public static DatabaseEvent CreateCertificate => new DatabaseEvent(DatabaseEventValues.CreateCertificate);

	public static DatabaseEvent CreateColumnEncryptionKey => new DatabaseEvent(DatabaseEventValues.CreateColumnEncryptionKey);

	public static DatabaseEvent CreateColumnMasterKey => new DatabaseEvent(DatabaseEventValues.CreateColumnMasterKey);

	public static DatabaseEvent CreateContract => new DatabaseEvent(DatabaseEventValues.CreateContract);

	public static DatabaseEvent CreateDatabaseAuditSpecification => new DatabaseEvent(DatabaseEventValues.CreateDatabaseAuditSpecification);

	public static DatabaseEvent CreateDatabaseEncryptionKey => new DatabaseEvent(DatabaseEventValues.CreateDatabaseEncryptionKey);

	public static DatabaseEvent CreateDefault => new DatabaseEvent(DatabaseEventValues.CreateDefault);

	public static DatabaseEvent CreateEventNotification => new DatabaseEvent(DatabaseEventValues.CreateEventNotification);

	public static DatabaseEvent CreateExtendedProperty => new DatabaseEvent(DatabaseEventValues.CreateExtendedProperty);

	public static DatabaseEvent CreateExternalLibrary => new DatabaseEvent(DatabaseEventValues.CreateExternalLibrary);

	public static DatabaseEvent CreateFulltextCatalog => new DatabaseEvent(DatabaseEventValues.CreateFulltextCatalog);

	public static DatabaseEvent CreateFulltextIndex => new DatabaseEvent(DatabaseEventValues.CreateFulltextIndex);

	public static DatabaseEvent CreateFulltextStoplist => new DatabaseEvent(DatabaseEventValues.CreateFulltextStoplist);

	public static DatabaseEvent CreateFunction => new DatabaseEvent(DatabaseEventValues.CreateFunction);

	public static DatabaseEvent CreateIndex => new DatabaseEvent(DatabaseEventValues.CreateIndex);

	public static DatabaseEvent CreateMasterKey => new DatabaseEvent(DatabaseEventValues.CreateMasterKey);

	public static DatabaseEvent CreateMessageType => new DatabaseEvent(DatabaseEventValues.CreateMessageType);

	public static DatabaseEvent CreatePartitionFunction => new DatabaseEvent(DatabaseEventValues.CreatePartitionFunction);

	public static DatabaseEvent CreatePartitionScheme => new DatabaseEvent(DatabaseEventValues.CreatePartitionScheme);

	public static DatabaseEvent CreatePlanGuide => new DatabaseEvent(DatabaseEventValues.CreatePlanGuide);

	public static DatabaseEvent CreateProcedure => new DatabaseEvent(DatabaseEventValues.CreateProcedure);

	public static DatabaseEvent CreateQueue => new DatabaseEvent(DatabaseEventValues.CreateQueue);

	public static DatabaseEvent CreateRemoteServiceBinding => new DatabaseEvent(DatabaseEventValues.CreateRemoteServiceBinding);

	public static DatabaseEvent CreateRole => new DatabaseEvent(DatabaseEventValues.CreateRole);

	public static DatabaseEvent CreateRoute => new DatabaseEvent(DatabaseEventValues.CreateRoute);

	public static DatabaseEvent CreateRule => new DatabaseEvent(DatabaseEventValues.CreateRule);

	public static DatabaseEvent CreateSchema => new DatabaseEvent(DatabaseEventValues.CreateSchema);

	public static DatabaseEvent CreateSearchPropertyList => new DatabaseEvent(DatabaseEventValues.CreateSearchPropertyList);

	public static DatabaseEvent CreateSecurityPolicy => new DatabaseEvent(DatabaseEventValues.CreateSecurityPolicy);

	public static DatabaseEvent CreateSequence => new DatabaseEvent(DatabaseEventValues.CreateSequence);

	public static DatabaseEvent CreateService => new DatabaseEvent(DatabaseEventValues.CreateService);

	public static DatabaseEvent CreateSpatialIndex => new DatabaseEvent(DatabaseEventValues.CreateSpatialIndex);

	public static DatabaseEvent CreateStatistics => new DatabaseEvent(DatabaseEventValues.CreateStatistics);

	public static DatabaseEvent CreateSymmetricKey => new DatabaseEvent(DatabaseEventValues.CreateSymmetricKey);

	public static DatabaseEvent CreateSynonym => new DatabaseEvent(DatabaseEventValues.CreateSynonym);

	public static DatabaseEvent CreateTable => new DatabaseEvent(DatabaseEventValues.CreateTable);

	public static DatabaseEvent CreateTrigger => new DatabaseEvent(DatabaseEventValues.CreateTrigger);

	public static DatabaseEvent CreateType => new DatabaseEvent(DatabaseEventValues.CreateType);

	public static DatabaseEvent CreateUser => new DatabaseEvent(DatabaseEventValues.CreateUser);

	public static DatabaseEvent CreateView => new DatabaseEvent(DatabaseEventValues.CreateView);

	public static DatabaseEvent CreateXmlIndex => new DatabaseEvent(DatabaseEventValues.CreateXmlIndex);

	public static DatabaseEvent CreateXmlSchemaCollection => new DatabaseEvent(DatabaseEventValues.CreateXmlSchemaCollection);

	public static DatabaseEvent DenyDatabase => new DatabaseEvent(DatabaseEventValues.DenyDatabase);

	public static DatabaseEvent DropApplicationRole => new DatabaseEvent(DatabaseEventValues.DropApplicationRole);

	public static DatabaseEvent DropAssembly => new DatabaseEvent(DatabaseEventValues.DropAssembly);

	public static DatabaseEvent DropAsymmetricKey => new DatabaseEvent(DatabaseEventValues.DropAsymmetricKey);

	public static DatabaseEvent DropAudit => new DatabaseEvent(DatabaseEventValues.DropAudit);

	public static DatabaseEvent DropBrokerPriority => new DatabaseEvent(DatabaseEventValues.DropBrokerPriority);

	public static DatabaseEvent DropCertificate => new DatabaseEvent(DatabaseEventValues.DropCertificate);

	public static DatabaseEvent DropColumnEncryptionKey => new DatabaseEvent(DatabaseEventValues.DropColumnEncryptionKey);

	public static DatabaseEvent DropColumnMasterKey => new DatabaseEvent(DatabaseEventValues.DropColumnMasterKey);

	public static DatabaseEvent DropContract => new DatabaseEvent(DatabaseEventValues.DropContract);

	public static DatabaseEvent DropDatabase => new DatabaseEvent(DatabaseEventValues.DropDatabase);

	public static DatabaseEvent DropDatabaseAuditSpecification => new DatabaseEvent(DatabaseEventValues.DropDatabaseAuditSpecification);

	public static DatabaseEvent DropDatabaseEncryptionKey => new DatabaseEvent(DatabaseEventValues.DropDatabaseEncryptionKey);

	public static DatabaseEvent DropDefault => new DatabaseEvent(DatabaseEventValues.DropDefault);

	public static DatabaseEvent DropEventNotification => new DatabaseEvent(DatabaseEventValues.DropEventNotification);

	public static DatabaseEvent DropExtendedProperty => new DatabaseEvent(DatabaseEventValues.DropExtendedProperty);

	public static DatabaseEvent DropExternalLibrary => new DatabaseEvent(DatabaseEventValues.DropExternalLibrary);

	public static DatabaseEvent DropFulltextCatalog => new DatabaseEvent(DatabaseEventValues.DropFulltextCatalog);

	public static DatabaseEvent DropFulltextIndex => new DatabaseEvent(DatabaseEventValues.DropFulltextIndex);

	public static DatabaseEvent DropFulltextStoplist => new DatabaseEvent(DatabaseEventValues.DropFulltextStoplist);

	public static DatabaseEvent DropFunction => new DatabaseEvent(DatabaseEventValues.DropFunction);

	public static DatabaseEvent DropIndex => new DatabaseEvent(DatabaseEventValues.DropIndex);

	public static DatabaseEvent DropMasterKey => new DatabaseEvent(DatabaseEventValues.DropMasterKey);

	public static DatabaseEvent DropMessageType => new DatabaseEvent(DatabaseEventValues.DropMessageType);

	public static DatabaseEvent DropPartitionFunction => new DatabaseEvent(DatabaseEventValues.DropPartitionFunction);

	public static DatabaseEvent DropPartitionScheme => new DatabaseEvent(DatabaseEventValues.DropPartitionScheme);

	public static DatabaseEvent DropPlanGuide => new DatabaseEvent(DatabaseEventValues.DropPlanGuide);

	public static DatabaseEvent DropProcedure => new DatabaseEvent(DatabaseEventValues.DropProcedure);

	public static DatabaseEvent DropQueue => new DatabaseEvent(DatabaseEventValues.DropQueue);

	public static DatabaseEvent DropRemoteServiceBinding => new DatabaseEvent(DatabaseEventValues.DropRemoteServiceBinding);

	public static DatabaseEvent DropRole => new DatabaseEvent(DatabaseEventValues.DropRole);

	public static DatabaseEvent DropRoleMember => new DatabaseEvent(DatabaseEventValues.DropRoleMember);

	public static DatabaseEvent DropRoute => new DatabaseEvent(DatabaseEventValues.DropRoute);

	public static DatabaseEvent DropRule => new DatabaseEvent(DatabaseEventValues.DropRule);

	public static DatabaseEvent DropSchema => new DatabaseEvent(DatabaseEventValues.DropSchema);

	public static DatabaseEvent DropSearchPropertyList => new DatabaseEvent(DatabaseEventValues.DropSearchPropertyList);

	public static DatabaseEvent DropSecurityPolicy => new DatabaseEvent(DatabaseEventValues.DropSecurityPolicy);

	public static DatabaseEvent DropSensitivityClassification => new DatabaseEvent(DatabaseEventValues.DropSensitivityClassification);

	public static DatabaseEvent DropSequence => new DatabaseEvent(DatabaseEventValues.DropSequence);

	public static DatabaseEvent DropService => new DatabaseEvent(DatabaseEventValues.DropService);

	public static DatabaseEvent DropSignature => new DatabaseEvent(DatabaseEventValues.DropSignature);

	public static DatabaseEvent DropSignatureSchemaObject => new DatabaseEvent(DatabaseEventValues.DropSignatureSchemaObject);

	public static DatabaseEvent DropStatistics => new DatabaseEvent(DatabaseEventValues.DropStatistics);

	public static DatabaseEvent DropSymmetricKey => new DatabaseEvent(DatabaseEventValues.DropSymmetricKey);

	public static DatabaseEvent DropSynonym => new DatabaseEvent(DatabaseEventValues.DropSynonym);

	public static DatabaseEvent DropTable => new DatabaseEvent(DatabaseEventValues.DropTable);

	public static DatabaseEvent DropTrigger => new DatabaseEvent(DatabaseEventValues.DropTrigger);

	public static DatabaseEvent DropType => new DatabaseEvent(DatabaseEventValues.DropType);

	public static DatabaseEvent DropUser => new DatabaseEvent(DatabaseEventValues.DropUser);

	public static DatabaseEvent DropView => new DatabaseEvent(DatabaseEventValues.DropView);

	public static DatabaseEvent DropXmlSchemaCollection => new DatabaseEvent(DatabaseEventValues.DropXmlSchemaCollection);

	public static DatabaseEvent GrantDatabase => new DatabaseEvent(DatabaseEventValues.GrantDatabase);

	public static DatabaseEvent Rename => new DatabaseEvent(DatabaseEventValues.Rename);

	public static DatabaseEvent RevokeDatabase => new DatabaseEvent(DatabaseEventValues.RevokeDatabase);

	public static DatabaseEvent UnbindDefault => new DatabaseEvent(DatabaseEventValues.UnbindDefault);

	public static DatabaseEvent UnbindRule => new DatabaseEvent(DatabaseEventValues.UnbindRule);

	public static DatabaseEvent UpdateStatistics => new DatabaseEvent(DatabaseEventValues.UpdateStatistics);

	internal DatabaseEvent(DatabaseEventValues eventValue)
	{
		m_value = eventValue;
	}

	public static implicit operator DatabaseEventSet(DatabaseEvent eventValue)
	{
		return new DatabaseEventSet(eventValue);
	}

	public static DatabaseEventSet operator +(DatabaseEvent eventLeft, DatabaseEvent eventRight)
	{
		DatabaseEventSet databaseEventSet = new DatabaseEventSet(eventLeft);
		databaseEventSet.SetBit(eventRight);
		return databaseEventSet;
	}

	public static DatabaseEventSet Add(DatabaseEvent eventLeft, DatabaseEvent eventRight)
	{
		return eventLeft + eventRight;
	}

	public static DatabaseEventSet operator |(DatabaseEvent eventLeft, DatabaseEvent eventRight)
	{
		DatabaseEventSet databaseEventSet = new DatabaseEventSet(eventLeft);
		databaseEventSet.SetBit(eventRight);
		return databaseEventSet;
	}

	public static DatabaseEventSet BitwiseOr(DatabaseEvent eventLeft, DatabaseEvent eventRight)
	{
		return eventLeft | eventRight;
	}

	public override string ToString()
	{
		return m_value.ToString();
	}

	public static bool operator ==(DatabaseEvent a, DatabaseEvent b)
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

	public static bool operator !=(DatabaseEvent a, DatabaseEvent b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return this == obj as DatabaseEvent;
	}

	public override int GetHashCode()
	{
		return m_value.GetHashCode();
	}
}
