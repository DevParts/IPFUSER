namespace System.Diagnostics.CodeAnalysis;

/// <summary>Specifies that an output will not be null even if the corresponding type allows it.</summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
internal sealed class NotNullAttribute : Attribute
{
}
