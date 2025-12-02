using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Speedway.Mach1;

namespace SpeedwayReaderParamControl;

public class SpeedwayReaderParams : UserControl
{
	private IContainer components;

	private ComboBox cbRegion;

	private Label label2;

	private Label label1;

	private TextBox tbReaderName;

	private ComboBox cbReaderMode;

	private Label label4;

	private ComboBox cbSession;

	private Label label3;

	private Label label12;

	private Label label13;

	private TextBox tbRSSI2;

	private TextBox tbRSSI4;

	private CheckBox chAnt1;

	private TextBox tbPower4;

	private CheckBox chAnt2;

	private TextBox tbPower3;

	private CheckBox chAnt3;

	private TextBox tbRSSI3;

	private CheckBox chAnt4;

	private TextBox tbPower1;

	private TextBox tbRSSI1;

	private TextBox tbPower2;

	private TextBox textBox9;

	private ComboBox cbFrequencyMode;

	private Label label5;

	private CheckBox chMaxSens;

	private ComboBox cbLBTMode;

	private Label label7;

	private Label label6;

	private TextBox textBox2;

	private TabControl tabControl1;

	private TabPage tpReader;

	private TabPage tpAntenna;

	private TabPage tpGen2;

	private Label label9;

	private Label label10;

	private ComboBox cbFrequencyList;

	private TabPage tabPage1;

	private PictureBox pictureBox1;

	private LinkLabel llRFID;

	private Label label14;

	private Label label11;

	private Label lbFPGA;

	private Label lbFirmware;

	private Label lbSoftware;

	private Label label8;

	private Label label16;

	private ComboBox cbAutoSetMode;

	private ComboBox cbInventoryMode;

	private Label label15;

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpeedwayReaderParamControl.SpeedwayReaderParams));
		this.cbLBTMode = new System.Windows.Forms.ComboBox();
		this.cbFrequencyMode = new System.Windows.Forms.ComboBox();
		this.label7 = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.cbRegion = new System.Windows.Forms.ComboBox();
		this.label2 = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.tbReaderName = new System.Windows.Forms.TextBox();
		this.cbSession = new System.Windows.Forms.ComboBox();
		this.cbReaderMode = new System.Windows.Forms.ComboBox();
		this.label3 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.chMaxSens = new System.Windows.Forms.CheckBox();
		this.label12 = new System.Windows.Forms.Label();
		this.label13 = new System.Windows.Forms.Label();
		this.tbRSSI2 = new System.Windows.Forms.TextBox();
		this.tbRSSI4 = new System.Windows.Forms.TextBox();
		this.chAnt1 = new System.Windows.Forms.CheckBox();
		this.tbPower4 = new System.Windows.Forms.TextBox();
		this.chAnt2 = new System.Windows.Forms.CheckBox();
		this.tbPower3 = new System.Windows.Forms.TextBox();
		this.chAnt3 = new System.Windows.Forms.CheckBox();
		this.tbRSSI3 = new System.Windows.Forms.TextBox();
		this.chAnt4 = new System.Windows.Forms.CheckBox();
		this.tbPower1 = new System.Windows.Forms.TextBox();
		this.tbRSSI1 = new System.Windows.Forms.TextBox();
		this.tbPower2 = new System.Windows.Forms.TextBox();
		this.textBox9 = new System.Windows.Forms.TextBox();
		this.textBox2 = new System.Windows.Forms.TextBox();
		this.tabControl1 = new System.Windows.Forms.TabControl();
		this.tpReader = new System.Windows.Forms.TabPage();
		this.cbFrequencyList = new System.Windows.Forms.ComboBox();
		this.tpAntenna = new System.Windows.Forms.TabPage();
		this.label9 = new System.Windows.Forms.Label();
		this.label10 = new System.Windows.Forms.Label();
		this.tpGen2 = new System.Windows.Forms.TabPage();
		this.tabPage1 = new System.Windows.Forms.TabPage();
		this.label14 = new System.Windows.Forms.Label();
		this.label11 = new System.Windows.Forms.Label();
		this.lbFPGA = new System.Windows.Forms.Label();
		this.lbFirmware = new System.Windows.Forms.Label();
		this.lbSoftware = new System.Windows.Forms.Label();
		this.label8 = new System.Windows.Forms.Label();
		this.llRFID = new System.Windows.Forms.LinkLabel();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.label15 = new System.Windows.Forms.Label();
		this.cbAutoSetMode = new System.Windows.Forms.ComboBox();
		this.cbInventoryMode = new System.Windows.Forms.ComboBox();
		this.label16 = new System.Windows.Forms.Label();
		this.tabControl1.SuspendLayout();
		this.tpReader.SuspendLayout();
		this.tpAntenna.SuspendLayout();
		this.tpGen2.SuspendLayout();
		this.tabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		base.SuspendLayout();
		this.cbLBTMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbLBTMode.Enabled = false;
		this.cbLBTMode.FormattingEnabled = true;
		this.cbLBTMode.Items.AddRange(new object[2] { "0 - Auto Select", "1 - 4 Seconds" });
		this.cbLBTMode.Location = new System.Drawing.Point(85, 130);
		this.cbLBTMode.Name = "cbLBTMode";
		this.cbLBTMode.Size = new System.Drawing.Size(126, 21);
		this.cbLBTMode.TabIndex = 2;
		this.cbLBTMode.SelectedIndexChanged += new System.EventHandler(cbLBTMode_SelectedIndexChanged);
		this.cbFrequencyMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbFrequencyMode.Enabled = false;
		this.cbFrequencyMode.FormattingEnabled = true;
		this.cbFrequencyMode.Items.AddRange(new object[2] { "0 - Use Center Frequency", "1 - Frequency by Region (Automatic)" });
		this.cbFrequencyMode.Location = new System.Drawing.Point(85, 76);
		this.cbFrequencyMode.Name = "cbFrequencyMode";
		this.cbFrequencyMode.Size = new System.Drawing.Size(196, 21);
		this.cbFrequencyMode.TabIndex = 2;
		this.cbFrequencyMode.SelectedIndexChanged += new System.EventHandler(cbFrequencyMode_SelectedIndexChanged);
		this.label7.AutoSize = true;
		this.label7.Location = new System.Drawing.Point(27, 134);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(53, 13);
		this.label7.TabIndex = 1;
		this.label7.Text = "LBT Time";
		this.label6.AutoSize = true;
		this.label6.Location = new System.Drawing.Point(32, 107);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(50, 13);
		this.label6.TabIndex = 1;
		this.label6.Text = "Freq. List";
		this.label5.AutoSize = true;
		this.label5.Location = new System.Drawing.Point(22, 80);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(61, 13);
		this.label5.TabIndex = 1;
		this.label5.Text = "Freq. Mode";
		this.cbRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbRegion.FormattingEnabled = true;
		this.cbRegion.Items.AddRange(new object[11]
		{
			"0 - US, North America, FCC Part 15.247", "1 - ETSI EN 300-220", "2 - ETSI EN 302-208 (With LBT)", "3 - Hong Kong 920-925 MHz", "4 - Taiwan 922-928 MHz", "5 - Japan 952-954 MHz", "6 - Japan 952-955 MHz, 10mW", "7 - ETSI EN 302-208 (Without LBT)", "8 - Korea 910-914 MHz", "9 - Malaysia 919-923 MHz",
			"10 - China 920-925 MHz"
		});
		this.cbRegion.Location = new System.Drawing.Point(85, 49);
		this.cbRegion.Name = "cbRegion";
		this.cbRegion.Size = new System.Drawing.Size(252, 21);
		this.cbRegion.TabIndex = 2;
		this.cbRegion.SelectedIndexChanged += new System.EventHandler(cbRegion_SelectedIndexChanged);
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(41, 53);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(41, 13);
		this.label2.TabIndex = 1;
		this.label2.Text = "Region";
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(47, 26);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(35, 13);
		this.label1.TabIndex = 1;
		this.label1.Text = "Name";
		this.tbReaderName.Location = new System.Drawing.Point(85, 23);
		this.tbReaderName.Name = "tbReaderName";
		this.tbReaderName.Size = new System.Drawing.Size(252, 20);
		this.tbReaderName.TabIndex = 0;
		this.tbReaderName.Text = "speedway-00-";
		this.cbSession.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbSession.FormattingEnabled = true;
		this.cbSession.Items.AddRange(new object[4] { "Session 0", "Session 1", "Session 2", "Session 3" });
		this.cbSession.Location = new System.Drawing.Point(154, 36);
		this.cbSession.Name = "cbSession";
		this.cbSession.Size = new System.Drawing.Size(99, 21);
		this.cbSession.TabIndex = 2;
		this.cbReaderMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbReaderMode.FormattingEnabled = true;
		this.cbReaderMode.Items.AddRange(new object[5] { "0 - Max Throughput", "1 - Hybrid Mode", "2 - Dense Reader Mode M=4", "3 - Dense Reader Mode M=8", "4 - Max Miller" });
		this.cbReaderMode.Location = new System.Drawing.Point(154, 94);
		this.cbReaderMode.Name = "cbReaderMode";
		this.cbReaderMode.Size = new System.Drawing.Size(182, 21);
		this.cbReaderMode.TabIndex = 2;
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(102, 39);
		this.label3.Name = "label3";
		this.label3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
		this.label3.Size = new System.Drawing.Size(44, 13);
		this.label3.TabIndex = 1;
		this.label3.Text = "Session";
		this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(79, 98);
		this.label4.Name = "label4";
		this.label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
		this.label4.Size = new System.Drawing.Size(67, 13);
		this.label4.TabIndex = 1;
		this.label4.Text = "Mode ID List";
		this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.chMaxSens.AutoSize = true;
		this.chMaxSens.Location = new System.Drawing.Point(232, 115);
		this.chMaxSens.Name = "chMaxSens";
		this.chMaxSens.Size = new System.Drawing.Size(99, 17);
		this.chMaxSens.TabIndex = 3;
		this.chMaxSens.Text = "Max. Sensitivity";
		this.chMaxSens.UseVisualStyleBackColor = true;
		this.chMaxSens.CheckedChanged += new System.EventHandler(chMaxSens_CheckedChanged);
		this.label12.AutoSize = true;
		this.label12.Location = new System.Drawing.Point(83, 34);
		this.label12.Name = "label12";
		this.label12.Size = new System.Drawing.Size(66, 13);
		this.label12.TabIndex = 13;
		this.label12.Text = "Power (dbm)";
		this.label13.AutoSize = true;
		this.label13.Location = new System.Drawing.Point(29, 34);
		this.label13.Name = "label13";
		this.label13.Size = new System.Drawing.Size(30, 13);
		this.label13.TabIndex = 13;
		this.label13.Text = "Ant#";
		this.tbRSSI2.Location = new System.Drawing.Point(150, 72);
		this.tbRSSI2.Name = "tbRSSI2";
		this.tbRSSI2.Size = new System.Drawing.Size(37, 20);
		this.tbRSSI2.TabIndex = 9;
		this.tbRSSI2.Text = "-70";
		this.tbRSSI4.Location = new System.Drawing.Point(150, 112);
		this.tbRSSI4.Name = "tbRSSI4";
		this.tbRSSI4.Size = new System.Drawing.Size(37, 20);
		this.tbRSSI4.TabIndex = 9;
		this.tbRSSI4.Text = "-70";
		this.chAnt1.AutoSize = true;
		this.chAnt1.Checked = true;
		this.chAnt1.CheckState = System.Windows.Forms.CheckState.Checked;
		this.chAnt1.Location = new System.Drawing.Point(24, 54);
		this.chAnt1.Name = "chAnt1";
		this.chAnt1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
		this.chAnt1.Size = new System.Drawing.Size(32, 17);
		this.chAnt1.TabIndex = 11;
		this.chAnt1.Text = "1";
		this.chAnt1.UseVisualStyleBackColor = true;
		this.tbPower4.Location = new System.Drawing.Point(86, 111);
		this.tbPower4.Name = "tbPower4";
		this.tbPower4.Size = new System.Drawing.Size(38, 20);
		this.tbPower4.TabIndex = 9;
		this.tbPower4.Text = "30.00";
		this.chAnt2.AutoSize = true;
		this.chAnt2.Location = new System.Drawing.Point(24, 76);
		this.chAnt2.Name = "chAnt2";
		this.chAnt2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
		this.chAnt2.Size = new System.Drawing.Size(32, 17);
		this.chAnt2.TabIndex = 12;
		this.chAnt2.Text = "2";
		this.chAnt2.UseVisualStyleBackColor = true;
		this.tbPower3.Location = new System.Drawing.Point(86, 91);
		this.tbPower3.Name = "tbPower3";
		this.tbPower3.Size = new System.Drawing.Size(38, 20);
		this.tbPower3.TabIndex = 9;
		this.tbPower3.Text = "30.00";
		this.chAnt3.AutoSize = true;
		this.chAnt3.Location = new System.Drawing.Point(24, 95);
		this.chAnt3.Name = "chAnt3";
		this.chAnt3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
		this.chAnt3.Size = new System.Drawing.Size(32, 17);
		this.chAnt3.TabIndex = 12;
		this.chAnt3.Text = "3";
		this.chAnt3.UseVisualStyleBackColor = true;
		this.tbRSSI3.Location = new System.Drawing.Point(150, 92);
		this.tbRSSI3.Name = "tbRSSI3";
		this.tbRSSI3.Size = new System.Drawing.Size(37, 20);
		this.tbRSSI3.TabIndex = 9;
		this.tbRSSI3.Text = "-70";
		this.chAnt4.AutoSize = true;
		this.chAnt4.Location = new System.Drawing.Point(24, 114);
		this.chAnt4.Name = "chAnt4";
		this.chAnt4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
		this.chAnt4.Size = new System.Drawing.Size(32, 17);
		this.chAnt4.TabIndex = 12;
		this.chAnt4.Text = "4";
		this.chAnt4.UseVisualStyleBackColor = true;
		this.tbPower1.Location = new System.Drawing.Point(86, 51);
		this.tbPower1.Name = "tbPower1";
		this.tbPower1.Size = new System.Drawing.Size(38, 20);
		this.tbPower1.TabIndex = 9;
		this.tbPower1.Text = "30.00";
		this.tbRSSI1.Location = new System.Drawing.Point(150, 52);
		this.tbRSSI1.Name = "tbRSSI1";
		this.tbRSSI1.Size = new System.Drawing.Size(37, 20);
		this.tbRSSI1.TabIndex = 9;
		this.tbRSSI1.Text = "-70";
		this.tbPower2.Location = new System.Drawing.Point(86, 71);
		this.tbPower2.Name = "tbPower2";
		this.tbPower2.Size = new System.Drawing.Size(38, 20);
		this.tbPower2.TabIndex = 9;
		this.tbPower2.Text = "30.00";
		this.textBox9.Location = new System.Drawing.Point(152, 53);
		this.textBox9.Name = "textBox9";
		this.textBox9.Size = new System.Drawing.Size(35, 20);
		this.textBox9.TabIndex = 9;
		this.textBox9.Text = "4";
		this.textBox2.BackColor = System.Drawing.SystemColors.Info;
		this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.textBox2.Font = new System.Drawing.Font("Arial", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.textBox2.ForeColor = System.Drawing.Color.Navy;
		this.textBox2.Location = new System.Drawing.Point(0, 4);
		this.textBox2.Multiline = true;
		this.textBox2.Name = "textBox2";
		this.textBox2.Size = new System.Drawing.Size(238, 170);
		this.textBox2.TabIndex = 18;
		this.textBox2.Text = resources.GetString("textBox2.Text");
		this.tabControl1.Controls.Add(this.tpReader);
		this.tabControl1.Controls.Add(this.tpAntenna);
		this.tabControl1.Controls.Add(this.tpGen2);
		this.tabControl1.Controls.Add(this.tabPage1);
		this.tabControl1.Location = new System.Drawing.Point(3, 3);
		this.tabControl1.Name = "tabControl1";
		this.tabControl1.SelectedIndex = 0;
		this.tabControl1.Size = new System.Drawing.Size(389, 200);
		this.tabControl1.TabIndex = 19;
		this.tpReader.Controls.Add(this.cbLBTMode);
		this.tpReader.Controls.Add(this.cbFrequencyList);
		this.tpReader.Controls.Add(this.cbRegion);
		this.tpReader.Controls.Add(this.cbFrequencyMode);
		this.tpReader.Controls.Add(this.tbReaderName);
		this.tpReader.Controls.Add(this.label7);
		this.tpReader.Controls.Add(this.label1);
		this.tpReader.Controls.Add(this.label6);
		this.tpReader.Controls.Add(this.label2);
		this.tpReader.Controls.Add(this.label5);
		this.tpReader.Location = new System.Drawing.Point(4, 22);
		this.tpReader.Name = "tpReader";
		this.tpReader.Padding = new System.Windows.Forms.Padding(3);
		this.tpReader.Size = new System.Drawing.Size(381, 174);
		this.tpReader.TabIndex = 0;
		this.tpReader.Text = "Reader Information";
		this.tpReader.UseVisualStyleBackColor = true;
		this.cbFrequencyList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbFrequencyList.Enabled = false;
		this.cbFrequencyList.FormattingEnabled = true;
		this.cbFrequencyList.Location = new System.Drawing.Point(85, 103);
		this.cbFrequencyList.MaxDropDownItems = 13;
		this.cbFrequencyList.Name = "cbFrequencyList";
		this.cbFrequencyList.Size = new System.Drawing.Size(196, 21);
		this.cbFrequencyList.TabIndex = 2;
		this.tpAntenna.Controls.Add(this.chMaxSens);
		this.tpAntenna.Controls.Add(this.label9);
		this.tpAntenna.Controls.Add(this.label10);
		this.tpAntenna.Controls.Add(this.tbRSSI1);
		this.tpAntenna.Controls.Add(this.label12);
		this.tpAntenna.Controls.Add(this.tbPower1);
		this.tpAntenna.Controls.Add(this.textBox9);
		this.tpAntenna.Controls.Add(this.label13);
		this.tpAntenna.Controls.Add(this.tbRSSI3);
		this.tpAntenna.Controls.Add(this.tbRSSI4);
		this.tpAntenna.Controls.Add(this.tbPower3);
		this.tpAntenna.Controls.Add(this.tbRSSI2);
		this.tpAntenna.Controls.Add(this.chAnt4);
		this.tpAntenna.Controls.Add(this.tbPower4);
		this.tpAntenna.Controls.Add(this.chAnt1);
		this.tpAntenna.Controls.Add(this.chAnt3);
		this.tpAntenna.Controls.Add(this.tbPower2);
		this.tpAntenna.Controls.Add(this.chAnt2);
		this.tpAntenna.Location = new System.Drawing.Point(4, 22);
		this.tpAntenna.Name = "tpAntenna";
		this.tpAntenna.Padding = new System.Windows.Forms.Padding(3);
		this.tpAntenna.Size = new System.Drawing.Size(381, 174);
		this.tpAntenna.TabIndex = 1;
		this.tpAntenna.Text = "Antenna Setting";
		this.tpAntenna.UseVisualStyleBackColor = true;
		this.tpAntenna.Click += new System.EventHandler(tpAntenna_Click);
		this.label9.AutoSize = true;
		this.label9.Location = new System.Drawing.Point(137, 21);
		this.label9.Name = "label9";
		this.label9.Size = new System.Drawing.Size(0, 13);
		this.label9.TabIndex = 13;
		this.label10.AutoSize = true;
		this.label10.Location = new System.Drawing.Point(147, 34);
		this.label10.Name = "label10";
		this.label10.Size = new System.Drawing.Size(74, 13);
		this.label10.TabIndex = 13;
		this.label10.Text = "Rx. Sens. (db)";
		this.tpGen2.Controls.Add(this.cbSession);
		this.tpGen2.Controls.Add(this.label16);
		this.tpGen2.Controls.Add(this.label4);
		this.tpGen2.Controls.Add(this.cbAutoSetMode);
		this.tpGen2.Controls.Add(this.cbInventoryMode);
		this.tpGen2.Controls.Add(this.cbReaderMode);
		this.tpGen2.Controls.Add(this.label15);
		this.tpGen2.Controls.Add(this.label3);
		this.tpGen2.Location = new System.Drawing.Point(4, 22);
		this.tpGen2.Name = "tpGen2";
		this.tpGen2.Size = new System.Drawing.Size(381, 174);
		this.tpGen2.TabIndex = 2;
		this.tpGen2.Text = "Gen2 Parameters";
		this.tpGen2.UseVisualStyleBackColor = true;
		this.tabPage1.Controls.Add(this.label14);
		this.tabPage1.Controls.Add(this.label11);
		this.tabPage1.Controls.Add(this.lbFPGA);
		this.tabPage1.Controls.Add(this.lbFirmware);
		this.tabPage1.Controls.Add(this.lbSoftware);
		this.tabPage1.Controls.Add(this.label8);
		this.tabPage1.Controls.Add(this.llRFID);
		this.tabPage1.Controls.Add(this.pictureBox1);
		this.tabPage1.Controls.Add(this.textBox2);
		this.tabPage1.Location = new System.Drawing.Point(4, 22);
		this.tabPage1.Name = "tabPage1";
		this.tabPage1.Size = new System.Drawing.Size(381, 174);
		this.tabPage1.TabIndex = 3;
		this.tabPage1.Text = "GrandPrix Solution";
		this.tabPage1.UseVisualStyleBackColor = true;
		this.label14.AutoSize = true;
		this.label14.Location = new System.Drawing.Point(248, 154);
		this.label14.Name = "label14";
		this.label14.Size = new System.Drawing.Size(41, 13);
		this.label14.TabIndex = 21;
		this.label14.Text = "FPGA :";
		this.label11.AutoSize = true;
		this.label11.Location = new System.Drawing.Point(248, 137);
		this.label11.Name = "label11";
		this.label11.Size = new System.Drawing.Size(52, 13);
		this.label11.TabIndex = 21;
		this.label11.Text = "Firmware:";
		this.lbFPGA.AutoSize = true;
		this.lbFPGA.Location = new System.Drawing.Point(301, 155);
		this.lbFPGA.Name = "lbFPGA";
		this.lbFPGA.Size = new System.Drawing.Size(22, 13);
		this.lbFPGA.TabIndex = 21;
		this.lbFPGA.Text = "     ";
		this.lbFirmware.AutoSize = true;
		this.lbFirmware.Location = new System.Drawing.Point(301, 138);
		this.lbFirmware.Name = "lbFirmware";
		this.lbFirmware.Size = new System.Drawing.Size(19, 13);
		this.lbFirmware.TabIndex = 21;
		this.lbFirmware.Text = "    ";
		this.lbSoftware.AutoSize = true;
		this.lbSoftware.Location = new System.Drawing.Point(301, 122);
		this.lbSoftware.Name = "lbSoftware";
		this.lbSoftware.Size = new System.Drawing.Size(22, 13);
		this.lbSoftware.TabIndex = 21;
		this.lbSoftware.Text = "     ";
		this.label8.AutoSize = true;
		this.label8.Location = new System.Drawing.Point(248, 121);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(52, 13);
		this.label8.TabIndex = 21;
		this.label8.Text = "Software:";
		this.llRFID.AutoSize = true;
		this.llRFID.Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.llRFID.Location = new System.Drawing.Point(258, 7);
		this.llRFID.Name = "llRFID";
		this.llRFID.Size = new System.Drawing.Size(97, 19);
		this.llRFID.TabIndex = 20;
		this.llRFID.TabStop = true;
		this.llRFID.Text = "Impinj RFID";
		this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.pictureBox1.Image = (System.Drawing.Image)resources.GetObject("pictureBox1.Image");
		this.pictureBox1.Location = new System.Drawing.Point(257, 30);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(100, 87);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pictureBox1.TabIndex = 19;
		this.pictureBox1.TabStop = false;
		this.label15.AutoSize = true;
		this.label15.Location = new System.Drawing.Point(68, 68);
		this.label15.Name = "label15";
		this.label15.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
		this.label15.Size = new System.Drawing.Size(78, 13);
		this.label15.TabIndex = 1;
		this.label15.Text = "Auto Set Mode";
		this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.cbAutoSetMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbAutoSetMode.FormattingEnabled = true;
		this.cbAutoSetMode.Items.AddRange(new object[3] { "1 - Auto Set (Dense)", "2 - By Mode ID", "3 - Auto Set (Single)" });
		this.cbAutoSetMode.Location = new System.Drawing.Point(154, 65);
		this.cbAutoSetMode.Name = "cbAutoSetMode";
		this.cbAutoSetMode.Size = new System.Drawing.Size(137, 21);
		this.cbAutoSetMode.TabIndex = 2;
		this.cbAutoSetMode.SelectedIndexChanged += new System.EventHandler(cbAutoSetMode_SelectedIndexChanged);
		this.cbInventoryMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbInventoryMode.FormattingEnabled = true;
		this.cbInventoryMode.Items.AddRange(new object[3] { "0 - Dual Target Inventory", "1 - Single Target Inventory", "2 - Single Target Inventory (SDR)" });
		this.cbInventoryMode.Location = new System.Drawing.Point(154, 121);
		this.cbInventoryMode.Name = "cbInventoryMode";
		this.cbInventoryMode.Size = new System.Drawing.Size(198, 21);
		this.cbInventoryMode.TabIndex = 2;
		this.label16.AutoSize = true;
		this.label16.Location = new System.Drawing.Point(28, 124);
		this.label16.Name = "label16";
		this.label16.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
		this.label16.Size = new System.Drawing.Size(118, 13);
		this.label16.TabIndex = 1;
		this.label16.Text = "Inventory Search Mode";
		this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tabControl1);
		base.Name = "SpeedwayReaderParams";
		base.Size = new System.Drawing.Size(395, 206);
		base.Load += new System.EventHandler(SpeedwayReaderParams_Load);
		this.tabControl1.ResumeLayout(false);
		this.tpReader.ResumeLayout(false);
		this.tpReader.PerformLayout();
		this.tpAntenna.ResumeLayout(false);
		this.tpAntenna.PerformLayout();
		this.tpGen2.ResumeLayout(false);
		this.tpGen2.PerformLayout();
		this.tabPage1.ResumeLayout(false);
		this.tabPage1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		base.ResumeLayout(false);
	}

	public SpeedwayReaderParams()
	{
		InitializeComponent();
	}

	private void tpAntenna_Click(object sender, EventArgs e)
	{
	}

	private void cbLBTMode_SelectedIndexChanged(object sender, EventArgs e)
	{
	}

	private void SpeedwayReaderParams_Load(object sender, EventArgs e)
	{
		llRFID.LinkClicked += llRFID_LinkClicked;
	}

	public void UpdateForm(SpeedwayReaderSettings settings)
	{
		if (settings != null)
		{
			tbReaderName.Text = settings.reader_information.reader_name;
			cbRegion.SelectedIndex = settings.reader_information.region;
			cbFrequencyMode.SelectedIndex = settings.reader_information.frequency_mode;
			cbLBTMode.SelectedIndex = settings.reader_information.lbt_time;
			chAnt1.Checked = settings.antennas[0].enabled;
			chAnt2.Checked = settings.antennas[1].enabled;
			chAnt3.Checked = settings.antennas[2].enabled;
			chAnt4.Checked = settings.antennas[3].enabled;
			tbPower1.Text = settings.antennas[0].power.ToString();
			tbPower2.Text = settings.antennas[1].power.ToString();
			tbPower3.Text = settings.antennas[2].power.ToString();
			tbPower4.Text = settings.antennas[3].power.ToString();
			if (settings.maximum_sensitivity)
			{
				tbRSSI1.Enabled = false;
				tbRSSI2.Enabled = false;
				tbRSSI3.Enabled = false;
				tbRSSI4.Enabled = false;
				chMaxSens.Checked = true;
			}
			else
			{
				tbRSSI1.Text = settings.antennas[0].rssi.ToString();
				tbRSSI2.Text = settings.antennas[1].rssi.ToString();
				tbRSSI3.Text = settings.antennas[2].rssi.ToString();
				tbRSSI4.Text = settings.antennas[3].rssi.ToString();
			}
			if (settings.reader_information.frequency_mode == 0)
			{
				switch (settings.reader_information.region)
				{
				case 2:
				case 7:
					cbFrequencyList.SelectedIndex = (settings.reader_information.frequency - 114) / 12;
					break;
				case 5:
				case 6:
					cbFrequencyList.SelectedIndex = (settings.reader_information.frequency - 1846) / 2;
					break;
				}
			}
			cbInventoryMode.SelectedIndex = settings.gen2_params.inventory_mode;
			cbRegion.Enabled = false;
			cbSession.SelectedIndex = settings.gen2_params.session;
			cbAutoSetMode.SelectedIndex = settings.gen2_params.auto_set_mode - 1;
			cbReaderMode.Enabled = settings.gen2_params.auto_set_mode == 2;
			if (cbReaderMode.Enabled)
			{
				cbReaderMode.SelectedIndex = settings.gen2_params.mode_id;
			}
			lbFirmware.Text = settings.reader_information.firmware_ver;
			lbFPGA.Text = settings.reader_information.fpga_ver;
			lbSoftware.Text = settings.reader_information.software_ver;
		}
		else
		{
			tbReaderName.Text = "";
			cbRegion.SelectedIndex = 0;
			cbFrequencyMode.Enabled = false;
			cbFrequencyList.Enabled = false;
			chAnt1.Checked = true;
			chAnt2.Checked = true;
			chAnt3.Checked = true;
			chAnt4.Checked = true;
			cbInventoryMode.SelectedIndex = 0;
			cbAutoSetMode.SelectedIndex = 1;
			cbSession.SelectedIndex = 1;
			cbReaderMode.SelectedIndex = 2;
		}
	}

	private void llRFID_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Process.Start("http://www.impinj.com/RFID");
	}

	public SpeedwayReaderSettings GetSettings()
	{
		SpeedwayReaderSettings speedwayReaderSettings = new SpeedwayReaderSettings();
		speedwayReaderSettings.gen2_params.mode_id = cbReaderMode.SelectedIndex;
		speedwayReaderSettings.gen2_params.session = cbSession.SelectedIndex;
		speedwayReaderSettings.gen2_params.auto_set_mode = cbAutoSetMode.SelectedIndex + 1;
		speedwayReaderSettings.gen2_params.inventory_mode = cbInventoryMode.SelectedIndex;
		speedwayReaderSettings.antennas[0].enabled = chAnt1.Checked;
		speedwayReaderSettings.antennas[1].enabled = chAnt2.Checked;
		speedwayReaderSettings.antennas[2].enabled = chAnt3.Checked;
		speedwayReaderSettings.antennas[3].enabled = chAnt4.Checked;
		speedwayReaderSettings.antennas[0].power = float.Parse(tbPower1.Text);
		speedwayReaderSettings.antennas[1].power = float.Parse(tbPower2.Text);
		speedwayReaderSettings.antennas[2].power = float.Parse(tbPower3.Text);
		speedwayReaderSettings.antennas[3].power = float.Parse(tbPower4.Text);
		speedwayReaderSettings.antennas[0].rssi = short.Parse(tbRSSI1.Text);
		speedwayReaderSettings.antennas[1].rssi = short.Parse(tbRSSI2.Text);
		speedwayReaderSettings.antennas[2].rssi = short.Parse(tbRSSI3.Text);
		speedwayReaderSettings.antennas[3].rssi = short.Parse(tbRSSI4.Text);
		speedwayReaderSettings.maximum_sensitivity = chMaxSens.Checked;
		speedwayReaderSettings.reader_information.reader_name = tbReaderName.Text;
		speedwayReaderSettings.reader_information.region = cbRegion.SelectedIndex;
		speedwayReaderSettings.reader_information.frequency_mode = cbFrequencyMode.SelectedIndex;
		speedwayReaderSettings.reader_information.lbt_time = cbLBTMode.SelectedIndex;
		if (speedwayReaderSettings.reader_information.frequency_mode == 0)
		{
			switch (speedwayReaderSettings.reader_information.region)
			{
			case 2:
			case 7:
				speedwayReaderSettings.reader_information.frequency = (ushort)(114 + cbFrequencyList.SelectedIndex * 12);
				break;
			case 5:
			case 6:
				speedwayReaderSettings.reader_information.frequency = (ushort)(1846 + cbFrequencyList.SelectedIndex * 2);
				break;
			}
		}
		return speedwayReaderSettings;
	}

	private void cbRegion_SelectedIndexChanged(object sender, EventArgs e)
	{
		cbFrequencyList.Items.Clear();
		tbPower1.Text = "30.00";
		tbPower2.Text = "30.00";
		tbPower3.Text = "30.00";
		tbPower4.Text = "30.00";
		switch (cbRegion.SelectedIndex)
		{
		case 0:
			cbFrequencyMode.Enabled = false;
			cbFrequencyList.Enabled = false;
			cbLBTMode.Enabled = false;
			AddModeIdListItem(new int[5] { 0, 1, 2, 3, 4 });
			AddAutoSetListItem(new int[3] { 1, 2, 3 });
			break;
		case 1:
			cbFrequencyMode.Enabled = false;
			cbFrequencyList.Enabled = false;
			cbLBTMode.Enabled = false;
			AddModeIdListItem(new int[0]);
			AddAutoSetListItem(new int[0]);
			break;
		case 2:
			cbFrequencyMode.Enabled = true;
			cbFrequencyList.Items.Add("4 - 865.70MHz (114)");
			cbFrequencyList.Items.Add("7 - 866.30MHz (126)");
			cbFrequencyList.Items.Add("10 - 866.70MHz (138)");
			cbFrequencyList.Items.Add("13 - 867.50MHz (150)");
			tbPower1.Text = "28.00";
			tbPower2.Text = "28.00";
			tbPower3.Text = "28.00";
			tbPower4.Text = "28.00";
			cbLBTMode.Enabled = true;
			cbFrequencyMode.SelectedIndex = 1;
			AddModeIdListItem(new int[4] { 0, 1, 2, 3 });
			AddAutoSetListItem(new int[2] { 1, 2 });
			break;
		case 3:
			cbFrequencyMode.Enabled = false;
			cbFrequencyList.Enabled = false;
			cbLBTMode.Enabled = false;
			AddModeIdListItem(new int[5] { 0, 1, 2, 3, 4 });
			AddAutoSetListItem(new int[3] { 1, 2, 3 });
			break;
		case 4:
			cbFrequencyMode.Enabled = false;
			cbFrequencyList.Enabled = false;
			cbLBTMode.Enabled = false;
			AddModeIdListItem(new int[5] { 0, 1, 2, 3, 4 });
			AddAutoSetListItem(new int[3] { 1, 2, 3 });
			break;
		case 5:
			cbFrequencyMode.Enabled = true;
			cbFrequencyList.Items.Add("1 - 952.30MHz (1846)");
			cbFrequencyList.Items.Add("2 - 952.40MHz (1848)");
			cbFrequencyList.Items.Add("3 - 952.50MHz (1850)");
			cbFrequencyList.Items.Add("4 - 952.60MHz (1852)");
			cbFrequencyList.Items.Add("5 - 952.70MHz (1854)");
			cbFrequencyList.Items.Add("6 - 952.80MHz (1856)");
			cbFrequencyList.Items.Add("7 - 952.90MHz (1858)");
			cbFrequencyList.Items.Add("8 - 953.00MHz (1860)");
			cbFrequencyList.Items.Add("9 - 953.10MHz (1862)");
			cbFrequencyList.Items.Add("10 - 953.20MHz (1864)");
			cbFrequencyList.Items.Add("11 - 953.30MHz (1866)");
			cbFrequencyList.Items.Add("12 - 953.40MHz (1868)");
			cbFrequencyList.Items.Add("13 - 953.50MHz (1870)");
			cbFrequencyList.Items.Add("14 - 953.60MHz (1872)");
			cbFrequencyList.Items.Add("15 - 953.70MHz (1874)");
			cbFrequencyList.Items.Add("16 - 953.80MHz (1876)");
			cbLBTMode.Enabled = true;
			cbLBTMode.SelectedIndex = 1;
			cbFrequencyMode.SelectedIndex = 1;
			AddModeIdListItem(new int[4] { 0, 1, 2, 3 });
			AddAutoSetListItem(new int[3] { 1, 2, 3 });
			break;
		case 6:
			cbFrequencyMode.Enabled = true;
			cbFrequencyList.Items.Add("1 - 952.30MHz (1846)");
			cbFrequencyList.Items.Add("2 - 952.50MHz (1850)");
			cbFrequencyList.Items.Add("3 - 952.70MHz (1854)");
			cbFrequencyList.Items.Add("4 - 952.90MHz (1858)");
			cbFrequencyList.Items.Add("5 - 953.10MHz (1862)");
			cbFrequencyList.Items.Add("6 - 953.30MHz (1866)");
			cbFrequencyList.Items.Add("7 - 953.50MHz (1870)");
			cbFrequencyList.Items.Add("8 - 953.70MHz (1874)");
			cbFrequencyList.Items.Add("9 - 953.90MHz (1878)");
			cbFrequencyList.Items.Add("10 - 954.10MHz (1882)");
			cbFrequencyList.Items.Add("11 - 954.30MHz (1886)");
			cbFrequencyList.Items.Add("12 - 954.50MHz (1890)");
			cbFrequencyList.Items.Add("13 - 954.70MHz (1894)");
			tbPower1.Text = "10.00";
			tbPower2.Text = "10.00";
			tbPower3.Text = "10.00";
			tbPower4.Text = "10.00";
			cbLBTMode.Enabled = true;
			cbLBTMode.SelectedIndex = 1;
			cbFrequencyMode.SelectedIndex = 1;
			AddModeIdListItem(new int[4] { 0, 1, 2, 3 });
			AddAutoSetListItem(new int[3] { 1, 2, 3 });
			break;
		case 7:
			cbFrequencyMode.Enabled = true;
			cbFrequencyList.Items.Add("4 - 865.70MHz (114)");
			cbFrequencyList.Items.Add("7 - 866.30MHz (126)");
			cbFrequencyList.Items.Add("10 - 866.70MHz (138)");
			cbFrequencyList.Items.Add("13 - 867.50MHz (150)");
			tbPower1.Text = "28.00";
			tbPower2.Text = "28.00";
			tbPower3.Text = "28.00";
			tbPower4.Text = "28.00";
			cbLBTMode.Enabled = false;
			cbFrequencyMode.SelectedIndex = 1;
			AddModeIdListItem(new int[4] { 0, 1, 2, 3 });
			AddAutoSetListItem(new int[2] { 1, 2 });
			break;
		case 8:
			cbFrequencyMode.Enabled = false;
			cbFrequencyList.Enabled = false;
			cbLBTMode.Enabled = false;
			AddModeIdListItem(new int[5] { 0, 1, 2, 3, 4 });
			AddAutoSetListItem(new int[2] { 1, 2 });
			break;
		case 9:
			cbFrequencyMode.Enabled = false;
			cbFrequencyList.Enabled = false;
			cbLBTMode.Enabled = false;
			AddModeIdListItem(new int[5] { 0, 1, 2, 3, 4 });
			AddAutoSetListItem(new int[3] { 1, 2, 3 });
			break;
		case 10:
			cbFrequencyMode.Enabled = false;
			cbFrequencyList.Enabled = false;
			cbLBTMode.Enabled = false;
			AddModeIdListItem(new int[3] { 0, 1, 2 });
			AddAutoSetListItem(new int[2] { 1, 2 });
			break;
		}
		try
		{
			cbAutoSetMode.SelectedIndex = 1;
			cbReaderMode.SelectedIndex = 1;
		}
		catch
		{
		}
	}

	private void AddModeIdListItem(int[] modeIDs)
	{
		cbReaderMode.Items.Clear();
		foreach (int num in modeIDs)
		{
			string item = null;
			switch (num)
			{
			case 0:
				item = "0 - Max Throughput";
				break;
			case 1:
				item = "1 - Hybrid Mode";
				break;
			case 2:
				item = "2 - Dense Reader Mode M=4";
				break;
			case 3:
				item = "3 - Dense Reader Mode M=8";
				break;
			case 4:
				item = "4 - Max Miller";
				break;
			}
			cbReaderMode.Items.Add(item);
		}
	}

	private void AddAutoSetListItem(int[] autoSetIDs)
	{
		cbAutoSetMode.Items.Clear();
		foreach (int num in autoSetIDs)
		{
			string item = null;
			switch (num)
			{
			case 1:
				item = "1 - Auto Set (Dense)";
				break;
			case 2:
				item = "2 - By Mode ID";
				break;
			case 3:
				item = "3 - Auto Set (Single)";
				break;
			}
			cbAutoSetMode.Items.Add(item);
		}
	}

	private void chSingleTarget_CheckedChanged(object sender, EventArgs e)
	{
		cbSession.SelectedIndex = 0;
	}

	private void chMaxSens_CheckedChanged(object sender, EventArgs e)
	{
		tbRSSI1.Enabled = !chMaxSens.Checked;
		tbRSSI2.Enabled = !chMaxSens.Checked;
		tbRSSI3.Enabled = !chMaxSens.Checked;
		tbRSSI4.Enabled = !chMaxSens.Checked;
	}

	public void SetMode(bool mode_add)
	{
		tbReaderName.Enabled = mode_add;
	}

	private void cbAutoSetMode_SelectedIndexChanged(object sender, EventArgs e)
	{
		cbReaderMode.Enabled = cbAutoSetMode.SelectedIndex == 1;
	}

	private void cbFrequencyMode_SelectedIndexChanged(object sender, EventArgs e)
	{
		cbFrequencyList.Enabled = cbFrequencyMode.SelectedIndex == 0;
	}
}
