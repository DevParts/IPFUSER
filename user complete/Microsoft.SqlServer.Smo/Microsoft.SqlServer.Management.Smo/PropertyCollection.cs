using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class PropertyCollection : ICollection, IEnumerable, ISfcPropertySet
{
	private enum InitializationState
	{
		Empty,
		Partial,
		Full
	}

	internal class PropertyEnumerator : IEnumerator
	{
		protected PropertyCollection m_propertyCollection;

		private int m_currentPos;

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
					RetrieveProperty(m_currentPos);
				}
				return m_propertyCollection.GetProperty(m_currentPos);
			}
		}

		public PropertyEnumerator(PropertyCollection propertyCollection)
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

		protected virtual void RetrieveProperty(int m_currentPos)
		{
			m_propertyCollection.RetrieveProperty(m_currentPos, m_propertyCollection.IsDesignMode ? true : false);
		}
	}

	private class SfcPropertyEnumerator : PropertyEnumerator
	{
		public SfcPropertyEnumerator(PropertyCollection propertyCollection)
			: base(propertyCollection)
		{
		}

		protected override void RetrieveProperty(int m_currentPos)
		{
			m_propertyCollection.RetrieveProperty(m_currentPos, useDefaultOnMissingValue: true);
		}
	}

	internal SmoObjectBase m_parent;

	private PropertyStorageBase m_PropertyStorage;

	private PropertyMetadataProvider m_pmp;

	internal PropertyMetadataProvider PropertiesMetadata => m_pmp;

	internal bool IsDesignMode
	{
		get
		{
			if (m_parent is SqlSmoObject)
			{
				return ((SqlSmoObject)m_parent).IsDesignMode;
			}
			return false;
		}
	}

	internal bool Dirty
	{
		get
		{
			for (int i = 0; i < m_PropertyStorage.Count; i++)
			{
				if (IsDirty(i))
				{
					return true;
				}
			}
			return false;
		}
	}

	public int Count => PropertiesMetadata.Count;

	public Property this[string name] => GetPropertyObject(name);

	public Property this[int index] => GetPropertyObject(index);

	public bool IsSynchronized => false;

	public object SyncRoot => this;

	internal int LookupID(string propertyName, PropertyAccessPurpose pap)
	{
		return PropertiesMetadata.PropertyNameToIDLookupWithException(propertyName, pap);
	}

	protected int LookupID(string propertyName)
	{
		return PropertiesMetadata.PropertyNameToIDLookupWithException(propertyName);
	}

	private int LookupIDNoBoundCheck(string propertyName)
	{
		return PropertiesMetadata.PropertyNameToIDLookup(propertyName);
	}

	private void RetrieveProperty(int index, bool useDefaultOnMissingValue)
	{
		if (!IsAvailable(index))
		{
			SetValue(index, m_parent.OnPropertyMissing(GetName(index), useDefaultOnMissingValue));
			if (IsDesignMode)
			{
				SetRetrieved(index, val: true);
			}
		}
	}

	private void HandleNullValue(int index)
	{
		if (IsDesignMode && IsNull(index))
		{
			Property property = GetProperty(index);
			if (!property.Writable)
			{
				throw new PropertyNotAvailableException(ExceptionTemplatesImpl.PropertyNotAvailableInDesignMode(GetName(index)));
			}
			throw new PropertyNotAvailableException(ExceptionTemplatesImpl.PropertyNotSetInDesignMode(GetName(index)));
		}
		if (IsNull(index) && IsRetrieved(index))
		{
			throw new PropertyCannotBeRetrievedException(GetName(index), m_parent);
		}
	}

	private Property GetProperty(string name)
	{
		return GetProperty(LookupID(name));
	}

	private Property GetProperty(int index)
	{
		return new Property(this, index);
	}

	private StaticMetadata GetStaticMetadata(int index)
	{
		return PropertiesMetadata.GetStaticMetadata(index);
	}

	internal Property Get(string name)
	{
		return GetProperty(name);
	}

	internal Property Get(int index)
	{
		return GetProperty(index);
	}

	internal string GetName(int index)
	{
		return GetStaticMetadata(index).Name;
	}

	internal object GetValueWithNullReplacement(string propertyName)
	{
		return GetValueWithNullReplacement(propertyName, throwOnNullValue: true, IsDesignMode ? true : false);
	}

	internal object GetValueWithNullReplacement(string propertyName, bool throwOnNullValue, bool useDefaultOnMissingValue)
	{
		int index = LookupID(propertyName, PropertyAccessPurpose.Read);
		RetrieveProperty(index, useDefaultOnMissingValue);
		if (throwOnNullValue)
		{
			HandleNullValue(index);
		}
		return GetValue(index);
	}

	internal object GetValue(int index)
	{
		return m_PropertyStorage.GetValue(index);
	}

	internal void SetValueWithConsistencyCheck(string propertyName, object value)
	{
		SetValueWithConsistencyCheck(propertyName, value, allowNull: false);
	}

	internal void SetValueWithConsistencyCheck(string propertyName, object value, bool allowNull)
	{
		if (!allowNull && value == null)
		{
			throw new ArgumentNullException();
		}
		SetValueFromUser(LookupID(propertyName, PropertyAccessPurpose.Write), value);
	}

	internal void SetValueFromUser(int index, object value)
	{
		if (IsReadOnly(index))
		{
			throw new PropertyReadOnlyException(GetName(index));
		}
		if (value != null)
		{
			Type type = PropertyType(index);
			if (type != value.GetType() && type != typeof(object))
			{
				throw new PropertyTypeMismatchException(GetName(index), value.GetType().ToString(), type.ToString());
			}
		}
		m_parent.ValidateProperty(GetProperty(index), value);
		if (m_parent.ShouldNotifyPropertyChange)
		{
			if (GetValue(index) != value)
			{
				SetValue(index, value);
				SetDirty(index, val: true);
				m_parent.OnPropertyChanged(GetName(index));
			}
		}
		else
		{
			SetValue(index, value);
			SetDirty(index, val: true);
		}
	}

	internal void SetValue(int index, object value)
	{
		m_PropertyStorage.SetValue(index, value);
	}

	internal bool IsDirty(int index)
	{
		return m_PropertyStorage.IsDirty(index);
	}

	internal void SetDirty(int index, bool val)
	{
		m_PropertyStorage.SetDirty(index, val);
	}

	internal bool IsRetrieved(int index)
	{
		return m_PropertyStorage.IsRetrieved(index);
	}

	internal void SetAllRetrieved(bool val)
	{
		for (int i = 0; i < m_PropertyStorage.Count; i++)
		{
			m_PropertyStorage.SetRetrieved(i, val);
		}
	}

	internal void SetRetrieved(int index, bool val)
	{
		m_PropertyStorage.SetRetrieved(index, val);
	}

	internal bool IsEnabled(int index)
	{
		return m_PropertyStorage.IsEnabled(index);
	}

	internal void SetEnabled(int index, bool enabled)
	{
		if (m_parent.ShouldNotifyPropertyMetadataChange)
		{
			if (m_PropertyStorage.IsEnabled(index) != enabled)
			{
				m_PropertyStorage.SetEnabled(index, enabled);
				m_parent.OnPropertyMetadataChanged(GetName(index));
			}
		}
		else
		{
			m_PropertyStorage.SetEnabled(index, enabled);
		}
	}

	internal Type PropertyType(int index)
	{
		return GetStaticMetadata(index).PropertyType;
	}

	internal bool IsReadOnly(int index)
	{
		return GetStaticMetadata(index).ReadOnly;
	}

	internal bool IsExpensive(int index)
	{
		return GetStaticMetadata(index).Expensive;
	}

	internal bool IsAvailable(int index)
	{
		if (!IsDirty(index))
		{
			return IsRetrieved(index);
		}
		return true;
	}

	internal bool IsNull(int index)
	{
		return m_PropertyStorage.IsNull(index);
	}

	internal bool IsEnumeration(int index)
	{
		return GetStaticMetadata(index).IsEnumeration;
	}

	internal PropertyCollection(SmoObjectBase parent, PropertyMetadataProvider pmp)
	{
		m_pmp = pmp;
		m_parent = parent;
		m_PropertyStorage = PropertyStorageFactory.Create(parent, PropertiesMetadata.Count);
	}

	internal void SetAllDirty(bool val)
	{
		for (int i = 0; i < m_PropertyStorage.Count; i++)
		{
			SetDirty(i, val);
		}
	}

	internal void SetAllDirtyAsRetrieved(bool val)
	{
		for (int i = 0; i < m_PropertyStorage.Count; i++)
		{
			if (IsDirty(i))
			{
				SetRetrieved(i, val);
			}
		}
	}

	internal bool ArePropertiesDirty(StringCollection propsList)
	{
		StringEnumerator enumerator = propsList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				int index = LookupID(current);
				if (IsDirty(index))
				{
					return true;
				}
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		return false;
	}

	public Property GetPropertyObject(int index)
	{
		RetrieveProperty(index, IsDesignMode ? true : false);
		HandleNullValue(index);
		return Get(index);
	}

	internal Property GetPropertyObjectAllowNull(int index)
	{
		RetrieveProperty(index, IsDesignMode ? true : false);
		return Get(index);
	}

	public Property GetPropertyObject(int index, bool doNotLoadPropertyValues)
	{
		if (doNotLoadPropertyValues)
		{
			return Get(index);
		}
		return GetPropertyObject(index);
	}

	public Property GetPropertyObject(string name)
	{
		return GetPropertyObject(LookupID(name));
	}

	internal Property GetPropertyObjectAllowNull(string name)
	{
		return GetPropertyObjectAllowNull(LookupID(name));
	}

	public Property GetPropertyObject(string name, bool doNotLoadPropertyValues)
	{
		return Get(LookupID(name));
	}

	public void SetDirty(string propertyName, bool isDirty)
	{
		SetDirty(LookupID(propertyName, PropertyAccessPurpose.Read), isDirty);
	}

	public void CopyTo(Property[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	void ICollection.CopyTo(Array array, int index)
	{
		for (int i = 0; i < Count; i++)
		{
			array.SetValue(GetProperty(i), index + i);
		}
	}

	public IEnumerator GetEnumerator()
	{
		return new PropertyEnumerator(this);
	}

	public bool Contains(string propertyName)
	{
		int num = LookupIDNoBoundCheck(propertyName);
		if (num >= 0)
		{
			return num < m_PropertyStorage.Count;
		}
		return false;
	}

	private ISfcProperty GetISfcProperty(int index)
	{
		RetrieveProperty(index, useDefaultOnMissingValue: true);
		return Get(index);
	}

	bool ISfcPropertySet.Contains<T>(string name)
	{
		if (PropertiesMetadata.TryPropertyNameToIDLookup(name, out var index))
		{
			ISfcProperty iSfcProperty = GetISfcProperty(index);
			if (iSfcProperty != null && iSfcProperty.Type != null)
			{
				return typeof(T).GetIsAssignableFrom(iSfcProperty.Type);
			}
		}
		return false;
	}

	bool ISfcPropertySet.Contains(ISfcProperty property)
	{
		if (property != null && PropertiesMetadata.TryPropertyNameToIDLookup(property.Name, out var index))
		{
			ISfcProperty iSfcProperty = GetISfcProperty(index);
			return property.Equals(iSfcProperty);
		}
		return false;
	}

	bool ISfcPropertySet.Contains(string propertyName)
	{
		if (PropertiesMetadata.TryPropertyNameToIDLookup(propertyName, out var index))
		{
			ISfcProperty iSfcProperty = GetISfcProperty(index);
			return iSfcProperty != null;
		}
		return false;
	}

	IEnumerable<ISfcProperty> ISfcPropertySet.EnumProperties()
	{
		SfcPropertyEnumerator enumerator = new SfcPropertyEnumerator(this);
		while (enumerator.MoveNext())
		{
			yield return (ISfcProperty)enumerator.Current;
		}
	}

	bool ISfcPropertySet.TryGetProperty(string name, out ISfcProperty property)
	{
		if (PropertiesMetadata.TryPropertyNameToIDLookup(name, out var index))
		{
			property = GetISfcProperty(index);
			return property != null;
		}
		property = null;
		return false;
	}

	bool ISfcPropertySet.TryGetPropertyValue(string name, out object value)
	{
		return ((ISfcPropertySet)this).TryGetPropertyValue<object>(name, out value);
	}

	bool ISfcPropertySet.TryGetPropertyValue<T>(string name, out T value)
	{
		value = default(T);
		try
		{
			if (PropertiesMetadata.TryPropertyNameToIDLookup(name, out var index))
			{
				ISfcProperty iSfcProperty = GetISfcProperty(index);
				if (iSfcProperty != null)
				{
					value = (T)iSfcProperty.Value;
					return true;
				}
			}
		}
		catch
		{
		}
		return false;
	}
}
