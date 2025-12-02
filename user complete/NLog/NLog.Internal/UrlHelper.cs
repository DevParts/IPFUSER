using System;
using System.ComponentModel;
using System.Text;

namespace NLog.Internal;

/// <summary>
/// URL Encoding helper.
/// </summary>
internal static class UrlHelper
{
	[Flags]
	public enum EscapeEncodingOptions
	{
		None = 0,
		/// <summary>Allow UnreservedMarks instead of ReservedMarks, as specified by chosen RFC</summary>
		UriString = 1,
		/// <summary>Use RFC2396 standard (instead of RFC3986)</summary>
		LegacyRfc2396 = 2,
		/// <summary>Should use lowercase when doing HEX escaping of special characters</summary>
		LowerCaseHex = 4,
		/// <summary>Replace space ' ' with '+' instead of '%20'</summary>
		SpaceAsPlus = 8,
		/// <summary>Skip UTF8 encoding, and prefix special characters with '%u'</summary>
		[Obsolete("Instead use default Rfc2396 or Rfc3986. Marked obsolete with NLog v5.3")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		NLogLegacy = 0x17
	}

	private const string RFC2396ReservedMarks = ";/?:@&=+$,";

	private const string RFC3986ReservedMarks = ":/?#[]@!$&'()*+,;=";

	private const string RFC2396UnreservedMarks = "-_.!~*'()";

	private const string RFC3986UnreservedMarks = "-._~";

	private static readonly char[] hexUpperChars = new char[16]
	{
		'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
		'A', 'B', 'C', 'D', 'E', 'F'
	};

	private static readonly char[] hexLowerChars = new char[16]
	{
		'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
		'a', 'b', 'c', 'd', 'e', 'f'
	};

	/// <summary>
	/// Escape unicode string data for use in http-requests
	/// </summary>
	/// <param name="source">unicode string-data to be encoded</param>
	/// <param name="target">target for the encoded result</param>
	/// <param name="options"><see cref="T:NLog.Internal.UrlHelper.EscapeEncodingOptions" />s for how to perform the encoding</param>
	public static void EscapeDataEncode(string source, StringBuilder target, EscapeEncodingOptions options)
	{
		if (string.IsNullOrEmpty(source))
		{
			return;
		}
		bool num = Contains(options, EscapeEncodingOptions.LowerCaseHex);
		bool flag = Contains(options, EscapeEncodingOptions.SpaceAsPlus);
		bool flag2 = Contains(options, EscapeEncodingOptions.NLogLegacy);
		char[] array = null;
		byte[] array2 = null;
		char[] hexChars = (num ? hexLowerChars : hexUpperChars);
		foreach (char c in source)
		{
			target.Append(c);
			if (IsSimpleCharOrNumber(c))
			{
				continue;
			}
			if (flag && c == ' ')
			{
				target[target.Length - 1] = '+';
			}
			else
			{
				if (IsAllowedChar(options, c))
				{
					continue;
				}
				if (flag2)
				{
					HandleLegacyEncoding(target, c, hexChars);
					continue;
				}
				if (array == null)
				{
					array = new char[1];
				}
				array[0] = c;
				if (array2 == null)
				{
					array2 = new byte[8];
				}
				WriteWideChars(target, array, array2, hexChars);
			}
		}
	}

	private static bool Contains(EscapeEncodingOptions options, EscapeEncodingOptions option)
	{
		return (options & option) == option;
	}

	/// <summary>
	/// Convert the wide-char into utf8-bytes, and then escape
	/// </summary>
	private static void WriteWideChars(StringBuilder target, char[] charArray, byte[] byteArray, char[] hexChars)
	{
		int bytes = Encoding.UTF8.GetBytes(charArray, 0, 1, byteArray, 0);
		for (int i = 0; i < bytes; i++)
		{
			byte b = byteArray[i];
			if (i == 0)
			{
				target[target.Length - 1] = '%';
			}
			else
			{
				target.Append('%');
			}
			target.Append(hexChars[(b & 0xF0) >> 4]);
			target.Append(hexChars[b & 0xF]);
		}
	}

	[Obsolete("Instead use default Rfc2396 or Rfc3986. Marked obsolete with NLog v5.3")]
	private static void HandleLegacyEncoding(StringBuilder target, char ch, char[] hexChars)
	{
		if (ch < 'Ā')
		{
			target[target.Length - 1] = '%';
			target.Append(hexChars[((int)ch >> 4) & 0xF]);
			target.Append(hexChars[ch & 0xF]);
			return;
		}
		target[target.Length - 1] = '%';
		target.Append('u');
		target.Append(hexChars[((int)ch >> 12) & 0xF]);
		target.Append(hexChars[((int)ch >> 8) & 0xF]);
		target.Append(hexChars[((int)ch >> 4) & 0xF]);
		target.Append(hexChars[ch & 0xF]);
	}

	/// <summary>
	/// Is allowed?
	/// </summary>
	/// <param name="options"></param>
	/// <param name="ch"></param>
	/// <returns></returns>
	private static bool IsAllowedChar(EscapeEncodingOptions options, char ch)
	{
		bool num = (options & EscapeEncodingOptions.UriString) == EscapeEncodingOptions.UriString;
		bool flag = (options & EscapeEncodingOptions.LegacyRfc2396) == EscapeEncodingOptions.LegacyRfc2396;
		if (num)
		{
			if (!flag && "-._~".IndexOf(ch) >= 0)
			{
				return true;
			}
			if (flag && "-_.!~*'()".IndexOf(ch) >= 0)
			{
				return true;
			}
		}
		else
		{
			if (!flag && ":/?#[]@!$&'()*+,;=".IndexOf(ch) >= 0)
			{
				return true;
			}
			if (flag && ";/?:@&=+$,".IndexOf(ch) >= 0)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Is a-z / A-Z / 0-9
	/// </summary>
	/// <param name="ch"></param>
	/// <returns></returns>
	private static bool IsSimpleCharOrNumber(char ch)
	{
		if (ch >= 'a' && ch <= 'z')
		{
			return true;
		}
		if (ch >= 'A' && ch <= 'Z')
		{
			return true;
		}
		if (ch >= '0' && ch <= '9')
		{
			return true;
		}
		return false;
	}

	public static EscapeEncodingOptions GetUriStringEncodingFlags(bool escapeDataNLogLegacy, bool spaceAsPlus, bool escapeDataRfc3986)
	{
		EscapeEncodingOptions escapeEncodingOptions = EscapeEncodingOptions.UriString;
		if (escapeDataNLogLegacy)
		{
			escapeEncodingOptions |= EscapeEncodingOptions.NLogLegacy;
		}
		else if (!escapeDataRfc3986)
		{
			escapeEncodingOptions |= EscapeEncodingOptions.LegacyRfc2396 | EscapeEncodingOptions.LowerCaseHex;
		}
		if (spaceAsPlus)
		{
			escapeEncodingOptions |= EscapeEncodingOptions.SpaceAsPlus;
		}
		return escapeEncodingOptions;
	}
}
