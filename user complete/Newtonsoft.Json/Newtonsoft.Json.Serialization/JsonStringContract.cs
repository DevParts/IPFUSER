using System;
using System.Diagnostics.CodeAnalysis;

namespace Newtonsoft.Json.Serialization;

/// <summary>
/// Contract details for a <see cref="T:System.Type" /> used by the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
/// </summary>
public class JsonStringContract : JsonPrimitiveContract
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Serialization.JsonStringContract" /> class.
	/// </summary>
	/// <param name="underlyingType">The underlying type for the contract.</param>
	[RequiresUnreferencedCode("Newtonsoft.Json relies on reflection over types that may be removed when trimming.")]
	public JsonStringContract(Type underlyingType)
		: base(underlyingType)
	{
		ContractType = JsonContractType.String;
	}
}
