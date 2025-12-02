using System;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The host name that the process is running on.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/HostName-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/HostName-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("hostname")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public class HostNameLayoutRenderer : LayoutRenderer
{
	private string? _hostName;

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		base.InitializeLayoutRenderer();
		try
		{
			_hostName = GetHostName();
			if (string.IsNullOrEmpty(_hostName))
			{
				InternalLogger.Info("HostName is not available.");
			}
		}
		catch (Exception ex)
		{
			InternalLogger.Error(ex, "Error getting host name.");
			if (ex.MustBeRethrown())
			{
				throw;
			}
			_hostName = string.Empty;
		}
	}

	/// <summary>
	/// Resolves the hostname from environment-variables with fallback to <see cref="P:System.Environment.MachineName" />
	/// </summary>
	private static string GetHostName()
	{
		return TryLookupValue(() => Environment.GetEnvironmentVariable("HOSTNAME"), "HOSTNAME") ?? TryLookupValue(() => Environment.GetEnvironmentVariable("COMPUTERNAME"), "COMPUTERNAME") ?? TryLookupValue(() => Environment.GetEnvironmentVariable("MACHINENAME"), "MACHINENAME") ?? TryLookupValue(() => Environment.MachineName, "MachineName") ?? string.Empty;
	}

	/// <summary>
	/// Tries the lookup value.
	/// </summary>
	/// <param name="lookupFunc">The lookup function.</param>
	/// <param name="lookupType">Type of the lookup.</param>
	/// <returns></returns>
	private static string? TryLookupValue(Func<string> lookupFunc, string lookupType)
	{
		try
		{
			string text = lookupFunc()?.Trim();
			return string.IsNullOrEmpty(text) ? null : text;
		}
		catch (Exception ex)
		{
			InternalLogger.Warn(ex, "Failed to lookup {0}", lookupType);
			return null;
		}
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		builder.Append(_hostName);
	}
}
