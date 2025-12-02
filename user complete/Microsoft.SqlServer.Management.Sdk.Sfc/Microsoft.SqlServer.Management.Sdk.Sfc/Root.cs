namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class Root : AstNode
{
	internal override QueryType TypeOfAst => QueryType.Root;

	internal override RType ReturnType => RType.NodeSet;

	internal Root()
	{
	}
}
