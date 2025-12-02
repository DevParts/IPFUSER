using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

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
			throw new InternalEnumeratorException(SfcStrings.NoClassNamePostProcess);
		}
		m_className = xrpp.ClassName;
		m_assembly = asembly;
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
		_ = m_HitFields;
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
