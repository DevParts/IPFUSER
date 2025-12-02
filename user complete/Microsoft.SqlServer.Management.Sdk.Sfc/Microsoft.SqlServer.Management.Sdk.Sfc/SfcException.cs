using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public class SfcException : SqlServerManagementException
{
	private static string prodVer;

	protected static string ProdVer => prodVer;

	public override string HelpLink
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
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

	protected SfcException()
	{
		Init();
	}

	protected SfcException(string message)
		: base(message)
	{
		Init();
	}

	protected SfcException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	protected SfcException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	private void Init()
	{
		Data.Add("HelpLink.ProdVer", ProdVer);
	}

	static SfcException()
	{
		prodVer = string.Empty;
		object[] customAttributes = SmoManagementUtil.GetExecutingAssembly().GetCustomAttributes(inherit: true);
		if (customAttributes == null)
		{
			return;
		}
		object[] array = customAttributes;
		foreach (object obj in array)
		{
			if (obj is AssemblyFileVersionAttribute)
			{
				prodVer = ((AssemblyFileVersionAttribute)obj).Version;
				break;
			}
		}
	}

	protected internal SfcException SetHelpContext(string resource)
	{
		Data["HelpLink.EvtSrc"] = "Microsoft.SqlServer.Management.Sdk.Sfc.ExceptionTemplates." + resource;
		return this;
	}
}
