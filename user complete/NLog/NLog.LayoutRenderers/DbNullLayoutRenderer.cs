using System;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// DB null for a database
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/db-null-layout-renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/db-null-layout-renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("db-null")]
[ThreadAgnostic]
public class DbNullLayoutRenderer : LayoutRenderer, IRawValue
{
	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
	}

	bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object value)
	{
		value = DBNull.Value;
		return true;
	}
}
