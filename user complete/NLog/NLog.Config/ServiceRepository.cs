using System;

namespace NLog.Config;

/// <summary>
/// Interface to register available configuration objects type
/// </summary>
public abstract class ServiceRepository : IServiceProvider
{
	/// <summary>
	/// Registers instance of singleton object for use in NLog
	/// </summary>
	/// <param name="type">Type of service/interface to register</param>
	/// <param name="instance">Instance of service</param>
	public abstract void RegisterService(Type type, object instance);

	/// <summary>
	/// Gets the service object of the specified type.
	/// </summary>
	/// <remarks>Avoid calling this while handling a LogEvent, since random deadlocks can occur.</remarks>
	public abstract object GetService(Type serviceType);

	internal abstract bool TryGetService<T>(out T? serviceInstance) where T : class;

	internal ServiceRepository()
	{
	}
}
