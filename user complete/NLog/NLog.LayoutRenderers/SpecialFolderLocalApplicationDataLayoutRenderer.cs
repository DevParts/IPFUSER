using System;

namespace NLog.LayoutRenderers;

/// <summary>
/// System special folder path from <see cref="F:System.Environment.SpecialFolder.LocalApplicationData" />
/// </summary>
[LayoutRenderer("userLocalApplicationDataDir")]
public class SpecialFolderLocalApplicationDataLayoutRenderer : SpecialFolderLayoutRenderer
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.SpecialFolderLocalApplicationDataLayoutRenderer" /> class.
	/// </summary>
	public SpecialFolderLocalApplicationDataLayoutRenderer()
	{
		base.Folder = Environment.SpecialFolder.LocalApplicationData;
	}
}
