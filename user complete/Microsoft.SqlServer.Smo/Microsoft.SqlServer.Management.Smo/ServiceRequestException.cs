using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

[Serializable]
public sealed class ServiceRequestException : SmoException
{
	private static readonly string SmoExtendedAssemblyName = "SmoExtended";

	private uint errorCode;

	private string[] ServiceErrorMessageMap = new string[25]
	{
		ExceptionTemplatesImpl.ServiceError0,
		ExceptionTemplatesImpl.ServiceError1,
		ExceptionTemplatesImpl.ServiceError2,
		ExceptionTemplatesImpl.ServiceError3,
		ExceptionTemplatesImpl.ServiceError4,
		ExceptionTemplatesImpl.ServiceError5,
		ExceptionTemplatesImpl.ServiceError6,
		ExceptionTemplatesImpl.ServiceError7,
		ExceptionTemplatesImpl.ServiceError8,
		ExceptionTemplatesImpl.ServiceError9,
		ExceptionTemplatesImpl.ServiceError10,
		ExceptionTemplatesImpl.ServiceError11,
		ExceptionTemplatesImpl.ServiceError12,
		ExceptionTemplatesImpl.ServiceError13,
		ExceptionTemplatesImpl.ServiceError14,
		ExceptionTemplatesImpl.ServiceError15,
		ExceptionTemplatesImpl.ServiceError16,
		ExceptionTemplatesImpl.ServiceError17,
		ExceptionTemplatesImpl.ServiceError18,
		ExceptionTemplatesImpl.ServiceError19,
		ExceptionTemplatesImpl.ServiceError20,
		ExceptionTemplatesImpl.ServiceError21,
		ExceptionTemplatesImpl.ServiceError22,
		ExceptionTemplatesImpl.ServiceError23,
		ExceptionTemplatesImpl.ServiceError24
	};

	public override SmoExceptionType SmoExceptionType => SmoExceptionType.ServiceRequestException;

	public int ErrorCode => SmoApplication.ConvertUInt32ToInt32(errorCode);

	public override string Message
	{
		get
		{
			if (errorCode < ServiceErrorMessageMap.Length)
			{
				return ServiceErrorMessageMap[errorCode];
			}
			if ((2147749889u <= errorCode && errorCode <= 2147749991u) || (2147753985u <= errorCode && errorCode <= 2147753986u))
			{
				return ExceptionTemplatesImpl.WMIException(errorCode.ToString("X", SmoApplication.DefaultCulture));
			}
			if (SqlContext.IsAvailable)
			{
				return ExceptionTemplatesImpl.Win32Error(ErrorCode.ToString());
			}
			string path = GetType().GetAssembly().FullName.ToLowerInvariant().Replace("smo", SmoExtendedAssemblyName);
			Assembly assembly = Assembly.LoadFile(path);
			Module module = assembly.GetModules()[0];
			Type type = module.GetType("Microsoft.SqlServer.Management.Smo.SafeNativeMethodsExtended");
			MethodInfo method = type.GetMethod("GetLastErrorMessage");
			string text = (string)method.Invoke(null, new object[1] { ErrorCode });
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return ExceptionTemplatesImpl.UnknownError;
		}
	}

	public ServiceRequestException()
	{
		Init();
	}

	public ServiceRequestException(string message)
		: base(message)
	{
		Init();
	}

	public ServiceRequestException(string message, Exception innerException)
		: base(message, innerException)
	{
		Init();
	}

	internal ServiceRequestException(uint retcode)
	{
		Init();
		errorCode = retcode;
	}

	private ServiceRequestException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		Init();
		errorCode = info.GetUInt32("errorCode");
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("errorCode", errorCode);
		base.GetObjectData(info, context);
	}

	private void Init()
	{
		SetHelpContext("ServiceRequestException");
	}
}
