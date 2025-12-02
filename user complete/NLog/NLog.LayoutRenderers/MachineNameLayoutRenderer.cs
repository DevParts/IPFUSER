using System;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The machine name that the process is running on.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/MachineName-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/MachineName-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("machinename")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public class MachineNameLayoutRenderer : LayoutRenderer
{
	private string? _machineName;

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		base.InitializeLayoutRenderer();
		try
		{
			_machineName = EnvironmentHelper.GetMachineName();
			if (string.IsNullOrEmpty(_machineName))
			{
				InternalLogger.Info("MachineName is not available.");
			}
		}
		catch (Exception ex)
		{
			InternalLogger.Error(ex, "Error getting machine name.");
			if (ex.MustBeRethrown())
			{
				throw;
			}
			_machineName = string.Empty;
		}
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		builder.Append(_machineName);
	}
}
