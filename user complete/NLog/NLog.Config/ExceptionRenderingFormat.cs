namespace NLog.Config;

/// <summary>
/// Format of the exception output to the specific target.
/// </summary>
public enum ExceptionRenderingFormat
{
	/// <summary>
	/// Appends the Message of an Exception to the specified target.
	/// </summary>
	Message,
	/// <summary>
	/// Appends the type of an Exception to the specified target.
	/// </summary>
	Type,
	/// <summary>
	/// Appends the short type of an Exception to the specified target.
	/// </summary>
	ShortType,
	/// <summary>
	/// Appends the result of calling ToString() on an Exception to the specified target.
	/// </summary>
	ToString,
	/// <summary>
	/// Appends the method name from Exception's stack trace to the specified target.
	/// </summary>
	Method,
	/// <summary>
	/// Appends the stack trace from an Exception to the specified target.
	/// </summary>
	StackTrace,
	/// <summary>
	/// Appends the contents of an Exception's Data property to the specified target.
	/// </summary>
	Data,
	/// <summary>
	/// Destructure the exception (usually into JSON)
	/// </summary>
	Serialize,
	/// <summary>
	/// Appends the <see cref="P:System.Exception.Source" /> from the application or the object that caused the error.
	/// </summary>
	Source,
	/// <summary>
	/// Appends the <see cref="P:System.Exception.HResult" /> from the application or the object that caused the error.
	/// </summary>
	HResult,
	/// <summary>
	/// Appends any additional properties that specific type of Exception might have.
	/// </summary>
	Properties
}
