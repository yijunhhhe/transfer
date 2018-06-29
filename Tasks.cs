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
        //private Download dl;
        //private Download dl2;
        //private Download dl3;
        private long totalSize = 0;
        private string url;
        private int numThreads;
        private int from = -102400;
        private int to = -1;
        private int downloadEach = 102400;
        public int To
        {
            get { return this.to; }
            set { this.to = value; }
        }
        //IAccountService accountService = ServiceProvider.GetService<IAccountService>();
        private bool IsPause = false;
        private bool UIStop = false;
        private string fileName;
        private string fileFullName;
        private int bufferSize;
        private byte[] bytes;
        private FileStream fs;
        private List<Download> downloads;
        //private int bytesRead = -1;
        private int dlIndex = -1;

        private Object thisLock = new Object();
        private Object fsLock = new Object();
        public Tasks(string url, Form1 form, int numThreads)
        {
            this.url = url;
            this.numThreads = numThreads;
            this.form = form;
            this.fileName = url.Substring(url.LastIndexOf('/') + 1);
            this.fileName = fileName.Insert(this.fileName.LastIndexOf('.'), "final");
            this.fileFullName = Path.Combine(System.Environment.CurrentDirectory, "Files");
            this.fileFullName = Path.Combine(this.fileFullName, this.fileName);
            while (File.Exists(this.fileFullName))
            {
                this.fileFullName = this.fileFullName.Insert(this.fileFullName.LastIndexOf('.'), 1.ToString());
            }
            this.fs = new FileStream(this.fileFullName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            this.bufferSize = 1024 * 1024;
            this.bytes = new byte[bufferSize];
            this.downloads = new List<Download>();
            TotalSize();
        }

        //create a get total size method to totalSize
        public void TotalSize()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(this.url);
            WebResponse response = request.GetResponse();
            this.totalSize = response.ContentLength;
            response.Dispose();
        }
        public void StartDownload()
        {

            if (dlList.Count == 0)
            {
                string directory = System.IO.Path.Combine(System.Environment.CurrentDirectory, "Files");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                for (int i = 0; i < numThreads; i++)
                {
                    Download dll = new Download(this.url.Trim(), directory);
                   // dll.step = 102400;
                    dlList.Add(dll);
                }

            }
            if (this.form.isPause)
            {
                this.form.isPause = false;
            }
            //dl.GetTotalSize();
            //this.totalSize = dl.TotalSize;

            object[] parameters = new object[] { this, totalSize };
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

            for (int i = 0; i < numThreads; i++)
            {
                tasks.Add(Task.Factory.StartNew(() => ThreadStart()));
            }

            //Task.Factory.ContinueWhenAll(tasks.ToArray(), merge =>
            //{
            //    this.form.mre.Set(); 
            //    this.form.Invoke(new MethodInvoker(delegate
            //        {
            //            this.form.progressLabel.Text = "完成";
            //        }));
            //    this.form.backgroundWorker1.CancelAsync();
            //    this.form.backgroundWorker1.Dispose();
            //    Console.WriteLine("fuck");
            //    this.form.ExistTask = false;
            //});
        }

        private void ThreadStart()
        {
            while (to < totalSize - 1)
            {
                int newFrom = 0;
                int newTo = 0;
                int dlIndexx = 0;
                int readTotalSize = 0;
                lock (thisLock)
                {
                    dlIndex++;
                    if (dlIndex >= dlList.Count)
                    {
                        dlIndex = 0;
                    }

                    from = from + downloadEach;
                    to = to + downloadEach;
                    if (from >= totalSize)
                    {
                        return;
                    }
                    if (to >= totalSize - 1)
                    {
                        to = (int)totalSize - 1;
                    }
                    newFrom = from;
                    newTo = to;
                    dlIndexx = dlIndex;
                }

                dlList[dlIndexx].startDownload(newFrom, newTo);
                var stream = dlList[dlIndexx].Stream;
                int bytesRead = -1;
                if (this.form.isPause)
                {
                    this.form.mre.WaitOne();
                }
                //this.form.mre.Reset();
                lock (fsLock)
                {
                    readTotalSize = 0;
                    while ((bytesRead = stream.Read(bytes, 0, bufferSize)) > 0)
                    {
                        fs.Seek(newFrom + readTotalSize, SeekOrigin.Begin);
                        readTotalSize = readTotalSize + bytesRead;
                        fs.Write(bytes, 0, bytesRead);
                    }
                }
            }
        }




    }
}

//private void ThreadStart1()
//{
//    object[] objs = new object[] { 100, 0 };
//    dl2.GetTotalSize();
//    long totalSize = dl2.TotalSize;
//    dl2.CurrentSize = (int)(totalSize % 3 == 0 ? totalSize / 3 : totalSize / 3 + 1);
//    dl2.TotalSize = dl2.CurrentSize * 2;
//    while (!dl2.IsFinished)
//    {
//        if (this.form.isPause)
//        {
//            this.form.autoEvent2.WaitOne();
//        }
//            dl2.download();
//            Thread.Sleep(200);

//    }
//    Console.WriteLine("done2");

//}
//private void ThreadStart2()
//{
//    object[] objs = new object[] { 100, 0 };
//    dl3.GetTotalSize();
//    long totalSize = dl3.TotalSize;
//    dl3.CurrentSize = (int)(totalSize % 3 == 0 ? totalSize / 3 : totalSize / 3 + 1) * 2;
//    while (!dl3.IsFinished)
//    {
//        if (this.form.isPause)
//        {
//            this.form.autoEvent3.WaitOne();
//        }

//            dl3.download();
//            Thread.Sleep(200);

//    }
//    Console.WriteLine("done3");
//}

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

//var fileName = url.Substring(url.LastIndexOf('/') + 1);
//fileName = fileName.Insert(fileName.LastIndexOf('.'), "final");
//string fullName = Path.Combine(System.Environment.CurrentDirectory, "Files");
//fullName = Path.Combine(fullName, fileName);
//while (File.Exists(fullName))
//{
//    fullName = fullName.Insert(fullName.LastIndexOf('.'), 1.ToString());
//}
//FileStream fs = new FileStream(fullName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
//var bufferSize = 1024 * 1024;
//byte[] bytes = new byte[bufferSize];
//int bytesRead = -1;
//List<Download> downloads = new List<Download>();
////downloads.Add(dl);
////downloads.Add(dl2);
////downloads.Add(dl3);
//foreach (var eachDl in downloads)
//{
//    var eachFs = eachDl.Fs;
//    eachFs.Seek(0, SeekOrigin.Begin);
//    while ((bytesRead = eachFs.Read(bytes, 0, bufferSize)) > 0)
//    {
//        fs.Write(bytes, 0, bytesRead);
//    }
//    eachFs.Dispose();
//    File.Delete(eachDl.FilePath);
//}