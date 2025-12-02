using System;
using JetBrains.Annotations;

namespace NLog.Config;

/// <summary>
/// Used to mark configurable parameters which are arrays.
/// Specifies the mapping between XML elements and .NET types.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
[MeansImplicitUse]
public sealed class ArrayParameterAttribute : Attribute
{
	/// <summary>
	/// Gets the .NET type of the array item.
	/// </summary>
	public Type ItemType { get; }

	/// <summary>
	/// Gets the XML element name.
	/// </summary>
	public string ElementName { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.ArrayParameterAttribute" /> class.
	/// </summary>
	/// <param name="itemType">The type of the array item.</param>
	/// <param name="elementName">The XML element name that represents the item.</param>
	public ArrayParameterAttribute(Type itemType, string elementName)
	{
		ItemType = itemType;
		ElementName = elementName;
	}
}
