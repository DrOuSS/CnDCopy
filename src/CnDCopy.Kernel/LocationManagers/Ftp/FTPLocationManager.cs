using System;
using System.Diagnostics;
using System.Net;

namespace CnDCopy.Kernel.LocationManagers.Ftp
{
    public class FtpLocationManager : ILocationManager
    {
        public FtpLocationManager()
        {
            BufferSize = 2048;
            UseBinary = true;
            EnableSsl = false;
            UsePassive = false;
        }

        /// <summary>
        /// Default value is 2048 bytes
        /// </summary>
        public int BufferSize { get; set; }
        /// <summary>
        /// Default value is true.
        /// </summary>
        public bool UseBinary { get; set; }
        /// <summary>
        /// Default value is false.
        /// </summary>
        public bool EnableSsl { get; set; }
        /// <summary>
        /// Default value is false;
        /// </summary>
        public bool UsePassive { get; set; }

        public void PushFile(ILocation destination)
        {
            var request = (FtpWebRequest)WebRequest.Create(destination.ItemUri);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            using (var response = request.GetResponse())
            {
                response.Close();
            }
        }

        public void BeginRetreiveFile(ILocation source, Action<byte[]> bufferCallback, Action copyDone)
        {
            var ftpRequest = new FtpRequestState(BufferSize);
            ftpRequest.Buffering += bufferCallback;
            ftpRequest.CopyDone += copyDone;

            GetFileSize(source, ftpRequest);
            BeginDownload(source, ftpRequest);
        }

        private void BeginDownload(ILocation source, FtpRequestState ftpRequest)
        {
            ftpRequest.Request = CreateRequest(source.ItemUri, WebRequestMethods.Ftp.DownloadFile, source.Credentials);
            ftpRequest.Response = (FtpWebResponse) ftpRequest.Request.GetResponse();

            ftpRequest.FileStream = ftpRequest.Response.GetResponseStream();
            ftpRequest.Buffer = new byte[BufferSize];

            ftpRequest.FileStream.BeginRead(ftpRequest.Buffer, 0, BufferSize, DownloadCallback, ftpRequest);
        }

        private void GetFileSize(ILocation source, FtpRequestState ftpRequest)
        {
            ftpRequest.Request = CreateRequest(source.ItemUri, WebRequestMethods.Ftp.GetFileSize, source.Credentials);
            using (var response = ftpRequest.Request.GetResponse())
                ftpRequest.FileSize = response.ContentLength;
        }

        public FtpWebRequest CreateRequest(Uri uri, string methodRequest, Credentials credentials)
        {
            var request = (FtpWebRequest)WebRequest.Create(uri);
            request.ConnectionGroupName = Guid.NewGuid().ToString();
            request.Method = methodRequest;
            if (credentials != null)
                request.Credentials = new NetworkCredential(credentials.Username, credentials.Password);

            request.UseBinary = UseBinary;
            request.EnableSsl = EnableSsl;
            request.KeepAlive = true;
            request.UsePassive = UsePassive;

            return request;
        }

        private void DownloadCallback(IAsyncResult ar)
        {
            var ftpRequest = (FtpRequestState)ar.AsyncState;
            var bytesRead = ftpRequest.FileStream.EndRead(ar);

            if (bytesRead > 0)
            {
                ftpRequest.BytesRead += bytesRead;
                if (bytesRead == BufferSize)
                    ftpRequest.FireBuffering(ftpRequest.Buffer);
                else
                {
                    var lastBuffer = new byte[bytesRead];
                    Array.Copy(ftpRequest.Buffer, lastBuffer, bytesRead);
                    ftpRequest.FireBuffering(lastBuffer);
                    Array.Clear(lastBuffer, 0, bytesRead);
                }

                ftpRequest.FileStream.BeginRead(ftpRequest.Buffer, 0, BufferSize, DownloadCallback, ftpRequest);
            }
            else
            {
                ftpRequest.FileStream.Close();
                ftpRequest.Response.Close();

                ftpRequest.FireCopyDone();

                Trace.TraceInformation(ftpRequest.BytesRead + " bytes read on " + ftpRequest.FileSize + " bytes.");
            }
        }
    }
}
