using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public sealed class SfcPropertyCollection : ICollection, IEnumerable, ISfcPropertySet
{
	internal class SfcEnumerable : IEnumerable<ISfcProperty>, IEnumerable
	{
		private SfcPropertyCollection collection;

		internal SfcEnumerable(SfcPropertyCollection collection)
		{
			this.collection = collection;
		}

		public IEnumerator<ISfcProperty> GetEnumerator()
		{
			return new PropertyEnumerator(collection);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new PropertyEnumerator(collection);
		}
	}

	internal class PropertyEnumerator : IEnumerator<ISfcProperty>, IDisposable, IEnumerator
	{
		private SfcPropertyCollection m_propertyCollection;

		private int m_currentPos;

		ISfcProperty IEnumerator<ISfcProperty>.Current => (ISfcProperty)Current;

		public object Current
		{
			get
			{
				if (m_currentPos >= m_propertyCollection.Count)
				{
					throw new InvalidOperationException();
				}
				if (!m_propertyCollection.IsRetrieved(m_currentPos))
				{
					m_propertyCollection.RetrieveProperty(m_currentPos);
				}
				return m_propertyCollection.GetPropertyObject(m_currentPos);
			}
		}

		public PropertyEnumerator(SfcPropertyCollection propertyCollection)
		{
			m_propertyCollection = propertyCollection;
			m_currentPos = -1;
		}

		public bool MoveNext()
		{
			return ++m_currentPos < m_propertyCollection.Count;
		}

		public void Reset()
		{
			m_currentPos = -1;
		}

		public void Dispose()
		{
		}
	}

	internal PropertyDataDispatcher m_propertyDispatcher;

	private BitArray m_retrieved;

	private BitArray m_dirty;

	private BitArray m_enabled;

	internal bool DynamicMetaDataEnabled => m_enabled != null;

	public int Count => m_propertyDispatcher.GetParent().Metadata.InternalStorageSupportedCount;

	public SfcProperty this[string propertyName] => GetPropertyObject(propertyName);

	public bool IsSynchronized => false;

	public object SyncRoot => this;

	internal SfcPropertyCollection(PropertyDataDispatcher dispatcher)
	{
		m_propertyDispatcher = dispatcher;
		int internalStorageSupportedCount = m_propertyDispatcher.GetParent().Metadata.InternalStorageSupportedCount;
		m_retrieved = new BitArray(internalStorageSupportedCount);
		m_dirty = new BitArray(internalStorageSupportedCount);
	}

	public bool Contains(string propertyName)
	{
		int num = LookupID(propertyName);
		if (num >= 0)
		{
			return num < Count;
		}
		return false;
	}

	public bool Contains(ISfcProperty property)
	{
		SfcProperty sfcProperty = this[property.Name];
		if (sfcProperty == null)
		{
			return false;
		}
		return sfcProperty.Equals(property);
	}

	public bool Contains<T>(string propertyName)
	{
		if (Contains(propertyName))
		{
			return this[propertyName].Type == typeof(T);
		}
		return false;
	}

	public bool TryGetPropertyValue<T>(string propertyName, out T value)
	{
		value = default(T);
		try
		{
			value = (T)this[propertyName].Value;
			return true;
		}
		catch
		{
			return false;
		}
	}

	public bool TryGetPropertyValue(string propertyName, out object value)
	{
		return this.TryGetPropertyValue<object>(propertyName, out value);
	}

	public bool TryGetProperty(string propertyName, out ISfcProperty property)
	{
		property = null;
		try
		{
			property = this[propertyName];
			return true;
		}
		catch
		{
			return false;
		}
	}

	public IEnumerable<ISfcProperty> EnumProperties()
	{
		return new SfcEnumerable(this);
	}

	internal bool IsDirty(string propertyName)
	{
		int index = LookupID(propertyName);
		return m_dirty[index];
	}

	internal void SetDirty(string propertyName, bool val)
	{
		int index = LookupID(propertyName);
		m_dirty[index] = val;
	}

	public bool IsAvailable(string propertyName)
	{
		int index = LookupID(propertyName);
		if (!m_retrieved[index])
		{
			return m_dirty[index];
		}
		return true;
	}

	internal bool IsRetrieved(int index)
	{
		return m_retrieved[index];
	}

	internal bool IsRetrieved(string propertyName)
	{
		int index = LookupID(propertyName);
		return IsRetrieved(index);
	}

	internal void SetRetrieved(string propertyName, bool val)
	{
		int index = LookupID(propertyName);
		m_retrieved[index] = val;
	}

	public void CopyTo(SfcProperty[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	void ICollection.CopyTo(Array array, int index)
	{
		for (int i = 0; i < Count; i++)
		{
			array.SetValue(GetPropertyObject(i), index + i);
		}
	}

	internal object GetValue(string propertyName)
	{
		return m_propertyDispatcher.GetPropertyValue(propertyName);
	}

	internal void SetValue(string propertyName, object value)
	{
		object value2 = GetValue(propertyName);
		m_propertyDispatcher.SetPropertyValue(propertyName, value);
		SetDirty(propertyName, val: true);
		if (value2 == null || !value2.Equals(value))
		{
			PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
			m_propertyDispatcher.GetParent().InternalOnPropertyValueChanges(args);
		}
	}

	internal bool GetEnabled(string propertyName)
	{
		int index = LookupID(propertyName);
		if (m_enabled == null)
		{
			m_propertyDispatcher.InitializeState();
		}
		if (m_enabled == null)
		{
			return true;
		}
		return m_enabled[index];
	}

	internal void SetEnabled(string propertyName, bool value)
	{
		int index = LookupID(propertyName);
		bool enabled = GetEnabled(propertyName);
		if (m_enabled == null)
		{
			m_enabled = new BitArray(Count, defaultValue: true);
			m_propertyDispatcher.InitializeState();
		}
		m_enabled[index] = value;
		if (enabled != value)
		{
			SfcPropertyMetadataChangedEventArgs args = new SfcPropertyMetadataChangedEventArgs(propertyName);
			m_propertyDispatcher.GetParent().InternalOnPropertyMetadataChanges(args);
		}
	}

	internal int LookupID(string propertyName)
	{
		List<SfcMetadataRelation> internalStorageSupported = m_propertyDispatcher.GetParent().Metadata.InternalStorageSupported;
		for (int i = 0; i < internalStorageSupported.Count; i++)
		{
			if (string.Compare(propertyName, internalStorageSupported[i].PropertyName, StringComparison.Ordinal) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	private SfcProperty GetPropertyObject(int index)
	{
		return GetPropertyObject(m_propertyDispatcher.GetParent().Metadata.InternalStorageSupported[index].PropertyName);
	}

	private SfcProperty GetPropertyObject(string propertyName)
	{
		RetrieveProperty(propertyName);
		return new SfcProperty(this, propertyName);
	}

	private void RetrieveProperty(int index)
	{
		RetrieveProperty(m_propertyDispatcher.GetParent().Metadata.InternalStorageSupported[index].PropertyName);
	}

	private void RetrieveProperty(string propertyName)
	{
		SetRetrieved(propertyName, val: true);
	}

	public IEnumerator GetEnumerator()
	{
		return new PropertyEnumerator(this);
	}

	internal Type Type(string propertyName)
	{
		int index = LookupID(propertyName);
		return m_propertyDispatcher.GetParent().Metadata.InternalStorageSupported[index].Type;
	}

	internal bool IsNull(string propertyName)
	{
		return GetValue(propertyName) == null;
	}

	internal bool IsComputed(string propertyName)
	{
		int index = LookupID(propertyName);
		return 0 != (m_propertyDispatcher.GetParent().Metadata.InternalStorageSupported[index].PropertyFlags & SfcPropertyFlags.Computed);
	}

	internal bool IsEncrypted(string propertyName)
	{
		int index = LookupID(propertyName);
		return 0 != (m_propertyDispatcher.GetParent().Metadata.InternalStorageSupported[index].PropertyFlags & SfcPropertyFlags.Encrypted);
	}

	internal bool IsExpensive(string propertyName)
	{
		int index = LookupID(propertyName);
		return 0 != (m_propertyDispatcher.GetParent().Metadata.InternalStorageSupported[index].PropertyFlags & SfcPropertyFlags.Expensive);
	}

	internal bool IsStandalone(string propertyName)
	{
		int index = LookupID(propertyName);
		return 0 != (m_propertyDispatcher.GetParent().Metadata.InternalStorageSupported[index].PropertyFlags & SfcPropertyFlags.Standalone);
	}

	internal bool IsSqlAzureDatabase(string propertyName)
	{
		int index = LookupID(propertyName);
		return 0 != (m_propertyDispatcher.GetParent().Metadata.InternalStorageSupported[index].PropertyFlags & SfcPropertyFlags.SqlAzureDatabase);
	}

	internal bool IsIdentityKey(string propertyName)
	{
		int index = LookupID(propertyName);
		foreach (Attribute relationshipAttribute in m_propertyDispatcher.GetParent().Metadata.InternalStorageSupported[index].RelationshipAttributes)
		{
			if (relationshipAttribute is SfcKeyAttribute)
			{
				return true;
			}
		}
		return false;
	}

	internal bool IsReadOnly(string propertyName)
	{
		int index = LookupID(propertyName);
		return 0 != (m_propertyDispatcher.GetParent().Metadata.InternalStorageSupported[index].PropertyFlags & SfcPropertyFlags.Data);
	}

	internal bool IsRequired(string propertyName)
	{
		int index = LookupID(propertyName);
		return 0 != (m_propertyDispatcher.GetParent().Metadata.InternalStorageSupported[index].PropertyFlags & SfcPropertyFlags.Required);
	}
}
