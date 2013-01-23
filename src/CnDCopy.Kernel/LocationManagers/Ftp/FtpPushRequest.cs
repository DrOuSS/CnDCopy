using System.Diagnostics;
using System.IO;
using System.Net;

namespace CnDCopy.Kernel.LocationManagers.Ftp
{
	public class FtpPushRequest : PushRequest
	{
		public FtpWebRequest Request { get; set; }
		public Stream OutputStream { get; set; }

		public override void BufferWriteCallback (byte[] buffer)
		{
			OutputStream.Write (buffer, 0, buffer.Length);
		}

		protected override void OnDispose ()
		{
			if (Request != null) {
				var response = (FtpWebResponse)Request.GetResponse ();
				Trace.TraceInformation ("FTP response status is " + response.StatusDescription);
			}

			if (OutputStream != null) {
				OutputStream.Close ();
				OutputStream.Dispose ();
			}
		}
	}
}