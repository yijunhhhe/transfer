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
        private List<Download> dlList = new List<Download>();
        private Download dl;
        private Download dl2;
        private Download dl3;
        private long totalSize = 0;
        private string url;
        private int numThreads;
        
        //IAccountService accountService = ServiceProvider.GetService<IAccountService>();
        private bool IsPause = false;
        private bool UIStop = false;

        public Tasks(string url, Form1 form, int numThreads)
        {
            this.url = url;
            this.numThreads = numThreads;           
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

            object[] parameters = new object[] { dl, dl2, dl3 };
            if (!this.form.backgroundWorker1.IsBusy)
            {
                this.form.backgroundWorker1.RunWorkerAsync(parameters);
            }
            var url = this.url.Trim();
            Downloading();
        }
        public void Downloading()
        {
            List<Task> tasks = new List<Task>();
            // tasks.Add(Task.Factory.StartNew(() => ThreadStart3()));
            tasks.Add(Task.Factory.StartNew(() => ThreadStart()));
            tasks.Add(Task.Factory.StartNew(() => ThreadStart1()));
            tasks.Add(Task.Factory.StartNew(() => ThreadStart2()));
            Task.Factory.ContinueWhenAll(tasks.ToArray(), merge =>
            {
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
                this.form.Invoke(new MethodInvoker(delegate
                    {
                    this.form.progressLabel.Text = "done";
                        }));
                Console.WriteLine("fuck");
                this.form.ExistTask = false;
            });

        }
      
        private void ThreadStart()
        {
            object[] objs = new object[] { 100, 0 };
            dl.GetTotalSize();
            long totalSize = dl.TotalSize;
            dl.CurrentSize = 0;
            dl.TotalSize = totalSize % 3 == 0 ? totalSize / 3 : totalSize / 3 + 1;
            while (!dl.IsFinished)
            {
                if (this.form.isPause)
                {
                    this.form.autoEvent1.WaitOne();
                }
              
                    dl.download();
                    Thread.Sleep(200);
                
            }
            Console.WriteLine( "done1");
        }

        private void ThreadStart1()
        {
            object[] objs = new object[] { 100, 0 };
            dl2.GetTotalSize();
            long totalSize = dl2.TotalSize;
            dl2.CurrentSize = (int)(totalSize % 3 == 0 ? totalSize / 3 : totalSize / 3 + 1);
            dl2.TotalSize = dl2.CurrentSize * 2;
            while (!dl2.IsFinished)
            {
                if (this.form.isPause)
                {
                    this.form.autoEvent2.WaitOne();
                }
                    dl2.download();
                    Thread.Sleep(200);
                 
            }
            Console.WriteLine("done2");

        }
        private void ThreadStart2()
        {
            object[] objs = new object[] { 100, 0 };
            dl3.GetTotalSize();
            long totalSize = dl3.TotalSize;
            dl3.CurrentSize = (int)(totalSize % 3 == 0 ? totalSize / 3 : totalSize / 3 + 1) * 2;
            while (!dl3.IsFinished)
            {
                if (this.form.isPause)
                {
                    this.form.autoEvent3.WaitOne();
                }
               
                    dl3.download();
                    Thread.Sleep(200);
                        
            }
            Console.WriteLine("done3");
        }


    }
}



 //private void mergeThread()
 //       {
 //           var fileName = url.Substring(url.LastIndexOf('/') + 1);
 //           fileName = fileName.Insert(fileName.LastIndexOf('.'), "final");
 //           string fullName = Path.Combine(System.Environment.CurrentDirectory, "Files");
 //           fullName = Path.Combine(fullName, fileName);
 //           while (File.Exists(fullName))
 //           {
 //               fullName = fullName.Insert(fullName.LastIndexOf('.'), 1.ToString());
 //           }
 //           FileStream fs = new FileStream(fullName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
 //           var bufferSize = 1024 * 1024;
 //           byte[] bytes = new byte[bufferSize];
 //           int bytesRead = -1;
 //           List<Download> downloads = new List<Download>();
 //           downloads.Add(dl);
 //           downloads.Add(dl2);
 //           downloads.Add(dl3);
 //           foreach (var eachDl in downloads)
 //           {
 //               var eachFs = eachDl.Fs;
 //               eachFs.Seek(0, SeekOrigin.Begin);
 //               while ((bytesRead = eachFs.Read(bytes, 0, bufferSize)) > 0)
 //               {
 //                   fs.Write(bytes, 0, bytesRead);
 //               }
 //               eachFs.Dispose();
 //               File.Delete(eachDl.FilePath);
 //           }
           
 //           Console.WriteLine("fuck");
            
 //       }



  //private void ThreadStart3()
  //      {
  //          float currentSize = (dl.CurrentProgress + dl2.CurrentProgress + dl3.CurrentProgress) / 3;

  //          while ((dl.CurrentProgress + dl2.CurrentProgress + dl3.CurrentProgress) / 3 != 100.0)
  //          {
  //              this.form.Invoke(new MethodInvoker(delegate
  //              {
  //                  this.form.progressBar1.Value = (int)(dl.CurrentProgress + dl2.CurrentProgress + dl3.CurrentProgress) / 3;
  //                  //this.form.progressLabel.Text = ((int)(dl.CurrentProgress + dl2.CurrentProgress + dl3.CurrentProgress) / 3).ToString() + "%";
  //                  Thread.Sleep(500);
  //              }));
  //          }
  //          this.form.Invoke(new MethodInvoker(delegate
  //          {
  //              this.form.progressBar1.Value = (int)(dl.CurrentProgress + dl2.CurrentProgress + dl3.CurrentProgress) / 3;
  //              //this.form.progressLabel.Text = ((int)(dl.CurrentProgress + dl2.CurrentProgress + dl3.CurrentProgress) / 3).ToString();
  //              //Thread.Sleep(500);
  //          }));
  //          Console.WriteLine("thrads 1");
  //      }