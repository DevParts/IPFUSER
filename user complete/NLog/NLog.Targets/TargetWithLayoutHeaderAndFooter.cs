using NLog.Layouts;

namespace NLog.Targets;

/// <summary>
/// Represents target that supports string formatting using layouts.
/// </summary>
public abstract class TargetWithLayoutHeaderAndFooter : TargetWithLayout
{
	private Layout _layout;

	/// <inheritdoc />
	public override Layout Layout
	{
		get
		{
			return _layout;
		}
		set
		{
			if (value is LayoutWithHeaderAndFooter layoutWithHeaderAndFooter)
			{
				base.Layout = value;
				_layout = layoutWithHeaderAndFooter.Layout;
			}
			else if (base.Layout is LayoutWithHeaderAndFooter layoutWithHeaderAndFooter2)
			{
				layoutWithHeaderAndFooter2.Layout = value;
				_layout = value;
			}
			else
			{
				base.Layout = new LayoutWithHeaderAndFooter
				{
					Layout = value
				};
				_layout = value;
			}
		}
	}

	/// <summary>
	/// Gets or sets the footer.
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Layout Options" order="3" />
	public Layout? Footer
	{
		get
		{
			return (base.Layout as LayoutWithHeaderAndFooter)?.Footer;
		}
		set
		{
			if (base.Layout is LayoutWithHeaderAndFooter layoutWithHeaderAndFooter)
			{
				layoutWithHeaderAndFooter.Footer = value;
			}
			else if (value != null)
			{
				base.Layout = new LayoutWithHeaderAndFooter
				{
					Layout = base.Layout,
					Footer = value
				};
			}
		}
	}

	/// <summary>
	/// Gets or sets the header.
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Layout Options" order="2" />
	public Layout? Header
	{
		get
		{
			return (base.Layout as LayoutWithHeaderAndFooter)?.Header;
		}
		set
		{
			if (base.Layout is LayoutWithHeaderAndFooter layoutWithHeaderAndFooter)
			{
				layoutWithHeaderAndFooter.Header = value;
			}
			else if (value != null)
			{
				base.Layout = new LayoutWithHeaderAndFooter
				{
					Layout = base.Layout,
					Header = value
				};
			}
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.TargetWithLayoutHeaderAndFooter" /> class.
	/// </summary>
	/// <remarks>
	/// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
	/// </remarks>
	protected TargetWithLayoutHeaderAndFooter()
	{
		Layout layout = base.Layout;
		_layout = ((layout is LayoutWithHeaderAndFooter layoutWithHeaderAndFooter) ? layoutWithHeaderAndFooter.Layout : layout);
	}
}
