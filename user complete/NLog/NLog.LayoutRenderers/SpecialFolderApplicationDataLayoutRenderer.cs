using System;

namespace NLog.LayoutRenderers;

/// <summary>
/// System special folder path from <see cref="F:System.Environment.SpecialFolder.ApplicationData" />
/// </summary>
[LayoutRenderer("userApplicationDataDir")]
public class SpecialFolderApplicationDataLayoutRenderer : SpecialFolderLayoutRenderer
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.SpecialFolderApplicationDataLayoutRenderer" /> class.
	/// </summary>
	public SpecialFolderApplicationDataLayoutRenderer()
	{
		base.Folder = Environment.SpecialFolder.ApplicationData;
	}
}
