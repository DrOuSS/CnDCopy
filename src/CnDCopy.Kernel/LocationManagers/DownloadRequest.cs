using System;

namespace CnDCopy.Kernel.LocationManagers
{
    public class DownloadRequest
    {
        public int BytesRead { get; set; }
        public long FileSize { get; set; }
        public event Action CopyDone = () => { };
        public event Action<byte[]> Buffering = bytes => { };


        public void FireCopyDone()
        {
            CopyDone();
        }

        public void FireBuffering(byte[] data)
        {
            Buffering(data);
        }
    }
}