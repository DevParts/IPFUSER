using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SocketCommDllTest
{
    public partial class MainW : Form
    {
        SocketCommNet.SocketComm scomm = new SocketCommNet.SocketComm();
        SocketCommNet.SocketComm.CSStatusExt status = new SocketCommNet.SocketComm.CSStatusExt();

        int p = 0; int err;
        Point screen_inf_p = new Point(0, 0);

        public MainW()
        {
            InitializeComponent();        
        }

        /*
        When you first open a form, the following events occur in this order:
            Open → (Load) → Resize → Activate → Current
        When you close a form, the following events occur in this order:
            Unload → Deactivate → Close
        */
        private void MainW_Load(object sender, EventArgs e)
        {
            updatePvalues(p);
            statusextTV1.ExpandAll();
            statusextTV2.ExpandAll();
            coretempTV1.ExpandAll();
            coretempTV2.ExpandAll();
            sysinfoTV1.ExpandAll();
            sysinfoTV2.ExpandAll();
            cb_mode_laser_mode.SelectedIndex = 4; //(8 request mode)
            cb_var_setdynamic.SelectedIndex = 0;
            cb_var_getdynamic.SelectedIndex = 0;
            cb_option_copyfile.SelectedIndex = 0;
            cb_relative_offset.SelectedIndex = 0;
            cb_format_offset.SelectedIndex = 0;
            cb_reset_offset.SelectedIndex = 0;
            cb_flags_store.SelectedIndex = 0;
            cb_partial_asciiconfig.SelectedIndex = 0;
            cb_get_enablebufferedum.SelectedIndex = 0;
            cb_get_enablebuffereddatastring.SelectedIndex = 0;
            cb_on_testpointer.SelectedIndex = 0;
            cb_set_powerscale.SelectedIndex = 0;
            cb_member_powerscale.SelectedIndex = 0;     
        }

        
//LASER FUNCTIONS -------------------------------------------------------------------------------
        //Connection
        private void init_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            p = int.Parse(init_p.Text);
            string name = Connection.Text;
            string ip = ip0.Text + "." + ip1.Text + "." + ip2.Text + "." + ip3.Text;
            string path = init_path.Text;
            scomm.CS_Init(ref p, name, ip, path);         
            Cursor.Current = Cursors.Default;
        }


        private void getlasterror_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string txt = "";
            err = scomm.CS_GetLastError(p, ref txt);
            txt_getlasterror.Text = txt;

            getlasterror_return.Text = err.ToString();
            if (err == 0) { getlasterror_return.BackColor = System.Drawing.Color.LightGreen; }
            else { getlasterror_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }

        private void startclient_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            err = scomm.CS_StartClient(p);

            startclient_return.Text = err.ToString();
            if (err == 0) { startclient_return.BackColor = System.Drawing.Color.LightGreen; }
            else { startclient_return.BackColor = System.Drawing.Color.Red; }            
            Cursor.Current = Cursors.Default;
        }

        private void isconnected_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            err = scomm.CS_IsConnected(p);

            isconnected_return.Text = err.ToString();
            if (err == 1) { isconnected_return.BackColor = System.Drawing.Color.LightGreen; } // 1 == connected
            else { isconnected_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }

        //Status/Mode
        private void statusext_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            err = scomm.CS_StatusExt(p, ref status);
            
            statusext1.Text = status.alarmmask1.ToString();
            statusext2.Text = status.alarmmask2.ToString();
            statusext3.Text = status.d_counter.ToString();
            statusext4.Text = status.err.ToString();
            statusext5.Text = status.eventhandler;
            statusext6.Text = status.messagename;
            statusext7.Text = status.m_copies.ToString();
            statusext8.Text = status.n_messageport.ToString();

            statusext9.Text = status.option.ToString(); //byte
            statusext10.Text = status.request.ToString(); //byte
            statusext11.Text = status.res.ToString(); //byte
            statusext12.Text = status.signalstate.ToString();
            statusext13.Text = status.Start.ToString(); //byte
            statusext14.Text = status.s_counter.ToString();
            statusext15.Text = status.time.ToString();
            statusext16.Text = status.t_counter.ToString();

            statusext_return.Text = err.ToString();
            if (err == 0) { statusext_return.BackColor = System.Drawing.Color.LightGreen; }
            else { statusext_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void laser_mode_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int mode = int.Parse(cb_mode_laser_mode.SelectedItem.ToString().Split('(')[0].Split(' ')[0]);
            err = scomm.CS_Mode(p, ref mode);
            if (mode == 8){ mode = 4; }
            cb_mode_laser_mode.SelectedIndex = mode;

            laser_mode_return.Text = err.ToString();
            if (err == 0) { laser_mode_return.BackColor = System.Drawing.Color.LightGreen; }
            else { laser_mode_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void setdynamic_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int var = cb_var_setdynamic.SelectedIndex;
            int value = int.Parse(value_setdynamic.Text);    
            err = scomm.CS_SetDynamic(p, value, ref value);

            setdynamic_return.Text = err.ToString();
            if (err == 0) { setdynamic_return.BackColor = System.Drawing.Color.LightGreen; }
            else { setdynamic_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void getdynamic_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int var = cb_var_getdynamic.SelectedIndex;
            int value = 0;
            err = scomm.CS_GetDynamic(p, var, ref value);

            getdynamic_return.Text = err.ToString();
            if (err == 0) { getdynamic_return.BackColor = System.Drawing.Color.LightGreen; }
            else { getdynamic_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }

        //Files
        private void getfilenames_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string filenames = "";
            string extension = extension_getfilename.Text;
            int frame = int.Parse(frame_getfilenames.Text);
            err = scomm.CS_GetFilenames(p, extension, frame, ref filenames);

            string[] arr = filenames.Split('\n');

            //clean the combobox
            cb_filenames_getfilenames.Items.Clear();

            //sort the received names
            Array.Sort(arr, StringComparer.InvariantCulture);
            for (int i = 0; i<arr.Length-1; i++) //-1 to avoid the "" string created with the split('\n')
            {
                cb_filenames_getfilenames.Items.Insert(i,arr[i+1]);
                if (i == 0)
                {
                    cb_filenames_getfilenames.SelectedIndex = i;
                }
            }

            getfilenames_return.Text = err.ToString();
            if (err == 0) { getfilenames_return.BackColor = System.Drawing.Color.LightGreen; }
            else { getfilenames_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void copyfile_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string filename = filename_copyfile.Text;
            string path = path_copyfile.Text+"\\";
            byte option = Convert.ToByte(int.Parse(cb_option_copyfile.SelectedItem.ToString().Split('(')[0].Split(' ')[0]));
            err = scomm.CS_CopyFile(p, filename, path, option);

            copyfile_return.Text = err.ToString();
            if (err == 0) { copyfile_return.BackColor = System.Drawing.Color.LightGreen; }
            else { copyfile_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void delete_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string name = name_delete.Text;
            err = scomm.CS_Delete(p, name);

            delete_return.Text = err.ToString();
            if (err == 0) { delete_return.BackColor = System.Drawing.Color.LightGreen; }
            else { delete_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void asciiconfig_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string name = name_asciiconfig.Text;
            int partial = cb_partial_asciiconfig.SelectedIndex;
            err = scomm.CS_AsciiConfig(p, name, partial);

            asciiconfig_return.Text = err.ToString();
            if (err == 0) { asciiconfig_return.BackColor = System.Drawing.Color.LightGreen; }
            else { asciiconfig_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void setdefault_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string name = name_setdefault.Text;
            err = scomm.CS_SetDefault(p, name);

            setdefault_return.Text = err.ToString();
            if (err == 0) { setdefault_return.BackColor = System.Drawing.Color.LightGreen; }
            else { setdefault_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }


        //Printing
        private void startprintsession_p_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int ignorealarms = int.Parse(ignorealarms_startprintsession.Text);
            err = scomm.CS_StartPrintSession(p, ignorealarms);

            startprintsession_return.Text = err.ToString();
            if (err == 0) { startprintsession_return.BackColor = System.Drawing.Color.LightGreen; }
            else { startprintsession_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void start_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string filename = filename_start.Text;
            int nr = int.Parse(nr_start.Text);
            err = scomm.CS_Start(p, filename, nr);

            start_return.Text = err.ToString();
            if (err == 0) { start_return.BackColor = System.Drawing.Color.LightGreen; }
            else { start_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void startextended_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int nr = int.Parse(nr_startextended.Text);
            int msg = int.Parse(msg_startextended.Text);
            int batch = int.Parse(msg_startextended.Text);
            err = scomm.CS_StartExtended(p, nr, msg, batch);

            startextended_return.Text = err.ToString();
            if (err == 0) { startextended_return.BackColor = System.Drawing.Color.LightGreen; }
            else { startextended_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void reload_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            err = scomm.CS_Reload(p);

            reload_return.Text = err.ToString();
            if (err == 0) { reload_return.BackColor = System.Drawing.Color.LightGreen; }
            else { reload_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void stop_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int timeout = int.Parse(timeout_stop.Text);
            err = scomm.CS_Stop(p, timeout);

            stop_return.Text = err.ToString();
            if (err == 0) { stop_return.BackColor = System.Drawing.Color.LightGreen; }
            else { stop_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void triggerprint_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            err = scomm.CS_TriggerPrint(p);

            triggerprint_return.Text = err.ToString();
            if (err == 0) { triggerprint_return.BackColor = System.Drawing.Color.LightGreen; }
            else { triggerprint_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void endprintsession_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            err = scomm.CS_EndPrintSession(p);

            endprintsession_return.Text = err.ToString();
            if (err == 0) { endprintsession_return.BackColor = System.Drawing.Color.LightGreen; }
            else { endprintsession_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }


        //Offset
        private void offset_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int dx = int.Parse(dx_offset.Text);
            int dy = int.Parse(dy_offset.Text);
            int relative = cb_relative_offset.SelectedIndex;
            int format = cb_format_offset.SelectedIndex;
            int reset = cb_reset_offset.SelectedIndex;
            err = scomm.CS_Offset(p, ref dx, ref dy, relative, format, reset);    
            dx_offset.Text = dx.ToString();
            dy_offset.Text = dy.ToString();

            offset_return.Text = err.ToString();
            if (err == 0) { offset_return.BackColor = System.Drawing.Color.LightGreen; }
            else { offset_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void shiftrotate_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            float dx = float.Parse(dx_shiftrotate.Text);
            float dy = float.Parse(dy_shiftrotate.Text);
            float angle = float.Parse(angle_shiftrotate.Text);
            float x0 = float.Parse(x0_shiftrotate.Text);
            float y0 = float.Parse(y0_shiftrotate.Text);
            string layername = layername_shiftrotate.Text;
            string objectname = objectname_shiftrotate.Text;
            err = scomm.CS_ShiftRotate(p, dx, dy, angle, x0, y0, layername, objectname);

            shiftrotate_return.Text = err.ToString();
            if (err == 0) { shiftrotate_return.BackColor = System.Drawing.Color.LightGreen; }
            else { shiftrotate_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }


        //Counter/Time
        private void counterreset_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            err = scomm.CS_CounterReset(p);

            counterreset_return.Text = err.ToString();
            if (err == 0) { counterreset_return.BackColor = System.Drawing.Color.LightGreen; }
            else { counterreset_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void settime_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            err = scomm.CS_Settime(p);

            settime_return.Text = err.ToString();
            if (err == 0) { settime_return.BackColor = System.Drawing.Color.LightGreen; }
            else { settime_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void setglobalcounter_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int field = int.Parse(field_setglobalcounter.Text);
            string counter = counter_setglobalcounter.Text;
            err = scomm.CS_SetGlobalCounter(p, field, counter);

            setglobalcounter_return.Text = err.ToString();
            if (err == 0) { setglobalcounter_return.BackColor = System.Drawing.Color.LightGreen; }
            else { setglobalcounter_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void getglobalcounter_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int field = int.Parse(field_getglobalcounter.Text);
            string counter = "0";
            err = scomm.CS_GetGlobalCounter(p, field, ref counter);
            counter_getglobalcounter.Text = counter;

            getglobalcounter_return.Text = err.ToString();
            if (err == 0) { getglobalcounter_return.BackColor = System.Drawing.Color.LightGreen; }
            else { getglobalcounter_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void setprivatecounter_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int field = int.Parse(field_setprivatecounter.Text);
            int repeats = int.Parse(repeats_setprivatecounter.Text);
            int prints = int.Parse(prints_setprivatecounter.Text);
            err = scomm.CS_SetPrivateCounter(p, field, repeats, prints);

            setprivatecounter_return.Text = err.ToString();
            if (err == 0) { setprivatecounter_return.BackColor = System.Drawing.Color.LightGreen; }
            else { setprivatecounter_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }

        
        //User message
        private void store_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int flags = cb_flags_store.SelectedIndex + 1;
            err = scomm.CS_Store(p, ref flags);
            cb_flags_store.SelectedIndex = flags - 1;

            store_return.Text = err.ToString();
            if (err == 0) { store_return.BackColor = System.Drawing.Color.LightGreen; }
            else { store_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void fastusermessage_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int field = int.Parse(field_fastusermessage.Text);
            string text = text_fastusermessage.Text;
            err = scomm.CS_FastASCIIUsermessage(p, field, text);

            fastusermessage_return.Text = err.ToString();
            if (err == 0) { fastusermessage_return.BackColor = System.Drawing.Color.LightGreen; }
            else { fastusermessage_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void getfastusermessage_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int field = int.Parse(field_getfastusermessage.Text);
            string text = text_getfastusermessage.Text;
            err = scomm.CS_GetFastASCIIUsermessage(p, field, ref text);
            text_getfastusermessage.Text = text;

            getfastusermessage_return.Text = err.ToString();
            if (err == 0) { getfastusermessage_return.BackColor = System.Drawing.Color.LightGreen; }
            else { getfastusermessage_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void fastutf8usermessage_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int field = int.Parse(field_fastutf8usermessage.Text);
            string text = text_fastutf8usermessage.Text;
            err = scomm.CS_FastASCIIUsermessage(p, field, text);

            fastutf8usermessage_return.Text = err.ToString();
            if (err == 0) { fastutf8usermessage_return.BackColor = System.Drawing.Color.LightGreen; }
            else { fastutf8usermessage_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void getfastutf8usermessage_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int field = int.Parse(field_getfastutf8usermessage.Text);
            string text = text_getfastutf8usermessage.Text;
            err = scomm.CS_GetFastASCIIUsermessage(p, field, ref text);
            text_getfastutf8usermessage.Text = text;

            getfastutf8usermessage_return.Text = err.ToString();
            if (err == 0) { getfastutf8usermessage_return.BackColor = System.Drawing.Color.LightGreen; }
            else { getfastutf8usermessage_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void enablebufferedum_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int get = cb_get_enablebufferedum.SelectedIndex;
            int actsize = int.Parse(actsize_enablebufferedum.Text);
            int field = int.Parse(field_enablebufferedum.Text);
            int fillstatus = int.Parse(fillstatus_enablebufferedum.Text);
            int defsize = int.Parse(defsize_enablebufferedum.Text);
            
            err = scomm.CS_EnableBufferedUMExt(p, get, ref actsize, ref field, ref fillstatus, defsize);
            actsize_enablebufferedum.Text = actsize.ToString();
            field_enablebufferedum.Text = field.ToString();
            fillstatus_enablebufferedum.Text = fillstatus.ToString();

            enablebufferedum_return.Text = err.ToString();
            if (err == 0) { enablebufferedum_return.BackColor = System.Drawing.Color.LightGreen; }
            else { enablebufferedum_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }

        //DataString
        private void enablebuffereddatastring_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int get = cb_get_enablebuffereddatastring.SelectedIndex;
            int actsize = int.Parse(actsize_enablebuffereddatastring.Text);
            int field = int.Parse(field_enablebuffereddatastring.Text);
            int fillstatus = int.Parse(fillstatus_enablebuffereddatastring.Text);
            int defsize = int.Parse(defsize_enablebuffereddatastring.Text);

            err = scomm.CS_EnableBufferedDataString(p, get, ref actsize, ref field, ref fillstatus, defsize);
            actsize_enablebuffereddatastring.Text = actsize.ToString();
            field_enablebuffereddatastring.Text = field.ToString();
            fillstatus_enablebuffereddatastring.Text = fillstatus.ToString();

            enablebuffereddatastring_return.Text = err.ToString();
            if (err == 0) { enablebuffereddatastring_return.BackColor = System.Drawing.Color.LightGreen; }
            else { enablebuffereddatastring_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void fastdatastring_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int field = int.Parse(field_fastdatastring.Text);
            string buf_s = buf_fastdatastring.Text;
            len_fastdatastring.Text = buf_s.Length.ToString();
            byte[] buf = Encoding.ASCII.GetBytes(buf_s);
            int len = int.Parse(len_fastdatastring.Text);
            err = scomm.CS_FastDataString(p, field, buf, len);
   
            fastdatastring_return.Text = err.ToString();
            if (err == 0) { fastdatastring_return.BackColor = System.Drawing.Color.LightGreen; }
            else { fastdatastring_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void getfastdatastring_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int field = int.Parse(field_getfastdatastring.Text);
            byte[] buf = Encoding.ASCII.GetBytes("");
            int len = int.Parse(len_getfastdatastring.Text);
            err = scomm.CS_GetFastDataString(p, field, ref buf, ref len);
            buf_getfastdatastring.Text = Encoding.ASCII.GetString(buf);
            len_getfastdatastring.Text = len.ToString();

            getfastdatastring_return.Text = err.ToString();
            if (err == 0) { getfastdatastring_return.BackColor = System.Drawing.Color.LightGreen; }
            else { getfastdatastring_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }

        //MultipleUserMessage
        private void multipleusermessage_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            byte[] inbuf = Encoding.ASCII.GetBytes(inbuf_multipleusermessage.Text.ToString());
            inlen_multipleusermessage.Text = inbuf.Length.ToString();
            int inlen = inbuf.Length;
            byte[] outbuf = Encoding.ASCII.GetBytes(outbuf_multipleusermessage.Text.ToString());
            outlen_multipleusermessage.Text = outbuf.Length.ToString();
            int outlen = outbuf.Length;
            int fields = 0;
            err = scomm.CS_MultipleUsermessage(p, inbuf, inlen, outbuf, ref outlen, ref fields);
            outlen_multipleusermessage.Text = outlen.ToString();
            fields_multipleusermessage.Text = fields.ToString();

            multipleusermessage_return.Text = err.ToString();
            if (err == 0) { multipleusermessage_return.BackColor = System.Drawing.Color.LightGreen; }
            else { multipleusermessage_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void multipleutf8usermessage_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            byte[] inbuf = Encoding.UTF8.GetBytes(inbuf_multipleutf8usermessage.Text.ToString());
            inlen_multipleutf8usermessage.Text = inbuf.Length.ToString();
            int inlen = inbuf.Length;
            byte[] outbuf = Encoding.UTF8.GetBytes(outbuf_multipleutf8usermessage.Text.ToString());
            outlen_multipleutf8usermessage.Text = outbuf.Length.ToString();
            int outlen = outbuf.Length;
            int fields = 0;
            err = scomm.CS_MultipleUTF8Usermessage(p, inbuf, inlen, outbuf, ref outlen, ref fields);
            outlen_multipleutf8usermessage.Text = outlen.ToString();
            fields_multipleutf8usermessage.Text = fields.ToString();

            multipleutf8usermessage_return.Text = err.ToString();
            if (err == 0) { multipleutf8usermessage_return.BackColor = System.Drawing.Color.LightGreen; }
            else { multipleutf8usermessage_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void getmultipleusermessage_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            byte[] inbuf = Encoding.ASCII.GetBytes(infub_getmultipleusermessage.Text.ToString());
            inlen_getmultipleusermessage.Text = inbuf.Length.ToString();
            int inlen = inbuf.Length;
            byte[] outbuf = Encoding.ASCII.GetBytes(outbuf_getmultipleusermessage.Text.ToString());
            outlen_getmultipleusermessage.Text = outbuf.Length.ToString();
            int outlen = outbuf.Length;
            err = scomm.CS_GetMultipleUsermessage(p, inbuf, inlen, outbuf, ref outlen);
            outlen_getmultipleusermessage.Text = outlen.ToString();

            getmultipleusermessage_return.Text = err.ToString();
            if (err == 0) { getmultipleusermessage_return.BackColor = System.Drawing.Color.LightGreen; }
            else { getmultipleusermessage_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void getmultipleutf8usermessage_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            byte[] inbuf = Encoding.UTF8.GetBytes(inbuf_getmultipleutf8usermessage.Text.ToString());
            inlen_getmultipleutf8usermessage.Text = inbuf.Length.ToString();
            int inlen = inbuf.Length;
            byte[] outbuf = Encoding.UTF8.GetBytes(outbuf_getmultipleutf8usermessage.Text.ToString());
            outlen_getmultipleutf8usermessage.Text = outbuf.Length.ToString();
            int outlen = outbuf.Length;
            err = scomm.CS_GetMultipleUTF8Usermessage(p, inbuf, inlen, outbuf, ref outlen);

            getmultipleutf8usermessage_return.Text = err.ToString();
            if (err == 0) { getmultipleutf8usermessage_return.BackColor = System.Drawing.Color.LightGreen; }
            else { getmultipleutf8usermessage_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }

        //Fifo
        private void getfifofield_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int field = int.Parse(field_getfifofield.Text);
            int index = int.Parse(index_getfifofield.Text);
            string txt = txt_getfifofield.Text.ToString();
            int elements = int.Parse(elements_getfifofield.Text);
            err = scomm.CS_GetFifofield(p, field, index, ref txt, ref elements);
            txt_getfifofield.Text = txt;
            elements_getfifofield.Text = elements.ToString();

            getfifofield_return.Text = err.ToString();
            if (err == 0) { getfifofield_return.BackColor = System.Drawing.Color.LightGreen; }
            else { getfifofield_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void getutf8fifofield_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int field = int.Parse(field_getutf8fifofield.Text);
            int index = int.Parse(index_getutf8fifofield.Text);
            string txt = txt_getutf8fifofield.Text.ToString();
            int elements = int.Parse(elements_getutf8fifofield.Text);
            err = scomm.CS_GetUTF8Fifofield(p, field, index, ref txt, ref elements);
            txt_getutf8fifofield.Text = txt;
            elements_getutf8fifofield.Text = elements.ToString();

            getutf8fifofield_return.Text = err.ToString();
            if (err == 0) { getutf8fifofield_return.BackColor = System.Drawing.Color.LightGreen; }
            else { getutf8fifofield_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void fifodump_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            err = scomm.CS_FifoDump(p);

            fifodump_return.Text = err.ToString();
            if (err == 0) { fifodump_return.BackColor = System.Drawing.Color.LightGreen; }
            else { fifodump_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }

        //Others
        private void testpointer_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int on = cb_on_testpointer.SelectedIndex;
            err = scomm.CS_TestPointer(p, on);

            testpointer_return.Text = err.ToString();
            if (err == 0) { testpointer_return.BackColor = System.Drawing.Color.LightGreen; }
            else { testpointer_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void powerscale_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int set = cb_set_powerscale.SelectedIndex;
            int member = cb_member_powerscale.SelectedIndex;
            int value = int.Parse(value_powerscale.Text);
            err = scomm.CS_Powerscale(p, set, member, ref value);
            value_powerscale.Text = value.ToString();

            powerscale_return.Text = err.ToString();
            if (err == 0) { powerscale_return.BackColor = System.Drawing.Color.LightGreen; }
            else { powerscale_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void eventhandler_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string name = name_eventhandler.Text;
            err = scomm.CS_Eventhandler(p, name);

            eventhandler_return.Text = err.ToString();
            if (err == 0) { eventhandler_return.BackColor = System.Drawing.Color.LightGreen; }
            else { eventhandler_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void table_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            err = scomm.CS_MTable(p);

            table_return.Text = err.ToString();
            if (err == 0) { table_return.BackColor = System.Drawing.Color.LightGreen; }
            else { table_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void dumpsvg_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string name = name_dumpsvg.Text;
            err = scomm.CS_DumpSVG(p, name);

            dumpsvg_return.Text = err.ToString();
            if (err == 0) { dumpsvg_return.BackColor = System.Drawing.Color.LightGreen; }
            else { dumpsvg_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void sysinfo_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            SocketCommNet.SocketComm.CSSysinfo info = new SocketCommNet.SocketComm.CSSysinfo();
            err = scomm.CS_Sysinfo(p, ref info);

            cputemp_sysinfo.Text = info.cputemp.ToString();
            size0_sysinfo.Text = info.size0.ToString();
            avail0_sysinfo.Text = info.avail0.ToString();
            size1_sysinfo.Text = info.size1.ToString();
            avail1_sysinfo.Text = info.avail1.ToString();
            size2_sysinfo.Text = info.size2.ToString();

            avail2_sysinfo.Text = info.avail2.ToString();
            size3_sysinfo.Text = info.size3.ToString();
            avail3_sysinfo.Text = info.avail3.ToString();
            hours_sysinfo.Text = info.hours.ToString();
            longcounter_sysinfo.Text = info.longcounter.ToString();

            sysinfo_return.Text = err.ToString();
            if (err == 0) { sysinfo_return.BackColor = System.Drawing.Color.LightGreen; }
            else { sysinfo_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void coretemp_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            SocketCommNet.SocketComm.CSCoretemp coretemp = new SocketCommNet.SocketComm.CSCoretemp();
            err = scomm.CS_Coretemp(p, ref coretemp);

            cputemp_coretemp.Text = coretemp.cputemp.ToString();
            boardtemp_coretemp.Text = coretemp.boardtemp.ToString();
            humidity_coretemp.Text = coretemp.humidity.ToString();
            voltage1_coretemp.Text = coretemp.voltage1.ToString();
            voltage2_coretemp.Text = coretemp.voltage2.ToString();

            fanlocaltemp_coretemp.Text = coretemp.fanlocaltemp.ToString();
            fancurrentpwm_coretemp.Text = coretemp.fanlocaltemp.ToString();
            fantacho_coretemp.Text = coretemp.fantacho.ToString();
            fanremotetemp_coretemp.Text = coretemp.fanremotetemp.ToString();

            coretemp_return.Text = err.ToString();
            if (err == 0) { coretemp_return.BackColor = System.Drawing.Color.LightGreen; }
            else { coretemp_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void defocus_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int dz = int.Parse(dz_defocus.Text);
            int relative = int.Parse(relative_defocus.Text);
            int format = int.Parse(format_defocus.Text);
            err = scomm.CS_Defocus(p, ref dz, relative, format);
            dz_defocus.Text = dz.ToString();

            defocus_return.Text = err.ToString();
            if (err == 0) { defocus_return.BackColor = System.Drawing.Color.LightGreen; }
            else { defocus_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }


        //Close/Version
        private void knockout_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            err = scomm.CS_Knockout(p);

            knockout_return.Text = err.ToString();
            if (err == 0) { knockout_return.BackColor = System.Drawing.Color.LightGreen; }
            else { knockout_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void finish_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            scomm.CS_Finish(p);
            Cursor.Current = Cursors.Default;
        }
        private void getdllversion_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            err = scomm.CS_GetDllVersion();

            getdllversion_return.Text = err.ToString();
            if (err > 0) { getdllversion_return.BackColor = System.Drawing.Color.LightGreen; } //v>0 ok
            else { getdllversion_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;
        }
        private void getversion_b_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            err = scomm.CS_GetVersion(p);

            getversion_return.Text = err.ToString();
            if (err > 0 && err != 0xFFFF) { getversion_return.BackColor = System.Drawing.Color.LightGreen; } //v>0 ok
            else { getversion_return.BackColor = System.Drawing.Color.Red; }
            Cursor.Current = Cursors.Default;

        }
        //F LASER FUNCTIONS -------------------------------------------------------------------------------

        private void init_p_LostFocus(object sender, EventArgs e)
        {
            if ((init_p.Text.Length > 0) && int.TryParse(init_p.Text, out int a))
            {
                if (a >= 0 && a <= 48)
                {
                    p = a;
                    updatePvalues(p);
                }
                else
                {
                    init_p.Text = "0";
                }
            }
            else
            {
                init_p.Text = "0";
            }
        }

        private void ip0_LostFocus(object sender, EventArgs e)
        {
            if ((ip0.Text.Length >= 0) && (ip0.Text.Length <= 3) && (int.TryParse(ip0.Text, out int n)))
            {
                if (int.Parse(ip0.Text) < 0 || int.Parse(ip0.Text) > 255)
                {
                    ip0.Text = "0";
                }
            }
            else
            {
                ip0.Text = "0";
            }
        }

        private void ip1_LostFocus(object sender, EventArgs e)
        {
            if ((ip1.Text.Length > 0) && (ip1.Text.Length <= 3) && (int.TryParse(ip1.Text, out int n)))
            {
                if (int.Parse(ip1.Text) < 0 || int.Parse(ip1.Text) > 255)
                {
                    ip1.Text = "0";
                }
            }
            else
            {
                ip1.Text = "0";
            }

        }

        private void ip2_LostFocus(object sender, EventArgs e)
        {
            if ((ip2.Text.Length > 0) && (ip2.Text.Length <= 3) && (int.TryParse(ip2.Text, out int n)))
            {
                if (int.Parse(ip2.Text) < 0 || int.Parse(ip2.Text) > 255)
                {
                    ip2.Text = "0";
                }
            }
            else
            {
                ip2.Text = "0";
            }

        }

        private void ip3_LostFocus(object sender, EventArgs e)
        {
            if ((ip3.Text.Length > 0) && (ip3.Text.Length <= 3) && (int.TryParse(ip3.Text, out int n)))
            {
                if (int.Parse(ip3.Text) < 0 || int.Parse(ip3.Text) > 255)
                {
                    ip3.Text = "0";
                }
            }
            else
            {
                ip3.Text = "0";
            }

        }

        private void value_setdynamic_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(value_setdynamic.Text, out int n) && int.Parse(value_setdynamic.Text) >= 0) { }
            else
            {
                value_setdynamic.Text = "0";
            }
        }

        private void ignorealarms_startprintsession_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(ignorealarms_startprintsession.Text, out int n) && int.Parse(ignorealarms_startprintsession.Text) >= 0) { }
            else
            {
                ignorealarms_startprintsession.Text = "0";
            }
        }

        private void nr_start_LostFocus(object sender, EventArgs e) {
            if (int.TryParse(nr_start.Text, out int n) && int.Parse(nr_start.Text) >= 0) { }
            else
            {
                nr_start.Text = "0";
            }
        }

        private void nr_startextended_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(nr_startextended.Text, out int n) && int.Parse(nr_startextended.Text) >= 0) { }
            else
            {
                nr_startextended.Text = "0";
            }
        }

        private void msg_startextended_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(msg_startextended.Text, out int n) && int.Parse(msg_startextended.Text) >= 0)
            {
                if (n < 0 || n > 255)
                {
                    msg_startextended.Text = "0";
                }
            }
            else
            {
                msg_startextended.Text = "0";
            }
        }

        private void batch_startextended_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(batch_startextended.Text, out int n) && int.Parse(batch_startextended.Text) >= 0) { }
            else
            {
                batch_startextended.Text = "0";
            }
        }

        private void field_setglobalcounter_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(field_setglobalcounter.Text, out int n) && int.Parse(field_setglobalcounter.Text) >= 0) { }
            else
            {
                field_setglobalcounter.Text = "0";
            }
        }

        private void counter_setglobalcounter_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(counter_setglobalcounter.Text, out int n) && int.Parse(counter_setglobalcounter.Text) >= 0) { }
            else
            {
                counter_setglobalcounter.Text = "0";
            }
        }

        private void field_getglobalcounter_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(field_getglobalcounter.Text, out int n) && int.Parse(field_getglobalcounter.Text) >= 0) { }
            else
            {
                field_getglobalcounter.Text = "0";
            }
        }

        private void field_setprivatecounter_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(field_setprivatecounter.Text, out int n) && int.Parse(field_setprivatecounter.Text) >= 0) { }
            else
            {
                field_setprivatecounter.Text = "0";
            }
        }

        private void repeats_setprivatecounter_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(repeats_setprivatecounter.Text, out int n) && int.Parse(repeats_setprivatecounter.Text) >= 0) { }
            else
            {
                repeats_setprivatecounter.Text = "0";
            }
        }

        private void prints_setprivatecounter_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(prints_setprivatecounter.Text, out int n) && int.Parse(prints_setprivatecounter.Text) >= 0) { }
            else
            {
                prints_setprivatecounter.Text = "0";
            }
        }

        private void dx_offset_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(dx_offset.Text, out int n) && int.Parse(dx_offset.Text) >= 0) { }
            else
            {
                dx_offset.Text = "0";
            }
        }

        private void dy_offset_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(dy_offset.Text, out int n) && int.Parse(dy_offset.Text) >= 0) { }
            else
            {
                dy_offset.Text = "0";
            }
        }

        private void dx_shiftrotate_LostFocus(object sender, EventArgs e)
        {
            string strc = dx_shiftrotate.Text.ToString().Replace(".",",");
            dx_shiftrotate.Text = strc;
            if (float.TryParse(dx_shiftrotate.Text, out float n) && float.Parse(dx_shiftrotate.Text) >= 0) { }
            else
            {
                dx_shiftrotate.Text = "0,0";
            }
        }

        private void dy_shiftrotate_LostFocus(object sender, EventArgs e)
        {
            string strc = dy_shiftrotate.Text.ToString().Replace(".", ",");
            dy_shiftrotate.Text = strc;
            if (float.TryParse(dy_shiftrotate.Text, out float n) && float.Parse(dy_shiftrotate.Text) >= 0) { }
            else
            {
                dy_shiftrotate.Text = "0,0";
            }
        }

        private void angle_shiftrotate_LostFocus(object sender, EventArgs e)
        {
            string strc = angle_shiftrotate.Text.ToString().Replace(".", ",");
            angle_shiftrotate.Text = strc;
            if (float.TryParse(angle_shiftrotate.Text, out float n) && float.Parse(angle_shiftrotate.Text) >= 0) { }
            else
            {
                angle_shiftrotate.Text = "0,0";
            }
        }

        private void x0_shiftrotate_LostFocus(object sender, EventArgs e)
        {
            string strc = x0_shiftrotate.Text.ToString().Replace(".", ",");
            x0_shiftrotate.Text = strc;
            if (float.TryParse(x0_shiftrotate.Text, out float n) && float.Parse(x0_shiftrotate.Text) >= 0) { }
            else
            {
                x0_shiftrotate.Text = "0,0";
            }
        }

        private void y0_shiftrotate_LostFocus(object sender, EventArgs e)
        {
            string strc = y0_shiftrotate.Text.ToString().Replace(".", ",");
            y0_shiftrotate.Text = strc;
            if (float.TryParse(y0_shiftrotate.Text, out float n) && float.Parse(y0_shiftrotate.Text) >= 0) { }
            else
            {
                y0_shiftrotate.Text = "0,0";
            }
        }

        private void field_fastusermessage_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(field_fastusermessage.Text, out int n) && int.Parse(field_fastusermessage.Text) >= 0) { }
            else
            {
                field_fastusermessage.Text = "0";
            }
        }

        private void field_getfastusermessage_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(field_getfastusermessage.Text, out int n) && int.Parse(field_getfastusermessage.Text) >= 0) { }
            else
            {
                field_getfastusermessage.Text = "0";
            }
        }

        private void field_fastutf8usermessage_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(field_fastutf8usermessage.Text, out int n) && int.Parse(field_fastutf8usermessage.Text) >= 0) { }
            else
            {
                field_fastutf8usermessage.Text = "0";
            }
        }

        private void field_getfastutf8usermessage_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(field_getfastutf8usermessage.Text, out int n) && int.Parse(field_getfastutf8usermessage.Text) >= 0) { }
            else
            {
                field_getfastutf8usermessage.Text = "0";
            }
        }

        private void field_enablebufferedum_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(field_enablebufferedum.Text, out int n) && int.Parse(field_enablebufferedum.Text) >= 0) { }
            else
            {
                field_enablebufferedum.Text = "0";
            }
        }

        private void defsize_enablebufferedum_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(defsize_enablebufferedum.Text, out int n) && int.Parse(defsize_enablebufferedum.Text) >= 0 && int.Parse(defsize_enablebufferedum.Text) <= 1000) { }
            else
            {
                defsize_enablebufferedum.Text = "0";
            }
        }

        private void field_enablebuffereddatastring_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(field_enablebuffereddatastring.Text, out int n) && int.Parse(field_enablebuffereddatastring.Text) >= 0) { }
            else
            {
                field_enablebuffereddatastring.Text = "0";
            }
        }

        private void defsize_enablebuffereddatastring_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(defsize_enablebuffereddatastring.Text, out int n) && int.Parse(defsize_enablebuffereddatastring.Text) >= 0 && int.Parse(defsize_enablebuffereddatastring.Text) <= 1000) { }
            else
            {
                defsize_enablebuffereddatastring.Text = "0";
            }
        }

        private void field_fastdatastring_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(field_fastdatastring.Text, out int n) && int.Parse(field_fastdatastring.Text) >= 0) { }
            else
            {
                field_fastdatastring.Text = "0";
            }
        }

        private void field_getfastdatastring_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(field_getfastdatastring.Text, out int n) && int.Parse(field_getfastdatastring.Text) >= 0) { }
            else
            {
                field_getfastdatastring.Text = "0";
            }
        }

        private void frame_getfilenames_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(frame_getfilenames.Text, out int n) && int.Parse(frame_getfilenames.Text) >= 0)
            {
                if (n < 0 || n > 255)
                {
                    frame_getfilenames.Text = "0";
                }
            }
            else
            {
                frame_getfilenames.Text = "0";
            }
        }

        private void timeout_stop_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(timeout_stop.Text, out int n) && int.Parse(timeout_stop.Text) >= 0) { }
            else
            {
                timeout_stop.Text = "0";
            }
        }

        private void field_getfifofield_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(field_getfifofield.Text, out int n) && int.Parse(field_getfifofield.Text) >= 0) { }
            else
            {
                field_getfifofield.Text = "0";
            }
        }

        private void index_getfifofield_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(index_getfifofield.Text, out int n) && int.Parse(index_getfifofield.Text) >= 0) { }
            else
            {
                index_getfifofield.Text = "0";
            }
        }

        private void field_getutf8fifofield_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(field_getutf8fifofield.Text, out int n) && int.Parse(field_getutf8fifofield.Text) >= 0) { }
            else
            {
                field_getutf8fifofield.Text = "0";
            }
        }

        private void index_getutf8fifofield_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(index_getutf8fifofield.Text, out int n) && int.Parse(index_getutf8fifofield.Text) >= 0) { }
            else
            {
                index_getutf8fifofield.Text = "0";
            }
        }

        private void value_powerscale_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(value_powerscale.Text, out int n) && int.Parse(value_powerscale.Text) >= 0) { }
            else
            {
                value_powerscale.Text = "0";
            }
        }

        private void dz_defocus_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(dz_defocus.Text, out int n) && int.Parse(dz_defocus.Text) >= 0) { }
            else
            {
                dz_defocus.Text = "0";
            }
        }

        private void relative_defocus_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(relative_defocus.Text, out int n) && int.Parse(relative_defocus.Text) >= 0) { }
            else
            {
                relative_defocus.Text = "0";
            }
        }

        private void format_defocus_LostFocus(object sender, EventArgs e)
        {
            if (int.TryParse(format_defocus.Text, out int n) && int.Parse(format_defocus.Text) >= 0) { }
            else
            {
                format_defocus.Text = "0";
            }
        }

        //SHOW INFORMATION OF THE CLICKED FUNCTION
        private void rb_info_init_Click(object sender, EventArgs e)
        {                  
            if (num_thread > 0) { close_info_forms(); }          
            if (num_thread < max_thread)
            {  
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;                      
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_init, "Init", Get_current_inf_pos());                      
                    }
                    finally
                    {                  
                        ThreadCompleted();
                    }        
                    });
                t.Start();
            }          
        }      

        private void rb_info_startclient_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_startclient, "StartClient", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }
        }

        private void rb_info_isconnected_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_isconnected, "IsConnected", Get_current_inf_pos());
                    }
                    finally
                    {                
                        ThreadCompleted();
                    }
                });
                t.Start();
            }     
        }

        private void rb_info_getlasterror_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_getlasterror, "GetLastError", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }
        }

        private void rb_info_knockout_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_knockout, "Knockout", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }     
        }

        private void rb_info_finish_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_finish, "Finish", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }     
        }

        private void rb_info_getdllversion_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_getdllversion, "GetDllVersion", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }      
        }

        private void rb_info_getversion_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_getversion, "GetVersion", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }         
        }

        private void rb_info_statusext_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_statusext, "StatusExt", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }      
        }

        private void rb_info_mode_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_mode, "Mode", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }         
        }

        private void rb_info_setdynamic_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_setdynamic, "SetDynamic", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }           
        }

        private void rb_info_getdynamic_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_getdynamic, "GetDynamic", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }   
        }

        private void rb_info_getfilenames_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_getfilenames, "GetFilenames", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }   
        }

        private void rb_info_copyfile_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_copyfile, "CopyFile", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }        
        }

        private void rb_info_delete_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_delete, "Delete", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }         
        }

        private void rb_info_asciiconfig_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_asciiconfig, "AsciiConfig", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }
        }

        private void rb_info_setdefault_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_setdefault, "SetDefault", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }      
        }

        private void rb_info_startprintsession_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_startprintsession, "StartPrintSession", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }      
        }

        private void rb_info_start_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_start, "Start", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }         
        }

        private void rb_info_stop_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_stop, "Stop", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }           
        }

        private void rb_info_triggerprint_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_triggerprint, "TriggerPrint", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }         
        }

        private void rb_info_startextended_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_startextended, "StartExtended", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }     
        }

        private void rb_info_reload_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_reload, "Reload", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }       
        }

        private void rb_info_endprintsession_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_endprintsession, "EndPrintSession", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }         
        }

        private void rb_info_offset_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_offset, "Offset", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }       
        }

        private void rb_info_shiftrotate_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_shiftrotate, "ShiftRotate", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }           
        }

        private void rb_info_counterreset_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_counterreset, "CounterReset", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }           
        }

        private void rb_info_settime_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_settime, "SetTime", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }          
        }

        private void rb_info_setglobalcounter_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_setglobalcounter, "SetGlobalCounter", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }          
        }

        private void rb_info_getglobalcounter_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_getglobalcounter, "GetGlobalCounter", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }     
        }

        private void rb_info_setprivatecounter_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_setprivatecounter, "SetPrivateCounter", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }        
        }

        private void rb_info_store_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_store, "Store", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }       
        }

        private void rb_info_fastusermessage_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_fastasciiusermessage, "FastASCIIUsermessage", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }      
        }

        private void rb_info_getfastusermessage_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_getfastusermessage, "GetFastUsermessage", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }   
        }

        private void rb_info_fastutf8usermessage_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_fastutf8usermessage, "FastUTF8Usermessage", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }      
        }

        private void rb_info_getfastutf8usermessage_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_getfastutf8usermessage, "GetFastUTF8Usermessage", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }       
        }

        private void rb_info_enablebufferedumext_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_enablebufferedumext, "EnableBufferedUMExt", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }          
        }

        private void rb_info_enablebuffereddatastring_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_enablebuffereddatastring, "EnableBufferedDataString", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }      
        }

        private void rb_info_fastdatastring_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_fastdatastring, "FastDataString", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }           
        }

        private void rb_info_getfastdatastring_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_getfastdatastring, "GetFastDataString", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }         
        }

        private void rb_info_multipleusermessage_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_multipleusermessage, "MultipleUsermessage", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }      
        }

        private void rb_info_multipleutf8usermessage_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_multipleutf8usermessage, "MultipleUTF8Usermessage", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }    
        }

        private void rb_info_getmultipleusermessage_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_getmultipleusermessage, "GetMultipleUsermessage", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }            
        }

        private void rb_info_getmultipleutf8usermessage_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_getmultipleutf8usermessage, "GetMultipleUTF8Usermessage", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }        
        }

        private void rb_info_getfifofield_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_getfifofield, "GetFifofield", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }        
        }

        private void rb_info_getutf8fifofield_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_getutf8fifofield, "GetUTF8Fifofield", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }    
        }

        private void rb_info_fifodump_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() => {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_fifodump, "FifoDump", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }       
        }

        private void rb_info_testpointer_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() =>
                {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_testpointer, "TestPointer", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }          
        }

        private void rb_info_powerscale_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() =>
                {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_powerscale, "PowerScale", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }       
        }

        private void rb_info_eventhandler_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() =>
                {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_eventhandler, "Eventhandler", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }     
        }

        private void rb_info_mtable_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() =>
                {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_mtable, "MTable", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }          
        }

        private void rb_info_dumpsvg_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() =>
                {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_dumpsvg, "DumpSVG", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }
        }

        private void rb_info_sysinfo_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() =>
                {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_sysinfo, "Sysinfo", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }      
        }

        private void rb_info_coretemp_Click(object sender, EventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); }
            if (num_thread < max_thread)
            {
                Thread t = new Thread(() =>
                {
                    try
                    {
                        num_thread++;
                        MyInfoBox.ShowInfImage(SocketCommDllTest.Properties.Resources.inf_coretemp, "Coretemp", Get_current_inf_pos());
                    }
                    finally
                    {
                        ThreadCompleted();
                    }
                });
                t.Start();
            }        
        }

        private void rb_info_defocus_Click(object sender, EventArgs e)
        {

        }
        //F_SHOW INFORMATION OF THE CLICKED FUNCTION


        //Other functions
        private int max_thread = 1;
        private int num_thread = 0;
        private void ThreadCompleted()
        {
            Console.WriteLine("Thread completed n = " + num_thread.ToString());
        }

        private void close_info_forms()
        {
            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                if (Application.OpenForms[i].Name != "MainW")
                {
                    Invoke(new MethodInvoker(() => { Application.OpenForms[i].Close(); }));
                }
            }
            num_thread--;
        }
        private void updatePvalues(int p)
        {
            p_getlasterror.Text = p.ToString();
            p_startclient.Text = p.ToString();
            p_statusext.Text = p.ToString();            
            p_isconnected.Text = p.ToString();
            p_laser_mode.Text = p.ToString();
            p_setdynamic.Text = p.ToString();
            p_getdynamic.Text = p.ToString();
            p_startprintsession.Text = p.ToString();
            p_getfilenames.Text = p.ToString();
            p_copyfile.Text = p.ToString();
            p_delete.Text = p.ToString();
            p_asciiconfig.Text = p.ToString();
            p_setdefault.Text = p.ToString();
            p_startprintsession.Text = p.ToString();
            p_start.Text = p.ToString();
            p_startextended.Text = p.ToString();
            p_reload.Text = p.ToString();
            p_stop.Text = p.ToString();
            p_triggerprint.Text = p.ToString();
            p_endprintsession.Text = p.ToString();
            p_counterreset.Text = p.ToString();
            p_settime.Text = p.ToString();
            p_setglobalcounter.Text = p.ToString();
            p_getglobalcounter.Text = p.ToString();
            p_setprivatecounter.Text = p.ToString();
            p_offset.Text = p.ToString();
            p_shiftrotate.Text = p.ToString();
            p_store.Text = p.ToString();
            p_fastusermessage.Text = p.ToString();
            p_getfastusermessage.Text = p.ToString();
            p_fastutf8usermessage.Text = p.ToString();
            p_getfastutf8usermessage.Text = p.ToString();
            p_enablebufferedum.Text = p.ToString();
            p_enablebuffereddatastring.Text = p.ToString();
            p_fastdatastring.Text = p.ToString();
            p_getfastdatastring.Text = p.ToString();
            p_multipleusermessage.Text = p.ToString();
            p_multipleutf8usermessage.Text = p.ToString();
            p_getmultipleusermessage.Text = p.ToString();
            p_getmultipleutf8usermessage.Text = p.ToString();
            p_getfifofield.Text = p.ToString();
            p_getutf8fifofield.Text = p.ToString();
            p_fifodump.Text = p.ToString();
            p_knockout.Text = p.ToString();
            p_finish.Text = p.ToString();

            p_testpointer.Text = p.ToString();
            p_powerscale.Text = p.ToString();
            p_eventhandler.Text = p.ToString();
            p_table.Text = p.ToString();
            p_dumpsvg.Text = p.ToString();
            p_sysinfo.Text = p.ToString();
            p_coretemp.Text = p.ToString();
            p_defocus.Text = p.ToString();
            
        }

        private void statusextTV1_Click(object sender, EventArgs e)
        {
            statusextTV1.ExpandAll();
        }
        private void statusextTV2_Click(object sender, EventArgs e)
        {
            statusextTV2.ExpandAll();
        }
        private void coretempTV1_Click(object sender, EventArgs e)
        {
            coretempTV1.ExpandAll();
        }
        private void coretempTV2_Click(object sender, EventArgs e)
        {
            coretempTV2.ExpandAll(); 
        }
        private void sysinfoTV1_Click(object sender, EventArgs e)
        {
            sysinfoTV1.ExpandAll();
        }
        private void sysinfoTV2_Click(object sender, EventArgs e)
        {
            sysinfoTV2.ExpandAll();
        }

        private void close_current_connection()
        {
            Cursor.Current = Cursors.WaitCursor;
            err = scomm.CS_Knockout(p);
            knockout_return.Text = err.ToString();
            scomm.CS_Finish(p);
            Cursor.Current = Cursors.Default;
        }

        private void MainW_FormClosing(Object sender, FormClosingEventArgs e)
        {
            if (num_thread > 0) { close_info_forms(); } //close all info forms when the main form is closed
            close_current_connection();
            this.Dispose(true);
        }

        private void search_copyfile_b_Click(object sender, EventArgs e)
        {
            folderBrowser = new OpenFileDialog();
            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            folderBrowser.InitialDirectory = path_copyfile.Text;     //"C:\\Users";
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                path_copyfile.Text = Path.GetDirectoryName(folderBrowser.FileName);
                filename_copyfile.Text = folderBrowser.SafeFileName;
            }
        }

        private Point Get_current_inf_pos()
        {
            Invoke(new MethodInvoker(() => { screen_inf_p = PointToScreen(new Point(tabControl1.Right, tabControl1.Top)); }));
            return screen_inf_p;
        }

        //---------------------


    }
}
