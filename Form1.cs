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
        private long totalSize = 0;
        private Tasks task;
        //IAccountService accountService = ServiceProvider.GetService<IAccountService>();
        private bool IsPause = false;
        public Form1()
        {
            InitializeComponent();
            this.urlTextBox.Text = "https://www.tutorialspoint.com/cplusplus/cpp_tutorial.pdf";
           
            //long blockSize = totalSize % 3 == 0 ? totalSize / 3 : totalSize / 3 + 1;
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
            
            //StartDownload();
            task = new Tasks(urlTextBox.Text);
            task.StartDownload();
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
            else
            {
                dl = null;
                dl2 = null;
                dl3 = null;
                backgroundWorker1.CancelAsync();
                backgroundWorker1.Dispose();
                StartDownload();
            }
            if (IsPause)
            {
                IsPause = false;
            }
            dl.GetTotalSize();
            this.totalSize = dl.TotalSize;
            backgroundWorker1.RunWorkerAsync();
            var url = this.urlTextBox.Text.Trim();
            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(() => ThreadStart()));
            tasks.Add(Task.Factory.StartNew(() => ThreadStart1()));
            tasks.Add(Task.Factory.StartNew(() => ThreadStart2()));
            
            //tasks.Add(Task.Factory.StartNew(() => progressThread()));
            Task.WhenAll(tasks.ToArray()).Wait();
            
            var fileName = url.Substring(url.LastIndexOf('/') + 1) ;
            fileName = fileName.Insert(fileName.LastIndexOf('.'),"final");
            string fullName = Path.Combine(System.Environment.CurrentDirectory, "Files");
            fullName = Path.Combine(fullName, fileName);
            FileStream fs = new FileStream(fullName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            var bufferSize = 1024*1024;
            byte[] bytes = new byte[bufferSize];
            int bytesRead = -1;
            List<Download> downloads = new List<Download>();
            downloads.Add(dl);
            downloads.Add(dl2);
            downloads.Add(dl3);
            foreach (var eachDl in downloads)
            {
                var eachFs = eachDl.Fs;
                eachFs.Seek(0, SeekOrigin.Begin);
                while ((bytesRead = eachFs.Read(bytes, 0, bufferSize)) > 0)
                {
                    fs.Write(bytes, 0, bytesRead);
                }
                eachFs.Dispose();
                File.Delete(eachDl.FilePath);
            }
           
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

                //MethodInvoker m = new MethodInvoker(() => {
                //    this.progressBar1.Maximum = (int)objs[0];
                //    this.progressBar1.Value = (int)objs[1];
                //});
                //this.progressBar1.Invoke(m); 
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
                    //objs[1] = (int)dl2.CurrentProgress;
                    //this.BeginInvoke (new ShowProgressDelegate(ShowProgress2), objs);
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
                //objs[1] = (int)dl3.CurrentProgress;
                //this.BeginInvoke(new ShowProgressDelegate(ShowProgress3), objs);
            }
            Console.WriteLine("done3");
           
        }

        void ShowProgress1(int totalStep, int currentStep)
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

        private void progressBar2_Click(object sender, EventArgs e)
        {

        }

        private void progressBar3_Click(object sender, EventArgs e)
        {

        }

       
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            while ((int)(dl.CurrentProgress + dl2.CurrentProgress + dl3.CurrentProgress) / 3 != 100.0)
            {
                float currentSize = (dl.CurrentProgress + dl2.CurrentProgress + dl3.CurrentProgress) / 3;
                //System.Threading.Thread.Sleep(500);
                worker.ReportProgress((int)currentSize);
               // this.progressBar1.Value = (int)currentSize;
            }
            worker.ReportProgress((int)((dl.CurrentProgress + dl2.CurrentProgress + dl3.CurrentProgress)/3));
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
            this.progressLabel.Text = (e.ProgressPercentage.ToString() + "%");
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.progressLabel.Text = "done";
        }

     }
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