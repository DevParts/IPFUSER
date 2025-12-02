using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.Internal;
using NLog.Layouts;

namespace NLog.Targets;

/// <summary>
/// Represents target that supports context capture of <see cref="T:NLog.ScopeContext" /> Properties + Nested-states
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/How-to-write-a-custom-target-for-structured-logging">See NLog Wiki</a>
/// </remarks>
/// <example><code>
/// [Target("MyFirst")]
/// public sealed class MyFirstTarget : TargetWithContext
/// {
///    public MyFirstTarget()
///    {
///        this.Host = "localhost";
///    }
///
///    public Layout Host { get; set; }
///
///    protected override void Write(LogEventInfo logEvent)
///    {
///        string logMessage = this.RenderLogEvent(this.Layout, logEvent);
///        string hostName = this.RenderLogEvent(this.Host, logEvent);
///        return SendTheMessageToRemoteHost(hostName, logMessage);
///    }
///
///    private void SendTheMessageToRemoteHost(string hostName, string message)
///    {
///        // To be implemented
///    }
/// }
/// </code></example>
/// <seealso href="https://github.com/NLog/NLog/wiki/How-to-write-a-custom-target-for-structured-logging">Documentation on NLog Wiki</seealso>
public abstract class TargetWithContext : TargetWithLayout, IIncludeContext
{
	[ThreadAgnostic]
	internal sealed class TargetWithContextLayout : Layout, IIncludeContext, IUsesStackTrace, IStringValueRenderer
	{
		public class LayoutScopeContextProperties : Layout
		{
			private readonly TargetWithContext _owner;

			public bool IsActive { get; set; }

			public LayoutScopeContextProperties(TargetWithContext owner)
			{
				_owner = owner;
			}

			protected override string GetFormattedMessage(LogEventInfo logEvent)
			{
				CaptureContext(logEvent);
				return string.Empty;
			}

			public override void Precalculate(LogEventInfo logEvent)
			{
				CaptureContext(logEvent);
			}

			private void CaptureContext(LogEventInfo logEvent)
			{
				if (IsActive && !logEvent.TryGetCachedLayoutValue(this, out object _))
				{
					IDictionary<string, object> value2 = _owner.CaptureScopeContextProperties(logEvent, null);
					logEvent.AddCachedLayoutValue(this, value2);
				}
			}
		}

		public class LayoutScopeContextNestedStates : Layout
		{
			private readonly TargetWithContext _owner;

			public bool IsActive { get; set; }

			public LayoutScopeContextNestedStates(TargetWithContext owner)
			{
				_owner = owner;
			}

			protected override string GetFormattedMessage(LogEventInfo logEvent)
			{
				CaptureContext(logEvent);
				return string.Empty;
			}

			public override void Precalculate(LogEventInfo logEvent)
			{
				CaptureContext(logEvent);
			}

			private void CaptureContext(LogEventInfo logEvent)
			{
				if (IsActive && !logEvent.TryGetCachedLayoutValue(this, out object _))
				{
					IList<object> value2 = _owner.CaptureScopeContextNested(logEvent);
					logEvent.AddCachedLayoutValue(this, value2);
				}
			}
		}

		private Layout _targetLayout = NLog.Layouts.Layout.Empty;

		private IStringValueRenderer? _targetStringLayout;

		private bool? _includeScopeProperties;

		private bool? _includeScopeNested;

		private bool? _includeMdc;

		private bool? _includeMdlc;

		private bool? _includeNdc;

		private bool? _includeNdlc;

		public Layout TargetLayout
		{
			get
			{
				return _targetLayout;
			}
			set
			{
				_targetLayout = ((this == value) ? _targetLayout : value);
				_targetStringLayout = _targetLayout as IStringValueRenderer;
			}
		}

		/// <summary>Internal Layout that allows capture of <see cref="T:NLog.ScopeContext" /> properties-dictionary</summary>
		internal LayoutScopeContextProperties ScopeContextPropertiesLayout { get; }

		/// <summary>Internal Layout that allows capture of <see cref="T:NLog.ScopeContext" /> nested-states-stack</summary>
		internal LayoutScopeContextNestedStates ScopeContextNestedStatesLayout { get; }

		public bool IncludeEventProperties { get; set; }

		public bool IncludeCallSite { get; set; }

		public bool IncludeCallSiteStackTrace { get; set; }

		public bool IncludeScopeProperties
		{
			get
			{
				return _includeScopeProperties ?? ScopeContextPropertiesLayout.IsActive;
			}
			set
			{
				bool value2 = (ScopeContextPropertiesLayout.IsActive = value);
				_includeScopeProperties = value2;
			}
		}

		public bool IncludeScopeNested
		{
			get
			{
				return _includeScopeNested ?? ScopeContextNestedStatesLayout.IsActive;
			}
			set
			{
				bool value2 = (ScopeContextNestedStatesLayout.IsActive = value);
				_includeScopeNested = value2;
			}
		}

		[Obsolete("Replaced by IncludeScopeProperties. Marked obsolete on NLog 5.0")]
		public bool IncludeMdc
		{
			get
			{
				return _includeMdc == true;
			}
			set
			{
				_includeMdc = value;
				ScopeContextPropertiesLayout.IsActive = _includeScopeProperties ?? (_includeMdlc == true || value);
			}
		}

		[Obsolete("Replaced by IncludeScopeProperties. Marked obsolete on NLog 5.0")]
		public bool IncludeMdlc
		{
			get
			{
				return _includeMdlc == true;
			}
			set
			{
				_includeMdlc = value;
				ScopeContextPropertiesLayout.IsActive = _includeScopeProperties ?? (_includeMdc == true || value);
			}
		}

		[Obsolete("Replaced by IncludeScopeNested. Marked obsolete on NLog 5.0")]
		public bool IncludeNdc
		{
			get
			{
				return _includeNdc == true;
			}
			set
			{
				_includeNdc = value;
				ScopeContextNestedStatesLayout.IsActive = _includeScopeNested ?? (_includeNdlc == true || value);
			}
		}

		[Obsolete("Replaced by IncludeScopeNested. Marked obsolete on NLog 5.0")]
		public bool IncludeNdlc
		{
			get
			{
				return _includeNdlc == true;
			}
			set
			{
				_includeNdlc = value;
				ScopeContextNestedStatesLayout.IsActive = _includeScopeNested ?? (_includeNdc == true || value);
			}
		}

		StackTraceUsage IUsesStackTrace.StackTraceUsage
		{
			get
			{
				if (IncludeCallSiteStackTrace)
				{
					return StackTraceUsage.WithSource;
				}
				if (IncludeCallSite)
				{
					return StackTraceUsage.WithCallSite | StackTraceUsage.WithCallSiteClassName;
				}
				return StackTraceUsage.None;
			}
		}

		public TargetWithContextLayout(TargetWithContext owner, Layout targetLayout)
		{
			TargetLayout = targetLayout;
			ScopeContextPropertiesLayout = new LayoutScopeContextProperties(owner);
			ScopeContextNestedStatesLayout = new LayoutScopeContextNestedStates(owner);
		}

		protected override void InitializeLayout()
		{
			base.InitializeLayout();
			if (IncludeScopeProperties || IncludeScopeNested)
			{
				base.ThreadAgnostic = false;
			}
			if (IncludeEventProperties)
			{
				base.ThreadAgnosticImmutable = true;
			}
		}

		public override string ToString()
		{
			return TargetLayout?.ToString() ?? base.ToString();
		}

		public override void Precalculate(LogEventInfo logEvent)
		{
			Layout targetLayout = TargetLayout;
			if (targetLayout == null || targetLayout.ThreadAgnostic)
			{
				Layout targetLayout2 = TargetLayout;
				if (targetLayout2 == null || !targetLayout2.ThreadAgnosticImmutable)
				{
					goto IL_004f;
				}
			}
			TargetLayout.Precalculate(logEvent);
			if (logEvent.TryGetCachedLayoutValue(TargetLayout, out object value))
			{
				logEvent.AddCachedLayoutValue(this, value);
			}
			goto IL_004f;
			IL_004f:
			PrecalculateContext(logEvent);
		}

		internal override void PrecalculateBuilder(LogEventInfo logEvent, StringBuilder target)
		{
			Layout targetLayout = TargetLayout;
			if (targetLayout == null || targetLayout.ThreadAgnostic)
			{
				Layout targetLayout2 = TargetLayout;
				if (targetLayout2 == null || !targetLayout2.ThreadAgnosticImmutable)
				{
					goto IL_0050;
				}
			}
			TargetLayout.PrecalculateBuilder(logEvent, target);
			if (logEvent.TryGetCachedLayoutValue(TargetLayout, out object value))
			{
				logEvent.AddCachedLayoutValue(this, value);
			}
			goto IL_0050;
			IL_0050:
			PrecalculateContext(logEvent);
		}

		private void PrecalculateContext(LogEventInfo logEvent)
		{
			if (IncludeScopeProperties)
			{
				ScopeContextPropertiesLayout.Precalculate(logEvent);
			}
			if (IncludeScopeNested)
			{
				ScopeContextNestedStatesLayout.Precalculate(logEvent);
			}
		}

		protected override string GetFormattedMessage(LogEventInfo logEvent)
		{
			return TargetLayout?.Render(logEvent) ?? string.Empty;
		}

		protected override void RenderFormattedMessage(LogEventInfo logEvent, StringBuilder target)
		{
			TargetLayout?.Render(logEvent, target);
		}

		string? IStringValueRenderer.GetFormattedString(LogEventInfo logEvent)
		{
			return _targetStringLayout?.GetFormattedString(logEvent);
		}
	}

	private TargetWithContextLayout _contextLayout;

	/// <inheritdoc />
	/// <docgen category="Layout Options" order="1" />
	public sealed override Layout Layout
	{
		get
		{
			return _contextLayout;
		}
		set
		{
			if (_contextLayout == null)
			{
				_contextLayout = new TargetWithContextLayout(this, value);
			}
			else
			{
				_contextLayout.TargetLayout = value;
			}
		}
	}

	/// <summary>
	/// Gets or sets the option to include all properties from the log events
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool IncludeEventProperties
	{
		get
		{
			return _contextLayout.IncludeEventProperties;
		}
		set
		{
			_contextLayout.IncludeEventProperties = value;
		}
	}

	/// <summary>
	/// Gets or sets whether to include the contents of the <see cref="T:NLog.ScopeContext" /> properties-dictionary.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool IncludeScopeProperties
	{
		get
		{
			return _contextLayout.IncludeScopeProperties;
		}
		set
		{
			_contextLayout.IncludeScopeProperties = value;
		}
	}

	/// <summary>
	/// Gets or sets whether to include the contents of the <see cref="T:NLog.ScopeContext" /> nested-state-stack.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool IncludeScopeNested
	{
		get
		{
			return _contextLayout.IncludeScopeNested;
		}
		set
		{
			_contextLayout.IncludeScopeNested = value;
		}
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="P:NLog.Targets.TargetWithContext.IncludeScopeProperties" /> with NLog v5.
	/// Gets or sets whether to include the contents of the <see cref="T:NLog.MappedDiagnosticsContext" />-dictionary.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	[Obsolete("Replaced by IncludeScopeProperties. Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool IncludeMdc
	{
		get
		{
			return _contextLayout.IncludeMdc;
		}
		set
		{
			_contextLayout.IncludeMdc = value;
		}
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="P:NLog.Targets.TargetWithContext.IncludeScopeNested" /> with NLog v5.
	/// Gets or sets whether to include the contents of the <see cref="T:NLog.NestedDiagnosticsContext" />-stack.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	[Obsolete("Replaced by IncludeScopeNested. Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool IncludeNdc
	{
		get
		{
			return _contextLayout.IncludeNdc;
		}
		set
		{
			_contextLayout.IncludeNdc = value;
		}
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="P:NLog.Targets.TargetWithContext.IncludeScopeProperties" /> with NLog v5.
	/// Gets or sets whether to include the contents of the <see cref="T:NLog.MappedDiagnosticsLogicalContext" />-properties.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	[Obsolete("Replaced by IncludeScopeProperties. Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool IncludeMdlc
	{
		get
		{
			return _contextLayout.IncludeMdlc;
		}
		set
		{
			_contextLayout.IncludeMdlc = value;
		}
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="P:NLog.Targets.TargetWithContext.IncludeScopeNested" /> with NLog v5.
	/// Gets or sets whether to include the contents of the <see cref="T:NLog.NestedDiagnosticsLogicalContext" />-stack.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	[Obsolete("Replaced by IncludeScopeNested. Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool IncludeNdlc
	{
		get
		{
			return _contextLayout.IncludeNdlc;
		}
		set
		{
			_contextLayout.IncludeNdlc = value;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether to include contents of the <see cref="T:NLog.GlobalDiagnosticsContext" /> dictionary
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool IncludeGdc { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to include call site (class and method name) in the <see cref="T:NLog.LogEventInfo" />
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool IncludeCallSite
	{
		get
		{
			return _contextLayout.IncludeCallSite;
		}
		set
		{
			_contextLayout.IncludeCallSite = value;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether to include source info (file name and line number) in the <see cref="T:NLog.LogEventInfo" />
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool IncludeCallSiteStackTrace
	{
		get
		{
			return _contextLayout.IncludeCallSiteStackTrace;
		}
		set
		{
			_contextLayout.IncludeCallSiteStackTrace = value;
		}
	}

	/// <summary>
	/// Gets the array of custom attributes to be passed into the logevent context
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	[ArrayParameter(typeof(TargetPropertyWithContext), "contextproperty")]
	public virtual IList<TargetPropertyWithContext> ContextProperties { get; } = new List<TargetPropertyWithContext>();

	/// <summary>
	/// List of property names to exclude when <see cref="P:NLog.Targets.TargetWithContext.IncludeEventProperties" /> is <see langword="true" />
	/// </summary>
	/// <docgen category="Layout Options" order="50" />
	public ISet<string> ExcludeProperties { get; set; }

	/// <summary>
	/// Constructor
	/// </summary>
	protected TargetWithContext()
	{
		_contextLayout = _contextLayout ?? new TargetWithContextLayout(this, base.Layout);
		ExcludeProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
	}

	/// <inheritdoc />
	protected override void InitializeTarget()
	{
		base.InitializeTarget();
		IList<TargetPropertyWithContext> contextProperties = ContextProperties;
		if (contextProperties == null || contextProperties.Count <= 0)
		{
			return;
		}
		foreach (TargetPropertyWithContext contextProperty in ContextProperties)
		{
			if (string.IsNullOrEmpty(contextProperty.Name))
			{
				throw new NLogConfigurationException($"{this}: Contains invalid ContextProperty with unassigned Name-property");
			}
		}
	}

	/// <summary>
	/// Check if logevent has properties (or context properties)
	/// </summary>
	/// <param name="logEvent"></param>
	/// <returns><see langword="true" /> if properties should be included</returns>
	protected bool ShouldIncludeProperties(LogEventInfo logEvent)
	{
		if (!IncludeGdc && !IncludeScopeProperties)
		{
			if (IncludeEventProperties)
			{
				return logEvent?.HasProperties ?? false;
			}
			return false;
		}
		return true;
	}

	/// <summary>
	/// Checks if any context properties, and if any returns them as a single dictionary
	/// </summary>
	/// <param name="logEvent"></param>
	/// <returns>Dictionary with any context properties for the logEvent (Null if none found)</returns>
	protected IDictionary<string, object?>? GetContextProperties(LogEventInfo logEvent)
	{
		return GetContextProperties(logEvent, null);
	}

	/// <summary>
	/// Checks if any context properties, and if any returns them as a single dictionary
	/// </summary>
	/// <param name="logEvent"></param>
	/// <param name="combinedProperties">Optional pre-allocated dictionary for the snapshot</param>
	/// <returns>Dictionary with any context properties for the logEvent (Null if none found)</returns>
	protected IDictionary<string, object?>? GetContextProperties(LogEventInfo logEvent, IDictionary<string, object?>? combinedProperties)
	{
		IList<TargetPropertyWithContext> contextProperties = ContextProperties;
		if (contextProperties != null && contextProperties.Count > 0)
		{
			combinedProperties = CaptureContextProperties(logEvent, combinedProperties);
		}
		if (IncludeScopeProperties && !CombineProperties(logEvent, _contextLayout.ScopeContextPropertiesLayout, ref combinedProperties))
		{
			combinedProperties = CaptureScopeContextProperties(logEvent, combinedProperties);
		}
		if (IncludeGdc)
		{
			combinedProperties = CaptureContextGdc(logEvent, combinedProperties);
		}
		return combinedProperties;
	}

	/// <summary>
	/// Creates combined dictionary of all configured properties for logEvent
	/// </summary>
	/// <param name="logEvent"></param>
	/// <returns>Dictionary with all collected properties for logEvent</returns>
	protected IDictionary<string, object?> GetAllProperties(LogEventInfo logEvent)
	{
		return GetAllProperties(logEvent, null);
	}

	/// <summary>
	/// Creates combined dictionary of all configured properties for logEvent
	/// </summary>
	/// <param name="logEvent"></param>
	/// <param name="combinedProperties">Optional prefilled dictionary</param>
	/// <returns>Dictionary with all collected properties for logEvent</returns>
	protected IDictionary<string, object?> GetAllProperties(LogEventInfo logEvent, IDictionary<string, object?>? combinedProperties)
	{
		if (IncludeEventProperties && logEvent.HasProperties)
		{
			combinedProperties = combinedProperties ?? CreateNewDictionary(logEvent.Properties.Count + (ContextProperties?.Count ?? 0));
			bool checkForDuplicates = combinedProperties.Count > 0;
			ISet<string> set = ((ExcludeProperties == null || ExcludeProperties.Count == 0) ? null : ExcludeProperties);
			using PropertiesDictionary.PropertyDictionaryEnumerator propertyDictionaryEnumerator = logEvent.CreatePropertiesInternal().GetPropertyEnumerator();
			while (propertyDictionaryEnumerator.MoveNext())
			{
				KeyValuePair<string, object> currentProperty = propertyDictionaryEnumerator.CurrentProperty;
				if (!string.IsNullOrEmpty(currentProperty.Key) && (set == null || !set.Contains(currentProperty.Key)))
				{
					AddContextProperty(logEvent, currentProperty.Key, currentProperty.Value, checkForDuplicates, combinedProperties);
				}
			}
		}
		combinedProperties = GetContextProperties(logEvent, combinedProperties);
		return combinedProperties ?? CreateNewDictionary(0);
	}

	private static Dictionary<string, object?> CreateNewDictionary(int initialCapacity)
	{
		return new Dictionary<string, object>((initialCapacity >= 3) ? initialCapacity : 0, StringComparer.Ordinal);
	}

	/// <summary>
	/// Generates a new unique name, when duplicate names are detected
	/// </summary>
	/// <param name="logEvent">LogEvent that triggered the duplicate name</param>
	/// <param name="itemName">Duplicate item name</param>
	/// <param name="itemValue">Item Value</param>
	/// <param name="combinedProperties">Dictionary of context values</param>
	/// <returns>New (unique) value (or null to skip value). If the same value is used then the item will be overwritten</returns>
	protected virtual string GenerateUniqueItemName(LogEventInfo logEvent, string itemName, object? itemValue, IDictionary<string, object?> combinedProperties)
	{
		return PropertiesDictionary.GenerateUniquePropertyName<string, object>(itemName, combinedProperties, (string newKey, IDictionary<string, object> props) => props.ContainsKey(newKey));
	}

	private bool CombineProperties(LogEventInfo logEvent, Layout contextLayout, ref IDictionary<string, object?>? combinedProperties)
	{
		if (!logEvent.TryGetCachedLayoutValue(contextLayout, out object value))
		{
			return false;
		}
		if (value is IDictionary<string, object> dictionary)
		{
			if (combinedProperties != null)
			{
				bool checkForDuplicates = combinedProperties.Count > 0;
				foreach (KeyValuePair<string, object> item in dictionary)
				{
					AddContextProperty(logEvent, item.Key, item.Value, checkForDuplicates, combinedProperties);
				}
			}
			else
			{
				combinedProperties = dictionary;
			}
		}
		return true;
	}

	private void AddContextProperty(LogEventInfo logEvent, string propertyName, object? propertyValue, bool checkForDuplicates, IDictionary<string, object?> combinedProperties)
	{
		if (checkForDuplicates && combinedProperties.ContainsKey(propertyName))
		{
			propertyName = GenerateUniqueItemName(logEvent, propertyName, propertyValue, combinedProperties);
		}
		combinedProperties[propertyName] = propertyValue;
	}

	/// <summary>
	/// Returns the captured snapshot of <see cref="T:NLog.ScopeContext" /> dictionary for the <see cref="T:NLog.LogEventInfo" />
	/// </summary>
	/// <param name="logEvent"></param>
	/// <returns>Dictionary with ScopeContext properties if any, else null</returns>
	protected IDictionary<string, object?>? GetScopeContextProperties(LogEventInfo logEvent)
	{
		if (logEvent.TryGetCachedLayoutValue(_contextLayout.ScopeContextPropertiesLayout, out object value))
		{
			return value as IDictionary<string, object>;
		}
		return CaptureScopeContextProperties(logEvent, null);
	}

	/// <summary>
	/// Returns the captured snapshot of nested states from <see cref="T:NLog.ScopeContext" /> for the <see cref="T:NLog.LogEventInfo" />
	/// </summary>
	/// <param name="logEvent"></param>
	/// <returns>Collection of nested state objects if any, else null</returns>
	protected IList<object>? GetScopeContextNested(LogEventInfo logEvent)
	{
		if (logEvent.TryGetCachedLayoutValue(_contextLayout.ScopeContextNestedStatesLayout, out object value))
		{
			return value as IList<object>;
		}
		return CaptureScopeContextNested(logEvent);
	}

	private IDictionary<string, object?>? CaptureContextProperties(LogEventInfo logEvent, IDictionary<string, object?>? combinedProperties)
	{
		combinedProperties = combinedProperties ?? CreateNewDictionary(ContextProperties.Count);
		for (int i = 0; i < ContextProperties.Count; i++)
		{
			TargetPropertyWithContext targetPropertyWithContext = ContextProperties[i];
			if (targetPropertyWithContext == null || string.IsNullOrEmpty(targetPropertyWithContext.Name))
			{
				continue;
			}
			try
			{
				if (TryGetContextPropertyValue(logEvent, targetPropertyWithContext, out object propertyValue))
				{
					combinedProperties[targetPropertyWithContext.Name] = propertyValue;
				}
			}
			catch (Exception ex)
			{
				if (ex.MustBeRethrownImmediately())
				{
					throw;
				}
				InternalLogger.Warn(ex, "{0}: Failed to add context property {1}", this, targetPropertyWithContext.Name);
			}
		}
		return combinedProperties;
	}

	private static bool TryGetContextPropertyValue(LogEventInfo logEvent, TargetPropertyWithContext contextProperty, out object? propertyValue)
	{
		propertyValue = contextProperty.RenderValue(logEvent);
		if (!contextProperty.IncludeEmptyValue && StringHelpers.IsNullOrEmptyString(propertyValue))
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// Takes snapshot of <see cref="T:NLog.GlobalDiagnosticsContext" /> for the <see cref="T:NLog.LogEventInfo" />
	/// </summary>
	/// <param name="logEvent"></param>
	/// <param name="contextProperties">Optional pre-allocated dictionary for the snapshot</param>
	/// <returns>Dictionary with GDC context if any, else null</returns>
	protected virtual IDictionary<string, object?>? CaptureContextGdc(LogEventInfo logEvent, IDictionary<string, object?>? contextProperties)
	{
		ICollection<string> names = GlobalDiagnosticsContext.GetNames();
		if (names.Count == 0)
		{
			return contextProperties;
		}
		contextProperties = contextProperties ?? CreateNewDictionary(names.Count);
		bool checkForDuplicates = contextProperties.Count > 0;
		ISet<string> set = ((ExcludeProperties == null || ExcludeProperties.Count == 0) ? null : ExcludeProperties);
		foreach (string item in names)
		{
			if (!string.IsNullOrEmpty(item) && (set == null || !set.Contains(item)))
			{
				object serializedValue = GlobalDiagnosticsContext.GetObject(item);
				if (SerializeItemValue(logEvent, item, serializedValue, out serializedValue))
				{
					AddContextProperty(logEvent, item, serializedValue, checkForDuplicates, contextProperties);
				}
			}
		}
		return contextProperties;
	}

	/// <summary>
	/// Takes snapshot of <see cref="T:NLog.ScopeContext" /> dictionary for the <see cref="T:NLog.LogEventInfo" />
	/// </summary>
	/// <param name="logEvent"></param>
	/// <param name="contextProperties">Optional pre-allocated dictionary for the snapshot</param>
	/// <returns>Dictionary with ScopeContext properties if any, else null</returns>
	protected virtual IDictionary<string, object?>? CaptureScopeContextProperties(LogEventInfo logEvent, IDictionary<string, object?>? contextProperties)
	{
		using ScopeContextPropertyEnumerator<object> scopeContextPropertyEnumerator = ScopeContext.GetAllPropertiesEnumerator();
		bool checkForDuplicates = contextProperties != null && contextProperties.Count > 0;
		ISet<string> set = ((ExcludeProperties == null || ExcludeProperties.Count == 0) ? null : ExcludeProperties);
		while (scopeContextPropertyEnumerator.MoveNext())
		{
			KeyValuePair<string, object> current = scopeContextPropertyEnumerator.Current;
			string key = current.Key;
			if (!string.IsNullOrEmpty(key) && (set == null || !set.Contains(key)))
			{
				contextProperties = contextProperties ?? CreateNewDictionary(0);
				object value = current.Value;
				if (SerializeScopeContextProperty(logEvent, key, value, out object serializedValue))
				{
					AddContextProperty(logEvent, key, serializedValue, checkForDuplicates, contextProperties);
				}
			}
		}
		return contextProperties;
	}

	/// <summary>
	/// Take snapshot of a single object value from <see cref="T:NLog.ScopeContext" /> dictionary
	/// </summary>
	/// <param name="logEvent">Log event</param>
	/// <param name="name">ScopeContext Dictionary key</param>
	/// <param name="value">ScopeContext Dictionary value</param>
	/// <param name="serializedValue">Snapshot of ScopeContext property-value</param>
	/// <returns>Include object value in snapshot</returns>
	protected virtual bool SerializeScopeContextProperty(LogEventInfo logEvent, string name, object? value, out object? serializedValue)
	{
		if (string.IsNullOrEmpty(name))
		{
			serializedValue = null;
			return false;
		}
		return SerializeItemValue(logEvent, name, value, out serializedValue);
	}

	/// <summary>
	/// Takes snapshot of nested states from <see cref="T:NLog.ScopeContext" /> for the <see cref="T:NLog.LogEventInfo" />
	/// </summary>
	/// <param name="logEvent"></param>
	/// <returns>Collection with <see cref="T:NLog.ScopeContext" /> stack items if any, else null</returns>
	protected virtual IList<object> CaptureScopeContextNested(LogEventInfo logEvent)
	{
		IList<object> allNestedStateList = ScopeContext.GetAllNestedStateList();
		if (allNestedStateList.Count == 0)
		{
			return allNestedStateList;
		}
		List<object> list = null;
		for (int i = 0; i < allNestedStateList.Count; i++)
		{
			object value = allNestedStateList[i];
			if (SerializeScopeContextNestedState(logEvent, value, out object serializedValue) && serializedValue != null)
			{
				if (list != null)
				{
					list.Add(serializedValue);
				}
				else
				{
					allNestedStateList[i] = serializedValue;
				}
			}
			else if (list == null)
			{
				list = new List<object>(allNestedStateList.Count);
				for (int j = 0; j < i; j++)
				{
					list.Add(allNestedStateList[j]);
				}
			}
		}
		IList<object> list2 = list;
		return list2 ?? allNestedStateList;
	}

	/// <summary>
	/// Take snapshot of a single object value from <see cref="T:NLog.ScopeContext" /> nested states
	/// </summary>
	/// <param name="logEvent">Log event</param>
	/// <param name="value"><see cref="T:NLog.ScopeContext" /> nested state value</param>
	/// <param name="serializedValue">Snapshot of <see cref="T:NLog.ScopeContext" /> stack item value</param>
	/// <returns>Include object value in snapshot</returns>
	protected virtual bool SerializeScopeContextNestedState(LogEventInfo logEvent, object value, out object? serializedValue)
	{
		return SerializeItemValue(logEvent, string.Empty, value, out serializedValue);
	}

	/// <summary>
	/// Take snapshot of a single object value
	/// </summary>
	/// <param name="logEvent">Log event</param>
	/// <param name="name">Key Name (null when NDC / NDLC)</param>
	/// <param name="value">Object Value</param>
	/// <param name="serializedValue">Snapshot of value</param>
	/// <returns>Include object value in snapshot</returns>
	protected virtual bool SerializeItemValue(LogEventInfo logEvent, string name, object? value, out object? serializedValue)
	{
		if (value == null)
		{
			serializedValue = null;
			return true;
		}
		if (value is string || Convert.GetTypeCode(value) != TypeCode.Object || value.GetType().IsValueType)
		{
			serializedValue = value;
			return true;
		}
		serializedValue = Convert.ToString(value, CultureInfo.InvariantCulture);
		return true;
	}

	/// <summary>
	/// Returns the captured snapshot of <see cref="T:NLog.NestedDiagnosticsLogicalContext" /> for the <see cref="T:NLog.LogEventInfo" />
	/// </summary>
	/// <param name="logEvent"></param>
	/// <returns>Collection with NDLC context if any, else null</returns>
	[Obsolete("Replaced by GetScopeContextNested. Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected IList<object>? GetContextNdlc(LogEventInfo logEvent)
	{
		return GetScopeContextNested(logEvent);
	}

	/// <summary>
	/// Returns the captured snapshot of <see cref="T:NLog.MappedDiagnosticsLogicalContext" /> for the <see cref="T:NLog.LogEventInfo" />
	/// </summary>
	/// <param name="logEvent"></param>
	/// <returns>Dictionary with MDLC context if any, else null</returns>
	[Obsolete("Replaced by GetScopeContextProperties. Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected IDictionary<string, object?>? GetContextMdlc(LogEventInfo logEvent)
	{
		return GetScopeContextProperties(logEvent);
	}
}
