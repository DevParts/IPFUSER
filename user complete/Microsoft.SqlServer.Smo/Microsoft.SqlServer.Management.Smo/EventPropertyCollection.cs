using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Management;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class EventPropertyCollection : ICollection, IEnumerable<EventProperty>, IEnumerable
{
	private class Collection : KeyedCollection<string, EventProperty>
	{
		protected override string GetKeyForItem(EventProperty item)
		{
			return item.Name;
		}
	}

	private Collection collection;

	public EventProperty this[int index] => collection[index];

	public EventProperty this[string name] => collection[name];

	public int Count => collection.Count;

	bool ICollection.IsSynchronized => ((ICollection)collection).IsSynchronized;

	object ICollection.SyncRoot => ((ICollection)collection).SyncRoot;

	internal EventPropertyCollection(PropertyDataCollection properties)
	{
		collection = new Collection();
		foreach (PropertyData property in properties)
		{
			string name;
			if ((name = property.Name) != null && name == "PostTime")
			{
				collection.Add(new EventProperty(property.Name, ConvertToDateTime((string)property.Value)));
			}
			else
			{
				collection.Add(new EventProperty(property.Name, property.Value));
			}
		}
	}

	public void CopyTo(EventProperty[] array, int index)
	{
		collection.CopyTo(array, index);
	}

	[CLSCompliant(false)]
	public IEnumerator<EventProperty> GetEnumerator()
	{
		return collection.GetEnumerator();
	}

	void ICollection.CopyTo(Array array, int index)
	{
		((ICollection)collection).CopyTo(array, index);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)collection).GetEnumerator();
	}

	private static DateTime ConvertToDateTime(string dateTime)
	{
		char[] value = dateTime.ToCharArray();
		IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
		return new DateTime(int.Parse(new string(value, 0, 4), NumberStyles.None, invariantCulture), int.Parse(new string(value, 4, 2), NumberStyles.None, invariantCulture), int.Parse(new string(value, 6, 2), NumberStyles.None, invariantCulture), int.Parse(new string(value, 8, 2), NumberStyles.None, invariantCulture), int.Parse(new string(value, 10, 2), NumberStyles.None, invariantCulture), int.Parse(new string(value, 12, 2), NumberStyles.None, invariantCulture));
	}

	internal void Add(string name, object value)
	{
		collection.Add(new EventProperty(name, value));
	}
}
