using System;
using NLog.Internal;

namespace NLog.Config;

/// <summary>
/// Registered service type in the service repository
/// </summary>
public class ServiceRepositoryUpdateEventArgs : EventArgs
{
	/// <summary>
	/// Type of service-interface that has been registered
	/// </summary>
	public Type ServiceType { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.ServiceRepositoryUpdateEventArgs" /> class.
	/// </summary>
	/// <param name="serviceType">Type of service that have been registered</param>
	public ServiceRepositoryUpdateEventArgs(Type serviceType)
	{
		ServiceType = Guard.ThrowIfNull(serviceType, "serviceType");
	}
}
