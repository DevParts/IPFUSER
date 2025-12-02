using System;

namespace NLog.Config;

internal static class FactoryExtensions
{
	public static TBaseType CreateInstance<TBaseType>(this IFactory<TBaseType> factory, string typeAlias) where TBaseType : class
	{
		try
		{
			if (factory.TryCreateInstance(typeAlias, out TBaseType result))
			{
				if (result == null)
				{
					throw new NLogConfigurationException("Failed to create " + typeof(TBaseType).Name + " of type: '" + typeAlias + "' - Factory method returned null");
				}
				return result;
			}
		}
		catch (Exception ex)
		{
			throw new NLogConfigurationException("Failed to create " + typeof(TBaseType).Name + " of type: '" + typeAlias + "' - " + ex.Message, ex);
		}
		string text = NormalizeName(typeAlias);
		string text2 = "Failed to create " + typeof(TBaseType).Name + " with unknown type-alias: '" + typeAlias + "'";
		text2 = ((!text.StartsWith("aspnet", StringComparison.OrdinalIgnoreCase) && !text.StartsWith("iis", StringComparison.OrdinalIgnoreCase)) ? (text.StartsWith("HostAppName", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Web.AspNetCore not included?") : (text.StartsWith("HostEnvironment", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Web.AspNetCore not included?") : (text.StartsWith("HostRootDir", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Web.AspNetCore not included?") : (text.StartsWith("configsetting", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Extensions.Logging not included?") : (text.StartsWith("MicrosoftConsoleJsonLayout", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Extensions.Logging not included?") : (text.StartsWith("MicrosoftConsoleLayout", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Extensions.Logging not included?") : (text.StartsWith("database", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Database not included?") : (text.StartsWith("network", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Targets.Network not included?") : (text.StartsWith("nlogviewer", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Targets.Network not included?") : (text.StartsWith("chainsaw", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Targets.Network not included?") : (text.StartsWith("Log4JXml", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Targets.Network not included?") : (text.StartsWith("syslog", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Targets.Network not included?") : (text.StartsWith("gelf", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Targets.Network not included?") : (text.StartsWith("localip", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Targets.Network not included?") : (text.StartsWith("webservice", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Targets.WebService not included?") : ((text.StartsWith("atomFile", StringComparison.OrdinalIgnoreCase) || text.StartsWith("atomicFile", StringComparison.OrdinalIgnoreCase)) ? (text2 + " - Extension NLog.Targets.AtomicFile not included?") : (text.StartsWith("GZipFile", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Targets.GZipFile not included?") : (text.StartsWith("trace", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Targets.Trace not included?") : (text.StartsWith("activityid", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Targets.Trace not included?") : (text.StartsWith("mailkit", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.MailKit not included?") : (text.StartsWith("mail", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.Targets.Mail not included?") : (text.StartsWith("eventlog", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.WindowsEventLog not included?") : (text.StartsWith("windowsidentity", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.WindowsIdentity not included?") : (text.StartsWith("outputdebugstring", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.OutputDebugString not included?") : (text.StartsWith("performancecounter", StringComparison.OrdinalIgnoreCase) ? (text2 + " - Extension NLog.PerformanceCounter not included?") : ((!text.StartsWith("regexreplace", StringComparison.OrdinalIgnoreCase)) ? (text2 + " - Verify type-alias and check extension is included.") : (text2 + " - Extension NLog.RegEx not included?"))))))))))))))))))))))))))) : (text2 + " - Extension NLog.Web not included?"));
		throw new NLogConfigurationException(text2);
	}

	public static string NormalizeName(string itemName)
	{
		if (itemName == null)
		{
			return string.Empty;
		}
		if (itemName.IndexOf('-') < 0)
		{
			return itemName;
		}
		int num = itemName.IndexOf(',');
		if (num >= 0)
		{
			string text = itemName.Substring(0, num).Replace("-", string.Empty);
			string text2 = itemName.Substring(num);
			return text + text2;
		}
		return itemName.Replace("-", string.Empty);
	}
}
