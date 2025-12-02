using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace IPFUser.My.Resources;

/// <summary>
///   Clase de recurso fuertemente tipado, para buscar cadenas traducidas, etc.
/// </summary>
[StandardModule]
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
[HideModuleName]
internal sealed class Resources
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	/// <summary>
	///   Devuelve la instancia de ResourceManager almacenada en caché utilizada por esta clase.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (object.ReferenceEquals(resourceMan, null))
			{
				ResourceManager temp = new ResourceManager("IPFUser.Resources", typeof(Resources).Assembly);
				resourceMan = temp;
			}
			return resourceMan;
		}
	}

	/// <summary>
	///   Reemplaza la propiedad CurrentUICulture del subproceso actual para todas las
	///   búsquedas de recursos mediante esta clase de recurso fuertemente tipado.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	/// <summary>
	///   Busca un recurso adaptado de tipo System.Drawing.Bitmap.
	/// </summary>
	internal static Bitmap Check
	{
		get
		{
			object obj = RuntimeHelpers.GetObjectValue(ResourceManager.GetObject("Check", resourceCulture));
			return (Bitmap)obj;
		}
	}

	/// <summary>
	///   Busca un recurso adaptado de tipo System.Drawing.Bitmap.
	/// </summary>
	internal static Bitmap Delete
	{
		get
		{
			object obj = RuntimeHelpers.GetObjectValue(ResourceManager.GetObject("Delete", resourceCulture));
			return (Bitmap)obj;
		}
	}

	/// <summary>
	///   Busca un recurso adaptado de tipo System.Drawing.Bitmap.
	/// </summary>
	internal static Bitmap door_in
	{
		get
		{
			object obj = RuntimeHelpers.GetObjectValue(ResourceManager.GetObject("door_in", resourceCulture));
			return (Bitmap)obj;
		}
	}

	/// <summary>
	///   Busca un recurso adaptado de tipo System.Drawing.Bitmap.
	/// </summary>
	internal static Bitmap green
	{
		get
		{
			object obj = RuntimeHelpers.GetObjectValue(ResourceManager.GetObject("green", resourceCulture));
			return (Bitmap)obj;
		}
	}

	/// <summary>
	///   Busca un recurso adaptado de tipo System.Drawing.Bitmap.
	/// </summary>
	internal static Bitmap Heart16
	{
		get
		{
			object obj = RuntimeHelpers.GetObjectValue(ResourceManager.GetObject("Heart16", resourceCulture));
			return (Bitmap)obj;
		}
	}

	/// <summary>
	///   Busca un recurso adaptado de tipo System.Drawing.Bitmap.
	/// </summary>
	internal static Bitmap logo_macsa_2011
	{
		get
		{
			object obj = RuntimeHelpers.GetObjectValue(ResourceManager.GetObject("logo_macsa_2011", resourceCulture));
			return (Bitmap)obj;
		}
	}

	/// <summary>
	///   Busca un recurso adaptado de tipo System.Drawing.Bitmap.
	/// </summary>
	internal static Bitmap logo_sistemas
	{
		get
		{
			object obj = RuntimeHelpers.GetObjectValue(ResourceManager.GetObject("logo_sistemas", resourceCulture));
			return (Bitmap)obj;
		}
	}

	/// <summary>
	///   Busca un recurso adaptado de tipo System.Drawing.Bitmap.
	/// </summary>
	internal static Bitmap omorn
	{
		get
		{
			object obj = RuntimeHelpers.GetObjectValue(ResourceManager.GetObject("omorn", resourceCulture));
			return (Bitmap)obj;
		}
	}

	/// <summary>
	///   Busca un recurso adaptado de tipo System.Drawing.Bitmap.
	/// </summary>
	internal static Bitmap orange
	{
		get
		{
			object obj = RuntimeHelpers.GetObjectValue(ResourceManager.GetObject("orange", resourceCulture));
			return (Bitmap)obj;
		}
	}

	/// <summary>
	///   Busca un recurso adaptado de tipo System.Drawing.Bitmap.
	/// </summary>
	internal static Bitmap panasonic
	{
		get
		{
			object obj = RuntimeHelpers.GetObjectValue(ResourceManager.GetObject("panasonic", resourceCulture));
			return (Bitmap)obj;
		}
	}

	/// <summary>
	///   Busca un recurso adaptado de tipo System.Drawing.Bitmap.
	/// </summary>
	internal static Bitmap red
	{
		get
		{
			object obj = RuntimeHelpers.GetObjectValue(ResourceManager.GetObject("red", resourceCulture));
			return (Bitmap)obj;
		}
	}

	/// <summary>
	///   Busca un recurso adaptado de tipo System.Drawing.Bitmap.
	/// </summary>
	internal static Bitmap Warning
	{
		get
		{
			object obj = RuntimeHelpers.GetObjectValue(ResourceManager.GetObject("Warning", resourceCulture));
			return (Bitmap)obj;
		}
	}
}
