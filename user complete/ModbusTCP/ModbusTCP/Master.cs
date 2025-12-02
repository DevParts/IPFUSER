using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ModbusTCP;

/// <summary>
/// Modbus TCP common driver class. This class implements a modbus TCP master driver.
/// It supports the following commands:
///
/// Read coils
/// Read discrete inputs
/// Write single coil
/// Write multiple cooils
/// Read holding register
/// Read input register
/// Write single register
/// Write multiple register
///
/// All commands can be sent in synchronous or asynchronous mode. If a value is accessed
/// in synchronous mode the program will stop and wait for slave to response. If the 
/// slave didn't answer within a specified time a timeout exception is called.
/// The class uses multi threading for both synchronous and asynchronous access. For
/// the communication two lines are created. This is necessary because the synchronous
/// thread has to wait for a previous command to finish.
///
/// </summary>
public class Master
{
	/// <summary>Response data event. This event is called when new data arrives</summary>
	public delegate void ResponseData(int id, byte function, byte[] data);

	/// <summary>Exception data event. This event is called when the data is incorrect</summary>
	public delegate void ExceptionData(int id, byte function, byte exception);

	private class ListenClass
	{
		internal delegate void ResponseData(int id, byte function, byte[] data);

		internal delegate void ExceptionData(int id, byte function, byte exception);

		private TcpClient tcpClient;

		private bool sync = false;

		private int req_id = 0;

		public byte[] resp_data = new byte[0];

		internal event ResponseData OnResponseData;

		internal event ExceptionData OnException;

		public bool WriteData(TcpClient _tcpClient, byte[] write_data, int _req_id, bool _sync)
		{
			tcpClient = _tcpClient;
			req_id = _req_id;
			sync = _sync;
			try
			{
				if (_connected)
				{
					tcpClient.GetStream().Write(write_data, 0, write_data.Length);
					Thread thrListen = new Thread(ListenThread);
					thrListen.Start();
					if (sync)
					{
						thrListen.Join();
					}
					return true;
				}
				if (this.OnException != null)
				{
					this.OnException(req_id, 0, 253);
				}
			}
			catch (Exception ex)
			{
				if (!(ex.InnerException.GetType() == typeof(SocketException)))
				{
					throw ex;
				}
				if (this.OnException != null)
				{
					this.OnException(req_id, 0, 254);
				}
			}
			return false;
		}

		public void ListenThread()
		{
			int time = 0;
			byte[] buffer = new byte[256];
			int id = 0;
			try
			{
				while (time < _timeout && tcpClient.Client.Connected)
				{
					if (tcpClient.GetStream().CanRead && tcpClient.GetStream().DataAvailable)
					{
						tcpClient.GetStream().Read(buffer, 0, buffer.GetUpperBound(0));
						id = BitConverter.ToInt16(buffer, 0);
						byte function = buffer[7];
						byte[] data;
						if (function >= 5)
						{
							data = new byte[2];
							Array.Copy(buffer, 10, data, 0, 2);
						}
						else
						{
							data = new byte[buffer[8]];
							Array.Copy(buffer, 9, data, 0, buffer[8]);
						}
						if (function > 128)
						{
							function -= 128;
							if (this.OnException != null)
							{
								this.OnException(id, function, buffer[8]);
							}
						}
						else if (this.OnResponseData != null && !sync)
						{
							this.OnResponseData(id, function, data);
						}
						else
						{
							resp_data = data;
						}
						break;
					}
					time += 10;
					Thread.Sleep(10);
				}
			}
			catch (Exception)
			{
				_connected = false;
			}
			if (time >= _timeout && this.OnException != null)
			{
				this.OnException(req_id, 0, byte.MaxValue);
			}
		}
	}

	private const byte fctReadCoil = 1;

	private const byte fctReadDiscreteInputs = 2;

	private const byte fctReadHoldingRegister = 3;

	private const byte fctReadInputRegister = 4;

	private const byte fctWriteSingleCoil = 5;

	private const byte fctWriteSingleRegister = 6;

	private const byte fctWriteMultipleCoils = 15;

	private const byte fctWriteMultipleRegister = 16;

	/// <summary>Constant for exception illegal function.</summary>
	public const byte excIllegalFunction = 1;

	/// <summary>Constant for exception illegal data address.</summary>
	public const byte excIllegalDataAdr = 2;

	/// <summary>Constant for exception illegal data value.</summary>
	public const byte excIllegalDataVal = 3;

	/// <summary>Constant for exception slave device failure.</summary>
	public const byte excSlaveDeviceFailure = 4;

	/// <summary>Constant for exception acknoledge.</summary>
	public const byte excAck = 5;

	/// <summary>Constant for exception memory parity error.</summary>
	public const byte excMemParityErr = 6;

	/// <summary>Constant for exception gate path unavailable.</summary>
	public const byte excGatePathUnavailable = 10;

	/// <summary>Constant for exception not connected.</summary>
	public const byte excExceptionNotConnected = 253;

	/// <summary>Constant for exception connection lost.</summary>
	public const byte excExceptionConnectionLost = 254;

	/// <summary>Constant for exception response timeout.</summary>
	public const byte excExceptionTimeout = byte.MaxValue;

	private const byte fctExceptionOffset = 128;

	private static int _timeout = 500;

	private static bool _connected = false;

	private TcpClient tcpAsyClient;

	private TcpClient tcpSynClient;

	private ListenClass thrAsyListen;

	private ListenClass thrSynListen;

	/// <summary>Response timeout. If the slave didn't answers within in this time an exception is called.</summary>
	/// <value>The default value is 500ms.</value>
	public int timeout
	{
		get
		{
			return _timeout;
		}
		set
		{
			_timeout = value;
		}
	}

	/// <summary>Shows if a connection is active.</summary>
	public bool connected => _connected;

	/// <summary>Response data event. This event is called when new data arrives</summary>
	public event ResponseData OnResponseData;

	/// <summary>Exception data event. This event is called when the data is incorrect</summary>
	public event ExceptionData OnException;

	/// <summary>Create master instance.</summary>
	public Master()
	{
	}

	/// <summary>Create master instance.</summary>
	/// <param name="ip">IP adress of modbus slave.</param>
	/// <param name="port">Port number of modbus slave. Usually port 502 is used.</param>
	public Master(string ip, int port)
	{
		connect(ip, port);
	}

	/// <summary>Start connection to slave.</summary>
	/// <param name="ip">IP adress of modbus slave.</param>
	/// <param name="port">Port number of modbus slave. Usually port 502 is used.</param>
	public void connect(string ip, int port)
	{
		try
		{
			tcpAsyClient = new TcpClient(ip, port);
			tcpAsyClient.ReceiveBufferSize = 256;
			tcpAsyClient.SendBufferSize = 256;
			_connected = true;
		}
		catch (IOException ex)
		{
			_connected = false;
			throw ex;
		}
	}

	/// <summary>Stop connection to slave.</summary>
	public void disconnect()
	{
		Dispose();
	}

	/// <summary>Destroy master instance.</summary>
	~Master()
	{
		Dispose();
	}

	/// <summary>Destroy master instance</summary>
	public void Dispose()
	{
		if (tcpAsyClient != null)
		{
			tcpAsyClient.Close();
			tcpAsyClient = null;
		}
		if (tcpSynClient != null)
		{
			tcpSynClient.Close();
			tcpSynClient = null;
		}
		if (thrAsyListen != null)
		{
			thrAsyListen = null;
		}
		if (thrSynListen != null)
		{
			thrSynListen = null;
		}
	}

	/// <summary>Read coils from slave asynchronous. The result is given in the response function.</summary>
	/// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
	/// <param name="startAddress">Address from where the data read begins.</param>
	/// <param name="numInputs">Length of data.</param>
	public void ReadCoils(int id, int startAddress, byte numInputs)
	{
		WriteAsyncData(CreateReadHeader(id, startAddress, numInputs, 1), id);
	}

	/// <summary>Read coils from slave synchronous.</summary>
	/// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
	/// <param name="startAddress">Address from where the data read begins.</param>
	/// <param name="numInputs">Length of data.</param>
	/// <param name="values">Contains the result of function.</param>
	public void ReadCoils(int id, int startAddress, byte numInputs, ref byte[] values)
	{
		values = WriteSyncData(CreateReadHeader(id, startAddress, numInputs, 1), id);
	}

	/// <summary>Read discrete inputs from slave asynchronous. The result is given in the response function.</summary>
	/// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
	/// <param name="startAddress">Address from where the data read begins.</param>
	/// <param name="numInputs">Length of data.</param>
	public void ReadDiscreteInputs(int id, int startAddress, byte numInputs)
	{
		WriteAsyncData(CreateReadHeader(id, startAddress, numInputs, 2), id);
	}

	/// <summary>Read discrete inputs from slave synchronous.</summary>
	/// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
	/// <param name="startAddress">Address from where the data read begins.</param>
	/// <param name="numInputs">Length of data.</param>
	/// <param name="values">Contains the result of function.</param>
	public void ReadDiscreteInputs(int id, int startAddress, byte numInputs, ref byte[] values)
	{
		values = WriteSyncData(CreateReadHeader(id, startAddress, numInputs, 2), id);
	}

	/// <summary>Read holding registers from slave asynchronous. The result is given in the response function.</summary>
	/// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
	/// <param name="startAddress">Address from where the data read begins.</param>
	/// <param name="numInputs">Length of data.</param>
	public void ReadHoldingRegister(int id, int startAddress, byte numInputs)
	{
		WriteAsyncData(CreateReadHeader(id, startAddress, numInputs, 3), id);
	}

	/// <summary>Read holding registers from slave synchronous.</summary>
	/// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
	/// <param name="startAddress">Address from where the data read begins.</param>
	/// <param name="numInputs">Length of data.</param>
	/// <param name="values">Contains the result of function.</param>
	public void ReadHoldingRegister(int id, int startAddress, byte numInputs, ref byte[] values)
	{
		values = WriteSyncData(CreateReadHeader(id, startAddress, numInputs, 3), id);
	}

	/// <summary>Read input registers from slave asynchronous. The result is given in the response function.</summary>
	/// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
	/// <param name="startAddress">Address from where the data read begins.</param>
	/// <param name="numInputs">Length of data.</param>
	public void ReadInputRegister(int id, int startAddress, byte numInputs)
	{
		WriteAsyncData(CreateReadHeader(id, startAddress, numInputs, 4), id);
	}

	/// <summary>Read input registers from slave synchronous.</summary>
	/// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
	/// <param name="startAddress">Address from where the data read begins.</param>
	/// <param name="numInputs">Length of data.</param>
	/// <param name="values">Contains the result of function.</param>
	public void ReadInputRegister(int id, int startAddress, byte numInputs, ref byte[] values)
	{
		values = WriteSyncData(CreateReadHeader(id, startAddress, numInputs, 4), id);
	}

	/// <summary>Write single coil in slave asynchronous. The result is given in the response function.</summary>
	/// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
	/// <param name="startAddress">Address from where the data read begins.</param>
	/// <param name="OnOff">Specifys if the coil should be switched on or off.</param>
	public void WriteSingleCoils(int id, int startAddress, bool OnOff)
	{
		byte[] data = CreateWriteHeader(id, startAddress, 1, 1, 5);
		if (OnOff)
		{
			data[10] = byte.MaxValue;
		}
		else
		{
			data[10] = 0;
		}
		WriteAsyncData(data, id);
	}

	/// <summary>Write single coil in slave synchronous.</summary>
	/// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
	/// <param name="startAddress">Address from where the data read begins.</param>
	/// <param name="OnOff">Specifys if the coil should be switched on or off.</param>
	/// <param name="result">Contains the result of the synchronous write.</param>
	public void WriteSingleCoils(int id, int startAddress, bool OnOff, ref byte[] result)
	{
		byte[] data = CreateWriteHeader(id, startAddress, 1, 1, 5);
		if (OnOff)
		{
			data[10] = byte.MaxValue;
		}
		else
		{
			data[10] = 0;
		}
		result = WriteSyncData(data, id);
	}

	/// <summary>Write multiple coils in slave asynchronous. The result is given in the response function.</summary>
	/// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
	/// <param name="startAddress">Address from where the data read begins.</param>
	/// <param name="numBits">Specifys number of bits.</param>
	/// <param name="values">Contains the bit information in byte format.</param>
	public void WriteMultipleCoils(int id, int startAddress, int numBits, byte[] values)
	{
		byte numBytes = Convert.ToByte(values.Length);
		byte[] data = CreateWriteHeader(id, startAddress, numBits, (byte)(numBytes + 2), 15);
		Array.Copy(values, 0, data, 13, numBytes);
		WriteAsyncData(data, id);
	}

	/// <summary>Write multiple coils in slave synchronous.</summary>
	/// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
	/// <param name="startAddress">Address from where the data read begins.</param>
	/// <param name="numBits">Specifys number of bits.</param>
	/// <param name="values">Contains the bit information in byte format.</param>
	/// <param name="result">Contains the result of the synchronous write.</param>
	public void WriteMultipleCoils(int id, int startAddress, int numBits, byte[] values, byte[] result)
	{
		byte numBytes = Convert.ToByte(values.Length);
		byte[] data = CreateWriteHeader(id, startAddress, numBits, (byte)(numBytes + 2), 15);
		Array.Copy(values, 0, data, 13, numBytes);
		result = WriteSyncData(data, id);
	}

	/// <summary>Write single register in slave asynchronous. The result is given in the response function.</summary>
	/// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
	/// <param name="startAddress">Address from where the data read begins.</param>
	/// <param name="values">Contains the register information.</param>
	public void WriteSingleRegister(int id, int startAddress, byte[] values)
	{
		byte[] data = CreateWriteHeader(id, startAddress, 1, 1, 6);
		data[10] = values[0];
		data[11] = values[1];
		WriteAsyncData(data, id);
	}

	/// <summary>Write single register in slave synchronous.</summary>
	/// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
	/// <param name="startAddress">Address from where the data read begins.</param>
	/// <param name="values">Contains the register information.</param>
	/// <param name="result">Contains the result of the synchronous write.</param>
	public void WriteSingleRegister(int id, int startAddress, byte[] values, byte[] result)
	{
		byte[] data = CreateWriteHeader(id, startAddress, 1, 1, 6);
		data[10] = values[0];
		data[11] = values[1];
		result = WriteSyncData(data, id);
	}

	/// <summary>Write multiple registers in slave asynchronous. The result is given in the response function.</summary>
	/// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
	/// <param name="startAddress">Address from where the data read begins.</param>
	/// <param name="numRegs">Number of registers to be written.</param>
	/// <param name="values">Contains the register information.</param>
	public void WriteMultipleRegister(int id, int startAddress, int numRegs, byte[] values)
	{
		byte numBytes = Convert.ToByte(values.Length);
		byte[] data = CreateWriteHeader(id, startAddress, numRegs, (byte)(numBytes + 2), 16);
		Array.Copy(values, 0, data, 13, numBytes);
		WriteAsyncData(data, id);
	}

	/// <summary>Write multiple registers in slave synchronous.</summary>
	/// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
	/// <param name="startAddress">Address from where the data read begins.</param>
	/// <param name="numRegs">Number of registers to be written.</param>
	/// <param name="values">Contains the register information.</param>
	/// <param name="result">Contains the result of the synchronous write.</param>
	public void WriteMultipleRegister(int id, int startAddress, int numRegs, byte[] values, byte[] result)
	{
		byte numBytes = Convert.ToByte(values.Length);
		byte[] data = CreateWriteHeader(id, startAddress, numRegs, (byte)(numBytes + 2), 16);
		Array.Copy(values, 0, data, 13, numBytes);
		result = WriteSyncData(data, id);
	}

	private byte[] CreateReadHeader(int id, int startAddress, byte length, byte function)
	{
		byte[] data = new byte[12];
		byte[] _id = BitConverter.GetBytes((short)id);
		data[0] = _id[0];
		data[1] = _id[1];
		data[5] = 6;
		data[6] = 0;
		data[7] = function;
		byte[] _adr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)startAddress));
		data[8] = _adr[0];
		data[9] = _adr[1];
		data[11] = length;
		return data;
	}

	private byte[] CreateWriteHeader(int id, int startAddress, int numData, byte numBytes, byte function)
	{
		byte[] data = new byte[numBytes + 11];
		byte[] _id = BitConverter.GetBytes((short)id);
		data[0] = _id[0];
		data[1] = _id[1];
		byte[] _size = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)(5 + numBytes)));
		data[4] = _size[0];
		data[5] = _size[1];
		data[6] = 0;
		data[7] = function;
		byte[] _adr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)startAddress));
		data[8] = _adr[0];
		data[9] = _adr[1];
		if (function >= 15)
		{
			byte[] cnt = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)numData));
			data[10] = cnt[0];
			data[11] = cnt[1];
			data[12] = (byte)(numBytes - 2);
		}
		return data;
	}

	private void WriteAsyncData(byte[] write_data, int id)
	{
		thrAsyListen = new ListenClass();
		thrAsyListen.OnException += thrListen_OnException;
		thrAsyListen.OnResponseData += thrListen_OnResponseData;
		thrAsyListen.WriteData(tcpAsyClient, write_data, id, _sync: false);
	}

	private byte[] WriteSyncData(byte[] write_data, int id)
	{
		int time = 0;
		while (thrSynListen != null && time < _timeout)
		{
			Thread.Sleep(10);
			time += 10;
		}
		thrSynListen = new ListenClass();
		thrSynListen.OnException += thrListen_OnException;
		if (thrSynListen.WriteData(tcpSynClient, write_data, id, _sync: true))
		{
			byte[] resp_data = thrSynListen.resp_data;
			thrSynListen = null;
			return resp_data;
		}
		thrSynListen = null;
		return null;
	}

	private void thrListen_OnException(int id, byte function, byte exception)
	{
		if (this.OnException != null)
		{
			this.OnException(id, function, exception);
		}
		if (exception == 254)
		{
			_connected = false;
			tcpAsyClient.Close();
			tcpSynClient.Close();
			tcpAsyClient = null;
		}
	}

	private void thrListen_OnResponseData(int id, byte function, byte[] data)
	{
		if (this.OnResponseData != null)
		{
			this.OnResponseData(id, function, data);
		}
	}
}
