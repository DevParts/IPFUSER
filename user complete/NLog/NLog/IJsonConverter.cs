using System.Text;

namespace NLog;

/// <summary>
/// Interface for serialization of object values into JSON format
/// </summary>
public interface IJsonConverter
{
	/// <summary>
	/// Serialization of an object into JSON format.
	/// </summary>
	/// <param name="value">The object to serialize to JSON.</param>
	/// <param name="builder">Output destination.</param>
	/// <returns>Serialize succeeded (true/false)</returns>
	bool SerializeObject(object? value, StringBuilder builder);
}
