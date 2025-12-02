using Microsoft.SqlServer.Management.Smo.Internal;

namespace Microsoft.SqlServer.Management.Smo;

public class SymmetricKeyEncryption
{
	private SqlSecureString objectNameOrPassword;

	public KeyEncryptionType KeyEncryptionType;

	public string ObjectNameOrPassword
	{
		get
		{
			return objectNameOrPassword.ToString();
		}
		set
		{
			objectNameOrPassword = value;
		}
	}

	public SymmetricKeyEncryption()
	{
	}

	public SymmetricKeyEncryption(KeyEncryptionType encryptionType, string value)
	{
		KeyEncryptionType = encryptionType;
		objectNameOrPassword = value;
	}
}
