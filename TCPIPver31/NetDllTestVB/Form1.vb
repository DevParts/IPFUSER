Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim scomm As SocketCommNet.SocketComm
        Dim count As Int32
        Dim pcount As Int32
        Dim name As String
        Dim ip As String
        Dim path As String
        Dim p As Int32
        Dim txt As String
        Dim err As Int32
        Dim mode As Int32
        Dim value As Int32
        Dim messname As String
        Dim status As SocketCommNet.SocketComm.CSStatusExt
        Dim consoletxt As String
        Dim sysinfo As SocketCommNet.SocketComm.CSSysinfo
        Dim coretemp As SocketCommNet.SocketComm.CSCoretemp

        'ListBox1.Items.Clear()
        name = "dummy"
        messname = ""
        ip = IPaddress.Text()
        path = "C:\\Fly"
        p = 0
        pcount = 0
        'create comm-socket
        scomm = New SocketCommNet.SocketComm()
        'create a status structure
        status = New SocketCommNet.SocketComm.CSStatusExt()
        'get the actual DLL version
        consoletxt = "DLL version  " + scomm.CS_GetDllVersion().ToString()
        Console.WriteLine(consoletxt)
        ListBox1.Items.Add(consoletxt)
        txt = "try"
        ListBox1.Items.Add(txt)
        consoletxt = "try to connect to: " + ip + ", path:" + path
        Console.WriteLine(consoletxt)
        ListBox1.Items.Add(consoletxt)
        'init socket
        scomm.CS_Init(p, name, ip, path)
        err = scomm.CS_GetLastError(p, txt)
        consoletxt = "GetError returns " + err.ToString() + txt
        Console.WriteLine(consoletxt)
        ListBox1.Items.Add(consoletxt)
        'start the client to connect with the server
        err = scomm.CS_StartClient(p)
        consoletxt = "Startclient returns " + err.ToString()
        Console.WriteLine(consoletxt)
        ListBox1.Items.Add(consoletxt)
        'get 1.version string
        err = scomm.CS_GetVersionString(p, txt, 0)
        consoletxt = "1. Versionstring " + txt
        Console.WriteLine(consoletxt)
        ListBox1.Items.Add(consoletxt)
        'get 2.version string
        err = scomm.CS_GetVersionString(p, txt, 1)
        consoletxt = "2. Versionstring " + txt
        Console.WriteLine(consoletxt)
        ListBox1.Items.Add(consoletxt)
        'get connection data
        Dim leading As Byte
        Dim control As Byte
        Dim foundmask As UInt16
        err = scomm.CS_GetConnectionData(p, leading, control, foundmask)
        consoletxt = "ConnectionData <" + Hex(leading) + "> <" + Hex(control) + "> <" + Hex(foundmask) + ">"
        Console.WriteLine(consoletxt)
        ListBox1.Items.Add(consoletxt)

        'get the extended status information
        err = scomm.CS_StatusExt(p, status)
        If err = 0 Then
            consoletxt = "StatusExt returns " + err.ToString() + pcount.ToString()
            Console.WriteLine(consoletxt)
        End If
        ListBox1.Items.Add(consoletxt)
        If err = 0 Then
            consoletxt = "StatusExt messagename=" + status.messagename
            Console.WriteLine(consoletxt)
            ListBox1.Items.Add(consoletxt)
            consoletxt = "StatusExt t_counter=" + status.t_counter.ToString()
            Console.WriteLine(consoletxt)
            ListBox1.Items.Add(consoletxt)
            consoletxt = "StatusExt time=" + status.time.ToString()
            Console.WriteLine(consoletxt)
            ListBox1.Items.Add(consoletxt)
            consoletxt = "StatusExt alarmmask1=" + status.alarmmask1.ToString("X")
            Console.WriteLine(consoletxt)
            ListBox1.Items.Add(consoletxt)
            consoletxt = "StatusExt alarmmask2=" + status.alarmmask2.ToString("X")
            Console.WriteLine(consoletxt)
            ListBox1.Items.Add(consoletxt)
            consoletxt = "StatusExt Start=" + status.Start.ToString("X")
            Console.WriteLine(consoletxt)
            ListBox1.Items.Add(consoletxt)
            messname = status.messagename
        End If
        'send some usermessages
        txt = "ätorgelätorgel"
        Dim field As Int32
        field = 0
        If err = 0 Then
            err = scomm.CS_FastUsermessage(p, field, txt)
        End If
        field = 1
        txt = "wweweeweew"
        If err = 0 Then
            err = scomm.CS_FastUsermessage(p, field, txt)
        End If
        'get them back again
        If err = 0 Then
            err = scomm.CS_GetFastUsermessage(p, 0, txt)
        End If
        If err = 0 Then
            consoletxt = "Usermessage Field " + field.ToString() + ": " + txt
            Console.WriteLine(consoletxt)
            ListBox1.Items.Add(consoletxt)
        End If
        If err = 0 Then
            err = scomm.CS_GetFastUsermessage(p, 1, txt)
        End If
        If err = 0 Then
            consoletxt = "Usermessage Field " + field.ToString() + ": " + txt
            Console.WriteLine(consoletxt)
            ListBox1.Items.Add(consoletxt)
        End If
        'set some global counter
        txt = "12342534653"
        field = 0
        If err = 0 Then
            err = scomm.CS_SetGlobalCounter(p, field, txt)
        End If
        txt = ""
        'get it back again
        If err = 0 Then
            err = scomm.CS_GetGlobalCounter(p, field, txt)
        End If
        If err = 0 Then
            consoletxt = "GetGlobalCounter " + field.ToString() + ": " + txt
            Console.WriteLine(consoletxt)
            ListBox1.Items.Add(consoletxt)
        End If
        'get filenames with extension 'msf'
        If err = 0 Then
            txt = ""
            err = scomm.CS_GetFilenames(p, "msf", 0, txt)
        End If
        If err = 0 Then
            consoletxt = "Filenames:" + txt
            Console.WriteLine(consoletxt)
            ListBox1.Items.Add(consoletxt)
        End If
        'set a default message inside the laser
        err = scomm.CS_SetDefault(p, "test.msf")
        'get the status
        err = scomm.CS_StatusExt(p, status)
        If err = 0 Then
            consoletxt = "Message = " + status.messagename
            Console.WriteLine(consoletxt)
            ListBox1.Items.Add(consoletxt)
        End If
        'shift and rotate the message
        err = scomm.CS_ShiftRotate(p, 10, 20, 45, 50, 50, "", "")
        'get the dynamic mode
        err = scomm.CS_GetDynamic(p, 1, mode)
        If err = 0 Then
            consoletxt = "GetDynamic (internal encoder velocity):" + mode.ToString()
            Console.WriteLine(consoletxt)
            ListBox1.Items.Add(consoletxt)
        End If
        'get the actual value of power scaling
        err = scomm.CS_Powerscale(p, 1, 3, value)
        If err = 0 Then
            consoletxt = "Scale power:" + value.ToString()
            Console.WriteLine(consoletxt)
            ListBox1.Items.Add(consoletxt)
        End If
        'get the actual value of layerspeed scaling
        If err = 0 Then
            err = scomm.CS_Powerscale(p, 1, 2, value)
        End If
        If err = 0 Then
            consoletxt = "Scale layerspeed:" + value.ToString()
            Console.WriteLine(consoletxt)
            ListBox1.Items.Add(consoletxt)
        End If
        'set the default message to the initial message
        If err = 0 Then
            err = scomm.CS_SetDefault(p, messname)
        End If
        'get the status
        If err = 0 Then
            err = scomm.CS_StatusExt(p, status)

        End If
        If err = 0 Then
            consoletxt = "Message = " + status.messagename
            Console.WriteLine(consoletxt)
            ListBox1.Items.Add(consoletxt)
        End If
        'get system information
        If err = 0 Then
            err = scomm.CS_Sysinfo(p, sysinfo)
            If err = 0 Then
                consoletxt = "Systeminformation"
                Console.WriteLine(consoletxt)
                ListBox1.Items.Add(consoletxt)
                consoletxt = "Harddisk available: " + (sysinfo.avail0 / 1000000).ToString + " Mb of " + (sysinfo.size0 / 1000000).ToString
                Console.WriteLine(consoletxt)
                ListBox1.Items.Add(consoletxt)
                consoletxt = "RAMdisk available: " + (sysinfo.avail1 / 1000000).ToString + " Mb of " + (sysinfo.size1 / 1000000).ToString
                Console.WriteLine(consoletxt)
                ListBox1.Items.Add(consoletxt)
                consoletxt = "Workinghours: " + (sysinfo.hours).ToString
                Console.WriteLine(consoletxt)
                ListBox1.Items.Add(consoletxt)
                consoletxt = "CPU temp: " + (sysinfo.cputemp / 1000.0).ToString
                Console.WriteLine(consoletxt)
                ListBox1.Items.Add(consoletxt)

            End If

        End If
        'get some temp. values
        If err = 0 Then
            err = scomm.CS_Coretemp(p, coretemp)
            If err = 0 Then
                consoletxt = "CPU temp: " + (coretemp.cputemp / 1000.0).ToString
                Console.WriteLine(consoletxt)
                ListBox1.Items.Add(consoletxt)
                consoletxt = "Board temp: " + (coretemp.boardtemp / 1000.0).ToString
                Console.WriteLine(consoletxt)
                ListBox1.Items.Add(consoletxt)
                consoletxt = "Fan remote temp: " + (coretemp.fanremotetemp / 10).ToString
                Console.WriteLine(consoletxt)
                ListBox1.Items.Add(consoletxt)
                consoletxt = "Fan local temp: " + (coretemp.fanlocaltemp / 10).ToString
                Console.WriteLine(consoletxt)
                ListBox1.Items.Add(consoletxt)
                consoletxt = "Fan PWM [%]: " + (coretemp.fancurrentpwm / 10).ToString
                Console.WriteLine(consoletxt)
                ListBox1.Items.Add(consoletxt)
                consoletxt = "Fan tacho [cps]: " + (coretemp.fantacho).ToString
                Console.WriteLine(consoletxt)
                ListBox1.Items.Add(consoletxt)

            End If
        End If
        'tell the server that you are going to leave
        scomm.CS_Knockout(p)
        'terminate the socket
        scomm.CS_Finish(p)
        consoletxt = "Finished\n"
        Console.WriteLine(consoletxt)
        ListBox1.Items.Add(consoletxt)

    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        IPaddress.Text() = "192.168.0.180"
    End Sub
End Class
