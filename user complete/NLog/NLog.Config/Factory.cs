using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NLog.Common;
using NLog.Internal;

namespace NLog.Config;

/// <summary>
/// Factory for class-based items.
/// </summary>
/// <typeparam name="TBaseType">The base type of each item.</typeparam>
/// <typeparam name="TAttributeType">The type of the attribute used to annotate items.</typeparam>
internal class Factory<TBaseType, TAttributeType> : IFactory, IFactory<TBaseType> where TBaseType : class where TAttributeType : NameBaseAttribute
{
	private delegate Type GetTypeDelegate();

	private readonly Dictionary<string, Func<TBaseType?>> _items;

	private readonly ConfigurationItemFactory _parentFactory;

	public bool Initialized { get; private set; }

	internal Factory(ConfigurationItemFactory parentFactory)
	{
		_parentFactory = parentFactory;
		_items = new Dictionary<string, Func<TBaseType>>(16, StringComparer.OrdinalIgnoreCase);
	}

	public void Initialize(Action<bool> itemRegistration)
	{
		lock (ConfigurationItemFactory.SyncRoot)
		{
			if (Initialized)
			{
				return;
			}
			try
			{
				bool obj = _items.Count == 0;
				itemRegistration(obj);
			}
			finally
			{
				Initialized = true;
			}
		}
	}

	public bool CheckTypeAliasExists(string typeAlias)
	{
		return _items.ContainsKey(FactoryExtensions.NormalizeName(typeAlias));
	}

	public void RegisterType<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties)] TType>(string typeAlias) where TType : TBaseType, new()
	{
		typeAlias = FactoryExtensions.NormalizeName(typeAlias);
		lock (ConfigurationItemFactory.SyncRoot)
		{
			_parentFactory.RegisterTypeProperties<TType>(() => new TType());
			_items[typeAlias] = () => (TBaseType)(object)new TType();
		}
	}

	public void RegisterType<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties)] TType>(string typeAlias, Func<TType> itemCreator) where TType : TBaseType
	{
		typeAlias = FactoryExtensions.NormalizeName(typeAlias);
		lock (ConfigurationItemFactory.SyncRoot)
		{
			_parentFactory.RegisterTypeProperties<TType>(() => itemCreator());
			_items[typeAlias] = () => (TBaseType)(object)itemCreator();
		}
	}

	/// <summary>
	/// Registers the type.
	/// </summary>
	/// <param name="type">The type to register.</param>
	/// <param name="itemNamePrefix">The item name prefix.</param>
	public void RegisterType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicProperties)] Type type, string itemNamePrefix)
	{
		if (!typeof(TBaseType).IsAssignableFrom(type))
		{
			return;
		}
		IEnumerable<TAttributeType> customAttributes = type.GetCustomAttributes<TAttributeType>(inherit: false);
		if (customAttributes == null)
		{
			return;
		}
		foreach (TAttributeType item in customAttributes)
		{
			RegisterDefinition(type, item.Name, itemNamePrefix);
		}
	}

	/// <summary>
	/// Registers the item based on a type name.
	/// </summary>
	/// <param name="itemName">Name of the item.</param>
	/// <param name="typeName">Name of the type.</param>
	[Obsolete("Instead use RegisterType<T>, as dynamic Assembly loading will be moved out. Marked obsolete with NLog v5.2")]
	[UnconditionalSuppressMessage("Trimming - Ignore since obsolete", "IL2072")]
	public void RegisterNamedType(string itemName, string typeName)
	{
		itemName = FactoryExtensions.NormalizeName(itemName);
		Type itemType = null;
		GetTypeDelegate typeLookup = delegate
		{
			if ((object)itemType == null)
			{
				InternalLogger.Debug("Object reflection needed to resolve type: {0}", typeName);
				itemType = PropertyTypeConverter.ConvertToType(typeName, throwOnError: false);
			}
			return itemType;
		};
		Func<TBaseType> value = delegate
		{
			Type type = typeLookup();
			return (!(type != null)) ? null : ((TBaseType)Activator.CreateInstance(type));
		};
		lock (ConfigurationItemFactory.SyncRoot)
		{
			_items[itemName] = value;
		}
	}

	/// <summary>
	/// Clears the contents of the factory.
	/// </summary>
	public virtual void Clear()
	{
		lock (ConfigurationItemFactory.SyncRoot)
		{
			_items.Clear();
		}
	}

	[Obsolete("Instead use RegisterType<T>, as dynamic Assembly loading will be moved out. Marked obsolete with NLog v5.2")]
	public void RegisterDefinition(string typeAlias, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties)] Type itemType)
	{
		if (string.IsNullOrEmpty(typeAlias))
		{
			throw new ArgumentException("Missing NLog " + typeof(TBaseType).Name + " type-alias", "typeAlias");
		}
		if (!typeof(TBaseType).IsAssignableFrom(itemType))
		{
			throw new ArgumentException("Not of type NLog " + typeof(TBaseType).Name, "itemType");
		}
		RegisterDefinition(itemType, typeAlias, string.Empty);
	}

	private void RegisterDefinition([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties)] Type itemType, string typeAlias, string itemNamePrefix)
	{
		typeAlias = FactoryExtensions.NormalizeName(typeAlias);
		Func<TBaseType> itemCreator = () => (TBaseType)Activator.CreateInstance(itemType);
		lock (ConfigurationItemFactory.SyncRoot)
		{
			_parentFactory.RegisterTypeProperties(itemType, () => itemCreator());
			_items[itemNamePrefix + typeAlias] = () => itemCreator();
		}
	}

	private bool TryGetItemFactory(string typeAlias, out Func<TBaseType?>? itemFactory)
	{
		lock (ConfigurationItemFactory.SyncRoot)
		{
			return _items.TryGetValue(typeAlias, out itemFactory);
		}
	}

	/// <inheritdoc />
	public virtual bool TryCreateInstance(string typeAlias, out TBaseType? result)
	{
		typeAlias = FactoryExtensions.NormalizeName(typeAlias);
		if (!TryGetItemFactory(typeAlias, out Func<TBaseType> itemFactory) || itemFactory == null)
		{
			result = null;
			return false;
		}
		result = itemFactory();
		return result != null;
	}
}
