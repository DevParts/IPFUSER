using System;
using NLog.Internal;

namespace NLog.Config;

/// <summary>
/// Failed to resolve the interface of service type
/// </summary>
public sealed class NLogDependencyResolveException : Exception
{
	/// <summary>
	/// Typed we tried to resolve
	/// </summary>
	public Type ServiceType { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.NLogDependencyResolveException" /> class.
	/// </summary>
	public NLogDependencyResolveException(string message, Type serviceType)
		: base(CreateFullMessage(serviceType, message))
	{
		ServiceType = Guard.ThrowIfNull(serviceType, "serviceType");
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.NLogDependencyResolveException" /> class.
	/// </summary>
	public NLogDependencyResolveException(string message, Exception innerException, Type serviceType)
		: base(CreateFullMessage(serviceType, message), innerException)
	{
		ServiceType = Guard.ThrowIfNull(serviceType, "serviceType");
	}

	private static string CreateFullMessage(Type typeToResolve, string message)
	{
		return ("Cannot resolve the type: '" + typeToResolve.Name + "'. " + message).Trim();
	}
}
