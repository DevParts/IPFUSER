using System;

namespace JetBrains.Annotations;

/// <summary>
/// Indicates that the marked method builds string by the format pattern and (optional) arguments.
/// The parameter, which contains the format string, should be given in the constructor. The format string
/// should be in <see cref="M:System.String.Format(System.IFormatProvider,System.String,System.Object[])" />-like form.
/// </summary>
/// <example><code>
/// [StringFormatMethod("message")]
/// void ShowError(string message, params object[] args) { /* do something */ }
///
/// void Foo() {
///   ShowError("Failed: {0}"); // Warning: Non-existing argument in format string
/// }
/// </code></example>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Delegate)]
internal sealed class StringFormatMethodAttribute : Attribute
{
	[NotNull]
	public string FormatParameterName { get; }

	/// <param name="formatParameterName">
	/// Specifies which parameter of an annotated method should be treated as the format string
	/// </param>
	public StringFormatMethodAttribute([NotNull] string formatParameterName)
	{
		FormatParameterName = formatParameterName;
	}
}
