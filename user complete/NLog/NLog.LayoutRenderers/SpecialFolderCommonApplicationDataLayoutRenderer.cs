using System;

namespace NLog.LayoutRenderers;

/// <summary>
/// System special folder path from <see cref="F:System.Environment.SpecialFolder.CommonApplicationData" />
/// </summary>
[LayoutRenderer("commonApplicationDataDir")]
public class SpecialFolderCommonApplicationDataLayoutRenderer : SpecialFolderLayoutRenderer
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.SpecialFolderCommonApplicationDataLayoutRenderer" /> class.
	/// </summary>
	public SpecialFolderCommonApplicationDataLayoutRenderer()
	{
		base.Folder = Environment.SpecialFolder.CommonApplicationData;
	}
}
