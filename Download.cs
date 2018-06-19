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
        int currentSize;
        bool isFinished;
        byte[] Buffer;
        FileStream fs;
        long step;
        string url;
        string directory;
        string fileName;
        string filePath;
        public long Step
        {
            get { return this.step; }
            set { this.step = value; }
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
        
        public static long GetFileContentLength(string url)
        {
            HttpWebRequest request = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = TimeOutWait;
                request.ReadWriteTimeout = ReadWriteTimeOut;
                WebResponse response = request.GetResponse();
                request.Abort();
                return response.ContentLength;
            }
            catch (Exception e)
            {
                if (request != null)
                {
                    request.Abort();
                }
                return 0;

            }
        }

        public void Download(string url)
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
            this.Download(from, to, url);
        }

        public void Download(long from, long to, string url)
        {
            if (this.totalSize == 0)
            {
                totalSize = GetFileContentLength(url);
            }
            if (from >= this.totalSize || this.currentSize >= this.totalSize)
            {
                this.isFinished = true;
                return;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
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
                    }
                    this.currentSize += readTotalSize;

                    if (response.Headers["Content-Range"] == null)
                    {
                        this.isFinished = true;
                    }
                }
            }
        }
    }
}
