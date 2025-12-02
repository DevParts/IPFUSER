namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class Group : AstNode
{
	private AstNode _groupNode;

	internal override QueryType TypeOfAst => QueryType.Group;

	internal override RType ReturnType => RType.NodeSet;

	internal AstNode GroupNode => _groupNode;

	internal override double DefaultPriority => 0.0;

	internal Group(AstNode groupNode)
	{
		_groupNode = groupNode;
	}
}
