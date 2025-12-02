namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Specifies the types of members that are dynamically accessed.
///
/// This enumeration has a <see cref="T:System.FlagsAttribute" /> attribute that allows a
/// bitwise combination of its member values.
/// </summary>
[Flags]
internal enum DynamicallyAccessedMemberTypes
{
	/// <summary>
	/// Specifies no members.
	/// </summary>
	None = 0,
	/// <summary>
	/// Specifies the default, parameterless public constructor.
	/// </summary>
	PublicParameterlessConstructor = 1,
	/// <summary>
	/// Specifies all public constructors.
	/// </summary>
	PublicConstructors = 3,
	/// <summary>
	/// Specifies all non-public constructors.
	/// </summary>
	NonPublicConstructors = 4,
	/// <summary>
	/// Specifies all public methods.
	/// </summary>
	PublicMethods = 8,
	/// <summary>
	/// Specifies all non-public methods.
	/// </summary>
	NonPublicMethods = 0x10,
	/// <summary>
	/// Specifies all public fields.
	/// </summary>
	PublicFields = 0x20,
	/// <summary>
	/// Specifies all non-public fields.
	/// </summary>
	NonPublicFields = 0x40,
	/// <summary>
	/// Specifies all public nested types.
	/// </summary>
	PublicNestedTypes = 0x80,
	/// <summary>
	/// Specifies all non-public nested types.
	/// </summary>
	NonPublicNestedTypes = 0x100,
	/// <summary>
	/// Specifies all public properties.
	/// </summary>
	PublicProperties = 0x200,
	/// <summary>
	/// Specifies all non-public properties.
	/// </summary>
	NonPublicProperties = 0x400,
	/// <summary>
	/// Specifies all public events.
	/// </summary>
	PublicEvents = 0x800,
	/// <summary>
	/// Specifies all non-public events.
	/// </summary>
	NonPublicEvents = 0x1000,
	/// <summary>
	/// Specifies all interfaces implemented by the type.
	/// </summary>
	Interfaces = 0x2000,
	/// <summary>
	/// Specifies all members.
	/// </summary>
	All = -1
}
