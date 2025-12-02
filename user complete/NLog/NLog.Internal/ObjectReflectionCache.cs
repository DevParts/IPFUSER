using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using NLog.Common;
using NLog.Config;

namespace NLog.Internal;

/// <summary>
/// Converts object into a List of property-names and -values using reflection
/// </summary>
internal sealed class ObjectReflectionCache : IObjectTypeTransformer
{
	internal struct ObjectPropertyList : IEnumerable<ObjectPropertyList.PropertyValue>, IEnumerable
	{
		public struct PropertyValue
		{
			public readonly string Name;

			public readonly object? Value;

			private readonly TypeCode _typecode;

			public TypeCode TypeCode
			{
				get
				{
					if (Value != null)
					{
						return _typecode;
					}
					return TypeCode.Empty;
				}
			}

			public bool HasNameAndValue
			{
				get
				{
					if (Name != null)
					{
						return Value != null;
					}
					return false;
				}
			}

			public PropertyValue(string name, object value, TypeCode typeCode)
			{
				Name = name;
				Value = value;
				_typecode = typeCode;
			}

			public PropertyValue(object owner, PropertyInfo propertyInfo)
			{
				Name = propertyInfo.Name;
				_typecode = TypeCode.Object;
				try
				{
					Value = propertyInfo.GetValue(owner, null);
				}
				catch
				{
					Value = null;
				}
			}

			public PropertyValue(object owner, FastPropertyLookup fastProperty)
			{
				Name = fastProperty.Name;
				_typecode = fastProperty.TypeCode;
				try
				{
					Value = fastProperty.ValueLookup(owner, ArrayHelper.Empty<object>());
				}
				catch
				{
					Value = null;
				}
			}
		}

		public struct Enumerator : IEnumerator<PropertyValue>, IDisposable, IEnumerator
		{
			private readonly object _owner;

			private readonly PropertyInfo[]? _properties;

			private readonly FastPropertyLookup[]? _fastLookup;

			private readonly IEnumerator<KeyValuePair<string, object>>? _enumerator;

			private int _index;

			public PropertyValue Current
			{
				get
				{
					try
					{
						if (_fastLookup != null)
						{
							return new PropertyValue(_owner, _fastLookup[_index]);
						}
						if (_properties != null)
						{
							return new PropertyValue(_owner, _properties[_index]);
						}
						if (_enumerator != null)
						{
							return new PropertyValue(_enumerator.Current.Key, _enumerator.Current.Value, TypeCode.Object);
						}
						return default(PropertyValue);
					}
					catch (Exception ex)
					{
						InternalLogger.Debug(ex, "Failed to get property value for object: {0}", _owner);
						return default(PropertyValue);
					}
				}
			}

			object IEnumerator.Current => Current;

			internal Enumerator(object owner, PropertyInfo[] properties, FastPropertyLookup[]? fastLookup)
			{
				_owner = owner;
				_properties = properties;
				_fastLookup = fastLookup;
				_index = -1;
				_enumerator = null;
			}

			internal Enumerator(IEnumerator<KeyValuePair<string, object>> enumerator)
			{
				_owner = enumerator;
				_properties = null;
				_fastLookup = null;
				_index = 0;
				_enumerator = enumerator;
			}

			public void Dispose()
			{
				_enumerator?.Dispose();
			}

			public bool MoveNext()
			{
				if (_properties != null)
				{
					int num = ++_index;
					FastPropertyLookup[]? fastLookup = _fastLookup;
					return num < ((fastLookup != null) ? fastLookup.Length : _properties.Length);
				}
				return _enumerator?.MoveNext() ?? false;
			}

			public void Reset()
			{
				if (_properties != null)
				{
					_index = -1;
				}
				else
				{
					_enumerator?.Reset();
				}
			}
		}

		internal static readonly StringComparer NameComparer = StringComparer.Ordinal;

		private static readonly FastPropertyLookup[] CreateIDictionaryEnumerator = new FastPropertyLookup[1]
		{
			new FastPropertyLookup(string.Empty, TypeCode.Object, (object o, object?[] p) => ((IDictionary<string, object>)o).GetEnumerator())
		};

		private readonly object _object;

		private readonly PropertyInfo[] _properties;

		private readonly FastPropertyLookup[]? _fastLookup;

		public bool IsSimpleValue
		{
			get
			{
				if (_properties.Length == 0)
				{
					if (_fastLookup != null)
					{
						return _fastLookup.Length == 0;
					}
					return true;
				}
				return false;
			}
		}

		public object ObjectValue => _object;

		internal ObjectPropertyList(object value, PropertyInfo[] properties, FastPropertyLookup[]? fastLookup)
		{
			_object = value;
			_properties = properties;
			_fastLookup = fastLookup;
		}

		public ObjectPropertyList(IDictionary<string, object> value)
		{
			_object = value;
			_properties = ArrayHelper.Empty<PropertyInfo>();
			_fastLookup = CreateIDictionaryEnumerator;
		}

		public bool TryGetPropertyValue(string name, out PropertyValue propertyValue)
		{
			if (_properties.Length == 0)
			{
				if (_object is IDictionary<string, object> dictionary)
				{
					if (dictionary.TryGetValue(name, out var value))
					{
						propertyValue = new PropertyValue(name, value, TypeCode.Object);
						return true;
					}
					propertyValue = default(PropertyValue);
					return false;
				}
				return TryListLookupPropertyValue(name, out propertyValue);
			}
			if (_fastLookup != null)
			{
				int hashCode = NameComparer.GetHashCode(name);
				FastPropertyLookup[] fastLookup = _fastLookup;
				for (int i = 0; i < fastLookup.Length; i++)
				{
					FastPropertyLookup fastProperty = fastLookup[i];
					if (fastProperty.NameHashCode == hashCode && NameComparer.Equals(fastProperty.Name, name))
					{
						propertyValue = new PropertyValue(_object, fastProperty);
						return true;
					}
				}
				propertyValue = default(PropertyValue);
				return false;
			}
			return TrySlowLookupPropertyValue(name, out propertyValue);
		}

		/// <summary>
		/// Scans properties for name (Skips property value lookup until finding match)
		/// </summary>
		private bool TrySlowLookupPropertyValue(string name, out PropertyValue propertyValue)
		{
			PropertyInfo[] properties = _properties;
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (NameComparer.Equals(propertyInfo.Name, name))
				{
					propertyValue = new PropertyValue(_object, propertyInfo);
					return true;
				}
			}
			propertyValue = default(PropertyValue);
			return false;
		}

		/// <summary>
		/// Scans properties for name
		/// </summary>
		private bool TryListLookupPropertyValue(string name, out PropertyValue propertyValue)
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PropertyValue current = enumerator.Current;
					if (NameComparer.Equals(current.Name, name))
					{
						propertyValue = current;
						return true;
					}
				}
			}
			propertyValue = default(PropertyValue);
			return false;
		}

		public override string ToString()
		{
			return _object?.ToString() ?? "null";
		}

		public Enumerator GetEnumerator()
		{
			if (_properties.Length != 0 || _fastLookup == null || _fastLookup.Length == 0)
			{
				return new Enumerator(_object, _properties, _fastLookup);
			}
			object obj = _fastLookup[0].ValueLookup(_object, ArrayHelper.Empty<object>());
			if (obj != null)
			{
				return new Enumerator((IEnumerator<KeyValuePair<string, object>>)obj);
			}
			return default(Enumerator);
		}

		IEnumerator<PropertyValue> IEnumerable<PropertyValue>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	internal struct FastPropertyLookup
	{
		public readonly string Name;

		public readonly ReflectionHelpers.LateBoundMethod ValueLookup;

		public readonly TypeCode TypeCode;

		public readonly int NameHashCode;

		public FastPropertyLookup(string name, TypeCode typeCode, ReflectionHelpers.LateBoundMethod valueLookup)
		{
			Name = name;
			ValueLookup = valueLookup;
			TypeCode = typeCode;
			NameHashCode = ObjectPropertyList.NameComparer.GetHashCode(name);
		}
	}

	private struct ObjectPropertyInfos : IEquatable<ObjectPropertyInfos>
	{
		public readonly PropertyInfo[] Properties;

		public readonly FastPropertyLookup[]? FastLookup;

		public static readonly ObjectPropertyInfos SimpleToString = new ObjectPropertyInfos(ArrayHelper.Empty<PropertyInfo>(), ArrayHelper.Empty<FastPropertyLookup>());

		public bool HasFastLookup => FastLookup != null;

		public ObjectPropertyInfos(PropertyInfo[] properties, FastPropertyLookup[]? fastLookup)
		{
			Properties = properties;
			FastLookup = fastLookup;
		}

		public bool Equals(ObjectPropertyInfos other)
		{
			if (Properties == other.Properties)
			{
				return FastLookup?.Length == other.FastLookup?.Length;
			}
			return false;
		}
	}

	/// <summary>
	/// Binder for retrieving value of <see cref="T:System.Dynamic.DynamicObject" />
	/// </summary>
	private sealed class GetBinderAdapter : GetMemberBinder
	{
		internal GetBinderAdapter(string name)
			: base(name, ignoreCase: false)
		{
		}

		/// <inheritdoc />
		public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
		{
			return target;
		}
	}

	private interface IDictionaryEnumerator
	{
		IEnumerator<KeyValuePair<string, object?>> GetEnumerator(object value);
	}

	private sealed class DictionaryEnumerator : IDictionaryEnumerator
	{
		private Func<IEnumerable, IEnumerator<KeyValuePair<string, object?>>>? _enumerateCollection;

		IEnumerator<KeyValuePair<string, object?>> IDictionaryEnumerator.GetEnumerator(object value)
		{
			if (value is IDictionary dictionary)
			{
				if (dictionary.Count > 0)
				{
					return YieldEnumerator(dictionary);
				}
			}
			else if (value is IEnumerable collection)
			{
				return YieldEnumerator(collection);
			}
			return EmptyDictionaryEnumerator.Default;
		}

		private static IEnumerator<KeyValuePair<string, object?>> YieldEnumerator(IDictionary dictionary)
		{
			foreach (DictionaryEntry item in new DictionaryEntryEnumerable(dictionary))
			{
				yield return new KeyValuePair<string, object>(item.Key?.ToString() ?? string.Empty, item.Value);
			}
		}

		private IEnumerator<KeyValuePair<string, object?>> YieldEnumerator(IEnumerable collection)
		{
			if (_enumerateCollection == null)
			{
				IEnumerator enumerator = collection.GetEnumerator();
				if (!enumerator.MoveNext())
				{
					return EmptyDictionaryEnumerator.Default;
				}
				_enumerateCollection = BuildEnumerator(enumerator.Current);
			}
			return _enumerateCollection(collection);
		}

		[UnconditionalSuppressMessage("Trimming - Allow reflection of message args", "IL2075")]
		private static Func<IEnumerable, IEnumerator<KeyValuePair<string, object?>>> BuildEnumerator(object firstItem)
		{
			if (firstItem.GetType().IsGenericType && firstItem.GetType().GetGenericTypeDefinition() == typeof(KeyValuePair<, >))
			{
				PropertyInfo getKeyProperty = firstItem.GetType().GetProperty("Key");
				PropertyInfo getValueProperty = firstItem.GetType().GetProperty("Value");
				return (IEnumerable collection) => EnumerateItems(collection, getKeyProperty, getValueProperty);
			}
			return (IEnumerable collection) => EmptyDictionaryEnumerator.Default;
		}

		private static IEnumerator<KeyValuePair<string, object?>> EnumerateItems(IEnumerable collection, PropertyInfo getKeyProperty, PropertyInfo getValueProperty)
		{
			foreach (object item in collection)
			{
				object value = getKeyProperty.GetValue(item, null);
				object value2 = getValueProperty.GetValue(item, null);
				yield return new KeyValuePair<string, object>(value?.ToString() ?? string.Empty, value2);
			}
		}
	}

	private sealed class DictionaryEnumerator<TKey, TValue> : IDictionaryEnumerator
	{
		public IEnumerator<KeyValuePair<string, object?>> GetEnumerator(object value)
		{
			if (value is IDictionary<TKey, TValue> dictionary)
			{
				if (dictionary.Count > 0)
				{
					return YieldEnumerator(dictionary);
				}
			}
			else if (value is IReadOnlyDictionary<TKey, TValue> { Count: >0 } readOnlyDictionary)
			{
				return YieldEnumerator(readOnlyDictionary);
			}
			return EmptyDictionaryEnumerator.Default;
		}

		private static IEnumerator<KeyValuePair<string, object?>> YieldEnumerator(IDictionary<TKey, TValue> dictionary)
		{
			foreach (KeyValuePair<TKey, TValue> item in dictionary)
			{
				TKey key = item.Key;
				yield return new KeyValuePair<string, object>(((key != null) ? key.ToString() : null) ?? string.Empty, item.Value);
			}
		}

		private static IEnumerator<KeyValuePair<string, object?>> YieldEnumerator(IReadOnlyDictionary<TKey, TValue> dictionary)
		{
			foreach (KeyValuePair<TKey, TValue> item in dictionary)
			{
				TKey key = item.Key;
				yield return new KeyValuePair<string, object>(((key != null) ? key.ToString() : null) ?? string.Empty, item.Value);
			}
		}
	}

	private sealed class EmptyDictionaryEnumerator : IEnumerator<KeyValuePair<string, object?>>, IDisposable, IEnumerator
	{
		public static readonly EmptyDictionaryEnumerator Default = new EmptyDictionaryEnumerator();

		KeyValuePair<string, object?> IEnumerator<KeyValuePair<string, object>>.Current => default(KeyValuePair<string, object>);

		object IEnumerator.Current => default(KeyValuePair<string, object>);

		bool IEnumerator.MoveNext()
		{
			return false;
		}

		void IDisposable.Dispose()
		{
		}

		void IEnumerator.Reset()
		{
		}
	}

	private MruCache<Type, ObjectPropertyInfos>? _objectTypeCache;

	private readonly IServiceProvider _serviceProvider;

	private IObjectTypeTransformer? _objectTypeTransformation;

	private MruCache<Type, ObjectPropertyInfos> ObjectTypeCache => _objectTypeCache ?? Interlocked.CompareExchange(ref _objectTypeCache, new MruCache<Type, ObjectPropertyInfos>(10000), null) ?? _objectTypeCache;

	private IObjectTypeTransformer ObjectTypeTransformation => _objectTypeTransformation ?? (_objectTypeTransformation = _serviceProvider?.GetService<IObjectTypeTransformer>() ?? this);

	public ObjectReflectionCache(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	object? IObjectTypeTransformer.TryTransformObject(object? obj)
	{
		return null;
	}

	public ObjectPropertyList LookupObjectProperties(object value)
	{
		if (TryLookupExpandoObject(value, out var objectPropertyList))
		{
			return objectPropertyList;
		}
		if (ObjectTypeTransformation != this)
		{
			object obj = ObjectTypeTransformation.TryTransformObject(value);
			if (obj != null)
			{
				if (obj is IConvertible)
				{
					return new ObjectPropertyList(obj, ObjectPropertyInfos.SimpleToString.Properties, ObjectPropertyInfos.SimpleToString.FastLookup);
				}
				if (TryLookupExpandoObject(obj, out objectPropertyList))
				{
					return objectPropertyList;
				}
				value = obj;
			}
		}
		Type type = value.GetType();
		ObjectPropertyInfos value2 = (ConvertSimpleToString(type) ? ObjectPropertyInfos.SimpleToString : BuildObjectPropertyInfos(value));
		ObjectTypeCache.TryAddValue(type, value2);
		return new ObjectPropertyList(value, value2.Properties, value2.FastLookup);
	}

	/// <summary>
	/// Try get value from <paramref name="value" />, using <paramref name="objectPath" />, and set into <paramref name="foundValue" />
	/// </summary>
	public bool TryGetObjectProperty(object? value, string[]? objectPath, out object? foundValue)
	{
		foundValue = null;
		if (objectPath == null)
		{
			return false;
		}
		for (int i = 0; i < objectPath.Length; i++)
		{
			if (value == null)
			{
				return false;
			}
			if (LookupObjectProperties(value).TryGetPropertyValue(objectPath[i], out var propertyValue))
			{
				value = propertyValue.Value;
				continue;
			}
			foundValue = null;
			return false;
		}
		foundValue = value;
		return true;
	}

	public bool TryLookupExpandoObject(object value, out ObjectPropertyList objectPropertyList)
	{
		if (value is IDictionary<string, object> value2)
		{
			objectPropertyList = new ObjectPropertyList(value2);
			return true;
		}
		if (value is DynamicObject d)
		{
			Dictionary<string, object> value3 = DynamicObjectToDict(d);
			objectPropertyList = new ObjectPropertyList(value3);
			return true;
		}
		Type type = value.GetType();
		if (ObjectTypeCache.TryGetValue(type, out var value4))
		{
			if (!value4.HasFastLookup)
			{
				FastPropertyLookup[] fastLookup = BuildFastLookup(value4.Properties, includeType: false);
				value4 = new ObjectPropertyInfos(value4.Properties, fastLookup);
				ObjectTypeCache.TryAddValue(type, value4);
			}
			objectPropertyList = new ObjectPropertyList(value, value4.Properties, value4.FastLookup);
			return true;
		}
		IDictionaryEnumerator dictionaryEnumerator = TryGetDictionaryEnumerator(value);
		if (dictionaryEnumerator != null)
		{
			value4 = new ObjectPropertyInfos(ArrayHelper.Empty<PropertyInfo>(), new FastPropertyLookup[1]
			{
				new FastPropertyLookup(string.Empty, TypeCode.Object, (object o, object?[] p) => dictionaryEnumerator.GetEnumerator(o))
			});
			ObjectTypeCache.TryAddValue(type, value4);
			objectPropertyList = new ObjectPropertyList(value, value4.Properties, value4.FastLookup);
			return true;
		}
		objectPropertyList = default(ObjectPropertyList);
		return false;
	}

	[UnconditionalSuppressMessage("Trimming - Allow reflection of message args", "IL2072")]
	private static ObjectPropertyInfos BuildObjectPropertyInfos(object value)
	{
		PropertyInfo[] publicProperties = GetPublicProperties(value.GetType());
		if (value is Exception)
		{
			FastPropertyLookup[] fastLookup = BuildFastLookup(publicProperties, includeType: true);
			return new ObjectPropertyInfos(publicProperties, fastLookup);
		}
		if (publicProperties.Length == 0)
		{
			return ObjectPropertyInfos.SimpleToString;
		}
		return new ObjectPropertyInfos(publicProperties, null);
	}

	private static bool ConvertSimpleToString(Type objectType)
	{
		if (typeof(IFormattable).IsAssignableFrom(objectType))
		{
			return true;
		}
		if (typeof(Uri).IsAssignableFrom(objectType))
		{
			return true;
		}
		if (typeof(Delegate).IsAssignableFrom(objectType))
		{
			return true;
		}
		if (typeof(MemberInfo).IsAssignableFrom(objectType))
		{
			return true;
		}
		if (typeof(Assembly).IsAssignableFrom(objectType))
		{
			return true;
		}
		if (typeof(Module).IsAssignableFrom(objectType))
		{
			return true;
		}
		if (typeof(Stream).IsAssignableFrom(objectType))
		{
			return true;
		}
		return false;
	}

	private static PropertyInfo[] GetPublicProperties([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type type)
	{
		PropertyInfo[] array = null;
		try
		{
			array = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
		}
		catch (Exception ex)
		{
			InternalLogger.Warn(ex, "Failed to get object properties for type: {0}", type);
		}
		if (array != null)
		{
			PropertyInfo[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				if (!array2[i].IsValidPublicProperty())
				{
					array = array.Where((PropertyInfo p) => p.IsValidPublicProperty()).ToArray();
					break;
				}
			}
		}
		return array ?? ArrayHelper.Empty<PropertyInfo>();
	}

	private static FastPropertyLookup[] BuildFastLookup(PropertyInfo[] properties, bool includeType)
	{
		int num = (includeType ? 1 : 0);
		FastPropertyLookup[] array = new FastPropertyLookup[properties.Length + num];
		if (includeType)
		{
			array[0] = new FastPropertyLookup("Type", TypeCode.String, (object o, object?[] p) => o.GetType().ToString());
		}
		foreach (PropertyInfo propertyInfo in properties)
		{
			MethodInfo getMethod = propertyInfo.GetGetMethod();
			Type returnType = getMethod.ReturnType;
			ReflectionHelpers.LateBoundMethod valueLookup = ReflectionHelpers.CreateLateBoundMethod(getMethod);
			TypeCode typeCode = Type.GetTypeCode(returnType);
			array[num++] = new FastPropertyLookup(propertyInfo.Name, typeCode, valueLookup);
		}
		return array;
	}

	private static Dictionary<string, object> DynamicObjectToDict(DynamicObject d)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (string dynamicMemberName in d.GetDynamicMemberNames())
		{
			if (d.TryGetMember(new GetBinderAdapter(dynamicMemberName), out var result))
			{
				dictionary[dynamicMemberName] = result;
			}
		}
		return dictionary;
	}

	private static IDictionaryEnumerator? TryGetDictionaryEnumerator(object value)
	{
		if (!(value is IEnumerable) || value is string)
		{
			return null;
		}
		if (value is IDictionary<string, object>)
		{
			return new DictionaryEnumerator<string, object>();
		}
		if (value is IDictionary<string, string>)
		{
			return new DictionaryEnumerator<string, string>();
		}
		if (value is IReadOnlyDictionary<string, object>)
		{
			return new DictionaryEnumerator<string, object>();
		}
		if (value is IReadOnlyDictionary<string, string>)
		{
			return new DictionaryEnumerator<string, string>();
		}
		if (value is IDictionary && value.GetType().IsGenericType)
		{
			if (value.GetType().GetGenericArguments()[0] == typeof(string))
			{
				return new DictionaryEnumerator();
			}
			return null;
		}
		return TryBuildDictionaryEnumerator(value);
	}

	[UnconditionalSuppressMessage("Trimming - Allow reflection of message args", "IL2075")]
	private static IDictionaryEnumerator? TryBuildDictionaryEnumerator(object value)
	{
		Type[] interfaces = value.GetType().GetInterfaces();
		foreach (Type type in interfaces)
		{
			if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IDictionary<, >) || type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<, >)) && type.GetGenericArguments()[0] == typeof(string))
			{
				return (IDictionaryEnumerator)Activator.CreateInstance(typeof(DictionaryEnumerator<, >).MakeGenericType(type.GetGenericArguments()));
			}
		}
		return null;
	}
}
