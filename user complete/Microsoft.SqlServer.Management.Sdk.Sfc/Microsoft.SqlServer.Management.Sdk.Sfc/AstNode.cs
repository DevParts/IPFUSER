namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class AstNode
{
	internal enum QueryType
	{
		Axis,
		Operator,
		Filter,
		ConstantOperand,
		Function,
		Group,
		Root,
		Error
	}

	internal enum RType
	{
		Number,
		String,
		Boolean,
		NodeSet,
		Variable,
		Any,
		Error
	}

	internal virtual QueryType TypeOfAst => QueryType.Error;

	internal virtual RType ReturnType => RType.Error;

	internal virtual double DefaultPriority => 0.5;
}
