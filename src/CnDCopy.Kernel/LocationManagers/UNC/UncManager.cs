using System;
using System.Diagnostics;
using System.IO;

namespace CnDCopy.Kernel.LocationManagers.UNC
{
    public class UncManager : LocationManagerBase
    {
        public UncManager(UncLocation location, ReplaceMode replaceMode) : base(location, replaceMode)
        {
            BufferSize = 2048;
            if (location.ItemUri.IsAbsoluteUri)
                _locationInfo = new FileInfo(location.ItemUri.AbsolutePath);
            else
                _locationInfo = new FileInfo(location.ItemUri.ToString());
        }

        private readonly FileInfo _locationInfo;

        /// <summary>
        /// Default value is 2048 bytes
        /// </summary>
        public int BufferSize { get; set; }

        public override void BeginRetreiveFile(Action<byte[]> bufferCallback, Action copyDone)
        {
            var uncRequest = new UncDownloadRequest(BufferSize);
            uncRequest.Buffering += bufferCallback;
            uncRequest.CopyDone += copyDone;

            var filePath = Location.ItemUri.IsAbsoluteUri ? Location.ItemUri.LocalPath : Location.ItemUri.ToString();
            var fi = new FileInfo(filePath);
            uncRequest.FileSize = fi.Length;

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
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

        public override PushRequest BeginPushFile()
        {
            var filePath = Location.ItemUri.IsAbsoluteUri ? Location.ItemUri.LocalPath : Location.ItemUri.ToString();
            var pushRequest = new UncPushRequest
                                  {OutputStream = File.Open(filePath, GetFileMode())};

            return pushRequest;
        }

        private FileMode GetFileMode()
        {
            if (ReplaceMode == ReplaceMode.Ignore)
                return FileMode.CreateNew;

            if(ReplaceMode == ReplaceMode.Resume)
                return FileMode.Append;

            if ((ReplaceMode & ReplaceMode.ReplaceIfDifferentSize) == ReplaceMode.ReplaceIfDifferentSize)
                return FileMode.Create;

            if (ReplaceMode == ReplaceMode.Replace)
                return FileMode.Create;

            if (ReplaceMode == ReplaceMode.Rename)
                return FileMode.CreateNew;

            if (ReplaceMode == ReplaceMode.UserAsking)
                throw new NotImplementedException("UserAsking is not not a valid ReplaceMode.");

            if ((ReplaceMode & ReplaceMode.ReplaceIfNewer) == ReplaceMode.ReplaceIfNewer)
                return FileMode.Create;

            throw new NotSupportedException("ReplaceMode not supported for UNC copy.");
        }

        public override void Delete()
        {
            _locationInfo.Delete();
        }

        public override bool Exists()
        {
            return _locationInfo.Exists;
        }

        public override long GetSize()
        {
            return _locationInfo.Length;
        }
    }
}
