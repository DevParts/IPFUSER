using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.Vision;

public class CognexInsight : MacsaDevice
{
	public enum TELNET_COMMANDS
	{
		NONE,
		IDENTIFICATION,
		GET_ONLINE,
		GET_VALUE,
		GET_FILE,
		LOAD_FILE,
		SET_ONLINE,
		SET_STRING
	}

	public enum SOCKET_TYPE
	{
		NONE,
		TELNET,
		DATA
	}

	public enum COMMAND_STATE
	{
		WORKING,
		FAILED,
		COMPLETED
	}

	public delegate void OnDataAvailableEventHandler(object sender, string Data);

	public delegate void OnTelnetOnLineEventHandler(object Sender);

	public delegate void OnTelnetCommandAcceptedEventHandler(object Sender, string Data);

	public delegate void OnTelnetCommandFailureEventHandler(object sender, string Data);

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private SOCKET_TYPE _SocketType;

	private TELNET_COMMANDS _TelnetCommand;

	private string _BufferIn;

	private string _Parameter;

	[method: DebuggerNonUserCode]
	public event OnDataAvailableEventHandler OnDataAvailable;

	[method: DebuggerNonUserCode]
	public event OnTelnetOnLineEventHandler OnTelnetOnLine;

	[method: DebuggerNonUserCode]
	public event OnTelnetCommandAcceptedEventHandler OnTelnetCommandAccepted;

	[method: DebuggerNonUserCode]
	public event OnTelnetCommandFailureEventHandler OnTelnetCommandFailure;

	[method: DebuggerNonUserCode]
	public event OnInformationEventHandler OnInformation;

	[DebuggerNonUserCode]
	private static void __ENCAddToList(object value)
	{
		checked
		{
			lock (__ENCList)
			{
				if (__ENCList.Count == __ENCList.Capacity)
				{
					int num = 0;
					int num2 = __ENCList.Count - 1;
					int num3 = 0;
					while (true)
					{
						int num4 = num3;
						int num5 = num2;
						if (num4 > num5)
						{
							break;
						}
						WeakReference weakReference = __ENCList[num3];
						if (weakReference.IsAlive)
						{
							if (num3 != num)
							{
								__ENCList[num] = __ENCList[num3];
							}
							num++;
						}
						num3++;
					}
					__ENCList.RemoveRange(num, __ENCList.Count - num);
					__ENCList.Capacity = __ENCList.Count;
				}
				__ENCList.Add(new WeakReference(RuntimeHelpers.GetObjectValue(value)));
			}
		}
	}

	public CognexInsight()
	{
		__ENCAddToList(this);
		_BufferIn = "";
		_TelnetCommand = TELNET_COMMANDS.NONE;
		_SocketType = SOCKET_TYPE.NONE;
	}

	public bool Init(int iPort, string sIp, SOCKET_TYPE SocketType)
	{
		_SocketType = SocketType;
		if (_SocketType == SOCKET_TYPE.TELNET)
		{
			_TelnetCommand = TELNET_COMMANDS.IDENTIFICATION;
		}
		else
		{
			_TelnetCommand = TELNET_COMMANDS.NONE;
		}
		return Init(iPort, sIp);
	}

	public bool SendTelnetCommand(TELNET_COMMANDS Command, string Parameters = null)
	{
		long lWaitTime = 2000L;
		_TelnetCommand = Command;
		_BufferIn = "";
		string text = default(string);
		switch ((int)Command)
		{
		case 4:
			text = "GF";
			break;
		case 2:
			text = "GO";
			break;
		case 3:
			text = "GV";
			break;
		case 5:
			lWaitTime = 90000L;
			text = "LF";
			break;
		case 6:
			text = "SO";
			break;
		case 7:
			text = "SS";
			break;
		}
		if (Parameters != null)
		{
			text += Parameters;
		}
		text += "\r\n";
		SendData(text, lWaitTime, WAIT_TYPE.Thread);
		bool result = default(bool);
		return result;
	}

	public bool SetOnLine(bool Mode)
	{
		SendTelnetCommand(TELNET_COMMANDS.SET_ONLINE, Conversions.ToString(Interaction.IIf(Mode, "1", "0")));
		bool result = default(bool);
		return result;
	}

	protected override void TcpComm_DataReceived(string sData)
	{
		_BufferIn += sData;
		if (_SocketType == SOCKET_TYPE.DATA)
		{
			int num = Strings.InStr(_BufferIn, "\r");
			if (num > 0)
			{
				string data = Strings.Mid(_BufferIn, 1, checked(Strings.InStr(_BufferIn, "\r") - 1));
				_BufferIn = "";
				OnDataAvailable?.Invoke(this, data);
			}
			return;
		}
		switch ((int)_TelnetCommand)
		{
		case 1:
			_BufferIn += sData;
			if (Strings.InStr(_BufferIn, "User:") > 0)
			{
				_BufferIn = "";
				AnswerRecv = true;
				SendData("admin\r\n", 0L);
			}
			if (Strings.InStr(_BufferIn, "Password:") > 0)
			{
				_BufferIn = "";
				AnswerRecv = true;
				SendData("\r\n", 0L);
			}
			if (Strings.InStr(_BufferIn, "User Logged In") > 0)
			{
				_BufferIn = "";
				_TelnetCommand = TELNET_COMMANDS.NONE;
				AnswerRecv = true;
				OnTelnetOnLine?.Invoke(this);
			}
			break;
		case 3:
		case 4:
		{
			int num = 1;
			if (num > 0)
			{
				switch ((int)GetParameter(Strings.Mid(_BufferIn, num, checked(_BufferIn.Length - num + 1)), 2))
				{
				case 2:
					_TelnetCommand = TELNET_COMMANDS.NONE;
					AnswerRecv = true;
					OnTelnetCommandAccepted?.Invoke(this, _Parameter);
					break;
				case 1:
					_TelnetCommand = TELNET_COMMANDS.NONE;
					AnswerRecv = true;
					OnTelnetCommandFailure?.Invoke(this, _Parameter);
					break;
				}
			}
			break;
		}
		case 2:
		{
			int num = 1;
			if (num > 0)
			{
				switch ((int)GetParameter(Strings.Mid(_BufferIn, num, checked(_BufferIn.Length - num + 1)), 1))
				{
				case 2:
					_TelnetCommand = TELNET_COMMANDS.NONE;
					AnswerRecv = true;
					OnTelnetCommandAccepted?.Invoke(this, _Parameter);
					break;
				case 1:
					_TelnetCommand = TELNET_COMMANDS.NONE;
					AnswerRecv = true;
					OnTelnetCommandAccepted?.Invoke(this, _Parameter);
					break;
				}
			}
			break;
		}
		case 5:
		case 6:
		case 7:
		{
			int num = 1;
			if (num > 0)
			{
				switch ((int)GetParameter(Strings.Mid(_BufferIn, num, checked(_BufferIn.Length - num + 1)), 1))
				{
				case 2:
					_TelnetCommand = TELNET_COMMANDS.NONE;
					AnswerRecv = true;
					OnTelnetCommandAccepted?.Invoke(this, _Parameter);
					break;
				case 1:
					_TelnetCommand = TELNET_COMMANDS.NONE;
					AnswerRecv = true;
					OnTelnetCommandFailure?.Invoke(this, _Parameter);
					break;
				}
			}
			break;
		}
		}
	}

	public COMMAND_STATE GetParameter(string Cadena, int CrNumber)
	{
		string[] array = Strings.Split(Cadena, "\r\n");
		checked
		{
			if (array.Length == CrNumber + 1)
			{
				_Parameter = array[CrNumber - 1];
				if (Operators.CompareString(array[0], "1", TextCompare: false) == 0)
				{
					return COMMAND_STATE.COMPLETED;
				}
				return COMMAND_STATE.WORKING;
			}
			return COMMAND_STATE.FAILED;
		}
	}
}
