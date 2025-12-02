using NLog.Conditions;
using NLog.Filters;
using NLog.LayoutRenderers;
using NLog.LayoutRenderers.Wrappers;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;
using NLog.Time;

namespace NLog.Config;

/// <summary>
/// Provides logging interface and utility functions.
/// </summary>
internal static class AssemblyExtensionTypes
{
	public static void RegisterTargetTypes(ConfigurationItemFactory factory, bool skipCheckExists)
	{
		factory.RegisterTypeProperties<TargetWithContext.TargetWithContextLayout>(() => (object?)null);
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("EventLog"))
		{
			factory.GetTargetFactory().RegisterType<EventLogTarget>("EventLog");
		}
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("ColoredConsole"))
		{
			factory.GetTargetFactory().RegisterType<ColoredConsoleTarget>("ColoredConsole");
		}
		factory.RegisterType<ConsoleRowHighlightingRule>();
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("Console"))
		{
			factory.GetTargetFactory().RegisterType<ConsoleTarget>("Console");
		}
		factory.RegisterType<ConsoleWordHighlightingRule>();
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("Debugger"))
		{
			factory.GetTargetFactory().RegisterType<DebuggerTarget>("Debugger");
		}
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("DebugSystem"))
		{
			factory.GetTargetFactory().RegisterType<DebugSystemTarget>("DebugSystem");
		}
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("Debug"))
		{
			factory.GetTargetFactory().RegisterType<DebugTarget>("Debug");
		}
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("File"))
		{
			factory.GetTargetFactory().RegisterType<FileTarget>("File");
		}
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("Memory"))
		{
			factory.GetTargetFactory().RegisterType<MemoryTarget>("Memory");
		}
		factory.RegisterType<MethodCallParameter>();
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("MethodCall"))
		{
			factory.GetTargetFactory().RegisterType<MethodCallTarget>("MethodCall");
		}
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("Null"))
		{
			factory.GetTargetFactory().RegisterType<NullTarget>("Null");
		}
		factory.RegisterType<TargetPropertyWithContext>();
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("AsyncWrapper"))
		{
			factory.GetTargetFactory().RegisterType<AsyncTargetWrapper>("AsyncWrapper");
		}
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("AutoFlushWrapper"))
		{
			factory.GetTargetFactory().RegisterType<AutoFlushTargetWrapper>("AutoFlushWrapper");
		}
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("BufferingWrapper"))
		{
			factory.GetTargetFactory().RegisterType<BufferingTargetWrapper>("BufferingWrapper");
		}
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("FallbackGroup"))
		{
			factory.GetTargetFactory().RegisterType<FallbackGroupTarget>("FallbackGroup");
		}
		factory.RegisterType<FilteringRule>();
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("FilteringWrapper"))
		{
			factory.GetTargetFactory().RegisterType<FilteringTargetWrapper>("FilteringWrapper");
		}
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("GroupByWrapper"))
		{
			factory.GetTargetFactory().RegisterType<GroupByTargetWrapper>("GroupByWrapper");
		}
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("LimitingWrapper"))
		{
			factory.GetTargetFactory().RegisterType<LimitingTargetWrapper>("LimitingWrapper");
		}
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("PostFilteringWrapper"))
		{
			factory.GetTargetFactory().RegisterType<PostFilteringTargetWrapper>("PostFilteringWrapper");
		}
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("RandomizeGroup"))
		{
			factory.GetTargetFactory().RegisterType<RandomizeGroupTarget>("RandomizeGroup");
		}
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("RepeatingWrapper"))
		{
			factory.GetTargetFactory().RegisterType<RepeatingTargetWrapper>("RepeatingWrapper");
		}
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("RetryingWrapper"))
		{
			factory.GetTargetFactory().RegisterType<RetryingTargetWrapper>("RetryingWrapper");
		}
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("RoundRobinGroup"))
		{
			factory.GetTargetFactory().RegisterType<RoundRobinGroupTarget>("RoundRobinGroup");
		}
		if (skipCheckExists || !factory.GetTargetFactory().CheckTypeAliasExists("SplitGroup"))
		{
			factory.GetTargetFactory().RegisterType<SplitGroupTarget>("SplitGroup");
		}
	}

	public static void RegisterLayoutTypes(ConfigurationItemFactory factory, bool skipCheckExists)
	{
		factory.RegisterTypeProperties<CsvLayout.CsvHeaderLayout>(() => (object?)null);
		if (skipCheckExists || !factory.GetLayoutFactory().CheckTypeAliasExists("CompoundLayout"))
		{
			factory.GetLayoutFactory().RegisterType<CompoundLayout>("CompoundLayout");
		}
		factory.RegisterType<CsvColumn>();
		if (skipCheckExists || !factory.GetLayoutFactory().CheckTypeAliasExists("CsvLayout"))
		{
			factory.GetLayoutFactory().RegisterType<CsvLayout>("CsvLayout");
		}
		if (skipCheckExists || !factory.GetLayoutFactory().CheckTypeAliasExists("JsonArrayLayout"))
		{
			factory.GetLayoutFactory().RegisterType<JsonArrayLayout>("JsonArrayLayout");
		}
		factory.RegisterType<JsonAttribute>();
		if (skipCheckExists || !factory.GetLayoutFactory().CheckTypeAliasExists("JsonLayout"))
		{
			factory.GetLayoutFactory().RegisterType<JsonLayout>("JsonLayout");
		}
		if (skipCheckExists || !factory.GetLayoutFactory().CheckTypeAliasExists("LayoutWithHeaderAndFooter"))
		{
			factory.GetLayoutFactory().RegisterType<LayoutWithHeaderAndFooter>("LayoutWithHeaderAndFooter");
		}
		if (skipCheckExists || !factory.GetLayoutFactory().CheckTypeAliasExists("SimpleLayout"))
		{
			factory.GetLayoutFactory().RegisterType<SimpleLayout>("SimpleLayout");
		}
		factory.RegisterType<ValueTypeLayoutInfo>();
		factory.RegisterType<XmlAttribute>();
		if (skipCheckExists || !factory.GetLayoutFactory().CheckTypeAliasExists("XmlLayout"))
		{
			factory.GetLayoutFactory().RegisterType<XmlLayout>("XmlLayout");
		}
	}

	public static void RegisterLayoutRendererTypes(ConfigurationItemFactory factory, bool skipCheckExists)
	{
		factory.GetLayoutRendererFactory().RegisterType<AppSettingLayoutRenderer>("appsetting");
		factory.RegisterTypeProperties<LiteralWithRawValueLayoutRenderer>(() => (object?)null);
		factory.RegisterTypeProperties<FuncLayoutRenderer>(() => (object?)null);
		factory.RegisterTypeProperties<FuncThreadAgnosticLayoutRenderer>(() => (object?)null);
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("all-event-properties"))
		{
			factory.GetLayoutRendererFactory().RegisterType<AllEventPropertiesLayoutRenderer>("all-event-properties");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("appdomain"))
		{
			factory.GetLayoutRendererFactory().RegisterType<AppDomainLayoutRenderer>("appdomain");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("assembly-version"))
		{
			factory.GetLayoutRendererFactory().RegisterType<AssemblyVersionLayoutRenderer>("assembly-version");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("basedir"))
		{
			factory.GetLayoutRendererFactory().RegisterType<BaseDirLayoutRenderer>("basedir");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("callsite-filename"))
		{
			factory.GetLayoutRendererFactory().RegisterType<CallSiteFileNameLayoutRenderer>("callsite-filename");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("callsite"))
		{
			factory.GetLayoutRendererFactory().RegisterType<CallSiteLayoutRenderer>("callsite");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("callsite-linenumber"))
		{
			factory.GetLayoutRendererFactory().RegisterType<CallSiteLineNumberLayoutRenderer>("callsite-linenumber");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("counter"))
		{
			factory.GetLayoutRendererFactory().RegisterType<CounterLayoutRenderer>("counter");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("currentdir"))
		{
			factory.GetLayoutRendererFactory().RegisterType<CurrentDirLayoutRenderer>("currentdir");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("date"))
		{
			factory.GetLayoutRendererFactory().RegisterType<DateLayoutRenderer>("date");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("db-null"))
		{
			factory.GetLayoutRendererFactory().RegisterType<DbNullLayoutRenderer>("db-null");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("dir-separator"))
		{
			factory.GetLayoutRendererFactory().RegisterType<DirectorySeparatorLayoutRenderer>("dir-separator");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("environment"))
		{
			factory.GetLayoutRendererFactory().RegisterType<EnvironmentLayoutRenderer>("environment");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("environment-user"))
		{
			factory.GetLayoutRendererFactory().RegisterType<EnvironmentUserLayoutRenderer>("environment-user");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("event-properties"))
		{
			factory.GetLayoutRendererFactory().RegisterType<EventPropertiesLayoutRenderer>("event-properties");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("event-property"))
		{
			factory.GetLayoutRendererFactory().RegisterType<EventPropertiesLayoutRenderer>("event-property");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("event-context"))
		{
			factory.GetLayoutRendererFactory().RegisterType<EventPropertiesLayoutRenderer>("event-context");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("exceptiondata"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ExceptionDataLayoutRenderer>("exceptiondata");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("exception-data"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ExceptionDataLayoutRenderer>("exception-data");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("exception"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ExceptionLayoutRenderer>("exception");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("gc"))
		{
			factory.GetLayoutRendererFactory().RegisterType<GarbageCollectorInfoLayoutRenderer>("gc");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("gdc"))
		{
			factory.GetLayoutRendererFactory().RegisterType<GdcLayoutRenderer>("gdc");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("guid"))
		{
			factory.GetLayoutRendererFactory().RegisterType<GuidLayoutRenderer>("guid");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("hostname"))
		{
			factory.GetLayoutRendererFactory().RegisterType<HostNameLayoutRenderer>("hostname");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("identity"))
		{
			factory.GetLayoutRendererFactory().RegisterType<IdentityLayoutRenderer>("identity");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("install-context"))
		{
			factory.GetLayoutRendererFactory().RegisterType<InstallContextLayoutRenderer>("install-context");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("level"))
		{
			factory.GetLayoutRendererFactory().RegisterType<LevelLayoutRenderer>("level");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("loglevel"))
		{
			factory.GetLayoutRendererFactory().RegisterType<LevelLayoutRenderer>("loglevel");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("literal"))
		{
			factory.GetLayoutRendererFactory().RegisterType<LiteralLayoutRenderer>("literal");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("loggername"))
		{
			factory.GetLayoutRendererFactory().RegisterType<LoggerNameLayoutRenderer>("loggername");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("logger"))
		{
			factory.GetLayoutRendererFactory().RegisterType<LoggerNameLayoutRenderer>("logger");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("longdate"))
		{
			factory.GetLayoutRendererFactory().RegisterType<LongDateLayoutRenderer>("longdate");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("machinename"))
		{
			factory.GetLayoutRendererFactory().RegisterType<MachineNameLayoutRenderer>("machinename");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("message"))
		{
			factory.GetLayoutRendererFactory().RegisterType<MessageLayoutRenderer>("message");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("newline"))
		{
			factory.GetLayoutRendererFactory().RegisterType<NewLineLayoutRenderer>("newline");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("nlogdir"))
		{
			factory.GetLayoutRendererFactory().RegisterType<NLogDirLayoutRenderer>("nlogdir");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("processdir"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ProcessDirLayoutRenderer>("processdir");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("processid"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ProcessIdLayoutRenderer>("processid");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("processinfo"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ProcessInfoLayoutRenderer>("processinfo");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("processname"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ProcessNameLayoutRenderer>("processname");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("processtime"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ProcessTimeLayoutRenderer>("processtime");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("scopeindent"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ScopeContextIndentLayoutRenderer>("scopeindent");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("scopenested"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ScopeContextNestedStatesLayoutRenderer>("scopenested");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("ndc"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ScopeContextNestedStatesLayoutRenderer>("ndc");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("ndlc"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ScopeContextNestedStatesLayoutRenderer>("ndlc");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("scopeproperty"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ScopeContextPropertyLayoutRenderer>("scopeproperty");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("mdc"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ScopeContextPropertyLayoutRenderer>("mdc");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("mdlc"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ScopeContextPropertyLayoutRenderer>("mdlc");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("scopetiming"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ScopeContextTimingLayoutRenderer>("scopetiming");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("ndlctiming"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ScopeContextTimingLayoutRenderer>("ndlctiming");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("sequenceid"))
		{
			factory.GetLayoutRendererFactory().RegisterType<SequenceIdLayoutRenderer>("sequenceid");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("shortdate"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ShortDateLayoutRenderer>("shortdate");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("userApplicationDataDir"))
		{
			factory.GetLayoutRendererFactory().RegisterType<SpecialFolderApplicationDataLayoutRenderer>("userApplicationDataDir");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("commonApplicationDataDir"))
		{
			factory.GetLayoutRendererFactory().RegisterType<SpecialFolderCommonApplicationDataLayoutRenderer>("commonApplicationDataDir");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("specialfolder"))
		{
			factory.GetLayoutRendererFactory().RegisterType<SpecialFolderLayoutRenderer>("specialfolder");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("userLocalApplicationDataDir"))
		{
			factory.GetLayoutRendererFactory().RegisterType<SpecialFolderLocalApplicationDataLayoutRenderer>("userLocalApplicationDataDir");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("stacktrace"))
		{
			factory.GetLayoutRendererFactory().RegisterType<StackTraceLayoutRenderer>("stacktrace");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("tempdir"))
		{
			factory.GetLayoutRendererFactory().RegisterType<TempDirLayoutRenderer>("tempdir");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("threadid"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ThreadIdLayoutRenderer>("threadid");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("threadname"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ThreadNameLayoutRenderer>("threadname");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("ticks"))
		{
			factory.GetLayoutRendererFactory().RegisterType<TicksLayoutRenderer>("ticks");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("time"))
		{
			factory.GetLayoutRendererFactory().RegisterType<TimeLayoutRenderer>("time");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("var"))
		{
			factory.GetLayoutRendererFactory().RegisterType<VariableLayoutRenderer>("var");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("cached"))
		{
			factory.GetLayoutRendererFactory().RegisterType<CachedLayoutRendererWrapper>("cached");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("Cached"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<CachedLayoutRendererWrapper>("Cached");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("ClearCache"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<CachedLayoutRendererWrapper>("ClearCache");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("CachedSeconds"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<CachedLayoutRendererWrapper>("CachedSeconds");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("filesystem-normalize"))
		{
			factory.GetLayoutRendererFactory().RegisterType<FileSystemNormalizeLayoutRendererWrapper>("filesystem-normalize");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("FSNormalize"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<FileSystemNormalizeLayoutRendererWrapper>("FSNormalize");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("json-encode"))
		{
			factory.GetLayoutRendererFactory().RegisterType<JsonEncodeLayoutRendererWrapper>("json-encode");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("JsonEncode"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<JsonEncodeLayoutRendererWrapper>("JsonEncode");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("left"))
		{
			factory.GetLayoutRendererFactory().RegisterType<LeftLayoutRendererWrapper>("left");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("Truncate"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<LeftLayoutRendererWrapper>("Truncate");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("lowercase"))
		{
			factory.GetLayoutRendererFactory().RegisterType<LowercaseLayoutRendererWrapper>("lowercase");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("Lowercase"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<LowercaseLayoutRendererWrapper>("Lowercase");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("ToLower"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<LowercaseLayoutRendererWrapper>("ToLower");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("norawvalue"))
		{
			factory.GetLayoutRendererFactory().RegisterType<NoRawValueLayoutRendererWrapper>("norawvalue");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("NoRawValue"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<NoRawValueLayoutRendererWrapper>("NoRawValue");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("Object-Path"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ObjectPathRendererWrapper>("Object-Path");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("ObjectPath"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<ObjectPathRendererWrapper>("ObjectPath");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("onexception"))
		{
			factory.GetLayoutRendererFactory().RegisterType<OnExceptionLayoutRendererWrapper>("onexception");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("onhasproperties"))
		{
			factory.GetLayoutRendererFactory().RegisterType<OnHasPropertiesLayoutRendererWrapper>("onhasproperties");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("pad"))
		{
			factory.GetLayoutRendererFactory().RegisterType<PaddingLayoutRendererWrapper>("pad");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("Padding"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<PaddingLayoutRendererWrapper>("Padding");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("PadCharacter"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<PaddingLayoutRendererWrapper>("PadCharacter");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("FixedLength"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<PaddingLayoutRendererWrapper>("FixedLength");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("AlignmentOnTruncation"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<PaddingLayoutRendererWrapper>("AlignmentOnTruncation");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("replace"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ReplaceLayoutRendererWrapper>("replace");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("replace-newlines"))
		{
			factory.GetLayoutRendererFactory().RegisterType<ReplaceNewLinesLayoutRendererWrapper>("replace-newlines");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("ReplaceNewLines"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<ReplaceNewLinesLayoutRendererWrapper>("ReplaceNewLines");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("right"))
		{
			factory.GetLayoutRendererFactory().RegisterType<RightLayoutRendererWrapper>("right");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("rot13"))
		{
			factory.GetLayoutRendererFactory().RegisterType<Rot13LayoutRendererWrapper>("rot13");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("substring"))
		{
			factory.GetLayoutRendererFactory().RegisterType<SubstringLayoutRendererWrapper>("substring");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("trim-whitespace"))
		{
			factory.GetLayoutRendererFactory().RegisterType<TrimWhiteSpaceLayoutRendererWrapper>("trim-whitespace");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("TrimWhiteSpace"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<TrimWhiteSpaceLayoutRendererWrapper>("TrimWhiteSpace");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("uppercase"))
		{
			factory.GetLayoutRendererFactory().RegisterType<UppercaseLayoutRendererWrapper>("uppercase");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("Uppercase"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<UppercaseLayoutRendererWrapper>("Uppercase");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("ToUpper"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<UppercaseLayoutRendererWrapper>("ToUpper");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("url-encode"))
		{
			factory.GetLayoutRendererFactory().RegisterType<UrlEncodeLayoutRendererWrapper>("url-encode");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("whenEmpty"))
		{
			factory.GetLayoutRendererFactory().RegisterType<WhenEmptyLayoutRendererWrapper>("whenEmpty");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("WhenEmpty"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<WhenEmptyLayoutRendererWrapper>("WhenEmpty");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("when"))
		{
			factory.GetLayoutRendererFactory().RegisterType<WhenLayoutRendererWrapper>("when");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("When"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<WhenLayoutRendererWrapper>("When");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("wrapline"))
		{
			factory.GetLayoutRendererFactory().RegisterType<WrapLineLayoutRendererWrapper>("wrapline");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("WrapLine"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<WrapLineLayoutRendererWrapper>("WrapLine");
		}
		if (skipCheckExists || !factory.GetLayoutRendererFactory().CheckTypeAliasExists("xml-encode"))
		{
			factory.GetLayoutRendererFactory().RegisterType<XmlEncodeLayoutRendererWrapper>("xml-encode");
		}
		if (skipCheckExists || !factory.GetAmbientPropertyFactory().CheckTypeAliasExists("XmlEncode"))
		{
			factory.GetAmbientPropertyFactory().RegisterType<XmlEncodeLayoutRendererWrapper>("XmlEncode");
		}
	}

	public static void RegisterFilterTypes(ConfigurationItemFactory factory, bool skipCheckExists)
	{
		if (skipCheckExists || !factory.GetFilterFactory().CheckTypeAliasExists("when"))
		{
			factory.GetFilterFactory().RegisterType<ConditionBasedFilter>("when");
		}
		if (skipCheckExists || !factory.GetFilterFactory().CheckTypeAliasExists("whenContains"))
		{
			factory.GetFilterFactory().RegisterType<WhenContainsFilter>("whenContains");
		}
		if (skipCheckExists || !factory.GetFilterFactory().CheckTypeAliasExists("whenEqual"))
		{
			factory.GetFilterFactory().RegisterType<WhenEqualFilter>("whenEqual");
		}
		if (skipCheckExists || !factory.GetFilterFactory().CheckTypeAliasExists("whenNotContains"))
		{
			factory.GetFilterFactory().RegisterType<WhenNotContainsFilter>("whenNotContains");
		}
		if (skipCheckExists || !factory.GetFilterFactory().CheckTypeAliasExists("whenNotEqual"))
		{
			factory.GetFilterFactory().RegisterType<WhenNotEqualFilter>("whenNotEqual");
		}
		if (skipCheckExists || !factory.GetFilterFactory().CheckTypeAliasExists("whenRepeated"))
		{
			factory.GetFilterFactory().RegisterType<WhenRepeatedFilter>("whenRepeated");
		}
	}

	public static void RegisterTimeSourceTypes(ConfigurationItemFactory factory, bool skipCheckExists)
	{
		if (skipCheckExists || !factory.GetTimeSourceFactory().CheckTypeAliasExists("AccurateLocal"))
		{
			factory.GetTimeSourceFactory().RegisterType<AccurateLocalTimeSource>("AccurateLocal");
		}
		if (skipCheckExists || !factory.GetTimeSourceFactory().CheckTypeAliasExists("AccurateUTC"))
		{
			factory.GetTimeSourceFactory().RegisterType<AccurateUtcTimeSource>("AccurateUTC");
		}
		if (skipCheckExists || !factory.GetTimeSourceFactory().CheckTypeAliasExists("FastLocal"))
		{
			factory.GetTimeSourceFactory().RegisterType<FastLocalTimeSource>("FastLocal");
		}
		if (skipCheckExists || !factory.GetTimeSourceFactory().CheckTypeAliasExists("FastUTC"))
		{
			factory.GetTimeSourceFactory().RegisterType<FastUtcTimeSource>("FastUTC");
		}
	}

	public static void RegisterConditionTypes(ConfigurationItemFactory factory, bool skipCheckExists)
	{
		factory.RegisterTypeProperties<ConditionAndExpression>(() => (object?)null);
		factory.RegisterTypeProperties<ConditionExceptionExpression>(() => (object?)null);
		factory.RegisterTypeProperties<ConditionLayoutExpression>(() => (object?)null);
		factory.RegisterTypeProperties<ConditionLevelExpression>(() => (object?)null);
		factory.RegisterTypeProperties<ConditionLiteralExpression>(() => (object?)null);
		factory.RegisterTypeProperties<ConditionLoggerNameExpression>(() => (object?)null);
		factory.RegisterTypeProperties<ConditionMessageExpression>(() => (object?)null);
		factory.RegisterTypeProperties<ConditionMethodExpression>(() => (object?)null);
		factory.RegisterTypeProperties<ConditionNotExpression>(() => (object?)null);
		factory.RegisterTypeProperties<ConditionOrExpression>(() => (object?)null);
		factory.RegisterTypeProperties<ConditionRelationalExpression>(() => (object?)null);
		if (skipCheckExists || !factory.GetConditionMethodFactory().CheckTypeAliasExists("length"))
		{
			factory.GetConditionMethodFactory().RegisterOneParameter("length", (LogEventInfo logEvent, object? arg1) => ConditionMethods.Length(arg1?.ToString()));
		}
		if (skipCheckExists || !factory.GetConditionMethodFactory().CheckTypeAliasExists("equals"))
		{
			factory.GetConditionMethodFactory().RegisterTwoParameters("equals", (LogEventInfo logEvent, object? arg1, object? arg2) => (!ConditionMethods.Equals2(arg1?.ToString(), arg2?.ToString())) ? ConditionExpression.BoxedFalse : ConditionExpression.BoxedTrue);
		}
		if (skipCheckExists || !factory.GetConditionMethodFactory().CheckTypeAliasExists("strequals"))
		{
			factory.GetConditionMethodFactory().RegisterTwoParameters("strequals", (LogEventInfo logEvent, object? arg1, object? arg2) => (!ConditionMethods.Equals2(arg1?.ToString(), arg2?.ToString())) ? ConditionExpression.BoxedFalse : ConditionExpression.BoxedTrue);
			factory.GetConditionMethodFactory().RegisterThreeParameters("strequals", (LogEventInfo logEvent, object? arg1, object? arg2, object? arg3) => (!ConditionMethods.Equals2(arg1?.ToString(), arg2?.ToString(), !(arg3 is bool flag) || flag)) ? ConditionExpression.BoxedFalse : ConditionExpression.BoxedTrue);
		}
		if (skipCheckExists || !factory.GetConditionMethodFactory().CheckTypeAliasExists("contains"))
		{
			factory.GetConditionMethodFactory().RegisterTwoParameters("contains", (LogEventInfo logEvent, object? arg1, object? arg2) => (!ConditionMethods.Contains(arg1?.ToString(), arg2?.ToString())) ? ConditionExpression.BoxedFalse : ConditionExpression.BoxedTrue);
			factory.GetConditionMethodFactory().RegisterThreeParameters("contains", (LogEventInfo logEvent, object? arg1, object? arg2, object? arg3) => (!ConditionMethods.Contains(arg1?.ToString(), arg2?.ToString(), !(arg3 is bool flag) || flag)) ? ConditionExpression.BoxedFalse : ConditionExpression.BoxedTrue);
		}
		if (skipCheckExists || !factory.GetConditionMethodFactory().CheckTypeAliasExists("starts-with"))
		{
			factory.GetConditionMethodFactory().RegisterTwoParameters("starts-with", (LogEventInfo logEvent, object? arg1, object? arg2) => (!ConditionMethods.StartsWith(arg1?.ToString(), arg2?.ToString())) ? ConditionExpression.BoxedFalse : ConditionExpression.BoxedTrue);
			factory.GetConditionMethodFactory().RegisterThreeParameters("starts-with", (LogEventInfo logEvent, object? arg1, object? arg2, object? arg3) => (!ConditionMethods.StartsWith(arg1?.ToString(), arg2?.ToString(), !(arg3 is bool flag) || flag)) ? ConditionExpression.BoxedFalse : ConditionExpression.BoxedTrue);
		}
		if (skipCheckExists || !factory.GetConditionMethodFactory().CheckTypeAliasExists("ends-with"))
		{
			factory.GetConditionMethodFactory().RegisterTwoParameters("ends-with", (LogEventInfo logEvent, object? arg1, object? arg2) => (!ConditionMethods.EndsWith(arg1?.ToString(), arg2?.ToString())) ? ConditionExpression.BoxedFalse : ConditionExpression.BoxedTrue);
			factory.GetConditionMethodFactory().RegisterThreeParameters("ends-with", (LogEventInfo logEvent, object? arg1, object? arg2, object? arg3) => (!ConditionMethods.EndsWith(arg1?.ToString(), arg2?.ToString(), !(arg3 is bool flag) || flag)) ? ConditionExpression.BoxedFalse : ConditionExpression.BoxedTrue);
		}
	}
}
