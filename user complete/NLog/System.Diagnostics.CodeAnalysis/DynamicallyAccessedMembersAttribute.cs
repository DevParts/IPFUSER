namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, Inherited = false)]
internal sealed class DynamicallyAccessedMembersAttribute : Attribute
{
	/// <summary>
	/// Gets the <see cref="T:System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes" /> which specifies the type
	/// of members dynamically accessed.
	/// </summary>
	public DynamicallyAccessedMemberTypes MemberTypes { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute" /> class
	/// with the specified member types.
	/// </summary>
	/// <param name="memberTypes">The types of members dynamically accessed.</param>
	public DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes)
	{
		MemberTypes = memberTypes;
	}
}
