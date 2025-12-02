using System;
using System.Diagnostics.CodeAnalysis;
using NLog.Common;
using NLog.Config;
using NLog.Internal;
using NLog.MessageTemplates;
using NLog.Targets;

namespace NLog;

/// <summary>
/// Extension methods to setup NLog extensions, so they are known when loading NLog LoggingConfiguration
/// </summary>
public static class SetupSerializationBuilderExtensions
{
	private sealed class ObjectTypeTransformation<T> : IObjectTypeTransformer
	{
		private readonly IObjectTypeTransformer _original;

		private readonly Func<T, object> _transformer;

		public ObjectTypeTransformation(Func<T, object> transformer, IObjectTypeTransformer original)
		{
			_original = original;
			_transformer = transformer;
		}

		public object? TryTransformObject(object obj)
		{
			if (obj is T arg)
			{
				object obj2 = _transformer(arg);
				if (obj2 != null)
				{
					return obj2;
				}
			}
			return _original?.TryTransformObject(obj);
		}
	}

	private sealed class ObjectTypeTransformation : IObjectTypeTransformer
	{
		private readonly IObjectTypeTransformer _original;

		private readonly Func<object, object> _transformer;

		private readonly Type _objectType;

		public ObjectTypeTransformation(Type objecType, Func<object, object> transformer, IObjectTypeTransformer original)
		{
			_original = original;
			_transformer = transformer;
			_objectType = objecType;
		}

		public object? TryTransformObject(object obj)
		{
			if (_objectType.IsAssignableFrom(obj.GetType()))
			{
				return _transformer(obj);
			}
			return _original?.TryTransformObject(obj);
		}
	}

	/// <summary>
	/// Enable/disables the NLog Message Template Parsing:
	/// <para>- <see langword="true" /> = Always use NLog mesage-template-parser and formatting.</para>
	/// <para>- <see langword="false" /> = Never use NLog-parser and only use string.Format (Disable support for message-template-syntax).</para>
	/// <para>- <see langword="null" /> = Auto detection of message-template-syntax, with fallback to string.Format (Default Behavior).</para>
	/// </summary>
	public static ISetupSerializationBuilder ParseMessageTemplates(this ISetupSerializationBuilder setupBuilder, bool? enable)
	{
		setupBuilder.LogFactory.ServiceRepository.ParseMessageTemplates(setupBuilder.LogFactory, enable);
		return setupBuilder;
	}

	/// <summary>
	/// Overrides the active <see cref="T:NLog.IJsonConverter" /> with a new custom implementation
	/// </summary>
	public static ISetupSerializationBuilder RegisterJsonConverter(this ISetupSerializationBuilder setupBuilder, IJsonConverter jsonConverter)
	{
		setupBuilder.LogFactory.ServiceRepository.RegisterJsonConverter(jsonConverter ?? new DefaultJsonSerializer(setupBuilder.LogFactory.ServiceRepository));
		return setupBuilder;
	}

	/// <summary>
	/// Overrides the active <see cref="T:NLog.IValueFormatter" /> with a new custom implementation
	/// </summary>
	public static ISetupSerializationBuilder RegisterValueFormatter(this ISetupSerializationBuilder setupBuilder, IValueFormatter valueFormatter)
	{
		setupBuilder.LogFactory.ServiceRepository.RegisterValueFormatter(valueFormatter ?? new ValueFormatter(setupBuilder.LogFactory.ServiceRepository, legacyStringQuotes: false));
		return setupBuilder;
	}

	/// <summary>
	/// Overrides the active <see cref="T:NLog.IValueFormatter" /> to use legacy-mode string-quotes (Before NLog v6)
	/// </summary>
	public static ISetupSerializationBuilder RegisterValueFormatterWithStringQuotes(this ISetupSerializationBuilder setupBuilder)
	{
		setupBuilder.LogFactory.ServiceRepository.RegisterValueFormatter(new ValueFormatter(setupBuilder.LogFactory.ServiceRepository, legacyStringQuotes: true));
		return setupBuilder;
	}

	/// <summary>
	/// Registers object Type transformation so build trimming will keep public properties.
	/// </summary>
	public static ISetupSerializationBuilder RegisterObjectTransformation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(this ISetupSerializationBuilder setupBuilder)
	{
		Guard.ThrowIfNull(setupBuilder, "setupBuilder");
		InternalLogger.Debug("RegisterObjectTransformation for {0} to keep public properties", typeof(T));
		return setupBuilder;
	}

	/// <summary>
	/// Registers object Type transformation from dangerous (massive) object to safe (reduced) object
	/// </summary>
	public static ISetupSerializationBuilder RegisterObjectTransformation<T>(this ISetupSerializationBuilder setupBuilder, Func<T, object> transformer)
	{
		Guard.ThrowIfNull(transformer, "transformer");
		InternalLogger.Debug("RegisterObjectTransformation for {0} to convert safely", typeof(T));
		IObjectTypeTransformer service = setupBuilder.LogFactory.ServiceRepository.GetService<IObjectTypeTransformer>();
		setupBuilder.LogFactory.ServiceRepository.RegisterObjectTypeTransformer(new ObjectTypeTransformation<T>(transformer, service));
		return setupBuilder;
	}

	/// <summary>
	/// Registers object Type transformation from dangerous (massive) object to safe (reduced) object
	/// </summary>
	public static ISetupSerializationBuilder RegisterObjectTransformation(this ISetupSerializationBuilder setupBuilder, Type objectType, Func<object, object> transformer)
	{
		Guard.ThrowIfNull(objectType, "objectType");
		Guard.ThrowIfNull(transformer, "transformer");
		IObjectTypeTransformer service = setupBuilder.LogFactory.ServiceRepository.GetService<IObjectTypeTransformer>();
		setupBuilder.LogFactory.ServiceRepository.RegisterObjectTypeTransformer(new ObjectTypeTransformation(objectType, transformer, service));
		return setupBuilder;
	}
}
