using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcTsqlProcFormatter
{
	public struct SprocArg
	{
		public string argName;

		public string property;

		public bool required;

		public bool output;

		public SprocArg(string name, string property, bool required, bool output)
		{
			argName = name;
			TraceHelper.Assert(!string.IsNullOrEmpty(property));
			this.property = property;
			this.required = required;
			this.output = output;
		}

		public SprocArg(string name, bool required)
		{
			argName = name;
			property = string.Empty;
			this.required = required;
			output = false;
		}

		public SprocArg(string name, bool required, bool output)
		{
			argName = name;
			property = string.Empty;
			this.required = required;
			this.output = output;
		}
	}

	public struct RuntimeArg
	{
		public Type type;

		public object value;

		public RuntimeArg(Type type, object value)
		{
			this.type = type;
			this.value = value;
		}
	}

	private List<SprocArg> arguments;

	private string procedure;

	public string Procedure
	{
		get
		{
			return procedure;
		}
		set
		{
			procedure = value;
		}
	}

	public List<SprocArg> Arguments => arguments;

	public SfcTsqlProcFormatter()
	{
		arguments = new List<SprocArg>();
	}

	public string GenerateScript(SfcInstance sfcObject)
	{
		return GenerateScript(sfcObject, null);
	}

	public string GenerateScript(SfcInstance sfcObject, IEnumerable<RuntimeArg> runtimeArgs)
	{
		return GenerateScript(sfcObject, runtimeArgs, declareArguments: true);
	}

	public string GenerateScript(SfcInstance sfcObject, IEnumerable<RuntimeArg> runtimeArgs, bool declareArguments)
	{
		IEnumerator<RuntimeArg> enumerator = null;
		if (runtimeArgs != null)
		{
			enumerator = runtimeArgs.GetEnumerator();
			enumerator.Reset();
			enumerator.MoveNext();
		}
		string value = string.Empty;
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		foreach (SprocArg argument in Arguments)
		{
			if (!argument.output)
			{
				continue;
			}
			if (flag)
			{
				TraceHelper.Assert(condition: false, "More than one OUTPUT parameters specified!");
				continue;
			}
			Type type;
			if (string.IsNullOrEmpty(argument.property))
			{
				type = enumerator.Current.type;
			}
			else
			{
				SfcProperty sfcProperty = sfcObject.Properties[argument.property];
				type = sfcProperty.Type;
			}
			if (declareArguments)
			{
				stringBuilder.AppendFormat("Declare @{0} ", argument.argName);
				if (type == typeof(int))
				{
					stringBuilder.Append("int");
				}
				else if (type == typeof(long))
				{
					stringBuilder.Append("bigint");
				}
				else
				{
					TraceHelper.Assert(condition: false, "Unexpected OUTPUT parameter type!");
				}
				stringBuilder.AppendLine();
			}
			value = "Select @" + argument.argName;
			flag = true;
		}
		stringBuilder.Append("EXEC ");
		stringBuilder.Append(Procedure);
		bool flag2 = true;
		foreach (SprocArg argument2 in Arguments)
		{
			if (!string.IsNullOrEmpty(argument2.property))
			{
				SfcProperty sfcProperty2 = sfcObject.Properties[argument2.property];
				if (sfcProperty2.Dirty)
				{
					flag2 = false;
					break;
				}
			}
		}
		int num = 0;
		foreach (SprocArg argument3 in Arguments)
		{
			Type type2 = null;
			object obj = null;
			if (string.IsNullOrEmpty(argument3.property))
			{
				TraceHelper.Assert(null != enumerator, $"No runtimeArgsEnum but there is no property with the name '!{argument3.argName}");
				RuntimeArg current4 = enumerator.Current;
				enumerator.MoveNext();
				type2 = current4.type;
				obj = current4.value;
			}
			else
			{
				SfcProperty sfcProperty3 = sfcObject.Properties[argument3.property];
				if (argument3.required)
				{
					if (null == sfcProperty3)
					{
						throw new SfcPropertyNotSetException(argument3.property);
					}
				}
				else if (!argument3.output && !sfcProperty3.Dirty && !flag2)
				{
					continue;
				}
				type2 = sfcProperty3.Type;
				obj = sfcProperty3.Value;
			}
			if (num > 0)
			{
				stringBuilder.Append(",");
			}
			if (argument3.output)
			{
				stringBuilder.AppendFormat(" @{0}=@{0} OUTPUT", argument3.argName);
			}
			else
			{
				stringBuilder.AppendFormat(" @{0}=", argument3.argName);
				if (type2 == typeof(string))
				{
					if (obj == null)
					{
						obj = string.Empty;
					}
					stringBuilder.Append(MakeSqlString((string)obj));
				}
				else if (type2 == typeof(DateTime))
				{
					if (obj == null)
					{
						obj = DateTime.MinValue;
					}
					stringBuilder.Append(MakeSqlString(((DateTime)obj).ToString("s", CultureInfo.InvariantCulture)));
				}
				else if (type2 == typeof(DateTimeOffset))
				{
					if (obj == null)
					{
						obj = DateTimeOffset.MinValue;
					}
					stringBuilder.Append(MakeSqlString(((DateTimeOffset)obj).ToString("o", CultureInfo.InvariantCulture)));
				}
				else if (type2 == typeof(TimeSpan))
				{
					if (obj == null)
					{
						obj = TimeSpan.MinValue;
					}
					stringBuilder.Append(MakeSqlString(((TimeSpan)obj/*cast due to .constrained prefix*/).ToString()));
				}
				else if (type2 == typeof(Guid) || type2 == typeof(SfcQueryExpression))
				{
					if (obj == null)
					{
						obj = ((!(type2 == typeof(Guid))) ? new SfcQueryExpression("") : ((object)Guid.Empty));
					}
					stringBuilder.Append(MakeSqlString(obj.ToString()));
				}
				else if (type2.IsEnum())
				{
					if (obj == null)
					{
						obj = 0;
					}
					stringBuilder.Append(Convert.ToInt32(obj));
				}
				else if (type2 == typeof(byte[]))
				{
					stringBuilder.Append("0x");
					stringBuilder.Append(ConvertToHexBinary((byte[])obj).ToString());
				}
				else
				{
					if (obj == null)
					{
						obj = "null";
					}
					stringBuilder.Append(obj);
				}
			}
			num++;
		}
		if (flag)
		{
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(value);
		}
		return stringBuilder.ToString();
	}

	private string ConvertToHexBinary(byte[] byteValue)
	{
		return new SoapHexBinary(byteValue).ToString();
	}

	public static string EscapeString(string value, char charToEscape)
	{
		if (value == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in value)
		{
			stringBuilder.Append(c);
			if (charToEscape == c)
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	public static string SqlString(string value)
	{
		return EscapeString(value, '\'');
	}

	public static string MakeSqlString(string value)
	{
		return string.Format(CultureInfo.InvariantCulture, "N'{0}'", new object[1] { EscapeString(value, '\'') });
	}

	public static string SqlBracket(string value)
	{
		return EscapeString(value, ']');
	}

	public static string MakeSqlBracket(string value)
	{
		return string.Format(CultureInfo.InvariantCulture, "[{0}]", new object[1] { EscapeString(value, ']') });
	}

	public static string SqlStringBracket(string value)
	{
		return SqlBracket(SqlString(value));
	}
}
