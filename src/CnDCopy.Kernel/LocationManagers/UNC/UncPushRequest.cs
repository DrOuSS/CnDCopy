using System.IO;

namespace CnDCopy.Kernel.LocationManagers.UNC
{
	public class UncPushRequest : PushRequest
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

			base.OnDispose ();
		}
	}
}
