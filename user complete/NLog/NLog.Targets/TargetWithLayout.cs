using NLog.LayoutRenderers;
using NLog.Layouts;

namespace NLog.Targets;

/// <summary>
/// Represents target that supports string formatting using layouts.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/How-to-write-a-custom-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/How-to-write-a-custom-target">Documentation on NLog Wiki</seealso>
public abstract class TargetWithLayout : Target
{
	private const string DefaultLayoutText = "${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}";

	private static LayoutRenderer[] DefaultLayout => new LayoutRenderer[7]
	{
		new LongDateLayoutRenderer(),
		new LiteralLayoutRenderer("|"),
		new LevelLayoutRenderer
		{
			Uppercase = true
		},
		new LiteralLayoutRenderer("|"),
		new LoggerNameLayoutRenderer(),
		new LiteralLayoutRenderer("|"),
		new MessageLayoutRenderer
		{
			WithException = true
		}
	};

	/// <summary>
	/// Gets or sets the layout used to format log messages.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code></remarks>
	/// <docgen category="Layout Options" order="1" />
	public virtual Layout Layout { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.TargetWithLayout" /> class.
	/// </summary>
	/// <remarks>
	/// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
	/// </remarks>
	protected TargetWithLayout()
	{
		Layout = new SimpleLayout(DefaultLayout, "${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}");
	}
}
