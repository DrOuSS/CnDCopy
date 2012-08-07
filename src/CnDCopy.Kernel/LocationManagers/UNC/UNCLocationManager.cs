using System;
using System.Diagnostics;
using System.IO;

namespace CnDCopy.Kernel.LocationManagers.UNC
{
    public class UncLocationManager : ILocationManager
    {
        public UncLocationManager()
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


        public void BeginRetreiveFile(ILocation source, Action<byte[]> bufferCallback, Action copyDone)
        {
            var uncRequest = new UncRequestState(BufferSize);
            uncRequest.Buffering += bufferCallback;
            uncRequest.CopyDone += copyDone;

            var fi = new FileInfo(source.ItemUri.LocalPath);
            uncRequest.FileSize = fi.Length;

            using (var stream = File.Open(source.ItemUri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int bytesCount;
                while((bytesCount = stream.Read(uncRequest.Buffer, 0, BufferSize)) > 0)
                {
                    uncRequest.BytesRead += bytesCount;
                    if (bytesCount == BufferSize)
                        uncRequest.FireBuffering(uncRequest.Buffer);
                    else
                    {
                        var lastBuffer = new byte[bytesCount];
                        Array.Copy(uncRequest.Buffer, lastBuffer, bytesCount);
                        uncRequest.FireBuffering(lastBuffer);
                        Array.Clear(lastBuffer, 0, bytesCount);
                    }
                }

                uncRequest.FireCopyDone();
                Trace.TraceInformation(uncRequest.BytesRead + " bytes read on " + uncRequest.FileSize + " bytes.");
            }
        }

        public void PushFile(ILocation destination)
        {
            throw new NotImplementedException();
        }
    }
}
