using System;

namespace Microsoft.SqlServer.Management.Smo;

[Flags]
public enum HttpAuthenticationModes
{
	Anonymous = 1,
	Basic = 2,
	Digest = 4,
	Integrated = 8,
	Ntlm = 0x10,
	Kerberos = 0x20,
	All = 0x3F
}
