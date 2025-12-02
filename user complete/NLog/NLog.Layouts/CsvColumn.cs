using NLog.Config;

namespace NLog.Layouts;

/// <summary>
/// A column in the CSV.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/CsvLayout">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/CsvLayout">Documentation on NLog Wiki</seealso>
[NLogConfigurationItem]
public class CsvColumn
{
	internal CsvQuotingMode? _quoting;

	/// <summary>
	/// Gets or sets the name of the column.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Layout Options" order="1" />
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the layout used for rendering the column value.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:NLog.Layouts.Layout.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public Layout Layout { get; set; }

	/// <summary>
	/// Gets or sets the override of Quoting mode
	/// </summary>
	/// <remarks>
	/// Default: <see cref="F:NLog.Layouts.CsvQuotingMode.Auto" /> .
	///
	/// For faster performance then consider <see cref="F:NLog.Layouts.CsvQuotingMode.All" /> and <see cref="F:NLog.Layouts.CsvQuotingMode.Nothing" />
	/// </remarks>
	/// <docgen category="Layout Options" order="50" />
	public CsvQuotingMode Quoting
	{
		get
		{
			return _quoting ?? CsvQuotingMode.Auto;
		}
		set
		{
			_quoting = value;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.CsvColumn" /> class.
	/// </summary>
	public CsvColumn()
		: this(string.Empty, NLog.Layouts.Layout.Empty)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.CsvColumn" /> class.
	/// </summary>
	/// <param name="name">The name of the column.</param>
	/// <param name="layout">The layout of the column.</param>
	public CsvColumn(string name, Layout layout)
	{
		Name = name;
		Layout = layout;
	}
}
