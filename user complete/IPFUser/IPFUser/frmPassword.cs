using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace IPFUser;

[DesignerGenerated]
public class frmPassword : Form
{
	private IContainer components;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Textbox1")]
	private TextBox _Textbox1;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Button1")]
	private Button _Button1;

	private string _Password;

	private Form _Parent;

	[field: AccessedThroughProperty("Label1")]
	internal virtual Label Label1
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual TextBox Textbox1
	{
		[CompilerGenerated]
		get
		{
			return _Textbox1;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			KeyPressEventHandler value2 = Textbox1_KeyPress;
			TextBox textBox = _Textbox1;
			if (textBox != null)
			{
				textBox.KeyPress -= value2;
			}
			_Textbox1 = value;
			textBox = _Textbox1;
			if (textBox != null)
			{
				textBox.KeyPress += value2;
			}
		}
	}

	internal virtual Button Button1
	{
		[CompilerGenerated]
		get
		{
			return _Button1;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler value2 = Button1_Click;
			Button button = _Button1;
			if (button != null)
			{
				button.Click -= value2;
			}
			_Button1 = value;
			button = _Button1;
			if (button != null)
			{
				button.Click += value2;
			}
		}
	}

	[DebuggerNonUserCode]
	protected override void Dispose(bool disposing)
	{
		try
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
		}
		finally
		{
			base.Dispose(disposing);
		}
	}

	[System.Diagnostics.DebuggerStepThrough]
	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IPFUser.frmPassword));
		this.Label1 = new System.Windows.Forms.Label();
		this.Textbox1 = new System.Windows.Forms.TextBox();
		this.Button1 = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.Label1.AutoSize = true;
		this.Label1.Location = new System.Drawing.Point(32, 20);
		this.Label1.Name = "Label1";
		this.Label1.Size = new System.Drawing.Size(117, 13);
		this.Label1.TabIndex = 0;
		this.Label1.Text = "Introduzca el Password";
		this.Textbox1.Location = new System.Drawing.Point(35, 46);
		this.Textbox1.Name = "Textbox1";
		this.Textbox1.Size = new System.Drawing.Size(136, 20);
		this.Textbox1.TabIndex = 1;
		this.Button1.Location = new System.Drawing.Point(35, 72);
		this.Button1.Name = "Button1";
		this.Button1.Size = new System.Drawing.Size(136, 29);
		this.Button1.TabIndex = 2;
		this.Button1.Text = "Aceptar";
		this.Button1.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(204, 113);
		base.Controls.Add(this.Button1);
		base.Controls.Add(this.Textbox1);
		base.Controls.Add(this.Label1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frmPassword";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Password";
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	public frmPassword(string Password, Form Parent)
	{
		base.Load += Form1_Load;
		InitializeComponent();
		Textbox1.MaxLength = Password.Length;
		Textbox1.PasswordChar = '*';
		_Password = Password;
		_Parent = Parent;
		_Parent.Tag = false;
	}

	private void Form1_Load(object sender, EventArgs e)
	{
	}

	private void Button1_Click(object sender, EventArgs e)
	{
		if (Operators.CompareString(Textbox1.Text, _Password, TextCompare: false) == 0)
		{
			_Parent.Tag = true;
		}
		else
		{
			_Parent.Tag = false;
		}
		Close();
	}

	private void Textbox1_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar == '\r')
		{
			Button1_Click(null, null);
		}
	}
}
