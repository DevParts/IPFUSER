using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization;

/// <summary>
/// Contract details for a <see cref="T:System.Type" /> used by the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
/// </summary>
public abstract class JsonContract
{
	internal bool IsNullable;

	internal bool IsConvertable;

	internal bool IsEnum;

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
	internal Type NonNullableUnderlyingType;

	internal ReadType InternalReadType;

	internal JsonContractType ContractType;

	internal bool IsReadOnlyOrFixedSize;

	internal bool IsSealed;

	internal bool IsInstantiable;

	private List<SerializationCallback>? _onDeserializedCallbacks;

	private List<SerializationCallback>? _onDeserializingCallbacks;

	private List<SerializationCallback>? _onSerializedCallbacks;

	private List<SerializationCallback>? _onSerializingCallbacks;

	private List<SerializationErrorCallback>? _onErrorCallbacks;

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
	private Type _createdType;

	/// <summary>
	/// Gets the underlying type for the contract.
	/// </summary>
	/// <value>The underlying type for the contract.</value>
	public Type UnderlyingType { get; }

	/// <summary>
	/// Gets or sets the type created during deserialization.
	/// </summary>
	/// <value>The type created during deserialization.</value>
	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
	public Type CreatedType
	{
		get
		{
			return _createdType;
		}
		set
		{
			ValidationUtils.ArgumentNotNull(value, "value");
			_createdType = value;
			IsSealed = _createdType.IsSealed();
			IsInstantiable = !_createdType.IsInterface() && !_createdType.IsAbstract();
		}
	}

	/// <summary>
	/// Gets or sets whether this type contract is serialized as a reference.
	/// </summary>
	/// <value>Whether this type contract is serialized as a reference.</value>
	public bool? IsReference { get; set; }

	/// <summary>
	/// Gets or sets the default <see cref="T:Newtonsoft.Json.JsonConverter" /> for this contract.
	/// </summary>
	/// <value>The converter.</value>
	public JsonConverter? Converter { get; set; }

	/// <summary>
	/// Gets the internally resolved <see cref="T:Newtonsoft.Json.JsonConverter" /> for the contract's type.
	/// This converter is used as a fallback converter when no other converter is resolved.
	/// Setting <see cref="P:Newtonsoft.Json.Serialization.JsonContract.Converter" /> will always override this converter.
	/// </summary>
	public JsonConverter? InternalConverter { get; internal set; }

	/// <summary>
	/// Gets or sets all methods called immediately after deserialization of the object.
	/// </summary>
	/// <value>The methods called immediately after deserialization of the object.</value>
	public IList<SerializationCallback> OnDeserializedCallbacks
	{
		get
		{
			if (_onDeserializedCallbacks == null)
			{
				_onDeserializedCallbacks = new List<SerializationCallback>();
			}
			return _onDeserializedCallbacks;
		}
	}

	/// <summary>
	/// Gets or sets all methods called during deserialization of the object.
	/// </summary>
	/// <value>The methods called during deserialization of the object.</value>
	public IList<SerializationCallback> OnDeserializingCallbacks
	{
		get
		{
			if (_onDeserializingCallbacks == null)
			{
				_onDeserializingCallbacks = new List<SerializationCallback>();
			}
			return _onDeserializingCallbacks;
		}
	}

	/// <summary>
	/// Gets or sets all methods called after serialization of the object graph.
	/// </summary>
	/// <value>The methods called after serialization of the object graph.</value>
	public IList<SerializationCallback> OnSerializedCallbacks
	{
		get
		{
			if (_onSerializedCallbacks == null)
			{
				_onSerializedCallbacks = new List<SerializationCallback>();
			}
			return _onSerializedCallbacks;
		}
	}

	/// <summary>
	/// Gets or sets all methods called before serialization of the object.
	/// </summary>
	/// <value>The methods called before serialization of the object.</value>
	public IList<SerializationCallback> OnSerializingCallbacks
	{
		get
		{
			if (_onSerializingCallbacks == null)
			{
				_onSerializingCallbacks = new List<SerializationCallback>();
			}
			return _onSerializingCallbacks;
		}
	}

	/// <summary>
	/// Gets or sets all method called when an error is thrown during the serialization of the object.
	/// </summary>
	/// <value>The methods called when an error is thrown during the serialization of the object.</value>
	public IList<SerializationErrorCallback> OnErrorCallbacks
	{
		get
		{
			if (_onErrorCallbacks == null)
			{
				_onErrorCallbacks = new List<SerializationErrorCallback>();
			}
			return _onErrorCallbacks;
		}
	}

	/// <summary>
	/// Gets or sets the default creator method used to create the object.
	/// </summary>
	/// <value>The default creator method used to create the object.</value>
	public Func<object>? DefaultCreator { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the default creator is non-public.
	/// </summary>
	/// <value><c>true</c> if the default object creator is non-public; otherwise, <c>false</c>.</value>
	public bool DefaultCreatorNonPublic { get; set; }

	[RequiresUnreferencedCode("Newtonsoft.Json relies on reflection over types that may be removed when trimming.")]
	internal JsonContract([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type underlyingType)
	{
		ValidationUtils.ArgumentNotNull(underlyingType, "underlyingType");
		UnderlyingType = underlyingType;
		underlyingType = ReflectionUtils.EnsureNotByRefType(underlyingType);
		IsNullable = ReflectionUtils.IsNullable(underlyingType);
		NonNullableUnderlyingType = ((IsNullable && ReflectionUtils.IsNullableType(underlyingType)) ? Nullable.GetUnderlyingType(underlyingType) : underlyingType);
		_createdType = (CreatedType = NonNullableUnderlyingType);
		IsConvertable = ConvertUtils.IsConvertible(NonNullableUnderlyingType);
		IsEnum = NonNullableUnderlyingType.IsEnum();
		InternalReadType = ReadType.Read;
	}

	internal void InvokeOnSerializing(object o, StreamingContext context)
	{
		if (_onSerializingCallbacks == null)
		{
			return;
		}
		foreach (SerializationCallback onSerializingCallback in _onSerializingCallbacks)
		{
			onSerializingCallback(o, context);
		}
	}

	internal void InvokeOnSerialized(object o, StreamingContext context)
	{
		if (_onSerializedCallbacks == null)
		{
			return;
		}
		foreach (SerializationCallback onSerializedCallback in _onSerializedCallbacks)
		{
			onSerializedCallback(o, context);
		}
	}

	internal void InvokeOnDeserializing(object o, StreamingContext context)
	{
		if (_onDeserializingCallbacks == null)
		{
			return;
		}
		foreach (SerializationCallback onDeserializingCallback in _onDeserializingCallbacks)
		{
			onDeserializingCallback(o, context);
		}
	}

	internal void InvokeOnDeserialized(object o, StreamingContext context)
	{
		if (_onDeserializedCallbacks == null)
		{
			return;
		}
		foreach (SerializationCallback onDeserializedCallback in _onDeserializedCallbacks)
		{
			onDeserializedCallback(o, context);
		}
	}

	internal void InvokeOnError(object o, StreamingContext context, ErrorContext errorContext)
	{
		if (_onErrorCallbacks == null)
		{
			return;
		}
		foreach (SerializationErrorCallback onErrorCallback in _onErrorCallbacks)
		{
			onErrorCallback(o, context, errorContext);
		}
	}

	internal static SerializationCallback CreateSerializationCallback(MethodInfo callbackMethodInfo)
	{
		return delegate(object o, StreamingContext context)
		{
			callbackMethodInfo.Invoke(o, new object[1] { context });
		};
	}

	internal static SerializationErrorCallback CreateSerializationErrorCallback(MethodInfo callbackMethodInfo)
	{
		return delegate(object o, StreamingContext context, ErrorContext econtext)
		{
			callbackMethodInfo.Invoke(o, new object[2] { context, econtext });
		};
	}
}
