namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public abstract class SfcTypeMetadata
{
	public abstract bool IsCrudActionHandledByParent(SfcDependencyAction dependencyAction);
}
