using System.IO;
using System.Net;

namespace CnDCopy.Kernel.LocationManagers.Ftp
{
    public class FtpDownloadRequest : DownloadRequest
    {
        public Stream FileStream { get; set; }
        public byte[] Buffer { get; set; }
        public FtpWebRequest Request { get; set; }
        public FtpWebResponse Response { get; set; }

        public FtpDownloadRequest(int buffSize)
        {
            BytesRead = 0;
            Buffer = new byte[buffSize];
            FileStream = null;
        }
    }
}