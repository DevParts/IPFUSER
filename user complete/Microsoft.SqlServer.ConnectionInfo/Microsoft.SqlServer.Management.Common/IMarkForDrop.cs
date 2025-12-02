namespace Microsoft.SqlServer.Management.Common;

public interface IMarkForDrop
{
	void MarkForDrop(bool dropOnAlter);
}
