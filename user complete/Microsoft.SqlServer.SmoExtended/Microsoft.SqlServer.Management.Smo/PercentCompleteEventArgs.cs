using System;
using System.Data.SqlClient;
using System.Text;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class PercentCompleteEventArgs : ServerMessageEventArgs
{
	private int percent;

	private string message = string.Empty;

	public int Percent => percent;

	public string Message => message;

	internal PercentCompleteEventArgs(SqlError error)
		: base(error)
	{
		message = error.Message;
		int num = message.LastIndexOf(']');
		StringBuilder stringBuilder = new StringBuilder();
		int num2 = num + 1;
		while (num2 < message.Length)
		{
			if (!char.IsNumber(message[num2++]))
			{
				stringBuilder.Append(message, num + 1, num2 - num - 1);
				break;
			}
		}
		percent = Convert.ToInt32(stringBuilder.ToString(), SmoApplication.DefaultCulture);
	}

	internal PercentCompleteEventArgs(SqlError error, string message)
		: this(error)
	{
		this.message = message;
	}
}
