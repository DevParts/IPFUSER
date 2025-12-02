using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[CLSCompliant(false)]
public class DatabaseEncryptionKeyExtender : SmoObjectExtender<DatabaseEncryptionKey>, ISfcValidate
{
	private StringCollection certificateNames;

	private StringCollection asymmetricKeyNames;

	private bool reencrypt;

	private bool regenerate;

	private string certificateName = string.Empty;

	private string asymmetricKeyName = string.Empty;

	private Hashtable certificateNameBackupDateHash = new Hashtable();

	[ExtendedProperty]
	public StringCollection CertificateNames
	{
		get
		{
			if (certificateNames == null)
			{
				certificateNames = new StringCollection();
				Server serverObject = base.Parent.Parent.GetServerObject();
				if (serverObject != null)
				{
					Urn urn = "Server/Database[@Name='master']/Certificate[@ID > 256]";
					string[] fields = new string[2] { "Name", "LastBackupDate" };
					Request request = new Request(urn, fields);
					DataTable dataTable = new Enumerator().Process(serverObject.ConnectionContext, request);
					foreach (DataRow row in dataTable.Rows)
					{
						string text = row["Name"].ToString();
						if (!text.StartsWith("##MS", StringComparison.Ordinal))
						{
							certificateNames.Add(row["Name"].ToString());
							certificateNameBackupDateHash.Add(row["Name"].ToString(), row["LastBackupDate"].ToString());
						}
					}
				}
			}
			return certificateNames;
		}
	}

	[ExtendedProperty]
	public StringCollection AsymmetricKeyNames
	{
		get
		{
			if (asymmetricKeyNames == null)
			{
				asymmetricKeyNames = new StringCollection();
				Server serverObject = base.Parent.Parent.GetServerObject();
				if (serverObject != null)
				{
					Urn urn = "Server/Database[@Name='master']/AsymmetricKey";
					string[] fields = new string[1] { "Name" };
					Request request = new Request(urn, fields);
					DataTable dataTable = new Enumerator().Process(serverObject.ConnectionContext, request);
					foreach (DataRow row in dataTable.Rows)
					{
						asymmetricKeyNames.Add(row["Name"].ToString());
					}
				}
			}
			return asymmetricKeyNames;
		}
	}

	[ExtendedProperty]
	public bool DatabaseEncryptionEnabled
	{
		get
		{
			return base.Parent.Parent?.EncryptionEnabled ?? false;
		}
		set
		{
			Database database = base.Parent.Parent;
			if (database != null)
			{
				database.EncryptionEnabled = value;
			}
		}
	}

	[ExtendedProperty]
	public SqlSmoState State => base.Parent.State;

	[ExtendedProperty]
	public DatabaseEncryptionState EncryptionState
	{
		get
		{
			Property property = base.Parent.Properties.Get("EncryptionState");
			if (property.Value == null)
			{
				return DatabaseEncryptionState.None;
			}
			return (DatabaseEncryptionState)property.Value;
		}
	}

	[ExtendedProperty]
	public bool Regenerate
	{
		get
		{
			return regenerate;
		}
		set
		{
			regenerate = value;
		}
	}

	[ExtendedProperty]
	public bool ReEncrypt
	{
		get
		{
			return reencrypt;
		}
		set
		{
			reencrypt = value;
		}
	}

	[ExtendedProperty]
	public string CertificateName
	{
		get
		{
			return certificateName;
		}
		set
		{
			certificateName = value;
		}
	}

	[ExtendedProperty]
	public string AsymmetricKeyName
	{
		get
		{
			return asymmetricKeyName;
		}
		set
		{
			asymmetricKeyName = value;
		}
	}

	public DatabaseEncryptionKeyExtender()
	{
	}

	public DatabaseEncryptionKeyExtender(DatabaseEncryptionKey dek)
		: base(dek)
	{
	}

	public ValidationState Validate(string methodName, params object[] arguments)
	{
		if (State == SqlSmoState.Creating || (State == SqlSmoState.Existing && ReEncrypt))
		{
			if (string.IsNullOrEmpty(base.Parent.EncryptorName))
			{
				if (base.Parent.EncryptionType == DatabaseEncryptionType.ServerCertificate)
				{
					return new ValidationState(ExceptionTemplatesImpl.EnterServerCertificate, "EncryptorName");
				}
				if (base.Parent.EncryptionType == DatabaseEncryptionType.ServerAsymmetricKey)
				{
					return new ValidationState(ExceptionTemplatesImpl.EnterServerAsymmetricKey, "EncryptorName");
				}
			}
			else if (base.Parent.EncryptionType == DatabaseEncryptionType.ServerCertificate && string.IsNullOrEmpty((string)certificateNameBackupDateHash[base.Parent.EncryptorName]))
			{
				string message = string.Format(CultureInfo.InvariantCulture, ExceptionTemplatesImpl.CertificateNotBackedUp, new object[1] { base.Parent.EncryptorName });
				return new ValidationState(message, "EncryptorName", isWarning: true);
			}
		}
		return base.Parent.Validate(methodName, arguments);
	}
}
