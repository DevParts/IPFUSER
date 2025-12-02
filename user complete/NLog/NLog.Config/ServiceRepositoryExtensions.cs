using System;
using NLog.Common;
using NLog.Internal;
using NLog.MessageTemplates;
using NLog.Targets;

namespace NLog.Config;

internal static class ServiceRepositoryExtensions
{
	internal static ServiceRepository GetServiceProvider(this LoggingConfiguration? loggingConfiguration)
	{
		return loggingConfiguration?.LogFactory?.ServiceRepository ?? LogManager.LogFactory.ServiceRepository;
	}

	internal static T ResolveService<T>(this ServiceRepository serviceProvider, bool ignoreExternalProvider = true) where T : class
	{
		if (ignoreExternalProvider)
		{
			return serviceProvider.GetService<T>();
		}
		IServiceProvider service;
		try
		{
			if (serviceProvider.TryGetService<T>(out T serviceInstance) && serviceInstance != null)
			{
				return serviceInstance;
			}
			service = serviceProvider.GetService<IServiceProvider>();
		}
		catch (NLogDependencyResolveException)
		{
			service = serviceProvider.GetService<IServiceProvider>();
			if (service == serviceProvider)
			{
				throw;
			}
		}
		catch (Exception ex2)
		{
			if (ex2.MustBeRethrownImmediately())
			{
				throw;
			}
			throw new NLogDependencyResolveException("Service Provider failed with exception - " + ex2.Message, ex2, typeof(T));
		}
		if (service == serviceProvider)
		{
			throw new NLogDependencyResolveException("Type not registered in Service Provider", typeof(T));
		}
		T service2 = service.GetService<T>();
		serviceProvider.RegisterService(typeof(T), service2);
		return service2;
	}

	internal static T GetService<T>(this IServiceProvider serviceProvider) where T : class
	{
		try
		{
			return ((serviceProvider ?? LogManager.LogFactory.ServiceRepository).GetService(typeof(T)) as T) ?? throw new NLogDependencyResolveException("Type not registered in Service Provider", typeof(T));
		}
		catch (NLogDependencyResolveException ex)
		{
			if (ex.ServiceType == typeof(T))
			{
				throw;
			}
			throw new NLogDependencyResolveException(ex.Message, ex, typeof(T));
		}
		catch (Exception ex2)
		{
			if (ex2.MustBeRethrownImmediately())
			{
				throw;
			}
			throw new NLogDependencyResolveException("Service Provider failed with exception - " + ex2.Message, ex2, typeof(T));
		}
	}

	/// <summary>
	/// Registers singleton-object as implementation of specific interface.
	/// </summary>
	/// <remarks>
	/// If the same single-object implements multiple interfaces then it must be registered for each interface
	/// </remarks>
	/// <typeparam name="T">Type of interface</typeparam>
	/// <param name="serviceRepository">The repo</param>
	/// <param name="singleton">Singleton object to use for override</param>
	internal static ServiceRepository RegisterSingleton<T>(this ServiceRepository serviceRepository, T singleton) where T : class
	{
		serviceRepository.RegisterService(typeof(T), singleton);
		return serviceRepository;
	}

	/// <summary>
	/// Registers the string serializer to use with <see cref="P:NLog.LogEventInfo.MessageTemplateParameters" />
	/// </summary>
	internal static ServiceRepository RegisterValueFormatter(this ServiceRepository serviceRepository, IValueFormatter valueFormatter)
	{
		Guard.ThrowIfNull(valueFormatter, "valueFormatter");
		serviceRepository.RegisterSingleton(valueFormatter);
		return serviceRepository;
	}

	internal static ServiceRepository RegisterJsonConverter(this ServiceRepository serviceRepository, IJsonConverter jsonConverter)
	{
		Guard.ThrowIfNull(jsonConverter, "jsonConverter");
		serviceRepository.RegisterSingleton(jsonConverter);
		return serviceRepository;
	}

	internal static ServiceRepository RegisterPropertyTypeConverter(this ServiceRepository serviceRepository, IPropertyTypeConverter converter)
	{
		Guard.ThrowIfNull(converter, "converter");
		serviceRepository.RegisterSingleton(converter);
		return serviceRepository;
	}

	internal static ServiceRepository RegisterObjectTypeTransformer(this ServiceRepository serviceRepository, IObjectTypeTransformer transformer)
	{
		Guard.ThrowIfNull(transformer, "transformer");
		serviceRepository.RegisterSingleton(transformer);
		return serviceRepository;
	}

	internal static ServiceRepository ParseMessageTemplates(this ServiceRepository serviceRepository, LogFactory logFactory, bool? enable)
	{
		if (enable == true)
		{
			InternalLogger.Debug("Message Template Format always enabled");
			serviceRepository.RegisterSingleton((ILogMessageFormatter)new LogMessageTemplateFormatter(logFactory, forceMessageTemplateRenderer: true, singleTargetOnly: false));
		}
		else if (enable == false)
		{
			InternalLogger.Debug("Message Template String Format always enabled");
			serviceRepository.RegisterSingleton((ILogMessageFormatter)LogMessageStringFormatter.Default);
		}
		else
		{
			InternalLogger.Debug("Message Template Auto Format enabled");
			serviceRepository.RegisterSingleton((ILogMessageFormatter)new LogMessageTemplateFormatter(logFactory, forceMessageTemplateRenderer: false, singleTargetOnly: false));
		}
		return serviceRepository;
	}

	internal static bool? ResolveParseMessageTemplates(this ServiceRepository serviceRepository)
	{
		return serviceRepository.GetService<ILogMessageFormatter>()?.EnableMessageTemplateParser;
	}

	internal static ServiceRepository RegisterDefaults(this ServiceRepository serviceRepository, LogFactory logFactory)
	{
		serviceRepository.RegisterSingleton((IServiceProvider)serviceRepository);
		serviceRepository.RegisterSingleton((ILogMessageFormatter)new LogMessageTemplateFormatter(logFactory, forceMessageTemplateRenderer: false, singleTargetOnly: false));
		serviceRepository.RegisterJsonConverter(new DefaultJsonSerializer(serviceRepository));
		serviceRepository.RegisterValueFormatter(new ValueFormatter(serviceRepository, legacyStringQuotes: false));
		serviceRepository.RegisterPropertyTypeConverter(PropertyTypeConverter.Instance);
		serviceRepository.RegisterObjectTypeTransformer(new ObjectReflectionCache(serviceRepository));
		return serviceRepository;
	}
}
