using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ResumeBrokenTransfer
{
    class Tasks
    {
        Form1 form;
        private Download dl;
        private Download dl2;
        private Download dl3;
        private long totalSize = 0;
        private string url;
        SynchronizationContext m_SyncContext;
        //IAccountService accountService = ServiceProvider.GetService<IAccountService>();
        private bool IsPause = false;
        private bool UIStop = false;

        public Tasks(string url, SynchronizationContext m_SyncContext, Form1 form)
        {
            this.url = url;
            this.m_SyncContext = m_SyncContext;
            this.form = form;
        }




        public void StartDownload()
        {

            if (dl == null)
            {
                string directory = System.IO.Path.Combine(System.Environment.CurrentDirectory, "Files");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                dl = new Download(this.url.Trim(), directory);
                dl2 = new Download(this.url.Trim(), directory);
                dl3 = new Download(this.url.Trim(), directory);
                dl.step = 102400;
                dl2.step = 102400;
                dl3.step = 102400;
            }
            else
            {

            }
            if (IsPause)
            {
                IsPause = false;
            }
            dl.GetTotalSize();
            this.totalSize = dl.TotalSize;
            //backgroundWorker1.RunWorkerAsync();
            var url = this.url.Trim();
            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(() => ThreadStart3()));
            tasks.Add(Task.Factory.StartNew(() => ThreadStart()));
            tasks.Add(Task.Factory.StartNew(() => ThreadStart1()));
            tasks.Add(Task.Factory.StartNew(() => ThreadStart2()));
            Task.WhenAll(tasks).Wait();
            //tasks.Add(Task.Factory.StartNew(() => progressThread()));
           // Task.WaitAll(tasks.ToArray());

            var fileName = url.Substring(url.LastIndexOf('/') + 1);
            fileName = fileName.Insert(fileName.LastIndexOf('.'), "final");
            string fullName = Path.Combine(System.Environment.CurrentDirectory, "Files");
            fullName = Path.Combine(fullName, fileName);
            while (File.Exists(fullName))
            {
                fullName = fullName.Insert(fullName.LastIndexOf('.'), 1.ToString());
            }
            FileStream fs = new FileStream(fullName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            var bufferSize = 1024 * 1024;
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

        delegate void SetTextCallback(string text);
        delegate void ShowProgressDelegate(int step);

        void ShowProgress(int step)
        {
            if (this.form.progressBar1.InvokeRequired)
            {
                ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);
                this.form.BeginInvoke(showProgress, new object[] { step });
            }
            else
            {
                this.form.progressBar1.Value = step;
                this.form.progressBar1.Update();
                this.form.progressBar1.Refresh();
            }
            
        }

        //private void SetTextSafePost(object text)
        //{

        //    float currentSize = (dl.CurrentProgress + dl2.CurrentProgress + dl3.CurrentProgress) / 3;
        //    ShowProgress((int)currentSize); 
        //}

        //private void callback(IAsyncResult ar)
        //{
        //    Console.WriteLine("Asdf");
        //    this.form.progressBar1.Value = 20;                                     
        //}


        private void ThreadStart3()
        {
            this.form.BeginInvoke(new MethodInvoker(delegate
            {
                this.form.progressBar1.Value = 30;
            }));
            Console.WriteLine("thrads 1");
        }
        private void ThreadStart()
        {

            object[] objs = new object[] { 100, 0 };   
            dl.GetTotalSize();
            long totalSize = dl.TotalSize;
            dl.CurrentSize = 0;
            dl.TotalSize = totalSize % 3 == 0 ? totalSize / 3 : totalSize / 3 + 1;
            while (!dl.IsFinished && !IsPause)
            {
                dl.download();
                Thread.Sleep(200);
                //ShowProgress((int)dl.CurrentProgress);
                //this.form.BeginInvoke(new  MethodInvoker(delegate{
                //     this.form.progressBar1.Value = 30;
                //}));
                
                //SetTextCallback setText = new SetTextCallback(SetTextSafePost);
                //setText.BeginInvoke("thread3", null, null);
                
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


    }
}
