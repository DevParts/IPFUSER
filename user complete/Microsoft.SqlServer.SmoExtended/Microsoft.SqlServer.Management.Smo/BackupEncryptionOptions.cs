using System;
using System.Globalization;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class BackupEncryptionOptions
{
	private bool noEncryption;

	private BackupEncryptionAlgorithm? algorithm;

	private BackupEncryptorType? encryptorType;

	private string encryptorName;

	public bool NoEncryption
	{
		get
		{
			return noEncryption;
		}
		set
		{
			noEncryption = value;
		}
	}

	public BackupEncryptionAlgorithm? Algorithm
	{
		get
		{
			return algorithm;
		}
		set
		{
			algorithm = value;
		}
	}

	public BackupEncryptorType? EncryptorType
	{
		get
		{
			return encryptorType;
		}
		set
		{
			encryptorType = value;
		}
	}

	public string EncryptorName
	{
		get
		{
			return encryptorName;
		}
		set
		{
			encryptorName = value;
		}
	}

	public BackupEncryptionOptions()
	{
		noEncryption = false;
	}

	public BackupEncryptionOptions(BackupEncryptionAlgorithm algorithm, BackupEncryptorType encryptorType, string encryptorName)
	{
		noEncryption = false;
		Algorithm = algorithm;
		EncryptorType = encryptorType;
		EncryptorName = encryptorName;
	}

	internal string Script()
	{
		if (noEncryption)
		{
			return string.Empty;
		}
		if (!algorithm.HasValue)
		{
			throw new PropertyNotSetException("Algorithm");
		}
		if (!encryptorType.HasValue)
		{
			throw new PropertyNotSetException("EncryptorType");
		}
		if (string.IsNullOrEmpty(encryptorName))
		{
			throw new PropertyNotSetException("EncryptorName");
		}
		string algorithmString = GetAlgorithmString(algorithm.Value);
		string encryptorTypeString = GetEncryptorTypeString(encryptorType.Value);
		return string.Format(SmoApplication.DefaultCulture, "ENCRYPTION(ALGORITHM = {0}, {1} = [{2}])", new object[3]
		{
			algorithmString,
			encryptorTypeString,
			SqlSmoObject.SqlBraket(encryptorName)
		});
	}

	public static string GetAlgorithmString(BackupEncryptionAlgorithm algorithm)
	{
		string empty = string.Empty;
		return algorithm switch
		{
			BackupEncryptionAlgorithm.Aes128 => "AES_128", 
			BackupEncryptionAlgorithm.Aes192 => "AES_192", 
			BackupEncryptionAlgorithm.Aes256 => "AES_256", 
			BackupEncryptionAlgorithm.TripleDes => "TRIPLE_DES_3KEY", 
			_ => throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Unknown BackupEncryptionAlgorithm: {0}", new object[1] { algorithm })), 
		};
	}

	private static string GetEncryptorTypeString(BackupEncryptorType encryptorType)
	{
		string empty = string.Empty;
		return encryptorType switch
		{
			BackupEncryptorType.ServerCertificate => "SERVER CERTIFICATE", 
			BackupEncryptorType.ServerAsymmetricKey => "SERVER ASYMMETRIC KEY", 
			_ => throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Unknown BackupEncryptorType: {0}", new object[1] { encryptorType })), 
		};
	}
}
