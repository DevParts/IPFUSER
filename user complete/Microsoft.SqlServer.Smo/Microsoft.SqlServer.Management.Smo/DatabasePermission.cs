namespace Microsoft.SqlServer.Management.Smo;

public sealed class DatabasePermission
{
	private DatabasePermissionSetValue m_value;

	internal DatabasePermissionSetValue Value => m_value;

	public static DatabasePermission Alter => new DatabasePermission(DatabasePermissionSetValue.Alter);

	public static DatabasePermission AlterAnyAsymmetricKey => new DatabasePermission(DatabasePermissionSetValue.AlterAnyAsymmetricKey);

	public static DatabasePermission AlterAnyApplicationRole => new DatabasePermission(DatabasePermissionSetValue.AlterAnyApplicationRole);

	public static DatabasePermission AlterAnyAssembly => new DatabasePermission(DatabasePermissionSetValue.AlterAnyAssembly);

	public static DatabasePermission AlterAnyCertificate => new DatabasePermission(DatabasePermissionSetValue.AlterAnyCertificate);

	public static DatabasePermission AlterAnyDatabaseAudit => new DatabasePermission(DatabasePermissionSetValue.AlterAnyDatabaseAudit);

	public static DatabasePermission AlterAnyDataspace => new DatabasePermission(DatabasePermissionSetValue.AlterAnyDataspace);

	public static DatabasePermission AlterAnyDatabaseEventNotification => new DatabasePermission(DatabasePermissionSetValue.AlterAnyDatabaseEventNotification);

	public static DatabasePermission AlterAnyExternalDataSource => new DatabasePermission(DatabasePermissionSetValue.AlterAnyExternalDataSource);

	public static DatabasePermission AlterAnyExternalFileFormat => new DatabasePermission(DatabasePermissionSetValue.AlterAnyExternalFileFormat);

	public static DatabasePermission AlterAnyFulltextCatalog => new DatabasePermission(DatabasePermissionSetValue.AlterAnyFulltextCatalog);

	public static DatabasePermission AlterAnyMask => new DatabasePermission(DatabasePermissionSetValue.AlterAnyMask);

	public static DatabasePermission AlterAnyMessageType => new DatabasePermission(DatabasePermissionSetValue.AlterAnyMessageType);

	public static DatabasePermission AlterAnyRole => new DatabasePermission(DatabasePermissionSetValue.AlterAnyRole);

	public static DatabasePermission AlterAnyRoute => new DatabasePermission(DatabasePermissionSetValue.AlterAnyRoute);

	public static DatabasePermission AlterAnyRemoteServiceBinding => new DatabasePermission(DatabasePermissionSetValue.AlterAnyRemoteServiceBinding);

	public static DatabasePermission AlterAnyContract => new DatabasePermission(DatabasePermissionSetValue.AlterAnyContract);

	public static DatabasePermission AlterAnySymmetricKey => new DatabasePermission(DatabasePermissionSetValue.AlterAnySymmetricKey);

	public static DatabasePermission AlterAnySchema => new DatabasePermission(DatabasePermissionSetValue.AlterAnySchema);

	public static DatabasePermission AlterAnySecurityPolicy => new DatabasePermission(DatabasePermissionSetValue.AlterAnySecurityPolicy);

	public static DatabasePermission AlterAnyService => new DatabasePermission(DatabasePermissionSetValue.AlterAnyService);

	public static DatabasePermission AlterAnyDatabaseDdlTrigger => new DatabasePermission(DatabasePermissionSetValue.AlterAnyDatabaseDdlTrigger);

	public static DatabasePermission AlterAnyUser => new DatabasePermission(DatabasePermissionSetValue.AlterAnyUser);

	public static DatabasePermission Authenticate => new DatabasePermission(DatabasePermissionSetValue.Authenticate);

	public static DatabasePermission BackupDatabase => new DatabasePermission(DatabasePermissionSetValue.BackupDatabase);

	public static DatabasePermission BackupLog => new DatabasePermission(DatabasePermissionSetValue.BackupLog);

	public static DatabasePermission Control => new DatabasePermission(DatabasePermissionSetValue.Control);

	public static DatabasePermission Connect => new DatabasePermission(DatabasePermissionSetValue.Connect);

	public static DatabasePermission ConnectReplication => new DatabasePermission(DatabasePermissionSetValue.ConnectReplication);

	public static DatabasePermission Checkpoint => new DatabasePermission(DatabasePermissionSetValue.Checkpoint);

	public static DatabasePermission CreateAggregate => new DatabasePermission(DatabasePermissionSetValue.CreateAggregate);

	public static DatabasePermission CreateAsymmetricKey => new DatabasePermission(DatabasePermissionSetValue.CreateAsymmetricKey);

	public static DatabasePermission CreateAssembly => new DatabasePermission(DatabasePermissionSetValue.CreateAssembly);

	public static DatabasePermission CreateCertificate => new DatabasePermission(DatabasePermissionSetValue.CreateCertificate);

	public static DatabasePermission CreateDatabase => new DatabasePermission(DatabasePermissionSetValue.CreateDatabase);

	public static DatabasePermission CreateDefault => new DatabasePermission(DatabasePermissionSetValue.CreateDefault);

	public static DatabasePermission CreateDatabaseDdlEventNotification => new DatabasePermission(DatabasePermissionSetValue.CreateDatabaseDdlEventNotification);

	public static DatabasePermission CreateFunction => new DatabasePermission(DatabasePermissionSetValue.CreateFunction);

	public static DatabasePermission CreateFulltextCatalog => new DatabasePermission(DatabasePermissionSetValue.CreateFulltextCatalog);

	public static DatabasePermission CreateMessageType => new DatabasePermission(DatabasePermissionSetValue.CreateMessageType);

	public static DatabasePermission CreateProcedure => new DatabasePermission(DatabasePermissionSetValue.CreateProcedure);

	public static DatabasePermission CreateQueue => new DatabasePermission(DatabasePermissionSetValue.CreateQueue);

	public static DatabasePermission CreateRole => new DatabasePermission(DatabasePermissionSetValue.CreateRole);

	public static DatabasePermission CreateRoute => new DatabasePermission(DatabasePermissionSetValue.CreateRoute);

	public static DatabasePermission CreateRule => new DatabasePermission(DatabasePermissionSetValue.CreateRule);

	public static DatabasePermission CreateRemoteServiceBinding => new DatabasePermission(DatabasePermissionSetValue.CreateRemoteServiceBinding);

	public static DatabasePermission CreateContract => new DatabasePermission(DatabasePermissionSetValue.CreateContract);

	public static DatabasePermission CreateSymmetricKey => new DatabasePermission(DatabasePermissionSetValue.CreateSymmetricKey);

	public static DatabasePermission CreateSchema => new DatabasePermission(DatabasePermissionSetValue.CreateSchema);

	public static DatabasePermission CreateSynonym => new DatabasePermission(DatabasePermissionSetValue.CreateSynonym);

	public static DatabasePermission CreateService => new DatabasePermission(DatabasePermissionSetValue.CreateService);

	public static DatabasePermission CreateTable => new DatabasePermission(DatabasePermissionSetValue.CreateTable);

	public static DatabasePermission CreateType => new DatabasePermission(DatabasePermissionSetValue.CreateType);

	public static DatabasePermission CreateView => new DatabasePermission(DatabasePermissionSetValue.CreateView);

	public static DatabasePermission CreateXmlSchemaCollection => new DatabasePermission(DatabasePermissionSetValue.CreateXmlSchemaCollection);

	public static DatabasePermission Delete => new DatabasePermission(DatabasePermissionSetValue.Delete);

	public static DatabasePermission Execute => new DatabasePermission(DatabasePermissionSetValue.Execute);

	public static DatabasePermission Insert => new DatabasePermission(DatabasePermissionSetValue.Insert);

	public static DatabasePermission References => new DatabasePermission(DatabasePermissionSetValue.References);

	public static DatabasePermission Select => new DatabasePermission(DatabasePermissionSetValue.Select);

	public static DatabasePermission Showplan => new DatabasePermission(DatabasePermissionSetValue.Showplan);

	public static DatabasePermission SubscribeQueryNotifications => new DatabasePermission(DatabasePermissionSetValue.SubscribeQueryNotifications);

	public static DatabasePermission TakeOwnership => new DatabasePermission(DatabasePermissionSetValue.TakeOwnership);

	public static DatabasePermission Unmask => new DatabasePermission(DatabasePermissionSetValue.Unmask);

	public static DatabasePermission Update => new DatabasePermission(DatabasePermissionSetValue.Update);

	public static DatabasePermission ViewAnyColumnEncryptionKeyDefinition => new DatabasePermission(DatabasePermissionSetValue.ViewAnyColumnEncryptionKeyDefinition);

	public static DatabasePermission ViewAnyColumnMasterKeyDefinition => new DatabasePermission(DatabasePermissionSetValue.ViewAnyColumnMasterKeyDefinition);

	public static DatabasePermission ViewDefinition => new DatabasePermission(DatabasePermissionSetValue.ViewDefinition);

	public static DatabasePermission ViewDatabaseState => new DatabasePermission(DatabasePermissionSetValue.ViewDatabaseState);

	internal DatabasePermission(DatabasePermissionSetValue permissionValue)
	{
		m_value = permissionValue;
	}

	public static implicit operator DatabasePermissionSet(DatabasePermission permission)
	{
		return new DatabasePermissionSet(permission);
	}

	public static DatabasePermissionSet ToDatabasePermissionSet(DatabasePermission permission)
	{
		return permission;
	}

	public static DatabasePermissionSet operator +(DatabasePermission permissionLeft, DatabasePermission permissionRight)
	{
		DatabasePermissionSet databasePermissionSet = new DatabasePermissionSet(permissionLeft);
		databasePermissionSet.SetBit(permissionRight);
		return databasePermissionSet;
	}

	public static DatabasePermissionSet Add(DatabasePermission permissionLeft, DatabasePermission permissionRight)
	{
		return permissionLeft + permissionRight;
	}

	public static DatabasePermissionSet operator |(DatabasePermission permissionLeft, DatabasePermission permissionRight)
	{
		DatabasePermissionSet databasePermissionSet = new DatabasePermissionSet(permissionLeft);
		databasePermissionSet.SetBit(permissionRight);
		return databasePermissionSet;
	}

	public static DatabasePermissionSet BitwiseOr(DatabasePermission permissionLeft, DatabasePermission permissionRight)
	{
		return permissionLeft | permissionRight;
	}

	public override int GetHashCode()
	{
		return m_value.GetHashCode();
	}

	public override bool Equals(object o)
	{
		return m_value.Equals(o);
	}

	public static bool operator ==(DatabasePermission p1, DatabasePermission p2)
	{
		return p1?.Equals(p2) ?? ((object)null == p2);
	}

	public static bool operator !=(DatabasePermission p1, DatabasePermission p2)
	{
		return !(p1 == p2);
	}
}
