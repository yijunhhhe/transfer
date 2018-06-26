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
        private long totalSize = 0;
        private Tasks task;
        private bool IsPause = false;
        //private bool pauseWorker = false;
        private bool existTask = false;
        private bool finished = false;
        private int numThreads = 3;
        public bool Finished
        {
            get { return this.finished; }               
            set { this.finished = value; }
        }
        public bool ExistTask
        {
            get { return this.existTask; }
            set { this.existTask = value; }
        }
        public AutoResetEvent autoEvent1 = new AutoResetEvent(false);
        public AutoResetEvent autoEvent2 = new AutoResetEvent(false);
        public AutoResetEvent autoEvent3 = new AutoResetEvent(false);
        public bool isPause
        {
            get { return this.IsPause; }
            set { this.IsPause = value; }
        }
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
            //StartDownload();
            if (!existTask)
            {
                this.progressLabel.Text = "";
                task = new Tasks(urlTextBox.Text, this, numThreads);
                existTask = true;
                task.StartDownload();
            }
            else
            {
                IsPause = false;
                autoEvent1.Set();
                autoEvent2.Set();
                autoEvent3.Set();
               
            }  
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
            if (existTask)
            {
                IsPause = true;
                //pauseWorker = true;              
            }
            
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
      
        private void progressLabel_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
           
                //continue process... your code here
                BackgroundWorker worker = sender as BackgroundWorker;
                object[] parameters = e.Argument as object[];
                Download dl = (Download)parameters[0];
                Download dl2 = (Download)parameters[1];
                Download dl3 = (Download)parameters[2];

                while ((dl.CurrentProgress + dl2.CurrentProgress + dl3.CurrentProgress) / 3 != 100.0)
                {
                    float currentSize = (dl.CurrentProgress + dl2.CurrentProgress + dl3.CurrentProgress) / 3;
                    System.Threading.Thread.Sleep(500);
                    worker.ReportProgress((int)currentSize);
                    // this.progressBar1.Value = (int)currentSize;
                }
                worker.ReportProgress((int)((dl.CurrentProgress + dl2.CurrentProgress + dl3.CurrentProgress) / 3));
            
        }

  
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
            //this.progressLabel.Text = (e.ProgressPercentage.ToString() + "%");
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



// private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
//{
//    BackgroundWorker worker = sender as BackgroundWorker;

//    for (int i = 1; i <= 10; i++)
//    {
//        if (worker.CancellationPending == true)
//        {
//            e.Cancel = true;
//            break;
//        }
//        else
//        {
//            // Perform a time consuming operation and report progress.
//            System.Threading.Thread.Sleep(500);
//            worker.ReportProgress(i * 10);
//        }
//    }
//}

// This event handler updates the progress.


// This event handler deals with the results of the background operation.