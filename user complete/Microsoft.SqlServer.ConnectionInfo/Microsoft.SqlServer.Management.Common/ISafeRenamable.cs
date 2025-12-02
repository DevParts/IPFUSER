namespace Microsoft.SqlServer.Management.Common;

public interface ISafeRenamable : IRenamable
{
	bool WarnOnRename { get; }
}
