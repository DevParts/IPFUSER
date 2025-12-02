using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class SqlObjectProperty : ObjectProperty
{
	private string m_value;

	private string m_dbType;

	private string m_size;

	private string m_Alias;

	private string m_SessionValue;

	private bool m_bCast;

	private LinkMultiple m_LinkMultiple;

	public ArrayList LinkFields
	{
		get
		{
			if (m_LinkMultiple == null)
			{
				return null;
			}
			return m_LinkMultiple.LinkFields;
		}
	}

	public string Value
	{
		get
		{
			return m_value;
		}
		set
		{
			m_value = value;
		}
	}

	public string SessionValue
	{
		get
		{
			return m_SessionValue;
		}
		set
		{
			m_SessionValue = value;
		}
	}

	public string DBType => m_dbType;

	public string Size => m_size;

	public string Alias
	{
		get
		{
			return m_Alias;
		}
		set
		{
			m_Alias = value;
		}
	}

	public SqlObjectProperty(XmlReadProperty xrp)
	{
		base.Name = xrp.Name;
		base.Type = xrp.ClrType;
		base.Expensive = xrp.Expensive;
		base.ReadOnly = xrp.ReadOnly;
		base.ExtendedType = xrp.ExtendedType;
		base.ReadOnlyAfterCreation = xrp.ReadOnlyAfterCreation;
		base.KeyIndex = xrp.KeyIndex;
		base.PropertyMode = xrp.PropertyMode;
		m_dbType = xrp.DbType;
		m_size = xrp.Size;
		base.Usage = xrp.Usage;
		m_bCast = xrp.Cast;
		m_value = xrp.Value;
		XmlReadMultipleLink multipleLink = xrp.MultipleLink;
		if (multipleLink != null)
		{
			m_LinkMultiple = new LinkMultiple();
			m_LinkMultiple.Init(multipleLink);
		}
	}

	internal string GetValue(SqlObjectBase o)
	{
		if (m_value != null)
		{
			return m_value;
		}
		if (m_LinkMultiple != null)
		{
			return m_LinkMultiple.GetSqlExpression(o);
		}
		return null;
	}

	internal string GetTypeWithSize()
	{
		if (Size == null)
		{
			return DBType;
		}
		return string.Format(CultureInfo.InvariantCulture, "{0}({1})", new object[2] { DBType, Size });
	}

	internal string GetValueWithCast(SqlObjectBase o)
	{
		if (m_bCast)
		{
			return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS {1})", new object[2]
			{
				GetValue(o),
				GetTypeWithSize()
			});
		}
		return GetValue(o);
	}

	private void InitSessionValue(SqlObjectBase o)
	{
		m_SessionValue = GetValueWithCast(o);
	}

	public void Add(SqlObjectBase o, bool isTriggered)
	{
		InitSessionValue(o);
		o.StatementBuilder.StoreParentProperty(this, isTriggered);
	}
}
