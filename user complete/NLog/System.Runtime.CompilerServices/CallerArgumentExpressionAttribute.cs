namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class CallerArgumentExpressionAttribute : Attribute
{
	public string Param { get; }

	public CallerArgumentExpressionAttribute(string param)
	{
		Param = param;
	}
}
