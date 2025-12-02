namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Specifies that the method will not return if the associated Boolean parameter is passed the specified value.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal class DoesNotReturnIfAttribute : Attribute
{
	/// <summary>Gets the condition parameter value.</summary>
	public bool ParameterValue { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:System.Diagnostics.CodeAnalysis.DoesNotReturnIfAttribute" /> class.
	/// </summary>
	/// <param name="parameterValue">
	/// The condition parameter value. Code after the method will be considered unreachable by diagnostics if the argument to
	/// the associated parameter matches this value.
	/// </param>
	public DoesNotReturnIfAttribute(bool parameterValue)
	{
		ParameterValue = parameterValue;
	}
}
