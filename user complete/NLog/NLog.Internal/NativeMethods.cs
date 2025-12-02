using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NLog.Internal;

internal static class NativeMethods
{
	[DllImport("kernel32.dll")]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	internal static extern int GetCurrentProcessId();

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	internal static extern uint GetModuleFileName([In] IntPtr hModule, [Out] StringBuilder lpFilename, [In][MarshalAs(UnmanagedType.U4)] int nSize);
}
