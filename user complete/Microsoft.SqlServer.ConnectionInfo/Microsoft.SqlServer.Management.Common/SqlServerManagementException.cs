using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Common;

public class SqlServerManagementException : Exception
{
	public static string ProductName => "Microsoft SQL Server";

	public SqlServerManagementException()
	{
		Init();
	}

	public SqlServerManagementException(string message)
		: base(message)
	{
		Init();
	}

	public SqlServerManagementException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	protected SqlServerManagementException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	private void Init()
	{
		Data.Add("HelpLink.ProdName", ProductName);
		Data.Add("HelpLink.BaseHelpUrl", "http://go.microsoft.com/fwlink");
		Data.Add("HelpLink.LinkId", "20476");
	}
}
