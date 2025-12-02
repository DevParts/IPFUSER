using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class SqlRequest : Request
{
	private ArrayList m_listLinkField;

	private StringCollection m_PrefixPostfixFields;

	private bool m_bResolveDatabases;

	public bool ResolveDatabases
	{
		get
		{
			return m_bResolveDatabases;
		}
		set
		{
			m_bResolveDatabases = value;
		}
	}

	public ArrayList LinkFields => m_listLinkField;

	internal StringCollection PrefixPostfixFields
	{
		get
		{
			return m_PrefixPostfixFields;
		}
		set
		{
			m_PrefixPostfixFields = value;
		}
	}

	public SqlRequest()
	{
		m_bResolveDatabases = true;
	}

	public SqlRequest(Request reqUser)
	{
		base.Urn = reqUser.Urn;
		base.Fields = reqUser.Fields;
		base.OrderByList = reqUser.OrderByList;
		base.ResultType = reqUser.ResultType;
		base.RequestFieldsTypes = reqUser.RequestFieldsTypes;
	}

	public void SetLinkFields(ArrayList list)
	{
		m_listLinkField = list;
	}

	internal void MergeLinkFileds(SqlRequest req)
	{
		if (m_listLinkField == null)
		{
			m_listLinkField = req.LinkFields;
			return;
		}
		foreach (object linkField in req.LinkFields)
		{
			m_listLinkField.Add(linkField);
		}
	}
}
