using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.SqlEnum;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class SqlPostProcess : ConditionedSql
{
	private string m_className;

	private Assembly m_assembly;

	private StringCollection m_triggeredFields;

	private SortedList m_HitFields;

	public StringCollection TriggeredFields => m_triggeredFields;

	protected override bool AcceptsMultipleHits => true;

	public SqlPostProcess(XmlReadConditionedStatementPostProcess xrpp, Assembly asembly)
	{
		SetFields(xrpp.Fields);
		m_triggeredFields = xrpp.TriggeredFields;
		if (xrpp.ClassName == null)
		{
			throw new InternalEnumeratorException(StringSqlEnumerator.NoClassNamePostProcess);
		}
		m_className = xrpp.ClassName;
		m_assembly = asembly;
	}

	private PostProcess GetPostProcessInstance()
	{
		if (!(Util.CreateObjectInstance(m_assembly, m_className) is PostProcess result))
		{
			throw new InternalEnumeratorException(StringSqlEnumerator.NotDerivedFrom(m_className, "PostProcess"));
		}
		return result;
	}

	public override void AddHit(string field, SqlObjectBase obj, StatementBuilder sb)
	{
		if (m_HitFields == null)
		{
			m_HitFields = new SortedList(StringComparer.Ordinal);
		}
		m_HitFields[field] = null;
	}

	internal void Register(SqlObjectBase obj)
	{
		if (m_HitFields == null)
		{
			return;
		}
		PostProcess postProcessInstance = GetPostProcessInstance();
		postProcessInstance.ObjectName = obj.ObjectName;
		postProcessInstance.ConnectionInfo = obj.ConnectionInfo;
		postProcessInstance.Request = obj.Request;
		postProcessInstance.InitNameBasedLookup(obj, TriggeredFields);
		postProcessInstance.HitFields = m_HitFields;
		foreach (string key in m_HitFields.Keys)
		{
			obj.StatementBuilder.AddPostProcess(obj.GetAliasPropertyName(key), postProcessInstance);
		}
		m_HitFields = null;
	}

	public static void AddAll(ConditionedSqlList list, XmlReadConditionedStatementPostProcess xrcs, Assembly asembly)
	{
		if (xrcs != null)
		{
			do
			{
				list.Add(new SqlPostProcess(xrcs, asembly));
			}
			while (xrcs.Next());
		}
	}
}
