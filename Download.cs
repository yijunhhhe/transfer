using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace ResumeBrokenTransfer
{
    public class Download
    {
        const int ReadWriteTimeOut = 2 * 1000;//超时等待时间
        const int TimeOutWait = 5 * 1000;//超时等待时间
        
        long totalSize = 0;
        public long TotalSize { get { return this.totalSize; }
            set { this.totalSize = value; }
        }

        int currentSize;
        public int CurrentSize
        {
            get { return this.currentSize; }
            set { this.currentSize = value; }
        }

        bool isFinished;
        int bufferSize = 1024;
        private byte[] Buffer
        {
            get
            {
                if (this.bufferSize <= 0)
                {
                    this.bufferSize = 1024;
                }
                return new byte[this.bufferSize];
            }
        }
        FileStream fs;
        public FileStream Fs
        {
            get
            {
                return this.fs;
            }
            set
            {
                this.fs = value;
            }
        }
       // long step;
        string url;
        string directory;
        string fileName;
        string filePath;
        public string FilePath
        {
            get
            {
                return this.filePath;
            }
            set
            {
                this.filePath = value;
            }
        }
        public bool IsFinished { get; set; }
        long Step;
        public long step
        {
            get { return this.Step; }
            set { this.Step = value; }
        }

        public Download(string url, string directory)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            this.url = url;
            this.directory = directory;
            this.fileName = url.Substring(url.LastIndexOf('/') + 1);
            string fullName = Path.Combine(this.directory, fileName);
            int num = 1;
            while (File.Exists(fullName))
            {
                fullName = fullName.Insert(fullName.LastIndexOf('.'), num.ToString());
            }
            this.filePath = fullName;
            
            this.fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public void GetTotalSize()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(this.url);
            WebResponse response = request.GetResponse();
            this.totalSize = response.ContentLength;
            response.Dispose();
        }

        public void download()
        {
            long from = this.currentSize;
            if (from < 0)
            {
                from = 0;
            }

            long to = this.currentSize + this.step - 1;
            if (to >= this.totalSize && this.totalSize > 0)
            {
                to = this.totalSize - 1;
            }
            this.startDownload(from, to);
        }

        public void startDownload(long from, long to)
        {
            string url = this.url;
            if (this.totalSize == 0)
            {
                GetTotalSize();
            }
            if (from >= this.totalSize || this.currentSize >= this.totalSize)
            {
                this.IsFinished = true;
                return;
            }
            try
            {
                System.Net.ServicePointManager.DefaultConnectionLimit = 200;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ServicePoint.ConnectionLimit = 100;
                Console.WriteLine("come");
                request.AddRange("bytes", from, to);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string result = string.Empty;
                if (response != null)
                {
                    byte[] buffer = this.Buffer;
                    using (Stream stream = response.GetResponseStream())
                    {
                        
                        int readTotalSize = 0;
                        int size = stream.Read(buffer, 0, buffer.Length);
                        while (size > 0)
                        {
                            fs.Write(buffer, 0, size);
                            readTotalSize += size;
                            size = stream.Read(buffer, 0, buffer.Length);
                            fs.Flush();
                        }
                        this.currentSize += readTotalSize;

                        if (response.Headers["Content-Range"] == null)
                        {
                            this.IsFinished = true;
                        }
                    }
                }
            }
            catch (Exception e) { 
            
            }
            
        }



        public float CurrentProgress
        {

            get
            {
                if (this.totalSize != 0)
                {
                    return (float)this.currentSize * 100 / (float)this.totalSize;
                }
                else
                {
                    return 0;
                }
            }
        
        }

        
    }
}
