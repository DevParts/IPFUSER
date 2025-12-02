using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.SqlEnum;

namespace Microsoft.SqlServer.Management.Smo;

internal abstract class PostProcess
{
	private object m_ci;

	private string m_objectName;

	private Request m_req;

	private SortedList m_HitFields;

	private bool bLookUpOrdinalDone;

	private object[] m_triggeredColumnsIdLookup;

	internal object ConnectionInfo
	{
		get
		{
			return m_ci;
		}
		set
		{
			m_ci = value;
		}
	}

	internal SortedList HitFields
	{
		get
		{
			return m_HitFields;
		}
		set
		{
			m_HitFields = value;
		}
	}

	internal string ObjectName
	{
		get
		{
			return m_objectName;
		}
		set
		{
			m_objectName = value;
		}
	}

	internal Request Request
	{
		get
		{
			return m_req;
		}
		set
		{
			m_req = value;
		}
	}

	protected virtual bool SupportDataReader => true;

	public PostProcess()
	{
		bLookUpOrdinalDone = false;
	}

	internal int HitFieldsCount()
	{
		return m_HitFields.Count;
	}

	internal bool GetIsFieldHit(string field)
	{
		return m_HitFields.ContainsKey(field);
	}

	internal bool IsLookupInit()
	{
		return null != m_triggeredColumnsIdLookup;
	}

	internal void CheckDataReaderSupport()
	{
		if (SupportDataReader)
		{
			return;
		}
		string text = string.Empty;
		bool flag = true;
		foreach (string key in HitFields.Keys)
		{
			if (!flag)
			{
				text += " ,";
			}
			flag = false;
			text += key;
		}
		throw new QueryNotSupportedEnumeratorException(StringSqlEnumerator.QueryNotSupportedPostProcess(text));
	}

	internal void InitNameBasedLookup(SqlObjectBase obj, StringCollection triggeredFields)
	{
		m_triggeredColumnsIdLookup = new object[triggeredFields.Count];
		int num = 0;
		StringEnumerator enumerator = triggeredFields.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				m_triggeredColumnsIdLookup[num++] = obj.GetAliasPropertyName(current);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	internal void UpdateFromNameBasedToOrdinalLookup(SortedList triggeredColumnsAliasNameLookup)
	{
		if (!bLookUpOrdinalDone)
		{
			bLookUpOrdinalDone = true;
			for (int i = 0; i < m_triggeredColumnsIdLookup.Length; i++)
			{
				m_triggeredColumnsIdLookup[i] = triggeredColumnsAliasNameLookup[m_triggeredColumnsIdLookup[i]];
			}
		}
	}

	protected int BinarySearch(DataRowCollection col, string objectIdentifier, string columnName)
	{
		int num = 0;
		int num2 = col.Count - 1;
		while (num <= num2)
		{
			int num3 = (num + num2) / 2;
			int num4 = objectIdentifier.CompareTo(col[num3][columnName].ToString());
			if (-1 == num4)
			{
				num2 = num3 - 1;
				continue;
			}
			if (1 == num4)
			{
				num = num3 + 1;
				continue;
			}
			return num3;
		}
		return -1;
	}

	protected int BinarySearchSetOnFirst(DataRowCollection col, string objectIdentifier, string columnName)
	{
		int num = BinarySearch(col, objectIdentifier, columnName);
		if (num < 0)
		{
			return num;
		}
		while (num > 0 && objectIdentifier == col[num - 1][columnName].ToString())
		{
			num--;
		}
		return num;
	}

	protected bool IsNull(object data)
	{
		return data.GetType() == Type.GetType("System.DBNull");
	}

	protected bool IsNull(DataProvider dp, int i)
	{
		return IsNull(GetTriggeredObject(dp, i));
	}

	protected object GetTriggeredObject(DataProvider dp, int i)
	{
		return dp.GetTrigeredValue((int)m_triggeredColumnsIdLookup[i]);
	}

	protected int GetTriggeredInt32(DataProvider dp, int i)
	{
		return (int)GetTriggeredObject(dp, i);
	}

	protected bool GetTriggeredBool(DataProvider dp, int i)
	{
		return (bool)GetTriggeredObject(dp, i);
	}

	protected string GetTriggeredString(DataProvider dp, int i)
	{
		object triggeredObject = GetTriggeredObject(dp, i);
		if (IsNull(triggeredObject))
		{
			return null;
		}
		return (string)triggeredObject;
	}

	public abstract object GetColumnData(string name, object data, DataProvider dp);

	public virtual void CleanRowData()
	{
	}
}
