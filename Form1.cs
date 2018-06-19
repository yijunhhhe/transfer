using Baoshen.RfidShowRoom.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResumeBrokenTransfer
{
    public partial class Form1 : Form
    {
        private Download dl;
        IAccountService accountService = ServiceProvider.GetService<IAccountService>();
        private bool IsPause = false;
        public Form1()
        {
            InitializeComponent();
            this.urlTextBox.Text = "http://qianqian.baidu.com/download/BaiduMusic-12345630.exe";          
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void url_Click(object sender, EventArgs e)
        {

        }

        private void download(object sender, EventArgs e)
        {
            StartDownload();
        }

       
        private void StartDownload()
        {
            if (dl == null)
            {
                string directory = System.IO.Path.Combine(System.Environment.CurrentDirectory, "Files");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                dl = new Download(this.urlTextBox.Text.Trim(), directory);
                dl.step = 102400;
            }
            if (IsPause)
            {
                IsPause = false;
            }
        }
        

    }
}