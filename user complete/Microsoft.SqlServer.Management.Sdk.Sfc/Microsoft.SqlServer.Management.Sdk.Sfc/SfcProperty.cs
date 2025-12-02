using System;
using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public sealed class SfcProperty : ISfcProperty
{
	private SfcPropertyCollection m_propertyCollection;

	private string m_propertyName;

	public AttributeCollection Attributes => m_propertyCollection.m_propertyDispatcher.GetParent().Metadata.InternalStorageSupported[m_propertyCollection.LookupID(m_propertyName)].RelationshipAttributes;

	public string Name => m_propertyName;

	public object Value
	{
		get
		{
			return m_propertyCollection.GetValue(m_propertyName);
		}
		set
		{
			m_propertyCollection.SetValue(m_propertyName, value);
		}
	}

	public bool IsAvailable => m_propertyCollection.IsAvailable(m_propertyName);

	public bool Enabled
	{
		get
		{
			return m_propertyCollection.GetEnabled(m_propertyName);
		}
		set
		{
			m_propertyCollection.SetEnabled(m_propertyName, value);
		}
	}

	public Type Type => m_propertyCollection.Type(m_propertyName);

	public bool Writable => !m_propertyCollection.IsReadOnly(m_propertyName);

	public bool Readable => true;

	public bool Expensive => m_propertyCollection.IsExpensive(m_propertyName);

	public bool Computed => m_propertyCollection.IsComputed(m_propertyName);

	public bool Encrypted => m_propertyCollection.IsEncrypted(m_propertyName);

	public bool Standalone => m_propertyCollection.IsStandalone(m_propertyName);

	public bool SqlAzureDatabase => m_propertyCollection.IsSqlAzureDatabase(m_propertyName);

	public bool IdentityKey => m_propertyCollection.IsIdentityKey(m_propertyName);

	public bool Required => m_propertyCollection.IsRequired(m_propertyName);

	public bool Dirty
	{
		get
		{
			return m_propertyCollection.IsDirty(m_propertyName);
		}
		internal set
		{
			m_propertyCollection.SetDirty(m_propertyName, value);
		}
	}

	public bool Retrieved
	{
		get
		{
			return m_propertyCollection.IsRetrieved(m_propertyName);
		}
		internal set
		{
			m_propertyCollection.SetRetrieved(m_propertyName, value);
		}
	}

	public bool IsNull => m_propertyCollection.IsNull(m_propertyName);

	internal SfcProperty(SfcPropertyCollection propertyCollection, string propertyName)
	{
		m_propertyCollection = propertyCollection;
		m_propertyName = propertyName;
	}

	public override string ToString()
	{
		return string.Format("Name={0}/Type={1}/Writable={2}/Value={3}", Name, Type.ToString(), Writable, (!IsNull) ? Value : "null");
	}

	public int CompareTo(object obj)
	{
		if (obj == null)
		{
			return -1;
		}
		try
		{
			if (!(obj is ISfcProperty sfcProperty))
			{
				return -1;
			}
			if (string.Compare(Name, sfcProperty.Name, StringComparison.Ordinal) == 0 && sfcProperty.Dirty == Dirty && sfcProperty.IsNull == IsNull && sfcProperty.Required == Required && sfcProperty.Type == Type && sfcProperty.Value == Value && sfcProperty.Writable == Writable && sfcProperty.Attributes.Equals(Attributes))
			{
				return 0;
			}
			return -1;
		}
		catch
		{
			return -1;
		}
	}

	public override bool Equals(object o)
	{
		return 0 == CompareTo(o);
	}

	public static bool operator ==(SfcProperty prop1, SfcProperty prop2)
	{
		if ((object)prop1 == null && (object)prop2 == null)
		{
			return true;
		}
		return prop1?.Equals(prop2) ?? false;
	}

	public static bool operator !=(SfcProperty prop1, SfcProperty prop2)
	{
		return !(prop1 == prop2);
	}

	public static bool operator >(SfcProperty prop1, SfcProperty prop2)
	{
		return 0 < prop1.CompareTo(prop2);
	}

	public static bool operator <(SfcProperty prop1, SfcProperty prop2)
	{
		return 0 > prop1.CompareTo(prop2);
	}

	public override int GetHashCode()
	{
		return Name.GetHashCode();
	}
}
