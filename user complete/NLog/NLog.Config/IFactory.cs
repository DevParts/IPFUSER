using System;
using System.Diagnostics.CodeAnalysis;

namespace NLog.Config;

/// <summary>
/// Factory of named items (such as <see cref="T:NLog.Targets.Target" />, <see cref="T:NLog.Layouts.Layout" />, <see cref="T:NLog.LayoutRenderers.LayoutRenderer" />, etc.).
/// </summary>
internal interface IFactory
{
	void Clear();

	void RegisterType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicProperties)] Type type, string itemNamePrefix);
}
/// <summary>
/// Factory of named items (such as <see cref="T:NLog.Targets.Target" />, <see cref="T:NLog.Layouts.Layout" />, <see cref="T:NLog.LayoutRenderers.LayoutRenderer" />, etc.).
/// </summary>
public interface IFactory<TBaseType> where TBaseType : class
{
	/// <summary>
	/// Tries to create an item instance with type-alias
	/// </summary>
	/// <returns><see langword="true" /> if instance was created successfully, <see langword="false" /> otherwise.</returns>
	bool TryCreateInstance(string typeAlias, out TBaseType? result);
}
