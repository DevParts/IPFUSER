namespace Microsoft.SqlServer.Management.Smo;

internal class DataPreferences
{
	public bool ChangeTracking { get; set; }

	public bool OptimizerData { get; set; }

	internal DataPreferences()
	{
	}

	internal object Clone()
	{
		return MemberwiseClone();
	}
}
