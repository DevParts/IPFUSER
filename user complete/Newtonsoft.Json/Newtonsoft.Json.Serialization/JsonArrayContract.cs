using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization;

/// <summary>
/// Contract details for a <see cref="T:System.Type" /> used by the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
/// </summary>
public class JsonArrayContract : JsonContainerContract
{
	private readonly Type? _genericCollectionDefinitionType;

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
	private Type? _genericWrapperType;

	private ObjectConstructor<object>? _genericWrapperCreator;

	private Func<object>? _genericTemporaryCollectionCreator;

	private readonly ConstructorInfo? _parameterizedConstructor;

	private ObjectConstructor<object>? _parameterizedCreator;

	private ObjectConstructor<object>? _overrideCreator;

	/// <summary>
	/// Gets the <see cref="T:System.Type" /> of the collection items.
	/// </summary>
	/// <value>The <see cref="T:System.Type" /> of the collection items.</value>
	public Type? CollectionItemType { get; }

	/// <summary>
	/// Gets a value indicating whether the collection type is a multidimensional array.
	/// </summary>
	/// <value><c>true</c> if the collection type is a multidimensional array; otherwise, <c>false</c>.</value>
	public bool IsMultidimensionalArray { get; }

	internal bool IsArray { get; }

	internal bool ShouldCreateWrapper { get; }

	internal bool CanDeserialize { get; private set; }

	internal ObjectConstructor<object>? ParameterizedCreator
	{
		[RequiresDynamicCode("Newtonsoft.Json relies on dynamically creating types that may not be available with Ahead of Time compilation.")]
		get
		{
			if (_parameterizedCreator == null && _parameterizedConstructor != null)
			{
				_parameterizedCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(_parameterizedConstructor);
			}
			return _parameterizedCreator;
		}
	}

	/// <summary>
	/// Gets or sets the function used to create the object. When set this function will override <see cref="P:Newtonsoft.Json.Serialization.JsonContract.DefaultCreator" />.
	/// </summary>
	/// <value>The function used to create the object.</value>
	public ObjectConstructor<object>? OverrideCreator
	{
		get
		{
			return _overrideCreator;
		}
		set
		{
			_overrideCreator = value;
			CanDeserialize = true;
		}
	}

	/// <summary>
	/// Gets a value indicating whether the creator has a parameter with the collection values.
	/// </summary>
	/// <value><c>true</c> if the creator has a parameter with the collection values; otherwise, <c>false</c>.</value>
	public bool HasParameterizedCreator { get; set; }

	internal bool HasParameterizedCreatorInternal
	{
		get
		{
			if (!HasParameterizedCreator && _parameterizedCreator == null)
			{
				return _parameterizedConstructor != null;
			}
			return true;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Serialization.JsonArrayContract" /> class.
	/// </summary>
	/// <param name="underlyingType">The underlying type for the contract.</param>
	[RequiresUnreferencedCode("Newtonsoft.Json relies on reflection over types that may be removed when trimming.")]
	[RequiresDynamicCode("Newtonsoft.Json relies on dynamically creating types that may not be available with Ahead of Time compilation.")]
	public JsonArrayContract(Type underlyingType)
		: base(underlyingType)
	{
		ContractType = JsonContractType.Array;
		IsArray = base.CreatedType.IsArray || (NonNullableUnderlyingType.IsGenericType() && NonNullableUnderlyingType.GetGenericTypeDefinition().FullName == "System.Linq.EmptyPartition`1");
		bool canDeserialize;
		Type implementingType;
		if (IsArray)
		{
			CollectionItemType = ReflectionUtils.GetCollectionItemType(base.UnderlyingType);
			IsReadOnlyOrFixedSize = true;
			_genericCollectionDefinitionType = typeof(List<>).MakeGenericType(CollectionItemType);
			canDeserialize = true;
			IsMultidimensionalArray = base.CreatedType.IsArray && base.UnderlyingType.GetArrayRank() > 1;
		}
		else if (typeof(IList).IsAssignableFrom(NonNullableUnderlyingType))
		{
			if (ReflectionUtils.ImplementsGenericDefinition(NonNullableUnderlyingType, typeof(ICollection<>), out _genericCollectionDefinitionType))
			{
				CollectionItemType = _genericCollectionDefinitionType.GetGenericArguments()[0];
			}
			else
			{
				CollectionItemType = ReflectionUtils.GetCollectionItemType(NonNullableUnderlyingType);
			}
			if (NonNullableUnderlyingType == typeof(IList))
			{
				base.CreatedType = typeof(List<object>);
			}
			if (CollectionItemType != null)
			{
				_parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(NonNullableUnderlyingType, CollectionItemType);
			}
			IsReadOnlyOrFixedSize = ReflectionUtils.InheritsGenericDefinition(NonNullableUnderlyingType, typeof(ReadOnlyCollection<>));
			canDeserialize = true;
		}
		else if (ReflectionUtils.ImplementsGenericDefinition(NonNullableUnderlyingType, typeof(ICollection<>), out _genericCollectionDefinitionType))
		{
			CollectionItemType = _genericCollectionDefinitionType.GetGenericArguments()[0];
			if (ReflectionUtils.IsGenericDefinition(NonNullableUnderlyingType, typeof(ICollection<>)) || ReflectionUtils.IsGenericDefinition(NonNullableUnderlyingType, typeof(IList<>)))
			{
				base.CreatedType = typeof(List<>).MakeGenericType(CollectionItemType);
			}
			if (ReflectionUtils.IsGenericDefinition(NonNullableUnderlyingType, typeof(ISet<>)))
			{
				base.CreatedType = typeof(HashSet<>).MakeGenericType(CollectionItemType);
			}
			_parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(NonNullableUnderlyingType, CollectionItemType);
			canDeserialize = true;
			ShouldCreateWrapper = true;
		}
		else if (ReflectionUtils.ImplementsGenericDefinition(NonNullableUnderlyingType, typeof(IReadOnlyCollection<>), out implementingType))
		{
			CollectionItemType = implementingType.GetGenericArguments()[0];
			if (ReflectionUtils.IsGenericDefinition(NonNullableUnderlyingType, typeof(IReadOnlyCollection<>)) || ReflectionUtils.IsGenericDefinition(NonNullableUnderlyingType, typeof(IReadOnlyList<>)))
			{
				base.CreatedType = typeof(ReadOnlyCollection<>).MakeGenericType(CollectionItemType);
			}
			_genericCollectionDefinitionType = typeof(List<>).MakeGenericType(CollectionItemType);
			_parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(base.CreatedType, CollectionItemType);
			StoreFSharpListCreatorIfNecessary(NonNullableUnderlyingType);
			IsReadOnlyOrFixedSize = true;
			canDeserialize = HasParameterizedCreatorInternal;
		}
		else if (ReflectionUtils.ImplementsGenericDefinition(NonNullableUnderlyingType, typeof(IEnumerable<>), out implementingType))
		{
			CollectionItemType = implementingType.GetGenericArguments()[0];
			if (ReflectionUtils.IsGenericDefinition(base.UnderlyingType, typeof(IEnumerable<>)))
			{
				base.CreatedType = typeof(List<>).MakeGenericType(CollectionItemType);
			}
			_parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(NonNullableUnderlyingType, CollectionItemType);
			StoreFSharpListCreatorIfNecessary(NonNullableUnderlyingType);
			if (NonNullableUnderlyingType.IsGenericType() && NonNullableUnderlyingType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			{
				_genericCollectionDefinitionType = implementingType;
				IsReadOnlyOrFixedSize = false;
				ShouldCreateWrapper = false;
				canDeserialize = true;
			}
			else
			{
				_genericCollectionDefinitionType = typeof(List<>).MakeGenericType(CollectionItemType);
				IsReadOnlyOrFixedSize = true;
				ShouldCreateWrapper = true;
				canDeserialize = HasParameterizedCreatorInternal;
			}
		}
		else
		{
			canDeserialize = false;
			ShouldCreateWrapper = true;
		}
		CanDeserialize = canDeserialize;
		if (CollectionItemType != null && ImmutableCollectionsUtils.TryBuildImmutableForArrayContract(NonNullableUnderlyingType, CollectionItemType, out Type createdType, out ObjectConstructor<object> parameterizedCreator))
		{
			base.CreatedType = createdType;
			_parameterizedCreator = parameterizedCreator;
			IsReadOnlyOrFixedSize = true;
			CanDeserialize = true;
		}
	}

	[RequiresDynamicCode("Newtonsoft.Json relies on dynamically creating types that may not be available with Ahead of Time compilation.")]
	internal IWrappedCollection CreateWrapper(object list)
	{
		if (_genericWrapperCreator == null)
		{
			_genericWrapperType = typeof(CollectionWrapper<>).MakeGenericType(CollectionItemType);
			Type type = ((!ReflectionUtils.InheritsGenericDefinition(_genericCollectionDefinitionType, typeof(List<>)) && !(_genericCollectionDefinitionType.GetGenericTypeDefinition() == typeof(IEnumerable<>))) ? _genericCollectionDefinitionType : typeof(ICollection<>).MakeGenericType(CollectionItemType));
			ConstructorInfo constructor = _genericWrapperType.GetConstructor(new Type[1] { type });
			_genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructor);
		}
		return (IWrappedCollection)_genericWrapperCreator(list);
	}

	[RequiresDynamicCode("Newtonsoft.Json relies on dynamically creating types that may not be available with Ahead of Time compilation.")]
	internal IList CreateTemporaryCollection()
	{
		if (_genericTemporaryCollectionCreator == null)
		{
			Type type = ((IsMultidimensionalArray || CollectionItemType == null) ? typeof(object) : CollectionItemType);
			Type type2 = typeof(List<>).MakeGenericType(type);
			_genericTemporaryCollectionCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(type2);
		}
		return (IList)_genericTemporaryCollectionCreator();
	}

	[RequiresUnreferencedCode("Newtonsoft.Json relies on reflection over types that may be removed when trimming.")]
	[RequiresDynamicCode("Newtonsoft.Json relies on dynamically creating types that may not be available with Ahead of Time compilation.")]
	private void StoreFSharpListCreatorIfNecessary(Type underlyingType)
	{
		if (!HasParameterizedCreatorInternal && underlyingType.Name == "FSharpList`1")
		{
			FSharpUtils.EnsureInitialized(underlyingType.Assembly());
			_parameterizedCreator = FSharpUtils.Instance.CreateSeq(CollectionItemType);
		}
	}
}
