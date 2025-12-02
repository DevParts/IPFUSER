using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class Property : ISfcProperty
{
	private PropertyCollection m_propertyCollection;

	private int m_propertyIndex;

	public string Name => m_propertyCollection.GetName(m_propertyIndex);

	public object Value
	{
		get
		{
			return m_propertyCollection.GetValue(m_propertyIndex);
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			m_propertyCollection.SetValueFromUser(m_propertyIndex, value);
		}
	}

	internal PropertyCollection Parent => m_propertyCollection;

	internal bool Enumeration => m_propertyCollection.IsEnumeration(m_propertyIndex);

	public Type Type => m_propertyCollection.PropertyType(m_propertyIndex);

	public bool Writable => !m_propertyCollection.IsReadOnly(m_propertyIndex);

	public bool Readable => true;

	public bool Expensive => m_propertyCollection.IsExpensive(m_propertyIndex);

	public bool Dirty => m_propertyCollection.IsDirty(m_propertyIndex);

	public bool Retrieved => m_propertyCollection.IsAvailable(m_propertyIndex);

	public bool IsNull => m_propertyCollection.IsNull(m_propertyIndex);

	AttributeCollection ISfcProperty.Attributes => new AttributeCollection();

	bool ISfcProperty.Dirty => Dirty;

	bool ISfcProperty.Enabled => m_propertyCollection.IsEnabled(m_propertyIndex);

	bool ISfcProperty.IsNull => IsNull;

	string ISfcProperty.Name => Name;

	bool ISfcProperty.Required => false;

	Type ISfcProperty.Type => Type;

	object ISfcProperty.Value
	{
		get
		{
			return Value;
		}
		set
		{
			Value = value;
		}
	}

	bool ISfcProperty.Writable => Writable;

	internal Property(PropertyCollection propertyCollection, int propertyIndex)
	{
		m_propertyCollection = propertyCollection;
		m_propertyIndex = propertyIndex;
	}

	internal Property(Property p)
	{
		m_propertyCollection = p.m_propertyCollection;
		m_propertyIndex = p.m_propertyIndex;
	}

	internal void SetValue(object value)
	{
		m_propertyCollection.SetValue(m_propertyIndex, value);
	}

	internal void SetRetrieved(bool retrieved)
	{
		m_propertyCollection.SetRetrieved(m_propertyIndex, retrieved);
	}

	internal void SetDirty(bool dirty)
	{
		m_propertyCollection.SetDirty(m_propertyIndex, dirty);
	}

	internal void SetEnabled(bool enabled)
	{
		m_propertyCollection.SetEnabled(m_propertyIndex, enabled);
	}

	internal Property(object o1, object o2, object o3)
	{
	}

	public override string ToString()
	{
		return string.Format(SmoApplication.DefaultCulture, "Name={0}/Type={1}/Writable={2}/Value={3}", Name, Type.ToString(), Writable, (!IsNull) ? Value : "null");
	}

	public int CompareTo(object obj)
	{
		if (obj == null)
		{
			return -1;
		}
		return string.Compare(Name, ((Property)obj).Name, StringComparison.Ordinal);
	}

	public override bool Equals(object o)
	{
		return 0 == CompareTo(o);
	}

	public static bool operator ==(Property prop1, Property prop2)
	{
		if ((object)prop1 == null && (object)prop2 == null)
		{
			return true;
		}
		return prop1?.Equals(prop2) ?? false;
	}

	public static bool operator !=(Property prop1, Property prop2)
	{
		return !(prop1 == prop2);
	}

	public static bool operator >(Property prop1, Property prop2)
	{
		return 0 < prop1.CompareTo(prop2);
	}

	public static bool operator <(Property prop1, Property prop2)
	{
		return 0 > prop1.CompareTo(prop2);
	}

	public override int GetHashCode()
	{
		return Name.GetHashCode();
	}
}
