using System;
using System.Text;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The information about the garbage collector.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Gc-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Gc-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("gc")]
public class GarbageCollectorInfoLayoutRenderer : LayoutRenderer
{
	/// <summary>
	/// Gets or sets the property to retrieve.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.LayoutRenderers.GarbageCollectorProperty.TotalMemory" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public GarbageCollectorProperty Property { get; set; }

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		long value = GetValue();
		if (value >= 0 && value < uint.MaxValue)
		{
			builder.AppendInvariant((uint)value);
		}
		else
		{
			builder.Append(value);
		}
	}

	private long GetValue()
	{
		long result = 0L;
		switch (Property)
		{
		case GarbageCollectorProperty.TotalMemory:
			result = GC.GetTotalMemory(forceFullCollection: false);
			break;
		case GarbageCollectorProperty.TotalMemoryForceCollection:
			result = GC.GetTotalMemory(forceFullCollection: true);
			break;
		case GarbageCollectorProperty.CollectionCount0:
			result = GC.CollectionCount(0);
			break;
		case GarbageCollectorProperty.CollectionCount1:
			result = GC.CollectionCount(1);
			break;
		case GarbageCollectorProperty.CollectionCount2:
			result = GC.CollectionCount(2);
			break;
		case GarbageCollectorProperty.MaxGeneration:
			result = GC.MaxGeneration;
			break;
		}
		return result;
	}
}
