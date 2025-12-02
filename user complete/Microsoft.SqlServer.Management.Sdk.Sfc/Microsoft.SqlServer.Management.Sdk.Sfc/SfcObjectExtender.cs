using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcObjectExtender<TSfcInstance> : ISfcPropertyProvider, INotifyPropertyChanged, ISfcNotifyPropertyMetadataChanged where TSfcInstance : ISfcPropertyProvider, new()
{
	private class SfcPropertyDictionary : ISfcPropertySet
	{
		private ISfcPropertySet parent;

		private Dictionary<string, ISfcProperty> properties;

		public IDictionary<string, ISfcProperty> Properties => properties;

		public SfcPropertyDictionary(ISfcPropertySet parent)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			this.parent = parent;
			properties = new Dictionary<string, ISfcProperty>();
			foreach (ISfcProperty item in this.parent.EnumProperties())
			{
				properties[item.Name] = item;
			}
		}

		bool ISfcPropertySet.Contains<T>(string name)
		{
			if (properties.TryGetValue(name, out var value))
			{
				return typeof(T).IsAssignableFrom(value.Type);
			}
			return false;
		}

		bool ISfcPropertySet.Contains(ISfcProperty property)
		{
			return properties.ContainsValue(property);
		}

		bool ISfcPropertySet.Contains(string propertyName)
		{
			return properties.ContainsKey(propertyName);
		}

		IEnumerable<ISfcProperty> ISfcPropertySet.EnumProperties()
		{
			return properties.Values;
		}

		bool ISfcPropertySet.TryGetProperty(string name, out ISfcProperty property)
		{
			return properties.TryGetValue(name, out property);
		}

		bool ISfcPropertySet.TryGetPropertyValue(string name, out object value)
		{
			return ((ISfcPropertySet)this).TryGetPropertyValue<object>(name, out value);
		}

		bool ISfcPropertySet.TryGetPropertyValue<T>(string name, out T value)
		{
			if (properties.TryGetValue(name, out var value2) && typeof(T).IsAssignableFrom(value2.Type) && !value2.IsNull)
			{
				value = (T)value2.Value;
				return true;
			}
			value = default(T);
			return false;
		}
	}

	private class ReflectedProperty : ISfcProperty
	{
		private PropertyInfo property;

		private AttributeCollection attributes;

		private object owner;

		private bool dirty;

		public AttributeCollection Attributes
		{
			get
			{
				if (attributes == null)
				{
					AttributeCollection attributeCollection = TypeDescriptor.GetAttributes(property.PropertyType);
					object[] customAttributes = property.GetCustomAttributes(inherit: true);
					Dictionary<Type, Attribute> dictionary = new Dictionary<Type, Attribute>();
					foreach (Attribute item in attributeCollection)
					{
						dictionary[item.GetType()] = item;
					}
					object[] array = customAttributes;
					for (int i = 0; i < array.Length; i++)
					{
						Attribute attribute2 = (Attribute)array[i];
						dictionary[attribute2.GetType()] = attribute2;
					}
					Attribute[] array2 = new Attribute[dictionary.Count];
					dictionary.Values.CopyTo(array2, 0);
					attributes = new AttributeCollection(array2);
				}
				return attributes;
			}
		}

		public bool Dirty => dirty;

		public bool Enabled => property.CanWrite;

		public bool IsNull => Value == null;

		public string Name => property.Name;

		public bool Required => false;

		public Type Type => property.PropertyType;

		public object Value
		{
			get
			{
				return property.GetValue(owner, null);
			}
			set
			{
				property.SetValue(owner, value, null);
				dirty = true;
			}
		}

		public bool Writable => property.CanWrite;

		public ReflectedProperty(object owner, PropertyInfo property)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			this.owner = owner;
			this.property = property;
		}
	}

	private class ParentedReflectedProperty : ISfcProperty
	{
		private PropertyInfo propertyInfo;

		private ISfcProperty parentProperty;

		private AttributeCollection attributes;

		private object owner;

		public AttributeCollection Attributes
		{
			get
			{
				if (attributes == null)
				{
					AttributeCollection attributeCollection = parentProperty.Attributes;
					AttributeCollection attributeCollection2 = TypeDescriptor.GetAttributes(propertyInfo.PropertyType);
					object[] customAttributes = propertyInfo.GetCustomAttributes(inherit: true);
					Dictionary<Type, Attribute> dictionary = new Dictionary<Type, Attribute>();
					if (attributeCollection != null)
					{
						foreach (Attribute item in attributeCollection)
						{
							dictionary[item.GetType()] = item;
						}
					}
					if (attributeCollection2 != null)
					{
						foreach (Attribute item2 in attributeCollection2)
						{
							dictionary[item2.GetType()] = item2;
						}
					}
					if (customAttributes != null)
					{
						object[] array = customAttributes;
						for (int i = 0; i < array.Length; i++)
						{
							Attribute attribute3 = (Attribute)array[i];
							dictionary[attribute3.GetType()] = attribute3;
						}
					}
					Attribute[] array2 = new Attribute[dictionary.Count];
					dictionary.Values.CopyTo(array2, 0);
					attributes = new AttributeCollection(array2);
				}
				return attributes;
			}
		}

		public bool Dirty => parentProperty.Dirty;

		public bool Enabled => parentProperty.Enabled;

		public bool IsNull => parentProperty.IsNull;

		public string Name => propertyInfo.Name;

		public bool Required => parentProperty.Required;

		public Type Type => propertyInfo.PropertyType;

		public object Value
		{
			get
			{
				return propertyInfo.GetValue(owner, null);
			}
			set
			{
				propertyInfo.SetValue(owner, value, null);
			}
		}

		public bool Writable => parentProperty.Writable;

		public ParentedReflectedProperty(object owner, PropertyInfo propertyInfo, ISfcProperty parentProperty)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			if (parentProperty == null)
			{
				throw new ArgumentNullException("parentProperty");
			}
			this.owner = owner;
			this.propertyInfo = propertyInfo;
			this.parentProperty = parentProperty;
		}
	}

	private TSfcInstance parent;

	private SfcPropertyDictionary properties;

	private Dictionary<string, string> propertyMapper;

	protected TSfcInstance Parent => parent;

	private SfcPropertyDictionary PropertyDictionary
	{
		get
		{
			if (properties == null)
			{
				properties = new SfcPropertyDictionary(GetParentSfcPropertySet());
				PropertyInfo[] array = GetType().GetProperties();
				foreach (PropertyInfo propertyInfo in array)
				{
					object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(ExtendedPropertyAttribute), inherit: true);
					for (int j = 0; j < customAttributes.Length; j++)
					{
						ExtendedPropertyAttribute extendedPropertyAttribute = (ExtendedPropertyAttribute)customAttributes[j];
						if (extendedPropertyAttribute.HasParent)
						{
							RegisterProperty(propertyInfo, extendedPropertyAttribute.ParentPropertyName);
						}
						else
						{
							RegisterProperty(propertyInfo);
						}
					}
				}
			}
			return properties;
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public event EventHandler<SfcPropertyMetadataChangedEventArgs> PropertyMetadataChanged;

	public SfcObjectExtender()
		: this(new TSfcInstance())
	{
	}

	public SfcObjectExtender(TSfcInstance parent)
	{
		if (parent == null)
		{
			throw new ArgumentNullException("parent");
		}
		this.parent = parent;
		this.parent.PropertyChanged += parent_PropertyChanged;
		this.parent.PropertyMetadataChanged += parent_PropertyMetadataChanged;
	}

	protected void RegisterProperty(PropertyInfo propertyInfo)
	{
		if (propertyInfo == null)
		{
			throw new ArgumentNullException("propertyInfo");
		}
		PropertyDictionary.Properties[propertyInfo.Name] = new ReflectedProperty(this, propertyInfo);
	}

	protected void RegisterParentProperty(PropertyInfo propertyInfo)
	{
		if (propertyInfo == null)
		{
			throw new ArgumentNullException("propertyInfo");
		}
		PropertyDictionary.Properties[propertyInfo.Name] = new ReflectedProperty(Parent, propertyInfo);
	}

	protected void RegisterProperty(PropertyInfo propertyInfo, string parentPropertyName)
	{
		if (propertyInfo == null)
		{
			throw new ArgumentNullException("propertyInfo");
		}
		if (string.IsNullOrEmpty(parentPropertyName))
		{
			throw new ArgumentNullException("parentPropertyName");
		}
		SfcPropertyDictionary propertyDictionary = PropertyDictionary;
		propertyDictionary.Properties[propertyInfo.Name] = new ParentedReflectedProperty(this, propertyInfo, propertyDictionary.Properties[parentPropertyName]);
		if (propertyMapper == null)
		{
			propertyMapper = new Dictionary<string, string>();
		}
		propertyMapper[parentPropertyName] = propertyInfo.Name;
	}

	protected virtual ISfcPropertySet GetParentSfcPropertySet()
	{
		return parent.GetPropertySet();
	}

	protected virtual void OnPropertyChanged(string propertyName)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	protected virtual void parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (propertyMapper != null && propertyMapper.TryGetValue(e.PropertyName, out var value))
		{
			OnPropertyChanged(value);
		}
	}

	protected virtual void OnPropertyMetadataChanged(string propertyName)
	{
		if (this.PropertyMetadataChanged != null)
		{
			this.PropertyMetadataChanged(this, new SfcPropertyMetadataChangedEventArgs(propertyName));
		}
	}

	protected virtual void parent_PropertyMetadataChanged(object sender, SfcPropertyMetadataChangedEventArgs e)
	{
		if (propertyMapper != null && propertyMapper.TryGetValue(e.PropertyName, out var value))
		{
			OnPropertyMetadataChanged(value);
		}
	}

	public ISfcPropertySet GetPropertySet()
	{
		return PropertyDictionary;
	}
}
