using System;

namespace NLog.Config;

/// <summary>
/// Identifies that the output of layout or layout render does not change for the lifetime of the current appdomain.
/// </summary>
/// <remarks>
///
/// Implementors must have the [ThreadAgnostic] attribute
///
/// A layout(renderer) could be converted to a literal when:
///  - The layout and all layout properties are SimpleLayout or [AppDomainFixedOutput]
///
/// Recommendation: Apply this attribute to a layout or layout-renderer which have the result only changes by properties of type Layout.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AppDomainFixedOutputAttribute : Attribute
{
}
