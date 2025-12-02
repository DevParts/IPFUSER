using System;
using System.Text;
using NLog.Config;

namespace NLog.Layouts;

internal interface ILayoutTypeValue
{
	Type? InnerType { get; }

	ILayoutTypeValue InnerLayout { get; }

	object? RenderObjectValue(LogEventInfo logEvent, StringBuilder? stringBuilder);
}
internal interface ILayoutTypeValue<T> : ILayoutTypeValue
{
	LoggingConfiguration? LoggingConfiguration { get; }

	bool ThreadAgnostic { get; }

	bool ThreadAgnosticImmutable { get; }

	StackTraceUsage StackTraceUsage { get; }

	void InitializeLayout();

	void CloseLayout();

	bool TryRenderValue(LogEventInfo logEvent, StringBuilder? stringBuilder, out T? value);
}
