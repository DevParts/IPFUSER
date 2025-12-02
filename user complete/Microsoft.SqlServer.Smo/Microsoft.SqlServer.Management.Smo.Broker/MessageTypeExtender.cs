using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Broker;

[CLSCompliant(false)]
public class MessageTypeExtender : SmoObjectExtender<MessageType>, ISfcValidate
{
	[ExtendedProperty]
	public string Name
	{
		get
		{
			return base.Parent.Name;
		}
		set
		{
			base.Parent.Name = value;
		}
	}

	public MessageTypeExtender()
	{
	}

	public MessageTypeExtender(MessageType messageType)
		: base(messageType)
	{
	}

	public ValidationState Validate(string methodName, params object[] arguments)
	{
		return base.Parent.Validate(methodName, arguments);
	}
}
