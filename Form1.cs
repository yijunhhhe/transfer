using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ResumeBrokenTransfer
{
    delegate void ShowProgressDelegate (int totalStep, int currentStep);
    public partial class Form1 : Form
    {
        private Download dl;
        //IAccountService accountService = ServiceProvider.GetService<IAccountService>();
        private bool IsPause = false;
        public Form1()
        {
            InitializeComponent();
            this.urlTextBox.Text = "https://www.tutorialspoint.com/cplusplus/cpp_tutorial.pdf";          
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
                dl.step = 204800;
            }
            if (IsPause)
            {
                IsPause = false;
            }

            Thread startDownload = new Thread(ThreadStart);
            startDownload.IsBackground = true;
            startDownload.Start();
        }

        private void ThreadStart()
        {
            object[] objs = new object[] { 100, 0 };
            while (!dl.IsFinished && !IsPause)
            {
                dl.download();
                Thread.Sleep(200);
                objs[1] = (int)dl.CurrentProgress;
                this.Invoke(new ShowProgressDelegate(ShowProgress), objs);
            }
            Console.WriteLine("done");
        }

        void ShowProgress(int totalStep, int currentStep)
        {
            this.progressBar1.Maximum = totalStep;
            this.progressBar1.Value = currentStep;
        }

        private void urlTextBox_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void pause_button(object sender, EventArgs e)
        {
            IsPause = true;
        }

        private void resume_button(object sender, EventArgs e)
        {     
            IsPause = false;
            StartDownload();
        }

    }
}