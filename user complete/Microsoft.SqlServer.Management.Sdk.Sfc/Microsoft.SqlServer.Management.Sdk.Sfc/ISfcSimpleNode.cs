namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcSimpleNode
{
	object ObjectReference { get; }

	Urn Urn { get; }

	ISfcSimpleMap<string, ISfcSimpleList> RelatedContainers { get; }

	ISfcSimpleMap<string, ISfcSimpleNode> RelatedObjects { get; }

	ISfcSimpleMap<string, object> Properties { get; }
}
