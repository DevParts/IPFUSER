using System.Data;
using System.Globalization;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class DatabaseSecurityAdapter : DatabaseAdapter, IDatabaseSecurityFacet, IDmfFacet
{
	public bool IsOwnerSysadmin
	{
		get
		{
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(base.Database.Parent != null, "Database Security Adapter database Parent is null");
			if (string.IsNullOrEmpty(base.Database.Owner))
			{
				return false;
			}
			bool flag = false;
			Login login = base.Database.Parent.Logins[base.Database.Owner];
			if (login != null && login.LoginType == LoginType.SqlLogin)
			{
				return login.IsMember("sysadmin");
			}
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Implies(login != null, login.LoginType == LoginType.WindowsUser, "The database owner is nether a SqlLogin nor a WindowsUser, which cannot happen according to BOL.");
			string arguments = string.Format(CultureInfo.InvariantCulture, "'{0}', 'all'", new object[1] { base.Database.Owner });
			string filter = string.Format(CultureInfo.InvariantCulture, "LOWER([privilege]) = 'admin'");
			DataTable dataTable = base.Database.Parent.EnumAccountInfo(arguments, filter);
			return dataTable.Rows.Count > 0;
		}
	}

	public DatabaseSecurityAdapter(Database obj)
		: base(obj)
	{
	}

	public override void Refresh()
	{
		base.Database.SymmetricKeys.Refresh();
		base.Database.AsymmetricKeys.Refresh();
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(base.Database.Parent != null, "Database Security Adapter database Parent is null");
		base.Database.Parent.Logins.Refresh();
	}
}
