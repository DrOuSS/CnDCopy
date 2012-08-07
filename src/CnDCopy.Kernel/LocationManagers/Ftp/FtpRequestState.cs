using System.IO;
using System.Net;

namespace CnDCopy.Kernel.LocationManagers.Ftp
{
    public class FtpRequestState : CopyRequestState
    {
        public Stream FileStream { get; set; }
        public byte[] Buffer { get; set; }
        public FtpWebRequest Request { get; set; }
        public FtpWebResponse Response { get; set; }

        public FtpRequestState(int buffSize)
        {
            BytesRead = 0;
            Buffer = new byte[buffSize];
            FileStream = null;
        }
    }
}