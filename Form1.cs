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
    //delegate void ShowProgressDelegate (int totalStep, int currentStep);
    public partial class Form1 : Form
    {
        private Download dl;
        private Download dl2;
        private Download dl3;
        private long totalSize = 0;
        private Tasks task;
        SynchronizationContext m_SyncContext = null;
        //IAccountService accountService = ServiceProvider.GetService<IAccountService>();
        private bool IsPause = false;
        public Form1()
        {
            InitializeComponent();
            this.urlTextBox.Text = "https://www.tutorialspoint.com/cplusplus/cpp_tutorial.pdf";
            m_SyncContext = SynchronizationContext.Current;
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
            task = new Tasks(urlTextBox.Text, this.m_SyncContext, this);
            backgroundWorker1.RunWorkerAsync();
            task.StartDownload();
           
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

            for (int i = 1; i <= 10; i++)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    // Perform a time consuming operation and report progress.
                    System.Threading.Thread.Sleep(500);
                    worker.ReportProgress(i * 10);
                }
            }
        }

        // This event handler updates the progress.
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            resultLabel.Text = (e.ProgressPercentage.ToString() + "%");
            
        }

        // This event handler deals with the results of the background operation.
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                resultLabel.Text = "Canceled!";
            }
            else if (e.Error != null)
            {
                resultLabel.Text = "Error: " + e.Error.Message;
            }
            else
            {
                resultLabel.Text = "Done!";
            }
        }

        private void progressLabel_Click(object sender, EventArgs e)
        {

        }
    

       
        //private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    BackgroundWorker worker = sender as BackgroundWorker;
        //    while ((int)(dl.CurrentProgress + dl2.CurrentProgress + dl3.CurrentProgress) / 3 != 100.0)
        //    {
        //        float currentSize = (dl.CurrentProgress + dl2.CurrentProgress + dl3.CurrentProgress) / 3;
        //        //System.Threading.Thread.Sleep(500);
        //        worker.ReportProgress((int)currentSize);
        //       // this.progressBar1.Value = (int)currentSize;
        //    }
        //    worker.ReportProgress((int)((dl.CurrentProgress + dl2.CurrentProgress + dl3.CurrentProgress)/3));
        //}

        //private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    this.progressBar1.Value = e.ProgressPercentage;
        //    this.progressLabel.Text = (e.ProgressPercentage.ToString() + "%");
        //}

        //private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    this.progressLabel.Text = "done";
        //}

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