using System;
using System.Diagnostics;
using System.Net;

namespace CnDCopy.Kernel.LocationManagers.Ftp
{
	public class FtpManager : LocationManagerBase, IStreamableLocationManager
	{
		public FtpManager (Credentials credentials) : base(credentials)
		{
			BufferSize = 2048;
			UseBinary = true;
			EnableSsl = false;
			UsePassive = false;
		}

		protected override void Dispose (bool disposing)
		{
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

		#region implemented abstract members of LocationManagerBase

		public override void Delete (ILocation location)
		{
			var request = CreateRequest (location.ItemUri, WebRequestMethods.Ftp.DeleteFile, Credentials);
			request.GetResponse ();
		}

		public override bool Exists (ILocation location)
		{
			try {
				GetSize (location);
				return true;
			} catch (WebException error) {
				var response = (FtpWebResponse)error.Response;
				if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
					return false;

				throw;
			}
		}

		public override long GetSize (ILocation location)
		{
			if (!location.IsFolder) {
				var ftpRequest = new FtpDownloadRequest (BufferSize);
				GetFileSize (location, ftpRequest);
			
				return ftpRequest.FileSize;
			} else
				throw new NotImplementedException ();
		}

		#endregion	

		#region IStreamableLocationManager implementation

		public void BeginRetreive (ILocation sourceLocation, Action<byte[]> bufferCallback, Action copyDone)
		{
			var ftpRequest = new FtpDownloadRequest (BufferSize);
			ftpRequest.Buffering += bufferCallback;
			ftpRequest.CopyDone += copyDone;
			
			GetFileSize (sourceLocation, ftpRequest);
			BeginDownload (sourceLocation, ftpRequest);
		}
		
		public PushRequest BeginPush (ILocation destinationLocation, ReplaceMode replaceMode)
		{
			var request = CreateRequest (destinationLocation.ItemUri, WebRequestMethods.Ftp.UploadFile, Credentials);
			var pushRequest = new FtpPushRequest
			{
				Request = request,
				OutputStream = request.GetRequestStream(),
			};
			
			return pushRequest;
		}

		#endregion	
		
        #region Download

		private void BeginDownload (ILocation source, FtpDownloadRequest ftpRequest)
		{
			ftpRequest.Request = CreateRequest (source.ItemUri, WebRequestMethods.Ftp.DownloadFile, Credentials);
			ftpRequest.Response = (FtpWebResponse)ftpRequest.Request.GetResponse ();

			ftpRequest.FileStream = ftpRequest.Response.GetResponseStream ();
			ftpRequest.Buffer = new byte[BufferSize];

			Debug.Assert (ftpRequest.FileStream != null, "FileStream shall be not null.");
			ftpRequest.FileStream.BeginRead (ftpRequest.Buffer, 0, BufferSize, DownloadCallback, ftpRequest);
		}

		private void GetFileSize (ILocation source, FtpDownloadRequest ftpRequest)
		{
			ftpRequest.Request = CreateRequest (source.ItemUri, WebRequestMethods.Ftp.GetFileSize, Credentials);
			using (var response = ftpRequest.Request.GetResponse())
				ftpRequest.FileSize = response.ContentLength;
		}

		private void DownloadCallback (IAsyncResult ar)
		{
			var ftpRequest = (FtpDownloadRequest)ar.AsyncState;
			var bytesRead = ftpRequest.FileStream.EndRead (ar);

			if (bytesRead > 0) {
				ftpRequest.BytesRead += bytesRead;
				if (bytesRead == BufferSize)
					ftpRequest.FireBuffering (ftpRequest.Buffer);
				else {
					var lastBuffer = new byte[bytesRead];
					Array.Copy (ftpRequest.Buffer, lastBuffer, bytesRead);
					ftpRequest.FireBuffering (lastBuffer);
					Array.Clear (lastBuffer, 0, bytesRead);
				}

				ftpRequest.FileStream.BeginRead (ftpRequest.Buffer, 0, BufferSize, DownloadCallback, ftpRequest);
			} else {
				ftpRequest.FileStream.Close ();
				ftpRequest.Response.Close ();

				ftpRequest.FireCopyDone ();

				Trace.TraceInformation (ftpRequest.BytesRead + " bytes read on " + ftpRequest.FileSize + " bytes.");
			}
		}
        
        #endregion

		protected FtpWebRequest CreateRequest (Uri uri, string methodRequest, Credentials credentials)
		{
			var request = (FtpWebRequest)WebRequest.Create (uri);
			request.ConnectionGroupName = Guid.NewGuid ().ToString ();
			request.Method = methodRequest;
			if (credentials != null)
				request.Credentials = new NetworkCredential (credentials.Username, credentials.Password);

			request.UseBinary = UseBinary;
			request.EnableSsl = EnableSsl;
			request.KeepAlive = true;
			request.UsePassive = UsePassive;

			return request;
		}

	}
}
