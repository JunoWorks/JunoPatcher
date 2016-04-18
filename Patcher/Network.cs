using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;

namespace Patcher
{
    class Network
    {

        public Network()
        {

        }

        public class ftpDownloader
        {
            public WebClient ftpClient;
            public string currentFileName;
            public long currentFileSize;

            public ftpDownloader()
            {
                ftpClient = new WebClient();
            }

            public void download(string url, string savePath, string user, string password)
            {
                currentFileName = Path.GetFileName(new Uri(url).LocalPath);
                currentFileSize = getFileSize(url, user, password);
                ftpClient.Credentials = new NetworkCredential(user, password);
                ftpClient.DownloadFileAsync(new Uri(url), savePath);
            }

            private long getFileSize(string url, string user, string password)
            {
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
                request.Proxy = null;
                request.Credentials = new NetworkCredential(user, password);
                request.Method = WebRequestMethods.Ftp.GetFileSize;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                long size = response.ContentLength;
                response.Close();
                return size;
            }
        }

        public static Task<string> MakeAsyncRequest(string url, string contentType)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = contentType;
            request.Method = WebRequestMethods.Http.Get;
            request.Timeout = 20000;
            request.Proxy = null;

            Task<WebResponse> task = Task.Factory.FromAsync(
                request.BeginGetResponse,
                asyncResult => request.EndGetResponse(asyncResult),
                (object)null);

            return task.ContinueWith(t => ReadStreamFromResponse(t.Result));
        }

        private static string ReadStreamFromResponse(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(responseStream))
            {
                //Need to return this response 
                string strContent = sr.ReadToEnd();
                return strContent;
            }
        }
    }
}
