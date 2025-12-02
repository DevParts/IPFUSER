using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[CLSCompliant(false)]
public class ServerRoleExtender : SmoObjectExtender<ServerRole>, ISfcValidate
{
	private DataTable serverRoleMembershipsTableData;

	private StringCollection containingRoleNames;

	private ServerRoleCollection serverRoleCol;

	private Dictionary<string, bool> svrRoleNameHasMembershipHash;

	private DataTable roleMembersTableData;

	private Dictionary<string, bool> memberNameIsMemberHash;

	private StringCollection memberNames;

	private object generalPageDataContainer;

	[ExtendedProperty]
	public SqlSmoState State => base.Parent.State;

	[ExtendedProperty]
	public SqlSmoObject CurrentObject => base.Parent;

	[ExtendedProperty]
	public bool IsFixedRoleOrPublic
	{
		get
		{
			if (base.Parent.State == SqlSmoState.Creating)
			{
				return false;
			}
			if (base.Parent.ServerVersion.Major > 8 && VersionUtils.IsSql11OrLater(base.Parent.ServerVersion) && !base.Parent.IsFixedRole)
			{
				return base.Parent.ID == 2;
			}
			return true;
		}
	}

	[ExtendedProperty]
	public string OwnerForUI
	{
		get
		{
			if (VersionUtils.IsSql11OrLater(base.Parent.ServerVersion))
			{
				return base.Parent.Properties.GetValueWithNullReplacement("Owner", throwOnNullValue: false, useDefaultOnMissingValue: true) as string;
			}
			return string.Empty;
		}
		set
		{
			if (VersionUtils.IsSql11OrLater(base.Parent.ServerVersion))
			{
				base.Parent.Properties.SetValueWithConsistencyCheck("Owner", value, allowNull: true);
			}
		}
	}

	[ExtendedProperty]
	public ServerConnection ConnectionContext => base.Parent.Parent.ConnectionContext;

	[ExtendedProperty]
	public DataTable ServerRoleMembershipsTableData => serverRoleMembershipsTableData;

	[ExtendedProperty]
	public object GeneralPageOnRunNow
	{
		get
		{
			return this.generalPageOnRunNow;
		}
		set
		{
			this.generalPageOnRunNow = (EventHandler)value;
		}
	}

	[ExtendedProperty]
	public object GeneralPageDataContainer
	{
		get
		{
			return generalPageDataContainer;
		}
		set
		{
			generalPageDataContainer = value;
		}
	}

	private DataTable ServerRoleMembershipsTableSchema
	{
		get
		{
			DataTable dataTable = new DataTable("ServerRoleMembershipsTableData");
			dataTable.Columns.Add(new DataColumn("HasMembership", typeof(bool)));
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
			dataTable.Columns.Add(new DataColumn("IsFixedRole", typeof(bool)));
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns["HasMembership"].AllowDBNull = false;
			dataTable.Columns["HasMembership"].DefaultValue = false;
			dataTable.Columns["IsFixedRole"].AllowDBNull = false;
			dataTable.Columns["IsFixedRole"].DefaultValue = false;
			return dataTable;
		}
	}

	public Dictionary<string, bool> ServerRoleNameHasMembershipHash
	{
		get
		{
			if (svrRoleNameHasMembershipHash == null)
			{
				svrRoleNameHasMembershipHash = new Dictionary<string, bool>();
			}
			return svrRoleNameHasMembershipHash;
		}
	}

	private ServerRoleCollection SvrRoleCollection
	{
		get
		{
			if (serverRoleCol == null)
			{
				serverRoleCol = base.Parent.Parent.Roles;
			}
			return serverRoleCol;
		}
	}

	[ExtendedProperty]
	public StringCollection ContainingRoleNames
	{
		get
		{
			if (containingRoleNames == null)
			{
				if (VersionUtils.IsSql11OrLater(base.Parent.ServerVersion) && base.Parent.State == SqlSmoState.Existing)
				{
					containingRoleNames = base.Parent.EnumContainingRoleNames();
				}
				else
				{
					containingRoleNames = new StringCollection();
				}
			}
			return containingRoleNames;
		}
	}

	[ExtendedProperty]
	public DataTable RoleMembersTableData => roleMembersTableData;

	private StringCollection MemberNames
	{
		get
		{
			if (memberNames == null)
			{
				memberNames = base.Parent.EnumMemberNames();
			}
			return memberNames;
		}
	}

	private DataTable RoleMembersTableSchema
	{
		get
		{
			DataTable dataTable = new DataTable("RoleMembersTableData");
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
			dataTable.Columns.Add(new DataColumn("IsLogin", typeof(bool)));
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns["IsLogin"].AllowDBNull = false;
			dataTable.Columns["IsLogin"].DefaultValue = false;
			return dataTable;
		}
	}

	public Dictionary<string, bool> MemberNameIsMemberHash
	{
		get
		{
			if (memberNameIsMemberHash == null)
			{
				memberNameIsMemberHash = new Dictionary<string, bool>();
			}
			return memberNameIsMemberHash;
		}
	}

	private event EventHandler generalPageOnRunNow;

	public ServerRoleExtender()
	{
		PopulateRequiredVariables();
	}

	public ServerRoleExtender(ServerRole serverRole)
		: base(serverRole)
	{
		PopulateRequiredVariables();
	}

	private void PopulateRequiredVariables()
	{
		serverRoleMembershipsTableData = ServerRoleMembershipsTableSchema;
		foreach (ServerRole item in SvrRoleCollection)
		{
			if (item.Name != base.Parent.Name && string.Compare("public", item.Name, StringComparison.Ordinal) != 0 && string.Compare("sysadmin", item.Name, StringComparison.Ordinal) != 0)
			{
				bool flag = !VersionUtils.IsSql11OrLater(base.Parent.ServerVersion) || item.IsFixedRole;
				DataRow dataRow = serverRoleMembershipsTableData.NewRow();
				dataRow["Name"] = item.Name;
				dataRow["HasMembership"] = ContainingRoleNames.Contains(item.Name);
				dataRow["IsFixedRole"] = flag;
				serverRoleMembershipsTableData.Rows.Add(dataRow);
			}
		}
		roleMembersTableData = RoleMembersTableSchema;
		StringEnumerator enumerator2 = MemberNames.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				string current = enumerator2.Current;
				DataRow dataRow2 = roleMembersTableData.NewRow();
				dataRow2["Name"] = current;
				if (base.Parent.Parent.Logins[current] == null)
				{
					dataRow2["IsLogin"] = false;
				}
				else
				{
					dataRow2["IsLogin"] = true;
				}
				roleMembersTableData.Rows.Add(dataRow2);
			}
		}
		finally
		{
			if (enumerator2 is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	public void RefreshServerRoleNameHasMembershipHash()
	{
		StringCollection stringCollection = new StringCollection();
		ServerRoleNameHasMembershipHash.Clear();
		foreach (DataRow row in serverRoleMembershipsTableData.Rows)
		{
			string text = row["Name"].ToString();
			bool flag = (bool)row["HasMembership"];
			stringCollection.Add(text);
			if (IsMembershipChanged(text, flag))
			{
				ServerRoleNameHasMembershipHash.Add(text, flag);
			}
		}
	}

	private bool IsMembershipChanged(string roleName, bool hasMembership)
	{
		if ((ContainingRoleNames.Contains(roleName) && hasMembership) || (!ContainingRoleNames.Contains(roleName) && !hasMembership))
		{
			return false;
		}
		return true;
	}

	public void RefreshRoleMembersHash()
	{
		StringCollection stringCollection = new StringCollection();
		MemberNameIsMemberHash.Clear();
		foreach (DataRow row in roleMembersTableData.Rows)
		{
			string text = row["Name"].ToString();
			stringCollection.Add(text);
			if (!MemberNames.Contains(text))
			{
				MemberNameIsMemberHash.Add(text, value: true);
			}
		}
		StringEnumerator enumerator2 = MemberNames.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				string current = enumerator2.Current;
				if (!stringCollection.Contains(current))
				{
					MemberNameIsMemberHash.Add(current, value: false);
				}
			}
		}
		finally
		{
			if (enumerator2 is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	public ValidationState Validate(string methodName, params object[] arguments)
	{
		if (methodName.Equals("Create") && string.IsNullOrEmpty(base.Parent.Name))
		{
			return new ValidationState(ExceptionTemplatesImpl.EnterServerRoleName, "Name");
		}
		if (VersionUtils.IsSql11OrLater(base.Parent.ServerVersion))
		{
			Property property = base.Parent.Properties.Get("Owner");
			if (methodName.Equals("Alter") && property.Dirty && string.IsNullOrEmpty(property.Value as string))
			{
				return new ValidationState(ExceptionTemplatesImpl.ServerRoleOwnerNameEmpty, "Owner");
			}
		}
		return base.Parent.Validate(methodName, arguments);
	}
}
