using System.IO;
using System.Net;

namespace CnDCopy.Kernel.LocationManagers.UNC
{
    public class UncDownloadRequest : DownloadRequest
    {
        public Stream FileStream { get; set; }
        public byte[] Buffer { get; set; }
        public FtpWebRequest Request { get; set; }
        public FtpWebResponse Response { get; set; }

        public BinaryWriter Writer { get; set; }
        public UncDownloadRequest(int buffSize)
        {
            BytesRead = 0;
            Buffer = new byte[buffSize];
            FileStream = null;
        }
    }
}