using System;
using System.Collections;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ConfigPropertyCollection : ICollection, IEnumerable
{
	internal class ConfigPropertyEnumerator : IEnumerator
	{
		private int m_idx;

		private ConfigPropertyCollection m_col;

		object IEnumerator.Current => m_col[m_idx];

		public ConfigPropertyEnumerator(ConfigPropertyCollection col)
		{
			m_idx = -1;
			m_col = col;
		}

		public bool MoveNext()
		{
			return ++m_idx < m_col.Count;
		}

		public void Reset()
		{
			m_idx = -1;
		}
	}

	private ConfigurationBase m_parent;

	public int Count
	{
		get
		{
			if (m_parent.ConfigDataTable == null)
			{
				m_parent.PopulateDataTable();
			}
			return m_parent.ConfigDataTable.Rows.Count;
		}
	}

	public bool IsSynchronized
	{
		get
		{
			if (m_parent.ConfigDataTable == null)
			{
				m_parent.PopulateDataTable();
			}
			return m_parent.ConfigDataTable.Rows.IsSynchronized;
		}
	}

	public object SyncRoot
	{
		get
		{
			if (m_parent.ConfigDataTable == null)
			{
				m_parent.PopulateDataTable();
			}
			return m_parent.ConfigDataTable.Rows.SyncRoot;
		}
	}

	public ConfigProperty this[int index]
	{
		get
		{
			if (m_parent.ConfigDataTable == null)
			{
				m_parent.PopulateDataTable();
			}
			return new ConfigProperty(m_parent, (int)m_parent.ConfigDataTable.Rows[index]["Number"]);
		}
	}

	public ConfigProperty this[string name]
	{
		get
		{
			if (m_parent.ConfigDataTable == null)
			{
				m_parent.PopulateDataTable();
			}
			try
			{
				return (ConfigProperty)m_parent.GetType().InvokeMember(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, m_parent, null, SmoApplication.DefaultCulture);
			}
			catch (MissingMethodException)
			{
				return null;
			}
		}
	}

	internal ConfigPropertyCollection(ConfigurationBase parent)
	{
		m_parent = parent;
	}

	void ICollection.CopyTo(Array array, int index)
	{
		int num = 0;
		IEnumerator enumerator = GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ConfigProperty value = (ConfigProperty)enumerator.Current;
				array.SetValue(value, num++);
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void CopyTo(ConfigProperty[] array, int index)
	{
		int num = 0;
		IEnumerator enumerator = GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ConfigProperty value = (ConfigProperty)enumerator.Current;
				array.SetValue(value, num++);
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
	}

	public IEnumerator GetEnumerator()
	{
		if (m_parent.ConfigDataTable == null)
		{
			m_parent.PopulateDataTable();
		}
		return new ConfigPropertyEnumerator(this);
	}
}
