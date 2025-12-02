namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class Filter : AstNode
{
	private AstNode _input;

	private AstNode _condition;

	internal override QueryType TypeOfAst => QueryType.Filter;

	internal override RType ReturnType => RType.NodeSet;

	internal AstNode Input => _input;

	internal AstNode Condition => _condition;

	internal Filter(AstNode input, AstNode condition)
	{
		_input = input;
		_condition = condition;
	}
}
