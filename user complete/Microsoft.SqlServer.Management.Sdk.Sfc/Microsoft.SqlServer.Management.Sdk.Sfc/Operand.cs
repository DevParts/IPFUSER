namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class Operand : AstNode
{
	private object _var;

	private string _prefix = string.Empty;

	private RType _type;

	internal override QueryType TypeOfAst => QueryType.ConstantOperand;

	internal override RType ReturnType => _type;

	internal object OperandValue => _var;

	internal Operand(string var)
	{
		_var = var;
		_type = RType.String;
	}

	internal Operand(double var)
	{
		_var = var;
		_type = RType.Number;
	}

	internal Operand(bool var)
	{
		_var = var;
		_type = RType.Boolean;
	}

	internal Operand(string var, string prefix)
	{
		_var = var;
		_prefix = prefix;
		_type = RType.Variable;
	}
}
