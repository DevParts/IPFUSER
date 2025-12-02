namespace NLog;

/// <summary>
/// Interface for handling object transformation
/// </summary>
internal interface IObjectTypeTransformer
{
	/// <summary>
	/// Takes a dangerous (or massive) object and converts into a safe (or reduced) object
	/// </summary>
	/// <returns>
	/// Null if unknown object, or object cannot be handled
	/// </returns>
	object? TryTransformObject(object obj);
}
