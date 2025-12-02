using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[CompilerGenerated]
internal class StringDifferencing
{
	[CompilerGenerated]
	public class Keys
	{
		public const string MismatchType = "MismatchType";

		public const string NotRecognizedGraph = "NotRecognizedGraph";

		public const string CannotFindMetadataProvider = "CannotFindMetadataProvider";

		public const string FailedProviderLookup = "FailedProviderLookup";

		public const string FailedProviderOperation = "FailedProviderOperation";

		public const string NotRecognizedProvider = "NotRecognizedProvider";

		private static ResourceManager resourceManager = new ResourceManager(typeof(StringDifferencing).FullName, typeof(StringDifferencing).Module.Assembly);

		private static CultureInfo _culture = null;

		public static CultureInfo Culture
		{
			get
			{
				return _culture;
			}
			set
			{
				_culture = value;
			}
		}

		private Keys()
		{
		}

		public static string GetString(string key)
		{
			return resourceManager.GetString(key, _culture);
		}

		public static string GetString(string key, object arg0, object arg1)
		{
			return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), new object[2] { arg0, arg1 });
		}
	}

	public static CultureInfo Culture
	{
		get
		{
			return Keys.Culture;
		}
		set
		{
			Keys.Culture = value;
		}
	}

	public static string NotRecognizedGraph => Keys.GetString("NotRecognizedGraph");

	public static string CannotFindMetadataProvider => Keys.GetString("CannotFindMetadataProvider");

	public static string NotRecognizedProvider => Keys.GetString("NotRecognizedProvider");

	protected StringDifferencing()
	{
	}

	public static string MismatchType(string type, string provider)
	{
		return Keys.GetString("MismatchType", type, provider);
	}

	public static string FailedProviderLookup(string provider, string node)
	{
		return Keys.GetString("FailedProviderLookup", provider, node);
	}

	public static string FailedProviderOperation(string provider, string node)
	{
		return Keys.GetString("FailedProviderOperation", provider, node);
	}
}
