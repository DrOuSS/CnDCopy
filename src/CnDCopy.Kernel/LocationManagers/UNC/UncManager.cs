using System;
using System.Diagnostics;
using System.IO;

namespace CnDCopy.Kernel.LocationManagers.UNC
{
	public class UncManager : LocationManagerBase
	{
		public UncManager (Credentials credentials, ReplaceMode replaceMode) : base(credentials, replaceMode)
		{
			BufferSize = 2048;
		}
		
		/// <summary>
		/// Default value is 2048 bytes
		/// </summary>
		public int BufferSize { get; set; }

		#region implemented abstract members of LocationManagerBase
		public override void BeginRetreive (ILocation sourceLocation, Action<byte[]> bufferCallback, Action copyDone)
		{
			var uncRequest = new UncDownloadRequest (BufferSize);
			uncRequest.Buffering += bufferCallback;
			uncRequest.CopyDone += copyDone;
			
			var filePath = GetPathFromLocation (sourceLocation);
			var fi = new FileInfo (filePath);
			uncRequest.FileSize = fi.Length;
			
			using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				int bytesCount;
				while ((bytesCount = stream.Read(uncRequest.Buffer, 0, BufferSize)) > 0) {
					uncRequest.BytesRead += bytesCount;
					if (bytesCount == BufferSize)
						uncRequest.FireBuffering (uncRequest.Buffer);
					else {
						var lastBuffer = new byte[bytesCount];
						Array.Copy (uncRequest.Buffer, lastBuffer, bytesCount);
						uncRequest.FireBuffering (lastBuffer);
						Array.Clear (lastBuffer, 0, bytesCount);
					}
				}
				
				uncRequest.FireCopyDone ();
				Trace.TraceInformation (uncRequest.BytesRead + " bytes read on " + uncRequest.FileSize + " bytes.");
			}
		}

		public override PushRequest BeginPush (ILocation destinationLocation, ReplaceMode replaceMode)
		{
			var filePath = GetPathFromLocation (destinationLocation);
			var pushRequest = new UncPushRequest
			{OutputStream = File.Open(filePath, GetFileMode())};
			
			return pushRequest;
		}
		public override void Delete (ILocation location)
		{
			if (location.IsFolder)
				Directory.Delete (GetPathFromLocation (location));
			else
				File.Delete (GetPathFromLocation (location));
		}
		public override bool Exists (ILocation location)
		{
			if (location.IsFolder)
				return Directory.Exists (GetPathFromLocation (location));
				
			return File.Exists (GetPathFromLocation (location));
		}
		public override long GetSize (ILocation location)
		{
			if (location.IsFolder)
				throw new NotImplementedException ();

			var fi = new FileInfo (GetPathFromLocation (location));
			return fi.Length;
		}
		#endregion


		private FileMode GetFileMode ()
		{
			if (DefaultReplaceMode == ReplaceMode.Ignore)
				return FileMode.CreateNew;

			if (DefaultReplaceMode == ReplaceMode.Resume)
				return FileMode.Append;

			if ((DefaultReplaceMode & ReplaceMode.ReplaceIfDifferentSize) == ReplaceMode.ReplaceIfDifferentSize)
				return FileMode.Create;

			if (DefaultReplaceMode == ReplaceMode.Replace)
				return FileMode.Create;

			if (DefaultReplaceMode == ReplaceMode.Rename)
				return FileMode.CreateNew;

			if (DefaultReplaceMode == ReplaceMode.UserAsking)
				throw new NotImplementedException ("UserAsking is not not a valid ReplaceMode.");

			if ((DefaultReplaceMode & ReplaceMode.ReplaceIfNewer) == ReplaceMode.ReplaceIfNewer)
				return FileMode.Create;

			throw new NotSupportedException ("ReplaceMode not supported for UNC copy.");
		}

		private static string GetPathFromLocation (ILocation destinationLocation)
		{
			var filePath = destinationLocation.ItemUri.IsAbsoluteUri ? destinationLocation.ItemUri.LocalPath : destinationLocation.ItemUri.ToString ();
			return filePath;
		}
	}
}
