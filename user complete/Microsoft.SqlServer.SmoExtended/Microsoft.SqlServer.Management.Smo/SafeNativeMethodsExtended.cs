using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

[SuppressUnmanagedCodeSecurity]
internal class SafeNativeMethodsExtended
{
	internal const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 256;

	internal const int FORMAT_MESSAGE_IGNORE_INSERTS = 512;

	internal const int FORMAT_MESSAGE_FROM_STRING = 1024;

	internal const int FORMAT_MESSAGE_FROM_HMODULE = 2048;

	internal const int FORMAT_MESSAGE_FROM_SYSTEM = 4096;

	internal const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 8192;

	internal const int FORMAT_MESSAGE_MAX_WIDTH_MASK = 255;

	internal const int ERROR_INSUFFICIENT_BUFFER = 122;

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, IntPtr arguments);

	internal static string GetLastErrorMessage(int errorCode)
	{
		StringBuilder stringBuilder = new StringBuilder(256);
		int num = 0;
		int num2 = 122;
		while (num == 0 && num2 == 122)
		{
			num = FormatMessage(4608, IntPtr.Zero, errorCode, 0, stringBuilder, stringBuilder.Capacity, IntPtr.Zero);
			if (num == 0)
			{
				num2 = Marshal.GetLastWin32Error();
				if (num2 == 122)
				{
					stringBuilder.Capacity *= 2;
				}
			}
		}
		return stringBuilder.ToString();
	}
}
