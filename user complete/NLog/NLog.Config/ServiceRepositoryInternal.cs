using System;
using System.Collections.Generic;
using NLog.Internal;

namespace NLog.Config;

/// <summary>
/// Repository of interfaces used by NLog to allow override for dependency injection
/// </summary>
internal sealed class ServiceRepositoryInternal : ServiceRepository
{
	private readonly Dictionary<Type, Func<object>> _creatorMap = new Dictionary<Type, Func<object>>();

	private readonly object _lockObject = new object();

	public event EventHandler<ServiceRepositoryUpdateEventArgs>? TypeRegistered;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.ServiceRepositoryInternal" /> class.
	/// </summary>
	internal ServiceRepositoryInternal(LogFactory logFactory)
	{
		this.RegisterDefaults(logFactory);
	}

	public override void RegisterService(Type type, object instance)
	{
		Guard.ThrowIfNull(type, "type");
		Guard.ThrowIfNull(instance, "instance");
		lock (_lockObject)
		{
			_creatorMap[type] = () => instance;
		}
		this.TypeRegistered?.Invoke(this, new ServiceRepositoryUpdateEventArgs(type));
	}

	public override object GetService(Type serviceType)
	{
		return TryGetService(serviceType) ?? throw new NLogDependencyResolveException("Type not registered in Service Provider", serviceType);
	}

	private object? TryGetService(Type serviceType)
	{
		Guard.ThrowIfNull(serviceType, "serviceType");
		Func<object> value = null;
		lock (_lockObject)
		{
			_creatorMap.TryGetValue(serviceType, out value);
		}
		return value?.Invoke();
	}

	internal override bool TryGetService<T>(out T? serviceInstance)
	{
		if (TryGetService(typeof(T)) is T val)
		{
			serviceInstance = val;
			return true;
		}
		serviceInstance = null;
		return false;
	}
}
