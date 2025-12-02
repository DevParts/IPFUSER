using System;
using System.Collections.Generic;
using System.Text;

namespace NetDllTest
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketCommNet.SocketComm scomm;
            SocketCommNet.SocketComm.CSStatusExt status = new SocketCommNet.SocketComm.CSStatusExt();
            String name, ip, path; Int32 p; String txt = ""; Int32 err;
            name = "dummy"; ip = "192.168.16.180"; path = "C:\\Fly";
            p = 0;
            scomm = new SocketCommNet.SocketComm();
            Console.WriteLine("try to connect to: " + ip + ", path:" + path);
            scomm.CS_Init(ref p, name, ip, path);
            err = scomm.CS_GetLastError(p, ref txt);
            Console.WriteLine("GetError returns " + err.ToString() + txt);
            err = scomm.CS_StartClient(p);
            Console.WriteLine("Startclient returns " + err.ToString());
            if (err == 0)
            {
                err = scomm.CS_StatusExt(p, ref status);
                Console.WriteLine("StatusExt returns " + err.ToString());
                if (err == 0)
                {
                    Console.WriteLine("StatusExt messagename=" + status.messagename);
                    Console.WriteLine("StatusExt t_counter=" + status.t_counter.ToString());
                }
                txt = "frefraljri";
                Int32 field = 0;
                if (err == 0) err = scomm.CS_FastASCIIUsermessage(p, field, txt);
                field = 1;
                txt = "wweweeweew";
                if (err == 0) err = scomm.CS_FastASCIIUsermessage(p, field, txt);
                if (err == 0) err = scomm.CS_GetFastASCIIUsermessage(p, 0, ref txt);
                if (err == 0)
                {
                    Console.WriteLine("Usermessage Field " + field.ToString() + ": " + txt);

                }
                if (err == 0) err = scomm.CS_GetFastASCIIUsermessage(p, 1, ref txt);
                if (err == 0)
                {
                    Console.WriteLine("Usermessage Field " + field.ToString() + ": " + txt);

                }
                Byte[] buf = new Byte[123]; Int32 i; Int32 len = 123;
                for (i = 0; i < len; i++) buf[i] = 0xf1;
                field = 0;
                if (err == 0) err = scomm.CS_FastDataString(p, field, buf, len);
                for (i = 0; i < len; i++) buf[i] = 0x10;
                field = 1;
                if (err == 0) err = scomm.CS_FastDataString(p, field, buf, len);
                len = 0;
                field = 0;
                err = scomm.CS_GetFastDataString(p, field, ref buf, ref len);
                if (err == 0)
                {
                    String s; Int32 j;
                    s = "";
                    for (j = 0; j < len; j++)
                    {
                        s += buf[j].ToString();
                    }
                    Console.WriteLine("Datastring Field " + field.ToString() + ": " + s);

                }

                len = 0;
                field = 1;
                if (err == 0) err = scomm.CS_GetFastDataString(p, field, ref buf, ref len);
                if (err == 0)
                {
                    String s; Int32 j;
                    s = "";
                    for (j = 0; j < len; j++)
                    {
                        s += buf[j].ToString();
                    }
                    Console.WriteLine("Datastring Field " + field.ToString() + ": " + s);

                }

                if (err == 0) err = scomm.CS_GetFilenames(p, "msf", 0, ref txt);
                if (err == 0)
                {
                    Console.WriteLine("Filenames:" + txt);

                }
                txt = "12342534653";
                if (err == 0) err = scomm.CS_SetGlobalCounter(p, 0, txt);
                txt = "";
                if (err == 0) err = scomm.CS_GetGlobalCounter(p, 0, ref txt);
                if (err == 0)
                {
                    Console.WriteLine("Counter field 0:" + txt);
                }
                {
                    //send multiple usermessage
                    String text;
                    Byte[] inbuf; int inlen = 2030;
                    int j, k, n; Byte[] baux; Byte[] outbuf; Int32 outlen; Int32 fields;
                    inbuf = new Byte[inlen];
                    outbuf = new Byte[inlen];
                    outlen = inlen;
                    j = 0;
                    n = 36;
                    fields = 0;
                    Console.WriteLine("Write multiple usermessage:");
                    for (i = 0; i < n; i++)
                    {
                        inbuf[j++] = (byte)i;
                        text = "abcde";
                        text += i.ToString();
                        Console.WriteLine("Field: " + i.ToString() + " string: " + text);
                        baux = Encoding.ASCII.GetBytes(text);
                        for (k = 0; k < baux.GetLength(0); k++)
                        {
                            if (j < 2030) inbuf[j++] = baux[k];
                        }
                        if (j < 2030) inbuf[j++] = 0;
                    }
                    err = scomm.CS_MultipleUsermessage(p, inbuf, j, outbuf, ref outlen, ref fields);
                    Console.WriteLine("changed fields: " + fields.ToString());
                    for (j = 0; j < outlen; j++)
                    {
                        Console.WriteLine("field return=" + outbuf[j].ToString());
                    }
                }

                //receive multiple usermessage
                {
                    String text;
                    Byte[] inbuf; Int32 inlen = 2030;
                    Byte[] outbuf; Int32 outlen = 2030;
                    Byte[] aux; Byte bct;
                    int j, k, n;
                    inbuf = new Byte[inlen];
                    outbuf = new Byte[outlen];
                    aux = new Byte[outlen];
                    j = 0;
                    n = 36;
                    Console.WriteLine("Read multiple usermessage:");
                    for (i = 0; i < n; i++)
                    {
                        inbuf[j++] = (byte)i;
                        Console.WriteLine("Field: " + i.ToString());
                    }
                    err = scomm.CS_GetMultipleUsermessage(p, inbuf, j, outbuf, ref outlen);
                    //decode data in outbuf
                    //  |field|string|0|field|string|0|....
                    j = 0;
                    while (j < outlen)
                    {
                        k = 0;
                        Console.WriteLine("field:" + outbuf[j++].ToString());
                        while (j < outlen)
                        {
                            bct = outbuf[j++];
                            if (bct != 0) aux[k++] = bct;
                            else break;
                        }
                        Console.WriteLine("string: " + Encoding.ASCII.GetString(aux, 0, k));
                    }

                }
                //enable buffered usermessage
                {
                    int actsize, defsize;
                    actsize = 0; defsize = 60;
                    err = scomm.CS_EnableBufferedUM(p, 0, ref actsize, defsize);
                    Console.WriteLine("Enable FIFOs: err=" + err.ToString());
                }
                //fill fifo fields with multiple message
                {
                    //send multiple usermessage
                    String text;
                    Byte[] inbuf; int inlen = 2030;
                    int j, k, m, n; Byte[] baux; Byte[] outbuf; Int32 outlen; Int32 fields;
                    inbuf = new Byte[inlen];
                    outbuf = new Byte[inlen];
                    outlen = inlen;
                    n = 3;//3 fields
                    Console.WriteLine("Write multiple usermessage:");
                    for (m = 0; m < 10; m++)//10 entries
                    {
                        j = 0;
                        fields = 0;
                        for (i = 0; i < n; i++)//fields
                        {
                            inbuf[j++] = (byte)i;
                            text = "abc";
                            text += m.ToString();
                            text += '.';
                            text += i.ToString();
                            Console.WriteLine("field: " + field.ToString() + " entry: " + m.ToString() + " text:" + text);
                            baux = Encoding.ASCII.GetBytes(text);
                            for (k = 0; k < baux.GetLength(0); k++)
                            {
                                if (j < 2030) inbuf[j++] = baux[k];
                            }
                            if (j < 2030) inbuf[j++] = 0;
                        }
                        //send all fields at once
                        Console.WriteLine("Send entry: " + m.ToString());
                        err = scomm.CS_MultipleUsermessage(p, inbuf, j, outbuf, ref outlen, ref fields);
                        Console.WriteLine("changed fields: " + fields.ToString());
                        for (j = 0; j < outlen; j++)
                        {
                            Console.WriteLine("field return=" + outbuf[j].ToString());
                        }
                    }
                }
                {//get fifo entry
                    String text; int elements;
                    int n;
                    elements = 0;
                    text = "";
                    n = 10;
                    Console.WriteLine("Get some fifo entries:");
                    field = 0;
                    for (i = 0; i < n; i++)
                    {
                        err = scomm.CS_GetFifofield(p, field, i, ref text, ref elements);
                        Console.WriteLine("GetFifofield: " + field.ToString() + ":" + i.ToString() + ":" + elements.ToString() + ":" + text);
                    }
                    field = 2;
                    for (i = 0; i < n; i++)
                    {
                        err = scomm.CS_GetFifofield(p, field, i, ref text, ref elements);
                        Console.WriteLine("GetFifofield: " + field.ToString() + ":" + i.ToString() + ":" + elements.ToString() + ":" + text);
                    }
                }
                {//get complete FIFO dump in a file (umdump.tmp in RAMDISK)
                    err = scomm.CS_FifoDump(p);
                    Console.WriteLine("FIFOdump: err=" + err.ToString());
                    err = scomm.CS_CopyFile(p, "umdump.tmp", "./", 4);

                }
                //disable buffered usermessage
                {
                    int actsize, defsize;
                    actsize = 0; defsize = 0;
                    err = scomm.CS_EnableBufferedUM(p, 0, ref actsize, defsize);
                    Console.WriteLine("Disable FIFOs: err=" + err.ToString());
                }

            }
            scomm.CS_Knockout(p);
            scomm.CS_Finish(p);
            Console.WriteLine("Finished\n");
            Console.WriteLine("press any key");
            Console.ReadKey();
        }
    }
}
