using System;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

internal static class SecurityUtils
{
	private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

	private static RandomNumberGenerator Rng => rng;

	public static void ScriptRandomPwd(StringBuilder pwdGenScript)
	{
		int num = 64;
		pwdGenScript.Append("/* To avoid disclosure of passwords, the password is generated in script. */");
		pwdGenScript.Append(Globals.newline);
		pwdGenScript.Append("declare @idx as int");
		pwdGenScript.Append(Globals.newline);
		pwdGenScript.AppendFormat(SmoApplication.DefaultCulture, "declare @randomPwd as nvarchar({0})", new object[1] { num });
		pwdGenScript.Append(Globals.newline);
		pwdGenScript.Append("declare @rnd as float");
		pwdGenScript.Append(Globals.newline);
		pwdGenScript.Append("select @idx = 0");
		pwdGenScript.Append(Globals.newline);
		pwdGenScript.Append("select @randomPwd = N''");
		pwdGenScript.Append(Globals.newline);
		pwdGenScript.Append("select @rnd = rand((@@CPU_BUSY % 100) + ((@@IDLE % 100) * 100) + ");
		pwdGenScript.Append(Globals.newline);
		pwdGenScript.Append("       (DATEPART(ss, GETDATE()) * 10000) + ((cast(DATEPART(ms, GETDATE()) as int) % 100) * 1000000))");
		pwdGenScript.Append(Globals.newline);
		pwdGenScript.AppendFormat(SmoApplication.DefaultCulture, "while @idx < {0}", new object[1] { num });
		pwdGenScript.Append(Globals.newline);
		pwdGenScript.Append("begin");
		pwdGenScript.Append(Globals.newline);
		pwdGenScript.Append("   select @randomPwd = @randomPwd + char((cast((@rnd * 83) as int) + 43))");
		pwdGenScript.Append(Globals.newline);
		pwdGenScript.Append("   select @idx = @idx + 1");
		pwdGenScript.Append(Globals.newline);
		pwdGenScript.Append("select @rnd = rand()");
		pwdGenScript.Append(Globals.newline);
		pwdGenScript.Append("end");
		pwdGenScript.Append(Globals.newline);
	}

	public static string GenerateRandomPassword()
	{
		byte[] array = new byte[32];
		Rng.GetNonZeroBytes(array);
		return Convert.ToBase64String(array);
	}
}
