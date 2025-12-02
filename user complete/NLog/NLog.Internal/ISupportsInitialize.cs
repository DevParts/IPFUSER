using NLog.Config;

namespace NLog.Internal;

/// <summary>
/// Supports object initialization and termination.
/// </summary>
internal interface ISupportsInitialize
{
	/// <summary>
	/// Initializes this instance.
	/// </summary>
	void Initialize(LoggingConfiguration? configuration);

	/// <summary>
	/// Closes this instance.
	/// </summary>
	void Close();
}
