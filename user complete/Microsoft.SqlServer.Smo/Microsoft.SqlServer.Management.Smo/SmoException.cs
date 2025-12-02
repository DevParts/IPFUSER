using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public class SmoException : SqlServerManagementException
{
	private static readonly SmoExceptionSingleton smoExceptionSingleton;

	protected static string ProdVer => smoExceptionSingleton.prodVer;

	public virtual SmoExceptionType SmoExceptionType => SmoExceptionType.SmoException;

	public override string HelpLink
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			stringBuilder.Append(Data["HelpLink.BaseHelpUrl"] as string);
			stringBuilder.Append("?");
			stringBuilder.AppendFormat("ProdName={0}", Data["HelpLink.ProdName"] as string);
			if (Data.Contains("HelpLink.ProdVer"))
			{
				stringBuilder.AppendFormat("&ProdVer={0}", Data["HelpLink.ProdVer"] as string);
			}
			if (Data.Contains("HelpLink.EvtSrc"))
			{
				stringBuilder.AppendFormat("&EvtSrc={0}", Data["HelpLink.EvtSrc"] as string);
			}
			if (Data.Contains("HelpLink.EvtData1"))
			{
				stringBuilder.AppendFormat("&EvtID={0}", Data["HelpLink.EvtData1"] as string);
				for (int i = 2; i < 10 && Data.Contains("HelpLink.EvtData" + i); i++)
				{
					stringBuilder.Append("+");
					stringBuilder.Append(Data["HelpLink.EvtData" + i] as string);
				}
			}
			stringBuilder.AppendFormat("&LinkId={0}", Data["HelpLink.LinkId"] as string);
			return stringBuilder.ToString().Replace(' ', '+');
		}
	}

	public SmoException()
	{
		Init();
	}

	public SmoException(string message)
		: base(message)
	{
		Init();
	}

	public SmoException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	protected SmoException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	private void Init()
	{
		Data.Add("HelpLink.ProdVer", ProdVer);
	}

	static SmoException()
	{
		smoExceptionSingleton = new SmoExceptionSingleton();
		smoExceptionSingleton.prodVer = string.Empty;
		object[] customAttributes = typeof(SmoException).GetAssembly().GetCustomAttributes(inherit: true);
		if (customAttributes == null)
		{
			return;
		}
		object[] array = customAttributes;
		foreach (object obj in array)
		{
			if (obj is AssemblyFileVersionAttribute)
			{
				smoExceptionSingleton.prodVer = ((AssemblyFileVersionAttribute)obj).Version;
				break;
			}
		}
	}

	protected internal SmoException SetHelpContext(string resource)
	{
		Data["HelpLink.EvtSrc"] = "Microsoft.SqlServer.Management.Smo.ExceptionTemplates." + resource;
		return this;
	}
}
