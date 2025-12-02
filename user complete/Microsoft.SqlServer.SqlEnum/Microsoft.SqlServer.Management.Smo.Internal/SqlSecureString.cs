using System;
using System.Data.SqlTypes;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo.Internal;

internal sealed class SqlSecureString : IComparable, IComparable<SqlSecureString>, IDisposable, ICloneable
{
	private SecureString data;

	private int length;

	private static readonly SqlSecureString empty;

	public char this[int index] => ToString()[index];

	public static SqlSecureString Empty => empty;

	public int Length => length;

	static SqlSecureString()
	{
		empty = new SqlSecureString();
	}

	public SqlSecureString()
	{
		data = new SecureString();
		length = 0;
	}

	public SqlSecureString(string str)
	{
		if (str == null)
		{
			throw new ArgumentNullException();
		}
		data = new SecureString();
		length = 0;
		char[] array = str.ToCharArray();
		foreach (char c in array)
		{
			data.AppendChar(c);
			length++;
		}
	}

	public SqlSecureString(SecureString secureString)
	{
		if (secureString == null)
		{
			throw new ArgumentNullException();
		}
		data = secureString.Copy();
		length = secureString.Length;
	}

	public SqlSecureString(IntPtr bstr, int length)
	{
		if (bstr == IntPtr.Zero)
		{
			throw new ArgumentNullException();
		}
		string text = Marshal.PtrToStringAuto(bstr, length);
		data = new SecureString();
		this.length = 0;
		char[] array = text.ToCharArray();
		foreach (char c in array)
		{
			data.AppendChar(c);
			this.length++;
		}
	}

	public void Dispose()
	{
		if (data != null && (object)this != (object)empty)
		{
			data.Dispose();
			data = null;
		}
	}

	public object Clone()
	{
		return Copy();
	}

	public static int Compare(SqlSecureString strA, SqlSecureString strB)
	{
		return string.Compare((string)strA, (string)strB);
	}

	public static int Compare(SqlSecureString strA, SqlSecureString strB, bool ignoreCase)
	{
		return string.Compare((string)strA, (string)strB, ignoreCase);
	}

	public static int Compare(SqlSecureString strA, SqlSecureString strB, StringComparison comparisonType)
	{
		return string.Compare((string)strA, (string)strB, comparisonType);
	}

	public static int Compare(SqlSecureString strA, SqlSecureString strB, bool ignoreCase, CultureInfo cultureInfo)
	{
		return string.Compare((string)strA, (string)strB, ignoreCase, cultureInfo);
	}

	public static int Compare(SqlSecureString strA, int indexA, SqlSecureString strB, int indexB, int length)
	{
		return string.Compare((string)strA, indexA, (string)strB, indexB, length);
	}

	public static int Compare(SqlSecureString strA, int indexA, SqlSecureString strB, int indexB, int length, bool ignoreCase)
	{
		return string.Compare((string)strA, indexA, (string)strB, indexB, length, ignoreCase);
	}

	public static int Compare(SqlSecureString strA, int indexA, SqlSecureString strB, int indexB, int length, StringComparison comparisonType)
	{
		return string.Compare((string)strA, indexA, (string)strB, indexB, length, comparisonType);
	}

	public static int Compare(SqlSecureString strA, int indexA, SqlSecureString strB, int indexB, int length, bool ignoreCase, CultureInfo cultureInfo)
	{
		return string.Compare((string)strA, indexA, (string)strB, indexB, length, ignoreCase, cultureInfo);
	}

	public static int CompareOrdinal(SqlSecureString strA, SqlSecureString strB)
	{
		return string.CompareOrdinal((string)strA, (string)strB);
	}

	public static int CompareOrdinal(SqlSecureString strA, int indexA, SqlSecureString strB, int indexB, int length)
	{
		return string.CompareOrdinal((string)strA, indexA, (string)strB, indexB, length);
	}

	public int CompareTo(object obj)
	{
		int result = 1;
		if (obj != null)
		{
			result = ToString().CompareTo(obj.ToString());
		}
		return result;
	}

	public int CompareTo(SqlSecureString other)
	{
		int result = 1;
		if ((object)other != null)
		{
			result = ToString().CompareTo(other.ToString());
		}
		return result;
	}

	public static SqlSecureString Concat(object obj)
	{
		if (obj != null)
		{
			return new SqlSecureString(obj.ToString());
		}
		return new SqlSecureString();
	}

	public static SqlSecureString Concat(params object[] args)
	{
		return new SqlSecureString(string.Concat(args));
	}

	public bool Contains(string value)
	{
		if (value == null)
		{
			throw new ArgumentNullException();
		}
		return ToString().Contains(value);
	}

	public bool Contains(SqlSecureString value)
	{
		if (value == null)
		{
			throw new ArgumentNullException();
		}
		return ToString().Contains(value.ToString());
	}

	public SqlSecureString Copy()
	{
		return new SqlSecureString(data);
	}

	public bool EndsWith(SqlSecureString value)
	{
		if (value == null)
		{
			throw new ArgumentNullException();
		}
		return ToString().EndsWith(value.ToString());
	}

	public bool EndsWith(SqlSecureString value, bool ignoreCase, CultureInfo cultureInfo)
	{
		if (value == null)
		{
			throw new ArgumentNullException();
		}
		return ToString().EndsWith(value.ToString(), ignoreCase, cultureInfo);
	}

	public override bool Equals(object obj)
	{
		bool result = false;
		if (obj != null)
		{
			result = ToString().Equals(obj);
		}
		return result;
	}

	public bool Equals(SqlSecureString other)
	{
		return this == other;
	}

	public static bool Equals(SqlSecureString strA, SqlSecureString strB)
	{
		return strA == strB;
	}

	public bool Equals(SqlSecureString other, StringComparison comparisonType)
	{
		return ToString().Equals((string)other, comparisonType);
	}

	public static bool Equals(SqlSecureString strA, SqlSecureString strB, StringComparison comparisonType)
	{
		return string.Equals((string)strA, (string)strB, comparisonType);
	}

	public static SqlSecureString Format(string format, params object[] arguments)
	{
		return new SqlSecureString(string.Format(format, arguments));
	}

	public static SqlSecureString Format(IFormatProvider formatProvider, string format, params object[] arguments)
	{
		return new SqlSecureString(string.Format(formatProvider, format, arguments));
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

	public int IndexOf(char value)
	{
		return ToString().IndexOf(value);
	}

	public int IndexOf(string value)
	{
		return ToString().IndexOf(value);
	}

	public int IndexOf(char value, int startIndex)
	{
		return ToString().IndexOf(value, startIndex);
	}

	public int IndexOf(string value, int startIndex)
	{
		return ToString().IndexOf(value, startIndex);
	}

	public int IndexOf(char value, int startIndex, int count)
	{
		return ToString().IndexOf(value, startIndex, count);
	}

	public int IndexOf(string value, int startIndex, int count)
	{
		return ToString().IndexOf(value, startIndex, count);
	}

	public int IndexOfAny(char[] anyOf)
	{
		return ToString().IndexOfAny(anyOf);
	}

	public int IndexOfAny(char[] anyOf, int startIndex)
	{
		return ToString().IndexOfAny(anyOf, startIndex);
	}

	public int IndexOfAny(char[] anyOf, int startIndex, int count)
	{
		return ToString().IndexOfAny(anyOf, startIndex, count);
	}

	public SqlSecureString Insert(int startIndex, SqlSecureString value)
	{
		if (value == null)
		{
			throw new ArgumentNullException();
		}
		return new SqlSecureString(ToString().Insert(startIndex, value.ToString()));
	}

	public SqlSecureString Insert(int startIndex, string value)
	{
		if (value == null)
		{
			throw new ArgumentNullException();
		}
		return new SqlSecureString(ToString().Insert(startIndex, value));
	}

	public static SqlSecureString Join(object separator, object[] value)
	{
		return Join(separator, value, 0, value.Length);
	}

	public static SqlSecureString Join(object separator, object[] value, int startIndex, int count)
	{
		if (value == null)
		{
			throw new ArgumentNullException();
		}
		if (startIndex < 0 || value.Length < startIndex + count)
		{
			throw new ArgumentOutOfRangeException();
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (0 < count)
		{
			string text = ((separator != null) ? separator.ToString() : string.Empty);
			bool flag = text.Length != 0;
			int num = startIndex;
			if (value[num] != null)
			{
				stringBuilder.Append(value[num].ToString());
			}
			for (num++; num - startIndex < count; num++)
			{
				if (value[num] != null)
				{
					if (flag)
					{
						stringBuilder.Append(text);
					}
					stringBuilder.Append(value[num].ToString());
				}
			}
		}
		return new SqlSecureString(stringBuilder.ToString());
	}

	public int LastIndexOf(char value)
	{
		return ToString().LastIndexOf(value);
	}

	public int LastIndexOf(string value)
	{
		return ToString().LastIndexOf(value);
	}

	public int LastIndexOf(char value, int startIndex)
	{
		return ToString().LastIndexOf(value, startIndex);
	}

	public int LastIndexOf(string value, int startIndex)
	{
		return ToString().LastIndexOf(value, startIndex);
	}

	public int LastIndexOf(char value, int startIndex, int count)
	{
		return ToString().LastIndexOf(value, startIndex, count);
	}

	public int LastIndexOf(string value, int startIndex, int count)
	{
		return ToString().LastIndexOf(value, startIndex, count);
	}

	public int LastIndexOfAny(char[] anyOf)
	{
		return ToString().LastIndexOfAny(anyOf);
	}

	public int LastIndexOfAny(char[] anyOf, int startIndex)
	{
		return ToString().LastIndexOfAny(anyOf, startIndex);
	}

	public int LastIndexOfAny(char[] anyOf, int startIndex, int count)
	{
		return ToString().LastIndexOfAny(anyOf, startIndex, count);
	}

	public static bool operator ==(SqlSecureString strA, SqlSecureString strB)
	{
		bool result = false;
		bool flag = (object)strA == null;
		bool flag2 = (object)strB == null;
		if (flag && flag2)
		{
			result = true;
		}
		else if (!flag && !flag2)
		{
			result = 0 == string.CompareOrdinal(strA.ToString(), strB.ToString());
		}
		return result;
	}

	public static bool operator ==(SqlSecureString strA, object strB)
	{
		bool result = false;
		bool flag = (object)strA == null;
		bool flag2 = strB == null;
		if (flag && flag2)
		{
			result = true;
		}
		else if (!flag && !flag2)
		{
			result = 0 == string.CompareOrdinal(strA.ToString(), strB.ToString());
		}
		return result;
	}

	public static bool operator ==(object strA, SqlSecureString strB)
	{
		bool result = false;
		bool flag = strA == null;
		bool flag2 = (object)strB == null;
		if (flag && flag2)
		{
			result = true;
		}
		else if (!flag && !flag2)
		{
			result = 0 == string.CompareOrdinal(strA.ToString(), strB.ToString());
		}
		return result;
	}

	public static bool operator !=(SqlSecureString strA, SqlSecureString strB)
	{
		return !(strA == strB);
	}

	public static bool operator !=(SqlSecureString strA, object strB)
	{
		return !(strA == strB);
	}

	public static bool operator !=(object strA, SqlSecureString strB)
	{
		return !(strA == strB);
	}

	public static SqlSecureString operator +(SqlSecureString strA, SqlSecureString strB)
	{
		return Concat(strA, strB);
	}

	public SqlSecureString PadLeft(int totalWidth)
	{
		return new SqlSecureString(ToString().PadLeft(totalWidth));
	}

	public SqlSecureString PadLeft(int totalWidth, char paddingChar)
	{
		return new SqlSecureString(ToString().PadLeft(totalWidth, paddingChar));
	}

	public SqlSecureString PadRight(int totalWidth)
	{
		return new SqlSecureString(ToString().PadRight(totalWidth));
	}

	public SqlSecureString PadRight(int totalWidth, char paddingChar)
	{
		return new SqlSecureString(ToString().PadRight(totalWidth, paddingChar));
	}

	public SqlSecureString Remove(int startIndex)
	{
		return new SqlSecureString(ToString().Remove(startIndex));
	}

	public SqlSecureString Remove(int startIndex, int count)
	{
		return new SqlSecureString(ToString().Remove(startIndex, count));
	}

	public SqlSecureString Replace(char oldChar, char newChar)
	{
		return new SqlSecureString(ToString().Replace(oldChar, newChar));
	}

	public SqlSecureString Replace(SqlSecureString oldValue, SqlSecureString newValue)
	{
		return new SqlSecureString(ToString().Replace(oldValue.ToString(), newValue.ToString()));
	}

	public SqlSecureString[] Split(char[] separator)
	{
		return StringArrayToSqlSecureStringArray(ToString().Split(separator));
	}

	public SqlSecureString[] Split(char[] separator, int count)
	{
		return StringArrayToSqlSecureStringArray(ToString().Split(separator, count));
	}

	public SqlSecureString[] Split(char[] separator, StringSplitOptions options)
	{
		return StringArrayToSqlSecureStringArray(ToString().Split(separator, options));
	}

	public SqlSecureString[] Split(char[] separator, int count, StringSplitOptions options)
	{
		return StringArrayToSqlSecureStringArray(ToString().Split(separator, count, options));
	}

	public SqlSecureString[] Split(string[] separator, StringSplitOptions options)
	{
		return StringArrayToSqlSecureStringArray(ToString().Split(separator, options));
	}

	public SqlSecureString[] Split(string[] separator, int count, StringSplitOptions options)
	{
		return StringArrayToSqlSecureStringArray(ToString().Split(separator, count, options));
	}

	public bool StartsWith(SqlSecureString value)
	{
		if (value == null)
		{
			throw new ArgumentNullException();
		}
		return ToString().StartsWith(value.ToString());
	}

	public bool StartsWith(SqlSecureString value, bool ignoreCase, CultureInfo culture)
	{
		if (value == null)
		{
			throw new ArgumentNullException();
		}
		return ToString().StartsWith(value.ToString(), ignoreCase, culture);
	}

	public static SqlSecureString[] StringArrayToSqlSecureStringArray(string[] array)
	{
		SqlSecureString[] array2 = null;
		if (array != null)
		{
			array2 = new SqlSecureString[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null)
				{
					array2[i] = new SqlSecureString(array[i]);
				}
				else
				{
					array2[i] = null;
				}
			}
		}
		return array2;
	}

	public SqlSecureString Substring(int startIndex)
	{
		return new SqlSecureString(ToString().Substring(startIndex));
	}

	public SqlSecureString Substring(int startIndex, int length)
	{
		return new SqlSecureString(ToString().Substring(startIndex, length));
	}

	public IntPtr ToBstr()
	{
		return Marshal.SecureStringToBSTR(data);
	}

	public SqlSecureString ToLower()
	{
		return new SqlSecureString(ToString().ToLower());
	}

	public SqlSecureString ToLower(CultureInfo culture)
	{
		return new SqlSecureString(ToString().ToLower(culture));
	}

	public SqlSecureString ToLowerInvariant()
	{
		return new SqlSecureString(ToString().ToLowerInvariant());
	}

	public SecureString ToSecureString()
	{
		return data.Copy();
	}

	public override string ToString()
	{
		string result = string.Empty;
		if (Length != 0)
		{
			new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
			IntPtr intPtr = Marshal.SecureStringToBSTR(data);
			result = Marshal.PtrToStringBSTR(intPtr);
			Marshal.ZeroFreeBSTR(intPtr);
		}
		return result;
	}

	public SqlSecureString ToUpper()
	{
		return new SqlSecureString(ToString().ToUpper());
	}

	public SqlSecureString ToUpper(CultureInfo culture)
	{
		return new SqlSecureString(ToString().ToUpper(culture));
	}

	public SqlSecureString ToUpperInvariant()
	{
		return new SqlSecureString(ToString().ToUpperInvariant());
	}

	public SqlSecureString Trim()
	{
		return new SqlSecureString(ToString().Trim());
	}

	public SqlSecureString Trim(char[] trimChars)
	{
		return new SqlSecureString(ToString().Trim(trimChars));
	}

	public SqlSecureString TrimEnd(char[] trimChars)
	{
		return new SqlSecureString(ToString().TrimEnd(trimChars));
	}

	public SqlSecureString TrimStart(char[] trimChars)
	{
		return new SqlSecureString(ToString().TrimStart(trimChars));
	}

	public static explicit operator string(SqlSecureString sqlSecureString)
	{
		string result = null;
		if (sqlSecureString != null)
		{
			result = sqlSecureString.ToString();
		}
		return result;
	}

	public static implicit operator SqlSecureString(string str)
	{
		SqlSecureString result = null;
		if (str != null)
		{
			result = new SqlSecureString(str);
		}
		return result;
	}

	public static implicit operator SecureString(SqlSecureString sqlSecureString)
	{
		SecureString result = null;
		if (sqlSecureString != null)
		{
			result = sqlSecureString.data.Copy();
		}
		return result;
	}

	public static implicit operator SqlSecureString(SecureString secureString)
	{
		SqlSecureString result = null;
		if (secureString != null)
		{
			result = new SqlSecureString(secureString);
		}
		return result;
	}

	public static explicit operator SqlString(SqlSecureString sqlSecureString)
	{
		SqlString result = null;
		if (sqlSecureString != null)
		{
			result = new SqlString(sqlSecureString.ToString());
		}
		return result;
	}

	public static implicit operator SqlSecureString(SqlString str)
	{
		SqlSecureString result = null;
		if (!str.IsNull)
		{
			result = new SqlSecureString(str.ToString());
		}
		return result;
	}
}
