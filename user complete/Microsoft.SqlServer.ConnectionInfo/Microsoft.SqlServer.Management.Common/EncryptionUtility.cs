using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Microsoft.SqlServer.Management.Common;

internal static class EncryptionUtility
{
	public static string DecryptSecureString(SecureString ss)
	{
		string result = string.Empty;
		if (ss != null)
		{
			new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
			IntPtr intPtr = Marshal.SecureStringToBSTR(ss);
			result = Marshal.PtrToStringBSTR(intPtr);
			Marshal.ZeroFreeBSTR(intPtr);
		}
		return result;
	}

	public static SecureString EncryptString(string s)
	{
		SecureString secureString = new SecureString();
		if (s != null)
		{
			char[] array = s.ToCharArray();
			foreach (char c in array)
			{
				secureString.AppendChar(c);
			}
		}
		return secureString;
	}
}
