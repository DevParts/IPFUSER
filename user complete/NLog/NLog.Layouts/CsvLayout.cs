using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.Layouts;

/// <summary>
/// A specialized layout that renders CSV-formatted events.
/// </summary>
/// <remarks>
/// <para>
/// If <see cref="P:NLog.Layouts.LayoutWithHeaderAndFooter.Header" /> is set, then the header generation with column names will be disabled.
/// </para>
/// <a href="https://github.com/NLog/NLog/wiki/CsvLayout">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/CsvLayout">Documentation on NLog Wiki</seealso>
[Layout("CsvLayout")]
[ThreadAgnostic]
[AppDomainFixedOutput]
public class CsvLayout : LayoutWithHeaderAndFooter
{
	/// <summary>
	/// Header with column names for CSV layout.
	/// </summary>
	[ThreadAgnostic]
	[AppDomainFixedOutput]
	internal sealed class CsvHeaderLayout : Layout
	{
		private readonly CsvLayout _parent;

		private string? _headerOutput;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:NLog.Layouts.CsvLayout.CsvHeaderLayout" /> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		public CsvHeaderLayout(CsvLayout parent)
		{
			_parent = parent;
		}

		/// <inheritdoc />
		protected override void InitializeLayout()
		{
			_headerOutput = null;
			base.InitializeLayout();
		}

		private string GetHeaderOutput()
		{
			return _headerOutput ?? (_headerOutput = BuilderHeaderOutput());
		}

		private string BuilderHeaderOutput()
		{
			StringBuilder stringBuilder = new StringBuilder();
			_parent.RenderHeader(stringBuilder);
			return stringBuilder.ToString();
		}

		internal override void PrecalculateBuilder(LogEventInfo logEvent, StringBuilder target)
		{
		}

		/// <inheritdoc />
		protected override string GetFormattedMessage(LogEventInfo logEvent)
		{
			return GetHeaderOutput();
		}

		/// <inheritdoc />
		protected override void RenderFormattedMessage(LogEventInfo logEvent, StringBuilder target)
		{
			target.Append(GetHeaderOutput());
		}
	}

	private string _actualColumnDelimiter;

	private string _doubleQuoteChar;

	private char[] _quotableCharacters;

	private Layout[]? _precalculateLayouts;

	private readonly List<CsvColumn> _columns = new List<CsvColumn>();

	/// <summary>
	/// Gets the array of parameters to be passed.
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	[ArrayParameter(typeof(CsvColumn), "column")]
	public IList<CsvColumn> Columns => _columns;

	/// <summary>
	/// Gets or sets a value indicating whether CSV should include header.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool WithHeader { get; set; } = true;

	/// <summary>
	/// Gets or sets the column delimiter.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.Layouts.CsvColumnDelimiterMode.Auto" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public CsvColumnDelimiterMode Delimiter { get; set; }

	/// <summary>
	/// Gets or sets the quoting mode.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.Layouts.CsvQuotingMode.Auto" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public CsvQuotingMode Quoting { get; set; } = CsvQuotingMode.Auto;

	/// <summary>
	/// Gets or sets the quote Character.
	/// </summary>
	/// <remarks>Default: <c>"</c></remarks>
	/// <docgen category="Layout Options" order="10" />
	public string QuoteChar { get; set; } = "\"";

	/// <summary>
	/// Gets or sets the custom column delimiter value (valid when <see cref="P:NLog.Layouts.CsvLayout.Delimiter" /> is set to <see cref="F:NLog.Layouts.CsvColumnDelimiterMode.Custom" />).
	/// </summary>
	/// <remarks>Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public string CustomColumnDelimiter { get; set; } = string.Empty;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.CsvLayout" /> class.
	/// </summary>
	public CsvLayout()
	{
		base.Layout = this;
		base.Header = new CsvHeaderLayout(this);
		base.Footer = null;
		ResolveQuoteChars(QuoteChar, out _actualColumnDelimiter, out _doubleQuoteChar, out _quotableCharacters);
	}

	/// <inheritdoc />
	protected override void InitializeLayout()
	{
		if (!WithHeader)
		{
			base.Header = null;
		}
		base.InitializeLayout();
		ResolveQuoteChars(QuoteChar, out _actualColumnDelimiter, out _doubleQuoteChar, out _quotableCharacters);
		_precalculateLayouts = ResolveLayoutPrecalculation(_columns.Select((CsvColumn cln) => cln.Layout));
		foreach (CsvColumn column in _columns)
		{
			if (string.IsNullOrEmpty(column.Name))
			{
				throw new NLogConfigurationException("CsvLayout: Contains invalid CsvColumn with unassigned Name-property");
			}
		}
	}

	private void ResolveQuoteChars(string quoteChar, out string actualColumnDelimiter, out string doubleQuoteChar, out char[] quotableCharacters)
	{
		actualColumnDelimiter = ResolveColumnDelimiter(Delimiter);
		quotableCharacters = (quoteChar + "\r\n" + actualColumnDelimiter).ToCharArray();
		doubleQuoteChar = quoteChar + quoteChar;
	}

	private string ResolveColumnDelimiter(CsvColumnDelimiterMode delimiter)
	{
		switch (delimiter)
		{
		case CsvColumnDelimiterMode.Auto:
			return CultureInfo.CurrentCulture.TextInfo.ListSeparator;
		case CsvColumnDelimiterMode.Comma:
			return ",";
		case CsvColumnDelimiterMode.Semicolon:
			return ";";
		case CsvColumnDelimiterMode.Pipe:
			return "|";
		case CsvColumnDelimiterMode.Tab:
			return "\t";
		case CsvColumnDelimiterMode.Space:
			return " ";
		case CsvColumnDelimiterMode.Custom:
			if (!string.IsNullOrEmpty(CustomColumnDelimiter))
			{
				return CustomColumnDelimiter;
			}
			return ";";
		default:
			return ";";
		}
	}

	/// <inheritdoc />
	protected override void CloseLayout()
	{
		_precalculateLayouts = null;
		base.CloseLayout();
	}

	internal override void PrecalculateBuilder(LogEventInfo logEvent, StringBuilder target)
	{
		PrecalculateBuilderInternal(logEvent, target, _precalculateLayouts);
	}

	/// <inheritdoc />
	protected override string GetFormattedMessage(LogEventInfo logEvent)
	{
		return RenderAllocateBuilder(logEvent);
	}

	/// <inheritdoc />
	protected override void RenderFormattedMessage(LogEventInfo logEvent, StringBuilder target)
	{
		bool initialColumn = true;
		foreach (CsvColumn column in _columns)
		{
			RenderColumnLayout(logEvent, column.Layout, column._quoting ?? Quoting, target, initialColumn);
			initialColumn = false;
		}
	}

	private void RenderColumnLayout(LogEventInfo logEvent, Layout columnLayout, CsvQuotingMode quoting, StringBuilder target, bool initialColumn)
	{
		if (!initialColumn)
		{
			target.Append(_actualColumnDelimiter);
		}
		if (quoting == CsvQuotingMode.All)
		{
			target.Append(QuoteChar);
		}
		int length = target.Length;
		columnLayout.Render(logEvent, target);
		if (length != target.Length && ColumnValueRequiresQuotes(quoting, target, length))
		{
			string text = target.ToString(length, target.Length - length);
			target.Length = length;
			if (quoting != CsvQuotingMode.All)
			{
				target.Append(QuoteChar);
			}
			target.Append(text.Replace(QuoteChar, _doubleQuoteChar));
			target.Append(QuoteChar);
		}
		else if (quoting == CsvQuotingMode.All)
		{
			target.Append(QuoteChar);
		}
	}

	/// <summary>
	/// Get the headers with the column names.
	/// </summary>
	/// <returns></returns>
	private void RenderHeader(StringBuilder sb)
	{
		LogEventInfo logEvent = LogEventInfo.CreateNullEvent();
		bool initialColumn = true;
		foreach (CsvColumn column in _columns)
		{
			Layout layout = NLog.Layouts.Layout.FromLiteral(column.Name);
			layout.Initialize(base.LoggingConfiguration);
			RenderColumnLayout(logEvent, layout, column._quoting ?? Quoting, sb, initialColumn);
			initialColumn = false;
		}
	}

	private bool ColumnValueRequiresQuotes(CsvQuotingMode quoting, StringBuilder sb, int startPosition)
	{
		switch (quoting)
		{
		case CsvQuotingMode.Nothing:
			return false;
		case CsvQuotingMode.All:
			if (QuoteChar.Length == 1)
			{
				return sb.IndexOf(QuoteChar[0], startPosition) >= 0;
			}
			return sb.IndexOfAny(_quotableCharacters, startPosition) >= 0;
		default:
			return sb.IndexOfAny(_quotableCharacters, startPosition) >= 0;
		}
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return ToStringWithNestedItems(_columns, (CsvColumn c) => c.Name);
	}
}
