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
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ResumeBrokenTransfer
{
    delegate void ShowProgressDelegate (int totalStep, int currentStep);
    public partial class Form1 : Form
    {
        private Download dl;
        private Download dl2;
        private Download dl3;
        object locker = new object();
        int _ThreadCount = 5;
        int finishcount = 0;
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
            //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(this.url);
            //WebResponse response = request.GetResponse();
            //long totalSize = response.ContentLength;
            //long blockSize = totalSize % 3 == 0 ? totalSize / 3 : totalSize / 3 + 1;
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
                dl2 = new Download(this.urlTextBox.Text.Trim(), directory);
                dl3 = new Download(this.urlTextBox.Text.Trim(), directory);
                dl.step = 204800;
                dl2.step = 102400;
                dl3.step = 102400;
            }
            if (IsPause)
            {
                IsPause = false;
            }

            //Thread startDownload = new Thread(new ThreadStart(ThreadStart));
            ////startDownload.IsBackground = true;
            
            //Thread startDownload1 = new Thread(new ThreadStart(ThreadStart1));
            ////startDownload1.IsBackground = true;
           
            //Thread startDownload2 = new Thread(new ThreadStart(ThreadStart2));
            ////startDownload2.IsBackground = true;
            
            //startDownload.Start();
            //startDownload1.Start();
            //startDownload2.Start();
            //startDownload.Join();
            //startDownload1.Join();
            //startDownload2.Join();

            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(() => ThreadStart()));
            tasks.Add(Task.Factory.StartNew(() => ThreadStart1()));
            tasks.Add(Task.Factory.StartNew(() => ThreadStart2()));
            Task.WhenAll(tasks.ToArray()).Wait();
              
             
            Console.WriteLine("fuck");
        }                                                 

        private void ThreadStart()
        {
            object[] objs = new object[] { 100, 0 };
            dl.GetTotalSize();
            long totalSize = dl.TotalSize;
            dl.CurrentSize = 0;
            dl.TotalSize = totalSize % 3 == 0 ? totalSize/3 : totalSize/3+1;
            while (!dl.IsFinished && !IsPause)
            {
                dl.download();
                Thread.Sleep(200);
                objs[1] = (int)dl.CurrentProgress;

                MethodInvoker m = new MethodInvoker(() => {
                    this.progressBar1.Maximum = (int)objs[0];
                    this.progressBar1.Value = (int)objs[1];
                });
                this.progressBar1.Invoke(m); 
                //this.BeginInvoke(new ShowProgressDelegate(ShowProgress1), objs);
            }
            Console.WriteLine("done1");
            
        }
        private void ThreadStart1()
        {
            object[] objs = new object[] { 100, 0 };
            dl2.GetTotalSize();
            long totalSize = dl2.TotalSize;
            dl2.CurrentSize = (int)(totalSize % 3 == 0 ? totalSize / 3 : totalSize / 3 + 1);
            dl2.TotalSize = dl2.CurrentSize * 2;
            while (!dl2.IsFinished && !IsPause)
             {
                try
                {
                    dl2.download();
                    Thread.Sleep(200);
                    objs[1] = (int)dl2.CurrentProgress;
                    this.BeginInvoke (new ShowProgressDelegate(ShowProgress2), objs);
                }
                catch (Exception e)
                {

                }
               
            }
            Console.WriteLine("done2");
           
        }
        private void ThreadStart2()
        {
            object[] objs = new object[] { 100, 0 };
            dl3.GetTotalSize();
            long totalSize = dl3.TotalSize;
            dl3.CurrentSize = (int)(totalSize % 3 == 0 ? totalSize / 3 : totalSize / 3 + 1) * 2;
            
            while (!dl3.IsFinished && !IsPause)
            {
                dl3.download();
                Thread.Sleep(200);
                objs[1] = (int)dl3.CurrentProgress;
                this.BeginInvoke(new ShowProgressDelegate(ShowProgress3), objs);
            }
            Console.WriteLine("done3");
           
        }

        void ShowProgress1(int totalStep, int currentStep)
        {
            this.progressBar1.Maximum = totalStep;
            this.progressBar1.Value = currentStep;
        }

        void ShowProgress2(int totalStep, int currentStep)
        {
            this.progressBar2.Maximum = totalStep;
            this.progressBar2.Value = currentStep;
        }

        void ShowProgress3(int totalStep, int currentStep)
        {
            this.progressBar3.Maximum = totalStep;
            this.progressBar3.Value = currentStep;
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

        private void progressBar2_Click(object sender, EventArgs e)
        {

        }

        private void progressBar3_Click(object sender, EventArgs e)
        {

        }

    }
}