using System.IO;

namespace CnDCopy.Kernel.LocationManagers.Ftp
{
	public class FtpPushRequest : PushRequest
	{
		public Stream OutputStream { get; set; }

		public override void BufferWriteCallback (byte[] buffer)
		{
			OutputStream.Write (buffer, 0, buffer.Length);
		}

		protected override void OnDispose ()
		{
			if (OutputStream != null) {
				OutputStream.Close ();
				OutputStream.Dispose ();
			}
		}
	}
}