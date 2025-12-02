using System;
using System.Drawing;
using System.Windows.Forms;

namespace SocketCommDllTest
{
    public partial class MyInfoBox : Form
    {
        public MyInfoBox()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
        }

        static MyInfoBox MsgBox;
        static DialogResult result = DialogResult.No;
        public static DialogResult ShowInfImage(Image image, string name, Point p)
        {            
            MsgBox = new MyInfoBox();
            MsgBox.inf_image.Image = image;
            MsgBox.Text = name + " function";
            MsgBox.StartPosition = FormStartPosition.Manual;
            MsgBox.Location = p;
            MsgBox.ShowDialog();              
            return result;            
        }

        private void MyInfoBox_FormClosing(Object sender, FormClosingEventArgs e)
        {
            MsgBox.Dispose(true);
            this.Dispose(true);        
        }

        

        
    }

   
}
