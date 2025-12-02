namespace Microsoft.SqlServer.Management.Sdk.Differencing;

public interface IDifferencer
{
	IDiffgram CompareGraphs(object source, object target);

	bool IsTypeEmitted(DiffType type);

	void SetTypeEmitted(DiffType type);

	void UnsetTypeEmitted(DiffType type);
}
