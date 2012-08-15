using System.Diagnostics;
using System.IO;
using System.Net;

namespace CnDCopy.Kernel.LocationManagers.Ftp
{
    public class FtpPushRequest : PushRequest
    {
        public FtpWebRequest Request { get; set; }
        public Stream OutputStream { get; set; }

        public override void BufferWriteCallback(byte[] buffer)
        {
            OutputStream.Write(buffer, 0, buffer.Length);
        }

        public override void OnCopyDone()
        {
            try
            {
                OutputStream.Close();
                OutputStream.Dispose();

                var response = (FtpWebResponse)Request.GetResponse();
                Trace.TraceInformation("FTP response status is " + response.StatusDescription);
            }
            finally 
            {
                base.OnCopyDone();
            }
        }
    }
}