using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json;

/// <summary>
/// Represents a reader that provides fast, non-cached, forward-only access to serialized JSON data.
/// </summary>
public abstract class JsonReader : IDisposable
{
	/// <summary>
	/// Specifies the state of the reader.
	/// </summary>
	protected internal enum State
	{
		/// <summary>
		/// A <see cref="T:Newtonsoft.Json.JsonReader" /> read method has not been called.
		/// </summary>
		Start,
		/// <summary>
		/// The end of the file has been reached successfully.
		/// </summary>
		Complete,
		/// <summary>
		/// Reader is at a property.
		/// </summary>
		Property,
		/// <summary>
		/// Reader is at the start of an object.
		/// </summary>
		ObjectStart,
		/// <summary>
		/// Reader is in an object.
		/// </summary>
		Object,
		/// <summary>
		/// Reader is at the start of an array.
		/// </summary>
		ArrayStart,
		/// <summary>
		/// Reader is in an array.
		/// </summary>
		Array,
		/// <summary>
		/// The <see cref="M:Newtonsoft.Json.JsonReader.Close" /> method has been called.
		/// </summary>
		Closed,
		/// <summary>
		/// Reader has just read a value.
		/// </summary>
		PostValue,
		/// <summary>
		/// Reader is at the start of a constructor.
		/// </summary>
		ConstructorStart,
		/// <summary>
		/// Reader is in a constructor.
		/// </summary>
		Constructor,
		/// <summary>
		/// An error occurred that prevents the read operation from continuing.
		/// </summary>
		Error,
		/// <summary>
		/// The end of the file has been reached successfully.
		/// </summary>
		Finished
	}

	private JsonToken _tokenType;

	private object? _value;

	internal char _quoteChar;

	internal State _currentState;

	private JsonPosition _currentPosition;

	private CultureInfo? _culture;

	private DateTimeZoneHandling _dateTimeZoneHandling;

	private int? _maxDepth;

	private bool _hasExceededMaxDepth;

	internal DateParseHandling _dateParseHandling;

	internal FloatParseHandling _floatParseHandling;

	private string? _dateFormatString;

	private List<JsonPosition>? _stack;

	/// <summary>
	/// Gets the current reader state.
	/// </summary>
	/// <value>The current reader state.</value>
	protected State CurrentState => _currentState;

	/// <summary>
	/// Gets or sets a value indicating whether the source should be closed when this reader is closed.
	/// </summary>
	/// <value>
	/// <c>true</c> to close the source when this reader is closed; otherwise <c>false</c>. The default is <c>true</c>.
	/// </value>
	public bool CloseInput { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether multiple pieces of JSON content can
	/// be read from a continuous stream without erroring.
	/// </summary>
	/// <value>
	/// <c>true</c> to support reading multiple pieces of JSON content; otherwise <c>false</c>.
	/// The default is <c>false</c>.
	/// </value>
	public bool SupportMultipleContent { get; set; }

	/// <summary>
	/// Gets the quotation mark character used to enclose the value of a string.
	/// </summary>
	public virtual char QuoteChar
	{
		get
		{
			return _quoteChar;
		}
		protected internal set
		{
			_quoteChar = value;
		}
	}

	/// <summary>
	/// Gets or sets how <see cref="T:System.DateTime" /> time zones are handled when reading JSON.
	/// </summary>
	public DateTimeZoneHandling DateTimeZoneHandling
	{
		get
		{
			return _dateTimeZoneHandling;
		}
		set
		{
			if (value < DateTimeZoneHandling.Local || value > DateTimeZoneHandling.RoundtripKind)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_dateTimeZoneHandling = value;
		}
	}

	/// <summary>
	/// Gets or sets how date formatted strings, e.g. "\/Date(1198908717056)\/" and "2012-03-21T05:40Z", are parsed when reading JSON.
	/// </summary>
	public DateParseHandling DateParseHandling
	{
		get
		{
			return _dateParseHandling;
		}
		set
		{
			if (value < DateParseHandling.None || value > DateParseHandling.DateTimeOffset)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_dateParseHandling = value;
		}
	}

	/// <summary>
	/// Gets or sets how floating point numbers, e.g. 1.0 and 9.9, are parsed when reading JSON text.
	/// </summary>
	public FloatParseHandling FloatParseHandling
	{
		get
		{
			return _floatParseHandling;
		}
		set
		{
			if (value < FloatParseHandling.Double || value > FloatParseHandling.Decimal)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_floatParseHandling = value;
		}
	}

	/// <summary>
	/// Gets or sets how custom date formatted strings are parsed when reading JSON.
	/// </summary>
	public string? DateFormatString
	{
		get
		{
			return _dateFormatString;
		}
		set
		{
			_dateFormatString = value;
		}
	}

	/// <summary>
	/// Gets or sets the maximum depth allowed when reading JSON. Reading past this depth will throw a <see cref="T:Newtonsoft.Json.JsonReaderException" />.
	/// A null value means there is no maximum. 
	/// The default value is <c>64</c>.
	/// </summary>
	public int? MaxDepth
	{
		get
		{
			return _maxDepth;
		}
		set
		{
			if (value <= 0)
			{
				throw new ArgumentException("Value must be positive.", "value");
			}
			_maxDepth = value;
		}
	}

	/// <summary>
	/// Gets the type of the current JSON token. 
	/// </summary>
	public virtual JsonToken TokenType => _tokenType;

	/// <summary>
	/// Gets the text value of the current JSON token.
	/// </summary>
	public virtual object? Value => _value;

	/// <summary>
	/// Gets the .NET type for the current JSON token.
	/// </summary>
	public virtual Type? ValueType => _value?.GetType();

	/// <summary>
	/// Gets the depth of the current token in the JSON document.
	/// </summary>
	/// <value>The depth of the current token in the JSON document.</value>
	public virtual int Depth
	{
		get
		{
			int num = _stack?.Count ?? 0;
			if (JsonTokenUtils.IsStartToken(TokenType) || _currentPosition.Type == JsonContainerType.None)
			{
				return num;
			}
			return num + 1;
		}
	}

	/// <summary>
	/// Gets the path of the current JSON token. 
	/// </summary>
	public virtual string Path
	{
		get
		{
			if (_currentPosition.Type == JsonContainerType.None)
			{
				return string.Empty;
			}
			JsonPosition? currentPosition = ((_currentState != State.ArrayStart && _currentState != State.ConstructorStart && _currentState != State.ObjectStart) ? new JsonPosition?(_currentPosition) : ((JsonPosition?)null));
			return JsonPosition.BuildPath(_stack, currentPosition);
		}
	}

	/// <summary>
	/// Gets or sets the culture used when reading JSON. Defaults to <see cref="P:System.Globalization.CultureInfo.InvariantCulture" />.
	/// </summary>
	public CultureInfo Culture
	{
		get
		{
			return _culture ?? CultureInfo.InvariantCulture;
		}
		set
		{
			_culture = value;
		}
	}

	/// <summary>
	/// Asynchronously reads the next JSON token from the source.
	/// </summary>
	/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous read. The <see cref="P:System.Threading.Tasks.Task`1.Result" />
	/// property returns <c>true</c> if the next token was read successfully; <c>false</c> if there are no more tokens to read.</returns>
	/// <remarks>The default behaviour is to execute synchronously, returning an already-completed task. Derived
	/// classes can override this behaviour for true asynchronicity.</remarks>
	public virtual Task<bool> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		return cancellationToken.CancelIfRequestedAsync<bool>() ?? Read().ToAsync();
	}

	/// <summary>
	/// Asynchronously skips the children of the current token.
	/// </summary>
	/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
	/// <remarks>The default behaviour is to execute synchronously, returning an already-completed task. Derived
	/// classes can override this behaviour for true asynchronicity.</remarks>
	public async Task SkipAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		if (TokenType == JsonToken.PropertyName)
		{
			await ReadAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		if (JsonTokenUtils.IsStartToken(TokenType))
		{
			int depth = Depth;
			while (await ReadAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false) && depth < Depth)
			{
			}
		}
	}

	internal async Task ReaderReadAndAssertAsync(CancellationToken cancellationToken)
	{
		if (!(await ReadAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false)))
		{
			throw CreateUnexpectedEndException();
		}
	}

	/// <summary>
	/// Asynchronously reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" />.
	/// </summary>
	/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous read. The <see cref="P:System.Threading.Tasks.Task`1.Result" />
	/// property returns the <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" />. This result will be <c>null</c> at the end of an array.</returns>
	/// <remarks>The default behaviour is to execute synchronously, returning an already-completed task. Derived
	/// classes can override this behaviour for true asynchronicity.</remarks>
	public virtual Task<bool?> ReadAsBooleanAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		return cancellationToken.CancelIfRequestedAsync<bool?>() ?? Task.FromResult(ReadAsBoolean());
	}

	/// <summary>
	/// Asynchronously reads the next JSON token from the source as a <see cref="T:System.Byte" />[].
	/// </summary>
	/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous read. The <see cref="P:System.Threading.Tasks.Task`1.Result" />
	/// property returns the <see cref="T:System.Byte" />[]. This result will be <c>null</c> at the end of an array.</returns>
	/// <remarks>The default behaviour is to execute synchronously, returning an already-completed task. Derived
	/// classes can override this behaviour for true asynchronicity.</remarks>
	public virtual Task<byte[]?> ReadAsBytesAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		return cancellationToken.CancelIfRequestedAsync<byte[]>() ?? Task.FromResult(ReadAsBytes());
	}

	internal async Task<byte[]?> ReadArrayIntoByteArrayAsync(CancellationToken cancellationToken)
	{
		List<byte> buffer = new List<byte>();
		do
		{
			if (!(await ReadAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false)))
			{
				SetToken(JsonToken.None);
			}
		}
		while (!ReadArrayElementIntoByteArrayReportDone(buffer));
		byte[] array = buffer.ToArray();
		SetToken(JsonToken.Bytes, array, updateIndex: false);
		return array;
	}

	/// <summary>
	/// Asynchronously reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" />.
	/// </summary>
	/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous read. The <see cref="P:System.Threading.Tasks.Task`1.Result" />
	/// property returns the <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" />. This result will be <c>null</c> at the end of an array.</returns>
	/// <remarks>The default behaviour is to execute synchronously, returning an already-completed task. Derived
	/// classes can override this behaviour for true asynchronicity.</remarks>
	public virtual Task<DateTime?> ReadAsDateTimeAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		return cancellationToken.CancelIfRequestedAsync<DateTime?>() ?? Task.FromResult(ReadAsDateTime());
	}

	/// <summary>
	/// Asynchronously reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" />.
	/// </summary>
	/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous read. The <see cref="P:System.Threading.Tasks.Task`1.Result" />
	/// property returns the <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" />. This result will be <c>null</c> at the end of an array.</returns>
	/// <remarks>The default behaviour is to execute synchronously, returning an already-completed task. Derived
	/// classes can override this behaviour for true asynchronicity.</remarks>
	public virtual Task<DateTimeOffset?> ReadAsDateTimeOffsetAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		return cancellationToken.CancelIfRequestedAsync<DateTimeOffset?>() ?? Task.FromResult(ReadAsDateTimeOffset());
	}

	/// <summary>
	/// Asynchronously reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" />.
	/// </summary>
	/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous read. The <see cref="P:System.Threading.Tasks.Task`1.Result" />
	/// property returns the <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" />. This result will be <c>null</c> at the end of an array.</returns>
	/// <remarks>The default behaviour is to execute synchronously, returning an already-completed task. Derived
	/// classes can override this behaviour for true asynchronicity.</remarks>
	public virtual Task<decimal?> ReadAsDecimalAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		return cancellationToken.CancelIfRequestedAsync<decimal?>() ?? Task.FromResult(ReadAsDecimal());
	}

	/// <summary>
	/// Asynchronously reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" />.
	/// </summary>
	/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous read. The <see cref="P:System.Threading.Tasks.Task`1.Result" />
	/// property returns the <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" />. This result will be <c>null</c> at the end of an array.</returns>
	/// <remarks>The default behaviour is to execute synchronously, returning an already-completed task. Derived
	/// classes can override this behaviour for true asynchronicity.</remarks>
	public virtual Task<double?> ReadAsDoubleAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		return Task.FromResult(ReadAsDouble());
	}

	/// <summary>
	/// Asynchronously reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" />.
	/// </summary>
	/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous read. The <see cref="P:System.Threading.Tasks.Task`1.Result" />
	/// property returns the <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" />. This result will be <c>null</c> at the end of an array.</returns>
	/// <remarks>The default behaviour is to execute synchronously, returning an already-completed task. Derived
	/// classes can override this behaviour for true asynchronicity.</remarks>
	public virtual Task<int?> ReadAsInt32Async(CancellationToken cancellationToken = default(CancellationToken))
	{
		return cancellationToken.CancelIfRequestedAsync<int?>() ?? Task.FromResult(ReadAsInt32());
	}

	/// <summary>
	/// Asynchronously reads the next JSON token from the source as a <see cref="T:System.String" />.
	/// </summary>
	/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous read. The <see cref="P:System.Threading.Tasks.Task`1.Result" />
	/// property returns the <see cref="T:System.String" />. This result will be <c>null</c> at the end of an array.</returns>
	/// <remarks>The default behaviour is to execute synchronously, returning an already-completed task. Derived
	/// classes can override this behaviour for true asynchronicity.</remarks>
	public virtual Task<string?> ReadAsStringAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		return cancellationToken.CancelIfRequestedAsync<string>() ?? Task.FromResult(ReadAsString());
	}

	internal async Task<bool> ReadAndMoveToContentAsync(CancellationToken cancellationToken)
	{
		bool flag = await ReadAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (flag)
		{
			flag = await MoveToContentAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		return flag;
	}

	internal Task<bool> MoveToContentAsync(CancellationToken cancellationToken)
	{
		JsonToken tokenType = TokenType;
		if (tokenType == JsonToken.None || tokenType == JsonToken.Comment)
		{
			return MoveToContentFromNonContentAsync(cancellationToken);
		}
		return AsyncUtils.True;
	}

	private async Task<bool> MoveToContentFromNonContentAsync(CancellationToken cancellationToken)
	{
		JsonToken tokenType;
		do
		{
			if (!(await ReadAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false)))
			{
				return false;
			}
			tokenType = TokenType;
		}
		while (tokenType == JsonToken.None || tokenType == JsonToken.Comment);
		return true;
	}

	internal JsonPosition GetPosition(int depth)
	{
		if (_stack != null && depth < _stack.Count)
		{
			return _stack[depth];
		}
		return _currentPosition;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonReader" /> class.
	/// </summary>
	protected JsonReader()
	{
		_currentState = State.Start;
		_dateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
		_dateParseHandling = DateParseHandling.DateTime;
		_floatParseHandling = FloatParseHandling.Double;
		_maxDepth = 64;
		CloseInput = true;
	}

	private void Push(JsonContainerType value)
	{
		UpdateScopeWithFinishedValue();
		if (_currentPosition.Type == JsonContainerType.None)
		{
			_currentPosition = new JsonPosition(value);
			return;
		}
		if (_stack == null)
		{
			_stack = new List<JsonPosition>();
		}
		_stack.Add(_currentPosition);
		_currentPosition = new JsonPosition(value);
		if (!_maxDepth.HasValue || !(Depth + 1 > _maxDepth) || _hasExceededMaxDepth)
		{
			return;
		}
		_hasExceededMaxDepth = true;
		throw JsonReaderException.Create(this, "The reader's MaxDepth of {0} has been exceeded.".FormatWith(CultureInfo.InvariantCulture, _maxDepth));
	}

	private JsonContainerType Pop()
	{
		JsonPosition currentPosition;
		if (_stack != null && _stack.Count > 0)
		{
			currentPosition = _currentPosition;
			_currentPosition = _stack[_stack.Count - 1];
			_stack.RemoveAt(_stack.Count - 1);
		}
		else
		{
			currentPosition = _currentPosition;
			_currentPosition = default(JsonPosition);
		}
		if (_maxDepth.HasValue && Depth <= _maxDepth)
		{
			_hasExceededMaxDepth = false;
		}
		return currentPosition.Type;
	}

	private JsonContainerType Peek()
	{
		return _currentPosition.Type;
	}

	/// <summary>
	/// Reads the next JSON token from the source.
	/// </summary>
	/// <returns><c>true</c> if the next token was read successfully; <c>false</c> if there are no more tokens to read.</returns>
	public abstract bool Read();

	/// <summary>
	/// Reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" />.
	/// </summary>
	/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" />. This method will return <c>null</c> at the end of an array.</returns>
	public virtual int? ReadAsInt32()
	{
		JsonToken contentToken = GetContentToken();
		switch (contentToken)
		{
		case JsonToken.None:
		case JsonToken.Null:
		case JsonToken.EndArray:
			return null;
		case JsonToken.Integer:
		case JsonToken.Float:
		{
			object value = Value;
			if (value is int)
			{
				return (int)value;
			}
			int num;
			if (value is BigInteger bigInteger)
			{
				num = (int)bigInteger;
			}
			else
			{
				try
				{
					num = Convert.ToInt32(value, CultureInfo.InvariantCulture);
				}
				catch (Exception ex)
				{
					throw JsonReaderException.Create(this, "Could not convert to integer: {0}.".FormatWith(CultureInfo.InvariantCulture, value), ex);
				}
			}
			SetToken(JsonToken.Integer, num, updateIndex: false);
			return num;
		}
		case JsonToken.String:
		{
			string s = (string)Value;
			return ReadInt32String(s);
		}
		default:
			throw JsonReaderException.Create(this, "Error reading integer. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
		}
	}

	internal int? ReadInt32String(string? s)
	{
		if (StringUtils.IsNullOrEmpty(s))
		{
			SetToken(JsonToken.Null, null, updateIndex: false);
			return null;
		}
		if (int.TryParse(s, NumberStyles.Integer, Culture, out var result))
		{
			SetToken(JsonToken.Integer, result, updateIndex: false);
			return result;
		}
		SetToken(JsonToken.String, s, updateIndex: false);
		throw JsonReaderException.Create(this, "Could not convert string to integer: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
	}

	/// <summary>
	/// Reads the next JSON token from the source as a <see cref="T:System.String" />.
	/// </summary>
	/// <returns>A <see cref="T:System.String" />. This method will return <c>null</c> at the end of an array.</returns>
	public virtual string? ReadAsString()
	{
		JsonToken contentToken = GetContentToken();
		switch (contentToken)
		{
		case JsonToken.None:
		case JsonToken.Null:
		case JsonToken.EndArray:
			return null;
		case JsonToken.String:
			return (string)Value;
		default:
			if (JsonTokenUtils.IsPrimitiveToken(contentToken))
			{
				object value = Value;
				if (value != null)
				{
					string text = ((!(value is IFormattable formattable)) ? ((value is Uri uri) ? uri.OriginalString : value.ToString()) : formattable.ToString(null, Culture));
					SetToken(JsonToken.String, text, updateIndex: false);
					return text;
				}
			}
			throw JsonReaderException.Create(this, "Error reading string. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
		}
	}

	/// <summary>
	/// Reads the next JSON token from the source as a <see cref="T:System.Byte" />[].
	/// </summary>
	/// <returns>A <see cref="T:System.Byte" />[] or <c>null</c> if the next JSON token is null. This method will return <c>null</c> at the end of an array.</returns>
	public virtual byte[]? ReadAsBytes()
	{
		JsonToken contentToken = GetContentToken();
		switch (contentToken)
		{
		case JsonToken.StartObject:
		{
			ReadIntoWrappedTypeObject();
			byte[] array2 = ReadAsBytes();
			ReaderReadAndAssert();
			if (TokenType != JsonToken.EndObject)
			{
				throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, TokenType));
			}
			SetToken(JsonToken.Bytes, array2, updateIndex: false);
			return array2;
		}
		case JsonToken.String:
		{
			string text = (string)Value;
			Guid g;
			byte[] array3 = ((text.Length == 0) ? CollectionUtils.ArrayEmpty<byte>() : ((!ConvertUtils.TryConvertGuid(text, out g)) ? Convert.FromBase64String(text) : g.ToByteArray()));
			SetToken(JsonToken.Bytes, array3, updateIndex: false);
			return array3;
		}
		case JsonToken.None:
		case JsonToken.Null:
		case JsonToken.EndArray:
			return null;
		case JsonToken.Bytes:
			if (Value is Guid guid)
			{
				byte[] array = guid.ToByteArray();
				SetToken(JsonToken.Bytes, array, updateIndex: false);
				return array;
			}
			return (byte[])Value;
		case JsonToken.StartArray:
			return ReadArrayIntoByteArray();
		default:
			throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
		}
	}

	internal byte[] ReadArrayIntoByteArray()
	{
		List<byte> list = new List<byte>();
		do
		{
			if (!Read())
			{
				SetToken(JsonToken.None);
			}
		}
		while (!ReadArrayElementIntoByteArrayReportDone(list));
		byte[] array = list.ToArray();
		SetToken(JsonToken.Bytes, array, updateIndex: false);
		return array;
	}

	private bool ReadArrayElementIntoByteArrayReportDone(List<byte> buffer)
	{
		switch (TokenType)
		{
		case JsonToken.None:
			throw JsonReaderException.Create(this, "Unexpected end when reading bytes.");
		case JsonToken.Integer:
			buffer.Add(Convert.ToByte(Value, CultureInfo.InvariantCulture));
			return false;
		case JsonToken.EndArray:
			return true;
		case JsonToken.Comment:
			return false;
		default:
			throw JsonReaderException.Create(this, "Unexpected token when reading bytes: {0}.".FormatWith(CultureInfo.InvariantCulture, TokenType));
		}
	}

	/// <summary>
	/// Reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" />.
	/// </summary>
	/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" />. This method will return <c>null</c> at the end of an array.</returns>
	public virtual double? ReadAsDouble()
	{
		JsonToken contentToken = GetContentToken();
		switch (contentToken)
		{
		case JsonToken.None:
		case JsonToken.Null:
		case JsonToken.EndArray:
			return null;
		case JsonToken.Integer:
		case JsonToken.Float:
		{
			object value = Value;
			if (value is double)
			{
				return (double)value;
			}
			double num = ((!(value is BigInteger bigInteger)) ? Convert.ToDouble(value, CultureInfo.InvariantCulture) : ((double)bigInteger));
			SetToken(JsonToken.Float, num, updateIndex: false);
			return num;
		}
		case JsonToken.String:
			return ReadDoubleString((string)Value);
		default:
			throw JsonReaderException.Create(this, "Error reading double. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
		}
	}

	internal double? ReadDoubleString(string? s)
	{
		if (StringUtils.IsNullOrEmpty(s))
		{
			SetToken(JsonToken.Null, null, updateIndex: false);
			return null;
		}
		if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, Culture, out var result))
		{
			SetToken(JsonToken.Float, result, updateIndex: false);
			return result;
		}
		SetToken(JsonToken.String, s, updateIndex: false);
		throw JsonReaderException.Create(this, "Could not convert string to double: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
	}

	/// <summary>
	/// Reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" />.
	/// </summary>
	/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" />. This method will return <c>null</c> at the end of an array.</returns>
	public virtual bool? ReadAsBoolean()
	{
		JsonToken contentToken = GetContentToken();
		switch (contentToken)
		{
		case JsonToken.None:
		case JsonToken.Null:
		case JsonToken.EndArray:
			return null;
		case JsonToken.Integer:
		case JsonToken.Float:
		{
			bool flag = ((!(Value is BigInteger bigInteger)) ? Convert.ToBoolean(Value, CultureInfo.InvariantCulture) : (bigInteger != 0L));
			SetToken(JsonToken.Boolean, flag, updateIndex: false);
			return flag;
		}
		case JsonToken.String:
			return ReadBooleanString((string)Value);
		case JsonToken.Boolean:
			return (bool)Value;
		default:
			throw JsonReaderException.Create(this, "Error reading boolean. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
		}
	}

	internal bool? ReadBooleanString(string? s)
	{
		if (StringUtils.IsNullOrEmpty(s))
		{
			SetToken(JsonToken.Null, null, updateIndex: false);
			return null;
		}
		if (bool.TryParse(s, out var result))
		{
			SetToken(JsonToken.Boolean, result, updateIndex: false);
			return result;
		}
		SetToken(JsonToken.String, s, updateIndex: false);
		throw JsonReaderException.Create(this, "Could not convert string to boolean: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
	}

	/// <summary>
	/// Reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" />.
	/// </summary>
	/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" />. This method will return <c>null</c> at the end of an array.</returns>
	public virtual decimal? ReadAsDecimal()
	{
		JsonToken contentToken = GetContentToken();
		switch (contentToken)
		{
		case JsonToken.None:
		case JsonToken.Null:
		case JsonToken.EndArray:
			return null;
		case JsonToken.Integer:
		case JsonToken.Float:
		{
			object value = Value;
			if (value is decimal)
			{
				return (decimal)value;
			}
			decimal num;
			if (value is BigInteger bigInteger)
			{
				num = (decimal)bigInteger;
			}
			else
			{
				try
				{
					num = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
				}
				catch (Exception ex)
				{
					throw JsonReaderException.Create(this, "Could not convert to decimal: {0}.".FormatWith(CultureInfo.InvariantCulture, value), ex);
				}
			}
			SetToken(JsonToken.Float, num, updateIndex: false);
			return num;
		}
		case JsonToken.String:
			return ReadDecimalString((string)Value);
		default:
			throw JsonReaderException.Create(this, "Error reading decimal. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
		}
	}

	internal decimal? ReadDecimalString(string? s)
	{
		if (StringUtils.IsNullOrEmpty(s))
		{
			SetToken(JsonToken.Null, null, updateIndex: false);
			return null;
		}
		if (decimal.TryParse(s, NumberStyles.Number, Culture, out var result))
		{
			SetToken(JsonToken.Float, result, updateIndex: false);
			return result;
		}
		if (ConvertUtils.DecimalTryParse(s.ToCharArray(), 0, s.Length, out result) == ParseResult.Success)
		{
			SetToken(JsonToken.Float, result, updateIndex: false);
			return result;
		}
		SetToken(JsonToken.String, s, updateIndex: false);
		throw JsonReaderException.Create(this, "Could not convert string to decimal: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
	}

	/// <summary>
	/// Reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" />.
	/// </summary>
	/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" />. This method will return <c>null</c> at the end of an array.</returns>
	public virtual DateTime? ReadAsDateTime()
	{
		switch (GetContentToken())
		{
		case JsonToken.None:
		case JsonToken.Null:
		case JsonToken.EndArray:
			return null;
		case JsonToken.Date:
			if (Value is DateTimeOffset dateTimeOffset)
			{
				SetToken(JsonToken.Date, dateTimeOffset.DateTime, updateIndex: false);
			}
			return (DateTime)Value;
		case JsonToken.String:
			return ReadDateTimeString((string)Value);
		default:
			throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, TokenType));
		}
	}

	internal DateTime? ReadDateTimeString(string? s)
	{
		if (StringUtils.IsNullOrEmpty(s))
		{
			SetToken(JsonToken.Null, null, updateIndex: false);
			return null;
		}
		if (DateTimeUtils.TryParseDateTime(s, DateTimeZoneHandling, _dateFormatString, Culture, out var dt))
		{
			dt = DateTimeUtils.EnsureDateTime(dt, DateTimeZoneHandling);
			SetToken(JsonToken.Date, dt, updateIndex: false);
			return dt;
		}
		if (DateTime.TryParse(s, Culture, DateTimeStyles.RoundtripKind, out dt))
		{
			dt = DateTimeUtils.EnsureDateTime(dt, DateTimeZoneHandling);
			SetToken(JsonToken.Date, dt, updateIndex: false);
			return dt;
		}
		throw JsonReaderException.Create(this, "Could not convert string to DateTime: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
	}

	/// <summary>
	/// Reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" />.
	/// </summary>
	/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" />. This method will return <c>null</c> at the end of an array.</returns>
	public virtual DateTimeOffset? ReadAsDateTimeOffset()
	{
		JsonToken contentToken = GetContentToken();
		switch (contentToken)
		{
		case JsonToken.None:
		case JsonToken.Null:
		case JsonToken.EndArray:
			return null;
		case JsonToken.Date:
			if (Value is DateTime dateTime)
			{
				SetToken(JsonToken.Date, new DateTimeOffset(dateTime), updateIndex: false);
			}
			return (DateTimeOffset)Value;
		case JsonToken.String:
		{
			string s = (string)Value;
			return ReadDateTimeOffsetString(s);
		}
		default:
			throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
		}
	}

	internal DateTimeOffset? ReadDateTimeOffsetString(string? s)
	{
		if (StringUtils.IsNullOrEmpty(s))
		{
			SetToken(JsonToken.Null, null, updateIndex: false);
			return null;
		}
		if (DateTimeUtils.TryParseDateTimeOffset(s, _dateFormatString, Culture, out var dt))
		{
			SetToken(JsonToken.Date, dt, updateIndex: false);
			return dt;
		}
		if (DateTimeOffset.TryParse(s, Culture, DateTimeStyles.RoundtripKind, out dt))
		{
			SetToken(JsonToken.Date, dt, updateIndex: false);
			return dt;
		}
		SetToken(JsonToken.String, s, updateIndex: false);
		throw JsonReaderException.Create(this, "Could not convert string to DateTimeOffset: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
	}

	internal void ReaderReadAndAssert()
	{
		if (!Read())
		{
			throw CreateUnexpectedEndException();
		}
	}

	internal JsonReaderException CreateUnexpectedEndException()
	{
		return JsonReaderException.Create(this, "Unexpected end when reading JSON.");
	}

	internal void ReadIntoWrappedTypeObject()
	{
		ReaderReadAndAssert();
		if (Value != null && Value.ToString() == "$type")
		{
			ReaderReadAndAssert();
			if (Value != null && Value.ToString().StartsWith("System.Byte[]", StringComparison.Ordinal))
			{
				ReaderReadAndAssert();
				if (Value.ToString() == "$value")
				{
					return;
				}
			}
		}
		throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, JsonToken.StartObject));
	}

	/// <summary>
	/// Skips the children of the current token.
	/// </summary>
	public void Skip()
	{
		if (TokenType == JsonToken.PropertyName)
		{
			Read();
		}
		if (JsonTokenUtils.IsStartToken(TokenType))
		{
			int depth = Depth;
			while (Read() && depth < Depth)
			{
			}
		}
	}

	/// <summary>
	/// Sets the current token.
	/// </summary>
	/// <param name="newToken">The new token.</param>
	protected void SetToken(JsonToken newToken)
	{
		SetToken(newToken, null, updateIndex: true);
	}

	/// <summary>
	/// Sets the current token and value.
	/// </summary>
	/// <param name="newToken">The new token.</param>
	/// <param name="value">The value.</param>
	protected void SetToken(JsonToken newToken, object? value)
	{
		SetToken(newToken, value, updateIndex: true);
	}

	/// <summary>
	/// Sets the current token and value.
	/// </summary>
	/// <param name="newToken">The new token.</param>
	/// <param name="value">The value.</param>
	/// <param name="updateIndex">A flag indicating whether the position index inside an array should be updated.</param>
	protected void SetToken(JsonToken newToken, object? value, bool updateIndex)
	{
		_tokenType = newToken;
		_value = value;
		switch (newToken)
		{
		case JsonToken.StartObject:
			_currentState = State.ObjectStart;
			Push(JsonContainerType.Object);
			break;
		case JsonToken.StartArray:
			_currentState = State.ArrayStart;
			Push(JsonContainerType.Array);
			break;
		case JsonToken.StartConstructor:
			_currentState = State.ConstructorStart;
			Push(JsonContainerType.Constructor);
			break;
		case JsonToken.EndObject:
			ValidateEnd(JsonToken.EndObject);
			break;
		case JsonToken.EndArray:
			ValidateEnd(JsonToken.EndArray);
			break;
		case JsonToken.EndConstructor:
			ValidateEnd(JsonToken.EndConstructor);
			break;
		case JsonToken.PropertyName:
			_currentState = State.Property;
			_currentPosition.PropertyName = (string)value;
			break;
		case JsonToken.Raw:
		case JsonToken.Integer:
		case JsonToken.Float:
		case JsonToken.String:
		case JsonToken.Boolean:
		case JsonToken.Null:
		case JsonToken.Undefined:
		case JsonToken.Date:
		case JsonToken.Bytes:
			SetPostValueState(updateIndex);
			break;
		case JsonToken.Comment:
			break;
		}
	}

	internal void SetPostValueState(bool updateIndex)
	{
		if (Peek() != JsonContainerType.None || SupportMultipleContent)
		{
			_currentState = State.PostValue;
		}
		else
		{
			SetFinished();
		}
		if (updateIndex)
		{
			UpdateScopeWithFinishedValue();
		}
	}

	private void UpdateScopeWithFinishedValue()
	{
		if (_currentPosition.HasIndex)
		{
			_currentPosition.Position++;
		}
	}

	private void ValidateEnd(JsonToken endToken)
	{
		JsonContainerType jsonContainerType = Pop();
		if (GetTypeForCloseToken(endToken) != jsonContainerType)
		{
			throw JsonReaderException.Create(this, "JsonToken {0} is not valid for closing JsonType {1}.".FormatWith(CultureInfo.InvariantCulture, endToken, jsonContainerType));
		}
		if (Peek() != JsonContainerType.None || SupportMultipleContent)
		{
			_currentState = State.PostValue;
		}
		else
		{
			SetFinished();
		}
	}

	/// <summary>
	/// Sets the state based on current token type.
	/// </summary>
	protected void SetStateBasedOnCurrent()
	{
		JsonContainerType jsonContainerType = Peek();
		switch (jsonContainerType)
		{
		case JsonContainerType.Object:
			_currentState = State.Object;
			break;
		case JsonContainerType.Array:
			_currentState = State.Array;
			break;
		case JsonContainerType.Constructor:
			_currentState = State.Constructor;
			break;
		case JsonContainerType.None:
			SetFinished();
			break;
		default:
			throw JsonReaderException.Create(this, "While setting the reader state back to current object an unexpected JsonType was encountered: {0}".FormatWith(CultureInfo.InvariantCulture, jsonContainerType));
		}
	}

	private void SetFinished()
	{
		_currentState = ((!SupportMultipleContent) ? State.Finished : State.Start);
	}

	private JsonContainerType GetTypeForCloseToken(JsonToken token)
	{
		return token switch
		{
			JsonToken.EndObject => JsonContainerType.Object, 
			JsonToken.EndArray => JsonContainerType.Array, 
			JsonToken.EndConstructor => JsonContainerType.Constructor, 
			_ => throw JsonReaderException.Create(this, "Not a valid close JsonToken: {0}".FormatWith(CultureInfo.InvariantCulture, token)), 
		};
	}

	void IDisposable.Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Releases unmanaged and - optionally - managed resources.
	/// </summary>
	/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (_currentState != State.Closed && disposing)
		{
			Close();
		}
	}

	/// <summary>
	/// Changes the reader's state to <see cref="F:Newtonsoft.Json.JsonReader.State.Closed" />.
	/// If <see cref="P:Newtonsoft.Json.JsonReader.CloseInput" /> is set to <c>true</c>, the source is also closed.
	/// </summary>
	public virtual void Close()
	{
		_currentState = State.Closed;
		_tokenType = JsonToken.None;
		_value = null;
	}

	internal void ReadAndAssert()
	{
		if (!Read())
		{
			throw JsonSerializationException.Create(this, "Unexpected end when reading JSON.");
		}
	}

	internal void ReadForTypeAndAssert(JsonContract? contract, bool hasConverter)
	{
		if (!ReadForType(contract, hasConverter))
		{
			throw JsonSerializationException.Create(this, "Unexpected end when reading JSON.");
		}
	}

	internal bool ReadForType(JsonContract? contract, bool hasConverter)
	{
		if (hasConverter)
		{
			return Read();
		}
		switch (contract?.InternalReadType ?? ReadType.Read)
		{
		case ReadType.Read:
			return ReadAndMoveToContent();
		case ReadType.ReadAsInt32:
			ReadAsInt32();
			break;
		case ReadType.ReadAsInt64:
		{
			bool result = ReadAndMoveToContent();
			if (TokenType == JsonToken.Undefined)
			{
				throw JsonReaderException.Create(this, "An undefined token is not a valid {0}.".FormatWith(CultureInfo.InvariantCulture, contract?.UnderlyingType ?? typeof(long)));
			}
			return result;
		}
		case ReadType.ReadAsDecimal:
			ReadAsDecimal();
			break;
		case ReadType.ReadAsDouble:
			ReadAsDouble();
			break;
		case ReadType.ReadAsBytes:
			ReadAsBytes();
			break;
		case ReadType.ReadAsBoolean:
			ReadAsBoolean();
			break;
		case ReadType.ReadAsString:
			ReadAsString();
			break;
		case ReadType.ReadAsDateTime:
			ReadAsDateTime();
			break;
		case ReadType.ReadAsDateTimeOffset:
			ReadAsDateTimeOffset();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return TokenType != JsonToken.None;
	}

	internal bool ReadAndMoveToContent()
	{
		if (Read())
		{
			return MoveToContent();
		}
		return false;
	}

	internal bool MoveToContent()
	{
		JsonToken tokenType = TokenType;
		while (tokenType == JsonToken.None || tokenType == JsonToken.Comment)
		{
			if (!Read())
			{
				return false;
			}
			tokenType = TokenType;
		}
		return true;
	}

	private JsonToken GetContentToken()
	{
		JsonToken tokenType;
		do
		{
			if (!Read())
			{
				SetToken(JsonToken.None);
				return JsonToken.None;
			}
			tokenType = TokenType;
		}
		while (tokenType == JsonToken.Comment);
		return tokenType;
	}
}
