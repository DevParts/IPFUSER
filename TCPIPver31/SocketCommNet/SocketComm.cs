using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


namespace SocketCommNet
{

    public class SocketComm
    {
        //the structure we need to call the C++ DLL     
        [StructLayout(LayoutKind.Sequential)]
        private struct PStatus
        {
            public UInt32 d_counter;//actual counter of ok-prints
            public UInt32 s_counter;//actual counter of nok-prints
            public UInt32 n_messageport;//actual external message selection
            public Byte Start;//0: system is printing 1: system is stopped
            public Byte request;//(internal variable)
            public Byte option;//(internal variable)
            public Byte res;//reserved (0x0:default, 0x1: UMT enabled, 0x4: Batchjob enabled)
            public UInt32 t_counter;//total counter of prints
            public UInt32 m_copies;//actual nr of copies to be printed
            public UInt32 err;//alarmcode (upper WORD: codifies lastactive alarm; lower WORD: 0: noalarm, else: alarm active)
            public UInt32 time;//reserved D-Word (actually used as timeofprint-info)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] name;//actual active filename (max.8 chars)
            public UInt32 reserved1;	//reserved
            public UInt32 reserved2;	//reserved
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct PStatusExt
        {
            public UInt32 d_counter;//actual counter of ok-prints (reseted when message changes)
            public UInt32 s_counter;//actual counter of nok-prints (reseted when message changes)
            public UInt32 n_messageport;//actual file-nr printing
            public Byte Start;//printing or stopped
            //BIT0: printing loop (prepared for printing) BIT1: printing (we are actually marking)
            public Byte request;//waiting for request
            public Byte option;//options for HandleRequest()
            public Byte res;//reserved (0x0:default, 0x1: UMT enabled, 0x4: Batchjob enabled)
            public UInt32 t_counter;//total counter of prints
            public UInt32 m_copies;//actual nr of copies to be print
            public UInt32 err;//alarmcode
            public UInt32 time;//reserved D-Word (actually used as time-info
            public UInt32 alarmmask1;//codes the alarms
            public UInt32 signalstate;	//codes the IO signalstate
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public Byte[] messagename;//filename (max.16 chars = 12 + 4)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public Byte[] eventhandler;//filename (max.16 chars = 12 + 4)
            public UInt32 alarmmask2;   //codes the extended alarms
            public UInt32 extra;//Bit0..Bit9l dynamic usage of scanfield in permille; 
                                // Bit10..Bit13 actual dynamic mode
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct PSysinfo
        {
            public UInt32 cputemp;//cpu temperature in 1/1000 Celsius
            public UInt32 size0;//harddisk in bytes total space
            public UInt32 avail0;//in bytes available space
            public UInt32 size1;//ramdisk in bytes total space
            public UInt32 avail1;//in bytes available space
            public UInt32 size2;//ramfont in bytes total space
            public UInt32 avail2;//in bytes available space
            public UInt32 size3;//logdrive in bytes total space
            public UInt32 avail3;//in bytes available space
            public float hours;//working hours
            public UInt64 longcounter;//total printcounter
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct PCoretemp
        {
            public UInt32 cputemp;//cpu temperature in 1/1000 Celsius
            public UInt32 boardtemp;//board temperature in 1/1000 Celsius
            public UInt32 humidity;//humidity in 1/1000 percent
            public UInt32 voltage1;//5 V in 1/1000 Volt
            public UInt32 voltage2;//3.3V in 1/1000 Volt
            public UInt32 fanlocaltemp;//in 1/10 Celsius
            public UInt32 fancurrentpwm;//PWM in permille
            public UInt32 fantacho;//Tacho in cps
            public UInt32 fanremotetemp;//in 1/10 Celsius
        }
        //the structure we use within C# or .Net
        [StructLayout(LayoutKind.Sequential)]
        public struct CSStatus
        {
            public UInt32 d_counter;//actual counter of ok-prints
            public UInt32 s_counter;//actual counter of nok-prints
            public UInt32 n_messageport;//actual external message selection
            public Byte Start;//0: system is printing 1: system is stopped
            public Byte request;//(internal variable)
            public Byte option;//(internal variable)
            public Byte res;//reserved (0x0:default, 0x1: UMT enabled, 0x4: Batchjob enabled)
            public UInt32 t_counter;//total counter of prints
            public UInt32 m_copies;//actual nr of copies to be printed
            public UInt32 err;//alarmcode (upper WORD: codifies lastactive alarm; lower WORD: 0: noalarm, else: alarm active)
            public UInt32 time;//reserved D-Word (actually used as timeofprint-info)
            public String name;//actual active filename (max.8 chars)
            public UInt32 reserved1;	//reserved
            public UInt32 reserved2;	//reserved
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct CSStatusExt
        {
            public UInt32 d_counter;//actual counter of ok-prints (reseted when message changes)
            public UInt32 s_counter;//actual counter of nok-prints (reseted when message changes)
            public UInt32 n_messageport;//actual file-nr printing
            public Byte Start;//printing or stopped
            //BIT0: printing loop (prepared for printing) BIT1: printing (we are actually marking)
            public Byte request;//waiting for request
            public Byte option;//options for HandleRequest()
            public Byte res;//reserved (0x0:default, 0x1: UMT enabled, 0x4: Batchjob enabled)
            public UInt32 t_counter;//total counter of prints
            public UInt32 m_copies;//actual nr of copies to be print
            public UInt32 err;//alarmcode
            public UInt32 time;//reserved D-Word (actually used as time-info
            public UInt32 alarmmask1;	//codes the alarms
            public UInt32 signalstate;	//codes the IO signalstate
            public String messagename;//filename (max.16 chars = 12 + 4)
            public String eventhandler;//filename (max.16 chars = 12 + 4)
            public UInt32 alarmmask2;	//codes the extended alarms
            public UInt32 extra;//Bit0..Bit9l dynamic usage of scanfield in permille; 
                                // Bit10..Bit13 actual dynamic mode
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct CSSysinfo
        {
            public UInt32 cputemp;//cpu temperature in 1/1000 Celsius
            public UInt32 size0;//harddisk in bytes total space
            public UInt32 avail0;//in bytes available space
            public UInt32 size1;//ramdisk in bytes total space
            public UInt32 avail1;//in bytes available space
            public UInt32 size2;//ramfont in bytes total space
            public UInt32 avail2;//in bytes available space
            public UInt32 size3;//logdrive in bytes total space
            public UInt32 avail3;//in bytes available space
            public float hours;//working hours
            public UInt64 longcounter;//total printcounter
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct CSCoretemp
        {
            public UInt32 cputemp;//cpu temperature in 1/1000 Celsius
            public UInt32 boardtemp;//board temperature in 1/1000 Celsius
            public UInt32 humidity;//humidity in 1/1000 percent
            public UInt32 voltage1;//5 V in 1/1000 Volt
            public UInt32 voltage2;//3.3V in 1/1000 Volt
            public UInt32 fanlocaltemp;//in 1/10 Celsius
            public UInt32 fancurrentpwm;//PWM in permille
            public UInt32 fantacho;//Tacho in cps
            public UInt32 fanremotetemp;//in 1/10 Celsius
        }

    [DllImport("SocketCommDll.dll")] public static extern int MGetDllVersion();
    [DllImport("SocketCommDll.dll")] public static extern void MInit(ref Int32 p, UInt16[] name, UInt16[] ip, UInt16[] path);
    [DllImport("SocketCommDll.dll")] private static extern void MFinish(Int32 p); //Close socket
    [DllImport("SocketCommDll.dll")] private static extern Int32 MStartClient(Int32 p); //Establishes connection to the system
                                                                                        //to indicate the laser that the connection will be going down
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_ServerShutdown(Int32 p, Int32 bexit);//restarts the server and/or shuts down the firmware program
    [DllImport("SocketCommDll.dll")] private static extern  Int32 MLaser_Knockout(Int32 p); //should be called before Shutdown client
    [DllImport("SocketCommDll.dll")] private static extern  Int32 MShutdownClient(Int32 p); //Close connection to the system
    [DllImport("SocketCommDll.dll")] private static extern Int32  MGetLastError(Int32 p, UInt16[] txt); //returns pointer to internal char-array; size: 256
    [DllImport("SocketCommDll.dll")] private static extern  Int32 MIsConnected(Int32 p);
    [DllImport("SocketCommDll.dll")] private static extern  Int32 MSetTimeout(Int32 p, Int32 timeout);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_Stop(Int32 p, Int32 timeout); //Stop signal
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_FastUsermessage(Int32 p, Byte field, Byte[] text);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_FastUTF8Usermessage(Int32 p, Byte field, Byte[] text);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_GetFastUsermessage(Int32 p, Byte field, Byte[] buf, ref Int32 len);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_GetFastUTF8Usermessage(Int32 p, Byte field, Byte[] buf, ref Int32 len);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_EnableBufferedUM(Int32 p, Int32 get,ref Int32 actsize,Int32 defsize);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_EnableBufferedUMExt(Int32 p, Int32 get,ref Int32 actsize,ref Int32 field, ref Int32 fillstatus,Int32 defsize);

    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_MultipleUsermessage(Int32 p, Byte[] inbuf, Int32 inlen,Byte[] outbuf, ref Int32 outlen, ref Int32 fields);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_MultipleUTF8Usermessage(Int32 p, Byte[] inbuf, Int32 inlen,Byte[] outbuf, ref Int32 outlen, ref Int32 fields);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_GetMultipleUsermessage(Int32 p, Byte[] inbuf, Int32 inlen,Byte[] outbuf, ref Int32 outlen);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_GetMultipleUTF8Usermessage(Int32 p, Byte[] inbuf, Int32 inlen,Byte[] outbuf, ref Int32 outlen);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_GetFifofield(Int32 p, Int32 field, Int32 index, Byte[] outbuf, ref Int32 outlen, ref Int32 elements);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_GetUTF8Fifofield(Int32 p, Int32 field, Int32 index, Byte[] outbuf, ref Int32 outlen, ref Int32 elements);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_FifoDump(Int32 p);
    

    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_FastDataString(Int32 p, Byte field, Byte[] buf, Int32 len);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_GetFastDataString(Int32 p, Byte field, Byte[] buf, ref Int32 len);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_EnableBufferedDataString(Int32 p, Int32 get,ref Int32 actsize,ref Int32 field, ref Int32 fillstatus,Int32 defsize);

    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_Status(Int32 p, out PStatus status); //gets status of lasersystem
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_StatusExt(Int32 p, out PStatusExt statusext); //gets status of lasersystem
    [DllImport("SocketCommDll.dll")] private static extern UInt16  MGetVersion(Int32 p); //get version number
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_GetVersionString(Int32 p, Byte[] buf, Int32 len, Int32 option); //get the version string
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_GetConnectionData(Int32 p, ref Byte leading, ref Byte control, ref UInt16 foundmask); //get internal connection data
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_Start(Int32 p, UInt16[] filename,Int32 nr); //Start signal
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_StartExtended(Int32 p, Int32 nr, Int32 msg,Int32 batch); //Start signal for ext table (batchjob)
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_Reload(Int32 p); //reloads actual printing file
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_TriggerPrint(Int32 p); //triggers a print (software trigger)
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_Settime(Int32 p); //Sets actual system time in remote lasersystem
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_Delete(Int32 p, UInt16[] name); //deletes file
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_SetDefault(Int32 p, UInt16[] name); //sets actual file
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_CopyFile(Int32 p, UInt16[] filename, UInt16[] path, Byte option);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_PrintMode(Int32 p, ref UInt32 mode); //sets/gets printmode (default,UMT,Batchjob))
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_Mode(Int32 p, ref Byte mode); //sets mode (static/dynamic)
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_SetDynamic(Int32 p, Int32 var, ref Int32 value); 
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_GetDynamic(Int32 p, Int32 var, ref Int32 value); 
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_CounterReset(Int32 p); //Reset of d_counter and s_counter (status variable)
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_SetGlobalCounter(Int32 p, Byte field, Byte[] counter); 
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_GetGlobalCounter(Int32 p, Byte field, Byte[] counter, Int32 len); 
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_SetPrivateCounter(Int32 p, Byte field, Int32 repeats, Int32 prints); 
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_Offset(Int32 p, ref Int32 dx, ref Int32 dy, Int32 relative,Int32 format,Int32 reset);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_Defocus(Int32 p, ref Int32 dz, Int32 relative, Int32 format);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_ShiftRotate(Int32 p, float dx, float dy, float deg, float x0, float y0, UInt16[] layername,UInt16[] objectname); 
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_Powerscale(Int32 p, Int32 set, Int32 member, ref Int32 value); 
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_Setusertime(Int32 p, Int32 day, Int32 month, Int32 year, Int32 hour, Int32 minute, Int32 second);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_AsciiConfig(Int32 p, UInt16[] name,Int32 partial);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_StartPrintSession(Int32 p, Int32 ignorealarms);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_EndPrintSession(Int32 p);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_TestPointer(Int32 p,Int32 on);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_Eventhandler(Int32 p, UInt16[] name);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_GetFilenames(Int32 p, UInt16[] extension,int frame,UInt16[] buf, ref Int32 bufsize);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_Store(Int32 p, ref Int32 flags);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_MTable(Int32 p);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_DumpSVG(Int32 p, UInt16[] name);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_DumpSVGExt(Int32 p, UInt16[] name, Int32 filter, UInt16[] layername);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_Coretemp(Int32 p, out PCoretemp info);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_Sysinfo(Int32 p, out PSysinfo info);
    [DllImport("SocketCommDll.dll")] private static extern Int32  MLaser_Signalstate(Int32 p, Int32 get, ref UInt32 signalstate, ref UInt32 enabled);

        public int CS_GetDllVersion()
        {
            return MGetDllVersion();
        }

        public void CS_Init(ref Int32 p, String name, String ip, String path)
        {
            Int32 i;
            UInt16[] aname,aip,apath;
            aname = new UInt16[name.Length];
            for (i = 0; i < name.Length; i++)
            {
                aname[i] = name[i];
            }
            aip = new UInt16[ip.Length];
            for (i = 0; i < ip.Length; i++)
            {
                aip[i] = ip[i];
            }
            apath = new UInt16[path.Length];
            for (i = 0; i < path.Length; i++)
            {
                apath[i] = path[i];
            }

            MInit(ref p, aname, aip, apath);
        }
        public void CS_Finish(Int32 p)//Close socket
        {
            MFinish(p);
        }
        public Int32 CS_StartClient(Int32 p)//Establishes connection to the system
        {
            return MStartClient(p);
        }
        public Int32 CS_Knockout(Int32 p)//should be called before Shutdown client
        {
            return MLaser_Knockout(p);
        }
        public Int32 CS_ServerShutdown(Int32 p, Int32 bexit)
        {
            return MLaser_ServerShutdown(p,bexit);
        }
        public Int32 CS_ShutdownClient(Int32 p)//Close connection to the system
        {
            return MShutdownClient(p);
        }
        public Int32 CS_GetLastError(Int32 p, ref String txt)//
        {
            UInt16[] bn; Int32 ret = 0; Int32 i,len;
            bn = new UInt16[256];
            ret = MGetLastError(p, bn);
            for (i = 0; i < bn.Length; i++)
            {
                if (bn[i] == '\0') break;
            }
            len = i;
            Char[] ch = new Char[len];
            for (i = 0; i < len; i++)
            {
                ch[i] = (Char) bn[i];
            }
            txt = new String(ch);
            return ret;
        }
        public Int32 CS_IsConnected(Int32 p)
        {
            return MIsConnected(p);
        }
        public Int32 CS_SetTimeout(Int32 p, Int32 timeout)
        {
            return MSetTimeout(p, timeout);
        }
        public Int32 CS_Stop(Int32 p, Int32 timeout)
        {
            return MLaser_Stop(p, timeout);
        }
        public Int32 CS_FastASCIIUsermessage(Int32 p, Int32 field, String text)//send string "text" (max. 2040 chars) for <field> to the laser
        {
            return  MLaser_FastUTF8Usermessage(p, (Byte)field, Encoding.ASCII.GetBytes(text));
        }
        public Int32 CS_FastUsermessage(Int32 p, Int32 field, String text)//send string "text" (max. 2040 chars) for <field> to the laser
        {
            return MLaser_FastUTF8Usermessage(p, (Byte)field, Encoding.UTF8.GetBytes(text));
        }
        public Int32 CS_GetFastASCIIUsermessage(Int32 p, Int32 field, ref String txt)
        {
            Byte[] bn; Int32 ret = 0; Int32 len = 2040;
            bn = new Byte[len];
            txt = "";
            ret = MLaser_GetFastUsermessage(p, (Byte)field, bn, ref len);
            if (ret == 0)
            {
                txt = Encoding.ASCII.GetString(bn, 0, len-1);
            }
            return ret;
        }
        public Int32 CS_GetFastUsermessage(Int32 p, Int32 field, ref String txt)
        {
            Byte[] bn; Int32 ret = 0; Int32 len = 2040;
            bn = new Byte[len];
            txt = "";
            ret = MLaser_GetFastUTF8Usermessage(p, (Byte)field, bn, ref len);
            if (ret == 0)
            {
                txt = Encoding.UTF8.GetString(bn, 0, len-1);
            }
            return ret;
        }
        public Int32 CS_EnableBufferedUM(Int32 p, Int32 get, ref Int32 actsize, Int32 defsize)
        {
            return MLaser_EnableBufferedUM(p, get, ref actsize, defsize);
        }
        public Int32 CS_EnableBufferedUMExt(Int32 p, Int32 get, ref Int32 actsize, ref Int32 field, ref Int32 fillstatus, Int32 defsize)
        {
            return MLaser_EnableBufferedUMExt(p, get, ref actsize, ref field,ref fillstatus,defsize);
        }

        public Int32 CS_MultipleUsermessage(Int32 p, Byte[] inbuf, Int32 inlen, Byte[] outbuf, ref Int32 outlen, ref Int32 fields)
        {
            return MLaser_MultipleUsermessage(p, inbuf, inlen, outbuf, ref outlen,ref fields);
            
        }
        public Int32 CS_GetMultipleUsermessage(Int32 p, Byte[] inbuf, Int32 inlen, Byte[] outbuf, ref Int32 outlen )
        {
            return MLaser_GetMultipleUsermessage(p, inbuf, inlen,outbuf, ref outlen);
        }

        public Int32 CS_MultipleUTF8Usermessage(Int32 p, Byte[] inbuf, Int32 inlen, Byte[] outbuf, ref Int32 outlen, ref Int32 fields)
        {
            return MLaser_MultipleUTF8Usermessage(p, inbuf, inlen, outbuf, ref outlen, ref fields);

        }
        public Int32 CS_GetMultipleUTF8Usermessage(Int32 p, Byte[] inbuf, Int32 inlen, Byte[] outbuf, ref Int32 outlen)
        {
            return MLaser_GetMultipleUTF8Usermessage(p, inbuf, inlen, outbuf, ref outlen);
        }

        public Int32 CS_GetFifofield(Int32 p, Int32 field, Int32 index, ref string txt, ref Int32 elements){
            Byte[] bn; Int32 ret = 0; Int32 len = 2036;
            bn = new Byte[len];
            txt = "";
            ret = MLaser_GetFifofield(p, field, index, bn, ref len, ref elements);
            if (ret == 0)
            {
                txt = Encoding.ASCII.GetString(bn, 0, len);
            }
            return ret;
        }
        
        public Int32 CS_GetUTF8Fifofield(Int32 p, Int32 field, Int32 index, ref string txt, ref Int32 elements)
        {
            Byte[] bn; Int32 ret = 0; Int32 len = 2036;
            bn = new Byte[len];
            txt = "";
            ret = MLaser_GetUTF8Fifofield(p, field, index, bn, ref len, ref elements);
            if (ret == 0)
            {
                txt = Encoding.UTF8.GetString(bn, 0, len);
            }
            return ret;
        }

        public Int32 CS_FifoDump(Int32 p)
        {
            return MLaser_FifoDump(p);
        }
        
        public Int32 CS_FastDataString(Int32 p, Int32 field, Byte[] buf,Int32 len)
        {
            return MLaser_FastDataString(p, (Byte)field, buf, len);
        }
        public Int32 CS_GetFastDataString(Int32 p, Int32 field, ref Byte[] buf, ref Int32 len)
        {
            len = 2000; Int32 ret, i;
            Byte[] bf = new Byte[len];
            ret = MLaser_GetFastDataString(p, (Byte)field, bf, ref len);
            if (ret == 0)
            {
                buf = new Byte[len];
                for (i = 0; i < len; i++)
                {
                    buf[i] = bf[i];
                }
            }
            return ret;
        }
        public Int32 CS_EnableBufferedDataString(Int32 p, Int32 get, ref Int32 actsize, ref Int32 field, ref Int32 fillstatus, Int32 defsize)
        {
            return MLaser_EnableBufferedDataString(p, get, ref actsize, ref field, ref fillstatus, defsize);
        }
        public Int32 CS_Status(Int32 p, ref CSStatus status)//gets status of lasersystem
        {
            PStatus pstat; Int32 ret; Int32 i;
            ret = MLaser_Status(p, out pstat);
            if (ret == 0)
            {
                for (i = 0; i < 8; i++)
                {
                    if (pstat.name[i] == '\0') break;
                }
                status.name = Encoding.ASCII.GetString(pstat.name, 0, i);
                status.d_counter = pstat.d_counter;
                status.s_counter = pstat.s_counter;
                status.n_messageport = pstat.n_messageport;
                status.Start = pstat.Start;
                status.request = pstat.request;
                status.option = pstat.option;
                status.res = pstat.res;
                status.t_counter = pstat.t_counter;
                status.m_copies = pstat.m_copies;
                status.err = pstat.err;
                status.time = pstat.time;
                status.reserved1 = pstat.reserved1;
                status.reserved2 = pstat.reserved2;
            }
            
            return ret;
        }
        public Int32 CS_StatusExt(Int32 p, ref CSStatusExt statusext)//gets status of lasersystem
        {
            PStatusExt pstat; Int32 ret; Int32 i;
            ret = MLaser_StatusExt(p, out pstat);
            if (ret == 0)
            {
                for (i = 0; i < 16; i++)
                {
                    if (pstat.messagename[i] == '\0') break;
                }
                statusext.messagename = Encoding.ASCII.GetString(pstat.messagename, 0, i);
                for (i = 0; i < 16; i++)
                {
                    if (pstat.eventhandler[i] == '\0') break;
                }
                statusext.eventhandler = Encoding.ASCII.GetString(pstat.eventhandler, 0, i);
                statusext.d_counter = pstat.d_counter;
                statusext.s_counter = pstat.s_counter;
                statusext.t_counter = pstat.t_counter;
                statusext.n_messageport = pstat.n_messageport;
                statusext.Start = pstat.Start;
                statusext.request = pstat.request;
                statusext.option = pstat.option;
                statusext.res = pstat.res;
                statusext.m_copies = pstat.m_copies;
                statusext.err = pstat.err;
                statusext.time = pstat.time;
                statusext.alarmmask1 = pstat.alarmmask1;
                statusext.signalstate = pstat.signalstate;
                statusext.alarmmask2 = pstat.alarmmask2;
                statusext.extra = pstat.extra;
            }
            return ret;
        }
        public UInt16 CS_GetVersion(Int32 p)
        {
            return MGetVersion(p);
        }
        public Int32 CS_GetVersionString(Int32 p, ref String txt, Int32 option)
        {
            Byte[] bn; Int32 ret = 0; Int32 len = 48;
            bn = new Byte[len];
            txt = "";
            ret = MLaser_GetVersionString(p, bn, len, option);
            if (ret == 0)
            {
                txt = Encoding.ASCII.GetString(bn, 0, len - 1);
            }
            return ret;
        }
        public Int32 CS_GetConnectionData(Int32 p, ref Byte leading, ref Byte control, ref UInt16 foundmask)
        {
            return MLaser_GetConnectionData( p, ref leading, ref control, ref foundmask); //get internal connection data
        }
        //filename: name of the message inside the laser to be printed (without extension)
        //nr:  number of prints to be done (nr=0 eternal printing, nr=1 testprint)
        public Int32 CS_Start(Int32 p, String filename, Int32 nr)
        {
            UInt16[] aname; Int32 i;
            aname = new UInt16[filename.Length];
            for (i = 0; i < filename.Length; i++)
            {
                aname[i] = filename[i];
            }
            return MLaser_Start(p, aname, nr);
        }
        //Start signal for ext table (batchjob)
        //filename: name of the message inside the laser to be printed (without extension)
        //nr:  number of prints to be done (nr=0 eternal printing, nr=1 testprint)
        public Int32 CS_StartExtended(Int32 p, Int32 nr, Int32 msg, Int32 batch)
        {
            return MLaser_StartExtended(p, nr, msg, batch);
        }
        //reloads actual printing file
        //used e.g after having sent new data to the laser to reload the actual message
        public Int32 CS_Reload(Int32 p)
        {
            return MLaser_Reload(p);
        }
        //triggers a print (software trigger)
        public Int32 CS_TriggerPrint(Int32 p)
        {
            return MLaser_TriggerPrint(p);
        }
        //Sets actual system time in remote lasersystem
        //uses the OS system time as a reference time
        public Int32 CS_Settime(Int32 p)
        {
            return MLaser_Settime(p);
        }
        //deletes file
        //name: filename with extension inside the laser
        public Int32 CS_Delete(Int32 p, String name)
        {
            UInt16[] aname; Int32 i;
            aname = new UInt16[name.Length];
            for (i = 0; i < name.Length; i++)
            {
                aname[i] = name[i];
            }

            return MLaser_Delete(p, aname);
        }
        //sets actual file
        //changes actual message inside the laser to <name> (without extension)
        public Int32 CS_SetDefault(Int32 p, String name)
        {
            UInt16[] aname; Int32 i;
            aname = new UInt16[name.Length];
            for (i = 0; i < name.Length; i++)
            {
                aname[i] = name[i];
            }
            return MLaser_SetDefault(p, aname);
        }
        //sourcefile: local filename to be sent to the laser (msf-file)
        //path: directory ending with "\\" of location of the file 
        //option: copy to ramdisk or to harddisk
        public Int32 CS_CopyFile(Int32 p, String filename, String path, Byte option)
        {
            UInt16[] aname,apath; Int32 i;
            aname = new UInt16[filename.Length];
            apath = new UInt16[path.Length];
            for (i = 0; i < filename.Length; i++)
            {
                aname[i] = filename[i];
            }
            for (i = 0; i < path.Length; i++)
            {
                apath[i] = path[i];
            }
            return MLaser_CopyFile(p, aname, apath, option);
        }
        //sets/get printmode (default,UMT,batchjob)
        //mode: 0  default, 1: enable UMT 2: just get actual printmode 4: enable batchjob
        public Int32 CS_PrintMode(Int32 p, ref UInt32 mode)
        {
            Int32 ret;
            ret = MLaser_PrintMode(p, ref mode);
            return ret;
        }
        //sets mode (static/dynamic)
        //mode: 0  static, 1: dynamic standard 2: dynamic distance
        // 3: dynamic-static
        // 8: fills <mode> variable with actual mode
        public Int32 CS_Mode(Int32 p, ref Int32 mode)
        {
            Int32 ret;
            Byte bmode = (Byte) mode;
            ret = MLaser_Mode(p, ref bmode);
            mode = (Int32) bmode;
            return ret;
        }
        public Int32 CS_SetDynamic(Int32 p, Int32 var, ref Int32 value)
        {
            return MLaser_SetDynamic(p, var, ref value);
        }
        public Int32 CS_GetDynamic(Int32 p, Int32 var, ref Int32 value)
        {
            return MLaser_GetDynamic(p, var, ref value);
        }
        public Int32 CS_CounterReset(Int32 p)//Reset of d_counter and s_counter (status variable)
        {
            return MLaser_CounterReset(p);
        }
        public Int32 CS_SetGlobalCounter(Int32 p, Int32 field, String counter)//Set global counter
        {
            return MLaser_SetGlobalCounter(p, (byte)field, Encoding.ASCII.GetBytes(counter));
        }
        public Int32 CS_GetGlobalCounter(Int32 p, Int32 field, ref String counter)//Get global counter
        {
            Byte[] buf = new Byte[128]; Int32 i;
            Int32 ret;
            counter = "";
            ret = MLaser_GetGlobalCounter(p, (Byte)field, buf, 128);
            if (ret == 0)
            {
                for (i = 0; i < buf.Length; i++)
                {
                    if (buf[i] == '\0') break;
                }
                counter = Encoding.ASCII.GetString(buf,0,i);
            }
            return ret;
        }

        //Set private counter
        public Int32 CS_SetPrivateCounter(Int32 p, Int32 field, Int32 repeats, Int32 prints)
        {
            return MLaser_SetPrivateCounter(p, (Byte)field, repeats, prints);     
        }

        //set offset  (format: 0 (ideal coordinates; 1: microns; 2: 0.1mm)
        public Int32 CS_Offset(Int32 p, ref Int32 dx, ref Int32 dy, Int32 relative, Int32 format, Int32 reset)
        {
            return MLaser_Offset(p, ref dx, ref dy, relative, format, reset);
        }
        //set z defocus  (format: 0 (ideal coordinates; 1: microns; 2: 0.1mm)
        public Int32 CS_Defocus(Int32 p, ref Int32 dz, Int32 relative, Int32 format)
        {
            return MLaser_Defocus(p, ref dz, relative, format);
        }
        public Int32 CS_ShiftRotate(Int32 p, float dx, float dy, float deg, float x0, float y0, String layername, String objectname)
        {
            UInt16[] layer, obj; Int32 i;
            layer = new UInt16[layername.Length];
            for (i = 0; i < layername.Length; i++)
            {
                layer[i] = layername[i];
            }
            obj = new UInt16[objectname.Length];
            for (i = 0; i < objectname.Length; i++)
            {
                obj[i] = objectname[i];
            }
            return MLaser_ShiftRotate(p, dx, dy, deg, x0, y0, layer, obj);
        }
        public Int32 CS_Powerscale(Int32 p, Int32 set, Int32 member, ref Int32 value)
        {
            return MLaser_Powerscale(p, set, member, ref value);
        }
        public Int32 CS_Setusertime(Int32 p, Int32 day, Int32 month, Int32 year, Int32 hour, Int32 minute, Int32 second)
        {
            return MLaser_Setusertime(p, day, month, year, hour, minute, second);
        }
        public Int32 CS_AsciiConfig(Int32 p, String name, Int32 partial)
        {
            UInt16[] aname; Int32 i;
            aname = new UInt16[name.Length];
            for (i = 0; i < name.Length; i++)
            {
                aname[i] = name[i];
            }

            return MLaser_AsciiConfig(p, aname, partial);
        }
        public Int32 CS_StartPrintSession(Int32 p, Int32 ignorealarms)
        {
            return MLaser_StartPrintSession(p, ignorealarms);
        }
        public Int32 CS_EndPrintSession(Int32 p)
        {
            return MLaser_EndPrintSession(p);
        }
        public Int32 CS_TestPointer(Int32 p, Int32 on)
        {
            return MLaser_TestPointer(p, on);
        }
        public Int32 CS_Eventhandler(Int32 p, String name)
        {
            UInt16[] aname; Int32 i;
            aname = new UInt16[name.Length];
            for (i = 0; i < name.Length; i++)
            {
                aname[i] = name[i];
            }
            return MLaser_Eventhandler(p, aname);
        }
        public Int32 CS_GetFilenames(Int32 p, String extension, int frame, ref String filenames)
        {
            Int32 ret; Int32 size = 25 * 48;
            UInt16[] buf = new UInt16[size];
            UInt16[] aname; Int32 i;
            aname = new UInt16[extension.Length];
            for (i = 0; i < extension.Length; i++)
            {
                aname[i] = extension[i];
            }
            filenames = "";
            ret = MLaser_GetFilenames(p, aname, frame, buf, ref size);
            if(ret==0){
                Char[] ch = new Char[size];
                for (i = 0; i < size; i++)
                {
                    ch[i] = (Char) buf[i];
                }
                filenames = new String(ch);
            }
            return ret;
        }
        public Int32 CS_Store(Int32 p, ref Int32 flags)
        {
            return MLaser_Store(p, ref flags);
        }
        public Int32 CS_MTable(Int32 p)
        {
            return MLaser_MTable(p);
        }
        public Int32 CS_DumpSVG(Int32 p, String name)
        {
            UInt16[] aname; Int32 i;
            aname = new UInt16[name.Length];
            for (i = 0; i < name.Length; i++)
            {
                aname[i] = name[i];
            }
            return MLaser_DumpSVG(p, aname);
        }
        public Int32 CS_DumpSVGExt(Int32 p, String name, Int32 filter, String layername)
        {
            UInt16[] aname; Int32 i;UInt16[] lname;
            aname = new UInt16[name.Length];
            for (i = 0; i < name.Length; i++)
            {
                aname[i] = name[i];
            }
            lname = new UInt16[layername.Length];
            for (i = 0; i < layername.Length; i++)
            {
                lname[i] = layername[i];
            }
            return MLaser_DumpSVGExt(p, aname,filter,lname);
        }
        public Int32 CS_Sysinfo(Int32 p, ref CSSysinfo info)
        {
            PSysinfo pinfo; Int32 ret; 
            ret = MLaser_Sysinfo(p, out pinfo);
            if (ret == 0)
            {
                info.size0 = pinfo.size0;
                info.size1 = pinfo.size1;
                info.size2 = pinfo.size2;
                info.size3 = pinfo.size3;
                info.avail0 = pinfo.avail0;
                info.avail1 = pinfo.avail1;
                info.avail2 = pinfo.avail2;
                info.avail3 = pinfo.avail3;
                info.cputemp = pinfo.cputemp;
                info.hours = pinfo.hours;
                info.longcounter = pinfo.longcounter;
            }

            return ret;
        }

        public Int32 CS_Coretemp(Int32 p, ref CSCoretemp info)
        {
            PCoretemp pinfo; Int32 ret; 
            ret = MLaser_Coretemp(p, out pinfo);
            if (ret == 0)
            {
                info.boardtemp = pinfo.boardtemp;
                info.cputemp = pinfo.cputemp;
                info.humidity = pinfo.humidity;
                info.voltage1 = pinfo.voltage1;
                info.voltage2 = pinfo.voltage2;
                info.fancurrentpwm = pinfo.fancurrentpwm;
                info.fanlocaltemp = pinfo.fanlocaltemp;
                info.fanremotetemp = pinfo.fanremotetemp;
                info.fantacho = pinfo.fantacho;
            }

            return ret;
        }
        public Int32 CS_Signalstate(Int32 p, Int32 get, ref UInt32 signalstate, ref UInt32 enabled)
        {
            return MLaser_Signalstate(p, get, ref signalstate, ref enabled);
        }


    }

}
