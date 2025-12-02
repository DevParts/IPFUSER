using System.Collections;
using System.Text;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class PermissionSetBase
{
	private BitArray m_storage;

	internal BitArray Storage
	{
		get
		{
			return m_storage;
		}
		set
		{
			m_storage = value;
		}
	}

	internal abstract int NumberOfElements { get; }

	public PermissionSetBase()
	{
		m_storage = new BitArray(NumberOfElements);
	}

	public PermissionSetBase(PermissionSetBase permissionSetBase)
	{
		m_storage = (BitArray)permissionSetBase.m_storage.Clone();
	}

	internal void SetBitAt(int idx)
	{
		m_storage[idx] = true;
	}

	internal abstract string PermissionCodeToPermissionName(int permissionCode);

	internal abstract string PermissionCodeToPermissionType(int permissionCode);

	private int YukonToShilohPermission(string permCode)
	{
		return permCode.TrimEnd(' ') switch
		{
			"RF" => 26, 
			"CRFN" => 178, 
			"SL" => 193, 
			"IN" => 195, 
			"DL" => 196, 
			"UP" => 197, 
			"CRTB" => 198, 
			"CRDB" => 203, 
			"CRVW" => 207, 
			"CRPR" => 222, 
			"EX" => 224, 
			"BADB" => 228, 
			"CRDF" => 233, 
			"BALO" => 235, 
			"CRRU" => 236, 
			_ => -1, 
		};
	}

	internal bool IsValidPermissionForVersion(SqlServerVersionInternal ver)
	{
		if (ver >= SqlServerVersionInternal.Version90)
		{
			return true;
		}
		for (int i = 0; i < m_storage.Length; i++)
		{
			if (m_storage[i])
			{
				int num = YukonToShilohPermission(PermissionCodeToPermissionType(i));
				if (0 > num)
				{
					return false;
				}
			}
		}
		return true;
	}

	internal void AddPermissionFilter(StringBuilder sb, ServerVersion ver)
	{
		string value = ((ver.Major < 9) ? "@SqlInt = " : "@StringCode = ");
		bool flag = true;
		for (int i = 0; i < m_storage.Length; i++)
		{
			if (!m_storage[i])
			{
				continue;
			}
			if (flag)
			{
				flag = false;
			}
			else
			{
				sb.Append(" or ");
			}
			sb.Append(value);
			string text = PermissionCodeToPermissionType(i);
			if (ver.Major < 9)
			{
				int num = YukonToShilohPermission(text);
				if (num < 0)
				{
					throw new SmoException(ExceptionTemplatesImpl.UnsupportedPermission(num.ToString()));
				}
				sb.Append(num);
			}
			else
			{
				sb.Append("'" + text + "'");
			}
		}
	}

	internal bool AddPermissionList(StringBuilder sb)
	{
		bool flag = true;
		for (int i = 0; i < m_storage.Length; i++)
		{
			if (m_storage[i])
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					sb.Append(", ");
				}
				sb.Append(PermissionCodeToPermissionName(i));
			}
		}
		return !flag;
	}

	internal int GetPermissionCount()
	{
		int num = 0;
		for (int i = 0; i < Storage.Length; i++)
		{
			if (Storage[i])
			{
				num++;
			}
		}
		return num;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		AddPermissionList(stringBuilder);
		return stringBuilder.ToString();
	}

	public override bool Equals(object o)
	{
		if (o == null)
		{
			return false;
		}
		if (GetType() != o.GetType())
		{
			return false;
		}
		PermissionSetBase permissionSetBase = o as PermissionSetBase;
		for (int i = 0; i < NumberOfElements; i++)
		{
			if (m_storage[i] != permissionSetBase.m_storage[i])
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		return m_storage.GetHashCode();
	}
}
